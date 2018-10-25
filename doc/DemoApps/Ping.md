---
id: Ping
title: Ping!
permalink: /docs/manual/DemoApps/Ping.html
---

##### <a href="https://github.com/Microsoft/GraphEngine/tree/master/samples/Ping" target="_blank">Source Code on GitHub</a>

In this section, we use a simple _ping_ example to demonstrate how to
realize server-side computation via message passing. The _Ping_
program can be considered as the simplified version of many much more
complicated distributed programs.  Without considering the message
handling logic, most full-fledged distributed program share the same
communication patterns with the simple ping program.

_Ping_ is probably the most well-known network utility, which is
usually used to test the network connection. To test whether machine
_A_ can reach machine _B_, we send machine _B_ a short message from
machine _A_. For the purpose of demonstration, we are going to use 3
different types of messages to make the _ping_ test.

Let us start with the simplest one: unidirectional ping. It looks like
the ordinary _ping_, but is simplified. When machine _B_ received
the _ping_ message from _A_, it only sends back a network
acknowledgement to _A_ without any meaningful response. After
completing the _ping_ test, machine _A_ knows the network connection
to _B_ is OK, but it does not know whether machine _B_ functions well.

Let us see how to implement this simple ping program. We need to specify three things: 

* What message is going to be sent?

* What is the protocol via which the message is sent?

* Which component of the system is responsible for handling the message?

These three things can be specified using the following Trinity specification script:

```C#
struct MyMessage
{
	int sn;
}

protocol SynPing
{
	Type: Syn;
	Request: MyMessage;
	Response: void;
}

server MyServer
{
	protocol SynPing;
}
```

The struct definition _MyMessage_ specifies the struct of our message,
which consists of a 32-bit sequential number _sn_.

Next, we specify a protocol named _SynPing_. The protocol definition
consists of three parts:

* Type of the protocol, which can be either _Syn_, _Asyn_, or _HTTP_.

* Schema/Structure of the request message.

* Schema/Structure of the response message.

In this example, we create _SynPing_ as a synchronous protocol. The
schema/structure of the message it uses is the previously defined
_MyMessage_ and it requires no response.

The last thing we need to do is to _register_ the _SynPing_ protocol
to a GE system component. As we have elaborated earlier,
three system components play different roles in a distributed
Trinity system: _server_, _proxy_, and _client_. Among them,
both _server_ and _proxy_ process messages, therefore we can register
message passing _protocols_ on them. In this example, we register
_SynPing_ to a server named _MyServer_.

We then create a _Graph Engine Project_ _Ping_ in visual
studio. We cut and paste the above script to the generated
_MyCell.tsl_ file.  Now we can create a _Graph Engine Application
Project_ called _PingTest_ in visual studio.

Open the generated _Program.cs_, and put the following content in:

```C#
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trinity.Data;
using Trinity.Storage;
using Trinity;
using System.Threading;

namespace TestTrinity
{
    class MyServer : MyServerBase
    {
        public override void SynPingHandler(MyMessageReader request)
        {
            Console.WriteLine("Received SynPing, sn={0}", request.sn);
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var my_server = new MyServer();
            my_server.Start(false);
            var synReq = new MyMessageWriter(1);
            Global.CloudStorage.SynPingToMyServer(0, synReq);
            Console.WriteLine();
            Console.ReadKey();
        }
    }
}
```

After compiling the Project _Ping_, an abstract class called
_MyServerBase_ is generated. Now we can implement our GE
server very easily. All we need to do is to implement the abstract
handlers provided by _MyServerBase_. 

Here, we only have one synchronous handler _SynPingHandler_ needs to
be implemented. The name of the handler is generated based on the
protocol name specified in the TSL script. For demonstration purpose,
here we just output the received message to the console.  When
implementing _SynPingHandler_, the received message is wrapped in a
_Message Reader_ object called _MyMessageReader_. The previously
specified _MyMessage_ contains a sequential number _sn_. We can access
this _sn_ via the interfaces provided by _MyMessageReader_ in the
_SynPingHandler_.


Now the implementation of _MyServer_ is done. Let us instantiate an
instance of _MyServer_ in the program's _Main_ entry. After
instantiating a _MyServer_ instance, we can start the server by
calling its _Start()_ method.

To send a ping message, we first construct a message using
_MyMessageWriter()_. Sending a message can be done by calling the
_SynPingToMyServer_ method of _Global.CloudStorage_. The method has
two parameters. The first one is the _server id_ to which the message
is sent. The second parameter is the message writer object.

After compilation, we can test the ping application by starting the
generated _PingTest.exe_. If everything goes well, you can see the
output message "Received SynPing, sn=1" on the output console of
_PingTest.exe_.

_SynPing_ is a synchronous request without response. It is called
synchronous because _SynPing_ will be returned only if
_SynPingHandler_ is complete. As you can imagine, there should be an
asynchronous counterpart. We can specify asynchronous requests very
easily using TSL. Continuing with the _ping_ example, we can define an
asynchronous message passing protocol using the following lines in
TSL:

```C#
protocol AsynPing
{
	Type: Asyn;
	Request: MyMessage;
	Response: void;
}
```

The only difference from protocol _SynPing_ is their Type definition,
one is _Syn_ and the other is _Asyn_.

Correspondingly, GE will generate an _AsynPingHandler_
abstract method in _MyServerBase_ as well as an extension method
called _AsynPingToMyServer_ to _CloudStorage_. We add the following
lines to _MyServer_ class to implement the _AsynPingHandler_.

```C#
public override void AsynPingHandler(MyMessageReader request 
{
    Console.WriteLine("Received AsynPing, sn={0}", request.sn); 
}
```

After that, we can send an asynchronous message using the following
two lines:

```C#
var asynReq = new MyMessageWriter(2);
Global.CloudStorage.AsynPingToMyServer(0, asynReq);
```

We call this message passing protocol asynchronous because _AsynPing_
will return immediately after _asynReq_ message is received by the
other servers. That means when _AsynPingToMyServer_ call returns, the
corresponding _AsynPingHandler_ may haven't been invoked.

Both _SynPing_ and _AsynPing_ are requests without response because no
response message is specified. For synchronized protocols, the
response could be set to a user-defined struct. Here is an example of
synchronized protocol with response:

```C#
protocol SynEchoPing
{
	Type: Syn;
	Request: MyMessage;
	Response: MyMessage;
}
```

As specified by the definition, _SynEchoPing_ is a synchronous message
passing protocol. It sends request _MyMessage_ out and will be
responded with another _MyMessage_ struct .

A complete specification script of the ping example is listed below:

```C#
struct MyMessage
{
	int sn;
}

protocol SynPing
{
	Type: Syn;
	Request: MyMessage;
	Response: void;
}

protocol SynEchoPing
{
	Type: Syn;
	Request: MyMessage;
	Response: MyMessage;
}

protocol AsynPing
{
	Type: Asyn;
	Request: MyMessage;
	Response: void;
}

server MyServer
{
	protocol SynPing;
	protocol SynEchoPing;
	protocol AsynPing;
}
```

The corresponding code of the server implementation and message sending is listed below. 

```C#
using System;
using System.Collections.Generic;
using System.Text;
using Trinity.Data;
using Trinity.Storage;
using Trinity;
using System.Threading;

namespace PingTest
{
    class MyServer : MyServerBase
    {
        public override void SynPingHandler(MyMessageReader request)
        {
            Console.WriteLine("Received SynPing, sn={0}", request.sn);
        }

        public override void AsynPingHandler(MyMessageReader request)
        {
            Console.WriteLine("Received AsynPing, sn={0}", request.sn);
        }

        public override void SynEchoPingHandler(MyMessageReader request,
        MyMessageWriter response)
        {
            Console.WriteLine("Received SynEchoPing, sn={0}", request.sn);
            response.sn = request.sn;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var server = new MyServer();
            server.Start(false);

            var synReq = new MyMessageWriter(1);

            Global.CloudStorage.SynPingToMyServer(0, synReq);

            var asynReq = new MyMessageWriter(2);
            Global.CloudStorage.AsynPingToMyServer(0, asynReq);

            var synReqRsp = new MyMessageWriter(3);
            Console.WriteLine("response: {0}", 
                Global.CloudStorage.SynEchoPingToMyServer(0, synReqRsp).sn);

            while (true)
            {
                Thread.Sleep(3000);
            }

        }
    }
}
```

Running this example, you will see the following messages on the
server console:

```C#
Received SynPing, sn=1
Received AsynPing, sn=2
Received SynEchoPing, sn=3
response: 3
```
