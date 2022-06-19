Import-Module "$PSScriptRoot\setenv.psm1" -WarningAction Ignore -Force -ErrorAction Stop

Push-Location "$REPO_ROOT\build"
ctest -C Release
Pop-Location
exit 0
