# Distributed Hashtable

Document: http://www.graphengine.io/docs/manual/DemoApps/DistributedHashtable.html
- Build the required Nuget packages by running `REPO_ROOT/tools/build.ps1`
- `cd DistributedHashtable`
- run `dotnet build --configuration Release`

### Running the application

* Start the server: `dotnet run --property:Configuration=Release --no-build -- -s`
* Start the client: `dotnet run --property:Configuration=Release --no-build -- -c`
