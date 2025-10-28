#!/bin/bash
set -euo pipefail
IFS=$'\n\t'

# Ensure this script has no BOM even if it ever gets committed with one again
# (the BOM only breaks the shebang at exec time, but this is a safety net).
sed -i '' $'1s/^\xEF\xBB\xBF//' "$0"


if [ "$UITEST_TEST_MODE_NAME" == 'Automated' ];
then
	export TEST_FILTERS="FullyQualifiedName !~ Uno.Toolkit.UITest.RuntimeTests";
elif [ "$UITEST_TEST_MODE_NAME" == 'RuntimeTests' ];
then
	export TEST_FILTERS="FullyQualifiedName ~ Uno.Toolkit.UITest.RuntimeTests";
fi

export UNO_UITEST_PLATFORM=iOS
export BASE_ARTIFACTS_PATH=$BUILD_ARTIFACTSTAGINGDIRECTORY/ios/$UITEST_TEST_MODE_NAME
export UNO_UITEST_IOSBUNDLE_PATH=$BUILD_SOURCESDIRECTORY/build/$SAMPLEAPP_ARTIFACT_NAME/$SAMPLE_PROJECT_NAME.app
export UNO_UITEST_SCREENSHOT_PATH=$BASE_ARTIFACTS_PATH/screenshots
export UNO_UITEST_PROJECT_PATH=$BUILD_SOURCESDIRECTORY/src/Uno.Toolkit.UITest
export UNO_UITEST_PROJECT=$UNO_UITEST_PROJECT_PATH/Uno.Toolkit.UITest.csproj
export UNO_UITEST_LOGFILE=$UNO_UITEST_SCREENSHOT_PATH/nunit-log.txt
export UNO_UITEST_BINARY=$BUILD_SOURCESDIRECTORY/build/toolkit-uitest-binaries/Uno.Toolkit.UITest.dll
export UNO_UITEST_NUNIT_VERSION=3.12.0
export UNO_UITEST_NUGET_URL=https://dist.nuget.org/win-x86-commandline/v5.7.0/nuget.exe
export UNO_ORIGINAL_TEST_RESULTS=$BUILD_SOURCESDIRECTORY/build/$UNO_TEST_RESULTS_FILE_NAME
export UNO_UITEST_RUNTIMETESTS_RESULTS_FILE_PATH=$UNO_ORIGINAL_TEST_RESULTS
export UNO_TESTS_RESPONSE_FILE=$BUILD_SOURCESDIRECTORY/build/nunit.response
export UNO_UITEST_SIMULATOR_VERSION="com.apple.CoreSimulator.SimRuntime.iOS-18-4"
export UNO_UITEST_SIMULATOR_NAME="iPad (10th generation)"

export UITEST_TEST_TIMEOUT=120m

echo "Listing iOS simulators"
xcrun simctl list devices --json

/Applications/Xcode.app/Contents/Developer/Applications/Simulator.app/Contents/MacOS/Simulator &

# Prime the output directory
mkdir -p $UNO_UITEST_SCREENSHOT_PATH/_logs

##
## Pre-install the application to avoid https://github.com/microsoft/appcenter/issues/2389
##

while true; do
	export UITEST_IOSDEVICE_ID=`xcrun simctl list -j | jq -r --arg sim "$UNO_UITEST_SIMULATOR_VERSION" --arg name "$UNO_UITEST_SIMULATOR_NAME" '.devices[$sim] | .[] | select(.name==$name) | .udid'`
	export UITEST_IOSDEVICE_DATA_PATH=`xcrun simctl list -j | jq -r --arg sim "$UNO_UITEST_SIMULATOR_VERSION" --arg name "$UNO_UITEST_SIMULATOR_NAME" '.devices[$sim] | .[] | select(.name==$name) | .dataPath'`

	if [ -n "$UITEST_IOSDEVICE_ID" ]; then
		break
	fi

	echo "Waiting for the simulator to be available"
	sleep 5
done

echo "Simulator Data Path: $UITEST_IOSDEVICE_DATA_PATH"
cp "$UITEST_IOSDEVICE_DATA_PATH/../device.plist" $UNO_UITEST_SCREENSHOT_PATH/_logs

echo "Starting simulator: [$UITEST_IOSDEVICE_ID] ($UNO_UITEST_SIMULATOR_VERSION / $UNO_UITEST_SIMULATOR_NAME)"

# Check for the presence of idb, and install it if it's not present
# NOTE: fb-idb currently breaks under Python 3.14 (asyncio get_event_loop change),
# so we pin fb-idb to Python 3.12 to avoid "There is no current event loop in thread 'MainThread'".
# Historical context: prior installs referenced an App Center issue/workaround.
# https://github.com/microsoft/appcenter/issues/2605#issuecomment-1854414963
export PATH=$PATH:~/.local/bin

if ! command -v idb >/dev/null 2>&1; then
  echo "Installing idb (fb-idb + idb-companion) pinned to Python 3.12"

  # 1) Make sure we have a usable python3.12, but don't fail if Homebrew linking conflicts
  if ! command -v python3.12 >/dev/null 2>&1; then
    # Install, but ignore link-step failure; we'll use the keg path explicitly
    brew list --versions python@3.12 >/dev/null 2>&1 || brew install python@3.12 || true
  fi
  # Prefer an existing python3.12 on PATH; otherwise use the keg path
  PY312_BIN="$(command -v python3.12 || echo "$(brew --prefix)/opt/python@3.12/bin/python3.12")"
  export PIPX_DEFAULT_PYTHON="$PY312_BIN"
  echo "Using Python for pipx: $PIPX_DEFAULT_PYTHON"

  # 2) Install helpers
  brew list --versions pipx >/dev/null 2>&1 || brew install pipx
  brew tap facebook/fb >/dev/null 2>&1 || true
  brew list --versions idb-companion >/dev/null 2>&1 || brew install idb-companion

  # 3) Install fb-idb under Python 3.12
  pipx install --force fb-idb
else
  echo "Using idb from: $(command -v idb)"
fi

xcrun simctl boot "$UITEST_IOSDEVICE_ID" || true

idb install --udid "$UITEST_IOSDEVICE_ID" "$UNO_UITEST_IOSBUNDLE_PATH"

cd $BUILD_SOURCESDIRECTORY/build

# Imported app bundle from artifacts is not executable
sudo chmod -R +x $UNO_UITEST_IOSBUNDLE_PATH

cd $UNO_UITEST_PROJECT_PATH

## Run tests
dotnet test \
	-c Release \
	-l:"console;verbosity=normal" \
	--logger "nunit;LogFileName=$UNO_UITEST_RUNTIMETESTS_RESULTS_FILE_PATH" \
	--filter "$TEST_FILTERS" \
	--blame-hang-timeout $UITEST_TEST_TIMEOUT \
	-v m \
	|| true

echo "Current system date"
date

if [[ -f $UNO_UITEST_RUNTIMETESTS_RESULTS_FILE_PATH ]]; then
	## Copy the results file to the results folder
	cp $UNO_UITEST_RUNTIMETESTS_RESULTS_FILE_PATH $BASE_ARTIFACTS_PATH
fi

# export the simulator logs
export LOG_FILEPATH=$UNO_UITEST_SCREENSHOT_PATH/_logs
export TMP_LOG_FILEPATH=/tmp/DeviceLog-`date +"%Y%m%d%H%M%S"`.logarchive
export LOG_FILEPATH_FULL=$LOG_FILEPATH/DeviceLog-`date +"%Y%m%d%H%M%S"`.txt

xcrun simctl spawn booted log collect --output $TMP_LOG_FILEPATH
log show --style syslog $TMP_LOG_FILEPATH > $LOG_FILEPATH_FULL