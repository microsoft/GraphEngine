---
id: geconfig
title: Configuration Editor
permalink: /docs/manual/Utilities/geconfig.html
---

As introduced in the GE [basics](/docs/manual/basics.html),
the machines in a GE cluster may have different roles to
play. Each GE instance reads its configuration file, an xml
file named _trinity.xml_ under its working directory, to know its role
to play.

There are three ways of configuring GE.

* Using the _TrinityConfig_ class.

* Using the graphical configuration editor <a
  href="/downloads/tools/geconfig.zip">geconfig</a>.

* Directly modifying _trinity.xml_ under the GE application
  working directory.

In this chapter we mainly introduce the usage of the graphical
configuration editor _geconfig_. It is no more than a GUI modification
tool of _trinity.xml_. But since it provides verification
functionality, instead of directly modifying _trinity.xml_, use
_geconfig_ whenever possible.  

A _Running Mode Selector_ dialog pops up each time we run
_geconfig_. We can choose between _embedded_ or _distributed_.

<img src="/img/config/ModeSelector.png" style="padding-top: 20px; margin-left:10em;width:30em;"></img>

The configuration for embedded GE is
very simple. Only two parameters need to set as shown below.

<img src="/img/config/embedded.png" style="padding-top: 20px; margin-left:10em;width:50em;"></img>

_StorageRoot_ is the directory where GE stores data.
_LoggingLevel_ controls what kinds of messages are logged:
_Off_ (no message is going to be logged), _Verbose_ (all
messages will be logged), and a few levels in between.

The distributed mode configuration editor has two tab pages.

The first tab configures the servers in the cluster. You can add or
remove GE servers by clicking the _Add_ / _Remove_
button. We can use one or more _#_ symbols to add a range of
servers. Each _#_ represent a single digit. The range of the _#_
digits is specified by the _'#' Range_.  The _Working Directory_ is
the directory where the current GE application can be
located.

<img src="/img/config/addserver.png" style="padding-top: 20px; margin-left:10em;width:50em;"></img>

We can modify the information of an existing server entry by making it
selected in the server list.

<img src="/img/config/modifyserver.png" style="padding-top: 20px; margin-left:10em;width:50em;"></img>

The proxy tab page is similar to the server page except it is for
configuring GE proxies.

Besides the graphical configuration editor, GE has a public
static class named _TrinityConfig_ for runtime configuration.  You can
change the configuration parameters loaded from _trinity.xml_ via the
interfaces provided by this class. For example, you can switch the
running mode of your program to _Embedded_ by the following line of
code.

```C#
TrinityConfig.CurrentRunningMode = RunningMode.Embedded;
```

Note the runtime changes are not written back to _trinity.xml_.  If
you would like to save your changes, you need to explicitly call
_TrinityConfig.SaveConfig()_.
