# OrbitShare Work Log

작성일: 2026-05-15
최종 점검: 2026-05-16

## 작업 요약

Unity 2022.3.62f3 기준으로 `OrbitShare` 공유 위성 운용 시뮬레이션 프로젝트를 구성하고, 현재 컨셉을 `서울 고정 발신자 + Seoul/New York/London 목적지 선택 + 24개 위성 + 1회성 통신 릴레이`로 정리했다. 자동 검증, Linux 빌드, 최신 Play Mode 스크린샷 상태를 함께 확인했다.

## 구현한 주요 기능

- 런타임 생성형 3D 지구, 대기권, 별 배경, 궤도 링 구성
- 서울 고정 발신자와 Seoul/New York/London 목적지 선택 UI 구현
- 24개 릴레이 위성 배치와 위성/경로 선택 흐름 구현
- 발신자-위성-목적지 1회성 통신 릴레이 시나리오 구현
- 목적지 마커, relay lock 진행률, delivery 진행률 처리
- 통신선, 릴레이 경로, 미션 완료 상태 구현
- 시간 배속 제어: Pause, 1x, 5x, 10x
- 카메라 모드: View, Earth, Sat, Route
- 자동 검증용 `Fast Demo` 버튼 구현
- 릴레이 컨셉 중심의 미션 패널, 요약 패널, UI 문구 개선
- Unity Editor 메뉴 기반 Linux/Windows 빌드 스크립트 추가
- Play Mode 자동 검증 스크립트 및 결과 문서 생성

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
- `SFX`: 선택사항이며 전용 SFX가 없으면 fallback 처리 유지

에셋 원본 `.unitypackage`는 용량이 크기 때문에 `.gitignore`로 제외했다. 실제 프로젝트에는 import된 `Assets` 하위 리소스와 Unity 메타 파일이 포함되어 있다.

## 검증 결과

자동 검증 결과는 `orbitshare-validation.md`에 남겼다.

- 결과: PASS
- Fast Demo 미션 완료: PASS
- 24개 릴레이 위성 표시: PASS
- 서울 발신자와 Seoul/New York/London 목적지 구성: PASS
- Relay lock 진행률 100%: PASS
- Delivery 진행률 100%: PASS
- 미션 품질 점수: 71
- 서비스 신뢰도 점수: 91
- Runtime Error/Exception 로그: PASS (0)
- 최신 Play Mode 스크린샷: `orbitshare-playmode.png`

Unity Editor 자동화가 Play Mode에 진입한 뒤 검증 완료 후 종료되는 동작은 의도된 동작이며, 게임 크래시가 아니다.

## 빌드 및 실행 관련 작업

- Unity 프로젝트 설정 파일 구성
- `Assets/Scenes/MainScene.unity` 생성
- `Assets/Scripts/OrbitShareBootstrap.cs` 중심의 런타임 scene bootstrap 구조 구성
- `Assets/Editor/BuildOrbitShare.cs`로 빌드 메뉴 추가
- Linux 빌드 생성 및 결과 Success 확인
- 최신 Play Mode 스크린샷 `orbitshare-playmode.png` 확인

## 제출 상태 점검 (2026-05-16)

2026-05-16 catch-up에서 파일 존재 여부와 기존 검증 리포트 기준으로 제출 상태를 다시 점검했다. 이 점검에서는 Unity 자동 검증과 빌드를 새로 실행하지 않았고, 기존 `orbitshare-validation.md`와 `unity-build.log` 결과를 확인 근거로 사용했다.

| 항목 | 상태 | 근거 / 확인 필요 |
|---|---|---|
| Unity 프로젝트 | 존재 | `Assets/`, `Packages/`, `ProjectSettings/`, `Assets/Scenes/MainScene.unity` 확인 |
| README / 작업 로그 | 존재 | `README.md`, `WORK_LOG.md` 확인 |
| 자동 검증 리포트 | 존재 | `orbitshare-validation.md` 기준 `Result: PASS`, Runtime Error/Exception `PASS (0)` |
| Linux 실행 빌드 | 존재 | `Builds/Linux/OrbitShare.x86_64` 확인 |
| Windows 실행 빌드 | 없음 / 확인 필요 | `Builds/Windows/OrbitShare.exe` 미확인. 제출 플랫폼이 Windows 실행 파일을 요구하는지 확인 필요 |
| Play Mode 스크린샷 | 존재 | `orbitshare-playmode.png` 확인 |
| 시연 GIF / 프레임 | 존재 | `orbitshare-demo.gif`, `orbitshare-contact-sheet.png`, `DemoFrames/` 확인 |
| MP4/MOV 시연 영상 | 없음 / 확인 필요 | `.mp4`, `.mov` 파일 미확인. GIF가 시연 영상으로 인정되는지 확인 필요 |
| 발표 PPT/PDF | 없음 / 확인 필요 | `.ppt`, `.pptx`, `.pdf` 파일 미확인 |
| 제출 패키지 폴더 | 없음 / 보류 | `Submission/OrbitShare` 미생성. 최종 문서와 제출 형식 확정 후 생성 권장 |

## GitHub 업로드

GitHub private 저장소를 생성하고 현재 작업 브랜치를 업로드했다.

- 저장소: `https://github.com/ddobagih/orbitshare-assignment1`
- 공개 범위: Private
- 브랜치: `codex/orbitshare-unity-project`
- 최초 업로드 커밋: `d9c1027`

GitHub 업로드 중 일부 64MB 텍스처 파일에 대해 50MB 권장 크기 초과 경고가 나왔지만, GitHub 하드 제한인 100MB를 넘지 않아 푸시는 성공했다.

## 현재 주의사항

- `Library/`, `Logs/`, `UserSettings/`, `Builds/`, `AssetStorePackages/`는 Git 추적에서 제외했다.
- 프로젝트에는 큰 텍스처와 모델 파일이 포함되어 있어 clone/push 시간이 길 수 있다.
- `git-lfs`는 아직 적용하지 않았다.
- Asset Store 라이선스가 포함된 에셋을 사용하므로 저장소는 private으로 유지하는 것이 안전하다.
- 최신 Play Mode 스크린샷은 확보했으며, 추가 UI 미세 조정은 제출 직전 화면 확인에서 필요할 때만 진행한다.

## 남은 개선점

- Git LFS를 도입해 50MB 이상 텍스처와 모델 파일을 별도로 관리
- 위성 본체 전용 무료/경량 모델 추가 여부 검토
- UI 전용 SFX 또는 sci-fi HUD 에셋 추가 여부 검토
- 실제 궤도 역학, TLE 데이터, 지도 API 연동은 추후 확장 기능으로 분리
- 제출 직전 Play Mode 화면을 다시 확인하고 필요한 경우에만 카메라 구도와 UI 겹침을 미세 조정
