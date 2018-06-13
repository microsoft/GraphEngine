if [%REPO_ROOT%] == [] (
  set REPO_ROOT=%~dp0..
)

call "%REPO_ROOT%\tools\setenv.bat"
if %errorlevel% neq 0 exit /b %errorlevel%

setlocal

:: Run msbuild to build Trinity.C
"%MSBUILD_EXE%" /p:Configuration=Release "%TRINITY_C_SLN%"
if %errorlevel% neq 0 exit /b %errorlevel%

:: Run msbuild to build Trinity.TSL.CodeGen
"%MSBUILD_EXE%" /p:Configuration=Release "%TRINITY_TSL_SLN%"
if %errorlevel% neq 0 exit /b %errorlevel%

:: Run nuget to restore nuget packages for Trinity.Core
"%NUGET_EXE%" restore "%TRINITY_CORE_SLN%"
if %errorlevel% neq 0 exit /b %errorlevel%

:: Run msbuild to build Trinity.Core
rd /S /Q "%REPO_ROOT%\src\Trinity.Core\obj"
"%MSBUILD_EXE%" /p:Configuration=Release "%TRINITY_CORE_SLN%"
if %errorlevel% neq 0 exit /b %errorlevel%
