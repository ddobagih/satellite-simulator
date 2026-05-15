# OrbitShare

Unity 3D project for a future one-shot satellite communication relay.

## Run

1. Open this folder with Unity Hub as a Unity project.
2. Open `Assets/Scenes/MainScene.unity`.
3. Press Play.

The scene is generated at runtime by `Assets/Scripts/OrbitShareBootstrap.cs`.

## Demo Flow

1. Sender is fixed at Seoul.
2. Select one destination city: Seoul, New York, or London.
3. Click `Queue`.
4. Click `Relay`.
5. Use `10x` time scale for a faster demo.
6. Watch the 24-satellite constellation route a one-shot message and show communication completion.

For a quick run, click `Quick Relay`. It queues and starts the request if needed, aligns the active relay satellite, and assists the delivery phase.

## Implemented Features

- 3D Earth with slow rotation
- Procedural Earth texture and atmosphere shell with an imported space skybox hook
- 24 visible satellites around the 3D Earth
- Three controllable relay assets plus 21 background relay nodes
- Active relay satellite with circular orbit and orbit line
- Animated relay satellites for a denser communication constellation
- Selected satellite focus halo and camera follow target switching
- Satellite list and detail panels
- Runtime Canvas UI with an optional IMGUI fallback overlay
- One-shot communication request flow from Seoul
- Relay request guidance for the Seoul-origin one-shot communication flow
- Destination city selection and highlighted markers
- Time controls: Pause, 1x, 5x, 10x
- Camera modes: Overview, Earth, Satellite, Route
- Smooth camera transitions
- Automatic presentation camera cuts during mission phases
- Distance/angle based relay path lock
- Destination signal beam, Seoul gateway communication range, and connection line
- Message delivery progress and communication completion status
- Quick relay button
- Communication timeline UI
- Relay intelligence panel with route ETA, gateway ETA, payload estimate, route score, and SLA reliability
- Compact final communication result summary panel
- Procedural city lights and extra orbit-slot rings for a denser digital-twin scene
- OrbitShare Station object with an Asset Store prefab hook and procedural fallback
- Procedural UI sound feedback for queueing, relay lock, delivery, and completion
- Unity internal demo-frame capture utility for visual verification
- Automated Play Mode validation report at `orbitshare-validation.md`

## Build

In Unity Editor, use one of these menu items:

- `OrbitShare > Build > Linux`
- `OrbitShare > Build > Windows`

The build script is in `Assets/Editor/BuildOrbitShare.cs`.

## Automated Validation

The project includes an editor-side validation runner:

```bash
LD_LIBRARY_PATH=/home/ddobagi/Code/grade3/computergraphics/assignment1/.unity-libs:$LD_LIBRARY_PATH \
/home/ddobagi/Unity/Hub/Editor/2022.3.62f3/Editor/Unity \
-batchmode -nographics \
-projectPath /home/ddobagi/Code/grade3/computergraphics/assignment1 \
-executeMethod OrbitShareEditorAutomation.ValidateMissionFlow \
-logFile unity-validation.log
```

It enters Play mode, runs the fast demo mission flow, and writes `orbitshare-validation.md`.
The editor automation intentionally exits Play mode after validation, so that behavior is not a game crash.

## Concept Notes

### Topic

OrbitShare is a future one-shot satellite communication relay. The local sender is fixed at Seoul, and the user chooses one destination city for a single message delivery.

### Practicality

The project visualizes a communication workflow instead of only showing orbital movement: destination selection, request queueing, relay path lock, Seoul gateway sync, and message delivery.

### Creativity

The key idea is a small-scale alternative to large continuous satellite networks: one Seoul-origin request is routed once through a visible relay constellation.

### Limitations

The orbit is simplified to a circular path for presentation stability. It does not use real orbital mechanics, payment systems, online databases, live satellite data, or map APIs.

### Optional Asset Store Upgrade Points

This project currently includes two lightweight Asset Store runtime hooks: `SpaceSkybox.mat` and `SpaceStation.prefab`. Missing optional assets fall back to procedural visuals and generated tones, so the project still opens reliably. See `ASSET_STORE_IMPORT.md` for the exact hook names and additional candidates.

Optional upgrade areas are:

- High-resolution Earth material or space skybox
- Low-poly satellite model pack
- Sci-fi UI sound effects
- Lightweight HUD/icon set
