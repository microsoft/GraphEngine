Import-Module -Force -WarningAction Ignore "$PSScriptRoot\..\..\..\tools\setenv.psm1"

Restore-GitSubmodules
Remove-GraphEngineCache -prefix "graphengine.jit"

New-Package    -proj "$TRINITY_JIT_ROOT\GraphEngine.Jit.sln"
