# Computer Graphics Assignment 1 - UI/Demo Flow/Presentation lane note (2026-05-17)

## 최근 진행 근거
- 사용자 제공 AGENTS 지침, 2026-05-16 KST: 추정 금지, 실제 실행한 검증만 기록, 작업 영향이 있으면 daylog 기록 필요.
- `README.md`, 2026-05-16 확인: Demo Flow는 `Queue` → `Relay` → `10x`, 빠른 실행 UI는 `Quick Relay`로 정리됨.
- `WORK_LOG.md`, 최종 점검 2026-05-16: Fast Demo, 시간 배속, 카메라 모드, 미션 상태/결과 UI 구현 완료 기록. 제출 직전 Play Mode 화면 재확인 필요가 남아 있음.
- `daylog/2026-05-15.md`: UI 문구/폰트/패널/Canvas-IMGUI 중복 표시 문제를 조정했고, 1600x900 스크린샷과 자동 검증 PASS 기록이 있음.
- `daylog/2026-05-16.md`: Reset 2회차, Canvas 1600x900, IMGUI fallback 기본 비활성, `PresentationDirector` 카메라 컷 검사가 자동 검증 코드에 추가됨. 단, 새 Unity 검증 재실행은 LicensingClient IPC 문제로 실패.
- `plans/daily/2026-05-16.md`: UI lane 미완료 핵심은 발표 해상도 Play Mode, 수동 Demo Flow, Fast Demo/Quick Relay, Reset 재실행, Play 재진입, 카메라 컷 확인.
- `plans/daily/2026-05-17.md`, 2026-05-16 확인: 기존 파일 없음.
- `Assets/Scripts/UIManager.cs`, 2026-05-16 확인: Canvas 기준 해상도는 1600x900이고 `Queue`, `Relay`, `Reset`, `Quick Relay`, 시간/카메라 버튼이 있음.
- `Assets/Scripts/ImmediateUIManager.cs`, 2026-05-16 확인: IMGUI fallback은 `showFallbackOverlay` 기본 false.
- `Assets/Scripts/PresentationDirector.cs`, 2026-05-16 확인: Reserved=Overview, Running/Transmitting=Satellite, Observing/ObservationComplete=Mission, Complete=Overview 컷 처리.
- `orbitshare-validation.md`, 2026-05-16 확인: 기존 리포트는 PASS지만 화면은 640x480이고 r1 추가 검사는 아직 반영되지 않은 이전 근거임.

## 내일 목표 후보
- Unity GUI 실행 가능한 환경에서 발표 해상도 또는 1600x900 기준 Play Mode 리허설을 1순위로 수행한다.
- `Quick Relay` 단일 클릭과 수동 `Queue` → `Relay` → `10x` 흐름을 각각 Mission Complete까지 확인한다.
- Reset 후 재실행과 Play Mode 종료 후 재진입을 확인해 발표 중 재시도 안정성을 확보한다.
- r1에서 보강된 자동 검증을 LicensingClient IPC 가능한 환경에서 재실행해 최신 `orbitshare-validation.md`를 갱신한다.
- `PresentationDirector` 카메라 컷이 발표 흐름을 방해하지 않는지 단계별로 기록한다.
- 실제 UI 겹침/잘림이 재현될 때만 `UIManager`, `ImmediateUIManager`, `PresentationDirector` 범위에서 최소 수정한다.
- `Fast Demo`라는 내부/문서 용어와 실제 버튼명 `Quick Relay`가 발표 중 혼동되지 않는지 확인한다.

## 상세 체크리스트 초안
- [ ] Unity GUI 실행 환경 확보 → 검증: LicensingClient IPC, DISPLAY 또는 GUI 세션에서 `Assets/Scenes/MainScene.unity` Play 진입 가능 여부 기록
- [ ] 발표 해상도 Play Mode 확인 → 검증: 위성 목록, 상세 패널, 요청 패널, 상태 패널, 시간/카메라 버튼, 결과 패널이 잘리지 않음
- [ ] 수동 Demo Flow 1회 수행 → 검증: `Queue` → `Relay` → `10x`로 relay lock, delivery, Mission Complete까지 도달
- [ ] `Quick Relay` 단일 클릭 수행 → 검증: 추가 조작 없이 Mission Complete 도달, Runtime Error/Exception 0
- [ ] Reset 후 `Quick Relay` 재실행 → 검증: 두 번째 Mission Complete까지 버튼 상태, 패널 갱신, 이벤트 중복 이상 없음
- [ ] Play Mode 종료 후 재진입 → 검증: 두 번째 Play에서도 UI 이벤트 중복 호출 없이 동일 흐름 완료
- [ ] `PresentationDirector` 카메라 컷 확인 → 검증: Reserved/Running/Observing/Transmitting/Complete 단계별 카메라 모드와 발표 가독성 기록
- [ ] r1 자동 검증 재실행 → 검증: `Result: PASS`, Reset replay 2 runs, Canvas 1600x900, IMGUI fallback disabled, camera cuts PASS
- [ ] 데모 프레임/GIF 갱신 여부 결정 → 검증: 제출 형식상 필요할 때만 최신 Mission Complete 장면 포함 여부 확인
- [ ] UI/Demo QA 결과 기록 → 검증: 실행한 항목, 미실행 사유, 발견한 UI 문제, 수정 여부를 daylog/merge note에 분리 기록

## 리스크/확인 필요
- 현재 환경 문제로 새 Unity 자동 검증과 GUI 리허설이 아직 완료되지 않았음.
- 기존 PASS 리포트는 r1 보강 전 640x480 기준이라 발표 UI 품질 근거로는 부족함.
- 실제 발표 해상도가 1600x900이 아닐 경우 Canvas/IMGUI 배치 확인이 다시 필요함.
- 문서/계획의 `Fast Demo`와 실제 버튼명 `Quick Relay`가 혼용되어 발표자 혼동 가능성이 있음.
- `orbitshare-demo.gif`, `DemoFrames/`가 최신 UI 변경을 반영하는지는 불확실함.
- PPT/PDF 또는 MP4/MOV 필수 여부는 submission lane과 조율 필요.

## 병렬 에이전트 활용 메모
- 사용하지 않았음.
- 이유: 이번 lane note는 문서, 최근 daylog, 기존 계획, UI 관련 소수 코드 확인으로 범위가 제한되어 하위 에이전트 분리가 필요하지 않았음.
- 통합 결론: 2026-05-17 UI lane 핵심은 새 기능 추가가 아니라 실제 GUI 리허설, 재시도 안정성, 발표 해상도 가독성 확인임.