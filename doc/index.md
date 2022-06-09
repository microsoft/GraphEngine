---
id: manual-index
title: Getting Started
permalink: /docs/manual/index.html
next: /docs/manual/FirstApp.html
---

This manual covers everything a GE developer needs to
know. We assume you know nothing about GE before reading
this manual. With the flexible data and message passing modeling
capability, GE makes the development of a real-time large
data serving system easy.

In this chapter, we introduce what GE is, followed by
our design philosophy. Then we help you set up a development environment
for playing with GE.

This document is still in progress. Your comments are highly
appreciated.

### What is GE?

In what follows, assume we are developers who have big data sets
(probably with rich and complex schema) and want to serve the data to
our customers, allowing the users to query the data in real time.

The data processing pipeline of a real-time data serving system is
usually composed of three layers: data ingestion layer, computation
layer, and query serving layer.

#### Data ingestion

We have data outside the system and we need to load the data into the system
before we can do anything useful with the system. This part is usually
harder than it appears to be.

Before we can feed data into the system, we need to first describe the
schema of the data to the system so that the system can correctly parse the
data. Let us illustrate this using an example.

  ```
  Leonhard Euler    Born           April 15, 1707
  Leonhard Euler    Age            76
  Leonhard Euler    Education      University of Basel
  Leonhard Euler    Book           Elements of Algebra
  Leonhard Euler    Son            Johann Euler
  Johann Euler      Born           Nov 27, 1734
  Johann Euler      Daughter       Charlotte Euler
  ...

  ```

The data snippet shown above is in the TSV (tab-separated values)
format. Naively, we can model the data as a plain text and use
'strings' to represent and store the data. This is super easy. But
except for building full-text indexes and performing free text search,
there is little we can do to support queries like "tell me the
granddaughter's name of Leonhard Euler".

We can associate more semantics to the data by making it more
structured. For example, we can define a _Person_ struct to hold the
data:

  ```C#
   struct Person
   {
      string Name;
      string Age;
      string Son;
      string Daughter;
      ...
   }
  ```

With the structured data representation, we can write a program based
on the semantics associated with the data fields to reason the
granddaughter of a _Person_.

GE provides a declarative language called
[TSL](/docs/manual/TSL/index.html) to support such fine-grained data
modeling. As a good data modeling practice, fine-grained data modeling
should almost always be strongly-typed: for every piece of data, if possible,
we assign a strongly-typed schema with it. For example, for the "Age",
we create an _Age_ data field and associate it with an integer
value. We can even specify the data field in a specific way,
i.e., specifying the integer value as an _8-bit_ unsigned value as
illustrated in the following TSL script.


  ```C#
  struct Person
  {
      int8 Age;
  }
  ```

We can model all the fields like 'Age' as `strings`. Why bother making
things complex. The reason is that we care about query performance as well as
storage costs. Making _Age_ an 8-bit integer not only makes it
occupy smaller space, but also makes the data processing easier and
faster.

After specifying the data schema in TSL, we can easily write a data
loader to import data into GE as will be elaborated in the
[Data Import](/docs/manual/DataImport.html) chapter.

#### Computation

Having the data in the system, we can now design and implement our
'business logic' now. For example, after we have imported a social
network to GE, we may want to allow the system users to
search the relations between any two social users.

Due to the great diversity of the application needs, it is almost
impossible to use a fixed set of built-in functions to serve every
data processing need. Instead of trying to provide an exhaustive set
of built-in computation modules, GE tries to provide
generic building blocks to allow the developers to easily build such modules. The
most important building block provided by GE for
distributed computation is declarative message passing. We can
implement almost any distributed algorithm using the fine-grained
event-driven message passing framework provided by GE. We
will cover this part in the following chapters in detail.

#### Query serving

For most of the time, GE functions as the backend of a
system. The computation layer is responsible for processing the data
before serving it to the users.  Now let us look at how to serve backend
services to the front-end applications.

GE provides two methods for serving a service: REST APIs and GE protocols.

* REST APIs: They are standard, cross-platform, and easy-to-use. If we
  specify a RESTful service protocol named `MyService`, GE
  will automatically generate a REST service endpoint:
  `http://example.com/MyService`.

* GE Protocols: They are the most efficient way to call a
  service implemented in the computation layer.

### Development Environment

To follow this manual and develop GE applications, you need to set up a development
environment by following the [Building on Windows](https://github.com/microsoft/GraphEngine#building-on-windows)
or [Building on Linux](https://github.com/microsoft/GraphEngine#building-on-linux) instructions.
