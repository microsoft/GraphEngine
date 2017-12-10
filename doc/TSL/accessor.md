---
id: accessor
title: Accessors 
permalink: /docs/manual/TSL/accessor.html
prev: /docs/manual/TSL/tsl-basics.html
next: /docs/manual/TSL/RESTProtocol.html
---

### Design Rationale

GE has a distributed in-memory infrastructure called Memory
Cloud. The memory cloud is composed of a set of memory trunks.  Each
machine in the cluster hosts 256 memory trunks. The reason we
partition a machine's local memory space into multiple memory trunks
is twofold: 1) Trunk level parallelism can be achieved without any
locking overhead; 2) Memory trunk uses a hashing mechanism to
do memory addressing. The performance of a single large hash table is
suboptimal due to a higher probability of hashing conflicts.

GE memory cloud provides key-value access interfaces. Keys are 64-bit
globally unique identifiers. Values are blobs of arbitrary
length. Because the memory cloud is distributed across multiple
machines, we cannot address a key-value pair using its physical memory
address.  In order to locate the value of a given key, we first 
identify the machine that stores the key-value pair, and then
locate the key-value pair in one of the memory trunks on that machine.

For different GE applications, the value component in a
key-value pair has different data structure or data schema. We use the
term cell to denote a value component. For example, let us consider a
cell with the following content: 1) an 32-bit integer Id; 2) a list
of 64-bit integers. In other words, we want to implement a
variable-length list (with an Id) as a _value_ component. In C#, such
a structure can be defined as follows:

```C#
struct CellA
{
  int Id;
  List<long> Links;
}
```

It is a challenge to efficiently store structured typed data and
support efficient and elegant operations.

We can directly use objects supported by most of the object-oriented
languages, such as C++ or C#, to model user data. This offers a
convenient and intuitive way of data manipulation. We can manipulate
an object by its interface, for example, `int Id = cell.Id` or
`cell.Links[0] = 1000001`, where _cell_ is an object of the type. This
approach, although simple and elegant, has significant
disadvantages. First, the storage overhead of holding objects in
memory is surprisingly high. Second, the language runtime is generally
not designed for handling massive amount of objects. The system
performance decreases dramatically as the number of objects goes very
large.  Third, it takes significant amount of time to load and store
data because serializing/deserializing objects is time consuming
especially the data is big.

Alternatively, we can treat _value_ components as blobs and access
data through pointers. Storing data as a blob minimizes memory
overhead. It may also boost the data manipulation performance since no
data deserialization is involved in the data manipulation. However,
the system is not aware of the schema of the data. We need to know the
exact memory layout before we can actually manipulate the data in the
blob. In other words, we need to use pointers and address offsets to
access the data elements in the blob. This makes programming difficult
and error-prone.

For the data structure described above, we need to know that field
_Id_ is stored at offset 0 and the first element of _Links_ is located
at offset 8 in the blob. To get the value of field _Id_ and set the
value of _Links[0]_, he needs to write the following code:

```C#
byte* ptr = GetBlobPtr(123); // Get the memory pointer
int Id = *(int*)ptr; // Get Id
ptr += sizeof(int); 
int listLen = *(int*)ptr;
ptr+=sizeof(int);
*(long*)ptr = 1000001; // Set Links[0] to 1000001
```

Note that we cannot naively cast a blob to a structure defined using
the built-in keyword _struct_ provided by programming languages like
C#. That is, the code shown below will fail:

```C#
// C# code snippet
struct
{
  int Id;
  List<long> Links;
}
....

struct * cell_p = (struct*) GetBlobPtr();
int id = *cell_p.Id; 
*cell_p.Links[0] = 100001;
```

This is because data fields such as _Links_ in the above example are
reference data fields. The data fields of such a struct are not flatly
laid out in the memory. We cannot manipulate a flat memory region
using a structured data pointer.

We can also treat _value_ components as blobs, and declare and access
data through a high level language. In the third approach, we define
and manipulate data through a declarative language (such as
SQL). However, typically, a declarative language either has very
limited expressive power or does not have efficient implementation.
But expressive power and efficiency are extremely important for
GE.

GE stores the user data as blobs instead of runtime objects
so that the storage overhead is minimized. At the same time,
GE enables us to access the data in an object-oriented
manner as we do in C# or Java. For example, in GE, we can
do the following even the data we operate on is a blob.

```C#
CellA cell = new CellA();
int Id = cell.Id;
cell.Links[0] = 1000001;
```

In other words, we can still operate on blobs in an elegant,
object-oriented manner. GE achieves this via a _cell
accessor_ mechanism. Specifically, we first declare the data schema
using a TSL script. GE compiles the script and generates
cell accessors for the cell constructs defined in the TSL
script. Then, we can access the blob data through the cell accessors
as if the data is runtime C# objects, but in fact, it is the cell
accessors that map the fields declared in the cell constructs to the
correct memory locations. All data accessing operations will be
correctly mapped to the correct memory locations without any memory
copy overhead.

Let us use an example to demonstrate how a cell accessor works. To use
a cell accessor, we must first specify its cell structure. This is
done using TSL. For the previous example, we define the data structure
in TSL as follows:

```C#
cell struct CellA
{
  int Id;
  List<long> Links;
}
```

Note that _CellA_ is not a _struct_ definition in C#, although it
looks similar. This code snippet will be compiled into a
_CellA_Accessor_ by the TSL compiler. The compiler generates code for
translating operations on the fields of _CellA_ to memory manipulation
operations on the underlying blob. After compilation, we can use
object-oriented data accessing interfaces to access the data in the
blob as shown below.

<object type="image/svg+xml" style="width:30em; display:block;
margin-left:auto;margin-right:auto;"
data="/img/svg/CellAccessor.svg">The browser does not support
SVG.</object>

Besides the intuitive data manipulation interfaces, cell accessors
also provide thread-safe data manipulation guarantee. GE is
designed to run in a heavily multi-threaded environment, where a large
number of cells interact with each other in very complex patterns. To
make the application developers' life easier, GE provides
thread-safe cell manipulation interfaces via cell accessors.

### Usage

Some usage rules must be applied when using accessors.

#### Accessors cannot be cached

An accessor works like a data 'pointer', it cannot be cached for
future use because the memory block pointed by an accessor may be
moved by other accessor operations. For example, if we have a `MyCell`
defined as:

```C#
cell struct MyCell
{
  List<string> list;
}
```

```C#
TrinityConfig.CurrentRunningMode = RunningMode.Embedded;
Global.LocalStorage.SaveMyCell(0, new List<string> { "aaa", "bbb", "ccc", "ddd"});
using (var cell = Global.LocalStorage.UseMyCell(0))
{
    Console.WriteLine("Example of non-cached accessors:");
    IEnumerable<StringAccessor> enumerable_accessor_collection = cell.list.Where(element => element.Length >= 3);
    foreach(var accessor in enumerable_accessor_collection)
    {
        Console.WriteLine(accessor);
    }
    Console.WriteLine("Example of cached accessors:");
    List<StringAccessor> cached_accessor_list = cell.list.Where(element => element.Length >= 3).ToList(); // Note the ToList() at the end
    foreach (var accessor in cached_accessor_list)
    {
        Console.WriteLine(accessor);
    }
}
```

The code snippet shown above will output:

```C#
Example of non-cached accessors:
aaa
bbb
ccc
ddd
Example of cached accessors:
ddd
ddd
ddd
ddd
```

For the example of the non-cached accessors, the value of the
accessors are evaluated at the time when they are being used. For the
example of the cached accessors, the list of accessors returned by
`cell.list.Where(element => element.Length >= 3).ToList()` are
actually just references pointing to the same accessor and this
accessor points to the last element of `cell.list`.

#### Cell accessors and message accessors must be disposed after use

A cell accessor is a
[disposable](https://msdn.microsoft.com/en-us/library/system.idisposable%28v=vs.110%29.aspx)
object. A cell accessor must be disposed after it is used. In C#, a
disposable object can be cleaned up via _using_ constructs.

```C#
long cellId = 314;
using (var myAccessor = Global.LocalStorage.UseMyCell(cellId))
{
  // Do something with myAccessor
}
```

> A _cell accessor_ or _message accessor_ must be properly
  disposed. Unmanaged resource leaks occur if an accessor is not
  disposed after use.

The _request_/_response_ reader and writer of a TSL protocol are
called _Message Accessors_. They are also disposable objects. They
must be properly disposed after use as well.

```C#
using (var request = new MyRequestWriter(...))
{
  using (var response = Global.CloudStorage.MyProtocolToMyServer(0, request))
  {
    // Do something with the response
  }
}
```

#### Cell accessors cannot be used in a nested manner

There is a spin-lock associated with each cell accessor. Nested usage
of cell accessors may cause deadlock.  The cell accessors shown in the
following code snippet are used incorrectly.

```C#
using (var accessorA = Global.LocalStorage.UseMyCellA(cellIdA))
{
  using(var accessorB = Global.LocalStorage.UseMyCellB(cellIdB))
  {
    // Nested cell accessors may cause deadlocks
  }
}
```

> A cell accessor should not be nested inside of another. Two or more
   nested cell accessors may cause deadlocks. 

Note, a cell accessor and one or more request/response
reader(s)/writer(s) can be used in a nested manner though.