# Computer Graphics Assignment 1 - Simulation/Mission Core lane note (2026-05-21)

## 최근 진행 근거
- 사용자 제공 AGENTS 지침, 2026-05-20 KST: 프로젝트 내부 별도 `AGENTS.md`는 없음. 사용자 제공 지침과 product 문서를 우선 적용.
- `product/vision.md`, `product/roadmap.md`, `product/backlog.md`, `product/done-criteria.md`, `product/decisions.md`, 2026-05-18 KST: 제품 기준은 `서울 고정 발신자 + 목적지/발신 시각/메시지 용량 + 24개 후보 위성 + 1회성 단문 릴레이`.
- `README.md`, `WORK_LOG.md`, 2026-05-18 KST: `빠른 발신`, `위성 확정`, `메시지 발신`, 가격/성공 예측, relay lock/delivery 흐름이 구현 기능으로 정리됨.
- `orbitshare-validation.md`, 최신 확인 2026-05-20 KST: `Result: PASS`, 24개 위성, 기본 목적지 New York, Fast demo 완료, relay lock/delivery 100%, Reset replay 2회, Runtime Error/Exception 0.
- `daylog/2026-05-20.md`, 2026-05-20 KST: 1600x900 캡처와 자동 검증 PASS는 있으나 실제 Editor GUI에서 사람이 `빠른 발신`, 수동 발신, Reset/재진입을 조작한 근거는 아직 없음.
- `plans/daily/2026-05-21.md`, 2026-05-20 KST: 내일 신규 feature slice 후보는 “선택 위성이 왜 발신 가능/불가능한지”를 설명하는 reservation preflight.
- `plans/features`, `plans/verify`: 확인 가능한 파일 없음. `PM/status`, `PM/reports`, `docs`: 경로 없음. 따라서 03:00 scheduler, verify, product audit, automation metrics 신규 근거는 반영 불가.
- `Assets/Scripts/SatelliteData.cs`, `SatelliteManager.cs`, `MissionManager.cs`: `CanServe()` 조건은 `Available`, `Shared`, battery 35% 이상, 목적지 지원, `nextWindowMinutes <= requestedDelayMinutes`; 예약 실패 사유는 현재 한 문장으로만 표시됨.
- `Assets/Scripts/SatelliteOrbit.cs`, `SimulationManager.cs`, `GroundStation.cs`, `ConnectionLine.cs`: 궤도/진행률은 `ScaledDeltaTime` 기반이고, 서울 게이트웨이 통신각과 업링크 라인 표시가 분리 구현됨.

## 내일 목표 후보
- 1순위: 선택 위성 reservation preflight core slice를 설계해 “왜 되는지/안 되는지”를 상태, 소유 유형, 목적지 지원, 발신 가능 창, 배터리 기준으로 설명한다.
- 2순위: preflight 판정과 기존 `CanServe()`, `ReserveSelected()`, 전체/가능/빠른창/저가 필터가 서로 어긋나지 않는지 확인한다.
- 3순위: 목적지와 발신 시각 조합별로 최소 1개 이상 가능한 후보가 있는지 확인하고, 없으면 안내 사유를 남기는 흐름을 계획한다.
- 4순위: 실제 Unity Editor Play Mode에서 `빠른 발신`과 수동 `위성 확정 → 메시지 발신 → 10x`를 Mission Complete까지 리허설한다.
- 5순위: Pause/1x/5x/10x, `GroundStation`, `ConnectionLine`, Reset 후 재실행을 core 관점에서 확인한다.
- 6순위: 실패가 재현될 때만 `MissionManager`, `SatelliteManager`, `SatelliteData`, `SatelliteOrbit`, `SimulationManager`, `GroundStation`, `ConnectionLine` 범위에서 최소 수정한다.

## 상세 체크리스트 초안
- [ ] preflight 사유 모델 정의 → 검증: `SatelliteData.CanServe()`의 각 조건을 가능/불가능 사유 표로 분리하고 기존 예약 성공 조건과 1:1로 대응되는지 확인.
- [ ] preflight helper 최소 구현 또는 report-only 대안 작성 → 검증: 가능 후보는 기존 `ReserveSelected()`와 같은 결과를 내고, 불가능 후보는 상태 변경 없이 사유를 반환/기록.
- [ ] 필터 정합성 확인 → 검증: 전체/가능/빠른창/저가 필터 결과와 preflight 가능 판정이 다른 위성을 목록화하고 의도/버그를 분리.
- [ ] 목적지/발신 시각 matrix 확인 → 검증: Seoul/New York/London과 지금/5분/15분/30분 조합에서 가능한 후보 수와 실패 사유를 기록.
- [ ] 메시지 용량별 estimate 확인 → 검증: 32/128/512KB에서 `EstimatedPriceCredits`, `SuccessProbabilityPercent`, `ResultMessage`가 갱신되고 실제 결제/실제 통신값처럼 표현되지 않음.
- [ ] `빠른 발신` GUI 리허설 → 검증: relay lock 100%, Seoul gateway sync, delivery 100%, Mission Complete 도달 여부 기록.
- [ ] 수동 정상 흐름 리허설 → 검증: New York 기준 가능한 위성을 선택해 `위성 확정 → 메시지 발신 → 10x` 완료 여부 기록.
- [ ] 시간 배속 확인 → 검증: Pause에서 궤도/진행률이 멈추고 1x/5x/10x에서 `SatelliteOrbit`과 relay/delivery 진행이 일관되게 가속됨.
- [ ] `GroundStation`/`ConnectionLine` lifecycle 확인 → 검증: 전송 구간에서 서울 업링크 라인이 보이고 Complete/Reset 후 숨겨짐.
- [ ] Reset/재진입 확인 → 검증: Mission Complete 후 Reset, 두 번째 `빠른 발신`, Play Mode 재진입에서 stale state, 중복 이벤트, Runtime Error/Exception 없음.

## 리스크/확인 필요
- 실제 Editor GUI E2E 근거가 아직 없다. Safe alternative: batch validation, 1600x900 screenshot 판독, display smoke를 보조 근거로만 기록하고 GUI 완료로 쓰지 않는다.
- preflight UI 표시는 UI lane과 충돌할 수 있다. Safe alternative: core helper 또는 validation/report matrix까지만 만들고 화면 표시 위치는 UI lane에 넘긴다.
- `orbitshare-validation.md`의 `Screen: 640x480`은 발표 해상도 GUI 근거가 아니다.
- `run_orbitshare.sh` display smoke는 player 시작 근거이지 버튼 조작 완료 근거가 아니다.
- 실제 TLE, 실제 위성 가용성, 결제, 운영 DB/API는 현재 범위 밖이다. 시뮬레이션 값 유지.
- Unity 자동 검증, Editor GUI, 캡처, 빌드는 project lock/log 충돌 위험이 있어 병렬 실행하지 않는다.
- Windows 빌드, MP4/PPT, `Submission/` 패키징, 새 Asset Store import, Git LFS는 제출 요구 변경 전 자동 계획에서 실행하지 않는다.

## 병렬 에이전트 활용 메모
- 사용하지 않음.
- 이번 범위는 product 문서, 최근 daylog/plan, validation report, core script 정적 확인으로 충분했고, 최종 산출물은 merge 에이전트가 사용할 lane note라 단일 통합이 더 안전했다.