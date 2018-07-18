Import-Module -Force -WarningAction Ignore "$PSScriptRoot\..\..\..\tools\setenv.psm1"

Write-Output "========> Checking Python build environment"
$python  = Get-Command -ErrorAction SilentlyContinue -Type Application "python"
$swig    = Get-Command -ErrorAction SilentlyContinue -Type Application "swig"
# todo check for pip dependencies: setuptools etc.

if ($python -eq $null -or $swig -eq $null) {
  Write-Output ("========>[x] Python build environment not configured. Skipping Python FFI build.")
  return
}

$SOL_ROOT=$TRINITY_FFI_ROOT

Remove-GraphEngineCache -prefix "graphengine.ffi.python"

Remove-And-Print "$SOL_ROOT\Trinity.FFI.Python\build"
Remove-And-Print "$SOL_ROOT\Trinity.FFI.Python\dist"

New-Package     -proj "$SOL_ROOT\Trinity.FFI.Python\Trinity.FFI.Python.csproj"
Invoke-DotNet   -proj "$SOL_ROOT\Trinity.FFI.Python.UnitTests\Trinity.FFI.Python.UnitTests.csproj" -action restore
Invoke-DotNet   -proj "$SOL_ROOT\Trinity.FFI.Python.UnitTests\Trinity.FFI.Python.UnitTests.csproj" -action build -config Release

Invoke-DotNet   -proj "$SOL_ROOT\Trinity.FFI.Playground\Trinity.FFI.Playground.csproj" -action restore
Invoke-DotNet   -proj "$SOL_ROOT\Trinity.FFI.Playground\Trinity.FFI.Playground.csproj" -action build -config Release
