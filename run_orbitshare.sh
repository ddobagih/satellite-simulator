#!/usr/bin/env bash
set -euo pipefail

PROJECT_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
EXECUTABLE="$PROJECT_ROOT/Builds/Linux/OrbitShare.x86_64"
LOG_FILE="$PROJECT_ROOT/build-run-live.log"
SCREEN_WIDTH="${ORBITSHARE_SCREEN_WIDTH:-1280}"
SCREEN_HEIGHT="${ORBITSHARE_SCREEN_HEIGHT:-720}"
DISPLAY_TO_USE="${DISPLAY:-:0}"
XWAYLAND_PID=""

cleanup() {
  if [[ -n "$XWAYLAND_PID" ]]; then
    kill "$XWAYLAND_PID" 2>/dev/null || true
    wait "$XWAYLAND_PID" 2>/dev/null || true
  fi
}
trap cleanup EXIT

display_dimensions() {
  xdpyinfo -display "$1" 2>/dev/null | awk '/dimensions:/ { print $2; exit }'
}

find_free_display() {
  local display_number
  for display_number in 99 98 97 96; do
    if ! xdpyinfo -display ":$display_number" >/dev/null 2>&1; then
      printf ':%s\n' "$display_number"
      return 0
    fi
  done
  return 1
}

if command -v xdpyinfo >/dev/null 2>&1; then
  DIMENSIONS="$(display_dimensions "$DISPLAY_TO_USE" || true)"
  if [[ "$DIMENSIONS" == "0x0" && -n "${WAYLAND_DISPLAY:-}" && -x "$(command -v Xwayland)" ]]; then
    DISPLAY_TO_USE="$(find_free_display)"
    Xwayland "$DISPLAY_TO_USE" \
      -geometry "${SCREEN_WIDTH}x${SCREEN_HEIGHT}" \
      -ac \
      -noreset \
      > "$PROJECT_ROOT/xwayland-orbitshare.log" 2>&1 &
    XWAYLAND_PID="$!"

    for _ in $(seq 1 50); do
      if xdpyinfo -display "$DISPLAY_TO_USE" >/dev/null 2>&1; then
        break
      fi
      sleep 0.1
    done

    if ! xdpyinfo -display "$DISPLAY_TO_USE" >/dev/null 2>&1; then
      echo "Failed to start Xwayland display $DISPLAY_TO_USE" >&2
      exit 1
    fi
  fi
fi

cd "$PROJECT_ROOT"
DISPLAY="$DISPLAY_TO_USE" "$EXECUTABLE" \
  -screen-fullscreen 0 \
  -screen-width "$SCREEN_WIDTH" \
  -screen-height "$SCREEN_HEIGHT" \
  -logFile "$LOG_FILE"
