using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class OrbitLineRenderer : MonoBehaviour
{
    public SatelliteOrbit satelliteOrbit;
    public int pointCount = 360;

    private LineRenderer lineRenderer;
    private float baseWidth = 0.0085f;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lineRenderer.receiveShadows = false;
        lineRenderer.loop = true;
        lineRenderer.useWorldSpace = true;
        lineRenderer.widthMultiplier = baseWidth;
        lineRenderer.alignment = LineAlignment.View;
        lineRenderer.textureMode = LineTextureMode.Stretch;
        lineRenderer.numCapVertices = 8;
        lineRenderer.numCornerVertices = 8;
        lineRenderer.colorGradient = CreateOrbitGradient();
    }

    private void Start()
    {
        DrawOrbit();
    }

    private void LateUpdate()
    {
        if (lineRenderer == null)
        {
            return;
        }

        float pulse = 0.88f + Mathf.Sin(Time.time * 0.9f) * 0.04f;
        lineRenderer.widthMultiplier = baseWidth * pulse;
        lineRenderer.colorGradient = CreateOrbitGradient();
    }

    public void DrawOrbit()
    {
        if (satelliteOrbit == null || lineRenderer == null)
        {
            return;
        }

        lineRenderer.positionCount = pointCount;
        for (int i = 0; i < pointCount; i++)
        {
            float degrees = i / (float)pointCount * 360f;
            lineRenderer.SetPosition(i, satelliteOrbit.GetOrbitPosition(degrees));
        }
    }

    private Gradient CreateOrbitGradient()
    {
        Color whiteBlue = new Color(0.92f, 0.97f, 1f, 1f);
        float alphaPulse = 0.92f + Mathf.Sin(Time.time * 0.7f) * 0.08f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new[]
            {
                new GradientColorKey(whiteBlue, 0f),
                new GradientColorKey(Color.white, 0.5f),
                new GradientColorKey(whiteBlue, 1f)
            },
            new[]
            {
                new GradientAlphaKey(0.1f * alphaPulse, 0f),
                new GradientAlphaKey(0.24f * alphaPulse, 0.5f),
                new GradientAlphaKey(0.1f * alphaPulse, 1f)
            });
        return gradient;
    }
}
