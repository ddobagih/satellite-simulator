# Computer Graphics Assignment 1 - Build/Validation Automation lane note (2026-05-25)

## 최근 진행 근거
- 사용자 제공 AGENTS 지침(2026-05-24 요청): 별도 로컬 `AGENTS.md` 파일은 확인되지 않아, 제공된 지침을 기준으로 적용.
- `product/roadmap.md`, `product/done-criteria.md`, `product/decisions.md`(2026-05-18): 제출 기준은 Unity 프로젝트 자체. Windows 빌드, MP4/PPT, `Submission/` 패키징은 현재 dropped.
- `plans/features`, `plans/verify/ready`, `plans/verify/reports`, `PM/status`, `PM/reports`(2026-05-24 확인): 파일 0개. feature/verify scheduler, product audit, automation metrics 산출물은 근거 없음.
- `plans/daily/2026-05-24.md`(작성 2026-05-23): Build/Validation lane은 신규 UX 검증 확대, 최신 validation 재생성, Linux build/display smoke, screenshot 대체 경로를 우선으로 잡음.
- `daylog/2026-05-22.md`, `daylog/2026-05-23.md`: 대기열, 기록/재전송, 실패 시뮬레이션, 전송 로그, 목적지 확장, 저장/상태 모델, feature pack이 추가되고 여러 Linux build가 success로 기록됨.
- `daylog/2026-05-24.md`: 고해상도 지구 텍스처와 shader 연결 확인, `unity-build-earth-textures.log`, `unity-build-earth-color-fix.log` 기준 Linux build success, `build-run-live.log` 기준 1280x720 player smoke 확인. 단, 새 `ValidateMissionFlow`는 실행하지 않음.
- 현재 작업트리(2026-05-24 확인): `orbitshare-validation.md`가 삭제 상태로 보임. 기존 PASS는 daylog/log 근거로만 취급하고, 최신 report 재생성이 필요함.
- `Assets/Editor/OrbitShareEditorAutomation.cs`: 자동 검증은 Fast Demo, reset replay, 24개 위성, 기본 runtime 상태 중심. 최근 추가된 대기열/기록/실패/전송 로그 UX까지 검증한다고 보기 어려움.
- `Assets/Editor/BuildOrbitShare.cs`: `BuildLinux`, `BuildWindows`, `PrepareSubmissionFolder`가 있으나 `PrepareSubmissionFolder` 문구는 현재 product 결정과 충돌 가능.

## 내일 목표 후보
- P0: `orbitshare-validation.md` 복구/재생성. 최신 코드 기준 `ValidateMissionFlow`를 실행해 PASS/FAIL과 Runtime Error/Exception 0 여부를 다시 확보한다.
- P0: automation feature slice 추가. `OrbitShareEditorAutomation`에 대기열, 기록/영수증, 실패 시뮬레이션, 전송 로그의 최소 sanity section을 추가하거나, 불안정하면 report-only matrix로 대체한다.
- P0: Unity 실행 preflight와 단일 runner 순서 확정. validation, screenshot, Resource Hook, Linux build, display smoke는 병렬 실행하지 않는다.
- P1: Linux build evidence manifest 작성. build 로그 success와 `Builds/Linux` 산출물 존재/mtime을 함께 기록한다.
- P1: 최신 visual evidence 갱신 경로 결정. `Camera.Render()` crash가 재발하면 player/manual screenshot 또는 실행 로그 기반 smoke로 대체하되 Visual Review 완료로 쓰지 않는다.
- P2: `BuildWindows`, `PrepareSubmissionFolder`, 외부 업로드, 새 에셋 import, Git LFS는 제출 요구 변경 전 실행하지 않는다.

## 상세 체크리스트 초안
- [ ] scheduler/PM 산출물 재확인 → 검증: `plans/features`, `plans/verify/ready`, `plans/verify/reports`, `PM/status`, `PM/reports` 파일 수를 기록한다.
- [ ] Unity preflight 확인 → 검증: Unity `2022.3.62f3`, enabled scene `Assets/Scenes/MainScene.unity`, LicensingClient, `Temp/UnityLockfile`, `DISPLAY`/Xwayland 상태를 로그에 남긴다.
- [ ] 최신 validation report 재생성 → 검증: `OrbitShareEditorAutomation.ValidateMissionFlow` 실행 후 `orbitshare-validation.md`가 새 시각으로 생성되고 `Result: PASS`, Runtime Error/Exception 0, reset replay 2회를 확인한다.
- [ ] 자동화 sanity slice 설계/구현 → 검증: 가능하면 `Assets/Editor/OrbitShareEditorAutomation.cs` 범위에서 대기열 생성, 실패 상태, 재시도 가능성, 영수증/전송 로그 non-empty 여부를 report에 추가한다.
- [ ] sanity slice가 타이밍 의존적이면 safe alternative 적용 → 검증: 코드 변경 없이 queue/history/failure/log 수동 검증 matrix와 미검증 사유를 남긴다.
- [ ] Linux build 재실행 여부 결정 → 검증: 실행 시 `BuildOrbitShare.BuildLinux` 로그의 `Build Finished, Result: Success.`와 산출물 manifest를 함께 기록한다.
- [ ] display smoke 재확인 → 검증: `run_orbitshare.sh` 로그에서 1280x720 resize, GfxDevice 생성, gameplay Error/Exception/Crash 부재를 확인한다.
- [ ] screenshot/capture 경로 결정 → 검증: 1600x900 캡처 성공 시 파일 mtime과 판독 필요 상태를 기록하고, 실패 시 crash 로그와 대체 근거를 기록한다.
- [ ] Resource Hook 검증/보류 판단 → 검증: 실행 시 `asset-store-import-report.md`와 `SpaceSkybox.mat`, `SpaceStation.prefab`, optional missing 상태를 대조한다.
- [ ] dropped 항목 실행 방지 → 검증: 요구 변경 없으면 `BuildWindows`, `PrepareSubmissionFolder`를 실행하지 않고 product decision 기준 제외 사유를 유지한다.

## 리스크/확인 필요
- `orbitshare-validation.md`가 현재 삭제 상태라 최신 자동 검증 PASS를 바로 인용할 수 없음.
- 자동 검증은 GUI E2E가 아니다. 실제 Editor GUI에서 단계형 발신, 드래그 패널, 대기열, 기록, 실패 후 재시도는 별도 수동 확인 필요.
- `Camera.Render()` 기반 screenshot은 native crash 이력이 있음. safe alternative는 player/manual screenshot 또는 display smoke 로그.
- Linux build success 로그는 있으나 산출물 manifest 없이 최신 실행 파일 상태로 단정하면 위험함.
- `PrepareSubmissionFolder`는 현재 제출 기준과 충돌 가능. safe alternative는 실행 대신 정적 제출 truth table 작성.
- Windows build, MP4/PPT, `Submission/`, Git LFS, 새 Asset Store/SFX는 B/C 작업. safe alternative는 dropped/deferred 사유 유지와 정적 인벤토리.
- 작업트리에 대규모 modified/deleted/untracked 파일이 섞여 있음. 되돌리지 말고 build/validation 대상과 제출 포함 여부만 분리해야 함.
- Unity project lock/log 충돌 위험 때문에 Unity 실행 작업은 순차 실행만 허용.

## 병렬 에이전트 활용 메모
- 하위/병렬 에이전트는 사용하지 않음.
- 이유: 이번 lane note는 read-only 문서/로그/스크립트 대조로 충분했고, 실제 Unity 실행 계열 작업은 병렬 금지 대상임.
- 파일 조회는 병렬 shell 호출로 처리했고, 결론은 Build/Validation lane 관점에서 단일 note로 통합함.