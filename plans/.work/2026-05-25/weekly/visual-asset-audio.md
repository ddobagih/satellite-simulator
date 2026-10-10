# Computer Graphics Assignment 1 - Visual/Asset/Audio weekly lane note (2026-W22)

## 이번 주 목표 후보

- `P1-004 Asset hook 화면 QA`를 이번 주 lane 핵심으로 둔다: `SpaceSkybox.mat`, `SpaceStation.prefab`, Earth texture/shader set, procedural/generated fallback이 실제 화면/문서/리포트에서 같은 상태를 가리키게 한다.
- 새 Asset Store/SFX import, Git LFS, 대형 에셋 정책 변경은 product 결정상 dropped 상태를 유지한다. 대신 local inventory, static hook check, player/manual visual review로 안전한 대안을 남긴다.
- phase visual/audio language를 정리한다: Reserved/Running/Observing/Transmitting/Complete/Failed/Replaying의 line, packet, halo, beacon, coverage ring, tone 의미가 발표 화면에서 일관되게 읽히는지 확인한다.
- `asset-store-import-report.md`와 실제 코드/파일 상태의 gap을 정리한다. 현재 리포트는 skybox/station/SFX 5종 기준이고, 실제 코드는 Earth 4K/8K texture set 및 `SFX_Click`, `SFX_Failure` generated fallback도 사용한다.
- 최신 visual evidence를 확보한다. 현재 `orbitshare-playmode.png`는 1600x900에서 skybox, 지구, fallback 위성, 경로, 완료 UI가 보이지만, 실제 Editor GUI E2E와 오디오 청취 근거는 아니다.
- 8K/4K Earth texture의 CC BY 4.0 attribution, private repo/제출 용량 리스크를 docs lane에 넘긴다.

## 날짜별/단계별 체크리스트

- 2026-05-25 월요일
  - [ ] W22 기준선 정리 → 검증: `plans/weekly/2026-W22.md`, `PM/`, `plans/features`, `plans/verify/ready` 부재를 근거로 기록하고 없는 산출물을 인용하지 않는다.
  - [ ] current resource inventory 작성 → 검증: skybox/station/Earth set FOUND, `SatelliteModel.prefab`/`PremiumEarthMaterial.mat`/SFX wav optional MISSING으로 분리한다.
  - [ ] 최신 캡처 판독 메모 작성 → 검증: `orbitshare-playmode.png` 1600x900에서 보이는/가려지는 visual 요소를 화면 기준으로 기록한다.

- 2026-05-26 화요일
  - [ ] phase visual/audio language 표 작성 → 검증: phase별 color, line, packet, halo, beacon, tone 기대값이 표로 남는다.
  - [ ] `AudioFeedbackManager` hook gap 정리 → 검증: `SFX_Click`, `SFX_Failure`를 문서 hook에 추가할지 generated-only 보조 tone으로 둘지 결정한다.
  - [ ] Earth texture load order 확인 → 검증: 코드 기준 4K 우선, 8K fallback, procedural fallback 순서가 문서화된다.

- 2026-05-27 수요일
  - [ ] Resource Hook 검증 또는 정적 대체 수행 → 검증: Unity hook validation을 실행하면 `asset-store-import-report.md` 갱신, 못 하면 파일 목록/코드 load path 대조표로 대체한다.
  - [ ] skybox/station dependency 재확인 → 검증: 대량 원본 삭제 후 `SpaceSkybox.mat` 6면 texture와 `SpaceStation.prefab` mesh/material 참조가 깨지지 않았는지 확인한다.

- 2026-05-28 목요일
  - [ ] visual matrix 수동/캡처 판독 → 검증: route line, packet stream, selected satellite beacon, destination halo, station range가 phase별로 보이는지 기록한다.
  - [ ] 최소 보정 여부 결정 → 검증: 안 읽히는 효과가 있을 때만 `MissionVisualController.cs`, `ConnectionLine.cs`, `SignalPacketStream.cs`, `SatelliteFocusController.cs`의 색/alpha/두께/속도 범위로 제한한다.

- 2026-05-29 금요일
  - [ ] audio fallback 확인 → 검증: 출력 장치 청취가 가능하면 click/reserve/start/observe/transmit/complete/failure tone을 실제 청취로 기록하고, 불가하면 clip/event 연결 확인까지만 남긴다.
  - [ ] visual/audio 변경 후 handoff → 검증: 변경이 있으면 validation/build/display smoke는 build-validation lane 단일 runner가 순차 실행하도록 넘긴다.

- 2026-05-30 토요일
  - [ ] 문서 정합성 후보 정리 → 검증: `ASSET_STORE_IMPORT.md`, `asset-store-import-report.md`, README/WORK_LOG의 asset/fallback 설명 충돌을 표로 정리한다.
  - [ ] 라이선스/용량 handoff → 검증: `Assets/Resources/OrbitShare/Earth/ATTRIBUTION.md`, CC BY 4.0, 8K/4K 포함 상태를 spec lane에 전달한다.

- 2026-05-31 일요일
  - [ ] visual/audio freeze 점검 → 검증: 새 에셋 추가 없이 hook/fallback 상태, 최신 visual evidence, 미확인 audio 청취 여부가 분리되어 있다.
  - [ ] 최종 lane 상태 요약 → 검증: 실제 실행한 검증과 미실행/보류 항목이 daylog/주간 통합에 들어갈 수 있는 형태로 정리된다.

## 검증 계획

- Static inventory: `Assets/Resources/OrbitShare` 파일 목록, `Resources.Load` 경로, `asset-store-import-report.md`, `ASSET_STORE_IMPORT.md`를 대조한다.
- Visual Review: 1600x900 캡처 또는 player/manual 화면에서 지구, skybox, station, route, packet, halo, beacon, 완료/실패 상태를 사람 판독으로 기록한다.
- Unity Resource Hook: 가능하면 `OrbitShare > Asset Store > Validate Resource Hooks` 또는 대응 editor method로 리포트를 갱신한다. 실패 시 환경/Unity lock 문제와 앱 문제를 분리한다.
- Audio: 실제 청취가 가능할 때만 청취 PASS로 쓴다. 아니면 `AudioFeedbackManager`의 generated clip 생성과 mission phase event 연결 확인으로 제한한다.
- Regression handoff: visual/audio 코드나 리소스 변경이 있으면 `ValidateMissionFlow`, capture, build, display smoke는 병렬 실행하지 않고 단일 Unity runner가 순차 실행한다.

## 리스크/확인 필요

- `orbitshare-validation.md`가 tracked 삭제 상태라 최신 자동 검증 PASS로 단정하면 안 된다.
- `asset-store-import-report.md`는 Earth texture/shader set과 `SFX_Click`, `SFX_Failure` hook gap을 반영하지 않는다.
- 현재 캡처는 visual evidence지만 실제 Editor GUI 수동 E2E나 오디오 청취 근거는 아니다.
- 대량 Asset Store 원본 삭제가 남아 있어 skybox/station 참조 무결성 재확인이 필요하다.
- 8K/4K Earth texture는 품질에는 유리하지만 제출 용량, attribution, private repo 정책 리스크가 있다.
- `PM/status`, `PM/reports`, product audit, automation metrics, 03:00 feature/verify 산출물은 확인되지 않았다.

## 병렬 에이전트 활용 메모

- 이번 lane note 작성에는 하위/병렬 에이전트를 사용하지 않았다.
- 근거 수집은 병렬 shell 조회로 나누었고, 최종 판단은 단일 markdown으로 통합했다.
- 실제 W22 실행에서는 resource inventory, audio hook gap, visual matrix, license/size 조사는 병렬 read-only 하위 작업으로 나눌 수 있다.
- Unity validation, capture, Resource Hook, build, player/display smoke는 project lock/log 충돌 방지를 위해 단일 runner가 순차 실행해야 한다.