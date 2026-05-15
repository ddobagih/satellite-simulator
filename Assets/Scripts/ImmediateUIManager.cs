using UnityEngine;

public class ImmediateUIManager : MonoBehaviour
{
    public bool showFallbackOverlay;
    public SimulationManager simulationManager;
    public SatelliteManager satelliteManager;
    public MissionManager missionManager;
    public CameraManager cameraManager;

    private GUIStyle panelStyle;
    private GUIStyle titleStyle;
    private GUIStyle headingStyle;
    private GUIStyle bodyStyle;
    private GUIStyle smallStyle;
    private GUIStyle buttonStyle;
    private Texture2D panelTexture;

    private void OnGUI()
    {
        if (!showFallbackOverlay)
        {
            return;
        }

        if (simulationManager == null || satelliteManager == null || missionManager == null)
        {
            return;
        }

        EnsureStyles();
        float sx = Screen.width / 1600f;
        float sy = Screen.height / 900f;
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(sx, sy, 1f));

        DrawTopBar();
        DrawSatelliteList();
        DrawDetails();
        DrawMissionControls();
        DrawMissionStatus();
        DrawTimeline();
        DrawMissionIntelligence();
        DrawResultPanel();
        DrawBottomControls();

        GUI.matrix = Matrix4x4.identity;
    }

    private void DrawTopBar()
    {
        GUI.Box(new Rect(0, 0, 1600, 72), GUIContent.none, panelStyle);
        GUI.Label(new Rect(24, 14, 1200, 44), "OrbitShare Relay Twin", titleStyle);
        GUI.Label(new Rect(1310, 18, 240, 38), "Time " + simulationManager.TimeScale.ToString("0.#") + "x", headingStyle);
    }

    private void DrawSatelliteList()
    {
        GUI.Box(new Rect(18, 110, 460, 660), GUIContent.none, panelStyle);
        GUI.Label(new Rect(42, 132, 400, 36), "Relay Control", headingStyle);

        float y = 182;
        foreach (SatelliteData satellite in satelliteManager.Satellites)
        {
            bool selected = satellite == satelliteManager.SelectedSatellite;
            string label = satellite.satelliteName + "  |  " + satellite.OwnerTypeLabel + "\n" + satellite.StateLabel + "  |  " + satellite.usageType;
            Color previous = GUI.backgroundColor;
            GUI.backgroundColor = selected ? new Color(0.16f, 0.45f, 0.72f) : new Color(0.12f, 0.16f, 0.22f);
            if (GUI.Button(new Rect(42, y, 390, 76), label, buttonStyle))
            {
                satelliteManager.SelectSatellite(satellite.satelliteName);
            }
            GUI.backgroundColor = previous;
            y += 92;
        }
    }

    private void DrawDetails()
    {
        GUI.Box(new Rect(1130, 110, 445, 360), GUIContent.none, panelStyle);
        GUI.Label(new Rect(1154, 132, 390, 34), "Selected Relay", headingStyle);

        SatelliteData satellite = satelliteManager.SelectedSatellite;
        if (satellite == null)
        {
            return;
        }

        string detail =
            "Name: " + satellite.satelliteName + "\n" +
            "Type: " + satellite.OwnerTypeLabel + "\n" +
            "Owner: " + satellite.ownerName + "\n" +
            "State: " + satellite.StateLabel + "\n" +
            "Use: " + satellite.usageType + "\n" +
            "Altitude: " + satellite.altitudeKm.ToString("0") + " km\n" +
            "Velocity: " + satellite.speedKmPerSec.ToString("0.0") + " km/s\n" +
            "Battery: " + satellite.batteryPercent.ToString("0") + "%\n" +
            "Buffer: " + satellite.dataStoragePercent.ToString("0") + "%\n" +
            "Destination: " + satellite.currentMissionTarget;
        GUI.Label(new Rect(1154, 176, 390, 260), detail, bodyStyle);
    }

    private void DrawMissionControls()
    {
        GUI.Box(new Rect(1130, 492, 445, 250), GUIContent.none, panelStyle);
        GUI.Label(new Rect(1154, 512, 390, 32), "Relay Request", headingStyle);

        GUI.Label(new Rect(1154, 552, 390, 26), "Destination", smallStyle);
        float x = 1154;
        foreach (ObservationTarget target in missionManager.Targets)
        {
            if (GUI.Button(new Rect(x, 584, 112, 38), target.targetName, buttonStyle))
            {
                missionManager.SelectTarget(target.targetName);
                if (cameraManager != null)
                {
                    cameraManager.missionTarget = missionManager.SelectedTarget.transform;
                }
            }
            x += 122;
        }

        if (GUI.Button(new Rect(1154, 642, 120, 42), "Queue", buttonStyle))
        {
            missionManager.ReserveMission();
        }
        if (GUI.Button(new Rect(1286, 642, 120, 42), "Relay", buttonStyle))
        {
            missionManager.StartMission();
        }
        if (GUI.Button(new Rect(1418, 642, 120, 42), "Quick Relay", buttonStyle))
        {
            missionManager.RunFastDemo();
        }
        if (GUI.Button(new Rect(1154, 694, 120, 34), "Reset", buttonStyle))
        {
            missionManager.ResetMission();
        }

        string hint = satelliteManager.SelectedSatellite != null && satelliteManager.SelectedSatellite.ownerType == SatelliteOwnerType.Shared
            ? "Seoul fixed. Queue one destination."
            : "Select active relay to queue.";
        GUI.Label(new Rect(1290, 692, 248, 44), hint, smallStyle);
    }

    private void DrawMissionStatus()
    {
        GUI.Box(new Rect(520, 535, 550, 170), GUIContent.none, panelStyle);
        GUI.Label(new Rect(544, 552, 500, 30), "Relay Status", headingStyle);
        string status =
            "Phase: " + PhaseLabel(missionManager.Phase) +
            "    Seoul → " + (missionManager.SelectedTarget != null ? missionManager.SelectedTarget.targetName : "None") + "\n" +
            "Relay: " + Mathf.RoundToInt(missionManager.ObservationProgress * 100f) + "%    Delivery: " + Mathf.RoundToInt(missionManager.TransferProgress * 100f) + "%\n" +
            "Link: " + (missionManager.CommunicationAvailable ? "Online" : "Offline") + "\n" +
            missionManager.ResultMessage;
        GUI.Label(new Rect(544, 590, 500, 90), status, bodyStyle);
        DrawBar(new Rect(544, 680, 500, 8), missionManager.ObservationProgress, new Color(1f, 0.68f, 0.12f));
        DrawBar(new Rect(544, 694, 500, 8), missionManager.TransferProgress, new Color(0.24f, 1f, 0.84f));
    }

    private void DrawTimeline()
    {
        GUI.Box(new Rect(520, 400, 550, 100), GUIContent.none, panelStyle);
        string[] labels = { "Queue", "Route", "Uplink", "Relay", "Downlink", "Done" };
        int active = PhaseIndex(missionManager.Phase);
        for (int i = 0; i < labels.Length; i++)
        {
            Rect dot = new Rect(548 + i * 82, 426, 56, 20);
            Color color = i < active ? new Color(0.24f, 1f, 0.84f) : i == active ? new Color(1f, 0.76f, 0.18f) : new Color(0.25f, 0.3f, 0.36f);
            DrawSolid(dot, color);
            GUI.Label(new Rect(524 + i * 82, 456, 100, 24), labels[i], smallStyle);
        }
    }

    private void DrawMissionIntelligence()
    {
        if (missionManager.Phase == MissionPhase.Complete)
        {
            return;
        }

        GUI.Box(new Rect(520, 118, 550, 250), GUIContent.none, panelStyle);
        GUI.Label(new Rect(544, 138, 500, 32), "Relay Ops", headingStyle);

        ObservationTarget target = missionManager.SelectedTarget;
        string targetText = target != null
            ? "Destination: " + target.targetName + " | " + target.priorityLabel + "\n" +
              "Payload: " + target.estimatedDataMb.ToString("0") + " MB\n" +
              "Delay: " + target.resolutionMeters.ToString("0.0") + " s\n" +
              "Relay load: " + target.missionCostCredits + "\n" +
              "Risk: " + target.weatherRiskPercent.ToString("0") + "%"
            : "No destination selected.";
        GUI.Label(new Rect(544, 176, 240, 150), targetText, bodyStyle);

        string opsText =
            "Route ETA: " + missionManager.EstimatedPassSeconds.ToString("0.0") + " s\n" +
            "Gateway ETA: " + missionManager.EstimatedDownlinkSeconds.ToString("0.0") + " s\n" +
            "Route score: " + missionManager.MissionQualityScore + "/100\n" +
            "SLA: " + missionManager.ServiceReliabilityScore + "%";
        GUI.Label(new Rect(806, 176, 240, 120), opsText, bodyStyle);
        DrawBar(new Rect(806, 314, 220, 10), missionManager.MissionQualityScore / 100f, new Color(1f, 0.76f, 0.18f));
        DrawBar(new Rect(806, 336, 220, 10), missionManager.ServiceReliabilityScore / 100f, new Color(0.24f, 1f, 0.84f));
    }

    private void DrawResultPanel()
    {
        if (missionManager.Phase != MissionPhase.Complete)
        {
            return;
        }

        GUI.Box(new Rect(520, 170, 550, 200), GUIContent.none, panelStyle);
        GUI.Label(new Rect(544, 190, 260, 34), "Relay Complete", headingStyle);
        GUI.Label(new Rect(544, 232, 260, 110), missionManager.CompletionSummary, bodyStyle);
        DrawPreview(new Rect(830, 204, 190, 126));
        ObservationTarget target = missionManager.SelectedTarget;
        string dataLabel = target != null
            ? target.estimatedDataMb.ToString("0") + " MB | " + target.resolutionMeters.ToString("0.0") + " s | Done"
            : "Message delivered";
        GUI.Label(new Rect(830, 334, 210, 24), dataLabel, smallStyle);
    }

    private void DrawBottomControls()
    {
        GUI.Box(new Rect(520, 780, 260, 72), GUIContent.none, panelStyle);
        if (GUI.Button(new Rect(535, 800, 54, 34), "Pause", buttonStyle)) simulationManager.SetTimeScale(0f);
        if (GUI.Button(new Rect(596, 800, 44, 34), "1x", buttonStyle)) simulationManager.SetTimeScale(1f);
        if (GUI.Button(new Rect(647, 800, 44, 34), "5x", buttonStyle)) simulationManager.SetTimeScale(5f);
        if (GUI.Button(new Rect(698, 800, 56, 34), "10x", buttonStyle)) simulationManager.SetTimeScale(10f);

        GUI.Box(new Rect(800, 780, 310, 72), GUIContent.none, panelStyle);
        if (GUI.Button(new Rect(815, 800, 82, 34), "View", buttonStyle)) cameraManager.SetOverview();
        if (GUI.Button(new Rect(904, 800, 60, 34), "Earth", buttonStyle)) cameraManager.SetEarth();
        if (GUI.Button(new Rect(971, 800, 82, 34), "Sat", buttonStyle)) cameraManager.SetSatellite();
        if (GUI.Button(new Rect(1060, 800, 60, 34), "Route", buttonStyle)) cameraManager.SetMission();
    }

    private void DrawBar(Rect rect, float value, Color color)
    {
        DrawSolid(rect, new Color(1f, 1f, 1f, 0.12f));
        DrawSolid(new Rect(rect.x, rect.y, rect.width * Mathf.Clamp01(value), rect.height), color);
    }

    private void DrawPreview(Rect rect)
    {
        DrawSolid(rect, new Color(0.03f, 0.12f, 0.18f));
        for (int i = 0; i < 8; i++)
        {
            DrawSolid(new Rect(rect.x + i * 24, rect.y, 1, rect.height), new Color(0.24f, 1f, 0.84f, 0.35f));
            DrawSolid(new Rect(rect.x, rect.y + i * 16, rect.width, 1), new Color(0.24f, 1f, 0.84f, 0.25f));
        }
        DrawSolid(new Rect(rect.x + 46, rect.y + 38, 98, 38), new Color(0.8f, 0.9f, 0.92f, 0.45f));
    }

    private void DrawSolid(Rect rect, Color color)
    {
        Color previous = GUI.color;
        GUI.color = color;
        GUI.DrawTexture(rect, Texture2D.whiteTexture);
        GUI.color = previous;
    }

    private void EnsureStyles()
    {
        if (panelStyle != null)
        {
            return;
        }

        panelTexture = new Texture2D(1, 1);
        panelTexture.SetPixel(0, 0, new Color(0.025f, 0.055f, 0.085f, 0.68f));
        panelTexture.Apply();

        panelStyle = new GUIStyle(GUI.skin.box);
        panelStyle.normal.background = panelTexture;

        titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 30, fontStyle = FontStyle.Bold, normal = { textColor = Color.white } };
        headingStyle = new GUIStyle(GUI.skin.label) { fontSize = 22, fontStyle = FontStyle.Bold, normal = { textColor = Color.white } };
        bodyStyle = new GUIStyle(GUI.skin.label) { fontSize = 17, normal = { textColor = Color.white }, wordWrap = true };
        smallStyle = new GUIStyle(GUI.skin.label) { fontSize = 14, normal = { textColor = new Color(0.86f, 0.93f, 1f) }, wordWrap = true, alignment = TextAnchor.MiddleCenter };
        buttonStyle = new GUIStyle(GUI.skin.button) { fontSize = 15, fontStyle = FontStyle.Bold, wordWrap = true };
    }

    private string PhaseLabel(MissionPhase phase)
    {
        switch (phase)
        {
            case MissionPhase.Reserved: return "Queued";
            case MissionPhase.Running: return "Routing";
            case MissionPhase.Observing: return "Uplink";
            case MissionPhase.ObservationComplete: return "Relay Locked";
            case MissionPhase.Transmitting: return "Delivering";
            case MissionPhase.Complete: return "Complete";
            default: return "Idle";
        }
    }

    private int PhaseIndex(MissionPhase phase)
    {
        switch (phase)
        {
            case MissionPhase.Reserved: return 0;
            case MissionPhase.Running: return 1;
            case MissionPhase.Observing: return 2;
            case MissionPhase.ObservationComplete: return 3;
            case MissionPhase.Transmitting: return 4;
            case MissionPhase.Complete: return 5;
            default: return -1;
        }
    }
}
