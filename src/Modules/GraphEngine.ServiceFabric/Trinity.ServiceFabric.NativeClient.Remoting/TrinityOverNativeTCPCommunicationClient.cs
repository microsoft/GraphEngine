using System;
using System.Collections.Generic;
using System.Fabric;
using System.Text;
using Microsoft.ServiceFabric.Services.Communication.Client;

namespace Trinity.ServiceFabric.NativeClient.Remoting
{
    public class TrinityOverNativeTCPCommunicationClient: ICommunicationClient
    {
        public ResolvedServicePartition ResolvedServicePartition { get; set; }
        public string ListenerName { get; set; }
        public ResolvedServiceEndpoint Endpoint { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public TrinityOverNativeTCPCommunicationClient()
        {
            
        }
    }
}
