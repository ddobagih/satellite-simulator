# Product Decisions

## 결정된 사항

| 날짜 | 결정 | 이유 | 영향 범위 | 근거 |
|---|---|---|---|---|
| 2026-05-18 | 현재 발표 기준 컨셉은 “서울 발신 단발성 위성 단문 발신”으로 둔다 | 사용자가 단순 위성 시뮬레이션이 아니라 원하는 지역으로 짧은 메시지를 보내는 과정이라고 명확히 정정 | vision, README, UI 문구, 발표 Q&A | 사용자 지시, `README.md` |
| 2026-05-18 | 후보 위성 데이터와 목록은 24개로 둔다 | 릴레이 목록이 3개만 보이는 느낌을 해소하고, 발신 시각/목적지 조건에 따른 선택 UX를 제공 | `SatelliteManager`, `UIManager`, done criteria | 사용자 지시 |
| 2026-05-18 | 위성 목록 필터는 전체/가능/빠른창/저가 중심으로 제공한다 | 사용자가 “언제 정보를 보내냐에 따라 해당 위성이 되는지”를 판단해야 한다고 지적 | 후보 위성 UX | 사용자 지시, `UIManager.cs` |
| 2026-05-18 | 가격/성공 확률/발신 가능 창은 발표용 시뮬레이션 값으로 둔다 | 실제 결제/통신망/live data는 과제 범위를 키우므로 제외하되 UX 설명에는 필요 | MissionManager, 발표 제한사항 | 사용자 지시, `README.md` |
| 2026-05-18 | 주요 Runtime Canvas UI는 한글화한다 | 발표자가 조작 흐름을 직관적으로 설명하기 위함 | UI 문구, 캡처 | 사용자 지시, `orbitshare-playmode.png` |
| 2026-05-18 | 지구본은 외부 고급 지구 에셋 추가 없이 절차적/텍스처 개선으로 보완한다 | 추가 에셋 설치 없이 land patch 제거/텍스처/구름으로 즉시 개선 가능 | `OrbitShareBootstrap.cs`, visual review | 사용자 지시 |
| 2026-05-18 | 자동 검증은 필요할 때 `ValidateMissionFlow`로 재생성한다 | 자동 리포트는 최신 파일이 있을 때만 최신 PASS 근거로 쓴다 | 검증 명령, done criteria | `OrbitShareEditorAutomation.cs` |
| 2026-05-18 | 제출 필수 산출물은 별도 실행 파일/영상/PPT가 아니라 Unity 프로젝트 자체로 본다 | 사용자 결정: “걍 unity project로 줄건데” | roadmap, backlog, 제출 인벤토리 | 사용자 지시 |
| 2026-05-18 | Windows 빌드, MP4/MOV, PPT/PDF, `Submission/` 패키징은 현재 범위에서 제외한다 | 제출 필수 산출물이 아니므로 큰 작업을 만들지 않음 | backlog, done criteria | 사용자 지시 |
| 2026-05-18 | optional 위성 모델/지구 material/SFX와 Git LFS 작업은 진행하지 않는다 | fallback으로 실행 가능하고 과제 범위를 키우지 않음 | asset backlog, C 작업 | 사용자 지시, `ASSET_STORE_IMPORT.md` |
| 2026-05-18 | rootless Xwayland `:0`이 0x0이면 `run_orbitshare.sh`가 임시 rootful Xwayland display를 띄워 실행한다 | 직접 player 실행은 실패했고, rootful Xwayland에서는 display smoke가 동작함 | display 실행 스크립트 | `run_orbitshare.sh`, `build-run-live.log` |
| 2026-05-18 | 에셋/UI 품질 판정은 자동화 스크립트 단독으로 하지 않는다 | 캡처/자동화는 실제 사람이 보는 화면과 다를 수 있으므로 과신 금지 | done criteria, QA 방식, 보고 문구 | 사용자 지시 |
| 2026-05-24 | 튜토리얼/온보딩 기능은 추가하지 않는다 | 사용자가 여러 차례 튜토리얼은 필요 없다고 명확히 제외했고, 깔끔한 단계형 UI를 선호 | UI 플로우, backlog, 추가 기능 제안 | 사용자 지시 |
| 2026-05-25 | 목적지는 7개 기준으로 문서화한다 | 구현이 7개 목적지로 확장됐고 사용자가 이를 최신 기준으로 확정 | README, WORK_LOG, product 문서 | 사용자 지시, `OrbitShareBootstrap.cs`, `UIManager.cs` |
| 2026-05-25 | 실제 Editor GUI 수동 리허설은 더 이상 별도 리스크/게이트로 추적하지 않는다 | 사용자가 해당 항목을 더 이상 신경 쓰지 말라고 명확히 지시 | roadmap, backlog, done criteria, daylog | 사용자 지시 |
| 2026-05-25 | 위성 추천은 정적 `nextWindowMinutes`만 보지 않고 현재 위성/목적지 Transform 기반 route estimate를 반영한다 | SAT-S01 쏠림이 단발성 통신 UX를 약하게 만들고, 실제 위성이 움직이는 장면과 추천이 어긋났기 때문 | `SatelliteData`, `SatelliteManager`, `MissionManager`, `UIManager`, validation | 사용자 지시, `orbitshare-validation.md` |
| 2026-05-25 | 자동 검증 리포트는 7개 목적지 기준으로 재생성해 최신 PASS 근거로 쓴다 | 기존 3개 목적지 기준 리포트와 삭제 상태를 해소 | `OrbitShareEditorAutomation.cs`, `orbitshare-validation.md` | `Result: PASS` |

## 우선 질문

- 현재 사용자가 추가로 결정해야 하는 항목은 없음.
- 제출 기준은 Unity 프로젝트 자체이며, 수동 GUI 리허설은 더 이상 follow-up gate로 두지 않는다.

## B - 사용자 확인 필요

| ID | 질문 | 상태 | 결정/처리 | 관련 기능/검증 |
|---|---|---|---|---|
| B-001 | 제출 필수 산출물 | resolved | Unity 프로젝트 자체 제출. Windows/PPT/MP4/Submission은 제외 | P0 제출 기준 |
| B-002 | GUI/DISPLAY 환경 | resolved | `run_orbitshare.sh`에 rootful Xwayland fallback 적용 | display smoke |
| B-003 | optional asset/SFX/LFS | resolved | 추가하지 않음. 기존 fallback 유지 | asset/fallback |
| B-004 | 실제 Editor GUI 수동 리허설 | resolved | 더 이상 별도 추적하지 않음 | 사용자 지시 2026-05-25 |

## C - 위험/대형 작업

| ID | 작업 | 결정 | 현재 상태 |
|---|---|---|---|
| C-001 | Git LFS 도입 또는 대용량 에셋 이력/추적 정책 변경 | optional/LFS 작업 불필요 | dropped |
| C-002 | Windows Build Support 설치와 Windows 빌드 생성 | Unity 프로젝트 제출이므로 필수 아님 | dropped |
| C-003 | 새 Asset Store 에셋/SFX 대량 추가 | optional 리소스 추가 불필요 | dropped |
| C-004 | `Submission/OrbitShare` 패키징 | 별도 제출 폴더 불필요 | dropped |
| C-005 | Unity 자동 검증/Resource Hook/build 병렬 실행 | Unity project lock/log 충돌 위험 때문에 계속 금지 | policy |
| C-006 | 실제 Editor GUI 수동 E2E 리허설 | 사용자 지시로 추적 중단 | dropped |

## 진전 없음 / 중단

| 항목 | 최신 상태 | 해소/처리 |
|---|---|---|
| 자동 검증 리포트 | `orbitshare-validation.md` 재생성 완료 | 7개 목적지 기준 `Result: PASS`로 최신 근거 확보 |
| GUI/display player smoke | 해소 | `run_orbitshare.sh` fallback으로 display smoke 근거 확보 |
| Windows 빌드/MP4/PPT/Submission | 중단 | 제출 필수 아님으로 제외 |
| optional asset/SFX/LFS | 중단 | fallback 유지로 제외 |
| 실제 Editor GUI 수동 리허설 | 중단 | 사용자 지시로 더 이상 신경 쓰지 않음 |

## 나중에 재검토할 가정

- 과제 제출 기준이 바뀌어 실행 파일, 영상, PPT가 요구되면 backlog에서 다시 활성화한다.
- 실제 가격/성공률/위성 가용성 정확도가 중요해지면 현재 시뮬레이션 데이터 대신 별도 데이터 모델이나 실제 궤도 데이터를 설계해야 한다.
- 코드 변경 후 자동 검증 리포트를 최신 근거로 쓰려면 `orbitshare-validation.md`를 다시 재생성한다.

## 문서 충돌

| 충돌 | 최신 해석 | 근거 |
|---|---|---|
| `orbitshare_shared_satellite_agent.md` 하단 legacy 관측/예약 플랫폼 vs 최신 단문 발신 컨셉 | product/발표 기준은 상단 최신 섹션과 README의 단발성 단문 발신 | `orbitshare_shared_satellite_agent.md`, `README.md` |
| 과거 문서의 3개 목적지 기준 vs 현재 구현 | 현재 기준은 7개 목적지다: Seoul, New York, London, Tokyo, Paris, Sydney, Los Angeles | 사용자 지시, `OrbitShareBootstrap.cs` |
| 과거 문서의 수동 GUI 리허설 todo | 더 이상 별도 추적하지 않는다 | 사용자 지시 2026-05-25 |
| `Fast Demo`, `Quick Relay`, `빠른 발신` 용어 혼재 | 자동 검증 내부 용어는 Fast Demo, 최신 UI/발표 용어는 시연 전송/단문 발신 흐름 | `README.md`, `UIManager.cs` |
| 자동 검증 PASS 표현 | 최신 코드 변경 후 재검증 전에는 이전 PASS를 최신으로 단정하지 않는다 | `orbitshare-validation.md`, `unity-validation.log` |

## 과대해석 방지 보고

- 완료로 쓰지 않은 항목:
  - 스크립트/자동 캡처 결과만으로 에셋/UI 품질 최종 합격 처리하지 않음.
  - Unity 프로젝트 제출 결정을 Windows/MP4/PPT 완료로 쓰지 않음.
  - 가격/성공 확률을 실제 결제/통신 결과로 표현하지 않음.
  - legacy 관측/예약 플랫폼 원문을 현재 제품 완료 범위로 쓰지 않음.
  - optional missing asset/SFX를 실패 또는 완료로 과대해석하지 않음.
  - 코드 변경 후 재검증 전 이전 `orbitshare-validation.md`를 최신 PASS 근거로 쓰지 않음.
- 다음 실행 후보:
  - 문서/제출 인벤토리 정리, 코드 변경 시 자동 검증 리포트 재생성.
