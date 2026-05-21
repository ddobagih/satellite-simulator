# Computer Graphics Assignment 1 - UI/Demo Flow/Presentation lane note (2026-05-19)

## 최근 진행 근거
- 사용자 제공 AGENTS 지침, 2026-05-18 KST: 실제 실행하지 않은 검증은 완료로 쓰지 않고, 불확실한 항목은 확인 필요로 분리해야 함.
- `AGENTS.md`, `docs/`, `plans/daily/2026-05-19.md`, 2026-05-18 KST 확인: 프로젝트 내부 파일/폴더는 확인되지 않음. 사용자 제공 지침과 기존 문서를 기준으로 계획함.
- `README.md`, 2026-05-18 KST 확인: Demo Flow는 `Queue` → `Relay` → `10x`, 빠른 실행은 `Quick Relay`로 안내됨. Runtime Canvas UI, optional IMGUI fallback, presentation camera cuts, timeline UI, 자동 검증 리포트가 기능으로 정리됨.
- `WORK_LOG.md`, 최종 점검 2026-05-16: Fast Demo, 시간 배속, 카메라 모드, 미션 상태/결과 UI 구현 완료 기록. 제출 직전 Play Mode 화면 재확인과 UI/카메라 미세 조정 필요가 남아 있음.
- `daylog/2026-05-16.md`, 2026-05-16 KST: `PresentationDirector` phase camera cut 기록, `ValidateMissionFlow`의 Reset 2회차/1600x900 Canvas/IMGUI fallback 비활성/camera cut 검사가 코드에 추가됐으나 Unity 재실행은 LicensingClient IPC 문제로 실패.
- `daylog/2026-05-17.md`, 2026-05-17 KST: Unity 자동 검증, Unity GUI 리허설, Linux/Windows 빌드 재실행은 수행하지 않음. 실제 GUI에서 `Quick Relay`, 수동 `Queue → Relay → 10x`, Reset/Play 재진입, 발표 해상도 UI 확인 필요가 남음.
- `daylog/2026-05-18.md`, 2026-05-18 KST: Unity 재실행 계열 작업은 여전히 `LicensingClient` IPC 문제로 막힘. 새 Unity 자동 검증, GUI 리허설, Linux/Windows 빌드 재실행 근거 없음.
- `orbitshare-validation.md`, 2026-05-18 KST 확인: `Result: PASS`, Fast demo mission complete, relay lock/delivery 100%, Runtime Error/Exception 0. 단, `Screen: 640x480`이고 Reset 2회차, Runtime Canvas 1600x900, IMGUI fallback disabled, PresentationDirector camera cuts 항목은 없음.
- `Assets/Scripts/UIManager.cs`, 2026-05-18 KST 확인: Canvas 기준 해상도 1600x900, `Queue`, `Relay`, `Reset`, `Quick Relay`, 시간/카메라 버튼, timeline/result panel 구현됨.
- `Assets/Scripts/ImmediateUIManager.cs`, 2026-05-18 KST 확인: `showFallbackOverlay` 기본 false이며 fallback overlay는 1600x900 기준 좌표를 화면 크기에 맞춰 스케일링함.
- `Assets/Scripts/PresentationDirector.cs`, 2026-05-18 KST 확인: Reserved=Overview, Running/Transmitting=Satellite, Observing/ObservationComplete=Mission, Complete=Overview로 자동 카메라 컷 처리.
- `Assets/Editor/OrbitShareEditorAutomation.cs`, 2026-05-18 KST 확인: 최신 검증 코드는 Runtime Canvas 1600x900, IMGUI fallback disabled, Reset replay second complete, PresentationDirector camera cuts valid를 리포트하도록 되어 있으나 최신 리포트 갱신은 아직 없음.
- `orbitshare-playmode.png`, `orbitshare-demo.gif`, `orbitshare-contact-sheet.png`, `DemoFrames/`, 2026-05-18 KST 확인: 1600x900 PNG, 640x196 GIF, 960x610 contact sheet, 13개 demo frame 존재. 최신 UI/skybox/station 상태 반영 여부는 GUI 확인 필요.

## 내일 목표 후보
- Unity LicensingClient와 GUI/DISPLAY가 되는 환경에서 Play Mode 진입 가능 여부를 최우선으로 확정한다.
- 최신 `OrbitShareEditorAutomation.ValidateMissionFlow`를 재실행해 r1 추가 UI/Presentation 검증 항목이 포함된 PASS 리포트를 확보한다.
- 발표 해상도 또는 1600x900에서 Canvas UI 패널, 버튼, timeline/result panel의 잘림·겹침을 실제 화면 기준으로 확인한다.
- `Quick Relay` 단일 클릭 흐름을 Mission Complete까지 리허설하고 2~3분 발표 흐름으로 설명 가능한지 기록한다.
- 수동 `Queue` → `Relay` → `10x` 흐름을 Mission Complete까지 수행해 README의 Demo Flow와 실제 UI 용어가 맞는지 확인한다.
- Reset 후 재실행과 Play Mode 재진입을 확인해 발표 중 재시도 안정성을 검증한다.
- `PresentationDirector` 카메라 컷이 단계별로 발표 가독성을 해치지 않는지 확인한다.
- 실제 문제가 재현될 때만 `UIManager`, `ImmediateUIManager`, `PresentationDirector`, `CameraManager` 범위에서 최소 수정한다.
- GUI 리허설 통과 후 기존 GIF/DemoFrames가 최신 발표 화면을 반영하는지 판단하고, 필요할 때만 frame capture 갱신을 build/validation lane과 조율한다.

## 상세 체크리스트 초안
- [ ] Unity GUI 실행 환경 확인 → 검증: LicensingClient IPC, DISPLAY/GUI 세션, `Assets/Scenes/MainScene.unity` Play 진입 가능 여부를 기록한다.
- [ ] 최신 자동 검증 재실행 → 검증: `orbitshare-validation.md`에 `Result: PASS`, Runtime Canvas 1600x900, IMGUI fallback disabled, Reset replay 2 runs, PresentationDirector camera cuts PASS가 포함되는지 확인한다.
- [ ] 발표 해상도 또는 1600x900 Play Mode 화면 QA → 검증: Relay Control, Relay Details, Relay Request, Relay Status, timeline, Time/Camera, result panel이 잘리거나 겹치지 않는지 확인한다.
- [ ] `Quick Relay` 발표 리허설 → 검증: 단일 클릭 후 relay lock, delivery, Mission Complete까지 도달하고 Runtime Error/Exception이 없는지 기록한다.
- [ ] 수동 Demo Flow 수행 → 검증: 목적지 선택 후 `Queue` → `Relay` → `10x` 순서로 Mission Complete까지 진행하고 README 용어와 버튼명이 충돌하지 않는지 확인한다.
- [ ] Reset 후 재실행 확인 → 검증: Mission Complete 후 Reset, 다시 `Quick Relay` 실행 시 버튼 활성 상태, 패널 갱신, timeline, result summary가 정상인지 확인한다.
- [ ] Play Mode 종료 후 재진입 확인 → 검증: 두 번째 Play에서도 UI 이벤트 중복, stale state, 예외 없이 `Quick Relay`가 완료되는지 확인한다.
- [ ] `PresentationDirector` 카메라 컷 QA → 검증: Reserved/Running/Observing 또는 ObservationComplete/Transmitting/Complete 단계별 카메라 모드와 화면 가독성을 기록한다.
- [ ] IMGUI fallback 기본 상태 확인 → 검증: `showFallbackOverlay=false`에서 Canvas UI만 보이고 중복 overlay가 없는지 확인한다.
- [ ] `Fast Demo`와 `Quick Relay` 용어 정리 필요 여부 확인 → 검증: README, WORK_LOG, 발표 Q&A에서 자동 검증 내부 용어와 실제 버튼명이 헷갈리지 않는지 확인한다.
- [ ] 데모 프레임/GIF 갱신 필요 여부 결정 → 검증: 기존 `orbitshare-demo.gif`/`DemoFrames/`가 최신 UI와 발표 화면을 반영하지 않으면 `CaptureDemoFrames` 갱신 대상으로 분류한다.
- [ ] UI 문제 재현 시 최소 수정 → 검증: 수정 전후 화면 캡처 또는 리허설 기록으로 겹침/잘림/카메라 문제가 해소됐는지 확인한다.
- [ ] UI/Demo QA 결과를 merge/daylog에 전달 → 검증: 실행 항목, PASS/FAIL, 미실행 사유, 수정 파일, 남은 확인 필요를 분리해 기록한다.

## 리스크/확인 필요
- 현재 최신 Unity 실행은 `IPC channel to LicensingClient doesn't exist; aborting`으로 막힌 기록이 있어 GUI 리허설과 r1 자동 검증 PASS를 아직 단정할 수 없음.
- 기존 `orbitshare-validation.md`는 640x480 기준 이전 PASS라 발표 해상도 UI 품질 근거로는 부족함.
- 실제 발표 해상도가 1600x900이 아니면 Canvas UI와 IMGUI fallback 배치를 해당 해상도에서 다시 확인해야 함.
- 문서에는 `Fast Demo`, 실제 버튼은 `Quick Relay`가 혼용되어 있어 발표자 설명과 제출 문서에서 구분이 필요할 수 있음.
- `orbitshare-demo.gif`는 640x196이라 제출용 시연 영상으로 충분한지, 최신 화면 상태를 반영하는지 불확실함.
- GUI 리허설, 자동 검증, frame capture, 빌드는 Unity project lock과 로그 충돌 위험이 있어 병렬 실행하면 안 됨.
- UI 수정은 실제 문제 재현 전에는 보류하는 것이 맞음. 현재 근거만으로 선제 리팩터링할 필요는 없음.
- PPT/PDF, MP4/MOV 필수 여부는 spec/submission lane과 조율 필요.

## 병렬 에이전트 활용 메모
- 하위/병렬 에이전트는 사용하지 않았음.
- 이유: 이번 lane note는 기존 문서, 최근 daylog, 관련 코드, 검증 로그의 정적 확인이 중심이라 별도 에이전트로 나눌 만큼 독립 하위 문제가 크지 않았음.
- 통합 결론: 2026-05-19 UI lane은 새 기능 추가보다 GUI 환경 확보, 최신 자동 검증 리포트 갱신, 실제 발표 리허설, Reset/Play 재진입 안정성, 발표 해상도 가독성 확인이 핵심임.