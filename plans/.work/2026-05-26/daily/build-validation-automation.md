# Computer Graphics Assignment 1 - Build/Validation Automation lane note (2026-05-27)

## 최근 진행 근거
- `product/vision.md`, `product/roadmap.md`, `product/backlog.md`, `product/done-criteria.md`, `product/decisions.md` 확인(2026-05-26): 제출 기준은 Unity 프로젝트 자체이며 Windows 빌드, MP4/PPT, `Submission/` 패키징은 dropped.
- `README.md`, `WORK_LOG.md` 확인(2026-05-26): 자동 검증은 `OrbitShareEditorAutomation.ValidateMissionFlow`, 빌드는 `BuildOrbitShare.BuildLinux/BuildWindows` 메뉴 기준.
- `plans/daily/2026-05-26.md`, `plans/weekly/2026-W22.md` 확인: 2026-05-27에는 validation 재생성, 실패/재시도/대기열 sanity, Resource Hook 또는 정적 inventory 갱신이 예정 후보.
- `daylog/2026-05-26.md`: route-clarity 작업 후 `orbitshare-validation.md` PASS, launcher menu capture PASS, Linux build success 로그를 판독. 단 오늘 새 Unity 실행이 아니라 기존 로그/파일 판독으로 분리되어 있음.
- `orbitshare-validation.md`(2026-05-26 00:57): `Result: PASS`, 7개 목적지, 24개 위성, Fast Demo 2회, Runtime Error/Exception `0`.
- `Assets/Editor/OrbitShareEditorAutomation.cs`, `MissionManager.cs`, `UIManager.cs` mtime이 `orbitshare-validation.md`보다 늦음(2026-05-26 10시대). 따라서 현재 validation report는 이후 코드 변경을 완전히 반영한 최신 PASS로 단정하면 안 됨.
- `orbitshare-menu-captures/report.md`(2026-05-26 00:56): 8개 하단 메뉴, 작성 5단계, 직접 설정 달력 캡처 non-empty 및 active-panel intersections PASS.
- `unity-build-route-clarity.log`(2026-05-26): `Build Finished, Result: Success.` 확인. 단 `Builds/Linux/OrbitShare.x86_64` mtime은 2026-05-10, 일부 Data 파일은 2026-05-25라 build manifest를 함께 남겨야 함.
- `build-run-live.log`(2026-05-25): GfxDevice 생성, 1280x720 resize 근거 있음. display smoke는 GUI E2E와 별도 증거.
- `asset-store-import-report.md`(2026-05-25): skybox/station/Earth texture/shader FOUND, SatelliteModel/PremiumEarth/SFX는 optional missing/fallback.
- `plans/features`, `plans/verify/ready`는 디렉터리는 있으나 파일 0개. `PM/`, `PM/status`, `PM/reports`, product audit, automation metrics 산출물은 없음. `plans/daily/2026-05-27.md`도 없음.

## 내일 목표 후보
- P0: 2026-05-26 10시대 코드 변경 이후 `ValidateMissionFlow`를 다시 실행해 `orbitshare-validation.md`를 최신화한다.
- P0: product 핵심 흐름을 지키는 automation feature slice로 `Automation Sanity v2`를 잡는다. queue/history/failure/retry/receipt/transmission log 중 안정적인 항목만 자동 검증에 추가하거나, 불안정하면 report-only matrix로 대체한다.
- P0: `AssetStorePackageImporter.ValidateResourceHooks`와 현재 `asset-store-import-report.md`의 범위 차이를 정리한다. 현재 validator를 그대로 실행하면 Earth shader/texture와 Click/Failure SFX 항목이 report에서 빠질 수 있다.
- P1: Linux build evidence를 로그 success와 `Builds/Linux` manifest로 함께 남긴다.
- P1: display smoke와 screenshot/menu capture는 순차 실행하고, GUI 수동 리허설이나 visual quality PASS로 과대해석하지 않는다.
- P2: `BuildWindows`, `PrepareSubmissionFolder`, MP4/PPT, `Submission/`, 외부 업로드, secret, 운영 DB, 새 Asset Store/SFX import는 요구 변경 전 실행하지 않는다.

## 상세 체크리스트 초안
- [ ] validation stale 여부 확인 → 검증: 자동화/mission/UI 코드 mtime과 `orbitshare-validation.md` mtime을 비교해 report 최신성 상태를 기록한다.
- [ ] Unity preflight 확인 → 검증: Unity `2022.3.62f3`, `Assets/Scenes/MainScene.unity` enabled, `Temp/UnityLockfile`, LicensingClient, `DISPLAY`/Xwayland 상태를 기록한다.
- [ ] `Automation Sanity v2` 범위 확정 → 검증: queue 생성, history/receipt non-empty, failure simulation, retry, transmission log 중 자동화할 항목과 보류 항목을 분리한다.
- [ ] `ValidateMissionFlow` 재실행 → 검증: `orbitshare-validation.md`에 `Result`, Runtime Error/Exception 수, Fast Demo 2회, reset replay, 7개 목적지 snapshot이 기록된다.
- [ ] 새 validation 항목 확인 → 검증: 현재 코드의 `Source Selection Snapshot`/non-blocking 항목이 report에 반영되는지 확인하고, 미반영이면 stale로 기록한다.
- [ ] Resource Hook validator 갱신 또는 보류 → 검증: 실행 시 Earth shader/texture와 Click/Failure SFX 항목이 report에서 사라지지 않거나, 정적 inventory로 대체한 사유를 남긴다.
- [ ] Linux build 재실행 여부 결정 → 검증: 실행 시 고유 로그명에 `Build Finished, Result: Success.`와 `Builds/Linux` 핵심 파일 mtime/size manifest를 함께 기록한다.
- [ ] display smoke 재확인 → 검증: `run_orbitshare.sh` 로그에서 GfxDevice 생성, 1280x720 resize, gameplay Error/Exception/Crash 부재를 확인한다.
- [ ] capture/menu capture 재실행 여부 결정 → 검증: PNG/report mtime, 해상도, non-empty 여부만 자동 근거로 기록하고 UI 품질은 Visual Review 필요로 분리한다.
- [ ] `PrepareSubmissionFolder` 충돌 처리 → 검증: 실행하지 않고, 필요 시 Unity project-only 기준으로 체크리스트 문구 수정 후보를 별도 기록한다.
- [ ] 정적 검증 실행 → 검증: 변경이 있으면 `git diff --check`, `bash -n run_orbitshare.sh` 통과 여부를 기록한다.
- [ ] evidence handoff 작성 → 검증: validation/build/display/capture/resource/dropped 항목을 PASS/FAIL/미실행/제외로 분리한다.

## 리스크/확인 필요
- 현재 `orbitshare-validation.md`는 PASS지만 이후 코드 변경보다 오래되어 최신 PASS로 단정하면 위험하다.
- `AssetStorePackageImporter.ValidateResourceHooks`는 현재 report보다 범위가 좁다. 실행 전 validator를 맞추거나 정적 inventory로 대체해야 한다.
- `Builds/Linux` 산출물 mtime이 파일별로 다르다. Unity incremental build 가능성이 있으므로 로그와 manifest를 같이 제시해야 한다.
- Licensing access token 경고는 여러 Unity 로그에 있으나 build/validation 실패와 분리해야 한다.
- Unity validation/build/capture/resource hook/display smoke는 project lock/log 충돌 때문에 병렬 실행 금지.
- 작업트리에 대량 modified/deleted/untracked가 섞여 있다. build lane은 되돌리지 않고 검증 대상과 제출 포함 여부만 분리한다.
- Windows 빌드, `Submission/`, MP4/PPT, 새 에셋/SFX/LFS, 실제 결제/DB/secret/외부 업로드는 B/C 작업이다. safe alternative는 local dry-run, 정적 truth table, report-only matrix다.

## 병렬 에이전트 활용 메모
- 별도 하위/병렬 에이전트는 사용하지 않았다.
- 이유: 이번 작업은 read-only lane note 작성이고, 실제 Unity 실행 계열은 병렬 금지 대상이다.
- 문서/코드/로그 조회는 병렬 shell 호출로 나누어 확인했고, 결론은 Build/Validation Automation 관점에서 단일 note로 통합했다.