using UnityEngine;

public class SignalPacketStream : MonoBehaviour
{
    public int packetCount = 3;
    public float packetSpeed = 1.05f;
    public float packetSize = 0.032f;
    public Color packetColor = new Color(0.34f, 1f, 0.9f, 1f);
    public float pathLift = 0.12f;
    public float sideWave = 0.02f;
    public float laneOffset;
    public bool useTrails = true;
    public float trailTime = 0.18f;
    public float emissionIntensity = 2.5f;
    public bool highContrastMode;
    public Color highContrastPacketColor = new Color(1f, 0.95f, 0.12f, 1f);
    public float pathGlowAlphaMultiplier = 1f;
    public float pathGlowWidthMultiplier = 1f;
    public float trailAlphaMultiplier = 1f;
    public float packetPulseMultiplier = 1f;

    private GameObject[] packets;
    private TrailRenderer[] trails;
    private LineRenderer pathGlowLine;
    private Material packetMaterial;
    private Material trailMaterial;
    private Material pathGlowMaterial;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private bool visible;
    private bool replayPaused;
    private float replaySpeedMultiplier = 1f;
    private float playbackProgress;

    public bool ReplayPaused
    {
        get { return replayPaused; }
    }

    public float ReplaySpeedMultiplier
    {
        get { return replaySpeedMultiplier; }
    }

    public float PlaybackProgress
    {
        get { return playbackProgress; }
    }

    private void Awake()
    {
        EnsurePackets();
        SetVisible(false);
    }

    private void LateUpdate()
    {
        if (!visible)
        {
            SetPathGlowVisible(false);
            return;
        }

        EnsurePackets();
        Vector3 direction = endPosition - startPosition;
        float distance = direction.magnitude;
        if (distance < 0.001f)
        {
            SetPacketRenderers(false);
            SetPathGlowVisible(false);
            return;
        }

        Vector3 normal = direction / distance;
        Vector3 side = PacketSide(normal);
        Vector3 arc = PacketArc(normal, side);
        UpdatePathGlow(side, arc);
        float speed = Mathf.Max(0f, packetSpeed) * replaySpeedMultiplier;
        if (!replayPaused && speed > 0f)
        {
            playbackProgress = Mathf.Repeat(playbackProgress + Time.deltaTime * speed, 1f);
        }

        float baseT = playbackProgress >= 1f ? 0.999f : Mathf.Repeat(playbackProgress, 1f);
        SetPacketRenderers(true);

        for (int i = 0; i < packets.Length; i++)
        {
            float t = Mathf.Repeat(baseT + i / (float)packets.Length, 1f);
            float pulseTime = replayPaused ? playbackProgress * 6f : Time.time * 6f;
            float pulse = 0.94f + Mathf.Sin(pulseTime + i) * 0.045f + Mathf.Sin(t * Mathf.PI) * 0.12f;
            packets[i].transform.position = EvaluatePath(t, side, arc);
            packets[i].transform.localScale = Vector3.one * packetSize * pulse * Mathf.Max(0.2f, packetPulseMultiplier);
            packets[i].SetActive(true);
        }
    }

    public void SetPositions(Vector3 start, Vector3 end)
    {
        bool endpointJumped = (start - startPosition).sqrMagnitude > 0.75f || (end - endPosition).sqrMagnitude > 0.75f;
        startPosition = start;
        endPosition = end;
        if (visible && endpointJumped)
        {
            ClearTrails();
        }
    }

    public void SetVisible(bool isVisible)
    {
        if (visible == isVisible)
        {
            return;
        }
        visible = isVisible;
        SetPacketRenderers(isVisible);
        SetPathGlowVisible(isVisible);
        if (!isVisible)
        {
            ClearTrails();
        }
    }

    public void SetReplayPaused(bool paused)
    {
        replayPaused = paused;
        UpdateTrailAppearance();
    }

    public void SetReplaySpeedMultiplier(float speedMultiplier)
    {
        replaySpeedMultiplier = Mathf.Max(0f, speedMultiplier);
        UpdateTrailAppearance();
    }

    public void SetProgressNormalized(float progress01)
    {
        float nextProgress = Mathf.Clamp01(progress01);
        if (nextProgress >= 1f)
        {
            nextProgress = 0.999f;
        }

        if (Mathf.Abs(nextProgress - playbackProgress) > 0.035f)
        {
            ClearTrails();
        }

        playbackProgress = nextProgress;
    }

    public void SetReplayControls(bool paused, float speedMultiplier, float progress01)
    {
        SetReplayPaused(paused);
        SetReplaySpeedMultiplier(speedMultiplier);
        SetProgressNormalized(progress01);
    }

    public void ClearReplayControls()
    {
        replayPaused = false;
        replaySpeedMultiplier = 1f;
        UpdateTrailAppearance();
    }

    private void EnsurePackets()
    {
        int count = Mathf.Max(1, packetCount);
        if (packets != null && packets.Length == count)
        {
            UpdateMaterialColor();
            UpdateTrailAppearance();
            return;
        }

        if (packets != null)
        {
            for (int i = 0; i < packets.Length; i++)
            {
                if (packets[i] != null)
                {
                    Destroy(packets[i]);
                }
            }
        }

        if (packetMaterial == null)
        {
            packetMaterial = CreatePacketMaterial();
        }
        if (trailMaterial == null)
        {
            trailMaterial = CreateTrailMaterial();
        }
        if (pathGlowMaterial == null)
        {
            pathGlowMaterial = CreateTrailMaterial();
            pathGlowMaterial.name = "Signal Route Soft Glow";
        }
        EnsurePathGlowLine();
        UpdateMaterialColor();

        packets = new GameObject[count];
        trails = new TrailRenderer[count];
        for (int i = 0; i < packets.Length; i++)
        {
            GameObject packet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            packet.name = "Signal Packet " + (i + 1);
            packet.transform.SetParent(transform, false);
            packet.transform.localScale = Vector3.one * packetSize;
            Renderer renderer = packet.GetComponent<Renderer>();
            renderer.sharedMaterial = packetMaterial;
            Collider collider = packet.GetComponent<Collider>();
            if (collider != null)
            {
                Destroy(collider);
            }

            TrailRenderer trail = packet.AddComponent<TrailRenderer>();
            trail.sharedMaterial = trailMaterial;
            trail.autodestruct = false;
            trail.emitting = false;
            trail.numCapVertices = 4;
            trail.minVertexDistance = 0.015f;
            trails[i] = trail;

            packet.SetActive(false);
            packets[i] = packet;
        }
        UpdateTrailAppearance();
    }


    private void EnsurePathGlowLine()
    {
        if (pathGlowLine != null)
        {
            return;
        }

        GameObject pathObject = new GameObject("Signal Route Glow");
        pathObject.transform.SetParent(transform, false);
        pathGlowLine = pathObject.AddComponent<LineRenderer>();
        pathGlowLine.sharedMaterial = pathGlowMaterial;
        pathGlowLine.useWorldSpace = true;
        pathGlowLine.loop = false;
        pathGlowLine.positionCount = 36;
        pathGlowLine.widthMultiplier = packetSize * 0.24f;
        pathGlowLine.numCapVertices = 8;
        pathGlowLine.numCornerVertices = 8;
        pathGlowLine.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        pathGlowLine.receiveShadows = false;
        pathGlowLine.enabled = false;
    }

    private void UpdatePathGlow(Vector3 side, Vector3 arc)
    {
        EnsurePathGlowLine();
        if (pathGlowLine == null)
        {
            return;
        }

        pathGlowLine.enabled = visible;
        pathGlowLine.widthMultiplier = packetSize * (highContrastMode ? 0.38f : 0.24f) * Mathf.Max(0.25f, pathGlowWidthMultiplier);
        for (int i = 0; i < pathGlowLine.positionCount; i++)
        {
            float t = i / (float)(pathGlowLine.positionCount - 1);
            pathGlowLine.SetPosition(i, EvaluatePath(t, side, arc));
        }

        Color color = EffectivePacketColor(packetColor);
        float alpha = (highContrastMode ? 0.13f : 0.07f)
            * Mathf.Max(0f, pathGlowAlphaMultiplier)
            * (0.95f + Mathf.Sin(Time.time * 2.2f) * 0.04f);
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new[]
            {
                new GradientColorKey(color, 0f),
                new GradientColorKey(Color.white, 0.5f),
                new GradientColorKey(color, 1f)
            },
            new[]
            {
                new GradientAlphaKey(0f, 0f),
                new GradientAlphaKey(alpha, 0.22f),
                new GradientAlphaKey(alpha * 1.25f, 0.5f),
                new GradientAlphaKey(alpha, 0.78f),
                new GradientAlphaKey(0f, 1f)
            });
        pathGlowLine.colorGradient = gradient;
    }

    private void SetPathGlowVisible(bool isVisible)
    {
        if (pathGlowLine != null)
        {
            pathGlowLine.enabled = isVisible;
        }
    }

    private void SetPacketRenderers(bool isVisible)
    {
        if (packets == null)
        {
            return;
        }

        for (int i = 0; i < packets.Length; i++)
        {
            if (packets[i] != null)
            {
                if (packets[i].activeSelf != isVisible)
                {
                    packets[i].SetActive(isVisible);
                }
            }
            if (trails != null && i < trails.Length && trails[i] != null)
            {
                trails[i].emitting = isVisible && useTrails && !replayPaused && replaySpeedMultiplier > 0f;
                if (!isVisible)
                {
                    trails[i].Clear();
                }
            }
        }
    }

    private Material CreatePacketMaterial()
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
        if (shader == null)
        {
            shader = Shader.Find("Unlit/Color");
        }
        if (shader == null)
        {
            shader = Shader.Find("Standard");
        }
        Material material = new Material(shader);
        material.name = "Signal Packet Glow";
        ApplyMaterialColor(material, packetColor, true);
        return material;
    }

    private Material CreateTrailMaterial()
    {
        Shader shader = Shader.Find("Sprites/Default");
        if (shader == null)
        {
            shader = Shader.Find("Unlit/Transparent");
        }
        if (shader == null)
        {
            shader = Shader.Find("Unlit/Color");
        }

        Material material = new Material(shader);
        material.name = "Signal Packet Trail";
        ApplyMaterialColor(material, packetColor, false);
        return material;
    }

    private void UpdateMaterialColor()
    {
        if (packetMaterial != null)
        {
            ApplyMaterialColor(packetMaterial, packetColor, true);
        }
        if (trailMaterial != null)
        {
            ApplyMaterialColor(trailMaterial, packetColor, false);
        }
        if (pathGlowMaterial != null)
        {
            ApplyMaterialColor(pathGlowMaterial, packetColor, false);
        }
    }

    private void ApplyMaterialColor(Material material, Color color, bool emissive)
    {
        color = EffectivePacketColor(color);
        if (material.HasProperty("_BaseColor"))
        {
            material.SetColor("_BaseColor", color);
        }
        if (material.HasProperty("_Color"))
        {
            material.SetColor("_Color", color);
        }
        if (emissive && material.HasProperty("_EmissionColor"))
        {
            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", color * emissionIntensity);
        }
    }

    private Vector3 EvaluatePath(float t, Vector3 side, Vector3 arc)
    {
        float arcPulse = Mathf.Sin(t * Mathf.PI);
        float waveTime = replayPaused ? playbackProgress * 1.7f : Time.time * 1.7f;
        float wavePulse = Mathf.Sin(t * Mathf.PI * 2f + waveTime);
        float distance = Vector3.Distance(startPosition, endPosition);
        float effectiveLift = Mathf.Max(pathLift, distance * 0.02f);
        float effectiveSideWave = Mathf.Max(sideWave, distance * 0.002f);
        return Vector3.Lerp(startPosition, endPosition, t)
            + arc * (effectiveLift * arcPulse)
            + side * (laneOffset + effectiveSideWave * wavePulse);
    }

    private Vector3 PacketSide(Vector3 normal)
    {
        Vector3 cameraUp = Camera.main != null ? Camera.main.transform.up : Vector3.up;
        Vector3 side = Vector3.Cross(normal, cameraUp);
        if (side.sqrMagnitude < 0.0001f)
        {
            side = Vector3.Cross(normal, Vector3.right);
        }
        if (side.sqrMagnitude < 0.0001f)
        {
            side = Vector3.forward;
        }
        return side.normalized;
    }

    private Vector3 PacketArc(Vector3 normal, Vector3 side)
    {
        Vector3 midpoint = (startPosition + endPosition) * 0.5f;
        Vector3 arc = midpoint.sqrMagnitude > 0.001f ? midpoint.normalized : Vector3.up;
        if (Mathf.Abs(Vector3.Dot(arc, normal)) > 0.92f)
        {
            arc = Vector3.Cross(normal, side).normalized;
        }
        return arc;
    }

    private void UpdateTrailAppearance()
    {
        if (trails == null)
        {
            return;
        }

        Color effectiveColor = EffectivePacketColor(packetColor);
        float trailAlpha = (highContrastMode ? 0.72f : 0.54f) * Mathf.Max(0f, trailAlphaMultiplier);
        Color startColor = new Color(effectiveColor.r, effectiveColor.g, effectiveColor.b, Mathf.Clamp01(trailAlpha));
        Color endColor = new Color(effectiveColor.r, effectiveColor.g, effectiveColor.b, 0f);
        for (int i = 0; i < trails.Length; i++)
        {
            if (trails[i] == null)
            {
                continue;
            }

            trails[i].time = trailTime;
            trails[i].startWidth = packetSize * (highContrastMode ? 0.92f : 0.78f);
            trails[i].endWidth = 0f;
            trails[i].startColor = startColor;
            trails[i].endColor = endColor;
            trails[i].emitting = visible && useTrails && !replayPaused && replaySpeedMultiplier > 0f;
        }
    }

    private Color EffectivePacketColor(Color fallback)
    {
        return highContrastMode ? highContrastPacketColor : fallback;
    }

    private void ClearTrails()
    {
        if (trails == null)
        {
            return;
        }

        for (int i = 0; i < trails.Length; i++)
        {
            if (trails[i] != null)
            {
                trails[i].Clear();
            }
        }
    }
}
