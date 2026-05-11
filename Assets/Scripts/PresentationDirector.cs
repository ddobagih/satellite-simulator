using UnityEngine;

public class PresentationDirector : MonoBehaviour
{
    public MissionManager missionManager;
    public CameraManager cameraManager;
    public bool autoCamera = true;

    private MissionPhase lastPhase = MissionPhase.Idle;

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
                cameraManager.SetSatellite();
                break;
            case MissionPhase.Complete:
                cameraManager.SetOverview();
                break;
        }
    }
}
