# Computer Graphics Assignment 1 - Visual/Asset/Audio lane note (2026-05-25)

## 최근 진행 근거
- 2026-05-24 `daylog/2026-05-24.md`: Solar System Scope/Wikimedia 기반 `EarthDaySSS`, `EarthNightSSS`, `EarthCloudsSSS`, `EarthNormalSSS`, `EarthSpecularSSS` 5종 8192x4096 텍스처와 CC BY 4.0 `ATTRIBUTION.md`가 추가됨. `EarthDayNight.shader`, `EarthClouds.shader`, `OrbitShareBootstrap.cs` 연결 확인.
- 2026-05-24 `unity-build-earth-textures.log`, `unity-build-earth-color-fix.log`: Linux build `Build Finished, Result: Success.` 확인. 단, validation 재실행은 아님.
- 2026-05-24 `build-run-live.log`: 1280x720 player 실행, OpenGL 4.5 생성, `Exception`/`NullReference`/`Shader error` 매치 없음. 실제 GUI 리허설/화면 판독/오디오 청취 완료 근거는 아님.
- 2026-05-23 `daylog/2026-05-23.md`: 전송 패킷, 경로 glow, 고대비 모드, 목적지/위성 coverage ring, next connection hint, route replay visual control, 실패 tone 보강 기록.
- 2026-05-22 `daylog/2026-05-22.md`: 미사용 Asset Store 에셋 삭제 후 `SpaceSkybox.mat` texture 6개와 `SpaceStation.prefab` mesh/material 의존성 유지 확인, missing hook dependency 0개.
- 2026-05-11 `asset-store-import-report.md`: `SpaceSkybox.mat`, `SpaceStation.prefab` FOUND. `SatelliteModel.prefab`, `PremiumEarthMaterial.mat`, `SFX_Reserve/Start/Observe/Transmit/Complete.wav`는 optional MISSING.
- 2026-05-24 현재 파일 확인: `Assets/Resources/OrbitShare/`에는 skybox/station hook, 8K Earth texture set, `EarthDay.jpg`, shader 3종이 있음. SFX wav와 `PremiumEarthMaterial.mat`, `SatelliteModel.prefab`은 없음.
- `AudioFeedbackManager.cs`: 문서화된 SFX 5종 외에 `SFX_Click`, `SFX_Failure`도 optional load 후 generated tone fallback을 사용함. `ASSET_STORE_IMPORT.md`/report와 hook 목록이 일부 다름.
- `product/vision.md`, `product/backlog.md`, `product/done-criteria.md`, `product/decisions.md`: Asset Store skybox/station hook과 procedural/generated fallback 유지, 새 위성 모델/고급 material/SFX/Git LFS는 dropped. 에셋/UI 품질은 자동화만으로 완료 처리 금지.
- 03:00 scheduler/PM 산출물 확인: `plans/features`, `plans/verify/ready`, `plans/verify/reports`는 파일 0개, `PM/` 없음. `plans/daily/2026-05-25.md`도 없음.

## 내일 목표 후보
- 1순위: 최신 visual storytelling acceptance. 8K 지구, skybox, station, route line, packet stream, destination halo, selected satellite beacon이 1280x720 player와 가능하면 1600x900 이상에서 읽히는지 확인.
- 2순위: 전송 단계별 visual state matrix. 예약/연결/업링크/릴레이/다운링크/완료/실패/경로 재생 상태별로 어떤 3D 효과가 보여야 하는지 실제 화면 기준으로 판독.
- 3순위: Resource hook/fallback 정합화. Asset Store hook, 번들 runtime visual resource, generated audio fallback을 한 표로 분리하고 `SFX_Click`/`SFX_Failure` 문서 gap 처리 방침 결정.
- 4순위: audio fallback 검증. 실제 장치 청취가 가능하면 phase tone과 failure tone을 듣고, 불가하면 `AudioFeedbackManager` event/clip 생성 연결까지만 확인.
- 5순위: 라이선스/대용량 handoff. 8K Earth texture CC BY 4.0 표기와 Asset Store/private repo/대용량 제출 리스크를 docs lane에 전달.
- 6순위: 새 Asset Store/SFX import, Git LFS, 대규모 에셋 정책 변경은 product 결정 변경 전에는 실행하지 않음.

## 상세 체크리스트 초안
- [ ] 최신 화면 근거 확보 → 검증: Linux player 또는 Editor에서 1280x720 및 가능하면 1600x900 이상 화면을 확인하고, 기존 `orbitshare-playmode.png`가 2026-05-22 이전 stale 근거인지 분리한다.
- [ ] 고해상도 지구 판독 → 검증: day/night, cloud, atmosphere rim, specular 느낌이 보이고 UI/경로선을 가리지 않는지 사람 판독 메모를 남긴다.
- [ ] 전송 단계 visual matrix 작성 → 검증: Reserved/Running/Observing/Transmitting/Complete/Failed/Replaying에서 line, packet, halo, beacon, coverage ring의 표시/색/두께가 의도와 맞는지 기록한다.
- [ ] 최소 보정 여부 결정 → 검증: 화면에서 안 읽히는 효과가 있을 때만 `MissionVisualController.cs`, `ConnectionLine.cs`, `SignalPacketStream.cs`, `SatelliteFocusController.cs`, 필요 시 `OrbitShareBootstrap.cs`에서 색/alpha/두께/속도만 소폭 조정한다.
- [ ] Resource inventory 갱신 → 검증: `SpaceSkybox.mat`, `SpaceStation.prefab`, Earth texture/shader set은 FOUND, `SatelliteModel.prefab`, `PremiumEarthMaterial.mat`, SFX wav는 optional missing/fallback으로 분류한다.
- [ ] SFX hook 목록 gap 정리 → 검증: `AudioFeedbackManager.cs`의 `SFX_Click`, `SFX_Failure`를 generated-only 보조 tone으로 문서화할지, hook report 대상에 추가할지 결정한다. 실제 SFX import는 하지 않는다.
- [ ] generated tone 청취 → 검증: 예약/시작/관측/전송/완료/실패/click tone을 실제 출력 장치에서 들었는지 기록한다. 못 들으면 청취 미확인으로 남긴다.
- [ ] Asset Store dependency 재확인 → 검증: 미사용 에셋 삭제 후 `SpaceSkybox.mat` texture와 `SpaceStation.prefab` mesh/material 참조가 정적 확인 또는 Unity Resource Hook 검증과 맞는지 확인한다.
- [ ] validation/build lane 조율 → 검증: visual 보정이 있으면 단일 Unity runner로 build/validation을 순차 실행하고, `orbitshare-validation.md` 삭제 상태를 최신 결과로 복구할지 확인한다.
- [ ] 라이선스/용량 handoff → 검증: `Assets/Resources/OrbitShare/Earth/ATTRIBUTION.md` 존재, 8K texture 포함, Asset Store 리소스 포함, private repo 필요성을 docs lane에 전달한다.

## 리스크/확인 필요
- `orbitshare-validation.md`가 현재 작업트리에서 삭제 상태로 보인다. 2026-05-24 지구 텍스처 이후 최신 validation PASS 근거는 아직 없음.
- 최신 1600x900 이상 스크린샷이 없다. 과거 headless `CapturePlayScreenshot`은 `Camera.Render()` native crash 이력이 있어 player/manual 판독 대체가 필요할 수 있다.
- 8K 텍스처 5종은 품질에는 좋지만 메모리/용량/제출 정책 리스크가 있다. CC BY 4.0 attribution은 유지해야 한다.
- `ASSET_STORE_IMPORT.md`와 `asset-store-import-report.md`는 Earth texture/shader set과 `SFX_Click`/`SFX_Failure`를 반영하지 않는다. 의도된 범위인지 문서 gap인지 확인 필요.
- 실제 오디오 청취 근거가 없다. generated tone event 연결 확인만으로 “소리가 잘 들린다”고 쓰면 안 된다.
- 작업트리에 대량 삭제, modified 코드, untracked texture/shader/script/daylog가 섞여 있다. visual lane은 되돌리지 말고 포함/제외 인벤토리만 넘긴다.
- README/product 문서는 2026-05-22 이후 목적지 확장, 대기열/기록/실패 재시도, 고해상도 지구 텍스처 상태를 충분히 반영하지 못한다.
- 새 Asset Store/SFX import, Git LFS, Windows/PPT/MP4/Submission은 현재 product 결정상 자동 실행 대상이 아니다.

## 병렬 에이전트 활용 메모
- 하위/병렬 에이전트는 사용하지 않았음.
- 이유: 이번 산출물은 lane note 본문 작성이고, 근거 범위가 product/daylog/resource/code hook 확인으로 충분히 좁아 단일 통합이 안전했음.
- 파일 검색과 읽기는 병렬 shell 호출로 나눴다.
- 내일 실행 단계에서는 resource inventory, audio hook 문서 gap, visual matrix 조사는 병렬 가능하지만, Unity validation/capture/resource hook/build/player 실행은 project lock/log 충돌 방지를 위해 단일 runner가 순차 수행해야 함.