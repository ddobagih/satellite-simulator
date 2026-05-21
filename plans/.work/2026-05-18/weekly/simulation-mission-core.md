# Computer Graphics Assignment 1 - Simulation/Mission Core weekly lane note (2026-W21)

## 이번 주 목표 후보

- 최신 Unity 실행 환경에서 `OrbitShareEditorAutomation.ValidateMissionFlow`를 재실행해 r1 보강 항목까지 포함된 `orbitshare-validation.md` PASS 근거를 확보한다.
- `Quick Relay`와 수동 `Queue → Relay → 10x` 흐름을 실제 Play Mode에서 Mission Complete까지 확인한다.
- `SatelliteOrbit`의 시간 배속 반영, `MissionManager` phase 전환, relay lock/delivery 진행률이 일관되게 동작하는지 확인한다.
- `GroundStation`/`ConnectionLine`이 전송 가능 구간에서 보이고 Complete/Reset 후 숨겨지는지 화면 기준으로 검증한다.
- Reset 후 2회차 실행과 Play Mode 재진입에서 stale state, 중복 이벤트, Runtime Error/Exception이 없는지 확인한다.
- 실제 실패가 재현될 때만 `SatelliteOrbit`, `MissionManager`, `SimulationManager`, `GroundStation`, `ConnectionLine`, 필요 시 `OrbitShareBootstrap` 범위에서 최소 수정한다.

## 날짜별/단계별 체크리스트

- [ ] 2026-05-18 월: 현재 근거 정리  
  검증: `daylog/2026-05-16~18.md`, `plans/daily/2026-05-18~19.md`, 기존 lane note, `orbitshare-validation.md` 기준으로 완료/미완료 항목을 분리한다.

- [ ] 2026-05-19 화: Unity 실행 환경 확보 후 최신 자동 검증 시도  
  검증: LicensingClient IPC 오류 없이 `ValidateMissionFlow`가 실행되고, 최신 리포트에 `Result: PASS`, 24개 위성, 기본 목적지 New York, relay lock 100%, delivery 100%, Runtime Error/Exception 0이 기록되는지 확인한다.

- [ ] 2026-05-20 수: r1 추가 검증 항목 확인  
  검증: 최신 `orbitshare-validation.md`에 Runtime Canvas 1600x900, IMGUI fallback disabled, Reset replay 2회차, PresentationDirector camera cuts PASS가 포함되는지 확인한다.

- [ ] 2026-05-21 목: GUI core smoke 수행  
  검증: Play Mode에서 `Quick Relay` 단일 흐름이 SAT-S01 relay lock, Seoul gateway sync, delivery, Mission Complete까지 도달하는지 기록한다.

- [ ] 2026-05-22 금: 수동 흐름과 시간 배속 확인  
  검증: 목적지 선택 후 `Queue → Relay → 10x`로 완료까지 진행하고, Pause/1x/5x/10x에서 궤도 이동과 진행률 정지/가속이 일관되는지 확인한다.

- [ ] 2026-05-23 토: Reset/Play 재진입 안정성 확인  
  검증: Mission Complete 후 Reset, 두 번째 `Quick Relay`, Play 종료 후 재진입까지 반복해 stale state, 중복 이벤트, Runtime Error/Exception이 없는지 확인한다.

- [ ] 2026-05-24 일: core 수정 필요 여부 최종 판정 및 문서 lane 전달  
  검증: 실패가 재현된 항목만 최소 수정 후보로 분리하고, 수정하지 않은 항목은 “확인됨/환경 차단/확인 필요”로 명확히 기록한다.

## 검증 계획

- 자동 검증: `OrbitShareEditorAutomation.ValidateMissionFlow` 실행 결과로 `orbitshare-validation.md`를 확인한다.
- GUI 검증: 실제 Play Mode에서 `Quick Relay`, 수동 `Queue → Relay → 10x`, Reset 후 재실행, Play Mode 재진입을 확인한다.
- 상태 검증: `MissionManager` phase가 `Idle → Reserved → Running/Observing → ObservationComplete → Transmitting → Complete`로 끊기지 않는지 확인한다.
- 시간 검증: `SimulationManager.SetTimeScale()`이 `SatelliteOrbit.ScaledDeltaTime`과 relay/delivery 진행률에 같은 방향으로 반영되는지 확인한다.
- 표시 검증: `GroundStation` 통신 가능 조건과 `ConnectionLine.SetVisible()` 표시/숨김을 화면에서 확인한다.
- 로그 검증: Runtime Error/Exception 0을 기준으로 삼고, LicensingClient/DISPLAY/headless FMOD 오류는 gameplay 실패와 분리 기록한다.

## 리스크/확인 필요

- `plans/weekly/2026-W21.md`는 확인되지 않았고, `plans/weekly/` 폴더는 비어 있다.
- `docs/` 경로는 현재 확인되지 않았다.
- 기존 `orbitshare-validation.md`는 PASS지만 `Screen: 640x480` 기준이며, r1 추가 항목은 아직 반영되지 않았다.
- 최근 `unity-validation.log`, `unity-build-r1.log`, `unity-asset-hooks.log`는 `IPC channel to LicensingClient doesn't exist; aborting`으로 중단됐다.
- `build-run-smoke.log`의 headless `NullGfxDevice`/FMOD 결과는 GUI smoke PASS 근거로 사용하면 안 된다.
- 실제 궤도 역학, TLE, 지도 API는 README상 제한사항으로 분리되어 있으므로 과제 요구사항에 필수인지 별도 확인이 필요하다.
- core 코드는 정적 근거상 구현되어 있지만, `GroundStation`/`ConnectionLine` 표시와 수동 흐름은 화면 기준 검증이 아직 부족하다.

## 병렬 에이전트 활용 메모

- 이번 lane note 작성에는 하위/병렬 에이전트를 사용하지 않았다.
- 범위가 최근 daylog, 계획 문서, 핵심 core 스크립트, 검증 로그 확인으로 제한되어 직접 통합했다.
- 파일 읽기와 로그 확인은 병렬 shell 호출로 처리했고, 최종 판단은 단일 lane note로 정리했다.