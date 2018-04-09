Import-Module -Force -WarningAction Ignore "$PSScriptRoot\..\..\..\tools\setenv.psm1"

Restore-GitSubmodules
Remove-GraphEngineCache -prefix "graphengine.jit"

Invoke-MSBuild -proj "$TRINITY_JIT_ROOT\GraphEngine.Jit.Native\GraphEngine.Jit.Native.vcxproj" -config Release -platform x64
New-Package    -proj "$TRINITY_JIT_ROOT\GraphEngine.Jit.sln"
