Import-Module -Force -WarningAction Ignore "$PSScriptRoot\..\..\..\tools\setenv.psm1"

Write-Output "========> Checking GraphEngine.Jit build environment."
if (!(Get-Command cmake -ErrorAction SilentlyContinue)) {
  Write-Output ("========>[x] Cmake not configured. Skipping GraphEngine.Jit build.")
  return
}

Restore-GitSubmodules
Remove-GraphEngineCache -prefix "graphengine.jit"

$ASMJIT_BUILD =  "$TRINITY_JIT_ROOT\asmjit\build"
Remove-And-Print -item $ASMJIT_BUILD
New-Item -Type Directory -Force $ASMJIT_BUILD
Push-Location
Set-Location $ASMJIT_BUILD
& cmake .. -G "Visual Studio 15 2017 Win64"
Pop-Location

Invoke-MSBuild -proj "$ASMJIT_BUILD\asmjit.vcxproj" -config Release -platform x64

# Invoke-MSBuild -proj "$TRINITY_JIT_ROOT\GraphEngine.Jit.CppSharpCodegen\GraphEngine.Jit.CppSharpCodegen.csproj" -config Release -platform x64
New-Package -proj "$TRINITY_JIT_ROOT\GraphEngine.Jit.sln"
