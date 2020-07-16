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
        public unsafe Task<TrinityErrorCode> AddCellAsync(long cellId, byte* buff, int size, ushort cellType)
        {
            return new AsyncClosure(PickStorages(cellId), s => s.AddCellAsync(cellId, buff, size, cellType)).ExecuteAsync();
        }

        public async Task<bool> ContainsAsync(long cellId)
        {
            foreach (IStorage s in PickStorages(cellId))
            {
                if (await s.ContainsAsync(cellId)) return true;
            }
            return false;
        }

        public async Task<(TrinityErrorCode, ushort)> GetCellTypeAsync(long cellId)
        {
            List<ushort> templist = new List<ushort>();
            foreach (IStorage s in PickStorages(cellId))
            {
                var result = await s.GetCellTypeAsync(cellId);
                TrinityErrorCode eResult = result.ErrorCode;
                ushort cellType = result.CellType;
                if (eResult == TrinityErrorCode.E_SUCCESS) templist.Add(cellType);
                else return (eResult, cellType);
            }
            if (templist.Count == 0) { return (TrinityErrorCode.E_CELL_NOT_FOUND, 0); }
            if (templist.Any<ushort>(_ => _ != templist[0]))
            {
                return (TrinityErrorCode.E_FAILURE, 0);//TODO we need to decide new error type.
            }
            else
            {
                return (TrinityErrorCode.E_SUCCESS, templist[0]);
            }
        }

        public async Task<LoadCellResponse> LoadCellAsync(long cellId)
        {
            byte[] cellBuff;
            ushort cellType;
            List<byte[]> tempBuffList = new List<byte[]>();
            List<ushort> tempTypeList = new List<ushort>();
            foreach (IStorage s in PickStorages(cellId))
            {
                var result = await s.LoadCellAsync(cellId);
                TrinityErrorCode eResult = result.ErrorCode;
                cellBuff = result.CellBuff;
                cellType = result.CellType;
                if (eResult == TrinityErrorCode.E_SUCCESS)
                {
                    tempBuffList.Add(cellBuff);
                    tempTypeList.Add(cellType);
                }
                else return new LoadCellResponse(eResult, new byte[0], 0);
            }
            if (tempBuffList.Count == 0) { return new LoadCellResponse(TrinityErrorCode.E_CELL_NOT_FOUND, new byte[0], 0); }
            if (tempBuffList.Any<byte[]>(_ => _ != tempBuffList[0]) || tempTypeList.Any<ushort>(_ => _ != tempTypeList[0]))
            {
                return new LoadCellResponse(TrinityErrorCode.E_FAILURE, new byte[0], 0);//TODO we need to decide new error type.
            }
            else
            {
                return new LoadCellResponse(TrinityErrorCode.E_SUCCESS, tempBuffList[0], tempTypeList[0]);
            }
        }

        public unsafe Task<LoadCellUnsafeResponse> LoadCellUnsafeAsync(long cellId)
        {
            List<long> tempBuffList = new List<long>();
            List<int> tempSizeList = new List<int>();
            List<ushort> tempTypeList = new List<ushort>();

            return new AsyncClosure(
                PickStorages(cellId), 
                s => 
                {
                    return s.LoadCellUnsafeAsync(cellId).ContinueWith(
                        t =>
                        {
                            TrinityErrorCode eResult = t.Result.ErrorCode;
                            byte* cellBuf = t.Result.CellBuff;
                            int cellSize = t.Result.Size;
                            ushort cellType = t.Result.CellType;
                            if (eResult == TrinityErrorCode.E_SUCCESS)
                            {
                                tempBuffList.Add((long)cellBuf);
                                tempSizeList.Add(cellSize);
                                tempTypeList.Add(cellType);
                            }
                            return eResult;
                        });
                }).ExecuteAsync()
                  .ContinueWith(
                    t =>
                    {
                        TrinityErrorCode eResult = t.Result;
                        byte* cellBuf;
                        int cellSize;
                        ushort cellType;

                        if (eResult == TrinityErrorCode.E_SUCCESS && tempBuffList.Count == 0) { eResult = TrinityErrorCode.E_CELL_NOT_FOUND; }

                        if (eResult == TrinityErrorCode.E_SUCCESS)
                        {
                            var tbuf = (byte*)tempBuffList[0];
                            var ts = tempSizeList[0];
                            var tt = tempTypeList[0];

                            for (int i = 1; i < tempBuffList.Count; ++i)
                            {
                                if (ts != tempSizeList[i] || tt != tempTypeList[i] || 0 != Memory.memcmp(tbuf, (byte*)tempBuffList[i], (ulong)ts))
                                {
                                    //TODO we need to decide new error type.
                                    eResult = TrinityErrorCode.E_FAILURE;
                                    break;
                                }
                            }
                        }

                        if (eResult == TrinityErrorCode.E_SUCCESS)
                        {
                            cellBuf = (byte*)tempBuffList[0];
                            cellSize = tempSizeList[0];
                            cellType = tempTypeList[0];

                            tempBuffList.RemoveAt(0);
                            tempSizeList.RemoveAt(0);
                            tempTypeList.RemoveAt(0);
                        }
                        else
                        {
                            cellBuf = null;
                            cellSize = -1;
                            cellType = 0;
                        }

                        for (int i = 0; i < tempBuffList.Count; ++i)
                        {
                            Memory.free((byte*)tempBuffList[i]);
                        }

                        return new LoadCellUnsafeResponse(eResult, cellBuf, cellSize, cellType);
                    });
        }

        public async Task<TrinityErrorCode> RemoveCellAsync(long cellId)
        {
            foreach (IStorage s in PickStorages(cellId))
            {
                TrinityErrorCode eResult = await s.RemoveCellAsync(cellId);
                if (eResult != TrinityErrorCode.E_SUCCESS) return eResult;
            }
            return TrinityErrorCode.E_SUCCESS;
        }

        public unsafe Task<TrinityErrorCode> SaveCellAsync(long cellId, byte* buff, int size, ushort cellType)
        {
            return new AsyncClosure(PickStorages(cellId), s => s.SaveCellAsync(cellId, buff, size, cellType)).ExecuteAsync();
        }

        public unsafe Task<TrinityErrorCode> UpdateCellAsync(long cellId, byte* buff, int size)
        {
            return new AsyncClosure(PickStorages(cellId), s => s.UpdateCellAsync(cellId, buff, size)).ExecuteAsync();
        }

        private class AsyncClosure
        {
            IEnumerator<IStorage> _enumerator;
            Func<IStorage, Task<TrinityErrorCode>> _func;

            public AsyncClosure(IEnumerable<IStorage> storages, Func<IStorage, Task<TrinityErrorCode>> func)
            {
                _func = func;
                _enumerator = storages.GetEnumerator();
                _enumerator.Reset();
            }

            public Task<TrinityErrorCode> ExecuteAsync()
            {
                if (!_enumerator.MoveNext()) return Task.FromResult(TrinityErrorCode.E_SUCCESS);
                var s = _enumerator.Current;
                return _func(s).ContinueWith(
                    t =>
                    {
                        TrinityErrorCode eResult = t.Result;
                        if (eResult != TrinityErrorCode.E_SUCCESS) return Task.FromResult(eResult);
                        return ExecuteAsync();
                    }).Unwrap();
            }
        }

    }
}
