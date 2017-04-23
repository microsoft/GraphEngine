---
id: tsl-index
title: Trinity Specification Language
permalink: /docs/manual/TSL/index.html
prev: /docs/manual/FirstApp.html
next: /docs/manual/TSL/tsl-basics.html
---

The Trinity Specification Language (TSL) is a declarative
language that is designed to provide system flexibility.  Instead of
using a fixed data schema and fixed computation paradigms,
GE allows users to freely define data schemata and extend
the system capability by performing user-defined server-side
computation.

<object type="image/svg+xml" style="width:24em; display:block;
margin-left:auto;margin-right:auto;" data="/img/svg/TSL.svg">The
browser does not support SVG.</object>

TSL glues the following three things together: data modeling, message
passing protocol modeling, and data interchange format
specification. From the perspective of data modeling, TSL functions
like IDL in CORBA; From the perspective of message passing
protocol modeling, TSL resembles the Slice specification language of
ICE (Internet Communications Engine); From the perspective of the data
interchange format specification, TSL works like protocol buffers or
MessagePack, allowing users to exchange data without serialization and
deserialization via an efficient way of encoding structured data.

GE compiles a TSL script into a .Net assembly. The
generated assembly is a GE extension module which contains
a set of extension methods to the GE system.
