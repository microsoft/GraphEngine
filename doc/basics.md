---
id: basics
title: Graph Engine Basics
permalink: /docs/manual/basics.html
prev: /docs/manual/index.html
next: /docs/manual/TSL/index.html
---

GE is both a RAM store and a computation engine. As a RAM
store, GE organizes the main memory<sup><a
href="#memoryfn">*</a></sup> of a cluster of machines as a globally
addressable address space (a memory cloud) to store large scale data
sets; as a computation engine, GE provides user-defined
APIs to implement graph processing logic.

<object type="image/svg+xml" style="width:25em; display:block;
margin-left:auto;margin-right:auto;"
data="/img/svg/SystemStack.svg">The browser does not support
SVG.</object>

This figure shows the stack of GE system layers.  The
memory cloud is a distributed key-value store, which is supported by a
memory storage module and a message passing framework. The memory
storage module manages the main memory of a cluster of machines and
provides the mechanisms for concurrency control. The network communication
module is an efficient, one-sided, machine-to-machine message
passing infrastructure.

GE provides a specification language called
[TSL](/docs/manual/TSL/index.html) (Trinity specification
language) that bridges the graph model and the memory cloud.
It is hard, if not entirely impossible, to support efficient,
general-purpose graph computations using a fixed
graph schema due to the diversity of graph data and application
needs. Instead of using a fixed graph schema and fixed computation
models, GE allows the users to use TSL to specify graph
schemata, communication protocols, and so on.

GE has two running modes, _embedded mode_ and _distributed
mode_. In the embedded mode, GE serves as an in-process
library. In the distributed mode, GE can be deployed to one
or more machines.

When deployed in the distributed mode, GE consists of a
number of system components that communicate through the network. A
GE component may take one or more of the following roles: I)
storing data; II) handling messages and performing computations; III)
interacting with clients. According to the roles played by the
components, we classify GE components into three
categories: _Server_, _Proxy_, and _Client_.

<object type="image/svg+xml" style="width:25em; display:block;
margin-left:auto;margin-right:auto;"
data="/img/svg/ClusterArchitecture.svg">The browser does not support
SVG.</object>

* _Server_. A server plays two roles: storing data
  (owning a data partition if the data is partitioned and stored on multiple machines)
  and performing computations on the data. Computations usually involve
  sending messages to and receiving messages from other GE
  components.

* _Proxy_. A proxy handles messages but does not own a data
  partition. It usually serves as a middle tier between the servers and
  the clients. For example, a proxy can serve as a query aggregator: it
  dispatches the requests received from a client to the servers,
  aggregates the results returned by individual servers and sends the aggregated
  results back to the client. GE proxies are optional for distributed GE application.
  They can be added to the system as needed.

* _Client_. A client is a GE program that communicates with
  a GE cluster. It is an interface layer between
  the end user and the GE backend.

The GE components can form two typical system architectures:

* An architecture that consists of a number of GE servers
and a number of GE clients. The clients send queries directly
to the GE servers, and get the query results from the servers.

* An architecture that consists of a number of GE servers,
proxies, and clients. The clients communicate with the GE
cluster via the GE proxies. As the middle tier between the clients
and the servers, the proxies may generate optimized query execution plans,
decompose the queries into sub-queries and aggregate the partial results
obtained from individual servers before returning the final results to the clients.

<hr></hr> <sup id="memoryfn">* We use 'memory' to refer to dynamic
random-access memory (RAM) throughout this manual.
