using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Trinity.Configuration;
using Trinity.Diagnostics;
using Trinity.Network;
using Trinity.Network.Messaging;
using Trinity.Storage;
using Trinity.Utilities;

namespace Trinity.Client
{
    public class TrinityClient : CommunicationInstance, IMessagePassingEndpoint
    {
        private IClientConnectionFactory m_clientfactory = null;
        private IMessagePassingEndpoint m_client;
        private readonly string m_endpoint;

        public TrinityClient(string endpoint)
            :this(endpoint, null) { }

        public TrinityClient(string endpoint, IClientConnectionFactory clientConnectionFactory)
        {
            m_endpoint = endpoint;
            m_clientfactory = clientConnectionFactory;
            ExtensionConfig.Instance.Priority.Add(new ExtensionPriority { Name = typeof(ClientMemoryCloud).AssemblyQualifiedName, Priority = int.MaxValue });
            ExtensionConfig.Instance.Priority = ExtensionConfig.Instance.Priority; // trigger update of priority table
        }

        protected override sealed RunningMode RunningMode => RunningMode.Client;

        public unsafe void SendMessage(byte* message, int size)
            => m_client.SendMessage(message, size);

        public unsafe void SendMessage(byte* message, int size, out TrinityResponse response)
            => m_client.SendMessage(message, size, out response);

        public unsafe void SendMessage(byte** message, int* sizes, int count)
            => m_client.SendMessage(message, sizes, count);

        public unsafe void SendMessage(byte** message, int* sizes, int count, out TrinityResponse response)
            => m_client.SendMessage(message, sizes, count, out response);

        protected override sealed void DispatchHttpRequest(HttpListenerContext ctx, string handlerName, string url)
            => throw new NotSupportedException();

        protected override sealed void RootHttpHandler(HttpListenerContext ctx)
            => throw new NotSupportedException();

        protected override void StartCommunicationListeners()
        {
            if (m_clientfactory == null) { ScanClientConnectionFactory(); }
            m_client = m_clientfactory.ConnectAsync(m_endpoint).Result;
            ClientMemoryCloud.Initialize(m_client, this);
        }

        private void ScanClientConnectionFactory()
        {
            Log.WriteLine(LogLevel.Info, $"{nameof(TrinityClient)}: scanning for client connection factory.");
            var rank = ExtensionConfig.Instance.ResolveTypePriorities();
            Func<Type, int> rank_func = t =>
            {
                if(rank.TryGetValue(t, out var r)) return r;
                else return 0;
            };
            m_clientfactory = AssemblyUtility.GetBestClassInstance<IClientConnectionFactory, DefaultClientConnectionFactory>(null, rank_func);
        }

        protected override void StopCommunicationListeners()
        {
            m_clientfactory.DisconnectAsync(m_client).Wait();
        }
    }
}
