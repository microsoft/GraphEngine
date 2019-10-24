# Single Source Shortest Paths 

Document: http://www.graphengine.io/docs/manual/DemoApps/SSSP.html
Make sure to first run `REPO_ROOT/tools/build.ps1` to build required nuget packages.

### Running the application

* Start the server: `SSSP.exe -s`
* Generate a graph via _SSSP.exe -g NumberOfVertices_. For example, `SSSP.exe -g 10000`
* Start calculating each vertex's shortest distance to the specified source vertex via _SSSP.exe -c SourceVertexId_. For example, `SSSP.exe -c 1`
* Get the shortest path between a specified vertex and the source vertex via _SSSP.exe -q VertexId_. For example, `SSSP.exe -q 2`

