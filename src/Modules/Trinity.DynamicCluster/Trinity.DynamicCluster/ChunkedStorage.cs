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
using Trinity.Configuration;

namespace Trinity.DynamicCluster
{
    unsafe partial class ChunkedStorage : Storage.Storage
    {
        // List<Chunk> chunks;
        // need a protocol to communicate within an AG: QueryYourChunkIds
        // we also need a protocol to "discuss" about chunk allocation. (P2)
        // route messages to a specific storage
        private List<Storage.Storage> Storages; // <----
        private Dictionary<Storage.Storage, ChunkCollection> Chunks; //<-----ChunkCollection
        //DynamicMemoryCloud m_MemoryCloud; // get ChunkIds from m_MemoryCloud and decide routing.???????
        //List<long> chunk_range = DynamicMemoryCloud.ChunkRange;
        
        internal TrinityErrorCode Mount(Storage.Storage s)
        {
            try
            {
                Storages.Add(s);
                return TrinityErrorCode.E_SUCCESS;
            }
            catch (Exception ex)
            {
                Log.WriteLine(LogLevel.Error, "Errors occurred during Mount.");
                Log.WriteLine(LogLevel.Error, ex.ToString());
                return TrinityErrorCode.E_FAILURE;
            }
        }

        internal TrinityErrorCode Unmount(Storage.Storage s)
        {
            try
            {
                Storages.Remove(s);
                return TrinityErrorCode.E_SUCCESS;
            }
            catch (Exception ex)
            {
                Log.WriteLine(LogLevel.Error, "Errors occurred during Mount.");
                Log.WriteLine(LogLevel.Error, ex.ToString());
                return TrinityErrorCode.E_FAILURE;
            }
        }

        static internal long GetChunkIdByCellIdStatic(long cellId)
        {
            int chunk_id = -1;
            for (chunk_id = 0; chunk_id < DynamicMemoryCloud.ChunkRange.Count; chunk_id++)
            {
                if (cellId > DynamicMemoryCloud.ChunkRange[chunk_id]) return chunk_id + 1;
            }

            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal TrinityErrorCode _AddCell_impl(long cellId, byte* buff, int size, ushort cellType)
        {
            foreach (Storage.Storage s in PickStorages(cellId))
            {
                TrinityErrorCode eResult = s.AddCell(cellId, buff, size, cellType);
                if (eResult != TrinityErrorCode.E_SUCCESS) return eResult;
            }
            return TrinityErrorCode.E_SUCCESS;
        }

        public override TrinityErrorCode AddCell(long cellId, byte[] buff)
        {
            fixed (byte* p = buff)
            {
                TrinityErrorCode eResult = _AddCell_impl(cellId, p, buff.Length, StorageConfig.c_UndefinedCellType);
                return eResult;
            }
        }

        public override TrinityErrorCode AddCell(long cellId, byte* buff, int size)
        {
            return _AddCell_impl(cellId, buff, size, StorageConfig.c_UndefinedCellType);
        }

        public override TrinityErrorCode AddCell(long cellId, byte[] buff, int offset, int size)
        {
            fixed (byte* p = &buff[offset])
            {
                return _AddCell_impl(cellId, p, size, StorageConfig.c_UndefinedCellType);
            }
        }

        public override TrinityErrorCode AddCell(long cellId, byte* buff, int size, ushort cellType)
        {
            return _AddCell_impl(cellId, buff, size, cellType);
        }

        public override TrinityErrorCode AddCell(long cellId, byte[] buff, int offset, int size, ushort cellType)
        {
            fixed (byte* p = &buff[offset])
            {
                return _AddCell_impl(cellId, p, size, cellType);
            }
        }

        public override bool Contains(long cellId)
        {
            return (PickStorages(cellId).Where(s => s.Contains(cellId)).Any());
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
                TrinityErrorCode eResult = s.GetCellType(cellId, out cellType);
                if (eResult == TrinityErrorCode.E_SUCCESS) templist.Add(cellType);
                else return eResult;
            }
            if (templist.Count == 0) { cellType = 0; return TrinityErrorCode.E_CELL_NOT_FOUND; }
            if (templist.Any<ushort>(_ => _ != templist[0]))
            {
                cellType = 0; return TrinityErrorCode.E_FAILURE;//TODO we need to decide new error type.
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
                TrinityErrorCode eResult = s.LoadCell(cellId, out cellBuff);
                if (eResult == TrinityErrorCode.E_SUCCESS) templist.Add(cellBuff);
                else return eResult;
            }
            if (templist.Count == 0) { cellBuff = new byte[0]; return TrinityErrorCode.E_CELL_NOT_FOUND; }
            if (templist.Any<byte[]>(_ => _ != templist[0]))
            {
                cellBuff = new byte[0]; return TrinityErrorCode.E_FAILURE;//TODO we need to decide new error type.
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
                TrinityErrorCode eResult = s.LoadCell(cellId, out cellBuff, out cellType);
                if (eResult == TrinityErrorCode.E_SUCCESS)
                {
                    tempBuffList.Add(cellBuff);
                    tempTypeList.Add(cellType);
                }
                else return eResult;
            }
            if (tempBuffList.Count == 0) { cellBuff = new byte[0]; cellType = 0 ; return TrinityErrorCode.E_CELL_NOT_FOUND; }
            if (tempBuffList.Any<byte[]>(_ => _ != tempBuffList[0]) || tempTypeList.Any<ushort>(_ => _ != tempTypeList[0]))
            {
                cellBuff = new byte[0]; cellType = 0; return TrinityErrorCode.E_FAILURE;//TODO we need to decide new error type.
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
                TrinityErrorCode eResult = s.RemoveCell(cellId);
                if (eResult != TrinityErrorCode.E_SUCCESS) return eResult;
            }
            return TrinityErrorCode.E_SUCCESS;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal TrinityErrorCode _SaveCell_impl(long cellId, byte* buff, int size, ushort cellType)
        {
            foreach (Storage.Storage s in PickStorages(a => Chunks[a].Covers(cellId)))
            {
                TrinityErrorCode eResult = s.SaveCell(cellId, buff, size, cellType);
                if (eResult != TrinityErrorCode.E_SUCCESS) return eResult;
            }
            return TrinityErrorCode.E_SUCCESS;
        }

        public override TrinityErrorCode SaveCell(long cell_id, byte[] buff)
        {
            fixed (byte* p = buff)
            {
                return _SaveCell_impl(cell_id, p, buff.Length, StorageConfig.c_UndefinedCellType);
            }
        }

        public override TrinityErrorCode SaveCell(long cellId, byte* buff, int size)
        {
            return _SaveCell_impl(cellId, buff, size, StorageConfig.c_UndefinedCellType);
        }

        public override TrinityErrorCode SaveCell(long cellId, byte* buff, int size, ushort cellType)
        {
            return _SaveCell_impl(cellId, buff, size, cellType);
        }

        public override TrinityErrorCode SaveCell(long cell_id, byte[] buff, int offset, int size)
        {
            fixed (byte* p = buff)
            {
                return _SaveCell_impl(cell_id, p + offset, buff.Length, StorageConfig.c_UndefinedCellType);
            }
        }

        public override TrinityErrorCode SaveCell(long cellId, byte[] buff, int offset, int size, ushort cellType)
        {
            fixed (byte* p = buff)
            {
                return _SaveCell_impl(cellId, p + offset, buff.Length, cellType);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal TrinityErrorCode _UpdateCell_impl(long cellId, byte* buff, int size)
        {
            foreach (Storage.Storage s in PickStorages(a => Chunks[a].Covers(cellId)))
            {
                TrinityErrorCode eResult = s.UpdateCell(cellId, buff, size);
                if (eResult != TrinityErrorCode.E_SUCCESS) return eResult;
            }
            return TrinityErrorCode.E_SUCCESS;
        }

        public override TrinityErrorCode UpdateCell(long cellId, byte[] buff)
        {
            fixed (byte* p = buff)
            {
                return UpdateCell(cellId, p, buff.Length);
            }
        }

        public override TrinityErrorCode UpdateCell(long cellId, byte* buff, int size)
        {
            return _UpdateCell_impl(cellId, buff, size);
        }

        public override TrinityErrorCode UpdateCell(long cellId, byte[] buff, int offset, int size)
        {
            fixed (byte* p = buff)
            {
                return UpdateCell(cellId, p + offset, buff.Length);
            }
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

        public bool ContainsStorage(Storage.Storage storage)
        {
            return Storages.Any<Storage.Storage>(_ => _ == storage);
        }

        internal bool IsLocal(long cellId)
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
        public ChunkCollection QueryChunkCollection(Storage.Storage storage)
        {
           return Chunks[storage];
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
