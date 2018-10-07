---
id: data-access-index
title: Data Access 
permalink: /docs/manual/DataAccess/index.html
---

### Basics

At the core of GE is a key-value store that handles raw
binary blobs.  You can save a blob to the GE key-value
store identified by a _key_ you specified and load it back into your
program later. A _key_ is a 64-bit integer; and the only native way of
referencing a GE data object (or a _cell_ in the jargon of
GE) is through such a _key_. Nevertheless, other type of
keys such as string keys can be easily realized by hashing the key to
a 64-bit integer. Note each key can at most be associated with one
blob _value_; saving two blob values with the same key will cause one
to be overwritten by the other. We will also refer to the concept of
the _key_ as _id_.

GE supports high performance concurrent key-value operators
on cells with atomicity guaranteed. The built-in atomic cell operators
include: _AddCell_, _SaveCell_, _LoadCell_, and _RemoveCell_.
Accesses on the cell with a particular key are serialized; changes
made by a writer are observed by everyone in a determined order.

Durability is optional in GE. We can choose whether to
commit a [write-ahead
log](#cell-access-options) (WAL) to
the local persistent storage or not before writing a cell. WAL
guarantees durability, but it comes with some performance penalty. Use
it wisely.

### Data Access Patterns

GE provides multiple data access patterns. We should weigh
the tradeoff between convenience and performance before we decide to
use one of the data access methods we introduce here.

#### Built-in Key-Value Store Interfaces

The most convenient one is to use the built-in key-value store
interfaces, such as _SaveCell_ and _LoadCell_. In GE, these
interfaces are provided on `Trinity.Global.LocalStorage` and
`Trinity.Global.CloudStorage` for accessing data locally or over a
cluster respectively. 

The built-in interfaces treat _cells_ as blobs. After we define our
own cell types in TSL, additional typed key-value store interfaces
will be bound to `Trinity.Global.LocalStorage` and
`Trinity.Global.CloudStorage`. The TSL-generated interfaces take the
form of `LoadMyType` and `SaveMyType`. For example, if we define a
type of cell in TSL as follows:

```C#
cell GraphNode
{
   [Index]
   string       Name;
   float        Value;
   List<CellId> Neighbors;
}
```

Two new interfaces `LoadGraphNode` and `SaveGraphNode` will be bound
to `LocalStorage` and `CloudStorage`.  Multiple method overloads are
provided for these interfaces. We can then write:

```C#
 var root = Global.CloudStorage.LoadGraphNode(123); //123 as the cell id
 var sum  = 0.0f;
 
 foreach(var neighbor in root.Neighbors)
 {
    sum += Global.CloudStorage.LoadGraphNode(neighbor).Value;
 }
```

#### Remote Selective Data Access

GE supports server-side computation. It means that we do
not need to pull all the concerned cells over the network and then
apply operations on them locally. We can only pass the piece of
information we are interested in over the network. To do so, we define
a custom protocol for client-server communication. Instead of issuing
cell loading/saving operations, we now can send a user-defined request
to the server for the desired result. For example, to obtain the sum
of _Value_ for a set of `GraphNode`, instead of loading them to the
client, we can define such a protocol:

```C#
 struct RequestMessage
 {
    List<CellId> NodeSet;
 }

 struct ResponseMessage
 {
    float Result;
 }

 protocol CalculateSum
 {
    Type     : Syn;
    Request  : RequestMessage;
    Response : ResponseMessage;
 }
```

On the client side we can get the sum by:

```C#
var sum = 0.0f;

for(int serverId = 0; serverId < Global.ServerCount; serverId++)
{
   using(var request  = new RequestMessageWriter(new List<long>(){1,2,3}))
   {
      using(var response = Global.CloudStorage.CalculateSumToMyServer(serverId, request))
      {
         var sum += response.Result;
      }
   }
}
```

The server side protocol logic can be implemented as follows:

```C#
public override void CalculateSumHandler(
    RequestMessageReader  request,
    ResponseMessageWriter response)
{
    response.Result = .0f;
    foreach(var nodeId in request.NodeSet)
    {
        response.Result += Global.LocalStorage.LoadGraphNode(nodeId).Value;
    }
}
```

#### Cell Accessors

We can actually perform any data access tasks via the key-value store
interfaces. But soon we will notice that the whole cell needs to be
loaded even we only want to access a single data field.
In the code snippet shown above, the cells are accessed for a certain
field. For each cell, GE first pinpoint its memory location
in the memory storage. Then it calls the runtime to allocate a cell
object and copies the cell content from the storage to the object. The
field is then read out from the object and fed into the outer
computation loop.

Cell modification is trickier. It takes three cell operations just to
modify a small portion of a cell: load the cell, modify the cell, and
save the cell. The memory copies incurred in this process waste a lot
of memory and network bandwidth. Moreover, even each cell operation is
atomic, the cell modification as a whole is not atomic because the
three individual cell operations cannot be guaranteed to be executed
as a whole.

Having seen what the problems caused by using key-value store
interfaces are, we give the cure now. In GE, we solve
problems discussed above via a mechanism called data _accessor_.
For any cell construct defined in a TSL script, the TSL compiler
automatically generates a cell accessor. An accessor does not possess
any data.  Rather, all the fields are provided as C# properties;
operations on these properties will be translated into in-place memory
operations on the underlying cell blob. So that the server logic can
be rewritten as:

```C#
public override void ComputeAverageHandler(
    RequestMessageReader  request,
    ResponseMessageWriter response)
{
    response.Result = .0f;
    foreach(var nodeId in request.NodeSet)
    {
        using( var neighbor = Global.LocalStorage.UseGraphNode(nodeId) )
        {
            response.Result += neighbor.Value;
        }
    }
}
```

In this new version, accessing `neighbor.Value` will only result in a
4-byte memory access. For more information on cell accessors, please
refer to [Accessors](/docs/manual/TSL/accessor.html).

> Whenever possible, use cell accessors instead of key-value store
  interfaces to access the data.

#### Cell Access Options

We can provide a cell access option to most of the cell access
interfaces. According to the cell interface, one or more options
listed below can be applied.

```C#
public enum CellAccessOptions
{
   // Throws an exception when a cell is not found.
   ThrowExceptionOnCellNotFound,

   // Returns null when a cell is not found.
   ReturnNullOnCellNotFound,

   // Creates a new cell when a cell is not found.
   CreateNewOnCellNotFound,

   // Specifies that write-ahead-log should be performed with strong durability.
   StrongLogAhead,

   // Specifies that write-ahead-log should be performed with weak durability.
   // This option brings better performance, but under certain circumstances 
   // the log may fail to be persistent, for example, during a power outage 
   // or equipment failure.
   WeakLogAhead
}
```

Cell access options are used to control the behavior of a cell
operation.  For example, we can specify the _write-ahead-log_ level
for the cell access interfaces that may change the state of a cell.

```C#
long cellId = 1;

// MyCell is a cell type defined in a TSL project
Global.LocalStorage.SaveMyCell(CellAccessOptions.StrongLogAhead, cellId, ...);

Global.LocalStorage.RemoveCell(CellAccessOptions.WeakLogAhead, cellId);

using (var cell = Global.LocalStorage.UseMyCell(cellId, CellAccessOptions.ReturnNullOnCellNotFound))
{
    // Do something here
}

using (var cell = Global.LocalStorage.UseMyCell(3, CellAccessOptions.CreateNewOnCellNotFound))
{
    // Do something here
}
```
### Cell Selector

GE provides enumerators for iterating the cells stored in the local memory storage.
Note that enumeration on cloud memory storage is currently not
supported. 

 For each cell type defined in TSL, the TSL compiler generates a
_selector_ interface on the local memory storage:
`Global.LocalStorage.MyCellType_Selector`.  As the name implies, the
selector interface selects all the cells of the given type and returns
an `IEnumerable<MyCellType>` collection. We can then use _foreach_ to
iterate though the collection:

```C#
 foreach( var node in Global.LocalStorage.GraphNode_Selector() )
 {
    //Do something...
 }
```

Both cell objects and cell accessors are supported. Here is the
_accessor_ counterpart:

```C#
 foreach( var accessor in Global.LocalStorage.GraphNode_Accessor_Selector() )
 {
    //Do something...
 }
```

Enumerating cells through a selector is thread-safe; Multiple
enumerations can be carried out simultaneously.  However, it is not
allowed to cache _accessors_, because the accessor object will be
reused to point to other cells during enumeration. Thus, the following
piece of code would cause data corruption or system crash:

```C#
 // Attempting to cache cell accessors
 List<GraphNode_Accessor> accessor_cache = new List<GraphNode_Accessor>();
 foreach( var accessor in Global.LocalStorage.GraphNode_Accessor_Selector() )
 {
    accessor_cache.Add(accessor);
 }
 // It will crash on visiting the cached accessors!
 var f = accessor_cache.Sum(accessor.AdditionalData); 
```

#### Language-Integrated Query (LINQ)

The _selectors_ implement the _IEnumerable<T>_ interface, hence they
support LINQ/PLINQ queries. If substring index attribute is specified
for some cell fields, some queries can leverage the inverted index
directly to speed up the query processing.  It is convenient to
implement common data query logic using LINQ, for example:

```C#
 var results = from node in Global.LocalStorage.GraphNode_Accessor_Selector()
               where node.name.Contains("Alice")
               select node.Value;
 var min     = results.Min();
```

In this example, the _node.name.Contains_ clause is translated into a
substring query. The results are then projected into a list of float
numbers, then aggregated using a built-in LINQ interface _Min()_.

GE LINQ/PLINQ is elaborated in the [Language-Integrated
Query](/docs/manual/DataAccess/LINQ.html) section.

### Substring Query

If substring index attribute is specified on a cell field in the TSL,
a set of substring query interfaces will be generated for the
specified cell field. A substring query interface accepts one or more
query strings and returns a list of matched cell ids.  Refer to the
[substring query](/docs/manual/DataAccess/InvertedIndex.html) section
for more information.