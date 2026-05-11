#!/usr/bin/env bash
set -euo pipefail

PROJECT_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
UNITY="/home/ddobagi/Unity/Hub/Editor/2022.3.62f3/Editor/Unity"

LD_LIBRARY_PATH="$PROJECT_ROOT/.unity-libs:${LD_LIBRARY_PATH:-}" \
"$UNITY" \
  -batchmode \
  -nographics \
  -projectPath "$PROJECT_ROOT" \
  -executeMethod AssetStorePackageImporter.ImportAndValidateForBatch \
  -logFile "$PROJECT_ROOT/unity-asset-import.log"
