# Friends Graph

Document: http://www.graphengine.io/docs/manual/DemoApps/FriendsGraph.html

- Build the required NuGet packages by running `tools/build.ps1` in the root directory of this repository
- Run `dotnet restore`
- Run `dotnet run`

If you want to build and run the release version, please run the following commands instead:

- `dotnet restore`
- `dotnet build --configuration Release`
- `dotnet run --property:Configuration=Release --no-build`
