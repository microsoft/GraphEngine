---
id: DistributedHashtable
title: Distributed Hashtable
permalink: /docs/manual/DemoApps/DistributedHashtable.html
---

##### <a href="https://github.com/Microsoft/GraphEngine/tree/master/samples/DistributedHashtable" target="_blank">Source Code on GitHub</a>

Hashtable is among the most useful daily-used data structures. A
distributed hashtable is a hashtable served by a cluster of
machines. It can be accessed through network. We demonstrate how to
implement a distributed hashtable on top of GE and how it
can be accessed via user-defined interfaces.

## Data Model

A hashtable consists of a set of *buckets*, each of which is
collection of key-value pairs. We use string to represent both the
_Key_ and _Value_ in this demo application. In GE, we can
use a _cell_ as a hashtable bucket. We define _BucketCell_ in TSL for
this purpose:

```C#
struct KVPair
{
    string Key;
    string Value;
}

cell struct BucketCell
{
    List<KVPair> KVList;
}
```

Given an arbitrary _Key_, we first hash it to a 64-bit cell id. Then
we use this id to reference the cell bucket. 

## Setting A Key-Value Pair

Because we are working on a distributed hashtable, we need to define
protocols to allow the clients to manipulate the hashtable. Let us
first look at the protocol for setting a key-value pair.

The _Set_ protocol is very straightforward: we just need to send a
_Key_ (string) and a _Value_ (string) to the hashtable server. Here is
the protocol specification:

```C#
struct SetMessage
{
    string Key;
    string Value;
}

protocol Set
{
    Type:Syn;
    Request:SetMessage;
    Response:void;
}
```

The implementation logic of _Set_ operation is also straightforward:

```C#
public override void SetHandler(SetMessageReader request)
{
    long cellId = HashHelper.HashString2Int64(request.Key);
    using (var cell = Global.LocalStorage.UseBucketCell(cellId, CellAccessOptions.CreateNewOnCellNotFound))
    {
        int count = cell.KVList.Count;
        int index = -1;

        for (int i = 0; i < count; i++)
        {
            if (cell.KVList[i].Key == request.Key)
            {
                index = i;
                break;
            }
        }
        if (index != -1)
        {
            cell.KVList[index].Value = request.Value;
        }
        else
            cell.KVList.Add(new KVPair(request.Key, request.Value));
    }
}
```

In the cell bucket, we search the key against the key-value pair
list. If we find a match, we update the value; otherwise, we add a new
value.

## Getting A Key-Value Pair

_Get_ operation is even simpler. It fetches the value by the
user-specified key. This is the protocol specification:

```C#
struct GetMessage
{
    string Key;
}

struct GetResponse
{
    bool IsFound;
    string Value;
}

protocol Get
{
    Type:Syn;
    Request:GetMessage;
    Response:GetResponse;
}
```

This is the implementation logic:

```C#
public override void GetHandler(GetMessageReader request, GetResponseWriter response)
{
    long cellId = HashHelper.HashString2Int64(request.Key);
    response.IsFound = false;

    using (var cell = Global.LocalStorage.UseBucketCell(cellId, CellAccessOptions.ReturnNullOnCellNotFound))
    {
        if (cell == null)
            return;
        int count = cell.KVList.Count;
        for (int i = 0; i < count; i++)
        {
            if (cell.KVList[i].Key == request.Key)
            {
                response.IsFound = true;
                response.Value = cell.KVList[i].Value;
                break;
            }
        }
    }
}
```

If we do not find a matched bucket, we return immediately. Otherwise,
we search the key against the key-value pair list in the matched
bucket cell.
