using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum RealSatelliteDataSet
{
    Iss,
    Stations,
    Active,
    Starlink
}

public enum RealSatelliteLabelMode
{
    Off,
    First,
    Selected,
    All
}

public class RealSatelliteVisualizer : MonoBehaviour
{
    public Transform earth;
    public Transform markerParent;
    public SatelliteManager satelliteManager;
    public SatelliteFocusController focusController;
    public CameraManager cameraManager;
    public MissionVisualController missionVisualController;
    public CelesTrakTleClient tleClient;
    public RealSatelliteDataSet dataSet = RealSatelliteDataSet.Starlink;
    public bool autoFetchOnStart = true;
    public int maxDisplayed = 100;
    public int groupDisplayLimit = 100;
    public float updateIntervalSeconds = 0.5f;
    public float velocityLineLength = 0.42f;
    public float velocityPreviewSeconds = 40f;
    public bool showLabels = true;
    public RealSatelliteLabelMode labelMode = RealSatelliteLabelMode.First;
    public int selectedMarkerIndex;
    public bool enableKeyboardShortcuts = false;
    public bool showRuntimeControls = false;
    public bool showVelocityLines = true;
    public bool showFirstOrbitTrail = false;
    public bool showFirstGroundTrack = false;
    public int groupUpdateBatchSize = 36;
    public int markerPoolSlack = 8;
    public int hardMarkerPoolLimit = 120;
    public int orbitTrailPointCount = 80;
    public float orbitTrailStepSeconds = 45f;
    public float pathUpdateIntervalSeconds = 5f;
    public bool syncDisplayedSatellitesToMissionManager = true;
    public float satelliteManagerSyncIntervalSeconds = 10f;
    public float displayAltitudeScale = 4.5f;
    public float minimumDisplayAltitude = 0.75f;
    public float displayAltitudeScaleFadeKm = 3000f;

    public string LastStatus { get; private set; }
    public string LastUrl { get; private set; }
    public string LastError { get; private set; }
    public DateTime LastFetchUtc { get; private set; }
    public int LoadedCount { get { return loadedTles.Count; } }
    public int DisplayedCount { get; private set; }
    public string FirstSatelliteName { get; private set; }
    public GeodeticCoordinate FirstGeodetic { get; private set; }
    public bool LastFetchFromCache { get; private set; }
    public bool LastFetchFromFallback { get; private set; }
    public string LastDisplayMode { get; private set; }
    public string LastLabelMode { get; private set; }
    public string LastVelocityLineMode { get; private set; }
    public string LastPerformanceMode { get; private set; }

    private readonly List<RealSatelliteTle> loadedTles = new List<RealSatelliteTle>();
    private readonly List<RealSatelliteMarker> markers = new List<RealSatelliteMarker>();
    private Transform realSatelliteRoot;
    private Material markerMaterial;
    private Material velocityMaterial;
    private Material orbitTrailMaterial;
    private Material groundTrackMaterial;
    private LineRenderer firstOrbitTrail;
    private LineRenderer firstGroundTrack;
    private Canvas runtimeCanvas;
    private Text runtimeStatusText;
    private float nextUpdateTime;
    private float nextPathUpdateTime;
    private bool fetchInProgress;
    private int updateCursor;
    private int pathSatelliteIndex = -1;
    private float nextSatelliteManagerSyncTime;
    private readonly Dictionary<string, Transform> focusRegisteredTransforms = new Dictionary<string, Transform>();

    private void Awake()
    {
        EnsureRuntimeObjects();
    }

    private void Start()
    {
        if (autoFetchOnStart)
        {
            StartCoroutine(FetchCurrentDataSet(false));
        }
    }

    private void Update()
    {
        HandleKeyboardShortcuts();

        if (Time.time < nextUpdateTime)
        {
            return;
        }

        nextUpdateTime = Time.time + Mathf.Max(0.05f, updateIntervalSeconds);
        UpdateMarkers(DateTime.UtcNow);
    }

    private void HandleKeyboardShortcuts()
    {
        if (!enableKeyboardShortcuts)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            RefreshIss(false);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            RefreshStations(false, groupDisplayLimit);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            RefreshActive(false, groupDisplayLimit);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            RefreshStarlink(false, groupDisplayLimit);
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            CycleLabelMode();
        }
        else if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            SelectMarker(selectedMarkerIndex - 1);
        }
        else if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            SelectMarker(selectedMarkerIndex + 1);
        }
    }

    public void RefreshIss(bool forceRefresh)
    {
        RefreshStarlink(forceRefresh, groupDisplayLimit);
    }

    public void RefreshActive(bool forceRefresh, int displayLimit)
    {
        RefreshStarlink(forceRefresh, displayLimit);
    }

    public void RefreshStations(bool forceRefresh, int displayLimit)
    {
        RefreshStarlink(forceRefresh, displayLimit);
    }

    public void RefreshStarlink(bool forceRefresh, int displayLimit)
    {
        dataSet = RealSatelliteDataSet.Starlink;
        maxDisplayed = EffectiveDisplayLimit(displayLimit);
        selectedMarkerIndex = 0;
        StartCoroutine(FetchCurrentDataSet(forceRefresh));
    }

    public void CycleLabelMode()
    {
        labelMode = (RealSatelliteLabelMode)(((int)labelMode + 1) % Enum.GetValues(typeof(RealSatelliteLabelMode)).Length);
        UpdateMarkers(DateTime.UtcNow, true);
    }

    public void SelectMarker(int index)
    {
        if (loadedTles.Count == 0)
        {
            selectedMarkerIndex = 0;
            return;
        }

        selectedMarkerIndex = Mathf.Clamp(index, 0, loadedTles.Count - 1);
        nextPathUpdateTime = 0f;
        UpdateMarkers(DateTime.UtcNow, true);
        SelectRegisteredSatelliteForMarker(selectedMarkerIndex, true);
    }

    public IEnumerator FetchCurrentDataSet(bool forceRefresh)
    {
        if (fetchInProgress)
        {
            yield break;
        }

        fetchInProgress = true;
        dataSet = RealSatelliteDataSet.Starlink;
        EnsureRuntimeObjects();
        UpdateRuntimeUi();
        LastStatus = "CelesTrak TLE 요청 중";

        Action<CelesTrakTleFetchResult> onComplete = ApplyFetchResult;
        if (dataSet == RealSatelliteDataSet.Starlink)
        {
            yield return tleClient.FetchStarlinkGroup(onComplete, forceRefresh);
        }
        else if (dataSet == RealSatelliteDataSet.Active)
        {
            yield return tleClient.FetchActiveGroup(onComplete, forceRefresh);
        }
        else if (dataSet == RealSatelliteDataSet.Stations)
        {
            yield return tleClient.FetchStationsGroup(onComplete, forceRefresh);
        }
        else
        {
            yield return tleClient.FetchIss(onComplete, forceRefresh);
        }

        fetchInProgress = false;
        UpdateRuntimeUi();
    }

    public bool TryGetFirstState(out string summary)
    {
        summary = string.Empty;
        if (DisplayedCount <= 0 || loadedTles.Count == 0)
        {
            summary = string.IsNullOrEmpty(LastStatus) ? "실제 위성 표시 대기" : LastStatus;
            return false;
        }

        summary = FirstSatelliteName + " · " + FirstGeodetic.latitudeDegrees.ToString("0.00") + "°, "
            + FirstGeodetic.longitudeDegrees.ToString("0.00") + "° · 고도 " + FirstGeodetic.altitudeKm.ToString("0") + "km";
        return true;
    }

    private void ApplyFetchResult(CelesTrakTleFetchResult result)
    {
        LastFetchUtc = DateTime.UtcNow;
        LastUrl = result != null ? result.url : string.Empty;
        LastError = result != null ? result.error : "CelesTrak 결과가 없습니다.";
        LastFetchFromCache = result != null && result.fromCache;
        LastFetchFromFallback = result != null && result.fromFallback;

        if (result != null && result.success && result.satellites != null)
        {
            loadedTles.Clear();
            int limit = EffectiveDisplayLimit(maxDisplayed);
            for (int i = 0; i < result.satellites.Count && loadedTles.Count < limit; i++)
            {
                loadedTles.Add(result.satellites[i]);
            }
            LastStatus = "CelesTrak " + result.dataFormat + " " + result.satellites.Count + "개 수신 · 표시 " + loadedTles.Count + "개";
            if (result.isEpochStale && !string.IsNullOrEmpty(result.warning))
            {
                LastStatus += " · " + result.warning;
            }
        }
        else
        {
            if (loadedTles.Count > 0)
            {
                LastStatus = "실제 TLE " + DataSetLabel(dataSet) + " 표시 중: " + FirstSatelliteName + " 외 " + Mathf.Max(0, DisplayedCount - 1) + "개";
                return;
            }
            LastStatus = "CelesTrak TLE 수신 실패: " + LastError;
        }

        EnsureMarkerCount(loadedTles.Count);
        TrimMarkerPool(loadedTles.Count);
        selectedMarkerIndex = Mathf.Clamp(selectedMarkerIndex, 0, Mathf.Max(0, loadedTles.Count - 1));
        updateCursor = 0;
        pathSatelliteIndex = -1;
        nextPathUpdateTime = 0f;
        UpdateMarkers(DateTime.UtcNow, true);
        SelectRegisteredSatelliteForMarker(selectedMarkerIndex, false);
    }

    private void UpdateMarkers(DateTime utc)
    {
        UpdateMarkers(utc, false);
    }

    private void UpdateMarkers(DateTime utc, bool forceFullUpdate)
    {
        EnsureRuntimeObjects();
        EnsureMarkerCount(loadedTles.Count);
        TrimMarkerPool(loadedTles.Count);

        int visibleCount = 0;
        FirstSatelliteName = string.Empty;
        FirstGeodetic = new GeodeticCoordinate();
        int updateStart = updateCursor;
        int updateEnd = loadedTles.Count;
        bool useBatchedUpdate = !forceFullUpdate && loadedTles.Count > Mathf.Max(1, groupUpdateBatchSize);
        bool shouldSyncSatelliteManager = syncDisplayedSatellitesToMissionManager
            && satelliteManager != null
            && (forceFullUpdate || Time.time >= nextSatelliteManagerSyncTime);
        if (useBatchedUpdate)
        {
            updateEnd = Mathf.Min(loadedTles.Count, updateStart + Mathf.Max(1, groupUpdateBatchSize));
        }

        for (int i = 0; i < markers.Count; i++)
        {
            if (i >= loadedTles.Count)
            {
                markers[i].SetVisible(false);
                continue;
            }

            RealSatelliteTle tle = loadedTles[i];
            bool shouldUpdateMarker = forceFullUpdate || !useBatchedUpdate || i == 0 || i == selectedMarkerIndex || (i >= updateStart && i < updateEnd);
            if (!shouldUpdateMarker)
            {
                if (markers[i].gameObject.activeSelf)
                {
                    visibleCount++;
                }
                if (string.IsNullOrEmpty(FirstSatelliteName) && i == 0)
                {
                    FirstSatelliteName = tle.name;
                    FirstGeodetic = markers[i].LastGeodetic;
                }
                continue;
            }

            Vector3 positionEciKm;
            Vector3 velocityEciKmSec;
            string error;
            if (!Sgp4Propagator.TryPropagate(tle, utc, out positionEciKm, out velocityEciKmSec, out error))
            {
                markers[i].SetVisible(false);
                LastError = error;
                continue;
            }

            float earthRadius = UnityEarthRadius();
            Vector3 earthCenter = EarthCenter();
            Vector3 positionEcefKm = SatelliteCoordinateTransforms.EciToEcef(positionEciKm, utc);
            GeodeticCoordinate geodetic = SatelliteCoordinateTransforms.EcefToGeodetic(positionEcefKm);
            Vector3 actualUnityPosition = SatelliteCoordinateTransforms.EcefToUnityPosition(positionEcefKm, earthRadius, earthCenter);
            Vector3 unityPosition = ApplyDisplayAltitude(actualUnityPosition, earthRadius, earthCenter, geodetic);
            Vector3 velocityUnityPerSecond = SatelliteCoordinateTransforms.EciVelocityToUnityVector(positionEciKm, velocityEciKmSec, utc, earthRadius);
            float speedKmPerSecond = velocityEciKmSec.magnitude;

            bool isSelected = i == selectedMarkerIndex;
            markers[i].SetVisible(true);
            markers[i].SetIdentity(tle.name);
            markers[i].SetVisualStyle(MarkerColorForIndex(i, isSelected), LabelColorForIndex(i, isSelected), isSelected ? 0.048f : 0.032f, showVelocityLines);
            markers[i].SetState(
                unityPosition,
                velocityUnityPerSecond,
                geodetic,
                EffectiveVelocityPreviewSeconds(),
                ShouldShowLabel(i),
                BuildLabelText(tle.name, geodetic, i),
                showVelocityLines,
                isSelected);
            SyncRealSatelliteToMissionManager(tle, markers[i], geodetic, speedKmPerSecond, shouldSyncSatelliteManager);
            visibleCount++;

            if (string.IsNullOrEmpty(FirstSatelliteName))
            {
                FirstSatelliteName = tle.name;
                FirstGeodetic = geodetic;
            }
        }

        if (useBatchedUpdate)
        {
            updateCursor = updateEnd >= loadedTles.Count ? 0 : updateEnd;
        }

        DisplayedCount = visibleCount;
        if (DisplayedCount > 0)
        {
            LastStatus = "실제 TLE " + DataSetLabel(dataSet) + " 표시 중: " + FirstSatelliteName + " 외 " + Mathf.Max(0, DisplayedCount - 1) + "개";
            if (shouldSyncSatelliteManager)
            {
                nextSatelliteManagerSyncTime = Time.time + Mathf.Max(1f, satelliteManagerSyncIntervalSeconds);
            }
        }
        UpdateModeStrings(useBatchedUpdate);
        UpdateFirstPathLines(utc);
        UpdateRuntimeUi();
    }

    private void EnsureRuntimeObjects()
    {
        if (tleClient == null)
        {
            tleClient = GetComponent<CelesTrakTleClient>();
            if (tleClient == null)
            {
                tleClient = gameObject.AddComponent<CelesTrakTleClient>();
            }
        }

        if (earth == null)
        {
            GameObject earthObject = GameObject.Find("Earth");
            if (earthObject != null)
            {
                earth = earthObject.transform;
            }
        }

        if (satelliteManager == null)
        {
            satelliteManager = FindObjectOfType<SatelliteManager>();
        }
        if (focusController == null)
        {
            focusController = FindObjectOfType<SatelliteFocusController>();
        }
        if (cameraManager == null)
        {
            cameraManager = FindObjectOfType<CameraManager>();
        }
        if (missionVisualController == null)
        {
            missionVisualController = FindObjectOfType<MissionVisualController>();
        }

        if (markerParent == null)
        {
            GameObject satellitesRoot = GameObject.Find("Satellites");
            markerParent = satellitesRoot != null ? satellitesRoot.transform : null;
        }

        if (realSatelliteRoot == null)
        {
            GameObject root = new GameObject("RealSatelliteLiveTrackers");
            root.transform.SetParent(markerParent, false);
            realSatelliteRoot = root.transform;
        }
        else if (markerParent != null && realSatelliteRoot.parent != markerParent)
        {
            realSatelliteRoot.SetParent(markerParent, false);
        }
        else if (markerParent != null && realSatelliteRoot.parent != markerParent)
        {
            realSatelliteRoot.SetParent(markerParent, false);
        }

        if (markerMaterial == null)
        {
            markerMaterial = CreateMaterial("Real Satellite Marker", new Color(1f, 0.72f, 0.22f, 1f));
        }
        if (velocityMaterial == null)
        {
            velocityMaterial = CreateMaterial("Real Satellite Velocity", new Color(0.72f, 1f, 0.32f, 0.9f));
        }
        if (orbitTrailMaterial == null)
        {
            orbitTrailMaterial = CreateMaterial("Real Satellite Orbit Trail", new Color(1f, 0.84f, 0.28f, 0.045f));
        }
        if (groundTrackMaterial == null)
        {
            groundTrackMaterial = CreateMaterial("Real Satellite Ground Track", new Color(0.32f, 0.92f, 1f, 0.035f));
        }
        EnsurePathLines();
        EnsureRuntimeUi();
    }

    private void EnsureMarkerCount(int count)
    {
        EnsureRuntimeObjects();
        while (markers.Count < count)
        {
            GameObject markerObject = new GameObject("RealSatelliteMarker");
            markerObject.transform.SetParent(realSatelliteRoot, false);
            RealSatelliteMarker marker = markerObject.AddComponent<RealSatelliteMarker>();
            string satelliteName = markers.Count < loadedTles.Count ? loadedTles[markers.Count].name : "Real Satellite";
            marker.Initialize(satelliteName, markerMaterial, velocityMaterial);
            markers.Add(marker);
        }
    }

    private void TrimMarkerPool(int activeCount)
    {
        int targetPoolSize = Mathf.Min(Mathf.Max(activeCount + Mathf.Max(0, markerPoolSlack), activeCount), Mathf.Max(1, hardMarkerPoolLimit));
        while (markers.Count > targetPoolSize)
        {
            int last = markers.Count - 1;
            RealSatelliteMarker marker = markers[last];
            markers.RemoveAt(last);
            if (marker != null)
            {
                Destroy(marker.gameObject);
            }
        }
    }

    private int EffectiveDisplayLimit(int requested)
    {
        int limit = Mathf.Max(1, requested);
        if (dataSet != RealSatelliteDataSet.Iss)
        {
            limit = Mathf.Min(limit, Mathf.Max(1, groupDisplayLimit));
        }
        return Mathf.Min(limit, Mathf.Max(1, hardMarkerPoolLimit));
    }

    private float EffectiveVelocityPreviewSeconds()
    {
        if (velocityPreviewSeconds > 0f)
        {
            return velocityPreviewSeconds;
        }

        return Mathf.Max(0.01f, velocityLineLength);
    }

    private bool ShouldShowLabel(int index)
    {
        if (!showLabels || labelMode == RealSatelliteLabelMode.Off)
        {
            return false;
        }
        if (labelMode == RealSatelliteLabelMode.All)
        {
            return true;
        }
        if (labelMode == RealSatelliteLabelMode.Selected)
        {
            return index == selectedMarkerIndex;
        }
        return index == 0;
    }

    private string BuildLabelText(string satelliteName, GeodeticCoordinate geodetic, int index)
    {
        return satelliteName + "  #" + (index + 1)
            + "\n" + geodetic.latitudeDegrees.ToString("0.0") + "°, " + geodetic.longitudeDegrees.ToString("0.0") + "° · " + geodetic.altitudeKm.ToString("0") + "km";
    }

    private Vector3 ApplyDisplayAltitude(Vector3 actualUnityPosition, float earthRadius, Vector3 earthCenter, GeodeticCoordinate geodetic)
    {
        Vector3 fromCenter = actualUnityPosition - earthCenter;
        if (earthRadius <= 0f || fromCenter.sqrMagnitude < 1e-8f)
        {
            return actualUnityPosition;
        }

        float actualAltitudeUnity = Mathf.Max(0f, fromCenter.magnitude - earthRadius);
        float lowOrbitWeight = Mathf.InverseLerp(Mathf.Max(1f, displayAltitudeScaleFadeKm), 0f, Mathf.Max(0f, (float)geodetic.altitudeKm));
        float displayAltitude = Mathf.Lerp(actualAltitudeUnity, actualAltitudeUnity * Mathf.Max(1f, displayAltitudeScale), lowOrbitWeight);
        if (geodetic.altitudeKm > 80.0 && lowOrbitWeight > 0.01f)
        {
            displayAltitude = Mathf.Max(displayAltitude, Mathf.Max(0f, minimumDisplayAltitude) * lowOrbitWeight);
        }

        return earthCenter + fromCenter.normalized * (earthRadius + displayAltitude);
    }

    private void SyncRealSatelliteToMissionManager(RealSatelliteTle tle, RealSatelliteMarker marker, GeodeticCoordinate geodetic, float speedKmPerSecond, bool shouldSync)
    {
        if (!shouldSync || tle == null || marker == null || satelliteManager == null)
        {
            return;
        }

        string satelliteKey = BuildMissionSatelliteKey(tle);
        int uiHash = Mathf.Abs(SatelliteRouteEstimator.StableHash(satelliteKey));
        int uiWindowMinutes = uiHash % 6;
        int uiFeePer32Kb = 1 + uiHash % 5;
        int uiLatencySeconds = 3 + (uiHash / 7) % 15;
        int uiReliabilityPercent = Mathf.Clamp(84 + (uiHash / 13) % 15, 0, 100);
        int uiCongestionPercent = Mathf.Clamp(8 + (uiHash / 17) % 64, 0, 100);
        SatelliteData registeredSatellite = satelliteManager.UpsertRealSatellite(
            satelliteKey,
            marker.transform,
            (float)geodetic.altitudeKm,
            Mathf.Max(0.1f, speedKmPerSecond),
            uiWindowMinutes,
            uiFeePer32Kb,
            uiLatencySeconds,
            tle.sourceFormat == "TLE" ? uiReliabilityPercent : Mathf.Max(82, uiReliabilityPercent - 4),
            dataSet == RealSatelliteDataSet.Starlink ? Mathf.Min(72, uiCongestionPercent + 8) : uiCongestionPercent,
            true);
        ConfigureRealSatelliteSelection(registeredSatellite, marker);
    }

    private string BuildMissionSatelliteKey(RealSatelliteTle tle)
    {
        if (tle == null)
        {
            return "Unknown Real Satellite";
        }
        return tle.noradId > 0
            ? tle.noradId.ToString("00000") + " " + tle.name
            : tle.name;
    }

    private string BuildRegisteredMissionSatelliteName(RealSatelliteTle tle)
    {
        return "REAL-" + BuildMissionSatelliteKey(tle);
    }

    private void SelectRegisteredSatelliteForMarker(int index, bool focusCamera)
    {
        if (satelliteManager == null || index < 0 || index >= loadedTles.Count)
        {
            return;
        }

        string registeredName = BuildRegisteredMissionSatelliteName(loadedTles[index]);
        satelliteManager.SelectSatellite(registeredName);
        Transform selectedTransform = index < markers.Count && markers[index] != null ? markers[index].transform : null;
        if (selectedTransform != null)
        {
            if (focusCamera && cameraManager != null)
            {
                cameraManager.satellite = selectedTransform;
                cameraManager.SetSatellite();
            }
            if (missionVisualController != null)
            {
                missionVisualController.SetSelectedSatelliteTransform(selectedTransform);
            }
        }
    }

    private void ConfigureRealSatelliteSelection(SatelliteData satellite, RealSatelliteMarker marker)
    {
        if (satellite == null || marker == null)
        {
            return;
        }

        SatelliteClickTarget clickTarget = marker.GetComponent<SatelliteClickTarget>();
        if (clickTarget == null)
        {
            clickTarget = marker.gameObject.AddComponent<SatelliteClickTarget>();
        }
        clickTarget.hitRadius = 0.46f;
        clickTarget.Initialize(satellite.satelliteName, satelliteManager, cameraManager);

        Transform registeredTransform;
        bool needsFocusRegistration = focusController != null
            && (!focusRegisteredTransforms.TryGetValue(satellite.satelliteName, out registeredTransform) || registeredTransform != marker.transform);
        if (needsFocusRegistration)
        {
            focusController.RegisterSatellite(satellite.satelliteName, marker.transform);
            focusRegisteredTransforms[satellite.satelliteName] = marker.transform;
        }

        if (satelliteManager != null
            && satelliteManager.SelectedSatellite != null
            && satelliteManager.SelectedSatellite.satelliteName == satellite.satelliteName)
        {
            if (cameraManager != null)
            {
                cameraManager.satellite = marker.transform;
            }
            if (missionVisualController != null)
            {
                missionVisualController.SetSelectedSatelliteTransform(marker.transform);
            }
        }
    }

    private Color MarkerColorForIndex(int index, bool isSelected)
    {
        if (isSelected)
        {
            return new Color(0.28f, 1f, 0.95f, 1f);
        }
        if (dataSet == RealSatelliteDataSet.Starlink)
        {
            return new Color(0.72f, 0.86f, 1f, 1f);
        }
        if (dataSet == RealSatelliteDataSet.Stations)
        {
            return new Color(0.74f, 1f, 0.72f, 1f);
        }
        if (dataSet == RealSatelliteDataSet.Active)
        {
            return new Color(1f, 0.82f, 0.28f, 1f);
        }
        return index == 0 ? new Color(1f, 0.58f, 0.2f, 1f) : new Color(1f, 0.72f, 0.22f, 1f);
    }

    private Color LabelColorForIndex(int index, bool isSelected)
    {
        if (isSelected)
        {
            return new Color(0.74f, 1f, 1f, 0.98f);
        }
        return index == 0 ? new Color(1f, 0.92f, 0.68f, 0.96f) : new Color(0.78f, 0.9f, 1f, 0.9f);
    }

    private string DataSetLabel(RealSatelliteDataSet value)
    {
        if (value == RealSatelliteDataSet.Starlink)
        {
            return "STARLINK";
        }
        if (value == RealSatelliteDataSet.Stations)
        {
            return "STATIONS";
        }
        if (value == RealSatelliteDataSet.Active)
        {
            return "ACTIVE";
        }
        return "ISS";
    }

    private void UpdateModeStrings(bool batchedUpdate)
    {
        LastDisplayMode = DataSetLabel(dataSet) + " · loaded=" + LoadedCount + " · displayed=" + DisplayedCount + " · limit=" + EffectiveDisplayLimit(maxDisplayed);
        LastLabelMode = labelMode.ToString() + " · selectedIndex=" + selectedMarkerIndex;
        LastVelocityLineMode = "VelocityLine=Unity km/s scaled · previewSeconds=" + EffectiveVelocityPreviewSeconds().ToString("0.#");
        LastPerformanceMode = batchedUpdate
            ? "Batched SGP4 update · batch=" + Mathf.Max(1, groupUpdateBatchSize)
            : "Full SGP4 update";
    }

    private void EnsurePathLines()
    {
        if (realSatelliteRoot == null)
        {
            return;
        }
        if (firstOrbitTrail == null)
        {
            GameObject trailObject = new GameObject("FirstRealSatelliteOrbitTrail");
            trailObject.transform.SetParent(realSatelliteRoot, false);
            firstOrbitTrail = trailObject.AddComponent<LineRenderer>();
            ConfigurePathLine(firstOrbitTrail, orbitTrailMaterial, 0.0008f);
        }
        if (firstGroundTrack == null)
        {
            GameObject groundObject = new GameObject("FirstRealSatelliteGroundTrack");
            groundObject.transform.SetParent(realSatelliteRoot, false);
            firstGroundTrack = groundObject.AddComponent<LineRenderer>();
            ConfigurePathLine(firstGroundTrack, groundTrackMaterial, 0.0007f);
        }
    }

    private void ConfigurePathLine(LineRenderer line, Material material, float width)
    {
        line.useWorldSpace = true;
        line.loop = false;
        line.widthMultiplier = width;
        line.positionCount = 0;
        line.enabled = false;
        if (material != null)
        {
            line.material = material;
            line.startColor = material.color;
            line.endColor = material.color;
        }
    }

    private void UpdateFirstPathLines(DateTime utc)
    {
        if (loadedTles.Count == 0)
        {
            SetLineEnabled(firstOrbitTrail, false);
            SetLineEnabled(firstGroundTrack, false);
            pathSatelliteIndex = -1;
            return;
        }

        int targetIndex = Mathf.Clamp(selectedMarkerIndex, 0, loadedTles.Count - 1);
        bool targetChanged = pathSatelliteIndex != targetIndex;
        bool hasPathPoints = (firstOrbitTrail != null && firstOrbitTrail.positionCount > 0)
            || (firstGroundTrack != null && firstGroundTrack.positionCount > 0);
        if (!targetChanged && Time.time < nextPathUpdateTime && hasPathPoints)
        {
            SetLineEnabled(firstOrbitTrail, showFirstOrbitTrail);
            SetLineEnabled(firstGroundTrack, showFirstGroundTrack);
            return;
        }
        nextPathUpdateTime = Time.time + Mathf.Max(1f, pathUpdateIntervalSeconds);

        pathSatelliteIndex = targetIndex;
        RealSatelliteTle tle = loadedTles[targetIndex];
        int pointCount = Mathf.Clamp(orbitTrailPointCount, 8, 240);
        float earthRadius = UnityEarthRadius();
        Vector3 center = EarthCenter();
        if (showFirstOrbitTrail && firstOrbitTrail != null)
        {
            firstOrbitTrail.enabled = true;
            firstOrbitTrail.positionCount = pointCount;
        }
        if (showFirstGroundTrack && firstGroundTrack != null)
        {
            firstGroundTrack.enabled = true;
            firstGroundTrack.positionCount = pointCount;
        }

        for (int i = 0; i < pointCount; i++)
        {
            DateTime sampleUtc = utc.AddSeconds((i - pointCount / 2) * Mathf.Max(5f, orbitTrailStepSeconds));
            Vector3 samplePositionEciKm;
            Vector3 sampleVelocityEciKmSec;
            string error;
            if (!Sgp4Propagator.TryPropagate(tle, sampleUtc, out samplePositionEciKm, out sampleVelocityEciKmSec, out error))
            {
                SetLineEnabled(firstOrbitTrail, false);
                SetLineEnabled(firstGroundTrack, false);
                return;
            }

            Vector3 sampleEcefKm = SatelliteCoordinateTransforms.EciToEcef(samplePositionEciKm, sampleUtc);
            GeodeticCoordinate sampleGeodetic = SatelliteCoordinateTransforms.EcefToGeodetic(sampleEcefKm);
            Vector3 actualOrbitPosition = SatelliteCoordinateTransforms.EcefToUnityPosition(sampleEcefKm, earthRadius, center);
            Vector3 orbitPosition = ApplyDisplayAltitude(actualOrbitPosition, earthRadius, center, sampleGeodetic);
            Vector3 groundPosition = center + (orbitPosition - center).normalized * (earthRadius * 1.012f);
            if (showFirstOrbitTrail && firstOrbitTrail != null)
            {
                firstOrbitTrail.SetPosition(i, orbitPosition);
            }
            if (showFirstGroundTrack && firstGroundTrack != null)
            {
                firstGroundTrack.SetPosition(i, groundPosition);
            }
        }

        SetLineEnabled(firstOrbitTrail, showFirstOrbitTrail);
        SetLineEnabled(firstGroundTrack, showFirstGroundTrack);
    }

    public bool TryGetDisplayOrbitSamplesForTransform(Transform satelliteTransform, int pointCount, float sampleStepSeconds, Vector3[] samples)
    {
        if (satelliteTransform == null || samples == null || samples.Length == 0 || loadedTles.Count == 0)
        {
            return false;
        }

        int markerIndex = MarkerIndexForTransform(satelliteTransform);
        if (markerIndex < 0 || markerIndex >= loadedTles.Count)
        {
            return false;
        }

        int count = Mathf.Clamp(pointCount, 2, samples.Length);
        RealSatelliteTle tle = loadedTles[markerIndex];
        float earthRadius = UnityEarthRadius();
        Vector3 center = EarthCenter();
        DateTime utc = DateTime.UtcNow;
        float stepSeconds = Mathf.Max(5f, sampleStepSeconds);

        for (int i = 0; i < count; i++)
        {
            DateTime sampleUtc = utc.AddSeconds((i - count / 2) * stepSeconds);
            Vector3 samplePositionEciKm;
            Vector3 sampleVelocityEciKmSec;
            string error;
            if (!Sgp4Propagator.TryPropagate(tle, sampleUtc, out samplePositionEciKm, out sampleVelocityEciKmSec, out error))
            {
                LastError = error;
                return false;
            }

            Vector3 sampleEcefKm = SatelliteCoordinateTransforms.EciToEcef(samplePositionEciKm, sampleUtc);
            GeodeticCoordinate sampleGeodetic = SatelliteCoordinateTransforms.EcefToGeodetic(sampleEcefKm);
            Vector3 actualOrbitPosition = SatelliteCoordinateTransforms.EcefToUnityPosition(sampleEcefKm, earthRadius, center);
            samples[i] = ApplyDisplayAltitude(actualOrbitPosition, earthRadius, center, sampleGeodetic);
        }

        return true;
    }

    public bool IsRealSatelliteTransform(Transform satelliteTransform)
    {
        return MarkerIndexForTransform(satelliteTransform) >= 0;
    }

    private int MarkerIndexForTransform(Transform satelliteTransform)
    {
        if (satelliteTransform == null)
        {
            return -1;
        }

        for (int i = 0; i < markers.Count; i++)
        {
            if (markers[i] != null && markers[i].transform == satelliteTransform)
            {
                return i;
            }
        }

        return -1;
    }

    private void SetLineEnabled(LineRenderer line, bool enabled)
    {
        if (line != null)
        {
            line.enabled = enabled;
        }
    }

    private void EnsureRuntimeUi()
    {
        if (!showRuntimeControls || runtimeCanvas != null)
        {
            if (!showRuntimeControls && runtimeCanvas != null)
            {
                runtimeCanvas.gameObject.SetActive(false);
            }
            return;
        }

        GameObject canvasObject = new GameObject("RealSatelliteRuntimeControls", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        runtimeCanvas = canvasObject.GetComponent<Canvas>();
        runtimeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        EnsureEventSystem();
        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1600f, 900f);

        GameObject panel = new GameObject("Panel", typeof(Image));
        panel.transform.SetParent(canvasObject.transform, false);
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.315f, 0.078f);
        panelRect.anchorMax = new Vector2(0.315f, 0.078f);
        panelRect.pivot = new Vector2(0f, 0f);
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.sizeDelta = new Vector2(314f, 84f);
        Image panelImage = panel.GetComponent<Image>();
        panelImage.color = new Color(0.02f, 0.03f, 0.05f, 0.66f);

        CreateButton(panel.transform, "ISS", new Vector2(8f, -8f), delegate { RefreshIss(false); });
        CreateButton(panel.transform, "STATION", new Vector2(72f, -8f), delegate { RefreshStations(false, groupDisplayLimit); });
        CreateButton(panel.transform, "ACTIVE", new Vector2(154f, -8f), delegate { RefreshActive(false, groupDisplayLimit); });
        CreateButton(panel.transform, "STARLINK", new Vector2(236f, -8f), delegate { RefreshStarlink(false, groupDisplayLimit); });
        CreateButton(panel.transform, "LABEL", new Vector2(8f, -46f), delegate { CycleLabelMode(); });
        CreateButton(panel.transform, "PREV", new Vector2(92f, -46f), delegate { SelectMarker(selectedMarkerIndex - 1); });
        CreateButton(panel.transform, "NEXT", new Vector2(170f, -46f), delegate { SelectMarker(selectedMarkerIndex + 1); });

        runtimeStatusText = null;
        UpdateRuntimeUi();
    }

    private void EnsureEventSystem()
    {
        if (FindObjectOfType<EventSystem>() != null)
        {
            return;
        }

        new GameObject("RealSatelliteRuntimeEventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
    }

    private void CreateButton(Transform parent, string text, Vector2 anchoredPosition, UnityEngine.Events.UnityAction action)
    {
        GameObject buttonObject = new GameObject(text + "Button", typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(parent, false);
        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = new Vector2(text.Length > 5 ? 78f : 58f, 30f);
        Image image = buttonObject.GetComponent<Image>();
        image.color = new Color(0.12f, 0.18f, 0.22f, 0.86f);
        Button button = buttonObject.GetComponent<Button>();
        button.onClick.AddListener(action);

        GameObject labelObject = new GameObject("Text", typeof(Text));
        labelObject.transform.SetParent(buttonObject.transform, false);
        Text labelText = labelObject.GetComponent<Text>();
        labelText.font = RuntimeUiFont();
        labelText.fontSize = 12;
        labelText.alignment = TextAnchor.MiddleCenter;
        labelText.color = Color.white;
        labelText.text = text;
        RectTransform labelRect = labelObject.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;
    }

    private void UpdateRuntimeUi()
    {
        if (runtimeCanvas != null)
        {
            runtimeCanvas.gameObject.SetActive(showRuntimeControls);
        }
    }

    private static Font RuntimeUiFont()
    {
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        return font != null ? font : Font.CreateDynamicFontFromOSFont("Arial", 12);
    }

    private float UnityEarthRadius()
    {
        if (earth == null)
        {
            return SatelliteCoordinateTransforms.DefaultUnityEarthRadius;
        }
        Vector3 scale = earth.lossyScale;
        return Mathf.Max(scale.x, Mathf.Max(scale.y, scale.z)) * 0.5f;
    }

    private Vector3 EarthCenter()
    {
        return earth != null ? earth.position : Vector3.zero;
    }

    private static Material CreateMaterial(string materialName, Color color)
    {
        Shader shader = Shader.Find("Sprites/Default");
        if (shader == null)
        {
            shader = Shader.Find("Unlit/Color");
        }
        Material material = new Material(shader);
        material.name = materialName;
        material.color = color;
        return material;
    }
}
