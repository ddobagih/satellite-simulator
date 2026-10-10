using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ConnectionLine : MonoBehaviour
{
    public Transform satellite;
    public Transform groundStation;
    public Transform packetSource;
    public Transform packetDestination;
    public SignalPacketStream packetStream;
    public string routeStageLabel = "업링크";
    public string routeSourceLabel = "발신지";
    public string routeDestinationLabel = "LEO 수신 위성";
    public bool showPacketStream = true;
    public float lineBaseWidth = 0.010f;
    public float linePulseWidth = 0.0010f;
    public int arcPointCount = 18;
    public float arcLiftMultiplier = 0.05f;

    private LineRenderer lineRenderer;
    private float pulseSeed;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lineRenderer.receiveShadows = false;
        lineRenderer.positionCount = Mathf.Max(2, arcPointCount);
        lineRenderer.widthMultiplier = lineBaseWidth;
        lineRenderer.enabled = false;
        pulseSeed = Random.value * 10f;

        if (packetStream == null)
        {
            packetStream = gameObject.AddComponent<SignalPacketStream>();
            packetStream.packetCount = 3;
            packetStream.packetSpeed = 1.35f;
            packetStream.packetSize = 0.032f;
        }
    }

    private void LateUpdate()
    {
        Transform source = PacketSourceTransform();
        Transform destination = PacketDestinationTransform();
        bool active = lineRenderer.enabled && source != null && destination != null;
        if (!active)
        {
            if (packetStream != null)
            {
                packetStream.SetVisible(false);
            }
            return;
        }

        DrawArc(source.position, destination.position);
        lineRenderer.widthMultiplier = Mathf.Max(0.002f, lineBaseWidth + Mathf.Sin(Time.time * 3.2f + pulseSeed) * linePulseWidth);

        if (packetStream != null)
        {
            if (showPacketStream)
            {
                packetStream.SetPositions(source.position, destination.position);
                packetStream.SetVisible(true);
            }
            else
            {
                packetStream.SetVisible(false);
            }
        }
    }

    public void SetPacketRoute(Transform source, Transform destination)
    {
        packetSource = source;
        packetDestination = destination;
    }

    public void SetRouteLabels(string stageLabel, string sourceLabel, string destinationLabel)
    {
        routeStageLabel = string.IsNullOrEmpty(stageLabel) ? routeStageLabel : stageLabel;
        routeSourceLabel = string.IsNullOrEmpty(sourceLabel) ? routeSourceLabel : sourceLabel;
        routeDestinationLabel = string.IsNullOrEmpty(destinationLabel) ? routeDestinationLabel : destinationLabel;
    }

    public void SetLineWidth(float baseWidth, float pulseWidth)
    {
        lineBaseWidth = Mathf.Max(0.002f, baseWidth);
        linePulseWidth = Mathf.Max(0f, pulseWidth);
    }

    public string RouteLabel()
    {
        return routeStageLabel + ": " + routeSourceLabel + " → " + routeDestinationLabel;
    }

    public void SetPacketStreamVisible(bool visible)
    {
        showPacketStream = visible;
        if (!visible && packetStream != null)
        {
            packetStream.SetVisible(false);
        }
    }

    public void SetVisible(bool visible)
    {
        if (lineRenderer != null)
        {
            lineRenderer.enabled = visible;
        }
        if (!visible && packetStream != null)
        {
            packetStream.SetVisible(false);
        }
    }

    private Transform PacketSourceTransform()
    {
        return packetSource != null ? packetSource : groundStation;
    }

    private Transform PacketDestinationTransform()
    {
        return packetDestination != null ? packetDestination : satellite;
    }

    private void DrawArc(Vector3 sourcePosition, Vector3 destinationPosition)
    {
        int count = Mathf.Max(2, arcPointCount);
        if (lineRenderer.positionCount != count)
        {
            lineRenderer.positionCount = count;
        }

        float distance = Vector3.Distance(sourcePosition, destinationPosition);
        float surfaceRadius = Mathf.Min(sourcePosition.magnitude, destinationPosition.magnitude);
        float lift = Mathf.Max(0.08f, distance * arcLiftMultiplier);
        for (int i = 0; i < count; i++)
        {
            float t = i / (float)(count - 1);
            Vector3 position = Vector3.Lerp(sourcePosition, destinationPosition, t);
            Vector3 direction = position.sqrMagnitude > 0.001f ? position.normalized : Vector3.up;
            Vector3 arcedPosition = position + direction * Mathf.Sin(t * Mathf.PI) * lift;
            float minRadius = Mathf.Max(0f, surfaceRadius + 0.14f);
            if (arcedPosition.magnitude < minRadius && arcedPosition.sqrMagnitude > 0.001f)
            {
                arcedPosition = arcedPosition.normalized * minRadius;
            }
            lineRenderer.SetPosition(i, arcedPosition);
        }
    }
}
