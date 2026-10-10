using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// 결과/기록 UI: 전송 결과 카드와 기록 목록을 관리합니다.
public partial class UIManager
{
    private int historySessionBaselineCount = -1;
    private readonly HashSet<string> historyInitialRecordKeys = new HashSet<string>();

    private void CreateResultPanel(Transform parent)
    {
        resultDismissed = true;
        resultPanel = CreatePanel(parent, "CommunicationResultPanel", new Vector2(0.32f, 0.18f), new Vector2(0.68f, 0.74f), new Color(0.015f, 0.055f, 0.085f, 1f));
        MakeDraggable(resultPanel);
        resultText = CreateText(resultPanel, string.Empty, 23, TextAnchor.UpperLeft);
        resultText.fontStyle = FontStyle.Bold;
        resultText.lineSpacing = 1.02f;
        resultText.rectTransform.anchorMin = new Vector2(0f, 0.78f);
        resultText.rectTransform.anchorMax = new Vector2(1f, 1f);
        resultText.rectTransform.offsetMin = new Vector2(24f, 8f);
        resultText.rectTransform.offsetMax = new Vector2(-64f, -20f);

        resultDataText = CreateText(resultPanel, string.Empty, 18, TextAnchor.UpperLeft);
        resultDataText.lineSpacing = 1.18f;
        resultDataText.resizeTextForBestFit = true;
        resultDataText.resizeTextMinSize = 15;
        resultDataText.resizeTextMaxSize = 18;
        resultDataText.rectTransform.anchorMin = new Vector2(0f, 0.16f);
        resultDataText.rectTransform.anchorMax = new Vector2(1f, 0.77f);
        resultDataText.rectTransform.offsetMin = new Vector2(24f, 8f);
        resultDataText.rectTransform.offsetMax = new Vector2(-24f, -4f);
        resultUplinkFill = null;
        resultFlowFill = null;
        resultReceiveFill = null;

        GameObject previewObject = new GameObject("SignalRoutePreview", typeof(RectTransform), typeof(RawImage));
        previewObject.transform.SetParent(resultPanel, false);
        RectTransform previewRect = previewObject.GetComponent<RectTransform>();
        previewRect.anchorMin = new Vector2(0.10f, 0.13f);
        previewRect.anchorMax = new Vector2(0.90f, 0.20f);
        previewRect.offsetMin = Vector2.zero;
        previewRect.offsetMax = Vector2.zero;
        previewImage = previewObject.GetComponent<RawImage>();
        previewImage.color = Color.white;
        previewObject.SetActive(false);
        CreateButton(resultPanel, "닫기", new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-78f, -44f), new Vector2(-14f, -10f), delegate { DismissResultPanel(); });
        resultDetailsToggleButton = null;
        CreateButton(resultPanel, "닫기", new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(20f, 12f), new Vector2(-20f, 48f), delegate { DismissResultPanel(); });
        resultPanel.gameObject.SetActive(false);
    }

    private void CreateHistoryPanel(Transform parent)
    {
        historyPanel = CreatePanel(parent, "SentMessageHistoryPanel", new Vector2(0.18f, 0.14f), new Vector2(0.82f, 0.84f), new Color(0.02f, 0.06f, 0.1f, 0.98f));
        MakeDraggable(historyPanel);

        Text heading = CreateText(historyPanel, "전송 기록", 22, TextAnchor.UpperLeft);
        heading.fontStyle = FontStyle.Bold;
        heading.rectTransform.anchorMin = new Vector2(0f, 1f);
        heading.rectTransform.anchorMax = new Vector2(1f, 1f);
        heading.rectTransform.pivot = new Vector2(0.5f, 1f);
        heading.rectTransform.anchoredPosition = new Vector2(0f, -18f);
        heading.rectTransform.sizeDelta = new Vector2(-64f, 32f);

        historySearchInputField = CreateInputField(historyPanel, "HistorySearch", "기록/송신지/수신지 검색", new Vector2(0f, 1f), new Vector2(0.62f, 1f), new Vector2(22f, -86f), new Vector2(-8f, -50f));
        historySearchInputField.lineType = InputField.LineType.SingleLine;
        historySearchInputField.onValueChanged.AddListener(delegate
        {
            historySearchQuery = historySearchInputField.text ?? string.Empty;
            RefreshHistoryPanel();
        });

        historyText = CreateText(historyPanel, string.Empty, 15, TextAnchor.UpperLeft);
        historyText.lineSpacing = 1.0f;
        historyText.rectTransform.anchorMin = new Vector2(0f, 0f);
        historyText.rectTransform.anchorMax = new Vector2(0.68f, 1f);
        historyText.rectTransform.offsetMin = new Vector2(22f, 24f);
        historyText.rectTransform.offsetMax = new Vector2(-8f, -118f);

        historyRecordButtons.Clear();
        historyRecordResendButtons.Clear();
        historyDeleteButtons.Clear();
        for (int i = 0; i < HistoryActionRowCount; i++)
        {
            int index = i;
            float top = 0.80f - i * 0.145f;
            float bottom = top - 0.065f;
            historyRecordButtons.Add(CreateButton(historyPanel, "보기", new Vector2(0.69f, bottom), new Vector2(0.783f, top), Vector2.zero, Vector2.zero, delegate { ShowVisibleSentRecord(index); }));
            historyRecordResendButtons.Add(CreateButton(historyPanel, "수정", new Vector2(0.791f, bottom), new Vector2(0.884f, top), Vector2.zero, Vector2.zero, delegate { ReuseVisibleSentMessage(index); }));
            historyDeleteButtons.Add(CreateButton(historyPanel, "삭제", new Vector2(0.892f, bottom), new Vector2(0.985f, top), Vector2.zero, Vector2.zero, delegate { DeleteVisibleSentMessage(index); }));
            SetButtonTextSize(historyRecordButtons[i], 12);
            SetButtonTextSize(historyRecordResendButtons[i], 12);
            SetButtonTextSize(historyDeleteButtons[i], 12);
        }

        CreateButton(historyPanel, "닫기", new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-80f, -48f), new Vector2(-14f, -12f), delegate { historyPanel.gameObject.SetActive(false); RefreshPanelLauncherButtons(); });
        historyPanel.gameObject.SetActive(false);
        CaptureHistorySessionBaseline();
    }

    private void ApplyResultPanelPriority(bool showResult)
    {
        if (!showResult)
        {
            return;
        }

        if (timelinePanel != null)
        {
            timelinePanel.gameObject.SetActive(false);
        }
        if (transferLogPanel != null)
        {
            transferLogPanel.gameObject.SetActive(false);
        }
        if (missionStatusPanel != null)
        {
            missionStatusPanel.gameObject.SetActive(false);
        }
        if (resultPanel != null)
        {
            resultPanel.SetAsLastSibling();
        }
    }

    private void RefreshHistoryPanel()
    {
        if (historyText == null || missionManager == null)
        {
            return;
        }

        CaptureHistorySessionBaseline();
        if (CurrentSessionSentMessageCount() == 0)
        {
            historyVisibleIndices.Clear();
            historyText.text = "이번 실행에서 보낸 기록이 없습니다.\n전송을 시작하면 진행 중인 메시지도 이곳에 표시됩니다.";
            RefreshHistoryActionButtons(0);
            return;
        }

        System.Text.StringBuilder builder = new System.Text.StringBuilder();
        historyVisibleIndices.Clear();
        int sessionCount = CurrentSessionSentMessageCount();
        for (int i = 0; i < sessionCount; i++)
        {
            if (HistoryRecordMatchesSearch(missionManager.SentMessages[i]))
            {
                historyVisibleIndices.Add(i);
            }
        }
        historyVisibleIndices.Sort(CompareHistoryVisibleIndices);

        if (historyVisibleIndices.Count == 0)
        {
            historyText.text = "검색 결과가 없습니다.\n다른 수신지, 위성, 메시지로 검색해 보세요.";
            RefreshHistoryActionButtons(0);
            return;
        }

        RefreshHistoryActionButtons(historyVisibleIndices.Count);

        int count = Mathf.Min(historyRecordButtons.Count, historyVisibleIndices.Count);
        for (int i = 0; i < count; i++)
        {
            int recordIndex = historyVisibleIndices[i];
            SentMessageRecord record = missionManager.SentMessages[recordIndex];
            string message = string.IsNullOrWhiteSpace(record.messageText) ? "(빈 메시지)" : record.messageText.Trim();
            if (message.Length > 42)
            {
                message = message.Substring(0, 42) + "…";
            }

            string origin = string.IsNullOrEmpty(record.originLabel) ? "송신지" : record.originLabel;
            string destination = string.IsNullOrEmpty(record.destinationLabel)
                ? string.IsNullOrEmpty(record.targetName) ? "수신지" : DestinationLabel(record.targetName)
                : record.destinationLabel;

            builder.Append(i + 1).Append(". ")
                .Append(HistoryStatusRichText(record)).Append(" · ")
                .Append(RouteDisplayText(origin, destination))
                .Append("\n")
                .Append("   내용 : ").Append(message)
                .Append("\n")
                .Append("   요금 : ").Append(FormatUsd(record.priceUsd))
                .Append("\n")
                .Append("   크기 : ").Append(SentRecordPayloadBytesText(record));
            if (!string.IsNullOrEmpty(record.attachmentName))
            {
                builder.Append("\n")
                    .Append("   첨부 : ").Append(record.attachmentName);
            }
            if (i < count - 1)
            {
                builder.Append("\n\n");
            }
        }
        int hiddenCount = historyVisibleIndices.Count - count;
        if (hiddenCount > 0)
        {
            builder.Append("\n\n외 ").Append(hiddenCount).Append("건 더 있음");
        }

        historyText.text = builder.ToString();
    }

    private bool HistoryRecordComplete(SentMessageRecord record)
    {
        if (record == null)
        {
            return false;
        }
        return record.isComplete || (!string.IsNullOrEmpty(record.deliveredAt) && record.statusLabel != "전송 중");
    }

    private int CompareHistoryVisibleIndices(int left, int right)
    {
        SentMessageRecord leftRecord = left >= 0 && left < missionManager.SentMessages.Count ? missionManager.SentMessages[left] : null;
        SentMessageRecord rightRecord = right >= 0 && right < missionManager.SentMessages.Count ? missionManager.SentMessages[right] : null;
        bool leftComplete = HistoryRecordComplete(leftRecord);
        bool rightComplete = HistoryRecordComplete(rightRecord);
        if (leftComplete != rightComplete)
        {
            return leftComplete ? 1 : -1;
        }
        return left.CompareTo(right);
    }

    private string HistoryStatusRichText(SentMessageRecord record)
    {
        return HistoryRecordComplete(record)
            ? "<color=#6FFFD6>전달 완료</color>"
            : "<color=#FFD36A>전달 중</color>";
    }

    private bool HistoryRecordMatchesSearch(SentMessageRecord record)
    {
        if (record == null || string.IsNullOrWhiteSpace(historySearchQuery))
        {
            return true;
        }

        string query = historySearchQuery.Trim().ToLowerInvariant();
        return (record.destinationLabel ?? string.Empty).ToLowerInvariant().Contains(query)
            || (record.originLabel ?? string.Empty).ToLowerInvariant().Contains(query)
            || (record.trackingId ?? string.Empty).ToLowerInvariant().Contains(query)
            || (record.satelliteName ?? string.Empty).ToLowerInvariant().Contains(query)
            || (record.messageText ?? string.Empty).ToLowerInvariant().Contains(query)
            || (record.attachmentName ?? string.Empty).ToLowerInvariant().Contains(query);
    }

    private int VisibleHistoryIndex(int buttonIndex)
    {
        return buttonIndex >= 0 && buttonIndex < historyVisibleIndices.Count ? historyVisibleIndices[buttonIndex] : -1;
    }

    private void ShowVisibleSentRecord(int buttonIndex)
    {
        ShowSentRecord(VisibleHistoryIndex(buttonIndex));
    }

    private void ReuseVisibleSentMessage(int buttonIndex)
    {
        ReuseSentMessage(VisibleHistoryIndex(buttonIndex));
    }

    private void DeleteVisibleSentMessage(int buttonIndex)
    {
        DeleteSentMessage(VisibleHistoryIndex(buttonIndex));
    }

    private int CurrentRecordIndex()
    {
        if (missionManager == null || CurrentSessionSentMessageCount() == 0)
        {
            return -1;
        }
        int sessionCount = CurrentSessionSentMessageCount();
        return visibleRecordIndex >= 0 && visibleRecordIndex < sessionCount ? visibleRecordIndex : 0;
    }

    private void CopyVisibleRecord()
    {
        int index = CurrentRecordIndex();
        if (index < 0)
        {
            ShowToast("복사할 전송 결과가 없습니다.");
            return;
        }
        SentMessageRecord record = missionManager.SentMessages[index];
        GUIUtility.systemCopyBuffer = TransmissionResultText(record, string.IsNullOrEmpty(record.targetName) ? OrbitShareDestinations.DefaultTargetName : record.targetName);
        ShowToast("전송 결과를 복사했습니다.");
    }

    private void ReplayVisibleRecordRoute()
    {
        if (missionManager == null)
        {
            return;
        }
        missionManager.StartRouteReplay();
        ShowTimelinePanel();
        ShowToast("통신 과정을 다시 재생합니다.");
    }

    private void ToggleRouteReplayPause()
    {
        if (missionManager == null)
        {
            return;
        }

        if (!missionManager.IsReplayingRoute)
        {
            missionManager.StartRouteReplay();
        }
        else if (missionManager.IsRouteReplayPaused)
        {
            missionManager.ResumeRouteReplay();
        }
        else
        {
            missionManager.PauseRouteReplay();
        }
        RefreshStatusOptionButtons();
        RefreshTimeline();
    }

    private void CycleRouteReplaySpeed()
    {
        if (missionManager == null)
        {
            return;
        }

        float nextSpeed = missionManager.RouteReplaySpeed < 2f ? 30f : missionManager.RouteReplaySpeed < 120f ? 120f : missionManager.RouteReplaySpeed < 300f ? 600f : 1f;
        missionManager.SetRouteReplaySpeed(nextSpeed);
        if (!missionManager.IsReplayingRoute && !IsActiveTransferPhase(missionManager.Phase))
        {
            missionManager.StartRouteReplay();
        }
        RefreshStatusOptionButtons();
        RefreshTimeline();
    }

    private void ConfirmStopCurrentMission()
    {
        if (missionManager == null || missionManager.Phase == MissionPhase.Idle)
        {
            return;
        }

        if (demoModeActive)
        {
            ShowConfirm("현재 시연을 중지하고 메시지 작성으로 돌아갈까요?", delegate { StopDemoModeNow(); });
            return;
        }

        ShowConfirm("현재 전송을 중지하고 메시지 작성으로 돌아갈까요?", delegate { ResetFlowAndMission(); });
    }

    private void ConfirmDeleteVisibleRecord()
    {
        int index = CurrentRecordIndex();
        if (index < 0)
        {
            ShowToast("삭제할 전송 결과가 없습니다.");
            return;
        }
        ShowConfirm("이 기록을 삭제할까요?", delegate { DeleteSentMessageNow(index); });
    }

    private void ReuseVisibleRecordMessage()
    {
        int index = CurrentRecordIndex();
        if (index < 0)
        {
            ShowToast("다시 보낼 기록이 없습니다.");
            return;
        }
        ReuseSentMessage(index);
    }

    private void DismissResultPanel()
    {
        visibleRecordIndex = -1;
        resultDismissed = true;
        if (resultPanel != null)
        {
            resultPanel.gameObject.SetActive(false);
        }
        RefreshTopButtons();
    }

    private void ToggleResultDetails()
    {
        resultDetailsExpanded = !resultDetailsExpanded;
        RefreshMissionPanel();
    }

    private void RefreshResultSummaryBars(SentMessageRecord record)
    {
        if (resultUplinkFill != null)
        {
            resultUplinkFill.fillAmount = 1f;
        }
        if (resultFlowFill != null)
        {
            resultFlowFill.fillAmount = 1f;
        }
        if (resultReceiveFill != null)
        {
            float successRatio = record != null ? Mathf.Clamp01(record.successProbabilityPercent / 100f) : 1f;
            resultReceiveFill.fillAmount = successRatio;
            resultReceiveFill.color = successRatio >= 0.75f ? new Color(0.24f, 1f, 0.84f, 0.9f) : new Color(1f, 0.76f, 0.18f, 0.9f);
        }
    }

    private void ShowResultPanel()
    {
        if (missionManager == null || missionManager.Phase != MissionPhase.Complete || CurrentSessionSentMessageCount() == 0)
        {
            return;
        }

        visibleRecordIndex = -1;
        resultDismissed = false;
        if (resultPanel != null)
        {
            resultPanel.gameObject.SetActive(true);
            resultPanel.SetAsLastSibling();
        }
        ApplyResultPanelPriority(true);
        RefreshMissionPanel();
    }

    private void ToggleHistoryPanel()
    {
        if (historyPanel == null)
        {
            return;
        }

        historyPanel.gameObject.SetActive(!historyPanel.gameObject.activeSelf);
        if (historyPanel.gameObject.activeSelf)
        {
            historyPanel.SetAsLastSibling();
        }
        RefreshHistoryPanel();
        RefreshPanelLauncherButtons();
    }

    private void ReuseLatestSentMessage()
    {
        if (missionManager == null)
        {
            return;
        }

        string error;
        bool reused = missionManager.ReuseSentMessage(0, out error);
        if (reused && missionManager.Phase == MissionPhase.Idle)
        {
            currentFlowStep = 5;
            resultDismissed = true;
            if (historyPanel != null)
            {
                historyPanel.gameObject.SetActive(false);
            }
        }
        else if (!reused && historyText != null && !string.IsNullOrEmpty(error))
        {
            historyText.text = error;
        }
        RefreshAll();
    }

    private void ShowSentRecord(int index)
    {
        if (missionManager == null || index < 0 || index >= missionManager.SentMessages.Count)
        {
            return;
        }

        visibleRecordIndex = index;
        resultDismissed = false;
        resultDetailsExpanded = true;
        if (resultPanel != null)
        {
            resultPanel.SetAsLastSibling();
        }
        RefreshAll();
    }

    private void ReuseSentMessage(int index)
    {
        if (missionManager == null || index < 0 || index >= missionManager.SentMessages.Count)
        {
            return;
        }

        if (IsActiveTransferPhase(missionManager.Phase))
        {
            BeginQueuedMessageFlowFromRecord(missionManager.SentMessages[index]);
            return;
        }

        string error;
        bool reused = missionManager.ReuseSentMessage(index, out error);
        if (reused && missionManager.Phase == MissionPhase.Idle)
        {
            currentFlowStep = 5;
            visibleRecordIndex = -1;
            resultDismissed = true;
            if (historyPanel != null)
            {
                historyPanel.gameObject.SetActive(false);
            }
        }
        else if (!reused && historyText != null && !string.IsNullOrEmpty(error))
        {
            historyText.text = error;
        }
        RefreshAll();
    }

    private void DeleteSentMessage(int index)
    {
        if (missionManager == null || index < 0 || index >= missionManager.SentMessages.Count)
        {
            return;
        }

        SentMessageRecord record = missionManager.SentMessages[index];
        string message = HistoryRecordComplete(record)
            ? "이 전송 기록을 삭제할까요?"
            : "전송 중인 메시지만 취소할까요?\n대기 목록과 추가 작성 내용은 유지됩니다.";
        ShowConfirm(message, delegate { DeleteSentMessageNow(index); });
    }

    private void DeleteSentMessageNow(int index)
    {
        if (missionManager == null || index < 0 || index >= missionManager.SentMessages.Count)
        {
            return;
        }

        SentMessageRecord record = missionManager.SentMessages[index];
        bool cancelActiveTransfer = !HistoryRecordComplete(record);
        missionManager.RemoveSentMessage(index);
        if (visibleRecordIndex == index)
        {
            visibleRecordIndex = -1;
            resultDismissed = true;
        }
        else if (visibleRecordIndex > index)
        {
            visibleRecordIndex--;
        }
        ShowToast(cancelActiveTransfer ? "전송을 취소했습니다." : "기록을 삭제했습니다.");
        RefreshAll();
    }

    private void RefreshHistoryActionButtons(int recordCount)
    {
        for (int i = 0; i < historyRecordButtons.Count; i++)
        {
            bool enabled = i < recordCount;
            historyRecordButtons[i].gameObject.SetActive(enabled);
            if (i < historyRecordResendButtons.Count)
            {
                historyRecordResendButtons[i].gameObject.SetActive(enabled && !HistoryRecordComplete(i < historyVisibleIndices.Count ? missionManager.SentMessages[VisibleHistoryIndex(i)] : null));
            }
            historyDeleteButtons[i].gameObject.SetActive(enabled);
            if (enabled)
            {
                int recordIndex = VisibleHistoryIndex(i);
                SentMessageRecord record = recordIndex >= 0 && recordIndex < missionManager.SentMessages.Count ? missionManager.SentMessages[recordIndex] : null;
                bool complete = HistoryRecordComplete(record);
                ApplyHistoryActionButtonLayout(i, complete);
                SetButtonLabel(historyRecordButtons[i], complete ? "보기" : "진행");
                SetButtonVisual(historyRecordButtons[i], complete ? ButtonVisual.Secondary : ButtonVisual.Selected);
                if (i < historyRecordResendButtons.Count)
                {
                    bool canEdit = !complete;
                    historyRecordResendButtons[i].gameObject.SetActive(canEdit);
                    historyRecordResendButtons[i].interactable = canEdit;
                    SetButtonLabel(historyRecordResendButtons[i], "수정");
                    SetButtonVisual(historyRecordResendButtons[i], canEdit ? ButtonVisual.Cta : ButtonVisual.Disabled);
                }
                SetButtonLabel(historyDeleteButtons[i], complete ? "삭제" : "전송 취소");
                SetButtonVisual(historyDeleteButtons[i], complete ? ButtonVisual.Secondary : ButtonVisual.Cta);
            }
        }

        if (historyResendButton != null)
        {
            historyResendButton.gameObject.SetActive(false);
            historyResendButton.interactable = false;
        }
    }

    private void ApplyHistoryActionButtonLayout(int rowIndex, bool complete)
    {
        if (rowIndex < 0)
        {
            return;
        }

        float top = 0.80f - rowIndex * 0.145f;
        float bottom = top - 0.065f;
        if (rowIndex < historyRecordButtons.Count)
        {
            SetButtonRect(
                historyRecordButtons[rowIndex],
                new Vector2(0.69f, bottom),
                complete ? new Vector2(0.833f, top) : new Vector2(0.783f, top),
                Vector2.zero,
                Vector2.zero);
        }
        if (rowIndex < historyRecordResendButtons.Count)
        {
            SetButtonRect(
                historyRecordResendButtons[rowIndex],
                new Vector2(0.791f, bottom),
                new Vector2(0.884f, top),
                Vector2.zero,
                Vector2.zero);
        }
        if (rowIndex < historyDeleteButtons.Count)
        {
            SetButtonRect(
                historyDeleteButtons[rowIndex],
                complete ? new Vector2(0.842f, bottom) : new Vector2(0.892f, bottom),
                new Vector2(0.985f, top),
                Vector2.zero,
                Vector2.zero);
        }
    }

    private void RestoreDraftFromButton()
    {
        if (missionManager == null)
        {
            return;
        }
        if (missionManager.RestoreDraft())
        {
            currentFlowStep = 5;
        }
        RefreshAll();
    }

    private void RefreshDraftButtons()
    {
        if (missionManager == null)
        {
            return;
        }
        bool canEdit = missionManager.Phase == MissionPhase.Idle;
        if (draftSaveButton != null)
        {
            draftSaveButton.interactable = canEdit;
            SetButtonVisual(draftSaveButton, canEdit ? ButtonVisual.Secondary : ButtonVisual.Disabled);
        }
        if (draftRestoreButton != null)
        {
            draftRestoreButton.interactable = missionManager.HasDraft && canEdit;
            SetButtonLabel(draftRestoreButton, missionManager.HasDraft ? "불러오기" : "저장 없음");
            SetButtonVisual(draftRestoreButton, missionManager.HasDraft && canEdit ? ButtonVisual.Primary : ButtonVisual.Disabled);
        }
    }

    private string TransmissionResultText(SentMessageRecord record, string fallbackTargetName)
    {
        if (record == null)
        {
            int payloadBytes = missionManager != null ? missionManager.MessagePayloadBytes : 0;
            string price = missionManager != null ? FormatUsd(missionManager.EstimatedPriceUsd) : "요금 계산 중";
            return "상태 : 준비 중\n"
                + "용량 : " + FormatBytes(payloadBytes) + "\n"
                + "요금 : " + price;
        }

        string message = TrimReceiptText(string.IsNullOrWhiteSpace(record.messageText) ? "(빈 메시지)" : record.messageText.Trim(), 68);
        string attachments = ResultAttachmentText(record);
        attachments = string.IsNullOrEmpty(attachments) ? "첨부 파일 없음" : TrimReceiptText(attachments, 68);
        string originLabel = string.IsNullOrEmpty(record.originLabel) ? "송신지" : record.originLabel;
        string destinationLabel = string.IsNullOrEmpty(record.destinationLabel) ? DestinationLabel(fallbackTargetName) : record.destinationLabel;
        string uplinkSatellite = CompactSatelliteName(record.satelliteName, 18);
        string relaySatellite = CompactSatelliteName(string.IsNullOrEmpty(record.destinationRelaySatelliteName) ? record.satelliteName : record.destinationRelaySatelliteName, 18);
        string satelliteRoute = uplinkSatellite == relaySatellite ? uplinkSatellite : uplinkSatellite + " → " + relaySatellite;
        bool complete = HistoryRecordComplete(record);
        string timeLabel = complete ? "완료시간" : "진행시간";
        string timeValue = complete
            ? string.IsNullOrEmpty(record.deliveredAt) ? "확인 중" : record.deliveredAt
            : FormatSeconds(record.elapsedSeconds);

        System.Text.StringBuilder builder = new System.Text.StringBuilder();
        builder.Append("상태 : ").Append(complete ? "전달 완료" : string.IsNullOrEmpty(record.statusLabel) ? "전달 중" : record.statusLabel).Append("\n")
            .Append("내용 : ").Append(message).Append("\n")
            .Append("경로 : ").Append(RouteDisplayText(originLabel, destinationLabel)).Append("\n")
            .Append("위성망 : ").Append(satelliteRoute).Append("\n")
            .Append("용량 : ").Append(SentRecordPayloadBytesText(record)).Append("\n")
            .Append("요금 : ").Append(FormatUsd(record.priceUsd)).Append("\n")
            .Append("소요 시간 : ").Append(FormatSeconds(record.elapsedSeconds)).Append("\n")
            .Append("첨부 : ").Append(attachments).Append("\n")
            .Append(timeLabel).Append(" : ").Append(timeValue).Append("\n")
            .Append("요금상세 : 기본 ").Append(FormatUsd(record.baseFeeUsd))
            .Append(" / 내용(32,768 Bytes 단위) ").Append(FormatUsd(record.payloadFeeUsd))
            .Append(" / 시간 ").Append(FormatUsd(record.urgencyFeeUsd));
        return builder.ToString();
    }

    private string SentRecordPayloadBytesText(SentMessageRecord record)
    {
        return FormatBytes(SentRecordPayloadBytes(record));
    }

    private int SentRecordPayloadBytes(SentMessageRecord record)
    {
        if (record == null)
        {
            return missionManager != null ? missionManager.MessagePayloadBytes : 0;
        }
        if (record.payloadBytes > 0)
        {
            return record.payloadBytes;
        }
        if (missionManager != null)
        {
            List<MessageAttachmentRecord> attachments = AttachmentsOrLegacy(record.attachments, record.attachmentName, record.attachmentSizeKb);
            return missionManager.CalculateMessagePayloadBytes(record.messageText, attachments);
        }
        return Mathf.Max(0, record.payloadKb) * 1024;
    }

    private string ResultReceiptHeadline(SentMessageRecord record)
    {
        if (record == null)
        {
            return "영수증\n결과 준비 중";
        }
        return HistoryRecordComplete(record)
            ? "영수증\n#" + ResultReceiptId(record)
            : "영수증\n전달 중 · #" + ResultReceiptId(record);
    }

    private string ResultReceiptId(SentMessageRecord record)
    {
        if (record == null)
        {
            return "준비 중";
        }
        if (!string.IsNullOrEmpty(record.trackingId))
        {
            return record.trackingId;
        }
        if (!string.IsNullOrEmpty(record.deliveredAt))
        {
            return record.deliveredAt.Replace("/", string.Empty).Replace(":", string.Empty).Replace(" ", string.Empty);
        }
        return "UNKNOWN";
    }

    private string TrimReceiptText(string text, int maxLength)
    {
        if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
        {
            return text;
        }
        return text.Substring(0, Mathf.Max(1, maxLength - 1)) + "…";
    }

    private string ResultLogSummary(string transmissionLog)
    {
        if (string.IsNullOrWhiteSpace(transmissionLog))
        {
            return "  기록 없음";
        }

        string[] lines = transmissionLog.Split('\n');
        int start = Mathf.Max(0, lines.Length - 4);
        System.Text.StringBuilder builder = new System.Text.StringBuilder();
        for (int i = start; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
            {
                continue;
            }
            if (builder.Length > 0)
            {
                builder.Append("\n");
            }
            builder.Append("  - ").Append(SimplifyMissionEventLog(lines[i].Trim()));
        }
        return builder.Length == 0 ? "  기록 없음" : builder.ToString();
    }

    private void CaptureHistorySessionBaseline()
    {
        if (historySessionBaselineCount >= 0 || missionManager == null)
        {
            return;
        }
        historySessionBaselineCount = missionManager.SentMessages.Count;
        historyInitialRecordKeys.Clear();
        for (int i = 0; i < missionManager.SentMessages.Count; i++)
        {
            historyInitialRecordKeys.Add(HistoryRecordKey(missionManager.SentMessages[i]));
        }
    }

    private int CurrentSessionSentMessageCount()
    {
        if (missionManager == null)
        {
            return 0;
        }
        CaptureHistorySessionBaseline();
        int count = 0;
        for (int i = 0; i < missionManager.SentMessages.Count; i++)
        {
            if (historyInitialRecordKeys.Contains(HistoryRecordKey(missionManager.SentMessages[i]))
                && HistoryRecordComplete(missionManager.SentMessages[i]))
            {
                break;
            }
            count++;
        }
        return Mathf.Clamp(count, 0, missionManager.SentMessages.Count);
    }

    private string HistoryRecordKey(SentMessageRecord record)
    {
        if (record == null)
        {
            return string.Empty;
        }
        if (!string.IsNullOrEmpty(record.trackingId))
        {
            return record.trackingId;
        }
        return (record.deliveredAt ?? string.Empty) + "|" + (record.originLabel ?? string.Empty) + "|" + (record.targetName ?? string.Empty) + "|" + (record.messageText ?? string.Empty);
    }

    private string SenderLabel()
    {
        return SenderLabel(missionManager != null && missionManager.HasExplicitOriginSelection ? missionManager.OriginLabel : "송신지");
    }

    private string SenderLabel(string originLabel)
    {
        return "사용자(" + (string.IsNullOrEmpty(originLabel) ? "송신지" : originLabel) + ")";
    }

    private string RouteDisplayText(string originLabel, string destinationLabel)
    {
        string safeOrigin = string.IsNullOrEmpty(originLabel) ? "송신지" : originLabel;
        string safeDestination = string.IsNullOrEmpty(destinationLabel) ? "수신지" : destinationLabel;
        return safeOrigin + " → " + safeDestination;
    }

    private string CompactSatelliteName(string satelliteName)
    {
        return CompactSatelliteName(satelliteName, 24);
    }

    private string CompactSatelliteName(string satelliteName, int maxLength)
    {
        if (string.IsNullOrEmpty(satelliteName))
        {
            return "선택 위성";
        }

        string compactName = satelliteName.Trim();
        if (compactName.StartsWith("REAL-"))
        {
            compactName = compactName.Substring(5);
        }
        compactName = compactName.Replace(" (ZARYA)", string.Empty).Trim();
        if (compactName.Length > maxLength)
        {
            compactName = compactName.Substring(0, Mathf.Max(1, maxLength - 1)) + "…";
        }
        return compactName;
    }

    private string ResultAttachmentText(SentMessageRecord record)
    {
        List<MessageAttachmentRecord> attachments = AttachmentsOrLegacy(record.attachments, record.attachmentName, record.attachmentSizeKb);
        if (attachments.Count == 0)
        {
            return string.Empty;
        }
        return AttachmentPreview(attachments);
    }
}
