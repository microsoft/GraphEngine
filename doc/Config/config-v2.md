---
id: configv2
title: Configuration File Specification (Version 2.0)
permalink: /docs/manual/Config/config-v2.html
---

A Graph Engine Configuration File is an XML file with the default name
`trinity.xml`. When a Graph Engine instance is initialized, it will
automatically load the default configuration file, unless
`TrinityConfig.LoadConfig(...)` is explicitly called. The file can
contain both the configuration settings for a single machine or a
cluster of machines.


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
<td class="tableblock halign-left valign-top"><p class="tableblock">Optional. Specifies the version of the
configuration file. Currently, the value must be 1.0 or 2.0 if it is
specified. The value "2.0" indicates that the configuration file
conforms to the specification given here. 
Note, if ConfigVersion is not specified, the configuration file will
be parsed as version 1.0, which is now obsolete.</p></td>
</tr>
</tbody>
</table>

All valid nodes under Trinity node are called **top-level elements**.

### Top-Level Elements

All top-level elements must be children of the root node.

#### Local Node

A `Local` node configures the settings for the machine that reads the
current configuration file. There can be multiple `Local`
nodes. Multiple `Local` nodes will be _merged_.  The configuration
settings in a local node have the highest priority. If specified, it
will override configurations specified in a <a
href="#cluster-node">Cluster node</a>. A `Local` node can have an optional attribute `Template`.

<table class="tableblock frame-all grid-all spread">
<colgroup>
<col style="width: 10%;">
<col style="width: 86%;">
</colgroup>
<tbody>
<tr>
<td class="tableblock halign-left valign-top"><p class="tableblock">Template</p></td>
<td class="tableblock halign-left valign-top"><p class="tableblock">Optional. The value is the ID of a template. If it is specified, the current node will inherit the attributes and child elements of the template.</p></td>
</tr>
</tbody>
</table>

A `Local` node can have one or more <a
href='configuration-nodes'>configuration nodes</a> as its child
elements.

#### Cluster Node

A `Cluster` node contains information about servers and proxies of a
Graph Engine cluster. There can be multiple cluster nodes as long as
they have different identifiers. A `Cluster` node can have an optional attribute `Id`.

<table class="tableblock frame-all grid-all spread">
<colgroup>
<col style="width: 10%;">
<col style="width: 86%;">
</colgroup>
<tbody>
<tr>
<td class="tableblock halign-left valign-top"><p class="tableblock">Id</p></td>
<td class="tableblock halign-left valign-top"><p class="tableblock">Optional. 
The cluster configuration identified by an Id can be retrieved later in the program. If omitted, the current Cluster configuration is then treated as the default one.
</p></td>
</tr>
</tbody>
</table>

A `Cluster` node can have one or more `Server` or `Proxy` nodes as its
child elements.

##### Server

A `Server` node specifies a configuration that can later be used to
create TrinityServer communication instances. The properties of a
`Server` node can either be specified via node attributes or child
elements. A `Server` node can have one or more <a
href='configuration-nodes'>configuration nodes</a> as its child
elements. A `Server` node can have the following attributes.

<table class="tableblock frame-all grid-all spread">
<colgroup>
<col style="width: 10%;">
<col style="width: 86%;">
</colgroup>
<tbody>

<tr>

<td class="tableblock halign-left valign-top"><p
class="tableblock">Endpoint</p></td>

<td class="tableblock halign-left valign-top"><p
class="tableblock">Mandatory. Specifies an Hostname:Port string that
describes the server endpoint.</p></td>

</tr>

<tr>
<td class="tableblock halign-left valign-top"><p class="tableblock">AvailabilityGroup</p></td>

<td class="tableblock halign-left valign-top"><p
class="tableblock">Optional. Specifies the identifier of the
availability group that the current server belongs to.  Each server is
associated with an availability group.  If not specified, each server
will be in its own default availability group.  Multiple <i>Server</i>
nodes could have the same availability group.  The servers within the
same availability group correspond to the same server Id in
MemoryCloud.SendMessage methods. The <i>ServerId</i> parameter in the
SendMessage method is the index number of the availability
group. Note, when this attribute is used, all <i>Server</i> nodes
within a <i>Cluster</i> node specify the attribute or none of them
should specify.  </p></td>

</tr>

<tr>
<td class="tableblock halign-left valign-top"><p class="tableblock">Template</p></td>

<td class="tableblock halign-left valign-top"><p
class="tableblock">Optional. The value is the ID of a <a
href='#template'>template</a>. If it is specified, the current node
will inherit the attributes and child elements of the
template.</p></td>

</tr>

<tr>
<td class="tableblock halign-left valign-top"><p class="tableblock">AssemblyPath</p></td>
<td class="tableblock halign-left valign-top"><p class="tableblock">Optional. Specifies the directory of the running executable. Useful for running multiple Graph Engine instances on the same machine (listening on different ports, or bound to different network interfaces).</p></td>
</tr>

</tbody>
</table>

The server nodes within a `Cluster` node are interpreted as an ordered
list. An availability group, if not specified each `Server` node has
its own default available group, is assigned an index numbers upon
their first occurence. For example, if there are three `Server` nodes:
               
```xml
<Server AvailabilityGroup=”AG1” .../>
<Server AvailabilityGroup=”AG2” .../>
<Server AvailabilityGroup=”AG1” .../>
```

The three servers will be assigned index numbers 0, 1, 0,
respectively.

The reason why multiple `Server` configurations can be specified
within a `Cluster` node is for easily deploying Graph Engine to a
cluster of machines using a single configuration file. For a Graph
Engine cluster consisting of multiple machines, when a Graph Engine
instance is started, it loads the server configuration from one of the
`Server` nodes according to the following rules:

* The `Endpoint` property matches one of the network interfaces of the machine on which the Graph Engine instance is running.
* If `AssemblyPath` is specified, it matches the directory where the running Graph Engine instance resides.

##### Proxy

A `Proxy` node specifies a TrinityProxy communication instance. The
structure of a `Proxy` node is the same as that of a `Server` node.

#### Template

A **Template** node can be referenced by other nodes as a
configuration template. The content within a template node will be
inherited by the nodes that reference it.  Multiple templates with the
same identifier will be [merged](#merging-configuration-nodes). A
`Template` node must specify a mandatory attribute `Id`.

<table class="tableblock frame-all grid-all spread">
<colgroup>
<col style="width: 10%;">
<col style="width: 86%;">
</colgroup>
<tbody>
<tr>
<td class="tableblock halign-left valign-top"><p class="tableblock">Id</p></td>
<td class="tableblock halign-left valign-top"><p class="tableblock">Mandatory. Specifies the identifier of the template, which can be referenced by configuration node.</p></td>
</tr>
</tbody>
</table>

A `Template` node can have one or more <a
href='configuration-nodes'>configuration nodes</a> as its child
elements.

#### Import

An `Import` node specifies other configuration files to import. Each
`Import` node will be replaced with the top-level elements read from
the imported configuration files. Note that the imported configuration
files must have the **same** **ConfigVersion** with the current
configuration file. An imported configuration file can recursively
import other configuration files. A configuration file cannot import
itself.

<table class="tableblock frame-all grid-all spread">
<colgroup>
<col style="width: 10%;">
<col style="width: 86%;">
</colgroup>
<tbody>
<tr>
<td class="tableblock halign-left valign-top"><p class="tableblock">File</p></td>

<td class="tableblock halign-left valign-top"><p
class="tableblock">Mandatory.  Specifies a file to import. The file
must have .xml file extension, and has a &lt;Trinity&gt; root node,
with the same ConfigVersion.
</p></td>

</tr>
</tbody>
</table>

An `Import` node cannot have any child elements.

### Configuration Nodes

A configuration node contains a number of configuration entries. A
configuration entry specifies the configuration settings concerning a
certain aspect of the system functionality, such as `Storage` and
`Logging`. An external Graph Engine module could also register a
configuration entry so that its settings can be specified in the Graph
Engine configuration file. An external module should follow the
following rules for registering configuration entries:

* One module should register only one configuration entry. The name of
the entry should be distinctive, to avoid ambiguity. The name of the
entry should be highly related to the module. If there are two modules
that registered the same configuration entry name, the entries will be
renamed to _AssemblyName:EntryName_.

* When possible, use attributes to specify settings. Use children
nodes only if it is necessary, for example, to specify a list.

##### Configuration Override

Overrides take place at configuration entry level. The content in an
entry will not be merged. An entry in a lower priority configuration
section will be overridden by the one in a higher priority
configuration section. When a template section is referenced, it
inherits the priority of the referencing node.

##### Merging Configuration Nodes

Referencing a configuration template, or specifying multiple `Local`
configuration nodes, will result in the configuration sections being
merged. When merging occurs, configuration entries will be laid out in
the order they appear in the configuration file. The entries that
appear later will override the earlier entries with the same name.

#### Built-In Configuration Nodes

##### Storage

A `Storage` node can have the following attributes.

<table class="tableblock frame-all grid-all spread">
<colgroup>
<col style="width: 10%;">
<col style="width: 86%;">
</colgroup>
<tbody>
<tr>
<td class="tableblock halign-left valign-top"><p class="tableblock">
TrunkCount
</p></td>
<td class="tableblock halign-left valign-top"><p class="tableblock">
Optional. Specifies the number of memory trunks in the local memory storage. Must be a power of 2 within range [1, 256]. The default value is 256.
</p></td>
</tr>

<tr>
<td class="tableblock halign-left valign-top"><p class="tableblock">
ReadOnly
</p></td>
<td class="tableblock halign-left valign-top"><p class="tableblock">
Optional. A Boolean value that specifies whether the local memory storage is read-only. In read-only mode, the local memory storage is lock-free, but write operations will result in exceptions and system crashes. The default value is FALSE.
</p></td>
</tr>

<tr>
<td class="tableblock halign-left valign-top"><p class="tableblock">
StorageCapacity
</p></td>
<td class="tableblock halign-left valign-top"><p class="tableblock">
Optional. Specifies the capacity profile of the local memory storage. A capacity profile specifies the maximum number of entries that the local memory storage can hold. The value is a string, and must have one of the following values: <i>Max256M</i>, <i>Max512M</i>, <i>Max1G</i>, <i>Max2G</i>, <i>Max4G</i>, <i>Max8G</i>, <i>Max16G</i>, <i>Max32G</i>. The default is <i>Max8G</i>.

</p></td>
</tr>

<tr>
<td class="tableblock halign-left valign-top"><p class="tableblock">
StorageRoot
</p></td>
<td class="tableblock halign-left valign-top"><p class="tableblock">
Optional. Specifies the path of a directory for saving and loading Graph Engine disk images. The default path is the <i>storage</i> directory in <i>AssemblyPath</i>.
</p></td>
</tr>

<tr> <td class="tableblock halign-left valign-top"><p
class="tableblock"> DefragInterval </p></td> <td class="tableblock
halign-left valign-top"><p class="tableblock">Optional. An integer value that
specifies a time interval in milliseconds.  The local memory storage
garbage collector issues defragmentation operations periodically. The
value of <i>DefragInterval</i> specifies the time between two
defragmentation operations.  The default value is 600.  </p></td>
</tr>

</tbody>
</table>

A `Storage` node cannot have any child elements.

##### Logging

A `Logging` node can have the following attributes.

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
Optional. Specifies the path of a directory to store log files. The default path is the <i>trinity-log</i> in <i>AssemblyPath</i>.
</p></td>
</tr>

<tr>
<td class="tableblock halign-left valign-top"><p class="tableblock">
LogLevel
</p></td>
<td class="tableblock halign-left valign-top"><p class="tableblock">
Optional. Specifies the logging level. Must be one of the following values: <i>Verbose</i>, <i>Debug</i>, <i>Info</i>, <i>Warning</i>, <i>Error</i>, <i>Fatal</i>, <i>Off</i>.
The default level is <i>Info</i>.
</p></td>
</tr>

<tr>
<td class="tableblock halign-left valign-top"><p class="tableblock">
LogToFile
</p></td>
<td class="tableblock halign-left valign-top"><p class="tableblock">
Optional. A Boolean value that specifies whether to stream log entries to a file on disk. The default value is TRUE.
</p></td>
</tr>

<tr>
<td class="tableblock halign-left valign-top"><p class="tableblock">
EchoOnConsole
</p></td>
<td class="tableblock halign-left valign-top"><p class="tableblock">
Optional. A Boolean value that specifies whether to echo log entries on the standard output. The default value is TRUE.
</p></td>
</tr>

</tbody>
</table>

A `Logging` node cannot have any child elements.

##### Network

A `Network` node can have the following attributes.

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
Optional. If a server/proxy has Http endpoints, it will listen on the
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

<tr>
<td class="tableblock halign-left valign-top"><p class="tableblock">
ClientSendRetry
</p></td>
<td class="tableblock halign-left valign-top"><p class="tableblock">
Optional. Specifies how many retries will be attempted when a message cannot be sent. The default value is 1.
</p></td>
</tr>

<tr>
<td class="tableblock halign-left valign-top"><p class="tableblock">
ClientReconnectRetry
</p></td>
<td class="tableblock halign-left valign-top"><p class="tableblock">
Optional. Specifies how many reconnect attempts will be made when a message is being sent and the connection is down. The default value is 1.
</p></td>
</tr>

<tr> <td class="tableblock halign-left valign-top"><p
class="tableblock"> Handshake </p></td> <td class="tableblock
halign-left valign-top"><p class="tableblock"> Optional. A Boolean
value that specifies whether a Trinity server/proxy/client performs
handshaking when establishing a connection. Set this to FALSE for
compatibility with older versions of Graph Engine. The default value
is TRUE.  </p></td> </tr>

</tbody>
</table>

A `Network` node cannot have any child elements.


