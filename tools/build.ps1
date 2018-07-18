Import-Module "$PSScriptRoot\setenv.psm1" -WarningAction Ignore -Force -ErrorAction Stop
Write-Configuration
Remove-Build
Remove-GraphEngineCache

mkdir "$REPO_ROOT\build" -Force 
Set-Location "$REPO_ROOT\build"
cmake -G "Visual Studio 15 2017 Win64" --host=x64 $REPO_ROOT
cmake --build . --config Release --target install

New-Package -proj $TRINITY_CORE_SLN
Register-LocalRepo
Remove-GraphEngineCache -prefix "GraphEngine.Core"

New-Package -proj $LIKQ_SLN
Invoke-Sub "$TRINITY_CLIENT_ROOT\build.ps1"
New-Package -proj $TRINITY_STORAGE_COMPOSITE_SLN
New-Package -proj $TRINITY_DYNAMICCLUSTER_SLN
Invoke-Sub "$TRINITY_SERVICE_FABRIC_ROOT\build.ps1"
Invoke-Sub "$SPARK_MODULE_ROOT\build.ps1"
Invoke-Sub "$TRINITY_JIT_ROOT\build.ps1"
Invoke-Sub "$TRINITY_FFI_ROOT\build.ps1"
