# Computer Graphics Assignment 1 - Build/Validation Automation lane note (2026-05-22)

## 최근 진행 근거
- 사용자 제공 AGENTS 지침: 기본 응답/산출물은 한국어, 추정 금지, 실제 실행하지 않은 검증은 완료로 쓰지 않음.
- `product/roadmap.md`(2026-05-18 기준): 현재 모드는 `concept-update/submission-readiness`; 핵심 남은 일은 실제 Unity Editor Play Mode 수동 리허설과 제출 직전 정적 확인.
- `product/decisions.md`(2026-05-18): 제출은 Unity 프로젝트 자체. Windows 빌드, MP4/MOV, PPT/PDF, `Submission/` 패키징은 dropped.
- `product/done-criteria.md`: Unity Batch, Display Smoke, Visual Review, GUI E2E, Build 증거 등급을 분리해야 함. 기존 Linux build success를 최신 재빌드 success로 쓰면 안 됨.
- `plans/daily/2026-05-21.md`: 다음 신규 feature slice는 `선택 위성이 왜 발신 가능/불가능한지` preflight 사유 표시. build lane은 feature slice 후 자동 validation, GUI E2E 단일 runner, Linux rebuild/deferred 판단을 맡음.
- `daylog/2026-05-21.md`: 2026-05-21에는 정적 조사만 수행. Unity batch validation, Editor GUI E2E, Linux 재빌드, 캡처 재생성은 실행하지 않음.
- `plans/features`, `plans/verify`, `plans/verify/ready`, `plans/verify/reports`는 디렉터리만 있고 파일 없음. `PM/status`, `PM/reports`도 없음. feature/verify scheduler, product audit, automation metrics 근거는 부재.
- `plans/daily/2026-05-22.md`는 없음.
- `orbitshare-validation.md`(2026-05-20 20:56 KST): `Result: PASS`, 위성 24개, fast demo 2회, Runtime Error/Exception 0. 단 `Screen: 640x480`이라 발표 해상도 GUI E2E 근거는 아님.
- `orbitshare-playmode.png`(2026-05-20 20:57 KST): 1600x900 캡처 근거. 실제 버튼 조작/오디오 청취 완료 근거는 아님.
- `Assets/Editor/BuildOrbitShare.cs`: `BuildLinux`, `BuildWindows`, `PrepareSubmissionFolder` 존재. `PrepareSubmissionFolder` 체크리스트는 현재 dropped된 실행 파일/PPT/영상 표현을 포함해 최신 product 결정과 충돌 가능.
- `unity-build.log`(2026-05-15): Linux build success 확인. `Builds/Linux` 산출물 시각은 2026-05-10~2026-05-15로 최신 코드 재빌드 근거는 아님.
- `run_orbitshare.sh`, `build-run-live.log`(2026-05-18): rootful Xwayland fallback display smoke 근거. 수동 GUI E2E 근거는 아님.

## 내일 목표 후보
- P0: preflight 사유 표시 feature slice가 들어간 뒤 `OrbitShareEditorAutomation.ValidateMissionFlow`를 재실행해 기존 PASS 항목과 Runtime Error/Exception 0을 같은 날짜 기준으로 다시 확보한다.
- P0: 가능/불가능 사유 API가 안정되면 validation report에 최소 샘플 matrix를 추가한다. 불안정하면 UI 변경 없이 report-only matrix를 safe alternative로 남긴다.
- P0: Unity 실행 작업을 단일 runner로 순차 운영한다. 자동 validation, screenshot, Resource Hook, GUI E2E, build를 병렬 실행하지 않는다.
- P1: Editor GUI E2E를 지원하되, batch validation/display smoke와 완료 등급을 분리해 기록한다.
- P1: Linux 최신 재빌드는 실행 가치가 있으면 수행하고, Unity 프로젝트 제출 기준이면 deferred 유지 사유를 남긴다.
- P2: Windows build, `PrepareSubmissionFolder`, MP4/PPT 생성은 제출 기준 변경 전까지 실행하지 않는다.

## 상세 체크리스트 초안
- [ ] scheduler/PM 산출물 부재 재확인 → 검증: `plans/features`, `plans/verify/ready`, `plans/verify/reports`, `PM/status`, `PM/reports` 파일 수를 기록하고 없는 산출물을 근거로 쓰지 않는다.
- [ ] Unity 실행 preflight 확인 → 검증: Unity `2022.3.62f3`, `Assets/Scenes/MainScene.unity`, LicensingClient 연결, project lock, `DISPLAY`/Xwayland 상태를 로그에 남긴다.
- [ ] preflight feature slice 후 자동 validation 실행 → 검증: `orbitshare-validation.md`가 새 시각으로 갱신되고 `Result: PASS`, fast demo 2회, reset replay, Runtime Error/Exception 0을 유지한다.
- [ ] preflight 검증 보강 판단 → 검증: 가능/불가능 사유 샘플이 안정적으로 조회되면 validation report에 추가하고, 불안정하면 별도 matrix만 기록한다.
- [ ] GUI E2E 단일 runner 지원 → 검증: `빠른 발신`, `위성 확정 → 메시지 발신`, Reset, Play Mode 재진입 결과와 로그 파일명을 분리 기록한다.
- [ ] screenshot 재생성 필요 여부 결정 → 검증: UI 변경이 있으면 1600x900 캡처를 갱신하고, 없으면 기존 캡처를 정적 근거로만 유지한다.
- [ ] Linux 최신 재빌드 실행/보류 결정 → 검증: 실행 시 `BuildOrbitShare.BuildLinux` 로그에 `Build Finished, Result: Success.`와 최신 timestamp가 남는다. 보류 시 Unity 프로젝트 제출 기준의 deferred 사유를 기록한다.
- [ ] Linux player display smoke 재확인 여부 결정 → 검증: 실행 시 `run_orbitshare.sh` 로그에서 1280x720 display, `GfxDevice` 생성, gameplay Error/Exception/Crash 부재를 확인한다.
- [ ] Windows/Submission 제외 유지 → 검증: 요구 변경이 없으면 `BuildWindows`, `PrepareSubmissionFolder`를 실행하지 않고 dropped 사유를 기록한다.
- [ ] 결과 handoff 작성 → 검증: 실행 명령, 로그 파일, PASS/FAIL, 미실행 사유를 spec/worklog 담당에게 전달한다.

## 리스크/확인 필요
- 실제 Unity Editor GUI 버튼 조작 E2E는 아직 확인 필요. safe alternative는 batch validation + 1600x900 screenshot + display smoke지만, GUI E2E 완료로 쓰면 안 됨.
- `orbitshare-validation.md`의 `Screen: 640x480`은 발표 해상도 QA 근거가 아님.
- 최신 Linux 재빌드는 미확인. 기존 build success는 과거 근거로만 사용.
- Unity validation/build/capture/resource hook 병렬 실행은 project lock, SessionState, 로그 충돌 위험이 큼. safe alternative는 단일 Unity runner 순차 실행.
- preflight 사유 API가 core/UI에서 아직 확정되지 않았을 수 있음. safe alternative는 validation 코드 변경 없이 문서/report-only matrix로 먼저 남기는 것.
- Windows Build Support 설치 여부는 확인되지 않았지만 현재 dropped라 직접 확인/설치/빌드 계획으로 올리지 않음.
- `BuildOrbitShare.PrepareSubmissionFolder()`의 체크리스트는 최신 product 결정과 충돌 가능. 실행보다 문서/코드 표현 정리 후보로 전달하는 편이 안전함.

## 병렬 에이전트 활용 메모
- 하위/병렬 에이전트는 사용하지 않음.
- 이유: 이번 lane note는 product 문서, daylog, 기존 계획, editor script, validation/build 로그 대조로 충분했고 Unity 실행 계열은 병렬화 금지 대상임.
- 파일 확인은 병렬 shell 호출로 처리했고, 결론은 build/validation lane 관점에서 단일 note로 통합함.