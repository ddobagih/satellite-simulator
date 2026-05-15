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
                    return "Relay Node";
                case SatelliteOwnerType.Company:
                    return "Backbone Node";
                case SatelliteOwnerType.Shared:
                    return "Active Relay";
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
                    return ownerType == SatelliteOwnerType.Shared ? "Relay Ready" : "Available";
                case SatelliteAvailabilityState.Reserved:
                    return "Queued";
                case SatelliteAvailabilityState.InMission:
                    return "Relaying";
                case SatelliteAvailabilityState.MissionComplete:
                    return "Delivered";
                default:
                    return availabilityState.ToString();
            }
        }
    }
}
