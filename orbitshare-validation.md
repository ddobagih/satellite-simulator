# OrbitShare Automated Validation

Result: PASS
Elapsed seconds: 1.58
Screen: 640x480

## Checks
- Managers present: PASS
- Scene objects present: PASS
- Bootstrap references connected: PASS
- Fleet initialized: PASS
- Relay constellation has exactly 24 visible satellites: PASS (24)
- Satellite core data valid: PASS
- Target core data valid: PASS
- Default destination is New York: PASS
- Seoul sender gateway valid: PASS
- Relay summary terminology valid: PASS
- Fast demo reaches mission complete: PASS
- Relay lock reaches 100%: PASS
- Delivery reaches 100%: PASS
- Mission metrics calculated: PASS
- Score range 0-100: PASS
- Runtime Error/Exception logs: PASS (0)

## Runtime State
- Mission phase: Complete
- Sender: Seoul
- Destination: New York
- Relay lock progress: 100%
- Delivery progress: 100%
- Communication available: True
- Mission quality score: 71
- Service reliability score: 91
- Satellite count: 3
- SAT-S01 state: MissionComplete
- SAT-S01 storage: 0
- Target count: 3
- Destinations present: Seoul=True, New York=True, London=True

## Notes
- Unity editor token, REST port, curl, and update-check warnings are environment noise unless accompanied by runtime exceptions.
- The editor automation intentionally enters Play mode and exits after validation; this is not an application crash.
