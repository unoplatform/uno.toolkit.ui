#!/usr/bin/env bash
set -euo pipefail

# Create a desktop-only crosstargeting override for build pipelines
repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/../../.." && pwd)"
cat > "${repo_root}/src/crosstargeting_override.props" << 'EOF'
<Project ToolsVersion="15.0">
	<PropertyGroup>
		<DisableMobileTargets>true</DisableMobileTargets>
		<TargetFrameworkOverride>net9.0-desktop</TargetFrameworkOverride>
	</PropertyGroup>
</Project>
EOF

# Restore solution packages to ensure dependencies are ready for subsequent steps
dotnet restore Uno.Toolkit.sln