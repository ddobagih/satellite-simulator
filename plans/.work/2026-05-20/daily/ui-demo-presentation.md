# Computer Graphics Assignment 1 - UI/Demo Flow/Presentation lane note (2026-05-21)

## 최근 진행 근거
- 사용자 제공 AGENTS 지침, 2026-05-20 KST: 프로젝트 내부/상위 경로에 별도 `AGENTS.md` 파일은 확인되지 않았고, 사용자 제공 지침을 기준으로 적용.
- `product/vision.md`, `product/roadmap.md`, 2026-05-18 KST: 현재 제품은 `서울 고정 발신자 + 목적지/발신 시각/용량 선택 + 24개 후보 위성 + 1회성 단문 릴레이`; M6 최종 리허설은 실제 Editor GUI 수동 조작이 남은 `todo`.
- `product/backlog.md`, 2026-05-18 KST: P1-001 실제 Unity Editor Play Mode 수동 리허설은 `todo`; P1-002 1600x900 화면 판독은 `done_current`.
- `product/done-criteria.md`, 2026-05-18 KST: 자동 검증, Visual Review, Display Smoke, GUI E2E를 서로 다른 검증 등급으로 구분해야 함. `Screen: 640x480` batch 결과를 발표 GUI E2E 완료로 쓰면 안 됨.
- `README.md`, 최종 점검 2026-05-18 KST: 최신 Demo Flow는 `빠른 발신`, 또는 목적지/시각/용량/위성 선택 후 `위성 확정 → 메시지 발신`.
- `daylog/2026-05-20.md`: `UIManager.cs` 중심으로 완료 상태 UI, 선택 상태, 결과 카드, 우측 메시지 패널이 보강됐고 `orbitshare-playmode.png` 1600x900 판독 완료. 단, 실제 마우스 조작 GUI E2E와 오디오 청취는 미확인.
- `orbitshare-validation.md`, 2026-05-20 확인: `Result: PASS`, 24개 위성, Fast Demo 완료, Reset replay 2회, IMGUI fallback disabled, PresentationDirector camera cuts, Runtime Error/Exception 0. 단, batch `Screen: 640x480`.
- `plans/daily/2026-05-21.md`, 2026-05-20 KST: 다음 신규 feature slice 후보는 `선택 위성이 왜 발신 가능/불가능한지`를 설명하는 preflight 사유 표시. UI lane은 이 결과를 상세 패널/버튼 상태/힌트 문구에 연결해야 함.
- `plans/features`, `plans/verify/ready`, `plans/verify/reports`: 디렉터리는 있으나 확인 가능한 파일 없음.
- `PM/`: 디렉터리 없음. 따라서 product audit, verify status, automation metrics 산출물은 이번 lane note 근거로 인용하지 않음.
- `UIManager.cs`: 선택 위성 상세, 후보 목록, `위성 확정` 버튼 enable/disable, 완료 결과 패널, `추천 위성으로 바로 발신` 버튼 구현 확인.
- `ImmediateUIManager.cs`: `showFallbackOverlay` 기본 false 구조 확인.
- `PresentationDirector.cs`: Reserved/Running/Observing/Transmitting/Complete phase별 camera cut 로직 확인.

## 내일 목표 후보
- 1순위: 신규 feature slice로 `선택 위성 발신 가능/불가능 사유`를 UI에 최소 표시한다. 목표는 새 기능을 크게 늘리는 것이 아니라, 발표자가 “왜 이 위성이 되는지/안 되는지”를 바로 설명할 수 있게 하는 것.
- 2순위: 실제 Unity Editor Play Mode에서 `빠른 발신`을 Mission Complete까지 수동 리허설한다.
- 3순위: 목적지/발신 시각/메시지 용량/위성 선택 후 `위성 확정 → 메시지 발신 → 10x` 흐름을 수동으로 확인한다.
- 4순위: Reset 후 두 번째 실행과 Play Mode 재진입에서 stale state, 이벤트 중복, 버튼 상태 오류가 없는지 확인한다.
- 5순위: 발표 해상도 또는 1600x900에서 후보 목록, 상세 패널, 드롭다운, timeline, result panel, 카메라 컷 가독성을 확인한다.
- 6순위: 기존 `orbitshare-demo.gif`/`DemoFrames`가 최신 한글 UI와 완료 패널을 대표하지 못하면 갱신 필요만 build-validation lane에 전달한다.

## 상세 체크리스트 초안
- [ ] preflight 사유 표시 위치 확정 → 검증: `UIManager`의 선택 위성 상세 또는 reservation hint 영역에 1~2줄을 추가해도 1600x900에서 겹침·잘림이 없는지 확인.
- [ ] 발신 가능/불가능 사유 문구 최소 표시 → 검증: 가능 위성은 `발신 가능`과 예상 요금/성공률, 불가능 위성은 `상태/목적지/window/battery` 중 핵심 사유가 짧게 보임.
- [ ] 버튼 상태와 사유 일치 확인 → 검증: `위성 확정` 버튼 비활성 조건과 화면 사유가 같은 조건을 가리키는지 확인.
- [ ] Editor Play Mode 진입 → 검증: `Assets/Scenes/MainScene.unity` Play 후 Runtime Error/Exception 없음 기록.
- [ ] `빠른 발신` 리허설 → 검증: 단일 클릭으로 relay lock 100%, delivery 100%, Mission Complete 도달 확인.
- [ ] 수동 Demo Flow 리허설 → 검증: 목적지/시각/용량/후보 위성 선택 후 `위성 확정 → 메시지 발신 → 10x` 완료 확인.
- [ ] Reset/재진입 QA → 검증: Mission Complete 후 `초기화`, 두 번째 실행, Play Mode 종료/재진입에서 stale result, 이벤트 중복, 버튼 비활성 오류가 없는지 확인.
- [ ] 발표 화면 QA → 검증: 후보 목록, 선택 위성 정보, 단문 메시지 발신 패널, timeline, 완료 패널, dropdown이 겹치거나 잘리지 않음.
- [ ] `PresentationDirector` 카메라 컷 QA → 검증: Reserved/Running/Observing/Transmitting/Complete 단계별 컷이 설명 대상을 가리지 않음.
- [ ] IMGUI fallback 중복 확인 → 검증: `showFallbackOverlay=false` 기준 Canvas UI만 표시되고 fallback overlay가 중복 표시되지 않음.
- [ ] 데모 자료 갱신 판단 → 검증: 기존 GIF/프레임이 최신 UI 근거로 부족하면 갱신 필요를 build-validation lane에 전달하고, 직접 MP4/PPT 생성 계획으로 확대하지 않음.

## 리스크/확인 필요
- 실제 GUI E2E는 아직 완료 근거가 없다. Safe alternative는 최신 validation PASS, 1600x900 screenshot, display smoke지만 수동 완료로 대체 표기하면 안 된다.
- preflight 사유 표시가 UI를 복잡하게 만들 수 있다. Safe alternative는 UI 변경 없이 사유 matrix를 문서/검증 리포트에 먼저 남기는 것.
- 실제 발표 해상도가 1600x900이 아닐 수 있다. Safe alternative는 1600x900 기준 QA를 남기고, 필요 시 1280x720 display smoke/screenshot을 별도 근거로 분리하는 것.
- Unity GUI, LicensingClient, DISPLAY 환경이 막힐 수 있다. Safe alternative는 batch validation과 screenshot capture를 실행하되 GUI 조작은 미완료로 남기는 것.
- Unity 자동 검증, GUI 리허설, 캡처, 빌드는 project lock/log 충돌 위험이 있어 단일 runner가 순차 실행해야 한다.
- `Fast Demo`, `Quick Relay`, `빠른 발신` 용어가 섞여 있다. 발표 기준은 UI의 `빠른 발신`, 자동화 내부 용어는 Fast Demo로 구분한다.
- optional asset/SFX missing은 실패가 아니라 fallback 유지 상태다. UI lane에서는 새 에셋 추가를 계획하지 않는다.

## 병렬 에이전트 활용 메모
- 하위/병렬 에이전트는 사용하지 않았음.
- 이유: 이번 lane note는 product 문서, 최근 daylog, 기존 통합 계획, UI 관련 코드의 정적 대조가 중심이라 별도 하위 에이전트로 나눌 만큼 독립적인 수정 범위가 없었음.
- 통합 결론: 2026-05-21 UI lane은 기존 catch-up 반복보다 `선택 위성 가능/불가능 사유 표시`라는 작은 feature slice를 우선하고, 그 다음 실제 Editor GUI 수동 리허설 근거를 확보하는 것이 맞음.