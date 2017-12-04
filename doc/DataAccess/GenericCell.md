---
id: GenericCell
title: Generic Cells
permalink: /docs/manual/DataAccess/GenericCell.html
---

There are situations where we want to retrieve the data from a field
given its name alone, without knowing the type of the cell. For
example, when we have thousands of cell types and we want to write a
common routine to retrieve the value of a given field, it will be too
complicated to implement due to the richness of the schema. 

To address this issue, we can use an _ICell_ or an
_ICellAccessor_. _ICell_ is an interface implemented by all the
strongly-typed cell types specified in TSL, and so is _ICellAccessor_
for the strongly-typed accessor counterparts. They provide generic
manipulation methods on cells. Operations done via such methods will
be routed to certain fields of the strongly-typed cell/accessor
object.  We can access a cell's fields via a few unified
_Get/Set/Append_ interfaces, or select fields tagged with certain
attributes with _SelectFields/EnumerateValues_.

{% comment %}
The advantage of doing so is to avoid the high cost of using
reflection to dynamically allocate a cell/accessor object and retrieve
the data within, as the code in the generic cell/accessor are static
and much simpler than the built-in reflection mechanism. Also, it
enables generic programming without prior knowledge about the details
of data schemata.
{% endcomment %}

### Basic usage

The data access interfaces of accessing generic cells/accessors are
similar to those of strongly-typed cells, but not specialized for
user-defined data models. These interfaces are provided directly in
`Trinity.Core`, and available even without referencing a TSL project.
For local data accessing, `Trinity.Storage.LocalMemoryStorage` exposes
`LoadGenericCell`, `SaveGenericCell`, `UseGenericCell`, and
`NewGenericCell`; for remote data accessing,
`Trinity.Storage.MemoryCloud` exposes `LoadGenericCell` and
`SaveGenericCell`. For generic cell enumeration,
`Trinity.Storage.LocalMemoryStorage` provides `GenericCell_Selector`
and its accessor counterpart `GenericCellAccessor_Selector`.

We can call `Global.LocalStorage.LoadGenericCell` with a cell id to
load an _ICell_. Alternatively, we can allocate a new one by calling
`Global.LocalStorage.NewGenericCell`. Note that in `NewGenericCell`,
we must specify the type of the underlying strongly-typed cell by
providing a `CellType` string. Incorrect cell type will make the
method throw an exception. With a generic cell obtained, we can then
use `ICell.GetField<T>()` to retrieve a field (where `T` is the
desired output type) or `ICell.SetField()` to push a field back into
the cell.  `ICell.AppendToField()` treats a field as a container and
try to append the given value to its end. These three interfaces
manipulate fields directly by name, which means that we have to know
the exact name of the field.

These interfaces converts data types automatically --- it tries to
find a most suitable type if the desired data type is not exactly the
same as the type of the field. Ultimately, all the data types can be
converted into strings. For simple fields like `int`, `bool`, they
will be converted using the .NET built-in `ToString()` interface with
the default format. For complex data types, such as lists, arrays,
substructures, and cells, they will be serialized to JSON strings.
Arrays and lists will be regarded as native JSON arrays. Following
JSON's specification, strings will be escaped. Cells and substructures
are regarded as JSON objects, where each cell object has an additional
member `CellID`.

All the generated cell and struct classes have a `TryParse` interface,
which deserializes JSON strings back to objects. This is especially
useful when we are importing data or making communications over
RESTful APIs. The generic cell does not have `TryParse` as it is an
interface rather than a class. To parse a generic cell, one can call
the overloaded `Global.LocalStorage.NewGenericCell` with the JSON
string representation of the cell content. An interesting usage is to
declare a string field for a cell. It can then be used as a general
JSON store where objects can be deserialized from it.

### Accessing the metadata

Without prior knowledge about the cells, sometimes it is desirable to
inspect the structures of the cells, and the
[attributes](/docs/manual/TSL/tsl-basics.html#attributes) associated with
them. The interface `ICellDescriptor` provides metadata about the
cells and their fields. With it we can get the names, attributes, and
descriptors of the fields. `ICellDescriptor` also implements
`IAttributeCollection`, so that we can get a collection of attributes
associated with a cell.  There are two ways to obtain an
`ICellDescriptor`. Since both `ICell` and `ICellAccessor` implements
`ICellDescriptor`, so that we can directly invoke the metadata
accessing methods on them.  Another way is to use the generated class
`Schema`, which contains static metadata. Note that, if
`ICellDescriptor` is obtained via the `Schema` class, it contains only
static metadata, while the runtime objects will provide information
about the cells they are associated with. Calling
`ICell.GetFieldNames` will return the names of all *available* fields
in a particular cell, while the static one obtained from the `Schema`
class returns all the fields defined in the TSL.

### Generic programming 

We can leverage the [attributes](/docs/manual/TSL/tsl-basics.html#attributes) to
conduct generic programming. Attributes can be used to specify the
purpose of the fields. For example, we can mark a `List<CellId>`
storing neighbors of a graph node with an attribute `[GraphEdge]`, so
that a graph traversal library concerning this attribute can recognize
the field and enumerate the neighbors without knowing the field
name. Attributes to the field names in TSL are like tags to the file
paths in a file system, where file paths specify a hierarchical and
unique identifier, while a tag can be associated with multiple files,
identifying the purpose or semantics of the entries.

To select fields tagged with a specific attribute, call
`ICell.SelectFields<T>()`. The result is a list of `name-value` pairs
indicating the name and value of each field. Auto type conversion is
conducted on each of the fields. The method throws an exception when a
field cannot be automatically converted. It is desirable that the data
provider and the provider of a generic computation module agree on the
data type that an attribute implies, and the semantics of an attribute
(for example, `[GraphEdge]` in our example implies that the field
value should be convertible to a container of `CellId`). Furthermore,
to simplify the logic of handling different containers of an element
type, one can use `ICell.EnumerateValues<T>()`. The semantics of this
method is to first select fields according to the specified attribute
(and optionally with the attribute value), regard each of the
outcoming fields as a container of `T`, and enumerate its
elements. Therefore, if a cell has two fields of type `List<CellId>`
and `CellId` respectively, both tagged with `[GraphEdge]`, where the
former is a list of neighbors and the latter is a single edge,
`EnumerateValues<long>("GraphEdge")` will enumerate cell ids from both
fields, like a generic "flatten" operation.
