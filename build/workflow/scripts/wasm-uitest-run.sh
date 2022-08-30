#!/bin/bash
set -x #echo on
set -euo pipefail
IFS=$'\n\t'

export UNO_UITEST_TARGETURI=http://localhost:5000
export UNO_UITEST_DRIVERPATH_CHROME=$BUILD_SOURCESDIRECTORY/build/node_modules/chromedriver/lib/chromedriver
export UNO_UITEST_CHROME_BINARY_PATH=$BUILD_SOURCESDIRECTORY/build/node_modules/puppeteer/.local-chromium/linux-800071/chrome-linux/chrome
export UNO_UITEST_SCREENSHOT_PATH=$BUILD_ARTIFACTSTAGINGDIRECTORY/screenshots/wasm
export UNO_UITEST_PLATFORM=Browser
export UNO_UITEST_CHROME_CONTAINER_MODE=true
export UNO_UITEST_PROJECT=$BUILD_SOURCESDIRECTORY/src/Uno.Toolkit.UITest/Uno.Toolkit.UITest.csproj
export UNO_UITEST_BINARY=$BUILD_SOURCESDIRECTORY/src/Uno.Toolkit.UITest/bin/Uno.Toolkit.UITest/Release/Uno.Toolkit.UITest.dll
export UNO_UITEST_LOGFILE=$BUILD_ARTIFACTSTAGINGDIRECTORY/screenshots/wasm/nunit-log.txt
export UNO_UITEST_WASM_PROJECT=$BUILD_SOURCESDIRECTORY/samples/Uno.Toolkit.Samples/Uno.Toolkit.Samples.Wasm/Uno.Toolkit.Samples.Wasm.csproj
export UNO_UITEST_WASM_OUTPUT_PATH=$BUILD_SOURCESDIRECTORY/samples/Uno.Toolkit.Samples/Uno.Toolkit.Samples.Wasm/bin/Release/net5.0/dist/
export UNO_UITEST_NUNIT_VERSION=3.11.1
export UNO_UITEST_NUGET_URL=https://dist.nuget.org/win-x86-commandline/v5.7.0/nuget.exe
export UNO_ORIGINAL_TEST_RESULTS=$BUILD_SOURCESDIRECTORY/build/TestResult-original.xml
export UNO_TESTS_RESPONSE_FILE=$BUILD_SOURCESDIRECTORY/build/nunit.response

mkdir -p $UNO_UITEST_SCREENSHOT_PATH

cd $BUILD_SOURCESDIRECTORY

msbuild /r /p:Configuration=Release $UNO_UITEST_PROJECT
dotnet build /r /p:Configuration=Release $UNO_UITEST_WASM_PROJECT /p:IsUiAutomationMappingEnabled=True /p:DisableNet6MobileTargets=True

# Start the server
dotnet run --project $UNO_UITEST_WASM_PROJECT -c Release --no-build &

cd $BUILD_SOURCESDIRECTORY/build

npm i chromedriver@86.0.0
npm i puppeteer@5.3.1
wget $UNO_UITEST_NUGET_URL
mono nuget.exe install NUnit.ConsoleRunner -Version $UNO_UITEST_NUNIT_VERSION

## Build the NUnit configuration file
echo "--trace=Verbose" > $UNO_TESTS_RESPONSE_FILE
echo "--result=$UNO_ORIGINAL_TEST_RESULTS" >> $UNO_TESTS_RESPONSE_FILE
echo "$UNO_UITEST_BINARY" >> $UNO_TESTS_RESPONSE_FILE

mono $BUILD_SOURCESDIRECTORY/build/NUnit.ConsoleRunner.$UNO_UITEST_NUNIT_VERSION/tools/nunit3-console.exe \
    @$UNO_TESTS_RESPONSE_FILE || true

## Copy the results file to the results folder
cp --backup=t $UNO_ORIGINAL_TEST_RESULTS $UNO_UITEST_SCREENSHOT_PATH
