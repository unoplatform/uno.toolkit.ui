#!/bin/bash
set -euo pipefail
IFS=$'\n\t'

export UNO_UITEST_ANDROID_PROJECT_PATH=$BUILD_SOURCESDIRECTORY/samples/Uno.Toolkit.Samples/Uno.Toolkit.Samples

cd "$UNO_UITEST_ANDROID_PROJECT_PATH"
dotnet publish -f net9.0-android -c Release /p:SamplesTargetFrameworkOverride=net9.0-android /p:TargetFrameworkOverride=net9.0-android /p:AndroidPackageFormat=apk /p:RuntimeIdentifier=android-x64 /p:IsUiAutomationMappingEnabled=true /p:AndroidUseSharedRuntime=false /p:AndroidUseAssemblyStore=false /p:AndroidFastDeploymentType=None /p:AotAssemblies=false /p:RunAOTCompilation=false /p:PublishTrimmed=false /p:AndroidStripILAfterAOT=false -bl:"$BUILD_ARTIFACTSTAGINGDIRECTORY/android-uitest-build.binlog"
