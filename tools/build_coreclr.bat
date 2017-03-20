if [%REPO_ROOT%] == [] (
  set REPO_ROOT=%cd%
)

setlocal enabledelayedexpansion

set MSBUILD_EXE="C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe"

set TRINITY_C_SLN=%REPO_ROOT%\src\Trinity.C\Trinity.C.sln
set TRINITY_CORE_SLN=%REPO_ROOT%\src\Trinity.Core\Trinity.Core.NETStandard.sln

:: Run msbuild to build Trinity.C
%MSBUILD_EXE% /p:Configuration=Release-CoreCLR %TRINITY_C_SLN%
if %errorlevel% neq 0 exit /b %errorlevel%

:: Copy Linux native Trinity.C from lib
copy "%REPO_ROOT%\lib\libTrinity.so" "%REPO_ROOT%\bin\coreclr\"

:: Run dotnet to restore nuget packages for Trinity.Core
dotnet restore %TRINITY_CORE_SLN%
if %errorlevel% neq 0 exit /b %errorlevel%

:: Run dotnet to build Trinity.Core
dotnet build %TRINITY_CORE_SLN%
if %errorlevel% neq 0 exit /b %errorlevel%

:: Run dotnet to pack GraphEngine.CoreCLR nuget package
dotnet pack %TRINITY_CORE_SLN%
if %errorlevel% neq 0 exit /b %errorlevel%
