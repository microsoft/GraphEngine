# freebase-likq: hosting LIKQ queries on the freebase data set

In this demo we show how to host LIKQ queries on the freebase data set. 
In general, hosting LIKQ queries over a data set involves the following steps:

- Design the storage schema, and import the raw data into Graph Engine storage.
- Tag data fields in the schema file (TSL) with [GraphEdge] attribute so that LIKQ recognizes these TSL fields as graph edges.
- Create an application referencing GraphEngine.LIKQ and GraphEngine.Core packages (GraphEngine.Core will be automatically installed as a dependency).
- In the application, register the LIKQ module into a TrinityServer before starting it.
- For a particular data set, one may want to do some customization (polymorphic type system, custom indexing backend etc.). See `Program.cs` for details.

Please refer to `GraphEngine.DataImporter` sample project in the `experimental` branch to see how to automatically generate schema file and import the data.
For brevity, in this demo we only focus on how to host LIKQ queries with prebuilt data image/TSL script (The data/TSL script are imported with the importer).

A particularlly interesting feature of this demo is that it supports polymorphism for an entity (e.g. a `people_person` could also be a `film_actor`).
This is achieved by alternating the way that LIKQ accesses the storage. By default, when LIKQ visits a graph node, it directly allocates a TSL accessor specifically
built for a type (e.g. `people_person_Accessor`). Instead, we store each facet of an entity into one cell, and create a root cell that holds metadata of the entity.
We then create an adapter that implements the generic `ICellAccessor` interface, but not bound to a particular type.
When LIKQ requests a field from this adapter, it first queries the root cell to validate the operation, then re-routes the data access operation to one of the "physical" accessors.

A sample index service that interfaces with the LIKQ module is provided.
When query constraints are specified in a query, the LIKQ module calls a registered index service to retrieve vertices satisfying the constraints.
The constraints are specified in the match object, which is a json object. LIKQ itself does not specify the DSL syntax for the match object so here
we eatablish a simple one, which routes the constraints into SQLite.

## Building and running the demo

In the `master`/`dev` branch, simply build the freebase-likq solution. In the `experimental` branch,
it requires the experimental GraphEngine.Core and GraphEngine.LIKQ packages, so you should build them first and put them
into a local nuget package source, so that Nuget package manager can restore these packages during a build.

After the solution is built successfully, run `freebase-likq.exe`, it will automatically download the Freebase data image, build the SQLite index and start serving.
To enable the HTTP endpoints, either elevate the program, or grant the current user account the permission to listen to port 80: `netsh http add urlacl url=http://+:80/ user=Domain\username`.