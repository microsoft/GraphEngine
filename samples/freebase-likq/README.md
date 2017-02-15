# freebase-likq: LIKQ for Freebase

In this sample project, we demonstrate how to serve LIKQ queries for
the [Freebase](https://en.wikipedia.org/wiki/Freebase) data set.  It
generally takes the following 3 steps to serve LIKQ queries for a data
set:

- Specify the storage schema for the data in [TSL](https://www.graphengine.io/docs/manual/TSL/index.html). Remember to tag graph edges using [GraphEdge] [attribute](https://www.graphengine.io/docs/manual/TSL/tsl-basics.html#attributes) in the TSL script.
- Import data into Graph Engine.
- Create a C# project to implement a TrinityServer. The C# project need to reference GraphEngine.LIKQ and register the LIKQ module (refer to the sample code for an example).

For brevity, in this sample project we focus on how to serve LIKQ
queries with a prebuilt data image.  Please refer to the
`GraphEngine.DataImporter` sample project in the `experimental` branch
to see how to automatically generate a schema file and import data.

LIKQ module does not provide a built-in index backend. In this sample
project, a sample index backend based on SQLite is provided.  When
query constraints are specified in a LIKQ query, the LIKQ module calls
a registered index backend to retrieve the graph nodes satisfying the
constraints.  The constraints are specified in a _match_ object, which
is a json object. LIKQ does not parse the _match_ object; it will
relay the _match_ object to the index backend and let the index
backend to interpret the constraints specified by the _match_ object.

## Building and running the demo

After the freebase-likq solution is built successfully, run
`freebase-likq.exe`. It will automatically download Freebase graph
image, build SQLite index and start serving LIKQ.  To enable the LIKQ
HTTP endpoint, either run the program as administrator, or grant the
current user the permission to listen to port 80: `netsh http add
urlacl url=http://+:80/ user=Domain\username`.

Now you can query Freebase via LIKQ. Here is a quick example:


