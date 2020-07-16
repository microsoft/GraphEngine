using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Diagnostics;
using Trinity.Extension;

namespace Trinity.ServiceFabric.SampleProtocols
{
    [AutoRegisteredCommunicationModule]
    public class SampleModuleImpl : ServiceFabricSampleModuleBase
    {
        public override string GetModuleName() => "SampleModuleImpl";

        private string stateMsg = string.Empty;

        public SampleModuleImpl()
        {
            stateMsg = "Tracing";
        }
        public override void PingHandler()
        {
            Log.WriteLine("Ping received from Graph Engine SF Remoting Client!");
        }

        public override void ExternalClientPingHandler(PingMessagePayloadReader request)
        {
            Log.WriteLine("Ping received from External Graph Engine Native TCP Client!");
            Log.WriteLine($"Ping Message from External Synchronous Client: {request.PingMessage}");
        }

        public override void ExternalClientPingAsyncHandler(PingMessagePayloadReader request, PingMessageResponsePayloadWriter response)
        {
            Log.WriteLine("Ping received from External Async Graph Engine Native TCP Client!");
            Log.WriteLine($"Ping Message from External Synchronous Client: {request.PingMessage}");

            response = new PingMessageResponsePayloadWriter("Hello from Graph Engine Service Fabric Graph Data Service");

        }
    }
}
