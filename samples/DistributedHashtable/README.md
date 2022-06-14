# Distributed Hashtable

Document: http://www.graphengine.io/docs/manual/DemoApps/DistributedHashtable.html

- Build the required NuGet packages by running `tools/build.ps1` in the root directory of this repository
- Run `dotnet restore`
- Run `dotnet build --configuration Release`

### Running the application

* Start the server:
    ```shell
    dotnet run --property:Configuration=Release --no-build -- -s
    ```
* Start the client:
    ```shell
    dotnet run --property:Configuration=Release --no-build -- -c
    ```
