# Computer Graphics Assignment 1 - UI/Demo Flow/Presentation lane note (2026-05-26)

## 최근 진행 근거
- 2026-05-18 `product/vision.md`, `product/roadmap.md`, `product/backlog.md`, `product/done-criteria.md`: 제품 목표는 단발성 위성 메시지 발신 UX이며, 남은 핵심은 실제 Unity Editor GUI에서 `빠른 발신`, 수동 `위성 확정 → 메시지 발신`, Reset을 사람이 리허설하는 것이다.
- 2026-05-24 `product/decisions.md`: 튜토리얼/온보딩은 제외. Windows 빌드, MP4/PPT, `Submission/`, 새 Asset Store/SFX import, Git LFS는 제출 요구 변경 전 dropped/deferred다.
- 2026-05-25 `README.md`: 현재 발표 흐름은 `시간 선택 → 메시지/첨부 → 목적지 선택 → 위성 추천/선택 → 전송/대기/기록`으로 정리돼 있다.
- 2026-05-25 `daylog/2026-05-25.md`: UI/전송 흐름/월드 비주얼 보강, 한글 문구 정리, 1600x900 `orbitshare-playmode.png` 확인, Linux build/capture/display smoke 근거가 남았다.
- 2026-05-25 `daylog/2026-05-25.md`: `orbitshare-validation.md`는 tracked 삭제 상태이며, 최신 `ValidateMissionFlow` PASS로 단정하면 안 된다고 기록됐다.
- 2026-05-25 `daylog/2026-05-25.md`: 최신 캡처와 display smoke는 실제 Editor GUI 수동 E2E가 아니며, 오디오 청취, Resource Hook validation, 최신 GUI 발표 리허설은 새로 완료 확인하지 않았다.
- 2026-05-25 `plans/daily/2026-05-25.md`: `Route Preflight + 시연 준비 프리셋`을 신규 feature slice로 잡았고, 자동 발신 없는 준비 프리셋과 기존 즉시 시연을 분리하는 목표가 있었다.
- 2026-05-25 코드 확인 `Assets/Scripts/UIManager.cs`: `시연 전송` 버튼은 `StartDemoModeNow()`를 호출하고, 이 경로는 샘플 입력 뒤 `StartSelectedMission()`까지 호출해 자동 전송한다. 자동 발신 없는 `시연 준비` 완료 근거는 찾지 못했다.
- 2026-05-25 코드 확인 `Assets/Scripts/ImmediateUIManager.cs`, `Assets/Scripts/PresentationDirector.cs`: IMGUI fallback은 기본 비활성 구조이고, `PresentationDirector`는 phase별 camera cut을 유지한다.
- 2026-05-25 확인: `plans/features`, `plans/verify/ready`, `plans/verify/reports` 파일은 없고 `PM/`, product audit, automation metrics 산출물도 없다. `plans/daily/2026-05-26.md`도 아직 없다.

## 내일 목표 후보
- 1순위: `시연 준비`와 `시연 전송`을 분리하는 최소 feature slice. `시연 준비`는 샘플 메시지/첨부/목적지/추천 위성/확인 단계까지만 채우고 `MissionPhase.Idle`을 유지한다.
- 2순위: 발표 안전 상태 스트립 또는 확인 문구 보강. 자동 전송, 실패 시뮬레이션, 대기열 상태, 선택 위성 가능 여부를 발신 직전에 한눈에 확인하게 한다.
- 3순위: 기존 `시연 전송` 단일 클릭 자동 흐름 보존. 새 준비 모드가 기존 Fast Demo/validation 경로를 깨지 않는지 확인한다.
- 4순위: 실제 GUI 또는 player 리허설. 수동 `시간 → 메시지/첨부 → 목적지 → 위성 → 보내기 시작`, 기존 `시연 전송`, 실패 후 재시도, 대기/기록 복구를 분리 기록한다.
- 5순위: 발표 해상도 UI QA. 1600x900 기준 패널 런처, 작성/위성/상태/결과/기록/대기/설정 패널, 토스트/확인 팝업, 카메라 컷 겹침을 확인한다.
- 6순위: validation/capture 재생성은 build-validation lane과 연결한다. UI 변경 후 `orbitshare-validation.md`를 새로 만들기 전까지 최신 PASS로 쓰지 않는다.

## 상세 체크리스트 초안
- [ ] `시연 준비` 동작 범위 확정 → 검증: 자동 전송 없이 `MissionPhase.Idle`, `currentFlowStep=5`, 샘플 입력/추천 위성만 준비되는 조건이 문서화된다.
- [ ] `UIManager.cs` 최소 구현 또는 run card 대체 결정 → 검증: 버튼/문구 추가가 기존 패널 런처나 설정 패널 안에서 끝나고 대형 레이아웃 재배치가 없다.
- [ ] 기존 `시연 전송` 보존 → 검증: `StartDemoModeNow()` 또는 대응 경로가 단일 조작으로 Mission Complete까지 가는 기존 목적을 유지한다.
- [ ] 발신 직전 안전 상태 표시 → 검증: 자동 전송/실패 시뮬레이션/대기열/선택 위성 가능 여부가 발신 전 확인 단계에서 명확히 보인다.
- [ ] 준비 프리셋 수동 발신 리허설 → 검증: 발표자가 직접 `보내기 시작`을 눌러 Mission Complete까지 도달한 성공/실패가 기록된다.
- [ ] 단계형 수동 flow 리허설 → 검증: `시간 → 메시지/첨부 → 목적지 → 위성 → 확인`이 2~3분 안에 끊기지 않고 진행된다.
- [ ] 실패/재시도 flow 확인 → 검증: 실패 시뮬레이션 ON 상태가 명확하고, Reset/새 메시지 뒤 의도치 않게 켜져 있지 않다.
- [ ] 대기/기록/재사용 flow 확인 → 검증: 전송 중 추가 메시지는 대기열로 가고, 완료 뒤 기록 보기/재사용/삭제/경로 재생이 필수 CTA를 가리지 않는다.
- [ ] 패널 복구 QA → 검증: 작성/위성/상태/진행/기록/대기/통계/템플릿/설정 패널을 닫거나 드래그한 뒤 `패널 정리`로 복구된다.
- [ ] 첨부 UX 문구 확인 → 검증: demo attachment와 파일 경로 입력이 실제 외부 업로드/운영 파일 전송처럼 오해되지 않는다.
- [ ] `PresentationDirector` camera cut QA → 검증: Reserved/Running/Observing/Transmitting/Complete 컷이 UI 패널과 설명 대상을 가리지 않는다.
- [ ] IMGUI fallback 중복 확인 → 검증: `showFallbackOverlay=false` 기준 Runtime Canvas만 표시된다.
- [ ] 최신 validation handoff → 검증: UI 변경 후 build-validation lane에서 `orbitshare-validation.md` 재생성 또는 미실행 사유가 분리 기록된다.
- [ ] 최신 화면 근거 확보 → 검증: 1600x900 캡처 또는 player/manual 판독 결과가 남고, 이를 GUI E2E 완료로 과대해석하지 않는다.

## 리스크/확인 필요
- `orbitshare-validation.md` 삭제 상태가 가장 큰 검증 리스크다. Safe alternative: 과거 validation 로그는 과거 근거로만 쓰고, 최신 PASS는 재생성 뒤에만 쓴다.
- `시연 전송`은 현재 자동 발신까지 수행한다. Safe alternative: 코드 변경이 커지면 새 기능 대신 발표 run card로 `샘플 입력 → 직접 보내기` 순서를 남긴다.
- `AutoStartQueuedMessages`는 저장되는 상태라 발표 중 의도치 않은 다음 전송이 시작될 수 있다. Safe alternative: 준비 모드에서 OFF 확인 또는 발신 전 상태 스트립으로 노출한다.
- 실제 Editor GUI 수동 리허설은 아직 없다. Safe alternative: player display smoke와 1600x900 visual review를 보조 근거로 쓰되 GUI E2E 완료라고 쓰지 않는다.
- UI 기능이 많아 발표 흐름이 산만해질 수 있다. Safe alternative: 새 패널 추가 대신 기존 확인 단계 문구와 run card 중심으로 제한한다.
- headless capture/native crash 이력이 있다. Safe alternative: player/manual 판독으로 대체하고 capture 실패를 앱 런타임 실패로 단정하지 않는다.
- B/C 항목인 Windows/PPT/MP4/Submission/새 에셋/LFS/결제/운영 DB/secret/외부 업로드는 자동 계획에서 실행 대상으로 잡지 않는다.

## 병렬 에이전트 활용 메모
- 이번 lane note 작성에는 하위/병렬 에이전트를 사용하지 않았다.
- 이유: 최종 파일을 수정하지 않는 단일 lane 계획 작업이고, 문서/코드/로그 확인은 병렬 shell 조회로 충분했다.
- 2026-05-26 실제 실행 시에는 `UI run card 작성`, `패널/문구 정적 QA`, `문서 불일치 조사`는 병렬화 가능하다.
- `UIManager.cs` 수정과 Unity 실행 검증은 같은 파일/Unity project lock을 공유하므로 순차 처리해야 한다.