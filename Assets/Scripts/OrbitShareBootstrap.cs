using System.Collections.Generic;
using UnityEngine;

public class OrbitShareBootstrap : MonoBehaviour
{
    private static bool bootstrapped;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetBootstrapState()
    {
        bootstrapped = false;
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
        Material earthMaterial = CreateMaterial("Earth Ocean", new Color(0.05f, 0.28f, 0.62f));
        earthMaterial.mainTexture = CreateEarthTexture();
        Material importedEarthMaterial = Resources.Load<Material>("OrbitShare/PremiumEarthMaterial");
        if (importedEarthMaterial != null)
        {
            earthMaterial = importedEarthMaterial;
        }
        Material satelliteMaterial = CreateEmissiveMaterial("Satellite Silver", new Color(0.88f, 0.92f, 0.96f), 0.45f);
        Material panelMaterial = CreateMaterial("Solar Panel", new Color(0.05f, 0.12f, 0.26f));
        Material orbitMaterial = CreateEmissiveMaterial("Orbit Cyan", new Color(0.25f, 0.78f, 1f), 1.2f);
        Material targetMaterial = CreateEmissiveMaterial("Destination City", new Color(1f, 0.48f, 0.12f), 0.8f);
        Material selectedTargetMaterial = CreateEmissiveMaterial("Selected Destination", new Color(1f, 0.9f, 0.18f), 1.5f);
        Material stationMaterial = CreateEmissiveMaterial("Ground Station Green", new Color(0.2f, 1f, 0.55f), 1f);
        Material lineMaterial = CreateEmissiveMaterial("Connection Line", new Color(0.24f, 1f, 0.84f), 1.6f);
        Material atmosphereMaterial = CreateTransparentMaterial("Atmosphere", new Color(0.2f, 0.65f, 1f, 0.16f));
        Material cloudMaterial = CreateTransparentMaterial("Cloud Layer", new Color(0.9f, 0.96f, 1f, 0.26f));
        cloudMaterial.mainTexture = CreateCloudTexture();
        Material targetHaloMaterial = CreateTransparentMaterial("Signal Footprint", new Color(1f, 0.62f, 0.08f, 0.18f));
        Material targetActiveHaloMaterial = CreateTransparentMaterial("Active Signal Footprint", new Color(1f, 0.92f, 0.18f, 0.22f));
        Material stationRangeMaterial = CreateTransparentMaterial("Communication Range", new Color(0.2f, 0.95f, 0.72f, 0.08f));
        Material stationActiveRangeMaterial = CreateTransparentMaterial("Active Communication Range", new Color(0.24f, 1f, 0.84f, 0.18f));
        Material focusHaloMaterial = CreateTransparentMaterial("Selected Satellite Focus", new Color(1f, 0.82f, 0.18f, 0.22f));
        Material cityLightMaterial = CreateEmissiveMaterial("City Lights", new Color(1f, 0.78f, 0.34f), 1.8f);
        Material secondaryOrbitMaterial = CreateTransparentMaterial("Secondary Orbit Slot", new Color(0.42f, 0.62f, 1f, 0.22f));
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
        managers.AddComponent<AssetStoreIntegrationGuide>();

        GameObject earth = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        earth.name = "Earth";
        earth.transform.position = Vector3.zero;
        earth.transform.localScale = Vector3.one * 6f;
        earth.GetComponent<Renderer>().material = earthMaterial;
        EarthRotation earthRotation = earth.AddComponent<EarthRotation>();
        earthRotation.simulationManager = simulationManager;
        CreateAtmosphere(earth.transform, atmosphereMaterial);
        CreateCloudLayer(earth.transform, cloudMaterial);
        AddCityLights(earth.transform, cityLightMaterial);

        GameObject satellitesRoot = new GameObject("Satellites");
        GameObject sharedSatellite = CreateSatelliteVisual("SAT-S01", satellitesRoot.transform, satelliteMaterial, panelMaterial, importedSatellitePrefab);
        SatelliteOrbit satelliteOrbit = sharedSatellite.AddComponent<SatelliteOrbit>();
        satelliteOrbit.earth = earth.transform;
        satelliteOrbit.simulationManager = simulationManager;

        GameObject personalSatellite = CreateStaticSatellite("SAT-P01", satellitesRoot.transform, satelliteMaterial, panelMaterial, importedSatellitePrefab, new Vector3(-5.6f, 2.1f, 3.8f));
        GameObject companySatellite = CreateStaticSatellite("SAT-C01", satellitesRoot.transform, satelliteMaterial, panelMaterial, importedSatellitePrefab, new Vector3(4.9f, -1.5f, 4.4f));
        EnterpriseSatelliteAnimator companyAnimator = companySatellite.AddComponent<EnterpriseSatelliteAnimator>();
        companyAnimator.simulationManager = simulationManager;
        List<GameObject> relaySatellites = CreateRelayConstellation(satellitesRoot.transform, satelliteMaterial, panelMaterial, importedSatellitePrefab, simulationManager);
        GameObject orbitStation = CreateSpaceStation("OrbitShareStation", satellitesRoot.transform, stationHullMaterial, stationAccentMaterial, importedStationPrefab);
        SpaceStationAnimator stationAnimator = orbitStation.GetComponent<SpaceStationAnimator>();
        if (stationAnimator == null)
        {
            stationAnimator = orbitStation.AddComponent<SpaceStationAnimator>();
        }
        stationAnimator.simulationManager = simulationManager;

        GameObject orbitLines = new GameObject("OrbitLines");
        LineRenderer orbitRenderer = orbitLines.AddComponent<LineRenderer>();
        orbitRenderer.material = orbitMaterial;
        OrbitLineRenderer orbitLine = orbitLines.AddComponent<OrbitLineRenderer>();
        orbitLine.satelliteOrbit = satelliteOrbit;
        CreateStaticOrbitRing("Relay Orbit Slot A", orbitLines.transform, 6.3f, -32f, secondaryOrbitMaterial);
        CreateStaticOrbitRing("Relay Orbit Slot B", orbitLines.transform, 7.25f, 5f, secondaryOrbitMaterial);
        CreateStaticOrbitRing("Relay Orbit Slot C", orbitLines.transform, 8.2f, 32f, secondaryOrbitMaterial);

        GameObject targetRoot = new GameObject("RequestCities");
        ObservationTarget seoul = CreateTarget("SeoulCity", "Seoul", targetRoot.transform, targetMaterial, selectedTargetMaterial, 37.56f, 126.97f, 64f, 0.4f, 120, "Local Loopback", 3f);
        ObservationTarget newYork = CreateTarget("NewYorkCity", "New York", targetRoot.transform, targetMaterial, selectedTargetMaterial, 40.71f, -74.01f, 128f, 0.9f, 220, "Trans-Pacific Relay", 12f);
        ObservationTarget london = CreateTarget("LondonCity", "London", targetRoot.transform, targetMaterial, selectedTargetMaterial, 51.51f, -0.13f, 96f, 0.7f, 190, "Europe Relay", 8f);

        GameObject groundStationObject = CreateGroundStation(stationMaterial, 37.56f, 126.97f);
        GroundStation groundStation = groundStationObject.AddComponent<GroundStation>();
        GameObject stationRange = CreateSurfaceDisc("SeoulGatewayCommunicationRange", groundStationObject.transform.position, 1.18f, stationRangeMaterial);

        GameObject connectionObject = new GameObject("SeoulUplinkLine");
        LineRenderer connectionRenderer = connectionObject.AddComponent<LineRenderer>();
        connectionRenderer.material = lineMaterial;
        ConnectionLine connectionLine = connectionObject.AddComponent<ConnectionLine>();
        connectionLine.satellite = sharedSatellite.transform;
        connectionLine.groundStation = groundStationObject.transform;

        GameObject scanObject = new GameObject("DestinationDownlinkBeam");
        LineRenderer scanRenderer = scanObject.AddComponent<LineRenderer>();
        scanRenderer.positionCount = 2;
        scanRenderer.widthMultiplier = 0.075f;
        scanRenderer.material = lineMaterial;
        scanRenderer.enabled = false;

        GameObject targetHalo = CreateSurfaceDisc("SelectedDestinationFootprint", newYork.transform.position, 0.82f, targetHaloMaterial);
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

        missionManager.satelliteManager = satelliteManager;
        missionManager.simulationManager = simulationManager;
        missionManager.earth = earth.transform;
        missionManager.satelliteOrbit = satelliteOrbit;
        missionManager.groundStation = groundStation;
        missionManager.connectionLine = connectionLine;
        missionManager.Targets.Add(newYork);
        missionManager.Targets.Add(london);
        missionManager.Targets.Add(seoul);

        cameraManager.targetCamera = camera;
        cameraManager.earth = earth.transform;
        cameraManager.satellite = sharedSatellite.transform;
        cameraManager.missionTarget = newYork.transform;
        cameraManager.groundStation = groundStationObject.transform;
        cameraManager.SetOverview();

        focusController.Initialize(satelliteManager, cameraManager, focusHaloMaterial);
        focusController.RegisterSatellite("SAT-P01", personalSatellite.transform);
        focusController.RegisterSatellite("SAT-C01", companySatellite.transform);
        focusController.RegisterSatellite("SAT-S01", sharedSatellite.transform);
        for (int i = 0; i < relaySatellites.Count; i++)
        {
            focusController.RegisterSatellite(relaySatellites[i].name, relaySatellites[i].transform);
        }

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
        camera.fieldOfView = 48f;
        camera.transform.position = new Vector3(0f, 6.2f, -12f);
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
        lightObject.transform.rotation = Quaternion.Euler(45f, -35f, 0f);
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
        body.transform.localScale = new Vector3(0.32f, 0.28f, 0.42f);
        body.GetComponent<Renderer>().material = bodyMaterial;

        GameObject leftPanel = GameObject.CreatePrimitive(PrimitiveType.Cube);
        leftPanel.name = "Left Solar Panel";
        leftPanel.transform.SetParent(root.transform, false);
        leftPanel.transform.localPosition = new Vector3(-0.47f, 0f, 0f);
        leftPanel.transform.localScale = new Vector3(0.55f, 0.04f, 0.32f);
        leftPanel.GetComponent<Renderer>().material = panelMaterial;

        GameObject rightPanel = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rightPanel.name = "Right Solar Panel";
        rightPanel.transform.SetParent(root.transform, false);
        rightPanel.transform.localPosition = new Vector3(0.47f, 0f, 0f);
        rightPanel.transform.localScale = new Vector3(0.55f, 0.04f, 0.32f);
        rightPanel.GetComponent<Renderer>().material = panelMaterial;

        return root;
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
        AddWorldLabel("OrbitShare Station", new Vector3(0f, 8.2f, 0f), 0.26f);
        return root;
    }

    private ObservationTarget CreateTarget(string objectName, string displayName, Transform parent, Material normalMaterial, Material selectedMaterial, float latitude, float longitude, float dataMb, float resolution, int costCredits, string priority, float weatherRisk)
    {
        GameObject target = new GameObject(objectName);
        target.transform.SetParent(parent, false);
        target.transform.position = SurfacePosition(latitude, longitude, 3.16f);

        GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        marker.name = "Marker";
        marker.transform.SetParent(target.transform, false);
        marker.transform.localScale = Vector3.one * 0.22f;

        ObservationTarget observationTarget = target.AddComponent<ObservationTarget>();
        observationTarget.Initialize(displayName, normalMaterial, selectedMaterial, dataMb, resolution, costCredits, priority, weatherRisk);
        PulseScale pulse = marker.AddComponent<PulseScale>();
        pulse.amplitude = 0.12f;
        pulse.speed = 2.6f;
        Vector3 labelPosition = target.transform.position * 1.12f;
        if (displayName == "Seoul")
        {
            labelPosition += Vector3.down * 0.2f;
        }
        else if (displayName == "New York")
        {
            labelPosition += Vector3.up * 0.34f;
        }
        TextMesh label = AddWorldLabel("수신 " + DestinationLabel(displayName), labelPosition, 0.34f);
        observationTarget.SetLabel(label);
        return observationTarget;
    }

    private GameObject CreateGroundStation(Material material, float latitude, float longitude)
    {
        GameObject station = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        station.name = "GroundStation";
        station.transform.position = SurfacePosition(latitude, longitude, 3.25f);
        station.transform.localScale = new Vector3(0.2f, 0.26f, 0.2f);
        station.transform.up = station.transform.position.normalized;
        station.GetComponent<Renderer>().material = material;
        PulseScale pulse = station.AddComponent<PulseScale>();
        pulse.amplitude = 0.08f;
        pulse.speed = 2.2f;
        AddWorldLabel("발신 서울", station.transform.position * 1.13f + Vector3.up * 0.2f, 0.36f);
        return station;
    }

    private void CreateAtmosphere(Transform earth, Material atmosphereMaterial)
    {
        GameObject atmosphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        atmosphere.name = "Atmosphere";
        atmosphere.transform.SetParent(earth, false);
        atmosphere.transform.localPosition = Vector3.zero;
        atmosphere.transform.localScale = Vector3.one * 1.035f;
        atmosphere.GetComponent<Renderer>().material = atmosphereMaterial;
        Destroy(atmosphere.GetComponent<Collider>());
    }

    private void CreateCloudLayer(Transform earth, Material cloudMaterial)
    {
        GameObject clouds = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        clouds.name = "Cloud Layer";
        clouds.transform.SetParent(earth, false);
        clouds.transform.localPosition = Vector3.zero;
        clouds.transform.localScale = Vector3.one * 1.018f;
        clouds.GetComponent<Renderer>().material = cloudMaterial;
        EarthRotation rotation = clouds.AddComponent<EarthRotation>();
        rotation.simulationManager = earth.GetComponent<EarthRotation>() != null ? earth.GetComponent<EarthRotation>().simulationManager : null;
        Destroy(clouds.GetComponent<Collider>());
    }

    private GameObject CreateSurfaceDisc(string objectName, Vector3 surfacePosition, float diameter, Material material)
    {
        GameObject disc = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        disc.name = objectName;
        disc.transform.position = surfacePosition.normalized * 3.22f;
        disc.transform.up = surfacePosition.normalized;
        disc.transform.localScale = new Vector3(diameter, 0.012f, diameter);
        disc.GetComponent<Renderer>().material = material;
        Destroy(disc.GetComponent<Collider>());
        return disc;
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

    private void AddCityLights(Transform earth, Material material)
    {
        float[,] cities =
        {
            { 37.56f, 126.97f }, { 35.18f, 129.07f }, { 35.69f, 139.69f },
            { 31.23f, 121.47f }, { 1.35f, 103.82f }, { 28.61f, 77.21f },
            { 51.50f, -0.12f }, { 48.85f, 2.35f }, { 40.71f, -74.00f },
            { 34.05f, -118.24f }, { -33.87f, 151.21f }, { -23.55f, -46.63f }
        };

        for (int i = 0; i < cities.GetLength(0); i++)
        {
            GameObject light = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            light.name = "City Light";
            light.transform.SetParent(earth, true);
            light.transform.position = SurfacePosition(cities[i, 0], cities[i, 1], 3.08f);
            light.transform.localScale = Vector3.one * 0.045f;
            light.GetComponent<Renderer>().material = material;
            Destroy(light.GetComponent<Collider>());
        }
    }

    private void CreateStaticOrbitRing(string objectName, Transform parent, float radius, float inclination, Material material)
    {
        GameObject ring = new GameObject(objectName);
        ring.transform.SetParent(parent, false);
        LineRenderer line = ring.AddComponent<LineRenderer>();
        line.material = material;
        line.positionCount = 145;
        line.useWorldSpace = true;
        line.widthMultiplier = 0.007f;
        line.numCapVertices = 4;

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
        textMesh.color = new Color(0.88f, 0.96f, 1f, 0.82f);
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

    private Texture2D CreateEarthTexture()
    {
        Texture2D texture = new Texture2D(1024, 512, TextureFormat.RGBA32, true);
        texture.filterMode = FilterMode.Trilinear;
        texture.wrapMode = TextureWrapMode.Repeat;
        Color deepOcean = new Color(0.015f, 0.105f, 0.28f);
        Color ocean = new Color(0.025f, 0.22f, 0.48f);
        Color shallow = new Color(0.04f, 0.38f, 0.58f);
        Color land = new Color(0.055f, 0.28f, 0.15f);
        Color highLand = new Color(0.18f, 0.38f, 0.2f);
        Color ice = new Color(0.78f, 0.9f, 0.95f);

        for (int y = 0; y < texture.height; y++)
        {
            float v = y / (float)(texture.height - 1);
            float latitude = Mathf.Abs(v * 2f - 1f);
            for (int x = 0; x < texture.width; x++)
            {
                float u = x / (float)(texture.width - 1);
                float nx = u * 6.2f;
                float ny = v * 3.1f;
                float broadLand = Mathf.PerlinNoise(nx * 0.95f + 2.7f, ny * 1.35f);
                float coastDetail = Mathf.PerlinNoise(nx * 4.8f + 11.2f, ny * 4.2f);
                float oceanDetail = Mathf.PerlinNoise(nx * 9.5f, ny * 7.5f);
                float continentMask = broadLand * 0.72f + coastDetail * 0.28f - latitude * 0.18f;
                Color color = Color.Lerp(deepOcean, ocean, 0.5f + oceanDetail * 0.35f);
                color = Color.Lerp(color, shallow, Mathf.SmoothStep(0.48f, 0.58f, continentMask) * 0.45f);
                if (continentMask > 0.58f && latitude < 0.82f)
                {
                    float elevation = Mathf.PerlinNoise(nx * 11.5f + 4f, ny * 9.5f + 8f);
                    color = Color.Lerp(land, highLand, elevation * 0.7f);
                }
                if (latitude > 0.88f)
                {
                    color = Color.Lerp(color, ice, Mathf.SmoothStep(0.88f, 0.98f, latitude));
                }
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        return texture;
    }

    private Texture2D CreateCloudTexture()
    {
        Texture2D texture = new Texture2D(1024, 512, TextureFormat.RGBA32, true);
        texture.filterMode = FilterMode.Trilinear;
        texture.wrapMode = TextureWrapMode.Repeat;
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                float nx = x * 0.012f;
                float ny = y * 0.018f;
                float cloud = Mathf.PerlinNoise(nx, ny) * 0.65f + Mathf.PerlinNoise(nx * 2.1f + 18f, ny * 1.7f) * 0.35f;
                float alpha = Mathf.SmoothStep(0.56f, 0.82f, cloud) * 0.42f;
                texture.SetPixel(x, y, new Color(0.92f, 0.97f, 1f, alpha));
            }
        }
        texture.Apply();
        return texture;
    }

    private string DestinationLabel(string targetName)
    {
        if (targetName == "Seoul")
        {
            return "서울";
        }
        if (targetName == "New York")
        {
            return "뉴욕";
        }
        if (targetName == "London")
        {
            return "런던";
        }
        return targetName;
    }
}
