using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private SimulationManager simulationManager;
    private SatelliteManager satelliteManager;
    private MissionManager missionManager;
    private CameraManager cameraManager;
    private Font uiFont;

    private RectTransform satelliteListContent;
    private Text detailText;
    private Text missionStatusText;
    private Text titleText;
    private Dropdown targetDropdown;
    private Button reserveButton;
    private Button startMissionButton;
    private Button resetButton;
    private Button fastDemoButton;
    private Text reservationHintText;
    private Image observationFill;
    private Image transferFill;
    private RectTransform resultPanel;
    private Text resultText;
    private Text resultDataText;
    private RawImage previewImage;
    private readonly List<Image> timelineDots = new List<Image>();
    private readonly List<Text> timelineLabels = new List<Text>();

    private readonly Dictionary<string, Button> satelliteButtons = new Dictionary<string, Button>();

    public void BuildUI(
        SimulationManager simulation,
        SatelliteManager satellites,
        MissionManager mission,
        CameraManager cameras)
    {
        simulationManager = simulation;
        satelliteManager = satellites;
        missionManager = mission;
        cameraManager = cameras;
        uiFont = CreateUIFont();
        Canvas canvas = CreateCanvas();
        CreateTopPanel(canvas.transform);
        CreateSatelliteListPanel(canvas.transform);
        CreateDetailPanel(canvas.transform);
        CreateReservationPanel(canvas.transform);
        CreateTimeControlPanel(canvas.transform);
        CreateCameraControlPanel(canvas.transform);
        CreateTimelinePanel(canvas.transform);
        CreateResultPanel(canvas.transform);

        satelliteManager.OnDataChanged += RefreshAll;
        satelliteManager.OnSelectionChanged += delegate { RefreshAll(); };
        missionManager.OnMissionChanged += RefreshMissionPanel;
        simulationManager.OnTimeScaleChanged += RefreshAll;

        RefreshSatelliteList();
        RefreshAll();
    }

    private Canvas CreateCanvas()
    {
        GameObject canvasObject = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1600f, 900f);
        scaler.matchWidthOrHeight = 0.5f;
        return canvas;
    }

    private void CreateTopPanel(Transform parent)
    {
        RectTransform panel = CreatePanel(parent, "TopBar", new Vector2(0f, 0.91f), new Vector2(1f, 1f), new Color(0.02f, 0.05f, 0.09f, 0.88f));
        titleText = CreateText(panel, "OrbitShare Digital Twin | Shared Satellite Mission Platform", 30, TextAnchor.MiddleLeft);
        titleText.rectTransform.offsetMin = new Vector2(24f, 0f);
        titleText.rectTransform.offsetMax = new Vector2(-24f, 0f);
    }

    private void CreateSatelliteListPanel(Transform parent)
    {
        RectTransform panel = CreatePanel(parent, "SatelliteListPanel", new Vector2(0.015f, 0.16f), new Vector2(0.31f, 0.89f), new Color(0.04f, 0.07f, 0.11f, 0.9f));
        Text heading = CreateText(panel, "Satellite Fleet", 23, TextAnchor.UpperLeft);
        heading.rectTransform.anchorMin = new Vector2(0f, 1f);
        heading.rectTransform.anchorMax = new Vector2(1f, 1f);
        heading.rectTransform.pivot = new Vector2(0.5f, 1f);
        heading.rectTransform.anchoredPosition = new Vector2(0f, -18f);
        heading.rectTransform.sizeDelta = new Vector2(-28f, 36f);

        GameObject content = new GameObject("ListContent", typeof(RectTransform), typeof(VerticalLayoutGroup));
        content.transform.SetParent(panel, false);
        satelliteListContent = content.GetComponent<RectTransform>();
        satelliteListContent.anchorMin = new Vector2(0f, 0f);
        satelliteListContent.anchorMax = new Vector2(1f, 1f);
        satelliteListContent.offsetMin = new Vector2(14f, 14f);
        satelliteListContent.offsetMax = new Vector2(-14f, -62f);

        VerticalLayoutGroup layout = content.GetComponent<VerticalLayoutGroup>();
        layout.spacing = 10f;
        layout.childControlHeight = true;
        layout.childControlWidth = true;
        layout.childForceExpandHeight = false;
    }

    private void CreateDetailPanel(Transform parent)
    {
        RectTransform panel = CreatePanel(parent, "SatelliteDetailPanel", new Vector2(0.69f, 0.47f), new Vector2(0.985f, 0.89f), new Color(0.04f, 0.07f, 0.11f, 0.9f));
        Text heading = CreateText(panel, "Satellite Details", 23, TextAnchor.UpperLeft);
        heading.rectTransform.anchorMin = new Vector2(0f, 1f);
        heading.rectTransform.anchorMax = new Vector2(1f, 1f);
        heading.rectTransform.pivot = new Vector2(0.5f, 1f);
        heading.rectTransform.anchoredPosition = new Vector2(0f, -18f);
        heading.rectTransform.sizeDelta = new Vector2(-28f, 36f);

        detailText = CreateText(panel, string.Empty, 19, TextAnchor.UpperLeft);
        detailText.lineSpacing = 1.08f;
        detailText.rectTransform.offsetMin = new Vector2(18f, 18f);
        detailText.rectTransform.offsetMax = new Vector2(-18f, -64f);
    }

    private void CreateReservationPanel(Transform parent)
    {
        RectTransform panel = CreatePanel(parent, "MissionReservationPanel", new Vector2(0.69f, 0.16f), new Vector2(0.985f, 0.45f), new Color(0.04f, 0.07f, 0.11f, 0.9f));
        Text heading = CreateText(panel, "Mission Reservation", 23, TextAnchor.UpperLeft);
        heading.rectTransform.anchorMin = new Vector2(0f, 1f);
        heading.rectTransform.anchorMax = new Vector2(1f, 1f);
        heading.rectTransform.pivot = new Vector2(0.5f, 1f);
        heading.rectTransform.anchoredPosition = new Vector2(0f, -16f);
        heading.rectTransform.sizeDelta = new Vector2(-28f, 34f);

        targetDropdown = CreateDropdown(panel);
        targetDropdown.onValueChanged.AddListener(delegate
        {
            if (missionManager.Targets.Count > targetDropdown.value)
            {
                missionManager.SelectTarget(missionManager.Targets[targetDropdown.value].targetName);
                if (cameraManager != null)
                {
                    cameraManager.missionTarget = missionManager.SelectedTarget.transform;
                }
            }
        });

        reserveButton = CreateButton(panel, "Reserve", new Vector2(0f, 0f), new Vector2(0.32f, 0f), new Vector2(18f, 18f), new Vector2(-6f, 64f), delegate { missionManager.ReserveMission(); });
        startMissionButton = CreateButton(panel, "Start", new Vector2(0.34f, 0f), new Vector2(0.66f, 0f), new Vector2(0f, 18f), new Vector2(0f, 64f), delegate { missionManager.StartMission(); });
        resetButton = CreateButton(panel, "Reset", new Vector2(0.68f, 0f), new Vector2(1f, 0f), new Vector2(6f, 18f), new Vector2(-18f, 64f), delegate { missionManager.ResetMission(); });
        fastDemoButton = CreateButton(panel, "Fast Demo", new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(18f, 78f), new Vector2(-18f, 118f), delegate { missionManager.RunFastDemo(); });

        reservationHintText = CreateText(panel, string.Empty, 15, TextAnchor.UpperLeft);
        reservationHintText.rectTransform.anchorMin = new Vector2(0f, 0f);
        reservationHintText.rectTransform.anchorMax = new Vector2(1f, 0f);
        reservationHintText.rectTransform.offsetMin = new Vector2(18f, 122f);
        reservationHintText.rectTransform.offsetMax = new Vector2(-18f, 150f);

        PopulateTargetDropdown();
    }

    private void CreateTimeControlPanel(Transform parent)
    {
        RectTransform panel = CreatePanel(parent, "TimeControlPanel", new Vector2(0.325f, 0.02f), new Vector2(0.515f, 0.14f), new Color(0.04f, 0.07f, 0.11f, 0.86f));
        Text heading = CreateText(panel, "Time", 18, TextAnchor.UpperCenter);
        heading.rectTransform.anchorMin = new Vector2(0f, 1f);
        heading.rectTransform.anchorMax = new Vector2(1f, 1f);
        heading.rectTransform.pivot = new Vector2(0.5f, 1f);
        heading.rectTransform.anchoredPosition = new Vector2(0f, -8f);
        heading.rectTransform.sizeDelta = new Vector2(-18f, 28f);

        CreateButton(panel, "Pause", new Vector2(0f, 0f), new Vector2(0.25f, 0f), new Vector2(10f, 12f), new Vector2(-4f, 50f), delegate { simulationManager.SetTimeScale(0f); });
        CreateButton(panel, "1x", new Vector2(0.25f, 0f), new Vector2(0.5f, 0f), new Vector2(4f, 12f), new Vector2(-4f, 50f), delegate { simulationManager.SetTimeScale(1f); });
        CreateButton(panel, "5x", new Vector2(0.5f, 0f), new Vector2(0.75f, 0f), new Vector2(4f, 12f), new Vector2(-4f, 50f), delegate { simulationManager.SetTimeScale(5f); });
        CreateButton(panel, "10x", new Vector2(0.75f, 0f), new Vector2(1f, 0f), new Vector2(4f, 12f), new Vector2(-10f, 50f), delegate { simulationManager.SetTimeScale(10f); });
    }

    private void CreateCameraControlPanel(Transform parent)
    {
        RectTransform panel = CreatePanel(parent, "CameraControlPanel", new Vector2(0.525f, 0.02f), new Vector2(0.675f, 0.14f), new Color(0.04f, 0.07f, 0.11f, 0.86f));
        Text heading = CreateText(panel, "Camera", 18, TextAnchor.UpperCenter);
        heading.rectTransform.anchorMin = new Vector2(0f, 1f);
        heading.rectTransform.anchorMax = new Vector2(1f, 1f);
        heading.rectTransform.pivot = new Vector2(0.5f, 1f);
        heading.rectTransform.anchoredPosition = new Vector2(0f, -8f);
        heading.rectTransform.sizeDelta = new Vector2(-18f, 28f);

        CreateButton(panel, "Overview", new Vector2(0f, 0f), new Vector2(0.28f, 0f), new Vector2(10f, 12f), new Vector2(-4f, 50f), delegate { cameraManager.SetOverview(); });
        CreateButton(panel, "Earth", new Vector2(0.28f, 0f), new Vector2(0.48f, 0f), new Vector2(4f, 12f), new Vector2(-4f, 50f), delegate { cameraManager.SetEarth(); });
        CreateButton(panel, "Satellite", new Vector2(0.48f, 0f), new Vector2(0.74f, 0f), new Vector2(4f, 12f), new Vector2(-4f, 50f), delegate { cameraManager.SetSatellite(); });
        CreateButton(panel, "Mission", new Vector2(0.74f, 0f), new Vector2(1f, 0f), new Vector2(4f, 12f), new Vector2(-10f, 50f), delegate { cameraManager.SetMission(); });

        RectTransform statusPanel = CreatePanel(parent, "MissionStatusPanel", new Vector2(0.325f, 0.16f), new Vector2(0.675f, 0.36f), new Color(0.04f, 0.07f, 0.11f, 0.88f));
        missionStatusText = CreateText(statusPanel, string.Empty, 18, TextAnchor.UpperLeft);
        missionStatusText.rectTransform.offsetMin = new Vector2(18f, 62f);
        missionStatusText.rectTransform.offsetMax = new Vector2(-18f, -14f);

        observationFill = CreateProgressBar(statusPanel, "ObservationProgress", new Vector2(18f, 36f), new Color(1f, 0.68f, 0.12f, 0.95f));
        transferFill = CreateProgressBar(statusPanel, "TransferProgress", new Vector2(18f, 14f), new Color(0.24f, 1f, 0.84f, 0.95f));
    }

    private void CreateResultPanel(Transform parent)
    {
        resultPanel = CreatePanel(parent, "MissionResultPanel", new Vector2(0.325f, 0.37f), new Vector2(0.675f, 0.66f), new Color(0.02f, 0.08f, 0.12f, 0.93f));
        resultText = CreateText(resultPanel, string.Empty, 22, TextAnchor.UpperLeft);
        resultText.rectTransform.anchorMin = new Vector2(0f, 0.45f);
        resultText.rectTransform.anchorMax = new Vector2(0.58f, 1f);
        resultText.rectTransform.offsetMin = new Vector2(20f, 12f);
        resultText.rectTransform.offsetMax = new Vector2(-8f, -14f);

        resultDataText = CreateText(resultPanel, string.Empty, 16, TextAnchor.UpperLeft);
        resultDataText.rectTransform.anchorMin = new Vector2(0f, 0f);
        resultDataText.rectTransform.anchorMax = new Vector2(0.58f, 0.45f);
        resultDataText.rectTransform.offsetMin = new Vector2(20f, 16f);
        resultDataText.rectTransform.offsetMax = new Vector2(-8f, -4f);

        GameObject previewObject = new GameObject("ObservationPreview", typeof(RectTransform), typeof(RawImage));
        previewObject.transform.SetParent(resultPanel, false);
        RectTransform previewRect = previewObject.GetComponent<RectTransform>();
        previewRect.anchorMin = new Vector2(0.62f, 0.12f);
        previewRect.anchorMax = new Vector2(0.96f, 0.88f);
        previewRect.offsetMin = Vector2.zero;
        previewRect.offsetMax = Vector2.zero;
        previewImage = previewObject.GetComponent<RawImage>();
        previewImage.color = Color.white;
        resultPanel.gameObject.SetActive(false);
    }

    private void CreateTimelinePanel(Transform parent)
    {
        RectTransform panel = CreatePanel(parent, "MissionTimelinePanel", new Vector2(0.325f, 0.56f), new Vector2(0.675f, 0.66f), new Color(0.025f, 0.055f, 0.085f, 0.88f));
        string[] labels = { "Reserve", "Orbit", "Observe", "Store", "Transmit", "Done" };
        timelineDots.Clear();
        timelineLabels.Clear();

        for (int i = 0; i < labels.Length; i++)
        {
            float left = i / (float)labels.Length;
            float right = (i + 1f) / labels.Length;

            GameObject dotObject = new GameObject(labels[i] + "Dot", typeof(RectTransform), typeof(Image));
            dotObject.transform.SetParent(panel, false);
            RectTransform dotRect = dotObject.GetComponent<RectTransform>();
            dotRect.anchorMin = new Vector2(left + 0.045f, 0.46f);
            dotRect.anchorMax = new Vector2(right - 0.045f, 0.78f);
            dotRect.offsetMin = Vector2.zero;
            dotRect.offsetMax = Vector2.zero;
            Image dot = dotObject.GetComponent<Image>();
            dot.color = new Color(1f, 1f, 1f, 0.16f);
            timelineDots.Add(dot);

            Text label = CreateText(panel, labels[i], 14, TextAnchor.MiddleCenter);
            label.rectTransform.anchorMin = new Vector2(left, 0.04f);
            label.rectTransform.anchorMax = new Vector2(right, 0.42f);
            label.rectTransform.offsetMin = Vector2.zero;
            label.rectTransform.offsetMax = Vector2.zero;
            timelineLabels.Add(label);
        }
    }

    private void RefreshAll()
    {
        RefreshSatelliteList();
        RefreshDetailPanel();
        RefreshMissionPanel();
    }

    private void RefreshSatelliteList()
    {
        if (satelliteListContent == null || satelliteManager == null)
        {
            return;
        }

        foreach (Transform child in satelliteListContent)
        {
            Destroy(child.gameObject);
        }
        satelliteButtons.Clear();

        foreach (SatelliteData satellite in satelliteManager.Satellites)
        {
            string satelliteName = satellite.satelliteName;
            Button button = CreateListButton(satelliteListContent, FormatSatelliteRow(satellite), delegate { satelliteManager.SelectSatellite(satelliteName); });
            ColorBlock colors = button.colors;
            colors.normalColor = satellite == satelliteManager.SelectedSatellite ? new Color(0.12f, 0.38f, 0.68f, 0.96f) : new Color(0.1f, 0.14f, 0.2f, 0.96f);
            colors.highlightedColor = new Color(0.16f, 0.44f, 0.74f, 1f);
            button.colors = colors;
            satelliteButtons[satelliteName] = button;
        }
    }

    private void RefreshDetailPanel()
    {
        if (detailText == null || satelliteManager.SelectedSatellite == null)
        {
            return;
        }

        SatelliteData satellite = satelliteManager.SelectedSatellite;
        detailText.text =
            "Name: " + satellite.satelliteName + "\n" +
            "Type: " + satellite.OwnerTypeLabel + "\n" +
            "Owner: " + satellite.ownerName + "\n" +
            "State: " + satellite.StateLabel + "\n" +
            "Use: " + satellite.usageType + "\n" +
            "Altitude: " + satellite.altitudeKm.ToString("0") + " km\n" +
            "Velocity: " + satellite.speedKmPerSec.ToString("0.0") + " km/s\n" +
            "Battery: " + satellite.batteryPercent.ToString("0") + "%\n" +
            "Data: " + satellite.dataStoragePercent.ToString("0") + "%\n" +
            "Mission: " + satellite.currentMissionTarget;
    }

    private void RefreshMissionPanel()
    {
        if (missionStatusText == null || missionManager == null)
        {
            return;
        }

        if (titleText != null && simulationManager != null)
        {
            titleText.text = "OrbitShare Digital Twin | Shared Satellite Mission Platform    |    " + simulationManager.TimeScale.ToString("0.#") + "x";
        }

        missionStatusText.text =
            "Phase: " + MissionPhaseLabel(missionManager.Phase) + "\n" +
            "Target: " + (missionManager.SelectedTarget != null ? missionManager.SelectedTarget.targetName : "None") + "\n" +
            "Observe/Transfer: " + Mathf.RoundToInt(missionManager.ObservationProgress * 100f) + "% / " + Mathf.RoundToInt(missionManager.TransferProgress * 100f) + "%\n" +
            "Communication: " + (missionManager.CommunicationAvailable ? "Available" : "Unavailable") + "\n" +
            "ETA: pass " + missionManager.EstimatedPassSeconds.ToString("0.0") + "s / downlink " + missionManager.EstimatedDownlinkSeconds.ToString("0.0") + "s\n" +
            "Quality/SLA: " + missionManager.MissionQualityScore + "/100 / " + missionManager.ServiceReliabilityScore + "%\n" +
            "Result: " + missionManager.ResultMessage;

        if (observationFill != null)
        {
            observationFill.fillAmount = missionManager.ObservationProgress;
        }
        if (transferFill != null)
        {
            transferFill.fillAmount = missionManager.TransferProgress;
        }

        if (resultPanel != null && resultText != null)
        {
            bool showResult = missionManager.Phase == MissionPhase.Complete;
            resultPanel.gameObject.SetActive(showResult);
            resultText.text = missionManager.CompletionSummary;
            if (showResult)
            {
                string targetName = missionManager.SelectedTarget != null ? missionManager.SelectedTarget.targetName : "Unknown";
                ObservationTarget target = missionManager.SelectedTarget;
                resultDataText.text =
                    "Mission ID: OS-" + System.DateTime.Now.ToString("HHmm") + "\n" +
                    "Data Size: " + (target != null ? target.estimatedDataMb.ToString("0") : "428") + " MB\n" +
                    "Resolution: " + (target != null ? target.resolutionMeters.ToString("0.0") : "1.2") + " m/pixel\n" +
                    "Quality: " + missionManager.MissionQualityScore + "/100\n" +
                    "Ground Station: Daejeon\n" +
                    "Delivery: Complete";
                if (previewImage.texture == null || previewImage.texture.name != targetName + " Preview")
                {
                    previewImage.texture = CreatePreviewTexture(targetName);
                }
            }
        }

        RefreshTimeline();

        if (reserveButton != null)
        {
            reserveButton.interactable = satelliteManager != null && satelliteManager.CanReserveSelected() && missionManager.Phase == MissionPhase.Idle;
        }
        if (startMissionButton != null)
        {
            startMissionButton.interactable = missionManager.Phase == MissionPhase.Reserved;
        }
        if (resetButton != null)
        {
            resetButton.interactable = missionManager.Phase == MissionPhase.Complete || missionManager.Phase == MissionPhase.Reserved;
        }
        if (targetDropdown != null)
        {
            targetDropdown.interactable = missionManager.Phase == MissionPhase.Idle;
        }
        if (fastDemoButton != null)
        {
            fastDemoButton.interactable = missionManager.Phase != MissionPhase.Complete;
        }
        if (reservationHintText != null)
        {
            SatelliteData selected = satelliteManager != null ? satelliteManager.SelectedSatellite : null;
            if (selected == null)
            {
                reservationHintText.text = string.Empty;
            }
            else if (selected.ownerType != SatelliteOwnerType.Shared)
            {
                reservationHintText.text = "Only shared satellites can be reserved.";
            }
            else if (selected.availabilityState != SatelliteAvailabilityState.Available && missionManager.Phase == MissionPhase.Idle)
            {
                reservationHintText.text = "The shared satellite is not currently rentable.";
            }
            else
            {
                reservationHintText.text = "Reserve SAT-S01 to start an observation mission.";
            }
        }
    }

    private void RefreshTimeline()
    {
        if (timelineDots.Count == 0)
        {
            return;
        }

        int activeIndex = PhaseToTimelineIndex(missionManager.Phase);
        for (int i = 0; i < timelineDots.Count; i++)
        {
            bool complete = i < activeIndex;
            bool active = i == activeIndex;
            timelineDots[i].color = complete
                ? new Color(0.24f, 1f, 0.84f, 0.95f)
                : active ? new Color(1f, 0.76f, 0.18f, 0.95f) : new Color(1f, 1f, 1f, 0.14f);
            if (i < timelineLabels.Count)
            {
                timelineLabels[i].color = complete || active ? Color.white : new Color(1f, 1f, 1f, 0.48f);
            }
        }
    }

    private int PhaseToTimelineIndex(MissionPhase phase)
    {
        switch (phase)
        {
            case MissionPhase.Reserved:
                return 0;
            case MissionPhase.Running:
                return 1;
            case MissionPhase.Observing:
                return 2;
            case MissionPhase.ObservationComplete:
                return 3;
            case MissionPhase.Transmitting:
                return 4;
            case MissionPhase.Complete:
                return 5;
            default:
                return -1;
        }
    }

    private void PopulateTargetDropdown()
    {
        if (targetDropdown == null || missionManager == null)
        {
            return;
        }

        targetDropdown.ClearOptions();
        List<string> options = new List<string>();
        foreach (ObservationTarget target in missionManager.Targets)
        {
            options.Add(target.targetName);
        }
        targetDropdown.AddOptions(options);
    }

    private RectTransform CreatePanel(Transform parent, string objectName, Vector2 anchorMin, Vector2 anchorMax, Color color)
    {
        GameObject panelObject = new GameObject(objectName, typeof(RectTransform), typeof(Image));
        panelObject.transform.SetParent(parent, false);
        RectTransform rect = panelObject.GetComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        Image image = panelObject.GetComponent<Image>();
        image.color = color;
        return rect;
    }

    private Text CreateText(Transform parent, string content, int size, TextAnchor anchor)
    {
        GameObject textObject = new GameObject("Text", typeof(RectTransform), typeof(Text));
        textObject.transform.SetParent(parent, false);
        RectTransform rect = textObject.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Text text = textObject.GetComponent<Text>();
        if (uiFont != null)
        {
            text.font = uiFont;
        }
        text.text = content;
        text.fontSize = size;
        text.alignment = anchor;
        text.color = Color.white;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        return text;
    }

    private Font CreateUIFont()
    {
        string[] fontPaths =
        {
            "/usr/share/fonts/truetype/noto/NotoSans-Regular.ttf",
            "/usr/share/fonts/opentype/noto/NotoSansCJK-Regular.ttc",
            "/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf"
        };

        foreach (string path in fontPaths)
        {
            if (!System.IO.File.Exists(path))
            {
                continue;
            }

            Font font = new Font(path);
            if (font != null)
            {
                return font;
            }
        }

        return Resources.GetBuiltinResource<Font>("Arial.ttf");
    }

    private Button CreateListButton(Transform parent, string label, UnityAction action)
    {
        GameObject buttonObject = new GameObject("SatelliteButton", typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement));
        buttonObject.transform.SetParent(parent, false);
        LayoutElement layoutElement = buttonObject.GetComponent<LayoutElement>();
        layoutElement.preferredHeight = 82f;
        Button button = buttonObject.GetComponent<Button>();
        button.onClick.AddListener(action);

        Image image = buttonObject.GetComponent<Image>();
        image.color = new Color(0.1f, 0.14f, 0.2f, 0.96f);

        Text text = CreateText(buttonObject.transform, label, 17, TextAnchor.MiddleLeft);
        text.rectTransform.offsetMin = new Vector2(12f, 6f);
        text.rectTransform.offsetMax = new Vector2(-12f, -6f);
        return button;
    }

    private Button CreateButton(Transform parent, string label, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax, UnityAction action)
    {
        GameObject buttonObject = new GameObject(label + "Button", typeof(RectTransform), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(parent, false);
        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = new Vector2(0.5f, 0f);
        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;

        Image image = buttonObject.GetComponent<Image>();
        image.color = new Color(0.1f, 0.34f, 0.56f, 0.96f);

        Button button = buttonObject.GetComponent<Button>();
        button.onClick.AddListener(action);

        Text text = CreateText(buttonObject.transform, label, 17, TextAnchor.MiddleCenter);
        text.rectTransform.offsetMin = Vector2.zero;
        text.rectTransform.offsetMax = Vector2.zero;
        return button;
    }

    private Image CreateProgressBar(Transform parent, string objectName, Vector2 offset, Color fillColor)
    {
        GameObject root = new GameObject(objectName, typeof(RectTransform), typeof(Image));
        root.transform.SetParent(parent, false);
        RectTransform rootRect = root.GetComponent<RectTransform>();
        rootRect.anchorMin = new Vector2(0f, 0f);
        rootRect.anchorMax = new Vector2(1f, 0f);
        rootRect.offsetMin = new Vector2(offset.x, offset.y);
        rootRect.offsetMax = new Vector2(-offset.x, offset.y + 12f);
        root.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.09f);

        GameObject fillObject = new GameObject("Fill", typeof(RectTransform), typeof(Image));
        fillObject.transform.SetParent(root.transform, false);
        RectTransform fillRect = fillObject.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        Image fill = fillObject.GetComponent<Image>();
        fill.color = fillColor;
        fill.type = Image.Type.Filled;
        fill.fillMethod = Image.FillMethod.Horizontal;
        fill.fillAmount = 0f;
        return fill;
    }

    private Dropdown CreateDropdown(Transform parent)
    {
        GameObject dropdownObject = new GameObject("TargetDropdown", typeof(RectTransform), typeof(Image), typeof(Dropdown));
        dropdownObject.transform.SetParent(parent, false);
        RectTransform rect = dropdownObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2(0f, -62f);
        rect.sizeDelta = new Vector2(-36f, 42f);
        dropdownObject.GetComponent<Image>().color = new Color(0.08f, 0.11f, 0.16f, 0.98f);

        Dropdown dropdown = dropdownObject.GetComponent<Dropdown>();
        Text label = CreateText(dropdownObject.transform, string.Empty, 18, TextAnchor.MiddleLeft);
        label.rectTransform.offsetMin = new Vector2(12f, 0f);
        label.rectTransform.offsetMax = new Vector2(-34f, 0f);
        dropdown.captionText = label;

        GameObject templateObject = new GameObject("Template", typeof(RectTransform), typeof(Image), typeof(ScrollRect));
        templateObject.transform.SetParent(dropdownObject.transform, false);
        RectTransform templateRect = templateObject.GetComponent<RectTransform>();
        templateRect.anchorMin = new Vector2(0f, 0f);
        templateRect.anchorMax = new Vector2(1f, 0f);
        templateRect.pivot = new Vector2(0.5f, 1f);
        templateRect.anchoredPosition = new Vector2(0f, 0f);
        templateRect.sizeDelta = new Vector2(0f, 120f);
        templateObject.GetComponent<Image>().color = new Color(0.08f, 0.11f, 0.16f, 0.98f);
        templateObject.SetActive(false);

        GameObject viewportObject = new GameObject("Viewport", typeof(RectTransform), typeof(Mask), typeof(Image));
        viewportObject.transform.SetParent(templateObject.transform, false);
        RectTransform viewportRect = viewportObject.GetComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.offsetMin = Vector2.zero;
        viewportRect.offsetMax = Vector2.zero;
        viewportObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.04f);
        viewportObject.GetComponent<Mask>().showMaskGraphic = false;

        GameObject contentObject = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup));
        contentObject.transform.SetParent(viewportObject.transform, false);
        RectTransform contentRect = contentObject.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0f, 1f);
        contentRect.anchorMax = new Vector2(1f, 1f);
        contentRect.pivot = new Vector2(0.5f, 1f);
        contentRect.sizeDelta = new Vector2(0f, 120f);

        GameObject itemObject = new GameObject("Item", typeof(RectTransform), typeof(Toggle), typeof(Image));
        itemObject.transform.SetParent(contentObject.transform, false);
        RectTransform itemRect = itemObject.GetComponent<RectTransform>();
        itemRect.sizeDelta = new Vector2(0f, 36f);
        Text itemLabel = CreateText(itemObject.transform, string.Empty, 17, TextAnchor.MiddleLeft);
        itemLabel.rectTransform.offsetMin = new Vector2(12f, 0f);
        itemLabel.rectTransform.offsetMax = new Vector2(-12f, 0f);

        Toggle itemToggle = itemObject.GetComponent<Toggle>();
        itemToggle.targetGraphic = itemObject.GetComponent<Image>();
        dropdown.template = templateRect;
        dropdown.itemText = itemLabel;
        templateObject.GetComponent<ScrollRect>().content = contentRect;
        templateObject.GetComponent<ScrollRect>().viewport = viewportRect;
        return dropdown;
    }

    private Texture2D CreatePreviewTexture(string targetName)
    {
        Texture2D texture = new Texture2D(128, 96, TextureFormat.RGBA32, false);
        texture.name = targetName + " Preview";
        Color baseColor = targetName == "Seoul"
            ? new Color(0.08f, 0.12f, 0.16f)
            : targetName == "Jeju" ? new Color(0.04f, 0.18f, 0.2f) : new Color(0.02f, 0.1f, 0.22f);
        Color scanColor = targetName == "Seoul"
            ? new Color(0.3f, 0.95f, 1f)
            : targetName == "Jeju" ? new Color(0.4f, 1f, 0.58f) : new Color(0.22f, 0.5f, 1f);

        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                float grid = (x % 16 == 0 || y % 16 == 0) ? 0.22f : 0f;
                float noise = Mathf.PerlinNoise(x * 0.08f, y * 0.08f);
                Color color = Color.Lerp(baseColor, scanColor, grid + noise * 0.18f);

                if (targetName == "Jeju")
                {
                    Vector2 p = new Vector2((x - 64f) / 34f, (y - 48f) / 20f);
                    if (p.sqrMagnitude < 1f)
                    {
                        color = Color.Lerp(color, new Color(0.18f, 0.55f, 0.24f), 0.85f);
                    }
                }
                else if (targetName == "Seoul")
                {
                    if ((x > 24 && x < 104 && y > 34 && y < 62 && x % 9 < 2) || (y > 20 && y < 80 && x > 48 && x < 76 && y % 8 < 2))
                    {
                        color = Color.Lerp(color, new Color(0.75f, 0.82f, 0.88f), 0.8f);
                    }
                }
                else
                {
                    color = Color.Lerp(color, scanColor, Mathf.Sin((x + y) * 0.12f) * 0.08f + 0.08f);
                }

                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        return texture;
    }

    private string FormatSatelliteRow(SatelliteData satellite)
    {
        return satellite.satelliteName + " | " + satellite.OwnerTypeLabel + "\n" +
            satellite.StateLabel + " | " + satellite.usageType;
    }

    private string MissionPhaseLabel(MissionPhase phase)
    {
        switch (phase)
        {
            case MissionPhase.Idle:
                return "Idle";
            case MissionPhase.Reserved:
                return "Reserved";
            case MissionPhase.Running:
                return "Orbiting";
            case MissionPhase.Observing:
                return "Observing";
            case MissionPhase.ObservationComplete:
                return "Stored";
            case MissionPhase.Transmitting:
                return "Transmitting";
            case MissionPhase.Complete:
                return "Complete";
            default:
                return phase.ToString();
        }
    }
}
