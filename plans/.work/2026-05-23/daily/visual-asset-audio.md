# Computer Graphics Assignment 1 - Visual/Asset/Audio lane note (2026-05-24)

## 최근 진행 근거
- 2026-05-22 `daylog/2026-05-22.md`: 미사용 Asset Store 에셋 정리, NASA Blue Marble 기반 `EarthDay.jpg`, `EarthDayNight.shader`, `AtmosphereRim.shader`, 전송 패킷/라인 pulse, 청록 위성 focus 표시가 추가됨.
- 2026-05-22 `orbitshare-validation.md`, `unity-validation-more-features.log`, `unity-build-more-features.log`: 자동 검증 PASS와 Linux build Success 기록. 단, 시각 품질 판독이나 오디오 청취 근거는 아님.
- 2026-05-23 `daylog/2026-05-23.md`: 당일 신규 코드/에셋 변경은 없고 상태 조사만 수행. `Resources/OrbitShare`에 `SpaceSkybox.mat`, `SpaceStation.prefab`, `EarthDay.jpg`, shader 2종이 있고, `SatelliteModel.prefab`, `PremiumEarthMaterial.mat`, SFX 5종은 optional missing으로 확인됨.
- 2026-05-20 `orbitshare-playmode.png`: 1600x900 PNG는 존재하지만 2026-05-22의 단계형 UI/지구/패킷/대기열 변경 전 화면이므로 최신 visual evidence로는 부족함.
- 2026-05-11 `asset-store-import-report.md`: `SpaceSkybox.mat`, `SpaceStation.prefab` FOUND, 나머지 optional MISSING. 리포트 mtime이 오래됐고 `EarthDay.jpg`/shader는 해당 hook report 대상이 아님.
- `product/vision.md`, `product/backlog.md`, `product/done-criteria.md`, `product/decisions.md`: Asset Store skybox/station hook과 procedural/generated fallback 유지가 기준. 새 위성 모델, 고급 Earth material, SFX, Git LFS는 현재 dropped.
- `plans/features`, `plans/verify/ready`, `plans/verify/reports`, `PM/status`, `PM/reports`, `plans/daily/2026-05-24.md`: 파일 없음. scheduler/product-audit/automation-metrics 근거는 없음.

## 내일 목표 후보
- 1순위: 최신 visual storytelling acceptance. 예약→연결→업링크→릴레이→다운링크→완료 단계에서 route line, packet stream, destination halo, selected satellite beacon이 실제 발표 화면에서 읽히는지 확인하고, 부족할 때만 `MissionVisualController`, `ConnectionLine`, `SignalPacketStream`, `SatelliteFocusController` 범위에서 최소 보정.
- 2순위: `Resources/OrbitShare` 인벤토리 정리. Asset Store hook 2개와 번들 runtime visual 리소스(`EarthDay.jpg`, shader 2종)를 분리해 문서 lane에 넘길 상태표 작성.
- 3순위: 최신 화면 근거 확보. 기존 `orbitshare-playmode.png`가 stale이므로 Editor GUI 또는 Linux player 화면 기준으로 최신 지구/skybox/station/패킷/위성 focus를 사람 판독.
- 4순위: audio fallback 검증. 실제 출력 장치에서 phase tone을 청취하고, 불가하면 `AudioFeedbackManager`의 generated clip/event 연결만 확인하며 청취 완료로 쓰지 않음.
- 5순위: Asset Store dependency smoke. 에셋 삭제 후 `SpaceSkybox.mat` texture 6개와 `SpaceStation.prefab` mesh/material/texture 의존성이 깨지지 않았는지 재확인.
- 6순위: 새 Asset Store/SFX/LFS 작업은 보류. product 결정이 바뀌지 않는 한 현재 hook/fallback 구조를 유지.

## 상세 체크리스트 초안
- [ ] 단계별 전송 시각 효과 판독 → 검증: 예약/연결/업링크/릴레이/다운링크/완료 각 단계에서 line pulse, packet, halo, beacon이 실제 화면에서 보이는지 캡처 또는 수동 메모로 기록.
- [ ] 시각 효과 최소 보정 여부 결정 → 검증: 보이지 않을 때만 색/두께/속도/alpha 값을 소폭 조정하고 `OrbitShareEditorAutomation.ValidateMissionFlow` PASS를 다시 확인.
- [ ] `Resources/OrbitShare` 인벤토리 표 작성 → 검증: `SpaceSkybox.mat`, `SpaceStation.prefab`, `EarthDay.jpg`, `EarthDayNight.shader`, `AtmosphereRim.shader`, optional missing asset/SFX를 `FOUND/optional/fallback/문서 반영 필요`로 분류.
- [ ] Resource Hook 검증 실행 여부 조율 → 검증: Unity 단일 runner가 가능할 때만 `Validate Resource Hooks`를 실행하고 `asset-store-import-report.md` mtime을 기록. 미실행 시 정적 인벤토리만 근거로 남김.
- [ ] 최신 visual evidence 확보 → 검증: 기존 2026-05-20 PNG와 최신 2026-05-22 이후 화면 차이를 분리하고, 최신 캡처 실패 시 실패 로그와 stale 근거를 명시.
- [ ] skybox/station dependency 확인 → 검증: 삭제된 Asset Store 샘플/미사용 파일 때문에 runtime skybox/station 참조가 깨지지 않았는지 GUID/파일 존재 또는 Unity 실행으로 확인.
- [ ] generated tone 청취 → 검증: Reserved/Running/Observing/Transmitting/Complete phase tone을 실제 장치에서 들었는지 기록.
- [ ] 오디오 safe alternative → 검증: 청취가 막히면 `AudioFeedbackManager`의 `Resources.Load<SFX_*> ?? CreateTone/CreateChord`와 `OnMissionChanged` 연결만 확인하고 “청취 미확인”으로 남김.
- [ ] 대용량/라이선스 handoff → 검증: `Assets/Cobble Games`가 여전히 약 2.4GB이고 Asset Store 리소스가 포함됨을 spec/docs lane에 전달.

## 리스크/확인 필요
- 2026-05-24는 주간 계획상 final freeze 날이므로 신규 시각 기능은 발표 차단 요소가 있을 때만 최소 수정해야 함.
- 최신 1600x900 스크린샷이 없음. 2026-05-22 기록상 headless screenshot capture는 Unity native crash가 재발했으므로 자동 캡처 실패 가능성이 있음.
- `EarthDay.jpg`가 추가됐지만 product 결정에는 “외부 고급 지구 에셋 추가 없음” 취지가 남아 있어, 번들 runtime texture와 `PremiumEarthMaterial.mat` optional missing의 차이를 문서에서 분리해야 함.
- `EarthDay.jpg`, shader 2종, `SignalPacketStream.cs` 등 일부 최신 visual 파일이 untracked 상태라 제출/커밋 포함 범위 확인이 필요함.
- generated tone은 실제 SFX 파일이 아니며 발표 장비의 음량/출력 장치에 따라 들리지 않을 수 있음.
- Unity validation, Resource Hook, capture, build는 project lock/log 충돌 위험 때문에 병렬 실행하지 말고 단일 runner가 순차 실행해야 함.
- `plans/features`, `plans/verify`, `PM/*` 산출물이 없어 없는 산출물을 근거로 삼으면 안 됨.

## 병렬 에이전트 활용 메모
- 하위/병렬 에이전트는 사용하지 않았음.
- 이유: 이번 작업은 최종 파일을 수정하지 않는 lane note 작성이고, 근거가 product/daylog/asset report/관련 코드로 좁혀져 단일 통합이 더 안전했음.
- 파일 목록/문서/코드 확인은 병렬 shell 호출로 나눠 처리함.
- 실제 실행일에는 정적 asset inventory, 최신 화면 판독, audio 코드 확인은 병렬 조사 가능하지만 Unity 실행 작업은 단일 runner로 순차 처리해야 함.