# Instructions for building and running FanoutSearch.Server

- Build the required NuGet packages by running `tools/build.ps1` in the root directory of this repository if you haven't done so.
- cd `src/Modules/LIKQ/FanoutSearch.Server`
- Run `dotnet restore`
- Run `dotnet build --configuration Release`
- Run `dotnet run --framework net7.0 --property:Configuration=Release --no-build -- --help`
