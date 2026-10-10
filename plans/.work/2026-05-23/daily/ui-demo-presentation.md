# Computer Graphics Assignment 1 - UI/Demo Flow/Presentation lane note (2026-05-24)

## 최근 진행 근거
- 2026-05-23 `daylog/2026-05-23.md`: 신규 코드/에셋 변경은 없고, 최신 실행 근거는 2026-05-22 22:04 KST의 `orbitshare-validation.md`로 확인됨. `docs`, `PM`, `PM/status`, `PM/reports`, `AGENTS.md`는 없음.
- 2026-05-22 `daylog/2026-05-22.md`: `UIManager`, `MissionManager` 중심으로 단계형 발신 플로우, 메시지/첨부, 드래그 오버레이, 결과 닫기/새 메시지, 기록/재전송/삭제, 대기열, 실패/재시도, 로그, 리플레이, 혼잡도/추천 발신 시간 기능이 추가됨.
- 2026-05-22 `orbitshare-validation.md`: `Result: PASS`, 위성 24개, Fast demo 완료, Reset replay 2회, `PresentationDirector` camera cuts PASS, Runtime Error/Exception 0. 단 `Screen: 640x480`이라 발표 해상도 GUI E2E 근거는 아님.
- 2026-05-18 `product/vision.md`, `product/roadmap.md`, `product/backlog.md`, `product/done-criteria.md`: 제품 목표는 서울 고정 발신자의 1회성 단문 위성 발신 UX이며, 남은 핵심은 실제 Unity Editor GUI 수동 리허설.
- 2026-05-18 `README.md`: Demo Flow는 `빠른 발신` 또는 `위성 확정 → 메시지 발신` 기준으로 설명되어 있으나, 2026-05-22 이후 추가된 메시지/첨부/대기열/기록 UI는 아직 README 기준에 완전히 반영되지 않은 것으로 보임.
- 2026-05-23 확인: `plans/features`, `plans/verify/ready`, `plans/verify/reports`는 디렉터리만 있고 파일 0개. `PM` 산출물과 product audit/automation metrics 근거는 없음.
- 2026-05-23 확인: 기존 `plans/daily/2026-05-24.md`는 없음.
- 현재 작업트리: `UIManager.cs`, `MissionManager.cs`, `PresentationDirector` 관련 검증 파일, 신규 `UIDragHandle`, `SignalPacketStream`, `DestinationClickTarget` 등 대규모 변경과 에셋 삭제가 섞여 있음.

## 내일 목표 후보
- 1순위: 신규 feature slice로 `발표 준비 프리셋/가이드`를 추가한다. 자동 발신이 아니라, 발표자가 수동으로 `시간 → 메시지 → 목적지 → 위성 → 발신`을 따라가도록 안전한 샘플 메시지·목적지·추천 위성을 채우고 다음 조작만 짧게 안내하는 범위로 제한한다.
- 2순위: 발표 중 실수 방지용 UI 안정화. `실패`, `자동`, `대기열`, `기록`, `로그`, `리플레이`가 첫 발표 흐름을 방해하지 않도록 노출/상태/문구를 점검한다.
- 3순위: 실제 GUI에서 단계형 수동 흐름을 리허설한다. `빠른 발신`과 별도로 수동 `예약 발신 시작` 또는 `위성 확정 → 메시지 발신`이 Mission Complete까지 가는지 확인한다.
- 4순위: 1280x720 Linux player와 가능한 경우 1600x900/Editor Game View에서 드래그 패널, 첨부 패널, 결과 팝업, 기록/대기열 패널 겹침을 확인한다.
- 5순위: `PresentationDirector` 카메라 컷이 새 드래그 오버레이/결과 팝업과 충돌하지 않는지 확인하고, 문제가 재현될 때만 최소 수정한다.
- 6순위: README/product 문서가 최신 UI와 어긋나는 부분은 spec-worklog-submission lane에 수정 후보로 넘긴다.

## 상세 체크리스트 초안
- [ ] 발표 준비 프리셋 범위 확정 → 검증: 자동 전송 없이 샘플 메시지, 기본 목적지, 현재 발신 시각, 추천 위성 선택까지만 수행하도록 정리.
- [ ] 발표 가이드 문구 위치 확정 → 검증: `UIManager` 상단 또는 발신 요청 패널에 1줄 안내를 추가해도 주요 조작 버튼이 밀리거나 겹치지 않음.
- [ ] 프리셋 적용 후 수동 발신 흐름 점검 → 검증: 발표자가 직접 최종 발신 버튼을 눌러 Mission Complete까지 도달.
- [ ] 실패 테스트 버튼 안전성 확인 → 검증: 기본 OFF, 새 메시지/Reset 후 OFF 유지, 실수로 켜졌을 때 UI가 명확히 `실패 ON`을 표시.
- [ ] 대기열/기록/로그/리플레이 패널 노출 점검 → 검증: 첫 발표 흐름에서는 필수 CTA를 가리지 않고, 열린 패널은 닫기 버튼으로 복구됨.
- [ ] 첨부 선택 패널 수동 QA → 검증: 파일 카드 선택/첨부 제거/용량 초과 경고가 메시지 단계에서만 자연스럽게 보임.
- [ ] 드래그 오버레이 QA → 검증: 발신 요청, 위성 선택, 상태, 타임라인, 결과 팝업을 이동해도 화면 밖으로 사라지거나 필수 버튼이 클릭 불가 상태가 되지 않음.
- [ ] `빠른 발신` 리허설 → 검증: 단일 클릭으로 relay lock 100%, delivery 100%, Mission Complete 도달.
- [ ] 수동 Demo Flow 리허설 → 검증: `시간 → 메시지/첨부 → 목적지 → 위성 → 발신` 순서로 2~3분 안에 완료.
- [ ] Reset/새 메시지/재진입 QA → 검증: 완료 후 `새 메시지 보내기`, 두 번째 발신, Play Mode 재진입에서 stale result와 이벤트 중복 없음.
- [ ] `PresentationDirector` 컷 QA → 검증: Reserved/Running/Observing/Transmitting/Complete 컷이 UI 패널과 설명 대상을 가리지 않음.
- [ ] IMGUI fallback 중복 확인 → 검증: `ImmediateUIManager.showFallbackOverlay=false` 기준 Canvas UI만 표시.
- [ ] 최신 자동 검증 재실행 여부 전달 → 검증: UI 변경 후 build-validation lane에서 `ValidateMissionFlow` PASS 또는 미실행 사유를 분리 기록.
- [ ] 화면 근거 갱신 필요성 판단 → 검증: headless screenshot이 계속 crash이면 Linux player 화면 판독/사용자 수동 확인으로 대체하고, 자동 캡처 성공처럼 쓰지 않음.

## 리스크/확인 필요
- 실제 Unity Editor GUI 수동 리허설 근거가 아직 없다. `orbitshare-validation.md`는 자동 검증 PASS일 뿐 발표 화면 조작 완료 근거가 아니다.
- 2026-05-22 이후 UI 기능이 급격히 늘어 발표 흐름이 산만해질 수 있다. Safe alternative: 새 기능 추가 대신 발표 프리셋/가이드 문구만 최소 적용한다.
- headless screenshot capture는 Unity native crash 이력이 있다. Safe alternative: Linux player display smoke와 사람 판독 메모를 별도 등급으로 기록한다.
- `README.md`와 product 문서는 2026-05-18 기준이라 최신 메시지/첨부/기록/대기열 UX와 불일치 가능성이 있다.
- 현재 작업트리에 대규모 삭제와 untracked 신규 파일이 섞여 있다. UI lane은 되돌리거나 정리하지 말고, 검증 전 포함 범위 리스크만 기록한다.
- Unity validation, GUI 리허설, 캡처, 빌드는 project lock/log 충돌 위험이 있어 병렬 실행 금지.
- 실제 OS 파일 업로드는 구현되어 있지 않고 앱 내부 샘플 첨부 카드 방식이다. 발표에서는 “데모용 첨부 선택”으로 설명해야 한다.
- Windows 빌드, MP4/PPT, `Submission/` 패키징, 새 Asset Store/SFX import는 현재 product 결정상 dropped이므로 요구 변경 전 실행 계획에 넣지 않는다.

## 병렬 에이전트 활용 메모
- 하위/병렬 에이전트는 사용하지 않았음.
- 이유: 이번 작업은 lane note 작성을 위한 문서/코드 정적 대조가 중심이고, 최종 수정 파일이 없어서 별도 에이전트로 나눌 만큼 독립 실행 범위가 크지 않았음.
- 파일 탐색과 읽기는 병렬 shell 호출로 처리했음.
- 내일 실제 구현 시에는 `발표 프리셋 UX 점검`과 `GUI 리허설 체크리스트/문서 불일치 조사`는 read-only 병렬화 가능. `UIManager.cs` 수정과 Unity 실행 검증은 순차 진행이 필요함.