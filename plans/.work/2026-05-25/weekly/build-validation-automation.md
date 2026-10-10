# Computer Graphics Assignment 1 - Build/Validation Automation weekly lane note (2026-W22)

## 이번 주 목표 후보

- P0: 최신 `ValidateMissionFlow`로 `orbitshare-validation.md`를 재생성한다. 현재 파일은 tracked 삭제 상태라 최신 PASS로 단정하지 않는다.
- P0: 자동 검증 feature slice를 확장한다. queue/history/failure/receipt/transmission log 중 안정적으로 확인 가능한 최소 항목만 `OrbitShareEditorAutomation`에 추가하거나, 불안정하면 report-only matrix로 대체한다.
- P0: Unity 실행 단일 runner 순서를 확정한다. validation, capture, Resource Hook, Linux build, display smoke는 병렬 실행하지 않는다.
- P1: Linux build 근거를 로그와 산출물 manifest로 함께 남긴다. 최근 build 로그는 Success지만 executable mtime과 일부 data mtime이 달라 manifest가 필요하다.
- P1: display smoke와 screenshot/capture를 GUI E2E와 분리해 기록한다. 2026-05-25 capture는 1600x900 PNG 생성 근거이며 수동 Editor 조작 완료 근거는 아니다.
- P1: Resource Hook 검증을 갱신한다. `SpaceSkybox.mat`, `SpaceStation.prefab`, Earth texture/shader는 FOUND, SFX/SatelliteModel/PremiumEarth는 optional missing/fallback으로 분리한다.
- P2: `BuildWindows`, `PrepareSubmissionFolder`, MP4/PPT/Submission/Git LFS/새 Asset Store import는 요구 변경 전 실행하지 않는다. `PrepareSubmissionFolder`의 체크리스트 문구는 최신 product decision과 충돌 가능성이 있어 실행 금지 또는 별도 소폭 수정 후보로 둔다.

## 날짜별/단계별 체크리스트

- [ ] 2026-05-25 월: 기준선 확정 -> 검증: `plans/weekly/2026-W22.md` 없음, `plans/features`, `plans/verify/ready`, `PM/status`, `PM/reports`, product audit, automation metrics 산출물 0개를 기록한다.
- [ ] 2026-05-25 월: Unity preflight 정리 -> 검증: Unity `2022.3.62f3`, enabled scene `Assets/Scenes/MainScene.unity`, `Temp/UnityLockfile`, LicensingClient, `DISPLAY`/Xwayland 상태를 확인한다.
- [ ] 2026-05-26 화: 최신 validation report 재생성 -> 검증: `OrbitShareEditorAutomation.ValidateMissionFlow` 실행 후 `orbitshare-validation.md` mtime, `Result`, Runtime Error/Exception 수, completed fast demo runs를 기록한다.
- [ ] 2026-05-26 화: automation sanity slice 결정 -> 검증: queue/history/failure/receipt/log 중 0.5일 내 안정적으로 검사 가능한 항목과 보류 항목이 분리된다.
- [ ] 2026-05-27 수: Linux build 갱신 또는 보류 사유 확정 -> 검증: 실행 시 `Build Finished, Result: Success.`와 `Builds/Linux` 핵심 파일 mtime/크기 manifest를 함께 남긴다.
- [ ] 2026-05-27 수: display smoke 재확인 -> 검증: `run_orbitshare.sh` 로그에서 GfxDevice 생성, 1280x720 resize, gameplay Error/Exception/Crash 부재를 확인한다.
- [ ] 2026-05-28 목: screenshot/capture 경로 확정 -> 검증: capture 성공 시 `orbitshare-playmode.png` mtime/해상도와 Visual Review 필요 상태를 기록하고, 실패 시 player/manual 판독으로 대체한다.
- [ ] 2026-05-28 목: Resource Hook 검증 -> 검증: `AssetStorePackageImporter.ValidateResourceHooks` 또는 정적 inventory로 FOUND/MISSING/fallback 상태가 문서와 일치한다.
- [ ] 2026-05-29 금: `PrepareSubmissionFolder` 충돌 처리 계획 -> 검증: 실행하지 않고, 필요 시 Unity project-only 기준으로 체크리스트를 바꾸는 작은 수정 후보를 남긴다.
- [ ] 2026-05-30 토: 변경 후 회귀 게이트 -> 검증: 이번 주 코드/에셋 변경이 있었다면 validation, Linux build/display smoke 중 필요한 항목만 순차 재실행한다.
- [ ] 2026-05-31 일: 제출 전 automation truth table 정리 -> 검증: validation/build/display/capture/resource/GUI E2E/dropped 항목을 PASS/FAIL/미실행/제외로 분리한다.

## 검증 계획

- Static: `git diff --check`, `bash -n run_orbitshare.sh`, `ProjectSettings/ProjectVersion.txt`, `ProjectSettings/EditorBuildSettings.asset`, scheduler/PM 산출물 파일 수 확인.
- Unity Batch: `OrbitShareEditorAutomation.ValidateMissionFlow`는 `-quit` 없이 실행해 report 저장을 확인한다.
- Build: `BuildOrbitShare.BuildLinux` 실행 시 고유 로그명을 쓰고 `Build Finished, Result: Success.`와 산출물 manifest를 함께 본다.
- Display Smoke: `./run_orbitshare.sh` 실행 로그에서 GfxDevice, 1280x720 resize, 런타임 예외 부재를 확인한다. headless 결과는 GUI smoke PASS로 쓰지 않는다.
- Capture: `OrbitShareEditorAutomation.CapturePlayScreenshot` 성공 시 PNG 해상도와 mtime만 자동 근거로 기록하고, UI/에셋 품질은 사람이 판독한 Visual Review로 분리한다.
- Resource Hook: `asset-store-import-report.md`를 갱신하거나 정적 inventory로 대체한다. optional missing은 실패가 아니라 fallback 상태로 기록한다.
- GUI E2E: build lane은 자동화 근거만 제공하고, 실제 Editor GUI 수동 조작 완료 여부는 UI/demo lane 결과와 연결한다.

## 리스크/확인 필요

- `orbitshare-validation.md`가 삭제 상태라 최신 자동 검증 PASS가 이번 주 첫 P0 blocker다.
- 2026-05-25 build/capture 로그는 성공 근거가 있으나 수동 GUI E2E 근거는 아니다.
- `Camera.Render()` capture는 과거 native crash 이력이 있다. 실패하면 앱 런타임 실패와 분리해 capture 경로 문제로 기록한다.
- `Builds/Linux/OrbitShare.x86_64` mtime은 오래됐고 일부 data 파일만 최신이다. Unity incremental build 가능성이 있으므로 로그와 manifest를 함께 제시한다.
- `BuildOrbitShare.PrepareSubmissionFolder`는 built executable/PPT/demo video를 요구해 현재 “Unity 프로젝트 자체 제출” 결정과 충돌한다.
- Windows build, MP4/PPT, `Submission/`, Git LFS, 새 Asset Store/SFX import는 현재 dropped다. safe alternative는 정적 truth table, dry-run 수준의 target/support 확인, report-only matrix다.
- Unity validation/build/capture/resource hook을 병렬 실행하면 project lock/log 충돌 위험이 크다.
- 작업트리에 대량 modified/deleted/untracked가 섞여 있다. build lane은 되돌리지 않고 검증 대상과 제출 포함 여부만 분리한다.

## 병렬 에이전트 활용 메모

- 별도 하위/병렬 에이전트는 사용하지 않았다.
- 이유: 이번 산출물은 read-only 문서/로그/코드 근거를 통합하는 lane note이고, 실제 Unity 실행 작업은 병렬 금지 대상이다.
- 문서, daylog, product, 자동화 코드, 로그 상태 확인은 병렬 shell 조회로 나누어 수행했고 최종 결론은 Build/Validation Automation 관점에서 단일 note로 통합했다.