if ["%REPO_ROOT%"] == [""] (
  set REPO_ROOT=%~dp0..
)

call "%REPO_ROOT%\tools\setenv.bat"
if %errorlevel% neq 0 exit /b %errorlevel%
setlocal

:: Clear local nuget cache
rmdir /S /Q %USERPROFILE%\.nuget\packages\graphengine.*

:: Run msbuild to build Trinity.C
"%MSBUILD_EXE%" /p:Configuration=Release "%TRINITY_C_SLN%"
if %errorlevel% neq 0 exit /b %errorlevel%

:: Run msbuild to build Trinity.C.CoreCLR
"%MSBUILD_EXE%" /p:Configuration=Release-CoreCLR %TRINITY_C_SLN%
if %errorlevel% neq 0 exit /b %errorlevel%

:: Run msbuild to build Trinity.TSL.CodeGen
"%MSBUILD_EXE%" /p:Configuration=Release "%TRINITY_TSL_SLN%"
if %errorlevel% neq 0 exit /b %errorlevel%

:: Copy Linux native Trinity.C from lib
copy "%REPO_ROOT%\lib\libTrinity.so" "%REPO_ROOT%\bin\"

:: Copy Linux native Trinity.TSL.CodeGen from tools
copy "%REPO_ROOT%\tools\Trinity.TSL.CodeGen" "%REPO_ROOT%\bin\"

:: Run dotnet to restore nuget packages for Trinity.Core
"%DOTNET_EXE%" restore "%TRINITY_CORE_SLN%"
if %errorlevel% neq 0 exit /b %errorlevel%

:: Run dotnet to build Trinity.Core
"%DOTNET_EXE%" build -c Release "%TRINITY_CORE_SLN%"
if %errorlevel% neq 0 exit /b %errorlevel%

:: Run dotnet to pack GraphEngine.Core nuget package
"%DOTNET_EXE%" pack -c Release "%TRINITY_CORE_SLN%"
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
"%DOTNET_EXE%" restore "%LIKQ_SLN%"
if %errorlevel% neq 0 exit /b %errorlevel%

:: Run dotnet to build LIKQ
"%DOTNET_EXE%" build -c Release "%LIKQ_SLN%"
if %errorlevel% neq 0 exit /b %errorlevel%

:: Run dotnet to pack GraphEngine.Core nuget package
"%DOTNET_EXE%" pack -c Release "%LIKQ_SLN%"
if %errorlevel% neq 0 exit /b %errorlevel%

:: Run dotnet to restore Trinity.Client
"%DOTNET_EXE%" restore "%TRINITY_CLIENT_SLN%"
if %errorlevel% neq 0 exit /b %errorlevel%

:: Run dotnet to build Trinity.Client
"%DOTNET_EXE%" build -c Release "%TRINITY_CLIENT_SLN%"
if %errorlevel% neq 0 exit /b %errorlevel%

:: Run dotnet to pack Trinity.Client
"%DOTNET_EXE%" pack -c Release "%TRINITY_CLIENT_SLN%"
if %errorlevel% neq 0 exit /b %errorlevel%

:: Run dotnet to restore Trinity.DynamicCluster
"%DOTNET_EXE%" restore "%TRINITY_DYNAMICCLUSTER_SLN%"
if %errorlevel% neq 0 exit /b %errorlevel%

:: Run dotnet to build Trinity.DynamicCluster
"%DOTNET_EXE%" build -c Release "%TRINITY_DYNAMICCLUSTER_SLN%"
if %errorlevel% neq 0 exit /b %errorlevel%

:: Run dotnet to pack Trinity.DynamicCluster
"%DOTNET_EXE%" pack -c Release "%TRINITY_DYNAMICCLUSTER_SLN%"
if %errorlevel% neq 0 exit /b %errorlevel%

:: Run dotnet to restore Trinity.ServiceFabric
"%DOTNET_EXE%" restore "%TRINITY_SERVICE_FABRIC_SLN%"
if %errorlevel% neq 0 exit /b %errorlevel%

:: Run dotnet to build Trinity.ServiceFabric
"%DOTNET_EXE%" build -c Release "%TRINITY_SERVICE_FABRIC_SLN%"
if %errorlevel% neq 0 exit /b %errorlevel%

:: Run dotnet to pack Trinity.ServiceFabric
"%DOTNET_EXE%" pack -c Release "%TRINITY_SERVICE_FABRIC_SLN%"
if %errorlevel% neq 0 exit /b %errorlevel%

:: Build spark module
call "%SPARK_MODULE_ROOT%\build.bat"
exit /b %errorlevel%