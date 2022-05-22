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
  Invoke-Expression "$CMAKE_EXE -G 'Visual Studio 16 2019' -A x64 $REPO_ROOT"
}else {
  Invoke-Expression "$CMAKE_EXE -G 'Visual Studio 17 2022' -A x64 $REPO_ROOT"
}

Invoke-Expression "$CMAKE_EXE --build . --config Release"

Pop-Location
