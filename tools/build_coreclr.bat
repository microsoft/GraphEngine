if ["%REPO_ROOT%"] == [""] (
  set REPO_ROOT=%~dp0..
)

call "%REPO_ROOT%\tools\setenv.bat"
if %errorlevel% neq 0 exit /b %errorlevel%
setlocal
set TRINITY_CORE_SLN=%REPO_ROOT%\src\Trinity.Core\Trinity.Core.NETStandard.sln

:: Run msbuild to build Trinity.C
"%MSBUILD_EXE%" /p:Configuration=Release-CoreCLR %TRINITY_C_SLN%
if %errorlevel% neq 0 exit /b %errorlevel%

:: Copy Linux native Trinity.C from lib
copy "%REPO_ROOT%\lib\libTrinity.so" "%REPO_ROOT%\bin\coreclr\"

:: Run msbuild to build Trinity.TSL.CodeGen
"%MSBUILD_EXE%" /p:Configuration=Release %TRINITY_TSL_SLN%
if %errorlevel% neq 0 exit /b %errorlevel%

:: Copy Linux native Trinity.TSL.CodeGen from tools
copy "%REPO_ROOT%\tools\Trinity.TSL.CodeGen" "%REPO_ROOT%\bin\"

:: Run dotnet to restore nuget packages for Trinity.Core
dotnet restore %TRINITY_CORE_SLN%
if %errorlevel% neq 0 exit /b %errorlevel%

:: Run dotnet to build Trinity.Core
dotnet build %TRINITY_CORE_SLN%
if %errorlevel% neq 0 exit /b %errorlevel%

:: Run dotnet to pack GraphEngine.CoreCLR nuget package
dotnet pack %TRINITY_CORE_SLN%
if %errorlevel% neq 0 exit /b %errorlevel%

:: Register local nuget source
"%NUGET_EXE%" sources Remove -Name "Graph Engine OSS Local CoreCLR"
"%NUGET_EXE%" sources Add -Name "Graph Engine OSS Local CoreCLR" -Source %REPO_ROOT%\bin\coreclr
:: Clear local nuget cache
rmdir /S /Q %USERPROFILE%\.nuget\packages\graphengine.coreclr
:: Ignore local nuget source errors
exit /b 0
