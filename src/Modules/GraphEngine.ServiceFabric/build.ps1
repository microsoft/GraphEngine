Import-Module -Force -WarningAction Ignore "$PSScriptRoot\..\..\..\tools\setenv.psm1"
& $NUGET_EXE Install -o packages
New-Package -proj "$TRINITY_SERVICE_FABRIC_ROOT\GraphEngine.ServiceFabric.sln" -config RelSkipSFApp