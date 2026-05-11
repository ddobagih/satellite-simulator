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

    public SatelliteData(
        string satelliteName,
        SatelliteOwnerType ownerType,
        string ownerName,
        SatelliteAvailabilityState availabilityState,
        string usageType,
        float altitudeKm,
        float speedKmPerSec,
        float batteryPercent)
    {
        this.satelliteName = satelliteName;
        this.ownerType = ownerType;
        this.ownerName = ownerName;
        this.availabilityState = availabilityState;
        this.usageType = usageType;
        this.altitudeKm = altitudeKm;
        this.speedKmPerSec = speedKmPerSec;
        this.batteryPercent = batteryPercent;
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
                    return "Personal";
                case SatelliteOwnerType.Company:
                    return "Company";
                case SatelliteOwnerType.Shared:
                    return "Shared";
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
                    return ownerType == SatelliteOwnerType.Shared ? "Rentable" : "Available";
                case SatelliteAvailabilityState.Reserved:
                    return "Reserved";
                case SatelliteAvailabilityState.InMission:
                    return "In Mission";
                case SatelliteAvailabilityState.MissionComplete:
                    return "Complete";
                default:
                    return availabilityState.ToString();
            }
        }
    }
}
