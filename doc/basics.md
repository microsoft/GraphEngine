---
id: basics
title: Graph Engine Basics
permalink: /docs/manual/basics.html
prev: /docs/manual/index.html
next: /docs/manual/TSL/index.html
---

{{site.name}} is both a RAM store and a computation engine. As a RAM
store, {{site.name}} organizes the main memory<sup><a
href="#memoryfn">*</a></sup> of a cluster of machines as a globally
addressable address space (a memory cloud) to store large scale data
sets; as a computation engine, {{site.name}} provides user-customized
APIs to implement graph processing logic.

<object type="image/svg+xml" style="width:25em; display:block;
margin-left:auto;margin-right:auto;"
data="/img/svg/SystemStack.svg">The browser does not support
SVG.</object>

This figure shows the stack of {{site.name}} system layers.  The
memory cloud is a distributed key-value store, which is supported by a
memory storage module and a message passing framework. The memory
storage module manages the main memory of a cluster of machines and
provides mechanisms for concurrency control. The network communication
module provides an efficient, one-sided, machine-to-machine message
passing infrastructure.

{{site.name}} provides a specification language called
[TSL](/docs/manual/TSL/index.html) ({{site.codename}} specification
language) that bridges the graph model and the underlying storage and
computation infrastructure. It is hard, if not entirely impossible, to
support efficient, general-purpose graph computations using a fixed
graph schema due to the diversity of graph data and application
needs. Instead of using a fixed graph schema and fixed computation
models, {{site.name}} allows users to use TSL to specify graph
schemata, communication protocols, and computational paradigms.

{{site.name}} has two running modes, _embedded mode_ and _distributed
mode_. In the embedded mode, {{site.name}} serves as an in-process
library. In the distributed mode, {{site.name}} can be deployed on one
or more machines.

When deployed in the distributed mode, {{site.name}} consists of a
number of system components that communicate through a network. A
{{site.name}} component may have one or more following roles: I)
storing data; II) handling messages and performing computations; III)
interacting with clients. According to the roles played by the
components, we classify {{site.name}} components into three
categories: _Server_, _Proxy_, and _Client_.

<object type="image/svg+xml" style="width:25em; display:block;
margin-left:auto;margin-right:auto;"
data="/img/svg/ClusterArchitecture.svg">The browser does not support
SVG.</object>

* _Server_. A server plays two roles: storing data and
  performing computations on the data. Computations usually involve
  sending messages to and receiving messages from other {{site.name}}
  components.

* _Proxy_. A proxy handles messages but does not own a data
  partition. It usually serves as a middle tier between servers and
  clients. For example, a proxy can serve as a query aggregator: it
  dispatches requests received from a client to servers, aggregates the
  results returned by individual servers and sends the aggregated
  results back to the client. {{site.name}} proxies are optional. They
  can be added to the system as needed.

* _Client_. A client is a {{site.name}} program that communicates with
  a {{site.name}} cluster. It is a user-interface layer between
  {{site.name}} and the end user.

The {{site.name}} components can form two typical system
architectures:

* An architecture that consists of a number of {{site.name}} servers
and a number of {{site.name}} clients. Clients send queries directly
to {{site.name}} servers, and get results from the servers.

* An architecture that consists of a number of {{site.name}} servers,
proxies, and clients. Clients communicate with the {{site.name}}
cluster via {{site.name}} proxies. As the middle tier between clients
and servers, proxies may generate query execution plans, decompose
queries into sub-queries and aggregate partial results from individual
servers before returning the final results to the clients.

<hr></hr> <sup id="memoryfn">* We use 'memory' to refer to dynamic
random-access memory (RAM) throughout this manual.