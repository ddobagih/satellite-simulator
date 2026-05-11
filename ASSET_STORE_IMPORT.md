# Asset Store Upgrade Guide

This project currently runs without imported Asset Store packages. Unity Asset Store downloads require the user's Unity account and license entitlement, so packages must be downloaded through Unity Editor/Package Manager first.

Once `.unitypackage` files are available, OrbitShare can import and validate them automatically.

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

If these files exist, OrbitShare loads them automatically at runtime. If they are missing, it falls back to procedural visuals and generated tones.

Recommended replacements:

- Replace `CreateSatelliteVisual` with an imported satellite prefab instance.
- Add `Space Station Free 3D Asset` as `SpaceStation.prefab` to replace the procedural OrbitShare Station.
- Replace `CreateEarthTexture` and procedural Earth material with the imported Earth material.
- Assign imported skybox material to `RenderSettings.skybox`.
- Replace generated tones in `AudioFeedbackManager` with imported UI/Sci-fi SFX clips.

The current implementation is kept procedural so the project remains portable and does not break when Asset Store packages are missing.
