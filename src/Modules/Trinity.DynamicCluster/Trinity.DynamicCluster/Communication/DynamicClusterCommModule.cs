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
using Trinity.Diagnostics;
using System.Collections;
using Trinity.DynamicCluster.Config;
using System.Threading;

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
            var dmc = DynamicMemoryCloud.Instance;
            var uploader = dmc.m_persistent_storage.Upload(request.version, request.lowkey, request.highkey).Result;
        }

        public override void ReplicationHandler(ReplicationTaskInformationReader request, ErrnoResponseWriter response)
        {
            var chunks         = request.range.Cast<ChunkInformation>().ToList();
            chunks.Sort((x, y) => Math.Sign(x.lowKey - y.lowKey));
            var target_replica = (Guid)request.to.id;
            var task_id        = (Guid)request.task_id;
            var thres          = DynamicClusterConfig.Instance.BatchSaveSizeThreshold;
            var to_storage     = DynamicMemoryCloud.Instance.MyPartition.OfType<DynamicRemoteStorage>()
                                .First(_ => _.ReplicaInformation.Id == target_replica);
            var signal_c       = new SemaphoreSlim(4);
            var signal_t       = new ManualResetEventSlim(false);

            var batch_rsps     = Global.LocalStorage
                                .Where(cell => _Covered(chunks, cell.CellId))
                                .Segment(thres, cell => cell.CellSize + sizeof(long) + sizeof(int) + sizeof(ushort))
                                .Select(_ => _BuildBatch(_, task_id))
                                .Select(_ => _SendBatch(_, to_storage, signal_c, signal_t));

            Task.WhenAll(batch_rsps).Wait();

            response.errno = Errno.E_OK;
        }

        private async Task _SendBatch(BatchCellsWriter batch, DynamicRemoteStorage storage, SemaphoreSlim signal_c, ManualResetEventSlim signal_t)
        {
            await signal_c.WaitAsync();
            while (signal_t.IsSet) await Task.Delay(1);
            try
            {
                using (batch)
                {
                    //TODO it'd be much better if we can have message passing extension bindings directly on Storage, instead of relying on memory cloud. 
                    var idx = DynamicMemoryCloud.Instance.GetInstanceId(storage);
                    using (var reader = await this.BatchSaveCells(idx, batch))
                    {
                        if (reader.throttle)
                        {
                            signal_t.Set();
                            Log.WriteLine($"{nameof(_SendBatch)}: throttled by remote storage {storage.ReplicaInformation.Id}");
                            await Task.Delay(1000);
                            signal_t.Reset();
                        }
                    }
                }
            }
            finally
            {
                signal_c.Release();
            }
        }

        private BatchCellsWriter _BuildBatch(IEnumerable<CellInfo> cells, Guid task_id)
        {
            BatchCellsWriter writer = new BatchCellsWriter();
            writer.task_id = task_id;
            return writer;
        }

        private bool _Covered(List<ChunkInformation> chunks, long cellId)
        {
            //TODO binary search
            return chunks.Any(_ => _.lowKey <= cellId && cellId <= _.highKey);
        }

        public unsafe override void BatchSaveCellsHandler(BatchCellsReader request, ThrottleResponseWriter response)
        {
            foreach (var cell in request.cells)
            {
                Global.LocalStorage.SaveCell(cell.id, cell.content.CellPtr, cell.content.length, cell.cell_type);
            }
            //TODO throttle
            response.throttle = false;
        }
    }
}
