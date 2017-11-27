if [%REPO_ROOT%] == [] (
  set REPO_ROOT="%cd%\..\..\.."
)

set SPARK_MODULE_ROOT=%REPO_ROOT%\src\Modules\Spark

set MSBUILD_EXE="C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe"
set NUGET_EXE="%REPO_ROOT%\tools\NuGet.exe"

"%NUGET_EXE%" restore %SPARK_MODULE_ROOT%\Spark.sln
if %errorlevel% neq 0 exit /b %errorlevel%

%MSBUILD_EXE% /p:Configuration=Release %SPARK_MODULE_ROOT%\Spark.sln
if %errorlevel% neq 0 exit /b %errorlevel%
