# Computer Graphics Assignment 1 - Spec/Work Log/Submission Docs lane note (2026-05-26)

## 최근 진행 근거
- 사용자 제공 AGENTS 지침(2026-05-25 요청): 프로젝트 내부/상위 `AGENTS.md` 파일은 확인되지 않아 사용자 제공 지침을 기준으로 적용.
- `daylog/2026-05-25.md`(2026-05-25): README 일부 최신화, 1600x900 `orbitshare-playmode.png` 확인, 빌드/캡처 로그 확인. 단, 실제 Editor GUI 수동 E2E는 아직 미확인.
- `README.md`(파일 시각 2026-05-25 10:40 KST): 실행/조작/발표 흐름이 메시지/첨부, 목적지, 위성 선택, 전송/대기/기록 흐름 중심으로 갱신됨.
- `WORK_LOG.md`(최종 점검 2026-05-18): 2026-05-22~25의 대기열, 기록/재전송, 실패 재시도, 다중 첨부, 7개 목적지, 고해상도 지구 텍스처, 최신 UI 문구가 충분히 반영되지 않음.
- `product/vision.md`, `product/roadmap.md`, `product/backlog.md`, `product/done-criteria.md`(대부분 2026-05-18): Unity 프로젝트 자체 제출, Windows/PPT/MP4/Submission 제외, GUI 수동 리허설 todo 기준은 유효하지만 최신 기능 delta 반영이 부족함.
- `product/decisions.md`(2026-05-24 갱신): 튜토리얼/온보딩 제외 결정, Windows 빌드/MP4/PPT/Submission/새 Asset Store/SFX/Git LFS dropped 유지.
- `plans/daily/2026-05-25.md`(작성일 2026-05-24): 문서 truth table, README/WORK_LOG/product/Q&A 갱신, GUI E2E와 visual evidence 분리 기록을 요구함.
- `plans/daily/2026-05-26.md`: 없음.
- `plans/features`, `plans/verify/ready`, `plans/verify/reports`: 디렉터리는 있으나 파일 없음. `PM/`, `PM/status`, `PM/reports`, product audit, automation metrics 산출물도 없음.
- `orbitshare-validation.md`: 현재 git 상태에서 tracked 삭제 상태. 과거 PASS 로그는 있으나 최신 리포트 존재/PASS로 쓰면 안 됨.
- 현재 git 상태(2026-05-25 조사): modified 21개, deleted 270개, untracked 31개. 제출/커밋 인벤토리 분리 필요.

## 내일 목표 후보
- 1순위: 제출/발표 evidence pack 문서 slice 작성. README 또는 WORK_LOG에 “시연 run card + 성공 기준 + 현재 증거 상태”를 한눈에 보이게 정리한다.
- 2순위: WORK_LOG 최신화. 2026-05-22~25 기능 delta와 최신 검증/미검증/dropped 항목을 분리한다.
- 3순위: product 문서 정합성 갱신. 3개 목적지 기준 문구와 7개 목적지 확장, 대기열/기록/재시도/고해상도 지구 상태를 최신 기준으로 맞춘다.
- 4순위: 제출 truth table 작성. 자동 validation, Linux build, display smoke, screenshot, GUI E2E, Resource Hook, license/attribution, dropped 산출물을 섞지 않는다.
- 5순위: 발표 Q&A 갱신. 실제 TLE/결제/live data가 아니며 요금/성공률/혼잡도/추천 시간은 시뮬레이션 값임을 명확히 한다.
- 6순위: Git/리소스 인벤토리 작성. 대량 삭제, 신규 shader/script/texture, `orbitshare-validation.md` 삭제, CC BY 4.0 지구 텍스처 출처를 분리한다.

## 상세 체크리스트 초안
- [ ] 2026-05-22~25 기능 delta 표 작성 → 검증: README/WORK_LOG/product에 반영됨/미반영/확인 필요 항목이 분리됨.
- [ ] 제출/발표 evidence pack 초안 작성 → 검증: 3분 시연 순서, 성공 기준, 증거 파일, 미확인 항목이 한 표로 정리됨.
- [ ] `orbitshare-validation.md` 삭제 상태 문서화 → 검증: 과거 PASS와 현재 리포트 없음/재생성 필요가 분리됨.
- [ ] WORK_LOG 최종 점검일과 검증 상태 갱신 후보 작성 → 검증: 실행한 검증만 완료로 쓰고 GUI E2E는 todo로 유지됨.
- [ ] product 문서 최신화 후보 작성 → 검증: 7개 목적지, 대기열, 기록/재전송, 실패 재시도, 고해상도 지구 텍스처가 최신 구현 또는 확인 필요로 분류됨.
- [ ] 발표 Q&A 최신화 → 검증: legacy 관측/예약 플랫폼 문구보다 최신 단문 메시지 전송 컨셉을 우선함.
- [ ] 제출 인벤토리 작성 → 검증: `Assets/`, `Packages/`, `ProjectSettings/`, `README.md`, `WORK_LOG.md`, product/daylog, build/log/screenshot/validation 상태가 분리됨.
- [ ] 라이선스/대용량 리소스 handoff 작성 → 검증: `Assets/Resources/OrbitShare/Earth/ATTRIBUTION.md`, CC BY 4.0, private repo 필요성, Git LFS dropped 상태가 명시됨.
- [ ] B/C 제외 항목 유지 → 검증: Windows/PPT/MP4/Submission/외부 업로드/새 에셋 import/운영 DB/secret/실제 결제 작업이 자동 실행 계획에 들어가지 않음.
- [ ] `daylog/2026-05-26.md` 작성 준비 → 검증: 실제 수정 파일, 실행한 검증, 미실행 검증, 남은 리스크를 짧게 기록할 수 있음.

## 리스크/확인 필요
- `orbitshare-validation.md`가 삭제 상태다. Safe alternative: 과거 PASS는 과거 근거로만 쓰고, 최신 validation 재생성 전까지 PASS로 단정하지 않는다.
- 실제 Editor GUI 수동 E2E가 없다. Safe alternative: 1600x900 캡처와 display smoke는 보조 근거로만 쓰고 GUI 완료 표현은 금지한다.
- product/WORK_LOG가 최신 기능보다 뒤처져 있다. Safe alternative: 바로 완료 문구를 쓰지 말고 delta/truth table부터 작성한다.
- 작업트리에 대규모 삭제와 untracked 파일이 섞여 있다. Safe alternative: revert 없이 포함/제외 후보만 표로 정리한다.
- 외부 지구 텍스처는 CC BY 4.0 리소스다. Safe alternative: attribution 포함 여부와 공개 업로드 제한을 제출 인벤토리에 명시한다.
- scheduler/PM/product audit/automation metrics 산출물이 없다. Safe alternative: 없는 산출물은 인용하지 않고 README/product/daylog/plan 근거만 사용한다.

## 병렬 에이전트 활용 메모
- 이번 lane note 작성에는 하위/병렬 에이전트를 사용하지 않음.
- 이유: 최종 산출물이 단일 lane note이고, 범위가 문서/로그 read-only 근거 통합이라 병렬 에이전트 이점이 작음.
- 파일 확인은 병렬 shell 호출로 처리함.
- 내일 실제 문서 작업에서는 `기능 delta 조사`, `제출/Git 인벤토리 조사`, `Q&A/legacy 충돌 조사`, `검증 evidence 조사`를 병렬 read-only로 나눌 수 있다. README/WORK_LOG/product/daylog 최종 반영은 단일 Docs Integrator가 통합해야 한다.
- 이번 작업은 파일 수정을 요구하지 않는 lane note 조사이므로 별도 daylog는 작성하지 않음.