#!/usr/bin/env bash
set -euo pipefail
IFS=$'\n\t'

# echo commands
set -x

export UNO_UITEST_SCREENSHOT_PATH=$BUILD_ARTIFACTSTAGINGDIRECTORY/screenshots/android
export UNO_UITEST_PLATFORM=Android
export UNO_UITEST_ANDROIDAPK_PATH=$BUILD_SOURCESDIRECTORY/samples/$SAMPLE_PROJECT_NAME/$SAMPLE_PROJECT_NAME.Droid/bin/Release/$SAMPLE_PROJECT_NAME-Signed.apk
export UNO_UITEST_PROJECT=$BUILD_SOURCESDIRECTORY/src/Uno.Toolkit.UITest/Uno.Toolkit.UITest.csproj
export UNO_UITEST_ANDROID_PROJECT=$BUILD_SOURCESDIRECTORY/samples/$SAMPLE_PROJECT_NAME/$SAMPLE_PROJECT_NAME.Droid/$SAMPLE_PROJECT_NAME.Droid.csproj
export UNO_UITEST_BINARY=$BUILD_SOURCESDIRECTORY/src/Uno.Toolkit.UITest/bin/Uno.Toolkit.UITest/Release/Uno.Toolkit.UITest.dll
export UNO_UITEST_NUNIT_VERSION=3.12.0
export UNO_UITEST_NUGET_URL=https://dist.nuget.org/win-x86-commandline/v5.7.0/nuget.exe
export UNO_EMULATOR_INSTALLED=$BUILD_SOURCESDIRECTORY/build/.emulator_started
export UNO_ORIGINAL_TEST_RESULTS=$BUILD_SOURCESDIRECTORY/build/TestResult-original.xml
export UNO_TESTS_RESPONSE_FILE=$BUILD_SOURCESDIRECTORY/build/nunit.response
export ANDROID_SIMULATOR_APILEVEL=28
export CMDLINETOOLS=commandlinetools-mac-8512546_latest.zip
export ANDROID_SDK_ROOT=$ANDROID_HOME

mkdir -p $UNO_UITEST_SCREENSHOT_PATH

cd $BUILD_SOURCESDIRECTORY/build

AVD_NAME=xamarin_android_emulator
AVD_CONFIG_FILE=~/.android/avd/$AVD_NAME.avd/config.ini
EMU_UPDATE_FILE=~/.android/emu-update-last-check.ini

if [[ ! -f $EMU_UPDATE_FILE ]];
then
	touch $EMU_UPDATE_FILE
fi

if [[ ! -f $AVD_CONFIG_FILE ]];
then
	# Install AVD files
	echo "y" | $ANDROID_HOME/cmdline-tools/latest/bin/sdkmanager --sdk_root=${ANDROID_HOME} --install 'tools'| tr '\r' '\n' | uniq
	echo "y" | $ANDROID_HOME/cmdline-tools/latest/bin/sdkmanager --sdk_root=${ANDROID_HOME} --install 'platform-tools'  | tr '\r' '\n' | uniq
	echo "y" | $ANDROID_HOME/cmdline-tools/latest/bin/sdkmanager --sdk_root=${ANDROID_HOME} --install 'build-tools;33.0.0' | tr '\r' '\n' | uniq
	echo "y" | $ANDROID_HOME/cmdline-tools/latest/bin/sdkmanager --sdk_root=${ANDROID_HOME} --install 'platforms;android-28' | tr '\r' '\n' | uniq
	echo "y" | $ANDROID_HOME/cmdline-tools/latest/bin/sdkmanager --sdk_root=${ANDROID_HOME} --install 'extras;android;m2repository' | tr '\r' '\n' | uniq
	echo "y" | $ANDROID_HOME/cmdline-tools/latest/bin/sdkmanager --sdk_root=${ANDROID_HOME} --install 'system-images;android-28;google_apis_playstore;x86_64' | tr '\r' '\n' | uniq
	echo "y" | $ANDROID_HOME/cmdline-tools/latest/bin/sdkmanager --sdk_root=${ANDROID_HOME} --install "system-images;android-$ANDROID_SIMULATOR_APILEVEL;google_apis_playstore;x86_64" | tr '\r' '\n' | uniq

	if [[ -f $ANDROID_HOME/platform-tools/platform-tools/adb ]]
	then
		# It appears that the platform-tools 29.0.6 are extracting into an incorrect path
		mv $ANDROID_HOME/platform-tools/platform-tools/* $ANDROID_HOME/platform-tools
	fi

	# Create emulator
	echo "no" | $ANDROID_HOME/cmdline-tools/latest/bin/avdmanager create avd -n "$AVD_NAME" --abi "x86_64" -k "system-images;android-$ANDROID_SIMULATOR_APILEVEL;google_apis_playstore;x86_64" --sdcard 128M --force

	# based on https://docs.microsoft.com/en-us/azure/devops/pipelines/agents/hosted?view=azure-devops&tabs=yaml#hardware
	# >> Agents that run macOS images are provisioned on Mac pros with a 3 core CPU, 14 GB of RAM, and 14 GB of SSD disk space.
	echo "hw.cpu.ncore=3" >> $AVD_CONFIG_FILE

	$ANDROID_HOME/emulator/emulator -list-avds

	echo "Checking for hardware acceleration"
	$ANDROID_HOME/emulator/emulator -accel-check

	echo "Starting emulator"

	# kickstart ADB
	$ANDROID_HOME/platform-tools/adb devices

	# Start emulator in background
	nohup $ANDROID_HOME/emulator/emulator -avd "$AVD_NAME" -skin 1280x800 -no-window -gpu swiftshader_indirect -no-snapshot -noaudio -no-boot-anim > $BUILD_ARTIFACTSTAGINGDIRECTORY/screenshots/android/android-emulator-log.txt 2>&1 &

	# Wait for the emulator to finish booting
	source $BUILD_SOURCESDIRECTORY/build/workflow/scripts/android-uitest-wait-systemui.sh 500

else
	# Restart the emulator to avoid running first-time tasks
	$ANDROID_HOME/platform-tools/adb reboot

	# Wait for the emulator to finish booting
	source $BUILD_SOURCESDIRECTORY/build/workflow/scripts/android-uitest-wait-systemui.sh 500
fi

# list active devices
$ANDROID_HOME/platform-tools/adb devices

# Workaround for https://github.com/microsoft/appcenter/issues/1451
$ANDROID_HOME/platform-tools/adb shell settings put global hidden_api_policy 1

echo "Emulator started"

cd $BUILD_SOURCESDIRECTORY

# build the sample, while the emulator is starting
mono '/Applications/Visual Studio.app/Contents/MonoBundle/MSBuild/Current/bin/MSBuild.dll' /m /r /p:Configuration=Release $UNO_UITEST_PROJECT
mono '/Applications/Visual Studio.app/Contents/MonoBundle/MSBuild/Current/bin/MSBuild.dll' /m /r /p:Configuration=Release /p:IsUiAutomationMappingEnabled=True /p:DisableNet6MobileTargets=True /p:UnoUIUseRoslynSourceGenerators=False /p:AndroidBuildApplicationPackage=True $UNO_UITEST_ANDROID_PROJECT

cd $BUILD_SOURCESDIRECTORY/build

wget $UNO_UITEST_NUGET_URL
mono nuget.exe install NUnit.ConsoleRunner -Version $UNO_UITEST_NUNIT_VERSION

# Move to the screenshot directory so that the output path is the proper one, as
# required by Xamarin.UITest
cd $UNO_UITEST_SCREENSHOT_PATH

## Build the NUnit configuration file
echo "--trace=Verbose" > $UNO_TESTS_RESPONSE_FILE
echo "--framework=mono" >> $UNO_TESTS_RESPONSE_FILE
echo "--inprocess" >> $UNO_TESTS_RESPONSE_FILE
echo "--agents=1" >> $UNO_TESTS_RESPONSE_FILE
echo "--workers=1" >> $UNO_TESTS_RESPONSE_FILE
echo "--result=$UNO_ORIGINAL_TEST_RESULTS" >> $UNO_TESTS_RESPONSE_FILE
echo "$UNO_UITEST_BINARY" >> $UNO_TESTS_RESPONSE_FILE

echo Response file:
cat $UNO_TESTS_RESPONSE_FILE

mono $BUILD_SOURCESDIRECTORY/build/NUnit.ConsoleRunner.$UNO_UITEST_NUNIT_VERSION/tools/nunit3-console.exe \
    @$UNO_TESTS_RESPONSE_FILE || true

## Dump the emulator's system log
$ANDROID_HOME/platform-tools/adb shell logcat -d > $BUILD_ARTIFACTSTAGINGDIRECTORY/screenshots/android/android-device-log.txt

if [ ! -f "$UNO_ORIGINAL_TEST_RESULTS" ]; then
	echo "ERROR: The test results file $UNO_ORIGINAL_TEST_RESULTS does not exist (did nunit crash ?)"
	return 1
fi
