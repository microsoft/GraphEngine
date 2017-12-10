---
id: SSSP
title: Single Source Shortest Paths
permalink: /docs/manual/DemoApps/SSSP.html
---

##### <a href="https://github.com/Microsoft/GraphEngine/tree/master/samples/SSSP" target="_blank">Source Code on GitHub</a>

Vertex centric graph computation model provides an intuitive way of
computing single source shortest paths. Each vertex maintains its
current distance to the source vertex. Once a vertex's distance is
updated, it sends out its current shortest distance to its adjacent
vertices. When a vertex receives the shortest distances from its
neighbors, it re-calculates its shortest distance to the source
vertex. The process will continue until nothing can be updated.  In
the following, we use GE to model and implement this
algorithm for calculating single source shortest paths.

We first define a cell struct for this problem. First of all, we
define a _distance_ field to hold the current shortest distance to the
source vertex. To trace the path, we need a pointer pointing to its
_parent_ vertex from which it gets its latest shortest distance. Using
the _parent_ pointer, we can trace back to the source vertex from the
current vertex.

```C#
Cell Struct SSSPCell
{
	int distance;
	long parent;
	List<long> neighbors;
}

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

As shown in the tsl script shown above, the _SSSPCell_ has an integer
field _distance_ indicating its distance to the source vertex. We will
initially set it to the maximum positive value of 32-bit integer. The
second field _parent_ is the cell id of the vertex from which the
shortest path is routed. The field _neighbors_ stores the cell ids of
its neighbors.

Now we define two types of messages for the computation:
_StartSSSPMessage_ and _DistanceUpdatingMessage_. _StartSSSPMessage_
is used to initialize an SSSP computation.  It is a simple struct
containing only the cell id of the source vertex. A GE
client can kick off the computation by sending a _StartSSSPMessage_ to
all GE servers. Receiving this message, servers will check
whether the specified source vertex is in its _LocalStorage_. If this
is the case, it will load the source vertex's adjacent vertices and
propagates _DistanceUpdatingMessages_ to its neighbors. On receiving a
_DistanceUpdatingMessage_, a vertex will check whether its distance
can be updated according to the received shortest distance. The
corresponding tsl script is shown below.

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

The message handling logic can be implemented by override the
generated _SSSPServerBase_.

```C#
class SSSPServer : SSSPServerBase
{
    public override void DistanceUpdatingHandler(DistanceUpdatingMessageReader 
	request)
    {
        List<DistanceUpdatingMessage> DistanceUpdatingMessageList = 
		new List<DistanceUpdatingMessage>();
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
                            DistanceUpdatingMessageWriter msg = 
                                new DistanceUpdatingMessageWriter(cell.CellID.Value, 
                                cell.distance, sorter.GetCellRecipientList(i));
                            Global.CloudStorage.DistanceUpdatingToServer(i, msg);
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
                    DistanceUpdatingMessageWriter msg = new 
					DistanceUpdatingMessageWriter(rootCell.CellID.Value, 0,
					sorter.GetCellRecipientList(i));
                    Global.CloudStorage.DistanceUpdatingToServer(i, msg);
                }
            }
        }
    }
}
```

The key logic in the sample code shown above is very simple:

```C#
if (cell.distance > request.distance + 1)
{
    cell.distance = request.distance + 1;
    cell.parent = request.senderId;
}
```

We can try out the example by following the steps shown below:

* Generate a testing graph using `SSSP.exe -g NumberOfVertices`. For
  example, `SSSP.exe -g 10000`.

* Start computing each vertex's shortest distance to the specified
  source vertex by `SSSP.exe -c SourceVertexId`. For example,
  `SSSP.exe -c 1`.

* Get the shortest path between a specified vertex and the source
  vertex by `SSSP.exe -q VertexId`. For example, `SSSP.exe -q 123`.

A sample output of executing `SSSP.exe -q 123`:

```
Current vertex is 123, the distance to the source vertex is 3.
Current vertex is 1001, the distance to the source vertex is 2.
Current vertex is 2432, the distance to the source vertex is 1.
Current vertex is 1, the distance to the source vertex is 0.
```
