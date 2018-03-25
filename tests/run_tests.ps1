Import-Module "$PSScriptRoot\..\tools\setenv.psm1" -WarningAction Ignore -Force -ErrorAction Stop

Get-ChildItem -File -Recurse -Filter "**.csproj" "$PSSCriptRoot\exetests" | % { Test-ExeProject -proj $_.FullName }
Get-ChildItem -File -Recurse -Filter "**.csproj" "$PSScriptRoot\unittests" | % { Test-Xunit -proj $_.FullName }
