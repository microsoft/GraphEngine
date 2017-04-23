---
id: GraphGenerator
title: Graph Generator
permalink: /docs/manual/DemoApps/GraphGenerator.html
---

##### <a href="https://github.com/Microsoft/GraphEngine/tree/master/samples/GraphGenerator" target="_blank">Source Code on GitHub</a>

Graph generator is a useful tool for generating synthetic graphs. We
demonstrate how to write a simple yet efficient graph generator for
generating RMAT graphs using GE.

## Graph Modeling

Before we start working on generating a graph, we need to first define
the schema of the graph nodes. A possible graph node schema
specification can be:

```C#
cell struct GraphNode
{
  string Label;
  List<CellId> Edges;
}
```

This works. However, the graph generation process usually poses a
great pressure on GC.  In this demo application, we demonstrate how to
use a simple space reservation mechanism to improve performance by
reducing the GC-pressure. The trick is straightforward: instead of
appending _CellIds_ one by one to `Edges`, we reserve some edge slots
in the `Edges` container before actually generating graph edges. At
the same time, we use an `EdgeCount` to indicate the actual number of
edges inserted to the current graph node. The refined graph node
schema is as follows:

```C#
cell struct GraphNode
{
  int EdgeCount;
  string Label;
  List<CellId> Edges;
}
```

## Graph Generation

To work with the edge reservation mechanism described above, we split
the graph generation process into three steps: 

* Generate graph nodes and reserve edge slots; 
* Insert edges to the existing graph nodes;
* Clean up the graph nodes to remove the unused edge slots.

#### Graph Node Generation

In the newly created graph node cell, we reserve some empty edge
slots. The code sketch is:

```C#
...
List<long> reservationList = new List<long>((int)capacity);
...
GraphNode node = new GraphNode(nodeId, 0, label, reservationList);
Global.LocalStorage.SaveGraphNode(node);
```

#### Graph Edge Generation

We append a new edge to the `Edges` container of a graph node if there
are one or more free reserved edge slots. Otherwise, we need to
allocate a larger `Edges` container before we can append a new
edge. The code sketch is:

```C#
// y1 is the cell id of a new edge
// y2 is the cell id of the current graph node
using (var node = Global.LocalStorage.UseGraphNode(y2))
{
    if (!node.Edges.Contains(y1))
    {
        if (node.EdgeCount < node.Edges.Count)
        {
            node.Edges[node.EdgeCount++] = y1;
        }
        else
        {
            ...
            List<long> reserved_list = new List<long>();
            ...
            node.Edges.AddRange(reserved_list);
            node.Edges[node.EdgeCount++] = y1;
        }
    }
}
```

#### Cleaning Up

At the end, we need to clean up the unused edge slots to save
space. This is the code sketch:

```C#
using (var node = Global.LocalStorage.UseGraphNode(i))
{
    if (node.EdgeCount < node.Edges.Count)
    {
        node.Edges.RemoveRange(node.EdgeCount, node.Edges.Count - node.EdgeCount);
    }
}
```
