using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private enum ButtonVisual
    {
        Primary,
        Secondary,
        Disabled
    }

    private SimulationManager simulationManager;
    private SatelliteManager satelliteManager;
    private MissionManager missionManager;
    private CameraManager cameraManager;
    private Font uiFont;

    private RectTransform satelliteListContent;
    private Text satelliteListSummaryText;
    private Text detailText;
    private Text missionHeadlineText;
    private Text missionStatusText;
    private Text titleText;
    private Dropdown targetDropdown;
    private Button reserveButton;
    private Button startMissionButton;
    private Button resetButton;
    private Button fastDemoButton;
    private Image messageInputBox;
    private Text reservationHintText;
    private Image transferSummaryBox;
    private Text transferSummaryText;
    private RectTransform timeControlPanel;
    private RectTransform messageSizePanel;
    private Image observationFill;
    private Image transferFill;
    private RectTransform resultPanel;
    private Text resultText;
    private Text resultDataText;
    private RawImage previewImage;
    private readonly List<Image> timelineDots = new List<Image>();
    private readonly List<Text> timelineLabels = new List<Text>();

    private readonly Dictionary<string, Button> satelliteButtons = new Dictionary<string, Button>();
    private SatelliteListFilter satelliteFilter = SatelliteListFilter.All;
    private bool eventsAttached;

    public void BuildUI(
        SimulationManager simulation,
        SatelliteManager satellites,
        MissionManager mission,
        CameraManager cameras)
    {
        DetachEvents();
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

        AttachEvents();

        RefreshSatelliteList();
        RefreshAll();
    }

    private void OnDestroy()
    {
        DetachEvents();
    }

    private void AttachEvents()
    {
        if (eventsAttached)
        {
            return;
        }

        if (satelliteManager != null)
        {
            satelliteManager.OnDataChanged += RefreshAll;
            satelliteManager.OnSelectionChanged += HandleSatelliteSelectionChanged;
        }

        if (missionManager != null)
        {
            missionManager.OnMissionChanged += RefreshAll;
        }

        if (simulationManager != null)
        {
            simulationManager.OnTimeScaleChanged += RefreshAll;
        }

        eventsAttached = true;
    }

    private void DetachEvents()
    {
        if (!eventsAttached)
        {
            return;
        }

        if (satelliteManager != null)
        {
            satelliteManager.OnDataChanged -= RefreshAll;
            satelliteManager.OnSelectionChanged -= HandleSatelliteSelectionChanged;
        }

        if (missionManager != null)
        {
            missionManager.OnMissionChanged -= RefreshAll;
        }

        if (simulationManager != null)
        {
            simulationManager.OnTimeScaleChanged -= RefreshAll;
        }

        eventsAttached = false;
    }

    private void HandleSatelliteSelectionChanged(SatelliteData selected)
    {
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
        RectTransform panel = CreatePanel(parent, "TopBar", new Vector2(0f, 0.91f), new Vector2(1f, 1f), new Color(0.02f, 0.05f, 0.09f, 0.82f));
        titleText = CreateText(panel, "OrbitShare 단문 위성 발신", 30, TextAnchor.MiddleLeft);
        titleText.rectTransform.offsetMin = new Vector2(24f, 0f);
        titleText.rectTransform.offsetMax = new Vector2(-24f, 0f);
    }

    private void CreateSatelliteListPanel(Transform parent)
    {
        RectTransform panel = CreatePanel(parent, "SatelliteListPanel", new Vector2(0.015f, 0.47f), new Vector2(0.31f, 0.89f), new Color(0.035f, 0.06f, 0.095f, 0.9f));
        Text heading = CreateText(panel, "사용 가능한 위성 선택", 23, TextAnchor.UpperLeft);
        heading.rectTransform.anchorMin = new Vector2(0f, 1f);
        heading.rectTransform.anchorMax = new Vector2(1f, 1f);
        heading.rectTransform.pivot = new Vector2(0.5f, 1f);
        heading.rectTransform.anchoredPosition = new Vector2(0f, -16f);
        heading.rectTransform.sizeDelta = new Vector2(-28f, 32f);

        satelliteListSummaryText = CreateText(panel, "24개 후보 중 조건순 정렬", 13, TextAnchor.UpperLeft);
        satelliteListSummaryText.color = new Color(0.78f, 0.88f, 1f, 0.78f);
        satelliteListSummaryText.rectTransform.anchorMin = new Vector2(0f, 1f);
        satelliteListSummaryText.rectTransform.anchorMax = new Vector2(1f, 1f);
        satelliteListSummaryText.rectTransform.pivot = new Vector2(0.5f, 1f);
        satelliteListSummaryText.rectTransform.anchoredPosition = new Vector2(0f, -50f);
        satelliteListSummaryText.rectTransform.sizeDelta = new Vector2(-28f, 18f);

        CreateFilterButton(panel, "전체", 0f, delegate { satelliteFilter = SatelliteListFilter.All; RefreshSatelliteList(); });
        CreateFilterButton(panel, "가능", 0.25f, delegate { satelliteFilter = SatelliteListFilter.Available; RefreshSatelliteList(); });
        CreateFilterButton(panel, "빠른창", 0.5f, delegate { satelliteFilter = SatelliteListFilter.Fast; RefreshSatelliteList(); });
        CreateFilterButton(panel, "저가", 0.75f, delegate { satelliteFilter = SatelliteListFilter.LowCost; RefreshSatelliteList(); });

        GameObject viewport = new GameObject("SatelliteListViewport", typeof(RectTransform), typeof(Image), typeof(Mask), typeof(ScrollRect));
        viewport.transform.SetParent(panel, false);
        RectTransform viewportRect = viewport.GetComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.offsetMin = new Vector2(14f, 14f);
        viewportRect.offsetMax = new Vector2(-14f, -114f);
        viewport.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.03f);
        viewport.GetComponent<Mask>().showMaskGraphic = false;

        GameObject content = new GameObject("ListContent", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
        content.transform.SetParent(viewport.transform, false);
        satelliteListContent = content.GetComponent<RectTransform>();
        satelliteListContent.anchorMin = new Vector2(0f, 1f);
        satelliteListContent.anchorMax = new Vector2(1f, 1f);
        satelliteListContent.pivot = new Vector2(0.5f, 1f);
        satelliteListContent.anchoredPosition = Vector2.zero;
        satelliteListContent.sizeDelta = Vector2.zero;

        VerticalLayoutGroup layout = content.GetComponent<VerticalLayoutGroup>();
        layout.spacing = 7f;
        layout.childControlHeight = true;
        layout.childControlWidth = true;
        layout.childForceExpandHeight = false;
        ContentSizeFitter fitter = content.GetComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        ScrollRect scrollRect = viewport.GetComponent<ScrollRect>();
        scrollRect.content = satelliteListContent;
        scrollRect.viewport = viewportRect;
        scrollRect.horizontal = false;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;
    }

    private void CreateDetailPanel(Transform parent)
    {
        RectTransform panel = CreatePanel(parent, "SatelliteDetailPanel", new Vector2(0.69f, 0.47f), new Vector2(0.985f, 0.89f), new Color(0.035f, 0.06f, 0.095f, 0.9f));
        Text heading = CreateText(panel, "선택 위성 정보", 23, TextAnchor.UpperLeft);
        heading.rectTransform.anchorMin = new Vector2(0f, 1f);
        heading.rectTransform.anchorMax = new Vector2(1f, 1f);
        heading.rectTransform.pivot = new Vector2(0.5f, 1f);
        heading.rectTransform.anchoredPosition = new Vector2(0f, -18f);
        heading.rectTransform.sizeDelta = new Vector2(-28f, 36f);

        detailText = CreateText(panel, string.Empty, 18, TextAnchor.UpperLeft);
        detailText.lineSpacing = 1.12f;
        detailText.rectTransform.offsetMin = new Vector2(18f, 18f);
        detailText.rectTransform.offsetMax = new Vector2(-18f, -64f);
    }

    private void CreateReservationPanel(Transform parent)
    {
        RectTransform panel = CreatePanel(parent, "MissionReservationPanel", new Vector2(0.69f, 0.16f), new Vector2(0.985f, 0.45f), new Color(0.035f, 0.06f, 0.095f, 0.9f));
        Text heading = CreateText(panel, "단문 메시지 발신", 23, TextAnchor.UpperLeft);
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

        reserveButton = CreateButton(panel, "위성 확정", new Vector2(0f, 0f), new Vector2(0.32f, 0f), new Vector2(18f, 18f), new Vector2(-6f, 64f), delegate { missionManager.ReserveMission(); });
        startMissionButton = CreateButton(panel, "메시지 발신", new Vector2(0.34f, 0f), new Vector2(0.66f, 0f), new Vector2(0f, 18f), new Vector2(0f, 64f), delegate { missionManager.StartMission(); });
        resetButton = CreateButton(panel, "초기화", new Vector2(0.68f, 0f), new Vector2(1f, 0f), new Vector2(6f, 18f), new Vector2(-18f, 64f), delegate { missionManager.ResetMission(); });
        fastDemoButton = CreateButton(panel, "추천 위성으로 바로 발신", new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(18f, 78f), new Vector2(-18f, 118f), delegate { missionManager.RunFastDemo(); });

        GameObject messageBoxObject = new GameObject("MessageSampleBox", typeof(RectTransform), typeof(Image));
        messageBoxObject.transform.SetParent(panel, false);
        RectTransform messageBoxRect = messageBoxObject.GetComponent<RectTransform>();
        messageBoxRect.anchorMin = new Vector2(0f, 0f);
        messageBoxRect.anchorMax = new Vector2(1f, 0f);
        messageBoxRect.offsetMin = new Vector2(18f, 122f);
        messageBoxRect.offsetMax = new Vector2(-18f, 154f);
        messageInputBox = messageBoxObject.GetComponent<Image>();
        messageInputBox.color = new Color(0.08f, 0.11f, 0.16f, 0.98f);

        reservationHintText = CreateText(panel, string.Empty, 14, TextAnchor.MiddleLeft);
        reservationHintText.rectTransform.anchorMin = new Vector2(0f, 0f);
        reservationHintText.rectTransform.anchorMax = new Vector2(1f, 0f);
        reservationHintText.rectTransform.offsetMin = new Vector2(30f, 122f);
        reservationHintText.rectTransform.offsetMax = new Vector2(-30f, 154f);
        reservationHintText.color = new Color(0.9f, 0.96f, 1f, 0.92f);

        GameObject summaryBoxObject = new GameObject("TransferSummaryBox", typeof(RectTransform), typeof(Image));
        summaryBoxObject.transform.SetParent(panel, false);
        RectTransform summaryBoxRect = summaryBoxObject.GetComponent<RectTransform>();
        summaryBoxRect.anchorMin = new Vector2(0f, 0f);
        summaryBoxRect.anchorMax = new Vector2(1f, 0f);
        summaryBoxRect.offsetMin = new Vector2(18f, 72f);
        summaryBoxRect.offsetMax = new Vector2(-18f, 121f);
        transferSummaryBox = summaryBoxObject.GetComponent<Image>();
        transferSummaryBox.color = new Color(0.06f, 0.15f, 0.18f, 0.98f);
        transferSummaryBox.gameObject.SetActive(false);

        transferSummaryText = CreateText(panel, string.Empty, 12, TextAnchor.UpperLeft);
        transferSummaryText.lineSpacing = 0.85f;
        transferSummaryText.rectTransform.anchorMin = new Vector2(0f, 0f);
        transferSummaryText.rectTransform.anchorMax = new Vector2(1f, 0f);
        transferSummaryText.rectTransform.offsetMin = new Vector2(28f, 75f);
        transferSummaryText.rectTransform.offsetMax = new Vector2(-28f, 120f);
        transferSummaryText.color = new Color(0.88f, 0.96f, 1f, 0.95f);
        transferSummaryText.gameObject.SetActive(false);

        PopulateTargetDropdown();
    }

    private void CreateTimeControlPanel(Transform parent)
    {
        RectTransform panel = CreatePanel(parent, "TimeControlPanel", new Vector2(0.325f, 0.02f), new Vector2(0.515f, 0.14f), new Color(0.035f, 0.06f, 0.095f, 0.88f));
        timeControlPanel = panel;
        Text heading = CreateText(panel, "발신 시각", 18, TextAnchor.UpperCenter);
        heading.rectTransform.anchorMin = new Vector2(0f, 1f);
        heading.rectTransform.anchorMax = new Vector2(1f, 1f);
        heading.rectTransform.pivot = new Vector2(0.5f, 1f);
        heading.rectTransform.anchoredPosition = new Vector2(0f, -8f);
        heading.rectTransform.sizeDelta = new Vector2(-18f, 28f);

        CreateButton(panel, "지금", new Vector2(0f, 0f), new Vector2(0.25f, 0f), new Vector2(10f, 12f), new Vector2(-4f, 50f), delegate { missionManager.SetSendDelayMinutes(0); });
        CreateButton(panel, "5분", new Vector2(0.25f, 0f), new Vector2(0.5f, 0f), new Vector2(4f, 12f), new Vector2(-4f, 50f), delegate { missionManager.SetSendDelayMinutes(5); });
        CreateButton(panel, "15분", new Vector2(0.5f, 0f), new Vector2(0.75f, 0f), new Vector2(4f, 12f), new Vector2(-4f, 50f), delegate { missionManager.SetSendDelayMinutes(15); });
        CreateButton(panel, "30분", new Vector2(0.75f, 0f), new Vector2(1f, 0f), new Vector2(4f, 12f), new Vector2(-10f, 50f), delegate { missionManager.SetSendDelayMinutes(30); });
    }

    private void CreateCameraControlPanel(Transform parent)
    {
        RectTransform panel = CreatePanel(parent, "CameraControlPanel", new Vector2(0.525f, 0.02f), new Vector2(0.675f, 0.14f), new Color(0.035f, 0.06f, 0.095f, 0.88f));
        messageSizePanel = panel;
        Text heading = CreateText(panel, "메시지 용량", 18, TextAnchor.UpperCenter);
        heading.rectTransform.anchorMin = new Vector2(0f, 1f);
        heading.rectTransform.anchorMax = new Vector2(1f, 1f);
        heading.rectTransform.pivot = new Vector2(0.5f, 1f);
        heading.rectTransform.anchoredPosition = new Vector2(0f, -8f);
        heading.rectTransform.sizeDelta = new Vector2(-18f, 28f);

        CreateButton(panel, "32KB", new Vector2(0f, 0f), new Vector2(0.28f, 0f), new Vector2(10f, 12f), new Vector2(-4f, 50f), delegate { missionManager.SetMessageSizeKb(32); });
        CreateButton(panel, "128KB", new Vector2(0.28f, 0f), new Vector2(0.58f, 0f), new Vector2(4f, 12f), new Vector2(-4f, 50f), delegate { missionManager.SetMessageSizeKb(128); });
        CreateButton(panel, "512KB", new Vector2(0.58f, 0f), new Vector2(1f, 0f), new Vector2(4f, 12f), new Vector2(-10f, 50f), delegate { missionManager.SetMessageSizeKb(512); });

        RectTransform statusPanel = CreatePanel(parent, "CommunicationStatusPanel", new Vector2(0.325f, 0.16f), new Vector2(0.675f, 0.335f), new Color(0.035f, 0.06f, 0.095f, 0.9f));
        missionHeadlineText = CreateText(statusPanel, string.Empty, 23, TextAnchor.UpperLeft);
        missionHeadlineText.fontStyle = FontStyle.Bold;
        missionHeadlineText.rectTransform.offsetMin = new Vector2(18f, 108f);
        missionHeadlineText.rectTransform.offsetMax = new Vector2(-18f, -10f);

        missionStatusText = CreateText(statusPanel, string.Empty, 14, TextAnchor.UpperLeft);
        missionStatusText.rectTransform.offsetMin = new Vector2(18f, 52f);
        missionStatusText.rectTransform.offsetMax = new Vector2(-18f, -44f);

        CreateProgressLabel(statusPanel, "중계", new Vector2(18f, 36f));
        CreateProgressLabel(statusPanel, "수신", new Vector2(18f, 14f));
        observationFill = CreateProgressBar(statusPanel, "RelayLockProgress", new Vector2(74f, 36f), new Color(1f, 0.68f, 0.12f, 0.95f));
        transferFill = CreateProgressBar(statusPanel, "DeliveryProgress", new Vector2(74f, 14f), new Color(0.24f, 1f, 0.84f, 0.95f));
    }

    private void CreateResultPanel(Transform parent)
    {
        resultPanel = CreatePanel(parent, "CommunicationResultPanel", new Vector2(0.345f, 0.39f), new Vector2(0.59f, 0.535f), new Color(0.02f, 0.08f, 0.12f, 0.9f));
        resultText = CreateText(resultPanel, string.Empty, 20, TextAnchor.UpperLeft);
        resultText.fontStyle = FontStyle.Bold;
        resultText.rectTransform.anchorMin = new Vector2(0f, 0.48f);
        resultText.rectTransform.anchorMax = new Vector2(0.62f, 1f);
        resultText.rectTransform.offsetMin = new Vector2(20f, 12f);
        resultText.rectTransform.offsetMax = new Vector2(-20f, -12f);

        resultDataText = CreateText(resultPanel, string.Empty, 14, TextAnchor.UpperLeft);
        resultDataText.rectTransform.anchorMin = new Vector2(0f, 0f);
        resultDataText.rectTransform.anchorMax = new Vector2(0.62f, 0.48f);
        resultDataText.rectTransform.offsetMin = new Vector2(20f, 12f);
        resultDataText.rectTransform.offsetMax = new Vector2(-20f, -4f);

        GameObject previewObject = new GameObject("SignalRoutePreview", typeof(RectTransform), typeof(RawImage));
        previewObject.transform.SetParent(resultPanel, false);
        RectTransform previewRect = previewObject.GetComponent<RectTransform>();
        previewRect.anchorMin = new Vector2(0.66f, 0.12f);
        previewRect.anchorMax = new Vector2(0.96f, 0.88f);
        previewRect.offsetMin = Vector2.zero;
        previewRect.offsetMax = Vector2.zero;
        previewImage = previewObject.GetComponent<RawImage>();
        previewImage.color = Color.white;
        previewObject.SetActive(false);
        resultPanel.gameObject.SetActive(false);
    }

    private void CreateTimelinePanel(Transform parent)
    {
        RectTransform panel = CreatePanel(parent, "CommunicationTimelinePanel", new Vector2(0.34f, 0.8f), new Vector2(0.66f, 0.87f), new Color(0.025f, 0.055f, 0.085f, 0.82f));
        string[] labels = { "요청", "경로", "상향", "중계", "하향", "완료" };
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

        string targetName = missionManager != null && missionManager.SelectedTarget != null ? missionManager.SelectedTarget.targetName : "New York";
        int sendDelay = missionManager != null ? missionManager.RequestedSendDelayMinutes : 0;
        List<SatelliteData> visibleSatellites = satelliteManager.GetFilteredSatellites(satelliteFilter, targetName, sendDelay);
        if (satelliteListSummaryText != null)
        {
            int totalCount = satelliteManager.Satellites.Count;
            satelliteListSummaryText.text = satelliteFilter == SatelliteListFilter.All
                ? totalCount + "개 후보 중 조건순 정렬"
                : totalCount + "개 후보 중 " + visibleSatellites.Count + "개 표시 · 조건순 정렬";
        }
        foreach (SatelliteData satellite in visibleSatellites)
        {
            string satelliteName = satellite.satelliteName;
            Button button = CreateListButton(satelliteListContent, FormatSatelliteRow(satellite), delegate { satelliteManager.SelectSatellite(satelliteName); });
            ColorBlock colors = button.colors;
            bool canServe = satellite.CanServe(targetName, sendDelay);
            bool selected = satellite == satelliteManager.SelectedSatellite;
            colors.normalColor = selected
                ? new Color(0.08f, 0.42f, 0.72f, 1f)
                : canServe ? new Color(0.06f, 0.24f, 0.2f, 0.98f) : new Color(0.08f, 0.11f, 0.16f, 0.98f);
            colors.highlightedColor = selected ? new Color(0.11f, 0.52f, 0.86f, 1f) : new Color(0.16f, 0.44f, 0.74f, 1f);
            colors.pressedColor = new Color(0.06f, 0.24f, 0.38f, 1f);
            button.colors = colors;
            Transform selectionBar = button.transform.Find("SelectionBar");
            if (selectionBar != null)
            {
                Image bar = selectionBar.GetComponent<Image>();
                if (bar != null)
                {
                    bar.color = selected ? new Color(0.24f, 1f, 0.84f, 1f) : new Color(0.24f, 1f, 0.84f, 0.08f);
                }
            }
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
        string targetName = missionManager != null && missionManager.SelectedTarget != null ? missionManager.SelectedTarget.targetName : "New York";
        int messageSize = missionManager != null ? missionManager.MessageSizeKb : 128;
        int price = satellite.EstimatePriceCredits(messageSize);
        int success = missionManager != null ? missionManager.SuccessProbabilityPercent : satellite.reliabilityPercent;
        string availability = satellite.CanServe(targetName, missionManager != null ? missionManager.RequestedSendDelayMinutes : 0) ? "발신 가능" : satellite.StateLabel;
        int sendDelay = missionManager != null ? missionManager.RequestedSendDelayMinutes : 0;
        detailText.text =
            satellite.satelliteName + "  |  " + satellite.OwnerTypeLabel + "\n" +
            "상태: " + availability + "  ·  대상: " + DestinationLabel(targetName) + "\n" +
            "예상 지연: " + satellite.latencySeconds + "초  |  요금: " + price + "cr\n" +
            "성공 예측: " + success + "%  |  신뢰도: " + satellite.reliabilityPercent + "%\n" +
            "다음 창: " + (satellite.nextWindowMinutes == 0 ? "지금" : satellite.nextWindowMinutes + "분 후") + "\n" +
            "추천 이유: " + RecommendationReason(satellite, targetName, sendDelay) + "\n" +
            "—\n" +
            "궤도: " + satellite.orbitSlot + " / " + satellite.altitudeKm.ToString("0") + " km\n" +
            "배터리: " + satellite.batteryPercent.ToString("0") + "%  |  버퍼: " + satellite.dataStoragePercent.ToString("0") + "%";
    }

    private void RefreshMissionPanel()
    {
        if (missionStatusText == null || missionManager == null)
        {
            return;
        }

        if (titleText != null && simulationManager != null)
        {
            titleText.text = "OrbitShare 단문 위성 발신    |    " + HeaderStateLabel();
        }

        if (missionHeadlineText != null)
        {
            missionHeadlineText.text = StatusHeadline();
        }

        bool isComplete = missionManager.Phase == MissionPhase.Complete;
        if (timeControlPanel != null)
        {
            timeControlPanel.gameObject.SetActive(!isComplete);
        }
        if (messageSizePanel != null)
        {
            messageSizePanel.gameObject.SetActive(!isComplete);
        }

        string missionTargetName = missionManager.SelectedTarget != null ? missionManager.SelectedTarget.targetName : "None";
        missionStatusText.text =
            "목적지: " + DestinationLabel(missionTargetName) +
            "  |  메시지: " + missionManager.MessageSizeKb + "KB 단문  |  요금: " + missionManager.EstimatedPriceCredits + "cr\n" +
            "성공 예측: " + missionManager.SuccessProbabilityPercent + "%  |  링크: " + (missionManager.CommunicationAvailable ? "연결" : "대기") +
            "  |  중계/수신: " + Mathf.RoundToInt(missionManager.ObservationProgress * 100f) + "% / " + Mathf.RoundToInt(missionManager.TransferProgress * 100f) + "%";

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
            bool showResult = isComplete;
            resultPanel.gameObject.SetActive(showResult);
            if (showResult)
            {
                string targetName = missionManager.SelectedTarget != null ? missionManager.SelectedTarget.targetName : "Unknown";
                resultText.text = "수신 완료\n전달 성공";
                resultDataText.text =
                    "OS-" + System.DateTime.Now.ToString("HHmm") + " · 서울→" + DestinationLabel(targetName) + "\n" +
                    missionManager.MessageSizeKb + "KB · " + missionManager.EstimatedPriceCredits + "cr · 성공 " + missionManager.SuccessProbabilityPercent + "%";
                if (previewImage.texture == null || previewImage.texture.name != targetName + " Preview")
                {
                    previewImage.texture = CreatePreviewTexture(targetName);
                }
            }
            else
            {
                resultText.text = string.Empty;
            }
            if (previewImage != null)
            {
                previewImage.gameObject.SetActive(showResult);
            }
        }

        RefreshTimeline();

        if (reserveButton != null)
        {
            string targetName = missionManager.SelectedTarget != null ? missionManager.SelectedTarget.targetName : "New York";
            bool canReserve = satelliteManager != null && satelliteManager.CanReserveSelected(targetName, missionManager.RequestedSendDelayMinutes) && missionManager.Phase == MissionPhase.Idle;
            reserveButton.gameObject.SetActive(!isComplete);
            reserveButton.interactable = canReserve;
            SetButtonLabel(reserveButton, missionManager.Phase == MissionPhase.Reserved || missionManager.Phase == MissionPhase.Complete ? "위성 확정됨" : "위성 확정");
            SetButtonVisual(reserveButton, canReserve ? ButtonVisual.Secondary : ButtonVisual.Disabled);
            SetButtonRect(reserveButton, new Vector2(0f, 0f), new Vector2(0.32f, 0f), new Vector2(18f, 18f), new Vector2(-6f, 64f));
        }
        if (startMissionButton != null)
        {
            startMissionButton.gameObject.SetActive(!isComplete);
            startMissionButton.interactable = missionManager.Phase == MissionPhase.Reserved;
            SetButtonLabel(startMissionButton, missionManager.Phase == MissionPhase.Idle ? "확정 후 발신" : missionManager.Phase == MissionPhase.Complete ? "발신 완료" : "메시지 발신");
            SetButtonVisual(startMissionButton, missionManager.Phase == MissionPhase.Reserved ? ButtonVisual.Primary : ButtonVisual.Disabled);
            SetButtonRect(startMissionButton, new Vector2(0.34f, 0f), new Vector2(0.66f, 0f), new Vector2(0f, 18f), new Vector2(0f, 64f));
        }
        if (resetButton != null)
        {
            resetButton.interactable = missionManager.Phase == MissionPhase.Complete || missionManager.Phase == MissionPhase.Reserved;
            SetButtonLabel(resetButton, isComplete ? "새 메시지 보내기" : "초기화");
            SetButtonVisual(resetButton, isComplete ? ButtonVisual.Primary : resetButton.interactable ? ButtonVisual.Secondary : ButtonVisual.Disabled);
            if (isComplete)
            {
                SetButtonRect(resetButton, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(18f, 18f), new Vector2(-18f, 70f));
            }
            else
            {
                SetButtonRect(resetButton, new Vector2(0.68f, 0f), new Vector2(1f, 0f), new Vector2(6f, 18f), new Vector2(-18f, 64f));
            }
        }
        if (targetDropdown != null)
        {
            targetDropdown.interactable = missionManager.Phase == MissionPhase.Idle;
        }
        if (fastDemoButton != null)
        {
            fastDemoButton.gameObject.SetActive(!isComplete);
            fastDemoButton.interactable = !isComplete;
            SetButtonLabel(fastDemoButton, missionManager.Phase == MissionPhase.Idle ? "추천 위성으로 바로 발신" : isComplete ? "발신 완료" : "자동 발신 진행 중");
            SetButtonVisual(fastDemoButton, missionManager.Phase == MissionPhase.Idle ? ButtonVisual.Primary : fastDemoButton.interactable ? ButtonVisual.Secondary : ButtonVisual.Disabled);
        }
        if (reservationHintText != null)
        {
            reservationHintText.text = "메시지 입력  |  \"" + MessageSampleText(missionTargetName) + "\"";
        }
        if (messageInputBox != null)
        {
            messageInputBox.color = missionManager.Phase == MissionPhase.Complete
                ? new Color(0.04f, 0.18f, 0.2f, 0.98f)
                : new Color(0.08f, 0.11f, 0.16f, 0.98f);
        }
        if (transferSummaryBox != null)
        {
            transferSummaryBox.gameObject.SetActive(isComplete);
        }
        if (transferSummaryText != null)
        {
            bool showSummary = isComplete;
            transferSummaryText.gameObject.SetActive(showSummary);
            if (showSummary)
            {
                SatelliteData selected = satelliteManager != null ? satelliteManager.SelectedSatellite : null;
                string satelliteName = selected != null ? selected.satelliteName : "선택 위성";
                int latencySeconds = selected != null ? selected.latencySeconds : 0;
                transferSummaryText.text =
                    "전송 요약  |  " + satelliteName + "\n" +
                    missionManager.MessageSizeKb + "KB · " + missionManager.EstimatedPriceCredits + "cr · " + SendTimeLabel() + "\n" +
                    "지연 " + latencySeconds + "초 · 수신 확인";
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
                return 6;
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
            options.Add("목적지: " + DestinationLabel(target.targetName));
        }
        targetDropdown.AddOptions(options);
    }

    private void CreateFilterButton(RectTransform parent, string label, float leftAnchor, UnityAction action)
    {
        Button button = CreateButton(
            parent,
            label,
            new Vector2(leftAnchor, 1f),
            new Vector2(leftAnchor + 0.23f, 1f),
            new Vector2(14f, -102f),
            new Vector2(0f, -70f),
            action);
        Text text = button.GetComponentInChildren<Text>();
        if (text != null)
        {
            text.fontSize = 13;
        }
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
        string[] koreanFonts = { "Noto Sans CJK KR", "Noto Sans KR", "NanumGothic", "Arial Unicode MS" };
        Font koreanFont = Font.CreateDynamicFontFromOSFont(koreanFonts, 16);
        return koreanFont != null ? koreanFont : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
    }

    private Button CreateListButton(Transform parent, string label, UnityAction action)
    {
        GameObject buttonObject = new GameObject("SatelliteButton", typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement));
        buttonObject.transform.SetParent(parent, false);
        LayoutElement layoutElement = buttonObject.GetComponent<LayoutElement>();
        layoutElement.preferredHeight = 58f;
        Button button = buttonObject.GetComponent<Button>();
        button.onClick.AddListener(action);

        Image image = buttonObject.GetComponent<Image>();
        image.color = new Color(0.1f, 0.14f, 0.2f, 0.96f);

        Text text = CreateText(buttonObject.transform, label, 13, TextAnchor.MiddleLeft);
        text.rectTransform.offsetMin = new Vector2(12f, 6f);
        text.rectTransform.offsetMax = new Vector2(-12f, -6f);
        GameObject barObject = new GameObject("SelectionBar", typeof(RectTransform), typeof(Image));
        barObject.transform.SetParent(buttonObject.transform, false);
        RectTransform barRect = barObject.GetComponent<RectTransform>();
        barRect.anchorMin = new Vector2(0f, 0f);
        barRect.anchorMax = new Vector2(0f, 1f);
        barRect.offsetMin = new Vector2(0f, 0f);
        barRect.offsetMax = new Vector2(5f, 0f);
        barObject.GetComponent<Image>().color = new Color(0.24f, 1f, 0.84f, 0.08f);
        return button;
    }

    private Text CreateProgressLabel(Transform parent, string label, Vector2 offset)
    {
        Text text = CreateText(parent, label, 12, TextAnchor.MiddleLeft);
        text.rectTransform.anchorMin = new Vector2(0f, 0f);
        text.rectTransform.anchorMax = new Vector2(0f, 0f);
        text.rectTransform.offsetMin = new Vector2(offset.x, offset.y - 2f);
        text.rectTransform.offsetMax = new Vector2(offset.x + 46f, offset.y + 14f);
        text.color = new Color(0.86f, 0.93f, 1f, 0.88f);
        return text;
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
            : targetName == "New York" ? new Color(0.05f, 0.1f, 0.18f) : new Color(0.04f, 0.12f, 0.18f);
        Color scanColor = targetName == "Seoul"
            ? new Color(0.3f, 0.95f, 1f)
            : targetName == "New York" ? new Color(0.3f, 0.7f, 1f) : new Color(0.55f, 0.9f, 1f);

        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                float grid = (x % 16 == 0 || y % 16 == 0) ? 0.22f : 0f;
                float noise = Mathf.PerlinNoise(x * 0.08f, y * 0.08f);
                Color color = Color.Lerp(baseColor, scanColor, grid + noise * 0.18f);

                if (targetName == "New York")
                {
                    if ((x > 22 && x < 110 && y > 28 && y < 72 && (x + y) % 11 < 3) || (x > 54 && x < 74 && y > 18 && y < 82))
                    {
                        color = Color.Lerp(color, new Color(0.72f, 0.78f, 0.9f), 0.8f);
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
                    if ((x > 18 && x < 112 && y > 36 && y < 58) || (x > 42 && x < 86 && y > 24 && y < 74 && x % 10 < 2))
                    {
                        color = Color.Lerp(color, new Color(0.68f, 0.72f, 0.76f), 0.8f);
                    }
                }

                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        return texture;
    }

    private string FormatSatelliteRow(SatelliteData satellite)
    {
        string targetName = missionManager != null && missionManager.SelectedTarget != null ? missionManager.SelectedTarget.targetName : "New York";
        int sendDelay = missionManager != null ? missionManager.RequestedSendDelayMinutes : 0;
        bool selected = satelliteManager != null && satellite == satelliteManager.SelectedSatellite;
        bool canServe = satellite.CanServe(targetName, sendDelay);
        bool recommended = IsRecommendedSatellite(satellite, targetName, sendDelay);
        string badge = selected ? recommended ? "✓ 선택 · 추천" : "✓ 선택" : recommended ? "★ 추천" : canServe ? "발신 가능" : "대기";
        string window = satellite.nextWindowMinutes == 0 ? "즉시" : satellite.nextWindowMinutes + "분";
        return satellite.satelliteName + "   " + badge + "\n" +
            satellite.orbitSlot + " · " + window + " · " + satellite.feePer32KbCredits + "cr/32KB · 신뢰도 " + satellite.reliabilityPercent + "%";
    }

    private bool IsRecommendedSatellite(SatelliteData satellite, string targetName, int sendDelay)
    {
        if (satelliteManager == null || satellite == null)
        {
            return false;
        }

        SatelliteData best = satelliteManager.FindBestSatellite(targetName, sendDelay);
        return best == satellite;
    }

    private string RecommendationReason(SatelliteData satellite, string targetName, int sendDelay)
    {
        if (missionManager != null && missionManager.Phase == MissionPhase.Complete && satelliteManager != null && satellite == satelliteManager.SelectedSatellite)
        {
            return "이번 발신에 사용된 위성";
        }
        if (missionManager != null && missionManager.Phase != MissionPhase.Idle && satelliteManager != null && satellite == satelliteManager.SelectedSatellite)
        {
            return "현재 발신 흐름의 선택 위성";
        }
        if (!satellite.CanServe(targetName, sendDelay))
        {
            return "현재 조건 불일치";
        }
        if (IsRecommendedSatellite(satellite, targetName, sendDelay))
        {
            return "즉시 가능 · 낮은 요금 우선";
        }
        if (satellite.reliabilityPercent >= 94)
        {
            return "고신뢰 후보";
        }
        if (satellite.feePer32KbCredits <= 3)
        {
            return "저가 후보";
        }
        return "조건 만족 후보";
    }

    private string MissionPhaseLabel(MissionPhase phase)
    {
        switch (phase)
        {
            case MissionPhase.Idle:
                return "요청 작성";
            case MissionPhase.Reserved:
                return "위성 확정";
            case MissionPhase.Running:
                return "경로 탐색";
            case MissionPhase.Observing:
                return "중계 잠금";
            case MissionPhase.ObservationComplete:
                return "메시지 적재";
            case MissionPhase.Transmitting:
                return "수신 전송";
            case MissionPhase.Complete:
                return "수신 완료";
            default:
                return phase.ToString();
        }
    }

    private string SendTimeLabel()
    {
        if (missionManager == null || missionManager.RequestedSendDelayMinutes == 0)
        {
            return "지금 발신";
        }
        return missionManager.RequestedSendDelayMinutes + "분 후 발신";
    }

    private string HeaderStateLabel()
    {
        if (missionManager == null)
        {
            return "요청 준비";
        }
        if (missionManager.Phase == MissionPhase.Complete)
        {
            return "수신 완료";
        }
        if (missionManager.Phase == MissionPhase.Reserved)
        {
            return "위성 확정";
        }
        if (missionManager.Phase == MissionPhase.Running || missionManager.Phase == MissionPhase.Observing || missionManager.Phase == MissionPhase.ObservationComplete || missionManager.Phase == MissionPhase.Transmitting)
        {
            return "전송 중";
        }
        return SendTimeLabel();
    }

    private string StatusHeadline()
    {
        string destination = missionManager != null && missionManager.SelectedTarget != null ? DestinationLabel(missionManager.SelectedTarget.targetName) : "수신 지역";
        switch (missionManager.Phase)
        {
            case MissionPhase.Reserved:
                return "위성 확정됨 · 메시지 발신 대기";
            case MissionPhase.Running:
            case MissionPhase.Observing:
                return "서울 → 위성 경로 잠금 중";
            case MissionPhase.ObservationComplete:
            case MissionPhase.Transmitting:
                return "서울 → " + destination + " 단문 전송 중";
            case MissionPhase.Complete:
                return "전송 완료 · 수신 확인됨";
            default:
                return "사용 가능한 위성을 선택하세요";
        }
    }

    private string MessageSampleText(string targetName)
    {
        if (targetName == "Seoul")
        {
            return "서울 내부 테스트 단문";
        }
        if (targetName == "New York")
        {
            return "뉴욕 수신국 상태 확인 요청";
        }
        if (targetName == "London")
        {
            return "런던 수신국 경로 확인 요청";
        }
        return "수신국 상태 확인 요청";
    }

    private void SetButtonLabel(Button button, string label)
    {
        if (button == null)
        {
            return;
        }

        Text text = button.GetComponentInChildren<Text>();
        if (text != null)
        {
            text.text = label;
        }
    }

    private void SetButtonVisual(Button button, ButtonVisual visual)
    {
        if (button == null)
        {
            return;
        }

        Color normal;
        Color highlighted;
        Color disabled;
        if (visual == ButtonVisual.Primary)
        {
            normal = new Color(0.05f, 0.56f, 0.72f, 1f);
            highlighted = new Color(0.12f, 0.72f, 0.92f, 1f);
            disabled = new Color(0.08f, 0.12f, 0.16f, 0.55f);
        }
        else if (visual == ButtonVisual.Secondary)
        {
            normal = new Color(0.1f, 0.28f, 0.48f, 0.96f);
            highlighted = new Color(0.14f, 0.38f, 0.62f, 1f);
            disabled = new Color(0.08f, 0.12f, 0.16f, 0.55f);
        }
        else
        {
            normal = new Color(0.08f, 0.12f, 0.16f, 0.55f);
            highlighted = normal;
            disabled = normal;
        }

        ColorBlock colors = button.colors;
        colors.normalColor = normal;
        colors.highlightedColor = highlighted;
        colors.pressedColor = new Color(0.04f, 0.22f, 0.34f, 1f);
        colors.disabledColor = disabled;
        button.colors = colors;
    }

    private void SetButtonRect(Button button, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
    {
        if (button == null)
        {
            return;
        }

        RectTransform rect = button.GetComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;
    }

    private string DestinationLabel(string targetName)
    {
        if (targetName == "Seoul")
        {
            return "서울";
        }
        if (targetName == "New York")
        {
            return "뉴욕";
        }
        if (targetName == "London")
        {
            return "런던";
        }
        if (targetName == "None")
        {
            return "없음";
        }
        return targetName;
    }
}
