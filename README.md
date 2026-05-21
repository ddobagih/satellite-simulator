# OrbitShare

Unity 3D project for a future one-shot satellite short-message relay.

## Run

1. Open this folder with Unity Hub as a Unity project.
2. Open `Assets/Scenes/MainScene.unity`.
3. Press Play.

The scene is generated at runtime by `Assets/Scripts/OrbitShareBootstrap.cs`.

## Demo Flow

1. Sender is fixed at Seoul.
2. Select one destination city: Seoul, New York, or London.
3. Choose when to send: now, 5 minutes later, 15 minutes later, or 30 minutes later.
4. Choose message size: 32 KB, 128 KB, or 512 KB.
5. Filter/select a candidate satellite by all, available, fastest window, or lowest fee.
6. Click `위성 확정`, then `메시지 발신`.
7. Watch the one-shot uplink/relay/downlink flow and confirm the received result.

For a quick run, click `빠른 발신`. It chooses a usable candidate for the selected destination/time, aligns the active relay satellite, and completes the delivery flow.

## Implemented Features

- Korean Runtime Canvas UI for the main Play Mode flow
- 3D Earth with slow rotation, improved procedural Earth texture, atmosphere shell, cloud layer, and imported space skybox hook
- 24 visible/candidate satellites around the 3D Earth
- Multiple orbit slots and extra orbit-slot rings for a denser relay constellation
- Scrollable satellite candidate list with filters: all, available, fast window, low fee
- Satellite detail panel with orbit slot, next window, fee, latency, reliability, battery, and buffer
- One-shot short-message communication request flow from Seoul
- Destination city selection and highlighted markers
- Send-time selection, message-size selection, estimated fee, and success probability display
- Relay path lock, Seoul gateway communication range, connection line, and destination signal beam
- Message delivery progress and communication completion status
- Quick send button
- Communication timeline UI
- Compact final received-message result summary panel
- Selected satellite focus halo and camera follow target switching
- Runtime Canvas UI with an optional disabled-by-default IMGUI fallback overlay
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

OrbitShare is a future one-shot satellite short-message relay. The local sender is fixed at Seoul, and the user chooses one destination city, send time, message size, and a satellite candidate for a single delivery.

### Practicality

The project visualizes a communication service workflow instead of only showing orbital movement: destination selection, send-time planning, candidate filtering, fee/success estimate, relay path lock, Seoul gateway sync, and message delivery.

### Creativity

The key idea is a small-scale alternative to large continuous satellite networks: one Seoul-origin request finds a suitable satellite window and sends a short message once through a visible relay constellation.

### Limitations

The orbit and availability model are simplified for presentation stability. Fee and success values are simulated estimates, not real billing or live service guarantees. The project does not use real TLE data, payment systems, online databases, live satellite data, or map APIs.

### Optional Asset Store Upgrade Points

This project currently includes two lightweight Asset Store runtime hooks: `SpaceSkybox.mat` and `SpaceStation.prefab`. Missing optional assets fall back to procedural visuals and generated tones, so the project still opens reliably. See `ASSET_STORE_IMPORT.md` for the exact hook names and additional candidates.

Optional upgrade areas are:

- High-resolution Earth material or space skybox
- Low-poly satellite model pack
- Sci-fi UI sound effects
- Lightweight HUD/icon set
