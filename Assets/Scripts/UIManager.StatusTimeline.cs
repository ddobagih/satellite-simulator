using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// 전송 상태/진행 UI: 상태 창, 단계 타임라인, 통신 로그 표시를 관리합니다.
public partial class UIManager
{
    private Image statusIslFill;
    private Text statusUplinkLabelText;
    private Text statusIslLabelText;
    private Text statusDownlinkLabelText;
    private Text statusUplinkPercentText;
    private Text statusIslPercentText;
    private Text statusDownlinkPercentText;
    private float statusUplinkDisplayedProgress;
    private float statusIslDisplayedProgress;
    private float statusDownlinkDisplayedProgress;

    private void CreateMissionStatusPanel(Transform parent)
    {
        RectTransform statusPanel = CreatePanel(parent, "CommunicationStatusPanel", new Vector2(0.025f, 0.08f), new Vector2(0.30f, 0.275f), new Color(0.025f, 0.045f, 0.075f, 0.96f));
        missionStatusPanel = statusPanel;
        MakeDraggable(statusPanel);
        missionHeadlineText = CreateText(statusPanel, string.Empty, 22, TextAnchor.UpperLeft);
        missionHeadlineText.fontStyle = FontStyle.Bold;
        missionHeadlineText.rectTransform.offsetMin = new Vector2(18f, 136f);
        missionHeadlineText.rectTransform.offsetMax = new Vector2(-58f, -12f);

        missionStatusText = CreateText(statusPanel, string.Empty, 15, TextAnchor.UpperLeft);
        missionStatusText.lineSpacing = 1.04f;
        missionStatusText.rectTransform.offsetMin = new Vector2(18f, 82f);
        missionStatusText.rectTransform.offsetMax = new Vector2(-18f, -52f);

        observationFill = CreateStatusSegmentProgress(statusPanel, "업링크", "UplinkProgress", 58f, new Color(0.32f, 0.82f, 1f, 0.95f), out statusUplinkLabelText, out statusUplinkPercentText);
        statusIslFill = CreateStatusSegmentProgress(statusPanel, "ISL", "IslProgress", 34f, new Color(1f, 0.68f, 0.12f, 0.95f), out statusIslLabelText, out statusIslPercentText);
        transferFill = CreateStatusSegmentProgress(statusPanel, "다운링크", "DownlinkProgress", 10f, new Color(0.24f, 1f, 0.84f, 0.95f), out statusDownlinkLabelText, out statusDownlinkPercentText);

        retryButton = CreateButton(statusPanel, "재시도", new Vector2(0.79f, 1f), new Vector2(0.89f, 1f), new Vector2(4f, -40f), new Vector2(-4f, -12f), delegate
        {
            missionManager.RetryFailedMission();
        });
        SetButtonTextSize(retryButton, 11);
        replayButton = CreateButton(statusPanel, "통신 재생", new Vector2(0.90f, 1f), new Vector2(1f, 1f), new Vector2(4f, -40f), new Vector2(-18f, -12f), delegate
        {
            missionManager.StartRouteReplay();
        });
        SetButtonTextSize(replayButton, 11);
        replayButton.gameObject.SetActive(false);
        CreateButton(statusPanel, "닫기", new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-78f, -44f), new Vector2(-14f, -10f), delegate { missionStatusDismissed = true; statusPanel.gameObject.SetActive(false); RefreshPanelLauncherButtons(); ShowToast("상태 창을 숨겼습니다."); });
        statusPanel.gameObject.SetActive(false);
    }

    private Image CreateStatusSegmentProgress(Transform parent, string label, string objectName, float y, Color fillColor, out Text labelText, out Text percentText)
    {
        labelText = CreateText(parent, label, 14, TextAnchor.MiddleLeft);
        labelText.rectTransform.anchorMin = new Vector2(0f, 0f);
        labelText.rectTransform.anchorMax = new Vector2(0f, 0f);
        labelText.rectTransform.offsetMin = new Vector2(18f, y - 2f);
        labelText.rectTransform.offsetMax = new Vector2(122f, y + 17f);
        labelText.color = new Color(0.86f, 0.93f, 1f, 0.88f);

        Image fill = CreateAnchoredProgressFill(
            parent,
            objectName,
            new Vector2(0f, 0f),
            new Vector2(1f, 0f),
            new Vector2(128f, y + 3f),
            new Vector2(-74f, y + 15f),
            fillColor);

        percentText = CreateText(parent, string.Empty, 13, TextAnchor.MiddleRight);
        percentText.rectTransform.anchorMin = new Vector2(1f, 0f);
        percentText.rectTransform.anchorMax = new Vector2(1f, 0f);
        percentText.rectTransform.offsetMin = new Vector2(-68f, y - 1f);
        percentText.rectTransform.offsetMax = new Vector2(-18f, y + 18f);
        percentText.color = new Color(0.92f, 0.98f, 1f, 0.86f);
        return fill;
    }

    private void CreateTimelinePanel(Transform parent)
    {
        RectTransform panel = CreatePanel(parent, "CommunicationTimelinePanel", new Vector2(0.33f, 0.82f), new Vector2(0.67f, 0.895f), new Color(0.025f, 0.055f, 0.085f, 0.985f));
        timelinePanel = panel;
        MakeDraggable(panel);
        timelineStatusText = CreateText(panel, string.Empty, 15, TextAnchor.MiddleCenter);
        timelineStatusText.color = new Color(0.88f, 0.96f, 1f, 0.86f);
        timelineStatusText.rectTransform.anchorMin = new Vector2(0.03f, 0.72f);
        timelineStatusText.rectTransform.anchorMax = new Vector2(0.90f, 0.96f);
        timelineStatusText.rectTransform.offsetMin = Vector2.zero;
        timelineStatusText.rectTransform.offsetMax = Vector2.zero;
        transferLogToggleButton = CreateButton(panel, "진행 로그", new Vector2(0.80f, 0.74f), new Vector2(0.97f, 0.89f), Vector2.zero, Vector2.zero, delegate { ToggleTransferLogPanel(); });
        routeReplayPauseButton = CreateButton(panel, "재생", new Vector2(0.80f, 0.54f), new Vector2(0.97f, 0.69f), Vector2.zero, Vector2.zero, delegate { ToggleRouteReplayPause(); });
        routeReplaySpeedButton = CreateButton(panel, "x1", new Vector2(0.80f, 0.34f), new Vector2(0.97f, 0.49f), Vector2.zero, Vector2.zero, delegate { CycleRouteReplaySpeed(); });
        SetButtonTextSize(transferLogToggleButton, 13);
        SetButtonTextSize(routeReplayPauseButton, 13);
        SetButtonTextSize(routeReplaySpeedButton, 13);
        transferLogToggleButton.gameObject.SetActive(false);
        routeReplayPauseButton.gameObject.SetActive(false);
        routeReplaySpeedButton.gameObject.SetActive(false);
        CreateButton(panel, "닫기", new Vector2(0.865f, 0.83f), new Vector2(0.995f, 0.99f), Vector2.zero, Vector2.zero, delegate { timelineDismissed = true; panel.gameObject.SetActive(false); RefreshPanelLauncherButtons(); ShowToast("진행 창을 숨겼습니다."); });
        string[] labels = { "업링크", "ISL", "다운링크" };
        timelineDots.Clear();
        timelineLabels.Clear();

        for (int i = 0; i < labels.Length; i++)
        {
            float contentLeft = 0.04f;
            float contentRight = 0.90f;
            float contentWidth = contentRight - contentLeft;
            float left = contentLeft + i / (float)labels.Length * contentWidth;
            float right = contentLeft + (i + 1f) / labels.Length * contentWidth;

            GameObject dotObject = new GameObject(labels[i] + "Dot", typeof(RectTransform), typeof(Image));
            dotObject.transform.SetParent(panel, false);
            RectTransform dotRect = dotObject.GetComponent<RectTransform>();
            dotRect.anchorMin = new Vector2((left + right) * 0.5f, 0.46f);
            dotRect.anchorMax = dotRect.anchorMin;
            dotRect.pivot = new Vector2(0.5f, 0.5f);
            dotRect.sizeDelta = new Vector2(42f, 16f);
            Image dot = dotObject.GetComponent<Image>();
            dot.color = new Color(1f, 1f, 1f, 0.16f);
            timelineDots.Add(dot);

            Text label = CreateText(panel, labels[i], 13, TextAnchor.MiddleCenter);
            label.rectTransform.anchorMin = new Vector2(left, 0.08f);
            label.rectTransform.anchorMax = new Vector2(right, 0.31f);
            label.rectTransform.offsetMin = Vector2.zero;
            label.rectTransform.offsetMax = Vector2.zero;
            label.fontSize = 12;
            label.resizeTextForBestFit = true;
            label.resizeTextMinSize = 10;
            label.resizeTextMaxSize = 12;
            timelineLabels.Add(label);
        }
        panel.gameObject.SetActive(false);
    }

    private void CreateTransferLogPanel(Transform parent)
    {
        transferLogPanel = CreatePanel(parent, "TransferLogPanel", new Vector2(0.315f, 0.455f), new Vector2(0.605f, 0.66f), new Color(0.02f, 0.06f, 0.1f, 0.97f));
        MakeDraggable(transferLogPanel);

        Text heading = CreateText(transferLogPanel, "진행 기록", 18, TextAnchor.UpperLeft);
        heading.fontStyle = FontStyle.Bold;
        heading.rectTransform.anchorMin = new Vector2(0f, 1f);
        heading.rectTransform.anchorMax = new Vector2(1f, 1f);
        heading.rectTransform.pivot = new Vector2(0.5f, 1f);
        heading.rectTransform.anchoredPosition = new Vector2(0f, -12f);
        heading.rectTransform.sizeDelta = new Vector2(-64f, 28f);

        transferLogText = CreateText(transferLogPanel, string.Empty, 12, TextAnchor.UpperLeft);
        transferLogText.lineSpacing = 1.02f;
        transferLogText.rectTransform.offsetMin = new Vector2(18f, 18f);
        transferLogText.rectTransform.offsetMax = new Vector2(-18f, -52f);
        transferLogText.color = new Color(0.86f, 0.95f, 1f, 0.88f);

        CreateButton(transferLogPanel, "닫기", new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-78f, -44f), new Vector2(-14f, -10f), delegate { transferLogPanel.gameObject.SetActive(false); });
        transferLogPanel.gameObject.SetActive(false);
    }

    private void RefreshMissionPanel()
    {
        if (missionStatusText == null || missionManager == null)
        {
            return;
        }

        if (titleText != null && simulationManager != null)
        {
            titleText.text = HeaderStateLabel();
        }
        RefreshTopButtons();
        RefreshQueuePanel();

        if (missionHeadlineText != null)
        {
            missionHeadlineText.text = "전송 상태";
        }

        bool isComplete = missionManager.Phase == MissionPhase.Complete;
        bool isActiveTransfer = IsActiveTransferPhase(missionManager.Phase);
        bool canEdit = missionManager.Phase == MissionPhase.Idle || composingQueuedMessageInFlow;
        bool hasStatusMessage = HasStatusMessageContent();
        RefreshScheduleDropdowns();
        RefreshDestinationButtons();
        RefreshSendOptionButtons();
        RefreshFlowControls();

        if (scheduleSummaryText != null)
        {
            scheduleSummaryText.text = FlowHasScheduleSelection()
                ? "선택: " + (composingQueuedMessageInFlow ? QueueFlowScheduleLabel() : SendTimeLabel())
                : "보낼 시간을 선택하세요.";
        }
        if (availabilityTimelineText != null)
        {
            availabilityTimelineText.text = string.Empty;
        }

        if (messageInputField != null)
        {
            messageInputField.interactable = canEdit;
            if (!composingQueuedMessageInFlow && !messageInputField.isFocused && messageInputField.text != missionManager.MessageText)
            {
                messageInputField.SetTextWithoutNotify(missionManager.MessageText ?? string.Empty);
            }
        }

        if (payloadWarningText != null)
        {
            bool payloadTooLarge = FlowPayloadTooLarge();
            payloadWarningText.text = payloadTooLarge
                ? "⚠ 크기 초과\n" + PayloadBreakdownText()
                : PayloadBreakdownText();
            payloadWarningText.color = payloadTooLarge
                ? new Color(1f, 0.48f, 0.42f, 0.96f)
                : new Color(0.68f, 1f, 0.86f, 0.82f);
        }
        if (payloadFill != null && missionManager != null)
        {
            float payloadRatio = composingQueuedMessageInFlow
                ? QueuedFlowPayloadKb() / (float)Mathf.Max(1, missionManager.MaxPayloadKb)
                : missionManager.MessageSizeKb / (float)Mathf.Max(1, missionManager.MaxPayloadKb);
            payloadFill.fillAmount = Mathf.Clamp01(payloadRatio);
            payloadFill.color = FlowPayloadTooLarge() ? new Color(1f, 0.38f, 0.32f, 0.95f) : new Color(0.24f, 1f, 0.84f, 0.9f);
        }

        if (attachmentStatusText != null)
        {
            attachmentStatusText.text = string.Empty;
            attachmentStatusText.gameObject.SetActive(false);
        }
        if (attachmentPreviewText != null)
        {
            attachmentPreviewText.text = composingQueuedMessageInFlow ? AttachmentPreview(queueAttachments) : AttachmentPreview(missionManager.AttachmentFiles);
        }
        RefreshAttachmentButtons();
        RefreshAttachmentDeleteButtons();
        RefreshMessageInputModeButton();
        RefreshDraftButtons();

        string missionTargetName = missionManager.SelectedTarget != null ? missionManager.SelectedTarget.targetName : "None";
        SatelliteData selectedSatellite = satelliteManager != null ? satelliteManager.SelectedSatellite : null;
        string satelliteName = selectedSatellite != null ? selectedSatellite.satelliteName : "선택 필요";
        string countdown = CompactActiveCountdownText();
        if (!hasStatusMessage && !isActiveTransfer && !isComplete)
        {
            missionStatusText.text =
                "전송중인 메시지가 없습니다\n" +
                StatusFlowText() +
                QueuedStatusLine();
        }
        else if (!isActiveTransfer && (missionManager.Phase == MissionPhase.Idle || composingQueuedMessageInFlow))
        {
            string statusOriginLabel = composingQueuedMessageInFlow
                ? DestinationLabel(queueOriginName)
                : missionManager.HasExplicitOriginSelection ? missionManager.OriginLabel : "송신지 선택 전";
            string statusTargetLabel = composingQueuedMessageInFlow
                ? DestinationLabel(queueTargetName)
                : missionManager.HasExplicitDestinationSelection && missionManager.SelectedTarget != null ? DestinationLabel(missionTargetName) : "수신지 선택 전";
            missionStatusText.text =
                "경로 : " + statusOriginLabel + " → " + statusTargetLabel + "\n" +
                StatusFlowText() +
                QueuedStatusLine();
        }
        else
        {
            string statusLine = missionManager.Phase == MissionPhase.Failed
                ? "상태 : 실패 · " + (string.IsNullOrEmpty(missionManager.SuggestedRetryHint) ? "재시도 가능" : missionManager.SuggestedRetryHint)
                : "위성 : " + CompactSatelliteName(satelliteName) + (string.IsNullOrEmpty(countdown) ? string.Empty : " · " + countdown);
            missionStatusText.text =
                "경로 : " + missionManager.OriginLabel + " → " + DestinationLabel(missionTargetName) + "\n" +
                statusLine +
                QueuedStatusLine();
        }
        if (missionManager.Phase == MissionPhase.Complete || missionManager.Phase == MissionPhase.Failed)
        {
            MaybeShowMissionToast(UiMessage(missionManager.ResultMessage));
        }

        RefreshStatusSegmentBars(isActiveTransfer, isComplete, hasStatusMessage);

        if (resultPanel != null && resultText != null)
        {
            bool showHistoryRecord = visibleRecordIndex >= 0 && visibleRecordIndex < missionManager.SentMessages.Count;
            bool showResult = (isComplete && !resultDismissed) || showHistoryRecord;
            resultPanel.gameObject.SetActive(showResult);
            ApplyResultPanelPriority(showResult);
            if (showResult)
            {
                SentMessageRecord record = showHistoryRecord ? missionManager.SentMessages[visibleRecordIndex] : missionManager.SentMessages.Count > 0 ? missionManager.SentMessages[0] : null;
                string targetName = record != null && !string.IsNullOrEmpty(record.targetName)
                    ? record.targetName
                    : missionManager.SelectedTarget != null ? missionManager.SelectedTarget.targetName : "Unknown";
                resultDetailsExpanded = true;
                resultText.text = ResultReceiptHeadline(record);
                resultDataText.text = TransmissionResultText(record, targetName);
                RefreshResultSummaryBars(record);
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
                previewImage.gameObject.SetActive(false);
            }
            if (resultDetailsToggleButton != null)
            {
                resultDetailsToggleButton.gameObject.SetActive(false);
            }
            if (resultNextQueuedButton != null)
            {
                bool hasNextQueued = showResult && missionManager.QueuedMessageCount > 0;
                resultNextQueuedButton.gameObject.SetActive(hasNextQueued);
                resultNextQueuedButton.interactable = hasNextQueued;
            }
        }

        RefreshHistoryPanel();
        RefreshTimeline();
        RefreshTransferLogPanel();

        if (reserveButton != null)
        {
            string targetName = missionManager.SelectedTarget != null ? missionManager.SelectedTarget.targetName : OrbitShareDestinations.DefaultTargetName;
            bool hasMessage = !string.IsNullOrWhiteSpace(missionManager.MessageText);
            bool canReserve = satelliteManager != null
                && CanServeRoute(satelliteManager.SelectedSatellite, targetName, missionManager.RequestedSendDelayMinutes)
                && missionManager.Phase == MissionPhase.Idle
                && hasMessage
                && !missionManager.PayloadTooLarge;
            reserveButton.gameObject.SetActive(!isComplete);
            reserveButton.interactable = canReserve;
            SetButtonLabel(reserveButton, missionManager.Phase == MissionPhase.Reserved || missionManager.Phase == MissionPhase.Complete ? "예약 완료" : "위성 선택 완료");
            SetButtonVisual(reserveButton, canReserve ? ButtonVisual.Secondary : ButtonVisual.Disabled);
        }
        if (startMissionButton != null)
        {
            if (composingQueuedMessageInFlow)
            {
                bool canQueue = CanQueueFlowSelection();
                startMissionButton.gameObject.SetActive(true);
                startMissionButton.interactable = canQueue;
                SetButtonLabel(startMissionButton, "메시지 추가");
                SetButtonVisual(startMissionButton, canQueue ? ButtonVisual.Primary : ButtonVisual.Disabled);
            }
            else
            {
            bool canStart = (missionManager.Phase == MissionPhase.Idle && CanReserveCurrentSelection()) || missionManager.Phase == MissionPhase.Reserved;
            startMissionButton.gameObject.SetActive(!isComplete);
            startMissionButton.interactable = canStart;
            SetButtonLabel(startMissionButton, missionManager.Phase == MissionPhase.Idle ? "전송 시작" : missionManager.Phase == MissionPhase.Reserved ? "예약 전송 시작" : missionManager.Phase == MissionPhase.Failed ? "전송 실패" : "현재 전송");
            SetButtonVisual(startMissionButton, canStart ? ButtonVisual.Primary : isActiveTransfer ? ButtonVisual.Selected : ButtonVisual.Disabled);
            }
        }
        if (resetButton != null)
        {
            if (composingQueuedMessageInFlow)
            {
                resetButton.interactable = true;
                SetButtonLabel(resetButton, "작성 취소");
                SetButtonVisual(resetButton, ButtonVisual.Secondary);
            }
            else
            {
            resetButton.interactable = isActiveTransfer || missionManager.Phase != MissionPhase.Idle || currentFlowStep > 1;
            SetButtonLabel(resetButton, isActiveTransfer ? "전송 상태" : isComplete ? "새 메시지 작성" : "전송 취소");
            SetButtonVisual(resetButton, isActiveTransfer ? ButtonVisual.Primary : isComplete ? ButtonVisual.Cta : resetButton.interactable ? ButtonVisual.Secondary : ButtonVisual.Disabled);
            }
        }
        RefreshStatusOptionButtons();
        RefreshGuidanceButtons();
        ApplyBottomActionButtonLayout();
        SetConfirmSummaryText(ReservationHintText());
        if (messageInputBox != null)
        {
            messageInputBox.color = FlowPayloadTooLarge()
                ? new Color(0.24f, 0.08f, 0.08f, 0.98f)
                : new Color(0.08f, 0.11f, 0.16f, 0.98f);
        }
        RemoveMissionStatusResizeHandle();
    }

    private bool HasStatusMessageContent()
    {
        if (missionManager == null)
        {
            return false;
        }
        if (missionManager.Phase != MissionPhase.Idle)
        {
            return true;
        }
        if (composingQueuedMessageInFlow)
        {
            return !string.IsNullOrWhiteSpace(QueuedFlowMessageText());
        }
        return !string.IsNullOrWhiteSpace(missionManager.MessageText);
    }

    private string QueuedStatusLine()
    {
        return string.Empty;
    }

    private string StatusFlowText()
    {
        bool useIsl = missionManager == null || missionManager.ShouldUseInterSatelliteLink;
        return useIsl
            ? "흐름 : 업링크 → ISL → 다운링크"
            : "흐름 : 업링크 → 다운링크";
    }

    private void RefreshStatusSegmentBars(bool isActiveTransfer, bool isComplete, bool hasStatusMessage)
    {
        if (missionManager == null)
        {
            return;
        }

        int activeIndex = CommunicationStepToTimelineIndex(missionManager.CommunicationStep);
        bool failed = missionManager.Phase == MissionPhase.Failed;
        bool segmentsVisible = isActiveTransfer || isComplete || failed;
        SetStatusSegmentsVisible(segmentsVisible);
        bool showProgress = isActiveTransfer || isComplete || failed;
        bool snapProgress = !showProgress || isComplete || failed || !Application.isPlaying;
        float uplinkTarget = showProgress ? StatusSegmentProgressTarget(0, activeIndex, StatusUplinkProgress(isComplete), isComplete) : 0f;
        float islTarget = showProgress ? StatusSegmentProgressTarget(1, activeIndex, StatusIslProgress(isComplete), isComplete) : 0f;
        float downlinkTarget = showProgress ? StatusSegmentProgressTarget(2, activeIndex, StatusDownlinkProgress(isComplete), isComplete) : 0f;
        statusUplinkDisplayedProgress = SmoothedStatusProgress(statusUplinkDisplayedProgress, uplinkTarget, snapProgress);
        statusIslDisplayedProgress = SmoothedStatusProgress(statusIslDisplayedProgress, islTarget, snapProgress);
        statusDownlinkDisplayedProgress = SmoothedStatusProgress(statusDownlinkDisplayedProgress, downlinkTarget, snapProgress);
        UpdateStatusSegmentBar(
            observationFill,
            statusUplinkLabelText,
            statusUplinkPercentText,
            0,
            statusUplinkDisplayedProgress,
            activeIndex,
            isComplete,
            failed,
            new Color(0.32f, 0.82f, 1f, 0.95f),
            showProgress);
        UpdateStatusSegmentBar(
            statusIslFill,
            statusIslLabelText,
            statusIslPercentText,
            1,
            statusIslDisplayedProgress,
            activeIndex,
            isComplete,
            failed,
            new Color(1f, 0.68f, 0.12f, 0.95f),
            showProgress);
        UpdateStatusSegmentBar(
            transferFill,
            statusDownlinkLabelText,
            statusDownlinkPercentText,
            2,
            statusDownlinkDisplayedProgress,
            activeIndex,
            isComplete,
            failed,
            new Color(0.24f, 1f, 0.84f, 0.95f),
            showProgress);
    }

    private void SetStatusSegmentsVisible(bool visible)
    {
        SetStatusSegmentVisible(statusUplinkLabelText, observationFill, statusUplinkPercentText, visible);
        SetStatusSegmentVisible(statusIslLabelText, statusIslFill, statusIslPercentText, visible && (missionManager == null || missionManager.ShouldUseInterSatelliteLink));
        SetStatusSegmentVisible(statusDownlinkLabelText, transferFill, statusDownlinkPercentText, visible);
    }

    private void SetStatusSegmentVisible(Text label, Image fill, Text percentText, bool visible)
    {
        if (label != null)
        {
            label.gameObject.SetActive(visible);
        }
        if (fill != null && fill.transform.parent != null)
        {
            fill.transform.parent.gameObject.SetActive(visible);
        }
        if (percentText != null)
        {
            percentText.gameObject.SetActive(visible);
        }
    }

    private float SmoothedStatusProgress(float current, float target, bool snap)
    {
        target = Mathf.Clamp01(target);
        if (snap || target < current || Mathf.Abs(target - current) > 0.55f)
        {
            return target;
        }

        float step = 1f - Mathf.Exp(-Time.unscaledDeltaTime * 5.2f);
        float next = Mathf.Lerp(current, target, Mathf.Clamp01(step));
        if (Mathf.Abs(target - next) < 0.003f)
        {
            return target;
        }
        return next;
    }

    private float StatusSegmentProgressTarget(int index, int activeIndex, float progress, bool isComplete)
    {
        if (isComplete)
        {
            return 1f;
        }
        if (index < activeIndex)
        {
            return 1f;
        }
        if (index > activeIndex)
        {
            return 0f;
        }
        return Mathf.Clamp01(progress);
    }

    private void UpdateStatusSegmentBar(Image fill, Text labelText, Text percentText, int index, float progress, int activeIndex, bool isComplete, bool failed, Color baseColor, bool showProgress)
    {
        progress = Mathf.Clamp01(progress);
        bool segmentComplete = isComplete || index < activeIndex || progress >= 0.995f;
        bool segmentActive = index == activeIndex && !isComplete && (!segmentComplete || failed);
        bool failedHere = failed && segmentActive;
        Color color = failedHere
            ? new Color(1f, 0.28f, 0.24f, 0.95f)
            : segmentActive
            ? Color.Lerp(baseColor, new Color(1f, 1f, 1f, 0.98f), Mathf.PingPong(Time.time * 2.2f, 0.35f))
            : segmentComplete
            ? baseColor
            : ColorWithAlpha(baseColor, 0.28f);

        if (fill != null)
        {
            fill.fillAmount = progress;
            fill.color = color;
        }
        if (labelText != null)
        {
            string baseLabel = TimelineBaseLabel(index);
            labelText.text = !showProgress
                ? baseLabel
                : failedHere
                ? "실패 · " + baseLabel
                : segmentActive
                ? baseLabel
                : segmentComplete
                ? "완료 · " + baseLabel
                : baseLabel;
            labelText.fontStyle = segmentActive ? FontStyle.Bold : FontStyle.Normal;
            labelText.color = failedHere
                ? new Color(1f, 0.58f, 0.52f, 1f)
                : segmentActive ? Color.white : segmentComplete ? new Color(0.70f, 1f, 0.92f, 0.95f) : new Color(0.86f, 0.93f, 1f, 0.72f);
        }
        if (percentText != null)
        {
            percentText.text = showProgress ? Mathf.RoundToInt(progress * 100f) + "%" : string.Empty;
            percentText.color = failedHere
                ? new Color(1f, 0.58f, 0.52f, 1f)
                : segmentComplete || segmentActive ? Color.white : new Color(1f, 1f, 1f, 0.62f);
        }
    }

    private float StatusUplinkProgress(bool isComplete)
    {
        if (isComplete)
        {
            return 1f;
        }
        return Mathf.Clamp01(missionManager.ObservationProgress);
    }

    private void ApplyBottomActionButtonLayout()
    {
        ApplyBottomActionButtonLayout(startMissionButton);
        ApplyBottomActionButtonLayout(resetButton);
    }

    private void ApplyBottomActionButtonLayout(Button button)
    {
        if (button == null)
        {
            return;
        }

        RectTransform rect = button.GetComponent<RectTransform>();
        if (rect == null)
        {
            return;
        }

        Vector2 offsetMin = rect.offsetMin;
        Vector2 offsetMax = rect.offsetMax;
        offsetMin.y = FlowBottomActionMinY;
        offsetMax.y = FlowBottomActionMaxY;
        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;
    }

    private float StatusIslProgress(bool isComplete)
    {
        if (isComplete)
        {
            return 1f;
        }

        float interSatellite = Mathf.Clamp01(missionManager.InterSatelliteProgress);
        float relaySelection = Mathf.Clamp01(missionManager.RelaySelectionProgress);
        return interSatellite < 1f
            ? interSatellite * 0.7f
            : Mathf.Clamp01(0.7f + relaySelection * 0.3f);
    }

    private float StatusDownlinkProgress(bool isComplete)
    {
        if (isComplete)
        {
            return 1f;
        }

        float downlink = Mathf.Clamp01(missionManager.DownlinkProgress);
        float finalReceive = Mathf.Clamp01(missionManager.FinalReceiveProgress);
        return downlink < 1f
            ? downlink * 0.85f
            : Mathf.Clamp01(0.85f + finalReceive * 0.15f);
    }

    private Color ColorWithAlpha(Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, alpha);
    }

    private void RemoveMissionStatusResizeHandle()
    {
        if (missionStatusPanel == null)
        {
            return;
        }

        Transform handle = missionStatusPanel.Find("ResizeHandle");
        if (handle == null)
        {
            return;
        }

        if (Application.isPlaying)
        {
            Destroy(handle.gameObject);
        }
        else
        {
            DestroyImmediate(handle.gameObject);
        }
    }

    private void RefreshStatusOptionButtons()
    {
        if (missionManager == null)
        {
            return;
        }

        if (autoQueueToggleButton != null)
        {
            autoQueueToggleButton.gameObject.SetActive(false);
            SetButtonLabel(autoQueueToggleButton, missionManager.AutoStartQueuedMessages ? "자동 ON" : "자동");
            SetButtonVisual(autoQueueToggleButton, missionManager.AutoStartQueuedMessages ? ButtonVisual.Selected : ButtonVisual.Secondary);
        }
        if (failureToggleButton != null)
        {
            bool canToggle = missionManager.Phase == MissionPhase.Idle || missionManager.Phase == MissionPhase.Reserved || missionManager.Phase == MissionPhase.Running || missionManager.Phase == MissionPhase.Observing || missionManager.Phase == MissionPhase.ObservationComplete || missionManager.Phase == MissionPhase.Transmitting;
            failureToggleButton.gameObject.SetActive(demoModeActive);
            failureToggleButton.interactable = canToggle;
            SetButtonLabel(failureToggleButton, missionManager.SimulateFailureNextSend ? "실패 ON" : "실패");
            SetButtonVisual(failureToggleButton, missionManager.SimulateFailureNextSend ? ButtonVisual.Selected : canToggle ? ButtonVisual.Secondary : ButtonVisual.Disabled);
        }
        if (retryButton != null)
        {
            bool canRetry = missionManager.Phase == MissionPhase.Failed;
            retryButton.gameObject.SetActive(canRetry);
            retryButton.interactable = canRetry;
            SetButtonLabel(retryButton, "다시 시도");
            SetButtonVisual(retryButton, canRetry ? ButtonVisual.Cta : ButtonVisual.Disabled);
        }
        if (replayButton != null)
        {
            bool canReplay = missionManager.Phase == MissionPhase.Complete || missionManager.SentMessages.Count > 0;
            replayButton.gameObject.SetActive(demoModeActive);
            replayButton.interactable = canReplay;
            SetButtonLabel(replayButton, missionManager.IsReplayingRoute ? "재생 중" : "재생");
            SetButtonVisual(replayButton, missionManager.IsReplayingRoute ? ButtonVisual.Selected : canReplay ? ButtonVisual.Secondary : ButtonVisual.Disabled);
        }
        if (resultReplayButton != null)
        {
            resultReplayButton.gameObject.SetActive(demoModeActive);
            resultReplayButton.interactable = demoModeActive;
        }
        if (routeReplayPauseButton != null)
        {
            bool canControlReplay = missionManager.IsReplayingRoute || missionManager.Phase == MissionPhase.Complete || missionManager.SentMessages.Count > 0;
            routeReplayPauseButton.gameObject.SetActive(demoModeActive);
            routeReplayPauseButton.interactable = canControlReplay;
            SetButtonLabel(routeReplayPauseButton, missionManager.IsRouteReplayPaused ? "재개" : missionManager.IsReplayingRoute ? "일시정지" : "재생");
            SetButtonVisual(routeReplayPauseButton, missionManager.IsReplayingRoute ? ButtonVisual.Selected : canControlReplay ? ButtonVisual.Secondary : ButtonVisual.Disabled);
        }
        if (routeReplaySpeedButton != null)
        {
            bool canControlReplay = missionManager.IsReplayingRoute || missionManager.Phase == MissionPhase.Complete || missionManager.SentMessages.Count > 0 || (demoModeActive && IsActiveTransferPhase(missionManager.Phase));
            routeReplaySpeedButton.gameObject.SetActive(demoModeActive);
            routeReplaySpeedButton.interactable = canControlReplay;
            SetButtonLabel(routeReplaySpeedButton, "배속 x" + missionManager.RouteReplaySpeed.ToString("0.#"));
            SetButtonVisual(routeReplaySpeedButton, canControlReplay ? ButtonVisual.Secondary : ButtonVisual.Disabled);
        }
        if (transferLogToggleButton != null)
        {
            transferLogToggleButton.gameObject.SetActive(demoModeActive);
            if (!demoModeActive && transferLogPanel != null)
            {
                transferLogPanel.gameObject.SetActive(false);
            }
        }
        if (!demoModeActive && timelinePanel != null && timelinePanel.gameObject.activeSelf)
        {
            timelinePanel.gameObject.SetActive(false);
        }
        if (statusStopButton != null)
        {
            bool canStop = IsActiveTransferPhase(missionManager.Phase);
            statusStopButton.gameObject.SetActive(canStop);
            statusStopButton.interactable = canStop;
            SetButtonLabel(statusStopButton, demoModeActive ? "시연 중지" : "전송 중지");
            SetButtonVisual(statusStopButton, canStop ? ButtonVisual.Cta : ButtonVisual.Disabled);
        }
        LayoutTopActionButtons();
    }

    private void ShowMissionStatusPanel()
    {
        bool wasDismissed = missionStatusDismissed;
        missionStatusDismissed = false;
        if (missionStatusPanel != null)
        {
            missionStatusPanel.gameObject.SetActive(true);
            missionStatusPanel.SetAsLastSibling();
        }
        if (wasDismissed)
        {
            ShowToast("상태 창을 다시 열었습니다.");
        }
        RefreshAllPreservingLauncherPanels(missionStatusPanel);
    }

    private void ToggleMissionStatusPanel()
    {
        if (missionStatusPanel == null)
        {
            return;
        }

        if (missionStatusPanel.gameObject.activeSelf && !missionStatusDismissed)
        {
            missionStatusDismissed = true;
            missionStatusPanel.gameObject.SetActive(false);
            RefreshAllPreservingLauncherPanels(null);
            return;
        }

        ShowMissionStatusPanel();
    }

    private void ShowTimelinePanel()
    {
        if (!demoModeActive)
        {
            timelineDismissed = false;
            if (timelinePanel != null)
            {
                timelinePanel.gameObject.SetActive(false);
            }
            ShowMissionStatusPanel();
            return;
        }

        bool wasDismissed = timelineDismissed;
        timelineDismissed = false;
        if (timelinePanel != null)
        {
            timelinePanel.gameObject.SetActive(true);
            timelinePanel.SetAsLastSibling();
        }
        if (wasDismissed)
        {
            ShowToast("진행 창을 다시 열었습니다.");
        }
        RefreshAllPreservingLauncherPanels(timelinePanel);
    }

    private void ToggleTimelinePanel()
    {
        if (!demoModeActive)
        {
            ShowTimelinePanel();
            return;
        }

        if (timelinePanel == null)
        {
            return;
        }

        if (timelinePanel.gameObject.activeSelf && !timelineDismissed)
        {
            timelineDismissed = true;
            timelinePanel.gameObject.SetActive(false);
            RefreshAllPreservingLauncherPanels(null);
            return;
        }

        ShowTimelinePanel();
    }

    private void FocusCameraView(string view)
    {
        if (cameraManager == null)
        {
            return;
        }

        if (view == "earth")
        {
            cameraManager.SetEarth();
        }
        else if (view == "satellite")
        {
            cameraManager.SetSatellite();
        }
        else if (view == "mission")
        {
            if (missionManager != null && missionManager.SelectedTarget != null)
            {
                cameraManager.missionTarget = missionManager.SelectedTarget.transform;
            }
            cameraManager.SetMission();
        }
        else
        {
            cameraManager.SetOverview();
        }
    }

    private void ToggleTransferLogPanel()
    {
        if (transferLogPanel == null)
        {
            return;
        }

        transferLogPanel.gameObject.SetActive(!transferLogPanel.gameObject.activeSelf);
        if (transferLogPanel.gameObject.activeSelf)
        {
            transferLogPanel.SetAsLastSibling();
        }
        RefreshTransferLogPanel();
    }

    private void RefreshTransferLogPanel()
    {
        if (transferLogText == null || missionManager == null)
        {
            return;
        }

        if (missionManager.MissionEventLog.Count == 0)
        {
            transferLogText.text = "아직 진행 기록이 없습니다.";
            return;
        }

        System.Text.StringBuilder builder = new System.Text.StringBuilder();
        for (int i = 0; i < missionManager.MissionEventLog.Count; i++)
        {
            builder.Append(i + 1).Append(". ").Append(SimplifyMissionEventLog(missionManager.MissionEventLog[i]));
            if (i < missionManager.MissionEventLog.Count - 1)
            {
                builder.Append("\n");
            }
        }
        transferLogText.text = builder.ToString();
    }

    private string SimplifyMissionEventLog(string rawLog)
    {
        if (string.IsNullOrWhiteSpace(rawLog))
        {
            return "진행 상태를 확인했습니다.";
        }

        string log = UiMessage(rawLog);
        string lower = log.ToLowerInvariant();
        if (log.Contains("예약") || lower.Contains("reserve"))
        {
            return "보낼 시간이 정해졌습니다.";
        }
        if (log.Contains("LEO 수신 완료") || log.Contains("발신 완료") || log.Contains("서울→위성 전송 완료"))
        {
            return "LEO 수신 완료: 업링크 메시지가 위성에 도착했습니다.";
        }
        if (log.Contains("ISL 전달"))
        {
            return "ISL 전달: 위성 간 링크로 목적지 방향에 넘깁니다.";
        }
        if (log.Contains("목적지 근처 위성 선택"))
        {
            return "목적지 근처 위성을 선택했습니다.";
        }
        if (log.Contains("다운링크 시작"))
        {
            return "다운링크 시작: 목적지 근처 위성에서 수신지로 보냅니다.";
        }
        if (log.Contains("수신") && (log.Contains("시작") || log.Contains("전송")))
        {
            return "다운링크 중: 목적지로 보내는 중입니다.";
        }
        if (log.Contains("최종 수신") || log.Contains("수신 완료") || log.Contains("도착 확인"))
        {
            return "도착 확인 완료: 목적지 도착을 확인했습니다.";
        }
        if (log.Contains("위성") && (log.Contains("연결") || log.Contains("확인") || lower.Contains("lock")))
        {
            return "위성과 연결을 확인했습니다.";
        }
        if (log.Contains("완료") || log.Contains("도착") || lower.Contains("complete") || lower.Contains("delivered"))
        {
            return "목적지에 도착했습니다.";
        }
        if (log.Contains("발신") || log.Contains("전송") || lower.Contains("transmit") || lower.Contains("send"))
        {
            return "메시지를 보내는 중입니다.";
        }
        if (log.Contains("실패") || lower.Contains("fail"))
        {
            return "문제가 발생했습니다. 재시도를 확인하세요.";
        }
        if (log.Length > 54)
        {
            log = log.Substring(0, 54) + "…";
        }
        return log;
    }

    private void RefreshTimeline()
    {
        if (timelineDots.Count == 0)
        {
            return;
        }

        if (missionVisualController == null)
        {
            missionVisualController = FindObjectOfType<MissionVisualController>();
        }
        if (missionVisualController != null && missionManager != null)
        {
            missionVisualController.SetRouteReplayVisualControls(missionManager.IsRouteReplayPaused, missionManager.RouteReplaySpeed, missionManager.RouteReplayProgress);
        }

        int activeIndex = CommunicationStepToTimelineIndex(missionManager.CommunicationStep);
        bool useIsl = missionManager == null || missionManager.ShouldUseInterSatelliteLink;
        if (timelineStatusText != null)
        {
            timelineStatusText.text = DetailedTimelineText();
        }
        for (int i = 0; i < timelineDots.Count; i++)
        {
            bool segmentVisible = i != 1 || useIsl;
            timelineDots[i].gameObject.SetActive(segmentVisible);
            if (i < timelineLabels.Count)
            {
                timelineLabels[i].gameObject.SetActive(segmentVisible);
            }
            if (!segmentVisible)
            {
                continue;
            }

            bool complete = i < activeIndex;
            bool active = i == activeIndex;
            bool failed = missionManager.Phase == MissionPhase.Failed && active;
            timelineDots[i].color = failed
                ? new Color(1f, 0.28f, 0.24f, 0.95f)
                : complete ? new Color(0.24f, 1f, 0.84f, 0.95f)
                : active ? new Color(1f, 0.76f, 0.18f, 0.95f) : new Color(1f, 1f, 1f, 0.22f);
            if (i < timelineLabels.Count)
            {
                timelineLabels[i].text = TimelineBaseLabel(i);
                timelineLabels[i].color = failed
                    ? new Color(1f, 0.58f, 0.52f, 1f)
                    : complete || active ? Color.white : new Color(1f, 1f, 1f, 0.62f);
            }
        }
    }

    private string TimelineBaseLabel(int index)
    {
        switch (index)
        {
            case 0:
                return "업링크";
            case 1:
                return "ISL";
            case 2:
                return "다운링크";
            default:
                return "진행";
        }
    }

    private int CommunicationStepToTimelineIndex(SatelliteCommunicationStep step)
    {
        switch (step)
        {
            case SatelliteCommunicationStep.Reserved:
            case SatelliteCommunicationStep.WaitingForUplink:
            case SatelliteCommunicationStep.Uplink:
            case SatelliteCommunicationStep.LeoReceived:
                return 0;
            case SatelliteCommunicationStep.InterSatelliteLink:
            case SatelliteCommunicationStep.DestinationRelaySelection:
                return 1;
            case SatelliteCommunicationStep.WaitingForDownlink:
            case SatelliteCommunicationStep.Downlink:
            case SatelliteCommunicationStep.FinalReceive:
                return 2;
            case SatelliteCommunicationStep.Complete:
                return 3;
            case SatelliteCommunicationStep.Failed:
                return 2;
            default:
                return -1;
        }
    }

    private string DetailedTimelineText()
    {
        if (missionManager == null)
        {
            return string.Empty;
        }

        switch (missionManager.CommunicationStep)
        {
            case SatelliteCommunicationStep.Reserved:
                return "전송 상태 · 예약됨";
            case SatelliteCommunicationStep.WaitingForUplink:
                return missionManager.RemainingWaitSeconds > 0.1f
                    ? "전송 상태 · 대기 " + FormatSeconds(Mathf.CeilToInt(missionManager.RemainingWaitSeconds))
                    : "전송 상태 · 업링크 준비";
            case SatelliteCommunicationStep.Uplink:
                return "전송 상태 · 업링크 " + Mathf.RoundToInt(missionManager.ObservationProgress * 100f) + "%";
            case SatelliteCommunicationStep.LeoReceived:
                return "전송 상태 · ISL 준비";
            case SatelliteCommunicationStep.InterSatelliteLink:
                return "전송 상태 · ISL " + Mathf.RoundToInt(missionManager.InterSatelliteProgress * 100f) + "%";
            case SatelliteCommunicationStep.DestinationRelaySelection:
                return "전송 상태 · 경로 선택 " + Mathf.RoundToInt(missionManager.RelaySelectionProgress * 100f) + "%";
            case SatelliteCommunicationStep.WaitingForDownlink:
                return "전송 상태 · 다운링크 대기 " + FormatSeconds(Mathf.CeilToInt(missionManager.RemainingTransferSeconds));
            case SatelliteCommunicationStep.Downlink:
                return "전송 상태 · 다운링크 " + Mathf.RoundToInt(missionManager.DownlinkProgress * 100f) + "% · 남은 " + FormatSeconds(Mathf.CeilToInt(missionManager.RemainingTransferSeconds));
            case SatelliteCommunicationStep.FinalReceive:
                return "전송 상태 · 도착 확인 " + Mathf.RoundToInt(missionManager.FinalReceiveProgress * 100f) + "%";
            case SatelliteCommunicationStep.Complete:
                return "전송 상태 · 완료";
            case SatelliteCommunicationStep.Failed:
                return "전송 실패: " + (string.IsNullOrEmpty(missionManager.LastFailureReason) ? "원인 확인 중" : missionManager.LastFailureReason)
                    + (string.IsNullOrEmpty(missionManager.SuggestedRetryHint) ? " · 재시도 가능" : " · " + missionManager.SuggestedRetryHint);
            default:
                return string.Empty;
        }
    }

    private string BarText(float value, float maxValue, int width)
    {
        float ratio = maxValue <= 0f ? 0f : Mathf.Clamp01(value / maxValue);
        int filled = Mathf.RoundToInt(ratio * width);
        System.Text.StringBuilder builder = new System.Text.StringBuilder("[");
        for (int i = 0; i < width; i++)
        {
            builder.Append(i < filled ? "■" : "□");
        }
        builder.Append("]");
        return builder.ToString();
    }

    private string MissionPhaseLabel(MissionPhase phase)
    {
        switch (phase)
        {
            case MissionPhase.Idle:
                return "작성 중";
            case MissionPhase.Reserved:
                return "예약 완료";
            case MissionPhase.Running:
                return "업링크 대기";
            case MissionPhase.Observing:
                return "업링크";
            case MissionPhase.ObservationComplete:
                return "ISL";
            case MissionPhase.Transmitting:
                return "다운링크";
            case MissionPhase.Complete:
                return "전송 완료";
            case MissionPhase.Failed:
                return "전송 실패";
            default:
                return phase.ToString();
        }
    }

    private bool IsActiveTransferPhase(MissionPhase phase)
    {
        return phase == MissionPhase.Reserved
            || phase == MissionPhase.Running
            || phase == MissionPhase.Observing
            || phase == MissionPhase.ObservationComplete
            || phase == MissionPhase.Transmitting;
    }

    private string HeaderStateLabel()
    {
        if (missionManager == null)
        {
            return "요청 준비";
        }
        if (missionManager.Phase == MissionPhase.Complete)
        {
            return "전송 완료";
        }
        if (missionManager.Phase == MissionPhase.Reserved)
        {
            return "예약 완료";
        }
        if (missionManager.Phase == MissionPhase.Running || missionManager.Phase == MissionPhase.Observing || missionManager.Phase == MissionPhase.ObservationComplete || missionManager.Phase == MissionPhase.Transmitting)
        {
            return ThreeStageCommunicationLabel(missionManager.CommunicationStep);
        }
        return SendTimeLabel();
    }

    private string ActiveCountdownText()
    {
        if (missionManager == null)
        {
            return string.Empty;
        }
        if (missionManager.AutoQueueCountdown > 0f)
        {
            return " · 다음 전송까지 " + Mathf.CeilToInt(missionManager.AutoQueueCountdown) + "초";
        }
        if (missionManager.RemainingWaitSeconds > 0.1f)
        {
            return " · 대기 " + FormatSeconds(Mathf.CeilToInt(missionManager.RemainingWaitSeconds));
        }
        if (missionManager.RemainingTransferSeconds > 0.1f
            && (missionManager.Phase == MissionPhase.ObservationComplete || missionManager.Phase == MissionPhase.Transmitting))
        {
            return " · 남은 " + FormatSeconds(Mathf.CeilToInt(missionManager.RemainingTransferSeconds));
        }
        if (missionManager.IsReplayingRoute)
        {
            return " · 통신 재생 " + Mathf.RoundToInt(missionManager.RouteReplayProgress * 100f) + "%";
        }
        return string.Empty;
    }

    private string CompactActiveCountdownText()
    {
        string countdown = ActiveCountdownText();
        const string separator = " · ";
        if (countdown.StartsWith(separator))
        {
            return countdown.Substring(separator.Length);
        }
        return countdown.Trim();
    }

    private string StatusHeadline()
    {
        switch (missionManager.Phase)
        {
            case MissionPhase.Reserved:
                return "예약 완료 · 전송 대기";
            case MissionPhase.Running:
            case MissionPhase.Observing:
            case MissionPhase.ObservationComplete:
            case MissionPhase.Transmitting:
                return "전송 상태";
            case MissionPhase.Complete:
                return "전송 완료 · 목적지 도착";
            case MissionPhase.Failed:
                return "전송 실패 · 다시 시도 가능";
            default:
                return HasStatusMessageContent() ? "전송 준비 중" : "전송중인 메시지가 없습니다";
        }
    }

    private string ThreeStageCommunicationLabel(SatelliteCommunicationStep step)
    {
        switch (step)
        {
            case SatelliteCommunicationStep.Reserved:
            case SatelliteCommunicationStep.WaitingForUplink:
            case SatelliteCommunicationStep.Uplink:
            case SatelliteCommunicationStep.LeoReceived:
                return "업링크";
            case SatelliteCommunicationStep.InterSatelliteLink:
            case SatelliteCommunicationStep.DestinationRelaySelection:
                return "ISL";
            case SatelliteCommunicationStep.Downlink:
            case SatelliteCommunicationStep.WaitingForDownlink:
            case SatelliteCommunicationStep.FinalReceive:
            case SatelliteCommunicationStep.Complete:
                return "다운링크";
            case SatelliteCommunicationStep.Failed:
                return "전송 실패";
            default:
                return "전송 대기";
        }
    }
}
