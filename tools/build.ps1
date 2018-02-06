Import-Module "$PSScriptRoot\setenv.psm1" -WarningAction Ignore -Force -ErrorAction Stop
Write-Configuration
Remove-Build
Remove-GraphEngineCache
Invoke-MSBuild $TRINITY_C_SLN -config "Release"
Invoke-MSBuild $TRINITY_C_SLN -config "Release-CoreCLR"
Invoke-MSBuild $TRINITY_TSL_SLN
# Copy Linux native Trinity.C/Trinity.TSL.CodeGen
Copy-Item -Path "$REPO_ROOT\lib\libTrinity.so" -Destination "$REPO_ROOT\bin" -ErrorAction Stop
Copy-Item -Path "$REPO_ROOT\tools\Trinity.TSL.CodeGen" -Destination "$REPO_ROOT\bin" -ErrorAction Stop
New-Package -proj $TRINITY_CORE_SLN
Register-LocalRepo
New-Package -proj $LIKQ_SLN
Invoke-Sub "$TRINITY_CLIENT_ROOT\build.ps1"
New-Package -proj $TRINITY_STORAGE_COMPOSITE_SLN
New-Package -proj $TRINITY_DYNAMICCLUSTER_SLN
New-Package -proj $TRINITY_SERVICE_FABRIC_SLN
Invoke-Sub "$SPARK_MODULE_ROOT\build.ps1"
Invoke-Sub "$TRINITY_FFI_ROOT\build.ps1"