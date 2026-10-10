using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public partial class UIManager : MonoBehaviour
{
    private const string PanelLayoutPersistPrefix = "OrbitShare.UI.Panel.v10.";
    private const int CalendarDayButtonCount = 42;
    private const int HistoryActionRowCount = 5;
    private const float FlowBottomActionMinY = -62f;
    private const float FlowBottomActionMaxY = -22f;

    private enum ButtonVisual
    {
        Primary,
        Cta,
        Secondary,
        Selected,
        Disabled
    }

    private SimulationManager simulationManager;
    private SatelliteManager satelliteManager;
    private MissionManager missionManager;
    private CameraManager cameraManager;
    private MissionVisualController missionVisualController;
    private Font uiFont;
    private CanvasScaler canvasScaler;

    private RectTransform satelliteListContent;
    private Text satelliteListSummaryText;
    private Text detailText;
    private Text missionHeadlineText;
    private Text missionStatusText;
    private Text titleText;
    private Dropdown targetDropdown;
    private Dropdown scheduleDayDropdown;
    private Dropdown scheduleHourDropdown;
    private Dropdown scheduleMinuteDropdown;
    private InputField messageInputField;
    private Text payloadWarningText;
    private Text attachmentStatusText;
    private Text scheduleSummaryText;
    private Text flowInstructionText;
    private Button reserveButton;
    private Button startMissionButton;
    private Button resetButton;
    private Button resultToggleButton;
    private Button historyToggleButton;
    private Button historyResendButton;
    private Button newMessageButton;
    private Button nextQueuedButton;
    private Button autoQueueToggleButton;
    private Button failureToggleButton;
    private Button retryButton;
    private Button replayButton;
    private Button resultReplayButton;
    private Button resultDetailsToggleButton;
    private Button resultNextQueuedButton;
    private Image messageInputBox;
    private Text reservationHintText;
    private RectTransform confirmSummaryContent;
    private ScrollRect confirmSummaryScrollRect;
    private Image transferSummaryBox;
    private Text transferSummaryText;
    private RectTransform timeControlPanel;
    private RectTransform messageSizePanel;
    private Image observationFill;
    private Image transferFill;
    private Image payloadFill;
    private Image queuePayloadFill;
    private Image resultUplinkFill;
    private Image resultFlowFill;
    private Image resultReceiveFill;
    private RectTransform resultPanel;
    private Text resultText;
    private Text resultDataText;
    private RawImage previewImage;
    private RectTransform satelliteListPanel;
    private RectTransform requestFlowPanel;
    private RectTransform missionStatusPanel;
    private RectTransform timelinePanel;
    private Text timelineStatusText;
    private RectTransform transferLogPanel;
    private Text transferLogText;
    private Button transferLogToggleButton;
    private RectTransform scheduleStepPanel;
    private RectTransform messageStepPanel;
    private RectTransform destinationStepPanel;
    private RectTransform satelliteStepPanel;
    private RectTransform confirmStepPanel;
    private RectTransform calendarPickerPanel;
    private Text calendarMonthLabel;
    private RectTransform attachmentPickerPanel;
    private RectTransform historyPanel;
    private RectTransform queueMessagePanel;
    private RectTransform queueAttachmentPickerPanel;
    private RectTransform panelLauncher;
    private RectTransform compassNeedle;
    private RectTransform toastPanel;
    private Text toastText;
    private RectTransform confirmDialogPanel;
    private Text confirmDialogText;
    private RectTransform settingsPanel;
    private Text settingsText;
    private RectTransform failurePanel;
    private Text failureDetailText;
    private Text availabilityTimelineText;
    private Text windowHintText;
    private Text queueScheduleHintText;
    private Text flowStepTitleText;
    private Text flowStepHintText;
    private Button schedulePickerButton;
    private Button attachmentPickerButton;
    private Button queueAttachmentPickerButton;
    private Button scheduleRecommendedButton;
    private Button satelliteAvailableOnlyButton;
    private Button satelliteRecommendedTimeButton;
    private Button satelliteFavoriteButton;
    private Button failureRetryApplyButton;
    private Button routeReplayPauseButton;
    private Button routeReplaySpeedButton;
    private Button statusStopButton;
    private Button settingsDemoButton;
    private Button queueSaveButton;
    private Button favoriteDestinationButton;
    private Button draftSaveButton;
    private Button draftRestoreButton;
    private Button flowBackButton;
    private Button flowNextButton;
    private Button originPickButton;
    private Button destinationPickButton;
    private Button queueOriginPickButton;
    private Button queueDestinationPickButton;
    private Text historyText;
    private Text destinationSummaryText;
    private Text queueListText;
    private Text attachmentPreviewText;
    private Text queueAttachmentPreviewText;
    private Text satelliteCompareText;
    private InputField destinationSearchInputField;
    private InputField satelliteSearchInputField;
    private InputField historySearchInputField;
    private InputField queueMessageInputField;
    private InputField attachmentPathInputField;
    private InputField queueAttachmentPathInputField;
    private Text queuePayloadText;
    private Text queueStatusText;
    private Button attachmentRemoveFirstButton;
    private Button queueAttachmentRemoveFirstButton;
    private Button messageInputModeButton;
    private Button confirmEditTimeButton;
    private Button confirmEditMessageButton;
    private Button confirmEditDestinationButton;
    private Button confirmEditSatelliteButton;
    private readonly Dictionary<SatelliteListFilter, Button> filterButtons = new Dictionary<SatelliteListFilter, Button>();
    private readonly Dictionary<int, Button> sendDelayButtons = new Dictionary<int, Button>();
    private readonly Dictionary<int, Button> messageSizeButtons = new Dictionary<int, Button>();
    private readonly Dictionary<int, Button> scheduleDayButtons = new Dictionary<int, Button>();
    private readonly Dictionary<int, Button> scheduleHourButtons = new Dictionary<int, Button>();
    private readonly Dictionary<int, Button> scheduleMinuteButtons = new Dictionary<int, Button>();
    private readonly Dictionary<string, Button> destinationButtons = new Dictionary<string, Button>();
    private readonly Dictionary<string, Button> attachmentButtons = new Dictionary<string, Button>();
    private readonly Dictionary<string, Button> queueDestinationButtons = new Dictionary<string, Button>();
    private readonly Dictionary<string, Button> queueAttachmentButtons = new Dictionary<string, Button>();
    private readonly List<Button> historyRecordButtons = new List<Button>();
    private readonly List<Button> historyRecordResendButtons = new List<Button>();
    private readonly List<Button> historyDeleteButtons = new List<Button>();
    private readonly List<int> historyVisibleIndices = new List<int>();
    private readonly List<Button> queueEditButtons = new List<Button>();
    private readonly List<Button> queueDeleteButtons = new List<Button>();
    private readonly List<Button> attachmentDeleteButtons = new List<Button>();
    private readonly List<Button> queueAttachmentDeleteButtons = new List<Button>();
    private readonly List<Button> flowStepButtons = new List<Button>();
    private readonly List<Button> panelLauncherButtons = new List<Button>();
    private readonly List<Button> queueUpButtons = new List<Button>();
    private readonly List<Button> queueDownButtons = new List<Button>();
    private readonly HashSet<string> favoriteSatelliteNames = new HashSet<string>();
    private readonly List<Image> timelineDots = new List<Image>();
    private readonly List<Text> timelineLabels = new List<Text>();

    private readonly Dictionary<string, Button> satelliteButtons = new Dictionary<string, Button>();
    private readonly Dictionary<string, SatelliteRouteEstimateRecord> satelliteListRouteEstimateCache = new Dictionary<string, SatelliteRouteEstimateRecord>();
    private readonly List<string> satelliteListDisplayOrder = new List<string>();
    private readonly Dictionary<string, string> satelliteRowLabelCache = new Dictionary<string, string>();
    private readonly Dictionary<string, SatelliteRowVisualState> satelliteRowVisualStateCache = new Dictionary<string, SatelliteRowVisualState>();
    private readonly Dictionary<string, int> satelliteRowServiceBucketCache = new Dictionary<string, int>();
    private readonly Dictionary<string, Coroutine> satelliteRowMoveAnimations = new Dictionary<string, Coroutine>();
    private Button satelliteListEmptyButton;
    private string satelliteListRouteCacheOriginName = string.Empty;
    private string satelliteListRouteCacheTargetName = string.Empty;
    private int satelliteListRouteCacheDelayMinutes = -1;
    private int satelliteListRouteCachePayloadKb = -1;
    private string satelliteListRecommendedSatelliteName = string.Empty;
    private SatelliteListFilter satelliteFilter = SatelliteListFilter.All;
    private bool eventsAttached;
    private bool scheduleDropdownUpdating;
    private bool resultDismissed;
    private bool resultDetailsExpanded;
    private bool requestFlowDismissed;
    private bool satelliteListDismissed;
    private bool missionStatusDismissed;
    private bool timelineDismissed;
    private bool failurePanelDismissed;
    private bool highContrastMode;
    private float userUiScale = 1f;
    private int visibleRecordIndex = -1;
    private int currentFlowStep = 1;
    private string destinationSearchQuery = string.Empty;
    private string satelliteSearchQuery = string.Empty;
    private string historySearchQuery = string.Empty;
    private bool flowScheduleSelected;
    private int calendarDisplayMonthOffset;
    private bool selectingOriginInDestinationStep;
    private bool selectingOriginInQueuePanel;
    private bool queueFlowOriginSelected;
    private bool queueFlowTargetSelected;
    private float nextLiveUiRefreshTime;
    private float nextSatelliteRecommendationRefreshTime;
    private float nextSatelliteListReorderTime;
    private float lastSatelliteSearchChangedTime;
    private float nextSatelliteRefreshPerfLogTime;
    private bool satelliteListPendingReorder;
    private ScrollRect satelliteListScrollRect;
    private string queueSelectedSatelliteName = string.Empty;
    private string queueOriginName = OrbitShareDestinations.Seoul;
    private string queueTargetName = OrbitShareDestinations.DefaultTargetName;
    private string queueAttachmentName = string.Empty;
    private int queueAttachmentSizeKb;
    private readonly List<MessageAttachmentRecord> queueAttachments = new List<MessageAttachmentRecord>();
    private int queueEditingIndex = -1;
    private bool queueScheduleOverride;
    private int queueScheduledDayOffset;
    private int queueScheduledHour;
    private int queueScheduledMinute;
    private int demoScenarioIndex;
    private bool demoModeActive;
    private bool composingQueuedMessageInFlow;
    private bool messageKoreanInputMode = true;
    private string lastToastMessage = string.Empty;
    private string lastConfirmSummaryText = string.Empty;
    private float toastVisibleUntil;
    private UnityAction pendingConfirmAction;
    private string pendingConfirmMessage = string.Empty;

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
        missionVisualController = FindObjectOfType<MissionVisualController>();
        uiFont = CreateUIFont();
        LoadUiPreferences();
        EnsureEventSystem();
        Canvas canvas = CreateCanvas();
        CreateTopPanel(canvas.transform);
        CreateSatelliteListPanel(canvas.transform);
        CreateRequestFlowPanel(canvas.transform);
        CreateMissionStatusPanel(canvas.transform);
        CreateTimelinePanel(canvas.transform);
        CreateTransferLogPanel(canvas.transform);
        CreateResultPanel(canvas.transform);
        CreateHistoryPanel(canvas.transform);
        CreateQueueMessagePanel(canvas.transform);
        CreateFailurePanel(canvas.transform);
        CreateSettingsPanel(canvas.transform);
        CreatePanelLauncher(canvas.transform);
        CreateCompass(canvas.transform);
        CreateToastPanel(canvas.transform);
        CreateConfirmDialogPanel(canvas.transform);

        EnablePanelResize();
        ApplyUiScale();
        LoadPanelLayout();
        AttachEvents();

        RefreshSatelliteList();
        RefreshAll();
    }

    private void Update()
    {
        RefreshImeCompositionMode();
        HandleKeyboardShortcuts();
        RefreshLiveConfirmTime();
        UpdateToastPanel();
        UpdateCompass();
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

        eventsAttached = false;
    }

    private void HandleSatelliteSelectionChanged(SatelliteData selected)
    {
        RefreshAll();
    }

    private void EnsureEventSystem()
    {
        if (FindObjectOfType<EventSystem>() != null)
        {
            return;
        }

        GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        DontDestroyOnLoad(eventSystem);
    }

    private Canvas CreateCanvas()
    {
        GameObject canvasObject = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        canvasScaler = scaler;
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1600f, 900f);
        scaler.matchWidthOrHeight = 0.5f;
        return canvas;
    }


    private void RefreshAll()
    {
        bool satelliteListHandledByLiveTicker = composingQueuedMessageInFlow
            && missionManager != null
            && IsActiveTransferPhase(missionManager.Phase);
        if (!satelliteListHandledByLiveTicker)
        {
            RefreshSatelliteList();
        }
        RefreshDetailPanel();
        RefreshSatelliteCompare();
        RefreshMissionPanel();
        RefreshSettingsPanel();
        RefreshFailurePanel();
        RefreshPanelLauncherButtons();
        ApplyVisualAccessibility();
    }


    private void RefreshTopButtons()
    {
        bool hasHistory = missionManager != null && missionManager.SentMessages.Count > 0;
        bool complete = missionManager != null && missionManager.Phase == MissionPhase.Complete;
        bool idle = missionManager != null && missionManager.Phase == MissionPhase.Idle;
        bool active = missionManager != null && missionManager.Phase != MissionPhase.Idle && missionManager.Phase != MissionPhase.Complete;
        bool activeTransfer = missionManager != null && IsActiveTransferPhase(missionManager.Phase) && !composingQueuedMessageInFlow;

        if (nextQueuedButton != null)
        {
            nextQueuedButton.gameObject.SetActive(false);
            nextQueuedButton.interactable = false;
        }
        if (historyToggleButton != null)
        {
            historyToggleButton.gameObject.SetActive(hasHistory);
            historyToggleButton.interactable = hasHistory;
            SetButtonVisual(historyToggleButton, hasHistory ? ButtonVisual.Secondary : ButtonVisual.Disabled);
        }
        if (resultToggleButton != null)
        {
            resultToggleButton.gameObject.SetActive(complete && resultDismissed);
            resultToggleButton.interactable = complete && hasHistory;
            SetButtonVisual(resultToggleButton, complete && hasHistory ? ButtonVisual.Secondary : ButtonVisual.Disabled);
        }
        if (newMessageButton != null)
        {
            newMessageButton.gameObject.SetActive(active || complete);
            newMessageButton.interactable = active || complete;
            SetButtonLabel(newMessageButton, active ? "작성" : "새 작성");
            SetButtonVisual(newMessageButton, activeTransfer ? ButtonVisual.Primary : active ? ButtonVisual.Secondary : complete ? ButtonVisual.Cta : ButtonVisual.Disabled);
        }
        LayoutTopActionButtons();
    }

    private void LayoutTopActionButtons()
    {
        float right = 0.985f;
        const float width = 0.082f;
        const float gap = 0.008f;
        Button[] buttons = { newMessageButton, resultToggleButton, historyToggleButton, statusStopButton };
        for (int i = 0; i < buttons.Length; i++)
        {
            Button button = buttons[i];
            if (button == null || !button.gameObject.activeSelf)
            {
                continue;
            }

            SetButtonAnchors(button, new Vector2(right - width, 0.18f), new Vector2(right, 0.82f));
            right -= width + gap;
        }
    }


    private Transform satelliteOrbitTransformLookup(string satelliteName)
    {
        Transform selectedTransform = satelliteManager != null ? satelliteManager.GetSatelliteTransform(satelliteName) : null;
        if (selectedTransform != null)
        {
            return selectedTransform;
        }
        return missionManager != null && missionManager.satelliteOrbit != null ? missionManager.satelliteOrbit.transform : null;
    }


    private void ShowLatestResultPanel()
    {
        if (missionManager == null || missionManager.SentMessages.Count == 0)
        {
            ShowToast("아직 전송 결과가 없습니다.");
            return;
        }

        visibleRecordIndex = missionManager.Phase == MissionPhase.Complete ? -1 : 0;
        resultDismissed = false;
        if (resultPanel != null)
        {
            resultPanel.gameObject.SetActive(true);
            resultPanel.SetAsLastSibling();
        }
        RefreshMissionPanel();
    }


    private void HandleKeyboardShortcuts()
    {
        if (IsTypingInInputField())
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (CloseTopOverlay())
            {
                return;
            }
            if (missionManager != null && missionManager.Phase == MissionPhase.Idle && currentFlowStep > 1)
            {
                GoToFlowStep(currentFlowStep - 1);
            }
        }
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (confirmDialogPanel != null && confirmDialogPanel.gameObject.activeSelf)
            {
                RunConfirmAction();
                return;
            }
            if (currentFlowStep < 5)
            {
                AdvanceFlowStep();
            }
            else
            {
                StartSelectedMission();
            }
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                GoToFlowStep(currentFlowStep - 1);
            }
            else
            {
                AdvanceFlowStep();
            }
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            ToggleHistoryPanel();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            ToggleSettingsPanel();
        }
    }

    private bool IsTypingInInputField()
    {
        if (EventSystem.current == null || EventSystem.current.currentSelectedGameObject == null)
        {
            return false;
        }
        return EventSystem.current.currentSelectedGameObject.GetComponent<InputField>() != null;
    }

    private void RefreshImeCompositionMode()
    {
        bool typing = IsTypingInInputField();
        Input.imeCompositionMode = typing ? IMECompositionMode.On : IMECompositionMode.Auto;
        if (typing)
        {
            Input.compositionCursorPos = Input.mousePosition;
        }
    }


}
