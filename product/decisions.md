# Product Decisions

## 결정된 사항

| 날짜 | 결정 | 이유 | 영향 범위 | 근거 |
|---|---|---|---|---|
| 2026-05-18 | 현재 발표 기준 컨셉은 “서울 발신 단발성 위성 단문 발신”으로 둔다 | 사용자가 단순 위성 시뮬레이션이 아니라 원하는 지역으로 짧은 메시지를 보내는 과정이라고 명확히 정정 | vision, README, UI 문구, 발표 Q&A | 사용자 지시, `README.md` |
| 2026-05-18 | 후보 위성 데이터와 목록은 24개로 둔다 | 릴레이 목록이 3개만 보이는 느낌을 해소하고, 발신 시각/목적지 조건에 따른 선택 UX를 제공 | `SatelliteManager`, `UIManager`, done criteria | 사용자 지시, `orbitshare-validation.md` |
| 2026-05-18 | 위성 목록 필터는 전체/가능/빠른창/저가로 제공한다 | 사용자가 “언제 정보를 보내냐에 따라 해당 위성이 되는지”를 판단해야 한다고 지적 | 후보 위성 UX | 사용자 지시, `UIManager.cs` |
| 2026-05-18 | 가격/성공 확률/발신 가능 창은 발표용 시뮬레이션 값으로 둔다 | 실제 결제/통신망/live data는 과제 범위를 키우므로 제외하되 UX 설명에는 필요 | MissionManager, 발표 제한사항 | 사용자 지시, `README.md` |
| 2026-05-18 | 주요 Runtime Canvas UI는 한글화한다 | 발표자가 조작 흐름을 직관적으로 설명하기 위함 | UI 문구, 캡처 | 사용자 지시, `orbitshare-playmode.png` |
| 2026-05-18 | 지구본은 외부 고급 지구 에셋 추가 없이 절차적 개선으로 보완한다 | 사용자 지적은 품질 문제였고, 추가 에셋 설치 없이 land patch 제거/텍스처/구름으로 즉시 개선 가능 | `OrbitShareBootstrap.cs`, visual review | 사용자 지시 |
| 2026-05-18 | 최신 자동 검증은 `-quit` 없이 실행한다 | `-quit` 포함 실행은 exit 0이어도 report가 갱신되지 않았고, `-quit` 제거 후 report 저장 확인 | 검증 명령, done criteria | `unity-validation-product.log`, `orbitshare-validation.md` |
| 2026-05-18 | 제출 필수 산출물은 별도 실행 파일/영상/PPT가 아니라 Unity 프로젝트 자체로 본다 | 사용자 결정: “걍 unity project로 줄건데” | roadmap, backlog, 제출 인벤토리 | 사용자 지시 |
| 2026-05-18 | Windows 빌드, MP4/MOV, PPT/PDF, `Submission/` 패키징은 현재 범위에서 제외한다 | 제출 필수 산출물이 아니므로 큰 작업을 만들지 않음 | backlog, done criteria | 사용자 지시 |
| 2026-05-18 | optional 위성 모델/지구 material/SFX와 Git LFS 작업은 진행하지 않는다 | 사용자 결정: “3번은 굳이?”이며 fallback으로 실행 가능 | asset backlog, C 작업 | 사용자 지시, `ASSET_STORE_IMPORT.md` |
| 2026-05-18 | rootless Xwayland `:0`이 0x0이면 `run_orbitshare.sh`가 임시 rootful Xwayland display를 띄워 실행한다 | 직접 player 실행은 `XFree86-VidModeExtension` BadValue로 실패했고, 1280x720 rootful Xwayland에서는 display smoke가 동작함 | GUI/display 실행 스크립트 | `run_orbitshare.sh`, `build-run-live.log` |
| 2026-05-18 | 에셋/UI 품질 판정은 자동화 스크립트 단독으로 하지 않는다 | 캡처/자동화는 실제 사람이 보는 GUI와 다를 수 있으므로 과신 금지 | done criteria, QA 방식, 보고 문구 | 사용자 지시 |

## 우선 질문

- 현재 사용자가 추가로 결정해야 하는 항목은 없음.
- 남은 것은 실제 제출 전 Editor GUI 수동 리허설이며, 새 산출물 형식이나 대용량 에셋 정책은 추가 요구가 있을 때만 재검토한다.

## B - 사용자 확인 필요

| ID | 질문 | 상태 | 결정/처리 | 관련 기능/검증 |
|---|---|---|---|---|
| B-001 | 제출 필수 산출물 | resolved | Unity 프로젝트 자체 제출. Windows/PPT/MP4/Submission은 제외 | P0-005 |
| B-002 | GUI/DISPLAY 환경 | resolved | `:0` 직접 player 실패를 확인했고 `run_orbitshare.sh`에 rootful Xwayland fallback 적용 | P0-002, P0-004 |
| B-003 | optional asset/SFX/LFS | resolved | 추가하지 않음. 기존 fallback 유지 | P1-003, P2 |

## C - 위험/대형 작업

| ID | 작업 | 결정 | 현재 상태 |
|---|---|---|---|
| C-001 | Git LFS 도입 또는 대용량 에셋 이력/추적 정책 변경 | optional/LFS 작업 불필요 | dropped |
| C-002 | Windows Build Support 설치와 Windows 빌드 생성 | Unity 프로젝트 제출이므로 필수 아님 | dropped |
| C-003 | 새 Asset Store 에셋/SFX 대량 추가 | optional 리소스 추가 불필요 | dropped |
| C-004 | `Submission/OrbitShare` 패키징 | 별도 제출 폴더 불필요 | dropped |
| C-005 | Unity 자동 검증/GUI/Resource Hook/build 병렬 실행 | Unity project lock/log 충돌 위험 때문에 계속 금지 | policy |

## 진전 없음 / 중단

| 항목 | 최신 상태 | 해소/처리 |
|---|---|---|
| 최신 Unity 자동 검증 | 해소 | LicensingClient 연결 후 `orbitshare-validation.md` 갱신, `Result: PASS` |
| GUI/display player smoke | 해소 | rootless `:0` 직접 실행 실패를 `run_orbitshare.sh` fallback으로 우회 |
| Windows 빌드/MP4/PPT/Submission | 중단 | 제출 필수 아님으로 제외 |
| optional asset/SFX/LFS | 중단 | fallback 유지로 제외 |

## 나중에 재검토할 가정

- 과제 제출 기준이 바뀌어 실행 파일, 영상, PPT가 요구되면 backlog에서 다시 활성화한다.
- 실제 평가 환경에서 Unity Editor Play Mode 수동 조작은 제출 직전 한 번 더 확인하는 것이 안전하다.
- 에셋/UI 판정은 스크립트 자동 판정보다 실제 Play 화면 캡처와 사람 판독을 우선한다. 자동화는 로그 수집/보조 근거로만 사용한다.
- 실제 가격/성공률/위성 가용성 정확도가 중요해지면 현재 시뮬레이션 데이터 대신 별도 데이터 모델이나 실제 궤도 데이터를 설계해야 한다.

## 문서 충돌

| 충돌 | 최신 해석 | 근거 |
|---|---|---|
| `orbitshare_shared_satellite_agent.md` 하단 legacy 관측/예약 플랫폼 vs 최신 단문 발신 컨셉 | product/발표 기준은 상단 최신 섹션과 README의 단발성 단문 발신 | `orbitshare_shared_satellite_agent.md`, `README.md` |
| 과거 문서의 3개 주요 릴레이 설명 vs 현재 후보 목록 요구 | 현재 구현은 24개 위성을 후보 데이터와 목록/필터에 모두 노출한다. 실제 궤도역학은 단순화 모델이다 | 사용자 지시, `orbitshare-validation.md` |
| `Fast Demo`, `Quick Relay`, `빠른 발신` 용어 혼재 | 자동 검증 내부 용어는 Fast Demo, 최신 UI/발표 용어는 `빠른 발신` | `README.md`, `UIManager.cs` |
| 기존 Linux build Success vs 최신 코드 검증 | 최신 자동 검증은 PASS지만 Linux 재빌드는 별도 수행하지 않음 | `unity-validation-product.log`, `unity-build.log` |

## 과대해석 방지 보고

- 완료로 쓰지 않은 항목:
  - `run_orbitshare.sh` timeout smoke를 수동 GUI E2E 완료로 쓰지 않음.
  - 스크립트/자동 캡처 결과만으로 에셋 배치와 UI 품질을 최종 합격 처리하지 않음.
  - 기존 Linux 빌드 성공을 최신 코드 재빌드 성공으로 쓰지 않음.
  - Unity 프로젝트 제출 결정을 Windows/MP4/PPT 완료로 쓰지 않음.
  - 가격/성공 확률을 실제 결제/통신 결과로 표현하지 않음.
  - legacy 관측/예약 플랫폼 원문을 현재 제품 완료 범위로 쓰지 않음.
  - optional missing asset/SFX를 실패 또는 완료로 과대해석하지 않음.
- 다음 자동화가 신규 기능 개발로 착수해도 되는 첫 slice:
  - 신규 기능보다 실제 Unity Editor Play Mode 수동 리허설 또는 제출 직전 정적 인벤토리 확인이 우선이다.
