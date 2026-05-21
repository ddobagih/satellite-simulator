# Computer Graphics Assignment 1 - Visual/Asset/Audio lane note (2026-05-18)

## 최근 진행 근거
- 2026-05-17 `daylog/2026-05-17.md`: `Assets/Resources/OrbitShare`에는 `SpaceSkybox.mat`, `SpaceStation.prefab`, `IMPORT_HERE.md`만 있고, `SatelliteModel.prefab`, `PremiumEarthMaterial.mat`, SFX 5종은 optional fallback 상태로 확인됨.
- 2026-05-17 `daylog/2026-05-17.md`: `unity-validation.log`, `unity-build-r1.log`, `unity-asset-hooks.log`가 모두 `IPC channel to LicensingClient doesn't exist; aborting`으로 중단되어 최신 Unity 검증/Resource Hook 검증은 완료되지 않음.
- 2026-05-16 `daylog/2026-05-16.md`: `SpaceSkybox.mat` 로드 시 camera clear flag를 `Skybox`로 전환하는 보강이 반영됐지만, 이후 새 Play Mode 캡처나 GUI 리허설 근거는 아직 없음.
- 2026-05-15 `README.md`: 현재 런타임 asset hook은 `SpaceSkybox.mat`, `SpaceStation.prefab` 2개이고, missing optional assets는 procedural visual/generated tone fallback으로 동작한다고 설명됨.
- 2026-05-16 `WORK_LOG.md`: Asset Store 반영 항목은 `FREE Skyboxes - Space`, `Space Station Free 3D Asset HDRP URP Built-In`이며, Satellite/PremiumEarth/SFX는 선택 대체 리소스로 정리됨.
- 2026-05-15 `ASSET_STORE_IMPORT.md` 및 2026-05-11 `asset-store-import-report.md`: expected runtime assets 중 skybox/station만 FOUND, satellite/earth/SFX는 MISSING optional로 기록됨.
- 2026-05-16 `Assets/Scripts/OrbitShareBootstrap.cs`: `Resources.Load`로 `PremiumEarthMaterial`, `SatelliteModel`, `SpaceStation`, `SpaceSkybox`를 읽고 없으면 procedural fallback을 사용함. `SpaceSkybox`가 있으면 `RenderSettings.skybox`와 camera `Skybox` clear flag를 설정함.
- 2026-05-10 `Assets/Scripts/AudioFeedbackManager.cs`: `SFX_Reserve`, `SFX_Start`, `SFX_Observe`, `SFX_Transmit`, `SFX_Complete`가 없으면 생성 tone/chord fallback을 사용함.
- 2026-05-17 정적 조사 기준: `Assets` 안에 `.wav/.mp3/.ogg/.aif/.aiff` 오디오 파일은 없음.
- 2026-05-17 정적 조사 기준: `Assets/Cobble Games/Spaceship` 아래 50MB 초과 texture 파일이 다수 있고, nested `URP.unitypackage`/`HDRP.unitypackage`는 `.gitignore` 대상이나 import된 대용량 texture 일부는 git 추적 대상임.
- `plans/daily/2026-05-18.md`와 `plans/.work/2026-05-18` 하위 파일은 현재 확인되지 않음.

## 내일 목표 후보
- 1순위: Unity LicensingClient/GUI 환경이 확보되는 즉시 Resource Hook 검증을 재실행하고 `asset-store-import-report.md`를 현재 파일 상태로 갱신한다.
- 2순위: skybox clear flag 보강 이후 실제 Play Mode 화면에서 `SpaceSkybox.mat`가 보이는지, `SpaceStation.prefab`이 발표 카메라 컷에서 보이는지 확인한다.
- 3순위: `SatelliteModel.prefab`, `PremiumEarthMaterial.mat`, SFX 5종을 추가할지 아니면 optional fallback 유지로 제출할지 확정한다.
- 4순위: fallback 유지가 결정되면 README/WORK_LOG/ASSET_STORE_IMPORT/report/daylog에 같은 표현으로 정리한다.
- 5순위: 발표 화면에서 procedural satellite, procedural Earth, connection line, station, skybox가 UI를 방해하지 않고 충분히 식별되는지 확인한다.
- 6순위: Asset Store 리소스와 50MB 초과 visual asset의 private repo/Git LFS/제출 제외 방침을 확정한다.

## 상세 체크리스트 초안
- [ ] `Assets/Resources/OrbitShare` 인벤토리 재확인 → 검증: 실제 파일 목록이 `asset-store-import-report.md`의 FOUND/MISSING 상태와 일치하는지 대조한다.
- [ ] Resource Hook 검증 재실행 → 검증: Unity 메뉴 `OrbitShare > Asset Store > Validate Resource Hooks` 또는 대응 batch 실행 후 `asset-store-import-report.md`가 갱신되고 LicensingClient 실패가 없는지 확인한다.
- [ ] skybox 화면 QA 수행 → 검증: Play Mode에서 `SpaceSkybox.mat`가 실제 우주 배경으로 렌더링되고, skybox 보강 이후 새 캡처나 GUI 리허설 근거를 남긴다.
- [ ] `SpaceStation.prefab` 가시성 확인 → 검증: Fast Demo/Quick Relay 중 OrbitShare Station 위치, 크기, 카메라 컷, UI 가림 여부를 확인한다.
- [ ] procedural satellite fallback 제출 가능성 확정 → 검증: `SatelliteModel.prefab`을 추가하지 않으면 fallback 유지 사유를 문서화하고, 추가하면 24개 위성 표시와 Fast Demo 완료를 다시 확인한다.
- [ ] procedural Earth fallback 제출 가능성 확정 → 검증: `PremiumEarthMaterial.mat`을 추가하지 않으면 fallback 유지 사유를 문서화하고, 추가하면 지구 표면/대기권/도시 마커 가시성을 확인한다.
- [ ] SFX fallback 또는 파일 추가 결정 → 검증: SFX를 추가하지 않으면 generated tone fallback 유지 사유를 문서화하고, 추가하면 Queue/Relay/관측/전송/완료 단계별 재생을 직접 청취 확인한다.
- [ ] 시각 자료 갱신 필요 여부 결정 → 검증: 기존 `orbitshare-playmode.png`, `orbitshare-demo.gif`, `orbitshare-contact-sheet.png`, `DemoFrames/`가 최신 skybox/station 상태를 반영하는지 확인하고, 아니면 캡처 재생성 작업을 build-validation lane에 요청한다.
- [ ] 대용량/라이선스 리소스 방침 정리 → 검증: Cobble Games 대용량 texture와 Asset Store 리소스를 private repo 유지, Git LFS 이동, 제출 패키지 제외 중 하나로 분류해 기록한다.
- [ ] Visual/Asset 문서 정합성 점검 → 검증: `README.md`, `WORK_LOG.md`, `ASSET_STORE_IMPORT.md`, `asset-store-import-report.md`가 포함 에셋 2개와 optional fallback 상태를 모순 없이 설명하는지 확인한다.

## 리스크/확인 필요
- Unity LicensingClient IPC 실패 때문에 2026-05-16 r1 이후 Resource Hook 검증, 최신 Play Mode 검증, 빌드 재실행이 완료되지 않았다.
- skybox clear flag 수정은 코드상 확인됐지만, 수정 후 실제 화면에서 skybox가 보인다는 최신 캡처 근거가 필요하다.
- `SatelliteModel.prefab`, `PremiumEarthMaterial.mat`, SFX 5종이 과제 제출 필수인지 선택 개선인지 확인이 필요하다.
- 오디오 fallback은 자동 검증만으로 품질을 판단하기 어려워 발표 전 실제 청취 확인이 필요하다.
- Asset Store 원본 `.unitypackage`를 제출해야 하는지, import된 `Assets` 리소스만 포함하면 되는지 제출 기준 확인이 필요하다.
- 50MB 초과 tracked texture가 있어 GitHub private 유지로 충분한지, Git LFS 또는 제출 제외가 필요한지 결정해야 한다.
- visual 리소스 추가를 늦게 진행하면 자동 검증, GUI 리허설, 캡처, 문서 갱신을 모두 다시 해야 하므로 제출 전 안정성을 해칠 수 있다.

## 병렬 에이전트 활용 메모
- 사용하지 않았음.
- 이번 lane note는 문서, 최근 daylog, 리소스 인벤토리, 관련 코드 확인으로 범위가 좁아 단일 조사로 충분했다.
- 파일 읽기와 상태 확인은 병렬 shell 호출로 처리했고, 최종 계획 파일은 수정하지 않았다.