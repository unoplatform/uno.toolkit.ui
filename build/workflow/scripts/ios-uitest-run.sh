#!/bin/bash
set -euo pipefail
IFS=$'\n\t'


if [ "$UITEST_TEST_MODE_NAME" == 'Automated' ];
then
	export TEST_FILTERS="namespace != 'Uno.Toolkit.UITest.RuntimeTests'";
	export UITEST_RUNTIMETESTS_RESULTS_FILE_PATH=$BUILD_SOURCESDIRECTORY/build/UITestResults-ios-automated.xml
elif [ "$UITEST_TEST_MODE_NAME" == 'RuntimeTests' ];
then
	export TEST_FILTERS="class == 'Uno.Toolkit.UITest.RuntimeTests.RuntimeTestRunner'";
	export UITEST_RUNTIMETESTS_RESULTS_FILE_PATH=$BUILD_SOURCESDIRECTORY/build/UITestResults-ios-runtime.xml
fi
export UNO_UITEST_PLATFORM=iOS
export BASE_ARTIFACTS_PATH=$BUILD_ARTIFACTSTAGINGDIRECTORY/ios/$XAML_FLAVOR_BUILD/$UITEST_TEST_MODE_NAME
export UNO_UITEST_IOSBUNDLE_PATH=$BUILD_SOURCESDIRECTORY/build/$SAMPLEAPP_ARTIFACT_NAME/$SAMPLE_PROJECT_NAME.app
export UNO_UITEST_SCREENSHOT_PATH=$BASE_ARTIFACTS_PATH/screenshots
export UNO_UITEST_PROJECT=$BUILD_SOURCESDIRECTORY/src/Uno.Toolkit.UITest/Uno.Toolkit.UITest.csproj
export UNO_UITEST_LOGFILE=$UNO_UITEST_SCREENSHOT_PATH/nunit-log.txt
export UNO_UITEST_IOS_PROJECT=$BUILD_SOURCESDIRECTORY/samples/$SAMPLE_PROJECT_NAME/$SAMPLE_PROJECT_NAME.iOS/$SAMPLE_PROJECT_NAME.iOS.csproj
export UNO_UITEST_BINARY=$BUILD_SOURCESDIRECTORY/build/toolkit-uitest-binaries/Uno.Toolkit.UITest.dll
export UNO_UITEST_NUNIT_VERSION=3.12.0
export UNO_UITEST_NUGET_URL=https://dist.nuget.org/win-x86-commandline/v5.7.0/nuget.exe
export UNO_ORIGINAL_TEST_RESULTS=$BUILD_SOURCESDIRECTORY/build/$UNO_TEST_RESULTS_FILE_NAME
export UNO_UITEST_RUNTIMETESTS_RESULTS_FILE_PATH=$UNO_ORIGINAL_TEST_RESULTS
export UNO_TESTS_RESPONSE_FILE=$BUILD_SOURCESDIRECTORY/build/nunit.response
export UNO_UITEST_SIMULATOR_VERSION="com.apple.CoreSimulator.SimRuntime.iOS-15-2"
export UNO_UITEST_SIMULATOR_NAME="iPad Pro (12.9-inch) (4th generation)"
export UITEST_IOSDEVICE_ID="iPad Pro (12.9-inch) (4th generation)"

echo "Listing iOS simulators"
xcrun simctl list devices --json

/Applications/Xcode_13.2.1.app/Contents/Developer/Applications/Simulator.app/Contents/MacOS/Simulator &

cd $BUILD_SOURCESDIRECTORY/build

wget $UNO_UITEST_NUGET_URL
mono nuget.exe install NUnit.ConsoleRunner -Version $UNO_UITEST_NUNIT_VERSION

mkdir -p $UNO_UITEST_SCREENSHOT_PATH

# Imported app bundle from artifacts is not executable
chmod -R +x $UNO_UITEST_IOSBUNDLE_PATH

# Move to the screenshot directory so that the output path is the proper one, as
# required by Xamarin.UITest
cd $UNO_UITEST_SCREENSHOT_PATH

## Build the NUnit configuration file
echo "--trace=Verbose" > $UNO_TESTS_RESPONSE_FILE
echo "--inprocess" >> $UNO_TESTS_RESPONSE_FILE
echo "--agents=1" >> $UNO_TESTS_RESPONSE_FILE
echo "--workers=1" >> $UNO_TESTS_RESPONSE_FILE
echo "--result=$UNO_ORIGINAL_TEST_RESULTS" >> $UNO_TESTS_RESPONSE_FILE
echo "--where \"$TEST_FILTERS\"" >> $UNO_TESTS_RESPONSE_FILE
echo "$UNO_UITEST_BINARY" >> $UNO_TESTS_RESPONSE_FILE

echo Response file:
cat $UNO_TESTS_RESPONSE_FILE

## Show the tests list
mono $BUILD_SOURCESDIRECTORY/build/NUnit.ConsoleRunner.$UNO_UITEST_NUNIT_VERSION/tools/nunit3-console.exe \
    @$UNO_TESTS_RESPONSE_FILE --explore || true

## Run NUnit tests
mono $BUILD_SOURCESDIRECTORY/build/NUnit.ConsoleRunner.$UNO_UITEST_NUNIT_VERSION/tools/nunit3-console.exe \
   @$UNO_TESTS_RESPONSE_FILE || true

echo "Current system date"
date

## Copy the results file to the results folder
cp $UNO_ORIGINAL_TEST_RESULTS $BASE_ARTIFACTS_PATH

# export the simulator logs
export LOG_FILEPATH=$UNO_UITEST_SCREENSHOT_PATH/_logs
export TMP_LOG_FILEPATH=/tmp/DeviceLog-`date +"%Y%m%d%H%M%S"`.logarchive
export LOG_FILEPATH_FULL=$LOG_FILEPATH/DeviceLog-`date +"%Y%m%d%H%M%S"`.txt

mkdir -p $LOG_FILEPATH
xcrun simctl spawn booted log collect --output $TMP_LOG_FILEPATH
log show --style syslog $TMP_LOG_FILEPATH > $LOG_FILEPATH_FULL
