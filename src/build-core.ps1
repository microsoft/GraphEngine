Import-Module "$PSScriptRoot\..\tools\setenv.psm1" -WarningAction Ignore -Force -ErrorAction Stop
Invoke-MSBuild $TRINITY_C_SLN -config "Release"
Invoke-MSBuild $TRINITY_TSL_SLN
# Copy Linux native Trinity.C/Trinity.TSL.CodeGen
Copy-Item -Path "$REPO_ROOT\lib\libTrinity.so" -Destination "$REPO_ROOT\bin" -ErrorAction Stop
Copy-Item -Path "$REPO_ROOT\tools\Trinity.TSL.CodeGen" -Destination "$REPO_ROOT\bin" -ErrorAction Stop
New-Package -proj $TRINITY_CORE_SLN
Register-LocalRepo
Remove-GraphEngineCache -prefix "GraphEngine.Core"
