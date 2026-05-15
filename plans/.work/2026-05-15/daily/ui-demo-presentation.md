# Computer Graphics Assignment 1 - UI/Demo Flow/Presentation lane note (2026-05-16)

## 최근 진행 근거
- 사용자 제공 AGENTS 지침, 2026-05-15 KST: 한국어 응답, 추정 금지, 실제 실행한 검증만 기록, 작업 영향이 있으면 daylog 기록 필요.
- `README.md`, 2026-05-15 확인: Demo Flow와 `Fast Demo`, presentation-safe IMGUI overlay, 자동 카메라 컷, 미션 타임라인 UI가 구현 기능으로 정리되어 있음.
- `WORK_LOG.md`, 작성일 2026-05-11: `Fast Demo` 버튼, 시간 배속, 카메라 모드, 미션 상태/결과 UI 구현 완료 기록. 남은 개선점으로 Unity GUI 최종 시연 화면 직접 확인, 카메라 구도/UI 겹침 미세 조정이 남아 있음.
- `daylog/2026-05-14.md`: `UIManager`, `SatelliteFocusController` 이벤트 구독 해제 정리, Unity 자동 검증 PASS, Linux build Success 기록. 발표/대본 관련 작업은 사용자 요청으로 제외했다고 기록됨.
- `daylog/2026-05-15.md`: 실제 Unity GUI 발표 화면에서 UI 겹침, 카메라 구도, Fast Demo 리허설은 직접 확인 근거가 아직 없다고 기록됨.
- `plans/catchup/2026-05-15.md`: UI Demo QA 책임 범위가 Unity GUI 기준 Fast Demo 리허설, UI 겹침, 카메라 컷, Play 재진입 안정성 확인으로 정리되어 있음.
- `plans/.work/2026-05-15/catchup/ui-demo-presentation.md`: Fast Demo 자동 검증과 기본 발표 UI 흐름은 완료 근거가 있으나, 실제 GUI 리허설과 발표 화면 검증은 미완료 후보로 분류됨.
- `orbitshare-validation.md`, 2026-05-15 확인: 자동 검증 `Result: PASS`, Fast Demo Mission Complete PASS, Observation/Transfer 100%, Runtime Error/Exception 0. 단, 화면 크기는 `640x480` 자동 검증 기준임.
- `Assets/Scripts/ImmediateUIManager.cs`, 2026-05-15 확인: IMGUI overlay가 1600x900 기준 좌표를 `Screen.width / 1600`, `Screen.height / 900`으로 스케일링함. 발표 해상도별 실제 가독성 확인 필요.
- `Assets/Scripts/UIManager.cs`, 2026-05-15 확인: Canvas UI에 `Fast Demo`, Reset, 시간 배속, 카메라 버튼과 미션 결과 패널이 있고 이벤트 구독 해제 로직이 있음.
- `Assets/Scripts/PresentationDirector.cs`, 2026-05-15 확인: 미션 단계 변경 시 Overview/Satellite/Mission 카메라 컷을 자동 적용함.
- `DemoFrames/`와 `orbitshare-demo.gif`, 2026-05-15 확인: 기존 데모 프레임은 2026-05-11, GIF는 2026-05-10 산출물이라 2026-05-14 이후 UI 안정성 변경을 반영했는지는 불확실함.
- `plans/daily/2026-05-16.md`: 현재 확인한 파일 목록에는 없음.

## 내일 목표 후보
- 실제 Unity GUI에서 Fast Demo 전체 리허설을 1순위로 수행하고, Mission Complete까지 UI 겹침/가독성/카메라 구도를 확인한다.
- Play 종료 후 재진입, Reset 후 Fast Demo 재실행까지 확인해 발표 중 재시도 안정성을 확보한다.
- `PresentationDirector` 자동 카메라 컷이 예약, 이동, 관측, 전송, 완료 단계에서 발표 흐름을 방해하지 않는지 확인한다.
- Canvas UI와 IMGUI overlay가 동시에 표시될 때 화면이 과밀하거나 겹치는지 확인하고, 문제가 재현될 때만 최소 범위로 조정한다.
- 기존 `DemoFrames/`와 `orbitshare-demo.gif`가 오래된 산출물이므로, GUI 리허설 통과 후 발표 백업 캡처 갱신 여부를 결정한다.
- UI/Demo 검증 결과를 daylog와 제출 문서 갱신 대상으로 넘길 수 있게 짧은 QA 결과 표로 남긴다.

## 상세 체크리스트 초안
- [ ] Unity Editor GUI에서 `Assets/Scenes/MainScene.unity` Play 실행 → 검증: 시작 화면에서 위성 목록, 상세, 예약, 상태, 시간, 카메라 UI가 읽히고 겹침이 없는지 육안 확인
- [ ] `SAT-S01` 기준 수동 Demo Flow 1회 수행 → 검증: 대상 선택, `Reserve`, `Start`, `10x` 흐름으로 관측, 전송, Mission Complete까지 도달
- [ ] `Fast Demo` 단일 클릭 흐름 확인 → 검증: 필요 시 예약/시작을 자동 보조하고 Mission Complete까지 도달, 런타임 Error/Exception 로그 없음
- [ ] Reset 후 `Fast Demo` 재실행 확인 → 검증: 중복 이벤트, 중복 패널 갱신, 버튼 비활성 상태 오류 없이 Mission Complete 재도달
- [ ] Play Mode 종료 후 재진입 확인 → 검증: 두 번째 Play에서 UI 이벤트가 중복 호출되지 않고 Fast Demo가 동일하게 동작
- [ ] `PresentationDirector` 카메라 컷 확인 → 검증: Reserved는 Overview, Running/Transmitting은 Satellite, Observing/ObservationComplete는 Mission, Complete는 Overview 흐름이 자연스러운지 기록
- [ ] 발표 해상도 후보에서 UI 확인 → 검증: 1600x900 또는 실제 발표 해상도에서 IMGUI overlay/Canvas UI 텍스트, 버튼, 결과 패널이 잘리지 않음
- [ ] 자동 캡처 산출물 갱신 필요 여부 결정 → 검증: 필요 시 `OrbitShareEditorAutomation.CaptureDemoFrames` 실행 결과 프레임이 Mission Complete 장면을 포함하고 최신 변경을 반영
- [ ] UI/Demo QA 결과 기록 → 검증: 확인한 해상도, 성공/실패, 발견한 겹침, 수정 필요 여부, 실행하지 못한 검증을 daylog/merge note에 명확히 남김

## 리스크/확인 필요
- 자동 검증은 PASS지만 `640x480` 배치 검증이라 발표 화면의 실제 가독성, UI 겹침, 카메라 구도 품질을 보장하지 않음.
- `ImmediateUIManager`는 1600x900 고정 기준 스케일링이라 16:9가 아닌 해상도 또는 작은 창에서 글자/버튼 겹침 가능성이 있음.
- Runtime Canvas UI와 IMGUI overlay가 모두 존재하므로 실제 화면에서 중복 UI가 발표 시 복잡해 보이는지 확인 필요.
- 기존 `orbitshare-demo.gif`와 `DemoFrames/`는 2026-05-14 안정성 변경 전 산출물로 보이며, 제출 백업으로 쓰려면 갱신 여부 확인 필요.
- 2026-05-14 기록의 “발표/대본 관련 작업 제외” 결정이 2026-05-16에도 유지되는지 확인 필요. 이 lane은 대본 작성이 아니라 발표 안정용 UI/Demo 리허설에 초점을 둔다.
- `plans/daily/2026-05-16.md`가 아직 없어 merge 에이전트가 lane note를 통합해 신규 계획표를 만들어야 함.
- UI 전용 SFX/HUD 에셋은 발표 품질에는 영향이 있지만 visual-asset-audio lane 책임이므로, UI QA 중 실제 방해 요소가 있을 때만 연계한다.

## 병렬 에이전트 활용 메모
- 사용하지 않았음.
- 이유: 이번 작업은 문서, 최근 daylog, 검증 리포트, UI/Presentation 관련 소수 코드 확인으로 범위가 제한되어 하위 에이전트 분리가 필요하지 않았음.
- 통합 결론: 내일 UI/Demo lane의 핵심은 새 기능 추가가 아니라 실제 Unity GUI Fast Demo 리허설, Play 재진입 안정성, 발표 해상도 UI 겹침 확인임.