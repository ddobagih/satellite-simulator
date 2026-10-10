using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

// 메시지 작성 UI: 시간/메시지/위치/위성망/확인 플로우와 입력 보조 UI를 관리합니다.
public partial class UIManager
{
    private void CreateRequestFlowPanel(Transform parent)
    {
        requestFlowPanel = CreatePanel(parent, "RequestFlowPanel", new Vector2(0.64f, 0.13f), new Vector2(0.985f, 0.86f), new Color(0.025f, 0.045f, 0.075f, 0.99f));
        MakeDraggable(requestFlowPanel);
        Text heading = CreateText(requestFlowPanel, "메시지 작성", 24, TextAnchor.UpperLeft);
        heading.rectTransform.anchorMin = new Vector2(0f, 1f);
        heading.rectTransform.anchorMax = new Vector2(1f, 1f);
        heading.rectTransform.pivot = new Vector2(0.5f, 1f);
        heading.rectTransform.anchoredPosition = new Vector2(0f, -14f);
        heading.rectTransform.sizeDelta = new Vector2(-28f, 32f);

        flowInstructionText = CreateText(requestFlowPanel, string.Empty, 13, TextAnchor.UpperLeft);
        flowInstructionText.color = new Color(0.76f, 0.9f, 1f, 0.78f);
        flowInstructionText.rectTransform.anchorMin = new Vector2(0f, 1f);
        flowInstructionText.rectTransform.anchorMax = new Vector2(1f, 1f);
        flowInstructionText.rectTransform.pivot = new Vector2(0.5f, 1f);
        flowInstructionText.rectTransform.anchoredPosition = new Vector2(0f, -47f);
        flowInstructionText.rectTransform.sizeDelta = new Vector2(-28f, 20f);
        flowInstructionText.gameObject.SetActive(false);

        BuildFlowProgressButtons(requestFlowPanel);

        flowStepTitleText = CreateText(requestFlowPanel, string.Empty, 21, TextAnchor.UpperLeft);
        flowStepTitleText.fontStyle = FontStyle.Bold;
        flowStepTitleText.rectTransform.anchorMin = new Vector2(0f, 1f);
        flowStepTitleText.rectTransform.anchorMax = new Vector2(1f, 1f);
        flowStepTitleText.rectTransform.pivot = new Vector2(0.5f, 1f);
        flowStepTitleText.rectTransform.anchoredPosition = new Vector2(0f, -112f);
        flowStepTitleText.rectTransform.sizeDelta = new Vector2(-28f, 30f);
        flowStepTitleText.gameObject.SetActive(false);

        flowStepHintText = CreateText(requestFlowPanel, string.Empty, 14, TextAnchor.UpperLeft);
        flowStepHintText.color = new Color(0.9f, 0.96f, 1f, 0.84f);
        flowStepHintText.rectTransform.anchorMin = new Vector2(0f, 1f);
        flowStepHintText.rectTransform.anchorMax = new Vector2(1f, 1f);
        flowStepHintText.rectTransform.pivot = new Vector2(0.5f, 1f);
        flowStepHintText.rectTransform.anchoredPosition = new Vector2(0f, -146f);
        flowStepHintText.rectTransform.sizeDelta = new Vector2(-28f, 48f);
        flowStepHintText.gameObject.SetActive(false);

        scheduleStepPanel = CreateFlowStepPanel(requestFlowPanel, "ScheduleStep");
        messageStepPanel = CreateFlowStepPanel(requestFlowPanel, "MessageStep");
        destinationStepPanel = CreateFlowStepPanel(requestFlowPanel, "DestinationStep");
        satelliteStepPanel = CreateFlowStepPanel(requestFlowPanel, "SatelliteStep");
        confirmStepPanel = CreateFlowStepPanel(requestFlowPanel, "ConfirmStep");

        BuildScheduleStep(scheduleStepPanel);
        BuildMessageStep(messageStepPanel);
        BuildDestinationStep(destinationStepPanel);
        BuildSatelliteStep(satelliteStepPanel);
        BuildConfirmStep(confirmStepPanel);

        CreateButton(requestFlowPanel, "닫기", new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-78f, -44f), new Vector2(-14f, -10f), delegate { CloseRequestFlowPanel(); });
        flowBackButton = CreateButton(requestFlowPanel, "취소", new Vector2(0f, 0f), new Vector2(0.32f, 0f), new Vector2(18f, 18f), new Vector2(-6f, 56f), delegate
        {
            if (currentFlowStep <= 1)
            {
                CancelRequestFlow();
                return;
            }
            GoToFlowStep(currentFlowStep - 1);
        });
        flowNextButton = CreateButton(requestFlowPanel, "다음", new Vector2(0.34f, 0f), new Vector2(1f, 0f), new Vector2(0f, 18f), new Vector2(-18f, 56f), delegate { AdvanceFlowStep(); });
    }

    private RectTransform CreateFlowStepPanel(Transform parent, string objectName)
    {
        RectTransform panel = CreatePanel(parent, objectName, Vector2.zero, Vector2.one, new Color(1f, 1f, 1f, 0f));
        panel.offsetMin = new Vector2(0f, 90f);
        panel.offsetMax = new Vector2(0f, -110f);
        MakeDraggable(panel, requestFlowPanel);
        panel.gameObject.SetActive(false);
        return panel;
    }

    private void BuildFlowProgressButtons(RectTransform parent)
    {
        flowStepButtons.Clear();
        string[] labels = { "시간", "메시지", "위치", "위성망", "확인" };
        for (int i = 0; i < labels.Length; i++)
        {
            int step = i + 1;
            float contentRight = 1f;
            float left = i / (float)labels.Length * contentRight;
            float right = (i + 1f) / labels.Length * contentRight;
            Button button = CreateButton(parent, labels[i], new Vector2(left, 1f), new Vector2(right, 1f), new Vector2(i == 0 ? 18f : 4f, -92f), new Vector2(i == labels.Length - 1 ? -18f : -4f, -58f), delegate { JumpToFlowStep(step); });
            Text text = button.GetComponentInChildren<Text>();
            if (text != null)
            {
                text.fontSize = 12;
            }
            flowStepButtons.Add(button);
        }
    }

    private Button CreateConfirmEditButton(RectTransform parent, string label, int row, UnityAction action)
    {
        float left = row * 0.25f;
        float right = (row + 1f) * 0.25f;
        Button button = CreateButton(parent, label, new Vector2(left, 0f), new Vector2(right, 0f), new Vector2(row == 0 ? 18f : 4f, 0f), new Vector2(row == 3 ? -18f : -4f, 34f), action);
        Text text = button.GetComponentInChildren<Text>();
        if (text != null)
        {
            text.fontSize = 12;
        }
        return button;
    }

    private void BuildScheduleStep(RectTransform panel)
    {
        Text quickLabel = CreateText(panel, "빠른 선택", 14, TextAnchor.UpperLeft);
        quickLabel.color = new Color(0.82f, 0.9f, 1f, 0.78f);
        quickLabel.rectTransform.anchorMin = new Vector2(0f, 1f);
        quickLabel.rectTransform.anchorMax = new Vector2(1f, 1f);
        quickLabel.rectTransform.pivot = new Vector2(0.5f, 1f);
        quickLabel.rectTransform.anchoredPosition = new Vector2(0f, -4f);
        quickLabel.rectTransform.sizeDelta = new Vector2(-28f, 22f);
        quickLabel.gameObject.SetActive(false);

        sendDelayButtons[0] = CreateButton(panel, "지금", new Vector2(0f, 1f), new Vector2(0.25f, 1f), new Vector2(18f, -46f), new Vector2(-4f, -10f), delegate { SelectQuickDelay(0); });
        sendDelayButtons[5] = CreateButton(panel, "+5분", new Vector2(0.25f, 1f), new Vector2(0.5f, 1f), new Vector2(4f, -46f), new Vector2(-4f, -10f), delegate { SelectQuickDelay(5); });
        sendDelayButtons[15] = CreateButton(panel, "+15분", new Vector2(0.5f, 1f), new Vector2(0.75f, 1f), new Vector2(4f, -46f), new Vector2(-4f, -10f), delegate { SelectQuickDelay(15); });
        sendDelayButtons[30] = CreateButton(panel, "+30분", new Vector2(0.75f, 1f), new Vector2(1f, 1f), new Vector2(4f, -46f), new Vector2(-18f, -10f), delegate { SelectQuickDelay(30); });

        schedulePickerButton = CreateButton(panel, "직접 설정", new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(18f, -98f), new Vector2(-18f, -60f), delegate { ToggleCalendarPicker(); });

        scheduleSummaryText = CreateText(panel, string.Empty, 15, TextAnchor.UpperLeft);
        scheduleSummaryText.color = new Color(0.88f, 0.96f, 1f, 0.92f);
        scheduleSummaryText.rectTransform.anchorMin = new Vector2(0f, 1f);
        scheduleSummaryText.rectTransform.anchorMax = new Vector2(1f, 1f);
        scheduleSummaryText.rectTransform.pivot = new Vector2(0.5f, 1f);
        scheduleSummaryText.rectTransform.anchoredPosition = new Vector2(0f, -114f);
        scheduleSummaryText.rectTransform.sizeDelta = new Vector2(-28f, 24f);
        scheduleSummaryText.gameObject.SetActive(false);

        availabilityTimelineText = CreateText(panel, string.Empty, 12, TextAnchor.UpperLeft);
        availabilityTimelineText.color = new Color(0.82f, 0.94f, 1f, 0.74f);
        availabilityTimelineText.rectTransform.anchorMin = new Vector2(0f, 1f);
        availabilityTimelineText.rectTransform.anchorMax = new Vector2(1f, 1f);
        availabilityTimelineText.rectTransform.pivot = new Vector2(0.5f, 1f);
        availabilityTimelineText.rectTransform.anchoredPosition = new Vector2(0f, -184f);
        availabilityTimelineText.rectTransform.sizeDelta = new Vector2(-28f, 54f);
        availabilityTimelineText.gameObject.SetActive(false);

        scheduleRecommendedButton = CreateButton(panel, string.Empty, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(18f, -154f), new Vector2(-18f, -116f), delegate { ApplyRecommendedDelay(); });
        scheduleRecommendedButton.gameObject.SetActive(false);

        calendarPickerPanel = CreatePanel(panel, "CalendarPicker", new Vector2(0f, 1f), new Vector2(1f, 1f), new Color(0.02f, 0.045f, 0.075f, 1f));
        calendarPickerPanel.offsetMin = new Vector2(18f, -428f);
        calendarPickerPanel.offsetMax = new Vector2(-18f, -112f);
        MakeDraggable(calendarPickerPanel, requestFlowPanel);

        calendarMonthLabel = CreateText(calendarPickerPanel, System.DateTime.Now.ToString("yyyy년 M월"), 15, TextAnchor.UpperCenter);
        calendarMonthLabel.color = new Color(0.82f, 0.9f, 1f, 0.78f);
        calendarMonthLabel.rectTransform.anchorMin = new Vector2(0f, 1f);
        calendarMonthLabel.rectTransform.anchorMax = new Vector2(1f, 1f);
        calendarMonthLabel.rectTransform.pivot = new Vector2(0.5f, 1f);
        calendarMonthLabel.rectTransform.anchoredPosition = new Vector2(0f, -12f);
        calendarMonthLabel.rectTransform.sizeDelta = new Vector2(-24f, 20f);
        AttachCalendarMonthScroll(calendarPickerPanel);

        string[] weekdays = { "일", "월", "화", "수", "목", "금", "토" };
        for (int i = 0; i < weekdays.Length; i++)
        {
            Text weekdayLabel = CreateText(calendarPickerPanel, weekdays[i], 12, TextAnchor.MiddleCenter);
            weekdayLabel.color = new Color(0.82f, 0.9f, 1f, 0.62f);
            weekdayLabel.rectTransform.anchorMin = new Vector2(i / 7f, 1f);
            weekdayLabel.rectTransform.anchorMax = new Vector2((i + 1f) / 7f, 1f);
            weekdayLabel.rectTransform.pivot = new Vector2(0.5f, 1f);
            weekdayLabel.rectTransform.offsetMin = new Vector2(i == 0 ? 12f : 4f, -50f);
            weekdayLabel.rectTransform.offsetMax = new Vector2(i == 6 ? -12f : -4f, -30f);
        }

        for (int i = 0; i < CalendarDayButtonCount; i++)
        {
            int cellIndex = i;
            int row = cellIndex / 7;
            int col = cellIndex % 7;
            float left = col / 7f;
            float right = (col + 1f) / 7f;
            float top = -54f - row * 29f;
            float bottom = top - 24f;
            Button dayButton = CreateButton(calendarPickerPanel, CalendarCellLabel(cellIndex), new Vector2(left, 1f), new Vector2(right, 1f), new Vector2(col == 0 ? 12f : 4f, bottom), new Vector2(col == 6 ? -12f : -4f, top), delegate { SetCalendarDate(CalendarCellDayOffset(cellIndex)); });
            SetButtonTextSize(dayButton, 12);
            scheduleDayButtons[cellIndex] = dayButton;
        }

        Text timeLabel = CreateText(calendarPickerPanel, "시간", 15, TextAnchor.UpperLeft);
        timeLabel.color = new Color(0.82f, 0.9f, 1f, 0.78f);
        timeLabel.rectTransform.anchorMin = new Vector2(0f, 1f);
        timeLabel.rectTransform.anchorMax = new Vector2(1f, 1f);
        timeLabel.rectTransform.pivot = new Vector2(0.5f, 1f);
        timeLabel.rectTransform.anchoredPosition = new Vector2(0f, -226f);
        timeLabel.rectTransform.sizeDelta = new Vector2(-24f, 18f);

        scheduleDayDropdown = CreateCompactDropdown(calendarPickerPanel, "ScheduleDayDropdown", 12, new Vector2(0f, 1f), new Vector2(0.33f, 1f), new Vector2(12f, -262f), new Vector2(-4f, -222f));
        scheduleDayDropdown.gameObject.SetActive(false);
        scheduleHourDropdown = CreateCompactDropdown(calendarPickerPanel, "ScheduleHourDropdown", 15, new Vector2(0f, 1f), new Vector2(0.5f, 1f), new Vector2(12f, -270f), new Vector2(-5f, -242f));
        scheduleMinuteDropdown = CreateCompactDropdown(calendarPickerPanel, "ScheduleMinuteDropdown", 15, new Vector2(0.5f, 1f), new Vector2(1f, 1f), new Vector2(5f, -270f), new Vector2(-12f, -242f));
        ConfigureScheduleDropdown(scheduleHourDropdown);
        ConfigureScheduleDropdown(scheduleMinuteDropdown);
        scheduleDayDropdown.onValueChanged.AddListener(delegate { ApplyScheduleDropdownSelection(); });
        scheduleHourDropdown.onValueChanged.AddListener(delegate { ApplyScheduleDropdownSelection(); });
        scheduleMinuteDropdown.onValueChanged.AddListener(delegate { ApplyScheduleDropdownSelection(); });

        Text presetLabel = CreateText(calendarPickerPanel, "빠른 시간", 14, TextAnchor.UpperLeft);
        presetLabel.color = new Color(0.82f, 0.9f, 1f, 0.68f);
        presetLabel.rectTransform.anchorMin = new Vector2(0f, 1f);
        presetLabel.rectTransform.anchorMax = new Vector2(1f, 1f);
        presetLabel.rectTransform.pivot = new Vector2(0.5f, 1f);
        presetLabel.rectTransform.anchoredPosition = new Vector2(0f, -258f);
        presetLabel.rectTransform.sizeDelta = new Vector2(-24f, 20f);
        presetLabel.gameObject.SetActive(false);

        int[] hours = { 0, 6, 9, 12, 15, 18, 21, 23 };
        for (int i = 0; i < hours.Length; i++)
        {
            int hour = hours[i];
            int row = i / 4;
            int col = i % 4;
            float left = col * 0.25f;
            float right = left + 0.25f;
            float top = -262f - row * 38f;
            float bottom = top - 32f;
            Button hourButton = CreateButton(calendarPickerPanel, hour.ToString("00") + "시", new Vector2(left, 1f), new Vector2(right, 1f), new Vector2(col == 0 ? 12f : 4f, bottom), new Vector2(col == 3 ? -12f : -4f, top), delegate { SetCalendarHour(hour); });
            hourButton.gameObject.SetActive(false);
            scheduleHourButtons[hour] = hourButton;
        }

        Text minuteLabel = CreateText(calendarPickerPanel, "분", 14, TextAnchor.UpperLeft);
        minuteLabel.color = new Color(0.82f, 0.9f, 1f, 0.78f);
        minuteLabel.rectTransform.anchorMin = new Vector2(0f, 0f);
        minuteLabel.rectTransform.anchorMax = new Vector2(0.16f, 0f);
        minuteLabel.rectTransform.offsetMin = new Vector2(12f, 74f);
        minuteLabel.rectTransform.offsetMax = new Vector2(0f, 96f);
        minuteLabel.gameObject.SetActive(false);

        int[] minutes = { 0, 15, 30, 45 };
        for (int i = 0; i < minutes.Length; i++)
        {
            int minute = minutes[i];
            float left = 0.18f + i * 0.205f;
            float right = left + 0.185f;
            Button minuteButton = CreateButton(calendarPickerPanel, minute.ToString("00") + "분", new Vector2(left, 0f), new Vector2(right, 0f), new Vector2(0f, 72f), new Vector2(0f, 106f), delegate { SetCalendarMinute(minute); });
            minuteButton.gameObject.SetActive(false);
            scheduleMinuteButtons[minute] = minuteButton;
        }

        CreateButton(calendarPickerPanel, "적용", new Vector2(0f, 0f), new Vector2(0.48f, 0f), new Vector2(12f, 8f), new Vector2(-6f, 42f), delegate { ApplyScheduleDropdownSelection(); ToggleCalendarPicker(false); });
        CreateButton(calendarPickerPanel, "닫기", new Vector2(0.5f, 0f), new Vector2(1f, 0f), new Vector2(6f, 8f), new Vector2(-12f, 42f), delegate { ToggleCalendarPicker(false); });
        PopulateScheduleDropdowns();
        calendarPickerPanel.gameObject.SetActive(false);
    }

    private void ConfigureScheduleDropdown(Dropdown dropdown)
    {
        if (dropdown == null || dropdown.template == null)
        {
            return;
        }

        dropdown.template.sizeDelta = new Vector2(dropdown.template.sizeDelta.x, 170f);
        ScrollRect scrollRect = dropdown.template.GetComponent<ScrollRect>();
        if (scrollRect != null)
        {
            scrollRect.scrollSensitivity = 110f;
        }
    }

    private void BuildMessageStep(RectTransform panel)
    {
        Text inputModeHint = CreateText(panel, "한/영 입력을 직접 전환할 수 있습니다.", 13, TextAnchor.UpperLeft);
        inputModeHint.color = new Color(0.82f, 0.94f, 1f, 0.72f);
        inputModeHint.rectTransform.anchorMin = new Vector2(0f, 1f);
        inputModeHint.rectTransform.anchorMax = new Vector2(0.62f, 1f);
        inputModeHint.rectTransform.pivot = new Vector2(0.5f, 1f);
        inputModeHint.rectTransform.anchoredPosition = new Vector2(0f, -18f);
        inputModeHint.rectTransform.sizeDelta = new Vector2(-28f, 24f);
        messageInputModeButton = CreateButton(panel, "입력: 한글", new Vector2(0.64f, 1f), new Vector2(1f, 1f), new Vector2(4f, -50f), new Vector2(-18f, -14f), delegate { ToggleMessageInputMode(); });
        SetButtonTextSize(messageInputModeButton, 13);

        messageInputField = CreateInputField(panel, "MessageInput", "내용 입력", new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(18f, -112f), new Vector2(-18f, -58f));
        messageInputField.onValueChanged.AddListener(delegate
        {
            if (composingQueuedMessageInFlow)
            {
                RefreshAll();
                return;
            }
            missionManager.SetMessageText(messageInputField.text);
        });
        messageInputBox = messageInputField.GetComponent<Image>();

        payloadWarningText = CreateText(panel, string.Empty, 14, TextAnchor.UpperLeft);
        payloadWarningText.rectTransform.anchorMin = new Vector2(0f, 1f);
        payloadWarningText.rectTransform.anchorMax = new Vector2(1f, 1f);
        payloadWarningText.rectTransform.pivot = new Vector2(0.5f, 1f);
        payloadWarningText.rectTransform.anchoredPosition = new Vector2(0f, -124f);
        payloadWarningText.rectTransform.sizeDelta = new Vector2(-28f, 58f);
        payloadFill = null;

        attachmentPickerButton = CreateButton(panel, "파일 첨부", new Vector2(0f, 1f), new Vector2(0.48f, 1f), new Vector2(18f, -224f), new Vector2(-6f, -188f), delegate { OpenNativeFilePicker(false); });
        attachmentRemoveFirstButton = CreateButton(panel, "파일 제거", new Vector2(0.5f, 1f), new Vector2(1f, 1f), new Vector2(6f, -224f), new Vector2(-18f, -188f), delegate { RemoveFirstAttachment(); });
        SetButtonTextSize(attachmentPickerButton, 14);
        SetButtonTextSize(attachmentRemoveFirstButton, 14);

        attachmentStatusText = CreateText(panel, string.Empty, 13, TextAnchor.UpperLeft);
        attachmentStatusText.color = new Color(0.82f, 0.9f, 1f, 0.78f);
        attachmentStatusText.rectTransform.anchorMin = new Vector2(0f, 1f);
        attachmentStatusText.rectTransform.anchorMax = new Vector2(1f, 1f);
        attachmentStatusText.rectTransform.pivot = new Vector2(0.5f, 1f);
        attachmentStatusText.rectTransform.anchoredPosition = new Vector2(0f, -238f);
        attachmentStatusText.rectTransform.sizeDelta = new Vector2(-28f, 22f);
        attachmentStatusText.gameObject.SetActive(false);

        attachmentPreviewText = CreateText(panel, string.Empty, 12, TextAnchor.UpperLeft);
        attachmentPreviewText.rectTransform.anchorMin = new Vector2(0f, 1f);
        attachmentPreviewText.rectTransform.anchorMax = new Vector2(1f, 1f);
        attachmentPreviewText.rectTransform.pivot = new Vector2(0.5f, 1f);
        attachmentPreviewText.rectTransform.anchoredPosition = new Vector2(0f, -238f);
        attachmentPreviewText.rectTransform.sizeDelta = new Vector2(-28f, 38f);
        attachmentPreviewText.color = new Color(0.82f, 0.94f, 1f, 0.7f);
        CreateAttachmentDeleteButtons(panel);

        attachmentPickerPanel = CreatePanel(panel, "AttachmentPicker", new Vector2(0f, 1f), new Vector2(1f, 1f), new Color(0.045f, 0.075f, 0.115f, 0.98f));
        attachmentPickerPanel.offsetMin = new Vector2(18f, -344f);
        attachmentPickerPanel.offsetMax = new Vector2(-18f, -228f);
        MakeDraggable(attachmentPickerPanel, requestFlowPanel);

        attachmentPathInputField = null;
        Text attachmentPickerHint = CreateText(attachmentPickerPanel, "파일 찾기로 첨부할 파일을 선택하세요.", 14, TextAnchor.MiddleLeft);
        attachmentPickerHint.color = new Color(0.82f, 0.94f, 1f, 0.78f);
        attachmentPickerHint.rectTransform.anchorMin = new Vector2(0f, 1f);
        attachmentPickerHint.rectTransform.anchorMax = new Vector2(1f, 1f);
        attachmentPickerHint.rectTransform.offsetMin = new Vector2(14f, -58f);
        attachmentPickerHint.rectTransform.offsetMax = new Vector2(-14f, -18f);
        CreateButton(attachmentPickerPanel, "파일 찾기", new Vector2(0f, 0f), new Vector2(0.64f, 0f), new Vector2(12f, 12f), new Vector2(-4f, 48f), delegate { OpenNativeFilePicker(false); });
        CreateButton(attachmentPickerPanel, "닫기", new Vector2(0.66f, 0f), new Vector2(1f, 0f), new Vector2(4f, 12f), new Vector2(-12f, 48f), delegate { ToggleAttachmentPicker(false); });
        attachmentPickerPanel.gameObject.SetActive(false);
    }

    private void ToggleMessageInputMode()
    {
        messageKoreanInputMode = !messageKoreanInputMode;
        ApplyMessageInputMode();
        SaveUiPreferences();
        RestoreMessageInputFocus();
        ShowToast(messageKoreanInputMode ? "한글 입력 모드입니다." : "영문 입력 모드입니다.");
    }

    private void RefreshMessageInputModeButton()
    {
        ApplyMessageInputMode();
        if (messageInputModeButton == null)
        {
            return;
        }
        bool canChange = missionManager == null || missionManager.Phase == MissionPhase.Idle || composingQueuedMessageInFlow;
        messageInputModeButton.interactable = canChange;
        SetButtonLabel(messageInputModeButton, messageKoreanInputMode ? "입력: 한글" : "입력: English");
        SetButtonVisual(messageInputModeButton, !canChange ? ButtonVisual.Disabled : messageKoreanInputMode ? ButtonVisual.Selected : ButtonVisual.Secondary);
    }

    private void ApplyMessageInputMode()
    {
        if (messageInputField == null)
        {
            return;
        }

        LegacyInputFieldImeSupport imeSupport = messageInputField.GetComponent<LegacyInputFieldImeSupport>();
        if (imeSupport != null)
        {
            imeSupport.ResetRawFallbackState();
            imeSupport.ForceDubeolsikFallback = messageKoreanInputMode;
        }
    }

    private void RestoreMessageInputFocus()
    {
        if (messageInputField == null || currentFlowStep != 2 || missionManager == null)
        {
            return;
        }
        if (missionManager.Phase != MissionPhase.Idle && !composingQueuedMessageInFlow)
        {
            return;
        }

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(messageInputField.gameObject);
        }
        messageInputField.Select();
        messageInputField.ActivateInputField();
    }

    private void BuildDestinationStep(RectTransform panel)
    {
        originPickButton = CreateButton(panel, "송신지", new Vector2(0f, 1f), new Vector2(0.48f, 1f), new Vector2(18f, -42f), new Vector2(-6f, -6f), delegate
        {
            selectingOriginInDestinationStep = true;
            RefreshDestinationButtons();
        });
        destinationPickButton = CreateButton(panel, "수신지", new Vector2(0.5f, 1f), new Vector2(1f, 1f), new Vector2(6f, -42f), new Vector2(-18f, -6f), delegate
        {
            selectingOriginInDestinationStep = false;
            RefreshDestinationButtons();
        });

        destinationSearchInputField = CreateInputField(panel, "DestinationSearch", "도시 검색", new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(18f, -92f), new Vector2(-18f, -56f));
        destinationSearchInputField.onValueChanged.AddListener(delegate
        {
            destinationSearchQuery = destinationSearchInputField.text ?? string.Empty;
            RefreshDestinationButtons();
        });

        List<ObservationTarget> targets = missionManager != null ? missionManager.Targets : null;
        if (targets != null && targets.Count > 0)
        {
            for (int i = 0; i < targets.Count; i++)
            {
                CreateDestinationGridButton(panel, targets[i].targetName, DestinationLabel(targets[i].targetName), i);
            }
        }
        else
        {
            for (int i = 0; i < OrbitShareDestinations.Names.Length; i++)
            {
                string targetName = OrbitShareDestinations.Names[i];
                CreateDestinationGridButton(panel, targetName, DestinationLabel(targetName), i);
            }
        }

        favoriteDestinationButton = null;
        destinationSummaryText = CreateText(panel, string.Empty, 13, TextAnchor.UpperLeft);
        destinationSummaryText.color = new Color(0.82f, 0.94f, 1f, 0.78f);
        destinationSummaryText.rectTransform.anchorMin = new Vector2(0f, 1f);
        destinationSummaryText.rectTransform.anchorMax = new Vector2(1f, 1f);
        destinationSummaryText.rectTransform.pivot = new Vector2(0.5f, 1f);
        destinationSummaryText.rectTransform.anchoredPosition = new Vector2(0f, -318f);
        destinationSummaryText.rectTransform.sizeDelta = new Vector2(-28f, 72f);
        destinationSummaryText.gameObject.SetActive(false);
    }

    private void BuildSatelliteStep(RectTransform panel)
    {
        Text instruction = CreateText(panel, "업링크/ISL/다운링크에 사용할 위성망 추천을 확인하세요.", 15, TextAnchor.UpperLeft);
        instruction.color = new Color(0.88f, 0.96f, 1f, 0.86f);
        instruction.rectTransform.anchorMin = new Vector2(0f, 1f);
        instruction.rectTransform.anchorMax = new Vector2(1f, 1f);
        instruction.rectTransform.pivot = new Vector2(0.5f, 1f);
        instruction.rectTransform.anchoredPosition = new Vector2(0f, -8f);
        instruction.rectTransform.sizeDelta = new Vector2(-28f, 74f);
        instruction.gameObject.SetActive(false);

        satelliteAvailableOnlyButton = null;
        satelliteRecommendedTimeButton = CreateButton(panel, "추천 시간", new Vector2(0.34f, 1f), new Vector2(0.66f, 1f), new Vector2(4f, -42f), new Vector2(-4f, -6f), delegate { ApplyRecommendedDelay(); });
        failureRetryApplyButton = CreateButton(panel, "추천 재시도", new Vector2(0.68f, 1f), new Vector2(1f, 1f), new Vector2(4f, -42f), new Vector2(-18f, -6f), delegate { ApplyFailureRetrySuggestion(); });
        satelliteRecommendedTimeButton.gameObject.SetActive(false);
        failureRetryApplyButton.gameObject.SetActive(false);
        satelliteFavoriteButton = CreateButton(panel, "☆ 추천 즐겨찾기", new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(18f, -46f), new Vector2(-18f, -10f), delegate { ToggleFavoriteSatellite(); });
        satelliteFavoriteButton.gameObject.SetActive(false);

        detailText = CreateText(panel, string.Empty, 15, TextAnchor.UpperLeft);
        detailText.lineSpacing = 1.1f;
        detailText.rectTransform.offsetMin = new Vector2(18f, 18f);
        detailText.rectTransform.offsetMax = new Vector2(-18f, -60f);

        satelliteCompareText = CreateText(panel, string.Empty, 13, TextAnchor.UpperLeft);
        satelliteCompareText.lineSpacing = 1.02f;
        satelliteCompareText.rectTransform.anchorMin = new Vector2(0f, 0f);
        satelliteCompareText.rectTransform.anchorMax = new Vector2(1f, 0f);
        satelliteCompareText.rectTransform.offsetMin = new Vector2(18f, 18f);
        satelliteCompareText.rectTransform.offsetMax = new Vector2(-18f, 112f);
        satelliteCompareText.color = new Color(0.82f, 0.94f, 1f, 0.82f);
        satelliteCompareText.gameObject.SetActive(false);
    }

    private void BuildConfirmStep(RectTransform panel)
    {
        GameObject viewportObject = new GameObject("ConfirmSummaryViewport", typeof(RectTransform), typeof(Image), typeof(Mask), typeof(ScrollRect));
        viewportObject.transform.SetParent(panel, false);
        RectTransform viewportRect = viewportObject.GetComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.offsetMin = new Vector2(18f, 48f);
        viewportRect.offsetMax = new Vector2(-18f, -12f);
        viewportObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.035f);
        viewportObject.GetComponent<Mask>().showMaskGraphic = false;

        GameObject contentObject = new GameObject("ConfirmSummaryContent", typeof(RectTransform));
        contentObject.transform.SetParent(viewportObject.transform, false);
        confirmSummaryContent = contentObject.GetComponent<RectTransform>();
        confirmSummaryContent.anchorMin = new Vector2(0f, 1f);
        confirmSummaryContent.anchorMax = new Vector2(1f, 1f);
        confirmSummaryContent.pivot = new Vector2(0.5f, 1f);
        confirmSummaryContent.anchoredPosition = Vector2.zero;
        confirmSummaryContent.sizeDelta = new Vector2(0f, 180f);

        confirmSummaryScrollRect = viewportObject.GetComponent<ScrollRect>();
        confirmSummaryScrollRect.content = confirmSummaryContent;
        confirmSummaryScrollRect.viewport = viewportRect;
        confirmSummaryScrollRect.horizontal = false;
        confirmSummaryScrollRect.movementType = ScrollRect.MovementType.Clamped;
        confirmSummaryScrollRect.scrollSensitivity = 56f;

        reservationHintText = CreateText(contentObject.transform, string.Empty, 14, TextAnchor.UpperLeft);
        reservationHintText.rectTransform.anchorMin = Vector2.zero;
        reservationHintText.rectTransform.anchorMax = Vector2.one;
        reservationHintText.rectTransform.offsetMin = new Vector2(10f, 8f);
        reservationHintText.rectTransform.offsetMax = new Vector2(-10f, -8f);
        reservationHintText.color = new Color(0.9f, 0.96f, 1f, 0.92f);
        reservationHintText.fontSize = 14;
        reservationHintText.lineSpacing = 1.0f;
        reservationHintText.verticalOverflow = VerticalWrapMode.Overflow;

        confirmEditTimeButton = CreateConfirmEditButton(panel, "시간 수정", 0, delegate { GoToFlowStep(1); });
        confirmEditMessageButton = CreateConfirmEditButton(panel, "메시지 수정", 1, delegate { GoToFlowStep(2); });
        confirmEditDestinationButton = CreateConfirmEditButton(panel, "위치 수정", 2, delegate { GoToFlowStep(3); });
        confirmEditSatelliteButton = CreateConfirmEditButton(panel, "위성망 수정", 3, delegate { GoToFlowStep(4); });

        resetButton = CreateButton(panel, "전송 취소", new Vector2(0f, 0f), new Vector2(0.48f, 0f), new Vector2(18f, FlowBottomActionMinY), new Vector2(-6f, FlowBottomActionMaxY), delegate { HandleConfirmSecondaryButton(); });
        startMissionButton = CreateButton(panel, "전송 시작", new Vector2(0.5f, 0f), new Vector2(1f, 0f), new Vector2(6f, FlowBottomActionMinY), new Vector2(-18f, FlowBottomActionMaxY), delegate { StartSelectedMission(); });
        SetButtonTextSize(startMissionButton, 15);
        SetButtonTextSize(resetButton, 15);
    }

    private void CreateReservationPanel(Transform parent)
    {
        RectTransform panel = CreatePanel(parent, "MissionReservationPanel", new Vector2(0.69f, 0.08f), new Vector2(0.985f, 0.40f), new Color(0.025f, 0.045f, 0.075f, 0.94f));
        Text heading = CreateText(panel, string.Empty, 22, TextAnchor.UpperLeft);
        heading.gameObject.SetActive(false);
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

        reserveButton = CreateButton(panel, "위성 선택 완료", new Vector2(0f, 0f), new Vector2(0.32f, 0f), new Vector2(18f, 18f), new Vector2(-6f, 64f), delegate { missionManager.ReserveMission(); });
        startMissionButton = CreateButton(panel, "전송 시작", new Vector2(0.34f, 0f), new Vector2(0.66f, 0f), new Vector2(0f, 18f), new Vector2(0f, 64f), delegate { missionManager.StartMission(); });
        resetButton = CreateButton(panel, "전송 취소", new Vector2(0.68f, 0f), new Vector2(1f, 0f), new Vector2(6f, 18f), new Vector2(-18f, 64f), delegate { missionManager.ResetMission(); });
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

    private void ShowRequestFlowPanel()
    {
        bool wasDismissed = requestFlowDismissed;
        requestFlowDismissed = false;
        if (missionManager != null && missionManager.Phase == MissionPhase.Complete)
        {
            ResetFlowAndMission();
        }
        EnsureDefaultFlowSelections();
        if (missionManager != null && IsActiveTransferPhase(missionManager.Phase) && !composingQueuedMessageInFlow)
        {
            BeginQueuedMessageFlow();
            return;
        }
        if (requestFlowPanel != null)
        {
            requestFlowPanel.gameObject.SetActive(true);
            requestFlowPanel.SetAsLastSibling();
        }
        if (wasDismissed)
        {
            ShowToast("메시지 작성 창을 열었습니다.");
        }
        RefreshAllPreservingLauncherPanels(requestFlowPanel);
    }

    private void ToggleRequestFlowPanel()
    {
        if (requestFlowPanel == null)
        {
            return;
        }

        if (requestFlowPanel.gameObject.activeSelf && !requestFlowDismissed)
        {
            requestFlowDismissed = true;
            requestFlowPanel.gameObject.SetActive(false);
            RefreshPanelLauncherButtons();
            return;
        }

        ShowRequestFlowPanel();
    }

    private void CancelRequestFlow()
    {
        UnityAction cancelAction = delegate
        {
            ResetFlowAndMission();
            requestFlowDismissed = true;
            if (requestFlowPanel != null)
            {
                requestFlowPanel.gameObject.SetActive(false);
            }
            RefreshPanelLauncherButtons();
        };

        if (composingQueuedMessageInFlow)
        {
            CancelQueuedMessageFlow();
            return;
        }

        bool hasDraftInput = missionManager != null
            && (missionManager.HasExplicitOriginSelection
                || missionManager.HasExplicitDestinationSelection
                || !string.IsNullOrWhiteSpace(missionManager.MessageText)
                || missionManager.AttachmentCount > 0
                || flowScheduleSelected);
        if (hasDraftInput)
        {
            ShowConfirm("작성 중인 전송을 취소할까요?", cancelAction);
            return;
        }

        cancelAction.Invoke();
    }

    private void ShowAvailableSatellitesOnly()
    {
        satelliteFilter = SatelliteListFilter.All;
        satelliteListDismissed = false;
        RefreshSatelliteList(true, true, true);
        ShowToast("위성망 추천을 최신 순서로 정렬했습니다.");
    }

    private void ApplyRecommendedDelay()
    {
        if (missionManager == null || (missionManager.Phase != MissionPhase.Idle && !composingQueuedMessageInFlow))
        {
            return;
        }

        string targetName = composingQueuedMessageInFlow ? queueTargetName : missionManager.SelectedTarget != null ? missionManager.SelectedTarget.targetName : OrbitShareDestinations.DefaultTargetName;
        int recommendedDelay = RecommendedDelayMinutes(targetName);
        if (composingQueuedMessageInFlow)
        {
            SetQueuedFlowScheduleFromDelay(recommendedDelay);
        }
        else
        {
            flowScheduleSelected = true;
            missionManager.SetSendDelayMinutes(recommendedDelay);
        }
        ToggleCalendarPicker(false);
        ShowToast("추천 시간 " + DelayLabel(recommendedDelay) + "을 적용했습니다.");
        RefreshAll();
    }

    private void BeginQueuedMessageFlow()
    {
        if (missionManager == null)
        {
            return;
        }

        composingQueuedMessageInFlow = true;
        requestFlowDismissed = false;
        queueEditingIndex = -1;
        queueOriginName = missionManager.OriginName;
        queueTargetName = missionManager.SelectedTarget != null ? missionManager.SelectedTarget.targetName : OrbitShareDestinations.DefaultTargetName;
        queueSelectedSatelliteName = string.Empty;
        queueFlowOriginSelected = !string.IsNullOrEmpty(queueOriginName);
        queueFlowTargetSelected = !string.IsNullOrEmpty(queueTargetName);
        queueAttachments.Clear();
        SyncQueueAttachmentLegacyFields();
        SetQueuedFlowScheduleFromDelay(0);
        queueScheduleOverride = true;
        if (messageInputField != null)
        {
            messageInputField.SetTextWithoutNotify(string.Empty);
        }
        if (queueMessagePanel != null)
        {
            queueMessagePanel.gameObject.SetActive(false);
        }
        ToggleAttachmentPicker(false);
        currentFlowStep = 1;
        if (requestFlowPanel != null)
        {
            requestFlowPanel.gameObject.SetActive(true);
            requestFlowPanel.SetAsLastSibling();
        }
        ShowToast("현재 전송 뒤 이어서 보낼 메시지를 작성합니다.");
        RefreshAll();
    }

    private void BeginQueuedMessageFlowFromRecord(SentMessageRecord record)
    {
        if (record == null)
        {
            BeginQueuedMessageFlow();
            return;
        }

        BeginQueuedMessageFlow();
        queueOriginName = string.IsNullOrEmpty(record.originName) ? OrbitShareDestinations.Seoul : record.originName;
        queueTargetName = string.IsNullOrEmpty(record.targetName) ? OrbitShareDestinations.DefaultTargetName : record.targetName;
        queueSelectedSatelliteName = string.Empty;
        queueFlowOriginSelected = true;
        queueFlowTargetSelected = true;
        queueAttachments.Clear();
        List<MessageAttachmentRecord> attachments = AttachmentsOrLegacy(record.attachments, record.attachmentName, record.attachmentSizeKb);
        for (int i = 0; i < attachments.Count; i++)
        {
            queueAttachments.Add(attachments[i]);
        }
        SyncQueueAttachmentLegacyFields();
        SetQueuedFlowScheduleFromDelay(0);
        queueScheduleOverride = true;
        if (messageInputField != null)
        {
            messageInputField.SetTextWithoutNotify(record.messageText ?? string.Empty);
        }
        currentFlowStep = 5;
        ShowToast("기록을 수정해 추가 메시지로 작성합니다.");
        RefreshAll();
    }

    private void CloseRequestFlowPanel()
    {
        requestFlowDismissed = true;
        if (requestFlowPanel != null)
        {
            requestFlowPanel.gameObject.SetActive(false);
        }
        RefreshPanelLauncherButtons();
        ShowToast(composingQueuedMessageInFlow ? "추가 메시지 작성 창을 숨겼습니다." : "메시지 작성 창을 숨겼습니다.");
    }

    private bool QueuedFlowHasDraftInput()
    {
        return composingQueuedMessageInFlow
            && (!string.IsNullOrWhiteSpace(QueuedFlowMessageText())
                || queueAttachments.Count > 0
                || queueFlowOriginSelected
                || queueFlowTargetSelected
                || queueScheduleOverride
                || !string.IsNullOrEmpty(queueSelectedSatelliteName)
                || queueEditingIndex >= 0);
    }

    private void CancelQueuedMessageFlow()
    {
        if (QueuedFlowHasDraftInput())
        {
            ShowConfirm("추가 메시지 작성을 취소할까요?", delegate { CancelQueuedMessageFlowNow(); });
            return;
        }

        CancelQueuedMessageFlowNow();
    }

    private void CancelQueuedMessageFlowNow()
    {
        composingQueuedMessageInFlow = false;
        queueEditingIndex = -1;
        queueSelectedSatelliteName = string.Empty;
        queueFlowOriginSelected = false;
        queueFlowTargetSelected = false;
        queueAttachments.Clear();
        SyncQueueAttachmentLegacyFields();
        queueScheduleOverride = false;
        currentFlowStep = 5;
        if (messageInputField != null)
        {
            messageInputField.SetTextWithoutNotify(missionManager != null ? missionManager.MessageText ?? string.Empty : string.Empty);
        }
        ShowToast("추가 메시지 작성을 취소했습니다.");
        RefreshAll();
    }

    private void ConfirmResetFlowAndMission()
    {
        if (missionManager == null)
        {
            ResetFlowAndMission();
            return;
        }
        if (missionManager.Phase == MissionPhase.Complete)
        {
            ResetFlowAndMission();
            return;
        }
        if (missionManager.Phase == MissionPhase.Idle && string.IsNullOrWhiteSpace(missionManager.MessageText) && missionManager.AttachmentSizeKb == 0 && currentFlowStep <= 1)
        {
            ResetFlowAndMission();
            return;
        }
        ShowConfirm("작성 중인 전송을 취소하고 입력 내용을 지울까요?", delegate { ResetFlowAndMission(); });
    }

    private void OpenNativeFilePicker(bool queue)
    {
#if UNITY_EDITOR
        string path = UnityEditor.EditorUtility.OpenFilePanel("파일 선택", "", "");
        if (!string.IsNullOrEmpty(path))
        {
            SetAttachmentFromFilePath(path, queue);
            return;
        }
#else
        string path = TryOpenRuntimeFileDialog();
        if (!string.IsNullOrEmpty(path))
        {
            SetAttachmentFromFilePath(path, queue);
            return;
        }
#endif
        ShowToast("파일을 선택하지 않았습니다.");
    }

    private string TryOpenRuntimeFileDialog()
    {
        string path = TryRunFilePicker("zenity", "--file-selection");
        if (!string.IsNullOrEmpty(path))
        {
            return path;
        }
        return TryRunFilePicker("kdialog", "--getopenfilename .");
    }

    private string TryRunFilePicker(string command, string arguments)
    {
        try
        {
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo(command, arguments);
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.CreateNoWindow = true;
            using (System.Diagnostics.Process process = System.Diagnostics.Process.Start(startInfo))
            {
                if (process == null)
                {
                    return string.Empty;
                }
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit(15000);
                if (process.ExitCode == 0)
                {
                    return output.Trim();
                }
            }
        }
        catch
        {
        }
        return string.Empty;
    }

    private void SetAttachmentFromFilePath(string path, bool queue)
    {
        path = NormalizeAttachmentPath(path);
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
        {
            ShowToast("파일을 찾을 수 없습니다.");
            return;
        }
        if (!queue && !composingQueuedMessageInFlow && (missionManager == null || missionManager.Phase != MissionPhase.Idle))
        {
            ShowToast("전송 중에는 파일을 바꿀 수 없습니다.");
            return;
        }

        FileInfo info = new FileInfo(path);
        int sizeKb = Mathf.Max(1, Mathf.CeilToInt(info.Length / 1024f));
        long sizeBytes = info.Length;
        string attachmentError;
        if (!CanAcceptAttachment(queue, info.Name, sizeKb, sizeBytes, out attachmentError))
        {
            ShowToast(attachmentError);
            RestoreMessageInputFocus();
            return;
        }
        if (queue)
        {
            AddQueueAttachment(info.Name, sizeKb, sizeBytes);
            ClearAttachmentPathInput(true);
            ToggleQueueAttachmentPicker(false);
            RefreshQueuePanel();
        }
        else if (composingQueuedMessageInFlow)
        {
            AddQueueAttachment(info.Name, sizeKb, sizeBytes);
            ClearAttachmentPathInput(false);
            ToggleAttachmentPicker(false);
            RefreshAll();
        }
        else if (missionManager != null)
        {
            missionManager.AddAttachment(info.Name, sizeKb, sizeBytes);
            ClearAttachmentPathInput(false);
            ToggleAttachmentPicker(false);
        }
        ShowToast(info.Name + " 첨부됨 · " + FormatBytes(info.Length));
        RestoreMessageInputFocus();
    }

    private void ClearAttachmentPathInput(bool queue)
    {
        InputField source = queue ? queueAttachmentPathInputField : attachmentPathInputField;
        if (source != null)
        {
            source.SetTextWithoutNotify(string.Empty);
        }
    }

    private bool CanAcceptAttachment(bool queuePanelAttachment, string fileName, int sizeKb, long sizeBytes, out string error)
    {
        error = string.Empty;
        if (missionManager == null)
        {
            return true;
        }

        bool useQueueDraft = queuePanelAttachment || composingQueuedMessageInFlow;
        List<MessageAttachmentRecord> attachments = new List<MessageAttachmentRecord>();
        if (useQueueDraft)
        {
            for (int i = 0; i < queueAttachments.Count; i++)
            {
                if (queueAttachments[i] != null)
                {
                    attachments.Add(new MessageAttachmentRecord(queueAttachments[i].fileName, queueAttachments[i].sizeKb, queueAttachments[i].sizeBytes));
                }
            }
        }
        else
        {
            for (int i = 0; i < missionManager.AttachmentFiles.Count; i++)
            {
                MessageAttachmentRecord attachment = missionManager.AttachmentFiles[i];
                if (attachment != null)
                {
                    attachments.Add(new MessageAttachmentRecord(attachment.fileName, attachment.sizeKb, attachment.sizeBytes));
                }
            }
        }
        attachments.Add(new MessageAttachmentRecord(fileName, sizeKb, sizeBytes));

        string message = useQueueDraft
            ? queuePanelAttachment && queueMessageInputField != null ? queueMessageInputField.text : QueuedFlowMessageText()
            : missionManager.MessageText;
        int nextPayloadKb = missionManager.CalculateMessageSizeKb(message, attachments);
        if (nextPayloadKb <= missionManager.MaxPayloadKb)
        {
            return true;
        }

        error = "첨부 후 크기 초과: " + PayloadKbText(nextPayloadKb) + " / " + PayloadKbText(missionManager.MaxPayloadKb);
        return false;
    }

    private string NormalizeAttachmentPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return string.Empty;
        }

        string normalized = path.Trim().Trim('"', '\'');
        if (normalized.StartsWith("~/"))
        {
            string home = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
            if (!string.IsNullOrEmpty(home))
            {
                normalized = Path.Combine(home, normalized.Substring(2));
            }
        }
        return normalized;
    }

    private void RefreshFlowControls()
    {
        if (missionManager == null)
        {
            return;
        }

        if (missionManager.Phase != MissionPhase.Idle && !composingQueuedMessageInFlow)
        {
            currentFlowStep = 5;
        }

        SetStepActive(scheduleStepPanel, currentFlowStep == 1);
        SetStepActive(messageStepPanel, currentFlowStep == 2);
        SetStepActive(destinationStepPanel, currentFlowStep == 3);
        SetStepActive(satelliteStepPanel, currentFlowStep == 4);
        SetStepActive(confirmStepPanel, currentFlowStep == 5);

        bool isIdle = missionManager.Phase == MissionPhase.Idle;
        bool canUseFlow = isIdle || composingQueuedMessageInFlow;
        bool isComplete = missionManager.Phase == MissionPhase.Complete;
        if (requestFlowPanel != null)
        {
            requestFlowPanel.gameObject.SetActive(!isComplete && !requestFlowDismissed);
        }
        if (satelliteListPanel != null)
        {
            bool keepOpen = satelliteListPanel.gameObject.activeSelf && !satelliteListDismissed;
            bool shouldOpen = !satelliteListDismissed && (((isIdle || composingQueuedMessageInFlow) && currentFlowStep == 4) || (!isIdle && !composingQueuedMessageInFlow && keepOpen));
            if (satelliteListPanel.gameObject.activeSelf != shouldOpen)
            {
                SetSatellitePanelOpen(shouldOpen, satelliteListDismissed);
            }
        }
        if (missionStatusPanel != null)
        {
            bool keepOpen = missionStatusPanel.gameObject.activeSelf && !missionStatusDismissed;
            missionStatusPanel.gameObject.SetActive(!missionStatusDismissed && (!isIdle || keepOpen));
        }
        if (timelinePanel != null)
        {
            bool keepOpen = timelinePanel.gameObject.activeSelf && !timelineDismissed;
            timelinePanel.gameObject.SetActive(!timelineDismissed && (!isIdle || keepOpen));
        }

        RefreshFlowProgressButtons();
        RefreshConfirmEditButtons();

        if (flowStepTitleText != null)
        {
            flowStepTitleText.text = string.Empty;
            flowStepTitleText.gameObject.SetActive(false);
        }
        if (flowStepHintText != null)
        {
            flowStepHintText.text = string.Empty;
        }
        if (flowInstructionText != null)
        {
            flowInstructionText.text = string.Empty;
        }

        if (flowBackButton != null)
        {
            bool canGoBack = canUseFlow && (currentFlowStep == 1 || (currentFlowStep > 1 && currentFlowStep < 5));
            flowBackButton.gameObject.SetActive(canGoBack);
            flowBackButton.interactable = canGoBack;
            SetButtonLabel(flowBackButton, currentFlowStep == 1 ? "취소" : "← 이전 단계");
            SetButtonVisual(flowBackButton, canGoBack ? ButtonVisual.Secondary : ButtonVisual.Disabled);
        }
        if (flowNextButton != null)
        {
            bool canGoNext = canUseFlow && currentFlowStep < 5 && CanAdvanceFlowStep();
            flowNextButton.gameObject.SetActive(currentFlowStep < 5);
            flowNextButton.interactable = canGoNext;
            SetButtonLabel(flowNextButton, NextFlowButtonLabel());
            SetButtonVisual(flowNextButton, canGoNext ? ButtonVisual.Primary : ButtonVisual.Disabled);
        }
    }

    private bool FlowHasScheduleSelection()
    {
        return composingQueuedMessageInFlow ? queueScheduleOverride : flowScheduleSelected;
    }

    private void RefreshLiveConfirmTime()
    {
        if (missionManager == null)
        {
            return;
        }

        bool statusNeedsRefresh = missionStatusPanel != null
            && missionStatusPanel.gameObject.activeSelf
            && (IsActiveTransferPhase(missionManager.Phase) || missionManager.Phase == MissionPhase.Failed);
        bool confirmNeedsRefresh = requestFlowPanel != null
            && requestFlowPanel.gameObject.activeSelf
            && currentFlowStep == 5
            && missionManager.Phase == MissionPhase.Idle
            && missionManager.RequestedSendDelayMinutes == 0;

        if ((statusNeedsRefresh || confirmNeedsRefresh) && Time.unscaledTime >= nextLiveUiRefreshTime)
        {
            nextLiveUiRefreshTime = Time.unscaledTime + 0.12f;
            RefreshMissionPanel();
        }

        // 위성망 추천 live 갱신은 UIManager.SatellitePanel의
        // RefreshLiveSatelliteRouteTimes 한 경로에서만 처리한다.
    }

    private void SetStepActive(RectTransform panel, bool active)
    {
        if (panel != null)
        {
            panel.gameObject.SetActive(active);
        }
    }

    private void RefreshFlowProgressButtons()
    {
        bool activeTransfer = missionManager != null && IsActiveTransferPhase(missionManager.Phase) && !composingQueuedMessageInFlow;
        for (int i = 0; i < flowStepButtons.Count; i++)
        {
            int step = i + 1;
            Button button = flowStepButtons[i];
            if (activeTransfer)
            {
                button.interactable = false;
                SetButtonLabel(button, step == 5 ? "현재 전송" : FlowStepProgressLabel(step));
                SetButtonVisual(button, step == 5 ? ButtonVisual.Selected : ButtonVisual.Disabled);
                continue;
            }
            bool active = step == currentFlowStep;
            bool complete = step < currentFlowStep;
            bool reachable = CanReachFlowStep(step);
            button.interactable = reachable;
            SetButtonLabel(button, FlowStepProgressLabel(step));
            SetButtonVisual(button, active ? ButtonVisual.Selected : reachable ? ButtonVisual.Secondary : ButtonVisual.Disabled);
        }
    }

    private void RefreshConfirmEditButtons()
    {
        Button[] buttons = { confirmEditTimeButton, confirmEditMessageButton, confirmEditDestinationButton, confirmEditSatelliteButton };
        bool canEdit = missionManager != null && (missionManager.Phase == MissionPhase.Idle || composingQueuedMessageInFlow) && currentFlowStep == 5;
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == null)
            {
                continue;
            }
            buttons[i].gameObject.SetActive(canEdit);
            buttons[i].interactable = canEdit;
            SetButtonVisual(buttons[i], canEdit ? ButtonVisual.Secondary : ButtonVisual.Disabled);
        }
    }

    private string FlowStepProgressLabel(int step)
    {
        switch (step)
        {
            case 1:
                return "시간";
            case 2:
                return "메시지";
            case 3:
                return "위치";
            case 4:
                return "위성망";
            default:
                return "확인";
        }
    }

    private string NextFlowButtonLabel()
    {
        switch (currentFlowStep)
        {
            case 1:
                return "메시지로";
            case 2:
                return "위치로";
            case 3:
                return "위성망 확인";
            case 4:
                return "확인하기";
            default:
                return "다음";
        }
    }

    private bool CanReachFlowStep(int step)
    {
        if (missionManager == null)
        {
            return false;
        }
        if (composingQueuedMessageInFlow)
        {
            if (step <= currentFlowStep)
            {
                return true;
            }
            if (step == 2)
            {
                return FlowHasScheduleSelection();
            }
            bool queueHasMessage = !string.IsNullOrWhiteSpace(QueuedFlowMessageText()) && !FlowPayloadTooLarge();
            if (step == 3)
            {
                return queueHasMessage;
            }
            if (step == 4)
            {
                return queueHasMessage && queueFlowOriginSelected && queueFlowTargetSelected && !string.IsNullOrEmpty(queueOriginName) && !string.IsNullOrEmpty(queueTargetName);
            }
            return CanQueueFlowSelection();
        }
        if (missionManager.Phase != MissionPhase.Idle)
        {
            return step == 5;
        }
        if (step <= currentFlowStep)
        {
            return true;
        }
        if (step == 2)
        {
            return FlowHasScheduleSelection();
        }
        bool hasMessage = !string.IsNullOrWhiteSpace(missionManager.MessageText) && !missionManager.PayloadTooLarge;
        if (step == 3)
        {
            return hasMessage;
        }
        if (step == 4)
        {
            return hasMessage && missionManager.HasExplicitOriginSelection && missionManager.HasExplicitDestinationSelection && missionManager.SelectedTarget != null;
        }
        return CanReserveCurrentSelection();
    }

    private void JumpToFlowStep(int step)
    {
        if (CanReachFlowStep(step))
        {
            GoToFlowStep(step);
            return;
        }
        string blockedReason = FlowStepBlockedReason();
        ShowToast(string.IsNullOrEmpty(blockedReason) ? "이전 단계를 먼저 완료하세요." : blockedReason);
    }

    private void AdvanceFlowStep()
    {
        if (CanAdvanceFlowStep())
        {
            GoToFlowStep(currentFlowStep + 1);
            return;
        }
        string blockedReason = FlowStepBlockedReason();
        ShowToast(string.IsNullOrEmpty(blockedReason) ? "이전 단계를 먼저 완료하세요." : blockedReason);
    }

    private void GoToFlowStep(int step)
    {
        if (missionManager != null && missionManager.Phase != MissionPhase.Idle && !composingQueuedMessageInFlow)
        {
            currentFlowStep = 5;
        }
        else
        {
            currentFlowStep = Mathf.Clamp(step, 1, 5);
        }
        if (currentFlowStep == 3)
        {
            selectingOriginInDestinationStep = true;
        }
        if (currentFlowStep == 4)
        {
            satelliteListDismissed = false;
            if (satelliteListPanel != null)
            {
                satelliteListPanel.SetAsLastSibling();
            }
        }
        requestFlowDismissed = false;
        RefreshAll();
        RestoreMessageInputFocus();
    }

    private bool CanAdvanceFlowStep()
    {
        if (missionManager == null || missionManager.Phase != MissionPhase.Idle)
        {
            return composingQueuedMessageInFlow && CanAdvanceQueuedFlowStep();
        }

        switch (currentFlowStep)
        {
            case 1:
                return FlowHasScheduleSelection();
            case 2:
                return !string.IsNullOrWhiteSpace(missionManager.MessageText) && !missionManager.PayloadTooLarge;
            case 3:
                return missionManager.HasExplicitOriginSelection && missionManager.HasExplicitDestinationSelection && missionManager.SelectedTarget != null;
            case 4:
                return CanReserveCurrentSelection();
            default:
                return false;
        }
    }

    private bool CanReserveCurrentSelection()
    {
        if (composingQueuedMessageInFlow)
        {
            return CanQueueFlowSelection();
        }
        if (missionManager == null || satelliteManager == null || missionManager.SelectedTarget == null)
        {
            return false;
        }

        return missionManager.Phase == MissionPhase.Idle
            && !string.IsNullOrWhiteSpace(missionManager.MessageText)
            && !missionManager.PayloadTooLarge
            && missionManager.HasExplicitOriginSelection
            && missionManager.HasExplicitDestinationSelection
            && satelliteManager.SelectedSatellite != null
            && CanServeRoute(satelliteManager.SelectedSatellite, missionManager.SelectedTarget.targetName, missionManager.RequestedSendDelayMinutes);
    }

    private void StartSelectedMission()
    {
        if (missionManager == null)
        {
            return;
        }
        if (composingQueuedMessageInFlow)
        {
            AddQueuedMessageFromFlow();
            return;
        }

        resultDismissed = false;
        missionStatusDismissed = false;
        timelineDismissed = false;
        missionManager.ReserveAndStartMission();
        if (IsActiveTransferPhase(missionManager.Phase))
        {
            requestFlowDismissed = true;
            if (requestFlowPanel != null)
            {
                requestFlowPanel.gameObject.SetActive(false);
            }
        }
    }

    private void ResetFlowAndMission()
    {
        currentFlowStep = 1;
        visibleRecordIndex = -1;
        resultDismissed = false;
        requestFlowDismissed = false;
        satelliteListDismissed = false;
        missionStatusDismissed = false;
        timelineDismissed = false;
        demoModeActive = false;
        composingQueuedMessageInFlow = false;
        flowScheduleSelected = false;
        queueScheduleOverride = false;
        queueFlowOriginSelected = false;
        queueFlowTargetSelected = false;
        if (missionManager != null)
        {
            missionManager.ResetMission();
            flowScheduleSelected = true;
        }
        else
        {
            RefreshAll();
        }
    }

    private void EnsureDefaultFlowSelections()
    {
        if (missionManager != null && missionManager.Phase == MissionPhase.Idle && !composingQueuedMessageInFlow)
        {
            flowScheduleSelected = true;
        }
    }

    private void HandleNewMessageButton()
    {
        if (missionManager == null)
        {
            return;
        }

        if (missionManager.Phase == MissionPhase.Complete)
        {
            ResetFlowAndMission();
            ShowRequestFlowPanel();
            return;
        }

        if (IsActiveTransferPhase(missionManager.Phase))
        {
            BeginQueuedMessageFlow();
            return;
        }

        if (missionManager.Phase != MissionPhase.Idle)
        {
            ResetFlowAndMission();
            ShowRequestFlowPanel();
        }
    }

    private void HandleConfirmSecondaryButton()
    {
        if (composingQueuedMessageInFlow)
        {
            CancelQueuedMessageFlow();
            return;
        }
        if (missionManager != null && IsActiveTransferPhase(missionManager.Phase))
        {
            ShowMissionStatusPanel();
            return;
        }

        ConfirmResetFlowAndMission();
    }

    private void SelectQuickDelay(int minutes)
    {
        if (composingQueuedMessageInFlow)
        {
            SetQueuedFlowScheduleFromDelay(minutes);
        }
        else if (missionManager != null)
        {
            flowScheduleSelected = true;
            missionManager.SetSendDelayMinutes(minutes);
        }
        ToggleCalendarPicker(false);
        RefreshAll();
    }

    private void ToggleAttachmentPicker()
    {
        ToggleAttachmentPicker(attachmentPickerPanel == null || !attachmentPickerPanel.gameObject.activeSelf);
    }

    private void ToggleAttachmentPicker(bool visible)
    {
        if (attachmentPickerPanel != null)
        {
            attachmentPickerPanel.gameObject.SetActive(visible);
            if (visible)
            {
                ClearAttachmentPathInput(false);
                ClampPopupToParent(attachmentPickerPanel);
            }
        }
    }

    private void RemoveFirstAttachment()
    {
        RemoveAttachmentAt(0);
    }

    private void RemoveAttachmentAt(int index)
    {
        if (composingQueuedMessageInFlow)
        {
            RemoveQueueAttachmentAt(index);
            RefreshAll();
            return;
        }
        if (missionManager == null || missionManager.Phase != MissionPhase.Idle || missionManager.AttachmentCount == 0)
        {
            return;
        }
        missionManager.RemoveAttachmentAt(index);
        ShowToast("첨부 파일을 제거했습니다.");
        RefreshAll();
    }

    private void AddQueuedMessageFromFlow()
    {
        if (missionManager == null)
        {
            return;
        }

        string error;
        bool updatingQueuedMessage = queueEditingIndex >= 0;
        int preferredIndex = updatingQueuedMessage ? queueEditingIndex : missionManager.QueuedMessageCount;
        bool queued = updatingQueuedMessage
            ? missionManager.UpdateQueuedMessageFromOrigin(
                queueEditingIndex,
                QueuedFlowMessageText(),
                queueOriginName,
                queueTargetName,
                QueueScheduledDayOffset(),
                QueueScheduledHour(),
                QueueScheduledMinute(),
                CloneQueueAttachments(),
                out error)
            : missionManager.QueueMessageFromOrigin(
                QueuedFlowMessageText(),
                queueOriginName,
                queueTargetName,
                QueueScheduledDayOffset(),
                QueueScheduledHour(),
                QueueScheduledMinute(),
                CloneQueueAttachments(),
                out error);
        if (!queued)
        {
            ShowToast(string.IsNullOrEmpty(error) ? "메시지를 추가하지 못했습니다." : error);
            RefreshAll();
            return;
        }

        composingQueuedMessageInFlow = false;
        requestFlowDismissed = true;
        currentFlowStep = 5;
        queueEditingIndex = -1;
        if (!string.IsNullOrEmpty(queueSelectedSatelliteName))
        {
            missionManager.SetQueuedMessagePreferredSatellite(preferredIndex, queueSelectedSatelliteName);
        }
        queueSelectedSatelliteName = string.Empty;
        if (requestFlowPanel != null)
        {
            requestFlowPanel.gameObject.SetActive(false);
        }
        if (messageInputField != null)
        {
            messageInputField.SetTextWithoutNotify(missionManager.MessageText ?? string.Empty);
        }
        queueAttachments.Clear();
        SyncQueueAttachmentLegacyFields();
        queueScheduleOverride = false;
        ToggleAttachmentPicker(false);
        ShowQueuePanel();
        ShowToast(updatingQueuedMessage ? "대기 메시지를 수정했습니다." : "메시지를 대기 목록에 추가했습니다.");
        RefreshAll();
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
            options.Add("수신지: " + DestinationLabel(target.targetName));
        }
        targetDropdown.AddOptions(options);
    }

    private void CreateDestinationGridButton(RectTransform parent, string targetName, string label, int index)
    {
        int col = index % 3;
        int row = index / 3;
        float left = col / 3f;
        float right = left + 0.31f;
        float top = -112f - row * 52f;
        float bottom = top - 40f;
        Button button = CreateButton(
            parent,
            label,
            new Vector2(left, 1f),
            new Vector2(right, 1f),
            new Vector2(col == 0 ? 18f : 6f, bottom),
            new Vector2(col == 2 ? -18f : -6f, top),
            delegate
            {
                if (selectingOriginInDestinationStep)
                {
                    if (composingQueuedMessageInFlow)
                    {
                        queueOriginName = targetName;
                        queueFlowOriginSelected = true;
                    }
                    else
                    {
                        missionManager.SelectOrigin(targetName);
                    }
                    RefreshAll();
                    return;
                }
                if (composingQueuedMessageInFlow)
                {
                    queueTargetName = targetName;
                    queueFlowTargetSelected = true;
                    RefreshAll();
                    return;
                }
                missionManager.SelectTarget(targetName);
                if (cameraManager != null && missionManager.SelectedTarget != null)
                {
                    cameraManager.missionTarget = missionManager.SelectedTarget.transform;
                    cameraManager.SetMission();
                }
            });
        destinationButtons[targetName] = button;
    }

    private void CreateDestinationButton(RectTransform parent, string targetName, string label, float leftAnchor)
    {
        Button button = CreateButton(
            parent,
            label,
            new Vector2(leftAnchor, 1f),
            new Vector2(Mathf.Min(1f, leftAnchor + 0.3f), 1f),
            new Vector2(leftAnchor <= 0f ? 18f : 6f, -112f),
            new Vector2(leftAnchor >= 0.68f ? -18f : -6f, -58f),
            delegate
            {
                if (composingQueuedMessageInFlow)
                {
                    queueTargetName = targetName;
                    queueFlowTargetSelected = true;
                    RefreshAll();
                    return;
                }
                missionManager.SelectTarget(targetName);
                if (cameraManager != null && missionManager.SelectedTarget != null)
                {
                    cameraManager.missionTarget = missionManager.SelectedTarget.transform;
                    cameraManager.SetMission();
                }
            });
        destinationButtons[targetName] = button;
    }

    private void CreateAttachmentButton(RectTransform parent, string fileName, string label, int sizeKb, float leftAnchor, float rightAnchor, float top, float bottom)
    {
        Button button = CreateButton(
            parent,
            label,
            new Vector2(leftAnchor, 1f),
            new Vector2(rightAnchor, 1f),
            new Vector2(leftAnchor <= 0f ? 18f : 6f, bottom),
            new Vector2(rightAnchor >= 1f ? -18f : -6f, top),
            delegate
            {
                if (composingQueuedMessageInFlow)
                {
                    AddQueueAttachment(fileName, sizeKb);
                    ToggleAttachmentPicker(false);
                    RefreshAll();
                    return;
                }
                missionManager.AddAttachment(fileName, sizeKb);
                ToggleAttachmentPicker(false);
            });
        Text buttonText = button.GetComponentInChildren<Text>();
        if (buttonText != null)
        {
            buttonText.fontSize = 14;
            buttonText.lineSpacing = 0.9f;
        }
        attachmentButtons[fileName] = button;
    }

    private void CreateAttachmentDeleteButtons(RectTransform parent)
    {
        attachmentDeleteButtons.Clear();
        for (int i = 0; i < 3; i++)
        {
            int index = i;
            float top = -252f - i * 28f;
            Button button = CreateButton(
                parent,
                (i + 1) + " 삭제",
                new Vector2(0.68f, 1f),
                new Vector2(1f, 1f),
                new Vector2(4f, top - 24f),
                new Vector2(-18f, top),
                delegate { RemoveAttachmentAt(index); });
            SetButtonTextSize(button, 11);
            button.gameObject.SetActive(false);
            attachmentDeleteButtons.Add(button);
        }
    }

    private void ToggleCalendarPicker()
    {
        ToggleCalendarPicker(calendarPickerPanel == null || !calendarPickerPanel.gameObject.activeSelf);
    }

    private void ToggleCalendarPicker(bool visible)
    {
        if (calendarPickerPanel != null)
        {
            calendarPickerPanel.gameObject.SetActive(visible);
            if (visible)
            {
                FocusCalendarMonthForSelectedDate();
                RefreshCalendarButtons();
                PositionToastAboveActivePanels();
                calendarPickerPanel.SetAsLastSibling();
                ApplyFixedPanelOrder();
            }
        }
    }

    private void AttachCalendarMonthScroll(RectTransform panel)
    {
        if (panel == null)
        {
            return;
        }

        EventTrigger trigger = panel.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = panel.gameObject.AddComponent<EventTrigger>();
        }
        if (trigger.triggers == null)
        {
            trigger.triggers = new List<EventTrigger.Entry>();
        }

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.Scroll;
        entry.callback.AddListener(HandleCalendarMonthScroll);
        trigger.triggers.Add(entry);
    }

    private void HandleCalendarMonthScroll(BaseEventData data)
    {
        PointerEventData pointerData = data as PointerEventData;
        if (pointerData == null || Mathf.Abs(pointerData.scrollDelta.y) < 0.01f)
        {
            return;
        }

        ChangeCalendarDisplayMonth(pointerData.scrollDelta.y < 0f ? 1 : -1);
    }

    private void ChangeCalendarDisplayMonth(int delta)
    {
        int nextOffset = Mathf.Clamp(calendarDisplayMonthOffset + delta, 0, MaxCalendarDisplayMonthOffset());
        if (nextOffset == calendarDisplayMonthOffset)
        {
            return;
        }

        calendarDisplayMonthOffset = nextOffset;
        RefreshCalendarButtons();
    }

    private void SetCalendarDate(int dayOffset)
    {
        if (dayOffset < 0 || dayOffset > 365)
        {
            return;
        }

        if (composingQueuedMessageInFlow)
        {
            queueScheduleOverride = true;
            queueScheduledDayOffset = dayOffset;
            NormalizeQueuedScheduleIfPast();
            RefreshAll();
        }
        else if (missionManager != null)
        {
            flowScheduleSelected = true;
            missionManager.SetScheduledDateTime(dayOffset, missionManager.ScheduledHour, missionManager.ScheduledMinute);
        }
    }

    private void SetCalendarHour(int hour)
    {
        if (composingQueuedMessageInFlow)
        {
            queueScheduleOverride = true;
            queueScheduledHour = hour;
            NormalizeQueuedScheduleIfPast();
            RefreshAll();
        }
        else if (missionManager != null)
        {
            flowScheduleSelected = true;
            missionManager.SetScheduledDateTime(missionManager.ScheduledDayOffset, hour, missionManager.ScheduledMinute);
        }
    }

    private void SetCalendarMinute(int minute)
    {
        if (composingQueuedMessageInFlow)
        {
            queueScheduleOverride = true;
            queueScheduledMinute = minute;
            NormalizeQueuedScheduleIfPast();
            RefreshAll();
        }
        else if (missionManager != null)
        {
            flowScheduleSelected = true;
            missionManager.SetScheduledDateTime(missionManager.ScheduledDayOffset, missionManager.ScheduledHour, minute);
        }
    }

    private void PopulateScheduleDropdowns()
    {
        scheduleDropdownUpdating = true;
        if (scheduleDayDropdown != null)
        {
            scheduleDayDropdown.ClearOptions();
            List<string> days = new List<string>();
            for (int i = 0; i < CalendarDayButtonCount; i++)
            {
                days.Add(CalendarDayLabel(i));
            }
            scheduleDayDropdown.AddOptions(days);
        }
        if (scheduleHourDropdown != null)
        {
            scheduleHourDropdown.ClearOptions();
            List<string> hours = new List<string>();
            for (int i = 0; i < 24; i++)
            {
                hours.Add(i.ToString("00") + "시");
            }
            scheduleHourDropdown.AddOptions(hours);
        }
        if (scheduleMinuteDropdown != null)
        {
            scheduleMinuteDropdown.ClearOptions();
            List<string> minutes = new List<string>();
            for (int i = 0; i < 60; i += 5)
            {
                minutes.Add(i.ToString("00") + "분");
            }
            scheduleMinuteDropdown.AddOptions(minutes);
        }
        scheduleDropdownUpdating = false;
    }

    private void ApplyScheduleDropdownSelection()
    {
        if (scheduleDropdownUpdating || missionManager == null || scheduleDayDropdown == null || scheduleHourDropdown == null || scheduleMinuteDropdown == null)
        {
            return;
        }

        int minute = Mathf.Clamp(scheduleMinuteDropdown.value * 5, 0, 55);
        if (composingQueuedMessageInFlow)
        {
            queueScheduleOverride = true;
            queueScheduledHour = scheduleHourDropdown.value;
            queueScheduledMinute = minute;
            NormalizeQueuedScheduleIfPast();
            RefreshAll();
            return;
        }
        flowScheduleSelected = true;
        missionManager.SetScheduledDateTime(missionManager.ScheduledDayOffset, scheduleHourDropdown.value, minute);
    }

    private void RefreshScheduleDropdowns()
    {
        if (missionManager == null)
        {
            return;
        }

        if (scheduleDayDropdown != null && scheduleHourDropdown != null && scheduleMinuteDropdown != null)
        {
            scheduleDropdownUpdating = true;
            scheduleDayDropdown.value = Mathf.Clamp(composingQueuedMessageInFlow ? QueueScheduledDayOffset() : missionManager.ScheduledDayOffset, 0, scheduleDayDropdown.options.Count - 1);
            scheduleHourDropdown.value = Mathf.Clamp(composingQueuedMessageInFlow ? QueueScheduledHour() : missionManager.ScheduledHour, 0, scheduleHourDropdown.options.Count - 1);
            scheduleMinuteDropdown.value = Mathf.Clamp((composingQueuedMessageInFlow ? QueueScheduledMinute() : missionManager.ScheduledMinute) / 5, 0, scheduleMinuteDropdown.options.Count - 1);
            scheduleDropdownUpdating = false;
        }

        RefreshCalendarButtons();
    }

    private void NormalizeQueuedScheduleIfPast()
    {
        if (!queueScheduleOverride)
        {
            return;
        }

        System.DateTime now = System.DateTime.Now;
        System.DateTime scheduled = now.Date
            .AddDays(Mathf.Clamp(queueScheduledDayOffset, 0, 365))
            .AddHours(Mathf.Clamp(queueScheduledHour, 0, 23))
            .AddMinutes(Mathf.Clamp(queueScheduledMinute, 0, 59));
        if (scheduled > now)
        {
            return;
        }

        queueScheduledDayOffset = 0;
        queueScheduledHour = now.Hour;
        queueScheduledMinute = now.Minute - now.Minute % 5;
    }

    private void RefreshCalendarButtons()
    {
        if (missionManager == null)
        {
            return;
        }

        if (calendarMonthLabel != null)
        {
            calendarMonthLabel.text = CalendarDisplayMonth().ToString("yyyy년 M월");
        }

        foreach (KeyValuePair<int, Button> pair in scheduleDayButtons)
        {
            int dayOffset = CalendarCellDayOffset(pair.Key);
            bool selectable = dayOffset >= 0 && dayOffset <= 365;
            bool selected = selectable && dayOffset == (composingQueuedMessageInFlow ? QueueScheduledDayOffset() : missionManager.ScheduledDayOffset);
            bool today = dayOffset == 0;
            bool sameDisplayMonth = CalendarCellInDisplayMonth(pair.Key);
            SetButtonLabel(pair.Value, CalendarCellLabel(pair.Key));
            pair.Value.interactable = selectable;
            SetButtonVisual(pair.Value, selected ? ButtonVisual.Selected : today ? ButtonVisual.Cta : selectable ? ButtonVisual.Secondary : ButtonVisual.Disabled);
            Text dayText = pair.Value.GetComponentInChildren<Text>();
            if (dayText != null)
            {
                dayText.fontStyle = selected || today ? FontStyle.Bold : FontStyle.Normal;
                dayText.color = !selectable
                    ? new Color(1f, 1f, 1f, 0.34f)
                    : sameDisplayMonth ? Color.white : new Color(0.82f, 0.9f, 1f, 0.56f);
            }
        }

        foreach (KeyValuePair<int, Button> pair in scheduleHourButtons)
        {
            bool selected = (composingQueuedMessageInFlow ? QueueScheduledHour() : missionManager.ScheduledHour) == pair.Key;
            SetButtonLabel(pair.Value, pair.Key.ToString("00") + "시");
            SetButtonVisual(pair.Value, selected ? ButtonVisual.Selected : ButtonVisual.Secondary);
        }

        foreach (KeyValuePair<int, Button> pair in scheduleMinuteButtons)
        {
            bool selected = (composingQueuedMessageInFlow ? QueueScheduledMinute() : missionManager.ScheduledMinute) == pair.Key;
            SetButtonLabel(pair.Value, pair.Key.ToString("00") + "분");
            SetButtonVisual(pair.Value, selected ? ButtonVisual.Selected : ButtonVisual.Secondary);
        }

        if (schedulePickerButton != null)
        {
            SetButtonLabel(schedulePickerButton, "직접 설정");
        }
    }

    private string ScheduleDayLabel(int offset)
    {
        System.DateTime date = System.DateTime.Now.Date.AddDays(offset);
        return date.ToString("MM/dd (ddd)");
    }

    private string CalendarDayLabel(int offset)
    {
        System.DateTime date = System.DateTime.Now.Date.AddDays(offset);
        return date.Day == 1 || offset == 0 ? date.ToString("M/d") : date.Day.ToString();
    }

    private int CalendarCellDayOffset(int cellIndex)
    {
        System.DateTime today = System.DateTime.Now.Date;
        System.DateTime firstOfMonth = CalendarDisplayMonth();
        System.DateTime cellDate = firstOfMonth.AddDays(cellIndex - (int)firstOfMonth.DayOfWeek);
        return (int)(cellDate - today).TotalDays;
    }

    private string CalendarCellLabel(int cellIndex)
    {
        int dayOffset = CalendarCellDayOffset(cellIndex);
        if (dayOffset == 0)
        {
            return "오늘";
        }

        System.DateTime firstOfMonth = CalendarDisplayMonth();
        System.DateTime cellDate = firstOfMonth.AddDays(cellIndex - (int)firstOfMonth.DayOfWeek);
        return cellDate.Month == firstOfMonth.Month ? cellDate.Day.ToString() : cellDate.ToString("M/d");
    }

    private bool CalendarCellInDisplayMonth(int cellIndex)
    {
        System.DateTime firstOfMonth = CalendarDisplayMonth();
        System.DateTime cellDate = firstOfMonth.AddDays(cellIndex - (int)firstOfMonth.DayOfWeek);
        return cellDate.Month == firstOfMonth.Month;
    }

    private System.DateTime CalendarDisplayMonth()
    {
        System.DateTime today = System.DateTime.Now.Date;
        return new System.DateTime(today.Year, today.Month, 1).AddMonths(calendarDisplayMonthOffset);
    }

    private int MaxCalendarDisplayMonthOffset()
    {
        System.DateTime today = System.DateTime.Now.Date;
        System.DateTime maxDate = today.AddDays(365);
        return Mathf.Max(0, (maxDate.Year - today.Year) * 12 + maxDate.Month - today.Month);
    }

    private void FocusCalendarMonthForSelectedDate()
    {
        int selectedDayOffset = composingQueuedMessageInFlow ? QueueScheduledDayOffset() : missionManager != null ? missionManager.ScheduledDayOffset : 0;
        System.DateTime today = System.DateTime.Now.Date;
        System.DateTime selectedDate = today.AddDays(Mathf.Clamp(selectedDayOffset, 0, 365));
        calendarDisplayMonthOffset = Mathf.Clamp((selectedDate.Year - today.Year) * 12 + selectedDate.Month - today.Month, 0, MaxCalendarDisplayMonthOffset());
    }

    private void RefreshDestinationButtons()
    {
        if (missionManager == null)
        {
            return;
        }

        foreach (KeyValuePair<string, Button> pair in destinationButtons)
        {
            string selectedOriginName = composingQueuedMessageInFlow ? queueOriginName : missionManager.OriginName;
            string selectedTargetName = composingQueuedMessageInFlow ? queueTargetName : missionManager.SelectedTarget != null ? missionManager.SelectedTarget.targetName : string.Empty;
            bool hasOriginSelection = composingQueuedMessageInFlow || missionManager.HasExplicitOriginSelection;
            bool hasTargetSelection = composingQueuedMessageInFlow || missionManager.HasExplicitDestinationSelection;
            bool selected = selectingOriginInDestinationStep
                ? hasOriginSelection && selectedOriginName == pair.Key
                : hasTargetSelection && selectedTargetName == pair.Key;
            bool favorite = missionManager.IsFavoriteTarget(pair.Key);
            bool visible = DestinationMatchesSearch(pair.Key) || selected;
            pair.Value.gameObject.SetActive(visible);
            pair.Value.interactable = missionManager.Phase == MissionPhase.Idle || composingQueuedMessageInFlow;
            string prefix = !selected && favorite ? "★ " : string.Empty;
            SetButtonLabel(pair.Value, prefix + DestinationLabel(pair.Key));
            SetButtonVisual(pair.Value, selected ? ButtonVisual.Selected : missionManager.Phase == MissionPhase.Idle || composingQueuedMessageInFlow ? ButtonVisual.Secondary : ButtonVisual.Disabled);
        }

        if (originPickButton != null)
        {
            string label = composingQueuedMessageInFlow || missionManager.HasExplicitOriginSelection
                ? DestinationLabel(composingQueuedMessageInFlow ? queueOriginName : missionManager.OriginName)
                : "선택 전";
            SetButtonLabel(originPickButton, "송신지: " + label);
            SetButtonVisual(originPickButton, selectingOriginInDestinationStep ? ButtonVisual.Selected : ButtonVisual.Secondary);
        }
        if (destinationPickButton != null)
        {
            string targetName = composingQueuedMessageInFlow ? queueTargetName : missionManager.SelectedTarget != null ? missionManager.SelectedTarget.targetName : OrbitShareDestinations.DefaultTargetName;
            string label = composingQueuedMessageInFlow || missionManager.HasExplicitDestinationSelection
                ? DestinationLabel(targetName)
                : "선택 전";
            SetButtonLabel(destinationPickButton, "수신지: " + label);
            SetButtonVisual(destinationPickButton, selectingOriginInDestinationStep ? ButtonVisual.Secondary : ButtonVisual.Selected);
        }

        if (favoriteDestinationButton != null)
        {
            bool hasTarget = !selectingOriginInDestinationStep && !composingQueuedMessageInFlow && missionManager.SelectedTarget != null;
            bool favorite = hasTarget && missionManager.IsFavoriteTarget(missionManager.SelectedTarget.targetName);
            favoriteDestinationButton.interactable = hasTarget;
            SetButtonLabel(favoriteDestinationButton, composingQueuedMessageInFlow ? "목적지 선택" : favorite ? "★ 즐겨찾기 해제" : "☆ 즐겨찾기");
            SetButtonVisual(favoriteDestinationButton, favorite ? ButtonVisual.Selected : hasTarget ? ButtonVisual.Secondary : ButtonVisual.Disabled);
        }

        if (destinationSummaryText != null)
        {
            destinationSummaryText.text = string.Empty;
            destinationSummaryText.gameObject.SetActive(false);
        }
    }

    private bool DestinationMatchesSearch(string targetName)
    {
        if (string.IsNullOrWhiteSpace(destinationSearchQuery))
        {
            return true;
        }
        string query = destinationSearchQuery.Trim().ToLowerInvariant();
        return targetName.ToLowerInvariant().Contains(query) || DestinationLabel(targetName).ToLowerInvariant().Contains(query);
    }

    private string DestinationSummaryText()
    {
        System.Text.StringBuilder builder = new System.Text.StringBuilder();
        string originName = composingQueuedMessageInFlow ? queueOriginName : missionManager.OriginName;
        string targetName = composingQueuedMessageInFlow ? queueTargetName : missionManager.SelectedTarget != null ? missionManager.SelectedTarget.targetName : OrbitShareDestinations.DefaultTargetName;
        builder.Append("송신지 ").Append(DestinationLabel(originName))
            .Append(" → 수신지 ").Append(DestinationLabel(targetName))
            .Append("\n선택 모드: ").Append(selectingOriginInDestinationStep ? "송신지 선택" : "수신지 선택");
        if (originName == targetName)
        {
            builder.Append("\n동일 지역 전송도 가능합니다.");
        }
        ObservationTarget target = missionManager.Targets.Find(item => item.targetName == targetName);
        if (target != null)
        {
            builder.Append("\n").Append(DestinationLabel(target.targetName))
                .Append(" · 우선순위 ").Append(target.priorityLabel)
                .Append(" · 날씨 위험 ").Append(target.weatherRiskPercent.ToString("0")).Append("%")
                .Append("\n지도 표시  도시 라벨/목적지 핀 강조")
                .Append("\n예상 데이터 ").Append(FormatBytes((long)Mathf.Max(0f, target.estimatedDataMb) * 1024L * 1024L))
                .Append(" · 해상도 ").Append(target.resolutionMeters.ToString("0.0")).Append("m");
        }
        if (missionManager.FavoriteTargetNames.Count > 0)
        {
            if (builder.Length > 0)
            {
                builder.Append("\n");
            }
            builder.Append("최근 선택: ");
            int count = Mathf.Min(3, missionManager.FavoriteTargetNames.Count);
            for (int i = 0; i < count; i++)
            {
                if (i > 0)
                {
                    builder.Append(", ");
                }
                builder.Append(DestinationLabel(missionManager.FavoriteTargetNames[i]));
            }
        }
        if (missionManager.RecentTargetNames.Count > 0)
        {
            if (builder.Length > 0)
            {
                builder.Append("\n");
            }
            builder.Append("최근: ");
            int count = Mathf.Min(4, missionManager.RecentTargetNames.Count);
            for (int i = 0; i < count; i++)
            {
                if (i > 0)
                {
                    builder.Append(", ");
                }
                builder.Append(DestinationLabel(missionManager.RecentTargetNames[i]));
            }
        }
        string comparison = DestinationComparisonText();
        if (!string.IsNullOrEmpty(comparison))
        {
            if (builder.Length > 0)
            {
                builder.Append("\n");
            }
            builder.Append(comparison);
        }
        return builder.Length == 0 ? "목적지를 검색하거나 선택하세요." : builder.ToString();
    }

    private string DestinationComparisonText()
    {
        if (missionManager == null || satelliteManager == null || missionManager.Targets == null || missionManager.Targets.Count == 0)
        {
            return string.Empty;
        }

        System.Text.StringBuilder builder = new System.Text.StringBuilder("목적지 비교: ");
        int count = Mathf.Min(7, missionManager.Targets.Count);
        for (int i = 0; i < count; i++)
        {
            ObservationTarget target = missionManager.Targets[i];
            int available = AvailableSatelliteCountAtDelay(target.targetName, missionManager.RequestedSendDelayMinutes);
            SatelliteData best = satelliteManager.FindBestSatellite(CurrentOriginName(), target.targetName, missionManager.RequestedSendDelayMinutes, missionManager.MessageSizeKb);
            SatelliteRouteEstimateRecord estimate = best != null ? satelliteManager.GetRouteEstimate(best, CurrentOriginName(), target.targetName, missionManager.RequestedSendDelayMinutes, missionManager.MessageSizeKb) : null;
            string eta = estimate == null ? "불가" : FormatSeconds(Mathf.Max(1, estimate.estimatedDeliverySeconds));
            if (i > 0)
            {
                builder.Append(" / ");
            }
            builder.Append(DestinationLabel(target.targetName)).Append(" ").Append(eta).Append(" · ").Append(available).Append("개");
        }
        return builder.ToString();
    }

    private void ToggleFavoriteDestination()
    {
        if (missionManager == null || missionManager.SelectedTarget == null)
        {
            return;
        }
        missionManager.ToggleFavoriteTarget(missionManager.SelectedTarget.targetName);
        RefreshDestinationButtons();
    }

    private void RefreshAttachmentButtons()
    {
        if (missionManager == null)
        {
            return;
        }

        foreach (KeyValuePair<string, Button> pair in attachmentButtons)
        {
            bool selected = composingQueuedMessageInFlow ? HasQueueAttachment(pair.Key) : HasMissionAttachment(pair.Key);
            bool canChange = missionManager.Phase == MissionPhase.Idle || composingQueuedMessageInFlow;
            pair.Value.interactable = canChange;
            SetButtonVisual(pair.Value, selected ? ButtonVisual.Selected : canChange ? ButtonVisual.Secondary : ButtonVisual.Disabled);
        }
        if (attachmentPickerButton != null)
        {
            bool canChange = missionManager.Phase == MissionPhase.Idle || composingQueuedMessageInFlow;
            int attachmentSizeKb = composingQueuedMessageInFlow ? QueueAttachmentSizeKb() : missionManager.AttachmentSizeKb;
            attachmentPickerButton.interactable = canChange;
            int attachmentCount = composingQueuedMessageInFlow ? queueAttachments.Count : missionManager.AttachmentCount;
            SetButtonLabel(attachmentPickerButton, attachmentCount > 0 ? "파일 추가" : "파일 첨부");
            SetButtonVisual(attachmentPickerButton, attachmentSizeKb > 0 ? ButtonVisual.Selected : canChange ? ButtonVisual.Secondary : ButtonVisual.Disabled);
        }
        if (attachmentRemoveFirstButton != null)
        {
            int attachmentCount = composingQueuedMessageInFlow ? queueAttachments.Count : missionManager.AttachmentCount;
            bool canRemove = (missionManager.Phase == MissionPhase.Idle || composingQueuedMessageInFlow) && attachmentCount > 0;
            attachmentRemoveFirstButton.interactable = canRemove;
            SetButtonLabel(attachmentRemoveFirstButton, attachmentCount > 1 ? "첫 파일 제거" : "파일 제거");
            SetButtonVisual(attachmentRemoveFirstButton, canRemove ? ButtonVisual.Secondary : ButtonVisual.Disabled);
        }
        if (attachmentStatusText != null)
        {
            attachmentStatusText.text = string.Empty;
            attachmentStatusText.gameObject.SetActive(false);
        }
    }

    private void RefreshAttachmentDeleteButtons()
    {
        IReadOnlyList<MessageAttachmentRecord> attachments = composingQueuedMessageInFlow
            ? queueAttachments
            : missionManager != null ? missionManager.AttachmentFiles : null;
        int count = attachments != null ? attachments.Count : 0;
        bool canChange = missionManager != null && (missionManager.Phase == MissionPhase.Idle || composingQueuedMessageInFlow);
        for (int i = 0; i < attachmentDeleteButtons.Count; i++)
        {
            Button button = attachmentDeleteButtons[i];
            bool active = i < count && canChange;
            button.gameObject.SetActive(active);
            if (active)
            {
                SetButtonLabel(button, (i + 1) + "번 삭제");
                if (attachments != null && attachments[i] != null && !string.IsNullOrEmpty(attachments[i].fileName))
                {
                    SetButtonLabel(button, "삭제 " + EllipsizeFileName(attachments[i].fileName, 9));
                }
                SetButtonVisual(button, ButtonVisual.Secondary);
                button.interactable = true;
            }
        }
    }

    private bool HasMissionAttachment(string fileName)
    {
        if (missionManager == null || string.IsNullOrEmpty(fileName))
        {
            return false;
        }
        for (int i = 0; i < missionManager.AttachmentFiles.Count; i++)
        {
            MessageAttachmentRecord attachment = missionManager.AttachmentFiles[i];
            if (attachment != null && attachment.fileName == fileName)
            {
                return true;
            }
        }
        return false;
    }

    private string PayloadBreakdownText()
    {
        if (missionManager == null)
        {
            return "계산 중";
        }

        if (composingQueuedMessageInFlow)
        {
            return "내용 : " + PayloadContentSizeText(QueuedFlowTextBytes())
                + "\n파일 : " + queueAttachments.Count + "개 / " + PayloadKbText(QueueAttachmentSizeKb())
                + "\n합계 : " + PayloadKbText(QueuedFlowPayloadKb()) + " / " + PayloadKbText(missionManager.MaxPayloadKb);
        }

        return "내용 : " + PayloadContentSizeText(missionManager.MessageTextBytes)
            + "\n파일 : " + missionManager.AttachmentCount + "개 / " + PayloadKbText(missionManager.AttachmentSizeKb)
            + "\n합계 : " + PayloadKbText(missionManager.MessageSizeKb) + " / " + PayloadKbText(missionManager.MaxPayloadKb);
    }

    private string PayloadContentSizeText(int bytes)
    {
        return FormatBytes(bytes);
    }

    private string PayloadKbText(int sizeKb)
    {
        return FormatKilobytesAsBytes(sizeKb);
    }

    private string AttachmentPreview(string fileName, int sizeKb)
    {
        if (string.IsNullOrEmpty(fileName) || sizeKb <= 0)
        {
            return string.Empty;
        }

        string type = AttachmentTypeLabel(fileName);
        return AttachmentIcon(fileName) + " " + EllipsizeFileName(fileName, 24) + " · " + type + " · " + PayloadKbText(sizeKb);
    }

    private string AttachmentPreview(IReadOnlyList<MessageAttachmentRecord> attachments)
    {
        if (attachments == null || attachments.Count == 0)
        {
            return string.Empty;
        }

        int totalKb = 0;
        for (int i = 0; i < attachments.Count; i++)
        {
            if (attachments[i] != null)
            {
                totalKb += Mathf.Max(0, attachments[i].sizeKb);
            }
        }
        System.Text.StringBuilder builder = new System.Text.StringBuilder("총 ");
        builder.Append(attachments.Count).Append("개 · ").Append(PayloadKbText(totalKb));
        int count = Mathf.Min(3, attachments.Count);
        for (int i = 0; i < count; i++)
        {
            MessageAttachmentRecord attachment = attachments[i];
            if (attachment == null)
            {
                continue;
            }
            builder.Append("\n").Append(i + 1).Append(". ")
                .Append(AttachmentIcon(attachment.fileName)).Append(" ")
                .Append(EllipsizeFileName(attachment.fileName, 24)).Append(" · ")
                .Append(AttachmentTypeLabel(attachment.fileName)).Append(" · ")
                .Append(PayloadKbText(attachment.sizeKb));
        }
        if (attachments.Count > count)
        {
            builder.Append("\n외 ").Append(attachments.Count - count).Append("개");
        }
        return builder.ToString();
    }

    private string AttachmentTypeLabel(string fileName)
    {
        string lowerName = string.IsNullOrEmpty(fileName) ? string.Empty : fileName.ToLowerInvariant();
        if (lowerName.EndsWith(".jpg") || lowerName.EndsWith(".jpeg") || lowerName.EndsWith(".png") || lowerName.EndsWith(".gif"))
        {
            return "이미지";
        }
        if (lowerName.EndsWith(".csv"))
        {
            return "표 데이터";
        }
        if (lowerName.EndsWith(".pdf"))
        {
            return "문서";
        }
        if (lowerName.EndsWith(".zip") || lowerName.EndsWith(".rar") || lowerName.EndsWith(".7z"))
        {
            return "압축";
        }
        return "파일";
    }

    private string EllipsizeFileName(string fileName, int maxLength)
    {
        if (string.IsNullOrEmpty(fileName) || fileName.Length <= maxLength)
        {
            return fileName;
        }

        string extension = Path.GetExtension(fileName);
        int keep = Mathf.Max(6, maxLength - extension.Length - 1);
        if (extension.Length > 0 && keep < fileName.Length)
        {
            return fileName.Substring(0, keep) + "…" + extension;
        }
        return fileName.Substring(0, maxLength - 1) + "…";
    }

    private string AttachmentIcon(string fileName)
    {
        string lowerName = string.IsNullOrEmpty(fileName) ? string.Empty : fileName.ToLowerInvariant();
        if (lowerName.EndsWith(".jpg") || lowerName.EndsWith(".jpeg") || lowerName.EndsWith(".png") || lowerName.EndsWith(".gif"))
        {
            return "📷";
        }
        if (lowerName.EndsWith(".pdf") || lowerName.EndsWith(".txt") || lowerName.EndsWith(".csv") || lowerName.EndsWith(".md"))
        {
            return "📄";
        }
        if (lowerName.EndsWith(".zip") || lowerName.EndsWith(".rar") || lowerName.EndsWith(".7z"))
        {
            return "🗜";
        }
        return "📎";
    }

    private List<MessageAttachmentRecord> AttachmentsOrLegacy(List<MessageAttachmentRecord> attachments, string legacyAttachmentName, int legacyAttachmentSizeKb)
    {
        List<MessageAttachmentRecord> cloned = new List<MessageAttachmentRecord>();
        if (attachments != null)
        {
            for (int i = 0; i < attachments.Count; i++)
            {
                MessageAttachmentRecord attachment = attachments[i];
                if (attachment != null && !string.IsNullOrEmpty(attachment.fileName) && attachment.sizeKb > 0)
                {
                    cloned.Add(new MessageAttachmentRecord(attachment.fileName, attachment.sizeKb, attachment.sizeBytes));
                }
            }
        }
        if (cloned.Count == 0 && legacyAttachmentSizeKb > 0)
        {
            cloned.Add(new MessageAttachmentRecord(string.IsNullOrEmpty(legacyAttachmentName) ? "첨부 파일" : legacyAttachmentName, legacyAttachmentSizeKb));
        }
        return cloned;
    }

    private bool IsScheduledPastToday()
    {
        if (missionManager == null || missionManager.ScheduledDayOffset != 0)
        {
            return false;
        }

        System.DateTime now = System.DateTime.Now;
        return missionManager.ScheduledHour < now.Hour || (missionManager.ScheduledHour == now.Hour && missionManager.ScheduledMinute < now.Minute);
    }

    private string SchedulePastWarning()
    {
        return string.Empty;
    }

    private string ScheduleAvailabilitySummary()
    {
        if (satelliteManager == null || missionManager == null)
        {
            return "가능 시간 계산 중";
        }

        string targetName = missionManager.SelectedTarget != null ? missionManager.SelectedTarget.targetName : OrbitShareDestinations.DefaultTargetName;
        int nowCount = AvailableSatelliteCountAtDelay(targetName, 0);
        int fiveCount = AvailableSatelliteCountAtDelay(targetName, 5);
        int fifteenCount = AvailableSatelliteCountAtDelay(targetName, 15);
        int thirtyCount = AvailableSatelliteCountAtDelay(targetName, 30);
        int recommendedDelay = RecommendedDelayMinutes(targetName);
        return "사용 가능: 지금 " + nowCount + "개 · +5분 " + fiveCount + "개 · +15분 " + fifteenCount + "개 · +30분 " + thirtyCount + "개\n" +
            "추천 시간: " + DelayLabel(recommendedDelay) + " · 추천 가능 " + AvailableSatelliteCountAtDelay(targetName, recommendedDelay) + "개";
    }

    private int RecommendedDelayMinutes(string targetName)
    {
        int[] delays = { 0, 5, 15, 30 };
        int bestDelay = delays[0];
        int bestScore = -999;
        for (int i = 0; i < delays.Length; i++)
        {
            int delay = delays[i];
            int count = AvailableSatelliteCountAtDelay(targetName, delay);
            int score = count * 10 - delay / 5;
            if (score > bestScore)
            {
                bestScore = score;
                bestDelay = delay;
            }
        }
        return bestDelay;
    }

    private string DelayLabel(int delayMinutes)
    {
        return delayMinutes == 0 ? "지금" : "+" + delayMinutes + "분";
    }

    private int AvailableSatelliteCountAtDelay(string targetName, int delayMinutes)
    {
        if (satelliteManager == null)
        {
            return 0;
        }

        List<SatelliteRouteEstimateRecord> estimates = satelliteManager.GetRankedRouteEstimates(SatelliteListFilter.Recommended, CurrentOriginName(), targetName, delayMinutes, CurrentSatellitePayloadKb());
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

    private string SendTimeLabel()
    {
        if (missionManager == null)
        {
            return "시간 미정";
        }
        return ScheduleDisplayLabel(missionManager.RequestedSendDelayMinutes, missionManager.ScheduledLabel);
    }

    private string ScheduleDisplayLabel(int requestedDelayMinutes, string scheduledLabel)
    {
        if (requestedDelayMinutes <= 0)
        {
            return "즉시 전송";
        }
        string label = string.IsNullOrEmpty(scheduledLabel) ? System.DateTime.Now.AddMinutes(requestedDelayMinutes).ToString("MM/dd HH:mm") : scheduledLabel.Replace(" 보내기", string.Empty);
        return "예약 전송 · " + label;
    }

    private string MessageSampleText(string targetName)
    {
        if (targetName == "Seoul")
        {
            return "서울 도착 확인";
        }
        if (targetName == "New York")
        {
            return "뉴욕 수신 확인";
        }
        if (targetName == "London")
        {
            return "런던 위성망 확인";
        }
        if (targetName == "Tokyo")
        {
            return "도쿄 수신 확인";
        }
        if (targetName == "Paris")
        {
            return "파리 수신 확인";
        }
        if (targetName == "Sydney")
        {
            return "시드니 수신 확인";
        }
        if (targetName == "Los Angeles")
        {
            return "LA 수신 확인";
        }
        return "상태 확인";
    }

    private string ReservationHintText()
    {
        if (missionManager == null)
        {
            return "필요한 항목을 선택하세요.";
        }
        if (composingQueuedMessageInFlow)
        {
            return QueueFlowReservationHintText();
        }
        if (missionManager.Phase == MissionPhase.Complete)
        {
            return "전송 완료\n전송 기록 보기 또는 새 메시지 작성을 선택하세요.";
        }
        if (missionManager.Phase == MissionPhase.Reserved)
        {
            return "현재 전송\n예약 대기";
        }
        if (IsActiveTransferPhase(missionManager.Phase))
        {
            return "현재 전송\n상태 확인";
        }
        if (missionManager.Phase != MissionPhase.Idle)
        {
            return MissionPhaseLabel(missionManager.Phase);
        }
        if (string.IsNullOrWhiteSpace(missionManager.MessageText))
        {
            return "메시지를 입력하세요.";
        }
        if (missionManager.PayloadTooLarge)
        {
            return "크기 초과";
        }
        if (!missionManager.HasExplicitOriginSelection || !missionManager.HasExplicitDestinationSelection || missionManager.SelectedTarget == null)
        {
            return "송신지와 수신지를 선택하세요.";
        }
        if (satelliteManager == null || satelliteManager.SelectedSatellite == null || !CanServeRoute(satelliteManager.SelectedSatellite, missionManager.SelectedTarget.targetName, missionManager.RequestedSendDelayMinutes))
        {
            return "가능한 위성을 선택하세요.";
        }
        SatelliteData selectedSatellite = satelliteManager.SelectedSatellite;
        string targetName = missionManager.SelectedTarget.targetName;
        string message = missionManager.MessageText.Trim();
        if (message.Length > 48)
        {
            message = message.Substring(0, 48) + "…";
        }
        string attachmentLine = missionManager.AttachmentCount > 0
            ? AttachmentPreview(missionManager.AttachmentFiles)
            : "첨부 파일 없음";
        int payloadTransferSeconds = missionManager.EstimatePayloadTransferDurationSeconds(missionManager.MessageSizeKb, selectedSatellite);
        return "시간 : " + SendTimeLabel() + "\n" +
            "메시지 : " + message + "\n" +
            "경로 : " + missionManager.OriginLabel + " -> " + DestinationLabel(targetName) + "\n" +
            "위성망 : " + CompactSatelliteName(selectedSatellite.satelliteName) + "\n" +
            "요금 : " + missionManager.PriceBreakdownText() + "\n" +
            "예상 소요 시간 : " + FormatSeconds(payloadTransferSeconds) + "\n" +
            "첨부 : " + attachmentLine;
    }

    private string QueueFlowReservationHintText()
    {
        if (string.IsNullOrWhiteSpace(QueuedFlowMessageText()))
        {
            return "메시지를 입력하세요.";
        }
        if (FlowPayloadTooLarge())
        {
            return "크기 초과: " + PayloadBreakdownText();
        }

        string message = QueuedFlowMessageText().Trim();
        if (message.Length > 48)
        {
            message = message.Substring(0, 48) + "…";
        }
        int delayMinutes = QueueFlowRequestedDelayMinutes();
        int payloadKb = Mathf.Max(1, QueuedFlowPayloadKb());
        SatelliteData best = QueueFlowRecommendedSatellite();
        float baseFee = 0f;
        float payloadFee = 0f;
        float urgencyFee = 0f;
        float price = missionManager != null ? missionManager.EstimateDetailedPriceUsd(payloadKb, delayMinutes, best, out baseFee, out payloadFee, out urgencyFee) : 0f;
        string satellite = best != null ? CompactSatelliteName(best.satelliteName) : "전송 시 자동 추천";
        string attachmentLine = queueAttachments.Count > 0 ? QueueAttachmentSummary() : "첨부 파일 없음";
        string estimatedDuration = missionManager != null ? FormatSeconds(missionManager.EstimatePayloadTransferDurationSeconds(payloadKb, best)) : "계산 중";
        return "시간 : " + QueueFlowScheduleLabel() + "\n" +
            "메시지 : " + message + "\n" +
            "경로 : " + DestinationLabel(queueOriginName) + " -> " + DestinationLabel(queueTargetName) + "\n" +
            "위성망 : " + satellite + "\n" +
            "요금 : " + FormatUsd(price) + "\n" +
            "예상 소요 시간 : " + estimatedDuration + "\n" +
            "첨부 : " + attachmentLine + "\n" +
            "상태 : 현재 전송 뒤 이어서 보냅니다.";
    }

    private void SetConfirmSummaryText(string text)
    {
        if (reservationHintText == null)
        {
            return;
        }

        string nextText = text ?? string.Empty;
        bool textChanged = lastConfirmSummaryText != nextText;
        reservationHintText.text = nextText;
        lastConfirmSummaryText = nextText;
        if (confirmSummaryContent == null || confirmSummaryScrollRect == null)
        {
            return;
        }

        float viewportHeight = confirmSummaryScrollRect.viewport != null
            ? Mathf.Max(1f, confirmSummaryScrollRect.viewport.rect.height)
            : 180f;
        float contentHeight = Mathf.Max(viewportHeight, reservationHintText.preferredHeight + 22f);
        confirmSummaryContent.sizeDelta = new Vector2(confirmSummaryContent.sizeDelta.x, contentHeight);
        if (textChanged && confirmSummaryScrollRect.velocity.sqrMagnitude < 0.1f)
        {
            confirmSummaryScrollRect.verticalNormalizedPosition = 1f;
        }
    }
}
