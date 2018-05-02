Import-Module "$PSScriptRoot\setenv.psm1" -WarningAction Ignore -Force -ErrorAction Stop
Write-Configuration
Remove-Build
Remove-GraphEngineCache
Invoke-Sub "$REPO_ROOT\src\build-core.ps1"
New-Package -proj $LIKQ_SLN
Invoke-Sub "$TRINITY_CLIENT_ROOT\build.ps1"
New-Package -proj $TRINITY_STORAGE_COMPOSITE_SLN
New-Package -proj $TRINITY_DYNAMICCLUSTER_SLN
Invoke-Sub "$TRINITY_SERVICE_FABRIC_ROOT\build.ps1"
Invoke-Sub "$SPARK_MODULE_ROOT\build.ps1"
Invoke-Sub "$TRINITY_JIT_ROOT\build.ps1"
Invoke-Sub "$TRINITY_FFI_ROOT\build.ps1"
