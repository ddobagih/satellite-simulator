# Computer Graphics Assignment 1 - Build/Validation Automation lane note (2026-05-16)

## 최근 진행 근거
- `ProjectSettings/ProjectVersion.txt` (2026-05-10): Unity `2022.3.62f3` 기준 프로젝트.
- `Assets/Editor/BuildOrbitShare.cs` (2026-05-10): `OrbitShare > Build > Linux`, `OrbitShare > Build > Windows`, `Prepare Submission Folder` 메뉴 구현 확인.
- `ProjectSettings/EditorBuildSettings.asset` (2026-05-10): 빌드 씬은 `Assets/Scenes/MainScene.unity` 1개로 등록됨.
- `Assets/Editor/OrbitShareEditorAutomation.cs` (2026-05-14): `ValidateMissionFlow`가 Play Mode 진입, Fast Demo 실행, 검증 리포트 작성, 런타임 Error/Exception 카운트를 수행함.
- `orbitshare-validation.md` (2026-05-14): `Result: PASS`, Fast Demo 완료, 관측 100%, 전송 100%, Runtime Error/Exception `0`.
- `unity-build.log` (2026-05-14): `BuildOrbitShare.BuildLinux` 실행 및 `Build Finished, Result: Success.` 확인.
- `Builds/Linux/OrbitShare.x86_64` (현재 파일 존재): Linux 빌드 산출물 존재 확인. `Builds/Windows/OrbitShare.exe`는 없음.
- `daylog/2026-05-14.md`, `daylog/2026-05-15.md`: 자동 검증과 Linux 빌드는 기존 리포트/로그 기준 성공, Windows 빌드와 실제 GUI 리허설은 확인 필요로 기록됨.
- `plans/catchup/2026-05-15.md`: 다음 후보로 Windows 빌드 필요 여부 결정, 자동 검증 재실행, Linux 런타임 스모크, GUI Fast Demo 리허설을 제시함.
- `plans/daily/2026-05-16.md`: 현재 작업트리에서 발견되지 않음.

## 내일 목표 후보
- 자동 검증을 최신 작업트리 기준으로 재실행하고 `orbitshare-validation.md`를 새 기준으로 확정.
- Linux 빌드를 재생성하고 산출물/로그/런타임 스모크 결과를 제출 전 기준으로 갱신.
- Windows 빌드가 제출 필수인지 결정하고, 필요하면 `BuildOrbitShare.BuildWindows` 실행 및 산출물 확인.
- 빌드/검증 로그의 환경성 경고와 실제 런타임 오류를 분리해 최종 제출 문서에 넘길 근거 정리.
- 문서 최종화 뒤 `Prepare Submission Folder` 실행 여부를 결정하고 산출물 존재를 확인.

## 상세 체크리스트 초안
- [ ] 기준 파일 재확인 → 검증: `find plans -maxdepth 4 -type f -print`, `git status --short --branch`로 `plans/daily/2026-05-16.md` 존재 여부와 작업트리 상태 기록.
- [ ] Play Mode 자동 검증 재실행 → 검증: `OrbitShareEditorAutomation.ValidateMissionFlow` 실행 후 `orbitshare-validation.md`가 `Result: PASS`, Runtime Error/Exception `0`.
- [ ] 자동 검증 로그 검토 → 검증: `unity-validation.log`에서 실제 gameplay `Error/Exception`이 없는지 확인하고, license/update-check류 환경 경고는 별도 표시.
- [ ] Linux 빌드 재실행 → 검증: `BuildOrbitShare.BuildLinux` 실행 후 `unity-build.log`에 `Build Finished, Result: Success.`와 `Builds/Linux/OrbitShare.x86_64` 존재 확인.
- [ ] Linux 런타임 스모크 → 검증: `./run_orbitshare.sh` 또는 빌드 실행 로그에서 Error/Exception/Crash 미검출. GUI/DISPLAY 제약으로 못 돌리면 미실행 사유 기록.
- [ ] Windows 빌드 필요 여부 확정 → 검증: 필요 시 `BuildOrbitShare.BuildWindows` 실행 후 `Builds/Windows/OrbitShare.exe`와 `unity-build-windows.log` 확인, 불필요하면 제외 사유 기록.
- [ ] 제출 폴더 자동화 확인 → 검증: 문서 최종화 뒤 `OrbitShare > Prepare Submission Folder` 실행 여부 결정, 실행 시 `Submission/OrbitShare`와 체크리스트 파일 존재 확인.
- [ ] 결과 기록 → 검증: 실행한 명령, 변경 파일, PASS/FAIL, 남은 이슈를 `daylog/2026-05-16.md`에 기록.

## 리스크/확인 필요
- Windows 빌드 산출물과 로그가 아직 없으며, Linux 환경에서 Windows Build Support 모듈이 없으면 빌드가 실패할 수 있음.
- 현재 검증/빌드 성공 근거는 2026-05-14 로그 기준이고, 작업트리에 수정 파일이 남아 있어 최종 전 재검증이 필요함.
- 실제 Unity GUI 발표 화면 검증은 아직 직접 근거가 부족함. Build lane에서는 런타임 스모크까지만 확정하고, UI 리허설 결과는 UI/Demo lane과 맞춰야 함.
- `Submission/` 폴더는 현재 없음. 생성 시점은 README/WORK_LOG 등 제출 문서 정리 이후가 안전함.
- `unity-build.log`, `unity-validation.log`에는 Unity license/update-check 계열 경고와 `CrashService` 문자열이 있으나, 현재 검증 리포트 기준 런타임 Error/Exception은 0임.

## 병렬 에이전트 활용 메모
- 하위/병렬 에이전트는 사용하지 않음.
- 확인 범위가 빌드 스크립트, 자동 검증 스크립트, 기존 로그, 최근 daylog/plan 대조로 제한되어 단일 lane 조사로 충분했음.