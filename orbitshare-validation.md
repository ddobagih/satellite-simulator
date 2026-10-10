# OrbitShare Automated Validation

Result: FAIL
Elapsed seconds: 101.05
Screen: 640x480

## Checks
- Managers present: PASS
- Scene objects present: PASS
- Bootstrap references connected: PASS
- Runtime Canvas scaler configured: PASS
- IMGUI fallback overlay disabled by default: PASS
- Earth orbit camera controls available: PASS (우클릭 드래그/지구 좌클릭 드래그: 회전 · 휠/QE: 줌 · R: 초기화)
- Real TLE fleet initialized: PASS (REAL markers=100, REAL SatelliteData=100)
- Default visible constellation is real CelesTrak TLE: PASS
- No visible virtual SAT-* constellation: PASS (visible=0, total anchors=0)
- Real satellite core data valid: FAIL
- Selected or mission satellite is REAL: PASS
- Required 7 destinations present: PASS
- MissionManager has required 7 destinations: PASS
- Required destination core data valid: PASS
- Default destination is New York: PASS
- Sender gateway and uplink valid: PASS
- End-to-end satellite flow summary valid: FAIL
- End-to-end mission uses REAL satellite: PASS
- Fast demo reaches mission complete: FAIL
- Uplink reaches 100%: FAIL
- End-to-end satellite flow reaches 100%: FAIL
- Reset replay reaches second mission complete: FAIL (0 runs)
- PresentationDirector camera cuts valid: FAIL
- Mission metrics calculated: PASS
- Score range 0-100: PASS
- Runtime Error/Exception logs: PASS (0)
- Source selection wording visible: PASS
- Non-Seoul sender communication visible: PASS
- Communication step evidence available: PASS
- Korean message input fallback valid: PASS
- English input mode preserves Latin text: PASS
- Korean input mode still converts Dubeolsik keys: PASS
- Legacy InputField IME setup valid: PASS
- Satellite route timing responds to live geometry: PASS
- Satellite recommendation sort counts stable: PASS
- Satellite recommendation sort order regression: PASS (sets=24)
- Compose/history UI regression checks valid: PASS
- Input mode switch + attachment regression valid: PASS
- Queued continuation regression valid: FAIL
- Real satellite TLE visualized: PASS
- Real satellite source is not built-in fallback: PASS
- Live CelesTrak network fetch this run: PASS
- CelesTrak ISS/STATIONS/ACTIVE/STARLINK TLE URLs valid: PASS
- Real satellite STATIONS/ACTIVE/STARLINK multi-display config valid: PASS
- Real satellite marker/velocity evidence valid: PASS
- CelesTrak OMM JSON URL support: PASS

## Runtime State
- Mission phase: Running
- Sender: 런던
- Sender key: London
- Destination: New York
- Uplink progress: 0%
- Satellite flow progress: 0%
- Communication step: WaitingForUplink
- Communication available: False
- Completed fast demo runs: 0
- Camera mode at completion: Satellite
- Mission quality score: 75
- Service reliability score: 44
- Satellite count: 100
- REAL SatelliteData count: 100
- Active REAL marker count: 100
- Visible virtual SAT-* count: 0
- Mission satellite: REAL-44714 STARLINK-1008
- Mission satellite state: InMission
- Mission satellite storage: 0
- Mission satellite is REAL: True
- Selected satellite is REAL: True
- Required destination count: 7
- Scene target count: 7
- MissionManager target count: 7
- Destinations present: Seoul=True, New York=True, London=True, Tokyo=True, Paris=True, Sydney=True, Los Angeles=True

## Source Selection & Runtime Sender Snapshot
- Runtime sender: 런던 (London)
- Source selection evidence: runtimeSender=런던 (London); ui=송신지: 런던
- Non-Seoul communication evidence: 사용자(런던) 업링크 → REAL-44714 STARLINK-1008 LEO 수신 → ISL(REAL-44714 STARLINK-1008 → REAL-45361 STARLINK-1301 → REAL-45100 ...

## Communication Step Evidence
- Evidence completeness: PASS
- Route actors: 런던 → REAL-44714 STARLINK-1008 → REAL-45088 STARLINK-1183 → New York
- Segment progress: uplink=0%, ISL=0%, relay=0%, downlink=0%, final=0%
- Uplink evidence: 사용자(런던) 업링크 준비 중.
- LEO receive evidence: 사용자(런던) 업링크 → REAL-44714 STARLINK-1008 LEO 수신 → ISL(REAL-44714 STARLINK-1008 → REAL-45361 STARLINK-1301 → REAL-45100 ...
- ISL evidence: 사용자(런던) 업링크 → REAL-44714 STARLINK-1008 LEO 수신 → ISL(REAL-44714 STARLINK-1008 → REAL-45361 STARLINK-1301 → REAL-45100 ...
- Destination-relay evidence: 사용자(런던) 업링크 → REAL-44714 STARLINK-1008 LEO 수신 → ISL(REAL-44714 STARLINK-1008 → REAL-45361 STARLINK-1301 → REAL-45100 ...
- Downlink evidence: 사용자(런던) 업링크 → REAL-44714 STARLINK-1008 LEO 수신 → ISL(REAL-44714 STARLINK-1008 → REAL-45361 STARLINK-1301 → REAL-45100 ...
- Arrival confirmation evidence: 사용자(런던) 업링크 → REAL-44714 STARLINK-1008 LEO 수신 → ISL(REAL-44714 STARLINK-1008 → REAL-45361 STARLINK-1301 → REAL-45100 ...

## Lightweight Regression Evidence
- Input mode + attachment: firstKorean=한글, english=OrbitShare 123, secondKorean=안녕, textBytes=28, payloadKb=2, attachment=regression-note.txt/1KB
- Queued continuation: phaseComplete=False, queued=True, queueCountIncreased=True, origin=London, target=New York, selectedSatellite=REAL-44968 STARLINK-1090, satelliteSelected=True, routeAvailable=True, canStartAfterComplete=False, nextAction=전송 진행 중

## Real Satellite TLE Evidence
- CelesTrak URL: https://celestrak.org/NORAD/elements/gp.php?GROUP=STARLINK&FORMAT=TLE
- URL source is CelesTrak: PASS
- ISS TLE URL valid: PASS (https://celestrak.org/NORAD/elements/gp.php?CATNR=25544&FORMAT=TLE)
- STATIONS group TLE URL valid: PASS (https://celestrak.org/NORAD/elements/gp.php?GROUP=STATIONS&FORMAT=TLE)
- ACTIVE group TLE URL valid: PASS (https://celestrak.org/NORAD/elements/gp.php?GROUP=ACTIVE&FORMAT=TLE)
- STARLINK group TLE URL valid: PASS (https://celestrak.org/NORAD/elements/gp.php?GROUP=STARLINK&FORMAT=TLE)
- OMM JSON URL support: PASS (ISS=https://celestrak.org/NORAD/elements/gp.php?CATNR=25544&FORMAT=JSON; STATIONS=https://celestrak.org/NORAD/elements/gp.php?GROUP=STATIONS&FORMAT=JSON; ACTIVE=https://celestrak.org/NORAD/elements/gp.php?GROUP=ACTIVE&FORMAT=JSON; STARLINK=https://celestrak.org/NORAD/elements/gp.php?GROUP=STARLINK&FORMAT=JSON; reflectedApi=https://celestrak.org/NORAD/elements/gp.php?CATNR=25544&FORMAT=JSON)
- Status: 실제 TLE STARLINK 표시 중: STARLINK-1008 외 99개
- Loaded/displayed: 100/100
- Dataset/display cap: Starlink, maxDisplayed=100, groupDisplayLimit=100
- Default real fleet mode: PASS (requires STATIONS/ACTIVE/STARLINK and more than 1 visible real marker)
- STATIONS/ACTIVE/STARLINK switch methods: PASS
- Multi-display config: PASS
- First satellite: STARLINK-1008
- First state: STARLINK-1008 · -52.32°, -15.14° · 고도 436km
- Lat/lon valid: PASS
- Altitude sanity: PASS
- ISS LEO altitude sanity: PASS
- Marker count: PASS (active=100, total=100, displayed=100)
- Velocity direction line: PASS (enabled=True, points=2, length=0.139)
- Live CelesTrak network fetch this run: PASS
- Cache used for display: False
- Built-in fallback used: False

## Satellite Recommendation Snapshot
- Seoul: now 100대 rec=REAL-44723 STARLINK-1017 / +5m 100대 rec=REAL-44723 STARLINK-1017 / +15m 100대 rec=REAL-44723 STARLINK-1017 / +30m 100대 rec=REAL-44723 STARLINK-1017
- New York: now 100대 rec=REAL-45054 STARLINK-1156 / +5m 100대 rec=REAL-45054 STARLINK-1156 / +15m 100대 rec=REAL-45054 STARLINK-1156 / +30m 100대 rec=REAL-45054 STARLINK-1156
- London: now 100대 rec=REAL-45230 STARLINK-1227 / +5m 100대 rec=REAL-45230 STARLINK-1227 / +15m 100대 rec=REAL-45230 STARLINK-1227 / +30m 100대 rec=REAL-45230 STARLINK-1227
- Tokyo: now 100대 rec=REAL-45532 STARLINK-1338 / +5m 100대 rec=REAL-45532 STARLINK-1338 / +15m 100대 rec=REAL-45532 STARLINK-1338 / +30m 100대 rec=REAL-45532 STARLINK-1338
- Paris: now 100대 rec=REAL-45230 STARLINK-1227 / +5m 100대 rec=REAL-45230 STARLINK-1227 / +15m 100대 rec=REAL-45230 STARLINK-1227 / +30m 100대 rec=REAL-45230 STARLINK-1227
- Sydney: now 100대 rec=REAL-45080 STARLINK-1153 / +5m 100대 rec=REAL-45080 STARLINK-1153 / +15m 100대 rec=REAL-45080 STARLINK-1153 / +30m 100대 rec=REAL-45080 STARLINK-1153
- Los Angeles: now 100대 rec=REAL-45190 STARLINK-1234 / +5m 100대 rec=REAL-45190 STARLINK-1234 / +15m 100대 rec=REAL-45190 STARLINK-1234 / +30m 100대 rec=REAL-45190 STARLINK-1234

## Environment Note
- Editor token/REST/curl/update-check warnings are treated as environment noise only when runtime Error/Exception count is 0; Play mode entry/exit is intentional.
