Import-Module -Force -WarningAction Ignore "$PSScriptRoot\..\..\..\tools\setenv.psm1"
NuGet Install -o packages
New-Package -proj $TRINITY_SERVICE_FABRIC_SLN