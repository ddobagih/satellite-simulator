# Product Roadmap

## 운영 모드

- 기본 모드: `docs-sync/submission-readiness`
- 이유: 단발성 단문 발신 서비스 UX, 7개 목적지, 24개 후보 위성, 동적 추천, 대기/기록/재시도 흐름이 구현된 상태다. 지금은 제출 인벤토리와 남은 선택 산출물 정리가 우선이다.
- 실제 Editor GUI 수동 리허설은 사용자 지시로 더 이상 별도 추적하지 않는다.

## 단계별 목표

| 단계 | 목표 | 대표 기능 | 완료 기준 | 상태 |
|---|---|---|---|---|
| M0 기준선 | 기존 검증/산출물 상태 분리 | README, WORK_LOG, validation, product 문서 | 완료/미검증/제외 항목이 분리됨 | done |
| M1 핵심 경험 | 단발성 메시지 발신 UX로 전환 | 한글 UI, 발신 시각, 메시지/첨부, 위성 확정/발신 | 캡처에서 주요 흐름 판독 가능 | done_current |
| M2 후보 위성 UX | 24개 위성 목록과 조건 필터 제공 | 전체/가능/빠른창/저가, 가격/성공 예측 | 화면/코드 확인 | done_current |
| M3 목적지 확장 | 7개 목적지 선택 제공 | Seoul/New York/London/Tokyo/Paris/Sydney/LA | 코드/문서 기준 7개 목적지 확인 | done |
| M4 시각 품질 보완 | 지구본 조잡함 완화 | land patch 제거, 지구 텍스처/shader, 구름 레이어 | 최신 캡처에서 돌출 패치 제거 확인 | done_current |
| M5 검증 안정화 | 기능 회귀 방지 | build, display smoke, `ValidateMissionFlow` | 최신 자동 검증 PASS와 로그 상태 분리 | done_current |
| M6 제출 준비 | Unity 프로젝트 자체 제출 준비 | 제출 인벤토리, 불필요 산출물 제외 | Windows/MP4/PPT/Submission 제외가 문서화됨 | done |

## 이번 주 우선순위

1. Unity 프로젝트 제출 기준으로 불필요한 Windows/PPT/MP4/Submission 작업을 되살리지 않는다.
2. 7개 목적지 기준으로 README/WORK_LOG/product 문서 정합성을 유지한다.
3. `orbitshare-validation.md`는 최신 PASS로 재생성되어 있으며, 이후 코드 변경 시 다시 검증한다.
4. 제출 직전 `Assets/`, `Packages/`, `ProjectSettings/`, README/WORK_LOG/product/daylog 포함 여부를 정적 확인한다.

## 다음 기능 후보

| 우선순위 | 기능 | 왜 지금 필요한가 | 예상 범위 | 상태 |
|---|---|---|---|---|
| P0 | 단문 발신 컨셉 반영 | 사용자 피드백 핵심 | UI/문서/데이터 모델 | done_current |
| P0 | 7개 목적지 | 최신 구현/사용자 결정 반영 | `OrbitShareBootstrap`, `UIManager`, 문서 | done |
| P0 | 24개 후보 위성/필터 | 위성 후보 비교 UX | `SatelliteManager`, `UIManager` | done_current |
| P0 | 지구본 품질 보완 | “찰흙” 같은 visual issue 개선 | `OrbitShareBootstrap`, 캡처 판독 | done_current |
| P1 | 자동 검증 리포트 재생성 | 최신 자동 검증 근거 확보 | `OrbitShareEditorAutomation.ValidateMissionFlow` | done |
| P1 | 추천 다양성 개선 | 7개 목적지에서도 추천 위성이 한쪽으로 쏠리는 문제 완화 | `SatelliteData`, `SatelliteManager` | done |

## 검증/안정화 후보

| 우선순위 | 검증 항목 | 필요한 환경 | 현재 상태 | 대체 가능 여부 |
|---|---|---|---|---|
| P0 | 최신 Play screenshot 캡처/판독 | Unity batch capture + 사람 판독 | done_current | 정적 문서 근거와 병행 |
| P0 | GUI/display player smoke | Wayland 또는 X11 display | PASS: rootful Xwayland fallback | 앱 시작 근거 |
| P0 | Linux 최신 빌드 | Unity batch build | PASS | Unity 프로젝트 제출이라 필수는 아님 |
| P1 | 최신 validation 실행 | Unity LicensingClient, batch 실행 가능 환경 | PASS | 변경 후 재검증 필요 |
| P2 | 실제 Editor GUI 수동 리허설 | 실제 Editor GUI | dropped | 사용자 지시로 추적하지 않음 |
| P2 | Windows 빌드/MP4/PPT/Submission | 제출 요구 변경 시 | dropped | 현재 불필요 |

## B/C/중단 처리 정책

- 현재 unresolved B는 없다.
- 실제 가격/성공률/위성 가용성이 필요해지면 별도 데이터/궤도 설계가 필요하지만 현재 과제 범위에서는 시뮬레이션 값으로 둔다.
- Git LFS, Windows build, 대량 에셋 추가, 별도 제출 폴더 생성은 현재 제출 기준에서 dropped다.
- 실제 Editor GUI 수동 리허설은 사용자 지시로 dropped다.
- Unity 실행은 project lock과 로그 충돌 때문에 병렬 실행하지 않는다.

## 근거 문서

| 근거 파일 | 반영한 내용 |
|---|---|
| `README.md` | 최신 데모 흐름과 구현 기능 |
| `WORK_LOG.md` | 구현/검증/제출 상태 |
| `product/decisions.md` | 7개 목적지, 수동 GUI 리허설 중단 결정 |
| `orbitshare-playmode.png` | 최신 화면 판독 근거 |
| `run_orbitshare.sh` | display fallback 적용 |
| `unity-build-transfer-queue-density.log` | 최신 Linux build success |
| `daylog/2026-05-25.md` | 최신 실행/변경 내역 |
