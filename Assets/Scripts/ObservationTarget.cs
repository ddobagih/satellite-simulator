using UnityEngine;

public class ObservationTarget : MonoBehaviour
{
    public string targetName;
    public float observationAngle = 80f;
    public float estimatedDataMb = 428f;
    public float resolutionMeters = 1.2f;
    public int missionCostCredits = 320;
    public string priorityLabel = "Standard";
    public float weatherRiskPercent = 12f;

    private Renderer markerRenderer;
    private Vector3 baseScale;
    private Material defaultMaterial;
    private Material selectedMaterial;
    private TextMesh labelText;
    private float labelBaseSize;

    public void Initialize(string displayName, Material normal, Material selected, float dataMb, float resolution, int costCredits, string priority, float weatherRisk)
    {
        targetName = displayName;
        estimatedDataMb = dataMb;
        resolutionMeters = resolution;
        missionCostCredits = costCredits;
        priorityLabel = priority;
        weatherRiskPercent = weatherRisk;
        defaultMaterial = normal;
        selectedMaterial = selected;
        markerRenderer = GetComponentInChildren<Renderer>();
        baseScale = transform.localScale;
        SetSelected(false);
    }

    public void SetLabel(TextMesh label)
    {
        labelText = label;
        if (labelText != null)
        {
            labelBaseSize = labelText.characterSize;
            SetSelected(false);
        }
    }

    public void SetSelected(bool selected)
    {
        if (markerRenderer != null)
        {
            markerRenderer.sharedMaterial = selected ? selectedMaterial : defaultMaterial;
        }

        transform.localScale = selected ? baseScale * 1.45f : baseScale;
        if (labelText != null)
        {
            labelText.characterSize = selected ? labelBaseSize * 1.18f : labelBaseSize * 0.72f;
            labelText.color = selected ? new Color(1f, 0.96f, 0.62f, 1f) : new Color(0.86f, 0.92f, 1f, 0.38f);
        }
    }
}
