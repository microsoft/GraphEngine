@echo off

if [%REPO_ROOT%] == [] (
  set REPO_ROOT=%cd%\..\..\..
)

call %REPO_ROOT%\tools\setenv.bat
if %errorlevel% neq 0 exit /b %errorlevel%

%NUGET_EXE% restore %SPARK_MODULE_ROOT%\Spark.sln
if %errorlevel% neq 0 exit /b %errorlevel%

%MSBUILD_EXE% /p:Configuration=Release %SPARK_MODULE_ROOT%\Spark.sln
if %errorlevel% neq 0 exit /b %errorlevel%
