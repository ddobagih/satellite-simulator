using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class OrbitLineRenderer : MonoBehaviour
{
    public SatelliteOrbit satelliteOrbit;
    public int pointCount = 160;

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.loop = true;
        lineRenderer.useWorldSpace = true;
        lineRenderer.widthMultiplier = 0.025f;
    }

    private void Start()
    {
        DrawOrbit();
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
}
