#!/bin/bash
set -euo pipefail
IFS=$'\n\t'

export UNO_UITEST_IOS_PROJECT_PATH=$BUILD_SOURCESDIRECTORY/samples/$SAMPLE_APP_NAME/$SAMPLE_APP_NAME.Mobile

cd $UNO_UITEST_IOS_PROJECT_PATH
dotnet build -f net8.0-ios -c Release /p:RuntimeIdentifier=iossimulator-x64 /p:IsUiAutomationMappingEnabled=True /bl:$BUILD_ARTIFACTSTAGINGDIRECTORY/ios-$XAML_FLAVOR_BUILD-uitest.binlog
