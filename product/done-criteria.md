# Done Criteria

## 완료로 인정하는 기준

- 구현 완료만으로는 `done`이 아니다.
- 실제 실행, 테스트, smoke, 스크린샷, 로그, 리포트 중 하나 이상의 근거가 있어야 한다.
- Unity batch/headless/display smoke/GUI E2E/빌드/제출 자료 검증은 서로 다른 등급으로 기록한다.
- 기존 PASS와 최신 변경 이후 PASS를 섞어 쓰지 않는다.
- 에셋 배치와 UI 품질은 자동화 스크립트 단독 판정으로 완료 처리하지 않는다.
- 스크립트는 로그 수집, 실행 가능 여부 확인, 캡처 생성 같은 보조 수단으로만 쓰고, 실제 Play 화면 캡처를 사람이 판독한 근거를 남긴다.

## 기능 완료 기준 템플릿

| 기능/흐름 | 완료 조건 | 필수 검증 | 증거 파일/로그 | 상태 |
|---|---|---|---|---|
| 기본 OrbitShare 장면 | `MainScene` Play 시 지구, 서울 발신자, 목적지, 위성망이 생성됨 | Unity 자동 검증 | `orbitshare-validation.md` | 최신 PASS |
| 24개 후보 위성망 | 24개 visible/candidate satellites가 생성되고 목록/필터 UX와 연결됨 | 자동 검증 + 화면 판독 | `orbitshare-validation.md`, `orbitshare-playmode.png` | 최신 PASS |
| 목적지/발신자 | Sender Seoul, 기본 destination New York, Seoul/New York/London 존재 | 자동 검증 | `orbitshare-validation.md` | 최신 PASS |
| 한글화된 주요 UI | Runtime Canvas의 주요 버튼/패널이 단문 발신 용어로 표시됨 | Visual Review | `orbitshare-playmode.png` | done_current |
| 발신 시각/용량 선택 | 지금/5분/15분/30분, 32/128/512KB 선택 UI가 보임 | Visual Review + 코드 확인 | `UIManager.cs`, `MissionManager.cs`, `orbitshare-playmode.png` | done_current |
| 위성 필터 | 전체/가능/빠른창/저가 필터가 후보 목록에 적용됨 | 코드 확인 + 화면 판독 | `SatelliteManager.cs`, `UIManager.cs` | done_current |
| 가격/성공 예측 | 선택 위성/목적지/시각/용량 기준 예상 가격과 성공 예측 표시 | 코드 확인 + 화면 판독 | `MissionManager.cs`, `UIManager.cs`, `orbitshare-playmode.png` | done_current |
| 지구본 품질 보완 | land patch 제거, 고해상도 절차 텍스처, 구름 레이어 적용 | Visual Review + 코드 확인 | `OrbitShareBootstrap.cs`, `orbitshare-playmode.png` | done_current |
| `빠른 발신`/Fast Demo | 자동 흐름으로 Mission Complete 도달 | 최신 자동 검증 | `orbitshare-validation.md` | 최신 PASS |
| 수동 `위성 확정 → 메시지 발신` | 목적지/시각/용량/위성 선택부터 complete까지 발표 가능 | 실제 GUI 수동 확인 | 향후 daylog/리허설 기록 | todo |
| Reset/Play 재진입 | 2회차 실행에서 stale state/중복 이벤트/예외 없음 | r1 추가 자동 검증 | `orbitshare-validation.md` | 최신 PASS |
| Runtime Canvas/IMGUI | 1600x900 기준 Canvas와 fallback overlay 기본 비활성 | r1 추가 자동 검증 | `orbitshare-validation.md` | 최신 PASS |
| PresentationDirector camera cuts | 주요 phase camera cut 처리와 완료 Overview 상태 | r1 추가 자동 검증 | `orbitshare-validation.md` | 최신 PASS |
| GUI/display player 시작 | 그래픽 display에서 player가 시작됨 | display smoke | `run_orbitshare.sh`, `build-run-live.log` | smoke PASS |
| Linux 빌드 | 최신 코드 기준 Linux build success와 산출물 존재 | Unity build 로그 확인 | `unity-build.log`, `Builds/Linux/` | 기존 PASS, 최신 재빌드 deferred |
| Windows 빌드 | 현재 제출 기준에서는 필요 없음 | 제외 사유 | `product/decisions.md` | dropped |
| Asset hook/fallback | FOUND/MISSING 상태가 실제 파일과 문서에서 일치 | resource inventory, 필요 시 Unity hook validation | `asset-store-import-report.md`, `orbitshare-playmode.png` | static PASS, 1600x900 화면 판독 완료 |
| 에셋/UI visual review | 1600x900 Play 캡처에서 핵심 에셋과 주요 UI가 보이고, 자동 판정이 아닌 사람 판독 메모가 있음 | Visual Review | `orbitshare-playmode.png`, `daylog/2026-05-18.md` | done_current |
| 제출 패키지 | Unity 프로젝트 자체를 제출 | 제출 기준 확인 | 사용자 지시, product 문서 | done |

## 검증 등급

| 등급 | 의미 | 예시 |
|---|---|---|
| Static | 코드/문서/파일 정적 확인 | `git diff --check`, 파일 존재 검색, report 대조 |
| Unity Batch | Unity batchmode 자동 검증 | `OrbitShareEditorAutomation.ValidateMissionFlow` |
| Display Smoke | 그래픽 display에서 player 시작 확인 | `run_orbitshare.sh` + rootful Xwayland fallback |
| Visual Review | 실제 Play 화면 캡처를 사람이 판독 | 에셋 누락, UI 겹침/잘림, 버튼/패널 가독성 확인 |
| Headless Smoke | 그래픽 없는 실행 확인 | Linux player `-batchmode -nographics`; GUI PASS로 쓰지 않음 |
| GUI E2E | 실제 화면/Play Mode 사용자 흐름 | `빠른 발신`, `위성 확정 → 메시지 발신`, Reset, 발표 해상도 QA |
| Build | 실행 파일 생성 | Linux/Windows build log, 산출물 존재 |
| Submission | 제출 게이트 | Unity 프로젝트 자체 제출 여부와 제외 사유 |

## 일일 보고 비율

현재 운영 모드는 `concept-update/submission-readiness`다.

| 항목 | 비율 | 근거 |
|---|---:|---|
| 신규 기능 개발 | 35% | 한글화, 필터, 단문 발신 UX, 지구본 보완을 반영함 |
| 기존 기능 검증/안정화 | 45% | 자동 검증과 화면 판독은 확보, 실제 수동 GUI 리허설은 남음 |
| 유지보수/문서 | 20% | README/product/daylog/발표 문구 정합성 유지 필요 |

## 완료 금지 조건

다음은 완료로 쓰지 않는다.

- display smoke를 실제 수동 GUI E2E 완료처럼 표현
- 자동화 스크립트 또는 자동 캡처만으로 에셋/UI 품질 최종 PASS라고 표현
- headless `NullGfxDevice` 실행을 발표 화면 또는 GUI smoke PASS로 표현
- Linux 기존 build success를 최신 코드 재빌드 success로 표현
- Unity 프로젝트 제출 결정을 Windows 빌드, PPT/PDF, MP4/MOV 완료처럼 표현
- 가격/성공 확률/위성 가용성을 실제 결제 또는 실제 통신망 결과로 표현
- legacy 관측/예약 플랫폼을 현재 단문 발신 제품 완료 범위로 표현
- optional missing asset/SFX를 필수 실패 또는 품질 완료로 단정
- Asset Store 라이선스/대용량 리소스를 공개 저장소 제출 가능으로 단정

## 기능별 증거 우선순위

| 기능 영역 | 1순위 증거 | 2순위 증거 | 완료 판정 주의 |
|---|---|---|---|
| Core mission | 최신 자동 검증 | 수동 GUI 리허설 | 자동 PASS와 수동 조작은 구분 |
| 단문 발신 UX | 실제 발표 해상도 GUI 기록 | 코드 확인/PNG | 가격·성공 예측은 시뮬레이션임을 명시 |
| UI/Presentation | 실제 발표 해상도 GUI 기록 | PNG/GIF/static 문서 | 스크린샷만으로 Reset/재진입 완료 아님 |
| Asset/UI visual quality | 실제 Play 화면 캡처 + 사람 판독 메모 | 자동 캡처/로그 | 자동 판정 스크립트 단독 완료 금지 |
| Display 실행 | rootful Xwayland/player log | 직접 `:0` 실행 | `:0`이 0x0이면 직접 실행 실패 가능 |
| Build | 최신 build log와 산출물 | 기존 build log | 현재 제출 기준에서는 필수 아님 |
| Asset/Fallback | Unity hook validation + 화면 확인 | `asset-store-import-report.md` | optional missing은 실패가 아님 |
| Submission | Unity 프로젝트 자체 제출 기준 | 파일 존재 검색 | 별도 실행 파일/영상/PPT는 현재 제외 |
