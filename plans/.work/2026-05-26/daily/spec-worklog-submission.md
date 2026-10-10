# Computer Graphics Assignment 1 - Spec/Work Log/Submission Docs lane note (2026-05-27)

## 최근 진행 근거
- `daylog/2026-05-26.md`(2026-05-26): 통신 경로 명확화, 7개 목적지 기준 `orbitshare-validation.md` PASS, 메뉴 캡처 PASS, route-clarity build/validation/capture 로그 판독 기록. 단, 오늘 새 Unity 실행은 아니고 기존 로그/생성 파일 판독 근거로 정리됨.
- `orbitshare-validation.md`(2026-05-26 00:57 KST): `Result: PASS`, 7개 목적지 존재, 24개 위성, Fast Demo 2회 완료, Runtime Error/Exception 0, 목적지별 추천 스냅샷 기록.
- `orbitshare-menu-captures/report.md`(2026-05-26 00:56 KST): 8개 하단 메뉴, 작성 5단계, 직접 설정 달력 캡처가 모두 non-empty PASS이고 active-panel 교차 없음.
- `README.md`(2026-05-25): 발표 흐름이 `시간 선택 → 메시지/첨부 → 목적지 → 위성 추천/선택 → 전송/대기/기록` 기준으로 정리됨.
- `WORK_LOG.md`(최종 점검 2026-05-25): 7개 목적지, 24개 후보 위성, 대기/기록/재시도, 제출 제외 항목을 반영했지만 2026-05-26 route-clarity 검증 근거는 아직 최신 점검으로 반영 필요.
- `product/*.md`(2026-05-25): 현재 제품 기준은 서울 고정 발신자, 7개 목적지, 24개 후보 위성, 단발성 단문 릴레이. Windows/MP4/PPT/Submission, 실제 GUI 리허설, 새 optional 에셋/LFS는 dropped.
- `product/backlog.md`: `P1-005 발표 Q&A 최종 정리`가 아직 todo. legacy 관측 플랫폼과 최신 단문 발신 컨셉 충돌 방지가 남은 문서 feature slice.
- `orbitshare_shared_satellite_agent.md`(2026-05-18): 상단 최신 섹션이 아직 3개 목적지, Windows/PPT 확인 필요, GUI 수동 리허설 필요 표현을 포함해 최신 product/README와 충돌.
- `presentation/OrbitShare_midterm_progress_content.md`(2026-05-25): 중간발표 문맥상 “실제 인공위성 통신 방식은 남은 작업”이라고 설명함. product의 “실제 TLE/live data는 현재 범위 밖” 결정과 함께 표현 정리가 필요.
- `plans/features`, `plans/verify/ready`, `plans/verify/reports`: 파일 0개. `PM/status`, `PM/reports`, product audit, automation metrics도 확인되지 않음.
- `plans/daily/2026-05-27.md`: 없음. `plans/weekly/2026-W22.md`는 2026-05-25 작성이라 `orbitshare-validation.md` 삭제 리스크 등 일부 내용이 2026-05-26 상태보다 오래됨.
- 현재 작업트리: `M 31`, `D 269`, `?? 42`; `git diff --stat` 기준 300 files changed. 제출/커밋 전 포함/제외 인벤토리 필요.

## 내일 목표 후보
- 1순위: `Submission Evidence Pack v1` 문서 slice 작성. README/WORK_LOG에 3분 시연 순서, 성공 기준, 증거 파일, 미실행/제외 항목을 한 표로 묶는다.
- 2순위: `orbitshare_shared_satellite_agent.md` 상단 최신 섹션과 발표 Q&A를 7개 목적지/24개 위성/최신 validation/dropped 기준으로 갱신한다.
- 3순위: WORK_LOG 최종 점검을 2026-05-26 route-clarity 근거까지 반영해 최신화한다.
- 4순위: product/README/발표 원고 간 “실제 통신 방식 남음” 표현을 정리한다. 실제 TLE/DB/결제/live data 구현 계획으로 오해되지 않게 future/deferred로 둔다.
- 5순위: 제출/Git/리소스 인벤토리 작성. 대량 삭제, 신규 shader/script/texture, Earth attribution, optional missing asset/SFX, private repo 리스크를 분리한다.
- 6순위: 검증 evidence 용어 통일. Unity Batch, menu capture, display smoke, visual review, 수동 GUI 리허설 dropped를 서로 섞지 않는다.

## 상세 체크리스트 초안
- [ ] scheduler/PM 산출물 부재 재확인 → 검증: `plans/features`, `plans/verify/ready`, `PM/status`, `PM/reports` 파일 수와 부재 사유를 문서에 기록.
- [ ] `Submission Evidence Pack v1` 초안 작성 → 검증: 시연 순서, 성공 기준, 증거 파일, 미실행/제외 항목이 README 또는 WORK_LOG에서 한눈에 보임.
- [ ] `orbitshare_shared_satellite_agent.md` 상단 최신 섹션 갱신 → 검증: 3개 목적지, Windows/PPT 확인 필요, GUI 수동 리허설 필요 표현이 최신 섹션에서 제거 또는 과거 근거로 분리됨.
- [ ] 발표 Q&A 최신화 → 검증: 7개 목적지, 24개 위성, 자동 검증 PASS, 요금/성공률은 시뮬레이션 값, 실제 TLE/live data 제외가 명확함.
- [ ] WORK_LOG 2026-05-26 근거 반영 → 검증: `orbitshare-validation.md`, `orbitshare-menu-captures/report.md`, `unity-build-route-clarity.log`, `daylog/2026-05-26.md`가 날짜와 함께 분리 기록됨.
- [ ] product/발표 원고 표현 대조 → 검증: “실제 통신 방식 남음”이 내일 실행할 기능처럼 쓰이지 않고, 과제 범위 밖 또는 향후 개선으로 분류됨.
- [ ] 제출/Git 인벤토리 표 작성 → 검증: `M/D/??` 파일군, 대량 Asset Store 원본 삭제, 신규 Earth/shader/script, 문서/캡처/validation 산출물이 포함/제외 후보로 분리됨.
- [ ] 리소스/라이선스 handoff 작성 → 검증: `Assets/Resources/OrbitShare/Earth/ATTRIBUTION.md`, CC BY 4.0, optional SFX/model missing/fallback 상태가 문서와 일치함.
- [ ] 문서 완료 금지 표현 점검 → 검증: 실행하지 않은 검증, 실제 결제/운영 DB/외부 업로드/Windows/MP4/PPT/Submission 완료 표현이 없음.
- [ ] 문서 수정 시 `daylog/2026-05-27.md` 작성 → 검증: 변경 파일, 실제 확인한 검증, 미실행 검증, 남은 리스크가 짧게 기록됨.

## 리스크/확인 필요
- `orbitshare_shared_satellite_agent.md`는 legacy 문서다. Safe alternative: 하단 원문은 보존하고 상단 최신 기준/Q&A만 갱신한다.
- `plans/weekly/2026-W22.md` 일부는 2026-05-26 최신 validation 상태보다 오래됐다. Safe alternative: 날짜가 최신인 `daylog/2026-05-26.md`, `orbitshare-validation.md`를 우선 근거로 둔다.
- 작업트리에 대량 삭제와 untracked 파일이 섞여 있다. Safe alternative: 되돌리거나 정리하지 말고 제출/커밋 인벤토리만 작성한다.
- 중간발표 원고의 “실제 통신 방식 구현 남음”은 product의 deferred 범위와 충돌할 수 있다. Safe alternative: 실제 TLE/live data 구현이 아니라 현재 시뮬레이션 한계와 향후 개선으로 표현한다.
- 공개 저장소 제출 시 Earth texture/Asset Store/대용량 파일 정책 확인이 필요하다. Safe alternative: attribution과 private repo 권장만 문서화하고 LFS/새 에셋 작업은 보류한다.
- 다음 코드 변경이 있으면 기존 `orbitshare-validation.md`를 최신 PASS로 쓰면 안 된다. Safe alternative: build-validation lane의 새 validation 결과가 생긴 뒤 문서 근거를 갱신한다.
- 실제 결제, 운영 DB, secret, 외부 업로드, Windows 빌드, MP4/PPT 생성, `Submission/` 패키징은 자동 계획에서 실행 대상으로 두지 않는다.

## 병렬 에이전트 활용 메모
- 하위/병렬 에이전트는 사용하지 않음.
- 이유: 이번 산출물은 단일 spec lane note이고, 읽기 전용 문서 근거 통합이라 별도 에이전트보다 직접 취합이 명확했다.
- 대신 독립 파일 조회는 병렬 shell 호출로 처리했다.
- 내일 실제 문서 작업이 커지면 `legacy Q&A 조사`, `검증 evidence 조사`, `제출/Git 인벤토리 조사`를 read-only 병렬 하위 작업으로 나누고, README/WORK_LOG/product/daylog 최종 반영은 단일 Docs Integrator가 통합하는 방식이 적절하다.