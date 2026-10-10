# Computer Graphics Assignment 1 - Simulation/Mission Core lane note (2026-05-27)

## 최근 진행 근거
- 사용자 제공 AGENTS 지침, 2026-05-26 KST: 프로젝트/상위 `AGENTS.md` 파일은 확인되지 않아 사용자 제공 지침을 적용.
- `product/vision.md`, `product/roadmap.md`, `product/backlog.md`, `product/done-criteria.md`, `product/decisions.md`, 2026-05-26 확인: 제품 기준은 서울 고정 발신자, 7개 목적지, 24개 후보 위성, 발신 시각/메시지/첨부 선택, 1회성 단문 릴레이다.
- `product/backlog.md`, 2026-05-26 확인: 다음 후보로 “지연 시각별 미래 위치 예측”이 남아 있으며, 현재 동적 추천은 현재 위치 기준이라고 적혀 있다.
- `README.md`, `WORK_LOG.md`, 2026-05-26 확인: 7개 목적지, 24개 후보 위성, 조건 필터, 동적 가시권 추천, 대기/기록/재시도, 자동 검증 PASS가 최신 설명이다.
- `plans/daily/2026-05-26.md`, 2026-05-25 작성: core 목표는 preflight 기준, queue/failure/receipt sanity, line/packet lifecycle, 최신 validation 재생성으로 잡혀 있었다.
- `daylog/2026-05-26.md`, 2026-05-26 KST: 통신 경로를 `사용자(서울) → 인공위성 → 목적지` 기준으로 정리했고, `MissionVisualController.cs`, `ConnectionLine.cs`, `OrbitShareEditorAutomation.cs`, `orbitshare-validation.md` 갱신/확인을 기록했다.
- `orbitshare-validation.md`, 2026-05-26 확인: `Result: PASS`, Runtime Error/Exception 0, fast demo 2회 완료, 7개 목적지 존재 확인. 단 Route Recommendation Snapshot은 각 목적지에서 now/+5m/+15m/+30m 추천 위성이 모두 동일하다.
- `Assets/Scripts/SatelliteData.cs`, 2026-05-26 정적 확인: `supportsSeoul/NewYork/London/Tokyo/Paris/Sydney/LosAngeles` 7개 목적지 지원 플래그와 `SupportsTarget()`이 구현되어 있다.
- `Assets/Scripts/SatelliteManager.cs`, `SatelliteRouteEstimator.cs`, 2026-05-26 정적 확인: `requestedDelayMinutes`는 contact allowance와 delivery time에는 반영되지만, 동적 Transform 기반 표면각 자체를 미래 시각으로 투영하지는 않는다.
- `Assets/Scripts/MissionManager.cs`, 2026-05-26 정적 확인: queue route detail, retry suggestion, receipt/log, payload limit, route replay가 구현되어 있으며 `GetQueuedMessageRouteDetail()`은 `FindBestSatellite()` 결과를 사용한다.
- `Assets/Scripts/SimulationManager.cs`, `SatelliteOrbit.cs`, `GroundStation.cs`, `ConnectionLine.cs`, 2026-05-26 정적 확인: `TimeScale`은 1x 기준이고, connection line은 source/destination override와 packet hide를 지원한다.
- `plans/features`, `plans/verify/ready`, `plans/verify/reports`, 2026-05-26 확인: 디렉터리는 있으나 파일 없음. `PM/status`, `PM/reports`, product audit, automation metrics, `docs` 산출물도 확인되지 않음.
- 기존 `plans/daily/2026-05-27.md`, 2026-05-26 확인: 없음.

## 내일 목표 후보
- 1순위: 신규 feature slice로 `Scheduled Route Forecast v1`을 진행한다. 발신 시각 now/+5/+15/+30에 따라 미래 위성 위치를 로컬 시뮬레이션 기준으로 반영해 추천/예상 도착/점수가 달라질 수 있게 만든다.
- 2순위: forecast 결과를 `FindBestSatellite()`, `GetDestinationRecommendations()`, `ReserveSelected()`, `GetQueuedMessageRouteDetail()`, retry suggestion이 같은 기준으로 쓰는지 확인한다.
- 3순위: queue → 예약 발신 → 실패 시뮬레이션 → 추천 재시도 → 영수증/전송 로그를 core sanity로 묶어 자동 검증 또는 report-only matrix에 남긴다.
- 4순위: `ConnectionLine`/packet lifecycle은 route-clarity 작업 이후 Complete/Failed/Reset에서 stale source/destination 또는 패킷 잔상이 없는지 확인한다.
- 5순위: 코드 변경 후 `orbitshare-validation.md`를 재생성하고, 실제 실행하지 않은 검증은 문서 근거로 쓰지 않는다.

## 상세 체크리스트 초안
- [ ] 현재 route snapshot 기준선 기록 → 검증: `orbitshare-validation.md`의 7개 목적지 now/+5/+15/+30 추천 위성/후보 수를 변경 전 기준으로 표기한다.
- [ ] forecast 범위 확정 → 검증: 실제 TLE/정밀 궤도역학 없이 `SatelliteOrbit`/`EnterpriseSatelliteAnimator`의 속도와 delay만 쓰는 로컬 추정으로 제한한다.
- [ ] `SatelliteRouteEstimator`에 미래 표면각 추정 helper 추가 여부 결정 → 검증: `requestedDelayMinutes`가 0/5/15/30일 때 `routeAngleDegrees`, `estimatedWindowSeconds`, `score` 중 하나 이상이 달라지는지 확인한다.
- [ ] `SatelliteManager.BuildRouteEstimate()`에 forecast 적용 → 검증: 기존 `supportsTarget`, shared access, battery, availability state, contact allowance 판정과 모순 없이 `canServe`가 계산된다.
- [ ] 추천 다양성 smoke → 검증: 7개 목적지 matrix에서 추천 위성 또는 forecast metric delta가 기록되고, 동일 추천이면 동일한 이유가 `reasonSummary`에 설명된다.
- [ ] 예약 정합성 확인 → 검증: `FindBestSatellite()`가 반환한 위성은 같은 target/delay/payload에서 `CanReserveSelected()` 또는 `CanServeSatellite()`와 일치한다.
- [ ] queue forecast 연결 확인 → 검증: `GetQueuedMessageRouteDetail()`이 queued schedule의 effective delay 기준 추천 위성, 가격, 성공률, 도착 예상, route summary를 반환한다.
- [ ] retry forecast 연결 확인 → 검증: 실패 후 `UpdateSuggestedRetryAlternative()`/`RetryFailedMission()`이 now/+5/+15/+30 후보 중 예약 가능한 추천을 제시하거나 명확한 실패 사유를 남긴다.
- [ ] payload 제한 회귀 확인 → 검증: 메시지+첨부 512KB 초과 시 queue 저장/예약 시작이 막히고, forecast 변경이 payload 판정을 우회하지 않는다.
- [ ] receipt/log 회귀 확인 → 검증: 성공 후 `SentMessages[0]`에 tracking id, 목적지, 위성명, payload, price breakdown, success probability, `transmissionLog`가 남는다.
- [ ] line/packet lifecycle 확인 → 검증: Running/Transmitting 중 표시되고 Complete/Failed/Reset 뒤 `ConnectionLine.SetVisible(false)`와 packet hide가 유지된다.
- [ ] validation 보강 여부 결정 → 검증: 안정적이면 `OrbitShareEditorAutomation`에 route forecast matrix/queue sanity를 추가하고, 타이밍 의존이 크면 별도 report-only matrix로 대체한다.
- [ ] 코드 변경 후 정적 검증 → 검증: `git diff --check`, `bash -n run_orbitshare.sh` 통과.
- [ ] 코드 변경 후 Unity 검증 → 검증: `ValidateMissionFlow` 실행 시 `orbitshare-validation.md`가 새로 생성되고 `Result: PASS`, Runtime Error/Exception 0, 7개 목적지 snapshot을 기록한다.

## 리스크/확인 필요
- 미래 위치 예측이 실제 궤도/TLE 구현으로 커질 수 있다. Safe alternative: 현재 Transform, orbit speed, deterministic hash만 쓰는 mock/fixture성 forecast로 제한한다.
- 추천 위성이 delay별로 반드시 달라진다고 보장하면 테스트가 brittle해질 수 있다. Safe alternative: 추천명 변화 대신 `routeAngle/window/score` delta도 성공 기준으로 허용한다.
- 현재 validation은 640x480 자동 검증이고, UI 품질 최종 판정은 별도 visual 근거가 필요하다. Simulation lane에서는 core 결과만 넘긴다.
- Unity validation/build/capture는 project lock과 로그 충돌 위험이 있다. Safe alternative: 단일 runner가 순차 실행하도록 build-validation lane에 넘긴다.
- 작업트리에 대량 modified/deleted/untracked가 섞여 있다. Safe alternative: revert 없이 변경 범위 인벤토리만 작성하고, core 변경은 필요한 파일로 한정한다.
- 실제 결제, 운영 DB, secret, 외부 업로드, 실제 TLE/live data, Windows/MP4/PPT/Submission은 요구 변경 전 실행 계획에 넣지 않는다.

## 병렬 에이전트 활용 메모
- 하위/병렬 에이전트는 사용하지 않음.
- 이유: 이번 산출물은 단일 lane note이고, 필요한 근거는 product/daylog/plan/code 정적 확인으로 충분했다. 파일 조회만 병렬 shell 호출로 처리하고 최종 판단은 단일 note로 통합했다.