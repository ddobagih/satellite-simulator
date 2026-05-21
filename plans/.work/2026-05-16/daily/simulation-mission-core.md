# Computer Graphics Assignment 1 - Simulation/Mission Core lane note (2026-05-17)

## 최근 진행 근거
- 사용자 제공 AGENTS 지침, 2026-05-16 KST: 저장소 내부 `AGENTS.md`는 확인되지 않아 프롬프트 지침을 적용.
- `README.md`, 확인일 2026-05-16: 서울 고정 발신자, 3개 목적지, 24개 위성, `Quick Relay`, 시간 배속, 자동 검증 흐름이 문서화됨.
- `WORK_LOG.md`, 최종 점검 2026-05-16: 릴레이 미션 흐름, `GroundStation`/`ConnectionLine`, 자동 검증 PASS, Linux 빌드 성공 기록 확인.
- `daylog/2026-05-15.md`, 2026-05-15: 컨셉을 서울 발신 1회성 통신 릴레이로 전환, 기본 목적지 `New York`, 24개 위성, 자동 검증 PASS, Linux 빌드 성공 기록.
- `daylog/2026-05-16.md`, 2026-05-16: Reset 2회차 검증, 1600x900 Canvas 기준, PresentationDirector 카메라 컷 검사가 자동화 코드에 추가됐으나 Unity LicensingClient IPC 문제로 새 검증/빌드는 재실행 실패.
- `plans/.work/2026-05-16/{catchup,execute-r1}/simulation-mission-core.md`, 2026-05-16: core 코드 정적 점검은 완료, GUI Fast Demo/Reset/Play 재진입/화면 기준 `ConnectionLine` 확인은 미완료로 정리됨.
- `orbitshare-validation.md`, 확인일 2026-05-16: 기존 리포트 기준 `Result: PASS`, Fast demo complete, Relay lock 100%, Delivery 100%, Runtime Error/Exception 0. 단, r1에서 추가된 새 검증 항목은 아직 리포트에 반영되지 않음.
- 코드 확인, 2026-05-16: `SatelliteOrbit`은 `SimulationManager.ScaledDeltaTime` 사용, `MissionManager`는 `Idle → Reserved → Running → Observing → ObservationComplete → Transmitting → Complete` 흐름과 `RunFastDemo()`/`ResetMission()` 포함, `GroundStation` 기본 이름은 `Seoul Gateway`, `ConnectionLine`은 표시/숨김과 위치 갱신 처리.
- `plans/daily/2026-05-17.md`, 확인일 2026-05-16: 파일 없음.

## 내일 목표 후보
- 1순위: Unity LicensingClient 접근 가능한 환경에서 `OrbitShareEditorAutomation.ValidateMissionFlow`를 재실행해 최신 자동 검증 리포트를 갱신.
- 2순위: Unity GUI에서 `Quick Relay` 또는 `Queue → Relay` 흐름을 직접 리허설해 Mission Complete까지 화면 기준으로 확인.
- 3순위: `Pause/1x/5x/10x`가 위성 궤도 이동, relay lock, delivery 진행률에 일관되게 반영되는지 확인.
- 4순위: 전송 중 `GroundStation` 통신 가능 상태와 `ConnectionLine` 표시, 완료 후 숨김을 화면 기준으로 확인.
- 5순위: Reset 후 재실행과 Play 재진입에서 stale state, 중복 이벤트, Runtime Error/Exception이 없는지 확인.
- 6순위: core 버그가 실제 재현될 때만 `SatelliteOrbit`, `MissionManager`, `SimulationManager`, `GroundStation`, `ConnectionLine`, `OrbitShareBootstrap` 범위에서 최소 수정.

## 상세 체크리스트 초안
- [ ] 최신 자동 검증 재실행 → 검증: `orbitshare-validation.md`에 `Result: PASS`, Runtime Error/Exception 0, Reset replay 2회차 완료, 1600x900 Canvas, PresentationDirector camera cut 항목이 반영되는지 확인.
- [ ] Fast Demo GUI smoke → 검증: Play Mode에서 `Quick Relay` 실행 후 SAT-S01이 relay lock, delivery, Mission Complete까지 도달하는지 확인.
- [ ] 수동 미션 흐름 확인 → 검증: 목적지 선택, `Queue`, `Relay`, relay lock, gateway sync, delivery, completion summary가 끊기지 않는지 확인.
- [ ] 시간 배속 확인 → 검증: `Pause/1x/5x/10x`에서 `SatelliteOrbit` 이동과 `MissionManager` 진행률이 정지/가속으로 보이는지 기록.
- [ ] 지상국/통신선 확인 → 검증: 전송 가능 구간에서 `ConnectionLine`이 보이고, 완료 또는 Reset 후 숨겨지는지 확인.
- [ ] Reset/Play 재진입 안정성 확인 → 검증: 완료 후 Reset 및 Play 재진입 뒤 Fast Demo를 다시 완료하고 Runtime Error/Exception 0 확인.
- [ ] 결과 기록 전달 → 검증: 실행한 검증과 미실행 항목을 daylog 또는 merge 계획에 실제 수행 기준으로 기록.

## 리스크/확인 필요
- 현재 유효한 PASS 근거는 기존 `orbitshare-validation.md`이며, r1 자동화 보강 후 새 리포트는 LicensingClient IPC 문제로 아직 생성되지 않음.
- GUI 리허설, `ConnectionLine` 화면 표시, 시간 배속 화면 동작은 자동 검증만으로 충분히 증명되지 않음.
- `unity-validation.log`와 `unity-build-r1.log`에 `IPC channel to LicensingClient doesn't exist; aborting` 실패가 있어 실행 환경 확인이 먼저 필요함.
- Linux headless smoke는 FMOD/shader 오류가 있어 GUI smoke PASS 근거로 사용하면 안 됨.
- 실제 궤도 역학/TLE/지도 API는 README상 제한사항으로 분리되어 있음. 과제 요구사항에 필수 문구가 있으면 core 범위가 바뀔 수 있음.
- `daylog/2026-05-16.md`가 현재 수정 상태로 보이므로 merge 시 최신 본문 기준 확인 필요.

## 병렬 에이전트 활용 메모
- 사용하지 않음.
- 담당 범위가 문서, 최근 daylog, 기존 lane note, 핵심 5개 스크립트, 검증 로그 확인으로 제한되어 직접 탐색으로 충분했다.