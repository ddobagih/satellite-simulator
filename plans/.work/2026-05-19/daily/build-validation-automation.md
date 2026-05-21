# Computer Graphics Assignment 1 - Build/Validation Automation lane note (2026-05-20)

## 최근 진행 근거
- 2026-05-19 조사 기준: 프로젝트 내 별도 `AGENTS.md`, `docs/`, `plans/daily/2026-05-20.md`, `plans/.work/2026-05-19/daily/build-validation-automation.md`는 확인되지 않았다. 사용자 제공 AGENTS 지침과 기존 문서를 기준으로 삼는다.
- `README.md`(현재): Unity `2022.3.62f3`, `OrbitShare > Build > Linux/Windows`, `OrbitShareEditorAutomation.ValidateMissionFlow` batch 실행 방법을 안내한다.
- `ProjectSettings/ProjectVersion.txt`: Unity `2022.3.62f3 (96770f904ca7)`.
- `ProjectSettings/EditorBuildSettings.asset`: enabled build scene은 `Assets/Scenes/MainScene.unity` 1개다.
- `Assets/Editor/BuildOrbitShare.cs`: `BuildLinux`, `BuildWindows`, `PrepareSubmissionFolder`가 구현되어 있고 출력 경로는 `Builds/Linux/OrbitShare.x86_64`, `Builds/Windows/OrbitShare.exe`다.
- `Assets/Editor/OrbitShareEditorAutomation.cs`: 자동 검증이 fast demo 2회, Runtime Canvas 1600x900, IMGUI fallback 비활성, PresentationDirector camera cuts, Runtime Error/Exception 로그 수를 검사한다.
- `orbitshare-validation.md`(최신 확인): `Result: PASS`, Runtime Canvas 1600x900 PASS, Reset replay 2회 PASS, camera cuts PASS, Runtime Error/Exception `PASS (0)`. 단 batch `Screen` 값은 `640x480`이다.
- `unity-validation-product.log`, `unity-validation-concept.log`(2026-05-18): `Successfully connected to LicensingClient`, `OrbitShare validation report saved` 확인. 과거 `LicensingClient` IPC blocker는 최신 자동 검증 기준 해소된 상태다.
- `unity-validation.log`, `unity-build-r1.log`, `unity-asset-hooks.log`(2026-05-16): `IPC channel to LicensingClient doesn't exist; aborting` 실패 기록이 남아 있다. 최신 성공 로그와 구분해야 한다.
- `unity-build.log`(2026-05-15): `BuildOrbitShare.BuildLinux`, `Build Finished, Result: Success.` 확인. 다만 Linux 산출물 timestamp가 2026-05-10~2026-05-15로 섞여 있어 최신 코드 재빌드 근거로 보기는 어렵다.
- `Builds/Linux/OrbitShare.x86_64`, `OrbitShare_Data`, `UnityPlayer.so`는 존재한다. `Builds/Windows/OrbitShare.exe`는 확인되지 않았다.
- `run_orbitshare.sh`, `build-run-live.log`(2026-05-18): rootless Xwayland `0x0` 환경을 감지해 임시 rootful Xwayland로 실행하는 fallback이 있고, `1280x720` display에서 `GfxDevice` 생성까지 확인됐다. 이는 display smoke 근거이며 수동 GUI E2E 완료 근거는 아니다.
- `product/decisions.md`(2026-05-18): 제출은 Unity 프로젝트 자체로 결정됐고 Windows 빌드, MP4/MOV, PPT/PDF, `Submission/` 패키징은 현재 범위에서 제외됐다.
- `product/done-criteria.md`(현재): 기존 Linux build success를 최신 코드 재빌드 success로 쓰지 말고, display smoke를 수동 GUI E2E로 과대해석하지 말라고 명시한다.
- `daylog/2026-05-19.md`: 최신 자동 검증 PASS, 1600x900 캡처 존재, display smoke 존재, 실제 마우스 조작 기반 GUI E2E 미완료, 최신 Linux 재빌드 미수행, Windows 빌드 없음으로 정리되어 있다.

## 내일 목표 후보
- 최신 Linux 재빌드 필요 여부를 먼저 확정한다. Unity 프로젝트 제출만 유지한다면 필수는 아니지만, 빌드 lane 관점에서는 기존 산출물과 최신 코드 사이의 간극을 명확히 문서화한다.
- `BuildOrbitShare.BuildLinux`를 재실행할 수 있으면 최신 build log와 산출물을 갱신한다.
- 재빌드를 하지 않는다면 “Unity 프로젝트 제출 기준이라 deferred”로 문서 lane에 전달하고, 기존 `unity-build.log`는 과거 성공 근거로만 표시한다.
- `run_orbitshare.sh` display smoke를 한 번 더 짧게 확인하거나, 기존 `build-run-live.log`를 최종 제출 전 근거로 채택할지 결정한다.
- 실제 Editor GUI 수동 리허설은 UI/demo lane 주도 항목이지만, build lane은 실행 환경, 로그 파일, project lock 충돌 방지 순서를 지원한다.
- Resource Hook 검증은 Asset lane 필요 시에만 Unity 실행 순서에 포함한다. 현재 `asset-store-import-report.md` 정적 상태는 FOUND/MISSING이 문서와 일치한다.
- Windows 빌드, `PrepareSubmissionFolder`는 제출 기준 변경이 있을 때만 재활성화한다.

## 상세 체크리스트 초안
- [ ] Unity 실행 preflight 확인 → 검증: Unity `2022.3.62f3` 경로, LicensingClient 연결 가능 여부, project lock, `DISPLAY`/Xwayland 상태를 로그로 확인한다.
- [ ] 최신 자동 검증 근거 재확인 → 검증: `orbitshare-validation.md`에 `Result: PASS`, Runtime Error/Exception `0`, Reset replay 2회, Runtime Canvas 1600x900, IMGUI fallback disabled, camera cuts PASS가 유지되는지 확인한다.
- [ ] Linux 최신 재빌드 실행 또는 보류 결정 → 검증: 실행 시 `BuildOrbitShare.BuildLinux` 로그에 `Build Finished, Result: Success.`와 최신 timestamp 산출물을 확인하고, 보류 시 Unity 프로젝트 제출 기준에 따른 deferred 사유를 문서 lane에 전달한다.
- [ ] Linux player display smoke 확인 → 검증: `run_orbitshare.sh` 실행 로그에서 `1280x720` display, `GfxDevice` 생성, gameplay `Error`/`Exception`/`Crash` 부재를 확인한다.
- [ ] 실패 로그 분리 판정 → 검증: `LicensingClient`, update-check, access token, REST port류 환경 메시지와 실제 runtime 예외를 구분해 기록한다.
- [ ] Resource Hook 검증 필요 여부 확인 → 검증: Visual lane에서 요청할 경우 Unity 실행 작업과 순차로 수행하고 `asset-store-import-report.md`와 실제 `Assets/Resources/OrbitShare` 상태를 대조한다.
- [ ] Windows 빌드 제외 상태 유지 확인 → 검증: 제출 기준이 바뀌지 않았으면 `BuildWindows`를 실행하지 않고 `product/decisions.md`의 dropped 결정을 유지한다.
- [ ] `PrepareSubmissionFolder` 제외 상태 유지 확인 → 검증: 별도 제출 폴더가 다시 필요해진 경우에만 실행하고, 아니면 dropped 사유를 유지한다.
- [ ] 결과 전달 → 검증: 실행한 명령, 로그 파일, 성공/실패, 미실행 사유를 daylog/WORK_LOG 갱신 담당에게 짧게 전달한다.

## 리스크/확인 필요
- 기존 Linux 빌드 성공은 최신 코드 재빌드 성공이 아니다.
- 최신 자동 검증은 PASS지만 batch `Screen` 값은 `640x480`이므로 발표 해상도 GUI 확인과 구분해야 한다.
- display smoke는 player 시작 근거일 뿐, 실제 버튼 조작 기반 GUI E2E 완료 근거가 아니다.
- Unity 실행 작업을 병렬로 돌리면 project lock과 로그 충돌 위험이 있다.
- 제출 기준이 Unity 프로젝트 자체로 유지되는 한 Windows 빌드와 `Submission/` 패키징은 되살리지 않는 것이 맞다.
- 과제 제출 기준이 갑자기 실행 파일을 요구하면 Linux 최신 재빌드와 Windows Build Support 여부를 다시 확인해야 한다.
- `unity-asset-hooks.log`는 과거 LicensingClient 실패 로그이므로 Resource Hook 최신 성공 근거로 쓰면 안 된다.

## 병렬 에이전트 활용 메모
- 하위/병렬 에이전트는 사용하지 않았다.
- 이번 범위는 build/validation 문서, 로그, 에디터 스크립트 대조로 충분했고, Unity 실행 계열 작업은 project lock과 로그 충돌 위험 때문에 병렬화 대상이 아니다.
- 파일 읽기는 병렬 shell 호출로 처리했고, 결론은 단일 lane note로 통합했다.