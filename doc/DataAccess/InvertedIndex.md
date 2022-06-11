---
id: InvertedIndex
title: Inverted Index
permalink: /docs/manual/DataAccess/InvertedIndex.html
---

GE provides built-in inverted index support for substring queries on cell field
values. GE only indexes the cell field values that are marked with _[index]_
attribute in TSL.

Only the cell fields with _string_ type can be indexed. There are two cases.  1)
The type of the cell field is _string_. During substring query processing, a
cell is matched if the value of its indexed field contains the queried
substring.  2) The cell field is a collection of strings, e.g., `List<string>`
or `List<List<string>>`. During substring query processing, a cell is matched as
long as any string in the collection contains the queried substring.

### Index Declaration

To make a field indexed, add an `[index]` attribute to it:

```C#
 cell MyCell
 {
    [Index]
    string Name;
 }
```

> The index attribute is only valid for the fields whose type is
  string or a collection of strings.

It is possible to declare an index on a nested field, for example:

```C#
 struct leaf
 {
    [Index]
    string data;
 }

 cell root
 {
    leaf substructure;
 }
```

Because there is no way to globally identify such a struct in GE, the index
attribute on `struct leaf` alone does not produce a valid index.  Only when such
a struct is contained in a cell, say `root`, `root.leaf.data` becomes an
accessible cell field and a valid index will be built. It is allowed to have an
indexed field included in a substructure of a substructure,
`cell.inner_1.inner_2. ... .leaf.data` is indexable.

If a substructure with one or more indexed fields is included in multiple cell
structs, then an index will be built for each of these cell types, and the
indexes are independent with each other.

### Substring Queries

For an indexed field, we can issue a substring query against it by calling the
method `Index.SubstringQuery` with a field identifier and a query string.  A
field identifier, such as _Index.root.leaf.data_, is an automatically generated
nested class defined within the `Index` class. It is used to specify which cell
field we are going to query against: _Index.root.leaf.data_ identifies the
`data` field in the `leaf` struct within the root cell.
`Index.SubstringQuery(Index.root.leaf.data, "query string")` returns a list of
cell ids. Each root.leaf.data field of the corresponding cells contains the
"query string".

The method _Index.SubstringQuery_ also accepts a sequence of query strings.
Given a sequence of query strings `q1, q2, ..., qn`, this method will perform a
wildcard search with the `*q1*q2*...*qn*` pattern.  It means that the strings
`q1, q2, ..., qn` are substrings of a string in the order specified in the
sequence.

### Index Update

If the cells are continuously updated, the changes made to the indexed fields
may not be immediately reflected in the index.  That is, a substring query may
return outdated results. To rule out false positives (the previously matched
cells do not match now), we can check the field values again after
getting the matched cell ids.  It is not easy to address false negatives though,
i.e., a cell should be matched, but not included in the index yet.

The indexes are updated periodically by the system. To manually update an index,
we can call `Index.UpdateSubstringIndex()` on a specific field identifier.

### LINQ Integration

The inverted index subsystem is integrated with LINQ.  In a LINQ query over a
[selector](/docs/manual/DataAccess/index.html#cell-selector), GE translates
`String.Contains` on an indexed field into inverted index queries. The same
rule applies for `IEnumerable<string>.Contains`.  Refer to the
[LINQ](/docs/manual/DataAccess/LINQ.html) section for more details.
