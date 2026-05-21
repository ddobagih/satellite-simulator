# Computer Graphics Assignment 1 - Build/Validation Automation lane note (2026-05-18)

## 최근 진행 근거
- 2026-05-17 기준 별도 `AGENTS.md` 파일과 `docs/` 경로는 확인되지 않았고, 사용자 제공 AGENTS 지침을 기준으로 적용해야 한다.
- `plans/daily/`에는 `2026-05-15.md`, `2026-05-16.md`, `2026-05-17.md`만 있어 2026-05-18 기존 계획표는 아직 없다.
- `README.md`(현재 문서): Unity `2022.3.62f3` 경로 기반 `OrbitShareEditorAutomation.ValidateMissionFlow` 자동 검증 명령과 `OrbitShare > Build > Linux/Windows` 메뉴를 안내한다.
- `WORK_LOG.md`(최종 점검 2026-05-16): 자동 검증 기존 결과 PASS, Linux 빌드 Success, Windows 빌드 미확인, Play Mode 스크린샷 존재, MP4/MOV/PPT/PDF/Submission 미확인을 기록한다.
- `daylog/2026-05-16.md`: r1 보강으로 `OrbitShareEditorAutomation.ValidateMissionFlow`에 Reset 2회차, 1600x900 Canvas, IMGUI fallback 비활성, PresentationDirector camera cut 검사가 추가됐지만, 이후 Unity 재실행은 LicensingClient IPC 문제로 실패했다고 기록한다.
- `daylog/2026-05-17.md`: 기존 `orbitshare-validation.md`는 PASS이나 `Screen: 640x480` 형식이고 r1 추가 검증 항목이 리포트에 아직 반영되지 않았다고 정리한다.
- `plans/daily/2026-05-17.md`: Build/Validation lane의 핵심 목표는 Unity LicensingClient/GUI 환경 확보, 최신 자동 검증 재실행, Linux 재빌드, Windows 빌드 필요 여부 확정, Linux GUI smoke 분리 판정으로 정리되어 있다.
- `orbitshare-validation.md`(기존 리포트): `Result: PASS`, 24개 위성, Fast demo mission complete, relay lock 100%, delivery 100%, Runtime Error/Exception `PASS (0)`를 기록하지만, Reset 2회차/1600x900/PresentationDirector 검사는 포함하지 않는다.
- `Assets/Editor/OrbitShareEditorAutomation.cs`: 최신 코드에는 `ValidateMissionFlow`, `CapturePlayScreenshot`, `CaptureDemoFrames`가 있고, 검증 리포트에 Runtime Canvas 1600x900, IMGUI fallback disabled, Reset replay, camera cuts 항목을 쓰도록 되어 있다.
- `Assets/Editor/BuildOrbitShare.cs`: `BuildLinux`, `BuildWindows`, `PrepareSubmissionFolder` 메뉴가 있으며 Linux 출력은 `Builds/Linux/OrbitShare.x86_64`, Windows 출력은 `Builds/Windows/OrbitShare.exe`다.
- `ProjectSettings/ProjectVersion.txt`: Unity 버전은 `2022.3.62f3 (96770f904ca7)`이다.
- `ProjectSettings/EditorBuildSettings.asset`: 빌드 씬은 `Assets/Scenes/MainScene.unity` 1개가 enabled 상태다.
- `unity-build.log`: `BuildOrbitShare.BuildLinux`, `Build Finished, Result: Success.` 기록이 있다.
- `Builds/Linux/`: `OrbitShare.x86_64`, `OrbitShare_Data/*`, `UnityPlayer.so`가 존재한다.
- `unity-validation.log`, `unity-build-r1.log`, `unity-asset-hooks.log`: 모두 `IPC channel to LicensingClient doesn't exist; aborting`으로 최신 Unity 실행이 중단된 근거가 있다.
- `build-run-smoke.log`: `NullGfxDevice`, FMOD output device 초기화 오류가 있어 GUI runtime smoke PASS 근거로 쓰면 안 된다.
- `run_orbitshare.sh`: Linux 빌드를 `1280x720`, `DISPLAY=${DISPLAY:-:0}`, `build-run-live.log`로 실행하도록 되어 있다.
- `open_unity_editor.sh`: Unity Editor GUI 실행용 스크립트이며 `LD_LIBRARY_PATH=.unity-libs`, `DISPLAY=${DISPLAY:-:0}`, `unity-editor-live.log`를 사용한다.
- `Tools/import_asset_store_packages.sh`, `Assets/Editor/AssetStorePackageImporter.cs`, `asset-store-import-report.md`: Resource hook 검증 경로는 있으나 최신 `Validate Resource Hooks` 재실행도 LicensingClient 문제로 막힌 상태다.
- `git status --short --branch` 기준 현재 tracked 코드 변경은 없고 `daylog/2026-05-16.md` 수정, `daylog/2026-05-17.md`, `plans/.work/2026-05-16/daily/`, `plans/daily/2026-05-17.md` 미추적 상태가 남아 있다.

## 내일 목표 후보
- Unity LicensingClient IPC와 GUI/DISPLAY 실행 환경을 먼저 확보한다.
- 최신 `OrbitShareEditorAutomation.ValidateMissionFlow`를 재실행해 r1 추가 항목이 포함된 `orbitshare-validation.md`를 PASS로 갱신한다.
- 자동 검증 로그에서 실제 gameplay Error/Exception과 Unity license/update-check류 환경 메시지를 분리해 판정한다.
- `BuildOrbitShare.BuildLinux`를 최신 코드 기준으로 재실행하고 Linux 산출물과 로그 Success를 갱신한다.
- `run_orbitshare.sh` 또는 직접 실행으로 Linux GUI runtime smoke를 수행하고, headless/NullGfxDevice 결과와 구분한다.
- Windows 빌드가 제출 필수인지 확인하고, 필요하면 Windows Build Support 여부 확인 후 `BuildOrbitShare.BuildWindows`를 실행한다.
- Resource hook 검증은 Unity 환경이 복구된 뒤 `Validate Resource Hooks`로 재실행해 `asset-store-import-report.md`와 실제 파일 상태를 다시 맞춘다.
- `Prepare Submission Folder`는 제출 형식과 포함 범위가 확정된 뒤에만 실행한다.

## 상세 체크리스트 초안
- [ ] Unity preflight 확인 → 검증: Unity `2022.3.62f3` 경로, LicensingClient IPC 연결, GUI `DISPLAY` 또는 대체 실행 환경, 프로젝트 lock 여부를 기록한다.
- [ ] 최신 자동 검증 실행 → 검증: `OrbitShareEditorAutomation.ValidateMissionFlow` 실행 후 `orbitshare-validation.md`에 `Result: PASS`, Runtime Error/Exception `0`, Reset 2회차, 1600x900 Canvas, IMGUI fallback disabled, PresentationDirector camera cuts 항목이 포함되는지 확인한다.
- [ ] 검증 실패 시 원인 분류 → 검증: `unity-validation.log`에서 mission/runtime 예외인지, LicensingClient/update-check/token류 환경 문제인지 분리해 기록한다.
- [ ] Linux 빌드 재실행 → 검증: `BuildOrbitShare.BuildLinux` 실행 후 `unity-build.log` 또는 별도 최신 로그에 `Build Finished, Result: Success.`가 있고 `Builds/Linux/OrbitShare.x86_64`, `OrbitShare_Data`, `UnityPlayer.so`가 존재하는지 확인한다.
- [ ] Linux GUI runtime smoke 수행 → 검증: `run_orbitshare.sh` 실행 로그에서 gameplay Error/Exception/Crash가 없는지 확인하고, `NullGfxDevice`/FMOD nosound 로그는 GUI PASS 근거로 사용하지 않는다.
- [ ] Windows 빌드 필요 여부 확정 → 검증: 제출 기준상 필수이면 `BuildOrbitShare.BuildWindows` 실행 후 `Builds/Windows/OrbitShare.exe` 존재를 확인하고, 필수가 아니면 제외 사유를 WORK_LOG/daylog용으로 남긴다.
- [ ] Resource Hook 검증 재실행 → 검증: `OrbitShare > Asset Store > Validate Resource Hooks` 또는 배치 메서드 실행 후 `asset-store-import-report.md`가 `SpaceSkybox.mat`, `SpaceStation.prefab` FOUND 및 optional missing 상태와 일치하는지 확인한다.
- [ ] 스크린샷/프레임 자동화 필요 여부 결정 → 검증: 최신 GUI/검증 결과가 기존 `orbitshare-playmode.png`, `DemoFrames/`, `orbitshare-demo.gif`와 맞지 않으면 `CapturePlayScreenshot` 또는 `CaptureDemoFrames` 재실행 계획을 세운다.
- [ ] 제출 폴더 생성 여부 결정 → 검증: README/WORK_LOG/발표 자료/영상 형식 확정 후에만 `PrepareSubmissionFolder`를 실행하고 `Submission/OrbitShare` 포함 파일을 확인한다.
- [ ] 빌드/검증 결과를 문서 lane에 전달 → 검증: 실행 명령, 로그 파일, 성공/실패, 미실행 사유, 남은 리스크가 daylog와 최종 제출 문서에 반영될 수 있게 요약한다.

## 리스크/확인 필요
- Unity LicensingClient IPC가 계속 실패하면 최신 자동 검증, Resource Hook 검증, Linux/Windows 재빌드를 완료할 수 없다.
- GUI/DISPLAY 환경이 없으면 Linux runtime smoke와 발표 안정성 확인을 PASS로 판정할 수 없다.
- 기존 `orbitshare-validation.md`는 PASS지만 최신 검증 코드의 추가 항목을 반영하지 않으므로 2026-05-18 완료 근거로 그대로 쓰기에는 부족하다.
- 기존 Linux 빌드는 성공 근거가 있으나 최신 코드 기준 재빌드 성공 로그는 아직 없다.
- Windows 실행 파일이 제출 필수인지 미확정이다.
- Windows Build Support 설치 여부도 아직 확인되지 않았다.
- `Prepare Submission Folder`는 PPT/PDF, MP4/MOV/GIF, Windows 빌드 필요 여부가 확정되기 전 실행하면 포함 범위가 흔들릴 수 있다.
- `run_orbitshare.sh`는 `DISPLAY=:0` 기본값을 쓰므로 실제 GUI 세션 없는 환경에서는 실패하거나 headless성 로그가 나올 수 있다.
- 자동 검증/GUI/빌드/Resource Hook 검증은 같은 Unity 프로젝트 lock과 로그를 공유하므로 병렬 실행하지 않는 것이 안전하다.

## 병렬 에이전트 활용 메모
- 하위/병렬 에이전트는 사용하지 않았다.
- 이번 lane note 범위는 Build/Validation 관련 문서, 로그, 에디터 자동화 코드 확인으로 충분히 좁고, 같은 Unity 프로젝트를 여는 작업은 병렬화하면 lock/log 충돌 위험이 있어 단일 흐름으로 정리했다.
- 파일 읽기와 로그 확인은 병렬 shell 호출로 처리했고, 결론은 이 lane note에 단일 통합했다.