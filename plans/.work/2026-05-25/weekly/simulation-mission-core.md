# Computer Graphics Assignment 1 - Simulation/Mission Core weekly lane note (2026-W22)

## 이번 주 목표 후보
- 기준: `plans/weekly/2026-W22.md`, `PM/`, `plans/features`, `plans/verify/ready`는 확인되지 않음. 최신 근거는 `product/*.md`, `plans/weekly/2026-W21.md`, `plans/daily/2026-05-25.md`, `daylog/2026-05-22`~`2026-05-25`, core script 정적 확인이다.
- 1순위 목표: `7개 목적지 coverage/preflight source-of-truth v1`을 만든다. Seoul/New York/London/Tokyo/Paris/Sydney/Los Angeles와 발신 시각별로 “가능/불가능/추천 위성/불가 사유”가 같은 core 기준에서 설명되게 한다.
- 2순위 목표: `CanServe()`, `FindBestSatellite()`, `ReserveSelected()`, queue route detail, 실패 재시도 추천이 같은 조건을 쓰는지 맞춘다.
- 3순위 목표: 대기열 → 다음 발신 → 실패 시뮬레이션 → 추천 재시도 → 전송 기록/receipt/log 흐름을 core sanity 시나리오로 검증한다.
- 4순위 목표: `SatelliteOrbit`/`SimulationManager` 1x 흐름과 `GroundStation`/`ConnectionLine`/packet lifecycle이 Complete/Failed/Reset/Retry 뒤 stale 상태를 남기지 않게 확인한다.
- 5순위 목표: 삭제 상태인 `orbitshare-validation.md`는 revert가 아니라 최신 validation 재실행 또는 미실행 사유 기록으로 복구한다.

## 날짜별/단계별 체크리스트
- 5/25 월: 기준선 확정 → 검증: product 문서의 3개 목적지 기준과 코드/daylog의 7개 목적지 확장을 표로 분리한다.
- 5/25 월: 현재 core API 조사 → 검증: `SatelliteData`, `SatelliteManager`, `MissionManager`, `SimulationManager`, `SatelliteOrbit`, `GroundStation`, `ConnectionLine`의 실제 조건/상태 전환을 확인한다.
- 5/26 화: preflight 범위 결정 → 검증: 상태, 공유 위성 여부, 배터리, 목적지 지원, 발신 가능 창, payload 초과 중 core에서 설명할 불가 사유를 확정한다.
- 5/26 화: 구현 또는 report-only matrix 선택 → 검증: 작은 helper로 가능하면 `CanServe()`와 모순 없이 추가하고, 커지면 7개 목적지 × 지금/+5/+15/+30분 matrix로 대체한다.
- 5/27 수: 추천/예약 정합성 점검 → 검증: `FindBestSatellite()` 결과는 항상 `CanServe()` true이고, `ReserveSelected()` 실패 문구가 실제 실패 조건과 충돌하지 않는다.
- 5/27 수: queue route detail 점검 → 검증: `GetQueuedMessageRouteDetail()`이 추천 위성 없음/있음, payload, 가격, 성공률, 첨부 요약을 빈 값 없이 반환한다.
- 5/28 목: 실패/재시도/기록 sanity → 검증: `SetSimulateFailureNextSend(true)` 후 Failed가 되고 `RetryFailedMission()`이 추천 위성/지연으로 재시작하며 성공 시 `SentMessages[0]`에 tracking id, 목적지, 위성, payload, 가격 breakdown, success, transmission log가 남는다.
- 5/28 목: payload 제한 확인 → 검증: UTF-8 메시지 + 첨부 합산 512KB 초과 시 예약/대기열 저장이 막힌다.
- 5/29 금: orbit/line lifecycle 확인 → 검증: Running/Transmitting 중 `ConnectionLine`과 packet stream이 보이고 Complete/Failed/Reset 후 `SetVisible(false)`와 packet hide가 유지된다.
- 5/29 금: validation 보강 여부 결정 → 검증: queue/failure/receipt 최소 sanity를 `OrbitShareEditorAutomation`에 작게 추가하거나, 불안정하면 matrix를 검증 산출물로 남긴다.
- 5/30 토: 최신 validation 실행 버퍼 → 검증: 실행 시 `orbitshare-validation.md`가 새로 생성되고 `Result: PASS`, Runtime Error/Exception 0, Fast Demo, Reset replay가 기록된다.
- 5/31 일: freeze/핸드오프 → 검증: simulation lane의 완료/미실행/보류 항목을 spec lane daylog/문서 갱신용 truth table로 넘긴다.

## 검증 계획
- Static: 7개 목적지 × 4개 발신 시각 matrix로 후보 수, 추천 위성, 대표 불가 사유, 예약 가능 여부를 기록한다.
- Core scenario: 대기열 추가, 다음 발신 시작, 실패 시뮬레이션, 재시도, 완료 기록 생성을 한 흐름으로 확인한다.
- Unity batch: 코드 변경 후에만 `OrbitShareEditorAutomation.ValidateMissionFlow`를 실행해 `orbitshare-validation.md`를 재생성한다.
- GUI E2E: 실제 Editor 조작은 UI lane과 연결해 확인하되, batch validation/player smoke를 GUI 완료 근거로 쓰지 않는다.
- Regression: `git diff --check`, Unity compile/build 로그, Runtime Error/Exception 0을 분리해서 기록한다.

## 리스크/확인 필요
- `orbitshare-validation.md`가 tracked 삭제 상태라 최신 PASS로 단정하면 안 된다.
- `SupportsTarget()`은 현재 Seoul/New York/London 외 목적지를 모두 지원 처리한다. 7개 목적지 차별화를 구현할지, report-only matrix로 둘지 먼저 결정해야 한다.
- product 문서는 2026-05-22 이후의 7개 목적지, queue, failure/retry, record/log 상태를 일부 반영하지 못한다.
- `MissionManager`의 PlayerPrefs persistence가 validation 재현성에 영향을 줄 수 있다. 검증 전 상태 초기화 절차가 필요할 수 있다.
- `SimulationManager.TimeScale`은 1x 고정이다. 과거 시간가속 계획은 되살리지 않는다.
- 실제 TLE/live data, 결제, 운영 DB, secret, 외부 업로드는 범위 밖이다. 필요 시 local fixture/mock/report-only 대안으로 제한한다.
- 작업트리에 대량 삭제와 untracked 파일이 섞여 있다. 이 lane에서는 되돌리지 않고 포함/제외 인벤토리만 spec/build lane에 넘긴다.

## 병렬 에이전트 활용 메모
- 이번 lane note 작성에는 하위 에이전트를 쓰지 않았고, 문서/코드/로그 확인만 병렬 shell 호출로 나눠 읽었다.
- 주중 실행이 커지면 `Core Preflight Matrix`, `Queue/Failure Sanity`, `Orbit/Line Lifecycle` 조사를 서로 다른 읽기 전용 하위 작업으로 나눌 수 있다.
- Unity validation/build/capture/player 실행은 project lock과 로그 충돌 위험이 있으므로 단일 runner가 순차 실행해야 한다.