using System;
using System.Collections.Generic;
using UnityEngine;

public class SatelliteManager : MonoBehaviour
{
    public event Action OnDataChanged;
    public event Action<SatelliteData> OnSelectionChanged;

    public readonly List<SatelliteData> Satellites = new List<SatelliteData>();

    public SatelliteData SelectedSatellite { get; private set; }

    private void Awake()
    {
        Satellites.Clear();
        Satellites.Add(new SatelliteData("SAT-P01", SatelliteOwnerType.Personal, "Private User", SatelliteAvailabilityState.Available, "Personal imaging", 520f, 7.5f, 91f));
        Satellites.Add(new SatelliteData("SAT-C01", SatelliteOwnerType.Company, "GeoVision Corp.", SatelliteAvailabilityState.InMission, "Commercial imaging", 650f, 7.4f, 74f));
        Satellites.Add(new SatelliteData("SAT-S01", SatelliteOwnerType.Shared, "OrbitShare", SatelliteAvailabilityState.Available, "Short-term rental", 550f, 7.6f, 82f));
        SelectSatellite("SAT-S01");
    }

    public SatelliteData GetSharedSatellite()
    {
        return Satellites.Find(satellite => satellite.ownerType == SatelliteOwnerType.Shared);
    }

    public void SelectSatellite(string satelliteName)
    {
        SatelliteData selected = Satellites.Find(satellite => satellite.satelliteName == satelliteName);
        if (selected == null)
        {
            return;
        }

        SelectedSatellite = selected;
        if (OnSelectionChanged != null)
        {
            OnSelectionChanged.Invoke(SelectedSatellite);
        }
        NotifyChanged();
    }

    public bool CanReserveSelected()
    {
        return SelectedSatellite != null
            && SelectedSatellite.ownerType == SatelliteOwnerType.Shared
            && SelectedSatellite.availabilityState == SatelliteAvailabilityState.Available;
    }

    public bool ReserveSelected(string targetName)
    {
        if (!CanReserveSelected())
        {
            return false;
        }

        SelectedSatellite.availabilityState = SatelliteAvailabilityState.Reserved;
        SelectedSatellite.currentMissionTarget = targetName;
        NotifyChanged();
        return true;
    }

    public void SetSharedState(SatelliteAvailabilityState state)
    {
        SatelliteData shared = GetSharedSatellite();
        if (shared == null)
        {
            return;
        }

        shared.availabilityState = state;
        NotifyChanged();
    }

    public void UpdateSharedMission(string missionTarget, float storagePercent)
    {
        SatelliteData shared = GetSharedSatellite();
        if (shared == null)
        {
            return;
        }

        shared.currentMissionTarget = string.IsNullOrEmpty(missionTarget) ? "None" : missionTarget;
        shared.dataStoragePercent = Mathf.Clamp(storagePercent, 0f, 100f);
        NotifyChanged();
    }

    public void ResetSharedSatellite()
    {
        SatelliteData shared = GetSharedSatellite();
        if (shared == null)
        {
            return;
        }

        shared.availabilityState = SatelliteAvailabilityState.Available;
        shared.currentMissionTarget = "None";
        shared.dataStoragePercent = 0f;
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
