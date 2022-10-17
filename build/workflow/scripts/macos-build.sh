#!/bin/bash
set -euo pipefail
IFS=$'\n\t'

cd $BUILD_SOURCESDIRECTORY

mono "/Applications/Visual Studio.app/Contents/MonoBundle/MSBuild/Current/bin/MSBuild.dll" /m /r /p:UnoUIUseRoslynSourceGenerators=False /p:Configuration=Release /p:Platform=$BUILD_PLATFORM /p:PackageVersion=$BUILD_PACKAGEVERSION /p:GeneratePackageOnBuild=$BUILD_GENERATEPACKAGE /detailedsummary /bl:$BUILD_BINLOG $BUILD_SOLUTION