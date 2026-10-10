# Computer Graphics Assignment 1 - Spec/Work Log/Submission Docs lane note (2026-05-25)

## 최근 진행 근거
- 사용자 제공 AGENTS 지침(2026-05-24 요청): 프로젝트 내부/상위 `AGENTS.md` 파일은 없음. 사용자 제공 지침을 기준으로 적용.
- `product/*.md`(2026-05-18): 제출 기준은 Unity 프로젝트 자체이며 Windows 빌드, MP4/MOV, PPT/PDF, `Submission/` 패키징, 새 Asset Store/SFX, Git LFS는 현재 dropped. 실제 GUI 수동 리허설은 todo.
- `README.md`, `WORK_LOG.md`(2026-05-18): 기본 실행/데모 흐름은 설명하지만 2026-05-22 이후 추가된 대기열, 기록/재전송, 실패 재시도, 다중 첨부, 고해상도 지구 텍스처 상태가 충분히 반영되지 않음.
- `daylog/2026-05-22.md`: 단계형 발신 플로우, 드래그 오버레이, 메시지/첨부 UX, 대기열, 기록/재전송, 실패 시뮬레이션, 혼잡도, 추천 발신 시간, 전송 로그 기능 추가. 여러 validation/build는 PASS로 기록됨.
- `daylog/2026-05-23.md`: 다중 첨부, 저장/복원, 템플릿, 통계, 위성/목적지 검색, 대기 메시지 수정/보류/재개, 고대비, 패널 리사이즈, 경로 재생 제어, UI/3D/사운드 폴리싱이 기록됨. 최신 Linux build와 player 실행 smoke는 통과로 기록됨.
- `daylog/2026-05-24.md`: Solar System Scope/Wikimedia 기반 8192x4096 지구 텍스처 5종과 `ATTRIBUTION.md` 확인, `EarthDayNight.shader`/`EarthClouds.shader` 연결 확인, `unity-build-earth-textures.log`, `unity-build-earth-color-fix.log` build success 확인.
- `orbitshare-validation.md`(현재 작업트리): 파일이 삭제 상태로 확인됨. 2026-05-24에는 새 `ValidateMissionFlow` 실행 근거가 없음.
- `orbitshare-playmode.png`(2026-05-20): 2026-05-22 이후 UX 이전 화면이므로 최신 발표 화면 근거로 쓰기 부족함.
- `plans/daily/2026-05-24.md`: 전날 계획은 문서 truth table, README/WORK_LOG/product/Q&A 갱신, GUI E2E/visual evidence 분리 기록을 요구함.
- `plans/features`, `plans/verify/ready`, `plans/verify/reports`: 디렉터리는 있으나 파일 없음. `PM/status`, `PM/reports`, product audit, automation metrics 산출물도 없음.
- 현재 git 상태 요약(2026-05-24 조사): modified 18개, deleted 270개, untracked 28개. 제출/커밋 범위 오판 리스크가 큼.

## 내일 목표 후보
- 1순위: 발표/제출 readiness 문서 slice 작성. README 또는 별도 제출 섹션에 “3분 시연 run card + 성공 기준 + 증거 상태”를 한 번에 볼 수 있게 정리한다.
- 2순위: README/WORK_LOG/product/Q&A 최신화 범위 확정. 2026-05-22~24 기능을 반영하되, GUI 미확인 항목은 완료로 쓰지 않는다.
- 3순위: 제출 truth table 작성. 자동 validation, Linux build, display smoke, GUI E2E, screenshot, asset hook, license/attribution, dropped 산출물을 분리한다.
- 4순위: Git/리소스 인벤토리 작성. 대량 에셋 삭제, 신규 shader/script/texture/daylog, `orbitshare-validation.md` 삭제 상태, 고해상도 지구 텍스처 라이선스 표기를 분류한다.
- 5순위: 발표 Q&A 정리. 실제 TLE/결제/live data가 아니며 요금/성공률/혼잡도/추천 시간은 시뮬레이션 값임을 명확히 한다.
- 6순위: 다른 lane에서 최신 validation, GUI 리허설, screenshot, Resource Hook 결과가 나오면 문서에 반영한다. 없으면 “미실행/확인 필요”로 남긴다.

## 상세 체크리스트 초안
- [ ] 2026-05-22~24 기능 delta 표 작성 → 검증: 단계형 발신, 대기열, 기록/재전송, 실패 재시도, 다중 첨부, 템플릿/통계, 고해상도 지구 텍스처가 문서 반영됨/미반영/확인 필요로 분류됨.
- [ ] 제출 truth table 작성 → 검증: `README.md`, `WORK_LOG.md`, product 문서에서 자동 검증, 빌드, display smoke, GUI E2E, 최신 screenshot, Resource Hook, license/attribution 상태가 서로 섞이지 않음.
- [ ] `orbitshare-validation.md` 삭제 상태 반영 → 검증: “과거 PASS 로그 있음”과 “현재 리포트 파일 없음/재생성 필요”가 분리됨.
- [ ] README Demo Flow 갱신 후보 작성 → 검증: 현재 UI 흐름인 시간 → 메시지/첨부 → 목적지 → 위성 → 발신, 대기/기록/재시도 흐름이 실제 구현 기록과 충돌하지 않음.
- [ ] WORK_LOG 최종 점검일/검증 상태 갱신 후보 작성 → 검증: 2026-05-18 기준 설명과 2026-05-22~24 구현 차이가 명시됨.
- [ ] 발표 Q&A 최신화 → 검증: legacy 관측/예약 플랫폼 문구 대신 최신 단문 발신 컨셉을 기준으로 설명함.
- [ ] 제출 인벤토리 작성 → 검증: `Assets/`, `Packages/`, `ProjectSettings/`, `Assets/Scenes/MainScene.unity`, README/WORK_LOG/product/daylog, build/log/screenshot/validation 상태가 표로 분리됨.
- [ ] 대용량/라이선스 handoff 작성 → 검증: `Assets/Resources/OrbitShare/Earth/ATTRIBUTION.md`, CC BY 4.0 표기, private repo 유지 필요, Git LFS dropped 상태가 문서화됨.
- [ ] B/C 제외 항목 유지 → 검증: Windows/PPT/MP4/Submission/외부 업로드/새 에셋 import/운영 DB/secret 작업이 자동 실행 계획에 들어가지 않음.
- [ ] `daylog/2026-05-25.md` 작성 후보 준비 → 검증: 실제 수정 파일, 실행한 검증, 미실행 검증, 남은 리스크가 짧게 기록될 수 있음.

## 리스크/확인 필요
- `README.md`, `WORK_LOG.md`, product 문서가 2026-05-18 기준이라 최신 기능과 충돌한다.
- Safe alternative: 바로 완료 문구를 쓰지 말고 기능 delta/truth table을 먼저 만들고, 실제 GUI 미확인 항목은 todo로 둔다.
- `orbitshare-validation.md`가 현재 없음.
- Safe alternative: 과거 validation 로그는 과거 근거로만 쓰고, build-validation lane이 새 리포트를 만들기 전까지 최신 PASS로 단정하지 않는다.
- 최신 1600x900 이상 screenshot과 실제 Editor GUI E2E 근거가 없다.
- Safe alternative: Linux player display smoke와 build log는 보조 근거로만 쓰고, GUI E2E 완료 표현은 금지한다.
- 고해상도 지구 텍스처는 외부 CC BY 4.0 리소스다.
- Safe alternative: `ATTRIBUTION.md` 포함 여부와 라이선스 표기를 제출 인벤토리에 넣고, 공개 업로드는 계획하지 않는다.
- 작업트리에 대규모 삭제와 untracked 파일이 섞여 있다.
- Safe alternative: revert 없이 포함/제외 후보만 표로 정리한다.
- scheduler/PM/product audit/automation metrics 산출물이 없다.
- Safe alternative: 없는 산출물을 인용하지 않고 product/daylog/plan 근거만 사용한다.

## 병렬 에이전트 활용 메모
- 이번 lane note 작성에는 하위/병렬 에이전트를 사용하지 않음.
- 이유: 작업 범위가 read-only 문서/로그 근거 통합이고, 최종 산출물이 단일 lane note라 병렬 수정 이점이 작음.
- 파일 확인은 병렬 shell 호출로 처리함.
- 내일 실제 문서 작업에서는 `기능 delta 조사`, `제출/Git 인벤토리 조사`, `Q&A/legacy 충돌 조사`, `검증 evidence 조사`는 병렬 read-only로 나눌 수 있다. README/WORK_LOG/product/daylog 최종 반영은 단일 Docs Integrator가 통합해야 한다.