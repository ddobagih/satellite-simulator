# Computer Graphics Assignment 1 - Build/Validation Automation catch-up lane note (2026-05-18)

## 확인한 근거

- `plans/daily/2026-05-17.md`: Build/Validation Automation 항목 8개 확인.
- `daylog/2026-05-17.md`: 최신 Unity 실행은 LicensingClient IPC 문제로 실패, GUI/Windows/최신 빌드 검증 미수행으로 기록.
- `daylog/2026-05-18.md`: 파일 없음. 새벽 작업 완료 근거는 확인되지 않음.
- `plans/daily/2026-05-18.md`, `plans/.work/2026-05-17/daily/build-validation-automation.md`: 같은 미완료 항목이 2026-05-18 계획으로 이월됨.
- `orbitshare-validation.md`: 기존 `Result: PASS`, `Screen: 640x480`; Reset 2회차, Canvas 1600x900, IMGUI fallback, camera cuts 항목 없음.
- `Assets/Editor/OrbitShareEditorAutomation.cs`: 최신 검증 코드에는 Reset replay, 1600x900 Canvas, IMGUI fallback disabled, PresentationDirector camera cuts 검사가 포함됨.
- `Assets/Editor/BuildOrbitShare.cs`: `BuildLinux`, `BuildWindows`, `PrepareSubmissionFolder` 구현 확인.
- `unity-validation.log`, `unity-build-r1.log`, `unity-asset-hooks.log`: 모두 `IPC channel to LicensingClient doesn't exist; aborting`.
- `unity-build.log`: 기존 `BuildOrbitShare.BuildLinux`, `Build Finished, Result: Success.` 확인.
- `Builds/Linux/OrbitShare.x86_64`, `OrbitShare_Data`, `UnityPlayer.so` 존재. `Builds/Windows/OrbitShare.exe` 없음.
- `ProjectSettings/ProjectVersion.txt`: Unity `2022.3.62f3`.
- `ProjectSettings/EditorBuildSettings.asset`: enabled scene은 `Assets/Scenes/MainScene.unity` 1개.
- `build-run-smoke.log`: `NullGfxDevice`, FMOD/shader 오류가 있어 GUI runtime smoke PASS 근거로 부적합.

## 완료로 판단한 항목

- Build/Validation 실행 항목 중 완전히 완료로 볼 수 있는 것은 “결과 daylog 전달” 정도다. `daylog/2026-05-17.md`에 기존 PASS, LicensingClient 실패, GUI/Windows 미확인, 남은 리스크가 기록되어 있다.
- `BuildOrbitShare` 메뉴와 editor automation 메서드 구현 존재는 확인됐다.
- 기존 기준의 자동 검증 PASS와 기존 Linux 빌드 성공 근거는 확인됐다. 단, 둘 다 최신 r1 검증 항목 기준 완료 근거는 아니다.
- Windows 빌드 산출물이 없다는 점은 확인됐다.

## 미완료 작업 후보

- [ ] Unity preflight 완료 → 이유/근거: Unity 버전과 Linux support는 확인됐지만 LicensingClient IPC, GUI `DISPLAY`/xvfb, project lock 상태가 성공 기준으로 확인되지 않음.
- [ ] 최신 Play Mode 자동 검증 실행 → 이유/근거: 기존 리포트는 640x480 구버전 항목이고, 최신 실행은 LicensingClient IPC 실패.
- [ ] 최신 검증 로그 판정 분리 → 이유/근거: 기존 실패 원인은 분리됐지만, 최신 자동 검증 성공/실패 로그 자체가 아직 없음.
- [ ] Linux 빌드 재실행 → 이유/근거: 기존 `unity-build.log` 성공은 있으나 r1 재실행은 `unity-build-r1.log`에서 LicensingClient 실패.
- [ ] Linux GUI runtime smoke → 이유/근거: headless `NullGfxDevice` smoke만 있고 GUI/DISPLAY 기반 PASS 근거가 없음.
- [ ] Windows 빌드 또는 제외 사유 확정 → 이유/근거: `Builds/Windows/OrbitShare.exe`가 없고 제출 필수 여부도 미확정.
- [ ] `Prepare Submission Folder` 실행 여부 확정 → 이유/근거: 제출 형식, Windows 필요 여부, 발표/영상 산출물 범위가 확정되지 않음.
- [ ] 스크린샷/프레임 자동화 갱신 필요 여부 결정 → 이유/근거: 최신 GUI 검증이 없어 기존 `orbitshare-playmode.png`/GIF가 최신 상태인지 판단 불가.

## 오늘 catch-up 후보 스케줄

- [ ] Unity 실행 환경 preflight → 검증: Unity `2022.3.62f3`, LicensingClient IPC 연결, GUI `DISPLAY` 또는 xvfb 가능 여부, project lock 상태 기록.
- [ ] `OrbitShareEditorAutomation.ValidateMissionFlow` 실행 → 검증: `orbitshare-validation.md`에 `Result: PASS`, Runtime Error/Exception 0, Reset replay 2회차, Canvas 1600x900, IMGUI fallback disabled, camera cuts 항목 포함.
- [ ] 검증 로그 분류 → 검증: gameplay/runtime 예외와 LicensingClient/update-check/token류 환경 메시지를 분리 기록.
- [ ] `BuildOrbitShare.BuildLinux` 재실행 → 검증: 최신 로그에 `Build Finished, Result: Success.`, `Builds/Linux/OrbitShare.x86_64`, `OrbitShare_Data`, `UnityPlayer.so` 확인.
- [ ] Linux GUI runtime smoke 실행 → 검증: GUI/DISPLAY 환경 로그에서 Crash/C# Exception/gameplay Error 없음 확인.
- [ ] Windows 빌드 필요 여부 확정 → 검증: 필수면 Windows Build Support 확인 후 `BuildWindows` 실행 및 `OrbitShare.exe` 확인, 불필요하면 제외 사유 문서화.
- [ ] 제출 폴더 실행 시점 결정 → 검증: 제출 산출물 범위 확정 뒤에만 `PrepareSubmissionFolder` 실행 여부 기록.
- [ ] 결과를 daylog/문서 lane에 전달 → 검증: 실행 명령, 로그명, 성공/실패, 미실행 사유, 남은 확인 필요 요약.

## 확인 필요

- `daylog/2026-05-18.md`가 없어 2026-05-18 새벽 작업 완료 근거는 확인되지 않는다.
- Windows 실행 파일이 과제 제출 필수인지 확인 필요.
- 실제 GUI 세션 또는 xvfb 사용 가능 여부 확인 필요.
- 기존 Linux 빌드 산출물은 일부 파일 timestamp가 섞여 있어 최신 clean rebuild가 필요하다.
- `Submission/OrbitShare`는 아직 확인되지 않는다.
- Unity 자동 검증, GUI 리허설, 빌드, Resource Hook 검증은 같은 프로젝트 lock을 공유하므로 순차 실행이 맞다.

## 병렬 에이전트 활용 메모

- 하위/병렬 에이전트는 사용하지 않았다.
- 범위가 문서/로그/스크립트 대조로 충분히 좁았고, Unity 실행 계열 작업은 병렬화하면 lock/log 충돌 위험이 있다.
- 파일 수정 금지 요청이므로 catch-up 파일과 daylog는 작성하지 않았다.