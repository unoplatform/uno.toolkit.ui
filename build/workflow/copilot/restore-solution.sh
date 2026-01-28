#!/usr/bin/env bash
set -euo pipefail

cd src

dotnet workload restore

# Restore solution packages to ensure dependencies are ready for subsequent steps
dotnet restore Uno.Toolkit.sln