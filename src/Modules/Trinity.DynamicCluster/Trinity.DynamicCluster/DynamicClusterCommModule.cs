using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Trinity.Extension;
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
            var dmc = Global.CloudStorage as Trinity.Storage.DynamicMemoryCloud;
            response.partitionid = dmc.MyPartitionId;
            response.chunks.AddRange(dmc.MyChunkIds.Cast<int>().ToList());
        }
        public override void MotivateRemoteStorageOnLeavingStepOneHandler(_MotivateRemoteStorageOnLeavingStepOneRequestReader request)
        {
            var dmc = Global.CloudStorage as Trinity.Storage.DynamicMemoryCloud;
            dmc.OnStorageLeave(request.partitionid, request.chunks);
        }
        public override void MotivateRemoteStorageOnLeavingStepTwoHandler(_MotivateRemoteStorageOnLeavingStepTwoResponseWriter response)
        {
            var dmc = Global.CloudStorage as Trinity.Storage.DynamicMemoryCloud;
            response.leaving = dmc.MyLeavingStatus;
        }
    }
}
