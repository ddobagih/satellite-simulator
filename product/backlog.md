# Feature Backlog

## 작성 규칙

- 기능은 사용자 가치가 보이는 단위로 적는다.
- 바로 구현할 때는 0.5~1일 안에 끝낼 수 있는 slice로 쪼갠다.
- 기능마다 완료 기준과 검증 방법을 반드시 둔다.
- 근거 없는 기능은 `아이디어/확인 필요`로 둔다.

## P0 - 핵심 기능

| ID | 기능 | 사용자 가치 | 첫 slice | 완료 기준 | 검증 | 상태 | 근거 |
|---|---|---|---|---|---|---|---|
| P0-001 | 단문 발신 컨셉 전환 | 단순 위성 시뮬레이션이 아니라 단발성 통신 서비스를 설명 | UI/README/product 문구 전환 | 목적지/발신 시각/용량/위성 선택/수신 완료 흐름이 보임 | Visual Review + Static | done_current | 사용자 지시, `README.md` |
| P0-002 | 주요 UI 한글화 | 발표 조작 흐름을 직관적으로 이해 | Runtime Canvas 주요 패널/버튼 한글화 | 캡처에서 한글 패널/버튼이 판독됨 | Visual Review | done_current | `orbitshare-playmode.png` |
| P0-003 | 24개 후보 위성 목록 | 3개만 있는 것 같은 문제 해소 | `SAT-R04`~`SAT-R24` 데이터 추가 | validation `Satellite count: 24`, 목록 스크롤 표시 | Unity Batch + Visual Review | done | `orbitshare-validation.md` |
| P0-004 | 위성 필터 | 발신 시각/목적지에 따라 되는 위성 판단 | 전체/가능/빠른창/저가 필터 | 필터 버튼과 조건별 row 표시 | Static + Visual Review | done_current | `SatelliteManager.cs`, `UIManager.cs` |
| P0-005 | 가격/성공/용량/시각 UX | 메시지 발신 서비스 판단 기준 제공 | 메시지 크기·시각 상태와 estimate 계산 | 가격/성공 예측이 status/details에 표시 | Static + Visual Review | done_current | `MissionManager.cs`, `UIManager.cs` |
| P0-006 | 지구본 품질 보완 | 찰흙 같은 지구본 느낌 완화 | land patch 제거, 고해상도 텍스처, 구름 레이어 | 최신 캡처에서 조잡한 돌출 패치가 없음 | Visual Review | done_current | `OrbitShareBootstrap.cs`, `orbitshare-playmode.png` |
| P0-007 | 최신 자동 검증 재실행 | 기능 회귀 방지 | `OrbitShareEditorAutomation.ValidateMissionFlow` 실행 | `Result: PASS`, Runtime Error/Exception 0 | Unity Batch | done | `orbitshare-validation.md` |
| P0-008 | 제출 인벤토리 확정 | 빠진 산출물을 완료로 오해하지 않음 | 사용자 결정 반영 | 제출은 Unity 프로젝트 자체, Windows/PPT/MP4/Submission 제외 | Static/문서 | done | 사용자 지시, `product/decisions.md` |

## P1 - 중요하지만 P0 이후

| ID | 기능 | 사용자 가치 | 첫 slice | 완료 기준 | 검증 | 상태 | 근거 |
|---|---|---|---|---|---|---|---|
| P1-001 | 실제 Unity Editor Play Mode 수동 리허설 | 발표자가 직접 버튼 흐름을 확인 | Unity Editor에서 `빠른 발신`과 `위성 확정 → 메시지 발신` 1회 확인 | Mission Complete, UI 겹침/잘림, Reset/재진입 결과 기록 | GUI E2E | todo | `README.md` |
| P1-002 | 에셋/UI 화면 판독 | 자동화 스크립트 단독 판정으로 놓치는 문제를 줄임 | Play 화면 캡처를 사람이 보고 누락/겹침/잘림/가독성 메모 | 1600x900 캡처 판독 결과와 보완 내역 기록 | Visual Review | done_current | `orbitshare-playmode.png`, `daylog/2026-05-18.md` |
| P1-003 | 최신 Linux 재빌드 | 실행 파일 제출이 다시 필요해질 때 최신 빌드 확보 | 필요 시 `BuildOrbitShare.BuildLinux` 실행 | 최신 로그에 `Build Finished, Result: Success.` | Unity build | deferred | 사용자 지시: Unity 프로젝트 제출 |
| P1-004 | Asset hook 화면 QA | skybox/station hook이 실제 화면에 반영됐는지 확인 | 필요 시 Play Mode 화면 확인 | `SpaceSkybox.mat`, `SpaceStation.prefab` 표시와 optional fallback 상태 기록 | Static+GUI | todo | `ASSET_STORE_IMPORT.md`, `asset-store-import-report.md` |
| P1-005 | 발표 Q&A 최종 정리 | legacy 관측 플랫폼과 최신 단문 발신 컨셉 충돌 방지 | 상단 최신 Q&A만 추출 | 발표 기준 문구와 제한사항이 README/WORK_LOG와 일치 | Static/문서 | todo | `orbitshare_shared_satellite_agent.md` |

## P2 - 제외 / 나중에

| ID | 기능 | 상태 | 이유 |
|---|---|---|---|
| P2-001 | Windows 빌드 | dropped | Unity 프로젝트 자체 제출로 결정 |
| P2-002 | MP4/MOV/PPT/PDF 생성 | dropped | 제출 필수 아님 |
| P2-003 | `Submission/OrbitShare` 패키징 | dropped | 별도 제출 폴더 불필요 |
| P2-004 | SatelliteModel/Earth material/SFX 추가 | dropped | “3번은 굳이?” 결정, fallback 유지 |
| P2-005 | Git LFS 도입 | dropped | optional/대용량 정책 변경 불필요 |
| P2-006 | 실제 궤도/TLE/지도/API 확장 | deferred | 과제 현재 범위 밖, 발표 안정성 저하 가능 |

## 오늘/다음 실행 후보

| 후보 | 유형 | 예상 변경 범위 | 검증 방법 | 막힘 |
|---|---|---|---|---|
| Unity Editor Play Mode 수동 리허설 | 검증/안정화 | 로그/daylog | `빠른 발신`, `위성 확정 → 메시지 발신`, Reset 확인 | 없음: 수동 필요 |
| 에셋/UI 화면 재판독 | 검증/안정화 | 캡처 + 판독 메모 | 자동 스크립트 판정 없이 사람이 캡처를 확인 | 없음 |
| 제출 직전 정적 인벤토리 확인 | 문서/안정화 | daylog 또는 WORK_LOG | Unity 프로젝트 필수 파일 존재와 불필요 산출물 제외 확인 | 없음 |

## 백로그 상태 값

- `todo`: 아직 시작 전
- `done`: 완료 기준과 검증 근거 있음
- `done_current`: 현재 캡처/환경 기준 완료, 다른 해상도/수동 조작은 별도
- `deferred`: 현재 제출 범위상 뒤로 미룸
- `dropped`: 명시적으로 제외
