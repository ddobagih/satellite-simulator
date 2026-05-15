# Computer Graphics Assignment 1 - Spec/Work Log/Submission Docs catch-up lane note (2026-05-16)

## 확인한 근거

- `plans/daily/2026-05-15.md`: Spec/문서 관련 항목은 대부분 `[x]`, 단 “최종 답변에는 실행한 것과 아직 하지 않은 것을 분리해 보고” 항목만 `[ ]`.
- `daylog/2026-05-15.md`: README/WORK_LOG/ASSET_STORE_IMPORT 갱신, 검증 PASS, Linux build Success, 발표/PPT/대본 미작업, Windows 빌드 미실행 기록 확인.
- `daylog/2026-05-16.md`: 현재 없음. 2026-05-16 실행 기록은 아직 확인 불가.
- `README.md`: 서울 고정 발신, 3개 목적지, 24개 위성, Quick Relay, 자동 검증, Asset Store fallback 설명 반영 확인.
- `WORK_LOG.md`: 작성일 2026-05-15로 최신 릴레이 컨셉, 검증, Linux 빌드, 스크린샷, GitHub 업로드, 남은 개선점 반영 확인.
- `ASSET_STORE_IMPORT.md`, `asset-store-import-report.md`: `SpaceSkybox.mat`, `SpaceStation.prefab` FOUND, `SatelliteModel`, `PremiumEarthMaterial`, SFX는 optional fallback으로 정리됨.
- 제출물 검색: `orbitshare-demo.gif`, `orbitshare-contact-sheet.png`, `DemoFrames/` 존재. `.ppt/.pptx/.pdf/.mp4/.mov`는 확인되지 않음.
- 빌드 산출물: `Builds/Linux/OrbitShare.x86_64` 존재. Windows 산출물은 확인되지 않음.
- `orbitshare_shared_satellite_agent.md`: 최종 제출 기준, 발표 Q&A, 성공 기준은 있으나 관측/공유 위성 중심의 구 컨셉 표현이 남아 있음.
- `git status --short --branch`: 코드/문서/검증 리포트 수정과 `orbitshare-playmode.png`, `plans/daily/2026-05-15.md` 미추적 상태 확인.

## 완료로 판단한 항목

- README는 현재 릴레이 컨셉과 대체로 일치함.
- WORK_LOG는 2026-05-15 기준 최신 컨셉, 검증, Linux 빌드, 스크린샷 상태를 반영함.
- Asset Store/fallback 문서는 현재 포함 에셋과 optional missing 상태를 설명함.
- 2026-05-15 daylog가 작성되어 변경 파일, 검증, 미실행 항목이 기록됨.
- 자동 검증 `Result: PASS`와 Runtime Error/Exception `PASS (0)` 근거가 있음.
- Linux 빌드 성공 및 산출물 존재 근거가 있음.
- 2026-05-15 계획에 따라 발표/PPT/대본 작업은 수행하지 않은 것으로 기록됨.

## 미완료 작업 후보

- [ ] 2026-05-16 실행 daylog 확인/작성 → 이유/근거: `daylog/2026-05-16.md`가 없음.
- [ ] 최종 제출물 인벤토리 작성 → 이유/근거: 제출 기준별 `존재/없음/확인 필요` 표가 아직 최종 산출물로 확인되지 않음.
- [ ] 발표 PPT/PDF 존재 여부 확정 → 이유/근거: 검색 결과 `.ppt/.pptx/.pdf` 없음.
- [ ] 시연 영상 형식 확정 → 이유/근거: GIF/프레임은 있으나 MP4 등 “시연 영상” 요구 충족 여부 불명확.
- [ ] Windows 빌드 필요 여부 확정 → 이유/근거: Linux 산출물만 있고 Windows 산출물은 없음.
- [ ] 발표 Q&A/성공 기준 현행화 → 이유/근거: `orbitshare_shared_satellite_agent.md`는 관측/공유 위성 중심 구 컨셉이 남아 현재 릴레이 컨셉과 불일치.
- [ ] 제출 전 Git 범위 정리 → 이유/근거: 수정/미추적 파일이 남아 커밋/제외/푸시 대상 확정 필요.
- [ ] 2026-05-15 최종 보고 항목 확인 → 이유/근거: 계획표의 “최종 답변 분리 보고” 체크박스가 미완료 상태.

## 오늘 catch-up 후보 스케줄

- [ ] 2026-05-16 daylog 생성/갱신 → 검증: 실제 수행한 검증, 변경 파일, 미실행 항목, 남은 리스크가 기록됨.
- [ ] 제출물 인벤토리 작성 → 검증: Unity 프로젝트, Linux 빌드, Windows 빌드, README/WORK_LOG, PPT/PDF, GIF/MP4, Submission 폴더 상태가 표로 분류됨.
- [ ] 발표자료/영상 요구사항 결정 → 검증: PPT/PDF와 MP4 필요 여부가 `완료/보류/확인 필요`로 기록됨.
- [ ] Q&A/성공 기준 최신화 또는 legacy 표시 → 검증: 서울 발신 릴레이, 24개 위성, 3개 목적지, 실제 한계가 현재 구현과 일치함.
- [ ] README/WORK_LOG/Asset 문서 최종 정합성 점검 → 검증: 포함 에셋과 optional fallback 설명이 문서 간 충돌하지 않음.
- [ ] Windows 빌드 제외/실행 근거 문서화 → 검증: 산출물 생성 또는 제외 사유가 제출 기준과 연결됨.
- [ ] Git 제출 범위 정리 → 검증: `git status --short --branch` 기준 커밋 대상, 제외 대상, 조건부 포함 대상이 분리됨.

## 확인 필요

- 제출 플랫폼이 Windows 실행 파일을 요구하는지, Linux 빌드만으로 충분한지.
- `orbitshare-demo.gif`가 시연 영상 제출물로 인정되는지, 별도 MP4가 필요한지.
- 발표 PPT/PDF가 필수인지, 외부 파일이 따로 있는지.
- `orbitshare_shared_satellite_agent.md`를 실제 제출/발표 문서로 계속 사용할지, legacy 기획 문서로 남길지.
- optional missing asset/SFX가 과제 필수인지, fallback 상태 문서화만으로 충분한지.
- UI/Build lane의 실제 GUI 리허설 결과를 성공 기준 매핑에 반영해야 하는지.

## 병렬 에이전트 활용 메모

- 새 하위/병렬 에이전트는 사용하지 않음.
- 이유: 현재 lane은 계획표, daylog, 문서, 제출물 존재 여부 대조가 중심이고 범위가 단일 조사로 충분했음.
- 통합 결론: 2026-05-15 문서 정리는 상당 부분 완료됐지만, 2026-05-16 catch-up에서는 제출물 인벤토리, PPT/영상 형식, Windows 빌드 필요 여부, Q&A/성공 기준 현행화, Git 제출 범위 정리가 남음.