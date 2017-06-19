if [%REPO_ROOT%] == [] (
  set REPO_ROOT=%~dp0..
)

setlocal enabledelayedexpansion

set CSC_EXE="C:\Program Files (x86)\MSBuild\14.0\Bin\csc.exe"
set NEW_EXE=%REPO_ROOT%\tests\new_test.exe

if not exist %NEW_EXE% (
  %CSC_EXE% /out:%REPO_ROOT%\tests\new_test.exe %REPO_ROOT%\tests\new_test.cs
  if !errorlevel! neq 0 exit /b !errorlevel!
)

%NEW_EXE% %1