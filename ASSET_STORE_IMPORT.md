# Asset Store Upgrade Guide

This project currently runs with imported skybox/station resources, external Earth textures/shaders, and procedural fallbacks for everything else. Unity Asset Store downloads require the user's Unity account and license entitlement, so any additional packages must be downloaded through Unity Editor/Package Manager first.

Once `.unitypackage` files are available, OrbitShare can import and validate them automatically.

## Currently Included Runtime Assets

- `Assets/Resources/OrbitShare/Earth/`
  - Solar System Scope Earth day/night/cloud/normal/specular textures used by the runtime Earth material.
  - Attribution is stored in `Assets/Resources/OrbitShare/Earth/ATTRIBUTION.md`.
- `Assets/Resources/OrbitShare/AtmosphereRim.shader`, `EarthDayNight.shader`, `EarthClouds.shader`
  - Runtime Earth atmosphere/day-night/cloud rendering shaders.
- `Assets/Resources/OrbitShare/SpaceSkybox.mat`
  - Used as the runtime space skybox when present.
  - If missing, OrbitShare falls back to a procedural starfield.
- `Assets/Resources/OrbitShare/SpaceStation.prefab`
  - Used as the OrbitShare Station model when present.
  - If missing, OrbitShare falls back to a procedural station.

The remaining runtime hooks are optional and currently use fallback visuals or generated tones.

## Verified Candidate Assets

- Space skybox: `Starfield Space Skybox`
  - https://assetstore.unity.com/packages/2d/textures-materials/sky/starfield-space-skybox-16549
  - FREE, latest version 1.1, file size 16.3 MB
- Alternative space skybox: `*FREE* Skyboxes - Space`
  - https://assetstore.unity.com/packages/2d/textures-materials/sky/free-skyboxes-space-178953
  - FREE, latest version 1.0, file size 204.1 MB
- Earth model/material: `Planet Earth Free`
  - https://assetstore.unity.com/packages/3d/environments/sci-fi/planet-earth-free-23399
  - FREE, latest version 2.0, file size 26.7 MB
- Satellite model: `Animated Weather Satellite AR VR`
  - https://assetstore.unity.com/packages/3d/vehicles/space/animated-weather-satellite-ar-vr-109947
  - Paid, latest version 2.0, file size 33.2 MB
- Sci-fi environment props: `Sci Fi Base Assets - free demo pack`
  - https://assetstore.unity.com/packages/3d/environments/sci-fi/sci-fi-base-assets-free-demo-pack-131284
- Space station: `Space Station Free 3D Asset HDRP URP Built-In`
  - Already represented in the current runtime hook as `SpaceStation.prefab`
- UI/SFX fallback: `RPG Essentials Sound Effects - FREE!`
  - https://assetstore.unity.com/packages/audio/sound-fx/rpg-essentials-sound-effects-free-227708

## Import Workflow

1. In Unity Editor, sign in to Unity Hub/Package Manager.
2. Add/download the selected Asset Store packages to this account.
3. Export or place `.unitypackage` files into `AssetStorePackages/`.
4. Run:

```bash
Tools/import_asset_store_packages.sh
```

Or use Unity Editor:

```text
OrbitShare > Asset Store > Import UnityPackages
OrbitShare > Asset Store > Validate Resource Hooks
```

5. Move the best imported assets under `Assets/Resources/OrbitShare/` with the names below.
6. Open `MainScene` and press Play.

The importer writes `asset-store-import-report.md`.

## Runtime Resource Names

Automatic resource names:

- Satellite prefab: `Assets/Resources/OrbitShare/SatelliteModel.prefab`
- Space station prefab: `Assets/Resources/OrbitShare/SpaceStation.prefab`
- Earth material: `Assets/Resources/OrbitShare/PremiumEarthMaterial.mat`
- Space skybox material: `Assets/Resources/OrbitShare/SpaceSkybox.mat`
- Reservation sound: `Assets/Resources/OrbitShare/SFX_Reserve.wav`
- Mission start sound: `Assets/Resources/OrbitShare/SFX_Start.wav`
- Observation sound: `Assets/Resources/OrbitShare/SFX_Observe.wav`
- Transmission sound: `Assets/Resources/OrbitShare/SFX_Transmit.wav`
- Completion sound: `Assets/Resources/OrbitShare/SFX_Complete.wav`
- Click sound: `Assets/Resources/OrbitShare/SFX_Click.wav`
- Failure sound: `Assets/Resources/OrbitShare/SFX_Failure.wav`

If these files exist, OrbitShare loads them automatically at runtime. If they are missing, it falls back to procedural visuals and generated tones.

Current hook behavior:

- `SatelliteModel.prefab`: replaces the procedural satellite visual when present.
- `SpaceStation.prefab`: replaces the procedural OrbitShare Station when present.
- `PremiumEarthMaterial.mat`: replaces the procedural Earth material when present.
- `SpaceSkybox.mat`: replaces the procedural starfield when present.
- `SFX_*.wav`: replaces generated UI tones when present. Currently `SFX_Click.wav` and `SFX_Failure.wav` are also optional hooks.

No code change is required if the assets are placed with the names above. The implementation keeps procedural fallbacks so the project remains portable and does not break when optional Asset Store packages are missing.
