@echo off

if ["%REPO_ROOT%"] == [""] (
  set REPO_ROOT=%~dp0..
)

setlocal enabledelayedexpansion
set VSWHERE_EXE="%REPO_ROOT%\tools\vswhere.exe"
set VS_VERSION=[15.0,16.0)
set REQUIRES=Microsoft.Component.MSBuild Microsoft.VisualStudio.Component.VC.Tools.x86.x64
for /f "usebackq tokens=*" %%i in (`"%VSWHERE_EXE%" -version !VS_VERSION! -products * -requires %REQUIRES% -property installationPath`) do (
  endlocal
  set VS_INSTALLDIR=%%i
)

set MSBUILD_EXE=%VS_INSTALLDIR%\MSBuild\15.0\Bin\MSBuild.exe
if not exist "%MSBUILD_EXE%" (
  echo [91m"Visual Studio 2017 or required components were not found"[0m
  exit /b 1
)

set NUGET_EXE=%REPO_ROOT%\tools\NuGet.exe

if not exist "%NUGET_EXE%" (
  setlocal enabledelayedexpansion
  powershell -Command "Invoke-WebRequest https://dist.nuget.org/win-x86-commandline/latest/nuget.exe -OutFile %NUGET_EXE%"
  if !errorlevel! neq 0 exit /b !errorlevel!
  endlocal
)

set TRINITY_CORE_SLN=%REPO_ROOT%\src\Trinity.Core\Trinity.Core.sln
set TRINITY_C_SLN=%REPO_ROOT%\src\Trinity.C\Trinity.C.sln
set TRINITY_TSL_SLN=%REPO_ROOT%\src\Trinity.TSL\Trinity.TSL.sln
set SPARK_MODULE_ROOT=%REPO_ROOT%\src\Modules\Spark
set LIKQ_SLN=%REPO_ROOT%\src\Modules\LIKQ\LIKQ.sln
