# Computer Graphics Assignment 1 - Visual/Asset/Audio lane note (2026-05-26)

## 최근 진행 근거
- 2026-05-25 `daylog/2026-05-25.md`: UI/전송 흐름/월드 비주얼 보강 확인. 전송 패킷, 경로 glow/halo, 완료 pulse, 목적지/위성 시각 강조가 보강됐고 최신 `orbitshare-playmode.png`는 1600x900 캡처로 확인됨. 단, 실제 Editor GUI 수동 E2E와 오디오 청취 근거는 아님.
- 2026-05-25 `git status --short --branch`: `orbitshare-validation.md`가 삭제 상태이고, 대량 삭제/수정/untracked 에셋·스크립트가 섞여 있음. visual lane은 되돌리지 않고 포함/제외 인벤토리와 검증 필요만 넘겨야 함.
- 2026-05-24 `daylog/2026-05-24.md`: Solar System Scope/Wikimedia 기반 Earth day/night/cloud/normal/specular 8K 텍스처와 4K fallback, `EarthDayNight.shader`, `EarthClouds.shader`, CC BY 4.0 `ATTRIBUTION.md` 확인.
- 2026-05-25 파일 확인: `Assets/Resources/OrbitShare/SpaceSkybox.mat`, `SpaceStation.prefab`, Earth texture/shader set은 존재. `SatelliteModel.prefab`, `PremiumEarthMaterial.mat`, SFX wav는 없음.
- 2026-05-11 `asset-store-import-report.md`: `SpaceSkybox.mat`, `SpaceStation.prefab` FOUND, 기존 SFX 5종과 satellite/premium earth는 optional MISSING. 현재 Earth texture set과 `SFX_Click`, `SFX_Failure` hook은 이 리포트에 반영되지 않음.
- `Assets/Scripts/AudioFeedbackManager.cs`: `SFX_Click`, `SFX_Reserve`, `SFX_Start`, `SFX_Observe`, `SFX_Transmit`, `SFX_Complete`, `SFX_Failure`를 `Resources.Load`하고 없으면 generated tone/chord fallback 사용.
- `product/decisions.md`: 새 Asset Store/SFX import, Git LFS, Windows/PPT/MP4/Submission은 현재 dropped. 에셋/UI 품질은 자동화만으로 완료 처리 금지.
- `plans/features`, `plans/verify`, `PM/status`, `PM/reports`, product audit, automation metrics 산출물은 확인되지 않음. `plans/daily/2026-05-26.md`도 없음.

## 내일 목표 후보
- 1순위: 신규 feature slice로 `phase visual/audio language` 정리. Reserved/Running/Observing/Transmitting/Complete/Failed/Replaying 단계가 색, 선 두께, packet, halo, beacon, tone에서 같은 의미로 읽히게 최소 보정한다.
- 2순위: 최신 visual acceptance 확보. 1600x900 캡처와 가능하면 player/Editor 화면에서 지구, skybox, station, route, packet, 완료/실패 표시가 발표 화면에서 읽히는지 판독한다.
- 3순위: Resource hook/fallback 인벤토리 갱신. Asset Store hook, Earth texture/shader bundle, optional missing, generated audio fallback을 한 표로 분리한다.
- 4순위: audio fallback 검증. 실제 출력 장치 청취가 가능하면 phase/click/failure tone을 확인하고, 불가하면 clip 생성과 mission event 연결까지만 확인한다.
- 5순위: 8K/4K 지구 텍스처 라이선스·용량 handoff. CC BY 4.0 attribution, private repo 필요성, 제출 포함 리스크를 docs lane에 전달한다.
- 6순위: 새 Asset Store/SFX import, Git LFS, 외부 업로드, 대형 에셋 정책 변경은 요구 변경 전 실행하지 않는다.

## 상세 체크리스트 초안
- [ ] phase visual/audio language slice 범위 확정 → 검증: 단계별 color/line/packet/halo/beacon/tone 표가 생기고, 새 UI/튜토리얼/대형 리팩터링 없이 가능한 수정만 남는다.
- [ ] 전송 단계별 visual matrix 작성 → 검증: Reserved/Running/Observing/Transmitting/Complete/Failed/Replaying에서 line, packet, halo, beacon, coverage ring 표시 여부를 캡처 또는 수동 판독으로 기록한다.
- [ ] 최소 보정 수행 여부 결정 → 검증: 화면에서 안 읽히는 효과가 있을 때만 `MissionVisualController.cs`, `ConnectionLine.cs`, `SignalPacketStream.cs`, `SatelliteFocusController.cs`의 색/alpha/두께/속도만 소폭 조정한다.
- [ ] 최신 visual evidence 판독 → 검증: `orbitshare-playmode.png` 1600x900 캡처와 가능하면 player/Editor 화면에서 지구·경로·완료 상태가 보이는지 기록한다.
- [ ] Earth texture loading 상태 확인 → 검증: 4K 우선, 8K fallback, procedural fallback 순서와 day/night/cloud/normal/specular 표시 상태를 확인한다.
- [ ] Resource inventory 갱신 → 검증: skybox/station/Earth set은 FOUND, satellite/premium material/SFX wav는 optional missing/fallback으로 분류한다.
- [ ] `SFX_Click`/`SFX_Failure` 문서 gap 처리 → 검증: generated-only 보조 tone으로 둘지 hook report 대상에 추가할지 결정한다. 실제 SFX import는 하지 않는다.
- [ ] audio fallback 확인 → 검증: 예약/시작/관측/전송/완료/실패/click tone 청취 여부를 기록하고, 못 들으면 “청취 미확인, clip/event 연결만 확인”으로 둔다.
- [ ] skybox/station dependency 재확인 → 검증: 미사용 Asset Store 원본 삭제 후 `SpaceSkybox.mat`, `SpaceStation.prefab` 참조가 깨지지 않았는지 정적 또는 Resource Hook 검증으로 확인한다.
- [ ] 검증 lane handoff → 검증: visual code 변경이 있으면 Unity validation/build/display smoke는 단일 runner가 순차 실행하도록 넘긴다.
- [ ] 라이선스/용량 handoff → 검증: `Assets/Resources/OrbitShare/Earth/ATTRIBUTION.md`, CC BY 4.0, 8K/4K 포함, private repo 필요성을 docs lane에 전달한다.

## 리스크/확인 필요
- `orbitshare-validation.md`가 삭제 상태라 최신 자동 검증 PASS로 단정할 수 없음.
- 최신 캡처는 화면 근거일 뿐 실제 Editor GUI 조작 완료나 오디오 청취 완료가 아님.
- `asset-store-import-report.md`가 2026-05-11 기준이라 Earth texture set, 4K fallback, `SFX_Click`, `SFX_Failure` 상태를 반영하지 않음.
- 대량 Asset Store 원본 삭제 후 skybox/station runtime hook 참조가 깨지지 않았는지 재확인 필요.
- 8K 텍스처는 품질에는 좋지만 제출 용량·공개 저장소·라이선스 리스크가 있음. attribution 유지가 필요함.
- generated tone은 실제 음원 파일이 아니며 발표 장비에서 들렸는지 별도 확인 전까지 청취 PASS로 쓰면 안 됨.
- scheduler/PM/product audit/automation metrics 산출물이 없으므로 없는 근거를 인용하지 않음.

## 병렬 에이전트 활용 메모
- 사용하지 않았음.
- 이유: 이번 작업은 lane note 작성을 위한 read-only 근거 통합이고, 최종 산출물이 단일 markdown 본문이라 하위 에이전트 분리 이점이 작았음.
- 파일 목록/문서/코드 확인은 병렬 shell 호출로 처리함.
- 내일 실제 실행에서는 resource inventory, audio hook gap, visual matrix 조사는 병렬 read-only로 나눌 수 있음. Unity validation/capture/build/player 실행은 project lock/log 충돌 방지를 위해 단일 runner가 순차 수행해야 함.