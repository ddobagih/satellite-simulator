#!/usr/bin/env bash
set -euo pipefail

PROJECT_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
EXECUTABLE="$PROJECT_ROOT/Builds/Linux/OrbitShare.x86_64"
LOG_FILE="$PROJECT_ROOT/build-run-live.log"

cd "$PROJECT_ROOT"
DISPLAY="${DISPLAY:-:0}" "$EXECUTABLE" \
  -screen-fullscreen 0 \
  -screen-width 1280 \
  -screen-height 720 \
  -logFile "$LOG_FILE"
