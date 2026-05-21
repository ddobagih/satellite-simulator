# Computer Graphics Assignment 1 - Simulation/Mission Core lane note (2026-05-22)

## 최근 진행 근거
- 사용자 제공 AGENTS 지침, 2026-05-21 KST: 프로젝트 내부/상위 경로에 별도 `AGENTS.md` 없음. 사용자 제공 지침과 product 문서를 우선 적용.
- `product/vision.md`, `product/roadmap.md`, `product/backlog.md`, `product/decisions.md`, `product/done-criteria.md`, 2026-05-18 KST: 제품 기준은 `서울 고정 발신자 + Seoul/New York/London 목적지 + 발신 시각/메시지 용량 + 24개 후보 위성 + 1회성 단문 릴레이`.
- `README.md`, `WORK_LOG.md`, 2026-05-18 KST: `빠른 발신`, 수동 `위성 확정 → 메시지 발신`, 가격/성공 예측, relay lock/delivery 흐름이 구현 기능으로 정리됨.
- `orbitshare-validation.md`, 2026-05-20 20:56 KST: `Result: PASS`, 24개 위성, 기본 목적지 New York, Fast demo 완료, relay/delivery 100%, reset replay 2회, Runtime Error/Exception 0.
- `daylog/2026-05-21.md`, 2026-05-21 KST: 03:00 feature/verify scheduler, PM product audit, automation metrics 산출물은 현재 파일 근거 없음. Unity Editor GUI E2E는 아직 확인 필요.
- `plans/daily/2026-05-21.md`, 2026-05-20 KST: 다음 핵심 feature slice는 `선택 위성이 왜 발신 가능/불가능한지`를 설명하는 preflight 사유 표시.
- `plans/features`, `plans/verify/ready`, `plans/verify/reports`, `PM/status`, `PM/reports`: 확인 가능한 파일 없음. `plans/daily/2026-05-22.md`도 없음.
- `Assets/Scripts/SatelliteData.cs`, `SatelliteManager.cs`, `MissionManager.cs`, 2026-05-18 KST: `CanServe()` 조건은 `Available`, `Shared`, battery 35% 이상, 목적지 지원, `nextWindowMinutes <= requestedDelayMinutes`; 예약 실패 메시지는 현재 포괄적 한 문장.
- `Assets/Scripts/SatelliteOrbit.cs`, `SimulationManager.cs`, `GroundStation.cs`, `ConnectionLine.cs`: 궤도와 진행률은 `ScaledDeltaTime` 기반이며, 서울 게이트웨이 통신 가능 여부와 업링크 line 표시가 분리 구현됨.

## 내일 목표 후보
- 1순위: 기존 보강 반복이 아니라 product 목표를 앞으로 미는 feature slice로, 선택 위성 preflight 사유 모델을 core source-of-truth로 만든다.
- 2순위: preflight 판정이 `CanServe()`, `CanReserveSelected()`, `ReserveSelected()`, `FindBestSatellite()`, 후보 필터와 어긋나지 않게 정합성을 확인한다.
- 3순위: Seoul/New York/London × 지금/5분/15분/30분 matrix에서 가능한 후보 수와 대표 실패 사유를 기록한다.
- 4순위: 메시지 용량 32/128/512KB별 가격·성공 예측 갱신이 실제 결제/실제 통신값으로 오해되지 않는지 확인한다.
- 5순위: `빠른 발신`, 수동 `위성 확정 → 메시지 발신 → 10x`, Reset 후 재실행을 core 관점에서 리허설한다.
- 6순위: 실패가 재현될 때만 `SatelliteData`, `SatelliteManager`, `MissionManager`, `SimulationManager`, `SatelliteOrbit`, `GroundStation`, `ConnectionLine` 범위에서 최소 수정한다.

## 상세 체크리스트 초안
- [ ] preflight 사유 모델 정의 → 검증: `CanServe()`의 조건을 상태/공유 위성/battery/목적지/window 사유로 분해하고 예약 성공 조건과 1:1 대응되는지 확인.
- [ ] preflight helper 최소 구현 또는 report-only 대안 작성 → 검증: 가능한 위성은 기존 예약 결과와 같고, 불가능 위성은 상태 변경 없이 사유만 반환/기록.
- [ ] 필터 정합성 확인 → 검증: 전체/가능/빠른창/저가 필터 결과와 preflight 가능 판정이 다른 위성을 목록화.
- [ ] 목적지·발신 시각 matrix 작성 → 검증: Seoul/New York/London과 0/5/15/30분 조합별 가능한 후보 수와 대표 실패 사유 기록.
- [ ] estimate 갱신 확인 → 검증: 32/128/512KB에서 `EstimatedPriceCredits`, `SuccessProbabilityPercent`, `ResultMessage`가 선택 위성과 목적지에 맞게 갱신됨.
- [ ] `빠른 발신` core 흐름 확인 → 검증: relay lock 100%, delivery 100%, Mission Complete 도달 여부 기록.
- [ ] 수동 발신 흐름 확인 → 검증: 가능한 위성 선택 후 `위성 확정 → 메시지 발신 → 10x`로 Mission Complete 도달 여부 기록.
- [ ] 시간 배속 확인 → 검증: Pause에서 궤도/진행률이 멈추고 1x/5x/10x에서 `SatelliteOrbit`과 relay/delivery 진행률이 일관되게 가속됨.
- [ ] `GroundStation`/`ConnectionLine` lifecycle 확인 → 검증: 전송 가능 구간에서 line 표시, Complete/Reset 후 숨김 확인.
- [ ] Reset/재진입 확인 → 검증: 두 번째 `빠른 발신`과 Play Mode 재진입에서 stale state, 중복 이벤트, Runtime Error/Exception 없음.
- [ ] 변경 후 자동 검증 실행 또는 미실행 사유 기록 → 검증: 실행 시 `orbitshare-validation.md`가 PASS와 Runtime Error/Exception 0을 유지하는지 확인.

## 리스크/확인 필요
- 실제 Unity Editor GUI E2E 근거가 아직 없다. Safe alternative: batch validation, 1600x900 screenshot, display smoke는 보조 근거로만 쓰고 GUI 완료로 쓰지 않는다.
- preflight UI 표시는 UI lane과 겹칠 수 있다. Safe alternative: core helper와 matrix/report까지만 만들고 화면 배치는 UI lane에 넘긴다.
- `orbitshare-validation.md`의 `Screen: 640x480`은 발표 해상도 GUI 근거가 아니다.
- 실제 TLE, 실제 위성 가용성, 실제 결제/운영 DB/API는 현재 범위 밖이다. Safe alternative: 현재 시뮬레이션 값과 제한사항 문구 유지.
- Unity validation, Editor GUI, screenshot capture, Resource Hook 검증, build는 project lock/log 충돌 위험이 있어 병렬 실행하지 않는다.
- Windows build, MP4/PPT, `Submission/` 패키징, 새 Asset Store import, Git LFS는 제출 요구 변경 전 실행 계획에 넣지 않는다.
- 현재 작업트리에 tracked 코드/문서 변경과 untracked daylog/plan/product 문서가 섞여 있다. Safe alternative: 되돌리지 말고 제출/커밋 포함 범위만 정리한다.

## 병렬 에이전트 활용 메모
- 사용하지 않음.
- 이번 lane note 작성 범위는 product 문서, 최근 daylog/plan, validation report, core script 정적 확인으로 충분했고 최종 산출물이 단일 lane note라 하위 에이전트 위임 없이 통합했다.