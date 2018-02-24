if [%REPO_ROOT%] == [] (
  set REPO_ROOT=%~dp0..
)

set NUGET_EXE="%REPO_ROOT%\tools\NuGet.exe"
set DOTNET_EXE="dotnet"

%DOTNET_EXE% msbuild /t:RunStressTests %REPO_ROOT%\tests\test_coreclr.csproj
