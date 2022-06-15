---
id: SSSP
title: Single Source Shortest Paths
permalink: /docs/manual/DemoApps/SSSP.html
---

##### <a href="https://github.com/Microsoft/GraphEngine/tree/master/samples/SSSP" target="_blank">Source Code on GitHub</a>

The vertex centric graph computation model provides an intuitive way of
computing single source shortest paths. Each vertex maintains its current
distance to the source vertex. Once a vertex's distance is updated, it sends out
its shortest distance to its adjacent vertices. When a vertex receives the
shortest distances from its neighbors, it re-calculates its shortest distance to
the source vertex. The process continues until nothing can be updated. Now, we
use GE to implement this algorithm.

```C#
Cell Struct SSSPCell
{
	int distance;
	long parent;
	List<long> neighbors;
}
```

We first define a cell struct _SSSPCell_ in TSL for this problem. The _SSSPCell_
has an integer field _distance_ that holds the current shortest distance to the
source vertex. We will initially set it to the maximum positive value of 32-bit
integer. The second field _parent_ is the cell id of the vertex from which the
shortest path is routed. To trace the path, we need a pointer pointing to its
_parent_ vertex through which the current vertex has the shortest distance to
the source vertex. Using the _parent_ pointer, we can trace back to the source
vertex from the current vertex. The field _neighbors_ stores the cell ids of its
neighbors.

```C#
struct StartSSSPMessage
{
	long root;
}

struct DistanceUpdatingMessage
{
	long senderId;
	int distance;
	List<long> recipients;
}
```

Now we define two types of messages for the computation: _StartSSSPMessage_ and
_DistanceUpdatingMessage_. _StartSSSPMessage_ is used to initialize an SSSP
computation.  It is a simple struct containing only the cell id of the source
vertex. A GE client can kick off the computation by sending a _StartSSSPMessage_
to all GE servers. Receiving this message, the servers will check whether the
specified source vertex is in its _LocalStorage_. If this is the case, it will
load the source vertex's adjacent vertices and propagates
_DistanceUpdatingMessages_ to its neighbors. On receiving a
_DistanceUpdatingMessage_, a vertex will check whether its distance can be
updated according to the received shortest distance. The corresponding TSL
protocols are shown below.

```C#
protocol StartSSSP
{
	Type: Asyn;
	Request: StartSSSPMessage;
	Response: void;
}

protocol DistanceUpdating
{
	Type: Asyn;
	Request: DistanceUpdatingMessage;
	Response: void;
}

server SSSPServer
{
	protocol StartSSSP;
	protocol DistanceUpdating;
}
```

The message handling logic can be implemented by overriding the generated
_SSSPServerBase_.

```C#
class SSSPServerImpl : SSSPServerBase
{
    public override void DistanceUpdatingHandler(DistanceUpdatingMessageReader request)
    {
        List<DistanceUpdatingMessage> DistanceUpdatingMessageList = new List<DistanceUpdatingMessage>();
        request.recipients.ForEach((cellId) =>
        {
            using (var cell = Global.LocalStorage.UseSSSPCell(cellId))
            {
                if (cell.distance > request.distance + 1)
                {
                    cell.distance = request.distance + 1;
                    cell.parent = request.senderId;
                    Console.Write(cell.distance + " ");
                    MessageSorter sorter = new MessageSorter(cell.neighbors);

                    for (int i = 0; i < Global.ServerCount; i++)
                    {
                        DistanceUpdatingMessageWriter msg = new DistanceUpdatingMessageWriter(cell.CellId,
                            cell.distance, sorter.GetCellRecipientList(i));
                        Global.CloudStorage.DistanceUpdatingToSSSPServer(i, msg);
                    }

                }
            }
        });
    }

    public override void StartSSSPHandler(StartSSSPMessageReader request)
    {
        if (Global.CloudStorage.IsLocalCell(request.root))
        {
            using (var rootCell = Global.LocalStorage.UseSSSPCell(request.root))
            {
                rootCell.distance = 0;
                rootCell.parent = -1;
                MessageSorter sorter = new MessageSorter(rootCell.neighbors);

                for (int i = 0; i < Global.ServerCount; i++)
                {
                    DistanceUpdatingMessageWriter msg = new DistanceUpdatingMessageWriter(rootCell.CellId, 0, sorter.GetCellRecipientList(i));
                    Global.CloudStorage.DistanceUpdatingToSSSPServer(i, msg);
                }
            }
        }
    }
}
```

The key logic in the sample code shown above is truly simple:

```C#
if (cell.distance > request.distance + 1)
{
    cell.distance = request.distance + 1;
    cell.parent = request.senderId;
}
```

We can try out the example by following the steps shown below:

* Generate a testing graph using `SSSP.exe -g NumberOfVertices`. For example,
  `SSSP.exe -g 10000`.

* Start computing each vertex's shortest distance to the specified source vertex
  by `SSSP.exe -c SourceVertexId`. For example, `SSSP.exe -c 1`.

* Get the shortest path between a specified vertex and the source vertex by
  `SSSP.exe -q VertexId`. For example, `SSSP.exe -q 123`.

A sample output of executing `SSSP.exe -q 123`:

```
Current vertex is 123, the distance to the source vertex is 3.
Current vertex is 1001, the distance to the source vertex is 2.
Current vertex is 2432, the distance to the source vertex is 1.
Current vertex is 1, the distance to the source vertex is 0.
```
