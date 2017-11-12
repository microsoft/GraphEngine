using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Trinity.Extension;
using Trinity.Storage;
using Trinity.Network;

namespace Trinity.DynamicCluster
{
    [AutoRegisteredCommunicationModule]
    class DynamicClusterCommModule : DynamicClusterBase
    {
        public override string GetModuleName()
        {
            return "DynamicClusterCommModule";
        }

        public override void QueryChunkedRemoteStorageInformationHandler(_QueryChunkedRemoteStorageInformationReusltWriter response)
        {
            var dmc = DynamicMemoryCloud.Instance;
            var myname = dmc.MyName;
            StorageInformation info = new StorageInformation
            {
                partition = dmc.MyPartitionId,
                id = myname.ServerId,
                name = myname.Nickname,
            };
            response.info = info;
            response.chunks.AddRange(dmc.MyChunkIds.ToList());
        }
        public override void NotifyRemoteStorageOnLeavingHandler(StorageInformationReader request)
        {
            var dmc = DynamicMemoryCloud.Instance;
            dmc.OnStorageLeave(request.partition, (StorageInformation)request);
        }
    }
}
