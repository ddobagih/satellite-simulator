# Computer Graphics Assignment 1 - Visual/Asset/Audio lane note (2026-05-22)

## 최근 진행 근거
- 2026-05-21 `daylog/2026-05-21.md`: 오늘 신규 코드/에셋 변경은 없고, `Assets/Resources/OrbitShare/` 정적 인벤토리만 재확인됨. `SpaceSkybox.mat`, `SpaceStation.prefab`, `IMPORT_HERE.md` 존재, `SatelliteModel.prefab`, `PremiumEarthMaterial.mat`, SFX 5종은 optional missing.
- 2026-05-21 `daylog/2026-05-21.md`: Unity Editor GUI E2E, Unity batch validation, Linux 재빌드, 캡처 재생성은 오늘 실행하지 않음.
- 2026-05-20 `daylog/2026-05-20.md`: 1600x900 `orbitshare-playmode.png` 판독 완료. skybox/지구/UI/완료 화면은 확인됐지만 실제 마우스 조작과 오디오 청취 근거는 아님.
- 2026-05-20 `orbitshare-validation.md`: `Result: PASS`, 24개 위성, Fast demo 2회, Reset replay 2회, Runtime Error/Exception 0. 단, `Screen: 640x480`이라 발표 해상도 GUI E2E 근거로 쓰면 안 됨.
- 2026-05-18~21 `product/vision.md`, `product/backlog.md`, `product/decisions.md`, `product/done-criteria.md`: Asset Store skybox/station hook과 procedural/generated fallback을 유지하는 방향. optional 위성 모델/고급 지구 material/SFX/Git LFS는 dropped.
- 2026-05-18~21 `product/backlog.md`: `P1-004 Asset hook 화면 QA`는 아직 todo. `P2-004 SatelliteModel/Earth material/SFX 추가`는 dropped.
- 2026-05-15 `ASSET_STORE_IMPORT.md`, 2026-05-11 `asset-store-import-report.md`: 런타임 hook 이름은 `SatelliteModel.prefab`, `SpaceStation.prefab`, `PremiumEarthMaterial.mat`, `SpaceSkybox.mat`, `SFX_*.wav`; 현재 report는 skybox/station FOUND, 나머지 optional MISSING.
- `Assets/Scripts/OrbitShareBootstrap.cs`: `Resources.Load`로 `PremiumEarthMaterial`, `SatelliteModel`, `SpaceStation`, `SpaceSkybox`를 읽고 누락 시 절차적 지구/위성/station/starfield fallback 사용.
- `Assets/Scripts/AudioFeedbackManager.cs`: `SFX_Reserve/Start/Observe/Transmit/Complete`를 읽고 없으면 generated tone/chord 생성.
- 2026-05-21 확인: 로컬/상위 `AGENTS.md`는 별도 파일 없음. 사용자 제공 AGENTS 지침 적용.
- 2026-05-21 확인: `plans/daily/2026-05-22.md`는 없음. `plans/features`는 비어 있고, `plans/verify/ready`, `plans/verify/reports`도 파일 없음. `PM/` 디렉터리가 없어 verify status, product audit, automation metrics 신규 근거는 없음.

## 내일 목표 후보
- 1순위: `OrbitShare Station` 식별성 feature slice. 단순 에셋 QA 반복이 아니라, 단문 릴레이 서비스의 “인프라 거점”이 화면에서 읽히도록 label/halo/beacon/framing 중 하나만 최소 보강하는 방향을 우선 검토.
- 2순위: Asset hook 화면 QA. `SpaceSkybox.mat`, `SpaceStation.prefab`이 실제 Play 화면에 반영되는지 1600x900 또는 발표 해상도 캡처와 사람 판독으로 확인.
- 3순위: generated tone 발표 안정성 확인. 실제 출력 장치에서 phase tone이 들리는지 확인하고, 청취 불가 시 코드/phase event 확인까지만 기록.
- 4순위: Resource Hook 리포트 최신화 여부 결정. Unity 실행 가능 시 Build/Validation lane 단일 runner가 `Validate Resource Hooks`를 순차 실행하고 report mtime을 갱신.
- 5순위: 시각 근거 정리. `orbitshare-playmode.png`는 최신 정적 근거, `orbitshare-demo.gif`/contact sheet/DemoFrames는 legacy 또는 보조 근거로 분리.
- 6순위: 문서 lane에 handoff. “포함 에셋 2개 + 나머지 optional fallback” 기준이 README/WORK_LOG/product/Q&A에서 충돌하지 않게 전달.

## 상세 체크리스트 초안
- [ ] Station 식별성 판단 → 검증: 발표 해상도에서 `OrbitShare Station`이 서비스 인프라로 읽히는지 캡처 판독. 부족하면 새 에셋 없이 label/halo/beacon/framing 중 하나만 선택.
- [ ] Station 최소 보강 후보 확정 → 검증: 보강이 필요할 때만 `OrbitShareBootstrap.cs`, `SpaceStationAnimator.cs`, `MissionVisualController.cs` 범위에서 write set을 제한하고 전후 캡처 비교.
- [ ] runtime asset 인벤토리 재확인 → 검증: `SpaceSkybox.mat`, `SpaceStation.prefab` FOUND와 `SatelliteModel.prefab`, `PremiumEarthMaterial.mat`, SFX 5종 optional MISSING 상태를 `asset-store-import-report.md`와 대조.
- [ ] Play 화면 visual review → 검증: skybox, station, 절차적 지구/구름, 도시 marker, 24개 위성망, relay line, destination beam, 완료 preview가 실제 화면에서 보이는지 사람 판독 메모 작성.
- [ ] Resource Hook 검증 조율 → 검증: Unity 가능 시 `OrbitShare > Asset Store > Validate Resource Hooks` 실행 결과와 report mtime 기록. 미실행 시 정적 인벤토리만 근거로 남김.
- [ ] generated tone 청취 확인 → 검증: 예약/시작/관측/전송/완료 phase tone을 실제 출력 장치에서 청취. 듣지 못하면 “오디오 청취 미확인”으로 기록.
- [ ] 오디오 safe alternative 확인 → 검증: 출력 장치가 막히면 `AudioFeedbackManager`의 generated clip 생성과 mission phase event 연결만 확인하고 음질/청취 완료로 쓰지 않음.
- [ ] optional asset 추가 금지 유지 → 검증: 사용자 결정 변경 전에는 새 Asset Store import, SFX 추가, Git LFS, 대용량 리소스 정책 변경을 실행 계획에서 제외.
- [ ] visual evidence 등급화 → 검증: 자동 validation, 1600x900 PNG, GUI E2E, audio 청취, Resource Hook report를 서로 다른 증거 등급으로 문서화.

## 리스크/확인 필요
- 실제 Unity Editor GUI 조작과 실제 오디오 청취는 아직 완료 근거가 없다.
- `asset-store-import-report.md`는 2026-05-11 산출물이라 최신 hook 검증 리포트로 보기 약하다.
- 현재 SFX 파일이 없어서 오디오는 generated tone에 의존한다. 발표 환경의 음량/출력 장치 설정이 리스크다.
- Station 보강을 과하게 하면 발표 안정성보다 리스크가 커진다. safe alternative는 코드 변경 없이 캡처 판독 메모와 발표 설명 문구로 처리하는 것.
- Unity 실행이 막히면 정적 인벤토리와 기존 1600x900 캡처만 safe alternative로 사용한다. 이 경우 GUI/audio 완료로 표현하지 않는다.
- 새 Asset Store/SFX import, 유료/계정 필요 asset, Git LFS, 대용량 리소스 재정리는 현재 product 결정상 자동 실행 대상이 아니다.
- `plans/features`, `plans/verify`, `PM/status`, `PM/reports` 산출물이 없어 scheduler/product audit/automation metrics 기반 신규 근거는 반영할 수 없다.
- 작업트리에 코드 변경과 미추적 daylog/plan/product 문서가 섞여 있어 제출/커밋 포함 범위는 별도 정리가 필요하다.

## 병렬 에이전트 활용 메모
- 하위/병렬 에이전트는 사용하지 않았음.
- 이번 lane note는 기존 product/daylog/plan/code hook/resource inventory 확인만으로 범위가 좁고, 최종 파일을 직접 수정하지 않는 작업이라 단일 통합으로 처리함.
- 파일 검색과 읽기는 병렬 shell 호출로 나눴다.
- 내일 실제 실행 단계에서는 정적 asset inventory/문서 handoff는 병렬 가능하지만, Unity validation/capture/Resource Hook/audio 확인은 project lock/log 충돌 방지를 위해 단일 runner가 순차 수행해야 함.