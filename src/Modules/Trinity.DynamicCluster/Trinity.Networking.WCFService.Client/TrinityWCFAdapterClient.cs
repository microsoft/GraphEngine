using Microsoft.ServiceFabric.Services.Communication.Client;
using System;
using System.ServiceModel;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Client;
using System.Fabric;
using System.Threading;
using Trinity.Networking.WCFService;

namespace Trinity.Networking.WCFService.Client
{
    public class TrinityWCFAdapterClient : ServicePartitionClient<WcfCommunicationClient<ITrinityWCFAdapter>>, ITrinityWCFAdapter
    {
        public TrinityWCFAdapterClient(ICommunicationClientFactory<WcfCommunicationClient<ITrinityWCFAdapter>> communicationClientFactory, Uri serviceUri, ServicePartitionKey partitionKey = null, TargetReplicaSelector targetReplicaSelector = TargetReplicaSelector.Default, string listenerName = null, OperationRetrySettings retrySettings = null) 
            : base(communicationClientFactory, serviceUri, partitionKey, targetReplicaSelector, listenerName, retrySettings)
        {
        }

        public void SendMessageWithoutResponse(byte[] request)
        {
            throw new NotImplementedException();
        }

        public byte[] SendMessageWithResponse(byte[] request)
        {
            throw new NotImplementedException();
        }
    }

    public class WcfCommunicationClient<TAdapter> : ICommunicationClient
    {
        public ResolvedServicePartition ResolvedServicePartition { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string ListenerName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ResolvedServiceEndpoint Endpoint { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }

    public class WcfCommunicationClientFactory : ICommunicationClientFactory<WcfCommunicationClient<ITrinityWCFAdapter>>
    {
        public event EventHandler<CommunicationClientEventArgs<WcfCommunicationClient<ITrinityWCFAdapter>>> ClientConnected;
        public event EventHandler<CommunicationClientEventArgs<WcfCommunicationClient<ITrinityWCFAdapter>>> ClientDisconnected;

        public Task<WcfCommunicationClient<ITrinityWCFAdapter>> GetClientAsync(Uri serviceUri, ServicePartitionKey partitionKey, TargetReplicaSelector targetReplicaSelector, string listenerName, OperationRetrySettings retrySettings, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<WcfCommunicationClient<ITrinityWCFAdapter>> GetClientAsync(ResolvedServicePartition previousRsp, TargetReplicaSelector targetReplicaSelector, string listenerName, OperationRetrySettings retrySettings, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<OperationRetryControl> ReportOperationExceptionAsync(WcfCommunicationClient<ITrinityWCFAdapter> client, ExceptionInformation exceptionInformation, OperationRetrySettings retrySettings, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }


}
