#!/usr/bin/env bash
set -euo pipefail
IFS=$'\n\t'

XVFB_DISPLAY="${XVFB_DISPLAY:-:99}"
SCREEN_SETTINGS="${XVFB_SCREEN_SETTINGS:-1280x1024x24}"
SOCKET_PATH="/tmp/.X11-unix/X${XVFB_DISPLAY#:}"

sudo apt-get update

# Divert /usr/bin/mandb to avoid package ownership conflicts
sudo dpkg-divert --local --rename --add /usr/bin/mandb
# Replace with a no-op
echo '#!/bin/sh' | sudo tee /usr/bin/mandb >/dev/null
echo 'exit 0' | sudo tee -a /usr/bin/mandb >/dev/null
sudo chmod 755 /usr/bin/mandb

# Install Xvfb and fluxbox for virtual display
sudo apt-get install -y xvfb fluxbox

sudo mkdir -p /tmp/.X11-unix
sudo chmod 1777 /tmp/.X11-unix

echo "Starting Xvfb on display ${XVFB_DISPLAY}"
sudo Xvfb "${XVFB_DISPLAY}" -screen 0 "${SCREEN_SETTINGS}" > /tmp/xvfb.log 2>&1 &
XVFB_PID=$!

# Wait until the X11 socket is available so subsequent steps can connect
for _ in $(seq 1 40); do
  if [ -S "${SOCKET_PATH}" ]; then
    break
  fi
  sleep 0.25
done

if [ ! -S "${SOCKET_PATH}" ]; then
  echo "Xvfb failed to create socket at ${SOCKET_PATH}" >&2
  exit 1
fi

echo "DISPLAY=${XVFB_DISPLAY}" >> "${GITHUB_ENV}"
echo "XVFB_PID=${XVFB_PID}" >> "${GITHUB_ENV}"

echo "Starting fluxbox window manager"
DISPLAY="${XVFB_DISPLAY}" fluxbox > /tmp/fluxbox.log 2>&1 &
FLUXBOX_PID=$!
echo "FLUXBOX_PID=${FLUXBOX_PID}" >> "${GITHUB_ENV}"

echo "Virtual display ready on ${XVFB_DISPLAY}."