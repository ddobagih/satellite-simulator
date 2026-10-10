# Computer Graphics Assignment 1 - UI/Demo Flow/Presentation weekly lane note (2026-W22)

## 이번 주 목표 후보

- `시연 준비`와 기존 `시연 전송`을 분리한다. `시연 준비`는 샘플 메시지/첨부/목적지/추천 위성/확인 단계까지만 채우고 자동 발신하지 않는다.
- 발표자가 따라갈 2~3분 수동 흐름을 고정한다: `시간 → 메시지/첨부 → 목적지 → 위성 → 확인 → 보내기 시작`.
- 기존 단일 클릭 자동 흐름은 보존한다. 현재 `StartDemoModeNow()`는 `StartSelectedMission()`까지 호출하므로, 기존 기능명은 `시연 전송`으로 두고 수동 리허설과 구분한다.
- 패널/런처/토스트/확인 팝업/기록/대기 목록이 첫 발표 CTA를 가리지 않도록 1280x720 및 1600x900 기준으로 확인한다.
- `PresentationDirector` phase camera cut이 작성 UI와 설명 대상을 가리지 않는지 확인한다.
- `orbitshare-validation.md`는 현재 파일 없음 상태이므로, build-validation lane과 연결해 최신 validation 재생성 전까지 최신 PASS로 쓰지 않는다.
- product 문서는 일부 3개 목적지 기준이고 최근 daylog/코드는 7개 목적지 확장 기록이 있으므로, UI 발표 문구는 README/daylog 최신 흐름을 근거로 하되 문서 불일치는 spec lane에 넘긴다.

## 날짜별/단계별 체크리스트

- [ ] 2026-05-25 월: W22 기준선 확정 → 검증: `plans/weekly/2026-W22.md`, `plans/features`, `plans/verify/ready`, `PM/status`, product audit/automation metrics 산출물 부재를 기록하고 없는 근거는 인용하지 않는다.
- [ ] 2026-05-25 월: UI 상태 정적 확인 → 검증: `UIManager.cs`의 `StartDemoModeNow()` 자동 발신 경로, `ImmediateUIManager.showFallbackOverlay=false`, `PresentationDirector` phase cut을 확인한다.
- [ ] 2026-05-26 화: `시연 준비` slice 설계/최소 구현 결정 → 검증: 준비 동작이 `MissionPhase.Idle`, `currentFlowStep=5`, 샘플 입력/추천 위성 설정까지만 수행한다고 명시된다.
- [ ] 2026-05-26 화: 구현이 커지면 run card로 대체 → 검증: 새 패널 추가 없이 기존 작성/설정/런처 문구로 발표 순서가 설명 가능하다.
- [ ] 2026-05-27 수: 수동 리허설 1차 → 검증: `시연 준비 → 보내기 시작`, 단계형 수동 flow, 기존 `시연 전송`을 각각 성공/실패/미실행으로 분리 기록한다.
- [ ] 2026-05-27 수: 실패/대기/기록 흐름 확인 → 검증: 실패 시뮬레이션 ON 표시, Reset 후 OFF 여부, 대기 메시지 추가/전송 기록/기록 재사용이 필수 CTA를 가리지 않는다.
- [ ] 2026-05-28 목: 발표 해상도 QA → 검증: 1280x720 player와 1600x900 캡처 또는 수동 판독에서 주요 버튼/텍스트/패널 겹침과 잘림을 확인한다.
- [ ] 2026-05-28 목: 패널 복구 QA → 검증: 작성/위성/상태/진행/기록/대기/통계/템플릿/설정 패널을 닫거나 드래그/리사이즈한 뒤 `패널 정리`로 복구된다.
- [ ] 2026-05-29 금: validation handoff → 검증: UI 변경 후 build-validation lane에서 `ValidateMissionFlow` 재생성 또는 미실행 사유를 받는다.
- [ ] 2026-05-29 금: 발표 문구 handoff → 검증: `시연 준비`, `시연 전송`, demo attachment, 시뮬레이션 요금/성공률 문구가 README/WORK_LOG/Q&A와 충돌하지 않게 spec lane에 전달한다.
- [ ] 2026-05-30 토: 재현된 문제만 최소 수정 → 검증: UI 겹침, 자동 발신 혼동, 카메라 컷 방해처럼 실제 확인된 문제만 `UIManager`, `ImmediateUIManager`, `PresentationDirector`, 필요 시 `CameraManager` 범위에서 고친다.
- [ ] 2026-05-31 일: 발표 freeze → 검증: 새 기능 추가를 멈추고 run card, 검증 근거, 미확인 항목, dropped 항목만 최종 정리한다.

## 검증 계획

- 정적 검증: `UIManager.cs`에서 `시연 준비`가 `StartSelectedMission()`을 호출하지 않는지, 기존 `시연 전송`은 기존 자동 완료 목적을 유지하는지 확인한다.
- 자동 검증: build-validation lane에서 `OrbitShareEditorAutomation.ValidateMissionFlow`를 순차 실행해 `orbitshare-validation.md` 재생성, Runtime Error/Exception 0, Fast Demo/Reset 결과를 확인한다.
- 수동 GUI 검증: 실제 Editor GUI 또는 player에서 `시간 → 메시지/첨부 → 목적지 → 위성 → 확인 → 보내기 시작`을 1회 이상 끝까지 조작한다.
- 빠른 시연 검증: 기존 `시연 전송` 단일 조작이 Mission Complete까지 도달하는지 확인한다.
- 화면 검증: 1280x720과 1600x900에서 작성 패널, 위성 후보, 상태, 진행, 결과, 기록, 대기, 설정, 토스트/확인 팝업이 겹치거나 잘리지 않는지 판독한다.
- 발표 안정성 검증: Reset/새 메시지/기록 재사용/대기 메시지 추가 후 stale state, 의도치 않은 자동 대기 시작, 중복 이벤트가 없는지 확인한다.
- 카메라 검증: Reserved/Running/Observing/Transmitting/Complete 전환에서 카메라 컷이 UI 또는 설명 대상을 가리지 않는지 확인한다.
- 미실행 검증은 완료로 쓰지 않고 `미실행`, `환경 차단`, `대체 근거` 중 하나로 기록한다.

## 리스크/확인 필요

- `plans/weekly/2026-W22.md`는 확인되지 않았다. 이 note는 weekly 통합 파일을 직접 수정하지 않는 lane note다.
- `plans/features`, `plans/verify/ready`, `PM/status`, `PM/reports`, product audit, automation metrics 산출물이 없어 product/daylog/README/코드 기준으로 계획한다.
- `orbitshare-validation.md`가 현재 삭제 상태라 최신 자동 검증 PASS로 단정하면 안 된다.
- `StartDemoModeNow()`는 현재 자동 발신까지 수행하므로 수동 발표 흐름과 섞일 수 있다.
- `AutoStartQueuedMessages`는 저장 상태라 발표 중 의도치 않은 다음 발신이 시작될 수 있다. Safe alternative: 준비 모드에서 OFF 확인 또는 발신 전 상태 문구로 노출한다.
- 최신 캡처/display smoke는 GUI E2E가 아니다. 실제 버튼 클릭 리허설을 별도로 기록해야 한다.
- headless `Camera.Render()` capture crash 이력이 있다. Safe alternative: player/manual 판독으로 대체하고 앱 런타임 실패로 단정하지 않는다.
- UI 기능이 많아 산만해질 수 있다. 새 패널을 늘리기보다 기존 확인 단계와 run card를 우선한다.
- Windows/PPT/MP4/`Submission`/새 Asset Store/SFX/Git LFS/결제/DB/secret/외부 업로드는 요구 변경 전 실행 대상으로 잡지 않는다.

## 병렬 에이전트 활용 메모

- 이번 lane note 작성에는 하위/병렬 에이전트를 사용하지 않았다.
- 이유: 최종 파일을 수정하지 않는 단일 lane 계획 작업이고, 문서/코드/로그 확인은 병렬 shell 조회로 충분했다.
- 이번 주 실제 실행에서는 `UI run card 작성`, `패널/문구 정적 QA`, `README/product 불일치 조사`, `캡처 판독 메모`는 병렬화 가능하다.
- `UIManager.cs` 수정, Unity validation, capture, build, GUI 리허설은 Unity project lock과 같은 파일 충돌 위험이 있어 순차 처리한다.