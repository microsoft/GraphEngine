@echo off

if [%REPO_ROOT%] == [] (
  set REPO_ROOT=%~dp0..
)

setlocal enabledelayedexpansion

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

set NUGET_EXE="%REPO_ROOT%\tools\NuGet.exe"

if not exist %NUGET_EXE% (
  powershell -Command "Invoke-WebRequest https://dist.nuget.org/win-x86-commandline/latest/nuget.exe -OutFile %NUGET_EXE%"
  if !errorlevel! neq 0 exit /b !errorlevel!
)

set TRINITY_C_SLN=%REPO_ROOT%\src\Trinity.C\Trinity.C.sln
set TRINITY_TSL_SLN=%REPO_ROOT%\src\Trinity.TSL\Trinity.TSL.sln
set TRINITY_CORE_SLN=%REPO_ROOT%\src\Trinity.Core\Trinity.Core.sln
set LIKQ_SLN=%REPO_ROOT%\src\Modules\LIKQ\LIKQ.sln
set SPARK_MODULE_ROOT=%REPO_ROOT%\src\Modules\Spark

:: Run msbuild to build Trinity.C
%MSBUILD_EXE% /p:Configuration=Release %TRINITY_C_SLN%
if %errorlevel% neq 0 exit /b %errorlevel%

:: Run msbuild to build Trinity.TSL.CodeGen
%MSBUILD_EXE% /p:Configuration=Release %TRINITY_TSL_SLN%
if %errorlevel% neq 0 exit /b %errorlevel%

:: Run nuget to restore nuget packages for Trinity.Core
%NUGET_EXE% restore %TRINITY_CORE_SLN%
if %errorlevel% neq 0 exit /b %errorlevel%

:: Run msbuild to build Trinity.Core
%MSBUILD_EXE% /p:Configuration=Release %TRINITY_CORE_SLN%
if %errorlevel% neq 0 exit /b %errorlevel%

:: Register local nuget source
:: calling `nuget sources list` will create the config file if it does not exist
%NUGET_EXE% sources list
%NUGET_EXE% sources Remove -Name "Graph Engine OSS Local"
%NUGET_EXE% sources Add -Name "Graph Engine OSS Local" -Source %REPO_ROOT%\bin\
:: Clear local nuget cache
:: for /f %i in ('dir /a:d /s /b %REPO_ROOT%\tests\packages\GraphEngine.Core*') do rmdir /S /Q %i
:: Ignore local nuget source errors

:: TODO currently LIKQ relies on .NET 4.6 which is not on our build
:: server. Disable it temporarily now.
:: Run nuget to restore nuget packages for LIKQ
:: %NUGET_EXE% restore %LIKQ_SLN%
:: if %errorlevel% neq 0 exit /b %errorlevel%

:: TODO: Fix the build of LIKQ\FanoutSerach\FanoutSearch.UnitTest
:: and then build LIKQ.sln as a whole

:: Run msbuild to build FanoutSearch
:: %MSBUILD_EXE% /p:Configuration=Release %REPO_ROOT%\src\Modules\LIKQ\FanoutSearch\FanoutSearch.csproj
:: if %errorlevel% neq 0 exit /b %errorlevel%

:: Run msbuild to build LIKQ nuget package
:: %MSBUILD_EXE% /p:Configuration=Release %REPO_ROOT%\src\Modules\LIKQ\BuildNuGetPkg\BuildNuGetPkg.csproj
:: if %errorlevel% neq 0 exit /b %errorlevel%

:: Run msbuild to build FanoutSearch.Server
:: %MSBUILD_EXE% /p:Configuration=Release %REPO_ROOT%\src\Modules\LIKQ\FanoutSearch.Server\FanoutSearch.Server.csproj
:: if %errorlevel% neq 0 exit /b %errorlevel%

:: Build spark module
call %SPARK_MODULE_ROOT%\build.bat
exit /b %errorlevel%
