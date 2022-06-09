---
id: tsl-index
title: Trinity Specification Language
permalink: /docs/manual/TSL/index.html
prev: /docs/manual/FirstApp.html
next: /docs/manual/TSL/tsl-basics.html
---

The Trinity Specification Language (TSL) is a declarative
language.  Instead of using a fixed data schema and a fixed computation paradigm,
GE allows the users to freely define their own data schemata and extend
the system with user-defined server-side computations.

<object type="image/svg+xml" style="width:24em; display:block;
margin-left:auto;margin-right:auto;" data="/img/svg/TSL.svg">The
browser does not support SVG.</object>

TSL combines the following three functionalities into one language:
data modeling, message passing modeling, and data interchange format
specification. From the perspective of data modeling, TSL functions
like IDL in CORBA; from the perspective of message passing
modeling, TSL resembles the Slice specification language of
ICE (Internet Communications Engine); from the perspective of the data
interchange format specification, TSL works like protocol buffers or
MessagePack, allowing the users to exchange data without serialization and
deserialization via efficient encoding of the structured data.

GE compiles a TSL script into a .Net assembly. The
generated assembly is a GE extension module which contains
a set of extension methods to the GE system.
