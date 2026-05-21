# Computer Graphics Assignment 1 - Spec/Work Log/Submission Docs lane note (2026-05-19)

## 최근 진행 근거
- 사용자 제공 AGENTS 지침(확인일 2026-05-18 KST): 한국어 응답, 추정 금지, 실제 실행하지 않은 검증은 완료로 쓰지 않기, 프로젝트 상태 변경 시 daylog 기록 원칙 확인.
- 로컬 `AGENTS.md`(확인일 2026-05-18 KST): 프로젝트/상위 경로에서 별도 파일은 확인되지 않아 사용자 제공 지침을 기준으로 적용.
- `docs/`(확인일 2026-05-18 KST): 별도 docs 경로 없음.
- `plans/daily/2026-05-19.md`(확인일 2026-05-18 KST): 기존 파일 없음. merge 에이전트가 신규 계획으로 통합해야 함.
- `README.md`(확인일 2026-05-18 KST): 실행 방법, Demo Flow, 구현 기능, 자동 검증 명령, Asset Store fallback 설명이 있음.
- `WORK_LOG.md`(작성일 2026-05-15, 최종 점검 2026-05-16, 확인일 2026-05-18 KST): 기존 자동 검증 PASS, Linux 빌드 성공, 제출 상태 점검표가 있음. Windows 빌드, MP4/MOV, PPT/PDF, `Submission/`은 없음 또는 확인 필요로 기록됨.
- `daylog/2026-05-18.md`(확인일 2026-05-18 KST): 정적 점검과 문서/계획 통합은 수행됨. Unity 자동 검증, GUI 리허설, Linux/Windows 빌드 재실행은 수행하지 않았다고 명시됨.
- `plans/daily/2026-05-18.md`(확인일 2026-05-18 KST): Spec lane 목표가 제출 요구사항 확정표, README/WORK_LOG/Q&A 정합성, 성공 기준 매핑, Git/대용량 에셋 방침 정리로 정리되어 있음.
- `plans/.work/2026-05-18/catchup/spec-worklog-submission.md`(확인일 2026-05-18 KST): Spec lane 체크박스 대부분이 미완료로 남아 있고, 2026-05-18에는 제출 요구사항 확정과 문서 정합성 정리가 우선이라고 결론남.
- `orbitshare_shared_satellite_agent.md`(확인일 2026-05-18 KST): 상단 최신 섹션에 현재 릴레이 컨셉, 제출물 상태 요약, 발표 Q&A, 성공 기준이 있음. 하단 legacy 관측 플랫폼 원문은 현재 구현과 충돌 가능성이 있음.
- `orbitshare-validation.md`(확인일 2026-05-18 KST): `Result: PASS`, `Screen: 640x480`, 24개 위성, 기본 목적지 New York, Fast Demo 완료, relay lock/delivery 100%, Runtime Error/Exception `PASS (0)` 확인. Reset 2회차, 1600x900 Canvas, IMGUI fallback, PresentationDirector camera cut 항목은 없음.
- `unity-validation.log`, `unity-build-r1.log`, `unity-asset-hooks.log`(확인일 2026-05-18 KST): 모두 `IPC channel to LicensingClient doesn't exist; aborting`으로 최신 Unity 실행이 중단됨.
- `unity-build.log`(확인일 2026-05-18 KST): 기존 Linux 빌드 `Build Finished, Result: Success.` 확인.
- 산출물 검색(확인일 2026-05-18 KST): `Builds/Linux/OrbitShare.x86_64`, `orbitshare-playmode.png`, `orbitshare-demo.gif`, `orbitshare-contact-sheet.png`, `DemoFrames/` 존재. `Builds/Windows/OrbitShare.exe`, `.ppt/.pptx/.pdf`, `.mp4/.mov`, `Submission/`은 확인되지 않음.
- `ASSET_STORE_IMPORT.md`, `asset-store-import-report.md`(확인일 2026-05-18 KST): `SpaceSkybox.mat`, `SpaceStation.prefab`은 FOUND. `SatelliteModel.prefab`, `PremiumEarthMaterial.mat`, SFX 5종은 optional missing/fallback 상태.
- `git status --short --branch`(확인일 2026-05-18 KST): 코드/에셋 변경은 확인되지 않고, daylog/plan 문서 변경 및 미추적 파일이 남아 있음.

## 내일 목표 후보
- 1순위: 제출 요구사항 확정표를 최종화한다. Windows 빌드, PPT/PDF, MP4/MOV, GIF 대체 가능 여부, `Submission/OrbitShare` 필요 여부를 `필수/불필요/확인 필요`로 분리한다.
- 2순위: `WORK_LOG.md`, `README.md`, `orbitshare_shared_satellite_agent.md`를 최신 근거와 맞춘다. 기존 PASS와 최신 미실행/환경 실패 검증을 섞어 쓰지 않는다.
- 3순위: 성공 기준 매핑을 만든다. 각 성공 기준을 README, WORK_LOG, validation report, GUI 리허설 기록, 산출물 검색 결과 중 하나와 연결한다.
- 4순위: Build/UI/Visual lane의 2026-05-19 실행 결과가 생기면 문서에 반영한다. 실행하지 못한 검증은 미실행 또는 환경 제약으로 남긴다.
- 5순위: 발표 Q&A에서 현재 통신 릴레이 컨셉과 legacy 관측 플랫폼 설명을 명확히 분리한다.
- 6순위: Git/대용량 에셋/Asset Store 라이선스 제출 방침을 정리한다. private repo 유지, Git LFS 필요 여부, 빌드 산출물 제외 여부를 결정한다.
- 7순위: 실제 수행 결과 기준으로 `daylog/2026-05-19.md`를 작성한다.

## 상세 체크리스트 초안
- [ ] 제출 요구사항 확정표 작성 → 검증: Unity 프로젝트, Linux 빌드, Windows 빌드, README/WORK_LOG, PPT/PDF, MP4/MOV, GIF, PNG, `Submission/` 상태가 `존재/없음/필수 여부 확인 필요`로 분류됨.
- [ ] `WORK_LOG.md` 최종 점검 → 검증: 기존 `orbitshare-validation.md` PASS와 r1 이후 LicensingClient 실패가 구분되고, 실행하지 않은 GUI/Windows/MP4/PPT 검증이 완료로 쓰이지 않음.
- [ ] `README.md` 실행/시연 안내 점검 → 검증: `Queue`, `Relay`, `Quick Relay`, 자동 검증 명령, Asset Store fallback 설명이 현재 코드와 `WORK_LOG.md` 설명과 충돌하지 않음.
- [ ] `orbitshare_shared_satellite_agent.md` 발표 Q&A 정리 → 검증: 상단 최신 섹션에 현재 릴레이 컨셉, 구현 한계, 검증 근거, 제출 미확정 항목이 반영되고 legacy 원문과 발표 기준이 구분됨.
- [ ] 성공 기준 매핑 작성 → 검증: 각 성공 기준이 `README.md`, `WORK_LOG.md`, `orbitshare-validation.md`, 산출물 검색 결과, GUI 리허설 기록 중 하나와 연결됨.
- [ ] Build/Validation lane 결과 반영 → 검증: 새 Unity 자동 검증이나 빌드가 실행됐으면 로그 파일과 결과를 문서화하고, 실패했으면 LicensingClient/DISPLAY/Windows Build Support 등 원인을 명확히 기록함.
- [ ] UI/Demo lane 결과 반영 → 검증: GUI 리허설, Reset 후 재실행, Play 재진입, 발표 해상도 UI 확인 결과가 있으면 Q&A/성공 기준에 반영하고, 없으면 미실행으로 남김.
- [ ] Visual/Asset lane 결과 반영 → 검증: `SpaceSkybox.mat`, `SpaceStation.prefab` 포함 상태와 optional missing asset/SFX fallback 상태가 README/WORK_LOG/asset 문서에서 같은 의미로 설명됨.
- [ ] 제출 패키지 생성 여부 결정 → 검증: 제출 형식이 확정된 뒤에만 `Submission/OrbitShare` 생성 또는 보류 사유를 기록함.
- [ ] Git 제출 범위 정리 → 검증: `git status --short --branch`, 대용량 에셋 정책, 빌드 산출물 제외 여부, private repo 유지 방침이 daylog 또는 WORK_LOG에 기록됨.
- [ ] `daylog/2026-05-19.md` 작성 → 검증: 실제 변경 파일, 실제 실행한 검증, 실패한 검증, 미실행 검증, 남은 확인 필요가 기록됨.

## 리스크/확인 필요
- Windows 실행 파일이 제출 필수인지 불확실함. 현재 `Builds/Windows/OrbitShare.exe`는 확인되지 않음.
- `orbitshare-demo.gif`가 시연 영상으로 인정되는지, 별도 MP4/MOV가 필요한지 확인 필요.
- 발표 PPT/PDF가 필수인지, 외부에서 따로 준비된 파일이 있는지 확인 필요.
- `Submission/OrbitShare` 폴더는 아직 없으며, 제출 형식 확정 전 생성하면 포함 범위가 흔들릴 수 있음.
- 최신 자동 검증과 Linux 빌드는 LicensingClient IPC 문제로 재실행 실패한 상태이므로, r1 추가 검증 항목까지 최신 PASS라고 단정하면 안 됨.
- GUI 발표 리허설, Reset 후 재실행, Play 재진입, 발표 해상도 UI 검증은 현재 직접 PASS 근거가 부족함.
- 기존 `orbitshare-validation.md`는 640x480 기준이며, 1600x900 Canvas/IMGUI fallback/PresentationDirector camera cut/Reset 2회차 검증은 리포트에 아직 반영되지 않음.
- `SatelliteModel.prefab`, `PremiumEarthMaterial.mat`, SFX 5종이 optional fallback으로 충분한지, 평가상 추가 에셋이 필요한지 확인 필요.
- 50MB 초과 에셋과 Asset Store 리소스가 있어 private repo 유지, Git LFS 도입, 제출 패키지 제외 범위를 확인해야 함.
- `orbitshare_shared_satellite_agent.md` 아래쪽 legacy 관측 플랫폼 설명이 발표자가 그대로 읽으면 현재 릴레이 컨셉과 충돌할 수 있음.
- 현재 작업트리에 daylog/plan 관련 수정 및 미추적 파일이 있어 최종 제출 전 포함/제외 범위 정리가 필요함.

## 병렬 에이전트 활용 메모
- 하위/병렬 에이전트는 사용하지 않음.
- 이유: 이번 spec lane note는 기존 문서, daylog, 계획, 산출물 상태를 통합 판단하는 작업이며, 서로 다른 파일을 동시에 수정할 책임 범위가 없음.
- 통합 결론: 2026-05-19 spec lane은 새 기능 추가보다 제출 요구사항 확정, 문서 간 정합성, 기존 PASS와 실패/미실행 검증의 분리 기록, 발표 Q&A 최신화, 제출 패키지 범위 결정에 집중해야 함.