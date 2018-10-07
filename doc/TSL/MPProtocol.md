---
id: MPProtocol
title: Message Passing Protocols
permalink: /docs/manual/TSL/MPProtocol.html
prev: /docs/manual/TSL/accessor.html
next: /docs/manual/TSL/RESTProtocol.html
---

For the dispersed machines, the only way that one machine talks to
another is via message passing over network.  We brought in the
concept of _protocol_ in [TSL
Basics](/docs/manual/TSL/tsl-basics.html). Three kinds of protocols
form the foundation of all kinds of distributed computation paradigms
on GE.

## Message Handling Flow

GE adopts _Request_ and _Response_ communication
paradigm. The program that plays the role of servicing requests is
called *server*. Correspondingly, the program that sends requests to a
_server_ is called *client*. We use _server_ or _client_ to refer to
the role played by a program. A program can act as both _server_ and
_client_ at the same time.

<object type="image/svg+xml" style="width:45em; display:block;
margin-left:auto;margin-right:auto;"
data="/img/svg/MessageHandling.svg">The browser does not
support SVG.</object>

The specification of the request/response message format and the
server-side message handling logic together is called a GE
_protocol_. The request and response of a _protocol_ resembles the
parameters and return value of a local function call, except that the
request handling logic is performed remotely at the server side.  The
request/response can be either a user-defined data structure specified
in TSL or _void_ in a GE protocol.

```C#
// A local function definition
Response func(Request myReq)
{
  // Request handling logic
}
```

Three types of protocols are supported in GE: synchronous
protocols, asynchronous protocols, and HTTP protocols.

## Synchronous Protocols

A synchronous protocol is akin to a normal synchronous function call,
except that the call is made across machine boundaries. It is usually
used to perform a synchronous function on the server side and await
the server side to response in a blocking manner as illustrated by the
time sequence diagram shown below.

<object type="image/svg+xml" style="width:25em; display:block;
margin-left:auto;margin-right:auto;"
data="/img/svg/SyncMessage.svg">The browser does not
support SVG.</object>

Here are a few synchronous protocol examples:

```C#
struct MyRequest
{
  string Value;
}

struct MyResponse
{
  int32 Result;
}

protocol mySynProtocol1
{
    Type: Syn;
    Request: MyRequest;
    Response: MyResponse;
}

protocol mySynProtocol2
{
    Type: Syn;
    Request: void;
    Response: void;
}

protocol mySynProtocol3
{
    Type: Syn;
    Request: MyRequest;
    Response: void;
}

protocol mySynProtocol4
{
    Type: Syn;
    Request: void;
    Response: MyResponse;
}
```

The request and response of a protocol can be _void_, as shown by
_mySynProtocol2_, _mySynProtocol3_, and _mySynProtocol4_.

## Asynchronous Protocols

For an asynchronous protocols, the server return an acknowledgement to
the client immediately after it receives the message. A thread from a
thread pool is then chosen to handle the received message as shown
below in the sequence diagram.

<object type="image/svg+xml" style="width:25em; display:block;
margin-left:auto;margin-right:auto;"
data="/img/svg/AsyncMessage.svg">The browser does not
support SVG.</object>

Here are a few examples:

```C#
struct MyRequest
{
  string Value;
}

protocol myAsynProtocol1
{
    Type: Asyn;
    Request: MyRequest;
    Response: void;
}

protocol myAsynProtocol2
{
    Type: Asyn;
    Request: void;
    Response: void;
}
```

Asynchronous protocols cannot return any user-defined data to the
client, because the server does not wait on the handler's completion
before sending back the acknowledgement.  Hence, the response of an
asynchronous protocol must be _void_, while the request can be a
user-defined message or void.  From the perspective of a client, the
fact that an asynchronous call returns only means the message is
successfully received by the remote peer.

## HTTP Protocols

An HTTP protocol is a synchronous remote procedure call. It is the
RESTful version of a _Syn_ protocol. It has almost the same time
sequence diagram with a _Syn_ protocol, except that the request and
response are Json structures.

<object type="image/svg+xml" style="width:25em; display:block;
margin-left:auto;margin-right:auto;"
data="/img/svg/HttpMessage.svg">The browser does not
support SVG.</object>

Here are a few HTTP API examples:

```C#
struct MyRequest
{
  string Value;
}

struct MyResponse
{
  int32 Result;
}

protocol myHttpProtocol1
{
    Type: Http;
    Request: MyRequest;
    Response: MyResponse;
}

protocol myHttpProtocol2
{
    Type: Http;
    Request: void;
    Response: void;
}

protocol myHttpProtocol3
{
    Type: Http;
    Request: MyRequest;
    Response: void;
}

protocol myHttpProtocol4
{
    Type: Http;
    Request: void;
    Response: MyResponse;
}
```

Just like _Syn_ protocols, request and response can be void or
user-defined data structures. GE will start a RESTful Http
API endpoint for each _Http_ protocol.

```lisp
http://example.com/myHttpProtocol1/
http://example.com/myHttpProtocol2/
http://example.com/myHttpProtocol3/
http://example.com/myHttpProtocol4/
```

HTTP protocols are supposed to be used to provide RESTful service
endpoints. They are not intended for inter-server
communications. Whenever we need to do message passing between
GE servers, we should use _Syn_ or _Asyn_ GE
protocols: they are much more efficient than their RESTful
counterparts for this purpose.
