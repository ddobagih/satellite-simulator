# Product Vision

## 한 줄 컨셉

- OrbitShare는 서울 고정 발신자가 7개 목적지 중 하나와 발신 시각, 메시지/첨부를 정하고, 24개 후보 위성 중 조건에 맞는 위성을 골라 짧은 메시지를 1회 발신하는 Unity 3D 위성 통신 릴레이 시뮬레이터다.

## 문제 / 기회

- 단순 인공위성 궤도 시뮬레이션이 아니라 “언제, 어느 지역으로, 얼마나 큰 단문을, 어느 위성으로, 얼마에, 성공 가능성은 어느 정도로 보내는가”를 보여줘야 한다.
- 과제 제출 전 구현 기능, 검증 리포트, 빌드/시연 산출물의 상태를 과대해석 없이 구분해야 한다.
- 제출 기준은 Unity 프로젝트 자체이며, 실제 Editor GUI 수동 리허설은 더 이상 별도 리스크/게이트로 추적하지 않는다.

## 대상 사용자

| 사용자 | 상황 | 핵심 니즈 | 근거 |
|---|---|---|---|
| 과제 발표자 | Unity 데모를 짧은 시간 안에 설명해야 함 | 목적지/발신 시각/용량/위성 선택/결과까지 이어지는 안정적 흐름 | `README.md`, `WORK_LOG.md` |
| 평가자/청중 | 위성 통신 서비스 컨셉과 구현 범위를 빠르게 이해해야 함 | 7개 목적지, 24개 후보 위성, 필터, 가격/성공 예측, 수신 완료 상태의 명확한 시각화 | `README.md`, `orbitshare-playmode.png` |
| 후속 작업자 | 제출 전 검증·빌드·자료 상태를 이어받아야 함 | 기존 PASS와 현재 리포트/빌드/캡처 상태 분리 | `WORK_LOG.md`, `daylog/2026-05-25.md` |

## 내가 원하는 최종 모습

- [x] 목적지는 7개 기준으로 제공한다. 근거: `OrbitShareBootstrap.cs`, `UIManager.cs`
- [x] 후보 위성 목록에 24개 위성이 들어가고 전체/가능/빠른창/저가 필터와 현재 위치 기반 추천으로 볼 수 있다. 근거: `SatelliteManager.cs`, `UIManager.cs`
- [x] 발신 시각, 메시지/첨부 용량, 예상 가격, 성공 예측, 수신 완료 결과가 보인다. 근거: `MissionManager.cs`, `UIManager.cs`
- [x] 대기 메시지, 전송 기록, 실패 재시도, 경로 재생 흐름이 있다. 근거: `MissionManager.cs`, `UIManager.cs`
- [x] 주요 Runtime Canvas UI가 단문 발신 컨셉에 맞게 한글화되어 있다. 근거: `orbitshare-playmode.png`
- [x] 지구본은 외부 텍스처/shader와 절차적 fallback을 함께 사용한다. 근거: `OrbitShareBootstrap.cs`, `Assets/Resources/OrbitShare/Earth/`
- [x] 최신 자동 검증 리포트 `orbitshare-validation.md`가 PASS로 재생성되어 있다. 근거: `orbitshare-validation.md`, `unity-validation.log`

## 핵심 사용 흐름

1. Unity Hub에서 프로젝트를 열고 `Assets/Scenes/MainScene.unity`를 Play한다.
2. 화면에 지구, 서울 발신자, 7개 목적지, 24개 후보 위성망이 표시된다.
3. 발표자는 발신 시각, 메시지/첨부, 목적지를 선택한다.
4. 위성 목록에서 전체/가능/빠른창/저가 필터를 사용해 후보를 좁힌다.
5. `위성 선택 완료` 후 `메시지 보내기`로 전송한다.
6. relay lock, uplink/relay/downlink, delivery 진행률, 수신 완료 패널과 전송 기록을 설명한다.

## 차별점 / 재미 요소 / 중요한 경험

- “위성이 도는 장면”보다 “단발성 통신을 위해 위성을 찾고 짧은 메시지를 보내는 서비스 UX”를 강조한다.
- 7개 목적지와 24개 후보 위성을 통해 목적지와 현재 위성 위치에 따라 추천 위성이 달라지는 느낌을 준다.
- 가격과 성공 확률은 실제 결제가 아니라 서비스 의사결정 UI를 설명하기 위한 시뮬레이션 지표로 둔다.
- Asset Store skybox/station hook, 외부 지구 텍스처, 절차적 fallback을 함께 둬 실행 안정성을 유지한다.

## 하지 않을 것 / 당분간 보류

- 실제 TLE, NASA/SpaceX live data, 케플러 방정식, 정밀 궤도역학은 현재 범위가 아니다.
- 온라인 서버/DB, 실제 결제, 실시간 멀티유저, 실제 지도 API는 구현하지 않는다.
- `orbitshare_shared_satellite_agent.md` 하단의 개인/기업/공유 위성 관측·예약 플랫폼 원문은 legacy 기획으로 보존하되, 발표 기준은 최신 단문 발신 컨셉으로 둔다.
- 현재 제출 기준에서는 `Submission/OrbitShare` 생성, Windows 빌드, MP4/PPT 생성, optional 에셋/LFS 작업을 진행하지 않는다.
- 실제 Editor GUI 수동 리허설은 사용자 지시로 더 이상 별도 추적하지 않는다.

## 제약과 전제

- 현재 fee/success/window는 발표용 시뮬레이션 데이터이며 실제 위성 운영 데이터가 아니다.
- 현재 `orbitshare-validation.md`는 2026-05-25 기준 PASS로 재생성되어 있다.
- UI/에셋 품질은 자동화 스크립트 단독으로 합격 처리하지 않고, Play 화면 캡처와 정적 근거를 함께 기록한다.
- `DISPLAY=:0`이 rootless Xwayland 0x0이면 `run_orbitshare.sh`가 1280x720 rootful Xwayland fallback으로 display smoke를 수행한다.

## 근거 문서

| 근거 파일 | 반영한 내용 | 신뢰도 |
|---|---|---|
| `README.md` | 실행 방법, 최신 Demo Flow, 구현 기능, 제한사항 | 높음 |
| `WORK_LOG.md` | 구현 요약, 제출 상태, 검증/빌드 근거 | 높음 |
| `orbitshare-playmode.png` | 최신 Play Mode 화면 판독 근거 | 중간 |
| `ASSET_STORE_IMPORT.md` | Asset Store hook과 fallback 정책 | 높음 |
| `asset-store-import-report.md` | 현재 리소스 FOUND/MISSING 상태 | 높음 |
| `orbitshare_shared_satellite_agent.md` | 최신 컨셉과 legacy 기획 충돌 주의 | 중간 |
| `daylog/2026-05-25.md` | 최신 실행/변경/검증 기록 | 높음 |
