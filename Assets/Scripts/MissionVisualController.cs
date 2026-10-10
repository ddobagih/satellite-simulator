using UnityEngine;

public class MissionVisualController : MonoBehaviour
{
    public MissionManager missionManager;
    public SatelliteOrbit satelliteOrbit;
    public LineRenderer scanLine;
    public Renderer targetHaloRenderer;
    public Renderer stationRangeRenderer;
    public Material idleTargetMaterial;
    public Material activeTargetMaterial;
    public Material stationIdleMaterial;
    public Material stationActiveMaterial;
    public bool highContrastMode;
    public bool showCoverageVisuals = false;
    public bool showNextConnectionHint = false;
    public bool showOrbitHighlight = false;
    public float routeReplaySpeedMultiplier = 1f;
    public bool routeReplayPaused;
    public float routeReplayProgressOverride = -1f;
    public Transform selectedSatelliteTransform;

    private const int SurfaceRingPoints = 128;
    private const int OrbitArcPoints = 56;
    private const int InterSatelliteArcPoints = 24;
    private const float EarthSurfaceOffset = 0.16f;
    private const float StatusPulseSeconds = 2.05f;
    private const float DestinationRelayMarkerBaseScale = 0.048f;
    private const float RouteReplayUplinkEnd = 0.34f;
    private const float RouteReplayInterSatelliteEnd = 0.67f;

    private enum RouteReplayLeg
    {
        Uplink,
        InterSatellite,
        Downlink
    }

    private SignalPacketStream uplinkPackets;
    private SignalPacketStream interSatellitePackets;
    private SignalPacketStream downlinkPackets;
    private LineRenderer targetCoverageLine;
    private LineRenderer satelliteCoverageLine;
    private LineRenderer orbitHighlightLine;
    private LineRenderer interSatelliteLine;
    private LineRenderer nextConnectionLine;
    private LineRenderer originDestinationPreviewLine;
    private LineRenderer successPulseLine;
    private LineRenderer failurePulseLine;
    private GameObject nextConnectionMarker;
    private GameObject destinationRelayMarker;
    private Material targetCoverageMaterial;
    private Material satelliteCoverageMaterial;
    private Material orbitHighlightMaterial;
    private Material interSatelliteMaterial;
    private Material hintMaterial;
    private Material successPulseMaterial;
    private Material failurePulseMaterial;
    private Material highContrastIdleTargetMaterial;
    private Material highContrastActiveTargetMaterial;
    private Material highContrastStationIdleMaterial;
    private Material highContrastStationActiveMaterial;
    private float scanPulseSeed;
    private MissionPhase lastPhase;
    private bool hasLastPhase;
    private float statusPulseEndTime;
    private bool statusPulseSuccess;
    private bool lastHighContrastMode;
    private RealSatelliteVisualizer realSatelliteVisualizer;
    private Vector3[] realSatelliteOrbitPreviewSamples;
    private readonly System.Collections.Generic.List<Transform> interSatelliteRouteTransforms = new System.Collections.Generic.List<Transform>();

    public SignalPacketStream UplinkPacketStream
    {
        get { return uplinkPackets; }
    }

    public SignalPacketStream DownlinkPacketStream
    {
        get { return downlinkPackets; }
    }

    public SignalPacketStream InterSatellitePacketStream
    {
        get { return interSatellitePackets; }
    }

    private void Awake()
    {
        scanPulseSeed = Random.value * 10f;
        CreateVisualMaterials();
        CreateIndependentVisuals();
        uplinkPackets = CreatePacketStream("UplinkGroundToLeoPackets", SenderColor(1f), SenderHighContrastColor(1f), 3, 0.95f, 0.028f);
        interSatellitePackets = CreatePacketStream("ISLLeoToDestinationRelayPackets", InterSatelliteColor(1f), InterSatelliteHighContrastColor(1f), 3, 1.05f, 0.030f);
        downlinkPackets = CreatePacketStream("DownlinkDestinationRelayToReceiverPackets", ReceiverColor(1f), ReceiverHighContrastColor(1f), 3, 1.05f, 0.030f);
        uplinkPackets.laneOffset = -0.010f;
        interSatellitePackets.laneOffset = 0.003f;
        downlinkPackets.laneOffset = 0.012f;
        uplinkPackets.pathLift = 0.20f;
        interSatellitePackets.pathLift = 0.16f;
        downlinkPackets.pathLift = 0.14f;
        ApplyPalette(true);
    }

    private void LateUpdate()
    {
        ApplyPalette(false);

        if (missionManager == null || satelliteOrbit == null)
        {
            HidePacketStreams();
            HideIndependentVisuals();
            return;
        }

        DetectStatusPulse();

        Transform activeSatellite = ActiveSatelliteTransform();
        Transform source = SourceTransform();
        Transform destination = DestinationTransform();
        Transform destinationRelay = DestinationRelayTransform(activeSatellite);
        bool hasDestination = destination != null;
        Vector3 destinationRelayPosition = hasDestination
            ? destinationRelay != null ? destinationRelay.position : DestinationRelayPosition(destination, activeSatellite)
            : activeSatellite.position;
        bool successPulse = IsStatusPulseActive() && statusPulseSuccess;
        bool failurePulse = IsStatusPulseActive() && !statusPulseSuccess;
        bool routeVisible = IsRouteVisible(missionManager.Phase, successPulse, failurePulse);
        bool uplinkVisible = IsUplinkPacketVisible(missionManager.Phase);
        bool interSatelliteVisible = IsInterSatellitePacketVisible(missionManager.Phase);
        bool downlinkVisible = IsDownlinkPacketVisible(missionManager.Phase);
        bool useInterSatelliteLink = missionManager.ShouldUseInterSatelliteLink;

        if (!hasDestination)
        {
            uplinkVisible = false;
            interSatelliteVisible = false;
            downlinkVisible = false;
        }
        if (!useInterSatelliteLink)
        {
            interSatelliteVisible = false;
        }

        bool packetVisible = IsPacketRoutePhase(missionManager.Phase);
        bool waitingForSend = IsWaitingForSend(missionManager.Phase);
        bool uplinkActive = IsUplinkActive(missionManager.Phase);
        bool interSatelliteActive = IsInterSatelliteActive(missionManager.Phase);
        bool downlinkActive = IsDownlinkActive(missionManager.Phase);
        if (IsRouteReplayVisualActive())
        {
            waitingForSend = false;
            uplinkActive = IsRouteReplayLegVisible(RouteReplayLeg.Uplink);
            interSatelliteActive = useInterSatelliteLink && IsRouteReplayLegVisible(RouteReplayLeg.InterSatellite);
            downlinkActive = IsRouteReplayLegVisible(RouteReplayLeg.Downlink);
        }

        if (scanLine != null)
        {
            scanLine.enabled = routeVisible && hasDestination;
            if (scanLine.enabled)
            {
                float downlinkLift = Mathf.Max(EarthRadius() * 0.045f, Vector3.Distance(destinationRelayPosition, destination.position) * 0.020f);
                DrawLineArc(scanLine, destinationRelayPosition, destination.position, 18, downlinkLift);
                ApplyDownlinkLineStyle(missionManager.Phase, failurePulse, downlinkActive, waitingForSend);
            }
        }

        if (downlinkPackets != null)
        {
            downlinkPackets.packetSpeed = missionManager.IsReplayingRoute ? 0.75f : Mathf.Lerp(0.85f, 1.45f, missionManager.TransferProgress);
            ConfigurePacketStreamVisuals(downlinkPackets, DownlinkColorForPhase(missionManager.Phase, failurePulse), downlinkActive, waitingForSend);
            downlinkPackets.SetPositions(
                destinationRelayPosition,
                hasDestination ? destination.position : destinationRelayPosition);
            ApplyReplayControls(downlinkPackets, RouteReplayLeg.Downlink);
            downlinkPackets.SetVisible(packetVisible && downlinkVisible && hasDestination);
        }

        if (uplinkPackets != null)
        {
            uplinkPackets.packetSpeed = missionManager.IsReplayingRoute ? 0.7f : missionManager.Phase == MissionPhase.Observing ? 1.35f : 0.95f;
            ConfigurePacketStreamVisuals(uplinkPackets, UplinkColorForPhase(missionManager.Phase, failurePulse), uplinkActive, waitingForSend);
            uplinkPackets.SetPositions(
                source != null ? source.position : activeSatellite.position,
                activeSatellite.position);
            ApplyReplayControls(uplinkPackets, RouteReplayLeg.Uplink);
            uplinkPackets.SetVisible(packetVisible && uplinkVisible && source != null);
        }

        if (interSatellitePackets != null)
        {
            Vector3 islPacketStart;
            Vector3 islPacketEnd;
            TryGetCurrentInterSatelliteSegment(activeSatellite.position, destinationRelayPosition, out islPacketStart, out islPacketEnd);
            interSatellitePackets.packetSpeed = missionManager.IsReplayingRoute ? 0.68f : interSatelliteActive ? 1.18f : 0.82f;
            ConfigurePacketStreamVisuals(interSatellitePackets, InterSatelliteColorForPhase(missionManager.Phase, failurePulse), interSatelliteActive, waitingForSend);
            interSatellitePackets.SetPositions(islPacketStart, islPacketEnd);
            ApplyReplayControls(interSatellitePackets, RouteReplayLeg.InterSatellite);
            interSatellitePackets.SetVisible(packetVisible && interSatelliteVisible && hasDestination && useInterSatelliteLink);
        }

        UpdateUplinkLineStyle(routeVisible && hasDestination, uplinkVisible, uplinkActive, failurePulse, waitingForSend, source, activeSatellite);
        UpdateInterSatelliteLine(routeVisible, interSatelliteActive, failurePulse, waitingForSend, activeSatellite.position, destinationRelayPosition, hasDestination && useInterSatelliteLink);
        UpdateDestinationRelayMarker(routeVisible, interSatelliteActive || downlinkActive, destinationRelayPosition, hasDestination);
        UpdateTargetHalo(routeVisible);
        UpdateStationRange();
        bool showRoutePlanningVisuals = hasDestination
            && (routeVisible || missionManager.HasExplicitRouteSelection);
        UpdateCoverageVisuals(showRoutePlanningVisuals, activeSatellite);
        UpdateNextConnectionHint(showRoutePlanningVisuals, activeSatellite);
        UpdateOrbitHighlight(showRoutePlanningVisuals, activeSatellite);
        UpdateOriginDestinationPreview(routeVisible, source, destination);
        UpdateDeliveryPulse(hasDestination);
    }

    public void SetHighContrastMode(bool enabled)
    {
        highContrastMode = enabled;
        ApplyPalette(true);
    }

    public void SetSelectedSatelliteTransform(Transform satelliteTransform)
    {
        selectedSatelliteTransform = satelliteTransform;
    }

    public void SetRouteReplayVisualControls(bool paused, float speedMultiplier, float progress01)
    {
        routeReplayPaused = paused;
        routeReplaySpeedMultiplier = Mathf.Max(0f, speedMultiplier);
        float replayProgress = Mathf.Clamp01(progress01);
        routeReplayProgressOverride = paused ? replayProgress : -1f;
        ApplyReplayControls(uplinkPackets, RouteReplayLeg.Uplink, replayProgress);
        ApplyReplayControls(interSatellitePackets, RouteReplayLeg.InterSatellite, replayProgress);
        ApplyReplayControls(downlinkPackets, RouteReplayLeg.Downlink, replayProgress);
    }

    public void SetRouteReplayProgressOverride(float progress01)
    {
        routeReplayProgressOverride = Mathf.Clamp01(progress01);
        ApplyReplayControls(uplinkPackets, RouteReplayLeg.Uplink);
        ApplyReplayControls(interSatellitePackets, RouteReplayLeg.InterSatellite);
        ApplyReplayControls(downlinkPackets, RouteReplayLeg.Downlink);
    }

    public void ReleaseRouteReplayProgressOverride()
    {
        routeReplayProgressOverride = -1f;
    }

    public void ClearRouteReplayVisualControls()
    {
        routeReplayPaused = false;
        routeReplaySpeedMultiplier = 1f;
        routeReplayProgressOverride = -1f;
        if (uplinkPackets != null)
        {
            uplinkPackets.ClearReplayControls();
        }
        if (interSatellitePackets != null)
        {
            interSatellitePackets.ClearReplayControls();
        }
        if (downlinkPackets != null)
        {
            downlinkPackets.ClearReplayControls();
        }
    }

    public void TriggerDeliveryPulse(bool success)
    {
        statusPulseSuccess = success;
        statusPulseEndTime = Time.time + StatusPulseSeconds;
    }

    private void DetectStatusPulse()
    {
        if (!hasLastPhase)
        {
            lastPhase = missionManager.Phase;
            hasLastPhase = true;
            return;
        }

        if (lastPhase == missionManager.Phase)
        {
            return;
        }

        if (missionManager.Phase == MissionPhase.Complete)
        {
            TriggerDeliveryPulse(true);
        }
        else if (missionManager.Phase == MissionPhase.Failed)
        {
            TriggerDeliveryPulse(false);
        }

        lastPhase = missionManager.Phase;
    }

    private void UpdateTargetHalo(bool routeVisible)
    {
        if (targetHaloRenderer == null)
        {
            return;
        }

        bool pulsing = IsStatusPulseActive();
        bool showTarget = missionManager.HasExplicitDestinationSelection
            || routeVisible
            || pulsing;
        targetHaloRenderer.enabled = showTarget && missionManager.SelectedTarget != null;
        if (!targetHaloRenderer.enabled)
        {
            return;
        }
        bool activeRoute = routeVisible && (missionManager.Phase == MissionPhase.Observing
            || missionManager.Phase == MissionPhase.ObservationComplete
            || missionManager.Phase == MissionPhase.Transmitting
            || missionManager.IsReplayingRoute);
        targetHaloRenderer.sharedMaterial = pulsing ? StatusPulseMaterial() : TargetHaloMaterial(activeRoute);

        Vector3 center = EarthCenter();
        Vector3 direction = (missionManager.SelectedTarget.transform.position - center).normalized;
        targetHaloRenderer.transform.position = center + direction * (EarthRadius() + EarthSurfaceOffset);
        targetHaloRenderer.transform.rotation = Quaternion.FromToRotation(Vector3.up, direction);
        float activePulse = missionManager.Phase == MissionPhase.Transmitting ? 0.08f : 0.04f;
        float haloPulse = routeVisible ? 1f + Mathf.Sin(Time.time * 3.6f + scanPulseSeed) * activePulse : 1f;
        if (missionManager.Phase == MissionPhase.Transmitting)
        {
            haloPulse += missionManager.TransferProgress * 0.04f;
        }
        if (pulsing)
        {
            float flash = 1f + Mathf.Sin(Time.time * 10f + scanPulseSeed) * 0.07f;
            haloPulse *= StatusPulseScale() * flash;
        }
        targetHaloRenderer.transform.localScale = Vector3.one * (0.42f * haloPulse);
    }

    private void UpdateStationRange()
    {
        if (stationRangeRenderer == null)
        {
            return;
        }

        bool pulsing = IsStatusPulseActive();
        bool routeVisible = IsRouteVisible(missionManager.Phase, pulsing && statusPulseSuccess, pulsing && !statusPulseSuccess);
        bool showStation = missionManager.HasExplicitOriginSelection || routeVisible;
        if (missionManager.groundStation != null)
        {
            Renderer stationRenderer = missionManager.groundStation.GetComponent<Renderer>();
            if (stationRenderer != null)
            {
                stationRenderer.enabled = showStation;
            }
        }

        stationRangeRenderer.enabled = showStation;
        if (!showStation)
        {
            return;
        }

        Transform source = SourceTransform();
        if (source != null)
        {
            Vector3 direction = (source.position - EarthCenter()).normalized;
            stationRangeRenderer.transform.position = EarthCenter() + direction * (EarthRadius() + EarthSurfaceOffset);
            stationRangeRenderer.transform.up = direction;
        }
        stationRangeRenderer.sharedMaterial = pulsing ? StatusPulseMaterial() : StationRangeMaterial(missionManager.CommunicationAvailable);
        float rangePulse = missionManager.CommunicationAvailable ? 1f + Mathf.Sin(Time.time * 4.2f) * 0.028f : 1f;
        if (pulsing)
        {
            rangePulse *= StatusPulseScale();
        }
        stationRangeRenderer.transform.localScale = Vector3.one * (0.44f * rangePulse);
    }

    private void UpdateCoverageVisuals(bool hasTarget, Transform activeSatellite)
    {
        if (!showCoverageVisuals || !hasTarget || missionManager.earth == null)
        {
            SetLineEnabled(targetCoverageLine, false);
            SetLineEnabled(satelliteCoverageLine, false);
            return;
        }

        Vector3 center = EarthCenter();
        Vector3 targetDirection = (missionManager.SelectedTarget.transform.position - center).normalized;
        DrawSurfaceRing(targetCoverageLine, targetDirection, missionManager.SelectedTarget.observationAngle, SurfaceRingPoints);
        targetCoverageLine.widthMultiplier = highContrastMode ? 0.024f : 0.012f;

        Vector3 satelliteDirection = (activeSatellite.position - center).normalized;
        DrawSurfaceRing(satelliteCoverageLine, satelliteDirection, SatelliteHorizonAngle(activeSatellite.position), SurfaceRingPoints);
        satelliteCoverageLine.widthMultiplier = highContrastMode ? 0.022f : 0.011f;
    }

    private void UpdateNextConnectionHint(bool hasTarget, Transform activeSatellite)
    {
        if (!showNextConnectionHint || !hasTarget)
        {
            SetLineEnabled(nextConnectionLine, false);
            SetMarkerVisible(false);
            return;
        }

        Vector3 markerPosition;
        if (activeSatellite != null && activeSatellite != satelliteOrbit.transform)
        {
            markerPosition = activeSatellite.position;
        }
        else
        {
            float nextAngle;
            float secondsUntilVisible;
            if (!TryFindNextVisibleOrbitAngle(out nextAngle, out secondsUntilVisible))
            {
                SetLineEnabled(nextConnectionLine, false);
                SetMarkerVisible(false);
                return;
            }
            markerPosition = satelliteOrbit.GetOrbitPosition(nextAngle);
        }

        SetMarkerVisible(true);
        nextConnectionMarker.transform.position = markerPosition;
        float markerPulse = 1f + Mathf.Sin(Time.time * 3.8f + scanPulseSeed) * 0.1f;
        nextConnectionMarker.transform.localScale = Vector3.one * (highContrastMode ? 0.15f : 0.11f) * markerPulse;

        if (nextConnectionLine != null)
        {
            nextConnectionLine.enabled = true;
            nextConnectionLine.positionCount = 2;
            nextConnectionLine.SetPosition(0, markerPosition);
            nextConnectionLine.SetPosition(1, missionManager.SelectedTarget.transform.position);
            nextConnectionLine.widthMultiplier = highContrastMode ? 0.010f : 0.004f;
            Color hintColor = RouteColorForPhase(missionManager.Phase, false);
            hintColor.a *= highContrastMode ? 0.42f : 0.16f;
            ApplyLineRendererColor(nextConnectionLine, hintColor);
        }
    }

    private void UpdateOrbitHighlight(bool hasTarget, Transform activeSatellite)
    {
        if (!showOrbitHighlight || !hasTarget)
        {
            SetLineEnabled(orbitHighlightLine, false);
            return;
        }

        if (activeSatellite != null && activeSatellite != satelliteOrbit.transform)
        {
            DrawFocusedOrbitPreview(activeSatellite);
            return;
        }

        float nextAngle;
        float secondsUntilVisible;
        if (!TryFindNextVisibleOrbitAngle(out nextAngle, out secondsUntilVisible))
        {
            SetLineEnabled(orbitHighlightLine, false);
            return;
        }

        float startAngle = satelliteOrbit.CurrentAngle;
        float endAngle = nextAngle;
        if (Mathf.Abs(endAngle - startAngle) < 2f)
        {
            endAngle = startAngle + Mathf.Sign(Mathf.Approximately(satelliteOrbit.orbitSpeed, 0f) ? 1f : satelliteOrbit.orbitSpeed) * 36f;
        }
        DrawOrbitArc(startAngle, endAngle);
    }

    private bool IsRouteVisible(MissionPhase phase, bool successPulse, bool failurePulse)
    {
        return phase == MissionPhase.Reserved
            || phase == MissionPhase.Running
            || phase == MissionPhase.Observing
            || phase == MissionPhase.ObservationComplete
            || phase == MissionPhase.Transmitting
            || (missionManager != null && missionManager.IsReplayingRoute)
            || successPulse
            || failurePulse;
    }

    private void UpdateOriginDestinationPreview(bool routeVisible, Transform source, Transform destination)
    {
        if (originDestinationPreviewLine == null)
        {
            return;
        }

        bool visible = !routeVisible
            && missionManager != null
            && missionManager.Phase == MissionPhase.Idle
            && missionManager.HasExplicitRouteSelection
            && source != null
            && destination != null
            && source != destination;
        if (!visible)
        {
            SetLineEnabled(originDestinationPreviewLine, false);
            return;
        }

        originDestinationPreviewLine.enabled = true;
        originDestinationPreviewLine.widthMultiplier = highContrastMode ? 0.028f : 0.014f;
        ApplyLineRendererColor(originDestinationPreviewLine, highContrastMode ? new Color(1f, 1f, 1f, 0.78f) : new Color(0.24f, 1f, 0.84f, 0.48f));
        DrawLineArc(originDestinationPreviewLine, source.position, destination.position, 28, EarthRadius() * 0.42f);
    }

    private bool IsPacketRoutePhase(MissionPhase phase)
    {
        return phase == MissionPhase.Running
            || phase == MissionPhase.Observing
            || phase == MissionPhase.ObservationComplete
            || phase == MissionPhase.Transmitting
            || (missionManager != null && missionManager.IsReplayingRoute);
    }

    private bool IsWaitingForSend(MissionPhase phase)
    {
        return phase == MissionPhase.Reserved
            || (phase == MissionPhase.Running && missionManager != null && missionManager.SendWaitProgress < 1f);
    }

    private bool IsUplinkPacketVisible(MissionPhase phase)
    {
        SatelliteCommunicationStep step = CurrentCommunicationStep();
        if (IsRouteReplayVisualActive())
        {
            return IsRouteReplayLegVisible(RouteReplayLeg.Uplink);
        }
        if (step == SatelliteCommunicationStep.Uplink || step == SatelliteCommunicationStep.LeoReceived)
        {
            return true;
        }
        bool previewPackets = phase == MissionPhase.Running
            && missionManager != null
            && missionManager.SendWaitProgress >= 1f;
        return previewPackets;
    }

    private bool IsInterSatellitePacketVisible(MissionPhase phase)
    {
        SatelliteCommunicationStep step = CurrentCommunicationStep();
        if (missionManager != null && !missionManager.ShouldUseInterSatelliteLink)
        {
            return false;
        }
        if (IsRouteReplayVisualActive())
        {
            return IsRouteReplayLegVisible(RouteReplayLeg.InterSatellite);
        }
        if (step == SatelliteCommunicationStep.LeoReceived
            || step == SatelliteCommunicationStep.InterSatelliteLink
            || step == SatelliteCommunicationStep.DestinationRelaySelection)
        {
            return true;
        }
        return false;
    }

    private bool IsDownlinkPacketVisible(MissionPhase phase)
    {
        SatelliteCommunicationStep step = CurrentCommunicationStep();
        if (IsRouteReplayVisualActive())
        {
            return IsRouteReplayLegVisible(RouteReplayLeg.Downlink);
        }
        if (step == SatelliteCommunicationStep.Downlink
            || step == SatelliteCommunicationStep.FinalReceive)
        {
            return true;
        }
        return false;
    }

    private bool IsUplinkActive(MissionPhase phase)
    {
        if (CurrentCommunicationStep() == SatelliteCommunicationStep.Uplink)
        {
            return true;
        }
        return phase == MissionPhase.Observing;
    }

    private bool IsInterSatelliteActive(MissionPhase phase)
    {
        if (missionManager != null && !missionManager.ShouldUseInterSatelliteLink)
        {
            return false;
        }
        SatelliteCommunicationStep step = CurrentCommunicationStep();
        if (step == SatelliteCommunicationStep.InterSatelliteLink
            || step == SatelliteCommunicationStep.DestinationRelaySelection)
        {
            return true;
        }
        return phase == MissionPhase.ObservationComplete;
    }

    private bool IsDownlinkActive(MissionPhase phase)
    {
        SatelliteCommunicationStep step = CurrentCommunicationStep();
        if (step == SatelliteCommunicationStep.Downlink
            || step == SatelliteCommunicationStep.FinalReceive)
        {
            return true;
        }
        return phase == MissionPhase.Transmitting;
    }

    private SatelliteCommunicationStep CurrentCommunicationStep()
    {
        return missionManager != null ? missionManager.CommunicationStep : SatelliteCommunicationStep.Draft;
    }

    private float DownlinkWidthForPhase(MissionPhase phase)
    {
        float pulse = Mathf.Sin(Time.time * 7f + scanPulseSeed) * 0.008f;
        float contrast = highContrastMode ? 1.28f : 1f;
        switch (phase)
        {
            case MissionPhase.Reserved:
                return 0.010f * contrast;
            case MissionPhase.Running:
                return (0.012f + pulse * 0.24f) * contrast;
            case MissionPhase.Observing:
                return (0.020f + pulse * 0.40f) * contrast;
            case MissionPhase.ObservationComplete:
                return (0.018f + pulse * 0.32f) * contrast;
            case MissionPhase.Transmitting:
                return (0.030f + pulse * 0.44f) * contrast;
            default:
                return 0.018f * contrast;
        }
    }

    private void ApplyDownlinkLineStyle(MissionPhase phase, bool failurePulse, bool active, bool waiting)
    {
        if (scanLine == null)
        {
            return;
        }

        if (failurePulse)
        {
            float pulse = 1f + Mathf.Sin(Time.time * 9f + scanPulseSeed) * 0.18f;
            scanLine.widthMultiplier = (highContrastMode ? 0.030f : 0.024f) * pulse;
            ApplyLineRendererColor(scanLine, FailureReasonColor(1f));
            return;
        }

        if (IsStatusPulseActive() && statusPulseSuccess)
        {
            float remaining = StatusPulseRemaining01();
            float pulse = 1f + Mathf.Sin(Time.time * 10f + scanPulseSeed) * 0.22f;
            scanLine.widthMultiplier = (highContrastMode ? 0.034f : 0.027f) * Mathf.Lerp(0.58f, 1.15f, remaining) * pulse;
            Color color = ReceiverColor(Mathf.Lerp(0.4f, 1f, remaining));
            ApplyLineRendererColor(scanLine, color);
            return;
        }

        scanLine.widthMultiplier = DownlinkWidthForPhase(phase);
        ApplyLineRendererColor(scanLine, RouteLegColor(DownlinkColorForPhase(phase, false), active, waiting));
    }

    private Color RouteColorForPhase(MissionPhase phase, bool failurePulse)
    {
        if (failurePulse)
        {
            return FailureReasonColor(1f);
        }

        switch (phase)
        {
            case MissionPhase.Reserved:
                return PendingColor(0.62f);
            case MissionPhase.Running:
                if (missionManager != null && missionManager.SendWaitProgress < 1f)
                {
                    return PendingColor(0.6f);
                }
                return SenderColor(0.92f);
            case MissionPhase.Observing:
                return UplinkColorForPhase(phase, false);
            case MissionPhase.ObservationComplete:
                return InterSatelliteColorForPhase(phase, false);
            case MissionPhase.Transmitting:
                return DownlinkColorForPhase(phase, false);
            default:
                return ReceiverColor(1f);
        }
    }

    private Color UplinkColorForPhase(MissionPhase phase, bool failurePulse)
    {
        if (failurePulse)
        {
            return FailureReasonColor(1f);
        }
        if (phase == MissionPhase.Reserved || (phase == MissionPhase.Running && missionManager != null && missionManager.SendWaitProgress < 1f))
        {
            return PendingColor(0.62f);
        }
        return SenderColor(1f);
    }

    private Color InterSatelliteColorForPhase(MissionPhase phase, bool failurePulse)
    {
        if (failurePulse)
        {
            return FailureReasonColor(1f);
        }
        if (phase == MissionPhase.ObservationComplete)
        {
            return InterSatelliteColor(1f);
        }
        if (phase == MissionPhase.Transmitting)
        {
            return InterSatelliteColor(0.7f);
        }
        return PendingColor(0.58f);
    }

    private Color DownlinkColorForPhase(MissionPhase phase, bool failurePulse)
    {
        if (failurePulse)
        {
            return FailureReasonColor(1f);
        }
        if (phase == MissionPhase.Transmitting)
        {
            return ReceiverColor(1f);
        }
        return phase == MissionPhase.ObservationComplete ? PendingColor(0.62f) : PendingColor(0.72f);
    }

    private Color SenderColor(float alpha)
    {
        return highContrastMode ? SenderHighContrastColor(alpha) : new Color(1f, 0.58f, 0.16f, alpha);
    }

    private Color ReceiverColor(float alpha)
    {
        return highContrastMode ? ReceiverHighContrastColor(alpha) : new Color(0.16f, 1f, 0.88f, alpha);
    }

    private Color InterSatelliteColor(float alpha)
    {
        return highContrastMode ? InterSatelliteHighContrastColor(alpha) : new Color(0.48f, 0.68f, 1f, alpha);
    }

    private Color SenderHighContrastColor(float alpha)
    {
        return new Color(1f, 0.92f, 0.12f, alpha);
    }

    private Color ReceiverHighContrastColor(float alpha)
    {
        return new Color(0f, 1f, 1f, alpha);
    }

    private Color InterSatelliteHighContrastColor(float alpha)
    {
        return new Color(0.72f, 0.82f, 1f, alpha);
    }

    private Color PendingColor(float alpha)
    {
        return highContrastMode ? SenderHighContrastColor(alpha) : new Color(1f, 0.72f, 0.24f, alpha);
    }

    private Color RouteLegColor(Color color, bool active, bool waiting)
    {
        color.a *= active ? 0.94f : waiting ? 0.28f : 0.34f;
        return color;
    }

    private void ConfigurePacketStreamVisuals(SignalPacketStream stream, Color color, bool active, bool waiting)
    {
        if (stream == null)
        {
            return;
        }

        stream.packetColor = color;
        if (highContrastMode)
        {
            stream.highContrastPacketColor = color;
        }
        stream.pathGlowAlphaMultiplier = active ? 0.68f : waiting ? 0.20f : 0.24f;
        stream.pathGlowWidthMultiplier = active ? 0.58f : waiting ? 0.26f : 0.30f;
        stream.trailAlphaMultiplier = active ? 0.46f : waiting ? 0.18f : 0.22f;
        stream.packetPulseMultiplier = active ? 0.92f : waiting ? 0.50f : 0.56f;
    }

    private void UpdateUplinkLineStyle(bool routeVisible, bool uplinkVisible, bool uplinkActive, bool failurePulse, bool waiting, Transform source, Transform relay)
    {
        if (missionManager == null || missionManager.connectionLine == null)
        {
            return;
        }

        Color color = RouteLegColor(UplinkColorForPhase(missionManager.Phase, failurePulse), uplinkActive, waiting);
        missionManager.connectionLine.SetPacketRoute(source, relay);
        missionManager.connectionLine.SetRouteLabels("업링크", "발신지", "LEO 수신 위성");
        if (missionManager.connectionLine.packetStream != null)
        {
            missionManager.connectionLine.packetStream.laneOffset = -0.010f;
            missionManager.connectionLine.packetStream.pathLift = 0.20f;
        }
        bool visible = routeVisible && source != null && relay != null;
        missionManager.connectionLine.SetVisible(visible);
        missionManager.connectionLine.SetPacketStreamVisible(visible && uplinkVisible && !IsRouteReplayVisualActive());
        LineRenderer line = missionManager.connectionLine.GetComponent<LineRenderer>();
        if (line != null)
        {
            ApplyLineRendererColor(line, color);
            if (line.enabled)
            {
                float pulse = 1f + Mathf.Sin(Time.time * (uplinkActive ? 4.8f : 3.4f) + scanPulseSeed) * (uplinkActive ? 0.065f : 0.035f);
                float baseWidth = uplinkActive ? 0.013f : uplinkVisible ? 0.0085f : waiting ? 0.0052f : 0.0060f;
                float contrast = highContrastMode ? 1.25f : 1f;
                float pulseWidth = baseWidth * (uplinkActive ? 0.065f : 0.035f);
                missionManager.connectionLine.SetLineWidth(baseWidth * contrast, pulseWidth * contrast);
                line.widthMultiplier = baseWidth * contrast * pulse;
            }
        }

        ConfigurePacketStreamVisuals(missionManager.connectionLine.packetStream, color, uplinkActive, waiting);
    }

    private void UpdateInterSatelliteLine(bool routeVisible, bool active, bool failurePulse, bool waiting, Vector3 source, Vector3 destination, bool hasDestination)
    {
        if (interSatelliteLine == null)
        {
            return;
        }

        bool visible = routeVisible && hasDestination;
        if (!visible)
        {
            SetLineEnabled(interSatelliteLine, false);
            return;
        }

        Color color = RouteLegColor(InterSatelliteColorForPhase(missionManager.Phase, failurePulse), active, waiting);
        interSatelliteLine.enabled = true;
        interSatelliteLine.loop = false;
        interSatelliteLine.widthMultiplier = InterSatelliteWidth(active, waiting);
        ApplyLineRendererColor(interSatelliteLine, color);
        DrawInterSatelliteRoute(interSatelliteLine, source, destination);
    }

    private void DrawInterSatelliteRoute(LineRenderer line, Vector3 fallbackSource, Vector3 fallbackDestination)
    {
        if (line == null)
        {
            return;
        }

        if (missionManager != null)
        {
            missionManager.CopyInterSatelliteRouteTransforms(interSatelliteRouteTransforms);
        }
        if (interSatelliteRouteTransforms.Count < 2)
        {
            float lift = Mathf.Max(EarthRadius() * 0.050f, Vector3.Distance(fallbackSource, fallbackDestination) * 0.015f);
            DrawLineArc(line, fallbackSource, fallbackDestination, InterSatelliteArcPoints, lift);
            return;
        }

        int pointsPerSegment = Mathf.Max(6, InterSatelliteArcPoints / Mathf.Max(1, interSatelliteRouteTransforms.Count - 1));
        int segmentCount = interSatelliteRouteTransforms.Count - 1;
        int totalPoints = segmentCount * (pointsPerSegment - 1) + 1;
        line.positionCount = totalPoints;
        int cursor = 0;
        for (int segment = 0; segment < segmentCount; segment++)
        {
            Vector3 start = interSatelliteRouteTransforms[segment].position;
            Vector3 end = interSatelliteRouteTransforms[segment + 1].position;
            float lift = Mathf.Max(EarthRadius() * 0.045f, Vector3.Distance(start, end) * 0.012f);
            for (int i = 0; i < pointsPerSegment; i++)
            {
                if (segment > 0 && i == 0)
                {
                    continue;
                }
                float t = i / (float)(pointsPerSegment - 1);
                line.SetPosition(cursor, EvaluateLineArcPoint(start, end, t, lift));
                cursor++;
            }
        }
    }

    private bool TryGetCurrentInterSatelliteSegment(Vector3 fallbackSource, Vector3 fallbackDestination, out Vector3 source, out Vector3 destination)
    {
        source = fallbackSource;
        destination = fallbackDestination;
        if (missionManager == null || !missionManager.ShouldUseInterSatelliteLink)
        {
            return false;
        }

        missionManager.CopyInterSatelliteRouteTransforms(interSatelliteRouteTransforms);
        if (interSatelliteRouteTransforms.Count < 2)
        {
            return false;
        }

        int segmentCount = interSatelliteRouteTransforms.Count - 1;
        float scaledProgress = Mathf.Clamp01(missionManager.InterSatelliteProgress) * segmentCount;
        int index = Mathf.Clamp(Mathf.FloorToInt(scaledProgress), 0, segmentCount - 1);
        source = interSatelliteRouteTransforms[index].position;
        destination = interSatelliteRouteTransforms[index + 1].position;
        return true;
    }

    private float InterSatelliteWidth(bool active, bool waiting)
    {
        float pulse = 1f + Mathf.Sin(Time.time * (active ? 4.4f : 3.2f) + scanPulseSeed) * (active ? 0.07f : 0.035f);
        float baseWidth = active ? 0.018f : waiting ? 0.0060f : 0.0085f;
        return baseWidth * (highContrastMode ? 1.28f : 1f) * pulse;
    }

    private void UpdateDestinationRelayMarker(bool routeVisible, bool active, Vector3 position, bool hasDestination)
    {
        if (destinationRelayMarker == null)
        {
            return;
        }

        bool visible = routeVisible && hasDestination;
        destinationRelayMarker.SetActive(visible);
        if (!visible)
        {
            return;
        }

        destinationRelayMarker.transform.position = position;
        float pulse = 1f + Mathf.Sin(Time.time * (active ? 3.8f : 2.8f) + scanPulseSeed) * (active ? 0.07f : 0.04f);
        destinationRelayMarker.transform.localScale = Vector3.one * DestinationRelayMarkerBaseScale * (highContrastMode ? 1.2f : 1f) * pulse;
    }

    private SignalPacketStream CreatePacketStream(string objectName, Color color, Color highContrastColor, int count, float speed, float size)
    {
        GameObject streamObject = new GameObject(objectName);
        streamObject.transform.SetParent(transform, false);

        SignalPacketStream stream = streamObject.AddComponent<SignalPacketStream>();
        stream.packetCount = count;
        stream.packetSpeed = speed;
        stream.packetSize = size;
        stream.packetColor = color;
        stream.highContrastPacketColor = highContrastColor;
        stream.highContrastMode = highContrastMode;
        stream.emissionIntensity = 2.45f;
        stream.pathLift = 0.24f;
        stream.sideWave = 0.028f;
        stream.trailTime = 0.22f;
        return stream;
    }

    private void ApplyReplayControls(SignalPacketStream stream, RouteReplayLeg leg)
    {
        ApplyReplayControls(stream, leg, CurrentRouteReplayProgress());
    }

    private void ApplyReplayControls(SignalPacketStream stream, RouteReplayLeg leg, float replayProgress)
    {
        if (stream == null)
        {
            return;
        }

        bool replayVisualActive = IsRouteReplayVisualActive();
        stream.highContrastMode = highContrastMode;
        stream.SetReplayPaused(routeReplayPaused || replayVisualActive);
        stream.SetReplaySpeedMultiplier(routeReplaySpeedMultiplier);
        if (replayVisualActive)
        {
            stream.SetProgressNormalized(RouteReplayLegProgress(replayProgress, leg));
        }
    }

    private bool IsRouteReplayVisualActive()
    {
        return (missionManager != null && missionManager.IsReplayingRoute)
            || routeReplayProgressOverride >= 0f;
    }

    private float CurrentRouteReplayProgress()
    {
        if (routeReplayProgressOverride >= 0f)
        {
            return routeReplayProgressOverride;
        }
        return missionManager != null ? Mathf.Clamp01(missionManager.RouteReplayProgress) : 0f;
    }

    private bool IsRouteReplayLegVisible(RouteReplayLeg leg)
    {
        float progress = CurrentRouteReplayProgress();
        switch (leg)
        {
            case RouteReplayLeg.Uplink:
                return progress < RouteReplayUplinkEnd;
            case RouteReplayLeg.InterSatellite:
                return missionManager != null && missionManager.ShouldUseInterSatelliteLink && progress >= RouteReplayUplinkEnd && progress < RouteReplayInterSatelliteEnd;
            case RouteReplayLeg.Downlink:
                return progress >= (missionManager != null && missionManager.ShouldUseInterSatelliteLink ? RouteReplayInterSatelliteEnd : RouteReplayUplinkEnd);
            default:
                return false;
        }
    }

    private float RouteReplayLegProgress(float replayProgress, RouteReplayLeg leg)
    {
        float start = 0f;
        float end = RouteReplayUplinkEnd;
        switch (leg)
        {
            case RouteReplayLeg.InterSatellite:
                start = RouteReplayUplinkEnd;
                end = RouteReplayInterSatelliteEnd;
                break;
            case RouteReplayLeg.Downlink:
                start = missionManager != null && missionManager.ShouldUseInterSatelliteLink ? RouteReplayInterSatelliteEnd : RouteReplayUplinkEnd;
                end = 1f;
                break;
        }
        return Mathf.Clamp01(Mathf.InverseLerp(start, end, replayProgress));
    }

    private void CreateVisualMaterials()
    {
        targetCoverageMaterial = CreateLineMaterial("Selected Destination Visibility", new Color(0.25f, 0.88f, 1f, 0.28f));
        satelliteCoverageMaterial = CreateLineMaterial("Selected Satellite Footprint", new Color(0.72f, 0.96f, 1f, 0.24f));
        orbitHighlightMaterial = CreateLineMaterial("Next Pass Orbit Highlight", new Color(0.94f, 1f, 1f, 0.94f));
        interSatelliteMaterial = CreateLineMaterial("Inter Satellite Link", InterSatelliteColor(0.86f));
        hintMaterial = CreateLineMaterial("Next Connection Hint", new Color(0.64f, 0.94f, 1f, 0.34f));
        successPulseMaterial = CreateLineMaterial("Delivery Success Pulse", ReceiverColor(1f));
        failurePulseMaterial = CreateLineMaterial("Delivery Failure Pulse", new Color(1f, 0.18f, 0.14f, 0.96f));
        highContrastIdleTargetMaterial = CreateLineMaterial("High Contrast Destination Idle", new Color(1f, 1f, 1f, 0.9f));
        highContrastActiveTargetMaterial = CreateLineMaterial("High Contrast Destination Active", new Color(0.05f, 0.95f, 1f, 1f));
        highContrastStationIdleMaterial = CreateLineMaterial("High Contrast Station Idle", new Color(1f, 0.92f, 0.12f, 0.92f));
        highContrastStationActiveMaterial = CreateLineMaterial("High Contrast Station Active", new Color(1f, 1f, 1f, 1f));
    }

    private void CreateIndependentVisuals()
    {
        targetCoverageLine = CreateVisualLine("SelectedDestinationVisibilityRing", targetCoverageMaterial, true, SurfaceRingPoints, 0.010f);
        satelliteCoverageLine = CreateVisualLine("SelectedSatelliteFootprintRing", satelliteCoverageMaterial, true, SurfaceRingPoints, 0.009f);
        orbitHighlightLine = CreateVisualLine("NextPassOrbitHighlight", orbitHighlightMaterial, false, OrbitArcPoints, 0.018f);
        interSatelliteLine = CreateVisualLine("ISLLeoToDestinationRelayLine", interSatelliteMaterial, false, InterSatelliteArcPoints, 0.026f);
        nextConnectionLine = CreateVisualLine("NextConnectionHintLine", hintMaterial, false, 2, 0.008f);
        originDestinationPreviewLine = CreateVisualLine("OriginDestinationPreviewArc", hintMaterial, false, 28, 0.010f);
        successPulseLine = CreateVisualLine("DeliverySuccessSurfacePulse", successPulseMaterial, true, SurfaceRingPoints, 0.028f);
        failurePulseLine = CreateVisualLine("DeliveryFailureSurfacePulse", failurePulseMaterial, true, SurfaceRingPoints, 0.034f);

        destinationRelayMarker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        destinationRelayMarker.name = "DestinationRelaySatelliteMarker";
        destinationRelayMarker.transform.SetParent(transform, false);
        destinationRelayMarker.GetComponent<Renderer>().sharedMaterial = interSatelliteMaterial;
        Collider destinationRelayCollider = destinationRelayMarker.GetComponent<Collider>();
        if (destinationRelayCollider != null)
        {
            Destroy(destinationRelayCollider);
        }
        destinationRelayMarker.SetActive(false);

        nextConnectionMarker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        nextConnectionMarker.name = "NextConnectionHintMarker";
        nextConnectionMarker.transform.SetParent(transform, false);
        nextConnectionMarker.GetComponent<Renderer>().sharedMaterial = hintMaterial;
        Collider collider = nextConnectionMarker.GetComponent<Collider>();
        if (collider != null)
        {
            Destroy(collider);
        }
        nextConnectionMarker.SetActive(false);
        HideIndependentVisuals();
    }

    private LineRenderer CreateVisualLine(string objectName, Material material, bool loop, int pointCount, float width)
    {
        GameObject lineObject = new GameObject(objectName);
        lineObject.transform.SetParent(transform, false);
        LineRenderer line = lineObject.AddComponent<LineRenderer>();
        line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        line.receiveShadows = false;
        line.useWorldSpace = true;
        line.loop = loop;
        line.alignment = LineAlignment.View;
        line.textureMode = LineTextureMode.Stretch;
        line.numCapVertices = 6;
        line.numCornerVertices = 6;
        line.positionCount = pointCount;
        line.widthMultiplier = width;
        line.sharedMaterial = material;
        line.enabled = false;
        return line;
    }

    private void ApplyPalette(bool force)
    {
        if (!force && lastHighContrastMode == highContrastMode)
        {
            return;
        }

        lastHighContrastMode = highContrastMode;
        ApplyMaterialColor(targetCoverageMaterial, highContrastMode ? new Color(0f, 0.95f, 1f, 0.72f) : new Color(0.2f, 0.82f, 1f, 0.28f));
        ApplyMaterialColor(satelliteCoverageMaterial, highContrastMode ? new Color(1f, 1f, 1f, 0.74f) : new Color(0.72f, 0.96f, 1f, 0.24f));
        ApplyMaterialColor(orbitHighlightMaterial, highContrastMode ? new Color(1f, 1f, 1f, 1f) : new Color(0.94f, 1f, 1f, 0.94f));
        ApplyMaterialColor(interSatelliteMaterial, InterSatelliteColor(highContrastMode ? 1f : 0.86f));
        ApplyMaterialColor(hintMaterial, highContrastMode ? new Color(0.95f, 1f, 1f, 0.76f) : new Color(0.64f, 0.94f, 1f, 0.34f));
        ApplyMaterialColor(successPulseMaterial, ReceiverColor(1f));
        ApplyMaterialColor(failurePulseMaterial, highContrastMode ? new Color(1f, 0.38f, 0.02f, 1f) : new Color(1f, 0.18f, 0.14f, 0.96f));
        ApplyMaterialColor(highContrastIdleTargetMaterial, new Color(1f, 1f, 1f, 0.9f));
        ApplyMaterialColor(highContrastActiveTargetMaterial, new Color(0.05f, 0.95f, 1f, 1f));
        ApplyMaterialColor(highContrastStationIdleMaterial, new Color(1f, 0.92f, 0.12f, 0.92f));
        ApplyMaterialColor(highContrastStationActiveMaterial, new Color(1f, 1f, 1f, 1f));
        ApplyLineRendererColor(scanLine, highContrastMode ? Color.white : new Color(0.38f, 1f, 0.9f, 1f));
        ApplyLineRendererColor(targetCoverageLine, highContrastMode ? new Color(0f, 0.95f, 1f, 0.72f) : new Color(0.2f, 0.82f, 1f, 0.36f));
        ApplyLineRendererColor(satelliteCoverageLine, highContrastMode ? new Color(1f, 1f, 1f, 0.74f) : new Color(0.72f, 0.96f, 1f, 0.32f));
        ApplyLineRendererColor(orbitHighlightLine, Color.white);
        ApplyLineRendererColor(interSatelliteLine, InterSatelliteColor(highContrastMode ? 1f : 0.86f));
        ApplyLineRendererColor(nextConnectionLine, highContrastMode ? new Color(1f, 1f, 1f, 0.76f) : new Color(0.64f, 0.94f, 1f, 0.34f));
        ApplyLineRendererColor(originDestinationPreviewLine, highContrastMode ? new Color(1f, 1f, 1f, 0.78f) : new Color(0.24f, 1f, 0.84f, 0.48f));
        ApplyLineRendererColor(successPulseLine, ReceiverColor(1f));
        ApplyLineRendererColor(failurePulseLine, highContrastMode ? new Color(1f, 0.42f, 0.02f, 1f) : new Color(1f, 0.18f, 0.14f, 1f));

        if (uplinkPackets != null)
        {
            uplinkPackets.highContrastMode = highContrastMode;
        }
        if (interSatellitePackets != null)
        {
            interSatellitePackets.highContrastMode = highContrastMode;
        }
        if (downlinkPackets != null)
        {
            downlinkPackets.highContrastMode = highContrastMode;
        }
    }

    private Material CreateLineMaterial(string materialName, Color color)
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
        if (shader == null)
        {
            shader = Shader.Find("Standard");
        }

        Material material = new Material(shader);
        material.name = materialName;
        ApplyMaterialColor(material, color);
        return material;
    }

    private void ApplyLineRendererColor(LineRenderer line, Color color)
    {
        if (line == null)
        {
            return;
        }

        line.startColor = color;
        line.endColor = color;
    }

    private void ApplyMaterialColor(Material material, Color color)
    {
        if (material == null)
        {
            return;
        }

        if (material.HasProperty("_BaseColor"))
        {
            material.SetColor("_BaseColor", color);
        }
        if (material.HasProperty("_Color"))
        {
            material.SetColor("_Color", color);
        }
        if (material.HasProperty("_EmissionColor"))
        {
            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", color * 1.7f);
        }
    }

    private void DrawLineArc(LineRenderer line, Vector3 start, Vector3 end, int pointCount, float lift)
    {
        if (line == null)
        {
            return;
        }

        int count = Mathf.Max(2, pointCount);
        line.positionCount = count;
        Vector3 center = EarthCenter();
        for (int i = 0; i < count; i++)
        {
            float t = i / (float)(count - 1);
            line.SetPosition(i, EvaluateLineArcPoint(start, end, t, lift));
        }
    }

    private Vector3 EvaluateLineArcPoint(Vector3 start, Vector3 end, float t, float lift)
    {
        Vector3 center = EarthCenter();
        Vector3 position = Vector3.Lerp(start, end, Mathf.Clamp01(t));
        Vector3 liftDirection = position - center;
        if (liftDirection.sqrMagnitude < 0.001f)
        {
            liftDirection = Vector3.up;
        }
        Vector3 arcedPosition = position + liftDirection.normalized * Mathf.Sin(Mathf.Clamp01(t) * Mathf.PI) * lift;
        Vector3 fromCenter = arcedPosition - center;
        float minRadius = EarthRadius() + 0.14f;
        if (fromCenter.magnitude < minRadius && fromCenter.sqrMagnitude > 0.001f)
        {
            arcedPosition = center + fromCenter.normalized * minRadius;
        }
        return arcedPosition;
    }

    private void DrawSurfaceRing(LineRenderer line, Vector3 normal, float halfAngleDegrees, int pointCount)
    {
        if (line == null)
        {
            return;
        }

        line.enabled = true;
        line.loop = true;
        line.positionCount = pointCount;
        Vector3 center = EarthCenter();
        float radius = EarthRadius() + EarthSurfaceOffset;
        normal = normal.sqrMagnitude < 0.001f ? Vector3.up : normal.normalized;
        Vector3 tangent = Vector3.Cross(normal, Vector3.up);
        if (tangent.sqrMagnitude < 0.001f)
        {
            tangent = Vector3.Cross(normal, Vector3.right);
        }
        tangent.Normalize();
        Vector3 bitangent = Vector3.Cross(normal, tangent).normalized;
        float angularRadius = Mathf.Clamp(halfAngleDegrees, 6f, 84f) * Mathf.Deg2Rad;

        for (int i = 0; i < pointCount; i++)
        {
            float angle = (i / (float)pointCount) * Mathf.PI * 2f;
            Vector3 ringDirection = normal * Mathf.Cos(angularRadius)
                + tangent * Mathf.Cos(angle) * Mathf.Sin(angularRadius)
                + bitangent * Mathf.Sin(angle) * Mathf.Sin(angularRadius);
            line.SetPosition(i, center + ringDirection.normalized * radius);
        }
    }

    private void DrawOrbitArc(float startAngle, float endAngle)
    {
        if (orbitHighlightLine == null)
        {
            return;
        }

        orbitHighlightLine.enabled = true;
        orbitHighlightLine.loop = false;
        orbitHighlightLine.positionCount = OrbitArcPoints;
        orbitHighlightLine.widthMultiplier = highContrastMode ? 0.006f : 0.002f;
        ApplyLineRendererColor(orbitHighlightLine, highContrastMode ? new Color(1f, 1f, 1f, 0.25f) : new Color(0.86f, 0.96f, 1f, 0.038f));
        for (int i = 0; i < OrbitArcPoints; i++)
        {
            float t = i / (float)(OrbitArcPoints - 1);
            float angle = Mathf.Lerp(startAngle, endAngle, t);
            Vector3 position = satelliteOrbit.GetOrbitPosition(angle);
            float lift = Mathf.Sin(t * Mathf.PI) * (highContrastMode ? 0.05f : 0.025f);
            Vector3 centerDirection = (position - EarthCenter()).normalized;
            orbitHighlightLine.SetPosition(i, position + centerDirection * lift);
        }
    }

    private void DrawFocusedOrbitPreview(Transform activeSatellite)
    {
        if (orbitHighlightLine == null || activeSatellite == null)
        {
            return;
        }

        if (TryDrawRealSatelliteOrbitPreview(activeSatellite))
        {
            return;
        }

        orbitHighlightLine.enabled = true;
        orbitHighlightLine.loop = false;
        orbitHighlightLine.positionCount = OrbitArcPoints;
        orbitHighlightLine.widthMultiplier = highContrastMode ? 0.006f : 0.0024f;
        ApplyLineRendererColor(orbitHighlightLine, highContrastMode ? new Color(1f, 1f, 1f, 0.28f) : new Color(0.86f, 0.96f, 1f, 0.048f));

        EnterpriseSatelliteAnimator enterpriseOrbit = activeSatellite.GetComponent<EnterpriseSatelliteAnimator>();
        SpaceStationAnimator stationOrbit = activeSatellite.GetComponent<SpaceStationAnimator>();
        float radius = (activeSatellite.position - EarthCenter()).magnitude;
        float inclination = 0f;
        float angle = CurrentSurfaceAngle(activeSatellite.position);
        float direction = 1f;
        if (enterpriseOrbit != null)
        {
            radius = enterpriseOrbit.radius;
            inclination = enterpriseOrbit.inclination;
            angle = enterpriseOrbit.angle;
            direction = Mathf.Sign(Mathf.Approximately(enterpriseOrbit.speed, 0f) ? 1f : enterpriseOrbit.speed);
        }
        else if (stationOrbit != null)
        {
            radius = stationOrbit.orbitRadius;
            inclination = stationOrbit.orbitInclination;
            angle = stationOrbit.angle;
            direction = Mathf.Sign(Mathf.Approximately(stationOrbit.orbitSpeed, 0f) ? 1f : stationOrbit.orbitSpeed);
        }

        Quaternion tilt = Quaternion.Euler(inclination, 0f, 0f);
        float arcDegrees = 118f * direction;
        for (int i = 0; i < OrbitArcPoints; i++)
        {
            float t = i / (float)(OrbitArcPoints - 1);
            float radians = (angle + arcDegrees * t) * Mathf.Deg2Rad;
            Vector3 planePosition = new Vector3(Mathf.Cos(radians) * radius, 0f, Mathf.Sin(radians) * radius);
            Vector3 position = EarthCenter() + tilt * planePosition;
            Vector3 liftDirection = (position - EarthCenter()).normalized;
            orbitHighlightLine.SetPosition(i, position + liftDirection * Mathf.Sin(t * Mathf.PI) * 0.035f);
        }
    }

    private bool TryDrawRealSatelliteOrbitPreview(Transform activeSatellite)
    {
        if (activeSatellite == null)
        {
            return false;
        }
        if (realSatelliteVisualizer == null)
        {
            realSatelliteVisualizer = FindObjectOfType<RealSatelliteVisualizer>();
        }
        if (realSatelliteVisualizer == null || !realSatelliteVisualizer.IsRealSatelliteTransform(activeSatellite))
        {
            return false;
        }
        if (realSatelliteOrbitPreviewSamples == null || realSatelliteOrbitPreviewSamples.Length < OrbitArcPoints)
        {
            realSatelliteOrbitPreviewSamples = new Vector3[OrbitArcPoints];
        }
        if (!realSatelliteVisualizer.TryGetDisplayOrbitSamplesForTransform(activeSatellite, OrbitArcPoints, 75f, realSatelliteOrbitPreviewSamples))
        {
            return false;
        }

        orbitHighlightLine.enabled = true;
        orbitHighlightLine.loop = false;
        orbitHighlightLine.positionCount = OrbitArcPoints;
        orbitHighlightLine.widthMultiplier = highContrastMode ? 0.006f : 0.002f;
        ApplyLineRendererColor(orbitHighlightLine, highContrastMode ? new Color(1f, 1f, 1f, 0.25f) : new Color(0.86f, 0.96f, 1f, 0.035f));
        for (int i = 0; i < OrbitArcPoints; i++)
        {
            orbitHighlightLine.SetPosition(i, realSatelliteOrbitPreviewSamples[i]);
        }
        return true;
    }

    private void UpdateDeliveryPulse(bool hasTarget)
    {
        if (!hasTarget || !IsStatusPulseActive() || missionManager.SelectedTarget == null)
        {
            SetLineEnabled(successPulseLine, false);
            SetLineEnabled(failurePulseLine, false);
            return;
        }

        float remaining = StatusPulseRemaining01();
        float expanding = 1f - remaining;
        Vector3 center = EarthCenter();
        Vector3 targetDirection = (missionManager.SelectedTarget.transform.position - center).normalized;
        if (statusPulseSuccess)
        {
            SetLineEnabled(failurePulseLine, false);
            float flash = 0.92f + Mathf.Sin(Time.time * 12f + scanPulseSeed) * 0.08f;
            DrawSurfaceRing(successPulseLine, targetDirection, Mathf.Lerp(5f, 32f, expanding), SurfaceRingPoints);
            successPulseLine.widthMultiplier = (highContrastMode ? 0.088f : 0.064f) * (0.55f + remaining * 0.9f) * flash;
            ApplyLineRendererColor(successPulseLine, ReceiverColor(Mathf.Lerp(0.18f, 1f, remaining)));
            return;
        }

        SetLineEnabled(successPulseLine, false);
        DrawSurfaceRing(failurePulseLine, targetDirection, Mathf.Lerp(8f, 21f, expanding), SurfaceRingPoints);
        failurePulseLine.widthMultiplier = (highContrastMode ? 0.076f : 0.052f) * (1f + remaining * 0.55f);
        ApplyLineRendererColor(failurePulseLine, FailureReasonColor(Mathf.Lerp(0.32f, 1f, remaining)));
    }

    private Color FailureReasonColor(float alpha)
    {
        string reason = missionManager != null ? missionManager.LastFailureReason : string.Empty;
        if (!string.IsNullOrEmpty(reason))
        {
            if (reason.Contains("혼잡"))
            {
                return highContrastMode ? new Color(1f, 0.42f, 0.02f, alpha) : new Color(1f, 0.24f, 0.08f, alpha);
            }
            if (reason.Contains("연결") || reason.Contains("가시") || reason.Contains("끊김"))
            {
                return highContrastMode ? new Color(1f, 0.25f, 1f, alpha) : new Color(0.86f, 0.26f, 1f, alpha);
            }
            if (reason.Contains("날씨") || reason.Contains("신호") || reason.Contains("공간"))
            {
                return highContrastMode ? new Color(1f, 0.95f, 0.08f, alpha) : new Color(1f, 0.72f, 0.12f, alpha);
            }
        }

        return highContrastMode ? new Color(1f, 0.42f, 0.02f, alpha) : new Color(1f, 0.18f, 0.14f, alpha);
    }

    private bool TryFindNextVisibleOrbitAngle(out float nextAngle, out float secondsUntilVisible)
    {
        nextAngle = satelliteOrbit.CurrentAngle;
        secondsUntilVisible = 0f;
        if (missionManager == null || missionManager.SelectedTarget == null || satelliteOrbit == null)
        {
            return false;
        }

        float direction = satelliteOrbit.orbitSpeed >= 0f ? 1f : -1f;
        float step = 1.5f * direction;
        float threshold = Mathf.Clamp(missionManager.SelectedTarget.observationAngle, 1f, 89f);
        int maxSteps = Mathf.CeilToInt(360f / Mathf.Abs(step));
        for (int i = 0; i <= maxSteps; i++)
        {
            float candidateAngle = satelliteOrbit.CurrentAngle + step * i;
            Vector3 satellitePosition = satelliteOrbit.GetOrbitPosition(candidateAngle);
            float surfaceAngle = SurfaceAngle(satellitePosition, missionManager.SelectedTarget.transform.position);
            if (surfaceAngle <= threshold)
            {
                nextAngle = candidateAngle;
                secondsUntilVisible = Mathf.Abs(candidateAngle - satelliteOrbit.CurrentAngle) / Mathf.Max(1f, Mathf.Abs(satelliteOrbit.orbitSpeed));
                return true;
            }
        }

        return false;
    }

    private float SurfaceAngle(Vector3 satellitePosition, Vector3 targetPosition)
    {
        Vector3 center = EarthCenter();
        Vector3 satelliteDirection = (satellitePosition - center).normalized;
        Vector3 targetDirection = (targetPosition - center).normalized;
        return Vector3.Angle(satelliteDirection, targetDirection);
    }

    private float CurrentSurfaceAngle(Vector3 position)
    {
        Vector3 offset = position - EarthCenter();
        if (offset.sqrMagnitude < 0.001f)
        {
            return 0f;
        }
        return Mathf.Atan2(offset.z, offset.x) * Mathf.Rad2Deg;
    }

    private float SatelliteHorizonAngle(Vector3 satellitePosition)
    {
        float radius = EarthRadius();
        float distance = Mathf.Max(radius + 0.01f, (satellitePosition - EarthCenter()).magnitude);
        return Mathf.Acos(Mathf.Clamp(radius / distance, -1f, 1f)) * Mathf.Rad2Deg;
    }

    private bool IsStatusPulseActive()
    {
        return Time.time < statusPulseEndTime;
    }

    private float StatusPulseRemaining01()
    {
        return Mathf.Clamp01((statusPulseEndTime - Time.time) / StatusPulseSeconds);
    }

    private float StatusPulseScale()
    {
        float remaining = StatusPulseRemaining01();
        return 1f + Mathf.Sin(remaining * Mathf.PI * 7f) * 0.22f + remaining * 0.56f;
    }

    private Material StatusPulseMaterial()
    {
        return statusPulseSuccess ? successPulseMaterial : failurePulseMaterial;
    }

    private Material TargetHaloMaterial(bool active)
    {
        if (highContrastMode)
        {
            return active ? highContrastActiveTargetMaterial : highContrastIdleTargetMaterial;
        }
        return active ? activeTargetMaterial : idleTargetMaterial;
    }

    private Material StationRangeMaterial(bool active)
    {
        if (highContrastMode)
        {
            return active ? highContrastStationActiveMaterial : highContrastStationIdleMaterial;
        }
        return active ? stationActiveMaterial : stationIdleMaterial;
    }

    private Transform ActiveSatelliteTransform()
    {
        return selectedSatelliteTransform != null ? selectedSatelliteTransform : satelliteOrbit.transform;
    }

    private Transform SourceTransform()
    {
        return missionManager != null && missionManager.groundStation != null ? missionManager.groundStation.transform : null;
    }

    private Transform DestinationTransform()
    {
        return missionManager != null && missionManager.SelectedTarget != null ? missionManager.SelectedTarget.transform : null;
    }

    private Transform DestinationRelayTransform(Transform activeSatellite)
    {
        if (missionManager == null || string.IsNullOrEmpty(missionManager.DestinationRelaySatelliteName))
        {
            return null;
        }

        Transform relay = missionManager.DestinationRelayTransform;
        return relay != null && relay != activeSatellite ? relay : null;
    }

    private Vector3 DestinationRelayPosition(Transform destination, Transform activeSatellite)
    {
        if (destination == null)
        {
            return activeSatellite != null ? activeSatellite.position : EarthCenter();
        }

        Vector3 center = EarthCenter();
        Vector3 direction = destination.position - center;
        if (direction.sqrMagnitude < 0.001f)
        {
            return destination.position;
        }

        float activeRadius = activeSatellite != null ? (activeSatellite.position - center).magnitude : 0f;
        float relayRadius = Mathf.Max(EarthRadius() + 0.4f, activeRadius);
        return center + direction.normalized * relayRadius;
    }

    private Vector3 EarthCenter()
    {
        return missionManager != null && missionManager.earth != null ? missionManager.earth.position : Vector3.zero;
    }

    private float EarthRadius()
    {
        if (missionManager == null || missionManager.earth == null)
        {
            return 3f;
        }

        Vector3 scale = missionManager.earth.lossyScale;
        return Mathf.Max(scale.x, Mathf.Max(scale.y, scale.z)) * 0.5f;
    }

    private void SetLineEnabled(LineRenderer line, bool enabled)
    {
        if (line != null)
        {
            line.enabled = enabled;
        }
    }

    private void SetMarkerVisible(bool visible)
    {
        if (nextConnectionMarker != null)
        {
            nextConnectionMarker.SetActive(visible);
        }
    }

    private void HideIndependentVisuals()
    {
        SetLineEnabled(targetCoverageLine, false);
        SetLineEnabled(satelliteCoverageLine, false);
        SetLineEnabled(orbitHighlightLine, false);
        SetLineEnabled(interSatelliteLine, false);
        SetLineEnabled(nextConnectionLine, false);
        SetLineEnabled(originDestinationPreviewLine, false);
        SetLineEnabled(successPulseLine, false);
        SetLineEnabled(failurePulseLine, false);
        if (destinationRelayMarker != null)
        {
            destinationRelayMarker.SetActive(false);
        }
        SetMarkerVisible(false);
    }

    private void HidePacketStreams()
    {
        if (interSatellitePackets != null)
        {
            interSatellitePackets.SetVisible(false);
        }
        if (downlinkPackets != null)
        {
            downlinkPackets.SetVisible(false);
        }
        if (uplinkPackets != null)
        {
            uplinkPackets.SetVisible(false);
        }
    }
}
