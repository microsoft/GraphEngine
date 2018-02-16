Import-Module -Force -WarningAction Ignore "$PSScriptRoot\..\..\..\tools\setenv.psm1"
$SOL_ROOT=$TRINITY_FFI_ROOT
Invoke-Expression "git submodule init" -ErrorAction Stop
Invoke-Expression "git submodule update --recursive" -ErrorAction Stop

Remove-GraphEngineCache -prefix "graphengine.ffi.python"
Remove-GraphEngineCache -prefix "python.runtime"

New-Package     -proj "$SOL_ROOT\pythonnet\src\runtime\Python.Runtime.15.csproj" -config ReleaseWinPY3
Move-Item       -Path "$SOL_ROOT\pythonnet\src\runtime\bin\Python.Runtime.2.4.0.nupkg" -Destination $TRINITY_OUTPUT_DIR -Force

New-Package     -proj "$SOL_ROOT\Trinity.FFI.Python\Trinity.FFI.Python.csproj"
Invoke-DotNet   -proj "$SOL_ROOT\Trinity.FFI.Python.UnitTests\Trinity.FFI.Python.UnitTests.csproj" -action restore
Invoke-DotNet   -proj "$SOL_ROOT\Trinity.FFI.Python.UnitTests\Trinity.FFI.Python.UnitTests.csproj" -action build -config Release

Invoke-DotNet   -proj "$SOL_ROOT\Trinity.FFI.Playground\Trinity.FFI.Playground.csproj" -action restore
Invoke-DotNet   -proj "$SOL_ROOT\Trinity.FFI.Playground\Trinity.FFI.Playground.csproj" -action build -config Release