# OrbitShare

Unity 3D assignment project for a future shared satellite operation platform.

## Run

1. Open this folder with Unity Hub as a Unity project.
2. Open `Assets/Scenes/MainScene.unity`.
3. Press Play.

The scene is generated at runtime by `Assets/Scripts/OrbitShareBootstrap.cs`.

## Demo Flow

1. Select `SAT-S01` in the satellite list.
2. Select an observation target: Seoul, Jeju, or Pacific.
3. Click `Reserve`.
4. Click `Start`.
5. Use `10x` time scale for a faster demo.
6. Watch the satellite complete observation, connect to the ground station, transmit data, and show mission completion.

For a stable presentation, click `Fast Demo`. It reserves and starts the shared satellite mission if needed, moves the satellite near the observation window, and assists the transfer phase after observation is complete.

## Implemented Features

- 3D Earth with slow rotation
- Procedural Earth texture, atmosphere shell, and starfield
- Three satellite entries: personal, company, shared
- Moving shared satellite with circular orbit and orbit line
- Animated company satellite for a more active platform scene
- Selected satellite focus halo and camera follow target switching
- Satellite list and detail panels
- Runtime Canvas UI plus a presentation-safe IMGUI overlay
- Shared satellite reservation flow
- Reservation eligibility guidance for non-shared satellites
- Observation target selection and highlighted markers
- Time controls: Pause, 1x, 5x, 10x
- Camera modes: Overview, Earth, Satellite, Mission
- Smooth camera transitions
- Automatic presentation camera cuts during mission phases
- Distance/angle based observation success
- Observation scan beam, ground station communication range, and connection line
- Data transfer progress and mission completion status
- Presentation-safe fast demo button
- Mission timeline UI
- Mission intelligence panel with pass ETA, downlink ETA, rental quote, data estimate, resolution, quality score, and SLA reliability
- Final mission result summary panel with generated data card and observation preview
- Procedural city lights and extra orbit-slot rings for a denser digital-twin scene
- OrbitShare Station object with an Asset Store prefab hook and procedural fallback
- Procedural UI sound feedback for reservation, observation, transmission, and completion
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

## Presentation Notes

### Topic

OrbitShare is a future satellite operation platform where personal users, companies, and shared satellite assets are managed in one service.

### Practicality

The project visualizes a service workflow instead of only showing orbital movement: satellite selection, reservation, observation, ground station communication, data transfer, and result delivery.

### Creativity

The key idea is a shared satellite rental model. The user can reserve `SAT-S01`, assign a target region, and watch the operation complete as a digital twin scenario.

### Limitations

The orbit is simplified to a circular path for presentation stability. It does not use real orbital mechanics, payment systems, online databases, live satellite data, or map APIs.

### Optional Asset Store Upgrade Points

This project currently avoids imported Asset Store packages so it opens reliably. See `ASSET_STORE_IMPORT.md` for concrete candidates and integration points. If importing Asset Store assets in Unity Editor, the best upgrade areas are:

- High-resolution Earth material or space skybox
- Low-poly satellite model pack
- Sci-fi UI sound effects
- Lightweight HUD/icon set
