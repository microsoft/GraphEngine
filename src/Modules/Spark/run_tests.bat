if [%REPO_ROOT%] == [] (
  set REPO_ROOT=%cd%\..\..\..
)

set SPARK_MODULE_ROOT=%REPO_ROOT%\src\Modules\Spark

set XUNIT_RUNNER=%SPARK_MODULE_ROOT%\packages\xunit.runner.console.2.2.0\tools\xunit.console.exe
set SPARK_MODULE_UTS=%SPARK_MODULE_ROOT%\SparkTrinityModule.UnitTests\bin\Release\SparkTrinityModule.UnitTests.dll

set XUNIT_REPORTS_DIR=%SPARK_MODULE_ROOT%\xUnitReports
rmdir %XUNIT_REPORTS_DIR% /S /Q
mkdir %XUNIT_REPORTS_DIR%

%XUNIT_RUNNER% %SPARK_MODULE_UTS% -xml %XUNIT_REPORTS_DIR%\SparkTrinityModule.UnitTests.xml
if %errorlevel% neq 0 exit /b %errorlevel%
