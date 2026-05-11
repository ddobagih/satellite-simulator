# OrbitShare Work Log

작성일: 2026-05-11

## 작업 요약

Unity 2022.3.62f3 기준으로 `OrbitShare` 공유 위성 운용 시뮬레이션 프로젝트를 구성하고, 과제 제출/시연에 필요한 실행 흐름, 에셋 연동, 자동 검증, GitHub 업로드까지 진행했다.

## 구현한 주요 기능

- 런타임 생성형 3D 지구, 대기권, 별 배경, 궤도 링 구성
- 개인/기업/공유 위성 3종 목록 및 선택 UI 구현
- 공유 위성 `SAT-S01` 예약, 관측 목표 선택, 미션 시작 흐름 구현
- 서울/제주/태평양 관측 지점 마커와 관측 진행률 처리
- 지상국 연결 범위, 통신선, 데이터 전송 진행률, 미션 완료 상태 구현
- 시간 배속 제어: Pause, 1x, 5x, 10x
- 카메라 모드: Overview, Earth, Satellite, Mission
- 발표 안정성을 위한 `Fast Demo` 버튼 구현
- 미션 타임라인, 지능형 미션 패널, 최종 결과 요약 패널 구현
- 예약/관측/전송/완료 단계별 오디오 피드백 구조 구현
- Unity Editor 메뉴 기반 Linux/Windows 빌드 스크립트 추가
- Play Mode 자동 검증 스크립트 및 결과 문서 생성

## 추가한 에셋

Unity Asset Store에서 다운로드한 무료 에셋을 프로젝트에 반영했다.

- `FREE Skyboxes - Space`
  - 우주 배경 skybox로 사용
  - 런타임 후크: `Assets/Resources/OrbitShare/SpaceSkybox.mat`
- `Space Station Free 3D Asset HDRP URP Built-In`
  - OrbitShare Station 오브젝트 대체용 3D 모델로 사용
  - 런타임 후크: `Assets/Resources/OrbitShare/SpaceStation.prefab`

에셋 원본 `.unitypackage`는 용량이 크기 때문에 `.gitignore`로 제외했다. 실제 프로젝트에는 import된 `Assets` 하위 리소스와 Unity 메타 파일이 포함되어 있다.

## 검증 결과

자동 검증 결과는 `orbitshare-validation.md`에 남겼다.

- 결과: PASS
- Fast Demo 미션 완료: PASS
- 관측 진행률 100%: PASS
- 전송 진행률 100%: PASS
- 미션 품질 점수: 85
- 서비스 신뢰도 점수: 93

Unity Editor 자동화가 Play Mode에 진입한 뒤 검증 완료 후 종료되는 동작은 의도된 동작이며, 게임 크래시가 아니다.

## 빌드 및 실행 관련 작업

- Unity 프로젝트 설정 파일 구성
- `Assets/Scenes/MainScene.unity` 생성
- `Assets/Scripts/OrbitShareBootstrap.cs` 중심의 런타임 scene bootstrap 구조 구성
- `Assets/Editor/BuildOrbitShare.cs`로 빌드 메뉴 추가
- Linux 빌드 생성 및 런타임 로그 확인
- 데모 프레임 캡처 및 GIF/연락 시트 생성

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
- Unity 실제 GUI 실행 검증은 로컬 환경 제약 때문에 자동 검증과 빌드/로그 확인 중심으로 진행했다.

## 남은 개선점

- Git LFS를 도입해 50MB 이상 텍스처와 모델 파일을 별도로 관리
- 위성 본체 전용 무료/경량 모델을 추가해 현재 스테이션 중심 시각 자산을 보완
- UI 전용 SFX 또는 sci-fi HUD 에셋을 추가해 발표 품질 강화
- 실제 궤도 역학, TLE 데이터, 지도 API 연동은 추후 확장 기능으로 분리
- Unity GUI에서 최종 시연 화면을 직접 확인하고 카메라 구도와 UI 겹침을 미세 조정
