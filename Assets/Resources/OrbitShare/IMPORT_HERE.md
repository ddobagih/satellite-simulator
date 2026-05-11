# OrbitShare Runtime Asset Hooks

Place imported Asset Store assets here with these exact names:

- `SatelliteModel.prefab`
- `SpaceStation.prefab`
- `PremiumEarthMaterial.mat`
- `SpaceSkybox.mat`
- `SFX_Reserve.wav`
- `SFX_Start.wav`
- `SFX_Observe.wav`
- `SFX_Transmit.wav`
- `SFX_Complete.wav`

OrbitShare loads these through `Resources.Load()` at runtime. Missing files are allowed because the project has procedural fallbacks.
