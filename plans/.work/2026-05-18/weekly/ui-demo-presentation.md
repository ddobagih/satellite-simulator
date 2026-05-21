# Computer Graphics Assignment 1 - UI/Demo Flow/Presentation weekly lane note (2026-W21)

## 이번 주 목표 후보

- 최신 Unity 실행 환경에서 UI/Demo/Presentation 검증 근거를 새로 확보한다.
- `Quick Relay` 단일 클릭 흐름과 수동 `Queue` → `Relay` → `10x` 흐름이 Mission Complete까지 안정적으로 도달하는지 실제 GUI에서 확인한다.
- 발표 해상도 또는 1600x900 기준으로 `Relay Control`, `Relay Details`, `Relay Request`, `Relay Status`, timeline, Time/Camera, result panel의 겹침·잘림을 확인한다.
- `PresentationDirector` 카메라 컷이 발표 가독성을 해치지 않는지 단계별로 확인한다.
- Reset 후 재실행과 Play Mode 재진입에서 버튼 상태, 패널 갱신, timeline/result summary, 이벤트 중복 문제가 없는지 확인한다.
- `Fast Demo`는 자동 검증 내부 흐름, `Quick Relay`는 실제 UI 버튼이라는 구분을 README/WORK_LOG/발표 Q&A에서 일관되게 정리한다.
- 실제 문제가 재현될 때만 `UIManager`, `ImmediateUIManager`, `PresentationDirector`, `CameraManager` 범위에서 최소 수정한다.

근거:
- `plans/weekly/2026-W21.md`는 현재 확인되지 않았다.
- `docs/` 경로도 확인되지 않아 `README.md`, `WORK_LOG.md`, `orbitshare-validation.md`, `orbitshare_shared_satellite_agent.md`, 최근 daylog와 lane note를 기준으로 계획했다.
- 기존 `orbitshare-validation.md`는 `Result: PASS`지만 `Screen: 640x480` 기준이며, Reset 2회차/1600x900 Canvas/IMGUI fallback disabled/PresentationDirector camera cuts 항목은 아직 리포트에 반영되지 않았다.
- 최근 daylog 기준 Unity 재실행 계열 작업은 `LicensingClient` IPC 문제로 막힌 기록이 있고, GUI 리허설은 아직 직접 PASS 근거가 부족하다.

## 날짜별/단계별 체크리스트

- [ ] 2026-05-18 월: 주간 기준선 정리 → 검증: 최근 daylog, lane note, README/WORK_LOG, 기존 validation report 기준으로 UI/Demo 미완료 항목과 확인 필요 항목을 분리한다.
- [ ] 2026-05-19 화: Unity GUI 실행 preflight → 검증: LicensingClient IPC, DISPLAY/GUI 세션, `Assets/Scenes/MainScene.unity` Play 진입 가능 여부를 기록한다.
- [ ] 2026-05-19 화: 최신 자동 검증 재실행 → 검증: `orbitshare-validation.md`에 `Result: PASS`, Runtime Canvas 1600x900, IMGUI fallback disabled, Reset replay 2 runs, PresentationDirector camera cuts PASS가 포함되는지 확인한다.
- [ ] 2026-05-20 수: 발표 화면 QA → 검증: 1600x900 또는 실제 발표 해상도에서 주요 패널, 버튼, timeline/result panel의 겹침·잘림이 없는지 확인한다.
- [ ] 2026-05-20 수: `Quick Relay` 리허설 → 검증: 단일 클릭 후 relay lock, delivery, Mission Complete까지 도달하고 Runtime Error/Exception이 없는지 기록한다.
- [ ] 2026-05-21 목: 수동 Demo Flow 리허설 → 검증: 목적지 선택 후 `Queue` → `Relay` → `10x` 순서가 README와 실제 UI에서 충돌하지 않는지 확인한다.
- [ ] 2026-05-21 목: `PresentationDirector` 카메라 컷 QA → 검증: Reserved=Overview, Running/Transmitting=Satellite, Observing/ObservationComplete=Mission, Complete=Overview 컷이 화면 설명을 방해하지 않는지 기록한다.
- [ ] 2026-05-22 금: Reset/Play 재진입 안정성 확인 → 검증: Mission Complete 후 Reset, 두 번째 `Quick Relay`, Play 종료 후 재진입에서 stale state, 중복 이벤트, UI 갱신 문제, 예외가 없는지 확인한다.
- [ ] 2026-05-22 금: 데모 프레임/GIF 갱신 필요 여부 결정 → 검증: 기존 `orbitshare-demo.gif`/`DemoFrames/`가 최신 UI와 skybox/station 상태를 반영하는지 확인하고, 부족하면 capture 갱신을 build/validation lane과 순차 조율한다.
- [ ] 2026-05-23 토: 발표 자료 연동 점검 → 검증: README, WORK_LOG, 발표 Q&A에서 `Fast Demo`/`Quick Relay`, 기존 PASS와 최신 검증 결과, 미실행 검증이 명확히 구분되는지 확인한다.
- [ ] 2026-05-24 일: 제출 전 UI freeze 점검 → 검증: 새 UI 문제 없으면 코드 수정 없이 검증/문서 근거만 확정하고, 문제가 재현된 경우에만 최소 수정 후 같은 리허설을 다시 통과시킨다.

## 검증 계획

- 자동 검증: `OrbitShareEditorAutomation.ValidateMissionFlow`를 Unity LicensingClient가 동작하는 환경에서 실행한다.
- GUI 리허설: `Quick Relay`와 수동 `Queue` → `Relay` → `10x`를 각각 Mission Complete까지 확인한다.
- 화면 QA: 발표 해상도 또는 1600x900에서 Canvas UI, result panel, timeline, Time/Camera 버튼, 목적지/위성 화면 가림을 확인한다.
- 안정성 QA: Reset 후 재실행, Play Mode 종료 후 재진입, IMGUI fallback 중복 표시 없음, Runtime Error/Exception 0을 확인한다.
- 카메라 QA: `PresentationDirector` phase별 카메라 컷과 `CameraManager.CurrentMode`가 자동 검증 및 실제 화면에서 일치하는지 확인한다.
- 실행하지 않은 검증은 daylog/문서에 완료로 쓰지 않는다.

## 리스크/확인 필요

- Unity LicensingClient IPC 문제로 최신 자동 검증과 GUI 리허설이 다시 막힐 수 있다.
- 기존 PASS 리포트는 640x480 기준이라 발표 화면 품질 근거로는 부족하다.
- 실제 발표 해상도가 1600x900이 아니면 해당 해상도에서 별도 확인이 필요하다.
- `Fast Demo`와 `Quick Relay` 용어 혼용은 발표 중 혼란을 줄 수 있다.
- `orbitshare-demo.gif`는 640x196이라 제출용 시연 영상으로 충분한지 확인 필요하다.
- GUI 리허설, 자동 검증, frame capture, 빌드는 Unity project lock과 로그 충돌 위험이 있어 병렬 실행하면 안 된다.
- PPT/PDF, MP4/MOV 필수 여부는 spec/submission lane과 조율해야 한다.

## 병렬 에이전트 활용 메모

- 이번 lane note 작성에는 하위/병렬 에이전트를 사용하지 않았다.
- 정적 파일 확인은 병렬 shell 호출로 처리했고, 최종 판단은 단일 통합으로 정리했다.
- 이번 주 실제 실행 단계에서 병렬화 가능한 범위는 UI 코드 체크리스트, 문서 용어 점검, 기존 산출물 상태 조사처럼 Unity를 열지 않는 정적 작업이다.
- Unity 자동 검증, GUI 리허설, capture, 빌드는 반드시 순차 실행 대상으로 둔다.