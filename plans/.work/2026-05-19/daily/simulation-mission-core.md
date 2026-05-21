# Computer Graphics Assignment 1 - Simulation/Mission Core lane note (2026-05-20)

## 최근 진행 근거
- 사용자 제공 AGENTS 지침, 2026-05-19 KST: 프로젝트 경로에서 별도 `AGENTS.md`는 확인되지 않아 프롬프트 지침을 기준으로 적용.
- `README.md`, 확인일 2026-05-19 KST: 서울 고정 발신자, 목적지/발신 시각/메시지 용량 선택, 24개 후보 위성, `위성 확정`/`메시지 발신`/`빠른 발신` 흐름이 최신 데모 기준으로 정리됨.
- `daylog/2026-05-18.md`, 2026-05-18 KST: `MissionManager`, `SatelliteManager`, `UIManager` 중심으로 24개 후보 위성, 발신 조건, 가격/성공 예측, 빠른 발신 보완이 반영됨.
- `daylog/2026-05-19.md`, 2026-05-19 KST: 최신 자동 검증 PASS와 1600x900 캡처 판독은 확인됐지만, 실제 마우스 조작 기반 Unity Editor GUI E2E는 아직 완료 근거가 없음.
- `orbitshare-validation.md`, 확인일 2026-05-19 KST: `Result: PASS`, 24개 위성, 기본 목적지 New York, relay lock 100%, delivery 100%, Reset replay 2회, PresentationDirector camera cuts, Runtime Error/Exception 0 확인.
- `product/backlog.md`, `product/done-criteria.md`, 확인일 2026-05-19 KST: core 자동 검증은 완료 상태지만 수동 `위성 확정 → 메시지 발신` GUI 리허설은 `todo`.
- `Assets/Scripts/MissionManager.cs`, 확인일 2026-05-19 KST: `Idle → Reserved → Running/Observing → ObservationComplete → Transmitting → Complete`, `RunFastDemo()`, `ResetMission()`, 가격/성공 예측, `ConnectionLine` 표시 제어 구현 확인.
- `Assets/Scripts/SatelliteOrbit.cs`, `SimulationManager.cs`, 확인일 2026-05-19 KST: 위성 궤도와 미션 진행이 `ScaledDeltaTime` 기반으로 시간 배속 영향을 받음.
- `Assets/Scripts/GroundStation.cs`, `ConnectionLine.cs`, 확인일 2026-05-19 KST: `Seoul Gateway` 통신각과 LineRenderer 표시/숨김/위치 갱신 구현 확인.
- `Assets/Scripts/SatelliteManager.cs`, `SatelliteData.cs`, 확인일 2026-05-19 KST: 24개 후보 데이터, 공유 위성 예약 조건, 목적지/발신 시각 기반 `CanServe()` 필터 구현 확인.
- `Assets/Scripts/OrbitShareBootstrap.cs`, 확인일 2026-05-19 KST: `MissionManager`에 `simulationManager`, `satelliteOrbit`, `groundStation`, `connectionLine`이 연결되고 기본 목적지 New York이 먼저 등록됨.
- `plans/daily/2026-05-20.md`, 확인일 2026-05-19 KST: 기존 파일 없음.
- `docs` 경로, 확인일 2026-05-19 KST: 별도 문서 경로 확인되지 않음.

## 내일 목표 후보
- 1순위: 실제 Unity Editor Play Mode에서 `빠른 발신` 단일 흐름을 Mission Complete까지 수동 리허설.
- 2순위: 수동 `위성 확정 → 메시지 발신` 흐름을 목적지/발신 시각/메시지 용량 선택부터 완료 패널까지 확인.
- 3순위: 후보 위성 조건 필터가 core 예약 조건과 맞는지 확인.
- 4순위: Pause/1x/5x/10x가 `SatelliteOrbit` 이동과 relay/delivery 진행률에 일관되게 반영되는지 확인.
- 5순위: `GroundStation`/`ConnectionLine`이 전송 구간에 보이고 Complete/Reset 후 숨겨지는지 화면 기준으로 확인.
- 6순위: Reset 후 두 번째 실행과 Play Mode 재진입에서 stale state, 중복 이벤트, Runtime Error/Exception이 없는지 확인.
- 7순위: 실제 실패가 재현될 때만 core 파일 범위에서 최소 수정.

## 상세 체크리스트 초안
- [ ] `빠른 발신` GUI 리허설 → 검증: Play Mode에서 `빠른 발신` 클릭 후 relay lock, Seoul gateway sync, delivery, Mission Complete까지 도달하는지 기록.
- [ ] 수동 정상 흐름 확인 → 검증: New York 기준 `위성 확정 → 메시지 발신 → 10x`로 완료까지 진행하고 `ResultMessage`, 진행률, 완료 요약이 끊기지 않는지 확인.
- [ ] 후보 위성 예약 조건 확인 → 검증: 전체/가능/빠른창/저가 필터와 `CanServe()` 조건이 맞고, 불가능한 위성 선택 시 Phase가 `Idle`에 머물며 안내 메시지가 표시되는지 확인.
- [ ] 목적지/발신 시각/용량 조합 확인 → 검증: Seoul/New York/London, 지금/5분/15분/30분, 32/128/512KB 선택 시 예상 가격과 성공 확률이 갱신되는지 확인.
- [ ] 시간 배속 확인 → 검증: Pause에서 궤도/진행률이 멈추고 1x/5x/10x에서 위성 이동과 relay/delivery 진행이 가속되는지 화면 기준으로 기록.
- [ ] `GroundStation`/`ConnectionLine` 표시 확인 → 검증: 전송 가능 구간에서 서울 업링크 라인이 보이고 Mission Complete 또는 Reset 후 숨겨지는지 확인.
- [ ] Reset 후 재실행 확인 → 검증: Mission Complete 후 Reset, 두 번째 `빠른 발신` 완료까지 상태/이벤트/위성 데이터가 꼬이지 않는지 확인.
- [ ] Play Mode 재진입 확인 → 검증: Play 종료 후 다시 Play에 진입해 기본 목적지 New York, 선택 위성, 진행률 초기값, `빠른 발신` 완료가 정상인지 확인.
- [ ] core 수정 필요 여부 판정 → 검증: 실패가 재현된 경우에만 `SatelliteOrbit`, `MissionManager`, `SimulationManager`, `GroundStation`, `ConnectionLine`, 필요 시 `OrbitShareBootstrap` 범위에서 최소 수정하고 자동 검증을 재실행.

## 리스크/확인 필요
- 최신 자동 검증은 PASS지만 실제 마우스 조작 기반 Unity Editor GUI E2E는 아직 완료 근거가 없다.
- `orbitshare-validation.md`의 `Screen: 640x480`은 실제 발표 해상도 검증이 아니라 Canvas reference 1600x900 설정 검증으로 봐야 한다.
- `run_orbitshare.sh` display smoke는 player 시작 근거이며, Editor 수동 조작 완료 근거는 아니다.
- `GroundStation`/`ConnectionLine` 표시/숨김과 시간 배속 체감은 자동 검증만으로 충분히 증명되지 않는다.
- 후보 위성 예약은 `Available`, `Shared`, 목적지 지원, `nextWindowMinutes <= requestedDelayMinutes` 조건을 모두 만족해야 하므로 발표자는 `가능` 필터나 `빠른 발신` 흐름을 우선 사용하는 것이 안전하다.
- 가격/성공 확률/발신 가능 창은 발표용 시뮬레이션 값이며 실제 결제, 실제 위성망, TLE 기반 궤도 계산이 아니다.
- 내일 core 코드 수정은 수동 리허설 또는 검증에서 실패가 재현될 때만 진행하는 것이 안전하다.

## 병렬 에이전트 활용 메모
- 하위/병렬 에이전트는 사용하지 않음.
- 이번 범위는 기존 문서, 최신 daylog, validation report, core 스크립트 정적 확인으로 충분했고 최종 계획 파일을 직접 수정하지 않는 lane note 작성 작업이었다.
- 파일 읽기는 병렬 shell 호출로 처리했고, 판단은 단일 통합으로 정리했다.
- 읽기 전용 계획 조사라 daylog는 작성하지 않았다.