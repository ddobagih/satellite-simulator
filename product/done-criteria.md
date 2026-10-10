# Done Criteria

## 완료로 인정하는 기준

- 구현 완료만으로는 `done`이 아니다.
- 실제 실행, 테스트, smoke, 스크린샷, 로그, 리포트 중 하나 이상의 근거가 있어야 한다.
- 기존 PASS와 최신 변경 이후 PASS를 섞어 쓰지 않는다.
- 에셋 배치와 UI 품질은 자동화 스크립트 단독 판정으로 완료 처리하지 않는다.
- 현재 제출 기준에서 실제 Editor GUI 수동 리허설은 별도 완료 조건이나 리스크로 추적하지 않는다.

## 기능 완료 기준 템플릿

| 기능/흐름 | 완료 조건 | 필수 검증 | 증거 파일/로그 | 상태 |
|---|---|---|---|---|
| 기본 OrbitShare 장면 | `MainScene` Play 시 지구, 서울 발신자, 목적지, 위성망이 생성됨 | Unity 자동 검증 또는 build/capture smoke | `unity-build-*.log`, `orbitshare-playmode.png` | done_current |
| 7개 목적지 | Seoul/New York/London/Tokyo/Paris/Sydney/Los Angeles가 선택 가능 | 코드 확인 + 화면/문서 확인 | `OrbitShareBootstrap.cs`, `UIManager.cs`, `README.md` | done |
| 24개 후보 위성망 | 24개 visible/candidate satellites가 생성되고 목록/필터 UX와 연결됨 | 자동 검증 또는 코드 확인 + 화면 판독 | `SatelliteManager.cs`, `orbitshare-validation.md` | done_current |
| 한글화된 주요 UI | Runtime Canvas의 주요 버튼/패널이 단문 발신 용어로 표시됨 | Visual Review | `orbitshare-playmode.png` | done_current |
| 발신 시각/용량 선택 | 지금/5분/15분/30분, 메시지/첨부 용량 선택 UI가 보임 | Visual Review + 코드 확인 | `UIManager.cs`, `MissionManager.cs`, `orbitshare-playmode.png` | done_current |
| 위성 필터/추천 | 전체/가능/빠른창/저가 중심 필터와 동적 추천이 후보 목록에 적용됨 | 자동 검증 + 코드 확인 | `SatelliteManager.cs`, `UIManager.cs`, `orbitshare-validation.md` | done_current |
| 가격/성공 예측 | 선택 위성/목적지/시각/용량 기준 예상 가격과 성공 예측 표시 | 코드 확인 + 화면 판독 | `MissionManager.cs`, `UIManager.cs`, `orbitshare-playmode.png` | done_current |
| 메시지/첨부 payload | UTF-8 메시지와 다중 첨부 합산, 512KB 제한 처리 | 코드 확인 | `MissionManager.cs` | done_current |
| 대기 메시지 | 전송 중 새 메시지를 대기 목록에 추가/수정/보류/취소/바로 보내기 가능 | 코드 확인 + UI 확인 | `MissionManager.cs`, `UIManager.cs` | done_current |
| 전송 기록/재사용 | 완료 후 전송 기록, 기록 재사용, 경로 재생 가능 | 코드 확인 + UI 확인 | `MissionManager.cs`, `UIManager.cs` | done_current |
| 실패 재시도 | 실패 시 재시도 후보와 흐름 제공 | 코드 확인 | `MissionManager.cs`, `UIManager.cs` | done_current |
| 지구본 품질 보완 | land patch 제거, 외부 지구 텍스처/shader, 구름/대기권 적용 | Visual Review + 코드 확인 | `OrbitShareBootstrap.cs`, `Assets/Resources/OrbitShare/Earth/`, `orbitshare-playmode.png` | done_current |
| Fast Demo/시연 전송 | 자동 흐름으로 Mission Complete 도달 가능 | 자동 검증 | `orbitshare-validation.md`, `OrbitShareEditorAutomation.cs` | PASS |
| Reset/Play 재진입 | 2회차 실행에서 stale state/중복 이벤트/예외 없음 | 자동 검증 | `orbitshare-validation.md`, `OrbitShareEditorAutomation.cs` | PASS |
| Runtime Canvas/IMGUI | Runtime Canvas 사용, fallback overlay 기본 비활성 | 자동 검증 또는 코드 확인 | `ImmediateUIManager.cs`, `OrbitShareEditorAutomation.cs` | PASS |
| Linux 빌드 | 최신 코드 기준 Linux build success와 산출물 존재 | Unity build 로그 확인 | `unity-build-transfer-queue-density.log`, `Builds/Linux/` | PASS |
| Display Smoke | 그래픽 display에서 player가 시작됨 | display smoke | `run_orbitshare.sh`, `build-run-live.log` | PASS |
| 자동 검증 리포트 파일 | `ValidateMissionFlow` 실행 결과 파일 존재 | Unity Batch | `orbitshare-validation.md` | PASS |
| 실제 Editor GUI 수동 리허설 | 별도 제출 게이트로 추적하지 않음 | 없음 | 사용자 지시 | dropped |
| Windows 빌드 | 현재 제출 기준에서는 필요 없음 | 제외 사유 | `product/decisions.md` | dropped |
| Asset hook/fallback | FOUND/MISSING 상태가 실제 파일과 문서에서 일치 | resource inventory | `asset-store-import-report.md`, `ASSET_STORE_IMPORT.md` | static PASS |
| 제출 패키지 | Unity 프로젝트 자체를 제출 | 제출 기준 확인 | 사용자 지시, product 문서 | done |

## 검증 등급

| 등급 | 의미 | 예시 |
|---|---|---|
| Static | 코드/문서/파일 정적 확인 | `git diff --check`, 파일 존재 검색, report 대조 |
| Unity Batch | Unity batchmode 자동 검증 | `OrbitShareEditorAutomation.ValidateMissionFlow` |
| Display Smoke | 그래픽 display에서 player 시작 확인 | `run_orbitshare.sh` + rootful Xwayland fallback |
| Visual Review | 실제 Play 화면 캡처를 사람이 판독 | 에셋 누락, UI 겹침/잘림, 버튼/패널 가독성 확인 |
| Headless Smoke | 그래픽 없는 실행 확인 | Linux player `-batchmode -nographics`; 발표 화면 PASS로 쓰지 않음 |
| Build | 실행 파일 생성 | Linux/Windows build log, 산출물 존재 |
| Submission | 제출 게이트 | Unity 프로젝트 자체 제출 여부와 제외 사유 |

## 일일 보고 비율

현재 운영 모드는 `docs-sync/submission-readiness`다.

| 항목 | 비율 | 근거 |
|---|---:|---|
| 신규 기능 개발 | 20% | 핵심 기능은 대부분 구현됨 |
| 검증/안정화 | 35% | 자동 검증 리포트 재생성, build/display smoke, 정적 확인 중심 |
| 유지보수/문서 | 45% | 7개 목적지, validation 상태, 제외 항목 정합성 유지 필요 |

## 완료 금지 조건

다음은 완료로 쓰지 않는다.

- 자동화 스크립트 또는 자동 캡처만으로 에셋/UI 품질 최종 PASS라고 표현
- headless `NullGfxDevice` 실행을 발표 화면 또는 display smoke PASS로 표현
- 새 코드 변경 후 자동 검증을 다시 돌리지 않은 상태에서 이전 `orbitshare-validation.md`를 최신 PASS처럼 표현
- Unity 프로젝트 제출 결정을 Windows 빌드, PPT/PDF, MP4/MOV 완료처럼 표현
- 가격/성공 확률/위성 가용성을 실제 결제 또는 실제 통신망 결과로 표현
- legacy 관측/예약 플랫폼을 현재 단문 발신 제품 완료 범위로 표현
- optional missing asset/SFX를 필수 실패 또는 품질 완료로 단정
- Asset Store 라이선스/대용량 리소스를 공개 저장소 제출 가능으로 단정

## 기능별 증거 우선순위

| 기능 영역 | 1순위 증거 | 2순위 증거 | 완료 판정 주의 |
|---|---|---|---|
| Core mission | 최신 자동 검증 또는 build/capture smoke | 코드 확인 | 코드 변경 후에는 `orbitshare-validation.md`를 다시 생성해야 최신 PASS |
| 단문 발신 UX | 실제 캡처/코드 확인 | README/WORK_LOG | 가격·성공 예측은 시뮬레이션임을 명시 |
| UI/Presentation | PNG/GIF/static 문서 | build/display smoke | 수동 GUI 리허설은 현재 추적 대상 아님 |
| Asset/UI visual quality | 실제 Play 화면 캡처 + 사람 판독 메모 | 자동 캡처/로그 | 자동 판정 스크립트 단독 완료 금지 |
| Display 실행 | rootful Xwayland/player log | 직접 `:0` 실행 | `:0`이 0x0이면 직접 실행 실패 가능 |
| Build | 최신 build log와 산출물 | 기존 build log | 현재 제출 기준에서는 필수 아님 |
| Asset/Fallback | resource inventory | 화면 확인 | optional missing은 실패가 아님 |
| Submission | Unity 프로젝트 자체 제출 기준 | 파일 존재 검색 | 별도 실행 파일/영상/PPT는 현재 제외 |
