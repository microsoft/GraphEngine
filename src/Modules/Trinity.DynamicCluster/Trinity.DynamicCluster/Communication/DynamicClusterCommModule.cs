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
using Trinity.DynamicCluster.Persistency;
using System.IO;
using Trinity.Core.Lib;

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
            var dmc        = DynamicMemoryCloud.Instance;
            var uploader   = dmc.m_persistent_storage.Upload(request.version, request.lowkey, request.highkey).Result;
            var chunks     = new[]{ new ChunkInformation{lowKey = request.lowkey, highKey = request.highkey, id = request.version} };
            var thres      = DynamicClusterConfig.Instance.PersistedChunkSizeThreshold;
            var tx_rsps    = Global.LocalStorage
                            .Where(cell => _Covered(chunks, cell.CellId))
                            .Segment(thres, cell => cell.CellSize + sizeof(long) + sizeof(int) + sizeof(ushort))
                            .Select(_ => _Upload(_, uploader, thres));

            Task.WhenAll(tx_rsps).Wait();

            response.errno = Errno.E_OK;
        }

        private unsafe Task _Upload(IEnumerable<CellInfo> cells, IPersistentUploader uploader, long estimated_size)
        {
            MemoryStream ms = new MemoryStream((int)estimated_size);
            byte[] buf = new byte[sizeof(long) + sizeof(ushort) + sizeof(int)];
            byte[] buf_cell = new byte[1024];
            long lowkey = 0, highkey = 0;
            int size = 0;
            fixed (byte* p = buf)
            {
                foreach (var cell in cells)
                {
                    if (size == 0) lowkey = cell.CellId;
                    highkey = cell.CellId;
                    PointerHelper sp = PointerHelper.New(p);
                    *sp.lp++ = cell.CellId;
                    *sp.sp++ = (short)cell.CellType;
                    *sp.ip++ = cell.CellSize;
                    ms.Write(buf, 0, buf.Length);
                    while(buf_cell.Length < cell.CellSize)
                    {
                        buf_cell = new byte[buf_cell.Length * 2];
                    }
                    Memory.Copy(cell.CellPtr, 0, buf_cell, 0, cell.CellSize);
                    ms.Write(buf_cell, 0, cell.CellSize);
                    size += cell.CellSize + buf.Length;
                }
            }
            if (size == 0) return Task.CompletedTask;
            byte[] payload = ms.GetBuffer();
            Chunk chunk = new Chunk(lowkey, highkey);
            InMemoryDataChunk payload_chunk = new InMemoryDataChunk(chunk, payload, lowkey, highkey);
            return uploader.UploadAsync(payload_chunk);
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

        private bool _Covered(IList<ChunkInformation> chunks, long cellId)
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
