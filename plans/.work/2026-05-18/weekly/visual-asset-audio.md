# Computer Graphics Assignment 1 - Visual/Asset/Audio weekly lane note (2026-W21)

## 이번 주 목표 후보

- `Assets/Resources/OrbitShare` 기준 runtime asset 상태를 확정한다.
  - 현재 확인: `SpaceSkybox.mat`, `SpaceStation.prefab`만 존재.
  - 현재 없음: `SatelliteModel.prefab`, `PremiumEarthMaterial.mat`, SFX 5종.
- `asset-store-import-report.md`와 실제 파일 상태를 최신 Unity 검증 결과로 맞춘다.
- Play Mode에서 skybox 렌더링, station 가시성, UI/카메라 가림 여부를 실제 화면 기준으로 확인한다.
- optional 위성 모델, 지구 머티리얼, SFX를 추가할지 fallback 유지할지 수요일까지 결정한다.
- 기존 `orbitshare-playmode.png`, `orbitshare-demo.gif`, `orbitshare-contact-sheet.png`, `DemoFrames/`가 최신 비주얼을 반영하는지 판단한다.
- Asset Store 원본 패키지와 50MB 초과 리소스의 제출/저장소 방침을 정리한다.
- `README.md`, `WORK_LOG.md`, `ASSET_STORE_IMPORT.md`, `asset-store-import-report.md`의 visual/fallback 설명을 서로 충돌 없게 맞춘다.

## 날짜별/단계별 체크리스트

- 2026-05-18 월요일
  - [ ] `Assets/Resources/OrbitShare` 인벤토리 기준선 확정.
  - [ ] `asset-store-import-report.md`의 FOUND/MISSING 상태와 실제 파일 목록 대조.
  - [ ] 현재 SFX 파일이 없고 generated tone fallback 상태임을 계획에 반영.
  - [ ] `plans/weekly/2026-W21.md`는 현재 확인되지 않았으므로 lane note만 출력.

- 2026-05-19 화요일
  - [ ] Build/Validation lane과 순서를 맞춰 Unity LicensingClient/GUI 환경 확인.
  - [ ] `OrbitShare > Asset Store > Validate Resource Hooks` 재실행.
  - [ ] `SpaceSkybox.mat`가 실제 Play Mode 배경으로 보이는지 확인.
  - [ ] `SpaceStation.prefab`가 Quick Relay/Fast Demo 중 보이고 UI나 카메라 컷에 가리지 않는지 확인.

- 2026-05-20 수요일
  - [ ] `SatelliteModel.prefab` 추가 여부 결정.
  - [ ] `PremiumEarthMaterial.mat` 추가 여부 결정.
  - [ ] SFX 5종 추가 여부 결정.
  - [ ] 추가하지 않는 항목은 fallback 유지 사유를 문서화 대상으로 넘긴다.
  - [ ] 추가하는 항목은 정확한 Resources 경로/파일명으로 배치한 뒤 자동 검증과 화면 확인을 다시 수행한다.

- 2026-05-21 목요일
  - [ ] station 크기, 위치, 조명, 카메라 컷 가독성 QA.
  - [ ] procedural Earth/satellite fallback이 발표 화면에서 충분히 식별되는지 확인.
  - [ ] SFX를 유지하거나 추가한 경우 Queue/Relay/전송/완료 단계별 청취 확인.
  - [ ] 비주얼 문제가 실제 재현될 때만 최소 수정.

- 2026-05-22 금요일
  - [ ] 기존 PNG/GIF/contact sheet/DemoFrames가 최신 visual 상태와 맞는지 비교.
  - [ ] 필요 시 캡처 재생성 작업을 Build/Validation lane에 순차 실행으로 넘긴다.
  - [ ] README, WORK_LOG, ASSET_STORE_IMPORT, asset-store-import-report 설명 정합성 점검.
  - [ ] 대용량 Asset Store 리소스와 원본 `.unitypackage` 포함/제외 방침 정리.

- 2026-05-23 토요일
  - [ ] optional asset/SFX 변경이 있었다면 회귀 확인.
  - [ ] Resource Hook report, 자동 검증, 화면 캡처, 문서 설명이 같은 상태를 가리키는지 재점검.
  - [ ] 제출 패키지에 포함할 visual asset 범위 확인.

- 2026-05-24 일요일
  - [ ] visual/audio asset freeze.
  - [ ] 새 Asset Store 추가 금지. 단, 제출을 막는 결함만 최소 수정.
  - [ ] 최종 daylog/WORK_LOG에 실제 수행한 검증과 미수행 검증을 분리해 남긴다.

## 검증 계획

- 정적 검증
  - `Assets/Resources/OrbitShare` 파일 목록과 `asset-store-import-report.md` 일치 여부 확인.
  - `Assets` 아래 실제 오디오 파일 존재 여부 확인.
  - 50MB 초과 Asset Store/texture 파일 목록 확인.

- Unity Resource Hook 검증
  - `OrbitShare > Asset Store > Validate Resource Hooks` 실행.
  - LicensingClient IPC 실패 없이 report가 갱신되어야 완료로 본다.

- Play Mode 화면 검증
  - 1600x900 또는 실제 발표 해상도에서 skybox, station, Earth, satellites, UI 가림 여부 확인.
  - Quick Relay/Fast Demo 중 station과 relay visual이 설명 가능한 상태인지 확인.

- Audio 검증
  - 실제 SFX를 추가했다면 5개 이벤트별 재생을 직접 청취 확인.
  - SFX를 추가하지 않으면 generated tone fallback이 들리는지만 확인하고, 고품질 SFX 검증으로 쓰지 않는다.

- 회귀 검증
  - asset 변경 후 `OrbitShareEditorAutomation.ValidateMissionFlow` 재실행.
  - 최소 기준: mission complete, relay lock/delivery 100%, Runtime Error/Exception 0.
  - 기존 `orbitshare-validation.md`는 640x480 기준 PASS이므로 최신 visual 변경의 PASS 근거로 단정하지 않는다.

## 리스크/확인 필요

- Unity LicensingClient IPC 문제가 계속되면 Resource Hook 검증, Play Mode 화면 QA, 오디오 청취 확인을 완료 처리할 수 없다.
- optional asset/SFX 결정을 늦추면 캡처, 검증, README/WORK_LOG 갱신을 다시 해야 한다.
- 현재 실제 오디오 파일은 없어 SFX 품질은 generated tone fallback에 의존한다.
- Asset Store 원본 `.unitypackage`와 Cobble Games 리소스가 매우 커서 private repo 유지, Git LFS, 제출 제외 중 방침 결정이 필요하다.
- `plans/weekly/2026-W21.md`와 `docs/` 경로는 현재 확인되지 않았다.
- Asset Store 후보의 최신 버전/라이선스 상태는 로컬 문서 기준이며, 외부 최신 정보는 이번 조사에서 확인하지 않았다.
- Windows 빌드, MP4/MOV, PPT/PDF 필수 여부에 따라 visual 자료 갱신 범위가 달라질 수 있다.

## 병렬 에이전트 활용 메모

- 하위/병렬 에이전트는 사용하지 않았다.
- 이번 범위는 최근 daylog, daily plan, asset 문서, 리소스 인벤토리, 관련 코드 훅 확인으로 충분히 좁았다.
- 파일 읽기와 정적 확인은 병렬 shell 호출로 처리했고, 최종 weekly lane note는 단일 판단으로 통합했다.