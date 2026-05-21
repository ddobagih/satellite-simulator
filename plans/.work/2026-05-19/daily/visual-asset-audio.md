# Computer Graphics Assignment 1 - Visual/Asset/Audio lane note (2026-05-20)

## 최근 진행 근거
- 2026-05-19 조사: 별도 `AGENTS.md`, `docs/`, `plans/daily/2026-05-20.md`는 확인되지 않아 사용자 제공 지침과 기존 문서를 기준으로 판단함.
- 2026-05-18 `README.md`, `WORK_LOG.md`: `SpaceSkybox.mat`, `SpaceStation.prefab`은 포함, 위성 모델/고급 지구 material/SFX는 optional fallback으로 설명됨.
- 2026-05-11 `asset-store-import-report.md`, 2026-05-19 파일 조사: `Assets/Resources/OrbitShare`에는 `SpaceSkybox.mat`, `SpaceStation.prefab`, `IMPORT_HERE.md`만 있음.
- 2026-05-18 `product/decisions.md`: optional 위성 모델/지구 material/SFX 추가와 Git LFS 작업은 진행하지 않고 fallback 유지로 결정됨.
- 2026-05-18 `daylog/2026-05-18.md`: 1600x900 `orbitshare-playmode.png` 기준으로 skybox, 지구/대기, 도시 marker, 24개 위성망, relay line, 완료 패널 preview가 보인다고 판독됨.
- 2026-05-18 `daylog/2026-05-18.md`, 2026-05-19 `daylog/2026-05-19.md`: 실제 마우스 조작 기반 GUI E2E와 실제 오디오 청취 확인은 아직 완료 근거로 확인되지 않음.
- 2026-05-18 `orbitshare-validation.md`: 자동 검증은 PASS이나, `product/done-criteria.md` 기준 에셋/UI 품질은 자동화 단독으로 완료 처리하지 않음.
- 2026-05-19 코드 조사: `OrbitShareBootstrap.cs`는 `PremiumEarthMaterial`, `SatelliteModel`, `SpaceStation`, `SpaceSkybox`를 `Resources.Load`로 읽고 누락 시 절차적 fallback을 사용함.
- 2026-05-19 코드/파일 조사: `AudioFeedbackManager.cs`는 `SFX_Reserve` 등 5종을 `Resources.Load`로 읽고 없으면 generated tone/chord를 만들며, 실제 `.wav/.mp3/.ogg/.aif/.aiff` 파일은 확인되지 않음.
- 2026-05-19 파일 조사: 50MB 초과 Cobble Games 텍스처와 `.unitypackage`가 다수 존재함.

## 내일 목표 후보
- 1순위: visual/audio asset freeze 상태 확인. 새 Asset Store/SFX 추가 없이 현재 `SpaceSkybox.mat`, `SpaceStation.prefab`, procedural/generated fallback 구성을 제출 기준으로 고정한다.
- 2순위: 발표 전 실제 Play 화면에서 skybox, station, 지구, 위성, relay line, 완료 preview, city label, UI 가독성을 다시 판독한다.
- 3순위: `빠른 발신`과 수동 `위성 확정 → 메시지 발신` 흐름 중 generated tone이 들리는지 직접 청취 확인한다.
- 4순위: `asset-store-import-report.md`가 실제 `Assets/Resources/OrbitShare` 상태와 맞는지 재확인하고, Unity 환경이 가능하면 Resource Hook 검증을 Build/Validation lane에 순차 요청한다.
- 5순위: `orbitshare-demo.gif`는 640x196 저해상도 자료이므로 최신 visual 근거로 쓸지 제외할지 결정한다.
- 6순위: README, WORK_LOG, ASSET_STORE_IMPORT, product 문서의 optional asset/fallback 표현을 최종 제출 기준과 맞춘다.

## 상세 체크리스트 초안
- [ ] `Assets/Resources/OrbitShare` 인벤토리 재확인 → 검증: `SpaceSkybox.mat`, `SpaceStation.prefab` 존재와 `SatelliteModel.prefab`, `PremiumEarthMaterial.mat`, SFX 5종 미존재를 `asset-store-import-report.md`와 대조한다.
- [ ] Resource Hook 검증 필요 여부 결정 → 검증: Unity 실행 환경이 확보되면 `Validate Resource Hooks` 재실행, 아니면 정적 인벤토리만 근거로 기록한다.
- [ ] Play 화면 visual review → 검증: 1600x900 또는 실제 발표 해상도에서 skybox/station/지구/위성/relay line/UI가 보이고 주요 텍스트가 겹치지 않는지 사람 판독 메모를 남긴다.
- [ ] station 가시성 확인 → 검증: Quick Relay/Fast Demo 중 `SpaceStation.prefab`가 카메라 컷과 UI에 가려지지 않는지 확인한다.
- [ ] 오디오 fallback 청취 확인 → 검증: 예약/시작/관측/전송/완료 단계의 generated tone이 실제로 들리는지 직접 확인하고, 미청취 시 완료로 쓰지 않는다.
- [ ] optional asset 추가 금지 확인 → 검증: 사용자 결정이 바뀌지 않으면 새 위성 모델/지구 material/SFX를 추가하지 않고 fallback 유지 사유만 문서화한다.
- [ ] 최신 시각 자료 근거 정리 → 검증: `orbitshare-playmode.png`를 최신 visual 근거로 쓰고, `orbitshare-demo.gif`/contact sheet가 오래된 표현이면 보조 자료로 낮춰 기록한다.
- [ ] 대용량/라이선스 리스크 점검 → 검증: 50MB 초과 Asset Store 리소스와 `.unitypackage`는 private repo/제출 범위 기준에서 문제 없는지 final 문서에 과대 표현하지 않는다.
- [ ] 문서 정합성 점검 → 검증: `README.md`, `WORK_LOG.md`, `ASSET_STORE_IMPORT.md`, `asset-store-import-report.md`, `product/decisions.md`가 “포함 에셋 2개 + optional fallback 유지”로 일치하는지 확인한다.

## 리스크/확인 필요
- 실제 GUI E2E와 실제 오디오 청취는 아직 완료 근거가 아니므로, 발표 안정성 판단에 남은 리스크가 있음.
- `asset-store-import-report.md`의 수정 시각은 2026-05-11이라 최신 visual review 이후 상태를 자동 검증한 리포트로 보기는 약함.
- SFX 파일이 없으므로 오디오는 generated tone 품질에 의존함. 발표 환경에서 음량/출력 장치 문제를 따로 확인해야 함.
- `orbitshare-demo.gif`는 640x196이라 최신 UI/visual 품질 근거로 쓰기 어렵다.
- optional asset을 뒤늦게 추가하면 자동 검증, 화면 판독, 캡처, 문서 갱신을 모두 다시 해야 하므로 제출 안정성에 불리함.
- Asset Store 리소스와 50MB 초과 파일이 있어 public repo나 압축 제출 범위를 바꾸는 경우 라이선스/용량 검토가 필요함.
- `WORK_LOG.md`의 “남은 개선점”과 `product/decisions.md`의 “optional asset/SFX dropped” 표현이 다르게 읽힐 수 있어 최종 기준은 2026-05-18 사용자 결정으로 통일해야 함.

## 병렬 에이전트 활용 메모
- 하위/병렬 에이전트는 사용하지 않았음.
- 이번 범위는 문서 확인, 정적 리소스 인벤토리, 관련 코드 hook 확인으로 충분히 좁고 파일 수정이 없어 단일 통합 판단으로 처리함.
- 파일 읽기와 정적 확인은 병렬 shell 호출로 나눠 수행했고, 최종 계획 파일은 수정하지 않았음.