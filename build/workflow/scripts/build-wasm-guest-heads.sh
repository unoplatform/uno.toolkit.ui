#!/bin/bash
set -euo pipefail

# Build raw IL guest payloads before publishing ToolkitSampleApp. Project references
# cannot order WASM guests because their shared StaticWebAssets paths collide.
# Usage: build-wasm-guest-heads.sh [Configuration] [extra MSBuild args...]
CONFIGURATION="${1:-Release}"
if [ "$#" -gt 0 ]; then
  shift
fi

for theme in Material Cupertino Simple; do
  binlog_args=()
  if [ -n "${BINLOG_DIR:-}" ]; then
    mkdir -p "$BINLOG_DIR"
    binlog_args=("-bl:$BINLOG_DIR/guest-${theme}.binlog")
  fi
  dotnet build "samples/Uno.Toolkit.Samples.${theme}/${theme}SampleApp.csproj" \
    -c "$CONFIGURATION" -f net10.0-browserwasm \
    -p:TargetFrameworkOverride=browserwasm \
    ${binlog_args[@]+"${binlog_args[@]}"} "$@"
done
