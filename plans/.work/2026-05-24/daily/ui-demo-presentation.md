# Computer Graphics Assignment 1 - UI/Demo Flow/Presentation lane note (2026-05-25)

## 최근 진행 근거
- 2026-05-24 `daylog/2026-05-24.md`: 지구 고해상도 텍스처/셰이더 연결과 Linux build/display smoke 로그를 확인했으나, 최신 GUI 수동 E2E·1600x900 visual review·오디오 청취·Resource Hook validation은 새로 확인하지 않았다.
- 2026-05-24 `daylog/2026-05-24.md`: 현재 작업트리에서 `orbitshare-validation.md`가 삭제 상태이며, 오늘 새 `ValidateMissionFlow` 결과는 확인하지 않았다고 기록되어 있다.
- 2026-05-23 `daylog/2026-05-23.md`: `UIManager`, `MissionManager`, `CameraManager`, `MissionVisualController` 중심으로 대기열, 기록/다시 보내기, 다중 첨부, 목적지 확장, 시연 버튼, 설정/토스트/확인 팝업, 패널 리사이즈, 고대비, 위성 클릭 선택, 경로 재생 제어 등이 추가됐다.
- 2026-05-23 `daylog/2026-05-23.md`: 여러 Linux build와 1280x720 player 실행 smoke는 통과했지만, 실제 Editor GUI 클릭 리허설과 최신 스크린샷 판독은 여전히 확인 필요로 남았다.
- 2026-05-22 `daylog/2026-05-22.md`: 단계형 발신 플로우(`시간 → 메시지 → 목적지 → 위성 → 발신`), 드래그 오버레이, 첨부 카드, 대기열, 실패/재시도, 로그 기능이 추가됐고 자동 검증은 PASS였으나 headless screenshot capture는 Unity native crash로 실패했다.
- 2026-05-18 `product/vision.md`, `product/roadmap.md`, `product/done-criteria.md`: 제품 목표는 단발성 단문 위성 발신 UX이며, 남은 핵심은 실제 Unity Editor GUI 수동 리허설이다.
- 2026-05-18 `README.md`, `WORK_LOG.md`: Demo Flow와 제출 상태는 3개 목적지/기존 UI 기준 설명이 많아, 2026-05-23 이후 목적지 확장·복잡 UI·고해상도 지구 텍스처 상태와 불일치 가능성이 있다.
- 2026-05-24 확인 기준: `plans/features`, `plans/verify/ready`, `plans/verify/reports`는 파일이 없고, `PM/`, product audit, automation metrics 산출물도 없다. 기존 `plans/daily/2026-05-25.md`도 없다.
- 코드 확인: `UIManager.StartDemoModeNow()`는 샘플 목적지/메시지/첨부/추천 위성을 채운 뒤 바로 `StartSelectedMission()`을 호출한다. 발표 수동 리허설용 “준비만 하는 프리셋”과 즉시 자동 시연이 현재 분리되어 있지 않다.
- 코드 확인: `ImmediateUIManager.showFallbackOverlay` 기본 false 구조와 `PresentationDirector` phase별 camera cut 로직은 유지되어 있다.

## 내일 목표 후보
- 1순위: 신규 feature slice로 `시연 준비 프리셋`과 `즉시 시연 시작`을 분리한다. 준비 프리셋은 샘플 메시지/목적지/추천 위성/확인 단계까지만 채우고, 최종 발신은 발표자가 직접 누르게 한다.
- 2순위: 발표 실수 방지 UX를 최소 보강한다. 자동 전송, 실패 시뮬레이션, 대기열 자동 시작, 기록/통계/템플릿 패널이 첫 발표 흐름을 방해하지 않도록 상태와 문구를 점검한다.
- 3순위: 실제 GUI 또는 player에서 수동 `시간 → 메시지/첨부 → 목적지 → 위성 → 보내기 시작` 흐름과 기존 `빠른 발신/시연 시작` 흐름을 분리해 리허설한다.
- 4순위: 1280x720 player와 가능하면 1600x900/Editor Game View에서 드래그 패널, 설정/토스트/확인 팝업, 결과/기록/대기열 패널 겹침을 확인한다.
- 5순위: `PresentationDirector` 카메라 컷과 새 UI 오버레이가 서로 가리지 않는지 확인하고, 문제가 재현될 때만 최소 수정한다.
- 6순위: README/product 문서의 목적지 수, 자동 검증 리포트 상태, 최신 UI 기능 불일치는 spec-worklog-submission lane에 수정 후보로 넘긴다.

## 상세 체크리스트 초안
- [ ] `시연 준비`와 `시연 시작` 동작 분리 설계 → 검증: 준비 버튼은 `MissionPhase.Idle`을 유지하고 샘플 입력·추천 위성·확인 단계까지만 이동한다.
- [ ] `UIManager.cs` 최소 수정 후보 확정 → 검증: 새 버튼/문구가 기존 패널 런처나 설정 패널 안에 들어가며 대형 레이아웃 재배치를 만들지 않는다.
- [ ] 준비 프리셋 적용 후 수동 발신 확인 → 검증: 발표자가 직접 `보내기 시작`을 눌러 Mission Complete까지 도달한다.
- [ ] 기존 즉시 시연 흐름 보존 → 검증: `시연 시작` 또는 기존 빠른 흐름은 단일 조작으로 relay/delivery 완료까지 간다.
- [ ] 실패/자동/대기열 안전 상태 확인 → 검증: 새 메시지/Reset 후 실패 시뮬레이션과 자동 대기 시작이 기본 OFF 또는 명확한 상태로 보인다.
- [ ] 패널 복구/닫기 QA → 검증: 작성, 위성, 상태, 진행, 기록, 대기, 통계, 템플릿, 설정 패널을 열고 닫아도 필수 CTA가 가려지지 않는다.
- [ ] 드래그/리사이즈 QA → 검증: 주요 패널을 움직이거나 크기 조절해도 화면 밖으로 사라지지 않고 `패널 정리`로 복구된다.
- [ ] 첨부/파일 선택 표현 확인 → 검증: demo attachment와 런타임 파일 선택 fallback이 실제 외부 업로드/운영 파일 전송처럼 오해되지 않는다.
- [ ] `PresentationDirector` camera cut 확인 → 검증: Reserved/Running/Observing/Transmitting/Complete 전환에서 카메라가 UI와 설명 대상을 가리지 않는다.
- [ ] IMGUI fallback 중복 확인 → 검증: `showFallbackOverlay=false` 기준 Runtime Canvas만 표시된다.
- [ ] 최신 validation 필요성 전달 → 검증: UI 변경 후 build-validation lane에서 `orbitshare-validation.md` 재생성 또는 미실행 사유를 기록한다.
- [ ] 최신 화면 근거 확보 경로 결정 → 검증: headless capture crash 재발 시 자동 캡처 PASS로 쓰지 않고 player/manual 판독으로 대체한다.

## 리스크/확인 필요
- `orbitshare-validation.md`가 현재 삭제 상태라 product 문서의 “최신 PASS” 표현을 그대로 다음 계획 근거로 쓰면 위험하다.
- `StartDemoModeNow()`가 자동 발신까지 수행하므로, 발표자가 수동으로 설명할 경로와 자동 시연 경로가 섞일 수 있다.
- UI 기능이 5/22~5/23 사이 크게 늘어 발표 흐름이 산만해질 수 있다. Safe alternative: 코드 변경 없이 run card와 “눌러야 할 버튼 순서”만 남긴다.
- 실제 Unity Editor GUI 리허설 근거는 아직 없다. batch validation, player smoke, screenshot은 GUI E2E 완료로 쓰지 않는다.
- 목적지가 3개에서 7개로 확장된 기록이 있으나 README/product는 아직 3개 목적지 기준이다. UI lane은 문서 수정 대신 불일치 후보를 넘긴다.
- headless screenshot capture는 native crash 이력이 있다. Safe alternative: 1280x720 player 화면 판독과 수동 체크리스트로 대체한다.
- Unity validation, GUI 리허설, screenshot, Resource Hook, build는 project lock/log 충돌 위험이 있어 병렬 실행하지 않는다.
- Windows build, MP4/PPT, `Submission/` 패키징, 새 Asset Store/SFX import, Git LFS는 product 결정상 dropped/deferred이므로 요구 변경 전 계획에 넣지 않는다.

## 병렬 에이전트 활용 메모
- 이번 lane note 작성에는 하위/병렬 에이전트를 사용하지 않았다.
- 이유: 최종 파일을 수정하지 않는 단일 lane 계획 작업이고, 문서/코드 확인 범위가 병렬 shell 조회로 충분했다.
- 2026-05-25 실제 실행 시에는 `UI 프리셋 동작 설계`, `GUI 리허설 체크리스트`, `문서 불일치 조사`는 read-only 병렬화 가능하다.
- `UIManager.cs` 수정과 Unity 실행 검증은 같은 파일/Unity project lock을 공유하므로 순차 처리해야 한다.