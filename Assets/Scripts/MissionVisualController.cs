using UnityEngine;

public class MissionVisualController : MonoBehaviour
{
    public MissionManager missionManager;
    public SatelliteOrbit satelliteOrbit;
    public LineRenderer scanLine;
    public Renderer targetHaloRenderer;
    public Renderer stationRangeRenderer;
    public Material idleTargetMaterial;
    public Material activeTargetMaterial;
    public Material stationIdleMaterial;
    public Material stationActiveMaterial;

    private void LateUpdate()
    {
        if (missionManager == null || satelliteOrbit == null)
        {
            return;
        }

        bool scanVisible = missionManager.Phase == MissionPhase.Observing
            || missionManager.Phase == MissionPhase.ObservationComplete
            || missionManager.Phase == MissionPhase.Transmitting;

        if (scanLine != null)
        {
            scanLine.enabled = scanVisible && missionManager.SelectedTarget != null;
            if (scanLine.enabled)
            {
                scanLine.SetPosition(0, satelliteOrbit.transform.position);
                scanLine.SetPosition(1, missionManager.SelectedTarget.transform.position);
            }
        }

        if (targetHaloRenderer != null)
        {
            targetHaloRenderer.enabled = missionManager.SelectedTarget != null;
            targetHaloRenderer.sharedMaterial = scanVisible ? activeTargetMaterial : idleTargetMaterial;
            if (missionManager.SelectedTarget != null)
            {
                targetHaloRenderer.transform.position = missionManager.SelectedTarget.transform.position * 1.01f;
                targetHaloRenderer.transform.up = missionManager.SelectedTarget.transform.position.normalized;
            }
        }

        if (stationRangeRenderer != null)
        {
            stationRangeRenderer.sharedMaterial = missionManager.CommunicationAvailable ? stationActiveMaterial : stationIdleMaterial;
        }
    }
}
