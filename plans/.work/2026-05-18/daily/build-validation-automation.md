# Computer Graphics Assignment 1 - Build/Validation Automation lane note (2026-05-19)

## 최근 진행 근거
- 2026-05-18 조사 기준 프로젝트 내 별도 `AGENTS.md`, `docs/`, `plans/daily/2026-05-19.md`는 확인되지 않았다. 사용자 제공 AGENTS 지침과 기존 `plans/daily/2026-05-18.md`를 기준으로 삼는다.
- `README.md`(현재): Unity `2022.3.62f3` batchmode 검증 명령, `OrbitShare > Build > Linux/Windows`, 자동 검증 산출물 `orbitshare-validation.md`를 안내한다.
- `daylog/2026-05-18.md`: 최신 Unity 자동 검증, GUI 리허설, Linux/Windows 재빌드는 아직 수행되지 않았고 `LicensingClient` IPC 문제로 Unity 재실행 계열 작업이 막혀 있다고 기록되어 있다.
- `orbitshare-validation.md`(기존 리포트): `Result: PASS`, 24개 릴레이 위성, 기본 목적지 New York, Fast demo 완료, relay lock/delivery 100%, Runtime Error/Exception `PASS (0)`를 기록한다. 단 `Screen: 640x480`이며 최신 r1 추가 항목은 없다.
- `Assets/Editor/OrbitShareEditorAutomation.cs`: 최신 검증 코드는 Runtime Canvas 1600x900, IMGUI fallback disabled, Reset replay 2회차, PresentationDirector camera cuts 항목을 리포트에 쓰도록 구현되어 있다.
- `unity-validation.log`, `unity-build-r1.log`, `unity-asset-hooks.log`: 모두 `IPC channel to LicensingClient doesn't exist; aborting`으로 중단된 기록이 있다.
- `unity-build.log`: 기존 Linux 빌드는 `BuildOrbitShare.BuildLinux`, `Build Finished, Result: Success.` 기록이 있다.
- `Builds/Linux/OrbitShare.x86_64`, `Builds/Linux/OrbitShare_Data`, `Builds/Linux/UnityPlayer.so`는 존재한다. `Builds/Windows/OrbitShare.exe`는 확인되지 않았다.
- `ProjectSettings/ProjectVersion.txt`: Unity `2022.3.62f3 (96770f904ca7)`.
- `ProjectSettings/EditorBuildSettings.asset`: enabled build scene은 `Assets/Scenes/MainScene.unity` 1개다.
- `Assets/Editor/BuildOrbitShare.cs`: `BuildLinux`, `BuildWindows`, `PrepareSubmissionFolder` 메뉴가 구현되어 있고 출력 경로는 각각 `Builds/Linux/OrbitShare.x86_64`, `Builds/Windows/OrbitShare.exe`다.
- `build-run-smoke.log`: `NullGfxDevice`, FMOD nosound 초기화 메시지가 있어 GUI runtime smoke PASS 근거로 보지 않는다.
- `run_orbitshare.sh`: Linux 빌드를 `1280x720`, `DISPLAY=${DISPLAY:-:0}`, `build-run-live.log`로 실행하도록 되어 있다.
- `open_unity_editor.sh`: Unity Editor GUI 실행 시 `.unity-libs`, `DISPLAY=${DISPLAY:-:0}`, `unity-editor-live.log`를 사용한다.
- `asset-store-import-report.md`: `SpaceSkybox.mat`, `SpaceStation.prefab`은 FOUND이고 나머지 runtime asset/SFX는 optional MISSING이다. Resource Hook 최신 재검증도 LicensingClient 문제로 막힌 상태다.
- `WORK_LOG.md`(최종 점검 2026-05-16): 자동 검증 PASS, Linux 빌드 Success, Windows 빌드/MP4/PPT/Submission 미확인을 구분해 기록한다.
- `git status --short --branch` 기준 현재 코드/에셋 tracked 변경은 없고 daylog/plans 문서 변경과 미추적 파일만 확인된다.

## 내일 목표 후보
- Unity LicensingClient IPC와 GUI/DISPLAY 실행 환경을 먼저 복구한다.
- 최신 `OrbitShareEditorAutomation.ValidateMissionFlow`를 실행해 r1 추가 검증 항목까지 포함된 `orbitshare-validation.md` PASS 근거를 확보한다.
- Unity 검증 로그에서 gameplay/runtime 예외와 LicensingClient/update-check/token류 환경 메시지를 분리 판정한다.
- `BuildOrbitShare.BuildLinux`를 최신 상태로 재실행하고 Linux 빌드 Success 로그와 산출물을 갱신한다.
- Linux GUI runtime smoke를 수행해 headless `NullGfxDevice` 결과와 별도인 실행 안정성 근거를 남긴다.
- Windows 빌드 제출 필요 여부를 확정하고, 필수이면 Windows Build Support 확인 후 `BuildOrbitShare.BuildWindows`를 실행한다.
- Resource Hook 검증을 Unity 환경 복구 후 재실행해 `asset-store-import-report.md`와 실제 리소스 상태를 다시 맞춘다.
- `Prepare Submission Folder`는 제출 형식과 포함 범위가 확정된 뒤에만 실행한다.

## 상세 체크리스트 초안
- [ ] Unity preflight 확인 → 검증: Unity `2022.3.62f3` 경로, LicensingClient IPC 연결, GUI `DISPLAY` 또는 대체 실행 환경, 프로젝트 lock 여부를 기록한다.
- [ ] 최신 자동 검증 실행 → 검증: `OrbitShareEditorAutomation.ValidateMissionFlow` 후 `orbitshare-validation.md`에 `Result: PASS`, Runtime Error/Exception `0`, Reset replay 2회차, Runtime Canvas 1600x900, IMGUI fallback disabled, PresentationDirector camera cuts 항목이 포함되는지 확인한다.
- [ ] 검증 실패 원인 분류 → 검증: `unity-validation.log`에서 mission/runtime 예외와 LicensingClient/update-check/token류 환경 메시지를 분리해 기록한다.
- [ ] Linux 빌드 재실행 → 검증: `BuildOrbitShare.BuildLinux` 실행 후 최신 로그에 `Build Finished, Result: Success.`가 있고 `Builds/Linux/OrbitShare.x86_64`, `OrbitShare_Data`, `UnityPlayer.so`가 존재하는지 확인한다.
- [ ] Linux GUI runtime smoke 수행 → 검증: `run_orbitshare.sh` 또는 동일 조건 실행 후 `build-run-live.log`에서 gameplay Error/Exception/Crash가 없는지 확인한다.
- [ ] Windows 빌드 필요 여부 확정 → 검증: 제출 기준상 필수이면 Windows Build Support 확인 후 `BuildOrbitShare.BuildWindows` 실행 및 `Builds/Windows/OrbitShare.exe` 존재를 확인하고, 불필요하면 제외 사유를 문서 lane에 전달한다.
- [ ] Resource Hook 검증 재실행 → 검증: `OrbitShare > Asset Store > Validate Resource Hooks` 또는 batch 메서드 실행 후 `asset-store-import-report.md`가 `SpaceSkybox.mat`, `SpaceStation.prefab` FOUND 및 optional MISSING 상태와 일치하는지 확인한다.
- [ ] 스크린샷/프레임 자동화 갱신 필요 여부 결정 → 검증: 최신 GUI/검증 결과가 기존 `orbitshare-playmode.png`, `DemoFrames/`, `orbitshare-demo.gif`와 다르면 `CapturePlayScreenshot` 또는 `CaptureDemoFrames` 재실행 대상으로 분류한다.
- [ ] 제출 폴더 생성 시점 결정 → 검증: README/WORK_LOG/발표 자료/영상 형식 확정 후에만 `PrepareSubmissionFolder`를 실행하고 `Submission/OrbitShare` 포함 파일을 확인한다.
- [ ] 결과를 문서 lane에 전달 → 검증: 실행 명령, 로그 파일, 성공/실패, 미실행 사유, 남은 리스크를 daylog와 제출 문서에 반영할 수 있게 요약한다.

## 리스크/확인 필요
- Unity LicensingClient IPC가 계속 실패하면 최신 자동 검증, Resource Hook 검증, Linux/Windows 재빌드가 모두 막힌다.
- GUI/DISPLAY 환경이 없으면 Linux runtime smoke와 발표 안정성 검증을 PASS로 판정하기 어렵다.
- 기존 `orbitshare-validation.md`는 PASS지만 최신 검증 코드의 Reset/1600x900/IMGUI/camera cut 항목을 포함하지 않아 최신 완료 근거로는 부족하다.
- 기존 Linux 빌드 성공 근거는 있으나 최신 코드 기준 재빌드 성공 로그는 아직 없다.
- Windows 실행 파일이 제출 필수인지, Windows Build Support가 설치되어 있는지 확인이 필요하다.
- `Prepare Submission Folder`는 PPT/PDF, MP4/MOV/GIF, Windows 빌드 포함 여부가 확정되기 전 실행하면 포함 범위가 흔들릴 수 있다.
- 자동 검증, GUI 리허설, Resource Hook 검증, frame capture, Linux/Windows 빌드는 Unity project lock과 로그 충돌 위험이 있어 순차 실행해야 한다.
- `Builds/`는 Git 제외 대상으로 보이므로 제출 패키징에서 산출물을 어떻게 포함할지 확인이 필요하다.

## 병렬 에이전트 활용 메모
- 하위/병렬 에이전트는 사용하지 않았다.
- 이번 lane note 범위는 Build/Validation 관련 문서, 로그, 에디터 자동화 코드 대조로 충분히 좁고, Unity 실행 계열 작업은 병렬화하면 project lock/log 충돌 위험이 커서 단일 조사로 정리했다.
- 파일 읽기와 로그 확인은 병렬 shell 호출로 처리했고, 결론은 이 lane note에 통합했다.