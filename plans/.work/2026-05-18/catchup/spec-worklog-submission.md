# Computer Graphics Assignment 1 - Spec/Work Log/Submission Docs catch-up lane note (2026-05-18)

## 확인한 근거

- `plans/daily/2026-05-17.md`: Spec/Work Log/Submission Docs 항목은 모두 `[ ]` 상태.
- `daylog/2026-05-17.md`: 정적 점검은 수행됨. 단, Unity 자동 검증, GUI 리허설, Linux/Windows 빌드 재실행은 수행하지 않았다고 명시됨.
- `daylog/2026-05-18.md`: 없음.
- `plans/.work/2026-05-17/daily/spec-worklog-submission.md`: 2026-05-18로 제출 요구사항 확정, 문서 정합성, 발표 Q&A, 성공 기준 매핑, Git/대용량 에셋 방침이 승계됨.
- `WORK_LOG.md`: 2026-05-16 제출 상태 표는 있으나 Windows/PPT/PDF/MP4/Submission은 없음 또는 확인 필요.
- `README.md`: 실행 방법과 `Queue`/`Relay`/`Quick Relay` 흐름은 정리되어 있음.
- `orbitshare_shared_satellite_agent.md`: 상단 최신 Q&A와 성공 기준은 있으나 하단 legacy 관측 플랫폼 원문이 남아 있음.
- `orbitshare-validation.md`: `Result: PASS`이나 `Screen: 640x480`; Reset 2회차, 1600x900 Canvas, IMGUI fallback, PresentationDirector camera cut 항목은 없음.
- 산출물 검색: `Builds/Linux/OrbitShare.x86_64`, `orbitshare-demo.gif`, `orbitshare-playmode.png`, `orbitshare-contact-sheet.png`, `DemoFrames/` 존재. `Builds/Windows/OrbitShare.exe`, `.ppt/.pptx/.pdf`, `.mp4/.mov`, `Submission/` 없음.

## 완료로 판단한 항목

- `daylog/2026-05-17.md` 작성: 변경 파일, 검증 확인, 미실행 검증, 남은 확인 필요가 기록됨.
- 제출 산출물 정적 인벤토리 확인: Linux 빌드, README/WORK_LOG, 검증 리포트, GIF/PNG 존재와 Windows/PPT/PDF/MP4/Submission 부재가 기록됨.
- 기존 문서 기반 확인: README 실행 흐름, WORK_LOG 제출 상태 표, 발표 Q&A 상단 최신 섹션은 존재함.

## 미완료 작업 후보

- [ ] 제출 요구사항 확정표 최종화 → 이유/근거: Windows 빌드, PPT/PDF, MP4/MOV, GIF 대체 가능 여부, `Submission/` 필수 여부가 아직 확인 필요.
- [ ] `WORK_LOG.md` 최종 점검/갱신 → 이유/근거: 2026-05-17 LicensingClient 실패와 최신 미실행 검증이 WORK_LOG 최종 상태로 반영됐는지 불명확.
- [ ] `README.md` 실행/시연 안내 최종 점검 → 이유/근거: 기본 흐름은 있으나 최신 GUI/검증 실패 및 제출 기준 반영 여부는 계획 체크박스 기준 미완료.
- [ ] 발표 Q&A 최신화 → 이유/근거: 상단 최신 섹션은 있으나 legacy 원문과 현재 릴레이 컨셉 충돌 가능성이 계속 확인 필요로 남음.
- [ ] 성공 기준 매핑 작성 → 이유/근거: 성공 기준 목록은 있으나 각 기준이 README/WORK_LOG/검증 리포트/GUI 리허설/산출물 근거와 1:1로 매핑된 문서는 확인되지 않음.
- [ ] Build/UI/Visual 결과 문서 반영 → 이유/근거: 최신 Unity 검증, GUI 리허설, Resource Hook, Linux 재빌드가 LicensingClient/DISPLAY 문제로 완료되지 않음.
- [ ] 제출 패키지 생성 여부 결정 → 이유/근거: `Submission/` 폴더 없음. 제출 형식 확정 전 생성 보류 상태.
- [ ] Git 제출 범위 정리 → 이유/근거: `git status`와 대용량 에셋 리스크는 기록됐지만 Git LFS/private repo/제출 제외 방침은 미확정.
- [ ] `daylog/2026-05-18.md` 작성 → 이유/근거: 대상 날짜 daylog가 아직 없음.

## 오늘 catch-up 후보 스케줄

- [ ] 제출 요구사항 확정표 작성 → 검증: Unity 프로젝트, Linux/Windows 빌드, README/WORK_LOG, PPT/PDF, MP4/MOV, GIF, PNG, `Submission/`을 `존재/없음/필수 여부 확인 필요`로 분류.
- [ ] 문서 최종 점검 순서 확정 → 검증: README, WORK_LOG, 발표 Q&A, 성공 기준이 기존 PASS와 최신 실패/미실행 검증을 섞어 쓰지 않음.
- [ ] Build/UI/Visual lane 결과 대기 후 반영 → 검증: 새 실행 결과가 있으면 성공/실패 로그를 반영하고, 없으면 미실행 사유를 명시.
- [ ] 제출 패키지 생성 판단 보류/실행 조건 기록 → 검증: 제출 형식 확정 전에는 `Submission/OrbitShare` 생성하지 않음.
- [ ] Git/대용량 에셋 제출 방침 정리 → 검증: private repo 유지, Git LFS 적용 여부, 빌드 산출물 제외 여부가 daylog 또는 WORK_LOG에 기록됨.
- [ ] `daylog/2026-05-18.md` 작성 → 검증: 실제 변경 파일, 실행/실패/미실행 검증, 남은 확인 필요가 기록됨.

## 확인 필요

- Windows 실행 파일이 제출 필수인지.
- 발표 PPT/PDF가 필수인지, 외부 파일이 따로 있는지.
- `orbitshare-demo.gif`가 시연 영상으로 인정되는지, 별도 MP4/MOV가 필요한지.
- Unity LicensingClient IPC와 GUI/DISPLAY 환경을 사용할 수 있는지.
- 최신 자동 검증의 r1 추가 항목을 PASS로 갱신할 수 있는지.
- `SatelliteModel.prefab`, `PremiumEarthMaterial.mat`, SFX 5종이 optional fallback으로 충분한지.
- 50MB 초과 에셋을 Git LFS로 옮길지, private repo 유지 또는 제출 제외로 처리할지.
- 현재 미추적 `plans/daily/2026-05-18.md`를 merge 에이전트가 공식 계획으로 사용할지.

## 병렬 에이전트 활용 메모

- 하위/병렬 에이전트는 사용하지 않음.
- 이번 범위는 문서와 로그 대조 중심이라 단일 판단으로 충분했음.
- 파일 읽기와 검색은 병렬 shell 호출로 처리했고, catch-up 파일은 수정하지 않음.