#!/usr/bin/env bash
set -euo pipefail

# Ensure uno.check is available before running validation
if ! dotnet tool update --global uno.check; then
  dotnet tool install --global uno.check
fi

echo "$HOME/.dotnet/tools" >> "$GITHUB_PATH"
uno-check -v --ci --non-interactive --fix \
  --skip xcode --skip gtk3 --skip vswin --skip vswinworkloads --skip unosdk \
  --skip dotnetnewunotemplates --skip vsmac --skip androidsdk --skip openjdk --skip androidemulator \
  --tfm net10.0-desktop --tfm net10.0-browserwasm