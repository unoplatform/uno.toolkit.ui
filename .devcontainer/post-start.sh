#!/usr/bin/env bash
set -euo pipefail

sudo /usr/local/bin/init-firewall.sh

# Start Xvfb virtual framebuffer for headless Skia desktop rendering.
# Works on both macOS and Linux hosts without host-side X11 setup.
if ! pgrep -x Xvfb > /dev/null 2>&1; then
  Xvfb :99 -screen 0 1920x1080x24 &
  disown
fi

dotnet dev-certs https --trust || true

printf 'claude --dangerously-skip-permissions\n' >> "$HOME/.bash_history"

echo "Registering Claude MCPs for Uno Platform: uno (HTTP docs server)."
echo "To verify, run: claude mcp list"

claude mcp add --scope user --transport http uno https://mcp.platform.uno/v1 || true

echo "Claude MCP registration complete. If you encounter issues, run 'claude mcp list' or 'claude mcp inspect uno'."
