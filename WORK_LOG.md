# OrbitShare Work Log

작성일: 2026-05-15
최종 점검: 2026-05-25

## 작업 요약

Unity 2022.3.62f3 기준으로 `OrbitShare` 단발성 위성 단문 발신 프로젝트를 구성했다. 현재 컨셉은 `서울 고정 발신자 + 7개 목적지 + 발신 시각/메시지/첨부 선택 + 24개 후보 위성 필터링 + 1회성 통신 릴레이`다.

수동 Unity Editor GUI 리허설은 사용자 지시로 더 이상 별도 리스크나 제출 게이트로 추적하지 않는다. 문서와 검증 상태는 자동 검증, build/display smoke, 캡처, 정적 확인 기준으로만 분리해 기록한다.

## 구현한 주요 기능

- 런타임 생성형 3D 지구, 대기권, 별 배경, 궤도 링 구성
- 외부 지구 텍스처/구름/대기권 shader와 절차적 fallback 구성
- 서울 고정 발신자와 7개 목적지 선택 UI 구현
  - Seoul, New York, London, Tokyo, Paris, Sydney, Los Angeles
- 24개 릴레이/후보 위성 배치와 스크롤 목록, 전체/가능/빠른창/저가/신뢰/혼잡도 필터 구현
- 후보 위성별 7개 목적지 지원 여부, 현재 위성 위치 기반 가시권, 요금, 지연, 신뢰도, 혼잡도 표시
- 7개 목적지 이름/라벨은 `OrbitShareDestinations`, 위성 route estimate 계산은 `SatelliteRouteEstimator`로 공통화
- 발신자-위성-목적지 1회성 통신 릴레이 시나리오 구현
- 메시지 UTF-8 용량 계산, 다중 첨부 합산, 512KB payload 제한 구현
- 진행 중 추가 메시지 작성, 메시지 목록 수정/보류/취소/바로 보내기 흐름 구현
- 하단 메뉴를 메시지 작성/위성 후보/전송 상태/진행/기록/대기/통계/설정/시연으로 정리하고 메시지 템플릿 탭 제거
- 하단 메뉴 패널은 같은 버튼을 다시 누르면 닫히도록 통일하고, 메뉴별/작성 단계별 캡처 자동화로 레이아웃을 확인
- 상단 카메라 프리셋 버튼을 제거하고 목적지/시연 흐름 중심 UI로 단순화
- 메시지 작성 5단계의 설명글을 숨기고 시간 달력, 메시지/확인 레이아웃, 진행 창 확대, 시연 중지 UX를 정리
- 전송 기록, 기록 재사용, 경로 재생, 실패 시 재시도 흐름 구현
- 목적지 마커, relay lock 진행률, delivery 진행률 처리
- 통신선, 릴레이 경로, 전송 패킷, 완료/실패 상태 시각화
- 발표용 주요 Runtime Canvas UI 한글화
- 자동 검증용 fast demo와 UI의 시연 전송 흐름 구현
- Unity Editor 메뉴 기반 Linux/Windows 빌드 스크립트 추가
- Play Mode 자동 검증 스크립트 및 결과 문서 생성 기능 추가

## 추가한 에셋

Unity Asset Store에서 다운로드한 무료 에셋 중 현재 반영된 항목은 다음과 같다.

- `FREE Skyboxes - Space`
  - 우주 배경 skybox로 사용
  - 런타임 후크: `Assets/Resources/OrbitShare/SpaceSkybox.mat`
- `Space Station Free 3D Asset HDRP URP Built-In`
  - OrbitShare Station 오브젝트 대체용 3D 모델로 사용
  - 런타임 후크: `Assets/Resources/OrbitShare/SpaceStation.prefab`

선택/대체 리소스 상태는 다음과 같이 정리했다.

- `SatelliteModel`: 전용 위성 모델이 없으면 런타임 생성형/fallback 표현 사용
- `PremiumEarth`: 선택사항이며 기본 생성형 지구/fallback 유지
- `SFX`: 선택사항이며 전용 SFX가 없으면 generated tone fallback 유지

외부 지구 텍스처 출처와 라이선스는 `Assets/Resources/OrbitShare/Earth/ATTRIBUTION.md`에 정리되어 있다.

## 검증/증거 상태

| 항목 | 현재 상태 | 근거 |
|---|---|---|
| 정적 문법/공백 검사 | PASS | `git diff --check` |
| 실행 스크립트 문법 | PASS | `bash -n run_orbitshare.sh` |
| 최신 Linux build | PASS | `unity-build-ux-calendar.log`: `Build Finished, Result: Success.` |
| 최신 Play 캡처 | 존재 | `orbitshare-playmode.png`, 1600x900 |
| 하단 메뉴/작성 단계 캡처 | PASS | `orbitshare-menu-captures/report.md`: 8개 패널 열기/재클릭 닫기, 작성 5단계, 직접 설정 달력 캡처 PASS |
| display smoke | PASS 근거 있음 | `build-run-live.log`에서 Display/GfxDevice 생성 및 Error/Exception 매치 없음 |
| 자동 검증 리포트 파일 | PASS | `orbitshare-validation.md`: `Result: PASS`, 7개 목적지/추천 스냅샷 포함 |
| 수동 GUI 리허설 | 추적하지 않음 | 2026-05-25 사용자 지시로 별도 리스크/게이트에서 제외 |

## 제출 상태

| 항목 | 상태 | 근거 / 처리 |
|---|---|---|
| Unity 프로젝트 | 포함 | `Assets/`, `Packages/`, `ProjectSettings/`, `Assets/Scenes/MainScene.unity` |
| README / 작업 로그 | 포함 | `README.md`, `WORK_LOG.md` |
| Linux 실행 빌드 | 존재 | `Builds/Linux/`, 최신 Data 파일은 2026-05-25 갱신 |
| Windows 실행 빌드 | 제외 | Unity 프로젝트 자체 제출 기준 |
| MP4/MOV 시연 영상 | 제외 | 제출 필수 아님 |
| 발표 PPT/PDF | 제외 | 제출 필수 아님 |
| `Submission/OrbitShare` | 제외 | 별도 제출 폴더 불필요 |
| optional 위성 모델/SFX/LFS | 제외 | fallback 유지 결정 |

## GitHub 업로드 참고

GitHub private 저장소를 생성하고 작업 브랜치를 업로드한 이력이 있다.

- 저장소: `https://github.com/ddobagih/orbitshare-assignment1`
- 공개 범위: Private
- 브랜치: `codex/orbitshare-unity-project`

Asset Store 리소스와 대용량 텍스처가 포함될 수 있으므로 저장소는 private 유지가 안전하다.

## 현재 주의사항

- `Library/`, `Logs/`, `UserSettings/`, `Builds/`, `AssetStorePackages/`는 `.gitignore` 대상이다.
- 현재 작업트리에는 대량 에셋 삭제, 신규 지구 텍스처/shader/script, 문서/daylog 변경이 함께 남아 있다.
- `orbitshare-validation.md`는 2026-05-25 재생성했고 최신 자동 검증 결과는 PASS다.
- 가격/성공률/위성 가용성은 발표용 시뮬레이션 값이며 실제 결제/실제 통신망 데이터가 아니다.

## 남은 개선 후보

- 24개 후보 위성 동적 추천은 7개 목적지 기준으로 반영됨. 필요 시 지연 시각별 미래 위치 예측을 추가 개선
- Git 포함/제외 인벤토리 정리
- 대용량 에셋 정책은 제출 방식이 바뀔 때만 재검토
