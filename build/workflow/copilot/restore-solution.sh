#!/usr/bin/env bash
set -euo pipefail

# Create a desktop-only crosstargeting override for build pipelines
cat > "src/crosstargeting_override.props" << 'EOF'
<Project ToolsVersion="15.0">
	<PropertyGroup>
		<DisableMobileTargets>true</DisableMobileTargets>
		<SamplesTargetFrameworkOverride>net10.0-desktop</SamplesTargetFrameworkOverride>
		<TargetFrameworkOverride>net9.0</TargetFrameworkOverride>
	</PropertyGroup>
</Project>
EOF

cd src

dotnet workload restore

# Restore solution packages to ensure dependencies are ready for subsequent steps
dotnet restore Uno.Toolkit.sln

# Preload the dev server tool
dotnet dnx -y uno.devserver --prerelease -- start -l trace

sleep 20

dotnet dnx -y uno.devserver --prerelease -- stop -l trace

