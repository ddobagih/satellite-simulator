using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// 공통 UI 유틸리티: 패널/버튼/텍스트/입력/드롭다운 생성과 공통 포맷팅을 제공합니다.
public partial class UIManager
{
    private string FormatBytes(long bytes)
    {
        return System.Math.Max(0L, bytes).ToString("N0") + " Bytes";
    }

    private string FormatKilobytesAsBytes(int sizeKb)
    {
        return FormatBytes((long)Mathf.Max(0, sizeKb) * 1024L);
    }

    private void RefreshSendOptionButtons()
    {
        if (missionManager == null)
        {
            return;
        }

        bool canChange = missionManager.Phase == MissionPhase.Idle || composingQueuedMessageInFlow;
        bool hasScheduleSelection = composingQueuedMessageInFlow ? queueScheduleOverride : flowScheduleSelected;
        int selectedDelay = hasScheduleSelection
            ? (composingQueuedMessageInFlow ? QueueFlowRequestedDelayMinutes() : missionManager.RequestedSendDelayMinutes)
            : -1;
        RefreshOptionButton(sendDelayButtons, 0, "지금", selectedDelay, canChange);
        RefreshOptionButton(sendDelayButtons, 5, "5분", selectedDelay, canChange);
        RefreshOptionButton(sendDelayButtons, 15, "15분", selectedDelay, canChange);
        RefreshOptionButton(sendDelayButtons, 30, "30분", selectedDelay, canChange);
    }

    private void RefreshMessageSizeButtons()
    {
        if (missionManager == null)
        {
            return;
        }

        bool canChange = missionManager.Phase == MissionPhase.Idle;
        RefreshOptionButton(messageSizeButtons, 32, FormatKilobytesAsBytes(32), missionManager.MessageSizeKb, canChange);
        RefreshOptionButton(messageSizeButtons, 128, FormatKilobytesAsBytes(128), missionManager.MessageSizeKb, canChange);
        RefreshOptionButton(messageSizeButtons, 512, FormatKilobytesAsBytes(512), missionManager.MessageSizeKb, canChange);
    }

    private void RefreshOptionButton(Dictionary<int, Button> buttons, int value, string label, int selectedValue, bool canChange)
    {
        if (!buttons.ContainsKey(value))
        {
            return;
        }

        Button button = buttons[value];
        bool selected = value == selectedValue;
        button.interactable = canChange;
        SetButtonLabel(button, label);
        SetButtonVisual(button, selected ? ButtonVisual.Selected : canChange ? ButtonVisual.Secondary : ButtonVisual.Disabled);
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
        if (color.a > 0.08f)
        {
            CreatePanelAccent(panelObject.transform);
        }
        return rect;
    }

    private void CreatePanelAccent(Transform parent)
    {
        GameObject accentObject = new GameObject("PanelAccent", typeof(RectTransform), typeof(Image));
        accentObject.transform.SetParent(parent, false);
        RectTransform accentRect = accentObject.GetComponent<RectTransform>();
        accentRect.anchorMin = new Vector2(0f, 1f);
        accentRect.anchorMax = new Vector2(1f, 1f);
        accentRect.offsetMin = new Vector2(12f, -2f);
        accentRect.offsetMax = new Vector2(-12f, 0f);
        Image accentImage = accentObject.GetComponent<Image>();
        accentImage.color = new Color(0.38f, 0.9f, 1f, 0.36f);
        accentImage.raycastTarget = false;
    }

    private void MakeDraggable(RectTransform dragArea, RectTransform target)
    {
        if (dragArea == null)
        {
            return;
        }

        UIDragHandle handle = dragArea.GetComponent<UIDragHandle>();
        if (handle == null)
        {
            handle = dragArea.gameObject.AddComponent<UIDragHandle>();
        }
        handle.Target = target != null ? target : dragArea;
        handle.PersistKey = PanelPersistKey(handle.Target);
    }

    private void MakeDraggable(RectTransform panel)
    {
        MakeDraggable(panel, panel);
    }

    private void MakeResizable(RectTransform panel)
    {
        if (panel == null)
        {
            return;
        }
        if (panel.Find("ResizeHandle") != null)
        {
            return;
        }

        GameObject handleObject = new GameObject("ResizeHandle", typeof(RectTransform), typeof(Image), typeof(UIResizeHandle));
        handleObject.transform.SetParent(panel, false);
        RectTransform rect = handleObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(1f, 0f);
        rect.anchorMax = new Vector2(1f, 0f);
        rect.pivot = new Vector2(1f, 0f);
        rect.anchoredPosition = new Vector2(-6f, 6f);
        rect.sizeDelta = new Vector2(18f, 18f);
        Image handleImage = handleObject.GetComponent<Image>();
        handleImage.color = new Color(0.7f, 0.95f, 1f, 0f);
        handleImage.raycastTarget = true;
        UIResizeHandle resizeHandle = handleObject.GetComponent<UIResizeHandle>();
        resizeHandle.Target = panel;
        resizeHandle.MinSize = PanelMinimumSize(panel);
        resizeHandle.PersistKey = PanelPersistKey(panel);
    }

    private Vector2 PanelMinimumSize(RectTransform panel)
    {
        if (panel == null)
        {
            return new Vector2(260f, 180f);
        }

        string panelName = panel.name;
        if (panelName == "RequestFlowPanel")
        {
            return new Vector2(540f, 640f);
        }
        if (panelName == "QueuedMessagePanel")
        {
            return new Vector2(600f, 590f);
        }
        if (panelName == "CommunicationResultPanel")
        {
            return new Vector2(470f, 340f);
        }
        if (panelName == "CommunicationStatusPanel")
        {
            return new Vector2(380f, 150f);
        }
        if (panelName == "CommunicationTimelinePanel")
        {
            return new Vector2(420f, 68f);
        }
        if (panelName == "SentMessageHistoryPanel")
        {
            return new Vector2(620f, 500f);
        }
        if (panelName == "SatelliteListPanel")
        {
            return new Vector2(380f, 520f);
        }

        return new Vector2(300f, 220f);
    }

    private void ClampPopupToParent(RectTransform popup)
    {
        if (popup == null || popup.parent == null)
        {
            return;
        }

        RectTransform parent = popup.parent as RectTransform;
        if (parent == null)
        {
            return;
        }

        const float margin = 12f;
        Vector2 offsetMin = popup.offsetMin;
        Vector2 offsetMax = popup.offsetMax;
        float parentHeight = parent.rect.height;
        if (parentHeight < 80f)
        {
            return;
        }
        float popupBottom = parentHeight + offsetMin.y;
        float popupTop = parentHeight + offsetMax.y;

        if (popupBottom < margin)
        {
            float delta = margin - popupBottom;
            offsetMin.y += delta;
            offsetMax.y += delta;
        }
        if (popupTop > parentHeight - margin)
        {
            float delta = popupTop - (parentHeight - margin);
            offsetMin.y -= delta;
            offsetMax.y -= delta;
        }

        offsetMin.x = Mathf.Max(offsetMin.x, margin);
        offsetMax.x = Mathf.Min(offsetMax.x, -margin);
        popup.offsetMin = offsetMin;
        popup.offsetMax = offsetMax;
    }

    private string PanelPersistKey(RectTransform panel)
    {
        return panel == null ? string.Empty : PanelLayoutPersistPrefix + panel.name;
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
        text.color = new Color(0.94f, 0.98f, 1f, 0.96f);
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        text.raycastTarget = false;
        return text;
    }

    private Font CreateUIFont()
    {
        string[] koreanFonts =
        {
            "Noto Sans CJK KR",
            "Noto Sans CJK",
            "Noto Sans KR",
            "NanumGothic",
            "Malgun Gothic",
            "Apple SD Gothic Neo",
            "Arial Unicode MS"
        };
        Font koreanFont = Font.CreateDynamicFontFromOSFont(koreanFonts, 16);
        return koreanFont != null ? koreanFont : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
    }

    private Button CreateListButton(Transform parent, string label, UnityAction action)
    {
        GameObject buttonObject = new GameObject("SatelliteButton", typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement));
        buttonObject.transform.SetParent(parent, false);
        LayoutElement layoutElement = buttonObject.GetComponent<LayoutElement>();
        layoutElement.preferredHeight = 122f;
        Button button = buttonObject.GetComponent<Button>();
        button.onClick.AddListener(action);

        Image image = buttonObject.GetComponent<Image>();
        image.color = new Color(0.055f, 0.085f, 0.13f, 0.98f);
        button.targetGraphic = image;

        Text text = CreateText(buttonObject.transform, label, 14, TextAnchor.MiddleLeft);
        text.rectTransform.offsetMin = new Vector2(16f, 8f);
        text.rectTransform.offsetMax = new Vector2(-14f, -8f);
        GameObject barObject = new GameObject("SelectionBar", typeof(RectTransform), typeof(Image));
        barObject.transform.SetParent(buttonObject.transform, false);
        RectTransform barRect = barObject.GetComponent<RectTransform>();
        barRect.anchorMin = new Vector2(0f, 0f);
        barRect.anchorMax = new Vector2(0f, 1f);
        barRect.offsetMin = new Vector2(0f, 0f);
        barRect.offsetMax = new Vector2(5f, 0f);
        Image barImage = barObject.GetComponent<Image>();
        barImage.color = new Color(0.24f, 1f, 0.84f, 0.08f);
        barImage.raycastTarget = false;
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
        button.transition = Selectable.Transition.ColorTint;
        button.targetGraphic = image;
        button.onClick.AddListener(action);

        Text text = CreateText(buttonObject.transform, label, 17, TextAnchor.MiddleCenter);
        text.rectTransform.offsetMin = Vector2.zero;
        text.rectTransform.offsetMax = Vector2.zero;
        SetButtonVisual(button, ButtonVisual.Secondary);
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

    private Image CreateAnchoredProgressFill(Transform parent, string objectName, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax, Color fillColor)
    {
        GameObject root = new GameObject(objectName, typeof(RectTransform), typeof(Image));
        root.transform.SetParent(parent, false);
        RectTransform rootRect = root.GetComponent<RectTransform>();
        rootRect.anchorMin = anchorMin;
        rootRect.anchorMax = anchorMax;
        rootRect.offsetMin = offsetMin;
        rootRect.offsetMax = offsetMax;
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

    private ColorBlock DropdownToggleColors()
    {
        ColorBlock colors = ColorBlock.defaultColorBlock;
        colors.normalColor = new Color(0.08f, 0.11f, 0.16f, 0.98f);
        colors.highlightedColor = new Color(0.14f, 0.28f, 0.42f, 1f);
        colors.pressedColor = new Color(0.08f, 0.2f, 0.32f, 1f);
        colors.selectedColor = new Color(0.06f, 0.36f, 0.32f, 1f);
        colors.disabledColor = new Color(0.04f, 0.05f, 0.07f, 0.6f);
        return colors;
    }

    private Dropdown CreateCompactDropdown(Transform parent, string objectName, int fontSize, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
    {
        GameObject dropdownObject = new GameObject(objectName, typeof(RectTransform), typeof(Image), typeof(Dropdown));
        dropdownObject.transform.SetParent(parent, false);
        RectTransform rect = dropdownObject.GetComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;
        dropdownObject.GetComponent<Image>().color = new Color(0.08f, 0.11f, 0.16f, 0.98f);

        Dropdown dropdown = dropdownObject.GetComponent<Dropdown>();
        dropdown.colors = DropdownToggleColors();
        dropdown.targetGraphic = dropdownObject.GetComponent<Image>();
        Text label = CreateText(dropdownObject.transform, string.Empty, fontSize, TextAnchor.MiddleLeft);
        label.rectTransform.offsetMin = new Vector2(10f, 0f);
        label.rectTransform.offsetMax = new Vector2(-24f, 0f);
        dropdown.captionText = label;

        GameObject templateObject = new GameObject("Template", typeof(RectTransform), typeof(Image), typeof(ScrollRect));
        templateObject.transform.SetParent(dropdownObject.transform, false);
        RectTransform templateRect = templateObject.GetComponent<RectTransform>();
        templateRect.anchorMin = new Vector2(0f, 0f);
        templateRect.anchorMax = new Vector2(1f, 0f);
        templateRect.pivot = new Vector2(0.5f, 1f);
        templateRect.anchoredPosition = Vector2.zero;
        templateRect.sizeDelta = new Vector2(0f, 150f);
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
        contentRect.sizeDelta = new Vector2(0f, 150f);

        GameObject itemObject = new GameObject("Item", typeof(RectTransform), typeof(Toggle), typeof(Image));
        itemObject.transform.SetParent(contentObject.transform, false);
        RectTransform itemRect = itemObject.GetComponent<RectTransform>();
        itemRect.sizeDelta = new Vector2(0f, 32f);
        itemObject.GetComponent<Image>().color = new Color(0.08f, 0.11f, 0.16f, 0.98f);
        Text itemLabel = CreateText(itemObject.transform, string.Empty, fontSize, TextAnchor.MiddleLeft);
        itemLabel.rectTransform.offsetMin = new Vector2(10f, 0f);
        itemLabel.rectTransform.offsetMax = new Vector2(-10f, 0f);

        Toggle itemToggle = itemObject.GetComponent<Toggle>();
        itemToggle.targetGraphic = itemObject.GetComponent<Image>();
        itemToggle.colors = DropdownToggleColors();
        dropdown.template = templateRect;
        dropdown.itemText = itemLabel;
        ScrollRect templateScroll = templateObject.GetComponent<ScrollRect>();
        templateScroll.content = contentRect;
        templateScroll.viewport = viewportRect;
        templateScroll.movementType = ScrollRect.MovementType.Clamped;
        templateScroll.inertia = true;
        templateScroll.scrollSensitivity = 72f;
        return dropdown;
    }

    private InputField CreateInputField(Transform parent, string objectName, string placeholderText, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
    {
        GameObject inputObject = new GameObject(objectName, typeof(RectTransform), typeof(Image), typeof(InputField));
        inputObject.transform.SetParent(parent, false);
        RectTransform rect = inputObject.GetComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;
        inputObject.GetComponent<Image>().color = new Color(0.08f, 0.11f, 0.16f, 0.98f);

        Text text = CreateText(inputObject.transform, string.Empty, 15, TextAnchor.UpperLeft);
        text.rectTransform.offsetMin = new Vector2(12f, 8f);
        text.rectTransform.offsetMax = new Vector2(-12f, -8f);
        text.supportRichText = false;
        text.verticalOverflow = VerticalWrapMode.Overflow;

        Text placeholder = CreateText(inputObject.transform, placeholderText, 15, TextAnchor.UpperLeft);
        placeholder.rectTransform.offsetMin = new Vector2(12f, 8f);
        placeholder.rectTransform.offsetMax = new Vector2(-12f, -8f);
        placeholder.color = new Color(0.78f, 0.86f, 1f, 0.42f);

        InputField input = inputObject.GetComponent<InputField>();
        input.targetGraphic = inputObject.GetComponent<Image>();
        input.selectionColor = new Color(0.24f, 0.82f, 1f, 0.45f);
        input.caretColor = new Color(0.24f, 1f, 0.84f, 1f);
        ColorBlock inputColors = input.colors;
        inputColors.highlightedColor = new Color(0.14f, 0.28f, 0.42f, 1f);
        inputColors.selectedColor = new Color(0.10f, 0.34f, 0.50f, 1f);
        input.colors = inputColors;
        input.textComponent = text;
        input.placeholder = placeholder;
        input.contentType = InputField.ContentType.Standard;
        input.inputType = InputField.InputType.Standard;
        input.characterValidation = InputField.CharacterValidation.None;
        input.keyboardType = TouchScreenKeyboardType.Default;
        input.shouldHideMobileInput = false;
        input.lineType = InputField.LineType.MultiLineNewline;
        input.characterLimit = 200000;
        LegacyInputFieldImeSupport imeSupport = inputObject.AddComponent<LegacyInputFieldImeSupport>();
        imeSupport.ForceDubeolsikFallback = objectName == "MessageInput";
        return input;
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
        dropdown.colors = DropdownToggleColors();
        dropdown.targetGraphic = dropdownObject.GetComponent<Image>();
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
        itemObject.GetComponent<Image>().color = new Color(0.08f, 0.11f, 0.16f, 0.98f);
        Text itemLabel = CreateText(itemObject.transform, string.Empty, 17, TextAnchor.MiddleLeft);
        itemLabel.rectTransform.offsetMin = new Vector2(12f, 0f);
        itemLabel.rectTransform.offsetMax = new Vector2(-12f, 0f);

        Toggle itemToggle = itemObject.GetComponent<Toggle>();
        itemToggle.targetGraphic = itemObject.GetComponent<Image>();
        itemToggle.colors = DropdownToggleColors();
        dropdown.template = templateRect;
        dropdown.itemText = itemLabel;
        ScrollRect templateScroll = templateObject.GetComponent<ScrollRect>();
        templateScroll.content = contentRect;
        templateScroll.viewport = viewportRect;
        templateScroll.movementType = ScrollRect.MovementType.Clamped;
        templateScroll.inertia = true;
        templateScroll.scrollSensitivity = 72f;
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

    private string FormatSeconds(int seconds)
    {
        if (seconds <= 0)
        {
            return "계산 중";
        }
        if (seconds >= 3600)
        {
            return (seconds / 3600) + "시간 " + ((seconds % 3600) / 60) + "분";
        }
        if (seconds >= 60)
        {
            return (seconds / 60) + "분 " + (seconds % 60) + "초";
        }
        return seconds + "초";
    }

    private string FormatUsd(float amount)
    {
        return "$" + amount.ToString("0.00");
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

    private void SetButtonAnchors(Button button, Vector2 anchorMin, Vector2 anchorMax)
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

        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private void SetButtonTextSize(Button button, int size)
    {
        if (button == null)
        {
            return;
        }

        Text text = button.GetComponentInChildren<Text>();
        if (text != null)
        {
            text.fontSize = size;
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
        Color pressed;
        Color disabled;
        if (visual == ButtonVisual.Primary)
        {
            normal = new Color(0.06f, 0.48f, 0.68f, 1f);
            highlighted = new Color(0.16f, 0.68f, 0.88f, 1f);
            pressed = new Color(0.04f, 0.34f, 0.52f, 1f);
            disabled = new Color(0.07f, 0.10f, 0.14f, 0.56f);
        }
        else if (visual == ButtonVisual.Cta)
        {
            normal = new Color(0.08f, 0.62f, 0.48f, 1f);
            highlighted = new Color(0.22f, 0.86f, 0.66f, 1f);
            pressed = new Color(0.04f, 0.42f, 0.34f, 1f);
            disabled = new Color(0.07f, 0.24f, 0.20f, 0.78f);
        }
        else if (visual == ButtonVisual.Selected)
        {
            normal = new Color(0.10f, 0.42f, 0.92f, 1f);
            highlighted = new Color(0.18f, 0.60f, 1f, 1f);
            pressed = new Color(0.04f, 0.26f, 0.60f, 1f);
            disabled = new Color(0.08f, 0.24f, 0.50f, 0.80f);
        }
        else if (visual == ButtonVisual.Secondary)
        {
            normal = new Color(0.075f, 0.18f, 0.30f, 0.96f);
            highlighted = new Color(0.12f, 0.30f, 0.48f, 1f);
            pressed = new Color(0.05f, 0.13f, 0.22f, 1f);
            disabled = new Color(0.07f, 0.10f, 0.14f, 0.55f);
        }
        else
        {
            normal = new Color(0.07f, 0.10f, 0.14f, 0.55f);
            highlighted = normal;
            pressed = normal;
            disabled = normal;
        }

        if (highContrastMode)
        {
            if (visual == ButtonVisual.Disabled)
            {
                normal = new Color(0.04f, 0.05f, 0.07f, 0.84f);
                highlighted = normal;
                pressed = normal;
                disabled = normal;
            }
            else if (visual == ButtonVisual.Selected)
            {
                normal = new Color(0.95f, 0.82f, 0.16f, 1f);
                highlighted = new Color(1f, 0.94f, 0.34f, 1f);
                pressed = new Color(0.76f, 0.62f, 0.08f, 1f);
            }
            else
            {
                normal = new Color(0.02f, 0.36f, 0.72f, 1f);
                highlighted = new Color(0.10f, 0.58f, 0.98f, 1f);
                pressed = new Color(0.02f, 0.24f, 0.52f, 1f);
            }
        }

        ColorBlock colors = button.colors;
        colors.normalColor = normal;
        colors.highlightedColor = highlighted;
        colors.selectedColor = Color.Lerp(highlighted, Color.white, 0.18f);
        colors.pressedColor = pressed;
        colors.disabledColor = disabled;
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.08f;
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
        return OrbitShareDestinations.Label(targetName);
    }
}
