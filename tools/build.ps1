Import-Module "$PSScriptRoot\setenv.psm1" -WarningAction Ignore -Force -ErrorAction Stop
Write-Configuration
Remove-Build
Remove-GraphEngineCache

mkdir "$REPO_ROOT\build" -Force 
mkdir "$REPO_ROOT\bin"   -Force 
cd "$REPO_ROOT\build"

Register-LocalRepo
cmake -G "Visual Studio 15 2017 Win64" --host=x64 $REPO_ROOT
cmake --build . --config Release
