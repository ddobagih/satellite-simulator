using UnityEngine;
using UnityEngine.EventSystems;

public class SatelliteClickTarget : MonoBehaviour
{
    public string satelliteName;
    public SatelliteManager satelliteManager;
    public CameraManager cameraManager;
    public float hitRadius = 0.72f;
    public bool respectCameraInputProtection = true;

    private const float MaxClickMovePixels = 10f;
    private bool pressed;
    private Vector3 pressPosition;

    public void Initialize(string name, SatelliteManager manager, CameraManager cameras)
    {
        satelliteName = name;
        satelliteManager = manager;
        cameraManager = cameras;
        ExpandHitArea();
    }

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

        SelectSatellite();
    }

    private void OnDisable()
    {
        pressed = false;
    }

    private bool CanSelect()
    {
        if (string.IsNullOrEmpty(satelliteName) || satelliteManager == null)
        {
            return false;
        }

        if (respectCameraInputProtection && cameraManager != null)
        {
            return !cameraManager.IsPointerInputBlocked();
        }

        return EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject();
    }

    private void SelectSatellite()
    {
        satelliteManager.SelectSatellite(satelliteName);
        if (cameraManager != null)
        {
            cameraManager.satellite = transform;
            cameraManager.SetSatellite();
        }
    }

    private void ExpandHitArea()
    {
        SphereCollider sphere = GetComponent<SphereCollider>();
        if (sphere == null && GetComponent<Collider>() == null)
        {
            sphere = gameObject.AddComponent<SphereCollider>();
        }

        if (sphere == null)
        {
            return;
        }

        Vector3 localCenter;
        float localRadius;
        CalculateRendererBounds(out localCenter, out localRadius);
        sphere.center = localCenter;
        sphere.radius = Mathf.Max(sphere.radius, Mathf.Max(hitRadius, localRadius));
    }

    private void CalculateRendererBounds(out Vector3 localCenter, out float localRadius)
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            localCenter = Vector3.zero;
            localRadius = hitRadius;
            return;
        }

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        localCenter = transform.InverseTransformPoint(bounds.center);
        float maxLossyScale = Mathf.Max(transform.lossyScale.x, Mathf.Max(transform.lossyScale.y, transform.lossyScale.z));
        localRadius = maxLossyScale > 0.001f ? bounds.extents.magnitude / maxLossyScale : hitRadius;
    }
}
