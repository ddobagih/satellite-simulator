using System;
using System.Collections.Generic;
using UnityEngine;

public enum MissionPhase
{
    Idle,
    Reserved,
    Running,
    Observing,
    ObservationComplete,
    Transmitting,
    Complete
}

public class MissionManager : MonoBehaviour
{
    public event Action OnMissionChanged;

    public SatelliteManager satelliteManager;
    public SimulationManager simulationManager;
    public Transform earth;
    public SatelliteOrbit satelliteOrbit;
    public GroundStation groundStation;
    public ConnectionLine connectionLine;

    public readonly List<ObservationTarget> Targets = new List<ObservationTarget>();

    public ObservationTarget SelectedTarget { get; private set; }
    public MissionPhase Phase { get; private set; }
    public float ObservationProgress { get; private set; }
    public float TransferProgress { get; private set; }
    public bool CommunicationAvailable { get; private set; }
    public string ResultMessage { get; private set; }
    public string CompletionSummary { get; private set; }
    public float EstimatedPassSeconds { get; private set; }
    public float EstimatedDownlinkSeconds { get; private set; }
    public int MissionQualityScore { get; private set; }
    public int ServiceReliabilityScore { get; private set; }

    private const float RequiredObservationSeconds = 1.2f;
    private const float StorageAfterObservation = 42f;
    private bool demoAssist;

    private void Start()
    {
        if (Targets.Count > 0)
        {
            SelectTarget(Targets[0].targetName);
        }
        ResultMessage = "Select the shared satellite and reserve an observation target.";
        CompletionSummary = "Mission result summary appears after completion.";
    }

    private void Update()
    {
        if (satelliteOrbit == null || earth == null)
        {
            return;
        }

        UpdateCommunicationAvailability();
        UpdateMissionEstimate();

        if (Phase == MissionPhase.Running || Phase == MissionPhase.Observing)
        {
            UpdateObservation();
        }
        else if (Phase == MissionPhase.ObservationComplete || Phase == MissionPhase.Transmitting)
        {
            UpdateTransmission();
        }
    }

    public void SelectTarget(string targetName)
    {
        ObservationTarget selected = Targets.Find(target => target.targetName == targetName);
        if (selected == null)
        {
            return;
        }

        SelectedTarget = selected;
        foreach (ObservationTarget target in Targets)
        {
            target.SetSelected(target == SelectedTarget);
        }

        if (Phase == MissionPhase.Idle)
        {
            ResultMessage = "Target selected: " + SelectedTarget.targetName;
        }

        NotifyChanged();
    }

    public void ReserveMission()
    {
        if (satelliteManager == null || SelectedTarget == null)
        {
            return;
        }

        if (satelliteManager.ReserveSelected(SelectedTarget.targetName))
        {
            Phase = MissionPhase.Reserved;
            ObservationProgress = 0f;
            TransferProgress = 0f;
            ResultMessage = "Reserved observation target: " + SelectedTarget.targetName;
            CompletionSummary = "Satellite: SAT-S01\nTarget: " + SelectedTarget.targetName + "\nState: Waiting for mission start";
            NotifyChanged();
        }
        else
        {
            ResultMessage = "SAT-S01 must be rentable before reservation.";
            NotifyChanged();
        }
    }

    public void StartMission()
    {
        if (Phase != MissionPhase.Reserved || satelliteManager == null)
        {
            return;
        }

        Phase = MissionPhase.Running;
        satelliteManager.SetSharedState(SatelliteAvailabilityState.InMission);
        ResultMessage = "Mission running: waiting for target pass.";
        CompletionSummary = "Satellite: SAT-S01\nTarget: " + SelectedTarget.targetName + "\nState: Orbiting toward target";
        NotifyChanged();
    }

    public void RunFastDemo()
    {
        if (SelectedTarget == null || satelliteOrbit == null)
        {
            return;
        }

        if (simulationManager != null)
        {
            simulationManager.SetTimeScale(10f);
        }

        if (Phase == MissionPhase.Idle)
        {
            ReserveMission();
        }

        demoAssist = true;

        if (ObservationProgress >= 1f && Phase != MissionPhase.Complete && groundStation != null)
        {
            AlignSatelliteTo(groundStation.transform);
            ResultMessage = "Fast demo: entering ground station range.";
            NotifyChanged();
            return;
        }

        if (Phase == MissionPhase.Reserved)
        {
            StartMission();
            AlignSatelliteTo(SelectedTarget.transform);
            ResultMessage = "Fast demo: moved near observation window.";
            NotifyChanged();
            return;
        }

        if (Phase == MissionPhase.Running || Phase == MissionPhase.Observing)
        {
            AlignSatelliteTo(SelectedTarget.transform);
            ResultMessage = "Fast demo: entering observation window.";
            NotifyChanged();
            return;
        }

        if ((Phase == MissionPhase.ObservationComplete || Phase == MissionPhase.Transmitting) && groundStation != null)
        {
            AlignSatelliteTo(groundStation.transform);
            ResultMessage = "Fast demo: entering ground station range.";
            NotifyChanged();
        }
    }

    public void ResetMission()
    {
        Phase = MissionPhase.Idle;
        demoAssist = false;
        ObservationProgress = 0f;
        TransferProgress = 0f;
        CommunicationAvailable = false;
        ResultMessage = "Ready for a new mission.";
        CompletionSummary = "Mission result summary appears after completion.";

        if (satelliteManager != null)
        {
            satelliteManager.ResetSharedSatellite();
        }

        if (connectionLine != null)
        {
            connectionLine.SetVisible(false);
        }

        NotifyChanged();
    }

    private void UpdateObservation()
    {
        if (SelectedTarget == null)
        {
            return;
        }

        float angle = SurfaceAngleTo(SelectedTarget.transform);
        if (angle <= SelectedTarget.observationAngle)
        {
            Phase = MissionPhase.Observing;
            float delta = simulationManager != null ? simulationManager.ScaledDeltaTime : Time.deltaTime;
            ObservationProgress = Mathf.Clamp01(ObservationProgress + delta / RequiredObservationSeconds);
            ResultMessage = "Observing: " + SelectedTarget.targetName;

            if (ObservationProgress >= 1f)
            {
                Phase = MissionPhase.ObservationComplete;
                ResultMessage = "Observation complete: data stored.";
                CompletionSummary = "Satellite: SAT-S01\nTarget: " + SelectedTarget.targetName + "\nState: Observation data stored\nData: " + SelectedTarget.estimatedDataMb.ToString("0") + " MB\nTransfer: Waiting for ground station";
                if (satelliteManager != null)
                {
                    satelliteManager.UpdateSharedMission(SelectedTarget.targetName, StorageAfterObservation);
                }
            }

            NotifyChanged();
        }
        else if (Phase == MissionPhase.Observing)
        {
            Phase = MissionPhase.Running;
            ResultMessage = "Waiting for next target pass.";
            NotifyChanged();
        }
    }

    private void UpdateTransmission()
    {
        bool canTransmit = CommunicationAvailable && ObservationProgress >= 1f;
        if (!canTransmit && demoAssist && groundStation != null && ObservationProgress >= 1f)
        {
            AlignSatelliteTo(groundStation.transform);
            canTransmit = CommunicationAvailable;
        }
        if (connectionLine != null)
        {
            connectionLine.SetVisible(canTransmit);
        }

        if (!canTransmit)
        {
            if (Phase != MissionPhase.ObservationComplete)
            {
                Phase = MissionPhase.ObservationComplete;
                NotifyChanged();
            }
            return;
        }

        Phase = MissionPhase.Transmitting;
        float delta = simulationManager != null ? simulationManager.ScaledDeltaTime : Time.deltaTime;
        TransferProgress = Mathf.Clamp01(TransferProgress + delta * 0.22f);
        ResultMessage = "Transmitting to ground station.";

        if (TransferProgress >= 1f)
        {
            Phase = MissionPhase.Complete;
            demoAssist = false;
            ResultMessage = "Mission complete: data delivered.";
            CompletionSummary = "Mission Complete\nSatellite: SAT-S01\nTarget: " + (SelectedTarget != null ? SelectedTarget.targetName : "None") + "\nResult: Observation data delivered\nQuality: " + MissionQualityScore + "/100\nStatus: Success";
            if (satelliteManager != null)
            {
                satelliteManager.SetSharedState(SatelliteAvailabilityState.MissionComplete);
                satelliteManager.UpdateSharedMission(SelectedTarget != null ? SelectedTarget.targetName : "None", 0f);
            }
            if (connectionLine != null)
            {
                connectionLine.SetVisible(false);
            }
        }

        NotifyChanged();
    }

    private void UpdateCommunicationAvailability()
    {
        if (groundStation == null)
        {
            CommunicationAvailable = false;
            return;
        }

        CommunicationAvailable = SurfaceAngleTo(groundStation.transform) <= groundStation.communicationAngle;
    }

    private void UpdateMissionEstimate()
    {
        if (SelectedTarget == null || satelliteOrbit == null)
        {
            EstimatedPassSeconds = 0f;
            EstimatedDownlinkSeconds = 0f;
            MissionQualityScore = 0;
            ServiceReliabilityScore = 0;
            return;
        }

        float timeScale = simulationManager != null ? Mathf.Max(1f, simulationManager.TimeScale) : 1f;
        float effectiveDegreesPerSecond = Mathf.Max(1f, satelliteOrbit.orbitSpeed * timeScale);
        float targetAngle = SurfaceAngleTo(SelectedTarget.transform);
        float targetGap = Mathf.Max(0f, targetAngle - SelectedTarget.observationAngle);
        EstimatedPassSeconds = targetGap / effectiveDegreesPerSecond;

        if (groundStation != null)
        {
            float stationGap = Mathf.Max(0f, SurfaceAngleTo(groundStation.transform) - groundStation.communicationAngle);
            EstimatedDownlinkSeconds = stationGap / effectiveDegreesPerSecond;
        }

        float angleScore = Mathf.Clamp01(1f - targetAngle / 140f);
        float storageScore = 1f - Mathf.Clamp01(SelectedTarget.estimatedDataMb / 1800f);
        MissionQualityScore = Mathf.Clamp(Mathf.RoundToInt(62f + angleScore * 26f + storageScore * 12f - SelectedTarget.weatherRiskPercent * 0.25f), 0, 100);
        ServiceReliabilityScore = Mathf.Clamp(Mathf.RoundToInt(98f - SelectedTarget.weatherRiskPercent * 0.6f - Mathf.Max(0f, EstimatedPassSeconds - 10f) * 0.3f), 0, 100);
    }

    private void AlignSatelliteTo(Transform target)
    {
        if (target == null || satelliteOrbit == null || earth == null)
        {
            return;
        }

        float bestAngle = 0f;
        float bestSurfaceAngle = float.MaxValue;
        Vector3 center = earth.position;
        Vector3 targetDirection = (target.position - center).normalized;

        for (int i = 0; i < 360; i++)
        {
            Vector3 satelliteDirection = (satelliteOrbit.GetOrbitPosition(i) - center).normalized;
            float surfaceAngle = Vector3.Angle(satelliteDirection, targetDirection);
            if (surfaceAngle < bestSurfaceAngle)
            {
                bestSurfaceAngle = surfaceAngle;
                bestAngle = i;
            }
        }

        satelliteOrbit.SetAngle(bestAngle);
        UpdateCommunicationAvailability();
    }

    private float SurfaceAngleTo(Transform point)
    {
        Vector3 center = earth != null ? earth.position : Vector3.zero;
        Vector3 satelliteDirection = (satelliteOrbit.transform.position - center).normalized;
        Vector3 pointDirection = (point.position - center).normalized;
        return Vector3.Angle(satelliteDirection, pointDirection);
    }

    private void NotifyChanged()
    {
        if (OnMissionChanged != null)
        {
            OnMissionChanged.Invoke();
        }
    }
}
