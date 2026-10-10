# Computer Graphics Assignment 1 - UI/Demo Flow/Presentation lane note (2026-05-27)

## 최근 진행 근거
- 2026-05-25~26 `product/vision.md`, `product/roadmap.md`, `product/backlog.md`, `product/done-criteria.md`, `product/decisions.md`: 제품 기준은 서울 고정 발신자, 7개 목적지, 24개 후보 위성, 발신 시각/메시지/첨부 선택, 1회성 통신 릴레이 시뮬레이터다.
- 2026-05-26 `README.md`: 발표 흐름은 `시간 선택 → 메시지/첨부 → 목적지 선택 → 위성 추천/선택 → 전송/메시지 목록/기록`으로 정리돼 있다.
- 2026-05-26 `daylog/2026-05-26.md`: 발신 흐름을 `사용자(서울) → 인공위성 → 목적지` 기준으로 UI/상태/결과/예약/타임라인 문구에 맞췄고, route/packet 방향 보강이 확인됐다.
- 2026-05-26 `orbitshare-validation.md`: `Result: PASS`, Runtime Error/Exception `PASS (0)`, Fast Demo 2회 완료, 7개 목적지와 목적지별 추천 스냅샷이 기록돼 있다.
- 2026-05-26 `orbitshare-menu-captures/report.md`: 하단 8개 패널 열기/재클릭 닫기, 작성 5단계, 직접 설정 달력 캡처가 non-empty PASS이며 active-panel intersection은 없음으로 기록됐다.
- 2026-05-26 코드 확인 `Assets/Scripts/UIManager.cs`: 현재 `StartDemoModeNow()`는 샘플 입력과 추천 위성 선택 뒤 `StartSelectedMission()`까지 호출하므로 `시연`은 자동 발신이다. 자동 발신 없는 `시연 준비`는 아직 별도 구현 근거가 없다.
- 2026-05-26 코드 확인 `Assets/Scripts/ImmediateUIManager.cs`: IMGUI fallback은 `showFallbackOverlay=false`일 때 표시되지 않는다. fallback 내부 문구는 영어/legacy 성격이 남아 있어 기본 발표 UI 근거로 삼지 않는 편이 안전하다.
- 2026-05-26 코드 확인 `Assets/Scripts/PresentationDirector.cs`: phase별 camera cut은 Reserved=Overview, Running/Transmitting=Satellite, Observing/ObservationComplete=Mission, Complete=Overview로 유지된다.
- 2026-05-26 확인: `plans/features`, `plans/verify/ready`, `plans/verify/reports`는 비어 있고 `PM/status`, `PM/reports`, product audit, automation metrics 산출물은 확인되지 않았다. `plans/daily/2026-05-27.md`도 아직 없다.
- 2026-05-26 `WORK_LOG.md`: 최종 점검일이 2026-05-25로 남아 있어, 2026-05-26 route-clarity 검증/캡처를 최신 제출 근거로 쓰려면 문서 동기화가 필요하다.

## 내일 목표 후보
- 1순위: `시연 준비`와 기존 `시연 전송` 분리. 준비는 샘플 메시지/첨부/목적지/추천 위성/확인 단계까지만 채우고 자동 발신하지 않는다.
- 2순위: 확인 단계의 발표 안전 상태 보강. 실패 테스트, 자동 대기 전송, payload 초과, 선택 위성 가능 여부, 추천 위성 정보를 발신 직전에 한눈에 보이게 한다.
- 3순위: 기존 Fast Demo/`시연 전송` 유지. 자동 검증이 의존하는 단일 조작 Mission Complete 흐름은 깨지지 않아야 한다.
- 4순위: 2~3분 발표 run card 고정. `시연 준비 → 전송 시작`, 수동 5단계 작성, 기존 자동 시연, 실패/재시도, 대기/기록 확인을 분리한다.
- 5순위: 패널/카메라 컷 발표 QA. 1600x900 캡처와 player 판독 기준으로 작성/위성/상태/진행/기록/대기/설정 패널, 토스트/확인 팝업, phase camera cut 겹침을 확인한다.

## 상세 체크리스트 초안
- [ ] `시연 준비` 동작 정의 → 검증: `MissionPhase.Idle`, `currentFlowStep=5`, 샘플 입력/추천 위성 설정, `StartSelectedMission()` 미호출 조건이 명확하다.
- [ ] `UIManager.cs` 최소 구현 범위 확정 → 검증: 새 기능은 기존 설정 패널/런처 안에서 끝나고 새 대형 패널이나 레이아웃 재배치를 만들지 않는다.
- [ ] 준비 프리셋 구현 또는 run card 대체 → 검증: 구현 시 자동 발신하지 않고, 보류 시 동일 흐름을 수동 조작 순서로 재현할 수 있다.
- [ ] 기존 `시연 전송` 보존 → 검증: 기존 `StartDemoModeNow()` 또는 대응 경로가 단일 조작으로 Mission Complete까지 도달한다.
- [ ] 확인 단계 safety summary 보강 → 검증: 실패 테스트 ON/OFF, 자동 전송 ON/OFF, payload 상태, 선택 위성 가능 여부가 확인 단계에서 보인다.
- [ ] 수동 5단계 flow 리허설 → 검증: `시간 → 메시지/첨부 → 목적지 → 위성 → 확인 → 전송 시작`이 2~3분 안에 끊기지 않는다.
- [ ] 준비 프리셋 수동 발신 리허설 → 검증: `시연 준비` 후 발표자가 직접 `전송 시작`을 눌러 Complete 또는 명확한 실패 상태에 도달한다.
- [ ] 실패/재시도 UI 확인 → 검증: 실패 테스트 ON 상태가 명확하고 Reset/새 메시지 뒤 의도치 않게 유지되지 않는다.
- [ ] 대기/기록/재사용 flow 확인 → 검증: 전송 중 추가 메시지는 대기열로 가고 완료 후 기록 보기/재사용/삭제/경로 재생이 필수 CTA를 가리지 않는다.
- [ ] 패널 복구 QA → 검증: 작성/위성/상태/진행/기록/대기/통계/설정 패널을 닫거나 드래그한 뒤 `패널 정리`로 복구된다.
- [ ] `PresentationDirector` camera cut QA → 검증: Reserved/Running/Observing/Transmitting/Complete 전환에서 UI가 설명 대상과 겹치지 않는다.
- [ ] IMGUI fallback 확인 → 검증: `showFallbackOverlay=false` 기준 Runtime Canvas만 표시되고, fallback은 발표 근거로 과대해석하지 않는다.
- [ ] UI 변경 후 validation handoff → 검증: build-validation lane에서 `ValidateMissionFlow` 재실행 결과 또는 미실행 사유를 받는다.
- [ ] 문서 handoff → 검증: `시연 준비`, `시연 전송`, 자동 검증, 수동 GUI 미추적 상태가 README/WORK_LOG/product 문서와 충돌하지 않는다.

## 리스크/확인 필요
- `시연` 버튼이 현재 자동 발신까지 수행해 수동 발표 흐름과 섞일 수 있다. Safe alternative: 구현 범위가 커지면 새 기능 대신 run card로 `샘플 입력 → 직접 전송 시작`을 고정한다.
- `AutoStartQueuedMessages`는 저장 상태라 발표 중 의도치 않은 다음 전송이 시작될 수 있다. Safe alternative: 확인 단계에 ON/OFF 상태를 노출하고 준비 모드에서는 상태 확인을 필수로 둔다.
- 자동 검증과 캡처는 수동 Editor GUI 리허설이 아니다. 수동 GUI 리허설은 사용자 결정상 제출 게이트는 아니지만, 실행했다면 별도 근거로만 기록한다.
- `ImmediateUIManager` fallback에는 legacy/영어 문구가 남아 있다. Safe alternative: fallback overlay는 기본 비활성 근거만 확인하고, 켜야 하는 상황이 생길 때만 최소 문구 정리한다.
- 작업트리에 대량 modified/deleted/untracked가 섞여 있다. UI 계획에서는 되돌리거나 정리하지 않고, 제출/커밋 인벤토리는 spec lane과 연결한다.
- Windows 빌드, MP4/PPT, `Submission/`, 새 Asset Store/SFX import, Git LFS, 실제 결제/운영 DB/secret/외부 업로드, 실제 TLE/live data는 요구 변경 전 실행 계획에 넣지 않는다.
- scheduler/PM/product audit/automation metrics 산출물이 현재 확인되지 않으므로 없는 결과를 근거로 쓰지 않는다.

## 병렬 에이전트 활용 메모
- 하위/병렬 에이전트는 사용하지 않았다.
- 이유: 최종 파일을 수정하지 않는 단일 lane note 작성이고, product/daylog/plan/code/report 확인은 병렬 shell 조회로 충분했다.
- 2026-05-27 실제 실행 시에는 `UI run card 작성`, `패널/문구 정적 QA`, `캡처 판독 메모`는 병렬화 가능하다.
- `UIManager.cs` 수정, Unity validation, capture/player smoke는 같은 Unity project lock과 파일을 공유하므로 순차 처리한다.