using UnityEngine;

public class PresentationDirector : MonoBehaviour
{
    public MissionManager missionManager;
    public CameraManager cameraManager;
    public bool autoCamera = true;

    private MissionPhase lastPhase = MissionPhase.Idle;
    private int handledPhaseMask;

    public bool HasHandledPhase(MissionPhase phase)
    {
        return (handledPhaseMask & PhaseBit(phase)) != 0;
    }

    private void OnEnable()
    {
        if (missionManager != null)
        {
            missionManager.OnMissionChanged += HandleMissionChanged;
        }
    }

    private void OnDisable()
    {
        if (missionManager != null)
        {
            missionManager.OnMissionChanged -= HandleMissionChanged;
        }
    }

    public void Attach(MissionManager mission, CameraManager cameras)
    {
        if (missionManager != null)
        {
            missionManager.OnMissionChanged -= HandleMissionChanged;
        }

        missionManager = mission;
        cameraManager = cameras;
        lastPhase = mission != null ? mission.Phase : MissionPhase.Idle;
        handledPhaseMask = 0;

        if (missionManager != null)
        {
            missionManager.OnMissionChanged += HandleMissionChanged;
        }
    }

    private void HandleMissionChanged()
    {
        if (!autoCamera || missionManager == null || cameraManager == null || missionManager.Phase == lastPhase)
        {
            return;
        }

        lastPhase = missionManager.Phase;
        bool cutIssued = true;
        switch (lastPhase)
        {
            case MissionPhase.Reserved:
                cameraManager.SetOverview();
                break;
            case MissionPhase.Running:
                cameraManager.SetSatellite();
                break;
            case MissionPhase.Observing:
            case MissionPhase.ObservationComplete:
                cameraManager.SetMission();
                break;
            case MissionPhase.Transmitting:
                cameraManager.SetOverview();
                break;
            case MissionPhase.Complete:
                cameraManager.SetOverview();
                break;
            default:
                cutIssued = false;
                break;
        }

        if (cutIssued)
        {
            handledPhaseMask |= PhaseBit(lastPhase);
        }
    }

    private static int PhaseBit(MissionPhase phase)
    {
        return 1 << (int)phase;
    }
}
