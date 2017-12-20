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

        public override void PersistedDownloadHandler(PersistedSliceReader request, ErrnoResponseWriter response)
        {
            throw new NotImplementedException();
        }

        public override void PersistedUploadHandler(PersistedSliceReader request, ErrnoResponseWriter response)
        {
            throw new NotImplementedException();
        }

        public override void ReplicationHandler(ReplicationTaskInformationReader request, ErrnoResponseWriter response)
        {
            var chunks = request.range.Cast<ChunkInformation>().ToList();
            using (var batch = new BatchCellsWriter(request.task_id, new List<RawCell>()))
            {
                chunks.Sort((x, y) => Math.Sign(x.lowKey - y.lowKey));
                foreach (var c in chunks)
                {
                    //TODO
                }
            }
        }

        public unsafe override void BatchSaveCellsHandler(BatchCellsReader request, ThrottleResponseWriter response)
        {
            foreach(var cell in request.cells)
            {
                Global.LocalStorage.SaveCell(cell.id, cell.content.CellPtr, cell.content.length, cell.cell_type);
            }
            response.throttle = false;
        }
    }
}
