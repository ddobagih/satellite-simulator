using System.Collections.Generic;
using UnityEngine;

public class OrbitShareBootstrap : MonoBehaviour
{
    [SerializeField]
    private bool useVirtualDemoSatellites = false;

    private const string EarthDayTexturePath = "OrbitShare/Earth/EarthDaySSS";
    private const string EarthNightTexturePath = "OrbitShare/Earth/EarthNightSSS";
    private const string EarthCloudsTexturePath = "OrbitShare/Earth/EarthCloudsSSS";
    private const string EarthNormalTexturePath = "OrbitShare/Earth/EarthNormalSSS";
    private const string EarthSpecularTexturePath = "OrbitShare/Earth/EarthSpecularSSS";
    private const string EarthTextureAttributionText = "Earth textures: Solar System Scope, CC BY 4.0, via Wikimedia Commons. See Assets/Resources/OrbitShare/Earth/ATTRIBUTION.md";
    private const string EarthTextureNotLoadedStatus = "Earth textures: not loaded";

    private static readonly Vector3 SunDirection = new Vector3(-0.35f, 0.32f, -0.88f).normalized;
    private static bool bootstrapped;

    public static string EarthTextureAttribution
    {
        get { return EarthTextureAttributionText; }
    }

    public static string EarthTextureStatus { get; private set; } = EarthTextureNotLoadedStatus;

    public static string GetEarthTextureAttribution()
    {
        return EarthTextureAttribution;
    }

    public static string GetEarthTextureStatus()
    {
        return EarthTextureStatus;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetBootstrapState()
    {
        bootstrapped = false;
        EarthTextureStatus = EarthTextureNotLoadedStatus;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void BootstrapOnPlay()
    {
        if (bootstrapped || FindObjectOfType<OrbitShareBootstrap>() != null)
        {
            return;
        }

        GameObject bootstrap = new GameObject("OrbitShareBootstrap");
        bootstrap.AddComponent<OrbitShareBootstrap>();
    }

    private void Awake()
    {
        Application.runInBackground = true;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        if (bootstrapped)
        {
            Destroy(gameObject);
            return;
        }

        bootstrapped = true;
        BuildScene();
    }

    private void BuildScene()
    {
        bool hasExternalEarthTextures = HasExternalEarthTextures();
        bool hasExternalNightTexture = LoadEarthTexture(EarthNightTexturePath) != null;
        Material earthMaterial = CreateEarthMaterial();
        Material importedEarthMaterial = Resources.Load<Material>("OrbitShare/PremiumEarthMaterial");
        if (importedEarthMaterial != null)
        {
            earthMaterial = importedEarthMaterial;
            ApplyEarthTextures(earthMaterial);
        }
        Material satelliteMaterial = CreateEmissiveMaterial("Satellite Brushed Metal", new Color(0.62f, 0.68f, 0.72f), 0.18f);
        Material panelMaterial = CreateEmissiveMaterial("Solar Panel Cyan", new Color(0.04f, 0.34f, 0.48f), 0.55f);
        Material orbitMaterial = CreateLineMaterial("Primary Orbit Soft White", new Color(0.94f, 0.98f, 1f, 0.24f), true);
        Material targetMaterial = CreateEmissiveMaterial("Destination City", new Color(0.58f, 0.9f, 1f), 0.55f);
        Material selectedTargetMaterial = CreateEmissiveMaterial("Selected Destination", new Color(0.72f, 1f, 1f), 1.15f);
        Material stationMaterial = CreateEmissiveMaterial("Ground Station Green", new Color(0.2f, 1f, 0.55f), 1f);
        Material lineMaterial = CreateEmissiveMaterial("Connection Line", new Color(0.58f, 1f, 0.96f), 1.35f);
        Material atmosphereMaterial = CreateAtmosphereMaterial(hasExternalEarthTextures);
        Material cloudMaterial = CreateCloudMaterial(0.34f);
        Material nightLightsMaterial = CreateNightLightsMaterial(hasExternalNightTexture);
        Material targetHaloMaterial = CreateTransparentMaterial("Signal Footprint", new Color(0.32f, 0.82f, 1f, 0.15f));
        Material targetActiveHaloMaterial = CreateTransparentMaterial("Active Signal Footprint", new Color(0.36f, 1f, 0.94f, 0.38f));
        Material stationRangeMaterial = CreateTransparentMaterial("Communication Range", new Color(0.45f, 0.86f, 1f, 0.055f));
        Material stationActiveRangeMaterial = CreateTransparentMaterial("Active Communication Range", new Color(0.55f, 1f, 0.96f, 0.15f));
        targetHaloMaterial.renderQueue = 3060;
        targetActiveHaloMaterial.renderQueue = 3060;
        stationRangeMaterial.renderQueue = 3060;
        stationActiveRangeMaterial.renderQueue = 3060;
        Material focusHaloMaterial = CreateEmissiveMaterial("Selected Satellite Indicator", new Color(0.24f, 1f, 0.92f), 1.8f);
        Material secondaryOrbitMaterial = CreateLineMaterial("Secondary Orbit Pearl White", new Color(0.88f, 0.95f, 1f, 0.09f), true);
        Material stationHullMaterial = CreateMaterial("Station Hull", new Color(0.62f, 0.68f, 0.72f));
        Material stationAccentMaterial = CreateEmissiveMaterial("Station Accent", new Color(0.25f, 0.92f, 1f), 1.1f);
        GameObject importedSatellitePrefab = Resources.Load<GameObject>("OrbitShare/SatelliteModel");
        GameObject importedStationPrefab = Resources.Load<GameObject>("OrbitShare/SpaceStation");
        Material importedSkybox = Resources.Load<Material>("OrbitShare/SpaceSkybox");

        Camera camera = CreateCamera();
        CreateLight();
        if (importedSkybox != null)
        {
            RenderSettings.skybox = importedSkybox;
            camera.clearFlags = CameraClearFlags.Skybox;
        }
        else
        {
            CreateStarfield();
        }

        GameObject managers = new GameObject("Managers");
        SimulationManager simulationManager = managers.AddComponent<SimulationManager>();
        SatelliteManager satelliteManager = managers.AddComponent<SatelliteManager>();
        MissionManager missionManager = managers.AddComponent<MissionManager>();
        CameraManager cameraManager = managers.AddComponent<CameraManager>();
        UIManager uiManager = managers.AddComponent<UIManager>();
        ImmediateUIManager immediateUIManager = managers.AddComponent<ImmediateUIManager>();
        PresentationDirector presentationDirector = managers.AddComponent<PresentationDirector>();
        AudioFeedbackManager audioFeedbackManager = managers.AddComponent<AudioFeedbackManager>();
        SatelliteFocusController focusController = managers.AddComponent<SatelliteFocusController>();
        CelesTrakTleClient celesTrakTleClient = managers.AddComponent<CelesTrakTleClient>();
        RealSatelliteVisualizer realSatelliteVisualizer = managers.AddComponent<RealSatelliteVisualizer>();
        managers.AddComponent<AssetStoreIntegrationGuide>();

        GameObject earth = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        earth.name = "Earth";
        earth.transform.position = Vector3.zero;
        earth.transform.localScale = Vector3.one * 6f;
        earth.GetComponent<Renderer>().material = earthMaterial;
        EarthRotation earthRotation = earth.AddComponent<EarthRotation>();
        earthRotation.simulationManager = simulationManager;
        earthRotation.rotationSpeed = 0f;
        CreateAtmosphere(earth.transform, atmosphereMaterial, hasExternalEarthTextures);
        if (!hasExternalNightTexture)
        {
            CreateNightLightsLayer(earth.transform, nightLightsMaterial);
        }
        CreateCloudLayer(earth.transform, cloudMaterial, simulationManager);
        GameObject satellitesRoot = new GameObject("Satellites");
        realSatelliteVisualizer.earth = earth.transform;
        realSatelliteVisualizer.markerParent = satellitesRoot.transform;
        realSatelliteVisualizer.satelliteManager = satelliteManager;
        realSatelliteVisualizer.tleClient = celesTrakTleClient;
        realSatelliteVisualizer.dataSet = RealSatelliteDataSet.Starlink;
        realSatelliteVisualizer.enableKeyboardShortcuts = false;
        realSatelliteVisualizer.maxDisplayed = 100;
        realSatelliteVisualizer.groupDisplayLimit = 100;
        realSatelliteVisualizer.hardMarkerPoolLimit = 120;

        GameObject sharedSatellite = useVirtualDemoSatellites
            ? CreateSatelliteVisual("SAT-S01", satellitesRoot.transform, satelliteMaterial, panelMaterial, importedSatellitePrefab)
            : CreateRouteFallbackAnchor("RouteFallbackAnchor", satellitesRoot.transform);
        SatelliteOrbit satelliteOrbit = sharedSatellite.AddComponent<SatelliteOrbit>();
        satelliteOrbit.earth = earth.transform;
        satelliteOrbit.simulationManager = simulationManager;

        GameObject personalSatellite = null;
        GameObject companySatellite = null;
        List<GameObject> relaySatellites = new List<GameObject>();
        if (useVirtualDemoSatellites)
        {
            personalSatellite = CreateStaticSatellite("SAT-P01", satellitesRoot.transform, satelliteMaterial, panelMaterial, importedSatellitePrefab, new Vector3(-5.6f, 2.1f, 3.8f));
            EnterpriseSatelliteAnimator personalAnimator = personalSatellite.AddComponent<EnterpriseSatelliteAnimator>();
            personalAnimator.simulationManager = simulationManager;
            personalAnimator.radius = 6.85f;
            personalAnimator.inclination = 18f;
            personalAnimator.angle = 142f;
            personalAnimator.speed = 4.8f;
            companySatellite = CreateStaticSatellite("SAT-C01", satellitesRoot.transform, satelliteMaterial, panelMaterial, importedSatellitePrefab, new Vector3(4.9f, -1.5f, 4.4f));
            EnterpriseSatelliteAnimator companyAnimator = companySatellite.AddComponent<EnterpriseSatelliteAnimator>();
            companyAnimator.simulationManager = simulationManager;
            companyAnimator.radius = 7.65f;
            companyAnimator.inclination = -22f;
            companyAnimator.angle = 42f;
            companyAnimator.speed = 5.4f;
            relaySatellites = CreateRelayConstellation(satellitesRoot.transform, satelliteMaterial, panelMaterial, importedSatellitePrefab, simulationManager);
        }
        if (useVirtualDemoSatellites)
        {
            GameObject orbitStation = CreateSpaceStation("OrbitShareStation", satellitesRoot.transform, stationHullMaterial, stationAccentMaterial, importedStationPrefab);
            SpaceStationAnimator stationAnimator = orbitStation.GetComponent<SpaceStationAnimator>();
            if (stationAnimator == null)
            {
                stationAnimator = orbitStation.AddComponent<SpaceStationAnimator>();
            }
            stationAnimator.simulationManager = simulationManager;

        }

        GameObject targetRoot = new GameObject("RequestCities");
        ObservationTarget seoul = CreateTarget("SeoulCity", "Seoul", targetRoot.transform, targetMaterial, selectedTargetMaterial, 37.56f, 126.97f, 64f, 0.4f, 120, "Local Loopback", 3f);
        ObservationTarget newYork = CreateTarget("NewYorkCity", "New York", targetRoot.transform, targetMaterial, selectedTargetMaterial, 40.71f, -74.01f, 128f, 0.9f, 220, "Trans-Pacific Relay", 12f);
        ObservationTarget london = CreateTarget("LondonCity", "London", targetRoot.transform, targetMaterial, selectedTargetMaterial, 51.51f, -0.13f, 96f, 0.7f, 190, "Europe Relay", 8f);
        ObservationTarget tokyo = CreateTarget("TokyoCity", "Tokyo", targetRoot.transform, targetMaterial, selectedTargetMaterial, 35.68f, 139.69f, 104f, 0.8f, 210, "Asia Relay", 10f);
        ObservationTarget paris = CreateTarget("ParisCity", "Paris", targetRoot.transform, targetMaterial, selectedTargetMaterial, 48.85f, 2.35f, 88f, 0.7f, 185, "Europe Relay", 9f);
        ObservationTarget sydney = CreateTarget("SydneyCity", "Sydney", targetRoot.transform, targetMaterial, selectedTargetMaterial, -33.87f, 151.21f, 112f, 0.85f, 240, "Southern Relay", 14f);
        ObservationTarget losAngeles = CreateTarget("LosAngelesCity", "Los Angeles", targetRoot.transform, targetMaterial, selectedTargetMaterial, 34.05f, -118.24f, 118f, 0.8f, 205, "Pacific Relay", 11f);

        GameObject groundStationObject = CreateGroundStation(stationMaterial, 37.56f, 126.97f);
        GroundStation groundStation = groundStationObject.AddComponent<GroundStation>();
        GameObject stationRange = CreateSurfaceDisc("SenderCommunicationRange", groundStationObject.transform.position, 0.46f, stationRangeMaterial);
        SetRendererEnabled(groundStationObject, false);
        SetRendererEnabled(stationRange, false);

        GameObject connectionObject = new GameObject("SenderUplinkLine");
        LineRenderer connectionRenderer = connectionObject.AddComponent<LineRenderer>();
        ConfigureLineRenderer(connectionRenderer);
        connectionRenderer.material = lineMaterial;
        ConnectionLine connectionLine = connectionObject.AddComponent<ConnectionLine>();
        connectionLine.satellite = sharedSatellite.transform;
        connectionLine.groundStation = groundStationObject.transform;

        GameObject scanObject = new GameObject("DestinationDownlinkBeam");
        LineRenderer scanRenderer = scanObject.AddComponent<LineRenderer>();
        ConfigureLineRenderer(scanRenderer);
        scanRenderer.positionCount = 2;
        scanRenderer.widthMultiplier = 0.026f;
        scanRenderer.material = lineMaterial;
        scanRenderer.enabled = false;

        GameObject targetHalo = CreateSurfaceDisc("SelectedDestinationFootprint", newYork.transform.position, 0.34f, targetHaloMaterial);
        SetRendererEnabled(targetHalo, false);
        MissionVisualController missionVisuals = managers.AddComponent<MissionVisualController>();
        missionVisuals.missionManager = missionManager;
        missionVisuals.satelliteOrbit = satelliteOrbit;
        missionVisuals.scanLine = scanRenderer;
        missionVisuals.targetHaloRenderer = targetHalo.GetComponent<Renderer>();
        missionVisuals.stationRangeRenderer = stationRange.GetComponent<Renderer>();
        missionVisuals.idleTargetMaterial = targetHaloMaterial;
        missionVisuals.activeTargetMaterial = targetActiveHaloMaterial;
        missionVisuals.stationIdleMaterial = stationRangeMaterial;
        missionVisuals.stationActiveMaterial = stationActiveRangeMaterial;
        missionVisuals.showOrbitHighlight = false;

        missionManager.satelliteManager = satelliteManager;
        missionManager.simulationManager = simulationManager;
        missionManager.earth = earth.transform;
        missionManager.satelliteOrbit = satelliteOrbit;
        missionManager.groundStation = groundStation;
        missionManager.connectionLine = connectionLine;
        missionManager.Targets.Add(newYork);
        missionManager.Targets.Add(london);
        missionManager.Targets.Add(seoul);
        missionManager.Targets.Add(tokyo);
        missionManager.Targets.Add(paris);
        missionManager.Targets.Add(sydney);
        missionManager.Targets.Add(losAngeles);

        cameraManager.targetCamera = camera;
        cameraManager.earth = earth.transform;
        cameraManager.satellite = sharedSatellite.transform;
        cameraManager.missionTarget = newYork.transform;
        cameraManager.groundStation = groundStationObject.transform;
        cameraManager.SetOverview();
        ConfigureTargetClick(seoul, missionManager, cameraManager);
        ConfigureTargetClick(newYork, missionManager, cameraManager);
        ConfigureTargetClick(london, missionManager, cameraManager);
        ConfigureTargetClick(tokyo, missionManager, cameraManager);
        ConfigureTargetClick(paris, missionManager, cameraManager);
        ConfigureTargetClick(sydney, missionManager, cameraManager);
        ConfigureTargetClick(losAngeles, missionManager, cameraManager);

        focusController.Initialize(satelliteManager, cameraManager, focusHaloMaterial, missionVisuals);
        if (useVirtualDemoSatellites)
        {
            satelliteManager.RegisterSatelliteTransform("SAT-P01", personalSatellite.transform);
            satelliteManager.RegisterSatelliteTransform("SAT-C01", companySatellite.transform);
            satelliteManager.RegisterSatelliteTransform("SAT-S01", sharedSatellite.transform);
        }
        satelliteManager.RegisterTargetTransform(seoul.targetName, seoul.transform);
        satelliteManager.RegisterTargetTransform(newYork.targetName, newYork.transform);
        satelliteManager.RegisterTargetTransform(london.targetName, london.transform);
        satelliteManager.RegisterTargetTransform(tokyo.targetName, tokyo.transform);
        satelliteManager.RegisterTargetTransform(paris.targetName, paris.transform);
        satelliteManager.RegisterTargetTransform(sydney.targetName, sydney.transform);
        satelliteManager.RegisterTargetTransform(losAngeles.targetName, losAngeles.transform);
        if (useVirtualDemoSatellites)
        {
            focusController.RegisterSatellite("SAT-P01", personalSatellite.transform);
            focusController.RegisterSatellite("SAT-C01", companySatellite.transform);
            focusController.RegisterSatellite("SAT-S01", sharedSatellite.transform);
        }
        missionVisuals.SetSelectedSatelliteTransform(sharedSatellite.transform);
        for (int i = 0; i < relaySatellites.Count; i++)
        {
            satelliteManager.RegisterSatelliteTransform(relaySatellites[i].name, relaySatellites[i].transform);
            focusController.RegisterSatellite(relaySatellites[i].name, relaySatellites[i].transform);
        }
        realSatelliteVisualizer.focusController = focusController;
        realSatelliteVisualizer.cameraManager = cameraManager;
        realSatelliteVisualizer.missionVisualController = missionVisuals;

        presentationDirector.Attach(missionManager, cameraManager);
        audioFeedbackManager.Attach(missionManager);

        uiManager.BuildUI(simulationManager, satelliteManager, missionManager, cameraManager);
        immediateUIManager.simulationManager = simulationManager;
        immediateUIManager.satelliteManager = satelliteManager;
        immediateUIManager.missionManager = missionManager;
        immediateUIManager.cameraManager = cameraManager;
    }

    private Camera CreateCamera()
    {
        Camera camera = Camera.main;
        if (camera == null)
        {
            GameObject cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            camera = cameraObject.GetComponent<Camera>();
            cameraObject.tag = "MainCamera";
        }

        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.005f, 0.01f, 0.025f);
        camera.fieldOfView = 52f;
        camera.transform.position = new Vector3(0f, 6.7f, -13.2f);
        camera.transform.LookAt(Vector3.zero);
        return camera;
    }

    private void CreateLight()
    {
        RenderSettings.ambientLight = new Color(0.025f, 0.035f, 0.06f);
        GameObject lightObject = new GameObject("Directional Light", typeof(Light));
        Light light = lightObject.GetComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.35f;
        light.shadows = LightShadows.None;
        lightObject.transform.rotation = Quaternion.LookRotation(-SunDirection, Vector3.up);
    }

    private void CreateStarfield()
    {
        GameObject root = new GameObject("ProceduralStarfield");
        Material starMaterial = CreateEmissiveMaterial("Star White", new Color(0.8f, 0.9f, 1f), 1.4f);
        Random.InitState(3105);
        for (int i = 0; i < 180; i++)
        {
            GameObject star = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            star.name = "Star";
            star.transform.SetParent(root.transform, false);
            star.transform.position = Random.onUnitSphere * Random.Range(24f, 42f);
            float size = Random.Range(0.025f, 0.075f);
            star.transform.localScale = Vector3.one * size;
            star.GetComponent<Renderer>().material = starMaterial;
            Destroy(star.GetComponent<Collider>());
        }
    }

    private GameObject CreateSatelliteVisual(string objectName, Transform parent, Material bodyMaterial, Material panelMaterial, GameObject importedPrefab)
    {
        if (importedPrefab != null)
        {
            GameObject imported = Instantiate(importedPrefab, parent);
            imported.name = objectName;
            imported.transform.localScale = Vector3.one * 0.38f;
            return imported;
        }

        GameObject root = new GameObject(objectName);
        root.transform.SetParent(parent, false);

        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
        body.name = "Body";
        body.transform.SetParent(root.transform, false);
        body.transform.localScale = new Vector3(0.34f, 0.28f, 0.44f);
        body.GetComponent<Renderer>().material = bodyMaterial;

        GameObject leftPanel = GameObject.CreatePrimitive(PrimitiveType.Cube);
        leftPanel.name = "Left Solar Panel";
        leftPanel.transform.SetParent(root.transform, false);
        leftPanel.transform.localPosition = new Vector3(-0.5f, 0f, 0f);
        leftPanel.transform.localScale = new Vector3(0.62f, 0.025f, 0.34f);
        leftPanel.GetComponent<Renderer>().material = panelMaterial;

        GameObject rightPanel = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rightPanel.name = "Right Solar Panel";
        rightPanel.transform.SetParent(root.transform, false);
        rightPanel.transform.localPosition = new Vector3(0.5f, 0f, 0f);
        rightPanel.transform.localScale = new Vector3(0.62f, 0.025f, 0.34f);
        rightPanel.GetComponent<Renderer>().material = panelMaterial;

        GameObject antenna = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        antenna.name = "Compact Antenna";
        antenna.transform.SetParent(root.transform, false);
        antenna.transform.localPosition = new Vector3(0f, 0.23f, 0f);
        antenna.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        antenna.transform.localScale = new Vector3(0.025f, 0.12f, 0.025f);
        antenna.GetComponent<Renderer>().material = bodyMaterial;
        Destroy(antenna.GetComponent<Collider>());

        return root;
    }

    private GameObject CreateRouteFallbackAnchor(string objectName, Transform parent)
    {
        GameObject anchor = new GameObject(objectName);
        anchor.transform.SetParent(parent, false);
        anchor.transform.position = new Vector3(0f, 0f, 7f);
        anchor.hideFlags = HideFlags.DontSave;
        return anchor;
    }

    private GameObject CreateStaticSatellite(string objectName, Transform parent, Material bodyMaterial, Material panelMaterial, GameObject importedPrefab, Vector3 position)
    {
        GameObject satellite = CreateSatelliteVisual(objectName, parent, bodyMaterial, panelMaterial, importedPrefab);
        satellite.transform.position = position;
        satellite.transform.LookAt(Vector3.zero);
        satellite.transform.localScale = Vector3.one * 0.72f;
        return satellite;
    }

    private List<GameObject> CreateRelayConstellation(Transform parent, Material bodyMaterial, Material panelMaterial, GameObject importedPrefab, SimulationManager simulationManager)
    {
        List<GameObject> satellites = new List<GameObject>();
        for (int i = 4; i <= 24; i++)
        {
            float radius = i % 3 == 0 ? 6.3f : i % 3 == 1 ? 7.25f : 8.2f;
            float inclination = i % 3 == 0 ? -32f : i % 3 == 1 ? 5f : 32f;
            float angle = i * 360f / 21f;
            Vector3 position = Quaternion.Euler(inclination, 0f, 0f) * new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * radius, 0f, Mathf.Sin(angle * Mathf.Deg2Rad) * radius);

            GameObject satellite = CreateStaticSatellite("SAT-R" + i.ToString("00"), parent, bodyMaterial, panelMaterial, importedPrefab, position);
            satellite.transform.localScale = Vector3.one * 0.64f;

            EnterpriseSatelliteAnimator animator = satellite.AddComponent<EnterpriseSatelliteAnimator>();
            animator.simulationManager = simulationManager;
            animator.radius = radius;
            animator.inclination = inclination;
            animator.angle = angle;
            animator.speed = 5.6f + (i % 3) * 0.8f;
            satellites.Add(satellite);
        }
        return satellites;
    }

    private GameObject CreateSpaceStation(string objectName, Transform parent, Material hullMaterial, Material accentMaterial, GameObject importedPrefab)
    {
        if (importedPrefab != null)
        {
            GameObject imported = Instantiate(importedPrefab, parent);
            imported.name = objectName;
            imported.transform.localScale = Vector3.one * 0.018f;
            return imported;
        }

        GameObject root = new GameObject(objectName);
        root.transform.SetParent(parent, false);

        GameObject core = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        core.name = "Station Core";
        core.transform.SetParent(root.transform, false);
        core.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        core.transform.localScale = new Vector3(0.24f, 0.78f, 0.24f);
        core.GetComponent<Renderer>().material = hullMaterial;

        GameObject ring = new GameObject("Station Ring");
        ring.transform.SetParent(root.transform, false);
        LineRenderer ringLine = ring.AddComponent<LineRenderer>();
        ConfigureLineRenderer(ringLine);
        ringLine.material = accentMaterial;
        ringLine.positionCount = 65;
        ringLine.useWorldSpace = false;
        ringLine.widthMultiplier = 0.035f;
        for (int i = 0; i < ringLine.positionCount; i++)
        {
            float angle = (i / (float)(ringLine.positionCount - 1)) * Mathf.PI * 2f;
            ringLine.SetPosition(i, new Vector3(Mathf.Cos(angle) * 0.72f, Mathf.Sin(angle) * 0.72f, 0f));
        }

        for (int i = 0; i < 4; i++)
        {
            GameObject truss = GameObject.CreatePrimitive(PrimitiveType.Cube);
            truss.name = "Station Truss";
            truss.transform.SetParent(root.transform, false);
            truss.transform.localRotation = Quaternion.Euler(0f, 0f, i * 45f);
            truss.transform.localScale = new Vector3(1.55f, 0.045f, 0.045f);
            truss.GetComponent<Renderer>().material = hullMaterial;
        }

        GameObject dock = GameObject.CreatePrimitive(PrimitiveType.Cube);
        dock.name = "Shared Satellite Dock";
        dock.transform.SetParent(root.transform, false);
        dock.transform.localPosition = new Vector3(0f, 0f, 0.64f);
        dock.transform.localScale = new Vector3(0.46f, 0.18f, 0.2f);
        dock.GetComponent<Renderer>().material = accentMaterial;

        SpaceStationAnimator animator = root.AddComponent<SpaceStationAnimator>();
        animator.rotatingRing = ring.transform;
        AddWorldLabel("오비트셰어 정거장", new Vector3(0f, 8.2f, 0f), 0.26f);
        return root;
    }

    private ObservationTarget CreateTarget(string objectName, string displayName, Transform parent, Material normalMaterial, Material selectedMaterial, float latitude, float longitude, float dataMb, float resolution, int costCredits, string priority, float weatherRisk)
    {
        GameObject target = new GameObject(objectName);
        target.transform.SetParent(parent, false);
        target.transform.position = SurfacePosition(latitude, longitude, 3.14f);

        GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        marker.name = "Marker";
        marker.transform.SetParent(target.transform, false);
        marker.transform.localPosition = target.transform.position.normalized * 0.03f;
        marker.transform.localScale = Vector3.one * 0.018f;

        ObservationTarget observationTarget = target.AddComponent<ObservationTarget>();
        observationTarget.Initialize(displayName, normalMaterial, selectedMaterial, dataMb, resolution, costCredits, priority, weatherRisk);
        DestinationClickTarget clickTarget = marker.AddComponent<DestinationClickTarget>();
        clickTarget.target = observationTarget;
        PulseScale pulse = marker.AddComponent<PulseScale>();
        pulse.amplitude = 0.006f;
        pulse.speed = 2.6f;
        Vector3 surfaceNormal = target.transform.position.normalized;
        Vector3 labelPosition = target.transform.position + surfaceNormal * 0.14f;
        if (displayName == OrbitShareDestinations.Seoul)
        {
            labelPosition += surfaceNormal * 0.02f;
        }
        else if (displayName == OrbitShareDestinations.NewYork)
        {
            labelPosition += surfaceNormal * 0.03f;
        }
        TextMesh label = AddWorldLabel(DestinationLabel(displayName), labelPosition, 0.22f);
        observationTarget.SetLabel(label);
        DestinationPinVisual pinVisual = target.AddComponent<DestinationPinVisual>();
        pinVisual.Initialize(observationTarget, label, marker.transform);
        return observationTarget;
    }

    private void ConfigureTargetClick(ObservationTarget target, MissionManager missionManager, CameraManager cameraManager)
    {
        if (target == null)
        {
            return;
        }

        DestinationClickTarget clickTarget = target.GetComponentInChildren<DestinationClickTarget>();
        if (clickTarget != null)
        {
            clickTarget.missionManager = missionManager;
            clickTarget.cameraManager = cameraManager;
        }

        DestinationPinVisual pinVisual = target.GetComponent<DestinationPinVisual>();
        if (pinVisual != null)
        {
            pinVisual.SetMissionManager(missionManager);
        }
    }

    private GameObject CreateGroundStation(Material material, float latitude, float longitude)
    {
        GameObject station = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        station.name = "GroundStation";
        station.transform.position = SurfacePosition(latitude, longitude, 3.14f);
        station.transform.localScale = Vector3.one * 0.026f;
        station.GetComponent<Renderer>().material = material;
        PulseScale pulse = station.AddComponent<PulseScale>();
        pulse.amplitude = 0.006f;
        pulse.speed = 2.2f;
        return station;
    }

    private void CreateAtmosphere(Transform earth, Material atmosphereMaterial, bool hasExternalEarthTextures)
    {
        GameObject atmosphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        atmosphere.name = "Atmosphere";
        atmosphere.transform.SetParent(earth, false);
        atmosphere.transform.localPosition = Vector3.zero;
        atmosphere.transform.localScale = Vector3.one * (hasExternalEarthTextures ? 1.072f : 1.085f);
        atmosphere.GetComponent<Renderer>().material = atmosphereMaterial;
        Destroy(atmosphere.GetComponent<Collider>());
    }

    private void CreateCloudLayer(Transform earth, Material cloudMaterial, SimulationManager simulationManager)
    {
        CreateSingleCloudLayer("Cloud Layer", earth, cloudMaterial, simulationManager, 1.032f, 0.22f);

        Material highCloudMaterial = CreateCloudMaterial(0.18f);
        CreateSingleCloudLayer("High Cloud Layer", earth, highCloudMaterial, simulationManager, 1.041f, -0.12f);
    }

    private void CreateSingleCloudLayer(string objectName, Transform earth, Material cloudMaterial, SimulationManager simulationManager, float scale, float rotationSpeed)
    {
        GameObject clouds = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        clouds.name = objectName;
        clouds.transform.SetParent(earth, false);
        clouds.transform.localPosition = Vector3.zero;
        clouds.transform.localScale = Vector3.one * scale;
        clouds.GetComponent<Renderer>().material = cloudMaterial;
        EarthRotation rotation = clouds.AddComponent<EarthRotation>();
        rotation.simulationManager = simulationManager;
        rotation.rotationSpeed = rotationSpeed;
        Destroy(clouds.GetComponent<Collider>());
    }

    private void CreateNightLightsLayer(Transform earth, Material nightLightsMaterial)
    {
        GameObject lights = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        lights.name = "Night City Lights";
        lights.transform.SetParent(earth, false);
        lights.transform.localPosition = Vector3.zero;
        lights.transform.localScale = Vector3.one * 1.019f;
        lights.GetComponent<Renderer>().material = nightLightsMaterial;
        Destroy(lights.GetComponent<Collider>());
    }


    private void CreateTerminatorBand(Transform earth)
    {
        GameObject band = new GameObject("Soft Terminator Band");
        band.transform.SetParent(earth, false);
        band.transform.localPosition = Vector3.zero;
        band.transform.localRotation = Quaternion.FromToRotation(Vector3.up, SunDirection);

        LineRenderer line = band.AddComponent<LineRenderer>();
        ConfigureLineRenderer(line);
        line.material = CreateLineMaterial("Terminator Soft Cyan Edge", new Color(0.32f, 0.68f, 1f, 0.12f), true);
        line.positionCount = 193;
        line.loop = true;
        line.useWorldSpace = false;
        line.widthMultiplier = 0.018f;
        line.colorGradient = CreateLineGradient(new Color(0.32f, 0.68f, 1f, 0.12f));

        float radius = 0.512f;
        for (int i = 0; i < line.positionCount; i++)
        {
            float angle = (i / (float)(line.positionCount - 1)) * Mathf.PI * 2f;
            line.SetPosition(i, new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius));
        }
    }

    private GameObject CreateSurfaceDisc(string objectName, Vector3 surfacePosition, float diameter, Material material)
    {
        GameObject ring = new GameObject(objectName);
        Vector3 normal = surfacePosition.normalized;
        ring.transform.position = normal * 3.14f;
        ring.transform.rotation = Quaternion.FromToRotation(Vector3.up, normal);

        LineRenderer line = ring.AddComponent<LineRenderer>();
        ConfigureLineRenderer(line);
        line.material = material;
        line.positionCount = 112;
        line.loop = true;
        line.useWorldSpace = false;
        line.widthMultiplier = objectName.Contains("Destination") ? 0.007f : 0.004f;
        line.numCapVertices = 4;
        line.numCornerVertices = 4;

        float radius = diameter * 0.5f;
        for (int i = 0; i < line.positionCount; i++)
        {
            float angle = (i / (float)line.positionCount) * Mathf.PI * 2f;
            line.SetPosition(i, new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius));
        }

        PulseScale pulse = ring.AddComponent<PulseScale>();
        pulse.amplitude = objectName.Contains("Destination") ? 0.035f : 0.018f;
        pulse.speed = objectName.Contains("Destination") ? 1.45f : 0.9f;
        return ring;
    }

    private void SetRendererEnabled(GameObject target, bool enabled)
    {
        Renderer renderer = target != null ? target.GetComponent<Renderer>() : null;
        if (renderer != null)
        {
            renderer.enabled = enabled;
        }
    }

    private void AddLandPatch(Transform earth, Material material, float latitude, float longitude, Vector3 scale)
    {
        GameObject patch = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        patch.name = "Land Patch";
        patch.transform.SetParent(earth, true);
        patch.transform.position = SurfacePosition(latitude, longitude, 3.03f);
        patch.transform.localScale = scale;
        patch.transform.up = patch.transform.position.normalized;
        patch.GetComponent<Renderer>().material = material;
    }

    private void ConfigureLineRenderer(LineRenderer line)
    {
        line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        line.receiveShadows = false;
        line.alignment = LineAlignment.View;
        line.textureMode = LineTextureMode.Stretch;
        line.numCapVertices = Mathf.Max(line.numCapVertices, 8);
        line.numCornerVertices = Mathf.Max(line.numCornerVertices, 8);
    }

    private void CreateStaticOrbitRing(string objectName, Transform parent, float radius, float inclination, Material material)
    {
        GameObject ring = new GameObject(objectName);
        ring.transform.SetParent(parent, false);
        LineRenderer line = ring.AddComponent<LineRenderer>();
        ConfigureLineRenderer(line);
        line.material = material;
        line.positionCount = 361;
        line.loop = true;
        line.useWorldSpace = true;
        line.widthMultiplier = 0.006f;
        line.colorGradient = CreateLineGradient(new Color(0.92f, 0.98f, 1f, 0.11f));

        Quaternion tilt = Quaternion.Euler(inclination, 0f, 0f);
        for (int i = 0; i < line.positionCount; i++)
        {
            float angle = (i / (float)(line.positionCount - 1)) * Mathf.PI * 2f;
            Vector3 point = new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
            line.SetPosition(i, tilt * point);
        }
    }

    private TextMesh AddWorldLabel(string text, Vector3 position, float size)
    {
        GameObject labelObject = new GameObject(text + " Label", typeof(TextMesh));
        labelObject.transform.position = position;
        if (Camera.main != null)
        {
            labelObject.transform.LookAt(Camera.main.transform);
        }
        TextMesh textMesh = labelObject.GetComponent<TextMesh>();
        textMesh.text = text;
        textMesh.fontSize = 54;
        textMesh.characterSize = size * 0.115f;
        textMesh.fontStyle = FontStyle.Normal;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.color = new Color(0.88f, 0.96f, 1f, 0.66f);
        labelObject.AddComponent<BillboardLabel>();
        return textMesh;
    }

    private Vector3 SurfacePosition(float latitude, float longitude, float radius)
    {
        float lat = latitude * Mathf.Deg2Rad;
        float lon = longitude * Mathf.Deg2Rad;
        return new Vector3(
            Mathf.Cos(lat) * Mathf.Cos(lon) * radius,
            Mathf.Sin(lat) * radius,
            Mathf.Cos(lat) * Mathf.Sin(lon) * radius);
    }

    private Material CreateEarthMaterial()
    {
        Texture2D importedEarthDay = LoadEarthTexture(EarthDayTexturePath);
        Shader earthShader = FindOrbitShareShader("EarthDayNight", "OrbitShare/EarthDayNight");
        Material material = earthShader != null
            ? new Material(earthShader)
            : CreateMaterial("Earth Ocean", new Color(0.025f, 0.16f, 0.34f));
        material.name = importedEarthDay != null ? "SSS Earth" : "Earth Blue Marble";

        ApplyEarthTextures(material);
        return material;
    }

    private void ApplyEarthTextures(Material material)
    {
        Texture2D dayTexture = LoadEarthTexture(EarthDayTexturePath);
        Texture2D nightTexture = LoadEarthTexture(EarthNightTexturePath);
        Texture2D cloudsTexture = LoadEarthTexture(EarthCloudsTexturePath);
        Texture2D normalTexture = LoadEarthTexture(EarthNormalTexturePath);
        Texture2D specularTexture = LoadEarthTexture(EarthSpecularTexturePath);

        SetTextureIfHas(material, "_MainTex", dayTexture != null ? dayTexture : CreateEarthTexture());
        SetTextureIfHas(material, "_NightTex", nightTexture != null ? nightTexture : CreateNightLightsTexture());
        SetTextureIfHas(material, "_NormalTex", normalTexture);
        SetTextureIfHas(material, "_BumpMap", normalTexture);
        SetTextureIfHas(material, "_SpecularTex", specularTexture);
        SetTextureIfHas(material, "_SpecGlossMap", specularTexture);
        SetTextureIfHas(material, "_SpecMap", specularTexture);

        SetVectorIfHas(material, "_SunDirection", new Vector4(SunDirection.x, SunDirection.y, SunDirection.z, 0f));
        SetColorIfHas(material, "_AtmosphereColor", new Color(0.3f, 0.84f, 1f, 1f));
        SetColorIfHas(material, "_OceanSpecColor", new Color(0.68f, 0.96f, 1f, 1f));
        SetFloatIfHas(material, "_SpecStrength", specularTexture != null ? 0.42f : 0.34f);
        SetFloatIfHas(material, "_SpecularStrength", specularTexture != null ? 0.42f : 0.34f);
        SetFloatIfHas(material, "_Glossiness", specularTexture != null ? 0.42f : 0.34f);
        SetFloatIfHas(material, "_NormalStrength", normalTexture != null ? 0.62f : 0f);
        SetFloatIfHas(material, "_BumpScale", normalTexture != null ? 0.62f : 1f);
        SetFloatIfHas(material, "_DaySaturation", dayTexture != null ? 1.24f : 1.08f);
        SetFloatIfHas(material, "_NightIntensity", nightTexture != null ? 0.72f : 0.9f);
        SetFloatIfHas(material, "_NightStrength", nightTexture != null ? 0.72f : 0.9f);
        SetFloatIfHas(material, "_CloudStrength", cloudsTexture != null ? 0.28f : 0f);
        SetFloatIfHas(material, "_Metallic", 0f);

        UpdateEarthTextureStatus(dayTexture, nightTexture, cloudsTexture, normalTexture, specularTexture);
    }

    private Material CreateAtmosphereMaterial(bool hasExternalEarthTextures)
    {
        Color rimColor = hasExternalEarthTextures ? new Color(0.26f, 0.62f, 1f, 1f) : new Color(0.36f, 0.82f, 1f, 1f);
        float alpha = hasExternalEarthTextures ? 0.24f : 0.32f;
        Shader atmosphereShader = FindOrbitShareShader("AtmosphereRim", "OrbitShare/AtmosphereRim");
        if (atmosphereShader == null)
        {
            return CreateTransparentMaterial("Atmosphere", new Color(rimColor.r, rimColor.g, rimColor.b, alpha * 0.38f));
        }

        Material material = new Material(atmosphereShader);
        material.name = "Atmosphere Rim";
        SetColorIfHas(material, "_RimColor", rimColor);
        SetVectorIfHas(material, "_SunDirection", new Vector4(SunDirection.x, SunDirection.y, SunDirection.z, 0f));
        SetFloatIfHas(material, "_RimPower", hasExternalEarthTextures ? 2.85f : 2.55f);
        SetFloatIfHas(material, "_Alpha", alpha);
        material.renderQueue = 3100;
        return material;
    }

    private Shader FindOrbitShareShader(string resourceName, string shaderName)
    {
        Shader shader = Resources.Load<Shader>("OrbitShare/" + resourceName);
        return shader != null ? shader : Shader.Find(shaderName);
    }

    private Material CreateMaterial(string materialName, Color color)
    {
        Material material = new Material(FindRuntimeShader());
        material.name = materialName;
        material.color = color;
        return material;
    }

    private Shader FindRuntimeShader()
    {
        string[] shaderNames =
        {
            "Standard",
            "Universal Render Pipeline/Lit",
            "Legacy Shaders/Diffuse",
            "Unlit/Color",
            "Sprites/Default",
            "UI/Default"
        };

        foreach (string shaderName in shaderNames)
        {
            Shader shader = Shader.Find(shaderName);
            if (shader != null)
            {
                return shader;
            }
        }

        return Shader.Find("Hidden/InternalErrorShader");
    }

    private Material CreateEmissiveMaterial(string materialName, Color color, float intensity)
    {
        Material material = CreateMaterial(materialName, color);
        material.EnableKeyword("_EMISSION");
        material.SetColor("_EmissionColor", color * intensity);
        return material;
    }

    private Material CreateTransparentMaterial(string materialName, Color color)
    {
        Material material = CreateMaterial(materialName, color);
        material.SetFloat("_Mode", 3f);
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 3000;
        return material;
    }

    private Material CreateCloudMaterial(float alpha)
    {
        Texture2D importedClouds = LoadEarthTexture(EarthCloudsTexturePath);
        Shader shader = importedClouds != null
            ? FindOrbitShareShader("EarthClouds", "OrbitShare/EarthClouds")
            : Shader.Find("Unlit/Transparent");
        Material material = new Material(shader != null ? shader : FindRuntimeShader());
        material.name = importedClouds != null ? "SSS Cloud Layer" : "High Altitude Cloud Layer";
        material.mainTexture = importedClouds != null ? importedClouds : CreateCloudTexture();
        material.color = new Color(0.94f, 0.98f, 1f, alpha);
        SetTextureIfHas(material, "_MainTex", material.mainTexture);
        SetVectorIfHas(material, "_SunDirection", new Vector4(SunDirection.x, SunDirection.y, SunDirection.z, 0f));
        SetColorIfHas(material, "_CloudColor", new Color(1f, 1f, 1f, Mathf.Clamp01(alpha)));
        SetColorIfHas(material, "_ShadowColor", new Color(0.22f, 0.34f, 0.46f, 1f));
        SetFloatIfHas(material, "_Alpha", importedClouds != null ? alpha * 0.78f : alpha);
        SetFloatIfHas(material, "_AlphaCutoff", importedClouds != null ? 0.16f : 0.05f);
        SetFloatIfHas(material, "_SunSideStrength", importedClouds != null ? 0.82f : 0.65f);
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.renderQueue = 3050;
        return material;
    }

    private Material CreateNightLightsMaterial(bool hasExternalNightTexture)
    {
        Texture2D importedNight = hasExternalNightTexture ? LoadEarthTexture(EarthNightTexturePath) : null;
        Shader shader = Shader.Find("Unlit/Transparent");
        Material material = new Material(shader != null ? shader : FindRuntimeShader());
        material.name = importedNight != null ? "Subtle External Night City Lights" : "Procedural Night City Lights";
        material.mainTexture = importedNight != null ? importedNight : CreateNightLightsTexture();
        material.color = importedNight != null ? new Color(0.5f, 0.72f, 1f, 0.16f) : new Color(0.62f, 0.82f, 1f, 0.54f);
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
        material.SetInt("_ZWrite", 0);
        material.renderQueue = 3040;
        return material;
    }

    private Material CreateLineMaterial(string materialName, Color color, bool additive)
    {
        Shader shader = Shader.Find("Sprites/Default");
        if (shader == null)
        {
            shader = Shader.Find(additive ? "Legacy Shaders/Particles/Additive" : "Unlit/Color");
        }
        Material material = new Material(shader != null ? shader : FindRuntimeShader());
        material.name = materialName;
        material.color = color;
        if (material.HasProperty("_Color"))
        {
            material.SetColor("_Color", color);
        }
        if (material.HasProperty("_TintColor"))
        {
            material.SetColor("_TintColor", color);
        }
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", additive ? (int)UnityEngine.Rendering.BlendMode.One : (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.renderQueue = additive ? 3100 : 3000;
        return material;
    }

    private Texture2D LoadEarthTexture(string path)
    {
        Texture2D fallback4K = Resources.Load<Texture2D>(path + "_4K");
        return fallback4K != null ? fallback4K : Resources.Load<Texture2D>(path);
    }

    private bool HasExternalEarthTextures()
    {
        return LoadEarthTexture(EarthDayTexturePath) != null
            || LoadEarthTexture(EarthNightTexturePath) != null
            || LoadEarthTexture(EarthCloudsTexturePath) != null
            || LoadEarthTexture(EarthNormalTexturePath) != null
            || LoadEarthTexture(EarthSpecularTexturePath) != null;
    }

    private void UpdateEarthTextureStatus(Texture2D dayTexture, Texture2D nightTexture, Texture2D cloudsTexture, Texture2D normalTexture, Texture2D specularTexture)
    {
        EarthTextureStatus = "Earth textures: "
            + "day=" + DescribeEarthTexture(dayTexture, "procedural")
            + ", night=" + DescribeEarthTexture(nightTexture, "procedural")
            + ", clouds=" + DescribeEarthTexture(cloudsTexture, "procedural")
            + ", normal=" + DescribeEarthTexture(normalTexture, "none")
            + ", specular=" + DescribeEarthTexture(specularTexture, "none");
    }

    private string DescribeEarthTexture(Texture2D texture, string missingStatus)
    {
        if (texture == null)
        {
            return missingStatus;
        }

        string size = texture.width <= 4096 ? "4K" : "8K";
        return size + "(" + texture.name + ")";
    }

    private void SetTextureIfHas(Material material, string propertyName, Texture texture)
    {
        if (texture != null && material.HasProperty(propertyName))
        {
            material.SetTexture(propertyName, texture);
        }
    }

    private void SetColorIfHas(Material material, string propertyName, Color color)
    {
        if (material.HasProperty(propertyName))
        {
            material.SetColor(propertyName, color);
        }
    }

    private void SetVectorIfHas(Material material, string propertyName, Vector4 value)
    {
        if (material.HasProperty(propertyName))
        {
            material.SetVector(propertyName, value);
        }
    }

    private void SetFloatIfHas(Material material, string propertyName, float value)
    {
        if (material.HasProperty(propertyName))
        {
            material.SetFloat(propertyName, value);
        }
    }

    private Gradient CreateLineGradient(Color color)
    {
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new[]
            {
                new GradientColorKey(color, 0f),
                new GradientColorKey(Color.white, 0.5f),
                new GradientColorKey(color, 1f)
            },
            new[]
            {
                new GradientAlphaKey(color.a * 0.62f, 0f),
                new GradientAlphaKey(color.a, 0.5f),
                new GradientAlphaKey(color.a * 0.62f, 1f)
            });
        return gradient;
    }

    private Texture2D CreateEarthTexture()
    {
        Texture2D texture = new Texture2D(2048, 1024, TextureFormat.RGBA32, true);
        texture.filterMode = FilterMode.Trilinear;
        texture.wrapMode = TextureWrapMode.Repeat;
        Color deepOcean = new Color(0.004f, 0.035f, 0.12f);
        Color ocean = new Color(0.008f, 0.12f, 0.28f);
        Color shallow = new Color(0.015f, 0.30f, 0.42f);
        Color coastFoam = new Color(0.2f, 0.48f, 0.48f);
        Color land = new Color(0.08f, 0.28f, 0.17f);
        Color highLand = new Color(0.25f, 0.39f, 0.22f);
        Color dryLand = new Color(0.34f, 0.29f, 0.18f);
        Color ice = new Color(0.75f, 0.88f, 0.94f);

        for (int y = 0; y < texture.height; y++)
        {
            float v = y / (float)(texture.height - 1);
            float latitude = Mathf.Lerp(-90f, 90f, v);
            float polar = Mathf.Abs(v * 2f - 1f);
            for (int x = 0; x < texture.width; x++)
            {
                float u = x / (float)(texture.width - 1);
                float longitude = Mathf.Lerp(-180f, 180f, u);
                float nx = u * 10.5f;
                float ny = v * 7.2f;
                float shelfNoise = Mathf.PerlinNoise(nx * 2.0f + 12.4f, ny * 2.0f) - 0.5f;
                float terrainNoise = Mathf.PerlinNoise(nx * 4.5f + 4.1f, ny * 3.6f + 9.3f);
                float fineTerrain = Mathf.PerlinNoise(nx * 12.0f + 1.8f, ny * 10.0f + 2.5f);
                float oceanDetail = Mathf.PerlinNoise(nx * 2.6f, ny * 2.2f);
                float landMask = ContinentMask(latitude, longitude) + shelfNoise * 0.15f;
                float coast = Mathf.SmoothStep(0.04f, 0.18f, landMask);
                float landPresence = Mathf.SmoothStep(0.16f, 0.27f, landMask);

                Color color = Color.Lerp(deepOcean, ocean, 0.25f + oceanDetail * 0.42f);
                color = Color.Lerp(color, shallow, Mathf.SmoothStep(-0.02f, 0.12f, landMask) * 0.55f);
                color = Color.Lerp(color, coastFoam, Mathf.SmoothStep(0.08f, 0.15f, landMask) * (1f - coast) * 0.35f);

                if (landPresence > 0.01f && polar < 0.94f)
                {
                    bool dryBand = Mathf.Abs(latitude) > 13f && Mathf.Abs(latitude) < 35f;
                    Color baseLand = dryBand ? Color.Lerp(land, dryLand, 0.58f) : land;
                    Color terrainColor = Color.Lerp(baseLand, highLand, terrainNoise * 0.72f);
                    terrainColor = Color.Lerp(terrainColor, ice, Mathf.SmoothStep(0.78f, 0.94f, Mathf.Abs(latitude) / 90f) * fineTerrain * 0.45f);
                    color = Color.Lerp(color, terrainColor, landPresence);
                }

                if (polar > 0.86f)
                {
                    color = Color.Lerp(color, ice, Mathf.SmoothStep(0.86f, 0.99f, polar));
                }

                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        return texture;
    }

    private Texture2D CreateNightLightsTexture()
    {
        Texture2D texture = new Texture2D(1024, 512, TextureFormat.RGBA32, true);
        texture.filterMode = FilterMode.Trilinear;
        texture.wrapMode = TextureWrapMode.Repeat;
        texture.SetPixels(new Color[texture.width * texture.height]);

        float[,] cities =
        {
            { 37.56f, 126.97f }, { 35.18f, 129.07f }, { 35.69f, 139.69f },
            { 31.23f, 121.47f }, { 1.35f, 103.82f }, { 28.61f, 77.21f },
            { 51.50f, -0.12f }, { 48.85f, 2.35f }, { 52.52f, 13.40f },
            { 40.71f, -74.00f }, { 34.05f, -118.24f }, { 19.43f, -99.13f },
            { -33.87f, 151.21f }, { -23.55f, -46.63f }, { -34.60f, -58.38f },
            { 30.04f, 31.24f }, { -26.20f, 28.04f }, { 55.75f, 37.62f }
        };

        System.Random random = new System.Random(220522);
        Color coreColor = new Color(0.45f, 0.78f, 1f, 1f);
        Color satelliteColor = new Color(0.18f, 0.48f, 0.9f, 1f);
        for (int i = 0; i < cities.GetLength(0); i++)
        {
            AddNightLightCluster(texture, cities[i, 0], cities[i, 1], 4.2f, coreColor, 0.9f);
            for (int j = 0; j < 4; j++)
            {
                float latitudeOffset = ((float)random.NextDouble() * 2f - 1f) * 2.2f;
                float longitudeOffset = ((float)random.NextDouble() * 2f - 1f) * 3.0f;
                AddNightLightCluster(texture, cities[i, 0] + latitudeOffset, cities[i, 1] + longitudeOffset, 2.0f, satelliteColor, 0.45f);
            }
        }

        texture.Apply();
        return texture;
    }

    private void AddNightLightCluster(Texture2D texture, float latitude, float longitude, float radiusPixels, Color color, float strength)
    {
        int centerX = Mathf.RoundToInt(Mathf.Repeat((longitude + 180f) / 360f, 1f) * (texture.width - 1));
        int centerY = Mathf.RoundToInt(Mathf.Clamp01((latitude + 90f) / 180f) * (texture.height - 1));
        int radius = Mathf.CeilToInt(radiusPixels * 2.0f);

        for (int y = -radius; y <= radius; y++)
        {
            int py = Mathf.Clamp(centerY + y, 0, texture.height - 1);
            for (int x = -radius; x <= radius; x++)
            {
                int px = (centerX + x + texture.width) % texture.width;
                float distance = Mathf.Sqrt(x * x + y * y) / Mathf.Max(1f, radiusPixels);
                if (distance > 2.0f)
                {
                    continue;
                }

                float intensity = Mathf.Exp(-distance * distance * 2.6f) * strength;
                Color existing = texture.GetPixel(px, py);
                texture.SetPixel(
                    px,
                    py,
                    new Color(
                        Mathf.Max(existing.r, color.r * intensity),
                        Mathf.Max(existing.g, color.g * intensity),
                        Mathf.Max(existing.b, color.b * intensity),
                        1f));
            }
        }
    }

    private float ContinentMask(float latitude, float longitude)
    {
        float mask = 0f;
        mask = Mathf.Max(mask, EllipseMask(latitude, longitude, 56f, -103f, 22f, 50f));
        mask = Mathf.Max(mask, EllipseMask(latitude, longitude, 38f, -100f, 26f, 36f));
        mask = Mathf.Max(mask, EllipseMask(latitude, longitude, 18f, -99f, 14f, 22f));
        mask = Mathf.Max(mask, EllipseMask(latitude, longitude, 65f, -42f, 13f, 24f));
        mask = Mathf.Max(mask, EllipseMask(latitude, longitude, 7f, -63f, 38f, 18f));
        mask = Mathf.Max(mask, EllipseMask(latitude, longitude, -22f, -58f, 24f, 15f));
        mask = Mathf.Max(mask, EllipseMask(latitude, longitude, 51f, 11f, 17f, 34f));
        mask = Mathf.Max(mask, EllipseMask(latitude, longitude, 49f, 74f, 29f, 78f));
        mask = Mathf.Max(mask, EllipseMask(latitude, longitude, 22f, 78f, 18f, 43f));
        mask = Mathf.Max(mask, EllipseMask(latitude, longitude, 5f, 21f, 35f, 27f));
        mask = Mathf.Max(mask, EllipseMask(latitude, longitude, -23f, 134f, 15f, 25f));
        mask = Mathf.Max(mask, EllipseMask(latitude, longitude, -42f, 173f, 6f, 7f));
        mask = Mathf.Max(mask, EllipseMask(latitude, longitude, -76f, 0f, 10f, 180f));

        mask -= EllipseMask(latitude, longitude, 28f, -87f, 9f, 17f) * 0.6f;
        mask -= EllipseMask(latitude, longitude, 58f, -80f, 14f, 24f) * 0.35f;
        mask -= EllipseMask(latitude, longitude, 42f, -70f, 8f, 16f) * 0.45f;
        mask -= EllipseMask(latitude, longitude, 44f, 25f, 10f, 17f) * 0.35f;
        mask -= EllipseMask(latitude, longitude, 20f, 42f, 12f, 17f) * 0.42f;
        mask -= EllipseMask(latitude, longitude, 10f, 114f, 16f, 24f) * 0.5f;
        return Mathf.Clamp01(mask);
    }

    private float EllipseMask(float latitude, float longitude, float centerLatitude, float centerLongitude, float latitudeRadius, float longitudeRadius)
    {
        float lonDelta = Mathf.DeltaAngle(centerLongitude, longitude);
        float latDelta = latitude - centerLatitude;
        float distance = (latDelta * latDelta) / (latitudeRadius * latitudeRadius) + (lonDelta * lonDelta) / (longitudeRadius * longitudeRadius);
        return Mathf.Clamp01(1f - distance);
    }

    private Texture2D CreateCloudTexture()
    {
        Texture2D texture = new Texture2D(1024, 512, TextureFormat.RGBA32, true);
        texture.filterMode = FilterMode.Trilinear;
        texture.wrapMode = TextureWrapMode.Repeat;
        for (int y = 0; y < texture.height; y++)
        {
            float v = y / (float)(texture.height - 1);
            float latitude = Mathf.Abs(v * 2f - 1f);
            for (int x = 0; x < texture.width; x++)
            {
                float nx = x * 0.009f;
                float ny = y * 0.014f;
                float broad = Mathf.PerlinNoise(nx, ny);
                float streak = Mathf.PerlinNoise(nx * 1.8f + 18f, ny * 0.65f);
                float fine = Mathf.PerlinNoise(nx * 4.2f + 6f, ny * 2.4f + 9f);
                float band = 1f - Mathf.SmoothStep(0.08f, 0.75f, latitude);
                float cloud = broad * 0.44f + streak * 0.38f + fine * 0.18f;
                float alpha = Mathf.SmoothStep(0.60f, 0.88f, cloud) * Mathf.Lerp(0.05f, 0.28f, band);
                texture.SetPixel(x, y, new Color(0.9f, 0.97f, 1f, alpha));
            }
        }
        texture.Apply();
        return texture;
    }

    private string DestinationLabel(string targetName)
    {
        return OrbitShareDestinations.Label(targetName);
    }
}
