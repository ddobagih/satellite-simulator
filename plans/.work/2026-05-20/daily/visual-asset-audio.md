# Computer Graphics Assignment 1 - Visual/Asset/Audio lane note (2026-05-21)

## 최근 진행 근거
- 2026-05-20 `plans/daily/2026-05-21.md`: 내일 visual lane 후보는 station 식별성 feature slice, Asset hook 화면 QA, generated tone 확인으로 정리되어 있음.
- 2026-05-20 `daylog/2026-05-20.md`: `orbitshare-playmode.png` 1600x900 최신 캡처 판독 완료. 단, 실제 버튼 조작 GUI E2E와 오디오 청취 근거는 아직 없음.
- 2026-05-20 `orbitshare-validation.md`: `Result: PASS`, 24개 위성, Fast demo, Reset replay 2회, Runtime Error/Exception 0. 에셋/UI 품질 최종 판정 근거로 과대해석 금지.
- 2026-05-20 `orbitshare-playmode.png`: 최신 완료 화면 근거. skybox/지구/UI는 확인 가능하나 station 식별성은 별도 확인 필요.
- 2026-05-20 `Assets/Resources/OrbitShare/`: `SpaceSkybox.mat`, `SpaceStation.prefab`, `IMPORT_HERE.md` 존재. `SatelliteModel.prefab`, `PremiumEarthMaterial.mat`, SFX 5종은 없음.
- 2026-05-15 `ASSET_STORE_IMPORT.md`: 런타임 hook 이름과 fallback 정책 명시. optional asset missing 시 절차적 visual/generated tone으로 동작.
- 2026-05-11 `asset-store-import-report.md`: skybox/station FOUND, satellite/earth material/SFX MISSING optional. 현재 파일 상태와 일치하지만 리포트 자체는 오래되어 최신 Unity hook 검증 근거로는 약함.
- 2026-05-18~20 `product/vision.md`, `product/backlog.md`, `product/decisions.md`, `product/done-criteria.md`: Asset Store skybox/station hook과 fallback 유지, optional 위성 모델/지구 material/SFX/LFS는 dropped. `P1-004 Asset hook 화면 QA`는 todo.
- 2026-05-20 확인: `plans/features`, `plans/verify/ready`, `plans/verify/reports`는 파일 없음. `PM/` 디렉터리도 없어 feature/verify scheduler, product audit, automation metrics 신규 근거는 없음.

## 내일 목표 후보
- 1순위: Station 식별성 feature slice. 발표 화면에서 `OrbitShare Station`이 서비스 인프라로 읽히지 않으면 새 에셋 없이 label/halo/beacon/framing 중 하나만 최소 보강.
- 2순위: Asset hook 화면 QA. `SpaceSkybox.mat`, `SpaceStation.prefab`이 실제 Play 화면에 반영되는지 캡처와 사람 판독으로 확인.
- 3순위: generated tone 발표 안정성 확인. 실제 출력 장치에서 예약/시작/관측/전송/완료 tone 청취 여부 기록.
- 4순위: Resource Hook 최신 리포트 갱신 여부 결정. Unity 실행 가능 시 Build/Validation lane 단일 runner로 `Validate Resource Hooks` 순차 실행.
- 5순위: 최신 시각 자료 기준 정리. `orbitshare-playmode.png`는 최신 visual 근거, `orbitshare-demo.gif`/contact sheet는 legacy 또는 보조 근거로 분리.
- 6순위: 문서 표현 통일. “포함 에셋 2개 + 나머지 optional fallback” 기준을 README/WORK_LOG/product에 전달.

## 상세 체크리스트 초안
- [ ] Station 식별성 판단 → 검증: 1600x900 또는 발표 해상도에서 station이 UI/카메라에 묻히는지 확인하고, 필요 시 label/halo/beacon/framing 최소 보강 후 전후 캡처 비교.
- [ ] runtime asset 인벤토리 재확인 → 검증: `SpaceSkybox.mat`, `SpaceStation.prefab` FOUND와 `SatelliteModel.prefab`, `PremiumEarthMaterial.mat`, SFX 5종 MISSING optional 상태를 report와 대조.
- [ ] Resource Hook 검증 최신화 조율 → 검증: Unity 가능 시 `OrbitShare > Asset Store > Validate Resource Hooks` 실행 결과와 report mtime 기록, 미실행 시 사유 기록.
- [ ] Play 화면 visual review → 검증: skybox, station, 절차적 지구/구름, 도시 marker, 24개 위성망, relay line, 완료 preview가 보이는지 사람 판독 메모 작성.
- [ ] generated tone 청취 확인 → 검증: `빠른 발신`과 수동 발신 중 phase tone이 실제 출력 장치에서 들리는지 기록. 듣지 못하면 완료로 쓰지 않음.
- [ ] 오디오 장치 불가 safe alternative 정리 → 검증: `AudioFeedbackManager`의 clip 생성과 phase event 호출만 확인하고, 음질/청취 검증으로 표현하지 않음.
- [ ] optional asset 추가 금지 유지 → 검증: 사용자 결정 변경 없으면 새 Asset Store/SFX import, Git LFS, 대용량 리소스 추가를 실행 계획에서 제외.
- [ ] visual evidence 정리 → 검증: 최신 PNG, legacy GIF/contact sheet, 자동 validation, 실제 GUI/audio 확인 여부를 서로 다른 증거 등급으로 문서화.

## 리스크/확인 필요
- 실제 Unity Editor GUI 조작과 실제 오디오 청취는 아직 완료 근거가 없다.
- 현재 캡처만으로 `SpaceStation.prefab` 식별성을 충분하다고 단정할 수 없다.
- SFX 파일이 없어 generated tone에 의존한다. 발표 환경의 출력 장치/음량/무음 설정이 리스크다.
- `asset-store-import-report.md`는 2026-05-11 산출물이라 최신 hook 검증 리포트로 보기 약하다.
- 외부 Asset Store 추가, 유료/계정 필요 asset, 대용량/LFS 변경은 자동 계획에서 직접 실행하지 않는다.
- Unity 실행이 막히면 정적 인벤토리와 기존 1600x900 캡처를 safe alternative로 쓰되, GUI/audio 완료로 표현하지 않는다.
- Asset Store 리소스와 50MB 초과 파일이 있어 public repo 전환이나 압축 제출 범위 변경 시 라이선스/용량 확인이 필요하다.

## 병렬 에이전트 활용 메모
- 하위/병렬 에이전트는 사용하지 않았음.
- 이번 범위는 기존 문서, 정적 리소스 인벤토리, code hook 확인으로 충분히 좁고 최종 파일을 직접 수정하지 않는 lane note 작성 작업이어서 단일 통합으로 처리함.
- 파일 읽기와 검색은 병렬 shell 호출로 나눴고, Unity 실행이 필요한 검증은 project lock/log 충돌 방지를 위해 내일도 단일 runner가 순차 수행해야 함.