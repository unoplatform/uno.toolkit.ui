#!/usr/bin/env bash
set -euo pipefail

# Restore solution packages to ensure dependencies are ready for subsequent steps
dotnet restore src/Uno.Toolkit.sln -p:SamplesTargetFrameworkOverride=net9.0-desktop -p:TargetFrameworkOverride=net9.0-desktop