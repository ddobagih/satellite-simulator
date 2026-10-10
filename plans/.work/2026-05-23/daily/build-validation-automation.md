# Computer Graphics Assignment 1 - Build/Validation Automation lane note (2026-05-24)

## 최근 진행 근거
- `product/roadmap.md`, `product/done-criteria.md`, `product/decisions.md`(2026-05-18 결정 기준): 현재 제출 기준은 Unity 프로젝트 자체이며 Windows 빌드, MP4/MOV, PPT/PDF, `Submission/` 패키징은 dropped. 완료 근거는 Unity Batch, Display Smoke, Visual Review, GUI E2E, Build를 분리해야 함.
- `plans/features`, `plans/verify/ready`, `plans/verify/reports`, `PM/status`, `PM/reports`(2026-05-23 조사): 디렉터리 또는 경로는 있으나 파일 없음. feature/verify scheduler, product audit, automation metrics 산출물은 근거로 인용 불가.
- `daylog/2026-05-22.md`: 단계형 발신 플로우, 드래그 오버레이, 메시지/첨부, 대기열/기록/재전송, 실패 시뮬레이션, 혼잡도, 추천 발신 시간, 전송 로그 등 신규 UX 기능이 추가됨.
- `orbitshare-validation.md`(2026-05-22 22:04 KST): `Result: PASS`, `Screen: 640x480`, 위성 24개, Fast Demo 2회, reset replay, Runtime Error/Exception 0.
- `unity-build-more-features.log`(2026-05-22 22:04 KST): `Build Finished, Result: Success.` 확인. 다만 `Builds/Linux/OrbitShare.x86_64`와 `UnityPlayer.so` mtime은 2026-05-10이고 `OrbitShare_Data` 일부만 2026-05-22로 갱신되어 산출물 timestamp 해석 주의 필요.
- `daylog/2026-05-23.md`: 신규 코드/에셋 변경은 없고 정적 조사만 수행. Unity batch validation, Linux/Windows 빌드, GUI E2E, screenshot capture, display smoke는 새로 실행하지 않음.
- `Assets/Editor/OrbitShareEditorAutomation.cs`: 현재 자동 검증은 Fast Demo 완료, reset replay, 24개 위성, 기본 runtime 상태 중심이다. 2026-05-22에 추가된 대기열/기록/실패/로그/목적지 직접 클릭 UX까지 검증한다고 단정할 근거는 없음.
- `Assets/Editor/BuildOrbitShare.cs`: `BuildLinux`, `BuildWindows`, `PrepareSubmissionFolder`가 있으나 `PrepareSubmissionFolder` 체크리스트는 built executable/PPT/demo video를 요구하는 과거 표현을 포함해 최신 product 결정과 충돌 가능.
- `orbitshare-playmode.png`(2026-05-20 20:57 KST): 1600x900 캡처지만 2026-05-22 신규 UX 이전 화면이다. `daylog/2026-05-22.md`에는 이후 `CapturePlayScreenshot`의 `Camera.Render()` native crash 재발이 기록됨.

## 내일 목표 후보
- P0: 신규 automation feature slice로 `ValidateMissionFlow` 리포트에 2026-05-22 UX 핵심 상태를 최소 추가한다. 우선 범위는 대기열/기록/재전송/실패 상태/전송 로그가 “생성 가능하고 runtime exception 없이 유지되는지” 확인하는 product UX sanity section.
- P0: automation 변경 또는 코드 변경 후 Unity 작업은 단일 runner로 순차 실행한다. validation, screenshot/capture, Resource Hook, Linux build, display smoke를 병렬 실행하지 않는다.
- P0: 최신 validation과 Linux build 근거를 2026-05-24 기준으로 다시 확보하거나, 환경 차단/보류 사유를 명확히 남긴다.
- P1: 최신 화면 근거를 갱신한다. `Camera.Render()` 경로가 다시 crash하면 `CaptureDemoFrames`/player window/manual screenshot 같은 safe alternative를 쓰고 Visual Review 완료로 과대해석하지 않는다.
- P1: Linux build 산출물 manifest를 남겨 로그 success와 실제 파일 mtime/존재를 함께 확인한다.
- P2: Windows build, `PrepareSubmissionFolder`, 외부 업로드, 새 Asset Store import, Git LFS는 제출 요구 변경 전 실행하지 않는다.

## 상세 체크리스트 초안
- [ ] scheduler/PM 산출물 재확인 → 검증: `plans/features`, `plans/verify/ready`, `plans/verify/reports`, `PM/status`, `PM/reports` 파일 수를 기록하고 없으면 “근거 없음”으로 둔다.
- [ ] Unity 실행 preflight 확인 → 검증: Unity `2022.3.62f3`, `Assets/Scenes/MainScene.unity`, LicensingClient 연결, `Temp/UnityLockfile`, `DISPLAY`/Xwayland 상태를 로그에 남긴다.
- [ ] automation feature slice 설계 → 검증: `OrbitShareEditorAutomation.ValidateMissionFlow`가 기존 Fast Demo PASS에 더해 대기열/기록/실패/전송 로그의 최소 상태만 검사하도록 범위를 제한한다.
- [ ] automation feature slice 구현 후 validation 실행 → 검증: `orbitshare-validation.md`가 새 시각으로 갱신되고 `Result: PASS`, Runtime Error/Exception 0, reset replay 2회를 유지한다.
- [ ] 신규 UX 검증이 불안정하면 report-only safe alternative 적용 → 검증: 코드 검증 대신 목적지/발신 시각/상태별 matrix와 미검증 사유를 work note/daylog에 남긴다.
- [ ] screenshot 갱신 경로 결정 → 검증: 1600x900 캡처가 성공하면 파일 mtime과 판독 필요 상태를 기록하고, 실패하면 crash 로그와 대체 증거 경로를 남긴다.
- [ ] Linux build 재실행 또는 보류 결정 → 검증: 실행 시 `BuildOrbitShare.BuildLinux` 로그의 `Build Finished, Result: Success.`와 `Builds/Linux` 산출물 mtime manifest를 함께 기록한다.
- [ ] display smoke 재확인 여부 결정 → 검증: 실행 시 `run_orbitshare.sh` 또는 동등 명령의 로그 파일 위치를 고정하고 1280x720 display, `GfxDevice` 생성, gameplay Error/Exception/Crash 부재를 확인한다.
- [ ] Resource Hook 검증 실행/보류 결정 → 검증: 실행 시 `asset-store-import-report.md` mtime과 `SpaceSkybox.mat`, `SpaceStation.prefab` FOUND 및 optional MISSING 상태를 기록한다.
- [ ] Windows/Submission 제외 유지 → 검증: 요구 변경이 없으면 `BuildWindows`, `PrepareSubmissionFolder`를 실행하지 않고 product decision 기준 dropped 사유를 유지한다.
- [ ] 결과 handoff 작성 → 검증: 실행 명령, 로그 파일명, PASS/FAIL, 미실행 사유, 오래된 캡처/빌드 근거의 한계를 spec-worklog lane에 전달한다.

## 리스크/확인 필요
- 실제 Unity Editor GUI에서 사람이 단계형 발신, 드래그 패널, 결과 닫기/새 메시지, 기록 재전송/삭제, 대기열, 실패 후 재시도를 조작한 근거는 아직 없음.
- `orbitshare-validation.md`의 `Screen: 640x480`은 발표 해상도 GUI E2E 근거가 아님.
- 최신 1600x900 캡처는 2026-05-22 신규 UX 이전 화면이다. 최신 UX 시각 증거는 갱신 필요.
- screenshot capture는 `Camera.Render()` native crash 재발 이력이 있어, 실패 시 자동 캡처 문제와 앱 runtime 문제를 분리해야 함.
- Linux build success 로그는 최신이나 executable mtime이 오래되어 산출물 manifest 없이 “최신 실행 파일 전체 갱신”으로 쓰면 위험함.
- 작업트리에 대규모 tracked 삭제, modified 코드, untracked 신규 shader/script/daylog가 섞여 있음. 계획상 revert 없이 포함/제외 상태만 기록한다.
- `BuildOrbitShare.PrepareSubmissionFolder()`는 최신 제출 결정과 충돌 가능하지만, 내일 build lane에서는 실행하지 않고 문서/코드 정리 후보로만 둔다.

## 병렬 에이전트 활용 메모
- 이번 lane note 작성에는 하위/병렬 에이전트를 사용하지 않음.
- 이유: 근거 확인 범위가 product/daylog/plan/log/editor script 대조로 충분했고, Unity 실행 계열 작업은 project lock과 로그 충돌 때문에 병렬화 금지 대상임.
- 파일 조회는 병렬 shell 호출로 처리했고, 결론은 build-validation lane 관점에서 단일 note로 통합함.