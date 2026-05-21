using System;
using System.Collections.Generic;
using UnityEngine;

public enum SatelliteListFilter
{
    All,
    Available,
    Fast,
    LowCost
}

public class SatelliteManager : MonoBehaviour
{
    public event Action OnDataChanged;
    public event Action<SatelliteData> OnSelectionChanged;

    public readonly List<SatelliteData> Satellites = new List<SatelliteData>();

    public SatelliteData SelectedSatellite { get; private set; }
    public string MissionSatelliteName { get; private set; }

    private void Awake()
    {
        Satellites.Clear();
        AddSatellite("SAT-S01", SatelliteOwnerType.Shared, "OrbitShare", SatelliteAvailabilityState.Available, "서울 단문 중계", 550f, 7.6f, 82f, "LEO-A", 0, 2, 4, 96, true, true, true);
        AddSatellite("SAT-P01", SatelliteOwnerType.Personal, "개인망", SatelliteAvailabilityState.Available, "개인 전용", 520f, 7.5f, 91f, "LEO-A", 3, 4, 5, 89, true, false, false);
        AddSatellite("SAT-C01", SatelliteOwnerType.Company, "기업 백본", SatelliteAvailabilityState.InMission, "기업 트래픽", 650f, 7.4f, 74f, "MEO-B", 12, 3, 7, 91, false, true, true);

        for (int i = 4; i <= 24; i++)
        {
            string orbitSlot = i % 3 == 0 ? "LEO-A" : i % 3 == 1 ? "MEO-B" : "Polar-C";
            int nextWindow = (i * 3) % 31;
            int fee = 2 + (i % 4);
            int latency = 4 + (i % 6);
            int reliability = 86 + (i % 10);
            bool supportsSeoul = i % 2 == 0 || i % 5 == 0;
            bool supportsNewYork = i % 3 != 0;
            bool supportsLondon = i % 4 != 0;
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
                supportsLondon);
        }

        SelectSatellite("SAT-S01");
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
        bool supportsLondon)
    {
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
            supportsSeoul,
            supportsNewYork,
            supportsLondon));
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

        SelectedSatellite = selected;
        if (OnSelectionChanged != null)
        {
            OnSelectionChanged.Invoke(SelectedSatellite);
        }
        NotifyChanged();
    }

    public bool CanReserveSelected(string targetName, int requestedDelayMinutes)
    {
        return SelectedSatellite != null && SelectedSatellite.CanServe(targetName, requestedDelayMinutes);
    }

    public bool CanReserveSelected()
    {
        return CanReserveSelected("New York", 0);
    }

    public bool ReserveSelected(string targetName, int requestedDelayMinutes)
    {
        if (!CanReserveSelected(targetName, requestedDelayMinutes))
        {
            return false;
        }

        MissionSatelliteName = SelectedSatellite.satelliteName;
        SelectedSatellite.availabilityState = SatelliteAvailabilityState.Reserved;
        SelectedSatellite.currentMissionTarget = targetName;
        NotifyChanged();
        return true;
    }

    public SatelliteData FindBestSatellite(string targetName, int requestedDelayMinutes)
    {
        List<SatelliteData> candidates = GetFilteredSatellites(SatelliteListFilter.Available, targetName, requestedDelayMinutes);
        return candidates.Count > 0 ? candidates[0] : null;
    }

    public List<SatelliteData> GetFilteredSatellites(SatelliteListFilter filter, string targetName, int requestedDelayMinutes)
    {
        List<SatelliteData> filtered = new List<SatelliteData>();
        for (int i = 0; i < Satellites.Count; i++)
        {
            SatelliteData satellite = Satellites[i];
            bool canServe = satellite.CanServe(targetName, requestedDelayMinutes);
            bool supportsTarget = string.IsNullOrEmpty(targetName) || satellite.SupportsTarget(targetName);

            if (filter == SatelliteListFilter.Available && !canServe)
            {
                continue;
            }
            if (filter == SatelliteListFilter.Fast && (!supportsTarget || satellite.nextWindowMinutes > 10))
            {
                continue;
            }
            if (filter == SatelliteListFilter.LowCost && (!supportsTarget || satellite.feePer32KbCredits > 3))
            {
                continue;
            }
            filtered.Add(satellite);
        }

        filtered.Sort(delegate (SatelliteData left, SatelliteData right)
        {
            bool leftCanServe = left.CanServe(targetName, requestedDelayMinutes);
            bool rightCanServe = right.CanServe(targetName, requestedDelayMinutes);
            if (leftCanServe != rightCanServe)
            {
                return leftCanServe ? -1 : 1;
            }
            int windowCompare = left.nextWindowMinutes.CompareTo(right.nextWindowMinutes);
            if (windowCompare != 0)
            {
                return windowCompare;
            }
            return left.feePer32KbCredits.CompareTo(right.feePer32KbCredits);
        });
        return filtered;
    }

    public void SetSharedState(SatelliteAvailabilityState state)
    {
        SatelliteData missionSatellite = GetMissionSatellite();
        if (missionSatellite == null)
        {
            return;
        }

        missionSatellite.availabilityState = state;
        NotifyChanged();
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
