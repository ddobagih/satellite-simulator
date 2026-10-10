using System;
using System.Collections.Generic;
using UnityEngine;

public enum SatelliteListFilter
{
    Recommended,
    LowCost,
    TransferSpeed,
    LowLatency,

    All = Recommended,
    Fast = TransferSpeed,
    Reliable = Recommended,
    LowCongestion = Recommended
}

public class SatelliteManager : MonoBehaviour
{
    [Tooltip("켜면 기존 24개 가상 시연용 위성 데이터를 시작 시 등록합니다. 기본값은 실제 TLE 위성 우선입니다.")]
    public bool seedVirtualDemoSatellitesOnAwake;

    public event Action OnDataChanged;
    public event Action<SatelliteData> OnSelectionChanged;

    public readonly List<SatelliteData> Satellites = new List<SatelliteData>();

    public SatelliteData SelectedSatellite { get; private set; }
    public string MissionSatelliteName { get; private set; }

    private const int DefaultPayloadKb = 128;
    private const string RealSatelliteNamePrefix = "REAL-";
    private const string RealSatelliteOwnerName = "CelesTrak TLE";
    private const string RealSatelliteUsageType = "실제 TLE 위성";
    private readonly Dictionary<string, Transform> satelliteTransforms = new Dictionary<string, Transform>();
    private readonly Dictionary<string, Transform> targetTransforms = new Dictionary<string, Transform>();
    private readonly Dictionary<string, RouteAngleSample> routeAngleSamples = new Dictionary<string, RouteAngleSample>();

    private struct RouteAngleSample
    {
        public float angleDegrees;
        public float timeSeconds;
        public float trendDegreesPerSecond;
    }

    private void Awake()
    {
        Satellites.Clear();
        if (seedVirtualDemoSatellitesOnAwake)
        {
            SeedVirtualDemoSatellites();
        }
    }

    public void SeedVirtualDemoSatellites()
    {
        if (FindSatelliteByName("SAT-S01") != null)
        {
            if (SelectedSatellite == null)
            {
                SelectSatellite("SAT-S01");
            }
            return;
        }

        AddSatellite("SAT-S01", SatelliteOwnerType.Shared, "OrbitShare", SatelliteAvailabilityState.Available, "서울/태평양 우선 단문 중계", 550f, 7.6f, 82f, "LEO-A", 0, 2, 4, 96, true, true, false, true, false, false, true);
        AddSatellite("SAT-P01", SatelliteOwnerType.Personal, "개인망", SatelliteAvailabilityState.Available, "개인 전용", 520f, 7.5f, 91f, "LEO-A", 3, 4, 5, 89, true, false, false, true, false, false, false);
        AddSatellite("SAT-C01", SatelliteOwnerType.Company, "기업 백본", SatelliteAvailabilityState.InMission, "기업 트래픽", 650f, 7.4f, 74f, "MEO-B", 12, 3, 7, 91, false, true, true, false, true, false, true);

        for (int i = 4; i <= 24; i++)
        {
            string orbitSlot = i % 3 == 0 ? "LEO-A" : i % 3 == 1 ? "MEO-B" : "Polar-C";
            int nextWindow = (i * 3) % 31;
            int fee = 2 + (i % 4);
            int latency = 4 + (i % 6);
            int reliability = 86 + (i % 10);
            bool supportsSeoul = i % 2 == 0 || i % 5 == 0;
            bool supportsNewYork = i % 3 != 0 || i % 7 == 0;
            bool supportsLondon = i % 4 != 0 || i % 6 == 0;
            bool supportsTokyo = i % 2 != 0 || i % 7 == 0;
            bool supportsParis = i % 3 == 0 || i % 5 == 1 || i % 4 == 1;
            bool supportsSydney = i % 4 == 0 || i % 5 == 2 || i % 7 == 0;
            bool supportsLosAngeles = i % 3 == 1 || i % 5 == 0 || i % 6 == 2;
            SatelliteAvailabilityState state = i % 8 == 0 ? SatelliteAvailabilityState.InMission : SatelliteAvailabilityState.Available;
            AddSatellite(
                "SAT-R" + i.ToString("00"),
                SatelliteOwnerType.Shared,
                "OrbitShare",
                state,
                "단문 중계 후보",
                540f + (i % 5) * 35f,
                7.2f + (i % 4) * 0.12f,
                58f + (i % 6) * 7f,
                orbitSlot,
                nextWindow,
                fee,
                latency,
                reliability,
                supportsSeoul,
                supportsNewYork,
                supportsLondon,
                supportsTokyo,
                supportsParis,
                supportsSydney,
                supportsLosAngeles);
        }

        if (SelectedSatellite == null)
        {
            SelectSatellite("SAT-S01");
        }
        else
        {
            NotifyChanged();
        }
    }

    private void AddSatellite(
        string satelliteName,
        SatelliteOwnerType ownerType,
        string ownerName,
        SatelliteAvailabilityState availabilityState,
        string usageType,
        float altitudeKm,
        float speedKmPerSec,
        float batteryPercent,
        string orbitSlot,
        int nextWindowMinutes,
        int feePer32KbCredits,
        int latencySeconds,
        int reliabilityPercent,
        bool supportsSeoul,
        bool supportsNewYork,
        bool supportsLondon,
        bool supportsTokyo,
        bool supportsParis,
        bool supportsSydney,
        bool supportsLosAngeles)
    {
        int congestionPercent = Mathf.Clamp(10 + nextWindowMinutes * 2 + Mathf.Max(0, latencySeconds - 4) * 4 + (availabilityState == SatelliteAvailabilityState.InMission ? 35 : 0), 0, 95);
        Satellites.Add(new SatelliteData(
            satelliteName,
            ownerType,
            ownerName,
            availabilityState,
            usageType,
            altitudeKm,
            speedKmPerSec,
            batteryPercent,
            orbitSlot,
            nextWindowMinutes,
            feePer32KbCredits,
            latencySeconds,
            reliabilityPercent,
            congestionPercent,
            supportsSeoul,
            supportsNewYork,
            supportsLondon,
            supportsTokyo,
            supportsParis,
            supportsSydney,
            supportsLosAngeles));
    }

    public void RegisterSatelliteTransform(string satelliteName, Transform satelliteTransform)
    {
        if (string.IsNullOrEmpty(satelliteName) || satelliteTransform == null)
        {
            return;
        }
        satelliteTransforms[satelliteName] = satelliteTransform;
    }

    public SatelliteData UpsertRealSatellite(string satelliteName, Transform satelliteTransform, float altitudeKm, float speedKmPerSec)
    {
        return UpsertRealSatellite(satelliteName, satelliteTransform, altitudeKm, speedKmPerSec, 0, 2, 6, 90, 24, true);
    }

    public SatelliteData UpsertRealSatellite(
        string satelliteName,
        Transform satelliteTransform,
        float altitudeKm,
        float speedKmPerSec,
        int nextWindowMinutes,
        int feePer32KbCredits,
        int latencySeconds,
        int reliabilityPercent,
        int congestionPercent,
        bool supportsAllKnownDestinations)
    {
        string normalizedName = NormalizeRealSatelliteName(satelliteName);
        if (string.IsNullOrEmpty(normalizedName))
        {
            return null;
        }

        float safeAltitudeKm = Mathf.Max(0f, altitudeKm);
        float safeSpeedKmPerSec = Mathf.Max(0.1f, speedKmPerSec);
        int safeNextWindowMinutes = Mathf.Max(0, nextWindowMinutes);
        int safeFeePer32KbCredits = Mathf.Max(1, feePer32KbCredits);
        int safeLatencySeconds = Mathf.Max(1, latencySeconds);
        int safeReliabilityPercent = Mathf.Clamp(reliabilityPercent, 1, 100);
        int safeCongestionPercent = Mathf.Clamp(congestionPercent, 0, 100);
        bool shouldNotify = false;

        SatelliteData satellite = FindSatelliteByName(normalizedName);
        bool shouldSelectInsertedRealSatellite = false;
        if (satellite == null)
        {
            satellite = new SatelliteData(
                normalizedName,
                SatelliteOwnerType.Shared,
                RealSatelliteOwnerName,
                SatelliteAvailabilityState.Available,
                RealSatelliteUsageType,
                safeAltitudeKm,
                safeSpeedKmPerSec,
                100f,
                "LEO-TLE",
                safeNextWindowMinutes,
                safeFeePer32KbCredits,
                safeLatencySeconds,
                safeReliabilityPercent,
                safeCongestionPercent,
                supportsAllKnownDestinations,
                supportsAllKnownDestinations,
                supportsAllKnownDestinations,
                supportsAllKnownDestinations,
                supportsAllKnownDestinations,
                supportsAllKnownDestinations,
                supportsAllKnownDestinations);
            Satellites.Add(satellite);
            shouldSelectInsertedRealSatellite = SelectedSatellite == null;
            shouldNotify = true;
        }
        else
        {
            shouldNotify = Mathf.Abs(satellite.altitudeKm - safeAltitudeKm) > 0.5f
                || Mathf.Abs(satellite.speedKmPerSec - safeSpeedKmPerSec) > 0.01f
                || satellite.ownerType != SatelliteOwnerType.Shared
                || satellite.ownerName != RealSatelliteOwnerName
                || satellite.usageType != RealSatelliteUsageType
                || satellite.orbitSlot != "LEO-TLE"
                || satellite.nextWindowMinutes != safeNextWindowMinutes
                || satellite.feePer32KbCredits != safeFeePer32KbCredits
                || satellite.latencySeconds != safeLatencySeconds
                || satellite.reliabilityPercent != safeReliabilityPercent
                || satellite.congestionPercent != safeCongestionPercent
                || satellite.supportsSeoul != supportsAllKnownDestinations
                || satellite.supportsNewYork != supportsAllKnownDestinations
                || satellite.supportsLondon != supportsAllKnownDestinations
                || satellite.supportsTokyo != supportsAllKnownDestinations
                || satellite.supportsParis != supportsAllKnownDestinations
                || satellite.supportsSydney != supportsAllKnownDestinations
                || satellite.supportsLosAngeles != supportsAllKnownDestinations;
            satellite.ownerType = SatelliteOwnerType.Shared;
            satellite.ownerName = RealSatelliteOwnerName;
            if (satellite.availabilityState != SatelliteAvailabilityState.Reserved
                && satellite.availabilityState != SatelliteAvailabilityState.InMission
                && satellite.availabilityState != SatelliteAvailabilityState.MissionComplete)
            {
                shouldNotify = shouldNotify || satellite.availabilityState != SatelliteAvailabilityState.Available;
                satellite.availabilityState = SatelliteAvailabilityState.Available;
            }
            satellite.usageType = RealSatelliteUsageType;
            satellite.altitudeKm = safeAltitudeKm;
            satellite.speedKmPerSec = safeSpeedKmPerSec;
            satellite.batteryPercent = Mathf.Max(35f, satellite.batteryPercent);
            satellite.orbitSlot = "LEO-TLE";
            satellite.nextWindowMinutes = safeNextWindowMinutes;
            satellite.feePer32KbCredits = safeFeePer32KbCredits;
            satellite.latencySeconds = safeLatencySeconds;
            satellite.reliabilityPercent = safeReliabilityPercent;
            satellite.congestionPercent = safeCongestionPercent;
            satellite.supportsSeoul = supportsAllKnownDestinations;
            satellite.supportsNewYork = supportsAllKnownDestinations;
            satellite.supportsLondon = supportsAllKnownDestinations;
            satellite.supportsTokyo = supportsAllKnownDestinations;
            satellite.supportsParis = supportsAllKnownDestinations;
            satellite.supportsSydney = supportsAllKnownDestinations;
            satellite.supportsLosAngeles = supportsAllKnownDestinations;
        }

        if (satelliteTransform != null)
        {
            shouldNotify = shouldNotify || GetSatelliteTransform(normalizedName) != satelliteTransform;
            RegisterSatelliteTransform(normalizedName, satelliteTransform);
        }

        if (shouldSelectInsertedRealSatellite)
        {
            SelectedSatellite = satellite;
            if (OnSelectionChanged != null)
            {
                OnSelectionChanged.Invoke(SelectedSatellite);
            }
        }
        if (shouldNotify)
        {
            NotifyChanged();
        }
        return satellite;
    }

    public Transform GetSatelliteTransform(string satelliteName)
    {
        Transform satelliteTransform;
        if (!string.IsNullOrEmpty(satelliteName) && satelliteTransforms.TryGetValue(satelliteName, out satelliteTransform) && satelliteTransform != null)
        {
            return satelliteTransform;
        }
        return null;
    }

    public void RegisterTargetTransform(string targetName, Transform targetTransform)
    {
        if (string.IsNullOrEmpty(targetName) || targetTransform == null)
        {
            return;
        }
        targetTransforms[targetName] = targetTransform;
    }

    public SatelliteData GetSharedSatellite()
    {
        SatelliteData primary = Satellites.Find(satellite => satellite.satelliteName == "SAT-S01");
        if (primary != null)
        {
            return primary;
        }
        return Satellites.Find(satellite => satellite.ownerType == SatelliteOwnerType.Shared);
    }

    public SatelliteData GetMissionSatellite()
    {
        SatelliteData missionSatellite = Satellites.Find(satellite => satellite.satelliteName == MissionSatelliteName);
        return missionSatellite != null ? missionSatellite : GetSharedSatellite();
    }

    public void SelectSatellite(string satelliteName)
    {
        SatelliteData selected = Satellites.Find(satellite => satellite.satelliteName == satelliteName);
        if (selected == null)
        {
            return;
        }

        if (SelectedSatellite == selected)
        {
            return;
        }

        SelectedSatellite = selected;
        if (OnSelectionChanged != null)
        {
            OnSelectionChanged.Invoke(SelectedSatellite);
        }
    }

    public bool CanReserveSelected(string targetName, int requestedDelayMinutes)
    {
        return SelectedSatellite != null && CanServeSatellite(SelectedSatellite, targetName, requestedDelayMinutes);
    }

    public bool CanReserveSelected()
    {
        return CanReserveSelected(OrbitShareDestinations.DefaultTargetName, 0);
    }

    public bool CanServeSatellite(SatelliteData satellite, string targetName, int requestedDelayMinutes)
    {
        SatelliteRouteEstimateRecord estimate = GetRouteEstimate(satellite, targetName, requestedDelayMinutes, DefaultPayloadKb);
        return estimate != null && estimate.canServe;
    }

    public bool ReserveSelected(string targetName, int requestedDelayMinutes)
    {
        return ReserveSelected(targetName, requestedDelayMinutes, true);
    }

    public bool ReserveSelected(string targetName, int requestedDelayMinutes, bool notifyChanged)
    {
        if (!CanReserveSelected(targetName, requestedDelayMinutes))
        {
            return false;
        }

        MissionSatelliteName = SelectedSatellite.satelliteName;
        SelectedSatellite.availabilityState = SatelliteAvailabilityState.Reserved;
        SelectedSatellite.currentMissionTarget = targetName;
        if (notifyChanged)
        {
            NotifyChanged();
        }
        return true;
    }

    public SatelliteData FindBestSatellite(string targetName, int requestedDelayMinutes)
    {
        return FindBestSatellite(targetName, requestedDelayMinutes, DefaultPayloadKb);
    }

    public SatelliteData FindBestSatellite(string targetName, int requestedDelayMinutes, int payloadKb)
    {
        List<SatelliteRouteEstimateRecord> estimates = GetRankedRouteEstimates(SatelliteListFilter.Recommended, targetName, requestedDelayMinutes, payloadKb);
        if (estimates.Count == 0)
        {
            return null;
        }
        for (int i = 0; i < estimates.Count; i++)
        {
            if (estimates[i] != null && estimates[i].canServe)
            {
                return FindSatelliteByName(estimates[i].satelliteName);
            }
        }
        return null;
    }

    public SatelliteData FindBestSatellite(string originName, string targetName, int requestedDelayMinutes, int payloadKb)
    {
        List<SatelliteRouteEstimateRecord> estimates = GetRankedRouteEstimates(SatelliteListFilter.Recommended, originName, targetName, requestedDelayMinutes, payloadKb);
        if (estimates.Count == 0)
        {
            return null;
        }
        for (int i = 0; i < estimates.Count; i++)
        {
            if (estimates[i] != null && estimates[i].canServe)
            {
                return FindSatelliteByName(estimates[i].satelliteName);
            }
        }
        return null;
    }

    public SatelliteRouteEstimateRecord GetRouteEstimate(SatelliteData satellite, string targetName, int requestedDelayMinutes, int payloadKb)
    {
        return BuildRouteEstimate(satellite, targetName, ResolveTargetTransform(targetName), requestedDelayMinutes, payloadKb);
    }

    public SatelliteRouteEstimateRecord GetRouteEstimate(SatelliteData satellite, string originName, string targetName, int requestedDelayMinutes, int payloadKb)
    {
        return BuildRouteEstimate(satellite, originName, ResolveTargetTransform(originName), targetName, ResolveTargetTransform(targetName), requestedDelayMinutes, payloadKb);
    }

    public List<SatelliteRecommendationRecord> GetDestinationRecommendations(string targetName, int requestedDelayMinutes, int payloadKb)
    {
        return GetDestinationRecommendations(OrbitShareDestinations.Seoul, targetName, requestedDelayMinutes, payloadKb);
    }

    public List<SatelliteRecommendationRecord> GetDestinationRecommendations(string originName, string targetName, int requestedDelayMinutes, int payloadKb)
    {
        List<SatelliteRecommendationRecord> recommendations = new List<SatelliteRecommendationRecord>();
        HashSet<string> usedSatelliteNames = new HashSet<string>();
        AddRecommendation(recommendations, usedSatelliteNames, "통신 가능 시간", PickRecommendedEstimate(originName, targetName, requestedDelayMinutes, payloadKb, SatelliteListFilter.LowLatency, usedSatelliteNames), payloadKb, requestedDelayMinutes);
        AddRecommendation(recommendations, usedSatelliteNames, "예상 소요 시간", PickRecommendedEstimate(originName, targetName, requestedDelayMinutes, payloadKb, SatelliteListFilter.TransferSpeed, usedSatelliteNames), payloadKb, requestedDelayMinutes);
        AddRecommendation(recommendations, usedSatelliteNames, "비용", PickRecommendedEstimate(originName, targetName, requestedDelayMinutes, payloadKb, SatelliteListFilter.LowCost, usedSatelliteNames), payloadKb, requestedDelayMinutes);
        return recommendations;
    }

    public string GetDestinationRecommendationSummary(string targetName, int requestedDelayMinutes, int payloadKb)
    {
        List<SatelliteRecommendationRecord> recommendations = GetDestinationRecommendations(targetName, requestedDelayMinutes, payloadKb);
        if (recommendations.Count == 0)
        {
            return "추천 가능한 위성이 없습니다.";
        }

        string summary = string.Empty;
        for (int i = 0; i < recommendations.Count; i++)
        {
            if (i > 0)
            {
                summary += " / ";
            }
            summary += recommendations[i].categoryLabel + " " + recommendations[i].shortSummary;
        }
        return summary;
    }

    public string GetDestinationRecommendationSummary(string originName, string targetName, int requestedDelayMinutes, int payloadKb)
    {
        List<SatelliteRecommendationRecord> recommendations = GetDestinationRecommendations(originName, targetName, requestedDelayMinutes, payloadKb);
        if (recommendations.Count == 0)
        {
            return "추천 가능한 위성이 없습니다.";
        }

        string summary = string.Empty;
        for (int i = 0; i < recommendations.Count; i++)
        {
            if (i > 0)
            {
                summary += " / ";
            }
            summary += recommendations[i].categoryLabel + " " + recommendations[i].shortSummary;
        }
        return summary;
    }

    public string GetDestinationRecommendationShortSummary(string targetName, int requestedDelayMinutes, int payloadKb)
    {
        SatelliteData satellite = FindBestSatellite(targetName, requestedDelayMinutes, payloadKb);
        if (satellite == null)
        {
            return "추천 위성 없음";
        }

        SatelliteRouteEstimateRecord estimate = GetRouteEstimate(satellite, targetName, requestedDelayMinutes, payloadKb);
        return estimate != null && !string.IsNullOrEmpty(estimate.reasonSummary)
            ? estimate.reasonSummary
            : satellite.GetShortRecommendationReason(payloadKb, requestedDelayMinutes);
    }

    public string GetSelectedSatelliteRecommendationReason(string targetName, int requestedDelayMinutes, int payloadKb)
    {
        return GetSelectedSatelliteRecommendationReason(OrbitShareDestinations.Seoul, targetName, requestedDelayMinutes, payloadKb);
    }

    public string GetSelectedSatelliteRecommendationReason(string originName, string targetName, int requestedDelayMinutes, int payloadKb)
    {
        if (SelectedSatellite == null)
        {
            return "선택 위성 없음";
        }
        if (string.IsNullOrEmpty(targetName))
        {
            return SelectedSatellite.satelliteName + " · 목적지 선택 필요";
        }

        SatelliteRouteEstimateRecord estimate = GetRouteEstimate(SelectedSatellite, originName, targetName, requestedDelayMinutes, payloadKb);
        if (estimate == null)
        {
            return SelectedSatellite.satelliteName + " · 예약 조건 확인 필요";
        }
        if (!estimate.supportsTarget)
        {
            return SelectedSatellite.satelliteName + " · 목적지 미지원";
        }
        if (!estimate.isAvailableState)
        {
            return SelectedSatellite.satelliteName + " · " + SelectedSatellite.StateLabel;
        }
        if (!estimate.hasSharedAccess)
        {
            return SelectedSatellite.satelliteName + " · 공유망 미지원";
        }
        if (!estimate.hasEnoughBattery)
        {
            return SelectedSatellite.satelliteName + " · 배터리 부족";
        }
        if (!estimate.canServe)
        {
            return SelectedSatellite.satelliteName + " · " + estimate.availabilitySummary;
        }
        return estimate.reasonSummary;
    }

    private SatelliteRouteEstimateRecord PickRecommendedEstimate(string targetName, int requestedDelayMinutes, int payloadKb, SatelliteListFilter filter, HashSet<string> usedSatelliteNames)
    {
        List<SatelliteRouteEstimateRecord> estimates = GetRankedRouteEstimates(filter, targetName, requestedDelayMinutes, payloadKb);
        for (int i = 0; i < estimates.Count; i++)
        {
            if (usedSatelliteNames == null || !usedSatelliteNames.Contains(estimates[i].satelliteName))
            {
                return estimates[i];
            }
        }
        return estimates.Count > 0 ? estimates[0] : null;
    }

    private SatelliteRouteEstimateRecord PickRecommendedEstimate(string originName, string targetName, int requestedDelayMinutes, int payloadKb, SatelliteListFilter filter, HashSet<string> usedSatelliteNames)
    {
        List<SatelliteRouteEstimateRecord> estimates = GetRankedRouteEstimates(filter, originName, targetName, requestedDelayMinutes, payloadKb);
        for (int i = 0; i < estimates.Count; i++)
        {
            if (usedSatelliteNames == null || !usedSatelliteNames.Contains(estimates[i].satelliteName))
            {
                return estimates[i];
            }
        }
        return estimates.Count > 0 ? estimates[0] : null;
    }

    private void AddRecommendation(List<SatelliteRecommendationRecord> recommendations, HashSet<string> usedSatelliteNames, string categoryLabel, SatelliteRouteEstimateRecord estimate, int payloadKb, int requestedDelayMinutes)
    {
        if (estimate == null)
        {
            return;
        }

        SatelliteData satellite = FindSatelliteByName(estimate.satelliteName);
        if (satellite == null)
        {
            return;
        }

        SatelliteRecommendationRecord recommendation = new SatelliteRecommendationRecord();
        recommendation.categoryLabel = categoryLabel;
        recommendation.satelliteName = satellite.satelliteName;
        recommendation.estimatedDeliverySeconds = estimate.estimatedDeliverySeconds;
        recommendation.estimatedWindowSeconds = estimate.estimatedWindowSeconds;
        recommendation.estimatedWindowMinutes = estimate.estimatedWindowMinutes;
        recommendation.estimatedAvailableForSeconds = estimate.estimatedAvailableForSeconds;
        recommendation.estimatedPriceUsd = estimate.estimatedPriceUsd > 0f ? estimate.estimatedPriceUsd : satellite.EstimatePriceUsd(payloadKb);
        recommendation.reliabilityPercent = satellite.RecentSuccessPercent;
        recommendation.congestionPercent = satellite.congestionPercent;
        recommendation.routeAngleDegrees = estimate.routeAngleDegrees;
        recommendation.score = estimate.score;
        recommendation.summary = categoryLabel + " · " + satellite.satelliteName
            + " · 예상 소요 시간 " + SatelliteRouteEstimator.FormatSeconds(recommendation.estimatedDeliverySeconds)
            + " · 예상 $" + recommendation.estimatedPriceUsd.ToString("0.00");
        recommendation.shortSummary = estimate.reasonSummary;
        recommendation.reasonSummary = estimate.reasonSummary;
        recommendations.Add(recommendation);
        if (usedSatelliteNames != null)
        {
            usedSatelliteNames.Add(satellite.satelliteName);
        }
    }

    public List<SatelliteData> GetFilteredSatellites(SatelliteListFilter filter, string targetName, int requestedDelayMinutes)
    {
        List<SatelliteRouteEstimateRecord> estimates = GetRankedRouteEstimates(filter, targetName, requestedDelayMinutes, DefaultPayloadKb);
        return SatellitesFromEstimates(estimates);
    }

    public List<SatelliteData> GetFilteredSatellites(SatelliteListFilter filter, string originName, string targetName, int requestedDelayMinutes)
    {
        List<SatelliteRouteEstimateRecord> estimates = GetRankedRouteEstimates(filter, originName, targetName, requestedDelayMinutes, DefaultPayloadKb);
        return SatellitesFromEstimates(estimates);
    }

    private List<SatelliteData> SatellitesFromEstimates(List<SatelliteRouteEstimateRecord> estimates)
    {
        List<SatelliteData> filtered = new List<SatelliteData>();
        for (int i = 0; i < estimates.Count; i++)
        {
            SatelliteData satellite = FindSatelliteByName(estimates[i].satelliteName);
            if (satellite != null)
            {
                filtered.Add(satellite);
            }
        }
        return filtered;
    }

    public List<SatelliteRouteEstimateRecord> GetRankedRouteEstimates(SatelliteListFilter filter, string targetName, int requestedDelayMinutes, int payloadKb)
    {
        List<SatelliteRouteEstimateRecord> estimates = new List<SatelliteRouteEstimateRecord>();
        for (int i = 0; i < Satellites.Count; i++)
        {
            SatelliteRouteEstimateRecord estimate = GetRouteEstimate(Satellites[i], targetName, requestedDelayMinutes, payloadKb);
            if (estimate == null || !ShouldIncludeEstimate(filter, estimate, Satellites[i], payloadKb))
            {
                continue;
            }
            estimates.Add(estimate);
        }

        estimates.Sort(delegate (SatelliteRouteEstimateRecord left, SatelliteRouteEstimateRecord right)
        {
            return CompareRouteEstimates(left, right, filter);
        });
        return estimates;
    }

    public List<SatelliteRouteEstimateRecord> GetRankedRouteEstimates(SatelliteListFilter filter, string originName, string targetName, int requestedDelayMinutes, int payloadKb)
    {
        List<SatelliteRouteEstimateRecord> estimates = new List<SatelliteRouteEstimateRecord>();
        for (int i = 0; i < Satellites.Count; i++)
        {
            SatelliteRouteEstimateRecord estimate = GetRouteEstimate(Satellites[i], originName, targetName, requestedDelayMinutes, payloadKb);
            if (estimate == null || !ShouldIncludeEstimate(filter, estimate, Satellites[i], payloadKb))
            {
                continue;
            }
            estimates.Add(estimate);
        }

        estimates.Sort(delegate (SatelliteRouteEstimateRecord left, SatelliteRouteEstimateRecord right)
        {
            return CompareRouteEstimates(left, right, filter);
        });
        return estimates;
    }

    private bool ShouldIncludeEstimate(SatelliteListFilter filter, SatelliteRouteEstimateRecord estimate, SatelliteData satellite, int payloadKb)
    {
        return true;
    }

    public int CompareRouteEstimates(SatelliteRouteEstimateRecord left, SatelliteRouteEstimateRecord right, SatelliteListFilter filter)
    {
        SatelliteData leftSatellite = left != null ? FindSatelliteByName(left.satelliteName) : null;
        SatelliteData rightSatellite = right != null ? FindSatelliteByName(right.satelliteName) : null;
        if (leftSatellite == null || rightSatellite == null)
        {
            return leftSatellite != null ? -1 : rightSatellite != null ? 1 : 0;
        }

        int serviceCompare = EstimateServiceSortBucket(left).CompareTo(EstimateServiceSortBucket(right));
        if (serviceCompare != 0)
        {
            return serviceCompare;
        }
        if (filter == SatelliteListFilter.LowLatency)
        {
            int communicationCompare = CompareCommunicationWindow(left, right);
            if (communicationCompare != 0)
            {
                return communicationCompare;
            }
            int deliveryCompare = left.estimatedDeliverySeconds.CompareTo(right.estimatedDeliverySeconds);
            if (deliveryCompare != 0)
            {
                return deliveryCompare;
            }
            return CompareSatelliteSourceOrder(leftSatellite, rightSatellite);
        }
        if (filter == SatelliteListFilter.TransferSpeed)
        {
            int deliveryCompare = left.estimatedDeliverySeconds.CompareTo(right.estimatedDeliverySeconds);
            if (deliveryCompare != 0)
            {
                return deliveryCompare;
            }
            int communicationCompare = CompareCommunicationWindow(left, right);
            if (communicationCompare != 0)
            {
                return communicationCompare;
            }
            return CompareSatelliteSourceOrder(leftSatellite, rightSatellite);
        }
        if (filter == SatelliteListFilter.LowCost)
        {
            int priceCompare = RoundedEstimatePriceCents(left).CompareTo(RoundedEstimatePriceCents(right));
            if (priceCompare != 0)
            {
                return priceCompare;
            }
            int deliveryCompare = left.estimatedDeliverySeconds.CompareTo(right.estimatedDeliverySeconds);
            if (deliveryCompare != 0)
            {
                return deliveryCompare;
            }
            return CompareSatelliteSourceOrder(leftSatellite, rightSatellite);
        }

        int scoreCompare = left.score.CompareTo(right.score);
        if (scoreCompare != 0)
        {
            return scoreCompare;
        }
        int deliveryFallbackCompare = left.estimatedDeliverySeconds.CompareTo(right.estimatedDeliverySeconds);
        if (deliveryFallbackCompare != 0)
        {
            return deliveryFallbackCompare;
        }
        int priceFallbackCompare = RoundedEstimatePriceCents(left).CompareTo(RoundedEstimatePriceCents(right));
        if (priceFallbackCompare != 0)
        {
            return priceFallbackCompare;
        }
        return CompareSatelliteSourceOrder(leftSatellite, rightSatellite);
    }

    private int RoundedEstimatePriceCents(SatelliteRouteEstimateRecord estimate)
    {
        return estimate != null ? Mathf.RoundToInt(Mathf.Max(0f, estimate.estimatedPriceUsd) * 100f) : int.MaxValue;
    }

    private int EstimateServiceSortBucket(SatelliteRouteEstimateRecord estimate)
    {
        if (estimate == null || !estimate.canServe || estimate.communicationWindowState == SatelliteCommunicationWindowState.Unavailable)
        {
            return 1;
        }
        return 0;
    }

    private int CompareCommunicationWindow(SatelliteRouteEstimateRecord left, SatelliteRouteEstimateRecord right)
    {
        int leftBucket = CommunicationWindowSortBucket(left);
        int rightBucket = CommunicationWindowSortBucket(right);
        int stateCompare = leftBucket.CompareTo(rightBucket);
        if (stateCompare != 0)
        {
            return stateCompare;
        }

        if (leftBucket == 0 && rightBucket == 0)
        {
            return CommunicationWaitSeconds(right).CompareTo(CommunicationWaitSeconds(left));
        }

        int waitCompare = CommunicationWaitSeconds(left).CompareTo(CommunicationWaitSeconds(right));
        if (waitCompare != 0)
        {
            return waitCompare;
        }

        return 0;
    }

    private int CommunicationWindowSortBucket(SatelliteRouteEstimateRecord estimate)
    {
        if (estimate != null && estimate.communicationSortSeconds > 0)
        {
            return estimate.communicationSortBucket;
        }
        if (estimate == null || estimate.communicationWindowState == SatelliteCommunicationWindowState.Unavailable || !estimate.canServe)
        {
            return 2;
        }
        return estimate.communicationWindowState == SatelliteCommunicationWindowState.Remaining ? 0 : 1;
    }

    private int CommunicationWaitSeconds(SatelliteRouteEstimateRecord estimate)
    {
        if (estimate == null)
        {
            return int.MaxValue;
        }
        if (estimate.communicationSortSeconds > 0)
        {
            return Mathf.Max(0, estimate.communicationSortSeconds);
        }
        return Mathf.Max(0, estimate.communicationWindowSeconds > 0 ? estimate.communicationWindowSeconds : estimate.estimatedWindowSeconds);
    }

    private int CompareSatelliteSourceOrder(SatelliteData leftSatellite, SatelliteData rightSatellite)
    {
        int leftIndex = Satellites.IndexOf(leftSatellite);
        int rightIndex = Satellites.IndexOf(rightSatellite);
        leftIndex = leftIndex >= 0 ? leftIndex : int.MaxValue;
        rightIndex = rightIndex >= 0 ? rightIndex : int.MaxValue;
        if (leftIndex != rightIndex)
        {
            return leftIndex.CompareTo(rightIndex);
        }
        return string.Compare(leftSatellite != null ? leftSatellite.satelliteName : string.Empty, rightSatellite != null ? rightSatellite.satelliteName : string.Empty, StringComparison.Ordinal);
    }

    private void ApplyCommunicationSortKeys(SatelliteRouteEstimateRecord estimate)
    {
        if (estimate == null)
        {
            return;
        }

        estimate.communicationSortBucket = CommunicationWindowSortBucket(estimate);
        estimate.communicationSortSeconds = CommunicationWaitSeconds(estimate);
    }

    private int EstimateUplinkDurationSeconds(int payloadKb, SatelliteData satellite)
    {
        int payloadSeconds = SatelliteRouteEstimator.EstimatePayloadSeconds(payloadKb);
        int congestionSeconds = satellite != null ? Mathf.RoundToInt(Mathf.Max(0f, satellite.congestionPercent) * 0.012f) : 0;
        return Mathf.Max(1, 1 + Mathf.CeilToInt(payloadSeconds * 0.35f) + congestionSeconds);
    }

    private int EstimateIslSeconds(SatelliteData satellite, float routeAngleDegrees, int windowGapSeconds)
    {
        float congestionSeconds = satellite != null ? Mathf.Max(0f, satellite.congestionPercent) * 0.018f : 0f;
        float latencySeconds = satellite != null ? Mathf.Max(1, satellite.latencySeconds) * 0.35f : 2.5f;
        float geometrySeconds = Mathf.Clamp(Mathf.Max(0f, routeAngleDegrees) * 0.035f, 0f, 8f);
        float windowGapPenalty = Mathf.Clamp(windowGapSeconds * 0.015f, 0f, 12f);
        return Mathf.Max(2, Mathf.CeilToInt(2f + latencySeconds + congestionSeconds + geometrySeconds + windowGapPenalty));
    }

    private int EstimateRelaySelectionSeconds(SatelliteData satellite)
    {
        float congestionSeconds = satellite != null ? Mathf.Max(0f, satellite.congestionPercent) * 0.006f : 0f;
        return Mathf.Max(1, Mathf.CeilToInt(1f + congestionSeconds));
    }

    private int EstimateDownlinkDurationSeconds(SatelliteData satellite, int payloadKb)
    {
        float latencySeconds = satellite != null ? Mathf.Max(1, satellite.latencySeconds) * 0.45f : 3f;
        float congestionSeconds = satellite != null ? Mathf.Max(0f, satellite.congestionPercent) * 0.02f : 0f;
        return Mathf.Max(2, Mathf.CeilToInt(latencySeconds + congestionSeconds + SatelliteRouteEstimator.EstimatePayloadSeconds(payloadKb)));
    }

    private int EstimateFinalReceiveSeconds()
    {
        return 1;
    }

    private SatelliteRouteEstimateRecord BuildRouteEstimate(SatelliteData satellite, string targetName, Transform targetTransform, int requestedDelayMinutes, int payloadKb)
    {
        if (satellite == null)
        {
            return null;
        }

        bool supportsTarget = !string.IsNullOrEmpty(targetName) && satellite.SupportsTarget(targetName);
        bool hasSharedAccess = satellite.ownerType == SatelliteOwnerType.Shared;
        bool hasEnoughBattery = satellite.batteryPercent >= 35f;
        bool isAvailableState = satellite.availabilityState != SatelliteAvailabilityState.MissionComplete;
        Transform satelliteTransform = GetSatelliteTransform(satellite.satelliteName);
        bool hasDynamicTransform = satelliteTransform != null && targetTransform != null;
        float routeAngle = SatelliteRouteEstimator.EstimateSurfaceAngleDegrees(satellite, satelliteTransform, targetName, targetTransform);
        float degreesPerSecond = DegreesPerSecond(satellite);
        float routeAngleTrend = RouteAngleTrendDegreesPerSecond(satellite, targetName, routeAngle, hasDynamicTransform);
        int windowSeconds = SatelliteRouteEstimator.EstimateWindowSeconds(satellite, routeAngle, hasDynamicTransform, degreesPerSecond, routeAngleTrend, targetName);
        int availableSeconds = SatelliteRouteEstimator.EstimateAvailableForSeconds(satellite, routeAngle, hasDynamicTransform, degreesPerSecond, routeAngleTrend, targetName);
        int requestedDelaySeconds = Mathf.Max(0, requestedDelayMinutes) * 60;
        int routeReadySeconds = Mathf.Max(0, windowSeconds);
        int scheduledStartSeconds = Mathf.Max(requestedDelaySeconds, routeReadySeconds);
        int uplinkDurationSeconds = EstimateUplinkDurationSeconds(payloadKb, satellite);
        int islSeconds = EstimateIslSeconds(satellite, routeAngle, 0);
        int relaySelectionSeconds = EstimateRelaySelectionSeconds(satellite);
        int downlinkDurationSeconds = EstimateDownlinkDurationSeconds(satellite, payloadKb);
        int finalReceiveSeconds = EstimateFinalReceiveSeconds();
        int deliverySeconds = scheduledStartSeconds
            + uplinkDurationSeconds
            + islSeconds
            + relaySelectionSeconds
            + downlinkDurationSeconds
            + finalReceiveSeconds;
        bool routeEligible = supportsTarget
            && hasSharedAccess
            && hasEnoughBattery
            && isAvailableState;
        bool hasReachableWindow = windowSeconds <= SatelliteRouteEstimator.ContactAllowanceSeconds(requestedDelayMinutes);
        bool canServe = routeEligible && hasReachableWindow;
        SatelliteCommunicationWindowState windowState = SatelliteRouteEstimator.CommunicationWindowState(canServe, windowSeconds, availableSeconds);
        int communicationSeconds = windowState == SatelliteCommunicationWindowState.Remaining
            ? Mathf.Max(0, availableSeconds)
            : windowState == SatelliteCommunicationWindowState.AvailableLater ? Mathf.Max(1, windowSeconds) : 0;

        SatelliteRouteEstimateRecord estimate = new SatelliteRouteEstimateRecord();
        estimate.satelliteName = satellite.satelliteName;
        estimate.targetName = targetName;
        estimate.supportsTarget = supportsTarget;
        estimate.hasSharedAccess = hasSharedAccess;
        estimate.hasEnoughBattery = hasEnoughBattery;
        estimate.isAvailableState = isAvailableState;
        estimate.canServe = canServe;
        estimate.routeEligible = routeEligible;
        estimate.canStartNow = canServe && windowSeconds <= 0;
        estimate.usesLiveGeometry = hasDynamicTransform;
        estimate.communicationWindowState = windowState;
        estimate.communicationWindowSeconds = communicationSeconds;
        estimate.originWindowSeconds = windowSeconds;
        estimate.originAvailableForSeconds = availableSeconds;
        estimate.targetWindowSeconds = windowSeconds;
        estimate.targetAvailableForSeconds = availableSeconds;
        estimate.routeReadySeconds = routeReadySeconds;
        estimate.scheduledStartSeconds = scheduledStartSeconds;
        estimate.uplinkWindowOpenSeconds = routeReadySeconds;
        estimate.uplinkAvailableForSeconds = availableSeconds;
        estimate.uplinkDurationSeconds = uplinkDurationSeconds;
        estimate.interSatelliteSeconds = islSeconds;
        estimate.relaySatelliteName = satellite.satelliteName;
        estimate.downlinkWindowOpenSeconds = routeReadySeconds;
        estimate.downlinkAvailableForSeconds = availableSeconds;
        estimate.downlinkWaitSeconds = 0;
        estimate.downlinkDurationSeconds = downlinkDurationSeconds;
        estimate.finalReceiveSeconds = finalReceiveSeconds;
        estimate.totalDeliverySeconds = deliverySeconds;
        estimate.estimatedWindowSeconds = routeReadySeconds;
        estimate.estimatedWindowMinutes = Mathf.CeilToInt(routeReadySeconds / 60f);
        estimate.estimatedAvailableForSeconds = availableSeconds;
        estimate.estimatedDeliverySeconds = deliverySeconds;
        estimate.routeAngleDegrees = routeAngle;
        estimate.estimatedPriceUsd = SatelliteRouteEstimator.EstimateRoutePriceUsd(satellite, payloadKb, deliverySeconds, routeAngle);
        estimate.availabilitySummary = SatelliteRouteEstimator.BuildAvailabilitySummary(satellite, estimate, requestedDelayMinutes);
        estimate.score = SatelliteRouteEstimator.CalculateRouteScore(satellite, estimate, payloadKb);
        ApplyCommunicationSortKeys(estimate);
        estimate.reasonSummary = estimate.availabilitySummary
            + " · 예상 소요 시간 " + SatelliteRouteEstimator.FormatSeconds(deliverySeconds)
            + " · 예상 $" + estimate.estimatedPriceUsd.ToString("0.00");
        return estimate;
    }

    private SatelliteRouteEstimateRecord BuildRouteEstimate(
        SatelliteData satellite,
        string originName,
        Transform originTransform,
        string targetName,
        Transform targetTransform,
        int requestedDelayMinutes,
        int payloadKb)
    {
        SatelliteRouteEstimateRecord originEstimate = BuildRouteEstimate(satellite, SafeRouteEndpointName(originName, OrbitShareDestinations.Seoul), originTransform, requestedDelayMinutes, payloadKb);
        SatelliteRouteEstimateRecord targetEstimate = BuildRouteEstimate(satellite, targetName, targetTransform, requestedDelayMinutes, payloadKb);
        if (originEstimate == null || targetEstimate == null)
        {
            return targetEstimate;
        }

        int uplinkReadySeconds = Mathf.Max(0, originEstimate.estimatedWindowSeconds);
        int targetReadySeconds = Mathf.Max(0, targetEstimate.estimatedWindowSeconds);
        int windowGapSeconds = Mathf.Abs(uplinkReadySeconds - targetReadySeconds);
        bool sameEndpoint = SafeRouteEndpointName(originName, OrbitShareDestinations.Seoul) == targetName;
        float routeSeparationAngle = EstimateEndpointSeparationDegrees(originTransform, targetTransform, originName, targetName);
        float combinedAngle = Mathf.Max(originEstimate.routeAngleDegrees, targetEstimate.routeAngleDegrees) + routeSeparationAngle * 0.2f;
        int requestedDelaySeconds = Mathf.Max(0, requestedDelayMinutes) * 60;
        int scheduledStartSeconds = Mathf.Max(requestedDelaySeconds, uplinkReadySeconds);
        int uplinkDurationSeconds = EstimateUplinkDurationSeconds(payloadKb, satellite);
        int islSeconds = sameEndpoint ? 0 : EstimateIslSeconds(satellite, combinedAngle, windowGapSeconds);
        int relaySelectionSeconds = sameEndpoint ? 0 : EstimateRelaySelectionSeconds(satellite);
        int downlinkDurationSeconds = EstimateDownlinkDurationSeconds(satellite, payloadKb);
        int finalReceiveSeconds = EstimateFinalReceiveSeconds();
        int downlinkEarliestSeconds = scheduledStartSeconds + uplinkDurationSeconds + islSeconds + relaySelectionSeconds;
        int downlinkWaitSeconds = Mathf.Max(0, targetReadySeconds - downlinkEarliestSeconds);
        int deliverySeconds = downlinkEarliestSeconds
            + downlinkWaitSeconds
            + downlinkDurationSeconds
            + finalReceiveSeconds;

        SatelliteRouteEstimateRecord estimate = new SatelliteRouteEstimateRecord();
        estimate.satelliteName = satellite.satelliteName;
        estimate.targetName = targetName;
        estimate.supportsTarget = originEstimate.supportsTarget && targetEstimate.supportsTarget;
        estimate.hasSharedAccess = originEstimate.hasSharedAccess && targetEstimate.hasSharedAccess;
        estimate.hasEnoughBattery = originEstimate.hasEnoughBattery && targetEstimate.hasEnoughBattery;
        estimate.isAvailableState = originEstimate.isAvailableState && targetEstimate.isAvailableState;
        estimate.canServe = originEstimate.canServe && targetEstimate.canServe;
        estimate.routeEligible = estimate.supportsTarget
            && estimate.hasSharedAccess
            && estimate.hasEnoughBattery
            && estimate.isAvailableState;
        estimate.canStartNow = estimate.canServe && originEstimate.estimatedWindowSeconds <= 0;
        estimate.usesLiveGeometry = originEstimate.usesLiveGeometry || targetEstimate.usesLiveGeometry;
        estimate.communicationWindowState = SatelliteRouteEstimator.CommunicationWindowState(
            estimate.canServe,
            uplinkReadySeconds,
            originEstimate.estimatedWindowSeconds <= 0 ? Mathf.Max(0, originEstimate.estimatedAvailableForSeconds) : 0);
        estimate.communicationWindowSeconds = estimate.communicationWindowState == SatelliteCommunicationWindowState.Remaining
            ? Mathf.Max(0, originEstimate.estimatedAvailableForSeconds)
            : estimate.communicationWindowState == SatelliteCommunicationWindowState.AvailableLater ? Mathf.Max(1, uplinkReadySeconds) : 0;
        estimate.originWindowSeconds = originEstimate.estimatedWindowSeconds;
        estimate.originAvailableForSeconds = originEstimate.estimatedAvailableForSeconds;
        estimate.targetWindowSeconds = targetEstimate.estimatedWindowSeconds;
        estimate.targetAvailableForSeconds = targetEstimate.estimatedAvailableForSeconds;
        estimate.routeReadySeconds = uplinkReadySeconds;
        estimate.scheduledStartSeconds = scheduledStartSeconds;
        estimate.uplinkWindowOpenSeconds = uplinkReadySeconds;
        estimate.uplinkAvailableForSeconds = originEstimate.estimatedAvailableForSeconds;
        estimate.uplinkDurationSeconds = uplinkDurationSeconds;
        estimate.interSatelliteSeconds = islSeconds;
        estimate.relaySatelliteName = satellite.satelliteName;
        estimate.downlinkWindowOpenSeconds = targetReadySeconds;
        estimate.downlinkAvailableForSeconds = targetEstimate.estimatedAvailableForSeconds;
        estimate.downlinkWaitSeconds = downlinkWaitSeconds;
        estimate.downlinkDurationSeconds = downlinkDurationSeconds;
        estimate.finalReceiveSeconds = finalReceiveSeconds;
        estimate.totalDeliverySeconds = deliverySeconds;
        estimate.estimatedWindowSeconds = uplinkReadySeconds;
        estimate.estimatedWindowMinutes = Mathf.CeilToInt(uplinkReadySeconds / 60f);
        estimate.estimatedAvailableForSeconds = originEstimate.estimatedWindowSeconds <= 0
            ? Mathf.Max(0, originEstimate.estimatedAvailableForSeconds)
            : 0;
        estimate.estimatedDeliverySeconds = deliverySeconds;
        estimate.routeAngleDegrees = combinedAngle;
        estimate.estimatedPriceUsd = SatelliteRouteEstimator.EstimateRoutePriceUsd(satellite, payloadKb, deliverySeconds, combinedAngle);
        estimate.availabilitySummary = BuildRouteAvailabilitySummary(originName, targetName, originEstimate, targetEstimate, requestedDelayMinutes);
        estimate.score = targetEstimate.score
            + originEstimate.score * 0.45f
            + deliverySeconds * 0.45f
            + windowGapSeconds * 0.12f
            + routeSeparationAngle * 1.2f
            + (estimate.canServe ? 0f : 5000f)
            + estimate.estimatedPriceUsd * 8f;
        ApplyCommunicationSortKeys(estimate);
        estimate.reasonSummary = estimate.availabilitySummary
            + " · 예상 소요 시간 " + SatelliteRouteEstimator.FormatSeconds(deliverySeconds)
            + " · 예상 $" + estimate.estimatedPriceUsd.ToString("0.00");
        return estimate;
    }

    private string BuildRouteAvailabilitySummary(string originName, string targetName, SatelliteRouteEstimateRecord originEstimate, SatelliteRouteEstimateRecord targetEstimate, int requestedDelayMinutes)
    {
        if (originEstimate == null || targetEstimate == null)
        {
            return "경로 계산 중";
        }
        if (!originEstimate.canServe || !targetEstimate.canServe)
        {
            return "통신 불가";
        }

        int waitSeconds = Mathf.Max(0, originEstimate.estimatedWindowSeconds);
        if (waitSeconds <= 0)
        {
            int availableSeconds = Mathf.Max(0, originEstimate.estimatedAvailableForSeconds);
            return SatelliteRouteEstimator.FormatSeconds(availableSeconds) + " 남음";
        }
        return SatelliteRouteEstimator.FormatSeconds(waitSeconds) + " 후 가능";
    }

    private float EstimateEndpointSeparationDegrees(Transform originTransform, Transform targetTransform, string originName, string targetName)
    {
        if (originTransform != null && targetTransform != null)
        {
            Vector3 originDirection = originTransform.position.normalized;
            Vector3 targetDirection = targetTransform.position.normalized;
            if (originDirection.sqrMagnitude > 0.001f && targetDirection.sqrMagnitude > 0.001f)
            {
                return Vector3.Angle(originDirection, targetDirection);
            }
        }

        return Mathf.Abs(SatelliteRouteEstimator.StableHash(SafeRouteEndpointName(originName, string.Empty) + ":" + SafeRouteEndpointName(targetName, string.Empty)) % 120);
    }

    private string SafeRouteEndpointName(string routeEndpointName, string fallbackName)
    {
        return string.IsNullOrEmpty(routeEndpointName) ? fallbackName : routeEndpointName;
    }

    private float RouteAngleTrendDegreesPerSecond(SatelliteData satellite, string targetName, float routeAngle, bool hasDynamicTransform)
    {
        if (!hasDynamicTransform || satellite == null)
        {
            return 0f;
        }

        string key = (satellite.satelliteName ?? string.Empty) + "|" + (targetName ?? string.Empty);
        float now = Application.isPlaying ? Time.unscaledTime : 0f;
        RouteAngleSample sample;
        if (!routeAngleSamples.TryGetValue(key, out sample))
        {
            routeAngleSamples[key] = new RouteAngleSample
            {
                angleDegrees = routeAngle,
                timeSeconds = now,
                trendDegreesPerSecond = 0f
            };
            return 0f;
        }

        float deltaTime = now - sample.timeSeconds;
        if (deltaTime >= 0.08f)
        {
            float baseDegreesPerSecond = Mathf.Max(0.01f, DegreesPerSecond(satellite));
            float measuredTrend = (routeAngle - sample.angleDegrees) / deltaTime;
            measuredTrend = Mathf.Clamp(measuredTrend, -baseDegreesPerSecond * 1.8f, baseDegreesPerSecond * 1.8f);
            float blend = Mathf.Clamp(deltaTime / 4f, 0.08f, 0.35f);
            if (Mathf.Sign(measuredTrend) != Mathf.Sign(sample.trendDegreesPerSecond)
                && Mathf.Abs(measuredTrend) > baseDegreesPerSecond * 0.12f
                && Mathf.Abs(sample.trendDegreesPerSecond) > baseDegreesPerSecond * 0.12f)
            {
                blend *= 0.45f;
            }
            sample.trendDegreesPerSecond = Mathf.Lerp(sample.trendDegreesPerSecond, measuredTrend, blend);
            if (Mathf.Abs(sample.trendDegreesPerSecond) < baseDegreesPerSecond * 0.08f)
            {
                sample.trendDegreesPerSecond = 0f;
            }
            sample.angleDegrees = routeAngle;
            sample.timeSeconds = now;
            routeAngleSamples[key] = sample;
        }
        return sample.trendDegreesPerSecond;
    }

    public float DegreesPerSecond(SatelliteData satellite)
    {
        if (satellite == null)
        {
            return 6f;
        }
        Transform satelliteTransform = GetSatelliteTransform(satellite.satelliteName);
        if (satelliteTransform != null)
        {
            SatelliteOrbit orbit = satelliteTransform.GetComponent<SatelliteOrbit>();
            if (orbit != null)
            {
                return Mathf.Max(1f, orbit.orbitSpeed);
            }

            EnterpriseSatelliteAnimator animator = satelliteTransform.GetComponent<EnterpriseSatelliteAnimator>();
            if (animator != null)
            {
                return Mathf.Max(1f, animator.speed);
            }
        }
        float orbitalRadiusKm = 6371f + Mathf.Max(0f, satellite.altitudeKm);
        float angularRateDegreesPerSecond = orbitalRadiusKm > 1f
            ? satellite.speedKmPerSec / orbitalRadiusKm * Mathf.Rad2Deg
            : 0.06f;
        return Mathf.Clamp(angularRateDegreesPerSecond, 0.005f, 0.2f);
    }

    private Transform ResolveTargetTransform(string targetName)
    {
        Transform targetTransform;
        if (!string.IsNullOrEmpty(targetName) && targetTransforms.TryGetValue(targetName, out targetTransform) && targetTransform != null)
        {
            return targetTransform;
        }
        return null;
    }

    private SatelliteData FindSatelliteByName(string satelliteName)
    {
        return Satellites.Find(satellite => satellite.satelliteName == satelliteName);
    }

    private string NormalizeRealSatelliteName(string satelliteName)
    {
        if (string.IsNullOrEmpty(satelliteName))
        {
            return string.Empty;
        }

        string trimmedName = satelliteName.Trim();
        if (trimmedName.Length == 0)
        {
            return string.Empty;
        }
        return trimmedName.StartsWith(RealSatelliteNamePrefix, StringComparison.Ordinal)
            ? trimmedName
            : RealSatelliteNamePrefix + trimmedName;
    }

    public void SetSharedState(SatelliteAvailabilityState state)
    {
        SetSharedState(state, true);
    }

    public void SetSharedState(SatelliteAvailabilityState state, bool notifyChanged)
    {
        SatelliteData missionSatellite = GetMissionSatellite();
        if (missionSatellite == null)
        {
            return;
        }

        missionSatellite.availabilityState = state;
        if (notifyChanged)
        {
            NotifyChanged();
        }
    }

    public void UpdateSharedMission(string missionTarget, float storagePercent)
    {
        SatelliteData missionSatellite = GetMissionSatellite();
        if (missionSatellite == null)
        {
            return;
        }

        missionSatellite.currentMissionTarget = string.IsNullOrEmpty(missionTarget) ? "None" : missionTarget;
        missionSatellite.dataStoragePercent = Mathf.Clamp(storagePercent, 0f, 100f);
        NotifyChanged();
    }

    public void ResetSharedSatellite()
    {
        SatelliteData missionSatellite = GetMissionSatellite();
        if (missionSatellite != null)
        {
            missionSatellite.availabilityState = SatelliteAvailabilityState.Available;
            missionSatellite.currentMissionTarget = "None";
            missionSatellite.dataStoragePercent = 0f;
        }
        MissionSatelliteName = string.Empty;
        NotifyChanged();
    }

    public void NotifyChanged()
    {
        if (OnDataChanged != null)
        {
            OnDataChanged.Invoke();
        }
    }
}
