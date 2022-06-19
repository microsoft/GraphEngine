# GraphEngine.Client

In a GraphEngine application, a client by default does not possess
message handling capabilities. `GraphEngine.Client` extends both the
client and the server to support client-side message handling through
a single messaging endpoint, while preserving the ability to access
the whole memory cloud.

When a client communication instance starts, it connects to the server
with a customizable messaging endpoint. The endpoint should implement
the interface `IMessagePassingEndpoint`, but is not limited to using
native Trinity protocol.  Therefore we could implement the interface
with any messaging transport, e.g.  WCF, gRPC etc. Once the endpoint
establishes connection, the client registers itself at the destination
server with a randomly generated cookie. The server keeps track of the
connected clients, and assigns a TTL value to each client.  The server
does not initiate new connections to the clients, but rather, responds
to client-side polling. New client-going messages are first queued at
the server, and piggybacked during polling. This allows a client to
send asynchronous messages, without being blocked by a single
request/response pair on the endpoint.

This module also implements `MemoryCloud` for both client and
server. The server side uses `HostedMemoryCloud: FixedMemoryCloud` and
provides aforementioned client-registration services. The client side
uses `ClientMemoryCloud: MemoryCloud`, which takes a single message
passing endpoint (rather than connecting to every remote storage in
the cluster), and send all messages as redirected messages, and all
key-value store operations as custom remote calls, which are handled
in `TrinityClientModule`.
