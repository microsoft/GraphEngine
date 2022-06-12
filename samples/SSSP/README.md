# Single Source Shortest Paths

Document: http://www.graphengine.io/docs/manual/DemoApps/SSSP.html

- Build the required NuGet packages by running `REPO_ROOT/tools/build.ps1`
- Run `dotnet restore`
- Run `dotnet build --configuration Release`

### Running the application

* Start the server: `dotnet run --property:Configuration=Release --no-build -- -s`
* Generate a graph via _SSSP.exe -g NumberOfVertices_. Open another console window, `dotnet run --property:Configuration=Release --no-build -- -g 10000`
* Start calculating each vertex's shortest distance to the specified source vertex via _SSSP.exe -c SourceVertexId_. For example, `dotnet run --property:Configuration=Release --no-build -- -c 1`
* Get the shortest path between a specified vertex and the source vertex via _SSSP.exe -q VertexId_. For example, `dotnet run --property:Configuration=Release --no-build -- -q 123`
