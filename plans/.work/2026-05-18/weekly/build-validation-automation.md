# Computer Graphics Assignment 1 - Build/Validation Automation weekly lane note (2026-W21)

## 이번 주 목표 후보

- Unity `2022.3.62f3` 실행 환경을 복구해 `LicensingClient` IPC 실패를 먼저 해소한다.
- 기존 `orbitshare-validation.md` PASS를 재사용하지 않고, 최신 `OrbitShareEditorAutomation.ValidateMissionFlow` 기준으로 새 PASS 리포트를 확보한다.
- 최신 검증 리포트에 Reset 2회차, Runtime Canvas 1600x900, IMGUI fallback disabled, `PresentationDirector` camera cuts 항목이 포함되게 한다.
- `BuildOrbitShare.BuildLinux`를 최신 상태로 재실행해 Linux 빌드 Success 로그와 산출물을 갱신한다.
- Windows 빌드가 제출 필수인지 확정하고, 필수이면 `BuildOrbitShare.BuildWindows`와 `Builds/Windows/OrbitShare.exe`를 검증한다.
- Linux GUI runtime smoke를 headless `NullGfxDevice` 로그와 분리해 실제 실행 안정성 근거로 남긴다.
- Resource Hook 검증과 제출 폴더 생성은 Unity 실행 환경과 제출 범위가 확정된 뒤 순차 실행한다.

## 날짜별/단계별 체크리스트

- [ ] 2026-05-18 월: 기준선 정리  
  -> 검증: `daylog/2026-05-17~18.md`, `plans/daily/2026-05-18~19.md`, `README.md`, `WORK_LOG.md`, `orbitshare-validation.md`, 기존 Unity 로그를 근거로 현재 병목을 정리한다.

- [ ] 2026-05-19 화: Unity preflight 및 자동 검증 재시도  
  -> 검증: Unity 경로, `LicensingClient` IPC, `DISPLAY`, project lock 여부를 기록한 뒤 `OrbitShareEditorAutomation.ValidateMissionFlow`를 실행한다.

- [ ] 2026-05-20 수: 자동 검증 안정화  
  -> 검증: `orbitshare-validation.md`에 `Result: PASS`, Runtime Error/Exception 0, Reset replay, Canvas 1600x900, IMGUI fallback disabled, camera cuts PASS가 포함되는지 확인한다. 실패 시 환경 문제와 gameplay/runtime 예외를 분리한다.

- [ ] 2026-05-21 목: 빌드 재확인  
  -> 검증: `BuildOrbitShare.BuildLinux` 실행 후 최신 로그에 `Build Finished, Result: Success.`가 있고 `Builds/Linux/OrbitShare.x86_64`, `OrbitShare_Data`, `UnityPlayer.so`가 존재하는지 확인한다.

- [ ] 2026-05-21~22 목~금: Windows 빌드 결정 및 실행  
  -> 검증: 제출 기준상 Windows 실행 파일이 필수이면 Windows Build Support 확인 후 `BuildOrbitShare.BuildWindows`를 실행하고 `Builds/Windows/OrbitShare.exe` 존재를 확인한다. 불필요하면 제외 사유를 문서 lane에 전달한다.

- [ ] 2026-05-22 금: Linux runtime smoke 및 캡처 자동화 판단  
  -> 검증: `run_orbitshare.sh` 또는 동일 GUI 조건으로 실행해 `build-run-live.log`에서 gameplay Error/Exception/Crash가 없는지 확인한다. 기존 자료가 최신 화면과 다르면 `CapturePlayScreenshot` 또는 `CaptureDemoFrames` 재실행 대상으로 분류한다.

- [ ] 2026-05-23 토: Resource Hook 및 제출 준비 연결  
  -> 검증: `AssetStorePackageImporter.ValidateResourceHooks` 실행 후 `asset-store-import-report.md`가 실제 `Assets/Resources/OrbitShare` 상태와 일치하는지 확인한다. 제출 형식이 확정된 경우에만 `BuildOrbitShare.PrepareSubmissionFolder`를 실행한다.

- [ ] 2026-05-24 일: 최종 동결 점검  
  -> 검증: 최신 자동 검증, 빌드 로그, runtime smoke, Windows 포함 여부, 제출 폴더 상태를 한 번에 대조하고 미실행 항목은 미실행으로 명시한다.

## 검증 계획

- 자동 검증 PASS 기준: `orbitshare-validation.md`의 `Result: PASS`, Runtime Error/Exception `PASS (0)`, 24개 위성, 기본 목적지 New York, relay lock/delivery 100%, Reset 2회차, 1600x900 Canvas, IMGUI fallback disabled, camera cuts PASS.
- Linux 빌드 PASS 기준: 최신 빌드 로그의 `Build Finished, Result: Success.`와 `Builds/Linux/OrbitShare.x86_64`, `OrbitShare_Data`, `UnityPlayer.so` 존재.
- Windows 빌드 PASS 기준: 제출 필수로 확정된 경우 `Builds/Windows/OrbitShare.exe` 존재와 빌드 로그 Success.
- Runtime smoke PASS 기준: GUI/DISPLAY 환경 실행 로그에서 gameplay Error/Exception/Crash가 없는 것. `NullGfxDevice`/FMOD nosound 로그는 PASS 근거로 사용하지 않는다.
- Resource Hook PASS 기준: `SpaceSkybox.mat`, `SpaceStation.prefab` FOUND와 optional asset/SFX MISSING 상태가 실제 파일 목록, 리포트, 문서에서 충돌하지 않는 것.
- 제출 준비 PASS 기준: `Submission/OrbitShare` 생성 여부와 포함 파일이 확정된 제출 요구사항과 일치하는 것.

## 리스크/확인 필요

- `plans/weekly/2026-W21.md`와 `docs/` 경로는 확인되지 않았다. 이번 lane note는 daylog, daily plan, README/WORK_LOG, 기존 로그 기준이다.
- 최신 Unity 실행 로그들은 `IPC channel to LicensingClient doesn't exist; aborting`으로 중단된 기록이 있어, 이번 주 첫 병목은 Unity 라이선스/GUI 실행 환경이다.
- 기존 `orbitshare-validation.md`는 PASS지만 `Screen: 640x480`이고 최신 r1 추가 검증 항목이 없다.
- 기존 Linux 빌드 Success 근거는 있으나 최신 코드 기준 재빌드 성공 근거는 아직 필요하다.
- Windows 실행 파일이 제출 필수인지, Windows Build Support가 설치되어 있는지 확인이 필요하다.
- `Builds/`가 Git 제외 대상이면 최종 제출 패키지에 빌드 산출물을 어떻게 포함할지 별도 확인이 필요하다.
- Unity 자동 검증, GUI 리허설, Resource Hook 검증, frame capture, Linux/Windows 빌드는 project lock과 로그 충돌 위험 때문에 순차 실행해야 한다.
- `PrepareSubmissionFolder`는 PPT/PDF, MP4/MOV/GIF, Windows 빌드 포함 여부가 확정되기 전에는 실행하지 않는 편이 안전하다.

## 병렬 에이전트 활용 메모

- 이번 lane note 작성에는 하위/병렬 에이전트를 사용하지 않았다.
- 조사 범위가 Build/Validation 관련 문서, 로그, 에디터 자동화 코드로 충분히 좁아 단일 통합으로 처리했다.
- 실제 주간 실행에서도 Unity를 여는 작업은 병렬화하지 않는다. 필요하면 정적 로그/문서 조사만 별도 에이전트로 분리하고, Unity Runner는 한 명이 순차 실행한다.