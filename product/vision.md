# Product Vision

## 한 줄 컨셉

- OrbitShare는 서울 고정 발신자가 목적지, 발신 시각, 메시지 용량을 정하고, 24개 후보 위성 중 조건에 맞는 위성을 골라 짧은 메시지를 1회 발신하는 Unity 3D 위성 통신 릴레이 시뮬레이터다. 근거: `README.md`, `WORK_LOG.md`, `orbitshare_shared_satellite_agent.md`

## 문제 / 기회

- 단순 인공위성 궤도 시뮬레이션이 아니라 “언제, 어느 지역으로, 얼마나 큰 단문을, 어느 위성으로, 얼마에, 성공 가능성은 어느 정도로 보내는가”를 보여줘야 한다.
- 과제 제출 전 구현 기능, 검증 리포트, 빌드/시연 산출물의 상태를 과대해석 없이 구분해야 한다.
- 최신 자동 검증은 PASS이고, 제출 형식은 Unity 프로젝트 자체로 확정했다. 남은 것은 실제 Editor GUI 수동 리허설이다.

## 대상 사용자

| 사용자 | 상황 | 핵심 니즈 | 근거 |
|---|---|---|---|
| 과제 발표자 | Unity 데모를 짧은 시간 안에 설명해야 함 | 목적지/발신 시각/용량/위성 선택/결과까지 이어지는 안정적 흐름 | `README.md`, `WORK_LOG.md` |
| 평가자/청중 | 위성 통신 서비스 컨셉과 구현 범위를 빠르게 이해해야 함 | 24개 후보 위성, 필터, 가격/성공 예측, 수신 완료 상태의 명확한 시각화 | `orbitshare-validation.md`, `orbitshare-playmode.png` |
| 후속 작업자 | 제출 전 검증·빌드·자료 상태를 이어받아야 함 | 기존 PASS와 최신 미검증 항목 분리, Unity 환경 blocker 확인 | `daylog/2026-05-18.md` |

## 내가 원하는 최종 모습

- [x] Unity 자동 검증 리포트가 24개 위성, 기본 목적지 New York, relay lock/delivery 100%, Runtime Error/Exception 0을 포함한다. 근거: `orbitshare-validation.md`
- [x] 주요 Runtime Canvas UI가 단문 발신 컨셉에 맞게 한글화되어 있다. 근거: `orbitshare-playmode.png`
- [x] 후보 위성 목록에 24개 위성이 들어가고 전체/가능/빠른창/저가 필터로 볼 수 있다. 근거: `SatelliteManager.cs`, `UIManager.cs`
- [x] 발신 시각, 메시지 용량, 예상 가격, 성공 예측, 수신 완료 결과가 보인다. 근거: `MissionManager.cs`, `UIManager.cs`
- [x] 지구본은 돌출 land patch를 제거하고 고해상도 절차 텍스처/구름 레이어로 보완했다. 근거: `OrbitShareBootstrap.cs`
- [ ] 실제 Unity Editor GUI에서 사람이 `빠른 발신`, `위성 확정 → 메시지 발신`, Reset을 직접 조작하는 리허설은 제출 전 별도 확인한다.

## 핵심 사용 흐름

1. Unity Hub에서 프로젝트를 열고 `Assets/Scenes/MainScene.unity`를 Play한다.
2. 화면에 지구, 서울 발신자, Seoul/New York/London 목적지, 24개 후보 위성망이 표시된다.
3. 발표자는 목적지, 발신 시각, 메시지 용량을 선택한다.
4. 위성 목록에서 전체/가능/빠른창/저가 필터를 사용해 후보를 좁힌다.
5. `위성 확정` 후 `메시지 발신`을 누르거나, 빠른 시연은 `빠른 발신`을 누른다.
6. relay lock, uplink/relay/downlink, delivery 진행률, 수신 완료 패널을 설명한다.

## 차별점 / 재미 요소 / 중요한 경험

- “위성이 도는 장면”보다 “단발성 통신을 위해 위성을 찾고 짧은 메시지를 보내는 서비스 UX”를 강조한다.
- 24개 후보 위성과 필터를 통해 발신 시각/목적지에 따라 가능한 위성이 달라지는 느낌을 준다.
- 가격과 성공 확률은 실제 결제가 아니라 서비스 의사결정 UI를 설명하기 위한 시뮬레이션 지표로 둔다.
- Asset Store skybox/station hook과 절차적 fallback을 함께 둬 실행 안정성을 유지한다.

## 하지 않을 것 / 당분간 보류

- 실제 TLE, NASA/SpaceX live data, 케플러 방정식, 정밀 궤도역학은 현재 범위가 아니다.
- 온라인 서버/DB, 실제 결제, 실시간 멀티유저, 실제 지도 API는 구현하지 않는다.
- `orbitshare_shared_satellite_agent.md` 하단의 개인/기업/공유 위성 관측·예약 플랫폼 원문은 legacy 기획으로 보존하되, 발표 기준은 상단 최신 단문 발신 컨셉으로 둔다.
- 현재 제출 기준에서는 `Submission/OrbitShare` 생성, Windows 빌드, MP4/PPT 생성, optional 에셋/LFS 작업을 진행하지 않는다.

## 제약과 전제

- 최신 `orbitshare-validation.md`는 PASS이며 `Satellite count: 24`로 갱신됐다.
- UI/에셋 품질은 자동화 스크립트 단독으로 합격 처리하지 않고, Play 화면 캡처를 사람이 판독한 결과와 한계를 함께 기록한다.
- 현재 fee/success/window는 발표용 시뮬레이션 데이터이며 실제 위성 운영 데이터가 아니다.
- `DISPLAY=:0`이 rootless Xwayland 0x0이면 `run_orbitshare.sh`가 1280x720 rootful Xwayland fallback으로 display smoke를 수행한다.

## 근거 문서

| 근거 파일 | 반영한 내용 | 신뢰도 |
|---|---|---|
| `README.md` | 실행 방법, 최신 Demo Flow, 구현 기능, 제한사항 | 높음 |
| `WORK_LOG.md` | 구현 요약, 제출 상태, 검증/빌드 근거 | 높음 |
| `orbitshare-validation.md` | 최신 자동 검증 PASS | 높음 |
| `orbitshare-playmode.png` | 최신 Play Mode 화면 판독 근거 | 중간: 수동 조작 E2E는 아님 |
| `ASSET_STORE_IMPORT.md` | Asset Store hook과 fallback 정책 | 높음 |
| `asset-store-import-report.md` | 현재 리소스 FOUND/MISSING 상태 | 높음 |
| `orbitshare_shared_satellite_agent.md` | 최신 컨셉과 legacy 기획 충돌 주의 | 중간 |
| `daylog/2026-05-18.md` | 최신 실행/변경/검증 기록 | 높음 |
