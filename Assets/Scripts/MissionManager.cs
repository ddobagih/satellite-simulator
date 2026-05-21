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
    public int RequestedSendDelayMinutes { get; private set; }
    public int MessageSizeKb { get; private set; }
    public int EstimatedPriceCredits { get; private set; }
    public int SuccessProbabilityPercent { get; private set; }

    private const float RequiredRelayLockSeconds = 1.2f;
    private const float QueuedMessageBufferPercent = 42f;
    private bool demoAssist;

    private void Start()
    {
        RequestedSendDelayMinutes = 0;
        MessageSizeKb = 128;
        if (Targets.Count > 0)
        {
            SelectTarget(Targets[0].targetName);
        }
        ResultMessage = "목적지와 메시지 조건을 고른 뒤 발신 가능한 위성을 찾으세요.";
        CompletionSummary = "전송 결과는 수신 완료 후 표시됩니다.";
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
            ResultMessage = "수신 지역 선택: " + DestinationLabel(SelectedTarget.targetName);
        }

        UpdateMissionEstimate();
        NotifyChanged();
    }

    public void SetSendDelayMinutes(int minutes)
    {
        if (Phase != MissionPhase.Idle)
        {
            return;
        }

        RequestedSendDelayMinutes = Mathf.Max(0, minutes);
        ResultMessage = RequestedSendDelayMinutes == 0
            ? "발신 시각: 지금 가능한 위성 우선."
            : "발신 시각: " + RequestedSendDelayMinutes + "분 후 창 기준.";
        UpdateMissionEstimate();
        NotifyChanged();
    }

    public void SetMessageSizeKb(int sizeKb)
    {
        if (Phase != MissionPhase.Idle)
        {
            return;
        }

        MessageSizeKb = Mathf.Max(1, sizeKb);
        ResultMessage = "메시지 용량: " + MessageSizeKb + " KB.";
        UpdateMissionEstimate();
        NotifyChanged();
    }

    public void ReserveMission()
    {
        if (satelliteManager == null || SelectedTarget == null)
        {
            return;
        }

        if (satelliteManager.ReserveSelected(SelectedTarget.targetName, RequestedSendDelayMinutes))
        {
            Phase = MissionPhase.Reserved;
            ObservationProgress = 0f;
            TransferProgress = 0f;
            ResultMessage = "발신 예약: 서울 → " + DestinationLabel(SelectedTarget.targetName) + ".";
            CompletionSummary = "Seoul → " + SelectedTarget.targetName + "\n발신 대기";
            NotifyChanged();
        }
        else
        {
            ResultMessage = "조건에 맞는 공유 중계 위성을 선택해야 합니다.";
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
        ResultMessage = "단문 메시지 라우팅: " + DestinationLabel(SelectedTarget.targetName) + ".";
        CompletionSummary = "Seoul → " + SelectedTarget.targetName + "\n전송 경로 탐색";
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

        if (Phase == MissionPhase.Idle && satelliteManager != null && !satelliteManager.CanReserveSelected(SelectedTarget.targetName, RequestedSendDelayMinutes))
        {
            SatelliteData bestSatellite = satelliteManager.FindBestSatellite(SelectedTarget.targetName, RequestedSendDelayMinutes);
            if (bestSatellite != null)
            {
                satelliteManager.SelectSatellite(bestSatellite.satelliteName);
            }
        }

        if (Phase == MissionPhase.Idle)
        {
            ReserveMission();
        }

        demoAssist = true;

        if (ObservationProgress >= 1f && Phase != MissionPhase.Complete && groundStation != null)
        {
            AlignSatelliteTo(groundStation.transform);
            ResultMessage = "Quick: syncing Seoul gateway.";
            NotifyChanged();
            return;
        }

        if (Phase == MissionPhase.Reserved)
        {
            StartMission();
            AlignSatelliteTo(SelectedTarget.transform);
            ResultMessage = "빠른 발신: 수신 지역 가시권 정렬.";
            NotifyChanged();
            return;
        }

        if (Phase == MissionPhase.Running || Phase == MissionPhase.Observing)
        {
            AlignSatelliteTo(SelectedTarget.transform);
            ResultMessage = "빠른 발신: 위성 경로 잠금 중.";
            NotifyChanged();
            return;
        }

        if ((Phase == MissionPhase.ObservationComplete || Phase == MissionPhase.Transmitting) && groundStation != null)
        {
            AlignSatelliteTo(groundStation.transform);
            ResultMessage = "빠른 발신: 서울 게이트웨이 동기화.";
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
        ResultMessage = "새 단문 발신 요청을 준비하세요.";
        CompletionSummary = "전송 결과는 수신 완료 후 표시됩니다.";

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
            ObservationProgress = Mathf.Clamp01(ObservationProgress + delta / RequiredRelayLockSeconds);
            ResultMessage = "중계 경로 확인: 서울 → 위성 → " + DestinationLabel(SelectedTarget.targetName);

            if (ObservationProgress >= 1f)
            {
                Phase = MissionPhase.ObservationComplete;
                ResultMessage = "경로 잠금 완료. 메시지를 위성 버퍼에 적재했습니다.";
                CompletionSummary = "Seoul → " + SelectedTarget.targetName + "\n경로 잠금\nMessage: " + MessageSizeKb + " KB";
                if (satelliteManager != null)
                {
                    satelliteManager.UpdateSharedMission(SelectedTarget.targetName, QueuedMessageBufferPercent);
                }
            }

            NotifyChanged();
        }
        else if (Phase == MissionPhase.Observing)
        {
            Phase = MissionPhase.Running;
            ResultMessage = "다음 가시권을 기다리는 중.";
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
        ResultMessage = "단문 전송 중: 서울 → " + (SelectedTarget != null ? DestinationLabel(SelectedTarget.targetName) : "수신 지역") + ".";

        if (TransferProgress >= 1f)
        {
            Phase = MissionPhase.Complete;
            demoAssist = false;
            ResultMessage = "Relay complete. Delivered. 단문 수신 완료.";
            CompletionSummary = "Relay Complete\nSeoul → " + (SelectedTarget != null ? SelectedTarget.targetName : "None") + " | Delivered";
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
            EstimatedPriceCredits = 0;
            SuccessProbabilityPercent = 0;
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
        float storageScore = 1f - Mathf.Clamp01(MessageSizeKb / 1024f);
        MissionQualityScore = Mathf.Clamp(Mathf.RoundToInt(62f + angleScore * 26f + storageScore * 12f - SelectedTarget.weatherRiskPercent * 0.25f), 0, 100);
        ServiceReliabilityScore = Mathf.Clamp(Mathf.RoundToInt(98f - SelectedTarget.weatherRiskPercent * 0.6f - Mathf.Max(0f, EstimatedPassSeconds - 10f) * 0.3f), 0, 100);
        SatelliteData selectedSatellite = satelliteManager != null ? satelliteManager.SelectedSatellite : null;
        if (selectedSatellite != null)
        {
            EstimatedPriceCredits = selectedSatellite.EstimatePriceCredits(MessageSizeKb);
            SuccessProbabilityPercent = Mathf.Clamp(
                Mathf.RoundToInt((ServiceReliabilityScore + selectedSatellite.reliabilityPercent) * 0.5f - SelectedTarget.weatherRiskPercent * 0.2f),
                0,
                100);
        }
    }

    private string DestinationLabel(string targetName)
    {
        if (targetName == "Seoul")
        {
            return "서울";
        }
        if (targetName == "New York")
        {
            return "뉴욕";
        }
        if (targetName == "London")
        {
            return "런던";
        }
        return targetName;
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
