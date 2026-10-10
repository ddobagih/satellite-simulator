using UnityEngine;
using UnityEngine.EventSystems;

public class DestinationClickTarget : MonoBehaviour
{
    public ObservationTarget target;
    public MissionManager missionManager;
    public CameraManager cameraManager;
    public float hitRadius = 1.35f;
    public bool respectCameraInputProtection = true;

    private const float MaxClickMovePixels = 10f;
    private bool pressed;
    private Vector3 pressPosition;

    private void Awake()
    {
        ExpandHitArea();
    }

    private void OnMouseDown()
    {
        if (!CanSelect())
        {
            return;
        }

        pressed = true;
        pressPosition = Input.mousePosition;
    }

    private void OnMouseUpAsButton()
    {
        if (!pressed)
        {
            return;
        }
        pressed = false;

        Vector3 clickDelta = Input.mousePosition - pressPosition;
        if (clickDelta.sqrMagnitude > MaxClickMovePixels * MaxClickMovePixels || !CanSelect())
        {
            return;
        }

        SelectTarget();
    }

    private void OnDisable()
    {
        pressed = false;
    }

    private bool CanSelect()
    {
        if (target == null || missionManager == null || missionManager.Phase != MissionPhase.Idle)
        {
            return false;
        }

        if (respectCameraInputProtection && cameraManager != null)
        {
            return !cameraManager.IsPointerInputBlocked();
        }

        return EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject();
    }

    private void SelectTarget()
    {
        missionManager.SelectTarget(target.targetName);
        if (cameraManager != null)
        {
            cameraManager.missionTarget = target.transform;
            cameraManager.SetMission();
        }
    }

    private void ExpandHitArea()
    {
        SphereCollider sphere = GetComponent<SphereCollider>();
        if (sphere == null && GetComponent<Collider>() == null)
        {
            sphere = gameObject.AddComponent<SphereCollider>();
        }

        if (sphere != null)
        {
            sphere.radius = Mathf.Max(sphere.radius, hitRadius);
        }
    }
}
