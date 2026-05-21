# Computer Graphics Assignment 1 - Spec/Work Log/Submission Docs lane note (2026-05-20)

## 최근 진행 근거
- 사용자 제공 AGENTS 지침(확인일 2026-05-19 KST): 한국어 응답, 추정 금지, 실제 실행하지 않은 검증은 완료로 쓰지 않기, 프로젝트 상태 변경 시 daylog 기록 원칙 확인.
- 로컬 `AGENTS.md`(확인일 2026-05-19 KST): 프로젝트/상위 경로에서 별도 파일은 확인되지 않아 사용자 제공 지침을 기준으로 적용.
- `plans/daily/2026-05-20.md`(확인일 2026-05-19 KST): 기존 파일 없음. merge 에이전트가 신규 계획으로 통합해야 함.
- `README.md`(확인일 2026-05-19 KST): 최신 데모 흐름은 서울 고정 발신자, 목적지/발신 시각/메시지 용량 선택, 24개 후보 위성 필터, `빠른 발신`/`위성 확정`/`메시지 발신` 흐름으로 정리됨.
- `WORK_LOG.md`(최종 점검 2026-05-18, 확인일 2026-05-19 KST): 구현 요약과 기존 제출 상태 표가 있으나 Windows/PPT/MP4/Submission을 “확인 필요”로 둔 표현이 남아 있어, 이후 사용자 결정과 맞춰 정리 필요.
- `daylog/2026-05-18.md`(확인일 2026-05-19 KST): 최신 자동 검증 PASS, 1600x900 캡처 판독, 단문 발신 컨셉 전환, 한글 UI/24개 후보 위성/지구본 보완, product 문서 생성, 사용자 결정 반영이 기록됨.
- `daylog/2026-05-19.md`(확인일 2026-05-19 KST): 최신 `orbitshare-validation.md` PASS, 1600x900 Canvas/IMGUI fallback/Reset 2회차/PresentationDirector 검증 PASS, 수동 GUI E2E는 아직 미완료로 기록됨.
- `orbitshare-validation.md`(확인일 2026-05-19 KST): `Result: PASS`, 24개 위성, New York 기본 목적지, Fast demo 완료, relay lock/delivery 100%, Reset 2회차, Runtime Error/Exception `PASS (0)` 확인.
- `product/decisions.md`(확인일 2026-05-19 KST): 제출 필수 산출물은 Unity 프로젝트 자체로 결정됨. Windows 빌드, MP4/MOV, PPT/PDF, `Submission/` 패키징, optional asset/SFX/LFS 작업은 현재 범위에서 제외됨.
- `product/done-criteria.md`(확인일 2026-05-19 KST): 자동 검증, display smoke, Visual Review, GUI E2E, Build, Submission의 검증 등급을 분리함. 수동 `위성 확정 → 메시지 발신` GUI E2E는 `todo`.
- `product/backlog.md`, `product/roadmap.md`(확인일 2026-05-19 KST): 남은 핵심 후보는 실제 Unity Editor Play Mode 수동 리허설, asset/fallback 화면 QA, 제출 직전 정적 인벤토리 확인.
- `orbitshare_shared_satellite_agent.md`(확인일 2026-05-19 KST): 상단 최신 발표 기준과 Q&A가 있으나 제출물 상태 요약에는 Windows/PPT/MP4 확인 필요 표현이 남아 있어 product 결정과 정합성 점검 필요.
- `ASSET_STORE_IMPORT.md`, `asset-store-import-report.md`(확인일 2026-05-19 KST): `SpaceSkybox.mat`, `SpaceStation.prefab`은 FOUND. 위성 모델, Premium Earth material, SFX 5종은 optional missing/fallback.
- `git status --short --branch`(확인일 2026-05-19 KST): 코드/문서/이미지 변경과 미추적 daylog/plan/product 문서가 남아 있어 제출 전 포함/제외 범위 정리 필요.

## 내일 목표 후보
- 1순위: 제출 기준 문서 정합성 정리. `WORK_LOG.md`, `orbitshare_shared_satellite_agent.md`, product 문서의 Windows/PPT/MP4/Submission 표현을 “Unity 프로젝트 자체 제출, 별도 산출물 제외” 결정과 맞춘다.
- 2순위: 수동 GUI E2E 결과를 문서에 반영할 준비를 한다. 실제 리허설이 수행되면 성공 기준/WORK_LOG/daylog에 반영하고, 수행되지 않으면 미실행으로 남긴다.
- 3순위: 성공 기준 매핑을 최신화한다. 자동 검증 PASS, Visual Review 완료, display smoke PASS, 수동 GUI E2E todo, Linux 최신 재빌드 deferred를 분리한다.
- 4순위: 제출 직전 정적 인벤토리 표를 만든다. Unity 프로젝트 필수 파일, README/WORK_LOG/product/daylog, validation report, screenshot, optional asset/fallback 상태를 구분한다.
- 5순위: 발표 Q&A를 상단 최신 단문 발신 컨셉 기준으로 다듬고, 하단 legacy 관측 플랫폼 원문은 발표 기준이 아님을 유지한다.
- 6순위: Git 제출 범위와 미추적 문서 포함 여부를 정리한다. 특히 `product/`, `daylog/2026-05-17~19.md`, `plans/` 산출물을 제출/커밋 범위에 넣을지 결정한다.
- 7순위: 실제 수행 결과 기준으로 `daylog/2026-05-20.md`에 변경 파일, 실행한 검증, 미실행 검증, 남은 확인 필요를 기록한다.

## 상세 체크리스트 초안
- [ ] 제출 기준 표현 통일 → 검증: `WORK_LOG.md`, `orbitshare_shared_satellite_agent.md`, `product/decisions.md`, `product/done-criteria.md`에서 Windows/PPT/MP4/`Submission/`이 완료가 아니라 현재 제외/dropped로 일관되게 설명됨.
- [ ] 최신 검증 근거 표 정리 → 검증: `orbitshare-validation.md` PASS 항목과 수동 GUI E2E todo 항목이 한 표에서 분리됨.
- [ ] README 실행/시연 안내 점검 → 검증: `빠른 발신`, `위성 확정`, `메시지 발신`, 자동 검증 명령, Asset Store fallback 설명이 현재 구현/문서와 충돌하지 않음.
- [ ] 발표 Q&A 정리 → 검증: 최신 Q&A가 서울 고정 발신자, 3개 목적지, 24개 후보 위성, 1회성 단문 발신, 시뮬레이션 한계를 기준으로 설명됨.
- [ ] 성공 기준 매핑 작성 → 검증: 각 기준이 `orbitshare-validation.md`, `orbitshare-playmode.png`, `daylog/2026-05-18.md`, 수동 GUI 리허설 기록 또는 “미실행” 상태 중 하나와 연결됨.
- [ ] 제출 직전 정적 인벤토리 작성 → 검증: `Assets/`, `Packages/`, `ProjectSettings/`, `Assets/Scenes/MainScene.unity`, `README.md`, `WORK_LOG.md`, product 문서, daylog, validation/screenshot 상태가 확인됨.
- [ ] Build/UI/Visual lane 결과 반영 → 검증: 2026-05-20에 수동 GUI 리허설이나 asset 화면 QA가 수행되면 근거 파일과 결과를 반영하고, 없으면 완료로 쓰지 않음.
- [ ] Git/대용량/라이선스 상태 정리 → 검증: private repo 유지, Git LFS dropped, optional asset/SFX dropped, 미추적 파일 포함 여부가 daylog 또는 작업 로그에 기록됨.
- [ ] `daylog/2026-05-20.md` 작성 → 검증: 실제 변경 파일, 실제 실행한 검증, 실패/미실행 검증, 남은 확인 필요가 기록됨.

## 리스크/확인 필요
- 수동 Unity Editor GUI E2E는 아직 완료 근거가 부족함. 자동 검증 PASS와 실제 사람이 누르는 `빠른 발신`/`위성 확정 → 메시지 발신` 리허설은 구분해야 함.
- `WORK_LOG.md`와 `orbitshare_shared_satellite_agent.md` 일부 표현은 product 결정 전의 Windows/PPT/MP4/Submission 확인 필요 상태를 포함하므로 최신 제출 기준과 충돌할 수 있음.
- `plans/weekly/2026-W21.md`는 최신 product 결정 전 기준도 섞여 있어, 내일 daily 계획에는 product/decisions 기준을 우선 반영해야 함.
- Linux 빌드는 기존 산출물이 있으나 최신 코드 기준 재빌드는 deferred 상태다. Unity 프로젝트 자체 제출 결정 때문에 필수로 되살리지 않는 것이 현재 방침.
- `SpaceSkybox.mat`, `SpaceStation.prefab` 외 optional asset/SFX는 missing이지만 fallback 유지 결정이 있으므로 실패로 쓰면 안 됨.
- 현재 작업트리에 코드 변경과 미추적 daylog/plan/product 문서가 함께 남아 있어 제출/커밋 범위를 정리해야 함.
- legacy 관측/예약 플랫폼 설명을 발표자가 그대로 사용하면 최신 단문 발신 컨셉과 충돌할 수 있음.

## 병렬 에이전트 활용 메모
- 하위/병렬 에이전트는 사용하지 않음.
- 이유: 이번 spec lane note는 기존 README, WORK_LOG, product 문서, daylog, validation report를 통합 판단하는 범위라 별도 파일 책임을 나눠 수정할 작업이 없었음.
- 파일 읽기와 검색은 병렬 shell 호출로 처리했고, 통합 결론은 단일 lane note로 정리함.