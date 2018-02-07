Import-Module -Force -WarningAction Ignore "$PSScriptRoot\..\..\..\tools\setenv.psm1"
Remove-GraphEngineCache -prefix "graphengine.client"
New-Package -proj "$TRINITY_CLIENT_ROOT\GraphEngine.Client.sln"
