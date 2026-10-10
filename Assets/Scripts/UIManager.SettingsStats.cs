using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// 설정/실패 UI: 접근성, 실패 재시도 패널을 관리합니다.
public partial class UIManager
{
    private void CreateFailurePanel(Transform parent)
    {
        failurePanel = CreatePanel(parent, "FailurePanel", new Vector2(0.33f, 0.34f), new Vector2(0.585f, 0.64f), new Color(0.12f, 0.035f, 0.045f, 0.97f));
        MakeDraggable(failurePanel);
        Text heading = CreateText(failurePanel, "전송 실패", 22, TextAnchor.UpperLeft);
        heading.fontStyle = FontStyle.Bold;
        heading.rectTransform.anchorMin = new Vector2(0f, 1f);
        heading.rectTransform.anchorMax = new Vector2(1f, 1f);
        heading.rectTransform.pivot = new Vector2(0.5f, 1f);
        heading.rectTransform.anchoredPosition = new Vector2(0f, -18f);
        heading.rectTransform.sizeDelta = new Vector2(-64f, 32f);
        failureDetailText = CreateText(failurePanel, string.Empty, 14, TextAnchor.UpperLeft);
        failureDetailText.lineSpacing = 1.08f;
        failureDetailText.rectTransform.offsetMin = new Vector2(22f, 72f);
        failureDetailText.rectTransform.offsetMax = new Vector2(-22f, -62f);
        CreateButton(failurePanel, "다시 시도", new Vector2(0f, 0f), new Vector2(0.48f, 0f), new Vector2(22f, 18f), new Vector2(-6f, 56f), delegate { ApplyFailureRetrySuggestion(); });
        CreateButton(failurePanel, "닫기", new Vector2(0.5f, 0f), new Vector2(1f, 0f), new Vector2(6f, 18f), new Vector2(-22f, 56f), delegate { failurePanelDismissed = true; failurePanel.gameObject.SetActive(false); });
        CreateButton(failurePanel, "닫기", new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-80f, -48f), new Vector2(-14f, -12f), delegate { failurePanelDismissed = true; failurePanel.gameObject.SetActive(false); });
        failurePanel.gameObject.SetActive(false);
    }

    private void CreateSettingsPanel(Transform parent)
    {
        settingsPanel = CreatePanel(parent, "SettingsPanel", new Vector2(0.315f, 0.25f), new Vector2(0.605f, 0.75f), new Color(0.02f, 0.06f, 0.1f, 0.96f));
        MakeDraggable(settingsPanel);
        Text heading = CreateText(settingsPanel, "설정", 22, TextAnchor.UpperLeft);
        heading.fontStyle = FontStyle.Bold;
        heading.rectTransform.anchorMin = new Vector2(0f, 1f);
        heading.rectTransform.anchorMax = new Vector2(1f, 1f);
        heading.rectTransform.pivot = new Vector2(0.5f, 1f);
        heading.rectTransform.anchoredPosition = new Vector2(0f, -18f);
        heading.rectTransform.sizeDelta = new Vector2(-64f, 32f);

        settingsText = CreateText(settingsPanel, "", 13, TextAnchor.UpperLeft);
        settingsText.lineSpacing = 1.08f;
        settingsText.rectTransform.offsetMin = new Vector2(22f, 174f);
        settingsText.rectTransform.offsetMax = new Vector2(-22f, -64f);
        settingsText.color = new Color(0.86f, 0.95f, 1f, 0.86f);

        CreateButton(settingsPanel, "기본값 복원", new Vector2(0f, 0f), new Vector2(0.48f, 0f), new Vector2(22f, 96f), new Vector2(-6f, 134f), delegate { ResetUiPreferences(); });
        CreateButton(settingsPanel, "고대비", new Vector2(0.5f, 0f), new Vector2(1f, 0f), new Vector2(6f, 96f), new Vector2(-22f, 134f), delegate { ToggleHighContrast(); });
        CreateButton(settingsPanel, "UI 크기", new Vector2(0f, 0f), new Vector2(0.48f, 0f), new Vector2(22f, 54f), new Vector2(-6f, 92f), delegate { CycleUiScale(); });
        CreateButton(settingsPanel, "패널 정리", new Vector2(0.5f, 0f), new Vector2(1f, 0f), new Vector2(6f, 54f), new Vector2(-22f, 92f), delegate { ResetPanelLayout(); });
        CreateButton(settingsPanel, "전체", new Vector2(0f, 0f), new Vector2(0.24f, 0f), new Vector2(22f, 136f), new Vector2(-3f, 170f), delegate { FocusCameraView("overview"); });
        CreateButton(settingsPanel, "지구", new Vector2(0.25f, 0f), new Vector2(0.49f, 0f), new Vector2(3f, 136f), new Vector2(-3f, 170f), delegate { FocusCameraView("earth"); });
        CreateButton(settingsPanel, "위성", new Vector2(0.50f, 0f), new Vector2(0.74f, 0f), new Vector2(3f, 136f), new Vector2(-3f, 170f), delegate { FocusCameraView("satellite"); });
        CreateButton(settingsPanel, "경로", new Vector2(0.75f, 0f), new Vector2(1f, 0f), new Vector2(3f, 136f), new Vector2(-22f, 170f), delegate { FocusCameraView("mission"); });
        CreateButton(settingsPanel, "닫기", new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(22f, 14f), new Vector2(-22f, 50f), delegate { settingsPanel.gameObject.SetActive(false); RefreshPanelLauncherButtons(); });
        CreateButton(settingsPanel, "닫기", new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-80f, -48f), new Vector2(-14f, -12f), delegate { settingsPanel.gameObject.SetActive(false); RefreshPanelLauncherButtons(); });
        settingsPanel.gameObject.SetActive(false);
    }

    private void RefreshSettingsPanel()
    {
        if (settingsText == null || missionManager == null)
        {
            return;
        }

        settingsText.text =
            "기본 설정\n" +
            "• 고대비: " + (highContrastMode ? "켬" : "끔") + "\n" +
            "• UI 크기: " + Mathf.RoundToInt(userUiScale * 100f) + "%\n" +
            "• 패널 위치: 드래그로 이동, '패널 정리'로 초기화\n\n" +
            "보기 설정\n" +
            "• 카메라 보기: 전체 / 지구 / 위성 / 경로\n" +
            "• 지구 텍스처: " + EarthTextureStatusLabel() + "\n\n" +
            "위성 데이터\n" +
            "• TLE 상태: " + TleStatusLabel() + "\n\n" +
            "설정 변경은 즉시 저장됩니다.";
    }

    private string QaStatusSummary()
    {
        int satelliteCount = satelliteManager != null && satelliteManager.Satellites != null ? satelliteManager.Satellites.Count : 0;
        int targetCount = missionManager != null && missionManager.Targets != null ? missionManager.Targets.Count : 0;
        return "QA 상태: 위성 " + satelliteCount + "개 · 목적지 " + targetCount + "개 · 텍스처 " + EarthTextureStatusLabel();
    }

    private string EarthTextureStatusLabel()
    {
        bool day = Resources.Load<Texture2D>("OrbitShare/Earth/EarthDaySSS") != null;
        bool night = Resources.Load<Texture2D>("OrbitShare/Earth/EarthNightSSS") != null;
        bool clouds = Resources.Load<Texture2D>("OrbitShare/Earth/EarthCloudsSSS") != null;
        if (day && night && clouds)
        {
            return "외부 텍스처 로드됨";
        }
        if (day || night || clouds)
        {
            return "일부 로드됨";
        }
        return "기본 생성 텍스처";
    }

    private void RefreshFailurePanel()
    {
        if (failurePanel == null || failureDetailText == null || missionManager == null)
        {
            return;
        }

        bool failed = missionManager.Phase == MissionPhase.Failed;
        if (!failed)
        {
            failurePanelDismissed = false;
            if (failurePanel.gameObject.activeSelf)
            {
                failurePanel.gameObject.SetActive(false);
            }
            return;
        }

        failureDetailText.text =
            (string.IsNullOrEmpty(missionManager.LastFailureReason) ? "원인을 확인하지 못했습니다." : missionManager.LastFailureReason) + "\n\n" +
            (string.IsNullOrEmpty(missionManager.SuggestedRetryHint) ? "다시 시도할 수 있습니다." : missionManager.SuggestedRetryHint) + "\n" +
            "메시지 크기: " + PayloadKbText(missionManager.MessageSizeKb) + "\n" +
            "선택 위성: " + (string.IsNullOrEmpty(missionManager.SelectedSatelliteName) ? "없음" : missionManager.SelectedSatelliteName);

        if (!failurePanelDismissed)
        {
            failurePanel.gameObject.SetActive(true);
            failurePanel.SetAsLastSibling();
        }
    }

    private void LoadUiPreferences()
    {
        userUiScale = PlayerPrefs.GetFloat("OrbitShare.UI.Scale", 1f);
        userUiScale = Mathf.Clamp(userUiScale, 0.85f, 1.18f);
        highContrastMode = PlayerPrefs.GetInt("OrbitShare.UI.HighContrast", 0) == 1;
        messageKoreanInputMode = PlayerPrefs.GetInt("OrbitShare.UI.MessageKoreanInputMode", 1) == 1;
    }

    private void SaveUiPreferences()
    {
        PlayerPrefs.SetFloat("OrbitShare.UI.Scale", userUiScale);
        PlayerPrefs.SetInt("OrbitShare.UI.HighContrast", highContrastMode ? 1 : 0);
        PlayerPrefs.SetInt("OrbitShare.UI.MessageKoreanInputMode", messageKoreanInputMode ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void ApplyUiScale()
    {
        if (canvasScaler == null)
        {
            return;
        }

        float compact = Screen.width > 0 && Screen.width < 1280 ? 0.92f : 1f;
        float scale = Mathf.Clamp(userUiScale, 0.85f, 1.18f) * compact;
        canvasScaler.referenceResolution = new Vector2(1600f / scale, 900f / scale);
        canvasScaler.matchWidthOrHeight = Screen.width > 0 && Screen.width < 1280 ? 0.7f : 0.5f;
    }

    private void CycleUiScale()
    {
        if (userUiScale < 0.95f)
        {
            userUiScale = 1f;
        }
        else if (userUiScale < 1.1f)
        {
            userUiScale = 1.15f;
        }
        else
        {
            userUiScale = 0.9f;
        }
        ApplyUiScale();
        SaveUiPreferences();
        RefreshSettingsPanel();
        ShowToast("UI 크기 " + Mathf.RoundToInt(userUiScale * 100f) + "%");
    }

    private void ResetUiPreferences()
    {
        userUiScale = 1f;
        highContrastMode = false;
        messageKoreanInputMode = true;
        ApplyUiScale();
        SaveUiPreferences();
        ApplyVisualAccessibility();
        RefreshSettingsPanel();
        ShowToast("기본 설정으로 복원했습니다.");
    }

    private string TleStatusLabel()
    {
        RealSatelliteVisualizer visualizer = Object.FindObjectOfType<RealSatelliteVisualizer>();
        if (visualizer == null)
        {
            return "대기";
        }
        if (visualizer.LastFetchFromFallback)
        {
            return "내장 샘플";
        }
        if (visualizer.LastFetchFromCache)
        {
            return string.IsNullOrEmpty(visualizer.LastError) ? "캐시" : "캐시 · 최근 요청 실패";
        }
        if (!string.IsNullOrEmpty(visualizer.LastError) && visualizer.DisplayedCount == 0)
        {
            return "수신 실패";
        }
        return visualizer.DisplayedCount > 0 ? "실시간" : "준비 중";
    }

    private void ToggleHighContrast()
    {
        highContrastMode = !highContrastMode;
        SaveUiPreferences();
        ApplyVisualAccessibility();
        RefreshAll();
    }

    private void ApplyVisualAccessibility()
    {
        if (missionVisualController == null)
        {
            missionVisualController = FindObjectOfType<MissionVisualController>();
        }
        if (missionVisualController != null)
        {
            missionVisualController.SetHighContrastMode(highContrastMode);
            if (satelliteManager != null && satelliteManager.SelectedSatellite != null && satelliteOrbitTransformLookup(satelliteManager.SelectedSatellite.satelliteName) != null)
            {
                missionVisualController.SetSelectedSatelliteTransform(satelliteOrbitTransformLookup(satelliteManager.SelectedSatellite.satelliteName));
            }
        }
    }

    private void ToggleSettingsPanel()
    {
        if (settingsPanel == null)
        {
            return;
        }

        bool next = !settingsPanel.gameObject.activeSelf;
        settingsPanel.gameObject.SetActive(next);
        if (next)
        {
            settingsPanel.SetAsLastSibling();
            RefreshSettingsPanel();
        }
        RefreshPanelLauncherButtons();
    }

    private void ApplyFailureRetrySuggestion()
    {
        if (missionManager == null || missionManager.Phase != MissionPhase.Failed)
        {
            ShowToast("재시도할 실패 기록이 없습니다.");
            return;
        }
        missionManager.RetryFailedMission();
        ShowToast("다시 시도합니다.");
        RefreshAll();
    }
}
