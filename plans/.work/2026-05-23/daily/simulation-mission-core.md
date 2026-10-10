# Computer Graphics Assignment 1 - Simulation/Mission Core lane note (2026-05-24)

## 최근 진행 근거
- 사용자 제공 AGENTS 지침, 2026-05-23 KST: 프로젝트 내부/상위 경로에 별도 `AGENTS.md` 없음. 사용자 제공 지침과 product 문서 우선 적용.
- `product/vision.md`, `product/roadmap.md`, `product/backlog.md`, `product/done-criteria.md`, `product/decisions.md`, 2026-05-18 KST: 현재 제품 기준은 서울 고정 발신자, 3개 목적지, 24개 후보 위성, 1회성 단문 릴레이이며 최종 남은 핵심은 실제 Editor GUI 리허설.
- `daylog/2026-05-22.md`, 2026-05-22 KST: 예약/메시지/첨부/대기열/기록/재전송/혼잡도/실패 원인/전송 로그 기능이 추가됐고, `unity-validation-more-features.log` 기준 자동 검증 PASS 및 `unity-build-more-features.log` 기준 Linux build success.
- `daylog/2026-05-23.md`, 2026-05-23 KST: 2026-05-22 이후 신규 코드/에셋 변경 근거 없음. 최신 확인 가능한 실행 근거는 2026-05-22 22:04 KST의 `orbitshare-validation.md`, `unity-validation-more-features.log`, `unity-build-more-features.log`.
- `orbitshare-validation.md`, 2026-05-22 KST: `Result: PASS`, 위성 24개, 기본 목적지 New York, Fast demo 2회 완료, relay/delivery 100%, Reset replay 2회, Runtime Error/Exception 0. 단 `Screen: 640x480`이라 발표 해상도 GUI E2E 근거는 아님.
- `Assets/Scripts/SatelliteData.cs`, `SatelliteManager.cs`, `MissionManager.cs`, 2026-05-23 확인: `CanServe()`는 상태/공유 위성/battery/목적지/window 조건이고, `MissionManager`에는 대기열, 실패 재시도, 영수증, 로그, 혼잡도 비용/성공률 반영이 들어가 있음.
- `Assets/Scripts/UIManager.cs`, 2026-05-23 확인: 추천/불가 사유는 UI 내부 `RecommendationReason()`에 중복 구현되어 있어 core source-of-truth와 어긋날 리스크가 있음.
- `plans/features`, `plans/verify/ready`, `plans/verify/reports`: 디렉터리는 있으나 파일 없음. `PM/status`, `PM/reports`, product audit, automation metrics 산출물 없음. 기존 `plans/daily/2026-05-24.md`도 확인되지 않음.

## 내일 목표 후보
- 1순위: 최종 freeze 원칙을 지키면서, 2026-05-22에 추가된 core 기능을 “발표 가능한 서비스 흐름”으로 묶는 최소 feature slice를 잡는다. 후보: `대기열 → 다음 발신 → 실패 시뮬레이션 → 재시도 → 영수증/전송 로그` core 시나리오를 자동/수동 검증 대상으로 명확히 만든다.
- 2순위: 위성 가능/불가능 사유를 UI 중복 로직이 아니라 `SatelliteData`/`SatelliteManager` 쪽 source-of-truth로 정리할지 결정한다. 구현이 크면 report-only matrix로 대체한다.
- 3순위: `CanServe()`, 추천 위성, 예약, 대기열 시작, 재전송, 실패 재시도 흐름이 같은 조건을 쓰는지 정합성 점검.
- 4순위: `GroundStation`/`ConnectionLine`은 전송 중 표시, Complete/Failed/Reset 후 숨김을 최신 queue/failure 흐름에서도 확인.
- 5순위: 자동 검증은 기존 Fast demo만 과신하지 말고, 새 core 기능의 최소 샘플 체크를 추가하거나 미추가 사유를 남긴다.
- 6순위: 실제 결제/실제 궤도/TLE/운영 DB/외부 업로드/Windows/Submission/MP4/PPT는 계획에서 실행 대상으로 잡지 않는다.

## 상세 체크리스트 초안
- [ ] 새 core 시나리오 범위 확정 → 검증: `빠른 발신` 기존 PASS와 별개로 대기열, 실패, 재시도, 영수증/로그 중 0.5일 안에 검증 가능한 최소 경로를 1개로 제한한다.
- [ ] queue/failure/retry 상태 전환 점검 → 검증: `QueueMessage()`, `StartNextQueuedMessage()`, `SetSimulateFailureNextSend()`, `RetryFailedMission()`이 Idle/Complete/Failed 상태에서만 의도대로 동작하는지 정적 확인과 Play/validation 결과로 대조한다.
- [ ] 영수증/전송 로그 생성 확인 → 검증: 성공 전송 후 `SentMessages[0]`, `receiptId`, `transmissionLog`, 가격 breakdown, 성공률이 비어 있지 않은지 확인한다.
- [ ] 위성 사유 source-of-truth 결정 → 검증: `CanServe()` 조건과 UI `RecommendationReason()`의 조건 순서가 다른 지점을 표로 기록하고, 안전하면 core helper로 통합, 크면 report-only로 보류한다.
- [ ] 추천/예약 정합성 확인 → 검증: `FindBestSatellite()`가 반환한 위성은 항상 `CanServe()` true이고, `ReserveSelected()` 실패 문구가 실제 실패 조건과 모순되지 않는지 확인한다.
- [ ] 목적지/발신 시각 matrix 재확인 → 검증: Seoul/New York/London × 지금/+5/+15/+30분 후보 수와 대표 불가 사유를 기록한다.
- [ ] payload/혼잡도 계산 확인 → 검증: 메시지+첨부 용량이 512KB 초과일 때 예약이 막히고, 혼잡도는 요금/성공률/전송 시간에 시뮬레이션 값으로만 반영되는지 확인한다.
- [ ] `GroundStation`/`ConnectionLine` lifecycle 확인 → 검증: Running/Transmitting 중 line과 packet stream이 보이고 Complete/Failed/Reset 후 `SetVisible(false)` 상태가 유지되는지 확인한다.
- [ ] 자동 검증 보강 여부 결정 → 검증: 가능하면 `OrbitShareEditorAutomation`에 queue/failure/receipt 샘플 체크를 소규모로 추가하고, 추가하지 않으면 기존 validation이 Fast demo 중심이라는 한계를 daylog에 남긴다.
- [ ] 변경 후 회귀 검증 → 검증: 실행 시 `OrbitShareEditorAutomation.ValidateMissionFlow`가 `Result: PASS`, Runtime Error/Exception 0을 유지하고, Linux build 실행 여부와 결과/보류 사유를 분리 기록한다.

## 리스크/확인 필요
- 실제 Editor GUI에서 최신 단계형 플로우, 드래그 패널, 대기열, 실패 재시도, 기록 버튼을 사람이 끝까지 누른 근거가 아직 없다. Safe alternative: 자동 validation과 Linux player smoke는 보조 근거로만 쓰고 GUI E2E 완료로 쓰지 않는다.
- 2026-05-22 이후 시간가속 UI는 제거됐고 `SimulationManager.TimeScale`도 1x 고정이다. 과거 계획의 Pause/1x/5x/10x 체크를 그대로 되살리면 현재 제품 흐름과 충돌한다.
- UI의 추천/불가 사유가 core `CanServe()`와 중복되어 유지보수 리스크가 있다. Safe alternative: core helper 구현이 UI 변경을 키우면 matrix/report-only로 먼저 남긴다.
- headless screenshot capture는 2026-05-22에 Unity native crash가 재발했다. core 검증은 screenshot 성공으로 단정하지 않고 validation/log/GUI 수동 확인과 분리한다.
- 현재 작업트리에 대규모 tracked 삭제, modified 파일, untracked 신규 스크립트/리소스/daylog가 섞여 있다. 계획상 되돌리지 말고 포함 범위만 별도 정리해야 한다.
- 실제 TLE, 실제 위성 가용성, 실제 결제/운영 DB/API는 범위 밖이다. Safe alternative: 현재 시뮬레이션 데이터와 제한사항 문구를 유지한다.

## 병렬 에이전트 활용 메모
- 사용하지 않음.
- 이번 lane note는 product 문서, 최근 daylog/plan, validation report, core script 정적 확인으로 범위가 충분히 좁았고 최종 산출물이 단일 lane note라 병렬 하위 에이전트 없이 통합했다.