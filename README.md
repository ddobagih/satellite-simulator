# OrbitShare

사용자가 발신지를 고르고 짧은 메시지와 필요한 파일을 작성한 뒤, 7개 목적지 중 하나와 위성 중계 창을 골라 한 번 전송하는 Unity 3D 시연 프로젝트입니다.

## 실행 방법

1. Unity Hub에서 이 폴더를 Unity 프로젝트로 엽니다.
2. `Assets/Scenes/MainScene.unity`를 엽니다.
3. Play를 누릅니다.

장면은 실행 시 `Assets/Scripts/OrbitShareBootstrap.cs`가 구성합니다.

## 발표용 핵심 시나리오

1. **시간 선택**: 지금 보내기/5분/15분/30분 또는 직접 설정 달력에서 날짜와 시간을 고릅니다.
2. **메시지/첨부**: 메시지를 입력하고 필요한 파일만 첨부합니다.
3. **목적지 선택**: 서울/뉴욕/런던/도쿄/파리/시드니/LA 중 목적지 카드나 지구 위 목적지 핀을 클릭합니다.
4. **위성 추천/선택**: 선택한 시간과 목적지 기준으로 CelesTrak 실제 TLE 위성 후보를 비교하고 하나를 고릅니다.
5. **전송/메시지 목록/기록**: 확인 단계에서 `전송 시작`을 누르고, 진행 중 추가 메시지는 현재 전송 뒤 이어서 보내도록 작성합니다. 시연 중에는 `시연 중지`로 즉시 작성 화면으로 돌아갈 수 있습니다.

## 조작 방법

- 우클릭 드래그: UI가 아닌 지구 화면 어디서든 시점 돌려보기
- 지구 좌클릭 드래그: 지구 표면을 끌어 시점 돌려보기
- 마우스 휠 또는 Q/E: 지구/경로 화면 확대·축소
- 방향키 또는 WASD: 지구 시점 미세 회전
- R: 지구 시점 초기화
- 목적지 클릭: 지구 위 목적지 핀 또는 목적지 목록에서 선택
- 위성 클릭: 3D 위성 또는 추천/후보 목록에서 선택

## 핵심 구현 범위

- 발신지 선택과 7개 목적지 선택
- 메시지/다중 첨부 작성, 512KB payload 제한, 전송 전 메시지 목록 수정/삭제/보류
- 공개 CelesTrak TLE 기반 실제 위성 후보, 조건 필터, 동적 가시권 기반 추천 위성/가격/성공 예측 표시
- 3D 지구, 목적지 마커, 위성 군집, 중계 경로와 전송 패킷 시각화
- 전송 완료 결과, 전송 기록, 재사용, 실패 재시도 흐름
- 가상 시연용 `SAT-*` 위성군은 기본 표시에서 제외하고, 공개 CelesTrak STATIONS 그룹을 기본 위성군으로 사용합니다. ISS/ACTIVE/STARLINK는 런타임 전환용 데이터셋으로 유지합니다.

## 실제 위성/TLE 구현 범위와 한계

- 데이터 출처는 공개 CelesTrak `gp.php` TLE 엔드포인트입니다. Space-Track 로그인 세션이나 계정 정보는 프로젝트 코드에서 사용하지 않습니다.
- 기본 표시 대상은 CelesTrak `GROUP=STATIONS` 실제 위성군입니다. ISS `CATNR=25544`, ACTIVE, STARLINK 그룹은 전환/URL 검증 대상으로 유지합니다.
- 런타임 소형 패널 또는 단축키 `1`/`2`/`3`/`4`로 ISS/STATIONS/ACTIVE/STARLINK 데이터셋을 전환합니다.
- `RealSatelliteVisualizer`가 표시한 실제 위성은 `SatelliteManager`에 `REAL-*` SatelliteData로 동기화되어 추천/전송 시뮬레이션 후보로 사용됩니다.
- 자동 검증은 visible `SAT-*` 가상 constellation이 없어야 PASS로 판단합니다. 단, 경로 계산이나 카메라 기준점 같은 비가시 보조 anchor는 허용합니다.
- CelesTrak 요청은 메모리 캐시와 `Application.persistentDataPath/OrbitShareTleCache` 파일 캐시를 사용하고, 네트워크 실패 시 stale cache를 허용합니다. ISS는 마지막 안전장치로 내장 샘플 TLE fallback을 표시할 수 있으며, ACTIVE/STARLINK는 ISS 샘플로 대체하지 않습니다. 자동 검증 리포트는 실제 CelesTrak/캐시 표시와 fallback 표시를 분리합니다.
- 현재 `Sgp4Propagator`는 TLE 평균궤도요소 기반 근사 전파와 1차 J2 보정을 사용합니다. 정밀 Vallado SGP4/SDP4 전체 구현이나 TEME→ITRF 고정밀 보정(극운동/세차·장동 등)은 아직 포함하지 않았습니다.
- 좌표 변환은 현재 시각 기준 ECI→ECEF→위경도/고도 변환으로 지표면 위치를 표시합니다. 발표에서는 “실제 TLE 기반 시각화 1차 버전”으로 설명하고, 정밀 궤도해석 결과처럼 단정하지 않습니다.

## 사용 에셋과 출처

- 외부 지구 텍스처는 `Assets/Resources/OrbitShare/Earth/`에 있으며, 출처와 라이선스는 `Assets/Resources/OrbitShare/Earth/ATTRIBUTION.md`에 정리되어 있습니다.
- 선택 Asset Store 리소스와 런타임 리소스 이름은 `ASSET_STORE_IMPORT.md`를 확인하세요.
- 선택 리소스가 없어도 프로젝트는 절차적 시각 요소와 생성 사운드로 실행됩니다.

## 빌드

Unity Editor에서 다음 메뉴를 사용할 수 있습니다.

- `OrbitShare > Build > Linux`
- `OrbitShare > Build > Windows`

빌드 스크립트는 `Assets/Editor/BuildOrbitShare.cs`에 있습니다.

## 자동 검증

```bash
LD_LIBRARY_PATH=/home/ddobagi/Code/grade3/computergraphics/assignment1/.unity-libs:$LD_LIBRARY_PATH \
/home/ddobagi/Unity/Hub/Editor/2022.3.62f3/Editor/Unity \
-batchmode -nographics \
-projectPath /home/ddobagi/Code/grade3/computergraphics/assignment1 \
-executeMethod OrbitShareEditorAutomation.ValidateMissionFlow \
-logFile unity-validation.log
```

검증 명령은 실제 TLE 위성군 로드를 기다린 뒤 Play Mode에서 빠른 시연 흐름을 실행하고 `orbitshare-validation.md`를 작성합니다. 최신 검증 기준은 7개 목적지, 목적지별 추천 스냅샷, visible `SAT-*` 가상 constellation 비활성 여부, `REAL-*` SatelliteData/marker 수, REAL 위성으로 선택·전송되었는지, CelesTrak ISS/STATIONS/ACTIVE/STARLINK/OMM(JSON) URL, 실제 위성 마커의 위경도/고도/속도 벡터/fallback 여부를 기록합니다. 현재 문서 기준 수동 GUI 리허설은 별도 제출 게이트로 추적하지 않습니다.

하단 메뉴 레이아웃, 재클릭 닫기 동작, 메시지 작성 5단계와 직접 설정 달력 화면은 다음 명령으로 메뉴별 캡처와 리포트를 생성해 확인합니다.

```bash
/home/ddobagi/Unity/Hub/Editor/2022.3.62f3/Editor/Unity \
-batchmode \
-projectPath /home/ddobagi/Code/grade3/computergraphics/assignment1 \
-executeMethod OrbitShareEditorAutomation.CaptureLauncherMenuScreenshots \
-logFile unity-capture-launcher-menus.log
```

결과는 `orbitshare-menu-captures/`에 저장되며 `flow-01-time.png`, `flow-01-calendar.png`, `flow-02-message.png`~`flow-05-confirm.png`도 함께 생성됩니다.
