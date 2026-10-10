# Computer Graphics Assignment 1 - Spec/Work Log/Submission Docs weekly lane note (2026-W22)

## 이번 주 목표 후보

- 제출/발표 evidence pack을 만든다: 3분 시연 순서, 성공 기준, 증거 파일, 미확인 항목을 README 또는 WORK_LOG에서 한눈에 보이게 정리한다.
- `WORK_LOG.md`를 최신화한다: 2026-05-22~25 기능 확장, 실행한 검증, 미실행 GUI E2E, dropped 산출물을 분리한다.
- product 문서 정합성을 맞춘다: 3개 목적지 기준 문구와 최신 7개 목적지/대기열/기록/재시도/고해상도 지구 텍스처 상태를 충돌 없이 정리한다.
- `orbitshare-validation.md` 상태를 바로잡는다: 과거 PASS 로그와 현재 리포트 삭제/재생성 필요를 섞지 않는다.
- 발표 Q&A를 최신 단문 발신 컨셉 기준으로 정리한다: 실제 TLE, 결제, live data가 아니라 시뮬레이션 값임을 명시한다.
- 제출/Git 인벤토리를 작성한다: 현재 `modified 21`, `deleted 270`, `untracked 33`, 대량 에셋 삭제, 신규 shader/script/texture, CC BY 4.0 attribution을 분리한다.
- `plans/features`, `plans/verify/ready`, `PM/status`, `PM/reports`, product audit, automation metrics 산출물은 현재 확인되지 않으므로 없는 근거를 인용하지 않는다.

## 날짜별/단계별 체크리스트

### 2026-05-25 월요일 - 주간 기준선 확정

- [ ] 최근 daylog, `plans/weekly/2026-W21.md`, `plans/daily/2026-05-25.md`, `plans/daily/2026-05-26.md`, product 문서를 기준으로 W22 문서 gap을 정리한다. -> 검증: W22 기존 주간 파일 없음, PM/scheduler 산출물 없음, 최신 근거/부재 근거가 분리됨.
- [ ] README/WORK_LOG/product/Q&A의 최신성 차이를 표로 잡는다. -> 검증: README는 2026-05-25 흐름 반영, WORK_LOG/product는 일부 2026-05-18 기준으로 남아 있음을 표시.
- [ ] `orbitshare-validation.md` 삭제 상태를 주간 리스크로 올린다. -> 검증: 과거 PASS를 최신 PASS로 쓰지 않음.

### 2026-05-26 화요일 - Evidence Pack 초안

- [ ] 2026-05-22~25 기능 delta 표를 작성한다. -> 검증: 단계형 발신, 대기열, 기록/재전송, 실패 재시도, 다중 첨부, 7개 목적지, 고해상도 지구 텍스처가 반영/미반영/확인 필요로 분류됨.
- [ ] 제출/발표 truth table 초안을 작성한다. -> 검증: validation, Linux build, display smoke, screenshot, GUI E2E, Resource Hook, license, dropped 산출물이 서로 다른 증거 등급으로 표시됨.
- [ ] `Mission Readiness Preflight v1 + 시연 준비 프리셋`의 문서 성공 기준을 먼저 정의한다. -> 검증: 자동 발신 없는 준비 프리셋, 7개 목적지 matrix, 수동 발신 리허설 근거가 필요한 항목으로 분리됨.

### 2026-05-27 수요일 - 검증 결과 반영 대기/통합

- [ ] Build/Validation lane의 최신 `ValidateMissionFlow` 결과를 반영한다. -> 검증: 새 `orbitshare-validation.md`가 있으면 PASS/FAIL/Runtime Error 수를 기록하고, 없으면 재생성 필요로 유지.
- [ ] GUI/Player 리허설 결과를 문서에 반영한다. -> 검증: `빠른 발신`, 수동 `시간 → 메시지/첨부 → 목적지 → 위성 → 보내기`, 대기열/기록/재시도 결과가 실행됨/미실행/blocked로 구분됨.
- [ ] screenshot과 GUI E2E를 혼동하지 않는다. -> 검증: `orbitshare-playmode.png` 1600x900은 visual evidence로만 표기.

### 2026-05-28 목요일 - WORK_LOG/Product/Q&A 정합성

- [ ] `WORK_LOG.md` 최신화 후보를 확정한다. -> 검증: 최종 점검일, 최신 변경, 검증 로그, 미실행 항목, dropped 항목이 한 섹션에서 충돌 없이 정리됨.
- [ ] product 문서 수정 후보를 확정한다. -> 검증: product/vision, roadmap, backlog, done-criteria가 7개 목적지와 최신 validation 상태를 과대해석하지 않음.
- [ ] 발표 Q&A를 최신 컨셉으로 정리한다. -> 검증: legacy 관측/예약 플랫폼 문구보다 서울 발신 단문 메시지 릴레이 컨셉이 우선됨.

### 2026-05-29 금요일 - 제출/Git/라이선스 인벤토리

- [ ] 제출 포함/제외 후보를 작성한다. -> 검증: `Assets/`, `Packages/`, `ProjectSettings/`, README/WORK_LOG/product/daylog, build/log/screenshot/validation 상태가 분리됨.
- [ ] 대량 삭제와 신규 파일군을 되돌리지 않고 분류한다. -> 검증: Asset Store 원본 삭제, Earth texture/shader 신규 추가, validation 삭제 상태가 표로 남음.
- [ ] 라이선스/용량 handoff를 정리한다. -> 검증: `Assets/Resources/OrbitShare/Earth/ATTRIBUTION.md`, CC BY 4.0, private repo 필요성, Git LFS dropped 상태가 명시됨.

### 2026-05-30 토요일 - 최종 문서 통합

- [ ] README, WORK_LOG, product, Q&A를 최신 검증 결과 기준으로 통합한다. -> 검증: 실행하지 않은 검증을 완료로 쓰지 않고, 문서 간 수치/상태/제출 기준이 일치함.
- [ ] 성공 기준 매핑표를 닫는다. -> 검증: 각 기준이 validation, GUI 리허설, screenshot, build/display smoke, 문서 중 하나의 근거와 연결됨.
- [ ] daylog 반영 상태를 확인한다. -> 검증: 실제 수정 파일, 실행한 검증, 실패/미실행 검증, 남은 리스크가 해당 날짜 로그에 남음.

### 2026-05-31 일요일 - 제출 전 Freeze 점검

- [ ] 최종 제출 체크리스트를 닫는다. -> 검증: 필수/선택/제외/확인 필요 항목이 분리됨.
- [ ] Windows/PPT/MP4/Submission/새 에셋/LFS가 되살아나지 않았는지 확인한다. -> 검증: 요구 변경 전까지 product decision의 dropped 상태 유지.
- [ ] 발표자가 읽을 한 줄 설명과 제한사항을 최종 확인한다. -> 검증: 실제 구현과 문서 설명이 같은 제품 컨셉을 말함.

## 검증 계획

- 문서 정합성: `README.md`, `WORK_LOG.md`, `product/*.md`, `orbitshare_shared_satellite_agent.md`, daylog의 기능/검증/제출 상태가 충돌하지 않는지 확인한다.
- 증거 등급 분리: Static, Unity Batch, Display Smoke, Visual Review, GUI E2E, Build, Submission을 한 표에서 구분한다.
- validation 판정: 현재 `orbitshare-validation.md`는 없음. 새로 생성되기 전까지 최신 PASS로 쓰지 않는다.
- 제출물 판정: 파일 검색과 `git status --short --branch`로 존재/없음/삭제/미추적/제외 후보를 확인한다.
- 실행 검증 반영: 이번 lane note에서는 새 Unity 검증을 실행하지 않았으므로, 주중 Build/Validation lane 결과만 실제 실행 근거로 반영한다.

## 리스크/확인 필요

- `plans/weekly/2026-W22.md`, `docs/`, `PM/status`, `PM/reports`, product audit, automation metrics 산출물이 현재 확인되지 않는다.
- `orbitshare-validation.md`가 tracked 삭제 상태다. Safe alternative: 과거 PASS는 과거 근거로만 두고 최신 리포트 재생성 전까지 “확인 필요”로 둔다.
- 실제 Editor GUI 수동 E2E가 아직 부족하다. Safe alternative: screenshot/display smoke는 보조 근거로만 쓰고 GUI 완료로 쓰지 않는다.
- product 문서의 3개 목적지 기준과 최신 7개 목적지 구현 기록이 충돌할 수 있다.
- 작업트리에 대량 삭제와 untracked 리소스가 섞여 있다. Safe alternative: revert 없이 포함/제외 인벤토리만 만든다.
- CC BY 4.0 지구 텍스처와 Asset Store 리소스가 있어 attribution, private repo, 제출 포함 범위를 명확히 해야 한다.
- Windows build, MP4/MOV, PPT/PDF, `Submission/` 패키징, 새 Asset Store/SFX import, Git LFS는 요구 변경 전까지 dropped 유지가 맞다.

## 병렬 에이전트 활용 메모

- 이번 weekly lane note 작성에는 하위/병렬 에이전트를 사용하지 않았다. 최종 산출물이 단일 문서 note이고, 범위가 문서/로그 근거 통합이라 병렬 에이전트 이점이 작았다.
- 주중 실제 작업에서는 병렬화 가능: `Docs Product Delta Auditor`, `Submission/Git Inventory Auditor`, `Q&A Legacy Conflict Auditor`, `Validation Evidence Auditor`를 읽기 전용으로 나눌 수 있다.
- 최종 README/WORK_LOG/product/Q&A/daylog 반영은 단일 Docs Integrator가 통합해야 한다. Unity validation/build/capture/resource hook은 다른 lane의 단일 runner가 순차 실행한 결과만 받는다.