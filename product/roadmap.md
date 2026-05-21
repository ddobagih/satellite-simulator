# Product Roadmap

## 운영 모드

- 기본 모드: `concept-update/submission-readiness`
- 이유: 사용자가 단순 위성 시뮬레이션이 아니라 단발성 단문 발신 서비스 UX를 원한다고 정정했고, 핵심 기능은 최신 자동 검증 PASS까지 확보했다. 남은 일은 실제 Editor 수동 리허설과 제출 직전 정적 확인이다.
- 권장 비율:
  - 신규 기능: 35%
  - 검증/안정화: 45%
  - 유지보수/문서: 20%

## 단계별 목표

| 단계 | 목표 | 대표 기능 | 완료 기준 | 상태 |
|---|---|---|---|---|
| M0 기준선 | 기존 검증/산출물 상태 분리 | README, WORK_LOG, validation, product 문서 | 완료/미검증/제외 항목이 분리됨 | done |
| M1 핵심 경험 | 단발성 메시지 발신 UX로 전환 | 한글 UI, 발신 시각, 메시지 용량, 위성 확정/발신 | 캡처에서 주요 흐름 판독 가능 | done_current |
| M2 후보 위성 UX | 24개 위성 목록과 조건 필터 제공 | 전체/가능/빠른창/저가, 가격/성공 예측 | validation 24개 + 화면/코드 확인 | done_current |
| M3 시각 품질 보완 | 지구본 조잡함 완화 | land patch 제거, 고해상도 텍스처, 구름 레이어 | 최신 캡처에서 돌출 패치 제거 확인 | done_current |
| M4 검증 안정화 | 기능 회귀 방지 | `ValidateMissionFlow`, display smoke | Runtime Error/Exception 0, player 시작 확인 | done |
| M5 제출 준비 | Unity 프로젝트 자체 제출 준비 | 제출 인벤토리, 불필요 산출물 제외 | Windows/MP4/PPT/Submission 제외가 문서화됨 | done |
| M6 최종 리허설 | 실제 제출/발표 직전 사람이 한 번 더 확인 | Unity Editor Play Mode 수동 조작 | `빠른 발신`, `위성 확정 → 메시지 발신`, Reset 확인 | todo |

## 이번 주 우선순위

1. Unity 프로젝트 제출 기준으로 불필요한 Windows/PPT/MP4/Submission 작업을 되살리지 않는다.
2. 실제 Unity Editor에서 Play Mode 수동 리허설을 1회 수행한다.
3. 제출 직전 `Assets/`, `Packages/`, `ProjectSettings/`, README/WORK_LOG/product/daylog 포함 여부를 정적 확인한다.

## 다음 기능 후보

| 우선순위 | 기능 | 왜 지금 필요한가 | 예상 범위 | 상태 |
|---|---|---|---|---|
| P0 | 단문 발신 컨셉 반영 | 사용자 피드백 핵심 | UI/문서/데이터 모델 | done_current |
| P0 | 24개 후보 위성/필터 | 릴레이 목록이 3개만 보이는 문제 해결 | `SatelliteManager`, `UIManager` | done_current |
| P0 | 지구본 품질 보완 | “찰흙” 같은 visual issue 개선 | `OrbitShareBootstrap`, 캡처 판독 | done_current |
| P0 | 최신 자동 검증 재실행 | 변경 후 회귀 확인 | `OrbitShareEditorAutomation.ValidateMissionFlow` | done |
| P1 | Unity Editor Play Mode 리허설 | batch/player smoke만으로는 수동 발표 흐름을 완전히 증명하지 못함 | `빠른 발신`, `위성 확정 → 메시지 발신`, Reset | todo |
| P1 | Asset/fallback 화면 QA | optional 리소스 fallback 표시를 눈으로 확인 | Play Mode 화면 확인 | todo |

## 검증/안정화 후보

| 우선순위 | 검증 항목 | 필요한 환경 | 현재 상태 | 대체 가능 여부 |
|---|---|---|---|---|
| P0 | 최신 validation 실행 | Unity LicensingClient, batch 실행 가능 환경 | PASS | 완료 |
| P0 | 최신 Play screenshot 캡처/판독 | Unity batch capture + 사람 판독 | done_current | 수동 GUI E2E와는 별개 |
| P0 | GUI/display player smoke | Wayland 또는 X11 display | PASS: rootful Xwayland fallback | 수동 GUI E2E와는 별개 |
| P1 | Unity Editor Play Mode 리허설 | 실제 Editor GUI | todo | 자동 검증으로 일부 대체 가능하나 최종 수동 확인 권장 |
| P1 | Linux 최신 빌드 | Unity batch build | deferred | Unity 프로젝트 제출이라 필수 아님 |
| P2 | Windows 빌드/MP4/PPT/Submission | 제출 요구 변경 시 | dropped | 현재 불필요 |

## B/C/중단 처리 정책

- 현재 unresolved B는 없다.
- 실제 가격/성공률/위성 가용성이 필요해지면 별도 데이터/궤도 설계가 필요하지만 현재 과제 범위에서는 시뮬레이션 값으로 둔다.
- Git LFS, Windows build, 대량 에셋 추가, 별도 제출 폴더 생성은 현재 제출 기준에서 dropped다.
- Unity 실행은 project lock과 로그 충돌 때문에 병렬 실행하지 않는다.

## 근거 문서

| 근거 파일 | 반영한 내용 |
|---|---|
| `README.md` | 최신 데모 흐름과 구현 기능 |
| `WORK_LOG.md` | 구현/검증/제출 상태 |
| `orbitshare-validation.md` | 최신 자동 검증 PASS |
| `orbitshare-playmode.png` | 최신 화면 판독 근거 |
| `run_orbitshare.sh` | GUI/display fallback 적용 |
| `daylog/2026-05-18.md` | 최신 실행/변경 내역 |
