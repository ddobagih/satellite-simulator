using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Camera targetCamera;
    public Transform earth;
    public Transform satellite;
    public Transform missionTarget;
    public Transform groundStation;

    private string mode = "Overview";
    private Vector3 desiredPosition;
    private Vector3 desiredLookAt;
    private bool useSmoothMove;

    public string CurrentMode
    {
        get { return mode; }
    }

    private void Start()
    {
        if (targetCamera != null)
        {
            desiredPosition = targetCamera.transform.position;
            desiredLookAt = earth != null ? earth.position : Vector3.zero;
        }
    }

    private void LateUpdate()
    {
        if (targetCamera == null)
        {
            return;
        }

        if (mode == "Satellite" && satellite != null)
        {
            desiredPosition = satellite.position + new Vector3(-3.5f, 2.4f, -3.5f);
            desiredLookAt = satellite.position;
            useSmoothMove = true;
        }
        else if (mode == "Mission" && missionTarget != null)
        {
            Vector3 station = groundStation != null ? groundStation.position : Vector3.zero;
            Vector3 center = (missionTarget.position + station) * 0.5f;
            desiredPosition = center + new Vector3(4.8f, 3.2f, -5.8f);
            desiredLookAt = center;
            useSmoothMove = true;
        }

        if (useSmoothMove)
        {
            targetCamera.transform.position = Vector3.Lerp(targetCamera.transform.position, desiredPosition, Time.deltaTime * 4.5f);
            Quaternion desiredRotation = Quaternion.LookRotation(desiredLookAt - targetCamera.transform.position);
            targetCamera.transform.rotation = Quaternion.Slerp(targetCamera.transform.rotation, desiredRotation, Time.deltaTime * 5.5f);
        }
    }

    public void SetOverview()
    {
        mode = "Overview";
        MoveTo(new Vector3(0f, 6.2f, -12f), Vector3.zero, true);
    }

    public void SetEarth()
    {
        mode = "Earth";
        MoveTo(new Vector3(0f, 3.4f, -7.5f), Vector3.zero, true);
    }

    public void SetSatellite()
    {
        mode = "Satellite";
    }

    public void SetMission()
    {
        mode = "Mission";
    }

    private void MoveTo(Vector3 position, Vector3 lookAt, bool smooth)
    {
        if (targetCamera == null)
        {
            return;
        }

        desiredPosition = position;
        desiredLookAt = lookAt;
        useSmoothMove = smooth;
        if (!smooth)
        {
            targetCamera.transform.position = position;
            targetCamera.transform.LookAt(lookAt);
        }
    }
}
