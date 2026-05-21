# Computer Graphics Assignment 1 - UI/Demo Flow/Presentation lane note (2026-05-20)

## 최근 진행 근거
- 사용자 제공 AGENTS 지침, 2026-05-19 KST: 별도 `AGENTS.md` 파일은 확인되지 않아 사용자 제공 지침을 기준으로 적용함.
- `plans/daily/2026-05-20.md`, 2026-05-19 KST 확인: 기존 파일 없음.
- `README.md`, 2026-05-18 KST: Runtime Canvas UI, `빠른 발신`, Demo Flow, 자동 검증 리포트가 구현 기능으로 정리됨.
- `daylog/2026-05-18.md`, 2026-05-18 KST: `UIManager.cs`에서 패널 가독성, 완료 결과 패널, `SignalRoutePreview`, `Relay/Delivery` 문구를 보완했고, `MissionManager.cs`에서 Quick Relay가 적절한 공유 위성을 선택하도록 보완함.
- `daylog/2026-05-18.md`, 2026-05-18 KST: 1600x900 `orbitshare-playmode.png`를 사람이 판독해 주요 UI와 완료 패널은 읽히지만, 실제 마우스 조작 GUI E2E는 아니라고 기록됨.
- `daylog/2026-05-19.md`, 2026-05-19 KST: 최신 자동 검증은 PASS이나 실제 Unity GUI에서 `빠른 발신`, 수동 `위성 확정 → 메시지 발신`, Reset, Play Mode 재진입 리허설은 아직 확인 필요로 남음.
- `orbitshare-validation.md`, 2026-05-18 KST: `Result: PASS`, Runtime Canvas reference 1600x900 PASS, IMGUI fallback disabled PASS, Reset replay 2 runs PASS, PresentationDirector camera cuts PASS, Runtime Error/Exception 0. 단, 실행 화면 크기는 `Screen: 640x480`이라 발표 해상도 GUI 근거로 과대해석하면 안 됨.
- `product/done-criteria.md`, 2026-05-18 KST: 자동화/캡처만으로 에셋/UI 품질 최종 PASS 처리 금지, 실제 Play 화면 판독과 GUI E2E를 구분해야 함.
- `product/backlog.md`, 2026-05-18 KST: 실제 Unity Editor Play Mode 수동 리허설은 P1 `todo`로 남아 있음.
- `Assets/Scripts/UIManager.cs`, 2026-05-19 KST 확인: 1600x900 기준 Canvas, 한글 UI, `위성 확정`, `메시지 발신`, `초기화`, `빠른 발신`, timeline/result panel 구현 확인.
- `Assets/Scripts/ImmediateUIManager.cs`, 2026-05-19 KST 확인: `showFallbackOverlay` 기본 false이며 fallback overlay는 꺼진 상태가 정상 기준임.
- `Assets/Scripts/PresentationDirector.cs`, 2026-05-19 KST 확인: Reserved/Running/Observing/Transmitting/Complete 단계별 자동 카메라 컷 로직 존재.

## 내일 목표 후보
- 실제 Unity Editor Play Mode에서 발표용 수동 리허설을 최우선으로 수행한다.
- `빠른 발신` 단일 클릭 흐름을 Mission Complete까지 확인한다.
- 수동 `위성 확정 → 메시지 발신 → 10x` 흐름을 Mission Complete까지 확인한다.
- Reset 후 재실행과 Play Mode 재진입 안정성을 확인한다.
- 발표 해상도 또는 최소 1600x900에서 UI 겹침/잘림/가독성을 다시 판독한다.
- `PresentationDirector` 카메라 컷이 발표 설명 흐름을 방해하지 않는지 확인한다.
- 문제가 실제 재현될 때만 `UIManager`, `ImmediateUIManager`, `PresentationDirector`, `CameraManager` 범위에서 최소 수정한다.
- 기존 `orbitshare-demo.gif`/`DemoFrames`가 최신 UI를 반영하는지 보고, 필요할 때만 갱신 대상으로 넘긴다.

## 상세 체크리스트 초안
- [ ] Unity Editor Play Mode 진입 → 검증: `Assets/Scenes/MainScene.unity`에서 Play 진입, Runtime Error/Exception 없음 기록
- [ ] `빠른 발신` 발표 리허설 → 검증: 단일 클릭 후 relay lock 100%, delivery 100%, Mission Complete 도달 확인
- [ ] 수동 Demo Flow 리허설 → 검증: 목적지/발신 시각/용량 선택 후 `위성 확정 → 메시지 발신 → 10x`로 완료되는지 확인
- [ ] Reset 후 재실행 → 검증: Mission Complete 후 `초기화`, 다시 `빠른 발신` 실행 시 버튼/패널/timeline/result summary 정상 확인
- [ ] Play Mode 재진입 → 검증: Play 종료 후 재진입해 이벤트 중복, stale state, 예외 없이 완료되는지 확인
- [ ] 발표 화면 QA → 검증: Relay Control, 선택 위성 정보, 단문 메시지 발신, 상태 패널, timeline, 결과 패널이 겹치거나 잘리지 않는지 확인
- [ ] 카메라 컷 QA → 검증: Reserved=Overview, Running/Transmitting=Satellite, Observing/ObservationComplete=Mission, Complete=Overview 흐름이 발표 가독성을 해치지 않는지 기록
- [ ] IMGUI fallback 확인 → 검증: `showFallbackOverlay=false` 기준 Canvas UI만 보이고 중복 overlay가 없는지 확인
- [ ] 용어 정합성 확인 → 검증: 문서/발표에서 자동화 내부 `Fast Demo`와 실제 버튼 `빠른 발신`을 구분해 설명 가능한지 확인
- [ ] 데모 자료 갱신 여부 판단 → 검증: 기존 GIF/프레임이 최신 UI와 완료 패널을 반영하지 않으면 capture 갱신 필요로 분류
- [ ] UI 문제 재현 시 최소 수정 → 검증: 수정 전후 동일 리허설 또는 화면 캡처로 문제 해소 확인

## 리스크/확인 필요
- 자동 검증은 최신 PASS지만 실제 수동 GUI E2E 완료 근거는 아직 없음.
- `orbitshare-validation.md`는 r1 항목 PASS를 포함하지만 `Screen: 640x480`이라 발표 해상도 품질 근거로는 부족함.
- 1600x900 캡처 판독은 완료됐지만 드롭다운 펼침, 실제 발표 모니터, 마우스 조작 흐름은 별도 확인 필요.
- display smoke와 자동 캡처를 수동 발표 리허설 완료로 쓰면 안 됨.
- UI 수정은 실제 겹침/잘림/카메라 문제가 재현될 때만 진행하는 것이 적절함.
- 제출 기준은 Unity 프로젝트 자체로 정리됐지만, 발표자가 별도 GIF/영상 자료를 요구받는 상황이 생기면 spec lane과 재조율 필요.

## 병렬 에이전트 활용 메모
- 하위/병렬 에이전트는 사용하지 않았음.
- 이유: 이번 lane note는 UI 관련 문서, 최근 daylog, 현재 코드, 검증 리포트의 정적 대조가 중심이라 별도 하위 문제로 나눌 만큼 크지 않았음.
- 통합 결론: 2026-05-20 UI lane은 새 기능 추가보다 실제 Editor 수동 리허설, 발표 해상도 가독성, Reset/재진입 안정성, 카메라 컷 QA에 집중하는 것이 맞음.