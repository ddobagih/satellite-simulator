using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

// 위성망 추천 UI: 추천 목록, 선택 비교, 필터, 위성 카드 표시를 관리합니다.
public partial class UIManager
{
    private const float SatelliteRowRefreshHeight = 122f;
    private const int SatelliteRowRefreshBufferRows = 2;
    private const float SatelliteListScrollReorderHoldSeconds = 0.9f;
    private const float SatelliteListScrollVelocityHoldThreshold = 8f;
    private const int SatelliteCommunicationWindowReorderThresholdSeconds = 6;

    private string satelliteHoveredSatelliteName = string.Empty;
    private float lastSatelliteListScrollChangedTime = -10f;
    private readonly HashSet<string> satelliteRowsWithHoverFeedback = new HashSet<string>();
    private readonly Dictionary<string, int> satelliteDisplayedWindowState = new Dictionary<string, int>();
    private readonly Dictionary<string, int> satelliteDisplayedWindowSeconds = new Dictionary<string, int>();
    private readonly Dictionary<string, float> satelliteDisplayedWindowUpdatedAt = new Dictionary<string, float>();
    private readonly Dictionary<string, int> satelliteDisplayedRouteTimeSeconds = new Dictionary<string, int>();
    private readonly Dictionary<string, float> satelliteDisplayedRouteTimeUpdatedAt = new Dictionary<string, float>();
    private readonly Dictionary<string, float> satelliteDisplayedRouteScores = new Dictionary<string, float>();
    private readonly Dictionary<string, float> satelliteDisplayedRouteScoreUpdatedAt = new Dictionary<string, float>();
    private readonly Dictionary<string, SatelliteWindowSortState> satelliteStableSortWindows = new Dictionary<string, SatelliteWindowSortState>();

    private void CreateSatelliteListPanel(Transform parent)
    {
        RectTransform panel = CreatePanel(parent, "SatelliteListPanel", new Vector2(0.015f, 0.08f), new Vector2(0.30f, 0.89f), new Color(0.025f, 0.045f, 0.075f, 0.96f));
        satelliteListPanel = panel;
        MakeDraggable(panel);
        Text heading = CreateText(panel, "위성망 추천", 23, TextAnchor.UpperLeft);
        heading.rectTransform.anchorMin = new Vector2(0f, 1f);
        heading.rectTransform.anchorMax = new Vector2(1f, 1f);
        heading.rectTransform.pivot = new Vector2(0.5f, 1f);
        heading.rectTransform.anchoredPosition = new Vector2(0f, -16f);
        heading.rectTransform.sizeDelta = new Vector2(-28f, 32f);

        satelliteListSummaryText = CreateText(panel, "전체 위성 개수 0개 중 통신 가능 위성 0개", 14, TextAnchor.UpperLeft);
        satelliteListSummaryText.color = new Color(0.78f, 0.88f, 1f, 0.78f);
        satelliteListSummaryText.rectTransform.anchorMin = new Vector2(0f, 1f);
        satelliteListSummaryText.rectTransform.anchorMax = new Vector2(1f, 1f);
        satelliteListSummaryText.rectTransform.pivot = new Vector2(0.5f, 1f);
        satelliteListSummaryText.rectTransform.anchoredPosition = new Vector2(0f, -50f);
        satelliteListSummaryText.rectTransform.sizeDelta = new Vector2(-28f, 18f);

        CreateFilterButton(panel, "추천", SatelliteListFilter.All, 0f, 0);
        CreateFilterButton(panel, "비용", SatelliteListFilter.LowCost, 0.33f, 0);
        CreateFilterButton(panel, "통신 가능 시간", SatelliteListFilter.LowLatency, 0.66f, 0);

        satelliteSearchInputField = CreateInputField(panel, "SatelliteSearch", "위성망 검색", new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(14f, -150f), new Vector2(-14f, -108f));
        satelliteSearchInputField.lineType = InputField.LineType.SingleLine;
        satelliteSearchInputField.onValueChanged.AddListener(delegate
        {
            satelliteSearchQuery = satelliteSearchInputField.text ?? string.Empty;
            lastSatelliteSearchChangedTime = Time.unscaledTime;
            nextSatelliteListReorderTime = 0f;
            RefreshSatelliteList(false, false);
        });

        GameObject scrollArea = new GameObject("SatelliteListScrollArea", typeof(RectTransform), typeof(Image), typeof(ScrollRect));
        scrollArea.transform.SetParent(panel, false);
        RectTransform scrollAreaRect = scrollArea.GetComponent<RectTransform>();
        scrollAreaRect.anchorMin = Vector2.zero;
        scrollAreaRect.anchorMax = Vector2.one;
        scrollAreaRect.offsetMin = new Vector2(14f, 14f);
        scrollAreaRect.offsetMax = new Vector2(-14f, -160f);
        scrollArea.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.03f);

        GameObject viewport = new GameObject("SatelliteListViewport", typeof(RectTransform), typeof(Image), typeof(Mask));
        viewport.transform.SetParent(scrollArea.transform, false);
        RectTransform viewportRect = viewport.GetComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.offsetMin = Vector2.zero;
        viewportRect.offsetMax = Vector2.zero;
        viewport.GetComponent<Image>().color = Color.white;
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
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.spacing = 7f;
        layout.childControlHeight = true;
        layout.childControlWidth = true;
        layout.childForceExpandHeight = false;
        layout.childForceExpandWidth = true;
        ContentSizeFitter fitter = content.GetComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        ScrollRect scrollRect = scrollArea.GetComponent<ScrollRect>();
        scrollRect.content = satelliteListContent;
        scrollRect.viewport = viewportRect;
        scrollRect.vertical = true;
        scrollRect.horizontal = false;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;
        scrollRect.inertia = true;
        scrollRect.decelerationRate = 0.12f;
        scrollRect.scrollSensitivity = 110f;
        scrollRect.onValueChanged.AddListener(delegate { lastSatelliteListScrollChangedTime = Time.unscaledTime; });
        satelliteListScrollRect = scrollRect;
        CreateButton(panel, "닫기", new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-80f, -48f), new Vector2(-14f, -12f), delegate
        {
            SetSatellitePanelOpen(false, true);
            ShowToast("위성망 추천 창을 숨겼습니다. 하단 '위성망 추천'으로 다시 볼 수 있습니다.");
        });
        panel.gameObject.SetActive(false);
        if (!IsInvoking(nameof(RefreshLiveSatelliteRouteTimes)))
        {
            InvokeRepeating(nameof(RefreshLiveSatelliteRouteTimes), 1f, 1f);
        }
    }

    private void RefreshLiveSatelliteRouteTimes()
    {
        if (satelliteListPanel != null && satelliteListPanel.gameObject.activeInHierarchy)
        {
            if (Time.unscaledTime >= nextSatelliteRecommendationRefreshTime)
            {
                nextSatelliteRecommendationRefreshTime = Time.unscaledTime + SatelliteRecommendationDataRefreshInterval();
                RefreshSatelliteList(ShouldAllowSatelliteListAutoReorder(), true);
            }
        }
        if (detailText != null && detailText.gameObject.activeInHierarchy)
        {
            RefreshDetailPanel();
        }
    }

    private void CreateDetailPanel(Transform parent)
    {
        RectTransform panel = CreatePanel(parent, "SatelliteDetailPanel", new Vector2(0.69f, 0.58f), new Vector2(0.985f, 0.89f), new Color(0.025f, 0.045f, 0.075f, 0.94f));
        Text heading = CreateText(panel, "선택 위성 정보", 23, TextAnchor.UpperLeft);
        heading.rectTransform.anchorMin = new Vector2(0f, 1f);
        heading.rectTransform.anchorMax = new Vector2(1f, 1f);
        heading.rectTransform.pivot = new Vector2(0.5f, 1f);
        heading.rectTransform.anchoredPosition = new Vector2(0f, -18f);
        heading.rectTransform.sizeDelta = new Vector2(-28f, 36f);

        detailText = CreateText(panel, string.Empty, 18, TextAnchor.UpperLeft);
        detailText.lineSpacing = 1.12f;
        detailText.rectTransform.offsetMin = new Vector2(18f, 18f);
        detailText.rectTransform.offsetMax = new Vector2(-18f, -54f);
    }

    private void RefreshSatelliteList()
    {
        RefreshSatelliteList(true, true);
    }

    private void RefreshSatelliteList(bool allowReorder)
    {
        RefreshSatelliteList(allowReorder, true);
    }

    private void RefreshSatelliteList(bool allowReorder, bool forceEstimateRefresh)
    {
        RefreshSatelliteList(allowReorder, forceEstimateRefresh, false);
    }

    private void RefreshSatelliteList(bool allowReorder, bool forceEstimateRefresh, bool forceReorder)
    {
        NormalizeSatelliteFilter();
        RefreshFilterButtons();

        if (satelliteListContent == null || satelliteManager == null)
        {
            return;
        }

        float previousScrollOffset = forceReorder ? 0f : SatelliteListTopScrollOffset();
        string targetName = CurrentTargetName();
        string originName = CurrentOriginName();
        int sendDelay = composingQueuedMessageInFlow ? QueueFlowRequestedDelayMinutes() : missionManager != null ? missionManager.RequestedSendDelayMinutes : 0;
        int payloadKb = CurrentSatellitePayloadKb();
        bool routeContextChanged = SatelliteListRouteContextChanged();
        if (routeContextChanged)
        {
            ClearSatelliteCommunicationWindowCache();
        }
        bool satelliteSelectionLocked = missionManager != null && IsActiveTransferPhase(missionManager.Phase) && !composingQueuedMessageInFlow;
        SatelliteData selectedSatellite = CurrentSatelliteListSelectedSatellite();
        if (satelliteSelectionLocked && missionManager != null && !string.IsNullOrEmpty(missionManager.SelectedSatelliteName))
        {
            SatelliteData missionSatellite = satelliteManager.Satellites.Find(satellite => satellite.satelliteName == missionManager.SelectedSatelliteName);
            if (missionSatellite != null)
            {
                selectedSatellite = missionSatellite;
            }
        }
        float refreshStartTime = Time.realtimeSinceStartup;
        int createdCount = 0;
        int removedCount = 0;
        int labelUpdateCount = 0;
        int visualUpdateCount = 0;
        int reorderCount = 0;
        bool satelliteListLayoutDirty = forceReorder;
        List<SatelliteRouteEstimateRecord> routeEstimates = RefreshSatelliteListEstimateCache(originName, targetName, sendDelay, payloadKb, satelliteFilter, forceEstimateRefresh);
        bool communicationBucketChanged = RefreshDisplayedCommunicationWindowSnapshot(routeEstimates);
        if (communicationBucketChanged)
        {
            satelliteListPendingReorder = true;
        }
        List<SatelliteData> visibleSatellites = SatelliteRowsFromRouteEstimates(routeEstimates, satelliteFilter);
        if (!string.IsNullOrWhiteSpace(satelliteSearchQuery))
        {
            string query = satelliteSearchQuery.Trim().ToLowerInvariant();
            visibleSatellites = visibleSatellites.FindAll(satellite =>
                satellite.satelliteName.ToLowerInvariant().Contains(query)
                || (satellite.ownerName ?? string.Empty).ToLowerInvariant().Contains(query)
                || (satellite.orbitSlot ?? string.Empty).ToLowerInvariant().Contains(query)
                || (satellite.SupportedTargetSummary ?? string.Empty).ToLowerInvariant().Contains(query));
        }
        bool shouldReorder = allowReorder && ShouldApplySatelliteListReorder(forceReorder);
        if (shouldReorder)
        {
            SortSatelliteRowsForFilter(visibleSatellites, targetName, sendDelay, selectedSatellite);
            nextSatelliteListReorderTime = Time.unscaledTime + SatelliteRecommendationReorderInterval();
            satelliteListPendingReorder = false;
        }
        else
        {
            PreserveSatelliteRowOrder(visibleSatellites);
        }
        if (satelliteListSummaryText != null)
        {
            int satelliteCount = satelliteManager != null ? satelliteManager.Satellites.Count : 0;
            int reachableCount = CountReachableRouteEstimates(routeEstimates);
            satelliteListSummaryText.text = "전체 위성 개수 " + satelliteCount + "개 중 통신 가능 위성 " + reachableCount + "개";
        }
        if (visibleSatellites.Count == 0)
        {
            foreach (KeyValuePair<string, Button> pair in satelliteButtons)
            {
                if (pair.Value != null)
                {
                    pair.Value.gameObject.SetActive(false);
                }
            }
            satelliteListDisplayOrder.Clear();
            if (satelliteListEmptyButton == null)
            {
                satelliteListEmptyButton = CreateListButton(satelliteListContent, EmptySatelliteListText(), delegate { ResetSatelliteListFiltersFromEmptyState(); });
                SetButtonVisual(satelliteListEmptyButton, ButtonVisual.Secondary);
            }
            satelliteListEmptyButton.gameObject.SetActive(true);
            satelliteListEmptyButton.interactable = true;
            ConfigureSatelliteListRow(satelliteListEmptyButton);
            SetButtonLabelIfChanged(satelliteListEmptyButton, EmptySatelliteListText(), string.Empty);
            RebuildSatelliteListLayout();
            LogSatelliteRefreshPerformance(refreshStartTime, routeEstimates.Count, 0, 1, 0, 0, 0, 0, shouldReorder);
            return;
        }

        if (satelliteListEmptyButton != null)
        {
            if (satelliteListEmptyButton.gameObject.activeSelf)
            {
                satelliteListEmptyButton.gameObject.SetActive(false);
                satelliteListLayoutDirty = true;
            }
        }
        RemoveDetachedSatelliteListChildren();
        HashSet<string> activeNames = new HashSet<string>();
        for (int i = 0; i < visibleSatellites.Count; i++)
        {
            activeNames.Add(visibleSatellites[i].satelliteName);
        }
        PruneSatelliteCommunicationWindowCaches(activeNames);
        List<string> staleNames = new List<string>();
        foreach (KeyValuePair<string, Button> pair in satelliteButtons)
        {
            if (pair.Value == null)
            {
                staleNames.Add(pair.Key);
                continue;
            }
            if (!activeNames.Contains(pair.Key))
            {
                if (SatelliteExists(pair.Key))
                {
                    if (pair.Value.gameObject.activeSelf)
                    {
                        pair.Value.gameObject.SetActive(false);
                        satelliteListLayoutDirty = true;
                    }
                }
                else
                {
                    Destroy(pair.Value.gameObject);
                    staleNames.Add(pair.Key);
                    removedCount++;
                    satelliteListLayoutDirty = true;
                }
            }
        }
        for (int i = 0; i < staleNames.Count; i++)
        {
            satelliteButtons.Remove(staleNames[i]);
            satelliteRowLabelCache.Remove(staleNames[i]);
                satelliteRowVisualStateCache.Remove(staleNames[i]);
                satelliteRowServiceBucketCache.Remove(staleNames[i]);
            satelliteRowMoveAnimations.Remove(staleNames[i]);
            satelliteListDisplayOrder.Remove(staleNames[i]);
            satelliteRowsWithHoverFeedback.Remove(staleNames[i]);
        }

        for (int i = 0; i < visibleSatellites.Count; i++)
        {
            SatelliteData satellite = visibleSatellites[i];
            string satelliteName = satellite.satelliteName;
            Button button;
            bool created = false;
            if (!satelliteButtons.TryGetValue(satelliteName, out button) || button == null)
            {
                string capturedName = satelliteName;
                button = CreateListButton(satelliteListContent, string.Empty, delegate { SelectSatelliteForCurrentFlow(capturedName); });
                ConfigureSatelliteListRow(button);
                satelliteButtons[satelliteName] = button;
                button.gameObject.name = "SatelliteButton_" + satelliteName.Replace(" ", "_");
                AttachSatelliteRowHoverFeedback(button, satelliteName);
                created = true;
                createdCount++;
                satelliteListLayoutDirty = true;
            }
            if (!button.gameObject.activeSelf)
            {
                button.gameObject.SetActive(true);
                satelliteListLayoutDirty = true;
            }

            int previousIndex = button.transform.GetSiblingIndex();
            if (shouldReorder && previousIndex != i)
            {
                button.transform.SetSiblingIndex(i);
                reorderCount++;
                satelliteListLayoutDirty = true;
                AnimateSatelliteRowMove(button, satelliteName);
            }
            bool canServe = CanServeRoute(satellite, targetName, sendDelay);
            bool selected = ShouldShowSatelliteSelection(satellite, selectedSatellite);
            SatelliteRowVisualState visualState = new SatelliteRowVisualState(selected, canServe, false, satelliteSelectionLocked, highContrastMode);
            bool visualChanged = created || SatelliteRowVisualChanged(satelliteName, visualState);
            bool shouldUpdateText = created || shouldReorder || selected || visualChanged || IsSatelliteRowVisibleForRefresh(i, visibleSatellites.Count);
            if (shouldUpdateText)
            {
                string rowLabel = FormatSatelliteRow(satellite, selectedSatellite, satelliteSelectionLocked);
                if (SetButtonLabelIfChanged(button, rowLabel, satelliteName))
                {
                    labelUpdateCount++;
                }
            }
            if (visualChanged)
            {
                ApplySatelliteRowVisual(button, selected, canServe, false, satelliteSelectionLocked);
                ApplySatelliteRowHoverFeedback(button, satelliteName == satelliteHoveredSatelliteName);
                satelliteRowVisualStateCache[satelliteName] = visualState;
                visualUpdateCount++;
            }
        }
        if (satelliteListScrollRect != null && satelliteListLayoutDirty)
        {
            RebuildSatelliteListLayout();
            RestoreSatelliteListTopScrollOffset(previousScrollOffset);
        }
        UpdateSatelliteListDisplayOrder(visibleSatellites);
        LogSatelliteRefreshPerformance(refreshStartTime, routeEstimates.Count, visibleSatellites.Count, labelUpdateCount, visualUpdateCount, createdCount, removedCount, reorderCount, shouldReorder);
    }

    private List<SatelliteRouteEstimateRecord> RefreshSatelliteListEstimateCache(string originName, string targetName, int sendDelay, int payloadKb, SatelliteListFilter filter, bool forceRefresh)
    {
        if (!forceRefresh && SatelliteListEstimateCacheMatches(originName, targetName, sendDelay, payloadKb) && satelliteListRouteEstimateCache.Count > 0)
        {
            List<SatelliteRouteEstimateRecord> cachedEstimates = new List<SatelliteRouteEstimateRecord>();
            for (int i = 0; i < satelliteManager.Satellites.Count; i++)
            {
                SatelliteData satellite = satelliteManager.Satellites[i];
                if (satellite == null)
                {
                    continue;
                }
                SatelliteRouteEstimateRecord cachedEstimate;
                if (satelliteListRouteEstimateCache.TryGetValue(satellite.satelliteName, out cachedEstimate))
                {
                    cachedEstimates.Add(cachedEstimate);
                }
            }
            cachedEstimates.Sort(delegate (SatelliteRouteEstimateRecord left, SatelliteRouteEstimateRecord right)
            {
                return satelliteManager.CompareRouteEstimates(left, right, filter);
            });
            return cachedEstimates;
        }

        satelliteListRouteEstimateCache.Clear();
        satelliteListRouteCacheOriginName = originName;
        satelliteListRouteCacheTargetName = targetName;
        satelliteListRouteCacheDelayMinutes = sendDelay;
        satelliteListRouteCachePayloadKb = payloadKb;
        satelliteListRecommendedSatelliteName = string.Empty;

        List<SatelliteRouteEstimateRecord> estimates = satelliteManager.GetRankedRouteEstimates(filter, originName, targetName, sendDelay, payloadKb);
        List<SatelliteRouteEstimateRecord> recommendedEstimates = filter == SatelliteListFilter.All
            ? estimates
            : satelliteManager.GetRankedRouteEstimates(SatelliteListFilter.All, originName, targetName, sendDelay, payloadKb);
        if (recommendedEstimates.Count > 0)
        {
            satelliteListRecommendedSatelliteName = recommendedEstimates[0].satelliteName;
        }
        for (int i = 0; i < estimates.Count; i++)
        {
            SatelliteRouteEstimateRecord estimate = estimates[i];
            if (estimate == null || string.IsNullOrEmpty(estimate.satelliteName))
            {
                continue;
            }
            satelliteListRouteEstimateCache[estimate.satelliteName] = estimate;
        }
        return estimates;
    }

    private List<SatelliteData> SatelliteRowsFromRouteEstimates(List<SatelliteRouteEstimateRecord> estimates, SatelliteListFilter filter)
    {
        List<SatelliteData> satellites = new List<SatelliteData>();
        if (satelliteManager == null || estimates == null)
        {
            return satellites;
        }

        for (int i = 0; i < estimates.Count; i++)
        {
            SatelliteRouteEstimateRecord estimate = estimates[i];
            if (estimate == null || string.IsNullOrEmpty(estimate.satelliteName))
            {
                continue;
            }
            SatelliteData satellite = satelliteManager.Satellites.Find(candidate => candidate != null && candidate.satelliteName == estimate.satelliteName);
            if (satellite == null)
            {
                continue;
            }
            satellites.Add(satellite);
        }
        return satellites;
    }

    private bool RefreshDisplayedCommunicationWindowSnapshot(List<SatelliteRouteEstimateRecord> estimates)
    {
        if (estimates == null)
        {
            return false;
        }

        bool sortBucketChanged = false;
        for (int i = 0; i < estimates.Count; i++)
        {
            SatelliteRouteEstimateRecord estimate = estimates[i];
            if (estimate == null || string.IsNullOrEmpty(estimate.satelliteName))
            {
                continue;
            }

            int rawBucket = CommunicationWindowSortBucketRaw(estimate);
            int rawSeconds = rawBucket == 2 ? 0 : CommunicationWindowRawSeconds(estimate);
            SatelliteWindowSortState previousSortWindow;
            bool hadPreviousSortWindow = satelliteStableSortWindows.TryGetValue(estimate.satelliteName, out previousSortWindow);
            SatelliteWindowSortState stableSortWindow = ComputeStableSortWindow(estimate.satelliteName, rawBucket, rawSeconds);
            if (hadPreviousSortWindow && previousSortWindow.bucket != stableSortWindow.bucket)
            {
                sortBucketChanged = true;
            }
            estimate.communicationSortBucket = stableSortWindow.bucket;
            estimate.communicationSortSeconds = stableSortWindow.bucket == 2 ? 0 : stableSortWindow.seconds;
            if (stableSortWindow.bucket == 0 || stableSortWindow.bucket == 1)
            {
                StabilizedCommunicationWindowSeconds(estimate.satelliteName, stableSortWindow.bucket, stableSortWindow.seconds);
            }
            StabilizeDisplayedRouteTimes(estimate);

            int serviceBucket = stableSortWindow.bucket == 2 || !estimate.canServe ? 1 : 0;
            int previousServiceBucket;
            if (satelliteRowServiceBucketCache.TryGetValue(estimate.satelliteName, out previousServiceBucket)
                && previousServiceBucket != serviceBucket)
            {
                sortBucketChanged = true;
            }
            satelliteRowServiceBucketCache[estimate.satelliteName] = serviceBucket;
        }
        return sortBucketChanged;
    }

    private SatelliteWindowSortState ComputeStableSortWindow(string satelliteName, int rawBucket, int rawSeconds)
    {
        rawBucket = Mathf.Clamp(rawBucket, 0, 2);
        rawSeconds = rawBucket == 2 ? 0 : Mathf.Max(1, rawSeconds);
        float now = Time.unscaledTime;
        if (string.IsNullOrEmpty(satelliteName))
        {
            return new SatelliteWindowSortState(rawBucket, rawSeconds, rawSeconds, now);
        }

        SatelliteWindowSortState previous;
        SatelliteWindowSortState next;
        if (!satelliteStableSortWindows.TryGetValue(satelliteName, out previous) || previous.bucket != rawBucket)
        {
            next = new SatelliteWindowSortState(rawBucket, rawSeconds, rawSeconds, now);
        }
        else if (rawBucket == 2)
        {
            next = new SatelliteWindowSortState(rawBucket, 0, rawSeconds, now);
        }
        else
        {
            int elapsedSeconds = Mathf.Max(0, Mathf.RoundToInt(now - previous.updatedAt));
            int expectedSeconds = Mathf.Max(1, previous.seconds - elapsedSeconds);
            int stableSeconds = Mathf.Min(expectedSeconds, rawSeconds);
            next = new SatelliteWindowSortState(rawBucket, Mathf.Max(1, stableSeconds), rawSeconds, now);
        }

        satelliteStableSortWindows[satelliteName] = next;
        return next;
    }

    private int CountReachableRouteEstimates(List<SatelliteRouteEstimateRecord> estimates)
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

    private void SortSatelliteRowsForFilter(List<SatelliteData> visibleSatellites, string targetName, int sendDelay, SatelliteData selectedSatellite)
    {
        if (visibleSatellites == null || visibleSatellites.Count <= 1)
        {
            return;
        }

        visibleSatellites.Sort(delegate (SatelliteData left, SatelliteData right)
        {
            SatelliteRouteEstimateRecord leftEstimate = CachedRouteEstimate(left, targetName, sendDelay);
            SatelliteRouteEstimateRecord rightEstimate = CachedRouteEstimate(right, targetName, sendDelay);
            int compare = CompareSatelliteRowsByStableUiOrder(left, right, leftEstimate, rightEstimate);
            if (compare != 0)
            {
                return compare;
            }
            compare = SatelliteDisplayOrderIndex(left).CompareTo(SatelliteDisplayOrderIndex(right));
            if (compare != 0)
            {
                return compare;
            }
            return CompareSatelliteRowsByFixedOrder(left, right);
        });
    }

    private int CompareSatelliteRowsByStableUiOrder(
        SatelliteData left,
        SatelliteData right,
        SatelliteRouteEstimateRecord leftEstimate,
        SatelliteRouteEstimateRecord rightEstimate)
    {
        int serviceCompare = UiServiceSortBucket(leftEstimate).CompareTo(UiServiceSortBucket(rightEstimate));
        if (serviceCompare != 0)
        {
            return serviceCompare;
        }

        if (satelliteFilter == SatelliteListFilter.LowLatency)
        {
            int communicationCompare = CompareCommunicationWindowPolicy(left, right, leftEstimate, rightEstimate);
            if (communicationCompare != 0)
            {
                return communicationCompare;
            }
            return CompareEstimateDeliveryWithHysteresis(left, right, leftEstimate, rightEstimate);
        }

        if (satelliteFilter == SatelliteListFilter.LowCost)
        {
            int priceCompare = DisplayedSatellitePriceCents(left).CompareTo(DisplayedSatellitePriceCents(right));
            if (priceCompare != 0)
            {
                return priceCompare;
            }
            return 0;
        }

        int recommendationStateCompare = CompareCommunicationWindowStateOnly(leftEstimate, rightEstimate);
        if (recommendationStateCompare != 0)
        {
            return recommendationStateCompare;
        }

        float scoreDelta = StableRouteScore(leftEstimate) - StableRouteScore(rightEstimate);
        if (Mathf.Abs(scoreDelta) > 1f)
        {
            return scoreDelta < 0f ? -1 : 1;
        }
        return CompareEstimateDeliveryWithHysteresis(left, right, leftEstimate, rightEstimate);
    }

    private int CompareCommunicationWindowStateOnly(SatelliteRouteEstimateRecord leftEstimate, SatelliteRouteEstimateRecord rightEstimate)
    {
        return CommunicationWindowSortBucket(leftEstimate).CompareTo(CommunicationWindowSortBucket(rightEstimate));
    }

    private int UiServiceSortBucket(SatelliteRouteEstimateRecord estimate)
    {
        return estimate == null || !estimate.canServe || CommunicationWindowSortBucket(estimate) == 2 ? 1 : 0;
    }

    private int CompareCommunicationWindowPolicy(
        SatelliteData left,
        SatelliteData right,
        SatelliteRouteEstimateRecord leftEstimate,
        SatelliteRouteEstimateRecord rightEstimate)
    {
        int leftBucket = CommunicationWindowSortBucket(leftEstimate);
        int rightBucket = CommunicationWindowSortBucket(rightEstimate);
        int bucketCompare = leftBucket.CompareTo(rightBucket);
        if (bucketCompare != 0)
        {
            return bucketCompare;
        }
        if (leftBucket == 2)
        {
            return SatelliteDisplayOrderIndex(left).CompareTo(SatelliteDisplayOrderIndex(right));
        }

        int leftSeconds = CommunicationWindowSortSeconds(leftEstimate);
        int rightSeconds = CommunicationWindowSortSeconds(rightEstimate);
        int direction = leftBucket == 0 ? -1 : 1;
        int secondsDelta = leftSeconds - rightSeconds;
        if (Mathf.Abs(secondsDelta) > SatelliteCommunicationWindowReorderThresholdSeconds)
        {
            return direction > 0 ? secondsDelta.CompareTo(0) : (-secondsDelta).CompareTo(0);
        }

        return SatelliteDisplayOrderIndex(left).CompareTo(SatelliteDisplayOrderIndex(right));
    }

    private int CompareEstimateDeliveryWithHysteresis(
        SatelliteData left,
        SatelliteData right,
        SatelliteRouteEstimateRecord leftEstimate,
        SatelliteRouteEstimateRecord rightEstimate)
    {
        int leftSeconds = EstimateDeliverySeconds(leftEstimate);
        int rightSeconds = EstimateDeliverySeconds(rightEstimate);
        int secondsDelta = leftSeconds - rightSeconds;
        if (Mathf.Abs(secondsDelta) > SatelliteCommunicationWindowReorderThresholdSeconds)
        {
            return secondsDelta.CompareTo(0);
        }
        return SatelliteDisplayOrderIndex(left).CompareTo(SatelliteDisplayOrderIndex(right));
    }

    private int CompareSatelliteRowsByFixedOrder(SatelliteData left, SatelliteData right)
    {
        int sourceIndexCompare = SatelliteSourceIndex(left).CompareTo(SatelliteSourceIndex(right));
        if (sourceIndexCompare != 0)
        {
            return sourceIndexCompare;
        }
        return string.Compare(left != null ? left.satelliteName : string.Empty, right != null ? right.satelliteName : string.Empty, System.StringComparison.Ordinal);
    }

    private int CompareCommunicationWindow(SatelliteRouteEstimateRecord leftEstimate, SatelliteRouteEstimateRecord rightEstimate)
    {
        int leftBucket = CommunicationWindowSortBucket(leftEstimate);
        int rightBucket = CommunicationWindowSortBucket(rightEstimate);
        int stateCompare = leftBucket.CompareTo(rightBucket);
        if (stateCompare != 0)
        {
            return stateCompare;
        }

        if (leftBucket == 0 && rightBucket == 0)
        {
            return CommunicationWindowSortSeconds(rightEstimate).CompareTo(CommunicationWindowSortSeconds(leftEstimate));
        }

        int secondsCompare = CommunicationWindowSortSeconds(leftEstimate).CompareTo(CommunicationWindowSortSeconds(rightEstimate));
        if (secondsCompare != 0)
        {
            return secondsCompare;
        }

        return 0;
    }

    private int CommunicationWindowSortBucket(SatelliteRouteEstimateRecord estimate)
    {
        if (estimate != null && estimate.communicationSortSeconds > 0)
        {
            return estimate.communicationSortBucket;
        }
        return CommunicationWindowSortBucketRaw(estimate);
    }

    private int CommunicationWindowSortBucketRaw(SatelliteRouteEstimateRecord estimate)
    {
        if (estimate == null || estimate.communicationWindowState == SatelliteCommunicationWindowState.Unavailable || !estimate.canServe)
        {
            return 2;
        }
        return estimate.communicationWindowState == SatelliteCommunicationWindowState.Remaining ? 0 : 1;
    }

    private int CommunicationWindowSortSeconds(SatelliteRouteEstimateRecord estimate)
    {
        if (estimate == null)
        {
            return int.MaxValue;
        }
        if (estimate.communicationSortSeconds > 0)
        {
            return Mathf.Max(0, estimate.communicationSortSeconds);
        }
        return CommunicationWindowRawSeconds(estimate);
    }

    private int CommunicationWindowRawSeconds(SatelliteRouteEstimateRecord estimate)
    {
        if (estimate == null)
        {
            return int.MaxValue;
        }
        return Mathf.Max(0, estimate.communicationWindowSeconds > 0 ? estimate.communicationWindowSeconds : estimate.estimatedWindowSeconds);
    }

    private int CompareSatelliteRowsByRecommendation(
        SatelliteData left,
        SatelliteData right,
        string targetName,
        int sendDelay,
        SatelliteData selectedSatellite,
        Dictionary<string, int> scoreCache)
    {
        int scoreCompare = CachedSatelliteCardSortScore(right, targetName, sendDelay, selectedSatellite, scoreCache)
            .CompareTo(CachedSatelliteCardSortScore(left, targetName, sendDelay, selectedSatellite, scoreCache));
        if (scoreCompare != 0)
        {
            return scoreCompare;
        }

        int indexCompare = SatelliteSourceIndex(left).CompareTo(SatelliteSourceIndex(right));
        if (indexCompare != 0)
        {
            return indexCompare;
        }
        return string.Compare(left != null ? left.satelliteName : string.Empty, right != null ? right.satelliteName : string.Empty, System.StringComparison.Ordinal);
    }

    private int CachedSatelliteCardSortScore(
        SatelliteData satellite,
        string targetName,
        int sendDelay,
        SatelliteData selectedSatellite,
        Dictionary<string, int> scoreCache)
    {
        if (satellite == null)
        {
            return -9999;
        }

        int score;
        if (scoreCache != null && scoreCache.TryGetValue(satellite.satelliteName, out score))
        {
            return score;
        }

        score = SatelliteCardSortScore(satellite, targetName, sendDelay, selectedSatellite);
        if (scoreCache != null)
        {
            scoreCache[satellite.satelliteName] = score;
        }
        return score;
    }

    private int DisplayedSatellitePriceCents(SatelliteData satellite)
    {
        return Mathf.RoundToInt(Mathf.Max(0f, EstimateSatellitePriceForCurrentMessage(satellite)) * 100f);
    }

    private int EstimateWindowSeconds(SatelliteRouteEstimateRecord estimate)
    {
        return CommunicationWindowSortSeconds(estimate);
    }

    private int EstimateDeliverySeconds(SatelliteRouteEstimateRecord estimate)
    {
        return estimate != null ? Mathf.Max(0, estimate.estimatedDeliverySeconds) : int.MaxValue;
    }

    private float StableRouteScore(SatelliteRouteEstimateRecord estimate)
    {
        return estimate != null ? estimate.score : float.MaxValue;
    }

    private int SatelliteSourceIndex(SatelliteData satellite)
    {
        if (satelliteManager == null || satellite == null)
        {
            return int.MaxValue;
        }

        int index = satelliteManager.Satellites.IndexOf(satellite);
        return index >= 0 ? index : int.MaxValue;
    }

    private bool SatelliteExists(string satelliteName)
    {
        return satelliteManager != null
            && !string.IsNullOrEmpty(satelliteName)
            && satelliteManager.Satellites.Exists(satellite => satellite != null && satellite.satelliteName == satelliteName);
    }

    private bool SetButtonLabelIfChanged(Button button, string label, string satelliteName)
    {
        if (button == null)
        {
            return false;
        }

        Text text = button.GetComponentInChildren<Text>();
        string previousLabel;
        bool changed = text == null
            || text.text != label
            || string.IsNullOrEmpty(satelliteName)
            || !satelliteRowLabelCache.TryGetValue(satelliteName, out previousLabel)
            || previousLabel != label;
        if (changed)
        {
            SetButtonLabel(button, label);
            if (!string.IsNullOrEmpty(satelliteName))
            {
                satelliteRowLabelCache[satelliteName] = label;
            }
        }
        return changed;
    }

    private bool SatelliteRowVisualChanged(string satelliteName, SatelliteRowVisualState visualState)
    {
        if (string.IsNullOrEmpty(satelliteName))
        {
            return true;
        }

        SatelliteRowVisualState previous;
        return !satelliteRowVisualStateCache.TryGetValue(satelliteName, out previous) || !previous.Equals(visualState);
    }

    private bool ShouldKeepSelectedSatelliteVisible(SatelliteData selectedSatellite)
    {
        return selectedSatellite != null
            && string.IsNullOrWhiteSpace(satelliteSearchQuery)
            && ShouldShowSatelliteSelection(selectedSatellite, selectedSatellite);
    }

    private bool ShouldKeepSelectedSatelliteVisibleAfterFiltering(SatelliteData selectedSatellite)
    {
        if (selectedSatellite == null)
        {
            return false;
        }
        return ShouldShowSatelliteSelection(selectedSatellite, selectedSatellite)
            || (missionManager != null
                && IsActiveTransferPhase(missionManager.Phase)
                && !composingQueuedMessageInFlow
                && selectedSatellite.satelliteName == missionManager.SelectedSatelliteName);
    }

    private bool IsSatelliteRowVisibleForRefresh(int rowIndex, int totalRows)
    {
        if (satelliteListScrollRect == null || satelliteListScrollRect.viewport == null || totalRows <= 0)
        {
            return true;
        }

        float viewportHeight = satelliteListScrollRect.viewport.rect.height;
        if (viewportHeight <= 1f)
        {
            return true;
        }

        float rowStride = SatelliteRowRefreshHeight + 7f;
        float contentHeight = satelliteListContent != null && satelliteListContent.rect.height > 1f
            ? satelliteListContent.rect.height
            : Mathf.Max(viewportHeight, totalRows * rowStride);
        float maxScroll = Mathf.Max(0f, contentHeight - viewportHeight);
        if (maxScroll <= 1f)
        {
            return true;
        }

        float topScroll = (1f - Mathf.Clamp01(satelliteListScrollRect.verticalNormalizedPosition)) * maxScroll;
        int firstVisible = Mathf.Max(0, Mathf.FloorToInt(topScroll / rowStride) - SatelliteRowRefreshBufferRows);
        int lastVisible = Mathf.Min(totalRows - 1, Mathf.CeilToInt((topScroll + viewportHeight) / rowStride) + SatelliteRowRefreshBufferRows);
        return rowIndex >= firstVisible && rowIndex <= lastVisible;
    }

    private void ConfigureSatelliteListRow(Button button)
    {
        if (button == null)
        {
            return;
        }

        LayoutElement layout = button.GetComponent<LayoutElement>();
        if (layout != null)
        {
            layout.minHeight = SatelliteRowRefreshHeight;
            layout.preferredHeight = SatelliteRowRefreshHeight;
            layout.flexibleHeight = 0f;
        }
    }

    private void RebuildSatelliteListLayout()
    {
        if (satelliteListContent == null)
        {
            return;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(satelliteListContent);
        Canvas.ForceUpdateCanvases();
    }

    private float SatelliteListTopScrollOffset()
    {
        if (satelliteListScrollRect == null || satelliteListScrollRect.viewport == null || satelliteListContent == null)
        {
            return 0f;
        }

        float viewportHeight = satelliteListScrollRect.viewport.rect.height;
        float contentHeight = satelliteListContent.rect.height;
        float maxScroll = Mathf.Max(0f, contentHeight - viewportHeight);
        return (1f - Mathf.Clamp01(satelliteListScrollRect.verticalNormalizedPosition)) * maxScroll;
    }

    private void RestoreSatelliteListTopScrollOffset(float topOffset)
    {
        if (satelliteListScrollRect == null || satelliteListScrollRect.viewport == null || satelliteListContent == null)
        {
            return;
        }

        float viewportHeight = satelliteListScrollRect.viewport.rect.height;
        float contentHeight = satelliteListContent.rect.height;
        float maxScroll = Mathf.Max(0f, contentHeight - viewportHeight);
        satelliteListScrollRect.verticalNormalizedPosition = maxScroll <= 1f
            ? 1f
            : 1f - Mathf.Clamp(topOffset, 0f, maxScroll) / maxScroll;
    }

    private SatelliteData CurrentSatelliteListSelectedSatellite()
    {
        if (satelliteManager == null)
        {
            return null;
        }
        if (composingQueuedMessageInFlow)
        {
            return QueueSelectedSatellite();
        }
        return satelliteManager.SelectedSatellite;
    }

    private void SelectSatelliteForCurrentFlow(string satelliteName)
    {
        if (satelliteManager == null || string.IsNullOrEmpty(satelliteName))
        {
            return;
        }

        if (composingQueuedMessageInFlow)
        {
            queueSelectedSatelliteName = satelliteName;
            RefreshSatelliteList(false, false);
            RefreshDetailPanel();
            SetConfirmSummaryText(ReservationHintText());
            ShowToast(CompactSatelliteName(satelliteName) + " 위성망을 추가 메시지 후보로 선택했습니다.");
            return;
        }

        satelliteManager.SelectSatellite(satelliteName);
    }

    private bool ShouldApplySatelliteListReorder(bool forceReorder)
    {
        if (forceReorder)
        {
            return true;
        }
        bool hold = ShouldHoldSatelliteListReorder();
        if (hold)
        {
            satelliteListPendingReorder = true;
            return satelliteListDisplayOrder.Count == 0;
        }
        return satelliteListDisplayOrder.Count == 0 || satelliteListPendingReorder || Time.unscaledTime >= nextSatelliteListReorderTime;
    }

    private bool ShouldAllowSatelliteListAutoReorder()
    {
        bool hold = ShouldHoldSatelliteListReorder();
        if (hold)
        {
            if (Time.unscaledTime >= nextSatelliteListReorderTime)
            {
                satelliteListPendingReorder = true;
            }
            return false;
        }
        return satelliteListDisplayOrder.Count == 0 || satelliteListPendingReorder || Time.unscaledTime >= nextSatelliteListReorderTime;
    }

    private bool SatelliteListRouteContextChanged()
    {
        string targetName = CurrentTargetName();
        string originName = CurrentOriginName();
        int sendDelay = composingQueuedMessageInFlow ? QueueFlowRequestedDelayMinutes() : missionManager != null ? missionManager.RequestedSendDelayMinutes : 0;
        int payloadKb = CurrentSatellitePayloadKb();
        return satelliteListRouteCacheOriginName != originName
            || satelliteListRouteCacheTargetName != targetName
            || satelliteListRouteCacheDelayMinutes != sendDelay
            || satelliteListRouteCachePayloadKb != payloadKb;
    }

    private bool ShouldHoldSatelliteListReorder()
    {
        if (satelliteListScrollRect != null)
        {
            if (Mathf.Abs(satelliteListScrollRect.velocity.y) > SatelliteListScrollVelocityHoldThreshold)
            {
                return true;
            }
            if (Time.unscaledTime - lastSatelliteListScrollChangedTime < SatelliteListScrollReorderHoldSeconds)
            {
                return true;
            }
        }
        if (!string.IsNullOrWhiteSpace(satelliteSearchQuery))
        {
            return true;
        }
        return Time.unscaledTime - lastSatelliteSearchChangedTime < 1.25f;
    }

    private void PreserveSatelliteRowOrder(List<SatelliteData> visibleSatellites)
    {
        if (visibleSatellites == null || visibleSatellites.Count <= 1)
        {
            return;
        }

        visibleSatellites.Sort(delegate (SatelliteData left, SatelliteData right)
        {
            int orderCompare = SatelliteDisplayOrderIndex(left).CompareTo(SatelliteDisplayOrderIndex(right));
            if (orderCompare != 0)
            {
                return orderCompare;
            }
            return SatelliteSourceIndex(left).CompareTo(SatelliteSourceIndex(right));
        });
    }

    private int SatelliteDisplayOrderIndex(SatelliteData satellite)
    {
        if (satellite == null)
        {
            return int.MaxValue;
        }

        int index = satelliteListDisplayOrder.IndexOf(satellite.satelliteName);
        return index >= 0 ? index : int.MaxValue;
    }

    private void UpdateSatelliteListDisplayOrder(List<SatelliteData> visibleSatellites)
    {
        satelliteListDisplayOrder.Clear();
        if (visibleSatellites == null)
        {
            return;
        }

        for (int i = 0; i < visibleSatellites.Count; i++)
        {
            SatelliteData satellite = visibleSatellites[i];
            if (satellite != null && !string.IsNullOrEmpty(satellite.satelliteName))
            {
                satelliteListDisplayOrder.Add(satellite.satelliteName);
            }
        }
    }

    private void PruneSatelliteCommunicationWindowCaches(HashSet<string> activeNames)
    {
        if (activeNames == null)
        {
            return;
        }

        PruneSatelliteCommunicationWindowCache(satelliteDisplayedWindowState, activeNames);
        PruneSatelliteCommunicationWindowCache(satelliteDisplayedWindowSeconds, activeNames);
        PruneSatelliteCommunicationWindowCache(satelliteDisplayedWindowUpdatedAt, activeNames);
        PruneSatelliteRouteTimeCache(satelliteDisplayedRouteTimeSeconds, activeNames);
        PruneSatelliteRouteTimeCache(satelliteDisplayedRouteTimeUpdatedAt, activeNames);
        PruneSatelliteCommunicationWindowCache(satelliteDisplayedRouteScores, activeNames);
        PruneSatelliteCommunicationWindowCache(satelliteDisplayedRouteScoreUpdatedAt, activeNames);
        PruneSatelliteCommunicationWindowCache(satelliteStableSortWindows, activeNames);
    }

    private void ClearSatelliteCommunicationWindowCache()
    {
        satelliteDisplayedWindowState.Clear();
        satelliteDisplayedWindowSeconds.Clear();
        satelliteDisplayedWindowUpdatedAt.Clear();
        satelliteDisplayedRouteTimeSeconds.Clear();
        satelliteDisplayedRouteTimeUpdatedAt.Clear();
        satelliteDisplayedRouteScores.Clear();
        satelliteDisplayedRouteScoreUpdatedAt.Clear();
        satelliteStableSortWindows.Clear();
    }

    private void PruneSatelliteCommunicationWindowCache<T>(Dictionary<string, T> cache, HashSet<string> activeNames)
    {
        if (cache == null || cache.Count == 0)
        {
            return;
        }

        List<string> staleNames = new List<string>();
        foreach (KeyValuePair<string, T> pair in cache)
        {
            if (!activeNames.Contains(pair.Key))
            {
                staleNames.Add(pair.Key);
            }
        }
        for (int i = 0; i < staleNames.Count; i++)
        {
            cache.Remove(staleNames[i]);
        }
    }

    private void PruneSatelliteRouteTimeCache<T>(Dictionary<string, T> cache, HashSet<string> activeNames)
    {
        if (cache == null || cache.Count == 0)
        {
            return;
        }

        List<string> staleNames = new List<string>();
        foreach (KeyValuePair<string, T> pair in cache)
        {
            string satelliteName = RouteTimeCacheSatelliteName(pair.Key);
            if (!activeNames.Contains(satelliteName))
            {
                staleNames.Add(pair.Key);
            }
        }
        for (int i = 0; i < staleNames.Count; i++)
        {
            cache.Remove(staleNames[i]);
        }
    }

    private string RouteTimeCacheSatelliteName(string cacheKey)
    {
        if (string.IsNullOrEmpty(cacheKey))
        {
            return string.Empty;
        }

        int separatorIndex = cacheKey.IndexOf('|');
        return separatorIndex >= 0 ? cacheKey.Substring(0, separatorIndex) : cacheKey;
    }

    private void AttachSatelliteRowHoverFeedback(Button button, string satelliteName)
    {
        if (button == null || string.IsNullOrEmpty(satelliteName) || satelliteRowsWithHoverFeedback.Contains(satelliteName))
        {
            return;
        }

        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<EventTrigger>();
        }
        if (trigger.triggers == null)
        {
            trigger.triggers = new List<EventTrigger.Entry>();
        }

        string capturedName = satelliteName;
        EventTrigger.Entry enter = new EventTrigger.Entry();
        enter.eventID = EventTriggerType.PointerEnter;
        enter.callback.AddListener(delegate { SetSatelliteRowHover(capturedName, true); });
        trigger.triggers.Add(enter);

        EventTrigger.Entry exit = new EventTrigger.Entry();
        exit.eventID = EventTriggerType.PointerExit;
        exit.callback.AddListener(delegate { SetSatelliteRowHover(capturedName, false); });
        trigger.triggers.Add(exit);

        satelliteRowsWithHoverFeedback.Add(satelliteName);
    }

    private void SetSatelliteRowHover(string satelliteName, bool hovered)
    {
        Button button;
        if (string.IsNullOrEmpty(satelliteName) || !satelliteButtons.TryGetValue(satelliteName, out button) || button == null)
        {
            return;
        }

        satelliteHoveredSatelliteName = hovered ? satelliteName : satelliteHoveredSatelliteName == satelliteName ? string.Empty : satelliteHoveredSatelliteName;
        ApplySatelliteRowHoverFeedback(button, hovered);
        if (!hovered && satelliteManager != null)
        {
            SatelliteData satellite = satelliteManager.Satellites.Find(item => item != null && item.satelliteName == satelliteName);
            if (satellite != null)
            {
                bool locked = missionManager != null && IsActiveTransferPhase(missionManager.Phase) && !composingQueuedMessageInFlow;
                bool selected = ShouldShowSatelliteSelection(satellite, CurrentSatelliteListSelectedSatellite());
                ApplySatelliteRowVisual(button, selected, CanServeRoute(satellite, CurrentTargetName(), composingQueuedMessageInFlow ? QueueFlowRequestedDelayMinutes() : missionManager != null ? missionManager.RequestedSendDelayMinutes : 0), false, locked);
            }
        }
    }

    private void ApplySatelliteRowHoverFeedback(Button button, bool hovered)
    {
        if (button == null)
        {
            return;
        }

        RectTransform rowRect = button.GetComponent<RectTransform>();
        if (rowRect != null)
        {
            rowRect.localScale = hovered ? Vector3.one * 1.012f : Vector3.one;
        }

        Transform selectionBar = button.transform.Find("SelectionBar");
        if (selectionBar == null)
        {
            return;
        }

        RectTransform barRect = selectionBar.GetComponent<RectTransform>();
        if (barRect != null && !hovered)
        {
            barRect.offsetMax = new Vector2(barRect.offsetMax.x > 8f ? 12f : 4f, 0f);
        }
        Image bar = selectionBar.GetComponent<Image>();
        if (bar != null && hovered)
        {
            bar.color = new Color(0.58f, 0.9f, 1f, 0.42f);
        }
    }

    private float SatelliteRecommendationDataRefreshInterval()
    {
        return 1f;
    }

    private float SatelliteRecommendationReorderInterval()
    {
        if (satelliteFilter == SatelliteListFilter.LowLatency)
        {
            return 1f;
        }
        if (satelliteFilter == SatelliteListFilter.LowCost)
        {
            return 3f;
        }
        return 3f;
    }

    private void AnimateSatelliteRowMove(Button button, string satelliteName)
    {
        if (button == null || string.IsNullOrEmpty(satelliteName))
        {
            return;
        }

        Coroutine runningAnimation;
        if (satelliteRowMoveAnimations.TryGetValue(satelliteName, out runningAnimation) && runningAnimation != null)
        {
            StopCoroutine(runningAnimation);
        }
        satelliteRowMoveAnimations[satelliteName] = StartCoroutine(AnimateSatelliteRowMoveRoutine(button.GetComponent<RectTransform>(), satelliteName));
    }

    private IEnumerator AnimateSatelliteRowMoveRoutine(RectTransform rowRect, string satelliteName)
    {
        if (rowRect == null)
        {
            yield break;
        }

        CanvasGroup group = rowRect.GetComponent<CanvasGroup>();
        if (group == null)
        {
            group = rowRect.gameObject.AddComponent<CanvasGroup>();
        }

        const float duration = 0.12f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float eased = 1f - Mathf.Pow(1f - t, 3f);
            group.alpha = Mathf.Lerp(0.985f, 1f, eased);
            rowRect.localScale = Vector3.one * Mathf.Lerp(0.998f, 1f, eased);
            yield return null;
        }

        group.alpha = 1f;
        rowRect.localScale = Vector3.one;
        if (!string.IsNullOrEmpty(satelliteName))
        {
            satelliteRowMoveAnimations.Remove(satelliteName);
        }
    }

    private void LogSatelliteRefreshPerformance(
        float startTime,
        int estimateCount,
        int rowCount,
        int labelUpdateCount,
        int visualUpdateCount,
        int createdCount,
        int removedCount,
        int reorderCount,
        bool reordered)
    {
        float elapsedMs = Mathf.Max(0f, (Time.realtimeSinceStartup - startTime) * 1000f);
        if (elapsedMs < 18f && Time.unscaledTime < nextSatelliteRefreshPerfLogTime)
        {
            return;
        }

        nextSatelliteRefreshPerfLogTime = Time.unscaledTime + 6f;
        Debug.Log(
            "SatelliteRecommendationRefresh"
            + " filter=" + SatelliteFilterLabel(satelliteFilter)
            + " mode=" + (reordered ? "data+reorder" : "data-only")
            + " estimates=" + estimateCount
            + " rows=" + rowCount
            + " labelUpdates=" + labelUpdateCount
            + " visualUpdates=" + visualUpdateCount
            + " created=" + createdCount
            + " removed=" + removedCount
            + " moved=" + reorderCount
            + " ms=" + elapsedMs.ToString("0.0"));
    }

    private struct SatelliteRowVisualState
    {
        private readonly bool selected;
        private readonly bool canServe;
        private readonly bool complete;
        private readonly bool locked;
        private readonly bool highContrast;

        public SatelliteRowVisualState(bool selected, bool canServe, bool complete, bool locked, bool highContrast)
        {
            this.selected = selected;
            this.canServe = canServe;
            this.complete = complete;
            this.locked = locked;
            this.highContrast = highContrast;
        }

        public bool Equals(SatelliteRowVisualState other)
        {
            return selected == other.selected
                && canServe == other.canServe
                && complete == other.complete
                && locked == other.locked
                && highContrast == other.highContrast;
        }
    }

    private struct SatelliteWindowSortState
    {
        public readonly int bucket;
        public readonly int seconds;
        public readonly int lastRawSeconds;
        public readonly float updatedAt;

        public SatelliteWindowSortState(int bucket, int seconds, int lastRawSeconds, float updatedAt)
        {
            this.bucket = bucket;
            this.seconds = seconds;
            this.lastRawSeconds = lastRawSeconds;
            this.updatedAt = updatedAt;
        }
    }

    private void RemoveDetachedSatelliteListChildren()
    {
        if (satelliteListContent == null)
        {
            return;
        }
        HashSet<Button> knownButtons = new HashSet<Button>(satelliteButtons.Values);
        if (satelliteListEmptyButton != null)
        {
            knownButtons.Add(satelliteListEmptyButton);
        }
        foreach (Transform child in satelliteListContent)
        {
            Button button = child.GetComponent<Button>();
            if (button == null || !knownButtons.Contains(button))
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void ApplySatelliteRowVisual(Button button, bool selected, bool canServe, bool isComplete, bool satelliteSelectionLocked)
    {
        ColorBlock colors = button.colors;
        if ((isComplete || satelliteSelectionLocked) && !selected)
        {
            colors.normalColor = new Color(0.04f, 0.07f, 0.1f, 0.36f);
            colors.highlightedColor = colors.normalColor;
            colors.pressedColor = colors.normalColor;
            colors.disabledColor = new Color(0.04f, 0.07f, 0.1f, 0.24f);
        }
        else if (selected)
        {
            colors.normalColor = new Color(0.10f, 0.42f, 0.92f, 1f);
            colors.highlightedColor = new Color(0.20f, 0.62f, 1f, 1f);
            colors.pressedColor = new Color(0.05f, 0.24f, 0.56f, 1f);
            colors.disabledColor = colors.normalColor;
        }
        else
        {
            colors.normalColor = canServe ? new Color(0.06f, 0.24f, 0.2f, 0.98f) : new Color(0.08f, 0.11f, 0.16f, 0.98f);
            colors.highlightedColor = new Color(0.16f, 0.44f, 0.74f, 1f);
            colors.pressedColor = new Color(0.06f, 0.24f, 0.38f, 1f);
        }
        colors.selectedColor = colors.highlightedColor;
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.18f;
        button.colors = colors;
        button.interactable = !satelliteSelectionLocked && (!isComplete || selected);

        LayoutElement layout = button.GetComponent<LayoutElement>();
        if (layout != null)
        {
            layout.minHeight = SatelliteRowRefreshHeight;
            layout.preferredHeight = SatelliteRowRefreshHeight;
            layout.flexibleHeight = 0f;
        }
        Text rowText = button.GetComponentInChildren<Text>();
        if (rowText != null)
        {
            rowText.color = (isComplete || satelliteSelectionLocked) && !selected ? new Color(1f, 1f, 1f, 0.38f) : Color.white;
            rowText.fontStyle = selected ? FontStyle.Bold : FontStyle.Normal;
        }
        Transform selectionBar = button.transform.Find("SelectionBar");
        if (selectionBar != null)
        {
            RectTransform barRect = selectionBar.GetComponent<RectTransform>();
            if (barRect != null)
            {
                barRect.offsetMax = new Vector2(selected ? 12f : 4f, 0f);
            }
            Image bar = selectionBar.GetComponent<Image>();
            if (bar != null)
            {
                bar.color = selected
                    ? new Color(0.55f, 1f, 0.96f, 1f)
                    : new Color(0.22f, 0.52f, 0.95f, isComplete ? 0.03f : 0.08f);
            }
        }
    }

    private int SatelliteCardSortScore(SatelliteData satellite, string targetName, int sendDelay, SatelliteData selectedSatellite)
    {
        if (satellite == null)
        {
            return -9999;
        }

        int score = 0;
        if (IsRecommendedSatellite(satellite, targetName, sendDelay))
        {
            score += 600;
        }
        if (CanServeRoute(satellite, targetName, sendDelay))
        {
            score += 400;
        }
        SatelliteRouteEstimateRecord estimate = RouteEstimate(satellite, targetName, sendDelay);
        score += Mathf.Clamp(satellite.reliabilityPercent, 0, 100);
        score -= estimate != null ? Mathf.Clamp(Mathf.RoundToInt(estimate.score), 0, 999) / 4 : Mathf.Clamp(satellite.nextWindowMinutes, 0, 999);
        score -= Mathf.Clamp(satellite.congestionPercent, 0, 100) / 2;
        return score;
    }

    private void RefreshDetailPanel()
    {
        if (detailText != null && composingQueuedMessageInFlow)
        {
            detailText.text = QueueFlowSatelliteDetailText();
            return;
        }
        if (detailText == null || satelliteManager.SelectedSatellite == null)
        {
            return;
        }

        SatelliteData satellite = satelliteManager.SelectedSatellite;
        string targetName = missionManager != null && missionManager.SelectedTarget != null ? missionManager.SelectedTarget.targetName : OrbitShareDestinations.DefaultTargetName;
        float price = EstimateSatellitePriceForCurrentMessage(satellite);
        int sendDelay = missionManager != null ? missionManager.RequestedSendDelayMinutes : 0;
        SatelliteRouteEstimateRecord estimate = RouteEstimate(satellite, targetName, sendDelay);
        detailText.text =
            "위성 이름 : " + satellite.satelliteName + "\n" +
            "위성 종류 : " + SatelliteKindText(satellite) + "\n" +
            "경로 : " + SatelliteRouteText(targetName) + "\n" +
            "통신 가능 시간 : " + CommunicationWindowText(satellite, estimate) + "\n" +
            "가격 정보 : " + FormatUsd(price) + "\n" +
            "성공률 : " + satellite.RecentSuccessPercent + "%\n" +
            "예상 소요 시간 : " + TransferDurationText(satellite, estimate);
    }

    private void RefreshSatelliteCompare()
    {
        if (satelliteCompareText == null)
        {
            return;
        }

        if (currentFlowStep == 4 && requestFlowPanel != null && requestFlowPanel.gameObject.activeSelf)
        {
            satelliteCompareText.text = string.Empty;
            satelliteCompareText.gameObject.SetActive(false);
            return;
        }

        if (satelliteManager == null || satelliteManager.Satellites.Count == 0 || missionManager == null)
        {
            satelliteCompareText.text = string.Empty;
            satelliteCompareText.gameObject.SetActive(false);
            return;
        }

        string targetName = CurrentTargetName();
        List<SatelliteData> candidates = new List<SatelliteData>();
        for (int i = 0; i < satelliteManager.Satellites.Count; i++)
        {
            SatelliteData satellite = satelliteManager.Satellites[i];
            if (satellite != null && CanServeRoute(satellite, targetName, missionManager.RequestedSendDelayMinutes))
            {
                candidates.Add(satellite);
            }
        }

        if (candidates.Count == 0)
        {
            satelliteCompareText.text = string.Empty;
            satelliteCompareText.gameObject.SetActive(false);
            return;
        }

        System.Text.StringBuilder builder = new System.Text.StringBuilder();
        AppendSatelliteRecommendationCard(builder, "빠른", BestSatelliteByMode(candidates, "fast"), targetName);
        AppendSatelliteRecommendationCard(builder, "안정", BestSatelliteByMode(candidates, "reliable"), targetName);
        AppendSatelliteRecommendationCard(builder, "저렴", BestSatelliteByMode(candidates, "cheap"), targetName);
        satelliteCompareText.text = builder.ToString().TrimEnd();
        satelliteCompareText.gameObject.SetActive(true);
    }

    private SatelliteData BestSatelliteByMode(List<SatelliteData> candidates, string mode)
    {
        if (candidates == null || candidates.Count == 0)
        {
            return null;
        }

        SatelliteData best = candidates[0];
        float bestScore = float.MinValue;
        for (int i = 0; i < candidates.Count; i++)
        {
            SatelliteData satellite = candidates[i];
            float price = EstimateSatellitePriceForCurrentMessage(satellite);
            SatelliteRouteEstimateRecord estimate = RouteEstimate(satellite, CurrentTargetName(), missionManager != null ? missionManager.RequestedSendDelayMinutes : 0);
            float deliveryPenalty = estimate != null ? estimate.estimatedDeliverySeconds : satellite.latencySeconds;
            float anglePenalty = estimate != null ? estimate.routeAngleDegrees : satellite.nextWindowMinutes;
            float score = mode == "fast"
                ? -deliveryPenalty * 1.2f - anglePenalty * 0.35f + satellite.reliabilityPercent - satellite.congestionPercent * 0.25f
                : mode == "reliable"
                    ? satellite.RecentSuccessPercent * 2f + satellite.reliabilityPercent - satellite.congestionPercent * 0.5f - anglePenalty * 0.15f
                    : -price * 100f - satellite.feePer32KbCredits * 3f + satellite.reliabilityPercent * 0.25f - deliveryPenalty * 0.08f;
            if (score > bestScore)
            {
                bestScore = score;
                best = satellite;
            }
        }
        return best;
    }

    private void AppendSatelliteRecommendationCard(System.Text.StringBuilder builder, string title, SatelliteData satellite, string targetName)
    {
        if (satellite == null)
        {
            builder.Append(title).Append(" 카드 | 추천 없음\n");
            return;
        }

        builder.Append(title)
            .Append(" 카드 | ")
            .Append(SatelliteRecommendationReasonLine(satellite, targetName))
            .Append("\n");
    }

    private string SatelliteRecommendationReasonLine(SatelliteData satellite, string targetName)
    {
        if (satellite == null)
        {
            return "추천 없음";
        }

        string apiReason = SelectedSatelliteRecommendationReason(satellite, targetName);
        if (!string.IsNullOrEmpty(apiReason))
        {
            return apiReason;
        }

        SatelliteRecommendationRecord recommendation = FindRecommendationRecord(satellite, targetName);
        if (recommendation != null && !string.IsNullOrEmpty(recommendation.reasonSummary))
        {
            return recommendation.reasonSummary;
        }

        int sendDelay = missionManager != null ? missionManager.RequestedSendDelayMinutes : 0;
        int payloadKb = missionManager != null ? missionManager.MessageSizeKb : 128;
        return satellite.GetRecommendationReasonSummary(payloadKb, sendDelay);
    }

    private string SelectedSatelliteRecommendationReason(SatelliteData satellite, string targetName)
    {
        if (missionManager == null || satelliteManager == null || satellite == null || satellite != satelliteManager.SelectedSatellite)
        {
            return string.Empty;
        }

        string reason = missionManager.GetSelectedSatelliteRecommendationReason(targetName, missionManager.RequestedSendDelayMinutes, missionManager.MessageSizeKb);
        return string.IsNullOrEmpty(reason) || reason == "선택 위성 없음" ? string.Empty : reason;
    }

    private SatelliteRecommendationRecord FindRecommendationRecord(SatelliteData satellite, string targetName)
    {
        if (missionManager == null || satellite == null)
        {
            return null;
        }

        List<SatelliteRecommendationRecord> recommendations = missionManager.GetDestinationRecommendations(CurrentOriginName(), targetName, missionManager.RequestedSendDelayMinutes, missionManager.MessageSizeKb);
        for (int i = 0; i < recommendations.Count; i++)
        {
            SatelliteRecommendationRecord recommendation = recommendations[i];
            if (recommendation != null && recommendation.satelliteName == satellite.satelliteName)
            {
                return recommendation;
            }
        }
        return null;
    }

    private float EstimateSatellitePriceForCurrentMessage(SatelliteData satellite)
    {
        if (satellite == null)
        {
            return 0f;
        }
        return satellite.EstimatePriceUsd(CurrentSatellitePayloadKb());
    }

    private void ShowSatellitePanel()
    {
        bool wasDismissed = satelliteListDismissed;
        if (missionManager == null || missionManager.Phase == MissionPhase.Idle || composingQueuedMessageInFlow)
        {
            currentFlowStep = 4;
        }
        SetSatellitePanelOpen(true, false);
        if (wasDismissed)
        {
            ShowToast("위성망 추천 창을 다시 열었습니다.");
        }
        RefreshAllPreservingLauncherPanels(satelliteListPanel);
    }

    private void ToggleSatellitePanel()
    {
        if (satelliteListPanel == null)
        {
            return;
        }

        if (satelliteListPanel.gameObject.activeSelf && !satelliteListDismissed)
        {
            SetSatellitePanelOpen(false, true);
            RefreshAllPreservingLauncherPanels(null);
            return;
        }

        ShowSatellitePanel();
    }

    private void SetSatellitePanelOpen(bool open, bool dismissed)
    {
        satelliteListDismissed = dismissed;
        if (satelliteListPanel != null)
        {
            satelliteListPanel.gameObject.SetActive(open && !dismissed);
            if (open && !dismissed)
            {
                satelliteListPanel.SetAsLastSibling();
                ResolveSatellitePanelOverlap();
                PositionToastAboveActivePanels();
            }
        }
        ApplyFixedPanelOrder();
        RefreshPanelLauncherButtons();
    }

    private void ResolveSatellitePanelOverlap()
    {
        if (satelliteListPanel == null || !satelliteListPanel.gameObject.activeSelf)
        {
            return;
        }

        bool moved = false;
        if (requestFlowPanel != null && requestFlowPanel.gameObject.activeSelf && PanelsOverlap(satelliteListPanel, requestFlowPanel, 0.04f))
        {
            SetPanelRect(satelliteListPanel, new Vector2(0.015f, 0.08f), new Vector2(0.30f, 0.89f));
            moved = true;
        }
        if (missionStatusPanel != null && missionStatusPanel.gameObject.activeSelf && PanelsOverlap(satelliteListPanel, missionStatusPanel, 0.08f))
        {
            SetPanelRect(missionStatusPanel, new Vector2(0.33f, 0.11f), new Vector2(0.67f, 0.27f));
            moved = true;
        }

        if (moved)
        {
            ApplyFixedPanelOrder();
        }
    }

    private bool PanelsOverlap(RectTransform first, RectTransform second, float minimumOverlapRatio)
    {
        if (first == null || second == null)
        {
            return false;
        }

        Rect firstRect = WorldRect(first);
        Rect secondRect = WorldRect(second);
        float xMin = Mathf.Max(firstRect.xMin, secondRect.xMin);
        float xMax = Mathf.Min(firstRect.xMax, secondRect.xMax);
        float yMin = Mathf.Max(firstRect.yMin, secondRect.yMin);
        float yMax = Mathf.Min(firstRect.yMax, secondRect.yMax);
        float overlapArea = Mathf.Max(0f, xMax - xMin) * Mathf.Max(0f, yMax - yMin);
        if (overlapArea <= 0f)
        {
            return false;
        }

        float baseArea = Mathf.Max(1f, Mathf.Min(firstRect.width * firstRect.height, secondRect.width * secondRect.height));
        return overlapArea / baseArea >= minimumOverlapRatio;
    }

    private Rect WorldRect(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        return Rect.MinMaxRect(corners[0].x, corners[0].y, corners[2].x, corners[2].y);
    }

    private void PositionToastAboveActivePanels()
    {
        if (toastPanel == null)
        {
            return;
        }

        toastPanel.anchorMin = new Vector2(0.34f, 0.915f);
        toastPanel.anchorMax = new Vector2(0.66f, 0.985f);
        toastPanel.offsetMin = Vector2.zero;
        toastPanel.offsetMax = Vector2.zero;
    }

    private void ToggleFavoriteSatellite()
    {
        if (satelliteManager == null || satelliteManager.SelectedSatellite == null)
        {
            return;
        }

        string satelliteName = satelliteManager.SelectedSatellite.satelliteName;
        if (favoriteSatelliteNames.Contains(satelliteName))
        {
            favoriteSatelliteNames.Remove(satelliteName);
            ShowToast(satelliteName + " 즐겨찾기를 해제했습니다.");
        }
        else
        {
            favoriteSatelliteNames.Add(satelliteName);
            ShowToast(satelliteName + " 위성을 즐겨찾기에 추가했습니다.");
        }
        RefreshSatelliteList();
        RefreshGuidanceButtons();
    }

    private void CreateFilterButton(RectTransform parent, string label, SatelliteListFilter filter, float leftAnchor, int row)
    {
        float top = -72f - row * 38f;
        float bottom = -106f - row * 38f;
        const float buttonWidth = 0.30f;
        Button button = CreateButton(
            parent,
            label,
            new Vector2(leftAnchor, 1f),
            new Vector2(leftAnchor + buttonWidth, 1f),
            new Vector2(14f, bottom),
            new Vector2(-4f, top),
            delegate
            {
                satelliteFilter = filter;
                nextSatelliteListReorderTime = 0f;
                satelliteListDisplayOrder.Clear();
                satelliteListPendingReorder = false;
                RefreshSatelliteList(true, true, true);
            });
        Text text = button.GetComponentInChildren<Text>();
        if (text != null)
        {
            text.fontSize = 13;
        }
        filterButtons[filter] = button;
        RefreshFilterButtons();
    }

    private void RefreshFilterButtons()
    {
        NormalizeSatelliteFilter();
        RefreshFilterButton(SatelliteListFilter.All, "추천");
        RefreshFilterButton(SatelliteListFilter.LowCost, "비용");
        RefreshFilterButton(SatelliteListFilter.LowLatency, "통신 가능 시간");
    }

    private void NormalizeSatelliteFilter()
    {
        if (satelliteFilter == SatelliteListFilter.TransferSpeed)
        {
            satelliteFilter = SatelliteListFilter.All;
        }
    }

    private void RefreshFilterButton(SatelliteListFilter filter, string label)
    {
        if (!filterButtons.ContainsKey(filter))
        {
            return;
        }

        Button button = filterButtons[filter];
        bool selected = satelliteFilter == filter;
        button.interactable = true;
        SetButtonLabel(button, label);
        SetButtonTextSize(button, filter == SatelliteListFilter.LowLatency ? 12 : 13);
        SetButtonVisual(button, selected ? ButtonVisual.Selected : ButtonVisual.Secondary);
    }

    private string SatelliteFilterLabel(SatelliteListFilter filter)
    {
        switch (filter)
        {
            case SatelliteListFilter.LowCost:
                return "비용";
            case SatelliteListFilter.LowLatency:
                return "통신 가능 시간";
            default:
                return "추천";
        }
    }

    private string SelectedVsRecommendedText(string targetName)
    {
        if (satelliteManager == null || missionManager == null || satelliteManager.SelectedSatellite == null)
        {
            return string.Empty;
        }
        SatelliteData selected = satelliteManager.SelectedSatellite;
        List<SatelliteRouteEstimateRecord> ranked = satelliteManager.GetRankedRouteEstimates(SatelliteListFilter.All, CurrentOriginName(), targetName, missionManager.RequestedSendDelayMinutes, missionManager.MessageSizeKb);
        SatelliteData recommended = ranked.Count > 0 ? satelliteManager.Satellites.Find(satellite => satellite.satelliteName == ranked[0].satelliteName) : null;
        if (recommended == null || recommended == selected)
        {
            return "자동 추천 위성망";
        }
        float selectedBase;
        float selectedPayload;
        float selectedUrgency;
        float recommendedBase;
        float recommendedPayload;
        float recommendedUrgency;
        float selectedPrice = missionManager.EstimateDetailedPriceUsd(missionManager.MessageSizeKb, missionManager.RequestedSendDelayMinutes, selected, out selectedBase, out selectedPayload, out selectedUrgency);
        float recommendedPrice = missionManager.EstimateDetailedPriceUsd(missionManager.MessageSizeKb, missionManager.RequestedSendDelayMinutes, recommended, out recommendedBase, out recommendedPayload, out recommendedUrgency);
        float priceDelta = selectedPrice - recommendedPrice;
        int latencyDelta = selected.latencySeconds - recommended.latencySeconds;
        return "자동 추천 " + CompactSatelliteName(recommended.satelliteName)
            + "보다 " + (priceDelta >= 0f ? "+" : "") + FormatUsd(priceDelta)
            + " · " + (latencyDelta >= 0 ? "+" : "") + latencyDelta + "초";
    }

    private string FormatSatelliteRow(SatelliteData satellite, SatelliteData selectedSatellite, bool satelliteSelectionLocked)
    {
        string targetName = CurrentTargetName();
        int sendDelay = composingQueuedMessageInFlow ? QueueFlowRequestedDelayMinutes() : missionManager != null ? missionManager.RequestedSendDelayMinutes : 0;
        bool selected = ShouldShowSatelliteSelection(satellite, selectedSatellite);
        bool recommended = satelliteFilter == SatelliteListFilter.All && IsRecommendedSatellite(satellite, targetName, sendDelay);
        SatelliteRouteEstimateRecord estimate = RouteEstimate(satellite, targetName, sendDelay);
        string badge = selected
            ? satelliteSelectionLocked ? "전송 중 고정" : recommended ? "선택됨 · 추천" : "선택됨"
            : recommended ? "추천" : string.Empty;
        string price = FormatUsd(EstimateSatellitePriceForCurrentMessage(satellite));
        string heading = CompactSatelliteName(satellite.satelliteName);
        if (selected)
        {
            heading = "▶ " + heading;
        }
        if (!string.IsNullOrEmpty(badge))
        {
            heading += "   " + badge;
        }
        return heading + "\n"
            + "위성 종류 : " + SatelliteKindText(satellite) + "\n"
            + "통신 가능 시간 : " + CommunicationWindowText(satellite, estimate) + "\n"
            + "예상 가격 : " + price;
    }

    private bool ShouldShowSatelliteSelection(SatelliteData satellite, SatelliteData selectedSatellite)
    {
        if (satellite == null || selectedSatellite == null || satellite != selectedSatellite)
        {
            return false;
        }
        return missionManager == null || missionManager.Phase != MissionPhase.Complete || composingQueuedMessageInFlow;
    }

    private string SatelliteKindText(SatelliteData satellite)
    {
        if (satellite == null)
        {
            return "위성망";
        }

        string altitudeBand = satellite.altitudeKm < 2000f
            ? "LEO"
            : satellite.altitudeKm < 20000f ? "MEO" : "GEO";
        return satellite.OwnerTypeLabel + " · " + altitudeBand + " 중계";
    }

    private string SatelliteRouteText(string targetName)
    {
        return DestinationLabel(CurrentOriginName()) + " -> " + DestinationLabel(targetName);
    }

    private string CommunicationWindowText(SatelliteData satellite, SatelliteRouteEstimateRecord estimate)
    {
        if (satellite == null)
        {
            return "계산 중";
        }
        if (estimate != null)
        {
            if (!estimate.canServe)
            {
                return "통신 불가";
            }
            if (CommunicationWindowSortBucket(estimate) == 0
                || estimate.communicationWindowState == SatelliteCommunicationWindowState.Remaining
                || estimate.estimatedWindowSeconds <= 0)
            {
                int availableSeconds = estimate.communicationSortSeconds > 0
                    ? estimate.communicationSortSeconds
                    : estimate.communicationWindowSeconds > 0 ? estimate.communicationWindowSeconds : Mathf.Max(0, estimate.estimatedAvailableForSeconds);
                return FormatSeconds(availableSeconds) + " 남음";
            }
            int estimatedWaitSeconds = estimate.communicationSortSeconds > 0
                ? estimate.communicationSortSeconds
                : estimate.communicationWindowSeconds > 0 ? estimate.communicationWindowSeconds : estimate.estimatedWindowSeconds;
            return FormatSeconds(estimatedWaitSeconds) + " 후 가능";
        }

        int waitSeconds = Mathf.Max(0, satellite.nextWindowMinutes) * 60;
        if (waitSeconds <= 0)
        {
            return FormatSeconds(StabilizedCommunicationWindowSeconds(satellite.satelliteName, 0, 0)) + " 남음";
        }
        waitSeconds = StabilizedCommunicationWindowSeconds(satellite.satelliteName, 1, waitSeconds);
        return FormatSeconds(waitSeconds) + " 후 가능";
    }

    private int StabilizedCommunicationWindowSeconds(string satelliteName, int state, int rawSeconds)
    {
        if (string.IsNullOrEmpty(satelliteName))
        {
            return Mathf.Max(0, rawSeconds);
        }

        rawSeconds = Mathf.Max(0, rawSeconds);
        float now = Time.unscaledTime;
        int previousState;
        int previousSeconds;
        float previousTime;
        if (!satelliteDisplayedWindowState.TryGetValue(satelliteName, out previousState)
            || !satelliteDisplayedWindowSeconds.TryGetValue(satelliteName, out previousSeconds)
            || !satelliteDisplayedWindowUpdatedAt.TryGetValue(satelliteName, out previousTime)
            || previousState != state)
        {
            satelliteDisplayedWindowState[satelliteName] = state;
            satelliteDisplayedWindowSeconds[satelliteName] = rawSeconds;
            satelliteDisplayedWindowUpdatedAt[satelliteName] = now;
            return rawSeconds;
        }

        int elapsedSeconds = Mathf.Max(0, Mathf.RoundToInt(now - previousTime));
        int expectedSeconds = Mathf.Max(0, previousSeconds - elapsedSeconds);
        int maximumDropSeconds = Mathf.Max(1, elapsedSeconds + 3);
        int displayedSeconds;
        if (state == 1 && expectedSeconds <= 0 && rawSeconds <= 3)
        {
            displayedSeconds = 0;
        }
        else if (rawSeconds <= expectedSeconds)
        {
            displayedSeconds = Mathf.Max(rawSeconds, expectedSeconds - maximumDropSeconds);
        }
        else
        {
            displayedSeconds = expectedSeconds;
        }

        satelliteDisplayedWindowState[satelliteName] = state;
        displayedSeconds = Mathf.Max(0, displayedSeconds);
        satelliteDisplayedWindowSeconds[satelliteName] = displayedSeconds;
        satelliteDisplayedWindowUpdatedAt[satelliteName] = now;
        return displayedSeconds;
    }

    private void StabilizeDisplayedRouteTimes(SatelliteRouteEstimateRecord estimate)
    {
        if (estimate == null || string.IsNullOrEmpty(estimate.satelliteName))
        {
            return;
        }

        estimate.originWindowSeconds = StabilizedDisplayedCountdownSeconds(estimate.satelliteName, "origin", estimate.originWindowSeconds);
        estimate.targetWindowSeconds = StabilizedDisplayedCountdownSeconds(estimate.satelliteName, "target", estimate.targetWindowSeconds);
        estimate.routeReadySeconds = StabilizedDisplayedCountdownSeconds(estimate.satelliteName, "routeReady", estimate.routeReadySeconds);
        estimate.scheduledStartSeconds = StabilizedDisplayedCountdownSeconds(estimate.satelliteName, "scheduledStart", estimate.scheduledStartSeconds);
        estimate.uplinkWindowOpenSeconds = StabilizedDisplayedCountdownSeconds(estimate.satelliteName, "uplinkOpen", estimate.uplinkWindowOpenSeconds);
        estimate.downlinkWindowOpenSeconds = StabilizedDisplayedCountdownSeconds(estimate.satelliteName, "downlinkOpen", estimate.downlinkWindowOpenSeconds);
        estimate.downlinkWaitSeconds = StabilizedDisplayedCountdownSeconds(estimate.satelliteName, "downlinkWait", estimate.downlinkWaitSeconds);
        int deliverySeconds = StabilizedDisplayedCountdownSeconds(estimate.satelliteName, "delivery", estimate.estimatedDeliverySeconds);
        estimate.estimatedDeliverySeconds = deliverySeconds;
        estimate.totalDeliverySeconds = deliverySeconds;
        estimate.score = StabilizedDisplayedRouteScore(estimate.satelliteName, estimate.score);
    }

    private int StabilizedDisplayedCountdownSeconds(string satelliteName, string key, int rawSeconds)
    {
        if (string.IsNullOrEmpty(satelliteName))
        {
            return Mathf.Max(0, rawSeconds);
        }

        rawSeconds = Mathf.Max(0, rawSeconds);
        string cacheKey = satelliteName + "|" + key;
        float now = Time.unscaledTime;
        int previousSeconds;
        float previousTime;
        if (!satelliteDisplayedRouteTimeSeconds.TryGetValue(cacheKey, out previousSeconds)
            || !satelliteDisplayedRouteTimeUpdatedAt.TryGetValue(cacheKey, out previousTime))
        {
            satelliteDisplayedRouteTimeSeconds[cacheKey] = rawSeconds;
            satelliteDisplayedRouteTimeUpdatedAt[cacheKey] = now;
            return rawSeconds;
        }

        int elapsedSeconds = Mathf.Max(0, Mathf.RoundToInt(now - previousTime));
        int expectedSeconds = Mathf.Max(0, previousSeconds - elapsedSeconds);
        int maximumDeltaSeconds = Mathf.Max(2, elapsedSeconds + 3);
        int displayedSeconds = Mathf.Clamp(rawSeconds, expectedSeconds - maximumDeltaSeconds, expectedSeconds + maximumDeltaSeconds);
        displayedSeconds = Mathf.Max(0, displayedSeconds);
        satelliteDisplayedRouteTimeSeconds[cacheKey] = displayedSeconds;
        satelliteDisplayedRouteTimeUpdatedAt[cacheKey] = now;
        return displayedSeconds;
    }

    private float StabilizedDisplayedRouteScore(string satelliteName, float rawScore)
    {
        if (string.IsNullOrEmpty(satelliteName) || float.IsNaN(rawScore) || float.IsInfinity(rawScore))
        {
            return rawScore;
        }

        float now = Time.unscaledTime;
        float previousScore;
        float previousTime;
        if (!satelliteDisplayedRouteScores.TryGetValue(satelliteName, out previousScore)
            || !satelliteDisplayedRouteScoreUpdatedAt.TryGetValue(satelliteName, out previousTime))
        {
            satelliteDisplayedRouteScores[satelliteName] = rawScore;
            satelliteDisplayedRouteScoreUpdatedAt[satelliteName] = now;
            return rawScore;
        }

        float elapsedSeconds = Mathf.Max(0f, now - previousTime);
        float maxDelta = Mathf.Max(8f, elapsedSeconds * 16f);
        float displayedScore = Mathf.Clamp(rawScore, previousScore - maxDelta, previousScore + maxDelta);
        satelliteDisplayedRouteScores[satelliteName] = displayedScore;
        satelliteDisplayedRouteScoreUpdatedAt[satelliteName] = now;
        return displayedScore;
    }

    private string TransferDurationText(SatelliteData satellite, SatelliteRouteEstimateRecord estimate)
    {
        if (ShouldUsePayloadTransferDurationText() && missionManager != null)
        {
            return FormatSeconds(missionManager.EstimatePayloadTransferDurationSeconds(CurrentSatellitePayloadKb(), satellite));
        }
        if (estimate != null)
        {
            return FormatSeconds(Mathf.Max(1, estimate.estimatedDeliverySeconds));
        }
        if (satellite == null)
        {
            return "계산 중";
        }
        return FormatSeconds(Mathf.Max(1, Mathf.Max(0, satellite.nextWindowMinutes) * 60 + satellite.latencySeconds));
    }

    private bool ShouldUsePayloadTransferDurationText()
    {
        return requestFlowPanel != null
            && requestFlowPanel.gameObject.activeSelf
            && missionManager != null
            && (composingQueuedMessageInFlow || missionManager.Phase == MissionPhase.Idle);
    }

    private SatelliteRouteEstimateRecord RouteEstimate(SatelliteData satellite, string targetName, int sendDelay)
    {
        if (satelliteManager == null || satellite == null)
        {
            return null;
        }
        int payloadKb = CurrentSatellitePayloadKb();
        SatelliteRouteEstimateRecord cachedEstimate = CachedRouteEstimate(satellite, targetName, sendDelay);
        if (cachedEstimate != null)
        {
            return cachedEstimate;
        }
        return satelliteManager.GetRouteEstimate(satellite, CurrentOriginName(), targetName, sendDelay, payloadKb);
    }

    private SatelliteRouteEstimateRecord CachedRouteEstimate(SatelliteData satellite, string targetName, int sendDelay)
    {
        if (satellite == null || !SatelliteListEstimateCacheMatches(CurrentOriginName(), targetName, sendDelay, CurrentSatellitePayloadKb()))
        {
            return null;
        }

        SatelliteRouteEstimateRecord estimate;
        return satelliteListRouteEstimateCache.TryGetValue(satellite.satelliteName, out estimate) ? estimate : null;
    }

    private bool SatelliteListEstimateCacheMatches(string originName, string targetName, int sendDelay, int payloadKb)
    {
        return satelliteListRouteCachePayloadKb == payloadKb
            && satelliteListRouteCacheDelayMinutes == sendDelay
            && satelliteListRouteCacheOriginName == originName
            && satelliteListRouteCacheTargetName == targetName;
    }

    private int CurrentSatellitePayloadKb()
    {
        return composingQueuedMessageInFlow ? Mathf.Max(1, QueuedFlowPayloadKb()) : missionManager != null ? missionManager.MessageSizeKb : 128;
    }

    private bool CanServeRoute(SatelliteData satellite, string targetName, int sendDelay)
    {
        SatelliteRouteEstimateRecord estimate = RouteEstimate(satellite, targetName, sendDelay);
        return estimate != null && estimate.canServe;
    }

    private string CurrentOriginName()
    {
        if (composingQueuedMessageInFlow && !string.IsNullOrEmpty(queueOriginName))
        {
            return queueOriginName;
        }
        return missionManager != null ? missionManager.OriginName : OrbitShareDestinations.Seoul;
    }

    private string CurrentTargetName()
    {
        if (composingQueuedMessageInFlow && !string.IsNullOrEmpty(queueTargetName))
        {
            return queueTargetName;
        }
        return missionManager != null && missionManager.SelectedTarget != null ? missionManager.SelectedTarget.targetName : OrbitShareDestinations.DefaultTargetName;
    }

    private bool IsRecommendedSatellite(SatelliteData satellite, string targetName, int sendDelay)
    {
        if (satelliteManager == null || satellite == null)
        {
            return false;
        }

        int payloadKb = composingQueuedMessageInFlow ? Mathf.Max(1, QueuedFlowPayloadKb()) : missionManager != null ? missionManager.MessageSizeKb : 128;
        if (SatelliteListEstimateCacheMatches(CurrentOriginName(), targetName, sendDelay, payloadKb))
        {
            return !string.IsNullOrEmpty(satelliteListRecommendedSatelliteName)
                && satelliteListRecommendedSatelliteName == satellite.satelliteName;
        }
        List<SatelliteRouteEstimateRecord> ranked = satelliteManager.GetRankedRouteEstimates(SatelliteListFilter.All, CurrentOriginName(), targetName, sendDelay, payloadKb);
        return ranked.Count > 0 && ranked[0].satelliteName == satellite.satelliteName;
    }

    private string EmptySatelliteListText()
    {
        string query = string.IsNullOrWhiteSpace(satelliteSearchQuery) ? string.Empty : "검색어 : " + satelliteSearchQuery.Trim() + "\n";
        return "조건에 맞는 위성망이 없습니다.\n"
            + query
            + "클릭하면 검색을 초기화합니다.";
    }

    private void ResetSatelliteListFiltersFromEmptyState()
    {
        satelliteFilter = SatelliteListFilter.All;
        satelliteSearchQuery = string.Empty;
        if (satelliteSearchInputField != null)
        {
            satelliteSearchInputField.SetTextWithoutNotify(string.Empty);
        }
        nextSatelliteListReorderTime = 0f;
        ShowToast("위성망 필터와 검색어를 초기화했습니다.");
        RefreshSatelliteList();
    }
}
