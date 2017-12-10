using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Trinity.Extension;
using Trinity.Storage;
using Trinity.Network;
using Trinity.DynamicCluster.Storage;

namespace Trinity.DynamicCluster.Communication
{
    [AutoRegisteredCommunicationModule]
    class DynamicClusterCommModule : DynamicClusterBase
    {
        public override string GetModuleName()
        {
            return "DynamicClusterCommModule";
        }

        public override void NotifyRemoteStorageOnLeavingHandler(StorageInformationReader request)
        {
            var dmc = DynamicMemoryCloud.Instance;
            dmc.OnStorageLeave(request.partition, request.id);
        }

        public override void RemoteTaskHandler(RemoteTaskRequestReader request)
        {
        }
    }
}
