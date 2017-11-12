using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Trinity.Configuration;

namespace Trinity.DynamicCluster
{
    using Storage = Trinity.Storage.Storage;
    internal unsafe partial class ChunkedStorage
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal TrinityErrorCode _AddCell_impl(long cellId, byte* buff, int size, ushort cellType)
        {
            foreach (Storage s in PickStorages(cellId))
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

        public override TrinityErrorCode GetCellType(long cellId, out ushort cellType)
        {
            List<ushort> templist = new List<ushort>();
            foreach (Storage s in PickStorages(cellId))
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
            foreach (Storage s in PickStorages(cellId))
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
            foreach (Storage s in PickStorages(cellId))
            {
                TrinityErrorCode eResult = s.LoadCell(cellId, out cellBuff, out cellType);
                if (eResult == TrinityErrorCode.E_SUCCESS)
                {
                    tempBuffList.Add(cellBuff);
                    tempTypeList.Add(cellType);
                }
                else return eResult;
            }
            if (tempBuffList.Count == 0) { cellBuff = new byte[0]; cellType = 0; return TrinityErrorCode.E_CELL_NOT_FOUND; }
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
            foreach (Storage s in PickStorages(cellId))
            {
                TrinityErrorCode eResult = s.RemoveCell(cellId);
                if (eResult != TrinityErrorCode.E_SUCCESS) return eResult;
            }
            return TrinityErrorCode.E_SUCCESS;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal TrinityErrorCode _SaveCell_impl(long cellId, byte* buff, int size, ushort cellType)
        {
            foreach (Storage s in PickStorages(cellId))
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
            foreach (Storage s in PickStorages(cellId))
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

    }
}
