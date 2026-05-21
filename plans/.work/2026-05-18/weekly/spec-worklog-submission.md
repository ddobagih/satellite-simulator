# Computer Graphics Assignment 1 - Spec/Work Log/Submission Docs weekly lane note (2026-W21)

## 이번 주 목표 후보

- 제출 요구사항 확정표를 만든다: Unity 프로젝트, Linux/Windows 빌드, README/WORK_LOG, PPT/PDF, MP4/MOV, GIF/PNG, `Submission/` 상태를 `존재/없음/필수 여부 확인 필요`로 분리한다.
- `README.md`, `WORK_LOG.md`, `orbitshare_shared_satellite_agent.md`의 설명을 현재 릴레이 컨셉 기준으로 맞춘다.
- 기존 PASS 근거와 최신 미실행/환경 실패 검증을 문서에서 섞지 않는다.
- 발표 Q&A는 상단 최신 릴레이 컨셉만 기준으로 정리하고, legacy 관측 플랫폼 원문과 충돌하지 않게 한다.
- 성공 기준 매핑을 작성한다: 각 기준을 검증 리포트, GUI 리허설, 빌드 산출물, README/WORK_LOG, 제출물 검색 결과 중 하나와 연결한다.
- 제출 형식이 확정되면 `Submission/OrbitShare` 생성 여부와 포함 범위를 결정한다.
- Git/private repo/Git LFS/대용량 Asset Store 리소스 제출 방침을 정리한다.

## 날짜별/단계별 체크리스트

### 2026-05-18 월요일 - 현황 기준선 확정

- [ ] 최근 daylog와 daily/catchup 계획을 기준으로 제출 문서 lane 상태를 정리한다.  
  검증: `daylog/2026-05-17.md`, `daylog/2026-05-18.md`, `plans/daily/2026-05-18.md`, `plans/daily/2026-05-19.md`의 미완료 항목이 주간 계획에 반영됨.
- [ ] `plans/weekly/2026-W21.md` 부재를 기록한다.  
  검증: `plans/weekly/`가 비어 있음을 확인한 상태로 lane note만 출력함.
- [ ] 제출 산출물 인벤토리를 최신화한다.  
  검증: Linux 빌드/GIF/PNG/DemoFrames 존재, Windows/PPT/PDF/MP4/MOV/`Submission/` 부재가 구분됨.

### 2026-05-19 화요일 - 제출 요구사항 표와 문서 gap 정리

- [ ] 제출 요구사항 확정표 초안을 작성한다.  
  검증: 각 제출 항목이 `존재`, `없음`, `필수 여부 확인 필요`, `보류` 중 하나로 분류됨.
- [ ] `WORK_LOG.md` 업데이트 후보를 정리한다.  
  검증: 기존 `orbitshare-validation.md` PASS와 LicensingClient 이후 재실행 실패/미실행 항목이 분리됨.
- [ ] `README.md` 실행/시연 안내를 점검한다.  
  검증: `Queue`, `Relay`, `Quick Relay`, 자동 검증 명령, Asset Store fallback 설명이 현재 구현과 충돌하지 않음.

### 2026-05-20 수요일 - 검증/빌드 결과 반영

- [ ] Build/Validation lane 결과를 문서에 반영한다.  
  검증: 최신 자동 검증이 성공하면 새 PASS 근거를 기록하고, 실패하면 LicensingClient/DISPLAY/Windows Build Support 원인을 완료가 아닌 리스크로 남김.
- [ ] UI/Demo lane 결과를 발표 Q&A와 성공 기준에 반영한다.  
  검증: GUI 리허설, Reset 재실행, Play 재진입, 1600x900 UI 확인 결과가 있으면 근거 파일/로그와 연결됨.
- [ ] Visual/Asset lane 결과를 README/WORK_LOG/Asset 문서에 반영한다.  
  검증: `SpaceSkybox.mat`, `SpaceStation.prefab` FOUND와 optional fallback 상태가 같은 의미로 설명됨.

### 2026-05-21 목요일 - 발표 Q&A와 성공 기준 매핑

- [ ] `orbitshare_shared_satellite_agent.md` 상단 최신 Q&A를 최종 발표 기준으로 정리한다.  
  검증: 현재 컨셉이 `서울 고정 발신자 + 3개 목적지 + 24개 위성 릴레이 + 1회성 메시지 전달`로 설명됨.
- [ ] legacy 관측 플랫폼 설명은 발표 기준이 아님을 명확히 한다.  
  검증: 발표자가 하단 원문을 그대로 읽어 현재 구현과 충돌하지 않게 구분됨.
- [ ] 성공 기준 매핑표를 만든다.  
  검증: 각 성공 기준이 `README.md`, `WORK_LOG.md`, `orbitshare-validation.md`, GUI 리허설 기록, 빌드/산출물 검색 결과 중 하나와 연결됨.

### 2026-05-22 금요일 - 제출 패키지/저장소 방침 결정

- [ ] Windows 빌드 필요 여부를 확정한다.  
  검증: 필요하면 `Builds/Windows/OrbitShare.exe` 생성 근거가 있고, 불필요하면 제외 사유가 문서화됨.
- [ ] MP4/MOV와 PPT/PDF 필요 여부를 확정한다.  
  검증: `orbitshare-demo.gif` 대체 가능 여부와 외부 발표 자료 존재 여부가 확인됨.
- [ ] 대용량 에셋과 Asset Store 라이선스 방침을 정리한다.  
  검증: private repo 유지, Git LFS 도입 여부, 제출 패키지 제외 범위가 기록됨.

### 2026-05-23 토요일 - 최종 문서 통합

- [ ] README, WORK_LOG, Asset Store 문서, 발표 Q&A의 검증/빌드/에셋 상태를 맞춘다.  
  검증: 실행하지 않은 검증을 완료로 쓰지 않고, 문서 간 수치와 산출물 상태가 충돌하지 않음.
- [ ] 제출 형식이 확정된 경우에만 `Submission/OrbitShare` 구성을 진행한다.  
  검증: 포함 파일 목록과 제외 사유가 기록됨.
- [ ] Git 상태와 제출 포함 범위를 확인한다.  
  검증: daylog/plan 미추적 파일, 빌드 산출물 제외 여부, 대용량 리소스 정책이 정리됨.

### 2026-05-24 일요일 - 제출 전 최종 점검

- [ ] 최종 제출 체크리스트를 닫는다.  
  검증: 필수 제출물과 확인 필요 항목이 분리되어 있고, 미확정 항목이 완료로 표시되지 않음.
- [ ] 최종 daylog를 작성한다.  
  검증: 실제 변경 파일, 실행한 검증, 실패한 검증, 미실행 검증, 남은 확인 필요가 기록됨.
- [ ] 발표자가 사용할 Q&A/한 줄 설명/제한사항을 최종 확인한다.  
  검증: 실제 구현과 문서 설명이 같은 컨셉을 말함.

## 검증 계획

- 문서 정합성 검증: `README.md`, `WORK_LOG.md`, `ASSET_STORE_IMPORT.md`, `asset-store-import-report.md`, `orbitshare_shared_satellite_agent.md`의 컨셉/검증/에셋 설명이 서로 충돌하지 않는지 확인한다.
- 제출물 검증: Linux 빌드, Windows 빌드, GIF/PNG/DemoFrames, PPT/PDF, MP4/MOV, `Submission/` 존재 여부를 파일 검색으로 확인한다.
- 검증 리포트 판정: 현재 확실한 PASS 근거는 기존 `orbitshare-validation.md`이며 `Screen: 640x480` 기준이다. Reset 2회차, 1600x900 Canvas, IMGUI fallback, PresentationDirector camera cut 항목은 최신 리포트에 아직 반영되지 않았으므로 별도 재검증 전까지 PASS로 쓰지 않는다.
- Unity 실행 결과 반영: Build/Validation lane이 최신 자동 검증, GUI 리허설, Linux/Windows 빌드 결과를 제공하면 해당 로그와 결과만 문서에 반영한다.
- daylog 검증: 매일 실제 수행한 작업과 미실행 항목이 daylog에 분리되어 기록됐는지 확인한다.

## 리스크/확인 필요

- `plans/weekly/2026-W21.md`는 현재 확인되지 않았다.
- 별도 `docs/` 경로도 확인되지 않았다.
- Unity LicensingClient IPC 문제로 최신 자동 검증, Resource Hook 검증, Linux 재빌드가 재실행 실패한 이력이 있다.
- GUI 발표 리허설, Reset 후 재실행, Play Mode 재진입, 발표 해상도 UI 검증은 직접 PASS 근거가 부족하다.
- Windows 빌드, PPT/PDF, MP4/MOV, `Submission/`은 현재 확인되지 않았다.
- `orbitshare-demo.gif`가 시연 영상으로 인정되는지 확인 필요하다.
- `SatelliteModel.prefab`, `PremiumEarthMaterial.mat`, SFX 5종은 optional fallback 상태다.
- 50MB 초과 에셋과 Asset Store 리소스가 있어 private repo 유지, Git LFS, 제출 제외 범위 결정이 필요하다.
- 현재 작업트리에 daylog/plan 문서 변경 및 미추적 파일이 남아 있어 최종 제출 전 포함/제외 정리가 필요하다.

## 병렬 에이전트 활용 메모

- 이번 lane note 작성에는 별도 하위/병렬 에이전트를 사용하지 않았다.
- 이유: 담당 범위가 기존 문서, daylog, 계획 파일, 산출물 상태를 통합 판단하는 작업이며, 같은 최종 문서를 여러 에이전트가 동시에 수정할 필요가 없었다.
- 병렬화가 필요해지는 시점은 주중 실행 단계다. 제출물 인벤토리, 발표 Q&A 초안, 성공 기준 매핑, 대용량/Git 정책 조사는 병렬 가능하지만, 최종 README/WORK_LOG/daylog 통합은 단일 담당자가 수행해야 한다.