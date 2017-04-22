---
id: manual-index
title: Getting Started
permalink: /docs/manual/index.html
next: /docs/manual/FirstApp.html
---

This manual covers everything a {{site.name}} developer needs to
know. We assume you know nothing about {{site.name}} before reading
this manual. With the flexible data and message passing modeling
capability, {{site.name}} makes the development of a real-time large
data serving system easy.

In this chapter, we will introduce what {{site.name}} is, followed by
our design philosophy. Then we help you setup a working environment
for playing with {{site.name}}.

This document is still in progress, and your comments are highly
appreciated. Feel free to send [us](/support.html) mails.

### What is {{site.name}}?

In what follows, assume we are developers who have big data sets
(probably with rich and complex schema) and want to serve the data to
our customers, allowing users to query the data in real time.

The data processing pipeline of a real-time data serving system is
usually composed of three layers: data ingestion layer, computation
layer, and query serving layer.

#### Data ingestion

We have data outside the system, we need to put the data to the system
before we can do anything useful with the system. This part is usually
harder than it appears to be.

Before we can ingest data to the system, we need to first describe the
schema of the data to the system so that the system can parse the
data. There are many ways to model a data set, from easy to hard. Let
us illustrate this using an example.

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

The data snippet shown above is in TSV (tab-separated values)
format. Most naively, we can model the data as a plain text and use
'strings' to represent and store the data. This is super easy. But
except for building full-text indexes and performing free text search,
there is little we can do to support queries like "tell me the
granddaughter's name of Leonhard Euler".

We can associate more semantics to the data by make it more
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

{{site.name}} provides a declarative language called
[TSL](/docs/manual/TSL/index.html) to support such fine-grained data
modeling. As a good data modeling practice, fine-grained data modeling
is almost always strongly-typed: for every piece of data, if possible,
we assign a strongly-typed schema with it. For example, for the "Age",
we create an _Age_ data field and associate it with an integer
value. We can even specify the data field in a finer-grained way,
i.e., specifying the integer value as an _8-bit_ unsigned value as
illustrated in the following TSL script.


  ```C#
  struct Person
  {
      int8 Age;
  }
  ```

We can model all fields like 'Age' as `strings`. Why bother making
things complex. The reason is that we care performance as well as
storage costs. Making _Age_ an 8-bit integer not only makes it
occupies smaller space, but also makes the data processing easier and
faster.

After specifying the data schema using TSL, we can easily write a data
loader to import data to {{site.name}} as will be elaborated in the
[Data Import](/docs/manual/DataImport.html) chapter later.

#### Computation

Having the data in the system, we can now design and implement our
'business logic' now. For example, after we have imported a social
network to {{site.name}}, we may want to allow the system users to
search relations between any two social users.

Due to the great diversity of the application needs, it is almost
impossible to use a fixed set of built-in functions to serve every
data processing need. Instead of trying to provide an exhaustive set
of built-in computation modules, {{site.name}} tries to provide
generic building blocks to allow us to easily build such modules. The
most important building block provided by {{site.name}} for
distributed computation is declarative message passing. We can almost
implement any distributed algorithm using the fine-grained
event-driven message passing framework provided by {{site.name}}. We
will cover this part in the following chapters in detail.

#### Query serving

For most of the time, {{site.name}} functions as the backend of the
system. The computation layer is responsible for processing data
before serving it to users.  Now let us look at how to serve backend
services to the front-end applications.

{{site.name}} provides two major methods for serving a service: REST
APIs and {{site.name}} protocols.

* REST APIs: They are standard, cross-platform, and easy-to-use. If we
  specify a RESTful service protocol named `MyService`, {{site.name}}
  will automatically generate a REST service endpoint:
  `http://example.com/MyService`.

* {{site.name}} Protocols: They are the most efficient way to call a
  service implemented in the computation layer.

### {{site.name}} Working Environment

The following prerequisites are required to follow this manual and
develop {{site.name}} applications:

* Windows Server 2008 R2, Windows Server 2012, Windows 7/8/10, or above.

* Visual Studio 2015, 2013, or 2012.

* Windows PowerShell 3.0 (or above).

* {{site.name}} [Visual Studio Extension](https://visualstudiogallery.msdn.microsoft.com/12835dd2-2d0e-4b8e-9e7e-9f505bb909b8).


