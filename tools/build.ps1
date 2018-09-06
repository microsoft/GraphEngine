Import-Module "$PSScriptRoot\setenv.psm1" -WarningAction Ignore -Force -ErrorAction Stop
Write-Configuration
Remove-Build

mkdir "$REPO_ROOT\build" -Force 
Push-Location "$REPO_ROOT\build"

cmake -G "Visual Studio 15 2017 Win64" --host=x64 $REPO_ROOT
cmake --build . --config Release
Pop-Location
