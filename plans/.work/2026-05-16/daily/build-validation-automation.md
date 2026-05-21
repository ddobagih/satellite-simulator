# Computer Graphics Assignment 1 - Build/Validation Automation lane note (2026-05-17)

## 최근 진행 근거
- `README.md` 확인(2026-05-16 조사): `OrbitShare > Build > Linux/Windows`, `OrbitShareEditorAutomation.ValidateMissionFlow` 실행 방법이 문서화되어 있음.
- `Assets/Editor/BuildOrbitShare.cs` 확인(2026-05-16 조사): Linux/Windows 빌드 메뉴와 `Prepare Submission Folder` 메뉴가 구현되어 있음.
- `Assets/Editor/OrbitShareEditorAutomation.cs` 확인(2026-05-16 조사): 최신 자동 검증 코드에는 Reset 2회차 완료, 1600x900 Canvas 기준, IMGUI fallback 비활성, PresentationDirector camera cut 검사가 포함됨.
- `orbitshare-validation.md` 확인(기존 리포트): `Result: PASS`, relay lock/delivery 100%, Runtime Error/Exception `PASS (0)`이나, 최신 자동 검증 코드에 추가된 2회차/Canvas/camera cut 항목은 아직 리포트에 반영되지 않음.
- `daylog/2026-05-16.md`, `plans/.work/2026-05-16/execute-r1/build-validation-automation.md`, `plans/catchup/2026-05-16-audit-final.md` 확인: r1의 새 Unity 자동 검증과 Linux 빌드 재실행은 `IPC channel to LicensingClient doesn't exist; aborting`으로 실패.
- `unity-build.log` 확인: 기존 Linux 빌드는 `BuildOrbitShare.BuildLinux`, `Build Finished, Result: Success.` 기록이 있음.
- `unity-validation.log`, `unity-build-r1.log`, `unity-asset-hooks.log` 확인: 최신 재실행 시도는 모두 LicensingClient IPC 단계에서 중단됨.
- `Builds/Linux/OrbitShare.x86_64` 확인: Linux 실행 파일 존재. `Builds/Windows/OrbitShare.exe`는 없음.
- Unity 설치 상태 확인: `ProjectSettings/ProjectVersion.txt`는 Unity `2022.3.62f3`, PlaybackEngines에는 `LinuxStandaloneSupport`만 확인됨.
- `build-run-smoke.log` 확인: headless `NullGfxDevice` 실행에서 FMOD/shader unsupported `ERROR`가 있어 GUI smoke PASS로 보지 않음.
- `plans/daily/2026-05-17.md`는 현재 없음.
- repo 내부 별도 `AGENTS.md`는 확인되지 않아 사용자 제공 지침을 기준으로 적용함.

## 내일 목표 후보
- Unity LicensingClient IPC가 동작하는 환경에서 최신 자동 검증을 다시 실행해 `orbitshare-validation.md`를 최신 검증 항목 기준으로 갱신한다.
- 같은 환경에서 Linux 빌드를 재실행해 최신 코드 기준 `Build Finished, Result: Success.` 로그와 산출물을 확보한다.
- 실제 GUI/DISPLAY 환경에서 Linux 빌드 런타임 smoke를 수행하고, headless shader 오류와 구분되는 실행 안정성 근거를 남긴다.
- Windows 실행 파일 제출 필요 여부를 확정한다. 필요하면 Windows Build Support 설치 후 `BuildOrbitShare.BuildWindows`를 실행하고, 아니면 제외 사유를 문서화한다.
- 제출 형식 확정 후에만 `Prepare Submission Folder` 실행 여부를 결정한다.

## 상세 체크리스트 초안
- [ ] Unity 실행 환경 preflight 확인 → 검증: LicensingClient IPC 연결 가능 여부, `DISPLAY` 또는 `xvfb-run` 사용 가능 여부, Unity `2022.3.62f3` 경로를 기록한다.
- [ ] 최신 Play Mode 자동 검증 재실행 → 검증: `OrbitShareEditorAutomation.ValidateMissionFlow` 후 `orbitshare-validation.md`가 `Result: PASS`, Runtime Error/Exception `PASS (0)`이며 Reset 2회차/1600x900 Canvas/IMGUI fallback/PresentationDirector 항목을 포함하는지 확인한다.
- [ ] 검증 로그 분리 검토 → 검증: `unity-validation.log`에서 gameplay `Error/Exception`과 license/update-check 환경성 메시지를 구분해 기록한다.
- [ ] Linux 빌드 재실행 → 검증: `BuildOrbitShare.BuildLinux` 후 `unity-build.log` 또는 새 로그에 `Build Finished, Result: Success.`가 있고 `Builds/Linux/OrbitShare.x86_64`와 `OrbitShare_Data`가 갱신됐는지 확인한다.
- [ ] Linux GUI 런타임 smoke 실행 → 검증: `./run_orbitshare.sh` 또는 동일 조건 실행 후 `build-run-live.log`에서 Crash/C# Exception/gameplay Error가 없는지 확인한다.
- [ ] headless smoke 판정 정리 → 검증: `NullGfxDevice` shader/FM0D 오류는 GUI smoke PASS 근거로 쓰지 않고, 필요한 경우 환경 제약으로 분리 기록한다.
- [ ] Windows 빌드 필요 여부 확정 → 검증: 제출 기준상 필요하면 `WindowsStandaloneSupport` 설치 후 `BuildOrbitShare.BuildWindows`와 `Builds/Windows/OrbitShare.exe` 확인, 불필요하면 제외 사유를 README/WORK_LOG 쪽에 넘긴다.
- [ ] `Prepare Submission Folder` 실행 시점 결정 → 검증: README/WORK_LOG/제출 형식 확정 뒤 실행하며, 실행 시 `Submission/OrbitShare` 포함 파일을 확인한다.
- [ ] 결과 daylog 전달 → 검증: 실행한 명령, 성공/실패 로그, 미실행 사유, 남은 확인 필요 사항이 `daylog/2026-05-17.md`에 반영될 수 있게 요약한다.

## 리스크/확인 필요
- 최신 검증 코드는 보강됐지만, 현재 `orbitshare-validation.md`는 그 보강 항목을 포함하지 않아 최신 PASS 근거로는 부족하다.
- Unity LicensingClient IPC 문제를 해결하지 못하면 자동 검증, Asset hook 검증, Linux/Windows 빌드 갱신이 모두 막힌다.
- GUI/DISPLAY 환경 없이는 발표 안정성에 가까운 런타임 smoke를 PASS로 판정하기 어렵다.
- 현재 확인된 Unity PlaybackEngines에는 `LinuxStandaloneSupport`만 있어 Windows 빌드는 지원 모듈 설치가 필요할 가능성이 높다.
- Windows 실행 파일, MP4/MOV, PPT/PDF가 제출 필수인지 사용자 또는 과제 기준 확인이 필요하다.
- `Builds/`는 Git 제외 대상으로 보이므로 제출 패키징 시 산출물 포함 방식 확인이 필요하다.
- Unity 프로젝트를 여는 작업은 project lock과 로그 충돌 위험이 있어 자동 검증, frame capture, Linux/Windows build를 병렬 실행하면 안 된다.

## 병렬 에이전트 활용 메모
- 하위/병렬 에이전트는 사용하지 않았다.
- Build/Validation 범위의 증거가 빌드 스크립트, 자동화 스크립트, 최근 daylog/계획, 로그 파일 대조로 좁았고, Unity 실행 작업은 병렬화하면 lock/log 충돌 위험이 커서 단일 조사로 충분하다고 판단했다.