# Computer Graphics Assignment 1 - Visual/Asset/Audio lane note (2026-05-17)

## 최근 진행 근거
- 2026-05-16 `daylog/2026-05-16.md`: `SpaceSkybox.mat` 로드 시 카메라 clear flag를 `Skybox`로 전환하는 보강이 있었지만, 이후 Unity LicensingClient IPC 문제로 새 Play Mode 캡처/검증은 미완료.
- 2026-05-16 `asset-store-import-report.md`: `SpaceSkybox.mat`, `SpaceStation.prefab`은 FOUND. `SatelliteModel.prefab`, `PremiumEarthMaterial.mat`, SFX 5종은 MISSING optional fallback.
- 2026-05-16 `Assets/Resources/OrbitShare/`: 실제 파일도 `SpaceSkybox.mat`, `SpaceStation.prefab`, `IMPORT_HERE.md`만 확인됨.
- 2026-05-16 `Assets/Scripts/OrbitShareBootstrap.cs`: `PremiumEarthMaterial`, `SatelliteModel`, `SpaceStation`, `SpaceSkybox`를 `Resources.Load`로 읽고 없으면 procedural fallback 사용.
- 2026-05-16 `Assets/Scripts/AudioFeedbackManager.cs`: SFX 5종이 없으면 런타임 생성 tone/chord fallback 사용.
- 2026-05-16 `orbitshare-validation.md`: 기존 리포트는 `Result: PASS`, Runtime Error/Exception 0이지만 화면 기준은 640x480이고 r1 skybox 수정 뒤 재생성된 근거는 아님.
- 2026-05-16 `orbitshare-playmode.png`: 1600x900 PNG가 존재하나, skybox clear flag 수정 이후 새로 캡처됐다는 근거는 부족함.
- `plans/daily/2026-05-17.md`는 현재 확인되지 않음. 별도 `AGENTS.md` 파일도 확인되지 않아 사용자 제공 AGENTS 지침을 기준으로 적용.

## 내일 목표 후보
- 1순위: Unity 실행 가능한 환경에서 Resource Hook 검증과 skybox/station 화면 QA를 다시 확정한다.
- 2순위: `SatelliteModel.prefab`, `PremiumEarthMaterial.mat`, SFX 5종을 추가할지, optional fallback 유지로 제출할지 결정한다.
- 3순위: fallback 유지 시 README/WORK_LOG/ASSET_STORE_IMPORT/report/daylog에 이유와 현재 상태를 모순 없이 남긴다.
- 4순위: 발표 화면에서 skybox, station, 위성 procedural visual, 통신선이 UI를 방해하지 않고 충분히 보이는지 확인한다.
- 5순위: 50MB 초과 visual asset의 Git LFS/private repo/제출 제외 방침을 확정한다.

## 상세 체크리스트 초안
- [ ] `Assets/Resources/OrbitShare` 인벤토리 재확인 → 검증: 실제 파일 목록과 `asset-store-import-report.md`의 FOUND/MISSING 상태가 일치하는지 대조.
- [ ] Unity `OrbitShare > Asset Store > Validate Resource Hooks` 실행 → 검증: `asset-store-import-report.md`가 현재 파일 상태로 갱신되고 로그에 LicensingClient 실패가 없는지 확인.
- [ ] skybox 적용 후 Play Mode 화면 확인 → 검증: `SpaceSkybox.mat`가 실제 배경으로 렌더링되고 procedural starfield fallback이 불필요하게 남지 않는지 캡처/GUI로 확인.
- [ ] `SpaceStation.prefab` 가시성 확인 → 검증: Fast Demo 화면에서 OrbitShare Station 위치, 크기, UI 가림 여부를 확인.
- [ ] `SatelliteModel.prefab` 처리 결정 → 검증: 추가하지 않으면 procedural satellite fallback 유지 사유를 기록, 추가하면 24개 위성 표시와 Fast Demo 완료 확인.
- [ ] `PremiumEarthMaterial.mat` 처리 결정 → 검증: 추가하지 않으면 procedural Earth 유지 사유를 기록, 추가하면 지구 표면/대기권/마커 가시성 확인.
- [ ] SFX 5종 처리 결정 → 검증: 추가하지 않으면 generated tone fallback 유지 사유를 기록, 추가하면 Queue/Relay/전달/완료 단계별 재생을 직접 확인.
- [ ] 대용량 에셋 방침 정리 → 검증: 50MB 초과 Cobble Games 텍스처/패키지 파일을 유지, LFS 이동, 제출 제외 중 하나로 기록.
- [ ] Visual/Asset 문서 정합성 점검 → 검증: `README.md`, `WORK_LOG.md`, `ASSET_STORE_IMPORT.md`, `asset-store-import-report.md`가 현재 포함 에셋과 optional fallback을 같은 방식으로 설명.

## 리스크/확인 필요
- Unity LicensingClient IPC 문제 때문에 2026-05-16 r1 이후 새 Unity 검증, 빌드, Resource Hook 검증이 완료되지 않았다.
- skybox clear flag 수정은 코드상 확인되지만, 수정 후 실제 화면에서 skybox가 보인다는 새 캡처 근거가 필요하다.
- `SatelliteModel.prefab`, `PremiumEarthMaterial.mat`, SFX 5종이 과제 필수인지 선택 개선인지 사용자/제출 기준 확인이 필요하다.
- Asset Store 원본 `.unitypackage`를 제출해야 하는지, import된 `Assets` 리소스만 유지하면 되는지 확인 필요.
- 50MB 초과 visual asset이 여러 개 있어 GitHub private 유지로 충분한지, Git LFS가 필요한지 결정 필요.
- 오디오 fallback은 자동 검증으로 청취 품질을 보장하지 못하므로 발표 전 직접 확인이 필요하다.

## 병렬 에이전트 활용 메모
- 사용하지 않음.
- 이번 작업은 기존 문서, 리소스 인벤토리, 관련 코드 확인으로 범위가 좁아 단일 조사로 충분했다.
- 파일 읽기와 상태 확인은 병렬 shell 호출로만 수행했고, 최종 계획 파일은 수정하지 않았다.