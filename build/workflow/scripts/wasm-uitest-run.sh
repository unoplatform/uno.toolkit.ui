#!/bin/bash
set -x #echo on
set -euo pipefail
IFS=$'\n\t'

if [ "$UITEST_TEST_MODE_NAME" == 'Automated' ];
then
	export TEST_FILTERS="namespace != 'Uno.Toolkit.UITest.RuntimeTests'";
	export UITEST_RUNTIMETESTS_RESULTS_FILE_PATH=$BUILD_SOURCESDIRECTORY/build/UITestResults-wasm-automated.xml
elif [ "$UITEST_TEST_MODE_NAME" == 'RuntimeTests' ];
then
	export TEST_FILTERS="class == 'Uno.Toolkit.UITest.RuntimeTests.RuntimeTestRunner'";
	export UITEST_RUNTIMETESTS_RESULTS_FILE_PATH=$BUILD_SOURCESDIRECTORY/build/UITestResults-wasm-runtime.xml
fi

cd $BUILD_SOURCESDIRECTORY/build/$SAMPLEAPP_ARTIFACT_NAME

npm i chromedriver@102.0.0
npm i puppeteer@14.1.0

# install dotnet serve / Remove as needed
dotnet tool uninstall dotnet-serve -g || true
dotnet tool uninstall dotnet-serve --tool-path $BUILD_SOURCESDIRECTORY/build/tools || true
dotnet tool install dotnet-serve --version 1.10.140 --tool-path $BUILD_SOURCESDIRECTORY/build/tools || true
export PATH="$PATH:$BUILD_SOURCESDIRECTORY/build/tools"

export BASE_ARTIFACTS_PATH=$BUILD_ARTIFACTSTAGINGDIRECTORY/wasm/$XAML_FLAVOR_BUILD/$UITEST_TEST_MODE_NAME
export UNO_UITEST_TARGETURI=http://localhost:5000
export UNO_UITEST_DRIVERPATH_CHROME=$BUILD_SOURCESDIRECTORY/build/$SAMPLEAPP_ARTIFACT_NAME/node_modules/chromedriver/lib/chromedriver
export UNO_UITEST_CHROME_BINARY_PATH=$BUILD_SOURCESDIRECTORY/build/$SAMPLEAPP_ARTIFACT_NAME/node_modules/puppeteer/.local-chromium/linux-991974/chrome-linux/chrome
export UNO_UITEST_SCREENSHOT_PATH=$BASE_ARTIFACTS_PATH/screenshots
export UNO_UITEST_PLATFORM=Browser
export UNO_UITEST_CHROME_CONTAINER_MODE=true
export UNO_UITEST_PROJECT=$BUILD_SOURCESDIRECTORY/src/Uno.Toolkit.UITest/Uno.Toolkit.UITest.csproj
export UNO_UITEST_BINARY=$BUILD_SOURCESDIRECTORY/build/toolkit-uitest-binaries-$XAML_FLAVOR_BUILD/Uno.Toolkit.UITest.dll
export UNO_UITEST_LOGFILE=$BASE_ARTIFACTS_PATH/nunit-log.txt
export UNO_UITEST_WASM_PROJECT=$BUILD_SOURCESDIRECTORY/samples/$SAMPLE_PROJECT_NAME/$SAMPLE_PROJECT_NAME.Wasm/$SAMPLE_PROJECT_NAME.Wasm.csproj
export UNO_UITEST_WASM_OUTPUT_PATH=$BUILD_SOURCESDIRECTORY/samples/$SAMPLE_PROJECT_NAME/$SAMPLE_PROJECT_NAME.Wasm/bin/Release/net5.0/dist/
export UNO_UITEST_NUNIT_VERSION=3.11.1
export UNO_UITEST_NUGET_URL=https://dist.nuget.org/win-x86-commandline/v5.7.0/nuget.exe
export UNO_ORIGINAL_TEST_RESULTS=$BUILD_SOURCESDIRECTORY/build/$UNO_TEST_RESULTS_FILE_NAME
export UNO_UITEST_RUNTIMETESTS_RESULTS_FILE_PATH=$UNO_ORIGINAL_TEST_RESULTS
export UNO_TESTS_RESPONSE_FILE=$BUILD_SOURCESDIRECTORY/build/nunit.response

mkdir -p $UNO_UITEST_SCREENSHOT_PATH

## The python server serves the current working directory, and may be changed by the nunit runner
dotnet-serve -p 5000 -d "$BUILD_SOURCESDIRECTORY/build/$SAMPLEAPP_ARTIFACT_NAME/site" &

cd $BUILD_SOURCESDIRECTORY/build

wget $UNO_UITEST_NUGET_URL
mono nuget.exe install NUnit.ConsoleRunner -Version $UNO_UITEST_NUNIT_VERSION

## Build the NUnit configuration file
echo "--trace=Verbose" > $UNO_TESTS_RESPONSE_FILE
echo "--result=$UNO_ORIGINAL_TEST_RESULTS" >> $UNO_TESTS_RESPONSE_FILE
echo "--where \"$TEST_FILTERS\"" >> $UNO_TESTS_RESPONSE_FILE
echo "$UNO_UITEST_BINARY" >> $UNO_TESTS_RESPONSE_FILE

mono $BUILD_SOURCESDIRECTORY/build/NUnit.ConsoleRunner.$UNO_UITEST_NUNIT_VERSION/tools/nunit3-console.exe \
    @$UNO_TESTS_RESPONSE_FILE || true

## Copy the results file to the results folder
cp --backup=t $UNO_ORIGINAL_TEST_RESULTS $BASE_ARTIFACTS_PATH