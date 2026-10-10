using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

[InitializeOnLoad]
public static class OrbitShareEditorAutomation
{
    private const string CaptureFlag = "OrbitShare.CapturePlayScreenshot";
    private const string CapturePathKey = "OrbitShare.CapturePath";
    private const string FastDemoStartedKey = "OrbitShare.FastDemoStarted";
    private const string StartTimeKey = "OrbitShare.CaptureStartTime";
    private const string ScreenshotRequestedKey = "OrbitShare.ScreenshotRequested";
    private const string FrameCaptureFlag = "OrbitShare.CaptureDemoFrames";
    private const string FrameIndexKey = "OrbitShare.FrameIndex";
    private const string LastFrameTimeKey = "OrbitShare.LastFrameTime";
    private const string FrameDirectoryKey = "OrbitShare.FrameDirectory";
    private const string FrameAgentStartedKey = "OrbitShare.FrameAgentStarted";
    private const string ValidationFlag = "OrbitShare.ValidateMissionFlow";
    private const string ValidationPathKey = "OrbitShare.ValidationPath";
    private const string ValidationCompletedRunsKey = "OrbitShare.ValidationCompletedRuns";
    private const string NormalDownlinkValidationFlag = "OrbitShare.ValidateNormalDownlink";
    private const string NormalDownlinkStartedKey = "OrbitShare.NormalDownlinkStarted";
    private const string NormalDownlinkSawDownlinkKey = "OrbitShare.NormalDownlinkSawDownlink";
    private const string NormalDownlinkEvidenceKey = "OrbitShare.NormalDownlinkEvidence";
    private const string NormalDownlinkValidationPathKey = "OrbitShare.NormalDownlinkValidationPath";
    private const string NormalDownlinkMissionStartTimeKey = "OrbitShare.NormalDownlinkMissionStartTime";
    private const string SameRegionNoIslValidationFlag = "OrbitShare.ValidateSameRegionNoIsl";
    private const string SameRegionNoIslStartedKey = "OrbitShare.SameRegionNoIslStarted";
    private const string SameRegionNoIslValidationPathKey = "OrbitShare.SameRegionNoIslValidationPath";
    private const string LauncherCaptureFlag = "OrbitShare.CaptureLauncherMenus";
    private const string LauncherCaptureDirectoryKey = "OrbitShare.LauncherCaptureDirectory";
    private const string LauncherCaptureIndexKey = "OrbitShare.LauncherCaptureIndex";
    private const string LauncherCapturePhaseKey = "OrbitShare.LauncherCapturePhase";
    private const string LauncherCaptureNextTimeKey = "OrbitShare.LauncherCaptureNextTime";
    private const string LauncherCaptureSeededKey = "OrbitShare.LauncherCaptureSeeded";
    private const long MinimumCaptureByteCount = 1024;
    private static readonly string[] RequiredDestinationNames = OrbitShareDestinations.Names;
    private static readonly int[] LauncherCaptureButtonIndices = { 0, 1, 2, 3, 4, 5 };
    private static readonly string[] LauncherCaptureLabels = { "메시지 작성", "위성망 추천", "전송 상태", "기록", "대기 메시지", "설정" };
    private static readonly string[] LauncherCapturePanelNames = { "RequestFlowPanel", "SatelliteListPanel", "CommunicationStatusPanel", "SentMessageHistoryPanel", "QueuedMessagePanel", "SettingsPanel" };
    private static readonly string[] LauncherCaptureFileNames = { "01-compose.png", "02-satellites.png", "03-status.png", "04-history.png", "05-queue.png", "06-settings.png" };
    private static int validationErrorLogCount;

    static OrbitShareEditorAutomation()
    {
        EditorApplication.update += Update;
        Application.logMessageReceived += HandleLogMessage;
    }

    public static void CapturePlayScreenshot()
    {
        string path = Path.GetFullPath("orbitshare-playmode.png");
        SessionState.SetBool(CaptureFlag, true);
        SessionState.SetBool(FastDemoStartedKey, false);
        SessionState.SetBool(ScreenshotRequestedKey, false);
        SessionState.SetString(CapturePathKey, path);
        SessionState.SetFloat(StartTimeKey, 0f);

        EditorSceneManager.OpenScene("Assets/Scenes/MainScene.unity");
        EditorApplication.isPlaying = true;
    }

    public static void CaptureDemoFrames()
    {
        string directory = Path.GetFullPath("DemoFrames");
        if (Directory.Exists(directory))
        {
            Directory.Delete(directory, true);
        }
        Directory.CreateDirectory(directory);

        SessionState.SetBool(CaptureFlag, false);
        SessionState.SetBool(FrameCaptureFlag, true);
        SessionState.SetBool(FastDemoStartedKey, false);
        SessionState.SetBool(FrameAgentStartedKey, false);
        SessionState.SetInt(FrameIndexKey, 0);
        SessionState.SetFloat(StartTimeKey, 0f);
        SessionState.SetFloat(LastFrameTimeKey, -1f);
        SessionState.SetString(FrameDirectoryKey, directory);

        EditorSceneManager.OpenScene("Assets/Scenes/MainScene.unity");
        EditorApplication.isPlaying = true;
    }

    public static void ValidateMissionFlow()
    {
        string path = Path.GetFullPath("orbitshare-validation.md");
        SessionState.SetBool(CaptureFlag, false);
        SessionState.SetBool(FrameCaptureFlag, false);
        SessionState.SetBool(ValidationFlag, true);
        SessionState.SetBool(SameRegionNoIslValidationFlag, false);
        SessionState.SetBool(FastDemoStartedKey, false);
        SessionState.SetFloat(StartTimeKey, 0f);
        SessionState.SetString(ValidationPathKey, path);
        SessionState.SetInt(ValidationCompletedRunsKey, 0);
        validationErrorLogCount = 0;

        EditorSceneManager.OpenScene("Assets/Scenes/MainScene.unity");
        EditorApplication.isPlaying = true;
    }

    public static void ValidateNormalDownlinkFlow()
    {
        string path = Path.GetFullPath("orbitshare-normal-downlink-validation.md");
        SessionState.SetBool(CaptureFlag, false);
        SessionState.SetBool(FrameCaptureFlag, false);
        SessionState.SetBool(ValidationFlag, false);
        SessionState.SetBool(LauncherCaptureFlag, false);
        SessionState.SetBool(SameRegionNoIslValidationFlag, false);
        SessionState.SetBool(NormalDownlinkValidationFlag, true);
        SessionState.SetBool(NormalDownlinkStartedKey, false);
        SessionState.SetBool(NormalDownlinkSawDownlinkKey, false);
        SessionState.SetString(NormalDownlinkEvidenceKey, string.Empty);
        SessionState.SetString(NormalDownlinkValidationPathKey, path);
        SessionState.SetFloat(NormalDownlinkMissionStartTimeKey, 0f);
        SessionState.SetFloat(StartTimeKey, 0f);

        EditorSceneManager.OpenScene("Assets/Scenes/MainScene.unity");
        EditorApplication.isPlaying = true;
    }

    public static void ValidateSameRegionNoIslFlow()
    {
        string path = Path.GetFullPath("orbitshare-same-region-no-isl-validation.md");
        SessionState.SetBool(CaptureFlag, false);
        SessionState.SetBool(FrameCaptureFlag, false);
        SessionState.SetBool(ValidationFlag, false);
        SessionState.SetBool(LauncherCaptureFlag, false);
        SessionState.SetBool(NormalDownlinkValidationFlag, false);
        SessionState.SetBool(SameRegionNoIslValidationFlag, true);
        SessionState.SetBool(SameRegionNoIslStartedKey, false);
        SessionState.SetString(SameRegionNoIslValidationPathKey, path);
        SessionState.SetFloat(StartTimeKey, 0f);

        EditorSceneManager.OpenScene("Assets/Scenes/MainScene.unity");
        EditorApplication.isPlaying = true;
    }

    public static void CaptureLauncherMenuScreenshots()
    {
        string directory = Path.GetFullPath("orbitshare-menu-captures");
        Directory.CreateDirectory(directory);
        File.WriteAllText(
            Path.Combine(directory, "report.md"),
            "# OrbitShare Launcher Menu Capture\n\n" +
            "- Generated: " + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n" +
            "- Second-click close is checked for panel buttons.\n" +
            "- Capture rows include capture/screen resolution, panel Rect, active-panel Intersection, active text evidence, and file size checks (>= " + MinimumCaptureByteCount + " bytes).\n\n");

        SessionState.SetBool(CaptureFlag, false);
        SessionState.SetBool(FrameCaptureFlag, false);
        SessionState.SetBool(ValidationFlag, false);
        SessionState.SetBool(LauncherCaptureFlag, true);
        SessionState.SetBool(LauncherCaptureSeededKey, false);
        SessionState.SetInt(LauncherCaptureIndexKey, 0);
        SessionState.SetInt(LauncherCapturePhaseKey, 0);
        SessionState.SetFloat(LauncherCaptureNextTimeKey, 0f);
        SessionState.SetFloat(StartTimeKey, 0f);
        SessionState.SetString(LauncherCaptureDirectoryKey, directory);

        EditorSceneManager.OpenScene("Assets/Scenes/MainScene.unity");
        EditorApplication.isPlaying = true;
    }

    private static void HandleLogMessage(string condition, string stackTrace, LogType type)
    {
        if (!SessionState.GetBool(ValidationFlag, false))
        {
            return;
        }

        if (type == LogType.Error || type == LogType.Exception)
        {
            validationErrorLogCount++;
        }
    }

    private static void Update()
    {
        if (SessionState.GetBool(FrameCaptureFlag, false))
        {
            UpdateFrameCapture();
            return;
        }

        if (SessionState.GetBool(ValidationFlag, false))
        {
            UpdateValidation();
            return;
        }

        if (SessionState.GetBool(NormalDownlinkValidationFlag, false))
        {
            UpdateNormalDownlinkValidation();
            return;
        }

        if (SessionState.GetBool(SameRegionNoIslValidationFlag, false))
        {
            UpdateSameRegionNoIslValidation();
            return;
        }

        if (SessionState.GetBool(LauncherCaptureFlag, false))
        {
            UpdateLauncherMenuCapture();
            return;
        }

        if (!SessionState.GetBool(CaptureFlag, false) || !EditorApplication.isPlaying)
        {
            return;
        }

        if (SessionState.GetFloat(StartTimeKey, 0f) <= 0f)
        {
            SessionState.SetFloat(StartTimeKey, (float)EditorApplication.timeSinceStartup);
        }

        float elapsed = (float)EditorApplication.timeSinceStartup - SessionState.GetFloat(StartTimeKey, 0f);
        string path = SessionState.GetString(CapturePathKey, Path.GetFullPath("orbitshare-playmode.png"));

        MissionManager missionManager = Object.FindObjectOfType<MissionManager>();
        if (elapsed > 1f && !SessionState.GetBool(FastDemoStartedKey, false) && missionManager != null)
        {
            missionManager.RunFastDemo();
            SessionState.SetBool(FastDemoStartedKey, true);
        }

        if (SessionState.GetBool(ScreenshotRequestedKey, false))
        {
            if (File.Exists(path))
            {
                SessionState.SetBool(CaptureFlag, false);
                EditorApplication.isPlaying = false;
                EditorApplication.delayCall += delegate
                {
                    Debug.Log("OrbitShare screenshot saved: " + path);
                    EditorApplication.Exit(0);
                };
            }
            else if (elapsed > 8f)
            {
                SessionState.SetBool(CaptureFlag, false);
                EditorApplication.isPlaying = false;
                EditorApplication.delayCall += delegate
                {
                    Debug.LogError("OrbitShare screenshot was requested but no file was written: " + path);
                    EditorApplication.Exit(1);
                };
            }
            return;
        }

        if (elapsed < 4.5f)
        {
            return;
        }

        bool captured = TryCaptureCameraPng(path, 1600, 900);
        SessionState.SetBool(CaptureFlag, false);
        EditorApplication.isPlaying = false;
        EditorApplication.delayCall += delegate
        {
            if (captured)
            {
                Debug.Log("OrbitShare screenshot saved: " + path);
                EditorApplication.Exit(0);
            }
            else
            {
                Debug.LogError("OrbitShare screenshot was requested but no file was written: " + path);
                EditorApplication.Exit(1);
            }
        };
    }

    private static void UpdateLauncherMenuCapture()
    {
        if (!EditorApplication.isPlaying)
        {
            return;
        }

        if (SessionState.GetFloat(StartTimeKey, 0f) <= 0f)
        {
            SessionState.SetFloat(StartTimeKey, (float)EditorApplication.timeSinceStartup);
        }

        float now = (float)EditorApplication.timeSinceStartup;
        float elapsed = now - SessionState.GetFloat(StartTimeKey, 0f);
        if (elapsed > 45f)
        {
            FinishLauncherMenuCapture(false, "Timed out before launcher menu captures completed.");
            return;
        }

        MissionManager missionManager = Object.FindObjectOfType<MissionManager>();
        if (missionManager == null || elapsed < 1f)
        {
            return;
        }

        if (!SessionState.GetBool(LauncherCaptureSeededKey, false))
        {
            SeedLauncherMenuCaptureData(missionManager);
            SessionState.SetBool(LauncherCaptureSeededKey, true);
        }

        int index = SessionState.GetInt(LauncherCaptureIndexKey, 0);
        if (index >= LauncherCaptureButtonIndices.Length)
        {
            AppendLauncherMenuReport("- 시연: PASS (패널이 아닌 시연 시작/중지 액션이라 닫기 검사는 제외)\n");
            FinishLauncherMenuCapture(true, "OrbitShare launcher menu screenshots saved.");
            return;
        }

        bool needsHistory = LauncherCaptureLabels[index] == "기록";
        if (needsHistory && missionManager.Phase != MissionPhase.Complete)
        {
            missionManager.RunFastDemo();
            return;
        }

        int phase = SessionState.GetInt(LauncherCapturePhaseKey, 0);
        if (phase == 0)
        {
            HideLauncherCapturePanels();
            if (!ClickLauncherButton(LauncherCaptureButtonIndices[index]))
            {
                FinishLauncherMenuCapture(false, "Could not click launcher button: " + LauncherCaptureLabels[index]);
                return;
            }
            SessionState.SetFloat(LauncherCaptureNextTimeKey, now + 0.45f);
            SessionState.SetInt(LauncherCapturePhaseKey, 1);
            return;
        }

        if (now < SessionState.GetFloat(LauncherCaptureNextTimeKey, 0f))
        {
            return;
        }

        string panelName = LauncherCapturePanelNames[index];
        if (phase == 1)
        {
            bool opened = IsNamedPanelActive(panelName);
            string directory = SessionState.GetString(LauncherCaptureDirectoryKey, Path.GetFullPath("orbitshare-menu-captures"));
            string path = Path.Combine(directory, LauncherCaptureFileNames[index]);
            DeleteExistingCaptureFile(path);
            bool captured = TryCaptureCameraPng(path, 1600, 900);
            bool captureFileReady = IsCaptureFileReady(path);
            AppendLauncherMenuReport("- " + LauncherCaptureLabels[index] + " 열기: " + Status(opened) + ", " + PanelSizeSummary(panelName) + ", " + PanelIntersectionSummary(panelName) + ", " + PanelTextEvidence(panelName) + ", captureWrite=" + Status(captured) + ", " + CaptureFileSummary(path) + " (`" + LauncherCaptureFileNames[index] + "`)\n");
            bool requestFlowStepsReady = true;
            if (index == 0 && opened)
            {
                requestFlowStepsReady = CaptureRequestFlowStepImages(directory);
            }
            if (!opened || !captured || !captureFileReady || !requestFlowStepsReady)
            {
                FinishLauncherMenuCapture(false, "Launcher panel capture failed: " + LauncherCaptureLabels[index]);
                return;
            }

            if (!ClickLauncherButton(LauncherCaptureButtonIndices[index]))
            {
                FinishLauncherMenuCapture(false, "Could not second-click launcher button: " + LauncherCaptureLabels[index]);
                return;
            }
            SessionState.SetFloat(LauncherCaptureNextTimeKey, now + 0.25f);
            SessionState.SetInt(LauncherCapturePhaseKey, 2);
            return;
        }

        if (phase == 2)
        {
            bool closed = !IsNamedPanelActive(panelName);
            AppendLauncherMenuReport("  - 두 번째 클릭 닫기: " + Status(closed) + "\n");
            if (!closed)
            {
                FinishLauncherMenuCapture(false, "Second-click did not close panel: " + LauncherCaptureLabels[index]);
                return;
            }

            SessionState.SetInt(LauncherCaptureIndexKey, index + 1);
            SessionState.SetInt(LauncherCapturePhaseKey, 0);
        }
    }

    private static void SeedLauncherMenuCaptureData(MissionManager missionManager)
    {
        if (missionManager == null)
        {
            return;
        }

        missionManager.SetMessageText("시연 캡처용 메시지");
        missionManager.SelectTarget(RequiredDestinationNames[0]);
        string errorMessage;
        missionManager.QueueMessageFromOrigin("대기 메시지 카드 확인 1", OrbitShareDestinations.Seoul, OrbitShareDestinations.NewYork, 0, 0, 0, new List<MessageAttachmentRecord>(), out errorMessage);
        missionManager.QueueMessageFromOrigin("대기 메시지 카드 확인 2", OrbitShareDestinations.Sydney, OrbitShareDestinations.London, 0, 0, 0, new List<MessageAttachmentRecord>(), out errorMessage);
        SatelliteManager satelliteManager = Object.FindObjectOfType<SatelliteManager>();
        if (satelliteManager != null)
        {
            SatelliteData best = satelliteManager.FindBestSatellite(RequiredDestinationNames[0], missionManager.RequestedSendDelayMinutes, missionManager.MessageSizeKb);
            if (best != null)
            {
                satelliteManager.SelectSatellite(best.satelliteName);
            }
        }
    }

    private static bool CaptureRequestFlowStepImages(string directory)
    {
        bool allCaptured = true;
        string[] labels = { "시간", "메시지", "위치", "위성망", "확인" };
        string[] files = { "flow-01-time.png", "flow-02-message.png", "flow-03-destination.png", "flow-04-satellite.png", "flow-05-confirm.png" };
        for (int i = 0; i < labels.Length; i++)
        {
            string path = Path.Combine(directory, files[i]);
            DeleteExistingCaptureFile(path);
            bool clicked = ClickRequestFlowStepButton(labels[i]);
            bool captured = clicked && TryCaptureCameraPng(path, 1600, 900);
            allCaptured = allCaptured && clicked && captured && IsCaptureFileReady(path);
            AppendLauncherMenuReport("  - 작성 단계 " + labels[i] + ": " + Status(clicked) + ", " + PanelTextEvidence("RequestFlowPanel") + ", captureWrite=" + Status(captured) + ", " + CaptureFileSummary(path) + " (`" + files[i] + "`)\n");
            if (i == 0 && clicked)
            {
                string calendarPath = Path.Combine(directory, "flow-01-calendar.png");
                DeleteExistingCaptureFile(calendarPath);
                bool openedCalendar = ClickVisibleButton("직접 설정");
                bool capturedCalendar = openedCalendar && TryCaptureCameraPng(calendarPath, 1600, 900);
                allCaptured = allCaptured && openedCalendar && capturedCalendar && IsCaptureFileReady(calendarPath);
                AppendLauncherMenuReport("  - 작성 단계 시간 달력: " + Status(openedCalendar) + ", " + PanelTextEvidence("RequestFlowPanel") + ", captureWrite=" + Status(capturedCalendar) + ", " + CaptureFileSummary(calendarPath) + " (`flow-01-calendar.png`)\n");
            }
        }
        return allCaptured;
    }

    private static bool ClickRequestFlowStepButton(string label)
    {
        GameObject requestPanel = GameObject.Find("RequestFlowPanel");
        if (requestPanel == null)
        {
            return false;
        }

        Button[] buttons = requestPanel.GetComponentsInChildren<Button>(true);
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == null || buttons[i].transform.parent != requestPanel.transform)
            {
                continue;
            }
            Text text = buttons[i].GetComponentInChildren<Text>();
            if (text == null)
            {
                continue;
            }
            string normalized = text.text.Replace("▶", string.Empty).Replace("✓", string.Empty).Trim();
            if (normalized == label)
            {
                buttons[i].onClick.Invoke();
                return true;
            }
        }
        return false;
    }

    private static bool ClickVisibleButton(string label)
    {
        Button[] buttons = Object.FindObjectsOfType<Button>(true);
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == null || !buttons[i].gameObject.activeInHierarchy)
            {
                continue;
            }
            Text text = buttons[i].GetComponentInChildren<Text>();
            if (text == null)
            {
                continue;
            }
            string normalized = text.text.Replace("▶", string.Empty).Replace("✓", string.Empty).Trim();
            if (normalized == label)
            {
                buttons[i].onClick.Invoke();
                return true;
            }
        }
        return false;
    }

    private static bool ClickLauncherButton(int buttonIndex)
    {
        GameObject launcherObject = GameObject.Find("PanelLauncher");
        if (launcherObject == null)
        {
            return false;
        }

        Button[] buttons = launcherObject.GetComponentsInChildren<Button>(true);
        List<Button> directButtons = new List<Button>();
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] != null && buttons[i].transform.parent == launcherObject.transform)
            {
                directButtons.Add(buttons[i]);
            }
        }
        directButtons.Sort(delegate (Button left, Button right)
        {
            RectTransform leftRect = left.GetComponent<RectTransform>();
            RectTransform rightRect = right.GetComponent<RectTransform>();
            float leftX = leftRect != null ? leftRect.anchorMin.x : 0f;
            float rightX = rightRect != null ? rightRect.anchorMin.x : 0f;
            return leftX.CompareTo(rightX);
        });

        if (buttonIndex < 0 || buttonIndex >= directButtons.Count)
        {
            return false;
        }

        directButtons[buttonIndex].onClick.Invoke();
        return true;
    }

    private static void HideLauncherCapturePanels()
    {
        string[] panelNames =
        {
            "RequestFlowPanel",
            "SatelliteListPanel",
            "CommunicationStatusPanel",
            "CommunicationTimelinePanel",
            "SentMessageHistoryPanel",
            "QueuedMessagePanel",
            "SettingsPanel",
            "CommunicationResultPanel",
            "TransferLogPanel",
            "FailurePanel",
            "ConfirmDialog",
            "ToastPanel"
        };

        for (int i = 0; i < panelNames.Length; i++)
        {
            GameObject panel = GameObject.Find(panelNames[i]);
            if (panel != null)
            {
                panel.SetActive(false);
            }
        }
    }

    private static bool IsNamedPanelActive(string panelName)
    {
        GameObject panel = GameObject.Find(panelName);
        return panel != null && panel.activeInHierarchy;
    }

    private static string PanelSizeSummary(string panelName)
    {
        GameObject panel = GameObject.Find(panelName);
        if (panel == null)
        {
            return "panel=Missing";
        }

        RectTransform rectTransform = panel.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            return "panel=NoRect";
        }

        Rect rect = rectTransform.rect;
        Rect screenRect;
        string screenRectSummary = TryGetPanelScreenRect(panelName, out screenRect) ? FormatScreenRect(screenRect) : "Missing";
        return "capture=1600x900, screen=" + Screen.width + "x" + Screen.height + ", canvasRef=" + CanvasReferenceSummary(rectTransform) + ", panel=" + Mathf.RoundToInt(rect.width) + "x" + Mathf.RoundToInt(rect.height) + ", screenRect=" + screenRectSummary;
    }

    private static string PanelIntersectionSummary(string panelName)
    {
        Rect panelRect;
        if (!TryGetPanelScreenRect(panelName, out panelRect))
        {
            return "activePeerPanels=0, intersections=SKIP(panelRectMissing)";
        }

        int activePeerCount = 0;
        StringBuilder overlaps = new StringBuilder();
        for (int i = 0; i < LauncherCapturePanelNames.Length; i++)
        {
            string otherName = LauncherCapturePanelNames[i];
            if (otherName == panelName)
            {
                continue;
            }

            GameObject otherPanel = GameObject.Find(otherName);
            if (otherPanel == null || !otherPanel.activeInHierarchy)
            {
                continue;
            }

            activePeerCount++;
            Rect otherRect;
            if (!TryGetPanelScreenRect(otherName, out otherRect))
            {
                continue;
            }

            Rect intersection = RectIntersection(panelRect, otherRect);
            if (intersection.width <= 0f || intersection.height <= 0f)
            {
                continue;
            }

            if (overlaps.Length > 0)
            {
                overlaps.Append("; ");
            }
            overlaps.Append(otherName)
                .Append("=")
                .Append(Mathf.RoundToInt(intersection.width))
                .Append("x")
                .Append(Mathf.RoundToInt(intersection.height));
        }

        if (overlaps.Length == 0)
        {
            return "activePeerPanels=" + activePeerCount + ", intersections=PASS(none)";
        }

        return "activePeerPanels=" + activePeerCount + ", intersections=FAIL(" + overlaps + ")";
    }

    private static bool TryGetPanelScreenRect(string panelName, out Rect screenRect)
    {
        screenRect = new Rect();
        GameObject panel = GameObject.Find(panelName);
        if (panel == null)
        {
            return false;
        }

        RectTransform rectTransform = panel.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            return false;
        }

        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        Camera camera = RectTransformCamera(rectTransform);
        float minX = float.MaxValue;
        float minY = float.MaxValue;
        float maxX = float.MinValue;
        float maxY = float.MinValue;
        for (int i = 0; i < corners.Length; i++)
        {
            Vector2 point = RectTransformUtility.WorldToScreenPoint(camera, corners[i]);
            minX = Mathf.Min(minX, point.x);
            minY = Mathf.Min(minY, point.y);
            maxX = Mathf.Max(maxX, point.x);
            maxY = Mathf.Max(maxY, point.y);
        }

        screenRect = Rect.MinMaxRect(minX, minY, maxX, maxY);
        return true;
    }

    private static Camera RectTransformCamera(RectTransform rectTransform)
    {
        Canvas canvas = rectTransform.GetComponentInParent<Canvas>();
        if (canvas == null || canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            return null;
        }

        return canvas.worldCamera != null ? canvas.worldCamera : Camera.main;
    }

    private static Rect RectIntersection(Rect first, Rect second)
    {
        float minX = Mathf.Max(first.xMin, second.xMin);
        float minY = Mathf.Max(first.yMin, second.yMin);
        float maxX = Mathf.Min(first.xMax, second.xMax);
        float maxY = Mathf.Min(first.yMax, second.yMax);
        if (maxX <= minX || maxY <= minY)
        {
            return new Rect(minX, minY, 0f, 0f);
        }

        return Rect.MinMaxRect(minX, minY, maxX, maxY);
    }

    private static string FormatScreenRect(Rect rect)
    {
        return "(" + Mathf.RoundToInt(rect.xMin) + "," + Mathf.RoundToInt(rect.yMin) + "," + Mathf.RoundToInt(rect.width) + "," + Mathf.RoundToInt(rect.height) + ")";
    }

    private static string CanvasReferenceSummary(RectTransform rectTransform)
    {
        CanvasScaler scaler = rectTransform.GetComponentInParent<CanvasScaler>();
        if (scaler == null)
        {
            scaler = Object.FindObjectOfType<CanvasScaler>();
        }

        if (scaler == null)
        {
            return "Missing";
        }

        return Mathf.RoundToInt(scaler.referenceResolution.x) + "x" + Mathf.RoundToInt(scaler.referenceResolution.y);
    }

    private static string PanelTextEvidence(string panelName)
    {
        GameObject panel = GameObject.Find(panelName);
        if (panel == null || !panel.activeInHierarchy)
        {
            return "textEvidence=Missing";
        }

        Text[] texts = panel.GetComponentsInChildren<Text>(false);
        for (int i = 0; i < texts.Length; i++)
        {
            if (texts[i] == null || string.IsNullOrWhiteSpace(texts[i].text))
            {
                continue;
            }

            return "textEvidence=" + OneLineEvidence(texts[i].text);
        }

        return "textEvidence=Empty";
    }

    private static bool IsCaptureFileReady(string path)
    {
        return CaptureFileByteCount(path) >= MinimumCaptureByteCount;
    }

    private static string CaptureFileSummary(string path)
    {
        try
        {
            if (!File.Exists(path))
            {
                return "fileExists=FAIL, nonEmpty=FAIL, sizeOk=FAIL(>=" + MinimumCaptureByteCount + "), bytes=0";
            }

            long byteCount = new FileInfo(path).Length;
            return "fileExists=PASS, nonEmpty=" + Status(byteCount > 0) + ", sizeOk=" + Status(byteCount >= MinimumCaptureByteCount) + "(>=" + MinimumCaptureByteCount + "), bytes=" + byteCount;
        }
        catch (System.Exception exception)
        {
            return "fileCheck=FAIL (" + exception.GetType().Name + ")";
        }
    }

    private static long CaptureFileByteCount(string path)
    {
        try
        {
            return File.Exists(path) ? new FileInfo(path).Length : 0;
        }
        catch (System.Exception)
        {
            return 0;
        }
    }

    private static void DeleteExistingCaptureFile(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch (System.Exception exception)
        {
            Debug.LogWarning("Could not delete old launcher capture file: " + path + " (" + exception.Message + ")");
        }
    }

    private static void AppendLauncherMenuReport(string line)
    {
        string directory = SessionState.GetString(LauncherCaptureDirectoryKey, Path.GetFullPath("orbitshare-menu-captures"));
        Directory.CreateDirectory(directory);
        File.AppendAllText(Path.Combine(directory, "report.md"), line);
    }

    private static void FinishLauncherMenuCapture(bool success, string message)
    {
        SessionState.SetBool(LauncherCaptureFlag, false);
        EditorApplication.isPlaying = false;
        EditorApplication.delayCall += delegate
        {
            if (success)
            {
                Debug.Log(message);
                EditorApplication.Exit(0);
            }
            else
            {
                Debug.LogError(message);
                EditorApplication.Exit(1);
            }
        };
    }

    private static bool TryCaptureCameraPng(string path, int width, int height)
    {
        Camera camera = Camera.main != null ? Camera.main : Object.FindObjectOfType<Camera>();
        if (camera == null)
        {
            return false;
        }

        RenderTexture previousTarget = camera.targetTexture;
        RenderTexture previousActive = RenderTexture.active;
        RenderTexture renderTexture = null;
        Texture2D texture = null;
        Canvas[] canvases = Object.FindObjectsOfType<Canvas>();
        RenderMode[] canvasModes = new RenderMode[canvases.Length];
        Camera[] canvasCameras = new Camera[canvases.Length];
        float[] canvasPlaneDistances = new float[canvases.Length];

        try
        {
            for (int i = 0; i < canvases.Length; i++)
            {
                canvasModes[i] = canvases[i].renderMode;
                canvasCameras[i] = canvases[i].worldCamera;
                canvasPlaneDistances[i] = canvases[i].planeDistance;
                if (canvases[i].renderMode == RenderMode.ScreenSpaceOverlay)
                {
                    canvases[i].renderMode = RenderMode.ScreenSpaceCamera;
                    canvases[i].worldCamera = camera;
                    canvases[i].planeDistance = 1f;
                }
            }

            renderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
            texture = new Texture2D(width, height, TextureFormat.RGB24, false);
            camera.targetTexture = renderTexture;
            RenderTexture.active = renderTexture;
            camera.Render();
            texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            texture.Apply();
            File.WriteAllBytes(path, texture.EncodeToPNG());
            return File.Exists(path);
        }
        catch (System.Exception exception)
        {
            Debug.LogError("OrbitShare camera screenshot failed: " + exception.Message);
            return false;
        }
        finally
        {
            for (int i = 0; i < canvases.Length; i++)
            {
                if (canvases[i] != null)
                {
                    canvases[i].renderMode = canvasModes[i];
                    canvases[i].worldCamera = canvasCameras[i];
                    canvases[i].planeDistance = canvasPlaneDistances[i];
                }
            }

            camera.targetTexture = previousTarget;
            RenderTexture.active = previousActive;
            if (renderTexture != null)
            {
                Object.DestroyImmediate(renderTexture);
            }
            if (texture != null)
            {
                Object.DestroyImmediate(texture);
            }
        }
    }

    private static void UpdateFrameCapture()
    {
        if (!EditorApplication.isPlaying)
        {
            return;
        }

        if (!SessionState.GetBool(FrameAgentStartedKey, false))
        {
            string directory = SessionState.GetString(FrameDirectoryKey, Path.GetFullPath("DemoFrames"));
            GameObject agentObject = new GameObject("OrbitShareFrameCaptureAgent");
            OrbitShareFrameCaptureAgent agent = agentObject.AddComponent<OrbitShareFrameCaptureAgent>();
            agent.Initialize(directory);
            SessionState.SetBool(FrameAgentStartedKey, true);
            SessionState.SetFloat(StartTimeKey, (float)EditorApplication.timeSinceStartup);
            return;
        }

        float elapsed = (float)EditorApplication.timeSinceStartup - SessionState.GetFloat(StartTimeKey, (float)EditorApplication.timeSinceStartup);
        if (elapsed > 20f)
        {
            SessionState.SetBool(FrameCaptureFlag, false);
            EditorApplication.isPlaying = false;
            EditorApplication.delayCall += delegate
            {
                Debug.LogError("OrbitShare frame capture timed out before all frames were written.");
                EditorApplication.Exit(1);
            };
        }

        return;
    }

    private static void UpdateValidation()
    {
        if (!EditorApplication.isPlaying)
        {
            return;
        }

        if (SessionState.GetFloat(StartTimeKey, 0f) <= 0f)
        {
            SessionState.SetFloat(StartTimeKey, (float)EditorApplication.timeSinceStartup);
        }

        float elapsed = (float)EditorApplication.timeSinceStartup - SessionState.GetFloat(StartTimeKey, 0f);
        MissionManager missionManager = Object.FindObjectOfType<MissionManager>();
        SatelliteManager satelliteManager = Object.FindObjectOfType<SatelliteManager>();
        RealSatelliteVisualizer realSatelliteVisualizer = Object.FindObjectOfType<RealSatelliteVisualizer>();
        int completedRuns = SessionState.GetInt(ValidationCompletedRunsKey, 0);
        if (elapsed > 1f && missionManager != null && missionManager.Phase != MissionPhase.Complete)
        {
            if (!RealSatelliteFleetReadyForValidation(realSatelliteVisualizer, satelliteManager) && elapsed < 30f)
            {
                return;
            }

            if (!SessionState.GetBool(FastDemoStartedKey, false))
            {
                missionManager.SelectOrigin(OrbitShareDestinations.London);
                missionManager.SelectTarget(OrbitShareDestinations.DefaultTargetName);
            }
            missionManager.RunFastDemo();
            SessionState.SetBool(FastDemoStartedKey, true);
        }

        if (missionManager != null && missionManager.Phase == MissionPhase.Complete && completedRuns < 2)
        {
            completedRuns++;
            SessionState.SetInt(ValidationCompletedRunsKey, completedRuns);
            if (completedRuns == 1)
            {
                missionManager.ResetMission();
                SessionState.SetBool(FastDemoStartedKey, false);
                return;
            }
        }

        if (elapsed < 90f && completedRuns < 2)
        {
            return;
        }

        string path = SessionState.GetString(ValidationPathKey, Path.GetFullPath("orbitshare-validation.md"));
        bool passed = WriteValidationReport(path, elapsed);
        SessionState.SetBool(ValidationFlag, false);
        EditorApplication.isPlaying = false;
        EditorApplication.delayCall += delegate
        {
            if (passed)
            {
                Debug.Log("OrbitShare validation report saved: " + path);
                EditorApplication.Exit(0);
            }
            else
            {
                Debug.LogError("OrbitShare validation failed. Report saved: " + path);
                EditorApplication.Exit(1);
            }
        };
    }

    private static void UpdateNormalDownlinkValidation()
    {
        if (!EditorApplication.isPlaying)
        {
            return;
        }

        if (SessionState.GetFloat(StartTimeKey, 0f) <= 0f)
        {
            SessionState.SetFloat(StartTimeKey, (float)EditorApplication.timeSinceStartup);
        }

        float elapsed = (float)EditorApplication.timeSinceStartup - SessionState.GetFloat(StartTimeKey, 0f);
        MissionManager missionManager = Object.FindObjectOfType<MissionManager>();
        SatelliteManager satelliteManager = Object.FindObjectOfType<SatelliteManager>();
        RealSatelliteVisualizer realSatelliteVisualizer = Object.FindObjectOfType<RealSatelliteVisualizer>();
        if (missionManager == null || satelliteManager == null || realSatelliteVisualizer == null)
        {
            if (elapsed > 12f)
            {
                FinishNormalDownlinkValidation(false, "필수 매니저를 찾지 못했습니다.");
            }
            return;
        }

        if (realSatelliteVisualizer.dataSet != RealSatelliteDataSet.Starlink)
        {
            realSatelliteVisualizer.RefreshStarlink(false, realSatelliteVisualizer.groupDisplayLimit);
        }

        if (!RealSatelliteFleetReadyForValidation(realSatelliteVisualizer, satelliteManager) && elapsed < 45f)
        {
            return;
        }

        if (!SessionState.GetBool(NormalDownlinkStartedKey, false))
        {
            string originName;
            string targetName;
            string satelliteName;
            string evidence;
            if (!TryFindImmediateNormalDownlinkScenario(satelliteManager, out originName, out targetName, out satelliteName, out evidence))
            {
                if (elapsed < 55f)
                {
                    return;
                }
                FinishNormalDownlinkValidation(false, "즉시 업링크/다운링크 가능한 STARLINK 경로를 찾지 못했습니다.");
                return;
            }

            missionManager.ResetMission();
            missionManager.SetDemoRouteReplayEnabled(false);
            missionManager.SelectOrigin(originName);
            missionManager.SelectTarget(targetName);
            missionManager.SetSendDelayMinutes(0);
            missionManager.SetMessageText("일반 전송 다운링크 검증");
            satelliteManager.SelectSatellite(satelliteName);
            missionManager.ReserveMission();
            missionManager.StartMission();
            SessionState.SetBool(NormalDownlinkStartedKey, true);
            SessionState.SetString(NormalDownlinkEvidenceKey, evidence);
            SessionState.SetFloat(NormalDownlinkMissionStartTimeKey, (float)EditorApplication.timeSinceStartup);
            return;
        }

        if (missionManager.CommunicationStep == SatelliteCommunicationStep.Downlink
            || missionManager.CommunicationStep == SatelliteCommunicationStep.FinalReceive
            || missionManager.CommunicationStep == SatelliteCommunicationStep.Complete)
        {
            SessionState.SetBool(NormalDownlinkSawDownlinkKey, true);
        }

        if (missionManager.Phase == MissionPhase.Complete && missionManager.CommunicationStep == SatelliteCommunicationStep.Complete)
        {
            FinishNormalDownlinkValidation(SessionState.GetBool(NormalDownlinkSawDownlinkKey, false), "일반 전송 완료");
            return;
        }

        float missionElapsed = (float)EditorApplication.timeSinceStartup - SessionState.GetFloat(NormalDownlinkMissionStartTimeKey, (float)EditorApplication.timeSinceStartup);
        if (missionElapsed > 150f)
        {
            FinishNormalDownlinkValidation(
                false,
                "시간 초과: phase=" + missionManager.Phase
                    + ", step=" + missionManager.CommunicationStep
                    + ", uplink=" + Mathf.RoundToInt(missionManager.ObservationProgress * 100f) + "%"
                    + ", flow=" + Mathf.RoundToInt(missionManager.TransferProgress * 100f) + "%"
                    + ", remaining=" + missionManager.RemainingTransferSeconds.ToString("0.0"));
        }
    }

    private static void UpdateSameRegionNoIslValidation()
    {
        if (!EditorApplication.isPlaying)
        {
            return;
        }

        if (SessionState.GetFloat(StartTimeKey, 0f) <= 0f)
        {
            SessionState.SetFloat(StartTimeKey, (float)EditorApplication.timeSinceStartup);
        }

        float elapsed = (float)EditorApplication.timeSinceStartup - SessionState.GetFloat(StartTimeKey, 0f);
        MissionManager missionManager = Object.FindObjectOfType<MissionManager>();
        SatelliteManager satelliteManager = Object.FindObjectOfType<SatelliteManager>();
        RealSatelliteVisualizer realSatelliteVisualizer = Object.FindObjectOfType<RealSatelliteVisualizer>();
        if (missionManager == null || satelliteManager == null || realSatelliteVisualizer == null)
        {
            if (elapsed > 12f)
            {
                FinishSameRegionNoIslValidation(false, "필수 매니저를 찾지 못했습니다.");
            }
            return;
        }

        if (!RealSatelliteFleetReadyForValidation(realSatelliteVisualizer, satelliteManager) && elapsed < 45f)
        {
            return;
        }

        if (!SessionState.GetBool(SameRegionNoIslStartedKey, false))
        {
            missionManager.ResetMission();
            missionManager.SelectOrigin(OrbitShareDestinations.Seoul);
            missionManager.SelectTarget(OrbitShareDestinations.Seoul);
            missionManager.SetSendDelayMinutes(0);
            missionManager.SetMessageText("동일 지역 ISL 생략 검증");
            SatelliteData best = satelliteManager.FindBestSatellite(OrbitShareDestinations.Seoul, OrbitShareDestinations.Seoul, 0, 1);
            if (best != null)
            {
                satelliteManager.SelectSatellite(best.satelliteName);
            }
            missionManager.ReserveMission();
            missionManager.StartMission();
            SessionState.SetBool(SameRegionNoIslStartedKey, true);
            return;
        }

        if (missionManager.Phase != MissionPhase.Complete)
        {
            missionManager.RunFastDemo();
        }

        if (missionManager.Phase == MissionPhase.Complete && missionManager.CommunicationStep == SatelliteCommunicationStep.Complete)
        {
            bool eventHasIsl = false;
            for (int i = 0; i < missionManager.MissionEventLog.Count; i++)
            {
                if (!string.IsNullOrEmpty(missionManager.MissionEventLog[i]) && missionManager.MissionEventLog[i].Contains("ISL 전달 시작"))
                {
                    eventHasIsl = true;
                    break;
                }
            }
            bool passed = !missionManager.ShouldUseInterSatelliteLink
                && !eventHasIsl
                && missionManager.InterSatelliteProgress >= 0.99f
                && missionManager.DownlinkProgress >= 0.99f
                && missionManager.CommunicationFlowSummary.IndexOf("ISL", System.StringComparison.OrdinalIgnoreCase) < 0;
            FinishSameRegionNoIslValidation(passed, passed ? "동일 지역 전송에서 ISL 생략 확인" : "동일 지역 전송에 ISL 흔적이 남았습니다.");
            return;
        }

        if (elapsed > 90f)
        {
            FinishSameRegionNoIslValidation(false, "시간 초과: phase=" + missionManager.Phase + ", step=" + missionManager.CommunicationStep);
        }
    }

    private static void FinishSameRegionNoIslValidation(bool passed, string message)
    {
        string path = SessionState.GetString(SameRegionNoIslValidationPathKey, Path.GetFullPath("orbitshare-same-region-no-isl-validation.md"));
        MissionManager missionManager = Object.FindObjectOfType<MissionManager>();
        StringBuilder report = new StringBuilder();
        report.AppendLine("# OrbitShare Same Region No-ISL Validation");
        report.AppendLine();
        report.AppendLine("Result: " + (passed ? "PASS" : "FAIL"));
        report.AppendLine("Message: " + message);
        report.AppendLine("Phase: " + (missionManager != null ? missionManager.Phase.ToString() : "Missing"));
        report.AppendLine("Step: " + (missionManager != null ? missionManager.CommunicationStep.ToString() : "Missing"));
        report.AppendLine("Uses ISL: " + (missionManager != null && missionManager.ShouldUseInterSatelliteLink));
        report.AppendLine("Flow: " + (missionManager != null ? missionManager.CommunicationFlowSummary : "Missing"));
        File.WriteAllText(path, report.ToString());

        SessionState.SetBool(SameRegionNoIslValidationFlag, false);
        EditorApplication.isPlaying = false;
        EditorApplication.delayCall += delegate
        {
            if (passed)
            {
                Debug.Log("OrbitShare same-region no-ISL validation saved: " + path);
                EditorApplication.Exit(0);
            }
            else
            {
                Debug.LogError("OrbitShare same-region no-ISL validation failed: " + path);
                EditorApplication.Exit(1);
            }
        };
    }

    private static bool TryFindImmediateNormalDownlinkScenario(SatelliteManager satelliteManager, out string originName, out string targetName, out string satelliteName, out string evidence)
    {
        originName = string.Empty;
        targetName = string.Empty;
        satelliteName = string.Empty;
        evidence = string.Empty;
        if (satelliteManager == null)
        {
            return false;
        }

        for (int pass = 0; pass < 2; pass++)
        {
            bool allowSameEndpoint = pass > 0;
            int payloadKb = 1;
            int bestAvailableSeconds = -1;
            for (int originIndex = 0; originIndex < OrbitShareDestinations.Names.Length; originIndex++)
            {
                string origin = OrbitShareDestinations.Names[originIndex];
                for (int targetIndex = 0; targetIndex < OrbitShareDestinations.Names.Length; targetIndex++)
                {
                    string target = OrbitShareDestinations.Names[targetIndex];
                    if (!allowSameEndpoint && origin == target)
                    {
                        continue;
                    }
                    List<SatelliteRouteEstimateRecord> estimates = satelliteManager.GetRankedRouteEstimates(SatelliteListFilter.Recommended, origin, target, 0, payloadKb);
                    for (int i = 0; i < estimates.Count; i++)
                    {
                        SatelliteRouteEstimateRecord estimate = estimates[i];
                        if (estimate == null || !estimate.canServe || !estimate.satelliteName.StartsWith("REAL-", System.StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }
                        if (estimate.originWindowSeconds > 0 || estimate.targetWindowSeconds > 0)
                        {
                            continue;
                        }
                        int availableSeconds = Mathf.Min(
                            Mathf.Max(0, estimate.originAvailableForSeconds),
                            Mathf.Max(0, estimate.targetAvailableForSeconds));
                        if (availableSeconds <= 25)
                        {
                            continue;
                        }
                        if (availableSeconds > bestAvailableSeconds)
                        {
                            bestAvailableSeconds = availableSeconds;
                            originName = origin;
                            targetName = target;
                            satelliteName = estimate.satelliteName;
                            evidence = origin + " → " + target
                                + " via " + satelliteName
                                + ", available=" + availableSeconds
                                + "s, delivery=" + estimate.estimatedDeliverySeconds
                                + "s";
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(satelliteName))
            {
                return true;
            }
        }

        return false;
    }

    private static void FinishNormalDownlinkValidation(bool passed, string message)
    {
        string path = SessionState.GetString(NormalDownlinkValidationPathKey, Path.GetFullPath("orbitshare-normal-downlink-validation.md"));
        MissionManager missionManager = Object.FindObjectOfType<MissionManager>();
        RealSatelliteVisualizer realSatelliteVisualizer = Object.FindObjectOfType<RealSatelliteVisualizer>();
        StringBuilder report = new StringBuilder();
        report.AppendLine("# OrbitShare Normal Downlink Validation");
        report.AppendLine();
        report.AppendLine("Result: " + (passed ? "PASS" : "FAIL"));
        report.AppendLine("Message: " + message);
        report.AppendLine("Dataset: " + (realSatelliteVisualizer != null ? realSatelliteVisualizer.dataSet.ToString() : "Missing"));
        report.AppendLine("Evidence: " + SessionState.GetString(NormalDownlinkEvidenceKey, string.Empty));
        report.AppendLine("Saw downlink: " + Status(SessionState.GetBool(NormalDownlinkSawDownlinkKey, false)));
        report.AppendLine("Phase: " + (missionManager != null ? missionManager.Phase.ToString() : "Missing"));
        report.AppendLine("Step: " + (missionManager != null ? missionManager.CommunicationStep.ToString() : "Missing"));
        report.AppendLine("Uplink progress: " + (missionManager != null ? Mathf.RoundToInt(missionManager.ObservationProgress * 100f).ToString() + "%" : "Missing"));
        report.AppendLine("Flow progress: " + (missionManager != null ? Mathf.RoundToInt(missionManager.TransferProgress * 100f).ToString() + "%" : "Missing"));
        report.AppendLine("Relay: " + (missionManager != null ? missionManager.DestinationRelaySatelliteName : "Missing"));
        File.WriteAllText(path, report.ToString());

        SessionState.SetBool(NormalDownlinkValidationFlag, false);
        EditorApplication.isPlaying = false;
        EditorApplication.delayCall += delegate
        {
            if (passed)
            {
                Debug.Log("OrbitShare normal downlink validation saved: " + path);
                EditorApplication.Exit(0);
            }
            else
            {
                Debug.LogError("OrbitShare normal downlink validation failed: " + path);
                EditorApplication.Exit(1);
            }
        };
    }

    private static bool WriteValidationReport(string path, float elapsed)
    {
        MissionManager missionManager = Object.FindObjectOfType<MissionManager>();
        SatelliteManager satelliteManager = Object.FindObjectOfType<SatelliteManager>();
        CameraManager cameraManager = Object.FindObjectOfType<CameraManager>();
        ImmediateUIManager immediateUIManager = Object.FindObjectOfType<ImmediateUIManager>();
        PresentationDirector presentationDirector = Object.FindObjectOfType<PresentationDirector>();
        RealSatelliteVisualizer realSatelliteVisualizer = Object.FindObjectOfType<RealSatelliteVisualizer>();
        ObservationTarget[] targets = Object.FindObjectsOfType<ObservationTarget>();
        SatelliteOrbit orbit = Object.FindObjectOfType<SatelliteOrbit>();
        GroundStation groundStation = Object.FindObjectOfType<GroundStation>();
        int completedRuns = SessionState.GetInt(ValidationCompletedRunsKey, 0);

        SatelliteData missionSatellite = satelliteManager != null ? satelliteManager.GetMissionSatellite() : null;
        bool requiredDestinationsPresent = RequiredDestinationsPresent(targets);
        bool requiredDestinationCoreDataValid = RequiredDestinationCoreDataValid(targets);
        bool missionManagerDestinationsRegistered = MissionManagerDestinationsRegistered(missionManager);
        int visibleVirtualSatelliteCount = CountVisibleVirtualSatelliteVisuals();
        int virtualSatelliteAnchorCount = CountVirtualSatelliteAnchors();
        int activeRealMarkerCount = CountActiveRealSatelliteMarkers();
        int realSatelliteDataCount = CountRealSatelliteData(satelliteManager);
        bool selectedSatelliteIsReal = satelliteManager != null && IsRealSatelliteData(satelliteManager.SelectedSatellite);
        bool missionSatelliteIsReal = IsRealSatelliteData(missionSatellite);
        string sourceSelectionWordingEvidence;
        string nonSeoulSenderRouteEvidence;
        string communicationStepEvidence;
        bool sourceSelectionWordingPresent = SourceSelectionWordingPresent(missionManager, out sourceSelectionWordingEvidence);
        bool nonSeoulSenderRoutePresent = TryFindNonSeoulSenderRouteEvidence(missionManager, out nonSeoulSenderRouteEvidence);
        bool communicationStepEvidencePresent = TryBuildCommunicationStepEvidence(missionManager, out communicationStepEvidence);
        bool imeFallbackKoreanValid = LegacyInputFieldImeSupport.ComposeDubeolsikForTest("dkssud") == "안녕"
            && LegacyInputFieldImeSupport.ComposeDubeolsikForTest("gksrmf") == "한글"
            && LegacyInputFieldImeSupport.ComposeDubeolsikForTest("dkssudgktpdy") == "안녕하세요";
        bool imeEnglishModePreservesInput = LegacyInputFieldImeSupport.NormalizePrintableInputForTest("hello OrbitShare", false) == "hello OrbitShare"
            && LegacyInputFieldImeSupport.NormalizePrintableInputForTest("test 123", false) == "test 123";
        bool imeKoreanModeStillConverts = LegacyInputFieldImeSupport.NormalizePrintableInputForTest("dkssud", true) == "안녕";
        bool imeJamoModeComposes = LegacyInputFieldImeSupport.NormalizePrintableInputForTest("ㅌㅔㅅㅡㅌㅡ", true) == "테스트"
            && LegacyInputFieldImeSupport.NormalizePrintableInputForTest("텟ㅡ", true) == "테스"
            && LegacyInputFieldImeSupport.NormalizePrintableInputForTest("테슽ㅡ", true) == "테스트";
        bool imeFieldSetupValid = ValidateLegacyImeFieldSetup();
        bool satelliteWindowTimingResponsive = ValidateSatelliteWindowTimingResponsive();
        bool satelliteFilterCountsStable = ValidateSatelliteFilterCountsStable(satelliteManager);
        string satelliteSortRegressionEvidence;
        bool satelliteSortRegressionValid = ValidateSatelliteSortOrdersRegression(satelliteManager, out satelliteSortRegressionEvidence);
        bool uiRegressionChecksValid = ValidateUiRegressionChecks(missionManager);
        RealSatelliteValidationResult realSatelliteValidation = BuildRealSatelliteValidation(realSatelliteVisualizer);

        bool hasManagers = missionManager != null && satelliteManager != null && cameraManager != null && immediateUIManager != null && presentationDirector != null;
        bool hasSceneObjects = targets.Length >= RequiredDestinationNames.Length
            && requiredDestinationsPresent
            && groundStation != null
            && realSatelliteVisualizer != null;
        bool bootstrapReferencesConnected = missionManager != null
            && missionManager.satelliteManager != null
            && missionManager.simulationManager != null
            && missionManager.earth != null
            && (missionManager.satelliteOrbit != null || realSatelliteVisualizer != null)
            && missionManager.groundStation != null
            && missionManager.connectionLine != null
            && cameraManager != null
            && cameraManager.targetCamera != null
            && cameraManager.earth != null
            && cameraManager.satellite != null
            && cameraManager.missionTarget != null
            && cameraManager.groundStation != null
            && immediateUIManager != null
            && immediateUIManager.simulationManager != null
            && immediateUIManager.satelliteManager != null
            && immediateUIManager.missionManager != null
            && immediateUIManager.cameraManager != null
            && (orbit == null || (orbit.earth != null && orbit.simulationManager != null));
        bool runtimeCanvasReferenceValid = RuntimeCanvasReferenceValid();
        bool fallbackOverlayDisabled = immediateUIManager != null && !immediateUIManager.showFallbackOverlay;
        bool cameraOrbitControlsAvailable = cameraManager != null && cameraManager.OrbitControlsEnabled;
        bool missionComplete = missionManager != null && missionManager.Phase == MissionPhase.Complete;
        bool observationComplete = missionManager != null && missionManager.ObservationProgress >= 0.99f;
        bool transferComplete = missionManager != null && missionManager.TransferProgress >= 0.99f;
        bool resetReplayComplete = completedRuns >= 2 && missionComplete && observationComplete && transferComplete;
        bool presentationCameraCutsValid = presentationDirector != null
            && presentationDirector.HasHandledPhase(MissionPhase.Reserved)
            && presentationDirector.HasHandledPhase(MissionPhase.Running)
            && (presentationDirector.HasHandledPhase(MissionPhase.Observing) || presentationDirector.HasHandledPhase(MissionPhase.ObservationComplete))
            && presentationDirector.HasHandledPhase(MissionPhase.Transmitting)
            && presentationDirector.HasHandledPhase(MissionPhase.Complete)
            && cameraManager != null
            && cameraManager.CurrentMode == "Overview";
        bool hasFleet = realSatelliteDataCount > 0 && activeRealMarkerCount > 0;
        bool noVisibleVirtualConstellation = visibleVirtualSatelliteCount == 0;
        bool realDefaultFleetVisible = realSatelliteValidation.defaultRealFleetVisible && activeRealMarkerCount > 0;
        bool realSatelliteCoreDataValid = missionSatellite != null
            && missionSatelliteIsReal
            && missionSatellite.ownerType == SatelliteOwnerType.Shared
            && missionSatellite.availabilityState == SatelliteAvailabilityState.MissionComplete
            && Mathf.Approximately(missionSatellite.dataStoragePercent, 0f);
        bool realSatelliteSelectedOrMission = selectedSatelliteIsReal || missionSatelliteIsReal;
        bool targetCoreDataValid = requiredDestinationsPresent
            && requiredDestinationCoreDataValid
            && missionManagerDestinationsRegistered
            && missionManager != null
            && missionManager.SelectedTarget != null;
        bool defaultDestinationValid = missionManager != null
            && missionManager.SelectedTarget != null
            && missionManager.SelectedTarget.targetName == OrbitShareDestinations.DefaultTargetName;
        bool sourceGatewayValid = groundStation != null
            && !string.IsNullOrEmpty(groundStation.stationName)
            && groundStation.stationName.Contains("Gateway")
            && missionManager != null
            && missionManager.SelectedOrigin != null
            && missionManager.CompletionSummary.Contains("사용자(" + missionManager.OriginLabel + ") 업링크");
        bool relaySummaryValid = missionManager != null
            && missionManager.CommunicationStep == SatelliteCommunicationStep.Complete
            && missionManager.ResultMessage.Contains("전송 완료")
            && (missionManager.CompletionSummary.Contains("전송 완료") || missionManager.CompletionSummary.Contains("도착 확인"))
            && missionManager.CompletionSummary.Contains("업링크")
            && missionManager.CompletionSummary.Contains("LEO 수신")
            && missionManager.CompletionSummary.Contains("ISL")
            && missionManager.CompletionSummary.Contains("다운링크");
        bool realRelayUsed = missionManager != null
            && (IsRealSatelliteName(missionManager.SelectedSatelliteName)
                || IsRealSatelliteName(missionManager.DestinationRelaySatelliteName)
                || missionSatelliteIsReal);
        bool missionMetricsReady = missionManager != null && missionManager.MissionQualityScore > 0 && missionManager.ServiceReliabilityScore > 0;
        bool scoreRangeValid = missionManager != null
            && IsScoreInRange(missionManager.MissionQualityScore)
            && IsScoreInRange(missionManager.ServiceReliabilityScore);
        bool runtimeLogsClean = validationErrorLogCount == 0;
        string inputModeRegressionEvidence;
        bool inputModeAttachmentRegressionValid = ValidateInputModeAndAttachmentRegression(missionManager, out inputModeRegressionEvidence);
        string queuedContinuationRegressionEvidence;
        bool queuedContinuationRegressionValid = ValidateQueuedContinuationRegression(missionManager, satelliteManager, out queuedContinuationRegressionEvidence);
        bool passed = hasManagers
            && hasSceneObjects
            && bootstrapReferencesConnected
            && runtimeCanvasReferenceValid
            && fallbackOverlayDisabled
            && cameraOrbitControlsAvailable
            && missionComplete
            && observationComplete
            && transferComplete
            && resetReplayComplete
            && presentationCameraCutsValid
            && hasFleet
            && noVisibleVirtualConstellation
            && realDefaultFleetVisible
            && realSatelliteCoreDataValid
            && realSatelliteSelectedOrMission
            && targetCoreDataValid
            && defaultDestinationValid
            && sourceGatewayValid
            && relaySummaryValid
            && realRelayUsed
            && missionMetricsReady
            && scoreRangeValid
            && realSatelliteValidation.visualized
            && realSatelliteValidation.sourceNotFallback
            && realSatelliteValidation.tleUrlsValid
            && realSatelliteValidation.multiDisplayConfigValid
            && realSatelliteValidation.markerAndVelocityValid
            && sourceSelectionWordingPresent
            && nonSeoulSenderRoutePresent
            && imeFallbackKoreanValid
            && imeEnglishModePreservesInput
            && imeKoreanModeStillConverts
            && imeJamoModeComposes
            && imeFieldSetupValid
            && satelliteWindowTimingResponsive
            && satelliteFilterCountsStable
            && satelliteSortRegressionValid
            && uiRegressionChecksValid
            && inputModeAttachmentRegressionValid
            && queuedContinuationRegressionValid
            && runtimeLogsClean;

        StringBuilder report = new StringBuilder();
        report.AppendLine("# OrbitShare Automated Validation");
        report.AppendLine();
        report.AppendLine("Result: " + (passed ? "PASS" : "FAIL"));
        report.AppendLine("Elapsed seconds: " + elapsed.ToString("0.00"));
        report.AppendLine("Screen: " + Screen.width + "x" + Screen.height);
        report.AppendLine();
        report.AppendLine("## Checks");
        report.AppendLine("- Managers present: " + Status(hasManagers));
        report.AppendLine("- Scene objects present: " + Status(hasSceneObjects));
        report.AppendLine("- Bootstrap references connected: " + Status(bootstrapReferencesConnected));
        report.AppendLine("- Runtime Canvas scaler configured: " + Status(runtimeCanvasReferenceValid));
        report.AppendLine("- IMGUI fallback overlay disabled by default: " + Status(fallbackOverlayDisabled));
        report.AppendLine("- Earth orbit camera controls available: " + Status(cameraOrbitControlsAvailable) + " (" + (cameraManager != null ? cameraManager.OrbitControlsSummary : "Missing") + ")");
        report.AppendLine("- Real TLE fleet initialized: " + Status(hasFleet) + " (REAL markers=" + activeRealMarkerCount + ", REAL SatelliteData=" + realSatelliteDataCount + ")");
        report.AppendLine("- Default visible constellation is real CelesTrak TLE: " + Status(realDefaultFleetVisible));
        report.AppendLine("- No visible virtual SAT-* constellation: " + Status(noVisibleVirtualConstellation) + " (visible=" + visibleVirtualSatelliteCount + ", total anchors=" + virtualSatelliteAnchorCount + ")");
        report.AppendLine("- Real satellite core data valid: " + Status(realSatelliteCoreDataValid));
        report.AppendLine("- Selected or mission satellite is REAL: " + Status(realSatelliteSelectedOrMission));
        report.AppendLine("- Required 7 destinations present: " + Status(requiredDestinationsPresent));
        report.AppendLine("- MissionManager has required 7 destinations: " + Status(missionManagerDestinationsRegistered));
        report.AppendLine("- Required destination core data valid: " + Status(targetCoreDataValid));
        report.AppendLine("- Default destination is New York: " + Status(defaultDestinationValid));
        report.AppendLine("- Sender gateway and uplink valid: " + Status(sourceGatewayValid));
        report.AppendLine("- End-to-end satellite flow summary valid: " + Status(relaySummaryValid));
        report.AppendLine("- End-to-end mission uses REAL satellite: " + Status(realRelayUsed));
        report.AppendLine("- Fast demo reaches mission complete: " + Status(missionComplete));
        report.AppendLine("- Uplink reaches 100%: " + Status(observationComplete));
        report.AppendLine("- End-to-end satellite flow reaches 100%: " + Status(transferComplete));
        report.AppendLine("- Reset replay reaches second mission complete: " + Status(resetReplayComplete) + " (" + completedRuns + " runs)");
        report.AppendLine("- PresentationDirector camera cuts valid: " + Status(presentationCameraCutsValid));
        report.AppendLine("- Mission metrics calculated: " + Status(missionMetricsReady));
        report.AppendLine("- Score range 0-100: " + Status(scoreRangeValid));
        report.AppendLine("- Runtime Error/Exception logs: " + Status(runtimeLogsClean) + " (" + validationErrorLogCount + ")");
        report.AppendLine("- Source selection wording visible: " + Status(sourceSelectionWordingPresent));
        report.AppendLine("- Non-Seoul sender communication visible: " + Status(nonSeoulSenderRoutePresent));
        report.AppendLine("- Communication step evidence available: " + SoftStatus(communicationStepEvidencePresent));
        report.AppendLine("- Korean message input fallback valid: " + Status(imeFallbackKoreanValid));
        report.AppendLine("- English input mode preserves Latin text: " + Status(imeEnglishModePreservesInput));
        report.AppendLine("- Korean input mode still converts Dubeolsik keys: " + Status(imeKoreanModeStillConverts));
        report.AppendLine("- Korean input mode composes separated jamo: " + Status(imeJamoModeComposes));
        report.AppendLine("- Legacy InputField IME setup valid: " + Status(imeFieldSetupValid));
        report.AppendLine("- Satellite route timing responds to live geometry: " + Status(satelliteWindowTimingResponsive));
        report.AppendLine("- Satellite recommendation sort counts stable: " + Status(satelliteFilterCountsStable));
        report.AppendLine("- Satellite recommendation sort order regression: " + Status(satelliteSortRegressionValid) + " (" + EvidenceOrPending(satelliteSortRegressionEvidence) + ")");
        report.AppendLine("- Compose/history UI regression checks valid: " + Status(uiRegressionChecksValid));
        report.AppendLine("- Input mode switch + attachment regression valid: " + Status(inputModeAttachmentRegressionValid));
        report.AppendLine("- Queued continuation regression valid: " + Status(queuedContinuationRegressionValid));
        report.AppendLine("- Real satellite TLE visualized: " + Status(realSatelliteValidation.visualized));
        report.AppendLine("- Real satellite source is not built-in fallback: " + Status(realSatelliteValidation.sourceNotFallback));
        report.AppendLine("- Live CelesTrak network fetch this run: " + SoftStatus(realSatelliteValidation.liveFetchThisRun));
        report.AppendLine("- CelesTrak ISS/STATIONS/ACTIVE/STARLINK TLE URLs valid: " + Status(realSatelliteValidation.tleUrlsValid));
        report.AppendLine("- Real satellite STATIONS/ACTIVE/STARLINK multi-display config valid: " + Status(realSatelliteValidation.multiDisplayConfigValid));
        report.AppendLine("- Real satellite marker/velocity evidence valid: " + Status(realSatelliteValidation.markerAndVelocityValid));
        report.AppendLine("- CelesTrak OMM JSON URL support: " + SoftStatus(realSatelliteValidation.ommJsonUrlSupported));
        report.AppendLine();
        report.AppendLine("## Runtime State");
        report.AppendLine("- Mission phase: " + (missionManager != null ? missionManager.Phase.ToString() : "Missing"));
        report.AppendLine("- Sender: " + (missionManager != null ? missionManager.OriginLabel : "Missing"));
        report.AppendLine("- Sender key: " + (missionManager != null ? missionManager.OriginName : "Missing"));
        report.AppendLine("- Destination: " + (missionManager != null && missionManager.SelectedTarget != null ? missionManager.SelectedTarget.targetName : "None"));
        report.AppendLine("- Uplink progress: " + (missionManager != null ? Mathf.RoundToInt(missionManager.ObservationProgress * 100f).ToString() : "0") + "%");
        report.AppendLine("- Satellite flow progress: " + (missionManager != null ? Mathf.RoundToInt(missionManager.TransferProgress * 100f).ToString() : "0") + "%");
        report.AppendLine("- Communication step: " + (missionManager != null ? missionManager.CommunicationStep.ToString() : "Missing"));
        report.AppendLine("- Communication available: " + (missionManager != null && missionManager.CommunicationAvailable));
        report.AppendLine("- Completed fast demo runs: " + completedRuns);
        report.AppendLine("- Camera mode at completion: " + (cameraManager != null ? cameraManager.CurrentMode : "Missing"));
        report.AppendLine("- Mission quality score: " + (missionManager != null ? missionManager.MissionQualityScore.ToString() : "0"));
        report.AppendLine("- Service reliability score: " + (missionManager != null ? missionManager.ServiceReliabilityScore.ToString() : "0"));
        report.AppendLine("- Satellite count: " + (satelliteManager != null ? satelliteManager.Satellites.Count.ToString() : "0"));
        report.AppendLine("- REAL SatelliteData count: " + realSatelliteDataCount);
        report.AppendLine("- Active REAL marker count: " + activeRealMarkerCount);
        report.AppendLine("- Visible virtual SAT-* count: " + visibleVirtualSatelliteCount);
        report.AppendLine("- Mission satellite: " + (missionSatellite != null ? missionSatellite.satelliteName : "Missing"));
        report.AppendLine("- Mission satellite state: " + (missionSatellite != null ? missionSatellite.availabilityState.ToString() : "Missing"));
        report.AppendLine("- Mission satellite storage: " + (missionSatellite != null ? missionSatellite.dataStoragePercent.ToString("0.##") : "Missing"));
        report.AppendLine("- Mission satellite is REAL: " + missionSatelliteIsReal);
        report.AppendLine("- Selected satellite is REAL: " + selectedSatelliteIsReal);
        report.AppendLine("- Required destination count: " + RequiredDestinationNames.Length);
        report.AppendLine("- Scene target count: " + targets.Length);
        report.AppendLine("- MissionManager target count: " + (missionManager != null ? missionManager.Targets.Count.ToString() : "0"));
        report.AppendLine("- Destinations present: " + RequiredDestinationPresenceSummary(targets));
        report.AppendLine();
        report.AppendLine("## Source Selection & Runtime Sender Snapshot");
        report.AppendLine("- Runtime sender: " + RuntimeSenderEvidence(missionManager));
        report.AppendLine("- Source selection evidence: " + EvidenceOrPending(sourceSelectionWordingEvidence));
        report.AppendLine("- Non-Seoul communication evidence: " + EvidenceOrPending(nonSeoulSenderRouteEvidence));
        report.AppendLine();
        report.AppendLine("## Communication Step Evidence");
        report.Append(communicationStepEvidence);
        report.AppendLine();
        report.AppendLine("## Lightweight Regression Evidence");
        report.AppendLine("- Input mode + attachment: " + EvidenceOrPending(inputModeRegressionEvidence));
        report.AppendLine("- Queued continuation: " + EvidenceOrPending(queuedContinuationRegressionEvidence));
        report.AppendLine();
        report.AppendLine("## Real Satellite TLE Evidence");
        report.Append(realSatelliteValidation.evidence);
        report.AppendLine();
        report.AppendLine("## Satellite Recommendation Snapshot");
        AppendRouteRecommendationSnapshot(report, satelliteManager);
        report.AppendLine();
        report.AppendLine("## Environment Note");
        report.AppendLine("- Editor token/REST/curl/update-check warnings are treated as environment noise only when runtime Error/Exception count is 0; Play mode entry/exit is intentional.");

        File.WriteAllText(path, report.ToString());
        return passed;
    }

    private static bool ValidateLegacyImeFieldSetup()
    {
        LegacyInputFieldImeSupport[] supports = Resources.FindObjectsOfTypeAll<LegacyInputFieldImeSupport>();
        bool messageInputFound = false;
        bool nonMessageFallbackOff = true;
        for (int i = 0; i < supports.Length; i++)
        {
            LegacyInputFieldImeSupport support = supports[i];
            if (support == null)
            {
                continue;
            }
            if (support.name == "MessageInput")
            {
                messageInputFound = support.GetComponent<UnityEngine.UI.InputField>() != null;
                continue;
            }
            if ((support.name.Contains("Search") || support.name.Contains("PathInput")) && support.ForceDubeolsikFallback)
            {
                nonMessageFallbackOff = false;
            }
        }
        return messageInputFound && nonMessageFallbackOff;
    }

    private static bool ValidateSatelliteWindowTimingResponsive()
    {
        SatelliteData satellite = new SatelliteData(
            "REAL-TEST-TIMING",
            SatelliteOwnerType.Shared,
            "CelesTrak TLE",
            SatelliteAvailabilityState.Available,
            "실제 TLE 위성",
            420f,
            7.66f,
            100f);
        float contactAngle = SatelliteRouteEstimator.ContactAngleDegrees(satellite);
        float degreesPerSecond = satellite.speedKmPerSec / (6371f + satellite.altitudeKm) * Mathf.Rad2Deg;
        int nearWait = SatelliteRouteEstimator.EstimateWindowSeconds(satellite, contactAngle + 0.5f, true, degreesPerSecond);
        int farWait = SatelliteRouteEstimator.EstimateWindowSeconds(satellite, contactAngle + 1.5f, true, degreesPerSecond);
        int nearMovingAwayWait = SatelliteRouteEstimator.EstimateWindowSeconds(satellite, contactAngle + 0.5f, true, degreesPerSecond, degreesPerSecond);
        int farMovingAwayWait = SatelliteRouteEstimator.EstimateWindowSeconds(satellite, contactAngle + 1.5f, true, degreesPerSecond, degreesPerSecond);
        int nearRemaining = SatelliteRouteEstimator.EstimateAvailableForSeconds(satellite, contactAngle - 0.5f, true, degreesPerSecond);
        int farRemaining = SatelliteRouteEstimator.EstimateAvailableForSeconds(satellite, contactAngle - 1.5f, true, degreesPerSecond);
        return degreesPerSecond > 0.005f
            && degreesPerSecond < 0.2f
            && nearWait > 0
            && farWait > nearWait
            && nearMovingAwayWait > farMovingAwayWait
            && nearRemaining > 0
            && farRemaining > nearRemaining;
    }

    private static bool ValidateSatelliteFilterCountsStable(SatelliteManager satelliteManager)
    {
        if (satelliteManager == null || satelliteManager.Satellites.Count == 0)
        {
            return false;
        }

        int payloadKb = 128;
        int delayMinutes = 0;
        string originName = OrbitShareDestinations.London;
        string targetName = OrbitShareDestinations.NewYork;
        int recommended = satelliteManager.GetRankedRouteEstimates(SatelliteListFilter.All, originName, targetName, delayMinutes, payloadKb).Count;
        int lowCost = satelliteManager.GetRankedRouteEstimates(SatelliteListFilter.LowCost, originName, targetName, delayMinutes, payloadKb).Count;
        int communicationWindow = satelliteManager.GetRankedRouteEstimates(SatelliteListFilter.LowLatency, originName, targetName, delayMinutes, payloadKb).Count;

        SatelliteData labelSatellite = new SatelliteData("LABEL-TEST", SatelliteOwnerType.Shared, "OrbitShare", SatelliteAvailabilityState.Available, "테스트", 420f, 7.6f, 100f);
        SatelliteRouteEstimateRecord remaining = new SatelliteRouteEstimateRecord();
        remaining.canServe = true;
        remaining.communicationWindowState = SatelliteCommunicationWindowState.Remaining;
        remaining.communicationWindowSeconds = 90;
        remaining.estimatedAvailableForSeconds = 90;
        SatelliteRouteEstimateRecord later = new SatelliteRouteEstimateRecord();
        later.canServe = true;
        later.communicationWindowState = SatelliteCommunicationWindowState.AvailableLater;
        later.communicationWindowSeconds = 120;
        later.estimatedWindowSeconds = 120;
        SatelliteRouteEstimateRecord unavailable = new SatelliteRouteEstimateRecord();
        unavailable.canServe = false;
        unavailable.communicationWindowState = SatelliteCommunicationWindowState.Unavailable;

        bool labelsValid = SatelliteRouteEstimator.BuildAvailabilitySummary(labelSatellite, remaining, 0).Contains("남음")
            && SatelliteRouteEstimator.BuildAvailabilitySummary(labelSatellite, later, 0).Contains("후 가능")
            && SatelliteRouteEstimator.BuildAvailabilitySummary(labelSatellite, unavailable, 0) == "통신 불가";

        return recommended > 0
            && recommended == lowCost
            && recommended == communicationWindow
            && labelsValid;
    }

    private static bool ValidateSatelliteSortOrdersRegression(SatelliteManager satelliteManager, out string evidence)
    {
        evidence = string.Empty;
        if (satelliteManager == null || satelliteManager.Satellites.Count == 0)
        {
            evidence = "satelliteManager missing";
            return false;
        }

        string[] origins = { OrbitShareDestinations.Seoul, OrbitShareDestinations.London };
        string[] targets = { OrbitShareDestinations.NewYork, OrbitShareDestinations.Tokyo, OrbitShareDestinations.Sydney };
        int[] delays = { 0, 5, 15, 30 };
        int checkedSets = 0;
        for (int originIndex = 0; originIndex < origins.Length; originIndex++)
        {
            for (int targetIndex = 0; targetIndex < targets.Length; targetIndex++)
            {
                for (int delayIndex = 0; delayIndex < delays.Length; delayIndex++)
                {
                    string origin = origins[originIndex];
                    string target = targets[targetIndex];
                    int delay = delays[delayIndex];
                    List<SatelliteRouteEstimateRecord> all = satelliteManager.GetRankedRouteEstimates(SatelliteListFilter.Recommended, origin, target, delay, 128);
                    List<SatelliteRouteEstimateRecord> lowCost = satelliteManager.GetRankedRouteEstimates(SatelliteListFilter.LowCost, origin, target, delay, 128);
                    List<SatelliteRouteEstimateRecord> latency = satelliteManager.GetRankedRouteEstimates(SatelliteListFilter.LowLatency, origin, target, delay, 128);
                    if (all.Count == 0 || all.Count != lowCost.Count || all.Count != latency.Count)
                    {
                        evidence = "count mismatch " + origin + "->" + target + " +" + delay;
                        return false;
                    }
                    if (!IsUnavailableAtBottom(all) || !IsLowCostSorted(lowCost) || !IsCommunicationWindowSorted(latency))
                    {
                        evidence = "sort mismatch " + origin + "->" + target + " +" + delay;
                        return false;
                    }
                    checkedSets++;
                }
            }
        }

        evidence = "sets=" + checkedSets;
        return checkedSets > 0;
    }

    private static bool IsLowCostSorted(List<SatelliteRouteEstimateRecord> estimates)
    {
        int previousPrice = -1;
        int previousServiceBucket = -1;
        for (int i = 0; i < estimates.Count; i++)
        {
            int serviceBucket = ServiceBucketForValidation(estimates[i]);
            if (previousServiceBucket > serviceBucket)
            {
                return false;
            }
            if (previousServiceBucket != serviceBucket)
            {
                previousPrice = -1;
            }
            int price = Mathf.RoundToInt(Mathf.Max(0f, estimates[i].estimatedPriceUsd) * 100f);
            if (previousPrice > price)
            {
                return false;
            }
            previousServiceBucket = serviceBucket;
            previousPrice = price;
        }
        return true;
    }

    private static bool IsDeliverySorted(List<SatelliteRouteEstimateRecord> estimates)
    {
        int previousSeconds = -1;
        int previousServiceBucket = -1;
        for (int i = 0; i < estimates.Count; i++)
        {
            int serviceBucket = ServiceBucketForValidation(estimates[i]);
            if (previousServiceBucket > serviceBucket)
            {
                return false;
            }
            if (previousServiceBucket != serviceBucket)
            {
                previousSeconds = -1;
            }
            int seconds = Mathf.Max(0, estimates[i].estimatedDeliverySeconds);
            if (previousSeconds > seconds)
            {
                return false;
            }
            previousServiceBucket = serviceBucket;
            previousSeconds = seconds;
        }
        return true;
    }

    private static bool IsCommunicationWindowSorted(List<SatelliteRouteEstimateRecord> estimates)
    {
        int previousBucket = -1;
        int previousSeconds = -1;
        for (int i = 0; i < estimates.Count; i++)
        {
            SatelliteRouteEstimateRecord estimate = estimates[i];
            int bucket = CommunicationBucketForValidation(estimate);
            int seconds = CommunicationSecondsForValidation(estimate);
            if (previousBucket > bucket)
            {
                return false;
            }
            if (previousBucket == bucket)
            {
                if (bucket == 0)
                {
                    if (previousSeconds >= 0 && previousSeconds < seconds)
                    {
                        return false;
                    }
                }
                else if (previousSeconds > seconds)
                {
                    return false;
                }
            }
            previousBucket = bucket;
            previousSeconds = seconds;
        }
        return true;
    }

    private static bool IsUnavailableAtBottom(List<SatelliteRouteEstimateRecord> estimates)
    {
        int previousServiceBucket = -1;
        for (int i = 0; i < estimates.Count; i++)
        {
            int serviceBucket = ServiceBucketForValidation(estimates[i]);
            if (previousServiceBucket > serviceBucket)
            {
                return false;
            }
            previousServiceBucket = serviceBucket;
        }
        return true;
    }

    private static int ServiceBucketForValidation(SatelliteRouteEstimateRecord estimate)
    {
        if (estimate == null || !estimate.canServe || estimate.communicationWindowState == SatelliteCommunicationWindowState.Unavailable)
        {
            return 1;
        }
        return 0;
    }

    private static int CommunicationBucketForValidation(SatelliteRouteEstimateRecord estimate)
    {
        if (estimate == null || !estimate.canServe || estimate.communicationWindowState == SatelliteCommunicationWindowState.Unavailable)
        {
            return 2;
        }
        return estimate.communicationWindowState == SatelliteCommunicationWindowState.Remaining ? 0 : 1;
    }

    private static int CommunicationSecondsForValidation(SatelliteRouteEstimateRecord estimate)
    {
        if (estimate == null)
        {
            return int.MaxValue;
        }
        return Mathf.Max(0, estimate.communicationWindowSeconds > 0 ? estimate.communicationWindowSeconds : estimate.estimatedWindowSeconds);
    }

    private static int CountReachableEstimates(List<SatelliteRouteEstimateRecord> estimates)
    {
        if (estimates == null)
        {
            return 0;
        }

        int count = 0;
        for (int i = 0; i < estimates.Count; i++)
        {
            if (estimates[i] != null && estimates[i].canServe)
            {
                count++;
            }
        }
        return count;
    }

    private static bool ValidateUiRegressionChecks(MissionManager missionManager)
    {
        bool noCheckMarks = true;
        bool bytesUnitVisible = false;
        bool confirmBottomButtonsValid = false;
        bool noPathAttachmentUi = true;
        bool noSelectedSatelliteJumpUi = true;
        bool satelliteSummaryClean = true;
        bool queueLauncherButtonVisible = false;
        bool satelliteOneSecondTransitionValid = ValidateSatelliteOneSecondDisplayTransition();
        bool satelliteListScrollValid = ValidateSatelliteListScrollSetup();
        Text[] texts = Resources.FindObjectsOfTypeAll<Text>();
        for (int i = 0; i < texts.Length; i++)
        {
            Text text = texts[i];
            if (!IsSceneComponent(text))
            {
                continue;
            }

            string value = text.text ?? string.Empty;
            if (value.Contains("✓") || value.Contains("✔"))
            {
                noCheckMarks = false;
            }
            if (value.Contains("Bytes"))
            {
                bytesUnitVisible = true;
            }
            if (value.Contains("경로 첨부") || value.Contains("파일 경로"))
            {
                noPathAttachmentUi = false;
            }
            if (value.Contains("선택 위성으로 이동"))
            {
                noSelectedSatelliteJumpUi = false;
            }
            if (IsUnderSceneObject(text.transform, "SatelliteListPanel")
                && (value.Contains("선택:")
                    || value.Contains("TLE")
                    || value.Contains("정렬 기준")
                    || value.Contains("소요 시간")
                    || value.Contains("필터:")))
            {
                satelliteSummaryClean = false;
            }
            if (IsUnderSceneObject(text.transform, "PanelLauncher")
                && (value == "대기 메시지" || value.StartsWith("대기 ")))
            {
                queueLauncherButtonVisible = true;
            }
        }

        Button[] buttons = Resources.FindObjectsOfTypeAll<Button>();
        for (int i = 0; i < buttons.Length; i++)
        {
            Button button = buttons[i];
            if (!IsSceneComponent(button) || button.transform.parent == null || button.transform.parent.name != "ConfirmStep")
            {
                continue;
            }

            Text label = button.GetComponentInChildren<Text>();
            string value = label != null ? label.text : string.Empty;
            string objectName = button.gameObject.name;
            if (value == "전송 시작"
                || value == "메시지 추가"
                || value == "전송 취소"
                || value == "작성 취소"
                || value == "현재 전송"
                || value == "새 메시지 작성"
                || objectName.Contains("전송 시작")
                || objectName.Contains("전송 취소"))
            {
                RectTransform rect = button.GetComponent<RectTransform>();
                if (rect != null && Mathf.Abs(rect.offsetMin.y + 62f) < 0.5f && Mathf.Abs(rect.offsetMax.y + 22f) < 0.5f)
                {
                    confirmBottomButtonsValid = true;
                }
            }
        }

        bool queueOrderingAvailable = typeof(MissionManager).GetMethod("MoveQueuedMessage") != null;
        bool receiptFieldsAvailable = typeof(SentMessageRecord).GetField("payloadKb") != null
            && typeof(SentMessageRecord).GetField("priceUsd") != null;
        bool priceBreakdownUsesBytes = missionManager == null || missionManager.PriceBreakdownText().Contains("Bytes");
        bytesUnitVisible = bytesUnitVisible || priceBreakdownUsesBytes;
        return noCheckMarks
            && bytesUnitVisible
            && confirmBottomButtonsValid
            && noPathAttachmentUi
            && noSelectedSatelliteJumpUi
            && satelliteSummaryClean
            && queueLauncherButtonVisible
            && satelliteOneSecondTransitionValid
            && satelliteListScrollValid
            && queueOrderingAvailable
            && receiptFieldsAvailable
            && priceBreakdownUsesBytes;
    }

    private static bool ValidateSatelliteListScrollSetup()
    {
        ScrollRect[] scrollRects = Resources.FindObjectsOfTypeAll<ScrollRect>();
        for (int i = 0; i < scrollRects.Length; i++)
        {
            ScrollRect scrollRect = scrollRects[i];
            if (!IsSceneComponent(scrollRect) || scrollRect.gameObject.name != "SatelliteListScrollArea")
            {
                continue;
            }

            bool standardHierarchy = scrollRect.content != null
                && scrollRect.viewport != null
                && scrollRect.content.parent == scrollRect.viewport;
            bool scrollSettings = scrollRect.vertical
                && !scrollRect.horizontal
                && scrollRect.movementType == ScrollRect.MovementType.Clamped
                && scrollRect.inertia
                && scrollRect.scrollSensitivity >= 100f;
            bool rowHeightConfigured = true;
            if (scrollRect.content != null)
            {
                LayoutElement[] rows = scrollRect.content.GetComponentsInChildren<LayoutElement>(true);
                rowHeightConfigured = rows.Length > 0;
                for (int rowIndex = 0; rowIndex < rows.Length; rowIndex++)
                {
                    if (rows[rowIndex] == null || rows[rowIndex].preferredHeight < 120f || rows[rowIndex].flexibleHeight > 0f)
                    {
                        rowHeightConfigured = false;
                        break;
                    }
                }
            }

            return standardHierarchy && scrollSettings && rowHeightConfigured;
        }

        return false;
    }

    private static bool ValidateSatelliteOneSecondDisplayTransition()
    {
        UIManager uiManager = Object.FindObjectOfType<UIManager>();
        if (uiManager == null)
        {
            return false;
        }

        System.Type type = typeof(UIManager);
        System.Reflection.FieldInfo stateField = type.GetField("satelliteDisplayedWindowState", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        System.Reflection.FieldInfo secondsField = type.GetField("satelliteDisplayedWindowSeconds", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        System.Reflection.FieldInfo updatedField = type.GetField("satelliteDisplayedWindowUpdatedAt", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        System.Reflection.FieldInfo routeSecondsField = type.GetField("satelliteDisplayedRouteTimeSeconds", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        System.Reflection.FieldInfo routeUpdatedField = type.GetField("satelliteDisplayedRouteTimeUpdatedAt", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        System.Reflection.MethodInfo method = type.GetMethod("StabilizedCommunicationWindowSeconds", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        System.Reflection.MethodInfo routeMethod = type.GetMethod("StabilizedDisplayedCountdownSeconds", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (stateField == null || secondsField == null || updatedField == null || routeSecondsField == null || routeUpdatedField == null || method == null || routeMethod == null)
        {
            return false;
        }

        Dictionary<string, int> states = stateField.GetValue(uiManager) as Dictionary<string, int>;
        Dictionary<string, int> seconds = secondsField.GetValue(uiManager) as Dictionary<string, int>;
        Dictionary<string, float> updatedAt = updatedField.GetValue(uiManager) as Dictionary<string, float>;
        Dictionary<string, int> routeSeconds = routeSecondsField.GetValue(uiManager) as Dictionary<string, int>;
        Dictionary<string, float> routeUpdatedAt = routeUpdatedField.GetValue(uiManager) as Dictionary<string, float>;
        if (states == null || seconds == null || updatedAt == null || routeSeconds == null || routeUpdatedAt == null)
        {
            return false;
        }

        const string satelliteName = "VALIDATION-1SEC";
        states[satelliteName] = 1;
        seconds[satelliteName] = 1;
        updatedAt[satelliteName] = Time.unscaledTime - 1.2f;
        object result = method.Invoke(uiManager, new object[] { satelliteName, 1, 1 });
        bool reachesZero = result is int && (int)result == 0;

        const string jumpSatelliteName = "VALIDATION-STABLE-JUMP";
        states[jumpSatelliteName] = 1;
        seconds[jumpSatelliteName] = 120;
        updatedAt[jumpSatelliteName] = Time.unscaledTime - 1.2f;
        object jumpResult = method.Invoke(uiManager, new object[] { jumpSatelliteName, 1, 300 });
        bool rawJumpClamped = jumpResult is int && (int)jumpResult <= 120;

        const string dropSatelliteName = "VALIDATION-STABLE-DROP";
        states[dropSatelliteName] = 1;
        seconds[dropSatelliteName] = 120;
        updatedAt[dropSatelliteName] = Time.unscaledTime - 1.2f;
        object dropResult = method.Invoke(uiManager, new object[] { dropSatelliteName, 1, 20 });
        bool rawDropClamped = dropResult is int && (int)dropResult >= 110;

        const string routeSatelliteName = "VALIDATION-ROUTE-TIME";
        string routeKey = routeSatelliteName + "|delivery";
        routeSeconds[routeKey] = 120;
        routeUpdatedAt[routeKey] = Time.unscaledTime - 1.2f;
        object routeJumpResult = routeMethod.Invoke(uiManager, new object[] { routeSatelliteName, "delivery", 420 });
        bool routeJumpClamped = routeJumpResult is int && (int)routeJumpResult <= 124;
        return reachesZero && rawJumpClamped && rawDropClamped && routeJumpClamped;
    }

    private static bool IsUnderSceneObject(Transform transform, string objectName)
    {
        while (transform != null)
        {
            if (transform.name == objectName)
            {
                return true;
            }
            transform = transform.parent;
        }
        return false;
    }

    private static bool ValidateInputModeAndAttachmentRegression(MissionManager missionManager, out string evidence)
    {
        string firstKorean = LegacyInputFieldImeSupport.NormalizePrintableInputForTest("gksrmf", true);
        string english = LegacyInputFieldImeSupport.NormalizePrintableInputForTest("OrbitShare 123", false);
        string secondKorean = LegacyInputFieldImeSupport.NormalizePrintableInputForTest("dkssud", true);
        string jamoKorean = LegacyInputFieldImeSupport.NormalizePrintableInputForTest("ㅌㅔㅅㅡㅌㅡ", true);
        string incrementalKorean = LegacyInputFieldImeSupport.NormalizePrintableInputForTest("테슽ㅡ", true);
        string combined = firstKorean + " " + english + " " + secondKorean + " " + jamoKorean + " " + incrementalKorean;
        List<MessageAttachmentRecord> attachments = new List<MessageAttachmentRecord>
        {
            new MessageAttachmentRecord("regression-note.txt", 1)
        };
        int textBytes = Encoding.UTF8.GetByteCount(combined);
        int payloadKb = missionManager != null ? missionManager.CalculateMessageSizeKb(combined, attachments) : 0;
        bool modeSwitchValid = firstKorean == "한글"
            && english == "OrbitShare 123"
            && secondKorean == "안녕"
            && jamoKorean == "테스트"
            && incrementalKorean == "테스트"
            && combined.Contains("OrbitShare");
        bool bytesReflectKorean = textBytes > combined.Length;
        bool attachmentAccepted = missionManager != null && payloadKb > 0 && payloadKb <= missionManager.MaxPayloadKb;

        evidence = "firstKorean=" + firstKorean
            + ", english=" + english
            + ", secondKorean=" + secondKorean
            + ", jamoKorean=" + jamoKorean
            + ", incrementalKorean=" + incrementalKorean
            + ", textBytes=" + textBytes
            + ", payloadKb=" + payloadKb
            + ", attachment=regression-note.txt/1KB";
        return modeSwitchValid && bytesReflectKorean && attachmentAccepted;
    }

    private static bool ValidateQueuedContinuationRegression(MissionManager missionManager, SatelliteManager satelliteManager, out string evidence)
    {
        evidence = string.Empty;
        if (missionManager == null || satelliteManager == null)
        {
            evidence = "MissionManager or SatelliteManager missing";
            return false;
        }

        bool phaseComplete = missionManager.Phase == MissionPhase.Complete;
        string originName = FirstValidTargetName(missionManager, missionManager.OriginName, string.Empty);
        string targetName = FirstValidTargetName(missionManager, missionManager.DestinationName, originName);
        if (string.IsNullOrEmpty(originName) || string.IsNullOrEmpty(targetName))
        {
            evidence = "origin/target missing (origin=" + originName + ", target=" + targetName + ")";
            return false;
        }

        string message = "회귀 추가 메시지 " + System.DateTime.Now.ToString("HHmmssfff");
        List<MessageAttachmentRecord> attachments = new List<MessageAttachmentRecord>();
        int payloadKb = Mathf.Max(1, missionManager.CalculateMessageSizeKb(message, attachments));
        SatelliteData previewSatellite = satelliteManager.FindBestSatellite(targetName, 0, payloadKb);
        if (previewSatellite != null)
        {
            satelliteManager.SelectSatellite(previewSatellite.satelliteName);
        }

        int beforeQueueCount = missionManager.QueuedMessageCount;
        string errorMessage;
        bool queued = missionManager.QueueMessageFromOrigin(
            message,
            originName,
            targetName,
            0,
            System.DateTime.Now.Hour,
            System.DateTime.Now.Minute - System.DateTime.Now.Minute % 5,
            attachments,
            out errorMessage);
        int queuedIndex = FindQueuedMessageByText(missionManager, message);
        QueuedMessageRouteDetail routeDetail = queuedIndex >= 0 ? missionManager.GetQueuedMessageRouteDetail(queuedIndex) : null;
        SatelliteData recommended = queuedIndex >= 0 ? missionManager.FindRecommendedSatelliteForQueuedMessage(queuedIndex) : null;
        bool satelliteSelectedForQueuedMessage = false;
        if (recommended != null)
        {
            satelliteManager.SelectSatellite(recommended.satelliteName);
            satelliteSelectedForQueuedMessage = satelliteManager.SelectedSatellite != null
                && satelliteManager.SelectedSatellite.satelliteName == recommended.satelliteName;
        }

        bool startApiAvailable = typeof(MissionManager).GetMethod("StartNextQueuedMessage") != null
            && typeof(MissionManager).GetMethod("CanStartNextQueuedMessage") != null;
        bool canStartAfterComplete = missionManager.CanStartNextQueuedMessage();
        string nextActionSummary = missionManager.GetCompletionNextActionSummary();
        bool nextActionValid = nextActionSummary.Contains("다음 메시지") || canStartAfterComplete;
        bool routeAvailable = routeDetail != null && routeDetail.isAvailable && recommended != null;
        bool queueCountIncreased = missionManager.QueuedMessageCount == beforeQueueCount + 1;

        if (queuedIndex >= 0
            && queuedIndex < missionManager.QueuedMessageCount
            && missionManager.QueuedMessages[queuedIndex] != null
            && missionManager.QueuedMessages[queuedIndex].messageText == message)
        {
            missionManager.RemoveQueuedMessage(queuedIndex);
        }

        evidence = "phaseComplete=" + phaseComplete
            + ", queued=" + queued
            + ", queueCountIncreased=" + queueCountIncreased
            + ", origin=" + originName
            + ", target=" + targetName
            + ", selectedSatellite=" + (recommended != null ? recommended.satelliteName : "None")
            + ", satelliteSelected=" + satelliteSelectedForQueuedMessage
            + ", routeAvailable=" + routeAvailable
            + ", canStartAfterComplete=" + canStartAfterComplete
            + ", nextAction=" + nextActionSummary
            + (string.IsNullOrEmpty(errorMessage) ? string.Empty : ", error=" + errorMessage);
        return phaseComplete
            && queued
            && queueCountIncreased
            && queuedIndex >= 0
            && routeAvailable
            && satelliteSelectedForQueuedMessage
            && startApiAvailable
            && canStartAfterComplete
            && nextActionValid;
    }

    private static string FirstValidTargetName(MissionManager missionManager, string preferredName, string excludedName)
    {
        if (missionManager == null)
        {
            return string.Empty;
        }

        if (!string.IsNullOrEmpty(preferredName)
            && preferredName != excludedName
            && missionManager.Targets.Find(target => target != null && target.targetName == preferredName) != null)
        {
            return preferredName;
        }

        for (int i = 0; i < RequiredDestinationNames.Length; i++)
        {
            string candidate = RequiredDestinationNames[i];
            if (candidate == excludedName)
            {
                continue;
            }
            if (missionManager.Targets.Find(target => target != null && target.targetName == candidate) != null)
            {
                return candidate;
            }
        }

        for (int i = 0; i < missionManager.Targets.Count; i++)
        {
            ObservationTarget target = missionManager.Targets[i];
            if (target != null && target.targetName != excludedName)
            {
                return target.targetName;
            }
        }
        return string.Empty;
    }

    private static int FindQueuedMessageByText(MissionManager missionManager, string messageText)
    {
        if (missionManager == null)
        {
            return -1;
        }

        for (int i = 0; i < missionManager.QueuedMessages.Count; i++)
        {
            QueuedMessageRecord record = missionManager.QueuedMessages[i];
            if (record != null && record.messageText == messageText)
            {
                return i;
            }
        }
        return -1;
    }

    private static bool RequiredDestinationsPresent(ObservationTarget[] targets)
    {
        for (int i = 0; i < RequiredDestinationNames.Length; i++)
        {
            if (FindTarget(targets, RequiredDestinationNames[i]) == null)
            {
                return false;
            }
        }
        return true;
    }

    private static bool RequiredDestinationCoreDataValid(ObservationTarget[] targets)
    {
        for (int i = 0; i < RequiredDestinationNames.Length; i++)
        {
            ObservationTarget target = FindTarget(targets, RequiredDestinationNames[i]);
            if (!IsTargetDataPositive(target))
            {
                return false;
            }
        }
        return true;
    }

    private static bool MissionManagerDestinationsRegistered(MissionManager missionManager)
    {
        if (missionManager == null || missionManager.Targets.Count < RequiredDestinationNames.Length)
        {
            return false;
        }
        for (int i = 0; i < RequiredDestinationNames.Length; i++)
        {
            string targetName = RequiredDestinationNames[i];
            if (missionManager.Targets.Find(target => target != null && target.targetName == targetName) == null)
            {
                return false;
            }
        }
        return true;
    }

    private static string RequiredDestinationPresenceSummary(ObservationTarget[] targets)
    {
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < RequiredDestinationNames.Length; i++)
        {
            if (i > 0)
            {
                builder.Append(", ");
            }
            string targetName = RequiredDestinationNames[i];
            builder.Append(targetName).Append("=").Append(FindTarget(targets, targetName) != null);
        }
        return builder.ToString();
    }

    private static void AppendRouteRecommendationSnapshot(StringBuilder report, SatelliteManager satelliteManager)
    {
        if (satelliteManager == null)
        {
            report.AppendLine("- SatelliteManager missing");
            return;
        }

        int[] delays = { 0, 5, 15, 30 };
        for (int i = 0; i < RequiredDestinationNames.Length; i++)
        {
            string targetName = RequiredDestinationNames[i];
            StringBuilder row = new StringBuilder();
            row.Append("- ").Append(targetName).Append(": ");
            for (int j = 0; j < delays.Length; j++)
            {
                int delay = delays[j];
                SatelliteData recommended = satelliteManager.FindBestSatellite(targetName, delay, 128);
                List<SatelliteRouteEstimateRecord> estimates = satelliteManager.GetRankedRouteEstimates(SatelliteListFilter.Recommended, OrbitShareDestinations.Seoul, targetName, delay, 128);
                int count = CountReachableEstimates(estimates);
                if (j > 0)
                {
                    row.Append(" / ");
                }
                row.Append(delay == 0 ? "now" : "+" + delay + "m")
                    .Append(" ")
                    .Append(count)
                    .Append("대")
                    .Append(" rec=")
                    .Append(recommended != null ? recommended.satelliteName : "None");
            }
            report.AppendLine(row.ToString());
        }
    }

    private static bool SourceSelectionWordingPresent(MissionManager missionManager, out string evidence)
    {
        string genericEvidence = string.Empty;
        Text[] texts = Resources.FindObjectsOfTypeAll<Text>();
        for (int i = 0; i < texts.Length; i++)
        {
            if (!IsSceneComponent(texts[i]))
            {
                continue;
            }

            string text = texts[i].text;
            if (ContainsSourceSelectionWording(text))
            {
                string oneLine = OneLineEvidence(text);
                if (ContainsCurrentSenderToken(text, missionManager))
                {
                    evidence = "runtimeSender=" + RuntimeSenderEvidence(missionManager) + "; ui=" + oneLine;
                    return true;
                }

                if (string.IsNullOrEmpty(genericEvidence))
                {
                    genericEvidence = oneLine;
                }
            }
        }

        if (!string.IsNullOrEmpty(genericEvidence))
        {
            evidence = "runtimeSender=" + RuntimeSenderEvidence(missionManager) + "; genericUi=" + genericEvidence;
            return true;
        }

        evidence = string.Empty;
        return false;
    }

    private static bool ContainsSourceSelectionWording(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return false;
        }

        string normalized = value
            .Replace(" ", string.Empty)
            .Replace("\n", string.Empty)
            .Replace("\r", string.Empty)
            .Replace("\t", string.Empty);

        return normalized.Contains("발신지선택")
            || normalized.Contains("발신지:")
            || normalized.Contains("발신지목적지")
            || normalized.Contains("발신지를선택")
            || normalized.Contains("발신위치선택")
            || normalized.Contains("발신지역선택")
            || normalized.Contains("송신지선택")
            || normalized.Contains("송신지:")
            || normalized.Contains("출발지선택")
            || normalized.Contains("보내는곳선택");
    }

    private static string RuntimeSenderEvidence(MissionManager missionManager)
    {
        if (missionManager == null)
        {
            return "Missing";
        }

        return missionManager.OriginLabel + " (" + missionManager.OriginName + ")";
    }

    private static bool ContainsCurrentSenderToken(string value, MissionManager missionManager)
    {
        if (missionManager == null || string.IsNullOrEmpty(value))
        {
            return false;
        }

        return ContainsToken(value, missionManager.OriginLabel)
            || ContainsToken(value, missionManager.OriginName);
    }

    private static bool ContainsToken(string value, string token)
    {
        return !string.IsNullOrEmpty(value)
            && !string.IsNullOrEmpty(token)
            && value.IndexOf(token, System.StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private static bool TryFindNonSeoulSenderRouteEvidence(MissionManager missionManager, out string evidence)
    {
        List<string> candidates = new List<string>();
        if (missionManager != null)
        {
            AddRouteCandidate(candidates, missionManager.ResultMessage);
            AddRouteCandidate(candidates, missionManager.CompletionSummary);
            for (int i = 0; i < missionManager.MissionEventLog.Count; i++)
            {
                AddRouteCandidate(candidates, missionManager.MissionEventLog[i]);
            }
            for (int i = 0; i < missionManager.SentMessages.Count; i++)
            {
                SentMessageRecord record = missionManager.SentMessages[i];
                if (record == null)
                {
                    continue;
                }

                AddRouteCandidate(candidates, record.transmissionLog);
                AddRouteCandidate(candidates, missionManager.GetSentMessageSummary(i));
            }
        }

        Text[] texts = Resources.FindObjectsOfTypeAll<Text>();
        for (int i = 0; i < texts.Length; i++)
        {
            if (IsSceneComponent(texts[i]))
            {
                AddRouteCandidate(candidates, texts[i].text);
            }
        }

        if (missionManager != null)
        {
            for (int i = 0; i < candidates.Count; i++)
            {
                if (TryGetCurrentSenderRouteLine(candidates[i], missionManager, out evidence))
                {
                    return true;
                }
            }
        }

        for (int i = 0; i < candidates.Count; i++)
        {
            if (TryGetNonSeoulSenderRouteLine(candidates[i], out evidence))
            {
                return true;
            }
        }

        evidence = string.Empty;
        return false;
    }

    private static void AddRouteCandidate(List<string> candidates, string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            candidates.Add(value);
        }
    }

    private static bool TryBuildCommunicationStepEvidence(MissionManager missionManager, out string evidence)
    {
        StringBuilder builder = new StringBuilder();
        if (missionManager == null)
        {
            evidence = "- MissionManager: Missing\n";
            return false;
        }

        List<string> candidates = BuildCommunicationEvidenceCandidates(missionManager);
        string uplinkEvidence = FirstEvidenceLineContaining(candidates, "업링크");
        string leoEvidence = FirstEvidenceLineContaining(candidates, "LEO");
        string islEvidence = FirstEvidenceLineContaining(candidates, "ISL");
        string relayEvidence = FirstEvidenceLineContaining(candidates, "목적지 근처 위성");
        string downlinkEvidence = FirstEvidenceLineContaining(candidates, "다운링크");
        string finalReceiveEvidence = FirstEvidenceLineContaining(candidates, "도착 확인");
        if (string.IsNullOrEmpty(finalReceiveEvidence))
        {
            finalReceiveEvidence = FirstEvidenceLineContaining(candidates, "최종 수신");
        }
        bool hasStepEvidence = !string.IsNullOrEmpty(uplinkEvidence)
            && !string.IsNullOrEmpty(leoEvidence)
            && !string.IsNullOrEmpty(islEvidence)
            && !string.IsNullOrEmpty(relayEvidence)
            && !string.IsNullOrEmpty(downlinkEvidence)
            && !string.IsNullOrEmpty(finalReceiveEvidence);

        builder.AppendLine("- Evidence completeness: " + SoftStatus(hasStepEvidence));
        builder.AppendLine("- Route actors: " + missionManager.OriginLabel + " → " + (string.IsNullOrEmpty(missionManager.SelectedSatelliteName) ? "LEO 위성" : missionManager.SelectedSatelliteName) + " → " + (string.IsNullOrEmpty(missionManager.DestinationRelaySatelliteName) ? "목적지 근처 위성" : missionManager.DestinationRelaySatelliteName) + " → " + (missionManager.SelectedTarget != null ? missionManager.SelectedTarget.targetName : "None"));
        builder.AppendLine("- Segment progress: uplink=" + Percent(missionManager.ObservationProgress) + ", ISL=" + Percent(missionManager.InterSatelliteProgress) + ", relay=" + Percent(missionManager.RelaySelectionProgress) + ", downlink=" + Percent(missionManager.DownlinkProgress) + ", final=" + Percent(missionManager.FinalReceiveProgress));
        builder.AppendLine("- Uplink evidence: " + EvidenceOrPending(uplinkEvidence));
        builder.AppendLine("- LEO receive evidence: " + EvidenceOrPending(leoEvidence));
        builder.AppendLine("- ISL evidence: " + EvidenceOrPending(islEvidence));
        builder.AppendLine("- Destination-relay evidence: " + EvidenceOrPending(relayEvidence));
        builder.AppendLine("- Downlink evidence: " + EvidenceOrPending(downlinkEvidence));
        builder.AppendLine("- Arrival confirmation evidence: " + EvidenceOrPending(finalReceiveEvidence));

        evidence = builder.ToString();
        return hasStepEvidence;
    }

    private static RealSatelliteValidationResult BuildRealSatelliteValidation(RealSatelliteVisualizer visualizer)
    {
        RealSatelliteValidationResult result = new RealSatelliteValidationResult();
        StringBuilder builder = new StringBuilder();
        if (visualizer == null)
        {
            result.evidence = "- RealSatelliteVisualizer: Missing\n";
            return result;
        }

        string firstState;
        bool hasFirstState = visualizer.TryGetFirstState(out firstState);
        GeodeticCoordinate geodetic = visualizer.FirstGeodetic;
        bool latLonValid = geodetic.latitudeDegrees >= -90.0
            && geodetic.latitudeDegrees <= 90.0
            && geodetic.longitudeDegrees >= -180.0
            && geodetic.longitudeDegrees <= 180.0;
        bool altitudeValid = geodetic.altitudeKm > -1000.0 && geodetic.altitudeKm < 50000.0;
        bool issAltitudeValid = visualizer.dataSet != RealSatelliteDataSet.Iss
            || (geodetic.altitudeKm > 100.0 && geodetic.altitudeKm < 1000.0);
        bool displayed = visualizer.DisplayedCount > 0;
        bool celesTrakUrlPresent = !string.IsNullOrEmpty(visualizer.LastUrl)
            && visualizer.LastUrl.IndexOf("celestrak.org", System.StringComparison.OrdinalIgnoreCase) >= 0;
        bool markerCountValid;
        int activeMarkerCount;
        int totalMarkerCount;
        string markerVelocityEvidence;
        bool velocityLineValid = TryBuildRealSatelliteMarkerEvidence(visualizer, out markerCountValid, out activeMarkerCount, out totalMarkerCount, out markerVelocityEvidence);
        bool defaultRealFleetVisible = visualizer.dataSet != RealSatelliteDataSet.Iss
            && visualizer.DisplayedCount > 1
            && activeMarkerCount > 1;
        string ommJsonUrl;
        bool ommJsonUrlSupported = TryBuildOmmJsonUrlEvidence(out ommJsonUrl);
        bool issUrlValid = CelesTrakTleClient.BuildIssUrl().Contains("CATNR=25544")
            && CelesTrakTleClient.BuildIssUrl().Contains("FORMAT=TLE");
        bool stationsUrlValid = CelesTrakTleClient.BuildGroupUrl(CelesTrakTleGroup.Stations).Contains("GROUP=STATIONS")
            && CelesTrakTleClient.BuildGroupUrl(CelesTrakTleGroup.Stations).Contains("FORMAT=TLE");
        bool activeUrlValid = CelesTrakTleClient.BuildGroupUrl(CelesTrakTleGroup.Active).Contains("GROUP=ACTIVE")
            && CelesTrakTleClient.BuildGroupUrl(CelesTrakTleGroup.Active).Contains("FORMAT=TLE");
        bool starlinkUrlValid = CelesTrakTleClient.BuildGroupUrl(CelesTrakTleGroup.Starlink).Contains("GROUP=STARLINK")
            && CelesTrakTleClient.BuildGroupUrl(CelesTrakTleGroup.Starlink).Contains("FORMAT=TLE");
        bool hasDatasetSwitchMethods = typeof(RealSatelliteVisualizer).GetMethod("RefreshIss") != null
            && typeof(RealSatelliteVisualizer).GetMethod("RefreshStations") != null
            && typeof(RealSatelliteVisualizer).GetMethod("RefreshActive") != null
            && typeof(RealSatelliteVisualizer).GetMethod("RefreshStarlink") != null;
        bool multiDisplayConfigValid = visualizer.groupDisplayLimit > 1 && hasDatasetSwitchMethods;

        builder.AppendLine("- CelesTrak URL: " + EvidenceOrPending(visualizer.LastUrl));
        builder.AppendLine("- URL source is CelesTrak: " + Status(celesTrakUrlPresent));
        builder.AppendLine("- ISS TLE URL valid: " + Status(issUrlValid) + " (" + CelesTrakTleClient.BuildIssUrl() + ")");
        builder.AppendLine("- STATIONS group TLE URL valid: " + Status(stationsUrlValid) + " (" + CelesTrakTleClient.BuildGroupUrl(CelesTrakTleGroup.Stations) + ")");
        builder.AppendLine("- ACTIVE group TLE URL valid: " + Status(activeUrlValid) + " (" + CelesTrakTleClient.BuildGroupUrl(CelesTrakTleGroup.Active) + ")");
        builder.AppendLine("- STARLINK group TLE URL valid: " + Status(starlinkUrlValid) + " (" + CelesTrakTleClient.BuildGroupUrl(CelesTrakTleGroup.Starlink) + ")");
        builder.AppendLine("- OMM JSON URL support: " + SoftStatus(ommJsonUrlSupported) + " (" + EvidenceOrPending(ommJsonUrl) + ")");
        builder.AppendLine("- Status: " + EvidenceOrPending(visualizer.LastStatus));
        builder.AppendLine("- Loaded/displayed: " + visualizer.LoadedCount + "/" + visualizer.DisplayedCount);
        builder.AppendLine("- Dataset/display cap: " + visualizer.dataSet + ", maxDisplayed=" + visualizer.maxDisplayed + ", groupDisplayLimit=" + visualizer.groupDisplayLimit);
        builder.AppendLine("- Default real fleet mode: " + Status(defaultRealFleetVisible) + " (requires STATIONS/ACTIVE/STARLINK and more than 1 visible real marker)");
        builder.AppendLine("- STATIONS/ACTIVE/STARLINK switch methods: " + Status(hasDatasetSwitchMethods));
        builder.AppendLine("- Multi-display config: " + Status(multiDisplayConfigValid));
        builder.AppendLine("- First satellite: " + EvidenceOrPending(visualizer.FirstSatelliteName));
        builder.AppendLine("- First state: " + EvidenceOrPending(firstState));
        builder.AppendLine("- Lat/lon valid: " + Status(latLonValid));
        builder.AppendLine("- Altitude sanity: " + Status(altitudeValid));
        builder.AppendLine("- ISS LEO altitude sanity: " + Status(issAltitudeValid));
        builder.AppendLine("- Marker count: " + Status(markerCountValid) + " (active=" + activeMarkerCount + ", total=" + totalMarkerCount + ", displayed=" + visualizer.DisplayedCount + ")");
        builder.AppendLine("- Velocity direction line: " + Status(velocityLineValid) + " (" + EvidenceOrPending(markerVelocityEvidence) + ")");
        builder.AppendLine("- Live CelesTrak network fetch this run: " + SoftStatus(!visualizer.LastFetchFromCache && !visualizer.LastFetchFromFallback && celesTrakUrlPresent));
        builder.AppendLine("- Cache used for display: " + visualizer.LastFetchFromCache);
        builder.AppendLine("- Built-in fallback used: " + visualizer.LastFetchFromFallback);
        if (!string.IsNullOrEmpty(visualizer.LastError))
        {
            builder.AppendLine("- Last fetch note: " + visualizer.LastError);
        }

        result.evidence = builder.ToString();
        result.visualized = displayed && hasFirstState && latLonValid && altitudeValid && issAltitudeValid;
        result.defaultRealFleetVisible = defaultRealFleetVisible;
        result.sourceNotFallback = celesTrakUrlPresent && !visualizer.LastFetchFromFallback;
        result.liveFetchThisRun = !visualizer.LastFetchFromCache && !visualizer.LastFetchFromFallback && celesTrakUrlPresent;
        result.tleUrlsValid = issUrlValid && stationsUrlValid && activeUrlValid && starlinkUrlValid;
        result.multiDisplayConfigValid = multiDisplayConfigValid;
        result.markerAndVelocityValid = markerCountValid && velocityLineValid;
        result.ommJsonUrlSupported = ommJsonUrlSupported;
        return result;
    }

    private static bool TryBuildRealSatelliteMarkerEvidence(RealSatelliteVisualizer visualizer, out bool markerCountValid, out int activeMarkerCount, out int totalMarkerCount, out string evidence)
    {
        markerCountValid = false;
        activeMarkerCount = 0;
        totalMarkerCount = 0;
        evidence = string.Empty;

        if (visualizer == null)
        {
            evidence = "visualizer missing";
            return false;
        }

        RealSatelliteMarker[] markers = Object.FindObjectsOfType<RealSatelliteMarker>(true);
        totalMarkerCount = markers.Length;
        LineRenderer firstVelocityLine = null;
        for (int i = 0; i < markers.Length; i++)
        {
            if (markers[i] == null || !markers[i].gameObject.activeInHierarchy)
            {
                continue;
            }

            activeMarkerCount++;
            if (firstVelocityLine == null)
            {
                firstVelocityLine = markers[i].GetComponentInChildren<LineRenderer>(true);
            }
        }

        markerCountValid = visualizer.DisplayedCount > 0 && activeMarkerCount == visualizer.DisplayedCount;
        if (firstVelocityLine == null)
        {
            evidence = "velocity line missing";
            return false;
        }

        bool lineEnabled = firstVelocityLine.enabled;
        bool hasTwoPoints = firstVelocityLine.positionCount >= 2;
        float distance = 0f;
        if (hasTwoPoints)
        {
            distance = Vector3.Distance(firstVelocityLine.GetPosition(0), firstVelocityLine.GetPosition(1));
        }

        evidence = "enabled=" + lineEnabled + ", points=" + firstVelocityLine.positionCount + ", length=" + distance.ToString("0.000");
        return lineEnabled && hasTwoPoints && distance > 0.001f && distance < 10f;
    }

    private static bool TryBuildOmmJsonUrlEvidence(out string evidence)
    {
        evidence = string.Empty;
        string reflectedUrl = string.Empty;
        string[] methodNames =
        {
            "BuildIssOmmJsonUrl",
            "BuildIssJsonUrl",
            "BuildCatalogNumberOmmJsonUrl",
            "BuildCatalogNumberJsonUrl",
            "BuildOmmJsonUrl"
        };

        for (int i = 0; i < methodNames.Length; i++)
        {
            System.Reflection.MethodInfo method = typeof(CelesTrakTleClient).GetMethod(methodNames[i], System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            if (method == null)
            {
                continue;
            }

            System.Reflection.ParameterInfo[] parameterInfos = method.GetParameters();
            object[] parameters = null;
            if (parameterInfos.Length == 1)
            {
                if (parameterInfos[0].ParameterType == typeof(int))
                {
                    parameters = new object[] { CelesTrakTleClient.IssNoradId };
                }
                else if (parameterInfos[0].ParameterType == typeof(string))
                {
                    parameters = new object[] { CelesTrakTleClient.IssNoradId.ToString() };
                }
                else
                {
                    continue;
                }
            }
            else if (parameterInfos.Length > 1)
            {
                continue;
            }

            string url;
            try
            {
                url = method.Invoke(null, parameters) as string;
            }
            catch (System.Exception)
            {
                continue;
            }
            if (string.IsNullOrEmpty(url))
            {
                continue;
            }

            reflectedUrl = url;
            break;
        }

        string issUrl = BuildCelesTrakOmmJsonCatalogUrl(CelesTrakTleClient.IssNoradId);
        string stationsUrl = BuildCelesTrakOmmJsonGroupUrl(CelesTrakTleClient.StationsGroupName);
        string activeUrl = BuildCelesTrakOmmJsonGroupUrl(CelesTrakTleClient.ActiveGroupName);
        string starlinkUrl = BuildCelesTrakOmmJsonGroupUrl(CelesTrakTleClient.StarlinkGroupName);
        evidence = "ISS=" + issUrl + "; STATIONS=" + stationsUrl + "; ACTIVE=" + activeUrl + "; STARLINK=" + starlinkUrl;
        if (!string.IsNullOrEmpty(reflectedUrl))
        {
            evidence += "; reflectedApi=" + reflectedUrl;
        }
        return IsCelesTrakJsonUrl(issUrl)
            && IsCelesTrakJsonUrl(stationsUrl)
            && IsCelesTrakJsonUrl(activeUrl)
            && IsCelesTrakJsonUrl(starlinkUrl)
            && stationsUrl.Contains("GROUP=STATIONS")
            && activeUrl.Contains("GROUP=ACTIVE")
            && starlinkUrl.Contains("GROUP=STARLINK");
    }

    private static string BuildCelesTrakOmmJsonCatalogUrl(int catalogNumber)
    {
        return "https://celestrak.org/NORAD/elements/gp.php?CATNR=" + catalogNumber + "&FORMAT=JSON";
    }

    private static string BuildCelesTrakOmmJsonGroupUrl(string groupName)
    {
        return "https://celestrak.org/NORAD/elements/gp.php?GROUP=" + groupName.Trim().ToUpperInvariant() + "&FORMAT=JSON";
    }

    private static bool IsCelesTrakJsonUrl(string url)
    {
        return !string.IsNullOrEmpty(url)
            && url.IndexOf("celestrak.org", System.StringComparison.OrdinalIgnoreCase) >= 0
            && url.IndexOf("FORMAT=JSON", System.StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private class RealSatelliteValidationResult
    {
        public string evidence;
        public bool visualized;
        public bool defaultRealFleetVisible;
        public bool sourceNotFallback;
        public bool liveFetchThisRun;
        public bool tleUrlsValid;
        public bool multiDisplayConfigValid;
        public bool markerAndVelocityValid;
        public bool ommJsonUrlSupported;
    }

    private static List<string> BuildCommunicationEvidenceCandidates(MissionManager missionManager)
    {
        List<string> candidates = new List<string>();
        if (missionManager == null)
        {
            return candidates;
        }

        AddRouteCandidate(candidates, missionManager.ResultMessage);
        AddRouteCandidate(candidates, missionManager.CompletionSummary);
        for (int i = 0; i < missionManager.MissionEventLog.Count; i++)
        {
            AddRouteCandidate(candidates, missionManager.MissionEventLog[i]);
        }
        for (int i = 0; i < missionManager.SentMessages.Count; i++)
        {
            SentMessageRecord record = missionManager.SentMessages[i];
            if (record == null)
            {
                continue;
            }

            AddRouteCandidate(candidates, record.transmissionLog);
            AddRouteCandidate(candidates, missionManager.GetSentMessageSummary(i));
        }

        return candidates;
    }

    private static string FirstEvidenceLineContaining(List<string> candidates, params string[] tokens)
    {
        for (int i = 0; i < candidates.Count; i++)
        {
            if (string.IsNullOrEmpty(candidates[i]))
            {
                continue;
            }

            string[] lines = candidates[i].Replace("\r", "\n").Split('\n');
            for (int j = 0; j < lines.Length; j++)
            {
                if (ContainsAllTokens(lines[j], tokens))
                {
                    return OneLineEvidence(lines[j]);
                }
            }
        }

        return string.Empty;
    }

    private static bool ContainsAllTokens(string value, string[] tokens)
    {
        if (string.IsNullOrEmpty(value))
        {
            return false;
        }

        for (int i = 0; i < tokens.Length; i++)
        {
            if (!ContainsToken(value, tokens[i]))
            {
                return false;
            }
        }

        return true;
    }

    private static string Percent(float value)
    {
        return Mathf.RoundToInt(Mathf.Clamp01(value) * 100f) + "%";
    }

    private static bool TryGetNonSeoulSenderRouteLine(string value, out string evidence)
    {
        evidence = string.Empty;
        if (string.IsNullOrEmpty(value))
        {
            return false;
        }

        string[] lines = value.Replace("\r", "\n").Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            if (LooksLikeNonSeoulSenderRouteLine(lines[i]))
            {
                evidence = OneLineEvidence(lines[i]);
                return true;
            }
        }

        return false;
    }

    private static bool TryGetCurrentSenderRouteLine(string value, MissionManager missionManager, out string evidence)
    {
        evidence = string.Empty;
        if (missionManager == null || string.IsNullOrEmpty(value))
        {
            return false;
        }

        string[] lines = value.Replace("\r", "\n").Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            if (LooksLikeNonSeoulSenderRouteLine(lines[i]) && ContainsCurrentSenderToken(lines[i], missionManager))
            {
                evidence = OneLineEvidence(lines[i]);
                return true;
            }
        }

        return false;
    }

    private static bool LooksLikeNonSeoulSenderRouteLine(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return false;
        }

        string normalized = value
            .Replace(" ", string.Empty)
            .Replace("\t", string.Empty);
        int arrowIndex = normalized.IndexOf("→");
        if (arrowIndex < 0)
        {
            arrowIndex = normalized.IndexOf("->");
        }
        if (arrowIndex <= 0)
        {
            return false;
        }

        bool mentionsSatellite = normalized.Contains("위성")
            || normalized.Contains("SAT-")
            || normalized.IndexOf("satellite", System.StringComparison.OrdinalIgnoreCase) >= 0;
        if (!mentionsSatellite)
        {
            return false;
        }

        string sender = StripRouteSourcePrefix(normalized.Substring(0, arrowIndex));
        return sender.Length > 0
            && !ContainsSeoulToken(sender)
            && !sender.Contains("위성")
            && !sender.Contains("SAT-")
            && sender.IndexOf("satellite", System.StringComparison.OrdinalIgnoreCase) < 0;
    }

    private static string StripRouteSourcePrefix(string value)
    {
        string[] prefixes = { "통신", "송수신", "발신:", "발신", "송신:", "송신", "Source:", "Sender:", "From:" };
        for (int i = 0; i < prefixes.Length; i++)
        {
            int index = value.LastIndexOf(prefixes[i]);
            if (index >= 0)
            {
                value = value.Substring(index + prefixes[i].Length);
            }
        }

        return value.Trim(':', '-', '·', ' ');
    }

    private static bool ContainsSeoulToken(string value)
    {
        return !string.IsNullOrEmpty(value)
            && (value.Contains("서울") || value.IndexOf("Seoul", System.StringComparison.OrdinalIgnoreCase) >= 0);
    }

    private static bool IsSceneComponent(Component component)
    {
        return component != null
            && component.gameObject != null
            && component.gameObject.scene.IsValid();
    }

    private static string SoftStatus(bool value)
    {
        return value ? "PASS" : "PENDING";
    }

    private static string EvidenceOrPending(string evidence)
    {
        return string.IsNullOrEmpty(evidence) ? "PENDING" : evidence;
    }

    private static string OneLineEvidence(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        string evidence = value
            .Replace("\r", " ")
            .Replace("\n", " ")
            .Replace("\t", " ")
            .Trim();
        while (evidence.Contains("  "))
        {
            evidence = evidence.Replace("  ", " ");
        }
        if (evidence.Length > 120)
        {
            evidence = evidence.Substring(0, 117) + "...";
        }
        return evidence;
    }

    private static string Status(bool value)
    {
        return value ? "PASS" : "FAIL";
    }

    private static SatelliteData FindSatellite(SatelliteManager satelliteManager, string satelliteName)
    {
        if (satelliteManager == null)
        {
            return null;
        }

        return satelliteManager.Satellites.Find(satellite => satellite.satelliteName == satelliteName);
    }

    private static ObservationTarget FindTarget(ObservationTarget[] targets, string targetName)
    {
        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i] != null && targets[i].targetName == targetName)
            {
                return targets[i];
            }
        }

        return null;
    }

    private static bool IsTargetDataPositive(ObservationTarget target)
    {
        return target != null
            && target.estimatedDataMb > 0f
            && target.resolutionMeters > 0f
            && target.missionCostCredits > 0;
    }

    private static bool IsScoreInRange(int score)
    {
        return score >= 0 && score <= 100;
    }

    private static bool RuntimeCanvasReferenceValid()
    {
        CanvasScaler scaler = Object.FindObjectOfType<CanvasScaler>();
        return scaler != null
            && scaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize
            && scaler.referenceResolution.x >= 1350f
            && scaler.referenceResolution.x <= 1900f
            && scaler.referenceResolution.y >= 760f
            && scaler.referenceResolution.y <= 1060f;
    }

    private static bool RealSatelliteFleetReadyForValidation(RealSatelliteVisualizer visualizer, SatelliteManager satelliteManager)
    {
        return visualizer != null
            && visualizer.dataSet != RealSatelliteDataSet.Iss
            && visualizer.DisplayedCount > 1
            && CountActiveRealSatelliteMarkers() > 1
            && CountRealSatelliteData(satelliteManager) > 1;
    }

    private static int CountRealSatelliteData(SatelliteManager satelliteManager)
    {
        if (satelliteManager == null)
        {
            return 0;
        }

        int count = 0;
        for (int i = 0; i < satelliteManager.Satellites.Count; i++)
        {
            if (IsRealSatelliteData(satelliteManager.Satellites[i]))
            {
                count++;
            }
        }

        return count;
    }

    private static bool IsRealSatelliteData(SatelliteData satellite)
    {
        return satellite != null
            && (IsRealSatelliteName(satellite.satelliteName)
                || ContainsToken(satellite.ownerName, "CelesTrak")
                || ContainsToken(satellite.usageType, "실제 TLE"));
    }

    private static bool IsRealSatelliteName(string satelliteName)
    {
        return !string.IsNullOrEmpty(satelliteName)
            && satelliteName.StartsWith("REAL-", System.StringComparison.OrdinalIgnoreCase);
    }

    private static int CountActiveRealSatelliteMarkers()
    {
        RealSatelliteMarker[] markers = Object.FindObjectsOfType<RealSatelliteMarker>(true);
        int count = 0;
        for (int i = 0; i < markers.Length; i++)
        {
            if (markers[i] != null && markers[i].gameObject.activeInHierarchy)
            {
                count++;
            }
        }

        return count;
    }

    private static int CountVisibleVirtualSatelliteVisuals()
    {
        GameObject satellitesRoot = GameObject.Find("Satellites");
        if (satellitesRoot == null)
        {
            return 0;
        }

        int count = 0;
        Transform[] children = satellitesRoot.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i] == null || !children[i].name.StartsWith("SAT-"))
            {
                continue;
            }

            if (VirtualSatelliteIsVisiblyRendered(children[i].gameObject))
            {
                count++;
            }
        }

        return count;
    }

    private static int CountVirtualSatelliteAnchors()
    {
        GameObject satellitesRoot = GameObject.Find("Satellites");
        if (satellitesRoot == null)
        {
            return 0;
        }

        int count = 0;
        Transform[] children = satellitesRoot.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i] != null && children[i].name.StartsWith("SAT-"))
            {
                count++;
            }
        }

        return count;
    }

    private static bool VirtualSatelliteIsVisiblyRendered(GameObject satelliteObject)
    {
        if (satelliteObject == null || !satelliteObject.activeInHierarchy)
        {
            return false;
        }

        Renderer[] renderers = satelliteObject.GetComponentsInChildren<Renderer>(false);
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null && renderers[i].enabled)
            {
                return true;
            }
        }

        return false;
    }

    private static bool RelayNodeNamesPresent()
    {
        if (GameObject.Find("SAT-P01") == null || GameObject.Find("SAT-C01") == null || GameObject.Find("SAT-S01") == null)
        {
            return false;
        }

        for (int i = 4; i <= 24; i++)
        {
            if (GameObject.Find("SAT-R" + i.ToString("00")) == null)
            {
                return false;
            }
        }

        return true;
    }
}

public class OrbitShareFrameCaptureAgent : MonoBehaviour
{
    private string directory;

    public void Initialize(string targetDirectory)
    {
        directory = targetDirectory;
    }

    private IEnumerator Start()
    {
        if (string.IsNullOrEmpty(directory))
        {
            directory = Path.GetFullPath("DemoFrames");
        }

        yield return new WaitForSecondsRealtime(1f);

        MissionManager missionManager = Object.FindObjectOfType<MissionManager>();
        if (missionManager != null)
        {
            missionManager.RunFastDemo();
        }

        for (int i = 0; i < 13; i++)
        {
            missionManager = Object.FindObjectOfType<MissionManager>();
            if (missionManager != null && missionManager.Phase != MissionPhase.Complete && i > 4)
            {
                missionManager.RunFastDemo();
            }

            yield return new WaitForEndOfFrame();

            Texture2D texture = ScreenCapture.CaptureScreenshotAsTexture();
            if (texture != null)
            {
                string framePath = Path.Combine(directory, "frame_" + i.ToString("000") + ".png");
                File.WriteAllBytes(framePath, texture.EncodeToPNG());
                Object.Destroy(texture);
            }

            yield return new WaitForSecondsRealtime(0.7f);
        }

        SessionState.SetBool("OrbitShare.CaptureDemoFrames", false);
        EditorApplication.isPlaying = false;
        EditorApplication.delayCall += delegate
        {
            Debug.Log("OrbitShare demo frames saved: " + directory);
            EditorApplication.Exit(0);
        };
    }
}
