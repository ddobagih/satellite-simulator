using System;

public enum SatelliteOwnerType
{
    Personal,
    Company,
    Shared
}

public enum SatelliteAvailabilityState
{
    Available,
    Reserved,
    InMission,
    MissionComplete
}

public enum SatelliteCommunicationWindowState
{
    Remaining,
    AvailableLater,
    Unavailable
}

[Serializable]
public class SatelliteRecommendationRecord
{
    public string categoryLabel;
    public string satelliteName;
    public string summary;
    public string shortSummary;
    public string reasonSummary;
    public int estimatedDeliverySeconds;
    public int estimatedWindowSeconds;
    public int estimatedWindowMinutes;
    public int estimatedAvailableForSeconds;
    public float estimatedPriceUsd;
    public int reliabilityPercent;
    public int congestionPercent;
    public float routeAngleDegrees;
    public float score;
}

[Serializable]
public class SatelliteRouteEstimateRecord
{
    public string satelliteName;
    public string targetName;
    public bool supportsTarget;
    public bool hasSharedAccess;
    public bool hasEnoughBattery;
    public bool isAvailableState;
    public bool canServe;
    public bool canStartNow;
    public bool routeEligible;
    public bool usesLiveGeometry;
    public SatelliteCommunicationWindowState communicationWindowState;
    public int communicationWindowSeconds;
    public int communicationSortBucket;
    public int communicationSortSeconds;
    public int originWindowSeconds;
    public int originAvailableForSeconds;
    public int targetWindowSeconds;
    public int targetAvailableForSeconds;
    public int routeReadySeconds;
    public int scheduledStartSeconds;
    public int uplinkWindowOpenSeconds;
    public int uplinkAvailableForSeconds;
    public int uplinkDurationSeconds;
    public int interSatelliteSeconds;
    public string relaySatelliteName;
    public int downlinkWindowOpenSeconds;
    public int downlinkAvailableForSeconds;
    public int downlinkWaitSeconds;
    public int downlinkDurationSeconds;
    public int finalReceiveSeconds;
    public int totalDeliverySeconds;
    public int estimatedWindowSeconds;
    public int estimatedWindowMinutes;
    public int estimatedAvailableForSeconds;
    public int estimatedDeliverySeconds;
    public float estimatedPriceUsd;
    public float routeAngleDegrees;
    public float score;
    public string availabilitySummary;
    public string reasonSummary;
}

[Serializable]
public class DestinationArrivalComparisonRecord
{
    public string targetName;
    public string destinationLabel;
    public int availableSatelliteCount;
    public string recommendedSatelliteName;
    public int estimatedArrivalSeconds;
    public string estimatedArrivalLabel;
    public string summary;
}

[Serializable]
public class ModelStateSummaryRecord
{
    public int satelliteCount;
    public int destinationCount;
    public int queuedMessageCount;
    public string summary;
}

[Serializable]
public class SatelliteData
{
    public string satelliteName;
    public SatelliteOwnerType ownerType;
    public string ownerName;
    public SatelliteAvailabilityState availabilityState;
    public string usageType;
    public float altitudeKm;
    public float speedKmPerSec;
    public float batteryPercent;
    public float dataStoragePercent;
    public string currentMissionTarget;
    public string orbitSlot;
    public int nextWindowMinutes;
    public int feePer32KbCredits;
    public int latencySeconds;
    public int reliabilityPercent;
    public int congestionPercent;
    public bool supportsSeoul;
    public bool supportsNewYork;
    public bool supportsLondon;
    public bool supportsTokyo;
    public bool supportsParis;
    public bool supportsSydney;
    public bool supportsLosAngeles;

    public SatelliteData(
        string satelliteName,
        SatelliteOwnerType ownerType,
        string ownerName,
        SatelliteAvailabilityState availabilityState,
        string usageType,
        float altitudeKm,
        float speedKmPerSec,
        float batteryPercent)
        : this(satelliteName, ownerType, ownerName, availabilityState, usageType, altitudeKm, speedKmPerSec, batteryPercent, "LEO-A", 0, 2, 4, 92, 15, true, true, true, true, true, true, true)
    {
    }

    public SatelliteData(
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
        int congestionPercent,
        bool supportsSeoul,
        bool supportsNewYork,
        bool supportsLondon,
        bool supportsTokyo,
        bool supportsParis,
        bool supportsSydney,
        bool supportsLosAngeles)
    {
        this.satelliteName = satelliteName;
        this.ownerType = ownerType;
        this.ownerName = ownerName;
        this.availabilityState = availabilityState;
        this.usageType = usageType;
        this.altitudeKm = altitudeKm;
        this.speedKmPerSec = speedKmPerSec;
        this.batteryPercent = batteryPercent;
        this.orbitSlot = orbitSlot;
        this.nextWindowMinutes = nextWindowMinutes;
        this.feePer32KbCredits = feePer32KbCredits;
        this.latencySeconds = latencySeconds;
        this.reliabilityPercent = reliabilityPercent;
        this.congestionPercent = Math.Max(0, Math.Min(100, congestionPercent));
        this.supportsSeoul = supportsSeoul;
        this.supportsNewYork = supportsNewYork;
        this.supportsLondon = supportsLondon;
        this.supportsTokyo = supportsTokyo;
        this.supportsParis = supportsParis;
        this.supportsSydney = supportsSydney;
        this.supportsLosAngeles = supportsLosAngeles;
        dataStoragePercent = 0f;
        currentMissionTarget = "None";
    }

    public string OwnerTypeLabel
    {
        get
        {
            switch (ownerType)
            {
                case SatelliteOwnerType.Personal:
                    return "개인 위성";
                case SatelliteOwnerType.Company:
                    return "기업 위성";
                case SatelliteOwnerType.Shared:
                    return "공유 위성";
                default:
                    return ownerType.ToString();
            }
        }
    }

    public string StateLabel
    {
        get
        {
            switch (availabilityState)
            {
                case SatelliteAvailabilityState.Available:
                    return "사용 가능";
                case SatelliteAvailabilityState.Reserved:
                    return "예약됨";
                case SatelliteAvailabilityState.InMission:
                    return "전송 중";
                case SatelliteAvailabilityState.MissionComplete:
                    return "완료";
                default:
                    return availabilityState.ToString();
            }
        }
    }

    public bool SupportsTarget(string targetName)
    {
        if (targetName == OrbitShareDestinations.Seoul)
        {
            return supportsSeoul;
        }
        if (targetName == OrbitShareDestinations.NewYork)
        {
            return supportsNewYork;
        }
        if (targetName == OrbitShareDestinations.London)
        {
            return supportsLondon;
        }
        if (targetName == OrbitShareDestinations.Tokyo)
        {
            return supportsTokyo;
        }
        if (targetName == OrbitShareDestinations.Paris)
        {
            return supportsParis;
        }
        if (targetName == OrbitShareDestinations.Sydney)
        {
            return supportsSydney;
        }
        if (targetName == OrbitShareDestinations.LosAngeles)
        {
            return supportsLosAngeles;
        }
        return false;
    }

    public bool HasBasicReservationEligibility(string targetName)
    {
        return availabilityState == SatelliteAvailabilityState.Available
            && ownerType == SatelliteOwnerType.Shared
            && batteryPercent >= 35f
            && SupportsTarget(targetName);
    }

    public string SupportedTargetSummary
    {
        get
        {
            string summary = string.Empty;
            AppendSupportedTarget(ref summary, supportsSeoul, OrbitShareDestinations.Label(OrbitShareDestinations.Seoul));
            AppendSupportedTarget(ref summary, supportsNewYork, OrbitShareDestinations.Label(OrbitShareDestinations.NewYork));
            AppendSupportedTarget(ref summary, supportsLondon, OrbitShareDestinations.Label(OrbitShareDestinations.London));
            AppendSupportedTarget(ref summary, supportsTokyo, OrbitShareDestinations.Label(OrbitShareDestinations.Tokyo));
            AppendSupportedTarget(ref summary, supportsParis, OrbitShareDestinations.Label(OrbitShareDestinations.Paris));
            AppendSupportedTarget(ref summary, supportsSydney, OrbitShareDestinations.Label(OrbitShareDestinations.Sydney));
            AppendSupportedTarget(ref summary, supportsLosAngeles, OrbitShareDestinations.Label(OrbitShareDestinations.LosAngeles));
            return summary.Length == 0 ? "지원 목적지 없음" : summary;
        }
    }

    private void AppendSupportedTarget(ref string summary, bool supported, string label)
    {
        if (!supported)
        {
            return;
        }
        summary += (summary.Length > 0 ? ", " : string.Empty) + label;
    }

    public int RecentSuccessPercent
    {
        get { return Math.Max(0, Math.Min(100, reliabilityPercent - CongestionReliabilityPenalty)); }
    }

    public bool CanServe(string targetName, int requestedDelayMinutes)
    {
        return HasBasicReservationEligibility(targetName);
    }

    public int EstimatePriceCredits(int messageSizeKb)
    {
        int units = Math.Max(1, (messageSizeKb + 31) / 32);
        return units * Math.Max(1, feePer32KbCredits);
    }

    public float EstimatePriceUsd(int messageSizeKb)
    {
        return EstimatePriceCredits(messageSizeKb) * 0.25f * CongestionPriceMultiplier;
    }

    public float CongestionPriceMultiplier
    {
        get { return 1f + congestionPercent * 0.0035f; }
    }

    public int CongestionReliabilityPenalty
    {
        get { return Math.Max(0, congestionPercent / 8); }
    }


    public string GetShortRecommendationReason(int payloadKb)
    {
        return GetShortRecommendationReason(payloadKb, 0);
    }

    public string GetShortRecommendationReason(int payloadKb, int requestedDelayMinutes)
    {
        return satelliteName
            + " · " + GetVisibilityWindowLabel(requestedDelayMinutes)
            + " · 도착 " + FormatSeconds(nextWindowMinutes * 60 + latencySeconds)
            + " · 32,768 Bytes당 $" + EstimatePriceUsd(32).ToString("0.00")
            + " · 예상 $" + EstimatePriceUsd(payloadKb).ToString("0.00");
    }

    public string GetRecommendationReasonSummary(int payloadKb)
    {
        return GetRecommendationReasonSummary(payloadKb, 0);
    }

    public string GetRecommendationReasonSummary(int payloadKb, int requestedDelayMinutes)
    {
        return GetShortRecommendationReason(payloadKb, requestedDelayMinutes)
            + " · 혼잡 " + CongestionLabel + " " + congestionPercent + "%"
            + " · 성공 " + RecentSuccessPercent + "%";
    }

    public string GetVisibilityWindowLabel(int requestedDelayMinutes)
    {
        if (nextWindowMinutes <= 0)
        {
            return "통신 가능 시간 남음";
        }
        if (requestedDelayMinutes > 0 && nextWindowMinutes <= requestedDelayMinutes)
        {
            return "예약 시간 가능";
        }
        return nextWindowMinutes + "분 후 가능";
    }

    private string FormatSeconds(int totalSeconds)
    {
        int safeSeconds = Math.Max(0, totalSeconds);
        if (safeSeconds >= 60)
        {
            return (safeSeconds / 60) + "분 " + (safeSeconds % 60) + "초";
        }
        return safeSeconds + "초";
    }

    public string CongestionLabel
    {
        get
        {
            if (congestionPercent >= 70)
            {
                return "혼잡";
            }
            if (congestionPercent >= 40)
            {
                return "보통";
            }
            return "여유";
        }
    }
}
