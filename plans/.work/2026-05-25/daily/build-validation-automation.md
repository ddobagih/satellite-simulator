# Computer Graphics Assignment 1 - Build/Validation Automation lane note (2026-05-26)

## 최근 진행 근거
- `README.md`(2026-05-25 확인): 빌드 메뉴는 `OrbitShare > Build > Linux/Windows`, 자동 검증은 `OrbitShareEditorAutomation.ValidateMissionFlow`가 `orbitshare-validation.md`를 생성하는 흐름.
- `product/roadmap.md`, `product/done-criteria.md`, `product/decisions.md`(2026-05-18 기준, 2026-05-25 재확인): 제출 기준은 Unity 프로젝트 자체. Windows 빌드, MP4/PPT, `Submission/` 패키징은 dropped. Unity Batch, Display Smoke, Visual Review, GUI E2E, Build 근거는 분리해야 함.
- `daylog/2026-05-25.md`: 2026-05-25에 Linux build 로그 2개가 `Build Finished, Result: Success.`였고, `orbitshare-playmode.png`는 1600x900 PNG로 확인됨. 단 `orbitshare-validation.md`는 tracked 삭제 상태라 최신 `ValidateMissionFlow` PASS로 단정 금지.
- `daylog/2026-05-24.md`: 고해상도 Earth texture/shader 연결 뒤 Linux build success와 player display smoke 근거가 있음. 새 `ValidateMissionFlow`는 실행하지 않음.
- `Assets/Editor/OrbitShareEditorAutomation.cs`(2026-05-25 작업트리): 최근 변경은 완료 문구 영문/한글 모두 허용하는 수준. Fast Demo, reset replay, 24개 위성, 기본 runtime 상태 중심이며 queue/history/failure/receipt/transmission log까지 자동 검증한다고 보기는 어려움.
- `Assets/Editor/BuildOrbitShare.cs`(현재): `BuildLinux`, `BuildWindows`, `PrepareSubmissionFolder` 존재. `PrepareSubmissionFolder` 체크리스트는 built executable/PPT/demo video를 요구해 최신 product decision과 충돌 가능.
- `Builds/Linux` 현재 파일 상태: `OrbitShare.x86_64`, `UnityPlayer.so` mtime은 2026-05-10이고 일부 `OrbitShare_Data` 파일은 2026-05-25로 갱신됨. 빌드 로그 success와 산출물 manifest를 함께 봐야 함.
- `plans/features`, `plans/verify/ready`, `plans/verify/reports`(2026-05-25 확인): 파일 0개. `PM/`, `PM/status`, `PM/reports`, product audit, automation metrics 산출물은 없음.
- `plans/daily/2026-05-26.md`: 현재 없음.

## 내일 목표 후보
- P0: 최신 `ValidateMissionFlow` 재실행으로 `orbitshare-validation.md`를 복구/재생성한다. 목표는 최신 코드 기준 PASS/FAIL, Runtime Error/Exception 0, Fast Demo 2회, reset replay를 다시 확보하는 것.
- P0: product 목표를 앞으로 미는 automation feature slice를 잡는다. `OrbitShareEditorAutomation`에 queue/history/failure/receipt/transmission log 최소 sanity section을 추가하거나, 타이밍 의존이 크면 report-only 검증 matrix로 대체한다.
- P0: Unity 실행 단일 runner 순서를 확정한다. validation, Resource Hook, capture, Linux build, display smoke는 project lock/log 충돌 방지를 위해 순차 실행한다.
- P1: Linux build evidence manifest를 남긴다. `BuildOrbitShare.BuildLinux` 로그 success와 `Builds/Linux` 산출물 mtime/크기/핵심 파일 존재를 함께 기록한다.
- P1: 최신 visual evidence 경로를 정리한다. 2026-05-25 캡처는 존재하지만 GUI E2E 근거가 아니므로, 필요 시 capture 재시도 또는 player/manual 판독으로 분리 기록한다.
- P1: Resource hook 검증을 갱신하거나 정적 inventory로 대체한다. `SpaceSkybox.mat`, `SpaceStation.prefab`, Earth texture/shader는 FOUND, SFX/SatelliteModel/PremiumEarth는 optional missing/fallback으로 분리한다.
- P2: `BuildWindows`, `PrepareSubmissionFolder`, 외부 업로드, 새 Asset Store/SFX import, Git LFS는 요구 변경 전 실행하지 않는다.

## 상세 체크리스트 초안
- [ ] scheduler/PM 산출물 부재 재확인 → 검증: `plans/features`, `plans/verify/ready`, `plans/verify/reports`, `PM/status`, `PM/reports` 파일 수 또는 경로 부재를 기록한다.
- [ ] Unity preflight 확인 → 검증: Unity `2022.3.62f3`, enabled scene `Assets/Scenes/MainScene.unity`, `Temp/UnityLockfile`, LicensingClient, `DISPLAY`/Xwayland 상태를 로그에 남긴다.
- [ ] 최신 validation report 재생성 → 검증: `OrbitShareEditorAutomation.ValidateMissionFlow` 실행 후 `orbitshare-validation.md`가 생성되고 `Result`, Runtime Error/Exception 수, completed fast demo runs를 기록한다.
- [ ] automation sanity slice 범위 확정 → 검증: queue 생성, history/receipt non-empty, failure simulation, retry 가능성, transmission log non-empty 중 0.5일 내 안정적으로 검사할 항목만 고른다.
- [ ] automation sanity 구현 또는 safe alternative 적용 → 검증: 코드 변경이 작으면 `OrbitShareEditorAutomation.cs`에 section 추가, 불안정하면 수동 matrix와 미검증 사유를 남긴다.
- [ ] validation 실패 원인 분리 → 검증: gameplay regression, Licensing/update-check noise, Unity 환경 경고, capture native crash를 별도 원인으로 분류한다.
- [ ] Linux build 재실행 여부 결정 → 검증: 실행 시 `BuildOrbitShare.BuildLinux` 로그의 `Build Finished, Result: Success.`와 `Builds/Linux` manifest를 함께 기록한다.
- [ ] display smoke 재확인 → 검증: `run_orbitshare.sh` 로그에서 1280x720 resize, GfxDevice 생성, gameplay Error/Exception/Crash 부재를 확인한다.
- [ ] screenshot/capture 경로 결정 → 검증: capture 성공 시 파일 mtime/해상도와 “Visual Review 필요” 상태를 기록하고, 실패 시 player/manual 대체 근거를 기록한다.
- [ ] Resource Hook 검증/보류 판단 → 검증: 실행 시 `asset-store-import-report.md`를 갱신하고, 보류 시 `Assets/Resources/OrbitShare` 정적 inventory로 대체한다.
- [ ] `PrepareSubmissionFolder` 충돌 처리 계획화 → 검증: 실행하지 않고, 필요하면 문구를 Unity project-only 기준으로 바꾸는 별도 작은 작업 후보로 남긴다.
- [ ] dropped 항목 실행 방지 → 검증: Windows build, MP4/PPT, `Submission/`, 외부 업로드, secret, 운영 DB, 새 에셋 import를 실행하지 않았다고 기록한다.

## 리스크/확인 필요
- `orbitshare-validation.md`가 현재 tracked 삭제 상태라 product 문서의 “최신 PASS” 표현은 재생성 전까지 과거 근거로만 취급해야 한다.
- 자동 검증은 GUI E2E가 아니다. 실제 Editor GUI에서 단계형 발신, 대기열/기록, 실패 후 재시도, 패널 드래그는 별도 수동 리허설 필요.
- `Camera.Render()` capture는 과거 native crash 이력이 있으나 2026-05-25에는 capture success 로그가 있다. 실패 시 앱 실패로 단정하지 말고 capture 경로 문제로 분리한다.
- Linux build 로그는 최신 success가 있으나 executable mtime이 오래됐다. Unity incremental build 특성일 수 있으므로 manifest와 로그를 함께 제시한다.
- `PrepareSubmissionFolder`는 현재 제출 기준과 충돌한다. safe alternative는 실행 금지와 정적 제출 truth table 작성.
- Windows build/MP4/PPT/Submission/Git LFS/새 에셋/실제 결제/운영 DB/secret/외부 업로드는 B/C 작업이다. safe alternative는 local mock, fixture, dry-run, report-only matrix로 제한한다.
- 작업트리에 대량 modified/deleted/untracked가 섞여 있다. build lane은 되돌리지 않고 검증 대상/제출 포함 여부만 분리해야 한다.
- Unity 실행 작업은 병렬화하면 project lock/log 충돌 위험이 크다.

## 병렬 에이전트 활용 메모
- 하위/병렬 에이전트는 사용하지 않음.
- 이유: 이번 lane note는 read-only 문서/코드/로그 근거 통합 작업이고, 실제 Unity validation/build/capture 계열은 병렬 금지 대상이다.
- 파일 조회는 병렬 shell 호출로 나누어 처리했고, 최종 결론은 Build/Validation Automation 관점에서 단일 note로 통합했다.