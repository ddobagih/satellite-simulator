# Computer Graphics Assignment 1 - Spec/Work Log/Submission Docs lane note (2026-05-22)

## 최근 진행 근거
- `product/vision.md`, `product/roadmap.md`, `product/backlog.md`, `product/done-criteria.md`, `product/decisions.md`(문서 기준 2026-05-18): 현재 제품은 “서울 고정 발신자 + 3개 목적지 + 발신 시각/메시지 용량 + 24개 후보 위성 + 1회성 단문 릴레이”이며, 제출은 Unity 프로젝트 자체로 결정됨.
- `README.md`(확인일 2026-05-21): 최신 Demo Flow, `빠른 발신`, `위성 확정 → 메시지 발신`, 자동 검증 명령, Asset Store fallback 설명이 정리되어 있음.
- `WORK_LOG.md`(최종 점검 2026-05-18): 구현 요약은 최신 컨셉을 반영하지만, 2026-05-16 제출 상태 표에는 Windows/PPT/MP4/Submission “확인 필요” 표현이 남아 최신 product 결정과 정합성 정리 필요.
- `orbitshare_shared_satellite_agent.md`(확인일 2026-05-21): 상단 최신 발표 기준/Q&A는 존재하지만 제출물 상태 요약에는 Windows/PPT/MP4 확인 필요 표현이 남아 있고, 하단 legacy 관측/예약 플랫폼 원문과 발표 기준 분리가 계속 필요.
- `orbitshare-validation.md`(확인일 2026-05-21): `Result: PASS`, 24개 위성, 기본 목적지 New York, Fast demo 완료, relay lock/delivery 100%, Reset replay 2회, Runtime Error/Exception 0 확인. 단 `Screen: 640x480`이므로 발표 해상도 GUI E2E 근거는 아님.
- `orbitshare-playmode.png`(daylog 기준 2026-05-20 20:57 KST): 1600x900 Play Mode 캡처 판독 근거. 버튼 조작 완료나 오디오 청취 완료 근거는 아님.
- `daylog/2026-05-20.md`(2026-05-20): UI/UX 보완, 최신 validation PASS, 최신 캡처 판독, 수동 GUI E2E 미완료, Linux 최신 재빌드 미실행, 제출/커밋 범위 정리 필요 기록.
- `daylog/2026-05-21.md`(2026-05-21): 신규 코드 변경 없이 정적 조사 중심. scheduler/PM 산출물 부재, Unity GUI E2E/배치 검증/캡처 재생성/재빌드 미실행이 명시됨.
- `plans/daily/2026-05-21.md`(작성일 2026-05-20): 신규 feature slice 후보는 선택 위성 발신 가능/불가능 사유 표시. spec lane은 성공 기준 매트릭스, WORK_LOG/Q&A 정합성, 정적 제출 인벤토리, daylog 작성 담당.
- `plans/features`, `plans/verify/ready`, `plans/verify/reports`(확인일 2026-05-21): 디렉터리는 있으나 파일 없음. `PM/`, `docs/`, 별도 로컬 `AGENTS.md`, 기존 `plans/daily/2026-05-22.md`도 확인되지 않음.
- `ASSET_STORE_IMPORT.md`, `asset-store-import-report.md`(확인일 2026-05-21): `SpaceSkybox.mat`, `SpaceStation.prefab`은 FOUND, 위성 모델/Premium Earth/SFX는 optional MISSING이며 fallback 정상 기준.
- `git status --short --branch`(확인일 2026-05-21): 코드/문서 tracked 변경과 미추적 `daylog/`, `plans/`, `product/` 문서가 함께 남아 제출/커밋 포함 범위 분류 필요.

## 내일 목표 후보
- 1순위: product 목표를 앞으로 미는 문서 feature slice로 “제출/발표 성공 기준 매트릭스”를 만든다. 자동 검증, visual review, GUI E2E, build, dropped 산출물을 한 표에서 분리한다.
- 2순위: `WORK_LOG.md`와 `orbitshare_shared_satellite_agent.md`의 Windows/PPT/MP4/Submission 표현을 Unity 프로젝트 제출 결정과 맞춘다.
- 3순위: 발표 Q&A를 최신 단문 발신 컨셉 기준으로 압축한다. legacy 관측/예약 플랫폼 원문은 보존하되 발표 기준이 아님을 명확히 한다.
- 4순위: README/WORK_LOG/product 용어를 대조해 `Fast Demo`, `빠른 발신`, `위성 확정 → 메시지 발신`의 의미를 일관되게 정리한다.
- 5순위: 제출 직전 정적 인벤토리를 정리한다. Unity 필수 폴더, README/WORK_LOG/product/daylog, validation, screenshot, optional fallback, 제외 산출물을 구분한다.
- 6순위: 다른 lane에서 GUI E2E, Resource Hook, visual/audio 확인 결과가 나오면 문서와 daylog에 반영한다. 결과가 없으면 미실행으로 남긴다.
- 7순위: `git status` 기준 tracked 변경과 미추적 문서의 제출/커밋 후보를 분류한다.

## 상세 체크리스트 초안
- [ ] 제출/발표 성공 기준 매트릭스 작성 → 검증: 자동 validation PASS, 1600x900 캡처, display smoke, 수동 GUI E2E done/todo, Linux 최신 재빌드 deferred, Windows/PPT/MP4/Submission dropped가 한 표에서 분리됨.
- [ ] `WORK_LOG.md` 제출 상태 표현 정리 → 검증: Windows 빌드, MP4/MOV, PPT/PDF, `Submission/`이 필수 확인 필요가 아니라 현재 product 결정상 제외 또는 요구 변경 시 재검토로 설명됨.
- [ ] `orbitshare_shared_satellite_agent.md` Q&A 정리 → 검증: 서울 고정 발신자, 3개 목적지, 24개 후보 위성, 1회성 단문 발신, 시뮬레이션 한계가 발표 기준으로 정리됨.
- [ ] README/WORK_LOG/product 용어 대조 → 검증: `Fast Demo`는 자동 검증 흐름, `빠른 발신`은 UI 버튼, `위성 확정 → 메시지 발신`은 수동 흐름으로 구분됨.
- [ ] 수동 GUI E2E 결과 반영 슬롯 준비 → 검증: `빠른 발신`, 수동 발신, Reset/재진입 결과가 제공되면 근거 파일과 함께 반영하고, 제공되지 않으면 완료로 쓰지 않음.
- [ ] Asset/fallback 제출 설명 통일 → 검증: `SpaceSkybox.mat`, `SpaceStation.prefab`은 포함 에셋, 나머지 optional missing은 fallback 정상 상태로 설명됨.
- [ ] 정적 제출 인벤토리 작성 → 검증: `Assets/`, `Packages/`, `ProjectSettings/`, `Assets/Scenes/MainScene.unity`, README/WORK_LOG/product/daylog/validation/screenshot 상태가 확인됨.
- [ ] Git/대용량/라이선스 상태 정리 → 검증: private repo 유지, Asset Store 리소스, 50MB 초과 리소스, Git LFS dropped, 미추적 문서 포함 여부가 기록됨.
- [ ] `daylog/2026-05-22.md` 작성 → 검증: 실제 변경 파일, 실제 실행한 검증, 미실행 검증, 남은 확인 필요가 기록됨.

## 리스크/확인 필요
- 실제 Unity Editor GUI에서 사람이 `빠른 발신`, 수동 `위성 확정 → 메시지 발신`, Reset 후 재실행, Play Mode 재진입을 조작한 근거는 아직 없음.
- Safe alternative: batch validation PASS, 1600x900 screenshot, display smoke를 보조 근거로 쓰되 GUI E2E 완료로 표현하지 않는다.
- `WORK_LOG.md`와 `orbitshare_shared_satellite_agent.md` 일부 제출물 표현은 최신 product 결정과 충돌 가능성이 있음.
- Safe alternative: legacy 원문을 삭제하지 않고 “현재 발표/제출 기준” 섹션을 우선 기준으로 명시한다.
- Windows/PPT/MP4/Submission 요구가 다시 생기면 자동 실행하지 않는다.
- Safe alternative: 기존 Unity 프로젝트 제출 결정, 정적 인벤토리, PNG/GIF 보조 자료 존재 여부를 먼저 정리한다.
- 새 Asset Store/SFX import, Git LFS, 대용량 리소스 정책 변경은 현재 dropped다.
- Safe alternative: `SpaceSkybox.mat`, `SpaceStation.prefab`과 procedural/generated fallback 유지 상태를 문서화한다.
- `plans/features`, `plans/verify`, `PM/status`, `PM/reports` 산출물이 없어 feature/verify/product audit/automation metrics 근거는 반영 불가.
- 작업트리에 tracked 변경과 미추적 daylog/plan/product 문서가 섞여 있어 제출/커밋 범위가 흐려질 수 있음.

## 병렬 에이전트 활용 메모
- 하위/병렬 에이전트는 사용하지 않음.
- 이유: 이번 lane note는 기존 문서와 근거를 통합 판단하는 read-only 계획 작업이고, 독립 수정 책임을 나눌 만큼 범위가 크지 않았음.
- 파일 검색과 읽기는 병렬 shell 호출로 처리했고, 통합 결론은 단일 lane note로 정리함.
- 내일 실제 작업에서 병렬화한다면 `제출 인벤토리 조사`, `발표 Q&A/legacy 충돌 조사`, `검증 근거 매트릭스 초안`은 독립 read-only 하위 작업으로 나눌 수 있다. 최종 README/WORK_LOG/daylog 반영은 단일 Docs Integrator가 수행해야 한다.