if ["%REPO_ROOT%"] == [""] (
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

:: Register local nuget source
:: calling `nuget sources list` will create the config file if it does not exist
"%NUGET_EXE%" sources list
"%NUGET_EXE%" sources Remove -Name "Graph Engine OSS Local"
"%NUGET_EXE%" sources Add -Name "Graph Engine OSS Local" -Source "%TRINITY_OUTPUT_DIR%"
:: Clear local nuget cache
:: for /f %i in ('dir /a:d /s /b %REPO_ROOT%\tests\packages\GraphEngine.Core*') do rmdir /S /Q %i
:: Ignore local nuget source errors

:: Run nuget to restore nuget packages for LIKQ
"%NUGET_EXE%" restore "%LIKQ_SLN%"
if %errorlevel% neq 0 exit /b %errorlevel%

:: Run msbuild to build LIKQ
"%MSBUILD_EXE%" /p:Configuration=Release "%LIKQ_SLN%"
if %errorlevel% neq 0 exit /b %errorlevel%

:: Build spark module
call "%SPARK_MODULE_ROOT%\build.bat"
exit /b %errorlevel%
