// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
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
using Trinity.DynamicCluster.DynamicCluster;

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
            var dmc        = DynamicMemoryCloud.Instance;
            var downloader = dmc.m_persistent_storage.Download(request.version, dmc.MyPartitionId, request.lowkey, request.highkey).Result;
            Task<IPersistentDataChunk> dtask = downloader.DownloadAsync();
            while (true)
            {
                var data = dtask?.Result;
                if (data == null) break;//fetched null from a task, EOF
                // start new download while we load it into memory storage
                var ndtask = downloader.DownloadAsync();
                foreach (var cell in data)
                { Global.LocalStorage.SaveCell(cell.CellId, cell.Buffer, cell.Offset, cell.Length, cell.CellType); }
                dtask = ndtask;
            }

            response.errno = Errno.E_OK;
        }

        public override void PersistedUploadHandler(PersistedSliceReader request, ErrnoResponseWriter response)
        {
            var dmc        = DynamicMemoryCloud.Instance;
            var uploader   = dmc.m_persistent_storage.Upload(request.version, dmc.MyPartitionId, request.lowkey, request.highkey).Result;
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
            var payload_chunk = InMemoryDataChunk.New(cells, (int)estimated_size);
            return uploader.UploadAsync(payload_chunk);
        }

        public override void ReplicationHandler(ReplicationTaskInformationReader request, ErrnoResponseWriter response)
        {
            var chunks         = request.range.Cast<ChunkInformation>().ToList();
            chunks.Sort((x, y) => Math.Sign(x.lowKey - y.lowKey));
            var target_replica = (Guid)request.to.id;
            var task_id        = (Guid)request.task_id;
            var thres       = DynamicClusterConfig.Instance.BatchSaveSizeThreshold;
            var to_storage     = DynamicMemoryCloud.Instance.MyPartition.OfType<DynamicRemoteStorage>()
                                .First(_ => _.ReplicaInformation.Id == target_replica);
            var signal_c       = new SemaphoreSlim(4);
            var signal_t       = new ManualResetEventSlim(false);

            //TODO timeout
            var batch_rsps     = Global.LocalStorage
                                .Where(cell => _Covered(chunks, cell.CellId))
                                .Segment(thres, cell => cell.CellSize + sizeof(long) + sizeof(int) + sizeof(ushort))
                                .Select(_ => _BuildBatch(_, task_id))
                                .Select(_ => _SendBatch(_, to_storage, signal_c, signal_t));

            Task.WhenAll(batch_rsps).Wait();

            response.errno = Errno.E_OK;
        }

        public override void ShrinkDataHandler(ShrinkDataTaskInformationReader request, ErrnoResponseWriter response)
        {
            var task_id        = (Guid)request.task_id;
            var remove_tgt     = request.remove_target.Cast<ChunkInformation>().ToList();
            var to_remove      = Global.LocalStorage
                                .Where(cell => _Covered(remove_tgt, cell.CellId))
                                .Select(cell => cell.CellId).ToList();
            foreach(var cellId in to_remove)
            {
                Global.LocalStorage.RemoveCell(cellId);
            }

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
                    using (var reader = await storage.BatchSaveCells(batch))
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
                Global.LocalStorage.SaveCell(cell.id, cell.content.m_ptr, cell.content.length, cell.cell_type);
            }
            //TODO throttle
            response.throttle = false;
        }

        public override void GetChunksHandler(GetChunksResponseWriter response)
        {
            var dmc = m_memorycloud as DynamicMemoryCloud;
            var chunks = dmc.m_chunktable.GetChunks(dmc.InstanceGuid).Result;
            foreach(var chunk in chunks)
            {
                response.chunk_info.Add(new ChunkInformation(chunk.LowKey, chunk.HighKey, chunk.Id));
            }
        }

        public override void AnnounceMasterHandler(StorageInformationReader request, ErrnoResponseWriter response)
        {
            (m_memorycloud as DynamicMemoryCloud).m_cloudidx.SetMaster(request.partition, request.id);
            response.errno = Errno.E_OK;
        }

        public override void PersistedLoadPartitionHandler(BackupTaskInformationReader request, ErrnoResponseWriter response)
        {
            try
            {
                (m_memorycloud as DynamicMemoryCloud).m_backupctl.RestoreCurrentPartition(request.task_id, request.version, EventArgs.Empty).Wait();
                response.errno = Errno.E_OK;
            }
            catch
            {
                response.errno = Errno.E_FAIL;
            }
        }

        public override void PersistedSavePartitionHandler(BackupTaskInformationReader request, ErrnoResponseWriter response)
        {
            try
            {
                (m_memorycloud as DynamicMemoryCloud).m_backupctl.BackupCurrentPartition(request.task_id, request.version, EventArgs.Empty).Wait();
                response.errno = Errno.E_OK;
            }
            catch
            {
                response.errno = Errno.E_FAIL;
            }
        }

        public override void QueryReplicaHealthHandler(ErrnoResponseWriter response)
        {
            response.errno = Errno.E_OK;
        }

        public override void QueryPartitionHealthHandler(ErrnoResponseWriter response)
        {
            if ((m_memorycloud as DynamicMemoryCloud).m_healthmon.IsPartitionHealthy())
            {
                response.errno = Errno.E_OK;
            }
            else
            {
                response.errno = Errno.E_FAIL;
            }
        }
    }
}
