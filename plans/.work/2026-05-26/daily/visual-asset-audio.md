# Computer Graphics Assignment 1 - Visual/Asset/Audio lane note (2026-05-27)

## 최근 진행 근거
- 2026-05-26 `daylog/2026-05-26.md`: 통신 경로를 `사용자(서울) → 인공위성 → 목적지` 기준으로 정리했고, 발신/수신 패킷 색상·방향·glow/width/pulse 보강을 확인함.
- 2026-05-26 `orbitshare-validation.md`: `Result: PASS`, 7개 목적지, 24개 위성, Runtime Error/Exception 0, Fast Demo 2회 완료 근거가 있음.
- 2026-05-26 `orbitshare-menu-captures/report.md`: 메뉴/작성 5단계 캡처가 1600x900 PNG로 생성됐고 active-panel intersections `PASS(none)` 기록이 있음.
- 2026-05-26 파일 확인: `orbitshare-playmode.png`와 메뉴 캡처 PNG들은 1600x900으로 존재함. 단, 화면 판독 근거이지 오디오 청취 근거는 아님.
- 2026-05-26 리소스 상태: `SpaceSkybox.mat`, `SpaceStation.prefab`, Earth texture/shader/attribution은 FOUND. `SatelliteModel.prefab`, `PremiumEarthMaterial.mat`, `SFX_*.wav`는 optional MISSING이며 procedural/generated fallback 대상.
- 2026-05-24 `daylog/2026-05-24.md`, `OrbitShareBootstrap.cs`: Solar System Scope 기반 Earth day/night/cloud/normal/specular 텍스처와 4K 우선, 8K fallback, procedural fallback 경로가 확인됨.
- `ASSET_STORE_IMPORT.md`, `asset-store-import-report.md`: 현재 문서는 Earth texture/shader와 `SFX_Click`/`SFX_Failure`까지 포함하지만, `Assets/Editor/AssetStorePackageImporter.cs`의 `ValidateResourceHooks`는 아직 Earth set과 click/failure hook을 쓰지 않아 report 재생성 시 누락될 위험이 있음.
- `plans/features`, `plans/verify/ready`, `plans/verify/reports`는 파일 0개이고 `PM/`, `docs/`, `plans/daily/2026-05-27.md`, 로컬 `AGENTS.md`는 확인되지 않음. 사용자 메시지의 AGENTS 지침을 적용함.

## 내일 목표 후보
- 1순위: `Resource Hook Report Parity v1` 진행. 런타임 리소스 이름, `ASSET_STORE_IMPORT.md`, `asset-store-import-report.md`, `AssetStorePackageImporter.ValidateResourceHooks`가 같은 FOUND/MISSING 기준을 쓰게 맞춘다.
- 2순위: `Phase Visual/Audio Language v1` 정리. Reserved/Running/Observing/Transmitting/Complete/Failed/Replaying 단계의 색, 선, packet, halo, beacon, tone 의미를 표로 고정한다.
- 3순위: 최신 visual evidence pack 확보. 1600x900 캡처에서 지구, skybox, station, route, packet, 완료/실패 pulse가 발표 화면에서 읽히는지 사람 판독한다.
- 4순위: audio fallback smoke. 실제 청취가 가능하면 click/reserve/start/observe/transmit/complete/failure tone을 확인하고, 불가하면 event/clip 연결만 확인으로 분리한다.
- 5순위: skybox/station dependency와 Earth license/size handoff. 대량 원본 에셋 삭제 후 runtime hook이 깨지지 않았는지 확인하고 CC BY 4.0 attribution/private repo 필요성을 docs lane에 넘긴다.
- 6순위: 새 Asset Store/SFX import, Git LFS, 외부 업로드, Windows/PPT/MP4/Submission은 요구 변경 전 실행하지 않는다.

## 상세 체크리스트 초안
- [ ] Runtime resource source-of-truth 확정 → 검증: `ASSET_STORE_IMPORT.md`, `asset-store-import-report.md`, `AudioFeedbackManager.cs`, `OrbitShareBootstrap.cs`의 hook 이름이 같은 표로 정리된다.
- [ ] `ValidateResourceHooks` parity 최소 수정 후보 작성 → 검증: Earth shader/texture/attribution, `SFX_Click`, `SFX_Failure`가 report 재생성 대상에 포함되는지 코드 기준으로 확인한다.
- [ ] Resource inventory dry-run → 검증: Unity 실행 없이 `Assets/Resources/OrbitShare` 파일 존재로 FOUND/MISSING/optional fallback 표를 만든다.
- [ ] Resource Hook Unity 실행 여부 결정 → 검증: 실행하면 단일 Unity runner로 `asset-store-import-report.md`를 재생성하고, 보류하면 정적 inventory 대체 사유를 기록한다.
- [ ] phase visual/audio language 표 작성 → 검증: 각 phase별 line color, packet visibility, pulse, halo/beacon, tone이 한 표에서 충돌 없이 설명된다.
- [ ] visual evidence 판독 → 검증: `orbitshare-playmode.png`와 메뉴 캡처에서 지구/경로/패킷/완료 상태가 읽히는지 PASS/보완 필요로 기록한다.
- [ ] 최소 visual 보정 여부 결정 → 검증: 화면에서 안 읽히는 효과가 있을 때만 `MissionVisualController.cs`, `ConnectionLine.cs`, `SignalPacketStream.cs`, `SatelliteFocusController.cs`의 색/alpha/두께/속도만 조정한다.
- [ ] audio hook gap 정리 → 검증: SFX wav 미존재는 실패가 아니라 generated tone fallback으로 분류하고, click/failure hook도 문서에 반영한다.
- [ ] generated tone 확인 → 검증: 실제 청취 가능 시 단계별 tone을 듣고, 불가 시 `AudioFeedbackManager` event/clip 생성 연결 확인까지만 적는다.
- [ ] skybox/station dependency 확인 → 검증: `SpaceSkybox.mat`, `SpaceStation.prefab`가 대량 원본 에셋 삭제 뒤에도 runtime hook으로 남아 있는지 정적 또는 Resource Hook 검증으로 확인한다.
- [ ] Earth texture/license handoff → 검증: `ATTRIBUTION.md`, CC BY 4.0, 4K/8K 포함 용량, 공개 저장소/제출 방식 변경 시 리스크를 docs lane에 전달한다.
- [ ] 변경 발생 시 검증 handoff → 검증: visual/resource 코드 변경 후 `ValidateMissionFlow`, capture/build/display smoke는 병렬 실행하지 않고 단일 runner가 순차 실행하도록 넘긴다.

## 리스크/확인 필요
- `AssetStorePackageImporter.cs`와 현재 `asset-store-import-report.md`가 불일치한다. Safe alternative: Unity 메뉴 실행 전 정적 inventory를 기준으로 report parity를 먼저 맞춘다.
- 자동 검증 PASS는 기능 회귀 근거지만, 에셋/UI 품질 최종 PASS나 오디오 청취 PASS로 쓰면 안 된다.
- `SFX_*.wav`는 optional missing이다. Safe alternative: 실제 SFX import 없이 generated tone fallback과 event 연결만 검증한다.
- 실제 출력 장치가 없으면 오디오 청취는 미확인으로 남긴다. “소리 확인 완료”로 쓰지 않는다.
- Earth 텍스처는 CC BY 4.0 attribution과 용량 리스크가 있다. 제출/공개 범위가 바뀌면 재확인이 필요하다.
- 작업트리에 대량 삭제와 untracked 리소스가 섞여 있다. visual lane은 되돌리지 않고 포함/제외 인벤토리만 넘긴다.
- `plans/features`, `plans/verify`, `PM/status`, product audit, automation metrics 산출물이 없으므로 없는 근거를 인용하지 않는다.
- Unity validation/capture/build/resource hook은 project lock/log 충돌 위험이 있어 병렬 실행 금지.

## 병렬 에이전트 활용 메모
- 사용하지 않았음.
- 이유: 이번 작업은 read-only lane note 작성이고 최종 산출물이 단일 markdown 본문이라 하위 에이전트 분리 이점이 작았음.
- 파일/문서/코드 조회는 병렬 shell 호출로 처리함.
- 내일 실제 작업에서는 resource inventory와 phase matrix 조사는 병렬 read-only로 나눌 수 있지만, Unity 실행 작업은 단일 runner가 순차 수행해야 함.