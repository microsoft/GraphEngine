@echo off

if [%REPO_ROOT%] == [] (
  set REPO_ROOT=%cd%\..\..\..
)

setlocal enabledelayedexpansion

set SPARK_MODULE_ROOT=%REPO_ROOT%\src\Modules\Spark

set VSWHERE_EXE=%REPO_ROOT%\tools\vswhere.exe
set VS_VERSION=[15.0,16.0)
set REQUIRES=Microsoft.Component.MSBuild Microsoft.VisualStudio.Component.VC.Tools.x86.x64

for /f "usebackq tokens=*" %%i in (`%VSWHERE_EXE% -version !VS_VERSION! -products * -requires %REQUIRES% -property installationPath`) do (
  set VS_INSTALLDIR=%%i
)

set MSBUILD_EXE="%VS_INSTALLDIR%\MSBuild\15.0\Bin\MSBuild.exe"
if not exist %MSBUILD_EXE% (
  echo [91m"Visual Studio 2017 or required components (%REQUIRES%) were not found"[0m
  exit /b 1
)

set NUGET_EXE=%REPO_ROOT%\tools\NuGet.exe

%NUGET_EXE% restore %SPARK_MODULE_ROOT%\Spark.sln
if %errorlevel% neq 0 exit /b %errorlevel%

%MSBUILD_EXE% /p:Configuration=Release %SPARK_MODULE_ROOT%\Spark.sln
if %errorlevel% neq 0 exit /b %errorlevel%
