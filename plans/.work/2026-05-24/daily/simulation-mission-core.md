# Computer Graphics Assignment 1 - Simulation/Mission Core lane note (2026-05-25)

## 최근 진행 근거
- 사용자 제공 AGENTS 지침, 2026-05-24 KST: 프로젝트 내부/상위 `AGENTS.md`는 확인되지 않았고, product 문서와 daylog 우선 원칙 적용.
- `product/*.md`, 2026-05-18 KST: 제품 기준은 서울 고정 발신자, 목적지/발신 시각/메시지 선택, 24개 후보 위성, 1회성 단문 릴레이. 실제 TLE/결제/운영 DB/외부 업로드는 범위 밖.
- `README.md`, 2026-05-18 KST: 기본 Demo Flow는 `위성 확정 → 메시지 발신`, `빠른 발신`, relay/delivery 확인 중심이나, 2026-05-22~23 신규 core 기능 전체는 아직 충분히 반영되지 않음.
- `plans/daily/2026-05-24.md`, 2026-05-23 KST: core 후보는 대기열, 실패 시뮬레이션, 재시도, 영수증, 전송 로그, `CanServe()` 정합성, `GroundStation`/`ConnectionLine` lifecycle 확인.
- `daylog/2026-05-22.md`, 2026-05-22 KST: 단계형 발신, 예약 대기, payload 제한, 대기열/다음 발신, 실패/재시도, 영수증/전송 로그, 패킷 이펙트가 추가됐고 여러 Unity validation/build PASS 로그가 남음.
- `daylog/2026-05-23.md`, 2026-05-23 KST: 도쿄/파리/시드니/LA 목적지, 성공률/혼잡 필터, 추천 재시도, PlayerPrefs 저장, 실제 파일 첨부 경로, feature pack이 추가됐고 Linux build/display 실행 로그가 갱신됨.
- `daylog/2026-05-24.md`, 2026-05-24 KST: 지구 텍스처/컬러 보정 후 Linux build와 1280x720 player smoke는 확인됐지만, 새 `ValidateMissionFlow`는 실행하지 않았고 `orbitshare-validation.md`는 작업트리에서 삭제 상태로 보임.
- 코드 정적 확인, 2026-05-24 KST: `MissionManager`에는 queue/failure/retry/receipt/log/persistence가 있고, `SatelliteData.CanServe()`는 상태/공유 위성/배터리/목적지/window 조건이다. `UIManager`에는 추천/불가 사유가 별도 중복 구현되어 있음.
- scheduler/PM 근거, 2026-05-24 KST: `plans/features`, `plans/verify/ready`, `plans/verify/reports`는 비어 있고 `PM/status`, `PM/reports`, product audit, automation metrics 산출물은 없음.

## 내일 목표 후보
- 1순위: 신규 feature slice로 “다중 목적지/발신시각 route preflight source-of-truth”를 잡는다. 7개 목적지 확장과 추천/예약/재시도 조건을 core 기준으로 설명 가능하게 만든다.
- 2순위: `CanServe()`, `FindBestSatellite()`, queue 시작, 실패 재시도, 예약 실패 사유가 같은 조건을 쓰는지 정합성 점검한다.
- 3순위: 대기열 → 다음 발신 → 실패 시뮬레이션 → 추천 재시도 → 영수증/전송 로그를 최소 core sanity 시나리오로 검증한다.
- 4순위: `GroundStation`/`ConnectionLine`/`SignalPacketStream`이 Complete/Failed/Reset/Retry에서 stale 표시를 남기지 않는지 확인한다.
- 5순위: 삭제 상태인 `orbitshare-validation.md`를 되살리는 방식은 revert가 아니라 최신 validation 재실행 또는 미실행 사유 기록으로 처리한다.
- 6순위: 실제 TLE, 결제, 운영 DB, secret, 외부 업로드, Windows/Submission/MP4/PPT 작업은 자동 실행 계획에 넣지 않는다.

## 상세 체크리스트 초안
- [ ] 최신 제품 범위 재대조 → 검증: product 문서의 3개 목적지 기준과 2026-05-23 코드/daylog의 7개 목적지 확장을 표로 분리한다.
- [ ] route preflight helper 범위 결정 → 검증: 상태, 공유 여부, 배터리, 목적지 지원, 발신 가능 창, payload 초과 중 핵심 사유 1개를 core에서 반환할 수 있는지 확인한다.
- [ ] 구현이 작으면 `SatelliteData`/`SatelliteManager`에 사유 helper 추가 → 검증: 기존 `CanServe()` 결과와 helper의 가능/불가능 판정이 1:1로 맞는다.
- [ ] 구현이 크면 report-only matrix로 대체 → 검증: Seoul/New York/London/Tokyo/Paris/Sydney/LA × 지금/+5/+15/+30분 후보 수와 대표 불가 사유를 기록한다.
- [ ] 추천/예약 정합성 확인 → 검증: `FindBestSatellite()` 결과는 항상 `CanServe()` true이고, 예약 실패 문구가 실제 실패 조건과 모순되지 않는다.
- [ ] queue 상태 전환 점검 → 검증: `QueueMessage()`와 `StartNextQueuedMessage()`가 Idle/Complete/Failed에서만 시작되고 Running 중에는 대기열로 남는다.
- [ ] 실패 재시도 점검 → 검증: `SetSimulateFailureNextSend(true)` 후 Failed가 되고, `RetryFailedMission()`이 추천 위성/지연을 적용해 Complete 또는 명확한 실패 상태에 도달한다.
- [ ] 영수증/로그 생성 확인 → 검증: 성공 후 `SentMessages[0]`의 receipt id, 위성명, 목적지, payload, 가격 breakdown, success probability, `transmissionLog`가 비어 있지 않다.
- [ ] payload 제한 확인 → 검증: UTF-8 메시지 + 첨부 합산이 512KB를 넘으면 예약/대기열 저장이 막힌다.
- [ ] `GroundStation`/`ConnectionLine` lifecycle 확인 → 검증: 전송 중 line/packet이 켜지고 Complete/Failed/Reset 뒤 `SetVisible(false)`와 packet hide가 유지된다.
- [ ] 자동 검증 보강 여부 결정 → 검증: 가능하면 `OrbitShareEditorAutomation`에 queue/failure/receipt 최소 sanity section을 추가하고, 불안정하면 matrix와 미실행 사유만 남긴다.
- [ ] 최신 validation 근거 확보 → 검증: 실행 시 `orbitshare-validation.md`가 새 시각으로 생성되고 `Result: PASS`, Runtime Error/Exception 0을 기록한다.

## 리스크/확인 필요
- `orbitshare-validation.md`가 현재 삭제 상태라 product 문서의 “최신 PASS”를 그대로 인용하면 위험하다. Safe alternative: 최신 validation을 재생성하거나, 기존 로그 근거와 현재 파일 부재를 분리 기록한다.
- product 문서는 3개 목적지 기준인데 코드/daylog는 7개 목적지로 확장됐다. Safe alternative: product 갱신 전에는 “추정: 최신 구현 확장”으로 표시하고 core matrix로 실제 조건을 확인한다.
- UI 추천/불가 사유가 core `CanServe()`와 중복되어 불일치 가능성이 있다. Safe alternative: 큰 리팩터링 대신 helper 또는 report-only matrix부터 진행한다.
- `SimulationManager.TimeScale`은 현재 1x 고정이다. 과거 Pause/10x/60x 체크를 되살리면 현재 흐름과 충돌한다.
- 실제 Editor GUI E2E는 아직 확인 근거가 부족하다. Safe alternative: batch validation/player smoke는 보조 근거로만 쓰고 GUI 완료로 쓰지 않는다.
- 작업트리에 modified/deleted/untracked 파일이 많다. 계획상 되돌리지 말고 포함/제외 인벤토리만 별도 정리한다.

## 병렬 에이전트 활용 메모
- 하위/병렬 에이전트는 사용하지 않음.
- 이유: 이번 산출물은 단일 lane note이며, 필요한 근거는 product/daylog/plan/core script/log 정적 확인으로 충분했다. 파일 확인은 병렬 shell 호출로 처리하고 최종 판단은 단일 note로 통합했다.