if [%REPO_ROOT%] == [] (
  set REPO_ROOT=%~dp0..
)

set MSBUILD_EXE="C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe"
set NUGET_EXE="%REPO_ROOT%\tools\NuGet.exe"
set DOTNET_EXE="dotnet"

::	%NUGET_EXE% restore %REPO_ROOT%\tests\test.csproj -PackagesDirectory %REPO_ROOT%\tests\packages
%MSBUILD_EXE% %REPO_ROOT%\tests\test.csproj
::	%DOTNET_EXE% build %REPO_ROOT%\tests\test_coreclr.csproj
