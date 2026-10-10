using UnityEngine;

public class RealSatelliteMarker : MonoBehaviour
{
    private const float LabelScale = 0.034f;

    private LineRenderer velocityLine;
    private TextMesh label;
    private BillboardLabel labelBillboard;
    private Transform visualRoot;
    private Renderer bodyRenderer;
    private Renderer leftPanelRenderer;
    private Renderer rightPanelRenderer;
    private Renderer antennaRenderer;
    private Renderer dishRenderer;

    public string SatelliteName { get; private set; }
    public GeodeticCoordinate LastGeodetic { get; private set; }
    public string LastStatusText { get; private set; }
    public bool IsLabelVisible { get { return label != null && label.gameObject.activeSelf; } }

    public void Initialize(string satelliteName, Material markerMaterial, Material velocityMaterial)
    {
        SetIdentity(satelliteName);

        visualRoot = new GameObject("Marker").transform;
        visualRoot.SetParent(transform, false);

        bodyRenderer = CreatePart("Body", PrimitiveType.Cube, Vector3.zero, new Vector3(0.24f, 0.17f, 0.30f), markerMaterial);
        leftPanelRenderer = CreatePart("Left Solar Panel", PrimitiveType.Cube, new Vector3(-0.36f, 0f, 0f), new Vector3(0.38f, 0.018f, 0.20f), markerMaterial);
        rightPanelRenderer = CreatePart("Right Solar Panel", PrimitiveType.Cube, new Vector3(0.36f, 0f, 0f), new Vector3(0.38f, 0.018f, 0.20f), markerMaterial);
        antennaRenderer = CreatePart("Compact Antenna", PrimitiveType.Cylinder, new Vector3(0f, 0.15f, 0f), new Vector3(0.018f, 0.13f, 0.018f), markerMaterial);
        antennaRenderer.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        dishRenderer = CreatePart("Forward Dish", PrimitiveType.Sphere, new Vector3(0f, 0f, 0.19f), Vector3.one * 0.085f, markerMaterial);
        dishRenderer.transform.localScale = new Vector3(0.09f, 0.09f, 0.035f);

        GameObject lineObject = new GameObject("VelocityDirection");
        lineObject.transform.SetParent(transform, false);
        velocityLine = lineObject.AddComponent<LineRenderer>();
        velocityLine.positionCount = 2;
        velocityLine.useWorldSpace = true;
        velocityLine.widthMultiplier = 0.0038f;
        velocityLine.enabled = false;
        if (velocityMaterial != null)
        {
            velocityLine.material = velocityMaterial;
        }

        GameObject labelObject = new GameObject("Label");
        labelObject.transform.SetParent(transform, false);
        label = labelObject.AddComponent<TextMesh>();
        label.anchor = TextAnchor.MiddleCenter;
        label.alignment = TextAlignment.Center;
        label.characterSize = LabelScale;
        label.fontSize = 20;
        label.color = new Color(0.72f, 1f, 0.92f, 0.82f);
        label.text = satelliteName;
        labelBillboard = labelObject.AddComponent<BillboardLabel>();
        labelBillboard.autoVisibility = false;
    }

    public void SetIdentity(string satelliteName)
    {
        SatelliteName = string.IsNullOrEmpty(satelliteName) ? "Real Satellite" : satelliteName;
        name = "RealSat-" + SatelliteName;
    }

    public void SetVisualStyle(Color markerColor, Color labelColor, float markerScale, bool showVelocityLine)
    {
        SetRendererColor(bodyRenderer, markerColor);
        SetRendererColor(antennaRenderer, markerColor * 0.92f);
        SetRendererColor(dishRenderer, Color.Lerp(markerColor, Color.white, 0.18f));
        Color panelColor = Color.Lerp(new Color(0.04f, 0.34f, 0.48f, 1f), markerColor, 0.18f);
        SetRendererColor(leftPanelRenderer, panelColor);
        SetRendererColor(rightPanelRenderer, panelColor);

        if (label != null)
        {
            label.color = labelColor;
        }
        if (visualRoot != null)
        {
            visualRoot.localScale = Vector3.one * Mathf.Max(0.08f, markerScale / 0.11f);
        }
        if (velocityLine != null && velocityLine.material != null)
        {
            Color velocityColor = markerColor;
            velocityColor.a = 0.82f;
            velocityLine.material.color = velocityColor;
            velocityLine.enabled = velocityLine.enabled && showVelocityLine;
        }
    }

    public void SetState(Vector3 position, Vector3 velocityUnityPerSecond, GeodeticCoordinate geodetic, float velocityPreviewSeconds, bool showLabel, string labelText, bool showVelocityLine, bool isSelected = false)
    {
        transform.position = position;
        if (position.sqrMagnitude > 1e-6f)
        {
            Vector3 up = position.normalized;
            if (velocityUnityPerSecond.sqrMagnitude > 1e-8f && Vector3.Cross(up, velocityUnityPerSecond.normalized).sqrMagnitude > 1e-5f)
            {
                transform.rotation = Quaternion.LookRotation(velocityUnityPerSecond.normalized, up);
            }
            else
            {
                transform.up = up;
            }
        }

        LastGeodetic = geodetic;
        LastStatusText = labelText;
        if (velocityLine != null)
        {
            bool hasVelocity = showVelocityLine && velocityUnityPerSecond.sqrMagnitude > 1e-10f && velocityPreviewSeconds > 0f;
            velocityLine.enabled = hasVelocity;
            if (hasVelocity)
            {
                velocityLine.SetPosition(0, position);
                velocityLine.SetPosition(1, position + velocityUnityPerSecond * velocityPreviewSeconds);
            }
        }

        if (label != null)
        {
            Vector3 labelNormal = position.sqrMagnitude > 1e-6f ? position.normalized : Vector3.up;
            Camera camera = Camera.main;
            bool labelNearPanel = false;
            bool labelNearCenter = false;
            bool labelVisible = showLabel;
            if (camera != null)
            {
                Vector3 viewport = camera.WorldToViewportPoint(position);
                labelNearPanel = viewport.z > 0f && viewport.x > 0.70f && viewport.y > 0.68f;
                labelNearCenter = viewport.z > 0f && Mathf.Abs(viewport.x - 0.5f) < 0.16f && Mathf.Abs(viewport.y - 0.5f) < 0.14f;
                labelVisible = labelVisible && viewport.z > 0f && (!isSelected || !labelNearPanel);
            }
            label.gameObject.SetActive(labelVisible);
            label.text = string.IsNullOrEmpty(labelText)
                ? SatelliteName + "\n" + geodetic.latitudeDegrees.ToString("0.0") + "°, " + geodetic.longitudeDegrees.ToString("0.0") + "°"
                : labelText;
            label.transform.position = position + labelNormal * (isSelected ? 0.085f : 0.055f);
            if (labelBillboard != null)
            {
                float labelAlpha = isSelected ? 0.30f : 0.20f;
                float labelScale = 0.34f;
                if (isSelected && labelNearCenter)
                {
                    labelAlpha = 0.18f;
                    labelScale = 0.32f;
                }
                labelBillboard.SetVisibilityOverride(labelAlpha, labelScale, isSelected && !labelNearCenter && !labelNearPanel);
            }
            if (camera != null)
            {
                label.transform.rotation = Quaternion.LookRotation(label.transform.position - camera.transform.position);
            }
        }
    }

    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }

    private Renderer CreatePart(string partName, PrimitiveType primitiveType, Vector3 localPosition, Vector3 localScale, Material material)
    {
        GameObject part = GameObject.CreatePrimitive(primitiveType);
        part.name = partName;
        part.transform.SetParent(visualRoot, false);
        part.transform.localPosition = localPosition;
        part.transform.localScale = localScale;
        Renderer renderer = part.GetComponent<Renderer>();
        if (material != null)
        {
            renderer.material = material;
        }
        Collider collider = part.GetComponent<Collider>();
        if (collider != null)
        {
            Destroy(collider);
        }
        return renderer;
    }

    private static void SetRendererColor(Renderer renderer, Color color)
    {
        if (renderer == null || renderer.material == null)
        {
            return;
        }
        color.a = Mathf.Clamp01(color.a);
        renderer.material.color = color;
        if (renderer.material.HasProperty("_EmissionColor"))
        {
            renderer.material.EnableKeyword("_EMISSION");
            renderer.material.SetColor("_EmissionColor", color * 0.75f);
        }
    }
}
