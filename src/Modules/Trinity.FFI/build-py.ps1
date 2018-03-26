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
Restore-GitSubmodules

Remove-GraphEngineCache -prefix "graphengine.ffi.python"
Remove-GraphEngineCache -prefix "python.runtime"

New-Package     -proj "$SOL_ROOT\pythonnet\src\runtime\Python.Runtime.15.csproj" -config ReleaseWinPY3
Move-Item       -Path "$SOL_ROOT\pythonnet\src\runtime\bin\Python.Runtime.2.4.0.nupkg" -Destination $TRINITY_OUTPUT_DIR -Force
Remove-And-Print "$SOL_ROOT\Trinity.FFI.Python\build"
Remove-And-Print "$SOL_ROOT\Trinity.FFI.Python\dist"

New-Package     -proj "$SOL_ROOT\Trinity.FFI.Python\Trinity.FFI.Python.csproj"
Invoke-DotNet   -proj "$SOL_ROOT\Trinity.FFI.Python.UnitTests\Trinity.FFI.Python.UnitTests.csproj" -action restore
Invoke-DotNet   -proj "$SOL_ROOT\Trinity.FFI.Python.UnitTests\Trinity.FFI.Python.UnitTests.csproj" -action build -config Release

Invoke-DotNet   -proj "$SOL_ROOT\Trinity.FFI.Playground\Trinity.FFI.Playground.csproj" -action restore
Invoke-DotNet   -proj "$SOL_ROOT\Trinity.FFI.Playground\Trinity.FFI.Playground.csproj" -action build -config Release
