Import-Module -Force -WarningAction Ignore "$PSScriptRoot\..\..\..\tools\setenv.psm1"
Remove-GraphEngineCache -prefix "graphengine.ffi"
$SOL_ROOT=$TRINITY_FFI_ROOT

New-Package     -proj "$SOL_ROOT\Trinity.FFI.Metagen\Trinity.FFI.Metagen.fsproj"
New-Package     -proj "$SOL_ROOT\Trinity.FFI\Trinity.FFI.csproj"
Invoke-DotNet   -proj "$SOL_ROOT\Trinity.FFI.UnitTests\Trinity.FFI.UnitTests.csproj" -action restore
Invoke-DotNet   -proj "$SOL_ROOT\Trinity.FFI.UnitTests\Trinity.FFI.UnitTests.csproj" -action build -config Release

#Invoke-Sub "$SOL_ROOT\build-py.ps1"
