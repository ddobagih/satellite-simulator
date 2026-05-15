# Computer Graphics Assignment 1 - UI/Demo Flow/Presentation catch-up lane note (2026-05-16)

## 확인한 근거

- `plans/daily/2026-05-15.md`: UI 품질, 검증/빌드, README/WORK_LOG 정합성 항목 대부분 `[x]`.
- `daylog/2026-05-15.md`: UI 문구 정리, `UIManager`/`ImmediateUIManager` 개선, `orbitshare-playmode.png` 1600x900 생성, 자동 검증 PASS, Linux 빌드 Success 기록.
- `orbitshare-validation.md`: `Result: PASS`, `Fast demo reaches mission complete: PASS`, Runtime Error/Exception `PASS (0)`.
- `WORK_LOG.md`, `README.md`: Fast Demo, Runtime Canvas UI, optional IMGUI fallback, presentation camera cuts 구현 기록.
- `plans/daily/2026-05-16.md`: 실제 GUI 발표 리허설, UI 겹침, 카메라 컷, Reset/Play 재진입은 아직 직접 근거 부족으로 재확인 대상.
- `daylog/2026-05-16.md`: 현재 작업트리에서 확인되지 않음.
- 보조 확인: `orbitshare-playmode.png`는 1600x900 PNG로 존재. `orbitshare-demo.gif`와 `DemoFrames/`는 존재하지만 최신 UI 변경 반영 여부는 불확실.

## 완료로 판단한 항목

- 2026-05-15 계획의 UI 화면 품질 점검 및 최소 수정
  - 근거: 최신 캡처 기반으로 패널 투명도, 라벨 위계, 궤도선 강조, 버튼 텍스트 잘림, Canvas/IMGUI 중복 표시 문제를 조정했다고 daylog에 기록됨.
- Fast Demo 자동 검증
  - 근거: `orbitshare-validation.md`에서 Mission Complete, relay lock 100%, delivery 100%, Runtime Error/Exception 0 확인.
- `UIManager`, `ImmediateUIManager` 관련 발표 화면 안정화 일부
  - 근거: Canvas UI 정상 생성, `ImmediateUIManager` 기본 비활성 fallback 처리, 짧은 버튼/문구 적용 기록.
- 1600x900 기준 최신 스크린샷 확보
  - 근거: `orbitshare-playmode.png` 생성 및 캡처 로그에서 `Arial.ttf`, `ArgumentException` 재발 없음 기록.
- UI 수정 후 기본 검증
  - 근거: `git diff --check` 통과, Unity 자동 검증 PASS, Linux 빌드 Success 기록.
- 발표/PPT/대본 비작업 결정
  - 근거: 2026-05-15 계획과 daylog에 사용자 지시에 따라 제외했다고 기록됨.

## 미완료 작업 후보

- [ ] 실제 Unity GUI 발표 리허설 → 이유/근거: 자동 검증과 스크린샷 근거는 있으나, 발표자가 수동으로 `Reserve → Start → 10x → 완료` 흐름을 진행한 기록은 없음.
- [ ] 발표 해상도/실행 환경 UI 가독성 확인 → 이유/근거: 1600x900 캡처는 있으나 실제 발표 예정 해상도에서 패널, 버튼, 결과 UI가 잘리지 않는지 검증 기록 없음.
- [ ] Reset 후 Fast Demo 재실행 → 이유/근거: 중복 구독 방지 코드는 보강됐지만, Reset 뒤 두 번째 Mission Complete까지 도달했다는 실행 기록 없음.
- [ ] Play Mode 종료 후 재진입 안정성 확인 → 이유/근거: bootstrap static 초기화 기록은 있으나 두 번째 Play에서 UI 이벤트 중복/예외 없음 확인 기록은 없음.
- [ ] `PresentationDirector` 단계별 카메라 컷 확인 → 이유/근거: 구현/README 근거는 있으나 예약/이동/관측/전송/완료 단계별 카메라 컷이 발표 흐름을 방해하지 않는지 실제 QA 기록 없음.
- [ ] 최신 데모 프레임/GIF 갱신 여부 결정 → 이유/근거: `orbitshare-demo.gif`, `DemoFrames/`는 존재하지만 2026-05-15 UI 변경 이후 산출물인지 불확실함.
- [ ] 2026-05-16 새벽 작업 반영 여부 → 이유/근거: `daylog/2026-05-16.md`가 없어 실행 완료 근거를 확인할 수 없음.

## 오늘 catch-up 후보 스케줄

- [ ] 발표 해상도 기준 Play Mode 실행 → 검증: 위성 목록, 상세/상태 패널, 시간/카메라 버튼, 결과 패널의 잘림·겹침 여부 기록.
- [ ] 수동 Demo Flow 1회 리허설 → 검증: `Reserve → Start → 10x → relay/delivery 완료`를 2~3분 안에 진행하고 Mission Complete 확인.
- [ ] Fast Demo 단일 클릭 리허설 → 검증: 사용자 추가 조작 없이 Mission Complete 도달, Runtime Error/Exception 0.
- [ ] Reset 후 Fast Demo 재실행 → 검증: 두 번째 완료까지 버튼 상태, 패널 갱신, 이벤트 중복 호출 이상 없음.
- [ ] Play Mode 종료 후 재진입 확인 → 검증: 두 번째 Play에서 Fast Demo가 동일하게 동작하고 예외 로그 없음.
- [ ] `PresentationDirector` 카메라 컷 QA → 검증: 단계별 Overview/Satellite/Mission 컷이 발표 흐름을 방해하지 않는지 표로 기록.
- [ ] 데모 백업 산출물 갱신 여부 결정 → 검증: 필요 시 승인된 방식으로만 프레임/GIF 갱신, 아니면 기존 산출물 보류 사유 기록.
- [ ] 문제가 재현될 때만 최소 UI 수정 → 검증: 수정 후 스크린샷 또는 리허설 재확인, `git diff --check`, 자동 검증 PASS.
- [ ] 실제 실행 결과를 `daylog/2026-05-16.md`에 기록 → 검증: 실행한 검증과 미실행 항목이 분리되어 남음.

## 확인 필요

- `daylog/2026-05-16.md`가 없어 2026-05-16 새벽 작업 완료 여부는 확인 불가.
- 실제 발표 해상도와 발표 실행 환경이 1600x900인지 확인 필요.
- 사용자가 “새 자동화 스크린샷 코드는 쓰지 말라”고 한 기록이 있으므로, 캡처/프레임 갱신 방식은 사전 확인 필요.
- 발표/PPT/대본 제외 결정이 2026-05-16 catch-up에서도 유지되는지 확인 필요.
- `orbitshare-demo.gif`가 제출/발표 백업으로 충분한지, MP4나 최신 프레임이 필요한지는 submission lane과 조율 필요.
- 현재 작업트리에 UI 관련 수정 파일과 `orbitshare-playmode.png`, `plans/daily/2026-05-15.md` 미추적 상태가 남아 있어 merge 단계에서 커밋/제외 분류 필요.

## 병렬 에이전트 활용 메모

- 이번 확인 작업에서는 새 하위/병렬 에이전트를 사용하지 않음.
- 기존 `daylog/2026-05-15.md`에는 UI/시각 점검 등 병렬 에이전트 결과가 통합되어 있었고, 결론은 “컨셉 전달은 양호하나 화면이 약간 복잡함”이었다.
- 현재 lane 결론: 5/15 UI 정리와 자동 Fast Demo 검증은 완료 근거가 충분하지만, 5/16 catch-up의 핵심은 실제 GUI 리허설, Reset/Play 재진입, `PresentationDirector` 카메라 컷 QA다.
- 읽기 전용 조사만 수행했으므로 daylog는 작성하지 않음.