using System.Collections.Generic;
using UnityEngine;

public class SatelliteFocusController : MonoBehaviour
{
    public SatelliteManager satelliteManager;
    public CameraManager cameraManager;
    public MissionVisualController missionVisualController;

    private readonly Dictionary<string, Transform> satelliteTransforms = new Dictionary<string, Transform>();
    private readonly Dictionary<string, GameObject> focusIndicators = new Dictionary<string, GameObject>();
    private readonly Dictionary<string, Renderer> statusIndicators = new Dictionary<string, Renderer>();
    private readonly Dictionary<string, LineRenderer> congestionHalos = new Dictionary<string, LineRenderer>();
    private Material focusMaterial;
    private Material statusAvailableMaterial;
    private Material statusReservedMaterial;
    private Material statusBusyMaterial;
    private Material statusCongestedMaterial;
    private const float RealSatelliteClickRadius = 0.32f;
    private const float FocusScaleNearDistance = 4.8f;
    private const float FocusScaleFarDistance = 10.5f;
    private const float FocusScaleNear = 0.28f;
    private const float FocusScaleFar = 0.34f;

    public void Initialize(SatelliteManager manager, CameraManager cameras, Material haloMaterial, MissionVisualController visuals = null)
    {
        if (satelliteManager != null)
        {
            satelliteManager.OnSelectionChanged -= HandleSelectionChanged;
        }

        satelliteManager = manager;
        cameraManager = cameras;
        missionVisualController = visuals;
        focusMaterial = haloMaterial;
        CreateStatusMaterials();
        if (satelliteManager != null)
        {
            satelliteManager.OnSelectionChanged += HandleSelectionChanged;
        }
    }

    private void OnDestroy()
    {
        if (satelliteManager != null)
        {
            satelliteManager.OnSelectionChanged -= HandleSelectionChanged;
        }
    }

    public void RegisterSatellite(string satelliteName, Transform satelliteTransform)
    {
        if (string.IsNullOrEmpty(satelliteName) || satelliteTransform == null)
        {
            return;
        }

        Transform existingTransform;
        bool sameRegistration = satelliteTransforms.TryGetValue(satelliteName, out existingTransform)
            && existingTransform == satelliteTransform
            && HasFocusVisuals(satelliteName);

        satelliteTransforms[satelliteName] = satelliteTransform;
        ConfigureSatelliteClickTarget(satelliteName, satelliteTransform);

        if (sameRegistration)
        {
            UpdateFocusIndicatorActive(satelliteName);
            return;
        }

        RemoveSatelliteVisuals(satelliteName);

        GameObject indicator = CreateFocusIndicator(satelliteName, satelliteTransform);
        indicator.SetActive(satelliteManager != null
            && satelliteManager.SelectedSatellite != null
            && satelliteManager.SelectedSatellite.satelliteName == satelliteName);
        focusIndicators[satelliteName] = indicator;

        statusIndicators[satelliteName] = CreateStatusIndicator(satelliteName, satelliteTransform);
        congestionHalos[satelliteName] = CreateCongestionHalo(satelliteName, satelliteTransform);
    }

    private bool HasFocusVisuals(string satelliteName)
    {
        GameObject focusIndicator;
        Renderer statusIndicator;
        LineRenderer congestionHalo;
        return focusIndicators.TryGetValue(satelliteName, out focusIndicator)
            && focusIndicator != null
            && statusIndicators.TryGetValue(satelliteName, out statusIndicator)
            && statusIndicator != null
            && congestionHalos.TryGetValue(satelliteName, out congestionHalo)
            && congestionHalo != null;
    }

    private void UpdateFocusIndicatorActive(string satelliteName)
    {
        GameObject indicator;
        if (!focusIndicators.TryGetValue(satelliteName, out indicator) || indicator == null)
        {
            return;
        }

        indicator.SetActive(satelliteManager != null
            && satelliteManager.SelectedSatellite != null
            && satelliteManager.SelectedSatellite.satelliteName == satelliteName);
    }

    private void RemoveSatelliteVisuals(string satelliteName)
    {
        GameObject focusIndicator;
        if (focusIndicators.TryGetValue(satelliteName, out focusIndicator) && focusIndicator != null)
        {
            Destroy(focusIndicator);
        }
        focusIndicators.Remove(satelliteName);

        Renderer statusIndicator;
        if (statusIndicators.TryGetValue(satelliteName, out statusIndicator) && statusIndicator != null)
        {
            Destroy(statusIndicator.gameObject);
        }
        statusIndicators.Remove(satelliteName);

        LineRenderer congestionHalo;
        if (congestionHalos.TryGetValue(satelliteName, out congestionHalo) && congestionHalo != null)
        {
            Destroy(congestionHalo.gameObject);
        }
        congestionHalos.Remove(satelliteName);
    }

    private void LateUpdate()
    {
        foreach (KeyValuePair<string, GameObject> pair in focusIndicators)
        {
            GameObject indicator = pair.Value;
            if (indicator == null || !indicator.activeSelf)
            {
                continue;
            }

            Camera camera = Camera.main;
            float pulse = 1f + Mathf.Sin(Time.time * 4.2f) * 0.018f;
            indicator.transform.localScale = Vector3.one * pulse * FocusDistanceScale(indicator.transform.position, camera);
            if (camera != null)
            {
                indicator.transform.rotation = camera.transform.rotation;
            }
        }

        UpdateStatusIndicators();
    }

    private void HandleSelectionChanged(SatelliteData selected)
    {
        foreach (KeyValuePair<string, GameObject> pair in focusIndicators)
        {
            if (pair.Value != null)
            {
                pair.Value.SetActive(selected != null && pair.Key == selected.satelliteName);
            }
        }

        if (selected != null && cameraManager != null && satelliteTransforms.ContainsKey(selected.satelliteName))
        {
            cameraManager.satellite = satelliteTransforms[selected.satelliteName];
        }
        if (selected != null && missionVisualController != null && satelliteTransforms.ContainsKey(selected.satelliteName))
        {
            missionVisualController.SetSelectedSatelliteTransform(satelliteTransforms[selected.satelliteName]);
        }
    }

    private void ConfigureSatelliteClickTarget(string satelliteName, Transform satelliteTransform)
    {
        SatelliteClickTarget clickTarget = satelliteTransform.GetComponent<SatelliteClickTarget>();
        if (clickTarget == null)
        {
            clickTarget = satelliteTransform.gameObject.AddComponent<SatelliteClickTarget>();
        }
        if (satelliteTransform.GetComponent<RealSatelliteMarker>() != null)
        {
            clickTarget.hitRadius = RealSatelliteClickRadius;
        }
        clickTarget.Initialize(satelliteName, satelliteManager, cameraManager);
        if (satelliteTransform.GetComponent<RealSatelliteMarker>() != null)
        {
            ClampRealSatelliteClickRadius(satelliteTransform);
        }
    }

    private GameObject CreateFocusIndicator(string satelliteName, Transform satelliteTransform)
    {
        GameObject root = new GameObject(satelliteName + " Selection Brackets");
        root.transform.SetParent(satelliteTransform, false);
        root.transform.localPosition = Vector3.zero;

        CreateCorner(root.transform, "Top Left", -0.13f, 0.13f, 1, -1);
        CreateCorner(root.transform, "Top Right", 0.13f, 0.13f, -1, -1);
        CreateCorner(root.transform, "Bottom Left", -0.13f, -0.13f, 1, 1);
        CreateCorner(root.transform, "Bottom Right", 0.13f, -0.13f, -1, 1);
        CreateBeacon(root.transform);
        CreateSoftHalo(root.transform);
        return root;
    }

    private void CreateCorner(Transform parent, string name, float x, float y, int xDirection, int yDirection)
    {
        GameObject corner = new GameObject(name, typeof(LineRenderer));
        corner.transform.SetParent(parent, false);
        LineRenderer line = corner.GetComponent<LineRenderer>();
        ConfigureFocusLine(line);
        line.positionCount = 3;
        line.useWorldSpace = false;
        float arm = 0.034f;
        line.SetPosition(0, new Vector3(x + arm * xDirection, y, 0f));
        line.SetPosition(1, new Vector3(x, y, 0f));
        line.SetPosition(2, new Vector3(x, y + arm * yDirection, 0f));
    }

    private void CreateSoftHalo(Transform parent)
    {
        GameObject halo = new GameObject("Selected Soft Halo", typeof(LineRenderer));
        halo.transform.SetParent(parent, false);
        LineRenderer line = halo.GetComponent<LineRenderer>();
        ConfigureFocusLine(line);
        line.positionCount = 96;
        line.loop = true;
        line.useWorldSpace = false;
        line.widthMultiplier = 0.0025f;
        DrawHalo(line, 0.14f, new Color(0.52f, 1f, 0.96f, 0.055f));
    }

    private void CreateBeacon(Transform parent)
    {
        GameObject beacon = new GameObject("Selected Beacon", typeof(LineRenderer));
        beacon.transform.SetParent(parent, false);
        LineRenderer line = beacon.GetComponent<LineRenderer>();
        ConfigureFocusLine(line);
        line.positionCount = 2;
        line.useWorldSpace = false;
        line.widthMultiplier = 0.0035f;
        line.SetPosition(0, new Vector3(0f, 0.10f, 0f));
        line.SetPosition(1, new Vector3(0f, 0.15f, 0f));
    }

    private Renderer CreateStatusIndicator(string satelliteName, Transform satelliteTransform)
    {
        GameObject indicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        indicator.name = satelliteName + " Status Dot";
        indicator.transform.SetParent(satelliteTransform, false);
        indicator.transform.localPosition = new Vector3(0f, 0.15f, 0f);
        indicator.transform.localScale = Vector3.one * 0.014f;
        Collider collider = indicator.GetComponent<Collider>();
        if (collider != null)
        {
            Destroy(collider);
        }

        Renderer renderer = indicator.GetComponent<Renderer>();
        renderer.sharedMaterial = statusAvailableMaterial;
        return renderer;
    }

    private LineRenderer CreateCongestionHalo(string satelliteName, Transform satelliteTransform)
    {
        GameObject halo = new GameObject(satelliteName + " Traffic Halo");
        halo.transform.SetParent(satelliteTransform, false);
        halo.transform.localPosition = Vector3.zero;
        LineRenderer line = halo.AddComponent<LineRenderer>();
        line.material = focusMaterial;
        line.useWorldSpace = false;
        line.loop = true;
        line.positionCount = 72;
        line.numCapVertices = 6;
        line.numCornerVertices = 6;
        line.alignment = LineAlignment.View;
        line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        line.receiveShadows = false;
        line.enabled = false;
        DrawHalo(line, 0.18f, new Color(0.22f, 1f, 0.9f, 0.24f));
        return line;
    }

    private void UpdateStatusIndicators()
    {
        foreach (KeyValuePair<string, Renderer> pair in statusIndicators)
        {
            Renderer renderer = pair.Value;
            if (renderer == null)
            {
                continue;
            }

            SatelliteData data = FindSatellite(pair.Key);
            if (data == null)
            {
                renderer.enabled = false;
                LineRenderer halo;
                if (congestionHalos.TryGetValue(pair.Key, out halo) && halo != null)
                {
                    halo.enabled = false;
                }
                continue;
            }

            renderer.enabled = true;
            renderer.sharedMaterial = StatusMaterial(data);
            bool selected = satelliteManager != null
                && satelliteManager.SelectedSatellite != null
                && satelliteManager.SelectedSatellite.satelliteName == data.satelliteName;
            bool busy = data.availabilityState == SatelliteAvailabilityState.InMission
                || data.availabilityState == SatelliteAvailabilityState.Reserved;
            float congestionScale = selected
                ? Mathf.Lerp(0.038f, 0.064f, data.congestionPercent / 100f)
                : Mathf.Lerp(0.028f, busy ? 0.052f : 0.046f, data.congestionPercent / 100f);
            float pulse = selected || busy ? 1f + Mathf.Sin(Time.time * 3.4f) * 0.035f : 1f;
            renderer.transform.localScale = Vector3.one * congestionScale * pulse;
            UpdateCongestionHalo(pair.Key, data, selected, busy);
        }
    }

    private void UpdateCongestionHalo(string satelliteName, SatelliteData data, bool selected, bool busy)
    {
        LineRenderer halo;
        if (!congestionHalos.TryGetValue(satelliteName, out halo) || halo == null)
        {
            return;
        }

        bool congested = data.congestionPercent >= 70;
        bool visible = selected || busy || congested;
        halo.enabled = visible;
        if (!visible)
        {
            return;
        }

        Color color = congested
            ? new Color(1f, 0.28f, 0.18f, selected ? 0.72f : 0.56f)
            : busy
                ? new Color(0.86f, 0.92f, 1f, selected ? 0.64f : 0.42f)
                : new Color(0.42f, 1f, 0.96f, selected ? 0.68f : 0.30f);
        float congestion01 = Mathf.Clamp01(data.congestionPercent / 100f);
        float pulseSpeed = congested ? 4.0f : busy ? 3.2f : 2.4f;
        float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * (congested ? 0.055f : 0.032f);
        float radius = Mathf.Lerp(0.12f, 0.17f, congestion01) * pulse;
        DrawHalo(halo, radius, color);
        halo.widthMultiplier = selected ? 0.0045f : congested ? 0.0038f : 0.0028f;

        if (Camera.main != null)
        {
            halo.transform.rotation = Camera.main.transform.rotation;
        }
    }

    private void DrawHalo(LineRenderer line, float radius, Color color)
    {
        if (line == null)
        {
            return;
        }

        line.startColor = color;
        line.endColor = color;
        for (int i = 0; i < line.positionCount; i++)
        {
            float t = i / (float)line.positionCount * Mathf.PI * 2f;
            line.SetPosition(i, new Vector3(Mathf.Cos(t) * radius, Mathf.Sin(t) * radius, 0f));
        }
    }

    private SatelliteData FindSatellite(string satelliteName)
    {
        if (satelliteManager == null)
        {
            return null;
        }

        for (int i = 0; i < satelliteManager.Satellites.Count; i++)
        {
            SatelliteData satellite = satelliteManager.Satellites[i];
            if (satellite.satelliteName == satelliteName)
            {
                return satellite;
            }
        }
        return null;
    }

    private Material StatusMaterial(SatelliteData data)
    {
        if (data.congestionPercent >= 70)
        {
            return statusCongestedMaterial;
        }

        switch (data.availabilityState)
        {
            case SatelliteAvailabilityState.Available:
                return statusAvailableMaterial;
            case SatelliteAvailabilityState.Reserved:
            case SatelliteAvailabilityState.MissionComplete:
                return statusReservedMaterial;
            case SatelliteAvailabilityState.InMission:
                return statusBusyMaterial;
            default:
                return statusAvailableMaterial;
        }
    }

    private void CreateStatusMaterials()
    {
        if (statusAvailableMaterial != null)
        {
            return;
        }

        statusAvailableMaterial = CreateStatusMaterial("Satellite Status Available", new Color(0.42f, 1f, 0.82f, 1f), 1.35f);
        statusReservedMaterial = CreateStatusMaterial("Satellite Status Reserved", new Color(0.72f, 0.94f, 1f, 1f), 1.25f);
        statusBusyMaterial = CreateStatusMaterial("Satellite Status Busy", new Color(0.95f, 0.98f, 1f, 1f), 1.35f);
        statusCongestedMaterial = CreateStatusMaterial("Satellite Status Congested", new Color(1f, 0.34f, 0.2f, 1f), 1.55f);
    }

    private Material CreateStatusMaterial(string materialName, Color color, float emission)
    {
        Shader shader = Shader.Find("Standard");
        if (shader == null)
        {
            shader = Shader.Find("Unlit/Color");
        }
        if (shader == null)
        {
            shader = Shader.Find("Sprites/Default");
        }
        if (shader == null)
        {
            shader = Shader.Find("Hidden/InternalErrorShader");
        }

        Material material = new Material(shader);
        material.name = materialName;
        material.color = color;
        if (material.HasProperty("_Color"))
        {
            material.SetColor("_Color", color);
        }
        if (material.HasProperty("_EmissionColor"))
        {
            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", color * emission);
        }
        return material;
    }

    private void ConfigureFocusLine(LineRenderer line)
    {
        line.material = focusMaterial;
        line.widthMultiplier = 0.014f;
        line.numCapVertices = 4;
        line.numCornerVertices = 4;
        line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        line.receiveShadows = false;
    }

    private float FocusDistanceScale(Vector3 position, Camera camera)
    {
        if (camera == null)
        {
            return FocusScaleFar;
        }

        float distance = Vector3.Distance(camera.transform.position, position);
        return Mathf.Lerp(FocusScaleNear, FocusScaleFar, Mathf.InverseLerp(FocusScaleNearDistance, FocusScaleFarDistance, distance));
    }

    private void ClampRealSatelliteClickRadius(Transform satelliteTransform)
    {
        SphereCollider sphere = satelliteTransform.GetComponent<SphereCollider>();
        if (sphere != null)
        {
            sphere.radius = Mathf.Min(sphere.radius, RealSatelliteClickRadius);
        }
    }
}
