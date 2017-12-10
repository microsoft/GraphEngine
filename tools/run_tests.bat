if [%REPO_ROOT%] == [] (
  set REPO_ROOT=%~dp0..
)

set SPARK_MODULE_ROOT=%REPO_ROOT%\src\Modules\Spark

:: Run Spark module tests
call %SPARK_MODULE_ROOT%\run_tests.bat

:: Run GraphEngine tests
call %REPO_ROOT%\tests\run_tests.bat