# Computer Graphics Assignment 1 - UI/Demo Flow/Presentation lane note (2026-05-22)

## 최근 진행 근거
- 2026-05-21 확인: 프로젝트 내부/상위 경로에 별도 `AGENTS.md` 파일 없음. 사용자 제공 AGENTS 지침을 기준으로 적용.
- `product/vision.md`, `product/roadmap.md`, `product/backlog.md`, `product/done-criteria.md`, `product/decisions.md`(2026-05-18): 현재 제품은 `서울 고정 발신자 + 3개 목적지 + 발신 시각/용량 + 24개 후보 위성 + 1회성 단문 릴레이`; 남은 핵심은 실제 Editor GUI 수동 리허설.
- `README.md`, `WORK_LOG.md`(최종 점검 2026-05-18): Demo Flow는 `빠른 발신` 또는 `위성 확정 → 메시지 발신`; 자동 검증과 display smoke는 기록되어 있으나 수동 GUI E2E는 별도 근거 필요.
- `daylog/2026-05-20.md`: `UIManager.cs` 중심으로 완료 화면, 결과 카드, 전송 요약, `새 메시지 보내기` CTA가 보강됐고 1600x900 `orbitshare-playmode.png` 판독 완료.
- `daylog/2026-05-21.md`: 신규 코드 변경 근거 없음. 다음 핵심은 `선택 위성이 왜 발신 가능/불가능한지`를 설명하는 preflight 사유 표시와 실제 Unity Editor GUI 수동 리허설.
- `orbitshare-validation.md`(2026-05-20 확인): `Result: PASS`, 24개 위성, Fast Demo 2회 완료, Reset replay, IMGUI fallback disabled, PresentationDirector camera cuts, Runtime Error/Exception 0. 단 `Screen: 640x480`이라 발표 해상도 GUI E2E 근거는 아님.
- `Assets/Scripts/UIManager.cs`: 후보 목록 필터, 선택 위성 상세, 추천 이유, 버튼 활성/비활성, 완료 결과 패널 구현 확인. 현재 불가능 사유는 대체로 `현재 조건 불일치` 수준이라 세부 설명이 부족함.
- `Assets/Scripts/SatelliteData.cs`, `SatelliteManager.cs`: 발신 가능 조건은 `Available`, `Shared`, 배터리 35% 이상, 목적지 지원, 발신 window 충족으로 확인.
- `ImmediateUIManager.cs`: `showFallbackOverlay` 기본 false 구조 확인.
- `PresentationDirector.cs`: Reserved/Running/Observing/Transmitting/Complete 단계별 카메라 컷 로직 확인.
- 2026-05-21 확인: `plans/features`, `plans/verify/ready`, `plans/verify/reports` 파일 0개, `PM/` 및 `docs/` 산출물 없음. product audit, verify status, automation metrics 근거는 인용 불가.
- 2026-05-21 확인: 기존 `plans/daily/2026-05-22.md` 없음.

## 내일 목표 후보
- 1순위: 신규 feature slice로 `선택 위성 발신 가능/불가능 사유`를 UI에 1~2줄로 표시한다. 발표자가 “왜 이 위성이 되는지/안 되는지”를 즉시 설명할 수 있게 하는 것이 목표.
- 2순위: `위성 확정` 버튼 비활성 조건, 후보 row badge, 상세 패널 사유가 같은 source-of-truth를 가리키도록 정합성을 확인한다.
- 3순위: 실제 Unity Editor Play Mode에서 `빠른 발신`을 Mission Complete까지 수동 리허설한다.
- 4순위: 목적지/발신 시각/용량/위성 선택 후 `위성 확정 → 메시지 발신 → 10x` 수동 흐름을 확인한다.
- 5순위: Reset 후 두 번째 실행과 Play Mode 재진입에서 stale result, 이벤트 중복, 버튼 상태 오류가 없는지 확인한다.
- 6순위: 1600x900 또는 실제 발표 해상도에서 후보 목록, 상세 패널, dropdown, timeline, 결과 패널, 카메라 컷 가독성을 확인한다.
- 7순위: `Fast Demo`, `빠른 발신`, 현재 버튼 문구 `추천 위성으로 바로 발신`의 발표 용어를 맞출지 결정한다.

## 상세 체크리스트 초안
- [ ] preflight 사유 문구 매핑 확정 → 검증: `CanServe()` 조건을 `상태`, `공유 중계 여부`, `배터리`, `목적지 지원`, `발신 가능 창`으로 분해하고 UI 문구 후보를 5개 이하로 정리.
- [ ] 표시 위치 확정 → 검증: `UIManager`의 선택 위성 상세 또는 reservation hint 영역에 1~2줄을 추가해도 1600x900에서 겹침·잘림이 없음.
- [ ] preflight UI 최소 표시 구현 → 검증: 가능 위성은 `발신 가능`, 불가능 위성은 핵심 사유 1개가 보이고 기존 가격/성공률 정보가 가려지지 않음.
- [ ] 버튼 상태 정합성 확인 → 검증: `위성 확정` 비활성 조건과 화면 사유가 같은 조건을 가리킴.
- [ ] 후보 목록 badge 정합성 확인 → 검증: `발신 가능`, `대기`, `추천`, `선택` 표시가 상세 패널 사유와 충돌하지 않음.
- [ ] Editor Play Mode 진입 → 검증: `Assets/Scenes/MainScene.unity` Play 후 Runtime Error/Exception 없음 기록.
- [ ] `빠른 발신` 리허설 → 검증: 단일 클릭으로 relay lock 100%, delivery 100%, Mission Complete 도달 확인.
- [ ] 수동 Demo Flow 리허설 → 검증: 목적지/시각/용량/후보 위성 선택 후 `위성 확정 → 메시지 발신 → 10x` 완료 확인.
- [ ] Reset/재진입 QA → 검증: `새 메시지 보내기` 또는 `초기화`, 두 번째 실행, Play Mode 종료/재진입에서 stale result와 이벤트 중복 없음.
- [ ] 발표 화면 QA → 검증: 후보 목록, 선택 위성 정보, 단문 메시지 발신 패널, timeline, 완료 패널, dropdown이 겹치거나 잘리지 않음.
- [ ] `PresentationDirector` 카메라 컷 QA → 검증: 단계별 컷이 설명 대상과 UI를 가리지 않음.
- [ ] IMGUI fallback 중복 확인 → 검증: `showFallbackOverlay=false` 기준 Canvas UI만 표시됨.
- [ ] 데모 자료 갱신 판단 → 검증: 기존 GIF/프레임이 최신 UI를 대표하지 못하면 build-validation lane에 갱신 필요만 전달하고, MP4/PPT 생성으로 확대하지 않음.

## 리스크/확인 필요
- 실제 GUI E2E 근거는 아직 없다. Safe alternative는 최신 batch validation, 1600x900 screenshot, display smoke를 보조 근거로 남기는 것이며 수동 완료로 쓰면 안 된다.
- preflight 사유 표시가 UI를 복잡하게 만들 수 있다. Safe alternative는 UI 변경 없이 사유 matrix/report-only 결과를 먼저 남기는 것.
- core helper가 확정되지 않으면 UI에서 로직을 중복 구현할 위험이 있다. Safe alternative는 `SatelliteData.CanServe()` 조건을 그대로 읽는 얕은 표시로 제한한다.
- 발표 해상도가 1600x900이 아닐 수 있다. Safe alternative는 1600x900 기준 QA와 실제 발표 해상도 미확인을 분리 기록한다.
- Unity GUI, LicensingClient, DISPLAY 환경이 막힐 수 있다. Safe alternative는 batch validation과 캡처를 실행하되 GUI 조작은 todo/blocked로 유지한다.
- Unity 자동 검증, GUI 리허설, 캡처, 빌드는 project lock/log 충돌 위험이 있어 단일 runner가 순차 실행해야 한다.
- optional asset/SFX missing은 실패가 아니라 fallback 유지 상태다. UI lane에서는 새 에셋 추가를 계획하지 않는다.
- Windows build, MP4/PPT, `Submission/` 패키징은 현재 product 결정상 dropped이므로 요구 변경 전 직접 실행 계획에 넣지 않는다.

## 병렬 에이전트 활용 메모
- 하위/병렬 에이전트는 사용하지 않았음.
- 이유: 이번 작업은 read-only 문서/코드 대조와 단일 lane note 작성이 중심이라 독립 하위 작업으로 나눌 만큼 범위가 크지 않았음.
- 파일 검색과 읽기는 병렬 shell 호출로 처리했음.
- 내일 실제 실행에서 병렬화한다면 `preflight 문구 매핑 조사`와 `GUI 리허설 체크리스트 정리`는 read-only로 나눌 수 있다. 단 `UIManager.cs` 수정과 Unity 실행 검증은 순차로 진행해야 한다.