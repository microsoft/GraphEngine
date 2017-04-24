---
id: configv1
title: Configuration File Specification (Version 1.0)
permalink: /docs/manual/Config/config-v1.html
---

A Graph Engine Configuration File is an XML file with the default name
`trinity.xml`. When a Graph Engine instance is initialized, it will
automatically load the default configuration file, unless
`TrinityConfig.LoadConfig(...)` is explicitly called. The file can
contain configuration settings for a single machine and/or a cluster
of machines.

### Root Node

The root node is always `Trinity`. There must be exactly one root
node. The root node has an optional attribute `ConfigVersion`.

<table class="tableblock frame-all grid-all spread">
<colgroup>
<col style="width: 10%;">
<col style="width: 86%;">
</colgroup>
<tbody>
<tr>
<td class="tableblock halign-left valign-top"><p class="tableblock">ConfigVersion</p></td>

<td class="tableblock halign-left valign-top"><p
class="tableblock">Optional. Specifies the version of the
configuration file. Currently, the value must be 1.0 or 2.0 if
specified. The value "1.0" indicates that the configuration file
conforms to the specification 1.0 given here.  Note, if ConfigVersion
is not specified, the configuration file will be parsed as version 1.0.</p></td> </tr>

</tbody>
</table>

Note, nuget package `GraphEngine.Core` 1.0.8482 and below only
support configuration specification 1.0.

All valid nodes under Trinity node are `section` nodes.

### Sections

A top-level node under the root node must be a `section` node. A
section is identified by its _name_ attribute, for example, `<section
name="Storage">...</section>`.  Each section contains a number of
_entries_. An entry is identified by its _name_ attribute, for
example, `<entry name="StorageRoot">...</entry>`.

#### Storage Section

The `Storage` section can have one optional entry `StorageRoot`.

<table class="tableblock frame-all grid-all spread">
<colgroup>
<col style="width: 10%;">
<col style="width: 86%;">
</colgroup>
<tbody>
<tr>
<td class="tableblock halign-left valign-top"><p class="tableblock">
StorageRoot
</p></td>
<td class="tableblock halign-left valign-top"><p class="tableblock">
Optional. Specifies the path of a directory for saving and loading Graph Engine disk images. The default path is the <i>storage</i> directory in <i>AssemblyPath</i>.
</p></td>
</tr>
</tbody>
</table>

#### Servers Section

The `Servers` section specifies the endpoints and configurations of
the servers in a cluster.  It can have a number of `Server`
entries. The value of a `Server` entry is an <i>Hostname:Port</i>
string that describes the server endpoint.

Each `Server` entry can have the following attributes.

<table class="tableblock frame-all grid-all spread">
<colgroup>
<col style="width: 10%;">
<col style="width: 86%;">
</colgroup>
<tbody>

<tr>
<td class="tableblock halign-left valign-top"><p class="tableblock">AvailabilityGroup</p></td>

<td class="tableblock halign-left valign-top"><p
class="tableblock">Optional. Each server is associated with an
availability group. This attribute specifies the identifier of the
availability group that the current server belongs to.  If not
specified, each server will be in its own default availability group.
Multiple <i>Server</i> nodes could have the same availability group.
The servers within the same availability group correspond to the same
server Id in <i>MemoryCloud.SendMessage(...)</i>. The <i>ServerId</i>
parameter in the SendMessage method is the index number of the
availability group. Note, when this attribute is used, all
<i>Server</i> nodes within a <i>Cluster</i> node must specify the
attribute or none of them should specify.  </p></td>

</tr>

<tr>
<td class="tableblock halign-left valign-top"><p class="tableblock">AssemblyPath</p></td>
<td class="tableblock halign-left valign-top"><p class="tableblock">Optional. Specifies the directory of the running Graph Engine instance. Useful for running multiple Graph Engine instances on the same machine (listening on different ports, or bound to different network interfaces).</p></td>
</tr>

<tr>
<td class="tableblock halign-left valign-top"><p class="tableblock">
StorageRoot
</p></td>
<td class="tableblock halign-left valign-top"><p class="tableblock">
Optional. Specifies the path of a directory for saving and loading Graph Engine disk images. The default path is the <i>storage</i> directory in <i>AssemblyPath</i>.
</p></td>
</tr>

<tr>
<td class="tableblock halign-left valign-top"><p class="tableblock">
LoggingLevel
</p></td>
<td class="tableblock halign-left valign-top"><p class="tableblock">
Optional. Specifies the logging level. Its value must be one of the following: <i>Verbose</i>, <i>Debug</i>, <i>Info</i>, <i>Warning</i>, <i>Error</i>, <i>Fatal</i>, <i>Off</i>.
The default level is <i>Info</i>.
</p></td>
</tr>

</tbody>
</table>
                
Multiple `Server` entries can be specified is for easily deploying
Graph Engine to a cluster of machines using a single configuration
file. For a Graph Engine cluster consisting of multiple machines, when
a Graph Engine instance is started, it loads its server configuration
from one of the `Server` entries according to the following rules:

* The `Endpoint` property matches one of the network interfaces of the machine on which the Graph Engine instance is running.
* If `AssemblyPath` is specified, it matches the directory where the running Graph Engine instance resides.                   

#### Proxies Section

The `Proxies` section specifies the endpoints and configurations of the proxies in a cluster. 
The structure of a `Proxy` section is the same as that of a `Server` section.

#### Network Section

The `Network` section can have the following entries.

<table class="tableblock frame-all grid-all spread">
<colgroup>
<col style="width: 10%;">
<col style="width: 86%;">
</colgroup>
<tbody>

<tr>
<td class="tableblock halign-left valign-top"><p class="tableblock">
HttpPort
</p></td>

<td class="tableblock halign-left valign-top"><p class="tableblock">
Optional. If a server/proxy has an Http endpoint, it will listen on the
specified Http port after it is started. The value is an integer.
When a negative integer is specified, the HTTP server is disabled and
no port will be opened. The default port is 80. </p></td>
</tr>

<tr> <td class="tableblock halign-left valign-top"><p
class="tableblock"> ClientMaxConn </p></td> <td class="tableblock
halign-left valign-top"><p class="tableblock"> Optional. Specifies how
many network connections will be established between a client and a
Graph Engine communication instance. The default value is 2.
</p></td> </tr>

</tbody>
</table>

#### Logging Section

The `Logging` section can have the following entries.

<table class="tableblock frame-all grid-all spread">
<colgroup>
<col style="width: 10%;">
<col style="width: 86%;">
</colgroup>
<tbody>

<tr>
<td class="tableblock halign-left valign-top"><p class="tableblock">
LogDirectory
</p></td>
<td class="tableblock halign-left valign-top"><p class="tableblock">
Optional. Specifies the path of a directory to store log files. The default path is the <i>trinity-log</i> directory in <i>AssemblyPath</i>.
</p></td>
</tr>

<tr>
<td class="tableblock halign-left valign-top"><p class="tableblock">
LogLevel
</p></td>
<td class="tableblock halign-left valign-top"><p class="tableblock">
Optional. Specifies the logging level. Its value must be one of the following: <i>Verbose</i>, <i>Debug</i>, <i>Info</i>, <i>Warning</i>, <i>Error</i>, <i>Fatal</i>, <i>Off</i>.
The default level is <i>Info</i>.
</p></td>
</tr>

</tbody>
</table>

### Sample Configuration File

```xml
<?xml version="1.0" encoding="utf-8"?>
<Trinity>
  <section name="Storage">
    <entry name="StorageRoot">D:\storage</entry>
  </section>
  <section name="Servers">
    <entry name="Server">192.168.0.1:5304</entry>
    <entry name="Server">192.168.0.2:5304</entry>
  </section>
  <section name="Proxies" />
  <section name="Network">
    <entry name="ClientMaxConn">2</entry>
  </section>
  <section name="Logging">
    <entry name="LoggingLevel">Info</entry>
  </section>
</Trinity>
```
