using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 추가 메시지 목록 UI: 전송 중 이어 보낼 메시지의 상태와 순서를 관리합니다.
public partial class UIManager
{
    private Text queueHeadingText;
    private RectTransform queueListContent;
    private ScrollRect queueListScrollRect;
    private int queueSelectedRowIndex = -1;
    private readonly List<RectTransform> queueRowRects = new List<RectTransform>();
    private readonly List<Text> queueRowTexts = new List<Text>();
    private readonly List<Button> queueRowCardButtons = new List<Button>();
    private readonly List<Image> queueCardBackgrounds = new List<Image>();

    private void CreateQueueMessagePanel(Transform parent)
    {
        queueMessagePanel = CreatePanel(parent, "QueuedMessagePanel", new Vector2(0.34f, 0.12f), new Vector2(0.86f, 0.62f), new Color(0.02f, 0.06f, 0.1f, 0.98f));
        MakeDraggable(queueMessagePanel);

        queueHeadingText = CreateText(queueMessagePanel, "대기 메시지", 22, TextAnchor.UpperLeft);
        queueHeadingText.fontStyle = FontStyle.Bold;
        queueHeadingText.rectTransform.anchorMin = new Vector2(0f, 1f);
        queueHeadingText.rectTransform.anchorMax = new Vector2(1f, 1f);
        queueHeadingText.rectTransform.pivot = new Vector2(0.5f, 1f);
        queueHeadingText.rectTransform.offsetMin = new Vector2(22f, -48f);
        queueHeadingText.rectTransform.offsetMax = new Vector2(-96f, -14f);

        queueStatusText = CreateText(queueMessagePanel, string.Empty, 16, TextAnchor.UpperLeft);
        queueStatusText.color = new Color(0.82f, 0.94f, 1f, 0.86f);
        queueStatusText.rectTransform.anchorMin = new Vector2(0f, 1f);
        queueStatusText.rectTransform.anchorMax = new Vector2(1f, 1f);
        queueStatusText.rectTransform.offsetMin = new Vector2(22f, -76f);
        queueStatusText.rectTransform.offsetMax = new Vector2(-22f, -48f);

        CreateQueueListScrollArea(queueMessagePanel);
        CreateButton(queueMessagePanel, "닫기", new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-78f, -46f), new Vector2(-14f, -12f), delegate { queueMessagePanel.gameObject.SetActive(false); RefreshPanelLauncherButtons(); });
        queueMessagePanel.gameObject.SetActive(false);

        queueAttachmentPickerPanel = null;
        queueScheduleHintText = null;
        queueOriginPickButton = null;
        queueDestinationPickButton = null;
        queueMessageInputField = null;
        queuePayloadText = null;
        queuePayloadFill = null;
        queueAttachmentPickerButton = null;
        queueAttachmentRemoveFirstButton = null;
        queueAttachmentPreviewText = null;
        queueAttachmentPathInputField = null;
        queueSaveButton = null;
        queueDestinationButtons.Clear();
        queueAttachmentButtons.Clear();
        queueAttachmentDeleteButtons.Clear();
    }

    private void CreateQueueListScrollArea(RectTransform parent)
    {
        GameObject scrollObject = new GameObject("QueueListScrollArea", typeof(RectTransform), typeof(Image), typeof(ScrollRect));
        scrollObject.transform.SetParent(parent, false);
        RectTransform scrollRectTransform = scrollObject.GetComponent<RectTransform>();
        scrollRectTransform.anchorMin = new Vector2(0f, 0f);
        scrollRectTransform.anchorMax = new Vector2(1f, 1f);
        scrollRectTransform.offsetMin = new Vector2(18f, 18f);
        scrollRectTransform.offsetMax = new Vector2(-18f, -88f);

        Image scrollBackground = scrollObject.GetComponent<Image>();
        scrollBackground.color = new Color(0.025f, 0.055f, 0.085f, 0.64f);
        scrollBackground.raycastTarget = true;

        GameObject viewportObject = new GameObject("QueueListViewport", typeof(RectTransform), typeof(RectMask2D));
        viewportObject.transform.SetParent(scrollObject.transform, false);
        RectTransform viewportRect = viewportObject.GetComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.offsetMin = new Vector2(8f, 8f);
        viewportRect.offsetMax = new Vector2(-18f, -8f);

        GameObject contentObject = new GameObject("QueueListContent", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
        contentObject.transform.SetParent(viewportObject.transform, false);
        queueListContent = contentObject.GetComponent<RectTransform>();
        queueListContent.anchorMin = new Vector2(0f, 1f);
        queueListContent.anchorMax = new Vector2(1f, 1f);
        queueListContent.pivot = new Vector2(0.5f, 1f);
        queueListContent.offsetMin = Vector2.zero;
        queueListContent.offsetMax = Vector2.zero;

        VerticalLayoutGroup layout = contentObject.GetComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(4, 4, 4, 4);
        layout.spacing = 10f;
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        ContentSizeFitter fitter = contentObject.GetComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        queueListScrollRect = scrollObject.GetComponent<ScrollRect>();
        queueListScrollRect.viewport = viewportRect;
        queueListScrollRect.content = queueListContent;
        queueListScrollRect.horizontal = false;
        queueListScrollRect.vertical = true;
        queueListScrollRect.movementType = ScrollRect.MovementType.Clamped;
        queueListScrollRect.inertia = true;
        queueListScrollRect.decelerationRate = 0.12f;
        queueListScrollRect.scrollSensitivity = 32f;
        queueListScrollRect.verticalScrollbar = CreateQueueListScrollbar(scrollObject.transform);
        queueListScrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
        queueListScrollRect.verticalScrollbarSpacing = -8f;
    }

    private Scrollbar CreateQueueListScrollbar(Transform parent)
    {
        GameObject scrollbarObject = new GameObject("QueueListScrollbar", typeof(RectTransform), typeof(Image), typeof(Scrollbar));
        scrollbarObject.transform.SetParent(parent, false);
        RectTransform scrollbarRect = scrollbarObject.GetComponent<RectTransform>();
        scrollbarRect.anchorMin = new Vector2(1f, 0f);
        scrollbarRect.anchorMax = new Vector2(1f, 1f);
        scrollbarRect.offsetMin = new Vector2(-12f, 8f);
        scrollbarRect.offsetMax = new Vector2(-4f, -8f);

        Image trackImage = scrollbarObject.GetComponent<Image>();
        trackImage.color = new Color(0.12f, 0.22f, 0.32f, 0.42f);

        GameObject handleObject = new GameObject("Handle", typeof(RectTransform), typeof(Image));
        handleObject.transform.SetParent(scrollbarObject.transform, false);
        RectTransform handleRect = handleObject.GetComponent<RectTransform>();
        handleRect.anchorMin = Vector2.zero;
        handleRect.anchorMax = Vector2.one;
        handleRect.offsetMin = Vector2.zero;
        handleRect.offsetMax = Vector2.zero;
        Image handleImage = handleObject.GetComponent<Image>();
        handleImage.color = new Color(0.34f, 0.84f, 1f, 0.72f);

        Scrollbar scrollbar = scrollbarObject.GetComponent<Scrollbar>();
        scrollbar.direction = Scrollbar.Direction.BottomToTop;
        scrollbar.targetGraphic = handleImage;
        scrollbar.handleRect = handleRect;
        return scrollbar;
    }

    private void RefreshQueuePanel()
    {
        string message = queueMessageInputField != null ? queueMessageInputField.text : string.Empty;
        int textBytes = string.IsNullOrEmpty(message) ? 0 : System.Text.Encoding.UTF8.GetByteCount(message);
        int queueAttachmentKb = QueueAttachmentSizeKb();
        int totalBytes = textBytes + queueAttachmentKb * 1024;
        int payloadKb = totalBytes <= 0 ? 0 : Mathf.CeilToInt(totalBytes / 1024f);
        bool tooLarge = missionManager != null && payloadKb > missionManager.MaxPayloadKb;

        if (queueScheduleHintText != null)
        {
            queueScheduleHintText.text = QueueScheduleHint();
        }

        if (queuePayloadText != null)
        {
            string maxPayload = missionManager != null ? PayloadKbText(missionManager.MaxPayloadKb) : PayloadKbText(512);
            queuePayloadText.text = "내용 : " + FormatBytes(textBytes)
                + "\n파일 : " + PayloadKbText(queueAttachmentKb)
                + "\n합계 : " + PayloadKbText(payloadKb) + " / " + maxPayload
                + "\n" + QueueDraftEstimateLine(message, payloadKb);
            queuePayloadText.color = tooLarge ? new Color(1f, 0.48f, 0.42f, 0.96f) : new Color(0.68f, 1f, 0.86f, 0.82f);
        }
        if (queuePayloadFill != null)
        {
            float maxKb = missionManager != null ? Mathf.Max(1, missionManager.MaxPayloadKb) : 512f;
            queuePayloadFill.fillAmount = Mathf.Clamp01(payloadKb / maxKb);
            queuePayloadFill.color = tooLarge ? new Color(1f, 0.38f, 0.32f, 0.95f) : new Color(0.24f, 1f, 0.84f, 0.9f);
        }

        foreach (KeyValuePair<string, Button> pair in queueDestinationButtons)
        {
            bool selected = selectingOriginInQueuePanel ? queueOriginName == pair.Key : queueTargetName == pair.Key;
            SetButtonLabel(pair.Value, DestinationLabel(pair.Key));
            SetButtonVisual(pair.Value, selected ? ButtonVisual.Selected : ButtonVisual.Secondary);
        }

        if (queueOriginPickButton != null)
        {
            SetButtonLabel(queueOriginPickButton, "송신지: " + DestinationLabel(queueOriginName));
            SetButtonVisual(queueOriginPickButton, selectingOriginInQueuePanel ? ButtonVisual.Selected : ButtonVisual.Secondary);
        }
        if (queueDestinationPickButton != null)
        {
            SetButtonLabel(queueDestinationPickButton, "수신지: " + DestinationLabel(queueTargetName));
            SetButtonVisual(queueDestinationPickButton, selectingOriginInQueuePanel ? ButtonVisual.Secondary : ButtonVisual.Selected);
        }

        foreach (KeyValuePair<string, Button> pair in queueAttachmentButtons)
        {
            bool selected = HasQueueAttachment(pair.Key);
            SetButtonVisual(pair.Value, selected ? ButtonVisual.Selected : ButtonVisual.Secondary);
        }

        if (queueAttachmentPickerButton != null)
        {
            SetButtonLabel(queueAttachmentPickerButton, queueAttachments.Count == 0 ? "파일 첨부" : "파일 추가");
            SetButtonVisual(queueAttachmentPickerButton, queueAttachments.Count == 0 ? ButtonVisual.Secondary : ButtonVisual.Selected);
        }
        if (queueAttachmentRemoveFirstButton != null)
        {
            queueAttachmentRemoveFirstButton.interactable = queueAttachments.Count > 0;
            SetButtonLabel(queueAttachmentRemoveFirstButton, queueAttachments.Count > 1 ? "첫 파일 제거" : "파일 제거");
            SetButtonVisual(queueAttachmentRemoveFirstButton, queueAttachments.Count > 0 ? ButtonVisual.Secondary : ButtonVisual.Disabled);
        }

        if (queueAttachmentPreviewText != null)
        {
            queueAttachmentPreviewText.text = QueueAttachmentPreview();
            queueAttachmentPreviewText.gameObject.SetActive(!string.IsNullOrEmpty(queueAttachmentPreviewText.text));
        }
        RefreshQueueAttachmentDeleteButtons();

        RefreshQueueListCards();

        if (queueSaveButton != null)
        {
            SetButtonLabel(queueSaveButton, queueEditingIndex >= 0 ? "수정 저장" : "대기 목록에 추가");
            SetButtonVisual(queueSaveButton, tooLarge ? ButtonVisual.Disabled : queueEditingIndex >= 0 ? ButtonVisual.Cta : ButtonVisual.Primary);
            queueSaveButton.interactable = !tooLarge;
        }

        if (queueStatusText != null)
        {
            int queuedCount = missionManager != null ? missionManager.QueuedMessageCount : 0;
            queueStatusText.text = queueEditingIndex >= 0
                ? "수정 중 : " + (queueEditingIndex + 1) + "번"
                : queuedCount <= 0 ? "대기 메시지가 없습니다." : string.Empty;
            queueStatusText.color = new Color(0.82f, 0.94f, 1f, 0.86f);
        }
    }

    private int QueueAttachmentSizeKb()
    {
        int total = 0;
        for (int i = 0; i < queueAttachments.Count; i++)
        {
            if (queueAttachments[i] != null)
            {
                total += Mathf.Max(0, queueAttachments[i].sizeKb);
            }
        }
        return total;
    }

    private bool HasQueueAttachment(string fileName)
    {
        for (int i = 0; i < queueAttachments.Count; i++)
        {
            if (queueAttachments[i] != null && queueAttachments[i].fileName == fileName)
            {
                return true;
            }
        }
        return false;
    }

    private void AddQueueAttachment(string fileName, int sizeKb, long sizeBytes = 0)
    {
        if (string.IsNullOrEmpty(fileName) || sizeKb <= 0)
        {
            return;
        }

        queueAttachments.Add(new MessageAttachmentRecord(fileName, sizeKb, sizeBytes));
        SyncQueueAttachmentLegacyFields();
    }

    private void SetQueueAttachments(List<MessageAttachmentRecord> attachments)
    {
        queueAttachments.Clear();
        if (attachments != null)
        {
            for (int i = 0; i < attachments.Count; i++)
            {
                if (attachments[i] != null && !string.IsNullOrEmpty(attachments[i].fileName) && attachments[i].sizeKb > 0)
                {
                    queueAttachments.Add(new MessageAttachmentRecord(attachments[i].fileName, attachments[i].sizeKb, attachments[i].sizeBytes));
                }
            }
        }
        SyncQueueAttachmentLegacyFields();
    }

    private List<MessageAttachmentRecord> CloneQueueAttachments()
    {
        List<MessageAttachmentRecord> attachments = new List<MessageAttachmentRecord>();
        for (int i = 0; i < queueAttachments.Count; i++)
        {
            if (queueAttachments[i] != null && !string.IsNullOrEmpty(queueAttachments[i].fileName) && queueAttachments[i].sizeKb > 0)
            {
                attachments.Add(new MessageAttachmentRecord(queueAttachments[i].fileName, queueAttachments[i].sizeKb, queueAttachments[i].sizeBytes));
            }
        }
        return attachments;
    }

    private void SyncQueueAttachmentLegacyFields()
    {
        if (queueAttachments.Count == 0)
        {
            queueAttachmentName = string.Empty;
            queueAttachmentSizeKb = 0;
            return;
        }

        queueAttachmentName = queueAttachments[0].fileName;
        queueAttachmentSizeKb = QueueAttachmentSizeKb();
    }

    private void RefreshQueueListCards()
    {
        int queuedCount = missionManager != null ? missionManager.QueuedMessageCount : 0;
        if (queueHeadingText != null)
        {
            queueHeadingText.text = QueuePanelStatusText(queuedCount);
        }
        if (queueListContent == null)
        {
            if (queueListText != null)
            {
                queueListText.text = QueueListPreview();
            }
            return;
        }
        if (queueSelectedRowIndex >= queuedCount)
        {
            queueSelectedRowIndex = queuedCount - 1;
        }

        for (int i = 0; i < queuedCount; i++)
        {
            EnsureQueueRowCard(i);
            queueRowRects[i].gameObject.SetActive(true);
            queueRowTexts[i].text = QueueRowSummaryText(i);
            queueUpButtons[i].interactable = i > 0;
            queueDownButtons[i].interactable = i < queuedCount - 1;
            queueEditButtons[i].interactable = missionManager.CanEditQueuedMessage(i);
            queueDeleteButtons[i].interactable = missionManager.CanCancelQueuedMessage(i);
            SetButtonLabel(queueUpButtons[i], "▲");
            SetButtonLabel(queueDownButtons[i], "▼");
            SetButtonLabel(queueEditButtons[i], "수정");
            SetButtonLabel(queueDeleteButtons[i], "삭제");
            SetButtonVisual(queueUpButtons[i], i > 0 ? ButtonVisual.Secondary : ButtonVisual.Disabled);
            SetButtonVisual(queueDownButtons[i], i < queuedCount - 1 ? ButtonVisual.Secondary : ButtonVisual.Disabled);
            SetButtonVisual(queueEditButtons[i], queueEditingIndex == i ? ButtonVisual.Selected : missionManager.CanEditQueuedMessage(i) ? ButtonVisual.Primary : ButtonVisual.Disabled);
            SetButtonVisual(queueDeleteButtons[i], missionManager.CanCancelQueuedMessage(i) ? ButtonVisual.Secondary : ButtonVisual.Disabled);
            SetButtonVisual(queueRowCardButtons[i], queueSelectedRowIndex == i || queueEditingIndex == i ? ButtonVisual.Selected : ButtonVisual.Secondary);
            queueCardBackgrounds[i].color = queueEditingIndex == i
                ? new Color(0.08f, 0.28f, 0.36f, 0.86f)
                : queueSelectedRowIndex == i
                    ? new Color(0.06f, 0.20f, 0.30f, 0.86f)
                    : new Color(0.05f, 0.13f, 0.19f, 0.78f);
        }

        for (int i = 0; i < queueEditButtons.Count; i++)
        {
            bool visible = i < queuedCount;
            if (i < queueRowRects.Count)
            {
                queueRowRects[i].gameObject.SetActive(visible);
            }
        }
    }

    private void EditQueuedMessage(int index)
    {
        if (missionManager == null || index < 0 || index >= missionManager.QueuedMessageCount)
        {
            return;
        }

        queueSelectedRowIndex = index;
        if (queueMessageInputField == null)
        {
            BeginQueuedMessageFlowFromQueuedRecord(index);
            return;
        }

        QueuedMessageRecord record = missionManager.QueuedMessages[index];
        queueEditingIndex = index;
        queueMessageInputField.SetTextWithoutNotify(record.messageText ?? string.Empty);
        queueOriginName = string.IsNullOrEmpty(record.originName) ? missionManager.OriginName : record.originName;
        queueTargetName = string.IsNullOrEmpty(record.targetName) ? OrbitShareDestinations.DefaultTargetName : record.targetName;
        queueSelectedSatelliteName = record.preferredSatelliteName ?? string.Empty;
        queueScheduleOverride = true;
        queueScheduledDayOffset = record.scheduledDayOffset;
        queueScheduledHour = record.scheduledHour;
        queueScheduledMinute = record.scheduledMinute;
        SetQueueAttachments(AttachmentsOrLegacy(record.attachments, record.attachmentName, record.attachmentSizeKb));
        if (queueStatusText != null)
        {
            queueStatusText.text = "수정 중: " + (index + 1) + "번";
        }
        ShowQueuePanel(false);
    }

    private void MoveQueuedMessage(int fromIndex, int toIndex)
    {
        if (missionManager == null || fromIndex < 0 || fromIndex >= missionManager.QueuedMessageCount || toIndex < 0 || toIndex >= missionManager.QueuedMessageCount)
        {
            return;
        }
        missionManager.MoveQueuedMessage(fromIndex, toIndex);
        if (queueEditingIndex == fromIndex)
        {
            queueEditingIndex = toIndex;
        }
        else if (queueEditingIndex == toIndex)
        {
            queueEditingIndex = fromIndex;
        }
        if (queueSelectedRowIndex == fromIndex)
        {
            queueSelectedRowIndex = toIndex;
        }
        else if (queueSelectedRowIndex == toIndex)
        {
            queueSelectedRowIndex = fromIndex;
        }
        RefreshAll();
    }

    private void SelectQueuedMessageRow(int index)
    {
        if (missionManager == null || index < 0 || index >= missionManager.QueuedMessageCount)
        {
            queueSelectedRowIndex = -1;
            return;
        }

        queueSelectedRowIndex = index;
        RefreshQueuePanel();
    }

    private void BeginQueuedMessageFlowFromQueuedRecord(int index)
    {
        if (missionManager == null || index < 0 || index >= missionManager.QueuedMessageCount)
        {
            return;
        }

        QueuedMessageRecord record = missionManager.QueuedMessages[index];
        BeginQueuedMessageFlow();
        queueEditingIndex = index;
        queueSelectedRowIndex = index;
        queueOriginName = string.IsNullOrEmpty(record.originName) ? missionManager.OriginName : record.originName;
        queueTargetName = string.IsNullOrEmpty(record.targetName) ? OrbitShareDestinations.DefaultTargetName : record.targetName;
        queueFlowOriginSelected = true;
        queueFlowTargetSelected = true;
        queueScheduleOverride = true;
        queueScheduledDayOffset = record.scheduledDayOffset;
        queueScheduledHour = record.scheduledHour;
        queueScheduledMinute = record.scheduledMinute;
        SetQueueAttachments(AttachmentsOrLegacy(record.attachments, record.attachmentName, record.attachmentSizeKb));
        if (messageInputField != null)
        {
            messageInputField.SetTextWithoutNotify(record.messageText ?? string.Empty);
        }
        if (queueMessagePanel != null)
        {
            queueMessagePanel.gameObject.SetActive(false);
        }
        currentFlowStep = 5;
        ShowToast((index + 1) + "번 대기 메시지를 수정합니다.");
        RefreshAll();
    }

    private void ConfirmRemoveQueuedMessage(int index)
    {
        if (missionManager == null || index < 0 || index >= missionManager.QueuedMessageCount)
        {
            return;
        }
        ShowConfirm((index + 1) + "번 메시지를 삭제할까요?", delegate
        {
            RemoveQueuedMessageAndShiftHeld(index);
            if (queueEditingIndex == index)
            {
                queueEditingIndex = -1;
                queueScheduleOverride = false;
            }
            else if (queueEditingIndex > index)
            {
                queueEditingIndex--;
            }
            if (queueSelectedRowIndex == index)
            {
                queueSelectedRowIndex = -1;
            }
            else if (queueSelectedRowIndex > index)
            {
                queueSelectedRowIndex--;
            }
            ShowToast("메시지를 삭제했습니다.");
            RefreshAll();
        });
    }

    private void ShowQueuePanel()
    {
        ShowQueuePanel(true);
    }

    private void ShowQueuePanel(bool resetEditing)
    {
        if (queueMessagePanel != null)
        {
            queueMessagePanel.gameObject.SetActive(true);
            queueMessagePanel.SetAsLastSibling();
        }
        if (resetEditing)
        {
            if (!QueuePanelHasDraftInput())
            {
                queueEditingIndex = -1;
                queueScheduleOverride = false;
            }
        }
        RefreshQueuePanel();
        ApplyFixedPanelOrder();
        RefreshPanelLauncherButtons();
    }

    private void ToggleQueuePanel()
    {
        if (queueMessagePanel != null && queueMessagePanel.gameObject.activeSelf)
        {
            queueMessagePanel.gameObject.SetActive(false);
            RefreshPanelLauncherButtons();
            return;
        }
        ShowQueuePanel();
        RefreshPanelLauncherButtons();
    }

    private void ConfirmRemoveFirstQueuedMessage()
    {
        if (missionManager == null || missionManager.QueuedMessageCount == 0)
        {
            return;
        }
        ShowConfirm("첫 번째 메시지를 삭제할까요?", delegate
        {
            RemoveQueuedMessageAndShiftHeld(0);
            if (queueSelectedRowIndex == 0)
            {
                queueSelectedRowIndex = -1;
            }
            else if (queueSelectedRowIndex > 0)
            {
                queueSelectedRowIndex--;
            }
            ShowToast("메시지를 삭제했습니다.");
            RefreshAll();
        });
    }

    private bool IsQueuedMessageHeld(int index)
    {
        return false;
    }

    private void RemoveQueuedMessageAndShiftHeld(int index)
    {
        if (missionManager == null || index < 0 || index >= missionManager.QueuedMessageCount)
        {
            return;
        }

        missionManager.CancelQueuedMessage(index);
    }

    private string QueuePanelStatusText(int queuedCount)
    {
        return "대기 메시지";
    }

    private string QueueShortSummary(int index)
    {
        if (missionManager == null || index < 0 || index >= missionManager.QueuedMessageCount)
        {
            return "확인 중";
        }

        QueuedMessageRecord record = missionManager.QueuedMessages[index];
        if (record == null)
        {
            return "확인 중";
        }
        string origin = string.IsNullOrEmpty(record.originLabel) ? DestinationLabel(record.originName) : record.originLabel;
        string destination = string.IsNullOrEmpty(record.destinationLabel) ? DestinationLabel(record.targetName) : record.destinationLabel;
        return (index + 1) + "번 · " + RouteDisplayText(origin, destination);
    }

    private void StartNextQueuedMessage()
    {
        // 메시지 목록 기능은 UI에서 숨겨져 있으므로 수동 시작도 노출하지 않습니다.
    }

    private bool StartNextQueuedMessageSkippingHeld()
    {
        return false;
    }

    private void RemoveFirstQueueAttachment()
    {
        RemoveQueueAttachmentAt(0);
    }

    private void RemoveQueueAttachmentAt(int index)
    {
        if (index < 0 || index >= queueAttachments.Count)
        {
            return;
        }
        queueAttachments.RemoveAt(index);
        SyncQueueAttachmentLegacyFields();
        ShowToast("첨부 파일을 제거했습니다.");
        RefreshAll();
    }

    private void ToggleQueueAttachmentPicker()
    {
        ToggleQueueAttachmentPicker(queueAttachmentPickerPanel == null || !queueAttachmentPickerPanel.gameObject.activeSelf);
    }

    private void ToggleQueueAttachmentPicker(bool visible)
    {
        if (queueAttachmentPickerPanel != null)
        {
            queueAttachmentPickerPanel.gameObject.SetActive(visible);
        }
    }

    private void AddQueuedMessageFromPanel()
    {
        if (missionManager == null || queueMessageInputField == null)
        {
            return;
        }

        string error;
        List<MessageAttachmentRecord> attachments = CloneQueueAttachments();
        int savedEditingIndex = queueEditingIndex;
        bool queued = queueEditingIndex >= 0
            ? UpdateQueuedMessageWithCurrentSchedule(queueEditingIndex, queueMessageInputField.text, queueTargetName, attachments, out error)
            : QueueMessageWithCurrentSchedule(queueMessageInputField.text, queueTargetName, attachments, out error);
        if (queueStatusText != null)
        {
            queueStatusText.text = queued ? (queueEditingIndex >= 0 ? "메시지를 수정했습니다." : "메시지를 추가했습니다.") : error;
            queueStatusText.color = queued ? new Color(0.68f, 1f, 0.86f, 0.9f) : new Color(1f, 0.48f, 0.42f, 0.96f);
        }

        if (queued)
        {
            queueMessageInputField.SetTextWithoutNotify(string.Empty);
            queueAttachmentName = string.Empty;
            queueAttachmentSizeKb = 0;
            queueAttachments.Clear();
            queueSelectedRowIndex = savedEditingIndex >= 0 ? savedEditingIndex : missionManager.QueuedMessageCount - 1;
            queueEditingIndex = -1;
            queueScheduleOverride = false;
            ToggleQueueAttachmentPicker(false);
            ShowQueuePanel();
        }
        RefreshAll();
    }

    private bool QueueMessageWithCurrentSchedule(string messageText, string targetName, List<MessageAttachmentRecord> attachments, out string error)
    {
        return missionManager.QueueMessageFromOrigin(
            messageText,
            queueOriginName,
            targetName,
            QueueScheduledDayOffset(),
            QueueScheduledHour(),
            QueueScheduledMinute(),
            attachments,
            out error);
    }

    private bool UpdateQueuedMessageWithCurrentSchedule(int index, string messageText, string targetName, List<MessageAttachmentRecord> attachments, out string error)
    {
        return missionManager.UpdateQueuedMessageFromOrigin(
            index,
            messageText,
            queueOriginName,
            targetName,
            QueueScheduledDayOffset(),
            QueueScheduledHour(),
            QueueScheduledMinute(),
            attachments,
            out error);
    }

    private int QueueScheduledDayOffset()
    {
        return queueScheduleOverride ? queueScheduledDayOffset : missionManager.ScheduledDayOffset;
    }

    private int QueueScheduledHour()
    {
        return queueScheduleOverride ? queueScheduledHour : missionManager.ScheduledHour;
    }

    private int QueueScheduledMinute()
    {
        return queueScheduleOverride ? queueScheduledMinute : missionManager.ScheduledMinute;
    }

    private string QueuedFlowMessageText()
    {
        return messageInputField != null ? messageInputField.text ?? string.Empty : string.Empty;
    }

    private int QueuedFlowTextBytes()
    {
        return missionManager != null ? missionManager.CalculateMessageTextBytes(QueuedFlowMessageText()) : 0;
    }

    private int QueuedFlowPayloadKb()
    {
        return missionManager != null ? missionManager.CalculateMessageSizeKb(QueuedFlowMessageText(), queueAttachments) : 0;
    }

    private bool FlowPayloadTooLarge()
    {
        if (missionManager == null)
        {
            return false;
        }
        return composingQueuedMessageInFlow ? QueuedFlowPayloadKb() > missionManager.MaxPayloadKb : missionManager.PayloadTooLarge;
    }

    private string QueueAttachmentSummary()
    {
        return queueAttachments.Count == 0 || missionManager == null ? "첨부 없음" : missionManager.GetAttachmentSummary(queueAttachments);
    }

    private bool CanAdvanceQueuedFlowStep()
    {
        if (!composingQueuedMessageInFlow || missionManager == null)
        {
            return false;
        }

        switch (currentFlowStep)
        {
            case 1:
                return FlowHasScheduleSelection();
            case 2:
                return !string.IsNullOrWhiteSpace(QueuedFlowMessageText()) && !FlowPayloadTooLarge();
            case 3:
                return queueFlowOriginSelected
                    && queueFlowTargetSelected
                    && !string.IsNullOrEmpty(queueOriginName)
                    && !string.IsNullOrEmpty(queueTargetName);
            case 4:
                return QueueFlowRecommendedSatellite() != null;
            default:
                return false;
        }
    }

    private bool CanQueueFlowSelection()
    {
        return composingQueuedMessageInFlow
            && missionManager != null
            && FlowHasScheduleSelection()
            && !string.IsNullOrWhiteSpace(QueuedFlowMessageText())
            && !FlowPayloadTooLarge()
            && queueFlowOriginSelected
            && queueFlowTargetSelected
            && !string.IsNullOrEmpty(queueOriginName)
            && !string.IsNullOrEmpty(queueTargetName)
            && QueueFlowRecommendedSatellite() != null;
    }

    private SatelliteData QueueFlowRecommendedSatellite()
    {
        if (satelliteManager == null || missionManager == null)
        {
            return null;
        }

        SatelliteData selected = QueueSelectedSatellite();
        int payloadKb = Mathf.Max(1, QueuedFlowPayloadKb());
        int delayMinutes = QueueFlowRequestedDelayMinutes();
        if (selected != null && CanServeRoute(selected, queueTargetName, delayMinutes))
        {
            return selected;
        }
        return satelliteManager.FindBestSatellite(queueOriginName, queueTargetName, delayMinutes, payloadKb);
    }

    private SatelliteData QueueSelectedSatellite()
    {
        if (satelliteManager == null || string.IsNullOrEmpty(queueSelectedSatelliteName))
        {
            return null;
        }
        return satelliteManager.Satellites.Find(satellite => satellite != null && satellite.satelliteName == queueSelectedSatelliteName);
    }

    private string QueueFlowSatelliteDetailText()
    {
        SatelliteData satellite = QueueSelectedSatellite();
        if (satellite == null)
        {
            satellite = QueueFlowRecommendedSatellite();
        }
        if (satellite == null)
        {
            return "위성 이름 : 추천 대기\n"
                + "위성 종류 : 위성망\n"
                + "경로 : " + DestinationLabel(queueOriginName) + " -> " + DestinationLabel(queueTargetName) + "\n"
                + "통신 가능 시간 : 계산 중\n"
                + "가격 정보 : 계산 중\n"
                + "성공률 : 계산 중\n"
                + "예상 소요 시간 : 계산 중";
        }

        int delayMinutes = QueueFlowRequestedDelayMinutes();
        SatelliteRouteEstimateRecord estimate = RouteEstimate(satellite, queueTargetName, delayMinutes);
        float price = EstimateSatellitePriceForCurrentMessage(satellite);
        return "위성 이름 : " + satellite.satelliteName + "\n"
            + "위성 종류 : " + SatelliteKindText(satellite) + "\n"
            + "경로 : " + SatelliteRouteText(queueTargetName) + "\n"
            + "통신 가능 시간 : " + CommunicationWindowText(satellite, estimate) + "\n"
            + "가격 정보 : " + FormatUsd(price) + "\n"
            + "성공률 : " + satellite.RecentSuccessPercent + "%\n"
            + "예상 소요 시간 : " + TransferDurationText(satellite, estimate);
    }

    private int QueueFlowRequestedDelayMinutes()
    {
        if (missionManager == null)
        {
            return 0;
        }
        System.DateTime now = System.DateTime.Now;
        System.DateTime scheduled = now.Date
            .AddDays(Mathf.Clamp(QueueScheduledDayOffset(), 0, 365))
            .AddHours(Mathf.Clamp(QueueScheduledHour(), 0, 23))
            .AddMinutes(Mathf.Clamp(QueueScheduledMinute(), 0, 59));
        return Mathf.Max(0, Mathf.CeilToInt((float)(scheduled - now).TotalMinutes));
    }

    private string QueueFlowScheduleLabel()
    {
        if (missionManager == null)
        {
            return "즉시 전송";
        }
        int delayMinutes = QueueFlowRequestedDelayMinutes();
        if (delayMinutes == 0)
        {
            return "즉시 전송";
        }
        System.DateTime scheduled = System.DateTime.Now.Date
            .AddDays(Mathf.Clamp(QueueScheduledDayOffset(), 0, 365))
            .AddHours(Mathf.Clamp(QueueScheduledHour(), 0, 23))
            .AddMinutes(Mathf.Clamp(QueueScheduledMinute(), 0, 59));
        return ScheduleDisplayLabel(delayMinutes, scheduled.ToString("MM/dd HH:mm"));
    }

    private void SetQueuedFlowScheduleFromDelay(int minutes)
    {
        System.DateTime scheduled = System.DateTime.Now.AddMinutes(Mathf.Max(0, minutes));
        queueScheduleOverride = true;
        queueScheduledDayOffset = Mathf.Clamp((scheduled.Date - System.DateTime.Now.Date).Days, 0, 365);
        queueScheduledHour = scheduled.Hour;
        queueScheduledMinute = scheduled.Minute - scheduled.Minute % 5;
    }

    private string FlowStepTitle()
    {
        if (composingQueuedMessageInFlow)
        {
            return "메시지 작성";
        }
        if (missionManager != null && IsActiveTransferPhase(missionManager.Phase))
        {
            return "현재 전송";
        }
        switch (currentFlowStep)
        {
            case 1:
                return "1/5 시간 선택";
            case 2:
                return "2/5 메시지 작성";
            case 3:
                return "3/5 송신지/수신지 선택";
            case 4:
                return "4/5 위성망 확인";
            default:
                return "5/5 확인 및 전송";
        }
    }

    private string FlowStepHint()
    {
        if (missionManager == null)
        {
            return string.Empty;
        }

        switch (currentFlowStep)
        {
            case 1:
                return composingQueuedMessageInFlow ? "추가 메시지를 보낼 시간을 선택하세요." : "보낼 시간을 선택하세요.";
            case 2:
                return "메시지를 입력하고 필요한 파일만 첨부하세요.";
            case 3:
                return "송신지와 수신지를 고르면 가능한 위성이 갱신됩니다.";
            case 4:
                return "좌측 위성 목록에서 사용할 위성을 선택하세요.";
            default:
                if (IsActiveTransferPhase(missionManager.Phase))
                {
                    return "현재 전송 진행 중";
                }
                return missionManager.Phase == MissionPhase.Idle
                    ? "선택 내용을 확인하고 전송을 시작하세요."
                    : "전송 진행 상태를 확인하세요.";
        }
    }

    private string FlowStepBlockedReason()
    {
        if (missionManager == null)
        {
            return string.Empty;
        }
        if (composingQueuedMessageInFlow)
        {
            if (currentFlowStep == 1 && !FlowHasScheduleSelection())
            {
                return "보낼 시간을 먼저 선택하세요.";
            }
            if (currentFlowStep == 2)
            {
                if (string.IsNullOrWhiteSpace(QueuedFlowMessageText()))
                {
                    return "메시지를 입력하세요.";
                }
                if (FlowPayloadTooLarge())
                {
                    return "크기 초과: " + PayloadBreakdownText();
                }
            }
            if (currentFlowStep == 3 && (!queueFlowOriginSelected || !queueFlowTargetSelected))
            {
                return "송신지와 수신지를 모두 선택하세요.";
            }
            if (currentFlowStep == 4 && !CanQueueFlowSelection())
            {
                return "사용할 위성망을 선택하세요.";
            }
            return string.Empty;
        }
        if (missionManager.Phase != MissionPhase.Idle)
        {
            return string.Empty;
        }

        if (currentFlowStep == 1 && !FlowHasScheduleSelection())
        {
            return "보낼 시간을 먼저 선택하세요.";
        }
        if (currentFlowStep == 2)
        {
            if (string.IsNullOrWhiteSpace(missionManager.MessageText))
            {
                return "메시지를 입력하세요.";
            }
            if (missionManager.PayloadTooLarge)
            {
                return "크기 초과: " + PayloadBreakdownText();
            }
        }
        if (currentFlowStep == 3 && (!missionManager.HasExplicitOriginSelection || !missionManager.HasExplicitDestinationSelection))
        {
            return "송신지와 수신지를 모두 선택하세요.";
        }
        if (currentFlowStep == 4 && !CanReserveCurrentSelection())
        {
            return "위성망 추천을 확인하세요.";
        }
        return string.Empty;
    }

    private void BuildQueueDestinationButtons(RectTransform parent)
    {
        queueDestinationButtons.Clear();
        List<string> targetNames = new List<string>();
        if (missionManager != null && missionManager.Targets != null && missionManager.Targets.Count > 0)
        {
            for (int i = 0; i < missionManager.Targets.Count; i++)
            {
                targetNames.Add(missionManager.Targets[i].targetName);
            }
        }
        else
        {
            targetNames.AddRange(OrbitShareDestinations.Names);
        }

        if (targetNames.Count > 0 && !targetNames.Contains(queueTargetName))
        {
            queueTargetName = targetNames[0];
        }

        int count = Mathf.Min(12, targetNames.Count);
        for (int i = 0; i < count; i++)
        {
            CreateQueueDestinationButton(parent, targetNames[i], DestinationLabel(targetNames[i]), i);
        }
    }

    private void CreateQueueDestinationButton(RectTransform parent, string targetName, string label, int index)
    {
        int col = index % 4;
        int row = index / 4;
        float left = col / 4f;
        float right = left + 0.235f;
        float top = -228f - row * 38f;
        float bottom = top - 32f;
        Button button = CreateButton(
            parent,
            label,
            new Vector2(left, 1f),
            new Vector2(right, 1f),
            new Vector2(col == 0 ? 22f : 4f, bottom),
            new Vector2(col == 3 ? -22f : -4f, top),
            delegate
            {
                if (selectingOriginInQueuePanel)
                {
                    queueOriginName = targetName;
                }
                else
                {
                    queueTargetName = targetName;
                }
                RefreshQueuePanel();
            });
        Text text = button.GetComponentInChildren<Text>();
        if (text != null)
        {
            text.fontSize = 12;
        }
        queueDestinationButtons[targetName] = button;
    }

    private void EnsureQueueRowCard(int index)
    {
        while (queueRowRects.Count <= index)
        {
            int rowIndex = queueRowRects.Count;
            int capturedIndex = rowIndex;
            GameObject cardObject = new GameObject("QueueCard" + (rowIndex + 1), typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement));
            cardObject.transform.SetParent(queueListContent, false);
            RectTransform cardRect = cardObject.GetComponent<RectTransform>();
            LayoutElement layoutElement = cardObject.GetComponent<LayoutElement>();
            layoutElement.minHeight = 116f;
            layoutElement.preferredHeight = 128f;
            layoutElement.flexibleWidth = 1f;

            Image cardBackground = cardObject.GetComponent<Image>();
            cardBackground.color = new Color(0.05f, 0.13f, 0.19f, 0.78f);
            cardBackground.raycastTarget = true;

            Button cardButton = cardObject.GetComponent<Button>();
            cardButton.targetGraphic = cardBackground;
            cardButton.transition = Selectable.Transition.ColorTint;
            cardButton.onClick.AddListener(delegate { SelectQueuedMessageRow(capturedIndex); });

            Text rowText = CreateText(cardObject.transform, string.Empty, 15, TextAnchor.UpperLeft);
            rowText.lineSpacing = 1.02f;
            rowText.rectTransform.anchorMin = new Vector2(0f, 0f);
            rowText.rectTransform.anchorMax = new Vector2(0.68f, 1f);
            rowText.rectTransform.offsetMin = new Vector2(14f, 10f);
            rowText.rectTransform.offsetMax = new Vector2(-8f, -10f);

            Button upButton = CreateButton(cardObject.transform, "▲", new Vector2(0.70f, 0.54f), new Vector2(0.765f, 0.92f), new Vector2(0f, 0f), new Vector2(-4f, 0f), delegate { MoveQueuedMessage(capturedIndex, capturedIndex - 1); });
            Button downButton = CreateButton(cardObject.transform, "▼", new Vector2(0.70f, 0.08f), new Vector2(0.765f, 0.46f), new Vector2(0f, 0f), new Vector2(-4f, 0f), delegate { MoveQueuedMessage(capturedIndex, capturedIndex + 1); });
            Button editButton = CreateButton(cardObject.transform, "수정", new Vector2(0.78f, 0.18f), new Vector2(0.88f, 0.82f), new Vector2(0f, 0f), new Vector2(-5f, 0f), delegate { EditQueuedMessage(capturedIndex); });
            Button deleteButton = CreateButton(cardObject.transform, "삭제", new Vector2(0.895f, 0.18f), new Vector2(0.995f, 0.82f), new Vector2(0f, 0f), new Vector2(-8f, 0f), delegate { ConfirmRemoveQueuedMessage(capturedIndex); });
            SetButtonTextSize(editButton, 14);
            SetButtonTextSize(upButton, 18);
            SetButtonTextSize(downButton, 18);
            SetButtonTextSize(deleteButton, 14);

            queueRowRects.Add(cardRect);
            queueRowTexts.Add(rowText);
            queueRowCardButtons.Add(cardButton);
            queueCardBackgrounds.Add(cardBackground);
            queueEditButtons.Add(editButton);
            queueUpButtons.Add(upButton);
            queueDownButtons.Add(downButton);
            queueDeleteButtons.Add(deleteButton);
        }
    }

    private void CreateQueueAttachmentButton(RectTransform parent, string fileName, string label, int sizeKb, float leftAnchor, float rightAnchor, float top, float bottom)
    {
        Button button = CreateButton(
            parent,
            label,
            new Vector2(leftAnchor, 1f),
            new Vector2(rightAnchor, 1f),
            new Vector2(leftAnchor <= 0f ? 12f : 6f, bottom),
            new Vector2(rightAnchor >= 1f ? -12f : -6f, top),
            delegate
            {
                AddQueueAttachment(fileName, sizeKb);
                ToggleQueueAttachmentPicker(false);
                RefreshAll();
            });
        Text buttonText = button.GetComponentInChildren<Text>();
        if (buttonText != null)
        {
            buttonText.fontSize = 13;
            buttonText.lineSpacing = 0.9f;
        }
        queueAttachmentButtons[fileName] = button;
    }

    private void CreateQueueAttachmentDeleteButtons(RectTransform parent)
    {
        queueAttachmentDeleteButtons.Clear();
        for (int i = 0; i < 3; i++)
        {
            int index = i;
            float top = -382f - i * 28f;
            Button button = CreateButton(
                parent,
                (i + 1) + " 삭제",
                new Vector2(0.68f, 1f),
                new Vector2(1f, 1f),
                new Vector2(4f, top - 24f),
                new Vector2(-22f, top),
                delegate { RemoveQueueAttachmentAt(index); });
            SetButtonTextSize(button, 11);
            button.gameObject.SetActive(false);
            queueAttachmentDeleteButtons.Add(button);
        }
    }

    private void RefreshQueueAttachmentDeleteButtons()
    {
        for (int i = 0; i < queueAttachmentDeleteButtons.Count; i++)
        {
            Button button = queueAttachmentDeleteButtons[i];
            bool active = i < queueAttachments.Count;
            button.gameObject.SetActive(active);
            if (active)
            {
                SetButtonLabel(button, (i + 1) + "번 삭제");
                SetButtonVisual(button, ButtonVisual.Secondary);
                button.interactable = true;
            }
        }
    }

    private string QueueAttachmentPreview()
    {
        return queueAttachments.Count == 0 ? string.Empty : AttachmentPreview(queueAttachments);
    }

    private string QueueScheduleHint()
    {
        if (missionManager == null)
        {
            return "예약 시간 선택 준비 중";
        }
        string activeHint = IsActiveTransferPhase(missionManager.Phase)
            ? " · 현재 전송 후 이어서 전송"
            : string.Empty;
        string scheduleText = queueScheduleOverride ? QueueFlowScheduleLabel() : SendTimeLabel();
        return "일정 : " + scheduleText + " · " + RouteDisplayText(DestinationLabel(queueOriginName), DestinationLabel(queueTargetName)) + activeHint;
    }

    private bool QueuePanelHasDraftInput()
    {
        return (queueMessageInputField != null && !string.IsNullOrWhiteSpace(queueMessageInputField.text))
            || queueAttachments.Count > 0
            || queueEditingIndex >= 0
            || queueScheduleOverride;
    }

    private string QueueDraftEstimateLine(string message, int payloadKb)
    {
        if (missionManager == null)
        {
            return "예상 : 계산 준비 중";
        }
        if (string.IsNullOrWhiteSpace(message))
        {
            return "예상 : 메시지 입력 후 계산";
        }
        if (satelliteManager == null)
        {
            return "예상 : 위성망 계산 중";
        }

        int safePayloadKb = Mathf.Max(1, payloadKb);
        int delayMinutes = QueueFlowRequestedDelayMinutes();
        SatelliteData satellite = satelliteManager.FindBestSatellite(queueOriginName, queueTargetName, delayMinutes, safePayloadKb);
        if (satellite == null)
        {
            return "예상 : 사용할 위성망 없음";
        }

        SatelliteRouteEstimateRecord estimate = satelliteManager.GetRouteEstimate(satellite, queueOriginName, queueTargetName, delayMinutes, safePayloadKb);
        float baseFee;
        float payloadFee;
        float urgencyFee;
        float price = missionManager.EstimateDetailedPriceUsd(safePayloadKb, delayMinutes, satellite, out baseFee, out payloadFee, out urgencyFee);
        string duration = estimate != null ? FormatSeconds(Mathf.Max(1, estimate.estimatedDeliverySeconds)) : "계산 중";
        return "예상 소요 시간 : " + duration + "\n요금 : " + FormatUsd(price);
    }

    private string QueueListPreview()
    {
        if (missionManager == null || missionManager.QueuedMessageCount == 0)
        {
            return "대기 메시지가 없습니다.";
        }

        System.Text.StringBuilder builder = new System.Text.StringBuilder();
        int count = missionManager.QueuedMessageCount;
        for (int i = 0; i < count; i++)
        {
            builder.Append(QueueRowSummaryText(i));
            if (i < count - 1)
            {
                builder.Append("\n\n");
            }
        }
        return builder.ToString();
    }

    private string QueueRowSummaryText(int index)
    {
        if (missionManager == null || index < 0 || index >= missionManager.QueuedMessageCount)
        {
            return "확인 중";
        }

        QueuedMessageRecord record = missionManager.QueuedMessages[index];
        QueuedMessageRouteDetail detail = missionManager.GetQueuedMessageRouteDetail(index);
        string message = record == null || string.IsNullOrWhiteSpace(record.messageText) ? "(빈 메시지)" : record.messageText.Trim();
        if (message.Length > 38)
        {
            message = message.Substring(0, 38) + "…";
        }
        string attachmentSummary = detail != null && !string.IsNullOrEmpty(detail.attachmentSummary)
            ? detail.attachmentSummary
            : record == null || string.IsNullOrEmpty(record.attachmentName)
                ? string.Empty
                : AttachmentIcon(record.attachmentName) + " " + record.attachmentName + " · " + PayloadKbText(record.attachmentSizeKb);
        string origin = record == null || string.IsNullOrEmpty(record.originLabel)
            ? record == null || string.IsNullOrEmpty(record.originName) ? "송신지" : DestinationLabel(record.originName)
            : record.originLabel;
        string destination = record == null || string.IsNullOrEmpty(record.destinationLabel)
            ? record == null || string.IsNullOrEmpty(record.targetName) ? "수신지" : DestinationLabel(record.targetName)
            : record.destinationLabel;
        string satelliteName = detail != null && detail.isAvailable && !string.IsNullOrEmpty(detail.satelliteName)
            ? CompactSatelliteName(detail.satelliteName)
            : "통신 불가";

        System.Text.StringBuilder builder = new System.Text.StringBuilder();
        builder.Append("<b>").Append(index + 1).Append("번</b>")
            .Append("\n경로 : ").Append(RouteDisplayText(origin, destination))
            .Append("\n위성명 : ").Append(satelliteName)
            .Append("\n내용 : ").Append(message);
        if (!string.IsNullOrEmpty(attachmentSummary))
        {
            builder.Append("\n첨부 : ").Append(attachmentSummary);
        }
        return builder.ToString();
    }

    private int FirstStartableQueuedMessageIndex()
    {
        if (missionManager == null)
        {
            return -1;
        }
        for (int i = 0; i < missionManager.QueuedMessageCount; i++)
        {
            if (!IsQueuedMessageHeld(i))
            {
                return i;
            }
        }
        return -1;
    }
}
