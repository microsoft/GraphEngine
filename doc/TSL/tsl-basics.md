---
id: tsl-basics
title: TSL Basics
permalink: /docs/manual/TSL/tsl-basics.html
prev: /docs/manual/TSL/index.html
next: /docs/manual/TSL/RESTProtocol.html
---

Identifiers are case-sensitive in TSL. Any legal C# identifier is
legal in TSL.  Line comments starting with `//` and block comments
surrounded by `/*` and `*/` are supported. The syntax of TSL is
similar to that of struct construct in C/C++ or C#.

The reserved keywords in TSL consists of three parts: all C# keywords,
TSL keywords, and primitive data types.

{% include tsl-keywords.html %}

{% include tsl-datatypes.html %}

### TSL Script

A TSL script may consist of one or more following top-level
constructs:

* TSL script inclusion statement

* Enum construct

* Struct construct

* Cell construct

* Protocol construct

* Server construct

* Proxy construct

We can include other TSL files in the current TSL project by using
"include" key word. For example, `include anthertsl.tsl`. Note, if a
TSL file is already added to the current TSL project, it is not
necessary to include it explicitly.

Enumerations are declared using _enum_ keyword. They can be referenced
in the current TSL project or the visual studio projects referencing
the TSL project.

#### Struct

A _struct_ specifies a user-defined type. It is composed of three
types of _fields_: built-in data types, other user-defined structs,
and container types. A container type stores a collection of data
elements. GE currently supports three container types:
_Array<T>_, _List<T>_, _String/string_.

TSL supports single-dimensional and multidimensional arrays.  A
single-dimensional array of type _T_ can be declared in the following
two ways: `Array<T>(3) myArray;` or `T[3] myArray;`.

For example:

```C#
Array<int32>(3) myArray;
int32[3] myArray;
```

Multidimensional arrays can be declared via: `T[m,n] myMultiArray;`

For example:

```C#
int32[3,5] mymultiArray;
```

Note TSL array does not support variable-length elements. Thus the following declaration is not allowed:

```C#
string [10] strings; // Not allowed!
```

A list of T can be declared via:`List<T> mylist;`

For example:

```C#
List<int32> mylist;
```

TSL String/string represents a sequence of characters.

Note that a modifier _optional_ can be applied to any field of a
struct.  Optional modifier indicates the current field can be absent.

Defining a struct in TSL is pretty much like defining a struct in
C/C++ or C#. For example:

```C#
struct MyStructA
{
    int32 Value;
    Guid Id;
    DateTime timestamp;
}
struct MyStructB
{
    int64 Link;
    List<float> Weights;
    bool[16] flags;
    string Name;
    bit[10] bitflags;
    MyStructA substruct;
    List<MyStructA> substructs;
}
```

#### Cell

A _Cell_ is a user-defined type. It is composed of three types of
fields: built-in types, other user-defined TSL structs, and container
types.

The storage layer of GE is a key-value store. The schema of
the _value_ part of a key-value pair can be specified by a TSL cell.
GE generates key-value store interfaces (_SaveCell_,  _LoadCell_,
and _UpdateCell_, etc.) and data access methods for each cell type.  A corresponding
cell 'accessor' will be generated for each TSL cell.  A _cell_ can be
accessed and manipulated through its cell _accessor_ interfaces. This
is a simple TSL cell:

```C#
cell MyCell
{
  int32 Value;
  string Name;
}
```

After compilation, we can access and manipulate _MyCell_ via its cell accessor:

```C#
long cellId = 1;
using(var cell = Global.LocalStorage.UseMyCell(cellId))
{
  Console.WriteLine("Value: {0}", cell.Value);
  cell.Name = "a new name";
}
```

> The manipulation is thread-safe when a cell is accessed via its cell
accessor.

At the storage level, a cell is just a blob of bytes in the main
memory. From the point view of a developer, a cell can be considered
as a flat structured data container. It looks very much like a
_struct_ in C/C++ or C#. A fundamental difference between a C/C++
_struct_ and a GE cell is their way of organizing
memory. All data fields in a cell are flattened in a continuous memory
region, while a C/C++ _struct_ may contain data field references
pointing to other data fields located at a different memory region.

### Data Modeling in TSL

_Cells_ are the basic building blocks of data modeling.  Here we use a
simple graph modeling example to demonstrate the basic of data
modeling techniques.

Note: any cell in GE is referenced by a 64-bit _CellId_,
thus a list of cells can be represented by `List<CellId>` in the cell
specification.

```C#
cell struct SimpleGraphNode
{
    List<CellId> Inlinks;
    List<CellId> Outlinks;
    string Name;
}
```

This cell type can be used to represent nodes of a simple directed
graph. Each node in this graph has a _name_ and a list of _inlinks_
and _outlinks_.

What if the graph edges have additional data associated. Here is an example:

```C#
struct MyEdge
{
    long Link;
    float Weight;
}

cell struct MyCell
{
    List<MyEdge> Inlinks;
    List<MyEdge> Outlinks;
    string Name;
}
```

This script contains a 'Cell' and a 'Struct'. This time, each edge
contains a _weight_. Sample code for accessing _MyCell_:


```C#
Global.LocalStorage.SaveMyCell(132);

using (var ca = Global.LocalStorage.UseMyCell(132))
{
    ca.Inlinks.Add(new MyEdge(10, 11));
    ca.Outlinks.Add(new MyEdge(11, 12));
    ca.Name = "Hello, Cell Manipulator!";
}

using (var ca = Global.LocalStorage.UseMyCell(132))
{
    Console.WriteLine("Inlink.Count: {0}", ca.Inlinks.Count);
    Console.WriteLine("Outlink.Count: {0}", ca.Outlinks.Count);
    Console.WriteLine("Cell Name: {0}", ca.Name);
}
```

### Protocol 

A _protocol_ specifies a contract between a message sender and its
receiver. It specifies three things: protocol type, request message,
and response message. For example:

```C#

struct MyRequest
{
  string Value;
}

struct MyResponse
{
  int32 Result;
}

protocol myProtocol
{
    Type: Syn;
    Request: MyRequest;
    Response: MyResponse;
}
```

Possible protocol _Type_ are: Syn, Asyn, and HTTP. 

If the protocol is a _Syn_ protocol: 

* Its _Request_ can be: _void_ or a user-defined struct.
* Its _Response_ can be: _void_ or a user-defined struct.

If the protocol is an _Asyn_ protocol:

* Its _Request_ can be: _void_ or a user-defined struct.
* Its _Response_ MUST be _void_.

If the protocol is a HTTP protocol:

* Its _Request_ can be: void, a user-defined struct, or _Stream_.
* Its _Response_ can be: void, a user-defined struct, or _Stream_.

A Request or Response with _Stream_ type can be manipulated via data stream interfaces.

### Server/Proxy/Module

We can bind a number of protocols to a server by defining a _server_
struct in TSL.  For example, if we have already defined two protocols
named protocolA and protocolB, then we can bind them to a server
struct called _myServer_:

```C#
server my_server
{
    protocol protocolA;
    protocol protocolB;
}
```

After compilation, an abstract base server named _myServerBase_
supporting both _protocolA_ and _protocolB_ will be generated. Two
abstract message handler _protocolAHandler_ and _protocolBHandler_
will be generated. We can implement our server logic by overriding the
generated handlers.

A _proxy_ can be defined similarly, except that the construct type
"proxy" is used.

Note that a server/proxy cannot inherit protocols from another
server/proxy instance.  This is because within a server/proxy
instance, the protocol Ids are numbered sequentially. When inheriting
protocols from other servers/proxies, different servers/proxies may
number the inherited protocols differently.  GE relies on protocol Ids
to dispatch messages. It causes message dispatching problems to number
the same protocol with different Ids.  If we want to make a group of
protocols reusable, we can group them into a _module_ struct:

```C#
module my_module
{
    protocol protocolC;
    protocol protocolD;
}
```

Then, we can "plug-in" the module into a server/proxy instance:

```C#
    ...
    my_server server = new my_server();
    server.RegisterCommunicationModule<my_module>();
```

In this way, a client that understands the protocols of `my_module`
can talk to the server.

### Attributes

An _attribute_ is a textual tag associated with a construct in TSL.
Attributes provide metadata about the construct and can be accessed at
run time. An attribute can be a string or a pair of strings, where it
is regarded as a key-value pair. A single-string attribute is regarded
as a key-value pair with an empty value. Duplicated keys are not
allowed on the same construct.  A few attributes are reserved for
built-in features, e.g.  [Inverted
Index](/docs/manual/DataAccess/InvertedIndex.html).

To tag a construct with an attribute, place the attribute above it.
The syntax is akin to that of C# attributes:

```C#
/// Tagging the cell
[GraphNode, BaseType : Person]
cell Student
{
    /// Tagging the field
    [GraphEdge : Outlinks]
    List<CellId> friends;
}
```

The key and its value of an attribute is separated with a colon (`:`).
Multiple attributes can reside within one pair of square brackets,
separated with a comma (`,`). The attribute strings can be any literal
strings with `:` and `,` escaped as `\:` and `\,`. Non-escaped spaces
at the start and the end of a string are trimmed, so that `[ a b :
123\ \ ]` will be interpreted as `[a b:123  ]`.

