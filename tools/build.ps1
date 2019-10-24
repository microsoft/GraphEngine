param(
    [Switch]
    $VS2019
)

Import-Module "$PSScriptRoot\setenv.psm1" -WarningAction Ignore -Force -ErrorAction Stop
Write-Configuration
Remove-Build

mkdir "$REPO_ROOT\build" -Force 
Push-Location "$REPO_ROOT\build"

if ($VS2019) {
  cmake -G "Visual Studio 16 2019" -A x64 --host=x64 $REPO_ROOT
}else {
  cmake -G "Visual Studio 15 2017 Win64" --host=x64 $REPO_ROOT
}

cmake --build . --config Release

Pop-Location
