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

Note that the full Freebase image is ~`32GB` so make sure that you run the demo on a beefy server!
We also have prepared a smaller dataset `freebase-film-dataset.zip`. Please modify `Program.cs` to
direct the program to download the small dataset if you want to.

Now you can query Freebase via LIKQ. Here is a quick example:

```
//Find wives of Tom Cruise through a 2-hop graph traversal.
Freebase
	.StartFrom(530972568887245, select: new[]{"type_object_name"})
	.FollowEdge("people_person_spouse_s")
	.VisitNode(Action.Continue)
	.FollowEdge("people_marriage_spouse")
	.VisitNode(Action.Return, select: new[]{"type_object_name"});
```

To issue this query via RESTful API, POST the query payload to `http://server/FanoutSearch/ExternalQuery/`:

```
{
	"queryString": "Freebase
	.StartFrom(530972568887245, select: new[]{\"type_object_name\"})
	.FollowEdge(\"people_person_spouse_s\")
	.VisitNode(Action.Continue)
	.FollowEdge(\"people_marriage_spouse\")
	.VisitNode(Action.Return, select: new[]{\"type_object_name\"});"
}
```

And the result shall look like this:

```
{
  "result": "[[{\"CellID\":530972568887245,\"type_object_name\":\"Tom Cruise\"},[],{\"CellID\":438165252269041},[],{\"CellID\":524140155134870,\"type_object_name\":\"Katie Holmes\"}],[{\"CellID\":530972568887245,\"type_object_name\":\"Tom Cruise\"},[],{\"CellID\":290269080985430},[],{\"CellID\":435682361078655,\"type_object_name\":\"Mimi Rogers\"}],[{\"CellID\":530972568887245,\"type_object_name\":\"Tom Cruise\"},[],{\"CellID\":332530798387447},[],{\"CellID\":547400553082314,\"type_object_name\":\"Nicole Kidman\"}],[{\"CellID\":530972568887245,\"type_object_name\":\"Tom Cruise\"},[],{\"CellID\":292606011314464},[],{\"CellID\":360255961521166,\"type_object_name\":\"Penélope Cruz\"}]]"
}
```
