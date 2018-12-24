using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Trinity.Configuration;
using Trinity.Storage;
using Trinity.Core.Lib;

namespace Trinity.DynamicCluster.Storage
{
    public partial class Partition
    {
        public unsafe TrinityErrorCode AddCell(long cellId, byte* buff, int size, ushort cellType)
        {
            foreach (IStorage s in PickStorages(cellId))
            {
                TrinityErrorCode eResult = s.AddCell(cellId, buff, size, cellType);
                if (eResult != TrinityErrorCode.E_SUCCESS) return eResult;
            }
            return TrinityErrorCode.E_SUCCESS;
        }

        public bool Contains(long cellId)
        {
            return (PickStorages(cellId).Where(s => s.Contains(cellId)).Any());
        }

        public TrinityErrorCode GetCellType(long cellId, out ushort cellType)
        {
            List<ushort> templist = new List<ushort>();
            foreach (IStorage s in PickStorages(cellId))
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

        public TrinityErrorCode LoadCell(long cellId, out byte[] cellBuff, out ushort cellType)
        {
            List<byte[]> tempBuffList = new List<byte[]>();
            List<ushort> tempTypeList = new List<ushort>();
            foreach (IStorage s in PickStorages(cellId))
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

        public unsafe TrinityErrorCode LoadCell(long cellId, out byte* cellBuf, out int cellSize, out ushort cellType)
        {
            List<long> tempBuffList = new List<long>();
            List<int> tempSizeList = new List<int>();
            List<ushort> tempTypeList = new List<ushort>();
            TrinityErrorCode eResult = TrinityErrorCode.E_SUCCESS;

            foreach (IStorage s in PickStorages(cellId))
            {
                eResult = s.LoadCell(cellId, out cellBuf, out cellSize, out cellType);
                if (eResult == TrinityErrorCode.E_SUCCESS)
                {
                    tempBuffList.Add((long)cellBuf);
                    tempSizeList.Add(cellSize);
                    tempTypeList.Add(cellType);
                }
                else break;
            }

            if (eResult == TrinityErrorCode.E_SUCCESS && tempBuffList.Count == 0) { eResult = TrinityErrorCode.E_CELL_NOT_FOUND; }

            if (eResult == TrinityErrorCode.E_SUCCESS)
            {
                var tbuf = (byte*)tempBuffList[0];
                var ts   = tempSizeList[0];
                var tt   = tempTypeList[0];

                for(int i = 1; i < tempBuffList.Count; ++i)
                {
                    if(ts != tempSizeList[i] || tt != tempTypeList[i] || 0 != Memory.memcmp(tbuf, (byte*)tempBuffList[i], (ulong)ts))
                    {
                        //TODO we need to decide new error type.
                        eResult = TrinityErrorCode.E_FAILURE;
                        break;
                    }
                }
            }

            if(eResult == TrinityErrorCode.E_SUCCESS)
            {
                cellBuf  = (byte*)tempBuffList[0];
                cellSize = tempSizeList[0];
                cellType = tempTypeList[0];

                tempBuffList.RemoveAt(0);
                tempSizeList.RemoveAt(0);
                tempTypeList.RemoveAt(0);
            }
            else
            {
                cellBuf  = null;
                cellSize = -1;
                cellType = 0;
            }

            for(int i = 0; i < tempBuffList.Count; ++i)
            {
                Memory.free((byte*)tempBuffList[i]);
            }

            return eResult;
        }

        public TrinityErrorCode RemoveCell(long cellId)
        {
            foreach (IStorage s in PickStorages(cellId))
            {
                TrinityErrorCode eResult = s.RemoveCell(cellId);
                if (eResult != TrinityErrorCode.E_SUCCESS) return eResult;
            }
            return TrinityErrorCode.E_SUCCESS;
        }

        public unsafe TrinityErrorCode SaveCell(long cellId, byte* buff, int size, ushort cellType)
        {
            foreach (IStorage s in PickStorages(cellId))
            {
                TrinityErrorCode eResult = s.SaveCell(cellId, buff, size, cellType);
                if (eResult != TrinityErrorCode.E_SUCCESS) return eResult;
            }
            return TrinityErrorCode.E_SUCCESS;
        }

        public unsafe TrinityErrorCode UpdateCell(long cellId, byte* buff, int size)
        {
            foreach (IStorage s in PickStorages(cellId))
            {
                TrinityErrorCode eResult = s.UpdateCell(cellId, buff, size);
                if (eResult != TrinityErrorCode.E_SUCCESS) return eResult;
            }
            return TrinityErrorCode.E_SUCCESS;
        }
    }
}
