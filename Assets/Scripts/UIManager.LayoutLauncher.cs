using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// 런처/레이아웃 UI: 상단 버튼, 하단 런처, 패널 열기/닫기, 드래그 위치 저장을 관리합니다.
public partial class UIManager
{
    private void CreateTopPanel(Transform parent)
    {
        titleText = null;
        statusStopButton = null;
        nextQueuedButton = null;
        historyToggleButton = null;
        resultToggleButton = null;
        newMessageButton = null;
    }

    private void CreateTimeControlPanel(Transform parent)
    {
        RectTransform panel = CreatePanel(parent, "TimeControlPanel", new Vector2(0.69f, 0.42f), new Vector2(0.835f, 0.55f), new Color(0.025f, 0.045f, 0.075f, 0.94f));
        timeControlPanel = panel;
        Text heading = CreateText(panel, "보낼 시간", 16, TextAnchor.UpperCenter);
        heading.rectTransform.anchorMin = new Vector2(0f, 1f);
        heading.rectTransform.anchorMax = new Vector2(1f, 1f);
        heading.rectTransform.pivot = new Vector2(0.5f, 1f);
        heading.rectTransform.anchoredPosition = new Vector2(0f, -8f);
        heading.rectTransform.sizeDelta = new Vector2(-18f, 28f);

        sendDelayButtons[0] = CreateButton(panel, "지금", new Vector2(0f, 0f), new Vector2(0.25f, 0f), new Vector2(8f, 10f), new Vector2(-3f, 42f), delegate { missionManager.SetSendDelayMinutes(0); });
        sendDelayButtons[5] = CreateButton(panel, "5분", new Vector2(0.25f, 0f), new Vector2(0.5f, 0f), new Vector2(3f, 10f), new Vector2(-3f, 42f), delegate { missionManager.SetSendDelayMinutes(5); });
        sendDelayButtons[15] = CreateButton(panel, "15분", new Vector2(0.5f, 0f), new Vector2(0.75f, 0f), new Vector2(3f, 10f), new Vector2(-3f, 42f), delegate { missionManager.SetSendDelayMinutes(15); });
        sendDelayButtons[30] = CreateButton(panel, "30분", new Vector2(0.75f, 0f), new Vector2(1f, 0f), new Vector2(3f, 10f), new Vector2(-8f, 42f), delegate { missionManager.SetSendDelayMinutes(30); });
    }

    private void CreateCameraControlPanel(Transform parent)
    {
        RectTransform panel = CreatePanel(parent, "CameraControlPanel", new Vector2(0.84f, 0.42f), new Vector2(0.985f, 0.55f), new Color(0.025f, 0.045f, 0.075f, 0.94f));
        messageSizePanel = panel;
        Text heading = CreateText(panel, "1-B. 메시지 크기", 16, TextAnchor.UpperCenter);
        heading.rectTransform.anchorMin = new Vector2(0f, 1f);
        heading.rectTransform.anchorMax = new Vector2(1f, 1f);
        heading.rectTransform.pivot = new Vector2(0.5f, 1f);
        heading.rectTransform.anchoredPosition = new Vector2(0f, -8f);
        heading.rectTransform.sizeDelta = new Vector2(-18f, 28f);

        messageSizeButtons[32] = CreateButton(panel, FormatKilobytesAsBytes(32), new Vector2(0f, 0f), new Vector2(0.3f, 0f), new Vector2(8f, 10f), new Vector2(-3f, 42f), delegate { missionManager.SetMessageSizeKb(32); });
        messageSizeButtons[128] = CreateButton(panel, FormatKilobytesAsBytes(128), new Vector2(0.3f, 0f), new Vector2(0.62f, 0f), new Vector2(3f, 10f), new Vector2(-3f, 42f), delegate { missionManager.SetMessageSizeKb(128); });
        messageSizeButtons[512] = CreateButton(panel, FormatKilobytesAsBytes(512), new Vector2(0.62f, 0f), new Vector2(1f, 0f), new Vector2(3f, 10f), new Vector2(-8f, 42f), delegate { missionManager.SetMessageSizeKb(512); });

        RectTransform statusPanel = CreatePanel(parent, "CommunicationStatusPanel", new Vector2(0.33f, 0.11f), new Vector2(0.67f, 0.27f), new Color(0.025f, 0.045f, 0.075f, 0.92f));
        missionHeadlineText = CreateText(statusPanel, string.Empty, 23, TextAnchor.UpperLeft);
        missionHeadlineText.fontStyle = FontStyle.Bold;
        missionHeadlineText.rectTransform.offsetMin = new Vector2(18f, 108f);
        missionHeadlineText.rectTransform.offsetMax = new Vector2(-18f, -10f);

        missionStatusText = CreateText(statusPanel, string.Empty, 14, TextAnchor.UpperLeft);
        missionStatusText.rectTransform.offsetMin = new Vector2(18f, 52f);
        missionStatusText.rectTransform.offsetMax = new Vector2(-18f, -44f);

        CreateProgressLabel(statusPanel, "업링크", new Vector2(18f, 36f));
        CreateProgressLabel(statusPanel, "위성망", new Vector2(18f, 14f));
        observationFill = CreateProgressBar(statusPanel, "RelayLockProgress", new Vector2(74f, 36f), new Color(1f, 0.68f, 0.12f, 0.95f));
        transferFill = CreateProgressBar(statusPanel, "DeliveryProgress", new Vector2(74f, 14f), new Color(0.24f, 1f, 0.84f, 0.95f));
    }

    private void CreatePanelLauncher(Transform parent)
    {
        panelLauncher = CreatePanel(parent, "PanelLauncher", new Vector2(0.10f, 0.012f), new Vector2(0.90f, 0.055f), new Color(0.02f, 0.05f, 0.09f, 0.88f));
        string[] labels = { "메시지 작성", "위성망 추천", "전송 상태", "전송 기록", "대기 메시지", "설정", "시연" };
        panelLauncherButtons.Clear();
        UnityAction[] actions = {
            delegate { OpenMessageComposerFromLauncher(); },
            delegate { ToggleSatellitePanel(); },
            delegate { ToggleMissionStatusPanel(); },
            delegate { ToggleHistoryPanel(); },
            delegate { ToggleQueuePanel(); },
            delegate { ToggleSettingsPanel(); },
            delegate { StartDemoMode(); }
        };

        for (int i = 0; i < labels.Length; i++)
        {
            float left = i / (float)labels.Length;
            float right = (i + 1f) / labels.Length;
            Button button = CreateButton(panelLauncher, labels[i], new Vector2(left, 0f), new Vector2(right, 1f), new Vector2(i == 0 ? 8f : 3f, 6f), new Vector2(i == labels.Length - 1 ? -8f : -3f, -6f), actions[i]);
            panelLauncherButtons.Add(button);
            Text text = button.GetComponentInChildren<Text>();
            if (text != null)
            {
                text.fontSize = 11;
            }
        }
    }

    private void OpenMessageComposerFromLauncher()
    {
        if (missionManager != null && IsActiveTransferPhase(missionManager.Phase))
        {
            if (composingQueuedMessageInFlow)
            {
                ToggleRequestFlowPanel();
                return;
            }
            BeginQueuedMessageFlow();
            return;
        }
        ToggleRequestFlowPanel();
    }

    private void RefreshPanelLauncherButtons()
    {
        if (panelLauncherButtons.Count < 7)
        {
            return;
        }

        bool activeTransfer = missionManager != null && IsActiveTransferPhase(missionManager.Phase);
        int queuedCount = missionManager != null ? missionManager.QueuedMessageCount : 0;
        SetButtonLabel(panelLauncherButtons[0], composingQueuedMessageInFlow ? "추가 작성 중" : activeTransfer ? "추가 작성" : "메시지 작성");
        SetButtonLabel(panelLauncherButtons[1], "위성망 추천");
        SetButtonLabel(panelLauncherButtons[2], activeTransfer ? "전송 중" : "전송 상태");
        SetButtonLabel(panelLauncherButtons[3], "전송 기록");
        SetButtonLabel(panelLauncherButtons[4], "대기 메시지");
        panelLauncherButtons[0].interactable = true;
        SetButtonVisual(panelLauncherButtons[0], composingQueuedMessageInFlow || IsPanelOpen(requestFlowPanel) ? ButtonVisual.Selected : activeTransfer ? ButtonVisual.Primary : requestFlowDismissed ? ButtonVisual.Cta : ButtonVisual.Primary);
        SetButtonVisual(panelLauncherButtons[1], satelliteListDismissed ? ButtonVisual.Cta : IsPanelOpen(satelliteListPanel) ? ButtonVisual.Selected : ButtonVisual.Secondary);
        SetButtonVisual(panelLauncherButtons[2], missionStatusDismissed ? ButtonVisual.Cta : IsPanelOpen(missionStatusPanel) ? ButtonVisual.Selected : ButtonVisual.Secondary);
        SetButtonVisual(panelLauncherButtons[3], IsPanelOpen(historyPanel) ? ButtonVisual.Selected : ButtonVisual.Secondary);
        SetButtonVisual(panelLauncherButtons[4], IsPanelOpen(queueMessagePanel) ? ButtonVisual.Selected : queuedCount > 0 ? ButtonVisual.Cta : ButtonVisual.Secondary);
        SetButtonVisual(panelLauncherButtons[5], IsPanelOpen(settingsPanel) ? ButtonVisual.Selected : ButtonVisual.Secondary);
        bool activeDemo = demoModeActive && missionManager != null && missionManager.Phase != MissionPhase.Idle;
        SetButtonLabel(panelLauncherButtons[6], activeDemo ? "시연 중지" : "시연");
        SetButtonVisual(panelLauncherButtons[6], activeDemo ? ButtonVisual.Cta : ButtonVisual.Secondary);
    }

    private bool IsPanelOpen(RectTransform panel)
    {
        return panel != null && panel.gameObject.activeSelf;
    }

    private void CreateCompass(Transform parent)
    {
        compassNeedle = null;
    }

    private void UpdateCompass()
    {
    }

    private void RefreshAllPreservingLauncherPanels(RectTransform forceOpenPanel)
    {
        bool requestOpen = IsPanelOpen(requestFlowPanel) || forceOpenPanel == requestFlowPanel;
        bool satelliteOpen = IsPanelOpen(satelliteListPanel) || forceOpenPanel == satelliteListPanel;
        bool statusOpen = IsPanelOpen(missionStatusPanel) || forceOpenPanel == missionStatusPanel;
        bool timelineOpen = IsPanelOpen(timelinePanel) || forceOpenPanel == timelinePanel;

        RefreshAll();

        RestoreLauncherPanelState(requestFlowPanel, requestOpen && !requestFlowDismissed);
        RestoreLauncherPanelState(satelliteListPanel, satelliteOpen && !satelliteListDismissed);
        RestoreLauncherPanelState(missionStatusPanel, statusOpen && !missionStatusDismissed);
        RestoreLauncherPanelState(timelinePanel, timelineOpen && !timelineDismissed);
        if (forceOpenPanel != null)
        {
            forceOpenPanel.SetAsLastSibling();
        }
        ApplyFixedPanelOrder();
        RefreshPanelLauncherButtons();
    }

    private void RestoreLauncherPanelState(RectTransform panel, bool open)
    {
        if (panel == null)
        {
            return;
        }

        panel.gameObject.SetActive(open);
    }

    private void ApplyFixedPanelOrder()
    {
        BringPanelForward(satelliteListPanel);
        BringPanelForward(missionStatusPanel);
        BringPanelForward(timelinePanel);
        BringPanelForward(transferLogPanel);
        BringPanelForward(queueMessagePanel);
        BringPanelForward(requestFlowPanel);
        BringPanelForward(historyPanel);
        BringPanelForward(resultPanel);
        BringPanelForward(settingsPanel);
        BringPanelForward(failurePanel);
        BringPanelForward(panelLauncher);
        BringPanelForward(confirmDialogPanel);
        BringPanelForward(toastPanel);
    }

    private void BringPanelForward(RectTransform panel)
    {
        if (panel != null)
        {
            panel.SetAsLastSibling();
        }
    }

    private void CreateToastPanel(Transform parent)
    {
        toastPanel = CreatePanel(parent, "ToastPanel", new Vector2(0.34f, 0.83f), new Vector2(0.66f, 0.90f), new Color(0.02f, 0.12f, 0.16f, 0.94f));
        toastText = CreateText(toastPanel, string.Empty, 15, TextAnchor.MiddleCenter);
        toastText.rectTransform.offsetMin = new Vector2(18f, 4f);
        toastText.rectTransform.offsetMax = new Vector2(-18f, -4f);
        toastPanel.gameObject.SetActive(false);
    }

    private void CreateConfirmDialogPanel(Transform parent)
    {
        confirmDialogPanel = CreatePanel(parent, "ConfirmDialog", new Vector2(0.34f, 0.34f), new Vector2(0.66f, 0.60f), new Color(0.02f, 0.06f, 0.1f, 0.98f));
        MakeDraggable(confirmDialogPanel);
        confirmDialogText = CreateText(confirmDialogPanel, string.Empty, 16, TextAnchor.UpperLeft);
        confirmDialogText.rectTransform.offsetMin = new Vector2(22f, 68f);
        confirmDialogText.rectTransform.offsetMax = new Vector2(-22f, -18f);
        confirmDialogText.color = new Color(0.92f, 0.98f, 1f, 0.94f);
        CreateButton(confirmDialogPanel, "취소", new Vector2(0f, 0f), new Vector2(0.48f, 0f), new Vector2(22f, 8f), new Vector2(-6f, 48f), delegate { HideConfirmDialog(); });
        CreateButton(confirmDialogPanel, "확인", new Vector2(0.5f, 0f), new Vector2(1f, 0f), new Vector2(6f, 8f), new Vector2(-22f, 48f), delegate { RunConfirmAction(); });
        confirmDialogPanel.gameObject.SetActive(false);
    }

    private void EnablePanelResize()
    {
        // 우측 하단 빈 사각형처럼 보이던 ResizeHandle 장식은 생성하지 않습니다.
    }

    private void LoadPanelLayout()
    {
        RestorePanelLayout(requestFlowPanel);
        RestorePanelLayout(satelliteListPanel);
        RestorePanelLayout(missionStatusPanel);
        RestorePanelLayout(timelinePanel);
        RestorePanelLayout(transferLogPanel);
        RestorePanelLayout(resultPanel);
        RestorePanelLayout(historyPanel);
        RestorePanelLayout(queueMessagePanel);
        RestorePanelLayout(settingsPanel);
        RestorePanelLayout(failurePanel);
    }

    private void RestorePanelLayout(RectTransform panel)
    {
        if (panel == null)
        {
            return;
        }

        string key = PanelPersistKey(panel);
        if (PlayerPrefs.HasKey(key + ".x") && PlayerPrefs.HasKey(key + ".y"))
        {
            panel.anchoredPosition = new Vector2(PlayerPrefs.GetFloat(key + ".x"), PlayerPrefs.GetFloat(key + ".y"));
        }
        if (PlayerPrefs.HasKey(key + ".w") && PlayerPrefs.HasKey(key + ".h"))
        {
            Vector2 minimumSize = PanelMinimumSize(panel);
            panel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Max(minimumSize.x, PlayerPrefs.GetFloat(key + ".w")));
            panel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Max(minimumSize.y, PlayerPrefs.GetFloat(key + ".h")));
        }
    }

    private void RefreshGuidanceButtons()
    {
        if (missionManager == null)
        {
            return;
        }

        if (scheduleRecommendedButton != null)
        {
            scheduleRecommendedButton.gameObject.SetActive(false);
        }
        if (satelliteAvailableOnlyButton != null)
        {
            satelliteAvailableOnlyButton.gameObject.SetActive(false);
        }
        if (satelliteFavoriteButton != null)
        {
            satelliteFavoriteButton.gameObject.SetActive(false);
            satelliteFavoriteButton.interactable = false;
        }
        if (satelliteRecommendedTimeButton != null)
        {
            satelliteRecommendedTimeButton.gameObject.SetActive(false);
        }
        if (failureRetryApplyButton != null)
        {
            failureRetryApplyButton.gameObject.SetActive(false);
        }
    }

    private void ResetPanelLayout()
    {
        SetPanelRect(requestFlowPanel, new Vector2(0.64f, 0.13f), new Vector2(0.985f, 0.86f));
        SetPanelRect(satelliteListPanel, new Vector2(0.015f, 0.08f), new Vector2(0.30f, 0.89f));
        SetPanelRect(missionStatusPanel, new Vector2(0.025f, 0.08f), new Vector2(0.30f, 0.255f));
        SetPanelRect(timelinePanel, new Vector2(0.33f, 0.82f), new Vector2(0.67f, 0.895f));
        SetPanelRect(transferLogPanel, new Vector2(0.315f, 0.455f), new Vector2(0.605f, 0.66f));
        SetPanelRect(resultPanel, new Vector2(0.34f, 0.22f), new Vector2(0.66f, 0.68f));
        SetPanelRect(historyPanel, new Vector2(0.18f, 0.14f), new Vector2(0.82f, 0.84f));
        SetPanelRect(queueMessagePanel, new Vector2(0.34f, 0.12f), new Vector2(0.86f, 0.62f));
        SetPanelRect(failurePanel, new Vector2(0.33f, 0.34f), new Vector2(0.585f, 0.64f));
        SetPanelRect(settingsPanel, new Vector2(0.315f, 0.25f), new Vector2(0.605f, 0.75f));
        SetPanelRect(confirmDialogPanel, new Vector2(0.34f, 0.34f), new Vector2(0.66f, 0.60f));
        ClearPanelPrefs(requestFlowPanel);
        ClearPanelPrefs(satelliteListPanel);
        ClearPanelPrefs(missionStatusPanel);
        ClearPanelPrefs(timelinePanel);
        ClearPanelPrefs(transferLogPanel);
        ClearPanelPrefs(resultPanel);
        ClearPanelPrefs(historyPanel);
        ClearPanelPrefs(queueMessagePanel);
        ClearPanelPrefs(failurePanel);
        ClearPanelPrefs(settingsPanel);
        ClearPanelPrefs(confirmDialogPanel);
        requestFlowDismissed = false;
        satelliteListDismissed = false;
        missionStatusDismissed = false;
        timelineDismissed = false;
        failurePanelDismissed = false;
        ShowToast("패널 위치를 정리했습니다.");
        RefreshAll();
    }

    private void SetPanelRect(RectTransform panel, Vector2 anchorMin, Vector2 anchorMax)
    {
        if (panel == null)
        {
            return;
        }
        panel.anchorMin = anchorMin;
        panel.anchorMax = anchorMax;
        panel.offsetMin = Vector2.zero;
        panel.offsetMax = Vector2.zero;
    }

    private void ClearPanelPrefs(RectTransform panel)
    {
        string key = PanelPersistKey(panel);
        if (string.IsNullOrEmpty(key))
        {
            return;
        }
        PlayerPrefs.DeleteKey(key + ".x");
        PlayerPrefs.DeleteKey(key + ".y");
        PlayerPrefs.DeleteKey(key + ".w");
        PlayerPrefs.DeleteKey(key + ".h");
        PlayerPrefs.Save();
    }

    private void ShowConfirm(string message, UnityAction action)
    {
        pendingConfirmAction = action;
        pendingConfirmMessage = message;
        if (confirmDialogText != null)
        {
            confirmDialogText.text = message;
        }
        if (confirmDialogPanel != null)
        {
            confirmDialogPanel.gameObject.SetActive(true);
            confirmDialogPanel.SetAsLastSibling();
        }
        ApplyFixedPanelOrder();
    }

    private void HideConfirmDialog()
    {
        pendingConfirmAction = null;
        pendingConfirmMessage = string.Empty;
        if (confirmDialogPanel != null)
        {
            confirmDialogPanel.gameObject.SetActive(false);
        }
    }

    private void RunConfirmAction()
    {
        UnityAction action = pendingConfirmAction;
        HideConfirmDialog();
        if (action != null)
        {
            action.Invoke();
        }
    }

    private void ShowToast(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }
        if (message == lastToastMessage && toastPanel != null && toastPanel.gameObject.activeSelf && Time.unscaledTime <= toastVisibleUntil)
        {
            return;
        }
        lastToastMessage = message;
        toastVisibleUntil = Time.unscaledTime + 2.4f;
        if (toastText != null)
        {
            toastText.text = message;
        }
        if (toastPanel != null)
        {
            toastPanel.gameObject.SetActive(true);
            toastPanel.SetAsLastSibling();
        }
        ApplyFixedPanelOrder();
    }

    private string UiMessage(string message)
    {
        return string.IsNullOrEmpty(message) ? string.Empty : message.Replace("실패 테스트", "실패 시뮬레이션");
    }

    private void MaybeShowMissionToast(string message)
    {
        if (string.IsNullOrWhiteSpace(message) || message == lastToastMessage)
        {
            return;
        }
        ShowToast(message);
    }

    private void UpdateToastPanel()
    {
        if (toastPanel == null || !toastPanel.gameObject.activeSelf)
        {
            return;
        }
        if (Time.unscaledTime > toastVisibleUntil)
        {
            toastPanel.gameObject.SetActive(false);
        }
    }

    private bool CloseTopOverlay()
    {
        if (confirmDialogPanel != null && confirmDialogPanel.gameObject.activeSelf)
        {
            HideConfirmDialog();
            return true;
        }
        if (calendarPickerPanel != null && calendarPickerPanel.gameObject.activeSelf)
        {
            ToggleCalendarPicker(false);
            return true;
        }
        if (attachmentPickerPanel != null && attachmentPickerPanel.gameObject.activeSelf)
        {
            ToggleAttachmentPicker(false);
            return true;
        }
        if (queueAttachmentPickerPanel != null && queueAttachmentPickerPanel.gameObject.activeSelf)
        {
            ToggleQueueAttachmentPicker(false);
            return true;
        }
        if (settingsPanel != null && settingsPanel.gameObject.activeSelf)
        {
            settingsPanel.gameObject.SetActive(false);
            RefreshPanelLauncherButtons();
            return true;
        }
        if (failurePanel != null && failurePanel.gameObject.activeSelf)
        {
            failurePanelDismissed = true;
            failurePanel.gameObject.SetActive(false);
            return true;
        }
        if (transferLogPanel != null && transferLogPanel.gameObject.activeSelf)
        {
            transferLogPanel.gameObject.SetActive(false);
            return true;
        }
        if (historyPanel != null && historyPanel.gameObject.activeSelf)
        {
            historyPanel.gameObject.SetActive(false);
            RefreshPanelLauncherButtons();
            return true;
        }
        if (queueMessagePanel != null && queueMessagePanel.gameObject.activeSelf)
        {
            queueMessagePanel.gameObject.SetActive(false);
            RefreshPanelLauncherButtons();
            return true;
        }
        if (resultPanel != null && resultPanel.gameObject.activeSelf)
        {
            DismissResultPanel();
            return true;
        }
        return false;
    }

    private void StartDemoMode()
    {
        if (missionManager == null)
        {
            return;
        }
        if (missionManager.Phase != MissionPhase.Idle)
        {
            if (demoModeActive)
            {
                ShowConfirm("현재 시연을 중지하고 메시지 작성으로 돌아갈까요?", delegate { StopDemoModeNow(); });
            }
            else
            {
                ShowConfirm("현재 전송을 정리하고 시연을 시작할까요?", delegate { StartDemoModeNow(); });
            }
            return;
        }
        StartDemoModeNow();
    }

    private void StopDemoModeNow()
    {
        if (missionManager == null)
        {
            return;
        }

        missionManager.ResetMission();
        demoModeActive = false;
        composingQueuedMessageInFlow = false;
        requestFlowDismissed = false;
        missionStatusDismissed = true;
        timelineDismissed = true;
        currentFlowStep = 1;
        ShowToast("시연을 중지했습니다.");
        RefreshAll();
    }

    private void StartDemoModeNow()
    {
        if (missionManager == null)
        {
            return;
        }
        missionManager.ResetMission();
        missionManager.SetDemoRouteReplayEnabled(true);
        demoModeActive = true;
        string[] targets = OrbitShareDestinations.DemoTargetNames;
        int[] delays = { 0, 5, 15, 30, 0, 5, 15 };
        string[] messages =
        {
            "뉴욕 수신 확인. 빠른 위성망으로 전송합니다.",
            "런던 지연 전송 테스트입니다.",
            "서울 백업 위성망 확인 메시지입니다.",
            "도쿄 즉시 위성망 확인 메시지입니다.",
            "파리 저비용 위성망 확인 메시지입니다.",
            "시드니 남반구 위성망 확인 메시지입니다.",
            "LA 태평양 위성망 확인 메시지입니다."
        };
        string[] files = { "demo-route.txt", "weather-note.csv", "status-image.jpg", "tokyo-ping.txt", "paris-cost.csv", "sydney-south.txt", "la-pacific.txt" };
        int[] sizes = { 8, 12, 48, 10, 16, 14, 18 };
        int scenario = demoScenarioIndex % targets.Length;
        demoScenarioIndex++;
        missionManager.SelectTarget(targets[scenario]);
        missionManager.SetSendDelayMinutes(delays[scenario]);
        missionManager.SetMessageText(messages[scenario]);
        missionManager.SetAttachment(files[scenario], sizes[scenario]);
        if (satelliteManager != null && missionManager.SelectedTarget != null)
        {
            SatelliteData best = satelliteManager.FindBestSatellite(missionManager.SelectedTarget.targetName, missionManager.RequestedSendDelayMinutes, missionManager.MessageSizeKb);
            if (best != null)
            {
                satelliteManager.SelectSatellite(best.satelliteName);
            }
        }
        requestFlowDismissed = false;
        missionStatusDismissed = false;
        timelineDismissed = false;
        currentFlowStep = 5;
        StartSelectedMission();
        ShowToast("발표 시연: " + DestinationLabel(targets[scenario]) + " · " + DelayLabel(delays[scenario]) + " · 추천 위성망으로 전송합니다.");
        RefreshAll();
        ShowTimelinePanel();
    }

    private string AvailabilityTimelineText()
    {
        if (satelliteManager == null || missionManager == null)
        {
            return string.Empty;
        }
        string targetName = missionManager.SelectedTarget != null ? missionManager.SelectedTarget.targetName : OrbitShareDestinations.DefaultTargetName;
        int[] delays = { 0, 5, 15, 30, 60, 120 };
        System.Text.StringBuilder builder = new System.Text.StringBuilder("예약 타임라인\n");
        for (int i = 0; i < delays.Length; i++)
        {
            if (i > 0)
            {
                builder.Append("  →  ");
            }
            builder.Append(DelayLabel(delays[i]))
                .Append("(")
                .Append(AvailableSatelliteCountAtDelay(targetName, delays[i]))
                .Append("개)");
        }
        builder.Append("\n빠른 시간순으로 추천 가능 수를 표시합니다.");
        return builder.ToString();
    }
}
