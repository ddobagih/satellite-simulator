# Computer Graphics Assignment 1 - Build/Validation Automation lane note (2026-05-21)

## 최근 진행 근거
- 2026-05-20 확인: 저장소 내 별도 `AGENTS.md`, `docs/`, `PM/` 파일/디렉터리는 없고, 사용자 제공 AGENTS 지침을 기준으로 삼아야 함.
- 2026-05-20 확인: `plans/features`, `plans/verify/ready`, `plans/verify/reports`, `PM/status`, `PM/reports` 산출물은 없음. 03:00 feature/verify scheduler, product audit, automation metrics 근거는 부재로 기록 필요.
- `product/decisions.md`(2026-05-18): 제출 기준은 Unity 프로젝트 자체. Windows 빌드, MP4/PPT, `Submission/` 패키징, 새 Asset Store/SFX/LFS 작업은 dropped.
- `product/done-criteria.md`: Unity Batch, Display Smoke, GUI E2E, Build, Submission 증거 등급을 분리해야 하며 기존 Linux build success를 최신 재빌드 success로 쓰면 안 됨.
- `plans/daily/2026-05-21.md`(작성일 2026-05-20): 신규 feature slice는 “선택 위성 발신 가능/불가능 사유 표시”이며, build lane은 feature slice 후 자동 validation, GUI E2E 단일 runner, Linux rebuild/deferred 판단을 맡음.
- `ProjectSettings/ProjectVersion.txt`: Unity `2022.3.62f3 (96770f904ca7)`.
- `ProjectSettings/EditorBuildSettings.asset`: enabled scene은 `Assets/Scenes/MainScene.unity`.
- `Assets/Editor/BuildOrbitShare.cs`: `BuildLinux`, `BuildWindows`, `PrepareSubmissionFolder` 메뉴가 있음. 단 `PrepareSubmissionFolder` 체크리스트는 현재 dropped된 실행 파일/PPT/영상 표현을 포함함.
- `Assets/Editor/OrbitShareEditorAutomation.cs`: fast demo 2회, Runtime Canvas 1600x900 reference, IMGUI fallback disabled, PresentationDirector camera cuts, Runtime Error/Exception 수를 검증함.
- `orbitshare-validation.md`(2026-05-20 20:56 KST): `Result: PASS`, 24개 위성, 기본 목적지 New York, fast demo complete, relay/delivery 100%, reset replay 2회, Runtime Error/Exception 0. 단 `Screen: 640x480`이라 발표 해상도 GUI E2E 근거는 아님.
- `orbitshare-playmode.png`(2026-05-20 20:57 KST): 1600x900 캡처 근거. 화면 판독 근거이지 실제 버튼 조작/오디오 청취 완료 근거는 아님.
- `unity-build.log`(2026-05-15): `BuildOrbitShare.BuildLinux`, `Build Finished, Result: Success.` 확인. `Builds/Linux` 산출물 시각은 2026-05-10~2026-05-15로 섞여 최신 코드 재빌드 근거가 아님.
- `build-run-live.log`(2026-05-18): rootful Xwayland fallback으로 1280x720 display와 `GfxDevice` 생성 확인. display smoke 근거이며 수동 GUI E2E 근거는 아님.
- `daylog/2026-05-20.md`: 실제 Editor GUI 수동 E2E, 최신 Linux 재빌드, Windows 빌드는 아직 확인 필요로 남아 있음.

## 내일 목표 후보
- P0: preflight 사유 표시 feature slice 이후 자동 validation과 GUI E2E 증거를 같은 날짜 기준으로 분리 확보한다.
- P0: Unity 실행 작업은 단일 runner로 순차 운영해 project lock/log 충돌을 막는다.
- P1: 최신 Linux 재빌드를 실행할지, Unity 프로젝트 제출 기준으로 deferred 유지할지 결정하고 근거를 남긴다.
- P1: 필요 시 `run_orbitshare.sh` display smoke를 재확인하되 GUI E2E 완료로 쓰지 않는다.
- P2: Windows build, `PrepareSubmissionFolder`, MP4/PPT 생성은 제출 기준 변경 전까지 실행하지 않는다.

## 상세 체크리스트 초안
- [ ] scheduler 산출물 부재 재확인 → 검증: `plans/features`, `plans/verify/ready`, `plans/verify/reports`, `PM/` 파일 수를 기록한다.
- [ ] Unity 실행 preflight → 검증: Unity `2022.3.62f3`, `MainScene.unity`, LicensingClient, project lock, `DISPLAY`/Xwayland 상태를 로그에 남긴다.
- [ ] preflight feature slice 후 자동 validation 실행 → 검증: `OrbitShareEditorAutomation.ValidateMissionFlow`가 `Result: PASS`, Runtime Error/Exception 0, reset replay 2회를 유지한다.
- [ ] preflight 검증 보강 여부 판단 → 검증: core/UI 사유 API가 안정되면 validation report에 가능/불가능 샘플을 추가하고, 불안정하면 report-only matrix로 대체한다.
- [ ] Editor GUI E2E 단일 runner 지원 → 검증: UI/demo 담당의 `빠른 발신`, `위성 확정 → 메시지 발신`, Reset/재진입 결과와 로그 파일명을 분리 기록한다.
- [ ] Linux 최신 재빌드 실행 또는 deferred 기록 → 검증: 실행 시 `Build Finished, Result: Success.`와 최신 timestamp를 확인하고, 보류 시 Unity 프로젝트 제출 기준 사유를 남긴다.
- [ ] Linux player display smoke 재확인 여부 결정 → 검증: 실행 시 1280x720 display, `GfxDevice` 생성, gameplay `Error`/`Exception`/`Crash` 부재를 확인한다.
- [ ] Resource Hook 검증은 Visual lane 요청 시만 순차 실행 → 검증: `asset-store-import-report.md`와 `Assets/Resources/OrbitShare`를 대조하고 optional missing은 실패로 쓰지 않는다.
- [ ] Windows/Submission 제외 유지 → 검증: 제출 기준 변경이 없으면 `BuildWindows`, `PrepareSubmissionFolder`를 실행하지 않고 dropped 사유를 기록한다.
- [ ] 결과 handoff 작성 → 검증: 실행 명령, 로그 파일, 성공/실패, 미실행 사유를 spec/worklog 담당에게 전달한다.

## 리스크/확인 필요
- 실제 Unity Editor GUI 버튼 조작 E2E는 아직 확인 필요. batch validation과 display smoke로 완료 처리하면 안 됨.
- 최신 Linux build는 미확인. 기존 build 성공은 과거 근거로만 사용.
- `orbitshare-validation.md`의 `Screen: 640x480`은 발표 해상도 QA 근거가 아님.
- Unity validation/build/capture/resource hook 병렬 실행은 project lock, SessionState, 로그 충돌 위험이 큼.
- Windows Build Support 설치 여부는 확인되지 않음. 현재 dropped라 정적 확인 이상의 작업은 불필요.
- `BuildOrbitShare.PrepareSubmissionFolder()`의 PPT/영상/실행 파일 체크리스트는 최신 product 결정과 충돌 가능.
- safe alternative: GUI가 막히면 batch validation + 1600x900 캡처 + display smoke를 보조 근거로 기록하고 GUI E2E는 todo 유지.
- safe alternative: Windows/Submission 요구가 바뀌면 즉시 실행하지 말고 Build Support, 예상 산출물, 포함/제외 파일을 먼저 dry-run/정적 인벤토리로 확인.

## 병렬 에이전트 활용 메모
- 하위/병렬 에이전트는 사용하지 않음.
- 이유: 이번 lane note는 문서, 로그, editor script 대조 범위로 충분했고 Unity 실행 계열은 병렬화 금지 대상임.
- 파일 확인은 병렬 shell 호출로 처리했고, 결론은 build/validation lane 관점에서 단일 note로 통합함.