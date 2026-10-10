# Computer Graphics Assignment 1 - Simulation/Mission Core lane note (2026-05-26)

## 최근 진행 근거
- 사용자 제공 AGENTS 지침, 2026-05-25 KST: 프로젝트 내부/상위 `AGENTS.md`는 확인되지 않아 사용자 제공 지침과 product 문서 우선 원칙 적용.
- `product/vision.md`, `product/roadmap.md`, `product/backlog.md`, `product/done-criteria.md`, `product/decisions.md`, 2026-05-18~2026-05-24 KST: 제품 기준은 서울 발신자, 목적지/발신 시각/메시지/첨부, 24개 후보 위성, 1회성 단문 릴레이. 실제 TLE/결제/운영 DB/외부 업로드/Windows/MP4/PPT/Submission은 제외.
- `README.md`, 2026-05-25 확인: 현재 발표 흐름은 `시간 선택 → 메시지/첨부 → 목적지 선택 → 위성 추천/선택 → 메시지 보내기 → 대기/기록 확인`.
- `plans/daily/2026-05-25.md`, 2026-05-24 KST: `Route Preflight + 시연 준비 프리셋`, core source-of-truth, queue/failure/receipt sanity, 최신 validation 재생성이 핵심 목표로 잡힘.
- `daylog/2026-05-25.md`, 2026-05-25 KST: UI/전송 흐름 polish, 추천 이유/대기 메시지/전송 기록 요약 API 보강, 1600x900 `orbitshare-playmode.png` 확인, Linux build success 로그 확인. 단 `orbitshare-validation.md`는 tracked 삭제 상태.
- `daylog/2026-05-22.md`~`daylog/2026-05-24.md`: 단계형 발신, 대기열, 실패/재시도, 영수증/전송 로그, PlayerPrefs 저장, 7개 목적지, 고해상도 지구 텍스처, display smoke/build 근거가 누적됨.
- `Assets/Scripts/MissionManager.cs`, 2026-05-25 정적 확인: queue, hold/update/load, auto-start queue, failure simulation, retry, receipt/log, route replay, destination comparison, model state summary, persistence가 구현되어 있음.
- `Assets/Scripts/SatelliteData.cs`, `SatelliteManager.cs`, 2026-05-25 정적 확인: `CanServe()`는 상태/공유 위성/배터리/목적지/window 조건을 사용하지만 `SupportsTarget()`은 Seoul/New York/London 외 비어 있지 않은 목적지를 모두 지원으로 처리함.
- `Assets/Scripts/OrbitShareBootstrap.cs`, 2026-05-25 정적 확인: 목적지는 Seoul/New York/London/Tokyo/Paris/Sydney/Los Angeles 7개가 생성됨.
- `Assets/Scripts/SimulationManager.cs`, `SatelliteOrbit.cs`, `GroundStation.cs`, `ConnectionLine.cs`, 2026-05-25 정적 확인: `TimeScale`은 1x 고정, 궤도는 `ScaledDeltaTime` 기반, connection line과 packet stream은 `SetVisible(false)`로 숨김 처리 가능.
- `plans/features`, `plans/verify/ready`, `plans/verify/reports`, 2026-05-25 확인: 파일 없음. `PM/`, `PM/status`, `PM/reports`, product audit, automation metrics 산출물 없음.
- 기존 `plans/daily/2026-05-26.md`, 2026-05-25 확인: 없음.

## 내일 목표 후보
- 1순위: 신규 feature slice로 `7개 목적지 coverage/preflight source-of-truth v1`을 잡는다. 확장 목적지에도 위성별 가능/불가능 차이가 보이도록 로컬 시뮬레이션 기준을 정리한다.
- 2순위: `CanServe()`, `FindBestSatellite()`, `ReserveSelected()`, queue route detail, retry 추천이 같은 preflight 기준을 쓰는지 확인한다.
- 3순위: 대기열 → 다음 발신 → 실패 시뮬레이션 → 추천 재시도 → 영수증/전송 로그를 core sanity 시나리오로 검증한다.
- 4순위: `SatelliteOrbit`/`SimulationManager` 1x 고정 흐름과 `GroundStation`/`ConnectionLine`/packet lifecycle이 Reset/Failed/Complete에서 stale 상태를 남기지 않는지 확인한다.
- 5순위: `orbitshare-validation.md`는 revert가 아니라 최신 validation 재실행 또는 미실행 사유 기록으로 복구한다.

## 상세 체크리스트 초안
- [ ] product/구현 차이 표 작성 → 검증: product 문서의 3개 목적지 기준과 코드의 7개 목적지 확장을 분리 기록한다.
- [ ] 7개 목적지 coverage v1 범위 결정 → 검증: 실제 TLE 없이 로컬 fixture/mock 기준으로 위성별 지원 목적지 또는 preflight 사유를 정의한다.
- [ ] preflight 사유 source-of-truth 결정 → 검증: 상태, 공유 여부, 배터리, 목적지 지원, 발신 가능 창, payload 초과 중 핵심 불가 사유 1개가 core에서 설명된다.
- [ ] 구현이 작으면 `SatelliteData`/`SatelliteManager` helper 추가 → 검증: helper의 가능 판정은 기존 `CanServe()` true와 모순되지 않는다.
- [ ] 구현이 크면 report-only matrix로 대체 → 검증: 7개 목적지 × 지금/+5/+15/+30분 후보 수, 추천 위성, 대표 불가 사유를 기록한다.
- [ ] 추천/예약 정합성 확인 → 검증: `FindBestSatellite()` 결과는 항상 예약 가능하고 `ReserveSelected()` 실패 문구가 실제 실패 조건과 일치한다.
- [ ] queue route detail 확인 → 검증: `GetQueuedMessageRouteDetail()`이 추천 위성 없음/있음을 명확히 반환하고 payload/가격/성공률이 비어 있지 않다.
- [ ] 실패 재시도 확인 → 검증: `SetSimulateFailureNextSend(true)` 후 Failed가 되고 `RetryFailedMission()`이 추천 위성/지연으로 재시작한다.
- [ ] 영수증/전송 로그 확인 → 검증: 성공 후 `SentMessages[0]`에 receipt id, 목적지, 위성명, payload, 가격 breakdown, 성공률, `transmissionLog`가 기록된다.
- [ ] payload 제한 확인 → 검증: 메시지+첨부 512KB 초과 시 예약/대기열 저장이 막힌다.
- [ ] 궤도/시간 모델 확인 → 검증: `SimulationManager.TimeScale` 1x 고정, `SatelliteOrbit` 이동, Fast Demo alignment가 현재 UI에서 충돌하지 않는다.
- [ ] line/packet lifecycle 확인 → 검증: Running/Transmitting 중 표시되고 Complete/Failed/Reset 뒤 `ConnectionLine.SetVisible(false)`와 packet hide가 유지된다.
- [ ] 자동 검증 보강 여부 결정 → 검증: queue/failure/receipt 최소 sanity를 `OrbitShareEditorAutomation`에 작게 추가하거나, 불안정하면 matrix로 대체한다.
- [ ] 최신 validation 근거 확보 → 검증: 실행 시 `orbitshare-validation.md`가 새 시각으로 생성되고 `Result: PASS`, Runtime Error/Exception 0을 기록한다.

## 리스크/확인 필요
- `orbitshare-validation.md`가 현재 삭제 상태다. Safe alternative: 과거 PASS 로그와 현재 리포트 부재를 분리하고, 최신 validation 재생성 전에는 최신 PASS로 쓰지 않는다.
- `SupportsTarget()`이 확장 목적지를 모두 지원 처리한다. Safe alternative: 큰 모델 변경 전 7개 목적지 matrix로 현 상태를 기록하고, 작은 helper부터 검토한다.
- product 문서가 7개 목적지/queue/failure/receipt 상태를 충분히 반영하지 못한다. Safe alternative: simulation lane은 구현 근거를 표로 넘기고 문서 수정은 spec lane에서 통합한다.
- `MissionManager` persistence가 validation 재현성에 영향을 줄 수 있다. Safe alternative: 검증 전 상태 초기화 절차 또는 별도 matrix로 기록한다.
- 자동 validation은 Fast Demo 중심이라 coverage/preflight 오류를 놓칠 수 있다. Safe alternative: 작은 core sanity section 또는 report-only matrix를 추가한다.
- 실제 Editor GUI E2E는 아직 별도 근거가 부족하다. Safe alternative: batch validation/player smoke는 보조 근거로만 쓴다.
- `SimulationManager.TimeScale`은 1x 고정이다. 과거 시간가속 체크를 되살리지 않는다.
- 실제 결제, 운영 DB, secret, 외부 업로드, 실제 TLE/live data, Windows/MP4/PPT/Submission은 요구 변경 전 실행 계획에 넣지 않는다.

## 병렬 에이전트 활용 메모
- 하위/병렬 에이전트는 사용하지 않음.
- 이유: 이번 산출물은 단일 lane note이고, 필요한 근거는 product/daylog/plan/code 정적 확인으로 충분했다. 파일 확인만 병렬 shell 호출로 처리하고 최종 판단은 단일 note로 통합했다.