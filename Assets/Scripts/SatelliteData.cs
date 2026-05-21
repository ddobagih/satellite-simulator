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
    public bool supportsSeoul;
    public bool supportsNewYork;
    public bool supportsLondon;

    public SatelliteData(
        string satelliteName,
        SatelliteOwnerType ownerType,
        string ownerName,
        SatelliteAvailabilityState availabilityState,
        string usageType,
        float altitudeKm,
        float speedKmPerSec,
        float batteryPercent)
        : this(satelliteName, ownerType, ownerName, availabilityState, usageType, altitudeKm, speedKmPerSec, batteryPercent, "LEO-A", 0, 2, 4, 92, true, true, true)
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
        bool supportsSeoul,
        bool supportsNewYork,
        bool supportsLondon)
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
        this.supportsSeoul = supportsSeoul;
        this.supportsNewYork = supportsNewYork;
        this.supportsLondon = supportsLondon;
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
                    return "전용 위성";
                case SatelliteOwnerType.Company:
                    return "기업 백본";
                case SatelliteOwnerType.Shared:
                    return "공유 중계";
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
                    return "발신 가능";
                case SatelliteAvailabilityState.Reserved:
                    return "발신 예약";
                case SatelliteAvailabilityState.InMission:
                    return "전송 중";
                case SatelliteAvailabilityState.MissionComplete:
                    return "수신 완료";
                default:
                    return availabilityState.ToString();
            }
        }
    }

    public bool SupportsTarget(string targetName)
    {
        if (targetName == "Seoul")
        {
            return supportsSeoul;
        }
        if (targetName == "New York")
        {
            return supportsNewYork;
        }
        if (targetName == "London")
        {
            return supportsLondon;
        }
        return false;
    }

    public bool CanServe(string targetName, int requestedDelayMinutes)
    {
        return availabilityState == SatelliteAvailabilityState.Available
            && ownerType == SatelliteOwnerType.Shared
            && batteryPercent >= 35f
            && SupportsTarget(targetName)
            && nextWindowMinutes <= requestedDelayMinutes;
    }

    public int EstimatePriceCredits(int messageSizeKb)
    {
        int units = Math.Max(1, (messageSizeKb + 31) / 32);
        return units * Math.Max(1, feePer32KbCredits);
    }
}
