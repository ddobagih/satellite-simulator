using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ConnectionLine : MonoBehaviour
{
    public Transform satellite;
    public Transform groundStation;

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.widthMultiplier = 0.065f;
        lineRenderer.enabled = false;
    }

    private void LateUpdate()
    {
        if (!lineRenderer.enabled || satellite == null || groundStation == null)
        {
            return;
        }

        lineRenderer.SetPosition(0, satellite.position);
        lineRenderer.SetPosition(1, groundStation.position);
    }

    public void SetVisible(bool visible)
    {
        if (lineRenderer != null)
        {
            lineRenderer.enabled = visible;
        }
    }
}
