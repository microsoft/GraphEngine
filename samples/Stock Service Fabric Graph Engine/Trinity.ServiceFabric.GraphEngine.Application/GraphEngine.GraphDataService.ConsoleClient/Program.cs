using System;
using System.Threading.Tasks;
using Trinity;
using Trinity.Client;
using Trinity.Diagnostics;
using Trinity.ServiceFabric.SampleProtocols;
using Trinity.ServiceFabric.SampleProtocols.ServiceFabricSampleModule;

namespace GraphEngine.GraphDataService.ConsoleClient
{
    class Program
    {
        private static TrinityClient m_trinity;

        static async Task Main(string[] args)
        {
            TrinityConfig.LoggingLevel = LogLevel.Verbose;

            m_trinity = new TrinityClient("testcluster100.southcentralus.cloudapp.azure.com:8800");

            m_trinity.Start();

            long iterations = 0;

            PingMessagePayload syncPingMessage  = new PingMessagePayload("Ping Message from External Synchronous GE Client over Native TCP. Hello GE/SF");
            PingMessagePayload asyncPingMessage = new PingMessagePayload("Ping Message from External Asynchronous GE Client over Native TCP.Hello GE/SF");


            while (true)
            {

                m_trinity?.ExternalClientPing(new PingMessagePayloadWriter(syncPingMessage.PingMessage));

                var response = await m_trinity
                    ?.ExternalClientPingAsync(new PingMessagePayloadWriter(asyncPingMessage.PingMessage));

                Console.WriteLine($"Async Response from GE/SF Graph Data Service: {response.PingMessage}");

                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }
    }
}
