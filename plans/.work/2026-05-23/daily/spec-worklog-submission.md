# Computer Graphics Assignment 1 - Spec/Work Log/Submission Docs lane note (2026-05-24)

## 최근 진행 근거
- `product/*.md`(2026-05-18): 제품 기준은 서울 발신자, 3개 목적지, 발신 시각/메시지 용량, 24개 후보 위성, 1회성 단문 릴레이이며 제출은 Unity 프로젝트 자체로 결정됨.
- `README.md`, `WORK_LOG.md`(2026-05-18): 최신 product 결정 일부는 반영됐지만, 2026-05-22에 추가된 단계형 발신, 첨부, 대기열, 기록/재전송, 실패/혼잡도/로그 기능은 아직 문서 반영이 부족해 보임.
- `daylog/2026-05-22.md`(2026-05-22): 단계형 발신 플로우, 메시지/첨부 UX, 드래그 오버레이, 대기열/기록/재전송, 전송 영수증, 실패 시뮬레이션, 혼잡도, 추천 발신 시간, 전송 로그 기능 추가 기록.
- `orbitshare-validation.md`(2026-05-22 22:04 KST): `Result: PASS`, 24개 위성, Fast demo 완료, relay/delivery 100%, reset replay 2회, Runtime Error/Exception 0. 단 `Screen: 640x480`이라 발표 해상도 GUI E2E 근거는 아님.
- `unity-validation-more-features.log`, `unity-build-more-features.log`(2026-05-22): validation report 저장과 Linux build success 로그 기록. 다만 제출 문서에는 build 산출물 timestamp/포함 범위 확인을 별도 분리해야 함.
- `daylog/2026-05-23.md`(2026-05-23): 신규 코드/에셋 구현 없이 정적 조사 중심. GUI E2E, 최신 1600x900 캡처, player display smoke는 새로 실행하지 않음.
- `orbitshare_shared_satellite_agent.md`(상단 기준 2026-05-18): 최신 발표 Q&A가 있지만 제출물 표에는 Windows/PPT/MP4 “확인 필요” 표현과 legacy 관측/예약 플랫폼 원문이 남아 있음.
- `ASSET_STORE_IMPORT.md`, `asset-store-import-report.md`: `SpaceSkybox.mat`, `SpaceStation.prefab`은 FOUND, 위성 모델/Premium Earth/SFX는 optional MISSING 및 fallback 대상.
- `plans/daily/2026-05-24.md`: 없음. `plans/features`, `plans/verify/ready`, `plans/verify/reports`, `PM/status`, `PM/reports`: 파일 없음. 03:00 scheduler, verify scheduler, product audit, automation metrics 근거는 부재로 기록해야 함.

## 내일 목표 후보
- 1순위: 최종 제출/발표 문서 truth table을 만든다. 구현 기능, 검증 근거, 미실행 GUI 확인, dropped/deferred 산출물을 한 표로 분리한다.
- 2순위: README/WORK_LOG/product/Q&A를 2026-05-22 기능 기준으로 갱신할 범위를 확정한다. 단, 실제 GUI로 확인하지 않은 항목은 “구현/자동 검증됨”과 “수동 확인 필요”를 구분한다.
- 3순위: 발표용 run card와 Q&A를 최신 단문 발신 컨셉으로 정리한다. legacy 관측/예약 플랫폼은 보존하되 발표 기준이 아님을 명시한다.
- 4순위: 제출 인벤토리와 Git 포함/제외 후보를 정리한다. 대량 asset 삭제, untracked shader/texture/script/daylog, build 산출물 제외 정책을 표로 분리한다.
- 5순위: GUI E2E, 최신 screenshot, audio, Resource Hook 결과가 다른 lane에서 나오면 문서에 반영하고, 없으면 완료로 쓰지 않는다.
- 6순위: `daylog/2026-05-24.md`에는 실제 실행한 검증, 미실행 검증, 문서 변경, 남은 제출 리스크를 기록한다.

## 상세 체크리스트 초안
- [ ] 2026-05-22 기능 delta 표 작성 → 검증: `daylog/2026-05-22.md`의 기능이 README/WORK_LOG/product/Q&A에 반영됨/미반영/확인 필요로 분류됨.
- [ ] 제출/발표 성공 기준 매트릭스 작성 → 검증: 자동 validation, Linux build log, visual review, GUI E2E, screenshot, asset hook, dropped 산출물이 서로 다른 등급으로 표시됨.
- [ ] README Demo Flow 갱신 후보 정리 → 검증: `시간 → 메시지/첨부 → 목적지 → 위성 → 발신`, 대기열/기록/재전송/영수증/로그 기능이 실제 구현 범위와 충돌하지 않음.
- [ ] WORK_LOG 최종 점검일/제출 상태 정리 → 검증: Windows/PPT/MP4/Submission이 “필수 확인 필요”가 아니라 현재 product 결정상 dropped 또는 요구 변경 시 재검토로 표현됨.
- [ ] 발표 Q&A 최신화 → 검증: 실제 궤도/TLE/결제/live data가 아님, 요금/성공률/혼잡도/추천시간은 시뮬레이션 값임을 명시함.
- [ ] GUI E2E 반영 슬롯 준비 → 검증: `빠른 발신`, 수동 발신, Reset/재진입, 드래그/첨부/기록/재전송 확인 결과가 없으면 todo로 남김.
- [ ] 정적 제출 인벤토리 작성 → 검증: `Assets/`, `Packages/`, `ProjectSettings/`, `Assets/Scenes/MainScene.unity`, README/WORK_LOG/product/daylog/validation/screenshot/build 상태가 분리됨.
- [ ] Git/대용량/라이선스 상태 표 작성 → 검증: tracked 삭제 269개, untracked 신규 리소스/스크립트/daylog, Asset Store 리소스, private repo/Git LFS dropped 상태가 기록됨.
- [ ] `daylog/2026-05-24.md` 작성 → 검증: 변경 파일, 실제 실행 검증, 미실행 검증, 남은 확인 필요가 짧게 기록됨.

## 리스크/확인 필요
- README/WORK_LOG/product 문서가 2026-05-18 기준이라 2026-05-22 대규모 기능과 불일치할 수 있음.
- Safe alternative: 코드 단정 대신 `daylog/2026-05-22.md`와 최신 validation 근거로 “문서 갱신 후보” 표를 먼저 만든다.
- 실제 Unity Editor GUI 수동 리허설은 아직 완료 근거가 없음.
- Safe alternative: batch validation PASS와 Linux build log는 보조 근거로만 쓰고 GUI E2E 완료로 표현하지 않는다.
- 최신 1600x900 screenshot capture는 headless crash 이력이 있음.
- Safe alternative: 자동 캡처 재시도는 build/validation lane에 맡기고, docs lane은 기존 PNG와 미확인 상태를 분리한다.
- Windows 빌드, PPT/PDF, MP4/MOV, `Submission/` 패키징은 현재 product 결정상 dropped.
- Safe alternative: 실제 생성/업로드 계획 대신 제출 요구 변경 시 실행할 체크리스트만 둔다.
- Asset Store 추가, SFX 추가, Git LFS, 대용량 리소스 정책 변경은 자동 계획에서 직접 실행하지 않는다.
- Safe alternative: 현재 포함 asset과 procedural/generated fallback 상태를 제출 설명에 명확히 둔다.
- scheduler/PM 산출물이 없으므로 없는 feature/verify/product audit/automation metrics를 근거로 인용하면 안 됨.
- 작업트리에 대규모 삭제와 untracked 파일이 섞여 있어 제출/커밋 범위 오판 위험이 있음.

## 병렬 에이전트 활용 메모
- 하위/병렬 에이전트는 사용하지 않음.
- 이유: 이번 작업은 read-only 문서 근거 통합과 lane note 작성이며, 최종 수정 파일을 나누는 단계가 아님.
- 파일 확인은 병렬 shell 호출로 처리했고, 통합 판단은 단일 lane note로 정리함.
- 내일 실제 문서 작업에서는 `기능 delta 조사`, `제출 인벤토리/Git 상태 조사`, `발표 Q&A/legacy 충돌 조사`는 병렬 read-only로 나눌 수 있다. 최종 README/WORK_LOG/daylog 반영은 단일 Docs Integrator가 수행해야 한다.