using UnityEngine;

public class DestinationPinVisual : MonoBehaviour
{
    public ObservationTarget target;
    public MissionManager missionManager;
    public TextMesh label;
    public Transform marker;

    private LineRenderer stemLine;
    private LineRenderer selectionRing;
    private LineRenderer pulseRing;
    private TextMesh labelShadow;
    private BillboardLabel labelBillboard;
    private BillboardLabel shadowBillboard;
    private Material lineMaterial;
    private string baseLabelText;
    private float baseLabelSize;

    public void Initialize(ObservationTarget observationTarget, TextMesh targetLabel, Transform markerTransform)
    {
        target = observationTarget;
        label = targetLabel;
        marker = markerTransform;
        if (label != null)
        {
            baseLabelText = label.text;
            baseLabelSize = label.characterSize;
        }
        CreateVisuals();
        EnsureLabelShadow();
        ConfigureLabelBillboards();
    }

    public void SetMissionManager(MissionManager manager)
    {
        missionManager = manager;
    }

    private void Awake()
    {
        CreateVisuals();
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            target = GetComponent<ObservationTarget>();
        }
        if (target == null)
        {
            SetVisible(false);
            return;
        }

        bool originSelected = missionManager != null
            && missionManager.HasExplicitOriginSelection
            && missionManager.SelectedOrigin == target;
        bool destinationSelected = missionManager != null
            && missionManager.HasExplicitDestinationSelection
            && missionManager.SelectedTarget == target;
        bool selected = originSelected || destinationSelected;
        float visibility = CalculateLabelVisibility(selected);
        bool labelVisible = selected || visibility > 0.06f;
        bool focused = selected || (visibility > 0.42f && IsNearCameraFocus());
        Color pinColor = selected
            ? new Color(0.78f, 1f, 1f, 1f)
            : new Color(0.48f, 0.86f, 1f, Mathf.Lerp(0.18f, focused ? 0.82f : 0.52f, visibility));

        EnsureLabelShadow();
        ConfigureLabelBillboards();
        SetLabelVisible(labelVisible);
        UpdateLabel(selected, focused, pinColor, visibility);
        UpdateStem(pinColor, focused && labelVisible);
        UpdateRing(pinColor, selected, (focused || selected) && labelVisible);
        UpdatePulseRing(pinColor, selected);
    }

    private void CreateVisuals()
    {
        if (lineMaterial == null)
        {
            Shader shader = Shader.Find("Sprites/Default");
            if (shader == null)
            {
                shader = Shader.Find("Unlit/Color");
            }
            if (shader == null)
            {
                shader = Shader.Find("Hidden/InternalErrorShader");
            }
            lineMaterial = new Material(shader);
            lineMaterial.name = "Destination Pin Accent";
        }

        if (stemLine == null)
        {
            GameObject stemObject = new GameObject("Destination Label Stem");
            stemObject.transform.SetParent(transform, false);
            stemLine = stemObject.AddComponent<LineRenderer>();
            ConfigureLine(stemLine, 2, false, 0.0025f);
        }

        if (selectionRing == null)
        {
            GameObject ringObject = new GameObject("Destination Selection Ring");
            ringObject.transform.SetParent(transform, false);
            selectionRing = ringObject.AddComponent<LineRenderer>();
            ConfigureLine(selectionRing, 64, true, 0.004f);
        }

        if (pulseRing == null)
        {
            GameObject pulseObject = new GameObject("Destination Arrival Pulse Ring");
            pulseObject.transform.SetParent(transform, false);
            pulseRing = pulseObject.AddComponent<LineRenderer>();
            ConfigureLine(pulseRing, 64, true, 0.004f);
        }
    }

    private void ConfigureLine(LineRenderer line, int points, bool loop, float width)
    {
        line.sharedMaterial = lineMaterial;
        line.useWorldSpace = true;
        line.loop = loop;
        line.positionCount = points;
        line.widthMultiplier = width;
        line.numCapVertices = 6;
        line.numCornerVertices = 6;
        line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        line.receiveShadows = false;
        line.enabled = false;
    }

    private void UpdateLabel(bool selected, bool focused, Color color, float visibility)
    {
        if (label == null)
        {
            return;
        }
        if (string.IsNullOrEmpty(baseLabelText))
        {
            baseLabelText = label.text;
        }
        if (baseLabelSize <= 0f)
        {
            baseLabelSize = label.characterSize;
        }

        label.text = RoleLabelText();
        float labelScale = selected ? 0.72f : Mathf.Lerp(0.36f, focused ? 0.54f : 0.44f, visibility);
        label.characterSize = baseLabelSize * labelScale;
        label.fontStyle = selected ? FontStyle.Bold : FontStyle.Normal;
        label.color = color;
        if (labelBillboard != null)
        {
            labelBillboard.SetVisibilityOverride(color.a, selected ? 0.92f : Mathf.Lerp(0.70f, 0.86f, visibility), selected);
        }
        if (labelShadow != null)
        {
            labelShadow.text = label.text;
            labelShadow.characterSize = label.characterSize;
            labelShadow.fontStyle = label.fontStyle;
            labelShadow.transform.position = label.transform.position + ShadowOffset();
            labelShadow.transform.rotation = label.transform.rotation;
            float shadowAlpha = selected ? 0.82f : Mathf.Lerp(0.2f, 0.55f, visibility);
            labelShadow.color = new Color(0.01f, 0.04f, 0.08f, shadowAlpha);
            labelShadow.gameObject.SetActive((focused || selected) && color.a > 0.05f);
            if (shadowBillboard != null)
            {
                shadowBillboard.SetVisibilityOverride(shadowAlpha, selected ? 0.92f : Mathf.Lerp(0.70f, 0.86f, visibility), selected);
            }
        }
    }

    private void UpdateStem(Color color, bool focused)
    {
        if (stemLine == null || label == null)
        {
            SetLineVisible(stemLine, false);
            return;
        }

        stemLine.enabled = focused;
        stemLine.widthMultiplier = focused ? 0.003f : 0.002f;
        stemLine.startColor = color;
        stemLine.endColor = new Color(color.r, color.g, color.b, color.a * 0.28f);
        stemLine.SetPosition(0, transform.position);
        stemLine.SetPosition(1, label.transform.position);
    }

    private string RoleLabelText()
    {
        string city = baseLabelText;
        if (target != null && !string.IsNullOrEmpty(target.targetName))
        {
            city = OrbitShareDestinations.Label(target.targetName);
        }

        if (missionManager == null)
        {
            return city;
        }

        bool origin = missionManager.HasExplicitOriginSelection && missionManager.SelectedOrigin == target;
        bool destination = missionManager.HasExplicitDestinationSelection && missionManager.SelectedTarget == target;
        if (origin && destination)
        {
            return "발신/수신 " + city;
        }
        if (origin)
        {
            return "발신 " + city;
        }
        if (destination)
        {
            return "수신 " + city;
        }
        return city;
    }

    private void UpdateRing(Color color, bool selected, bool focused)
    {
        if (selectionRing == null)
        {
            return;
        }

        selectionRing.enabled = focused;
        selectionRing.widthMultiplier = selected ? 0.005f : 0.003f;
        selectionRing.startColor = color;
        selectionRing.endColor = color;

        Vector3 normal = transform.position.sqrMagnitude < 0.001f ? Vector3.up : transform.position.normalized;
        Vector3 tangent = Vector3.Cross(normal, Vector3.up);
        if (tangent.sqrMagnitude < 0.001f)
        {
            tangent = Vector3.Cross(normal, Vector3.right);
        }
        tangent.Normalize();
        Vector3 bitangent = Vector3.Cross(normal, tangent).normalized;
        float pulse = selected ? 1f + Mathf.Sin(Time.time * 3.2f) * 0.05f : 1f;
        float radius = selected ? 0.05f * pulse : 0.032f;
        for (int i = 0; i < selectionRing.positionCount; i++)
        {
            float t = i / (float)selectionRing.positionCount * Mathf.PI * 2f;
            selectionRing.SetPosition(i, transform.position + normal * 0.016f + tangent * Mathf.Cos(t) * radius + bitangent * Mathf.Sin(t) * radius);
        }
    }

    private void UpdatePulseRing(Color color, bool selected)
    {
        if (pulseRing == null)
        {
            return;
        }

        pulseRing.enabled = selected;
        if (!selected)
        {
            return;
        }

        float t01 = Mathf.Repeat(Time.time * 0.58f, 1f);
        Color pulseColor = new Color(color.r, color.g, color.b, Mathf.Lerp(0.09f, 0.004f, t01));
        pulseRing.startColor = pulseColor;
        pulseRing.endColor = pulseColor;
        pulseRing.widthMultiplier = Mathf.Lerp(0.004f, 0.0015f, t01);
        DrawRing(pulseRing, Mathf.Lerp(0.05f, 0.09f, t01));
    }

    private void DrawRing(LineRenderer line, float radius)
    {
        Vector3 normal = transform.position.sqrMagnitude < 0.001f ? Vector3.up : transform.position.normalized;
        Vector3 tangent = Vector3.Cross(normal, Vector3.up);
        if (tangent.sqrMagnitude < 0.001f)
        {
            tangent = Vector3.Cross(normal, Vector3.right);
        }
        tangent.Normalize();
        Vector3 bitangent = Vector3.Cross(normal, tangent).normalized;
        for (int i = 0; i < line.positionCount; i++)
        {
            float t = i / (float)line.positionCount * Mathf.PI * 2f;
            line.SetPosition(i, transform.position + normal * 0.028f + tangent * Mathf.Cos(t) * radius + bitangent * Mathf.Sin(t) * radius);
        }
    }

    private void EnsureLabelShadow()
    {
        if (label == null || labelShadow != null)
        {
            return;
        }

        GameObject shadow = new GameObject(label.name + " Shadow", typeof(TextMesh));
        shadow.transform.SetParent(label.transform.parent, false);
        labelShadow = shadow.GetComponent<TextMesh>();
        labelShadow.anchor = label.anchor;
        labelShadow.alignment = label.alignment;
        labelShadow.fontSize = label.fontSize;
        shadowBillboard = labelShadow.gameObject.AddComponent<BillboardLabel>();
        shadowBillboard.autoVisibility = false;
        ConfigureLabelBillboards();
    }

    private void ConfigureLabelBillboards()
    {
        if (label != null && labelBillboard == null)
        {
            labelBillboard = label.GetComponent<BillboardLabel>();
            if (labelBillboard != null)
            {
                labelBillboard.autoVisibility = false;
            }
        }
        if (labelShadow != null && shadowBillboard == null)
        {
            shadowBillboard = labelShadow.GetComponent<BillboardLabel>();
            if (shadowBillboard != null)
            {
                shadowBillboard.autoVisibility = false;
            }
        }
    }

    private Vector3 ShadowOffset()
    {
        if (Camera.main == null)
        {
            return Vector3.down * 0.012f;
        }
        return (-Camera.main.transform.up + Camera.main.transform.right) * 0.012f;
    }

    private float CalculateLabelVisibility(bool selected)
    {
        Camera camera = Camera.main;
        if (camera == null || label == null)
        {
            return selected ? 1f : 0f;
        }

        Vector3 viewport = camera.WorldToViewportPoint(label.transform.position);
        if (viewport.z <= 0f)
        {
            return 0f;
        }
        if (selected)
        {
            return 1f;
        }

        float edgeFade = Mathf.Min(
            Mathf.Min(Mathf.InverseLerp(0f, 0.07f, viewport.x), Mathf.InverseLerp(1f, 0.93f, viewport.x)),
            Mathf.Min(Mathf.InverseLerp(0f, 0.08f, viewport.y), Mathf.InverseLerp(1f, 0.92f, viewport.y)));
        float distance = Vector3.Distance(camera.transform.position, label.transform.position);
        float distanceFade = Mathf.InverseLerp(17f, 7.5f, distance);
        Vector3 surfaceNormal = target != null && target.transform.position.sqrMagnitude > 0.001f
            ? target.transform.position.normalized
            : transform.position.normalized;
        Vector3 cameraDirection = camera.transform.position.sqrMagnitude > 0.001f
            ? camera.transform.position.normalized
            : Vector3.forward;
        float facingFade = Mathf.InverseLerp(-0.04f, 0.2f, Vector3.Dot(surfaceNormal, cameraDirection));
        return Mathf.Clamp01(edgeFade * distanceFade * facingFade);
    }

    private bool IsNearCameraFocus()
    {
        if (Camera.main == null || label == null)
        {
            return false;
        }
        Vector3 viewport = Camera.main.WorldToViewportPoint(label.transform.position);
        return viewport.z > 0f && viewport.x > 0.18f && viewport.x < 0.82f && viewport.y > 0.16f && viewport.y < 0.84f;
    }

    private void SetLabelVisible(bool visible)
    {
        if (label != null)
        {
            label.gameObject.SetActive(visible);
        }
        if (!visible && labelShadow != null)
        {
            labelShadow.gameObject.SetActive(false);
        }
    }

    private void SetVisible(bool visible)
    {
        SetLabelVisible(visible);
        SetLineVisible(stemLine, visible);
        SetLineVisible(selectionRing, visible);
        SetLineVisible(pulseRing, visible);
        if (labelShadow != null)
        {
            labelShadow.gameObject.SetActive(visible);
        }
    }

    private void SetLineVisible(LineRenderer line, bool visible)
    {
        if (line != null)
        {
            line.enabled = visible;
        }
    }
}
