#!/bin/bash
set -x #echo on
set -euo pipefail
IFS=$'\n\t'

if [ "$UITEST_TEST_MODE_NAME" == 'Automated' ];
then
	export TEST_FILTERS="FullyQualifiedName !~ Uno.Toolkit.UITest.RuntimeTests";
elif [ "$UITEST_TEST_MODE_NAME" == 'RuntimeTests' ];
then
	export TEST_FILTERS="FullyQualifiedName ~ Uno.Toolkit.UITest.RuntimeTests";
fi

export BASE_ARTIFACTS_PATH=$BUILD_ARTIFACTSTAGINGDIRECTORY/wasm/$XAML_FLAVOR_BUILD/$UITEST_TEST_MODE_NAME
export UNO_UITEST_TARGETURI=http://localhost:5000
export UNO_UITEST_DRIVERPATH_CHROME=$BUILD_SOURCESDIRECTORY/build/node_modules/chromedriver/lib/chromedriver
export UNO_UITEST_CHROME_BINARY_PATH=$BUILD_SOURCESDIRECTORY/build/node_modules/puppeteer/.local-chromium/linux-991974/chrome-linux/chrome
export UNO_UITEST_SCREENSHOT_PATH=$BASE_ARTIFACTS_PATH/screenshots
export UNO_UITEST_PLATFORM=Browser
export UNO_UITEST_CHROME_CONTAINER_MODE=true
export UNO_UITEST_PROJECT=$BUILD_SOURCESDIRECTORY/src/Uno.Toolkit.UITest/Uno.Toolkit.UITest.csproj
export UNO_UITEST_BINARY=$BUILD_SOURCESDIRECTORY/build/toolkit-uitest-binaries/Uno.Toolkit.UITest.dll
export UNO_UITEST_LOGFILE=$BASE_ARTIFACTS_PATH/nunit-log.txt
export UNO_UITEST_WASM_PROJECT=$BUILD_SOURCESDIRECTORY/samples/$SAMPLE_PROJECT_NAME/$SAMPLE_PROJECT_NAME.Wasm/$SAMPLE_PROJECT_NAME.Wasm.csproj
export UNO_UITEST_WASM_OUTPUT_PATH=$BUILD_SOURCESDIRECTORY/samples/$SAMPLE_PROJECT_NAME/$SAMPLE_PROJECT_NAME.Wasm/bin/Release/net7.0/dist/
export UNO_UITEST_NUNIT_VERSION=3.11.1
export UNO_UITEST_NUGET_URL=https://dist.nuget.org/win-x86-commandline/v5.7.0/nuget.exe
export UNO_ORIGINAL_TEST_RESULTS=$BUILD_SOURCESDIRECTORY/build/$UNO_TEST_RESULTS_FILE_NAME
export UNO_UITEST_RUNTIMETESTS_RESULTS_FILE_PATH=$UNO_ORIGINAL_TEST_RESULTS
export UNO_TESTS_RESPONSE_FILE=$BUILD_SOURCESDIRECTORY/build/nunit.response
export UITEST_TEST_TIMEOUT=60m

cd $BUILD_SOURCESDIRECTORY

dotnet build /r /p:Configuration=Release $UNO_UITEST_WASM_PROJECT /p:IsUiAutomationMappingEnabled=True /p:DisableMobileTargets=True /bl:$BASE_ARTIFACTS_PATH/wasm-$XAML_FLAVOR_BUILD-uitest.binlog

cd $BUILD_SOURCESDIRECTORY/build
mkdir -p tools

npm i chromedriver@102.0.0
npm i puppeteer@14.1.0

# install dotnet serve / Remove as needed
dotnet tool uninstall dotnet-serve -g || true
dotnet tool uninstall dotnet-serve --tool-path $BUILD_SOURCESDIRECTORY/build/tools || true
dotnet tool install dotnet-serve --version 1.10.140 --tool-path $BUILD_SOURCESDIRECTORY/build/tools || true
export PATH="$PATH:$BUILD_SOURCESDIRECTORY/build/tools"

mkdir -p $UNO_UITEST_SCREENSHOT_PATH

## The python server serves the current working directory, and may be changed by the nunit runner
dotnet-serve -p 5000 -d "$UNO_UITEST_WASM_OUTPUT_PATH" &

echo "Test Parameters:"
echo "  Timeout=$UITEST_TEST_TIMEOUT"
echo "  Test filters: $TEST_FILTERS"

cd $BUILD_SOURCESDIRECTORY/src/Uno.Toolkit.UITest

dotnet test \
	-c Release \
	-l:"console;verbosity=normal" \
	--logger "nunit;LogFileName=$UNO_UITEST_RUNTIMETESTS_RESULTS_FILE_PATH" \
	--filter "$TEST_FILTERS" \
	--blame-hang-timeout $UITEST_TEST_TIMEOUT \
	-v m \
	-property:FrameworkLineage=$XAML_FLAVOR_BUILD \
	|| true

## Copy the results file to the results folder
cp --backup=t $UNO_UITEST_RUNTIMETESTS_RESULTS_FILE_PATH $UNO_UITEST_SCREENSHOT_PATH
