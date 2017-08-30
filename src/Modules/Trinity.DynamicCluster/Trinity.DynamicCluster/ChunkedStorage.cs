using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Trinity;
using Trinity.Network.Messaging;
using Trinity.Diagnostics;
using System.Runtime.CompilerServices;
using Trinity.Network;
using Trinity.Utilities;
using Trinity.Storage;
using Trinity.DynamicCluster;

namespace Trinity.DynamicCluster
{
    unsafe partial class ChunkedStorage : Storage.Storage
    {
        // List<Chunk> chunks;
        // need a protocol to communicate within an AG: QueryYourChunkIds
        // we also need a protocol to "discuss" about chunk allocation. (P2)
        // route messages to a specific storage
        List<Storage.Storage> Storages; // <----
        Dictionary<Storage.Storage, ChunkCollection> Chunks; //<-----ChunkCollection
        //DynamicMemoryCloud m_MemoryCloud; // get ChunkIds from m_MemoryCloud and decide routing.???????
        //List<long> chunk_range = DynamicMemoryCloud.ChunkRange;
        static internal long GetChunkIdByCellIdStatic(long cellId)
        {
            int chunk_id = -1;
            for (chunk_id = 0; chunk_id < DynamicMemoryCloud.ChunkRange.Count; chunk_id++)
            {
                if (cellId > DynamicMemoryCloud.ChunkRange[chunk_id]) return chunk_id + 1;
            }

            return 0;
        }

        public override TrinityErrorCode AddCell(long cellId, byte[] buff)
        {
            foreach (Storage.Storage s in PickStorages(a => Chunks[a].Covers(cellId)))
            {
                try
                {
                    s.AddCell(cellId, buff);
                }
                catch (Exception ex)
                {
                    Log.WriteLine(LogLevel.Error, "Errors occurred during AddCell.");
                    Log.WriteLine(LogLevel.Error, ex.ToString());
                    return TrinityErrorCode.E_FAILURE;
                }
            }
            return TrinityErrorCode.E_SUCCESS;
        }

        public override TrinityErrorCode AddCell(long cellId, byte* buff, int size)
        {
            foreach (Storage.Storage s in PickStorages(a => Chunks[a].Covers(cellId)))
            {
                try
                {
                    s.AddCell(cellId, buff, size);
                }
                catch (Exception ex)
                {
                    Log.WriteLine(LogLevel.Error, "Errors occurred during AddCell.");
                    Log.WriteLine(LogLevel.Error, ex.ToString());
                    return TrinityErrorCode.E_FAILURE;
                }
            }
            return TrinityErrorCode.E_SUCCESS;
        }

        public override TrinityErrorCode AddCell(long cellId, byte[] buff, int offset, int size)
        {
            foreach (Storage.Storage s in PickStorages(a => Chunks[a].Covers(cellId)))
            {
                try
                {
                    s.AddCell(cellId, buff, offset, size);
                }
                catch (Exception ex)
                {
                    Log.WriteLine(LogLevel.Error, "Errors occurred during AddCell.");
                    Log.WriteLine(LogLevel.Error, ex.ToString());
                    return TrinityErrorCode.E_FAILURE;
                }
            }
            return TrinityErrorCode.E_SUCCESS;
        }

        public override TrinityErrorCode AddCell(long cellId, byte* buff, int size, ushort cellType)
        {
            foreach (Storage.Storage s in PickStorages(a => Chunks[a].Covers(cellId)))
            {
                try
                {
                    s.AddCell(cellId, buff, size, cellType);
                }
                catch (Exception ex)
                {
                    Log.WriteLine(LogLevel.Error, "Errors occurred during AddCell.");
                    Log.WriteLine(LogLevel.Error, ex.ToString());
                    return TrinityErrorCode.E_FAILURE;
                }
            }
            return TrinityErrorCode.E_SUCCESS;
        }

        public override TrinityErrorCode AddCell(long cellId, byte[] buff, int offset, int size, ushort cellType)
        {
            foreach (Storage.Storage s in PickStorages(a => Chunks[a].Covers(cellId)))
            {
                try
                {
                    s.AddCell(cellId, buff, offset, size, cellType);
                }
                catch (Exception ex)
                {
                    Log.WriteLine(LogLevel.Error, "Errors occurred during AddCell.");
                    Log.WriteLine(LogLevel.Error, ex.ToString());
                    return TrinityErrorCode.E_FAILURE;
                }
            }
            return TrinityErrorCode.E_SUCCESS;
        }

        public override bool Contains(long cellId)
        {
            bool f = false;
            foreach (Storage.Storage s in PickStorages(a => Chunks[a].Covers(cellId)))
            {
                try
                {
                    if (s.Contains(cellId))
                    {
                        f = true;
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Log.WriteLine(LogLevel.Error, "Errors occurred during Contains.");
                    Log.WriteLine(LogLevel.Error, ex.ToString());
                }
            }
            return f;
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }

        public override TrinityErrorCode GetCellType(long cellId, out ushort cellType)
        {
            List<ushort> templist = new List<ushort>();

            foreach (Storage.Storage s in PickStorages(a => Chunks[a].Covers(cellId)))
            {
                try
                {
                    TrinityErrorCode err = s.GetCellType(cellId, out cellType);
                    templist.Add(cellType);
                }
                catch (Exception ex)
                {
                    Log.WriteLine(LogLevel.Error, "Errors occurred during GetCellType.");
                    Log.WriteLine(LogLevel.Error, ex.ToString());
                    cellType = 0;
                    return TrinityErrorCode.E_FAILURE;
                }
            }
            if (templist.Count == 0) { cellType = 0; return TrinityErrorCode.E_FAILURE; }
            if (templist.Any<ushort>(_ => _ != templist[0]))
            {
                cellType = 0; return TrinityErrorCode.E_FAILURE;
            }
            else
            {
                cellType = templist[0]; return TrinityErrorCode.E_SUCCESS;
            }
        }

        public override TrinityErrorCode LoadCell(long cellId, out byte[] cellBuff)
        {
            List<byte[]> templist = new List<byte[]>();

            foreach (Storage.Storage s in PickStorages(a => Chunks[a].Covers(cellId)))
            {
                try
                {
                    TrinityErrorCode err = s.LoadCell(cellId, out cellBuff);
                    templist.Add(cellBuff);
                }
                catch (Exception ex)
                {
                    Log.WriteLine(LogLevel.Error, "Errors occurred during LoadCell.");
                    Log.WriteLine(LogLevel.Error, ex.ToString());
                    cellBuff = new byte[0];
                    return TrinityErrorCode.E_FAILURE;
                }
            }
            if (templist.Count == 0) { cellBuff = new byte[0]; ; return TrinityErrorCode.E_FAILURE; }
            if (templist.Any<byte[]>(_ => _ != templist[0]))
            {
                cellBuff = new byte[0]; return TrinityErrorCode.E_FAILURE;
            }
            else
            {
                cellBuff = templist[0]; return TrinityErrorCode.E_SUCCESS;
            }
        }

        public override TrinityErrorCode LoadCell(long cellId, out byte[] cellBuff, out ushort cellType)
        {
            List<byte[]> tempBuffList = new List<byte[]>();
            List<ushort> tempTypeList = new List<ushort>();

            foreach (Storage.Storage s in PickStorages(a => Chunks[a].Covers(cellId)))
            {
                try
                {
                    TrinityErrorCode err = s.LoadCell(cellId, out cellBuff, out cellType);
                    tempBuffList.Add(cellBuff);
                    tempTypeList.Add(cellType);
                }
                catch (Exception ex)
                {
                    Log.WriteLine(LogLevel.Error, "Errors occurred during LoadCell.");
                    Log.WriteLine(LogLevel.Error, ex.ToString());
                    cellBuff = new byte[0];
                    cellType = 0;
                    return TrinityErrorCode.E_FAILURE;
                }
            }
            if (tempBuffList.Count == 0) { cellBuff = new byte[0]; cellType = 0 ; return TrinityErrorCode.E_FAILURE; }
            if (tempBuffList.Any<byte[]>(_ => _ != tempBuffList[0]) || tempTypeList.Any<ushort>(_ => _ != tempTypeList[0]))
            {
                cellBuff = new byte[0]; cellType = 0; return TrinityErrorCode.E_FAILURE;
            }
            else
            {
                cellBuff = tempBuffList[0]; cellType = tempTypeList[0]; return TrinityErrorCode.E_SUCCESS;
            }
        }

        public override TrinityErrorCode RemoveCell(long cellId)
        {
            foreach (Storage.Storage s in PickStorages(a => Chunks[a].Covers(cellId)))
            {
                try
                {
                    s.RemoveCell(cellId);
                }
                catch (Exception ex)
                {
                    Log.WriteLine(LogLevel.Error, "Errors occurred during RemoveCell.");
                    Log.WriteLine(LogLevel.Error, ex.ToString());
                    return TrinityErrorCode.E_FAILURE;
                }
            }
            return TrinityErrorCode.E_SUCCESS;
        }

        public override TrinityErrorCode SaveCell(long cell_id, byte[] buff)
        {
            foreach (Storage.Storage s in PickStorages(a => Chunks[a].Covers(cellId)))
            {
                try
                {
                    s.SaveCell(cell_id, buff);
                }
                catch (Exception ex)
                {
                    Log.WriteLine(LogLevel.Error, "Errors occurred during SaveCell.");
                    Log.WriteLine(LogLevel.Error, ex.ToString());
                    return TrinityErrorCode.E_FAILURE;
                }
            }
            return TrinityErrorCode.E_SUCCESS;
        }

        public override TrinityErrorCode SaveCell(long cellId, byte* buff, int size)
        {
            foreach (Storage.Storage s in PickStorages(a => Chunks[a].Covers(cellId)))
            {
                try
                {
                    s.SaveCell(cellId, buff, size);
                }
                catch (Exception ex)
                {
                    Log.WriteLine(LogLevel.Error, "Errors occurred during SaveCell.");
                    Log.WriteLine(LogLevel.Error, ex.ToString());
                    return TrinityErrorCode.E_FAILURE;
                }
            }
            return TrinityErrorCode.E_SUCCESS;
        }

        public override TrinityErrorCode SaveCell(long cellId, byte* buff, int size, ushort cellType)
        {
            foreach (Storage.Storage s in PickStorages(a => Chunks[a].Covers(cellId)))
            {
                try
                {
                    s.SaveCell(cellId, buff, size, cellType);
                }
                catch (Exception ex)
                {
                    Log.WriteLine(LogLevel.Error, "Errors occurred during SaveCell.");
                    Log.WriteLine(LogLevel.Error, ex.ToString());
                    return TrinityErrorCode.E_FAILURE;
                }
            }
            return TrinityErrorCode.E_SUCCESS;
        }

        public override TrinityErrorCode SaveCell(long cell_id, byte[] buff, int offset, int size)
        {
            foreach (Storage.Storage s in PickStorages(a => Chunks[a].Covers(cellId)))
            {
                try
                {
                    s.SaveCell(cell_id, buff, offset, size);
                }
                catch (Exception ex)
                {
                    Log.WriteLine(LogLevel.Error, "Errors occurred during SaveCell.");
                    Log.WriteLine(LogLevel.Error, ex.ToString());
                    return TrinityErrorCode.E_FAILURE;
                }
            }
            return TrinityErrorCode.E_SUCCESS;
        }

        public override TrinityErrorCode SaveCell(long cellId, byte[] buff, int offset, int size, ushort cellType)
        {
            foreach (Storage.Storage s in PickStorages(a => Chunks[a].Covers(cellId)))
            {
                try
                {
                    s.SaveCell(cellId, buff, offset, size, cellType);
                }
                catch (Exception ex)
                {
                    Log.WriteLine(LogLevel.Error, "Errors occurred during SaveCell.");
                    Log.WriteLine(LogLevel.Error, ex.ToString());
                    return TrinityErrorCode.E_FAILURE;
                }
            }
            return TrinityErrorCode.E_SUCCESS;
        }

        public override TrinityErrorCode UpdateCell(long cellId, byte[] buff)
        {
            foreach (Storage.Storage s in PickStorages(a => Chunks[a].Covers(cellId)))
            {
                try
                {
                    s.UpdateCell(cellId, buff);
                }
                catch (Exception ex)
                {
                    Log.WriteLine(LogLevel.Error, "Errors occurred during UpdateCell.");
                    Log.WriteLine(LogLevel.Error, ex.ToString());
                    return TrinityErrorCode.E_FAILURE;
                }
            }
            return TrinityErrorCode.E_SUCCESS;
        }

        public override TrinityErrorCode UpdateCell(long cellId, byte* buff, int size)
        {
            foreach (Storage.Storage s in PickStorages(a => Chunks[a].Covers(cellId)))
            {
                try
                {
                    s.UpdateCell(cellId, buff, size);
                }
                catch (Exception ex)
                {
                    Log.WriteLine(LogLevel.Error, "Errors occurred during UpdateCell.");
                    Log.WriteLine(LogLevel.Error, ex.ToString());
                    return TrinityErrorCode.E_FAILURE;
                }
            }
            return TrinityErrorCode.E_SUCCESS;
        }

        public override TrinityErrorCode UpdateCell(long cellId, byte[] buff, int offset, int size)
        {
            foreach (Storage.Storage s in PickStorages(a => Chunks[a].Covers(cellId)))
            {
                try
                {
                    s.UpdateCell(cellId, buff, offset, size);
                }
                catch (Exception ex)
                {
                    Log.WriteLine(LogLevel.Error, "Errors occurred during UpdateCell.");
                    Log.WriteLine(LogLevel.Error, ex.ToString());
                    return TrinityErrorCode.E_FAILURE;
                }
            }
            return TrinityErrorCode.E_SUCCESS;
        }

        public override void SendMessage(TrinityMessage message)
        {
            throw new NotImplementedException();
        }

        public override void SendMessage(byte* message, int size)
        {
            throw new NotImplementedException();
        }

        public override void SendMessage(TrinityMessage message, out TrinityResponse response)
        {
            throw new NotImplementedException();
        }

        public override void SendMessage(byte* message, int size, out TrinityResponse response)
        {
            throw new NotImplementedException();
        }


        internal bool ContainsLocalStorage(long cellId)
        {
            return PickStorages(cellId).Any(s => s == Global.LocalStorage);
        }

        private IEnumerable<Storage.Storage> PickStorages(long cellId)
        {
            return PickStorages(s => Chunks[s].Covers(cellId));
        }

        private IEnumerable<Storage.Storage> PickStorages(Func<Storage.Storage, bool> pred)
        {
            return Storages.Where(pred);
        }
    }

    internal class ChunkCollection : List<long>
    {
        internal bool Covers(long cellId)
        {
            long chunk_id = ChunkedStorage.GetChunkIdByCellIdStatic(cellId);
            if (this.Any<long>(_ => _ == chunk_id)) return true;
            else return false;
        }
    }

}
