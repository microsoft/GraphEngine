# freebase-likq: hosting LIKQ queries on the freebase data set

In this demo, we demonstrate how to serve LIKQ queries for the
[Freebase](https://en.wikipedia.org/wiki/Freebase) data set.  It
generally takes the following 3 steps to serve LIKQ queries for a data
set:

- Specify the storage schema for the data in [TSL](https://www.graphengine.io/docs/manual/TSL/index.html). Remember to tag graph edges using [GraphEdge] attribute in the TSL script.
- Import data into Graph Engine.
- Create a C# project referencing GraphEngine.LIKQ and register the LIKQ module into a TrinityServer (refer to the sample code for an example).

For brevity, in this demo we focus on how to serve LIKQ queries with a
prebuilt data image.  Please refer to the `GraphEngine.DataImporter`
sample project in the `experimental` branch to see how to
automatically generate a schema file and import data.

A sample index service based on SQLite that interacts with the LIKQ
module is provided.  When query constraints are specified in a LIKQ
query, the LIKQ module calls a registered index service to retrieve
the graph nodes satisfying the constraints.  The constraints are
specified in the _match_ object, which is a json object. LIKQ itself
does not parse the _match_ object. The constraints specified by the
_match_ object will be relayed to the index backend and parsed by the
index backend.

## Building and running the demo

After the freebase-likq solution is built successfully, run
`freebase-likq.exe`. It will automatically download the Freebase data
image, build the SQLite index and start serving LIKQ.  To enable the
HTTP endpoints, either run the program as administrator, or grant the
current user the permission to listen to port 80: `netsh http add
urlacl url=http://+:80/ user=Domain\username`.

Now you can query Freebase via LIKQ. Here is a quick example:
```
Freebase
	.StartFrom("{ 'name' : 'Batman'}", select: new[]{"*"})
	.FollowEdge("film_film_actor")
	.VisitNode(Action.Return, select: new[]{"type_object_name"});
```

It is worth noting that Freebase entities are usually of multiple
types. For example, a person might be a book author and a film actor
at the same time.

