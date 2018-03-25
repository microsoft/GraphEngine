using System;
using System.Collections.Generic;
using System.Linq;

using Trinity.Extension;
using Trinity.TSL.Lib;

namespace Trinity.Storage.Composite
{
    [ExtensionPriority(int.MaxValue)]
    public class GenericCellOperations : IGenericCellOperations
    {
        public IEnumerable<ICellAccessor> EnumerateGenericCellAccessors(LocalMemoryStorage storage)
        {
            return CompositeStorage.s_GenericCellOperations.SelectMany(cellOps => cellOps.EnumerateGenericCellAccessors(storage));
        }

        public IEnumerable<ICell> EnumerateGenericCells(LocalMemoryStorage storage)
        {
            return CompositeStorage.s_GenericCellOperations.SelectMany(cellOps => cellOps.EnumerateGenericCells(storage));
        }

        public unsafe ICell LoadGenericCell(IKeyValueStore storage, long cellId)
        {
            var err = storage.LoadCell(cellId, out byte[] buff, out ushort type);
            if (err != Trinity.TrinityErrorCode.E_SUCCESS)
            {
                switch (err)
                {
                    case Trinity.TrinityErrorCode.E_CELL_NOT_FOUND:
                        throw new CellNotFoundException("Cannot access the cell.");
                    case Trinity.TrinityErrorCode.E_NETWORK_SEND_FAILURE:
                        throw new System.IO.IOException("Network error while accessing the cell.");
                    default:
                        throw new Exception("Cannot access the cell. Error code: " + err.ToString());
                }
            }
            int seg = CompositeStorage.GetIntervalIndexByCellTypeID(type);

            fixed (byte* p = buff)
            {
                return CompositeStorage.s_GenericCellOperations[seg].UseGenericCell(cellId, p, -1, type); 
            }
        }

        public unsafe ICell LoadGenericCell(LocalMemoryStorage storage, long cellId)
        {
            var err = storage.GetLockedCellInfo(cellId, out int size, out ushort cellType, out byte* cellPtr, out int entryIndex);
            if (err != Trinity.TrinityErrorCode.E_SUCCESS)
            {
                throw new CellNotFoundException("Cannot access the cell.");
            }
            int seg = CompositeStorage.GetIntervalIndexByCellTypeID(cellType);

            return CompositeStorage.s_GenericCellOperations[seg].UseGenericCell(cellId, cellPtr, entryIndex, cellType);

        }

        public ICell NewGenericCell(string cellType)
        {
            int seg = CompositeStorage.GetIntervalIndexByCellTypeName(cellType);
            return CompositeStorage.s_GenericCellOperations[seg].NewGenericCell(cellType);
        }

        public ICell NewGenericCell(long cellId, string cellType)
        {
            int seg = CompositeStorage.GetIntervalIndexByCellTypeName(cellType);
            return CompositeStorage.s_GenericCellOperations[seg].NewGenericCell(cellId, cellType);
        }

        public ICell NewGenericCell(string cellType, string content)
        {
            int seg = CompositeStorage.GetIntervalIndexByCellTypeName(cellType);
            return CompositeStorage.s_GenericCellOperations[seg].NewGenericCell(cellType, content);
        }

        public void SaveGenericCell(IKeyValueStore storage, ICell cell)
        {
            int seg = CompositeStorage.GetIntervalIndexByCellTypeID(cell.CellType);
            CompositeStorage.s_GenericCellOperations[seg].SaveGenericCell(storage, cell);
        }

        public void SaveGenericCell(IKeyValueStore storage, long cellId, ICell cell)
        {
            int seg = CompositeStorage.GetIntervalIndexByCellTypeID(cell.CellType);
            CompositeStorage.s_GenericCellOperations[seg].SaveGenericCell(storage, cellId, cell);
        }

        public void SaveGenericCell(LocalMemoryStorage storage, CellAccessOptions writeAheadLogOptions, ICell cell)
        {
            int seg = CompositeStorage.GetIntervalIndexByCellTypeID(cell.CellType);
            CompositeStorage.s_GenericCellOperations[seg].SaveGenericCell(storage, writeAheadLogOptions, cell);
        }

        public void SaveGenericCell(LocalMemoryStorage storage, CellAccessOptions writeAheadLogOptions, long cellId, ICell cell)
        {
            int seg = CompositeStorage.GetIntervalIndexByCellTypeID(cell.CellType);
            CompositeStorage.s_GenericCellOperations[seg].SaveGenericCell(storage, writeAheadLogOptions, cellId, cell);
        }
        
        public unsafe ICellAccessor UseGenericCell(LocalMemoryStorage storage, long cellId)
        {
            var err = storage.GetLockedCellInfo(cellId, 
                                                out int size, 
                                                out ushort cellType, 
                                                out byte* cellPtr, 
                                                out int entryIndex);

            if (err != Trinity.TrinityErrorCode.E_SUCCESS)
            {
                throw new CellNotFoundException("Cannot access the cell.");
            }

            int seg = CompositeStorage.GetIntervalIndexByCellTypeID(cellType);

            return CompositeStorage.s_GenericCellOperations[seg].UseGenericCell(cellId, cellPtr, entryIndex, cellType);
        }

        public unsafe ICellAccessor UseGenericCell(LocalMemoryStorage storage, long cellId, CellAccessOptions options)
        {
            var err = storage
                        .GetLockedCellInfo(cellId,
                                            out int size,
                                            out ushort cellType,
                                            out byte* cellPtr,
                                            out int entryIndex);

            switch (err)
            {
                case Trinity.TrinityErrorCode.E_SUCCESS:
                    break;
                case Trinity.TrinityErrorCode.E_CELL_NOT_FOUND:
                    {
                        if ((options & CellAccessOptions.ThrowExceptionOnCellNotFound) != 0)
                        {
                            throw new CellNotFoundException("The cell with id = " + cellId + " not found.");
                        }
                        else if ((options & CellAccessOptions.CreateNewOnCellNotFound) != 0)
                        {
                            throw new ArgumentException(
                                "CellAccessOptions.CreateNewOnCellNotFound is not valid for this method. Cannot determine new cell type.", "options");
                        }
                        else if ((options & CellAccessOptions.ReturnNullOnCellNotFound) != 0)
                        {
                            return null;
                        }
                        else
                        {
                            throw new CellNotFoundException("The cell with id = " + cellId + " not found.");
                        }
                    }
                default:
                    throw new CellNotFoundException("Cannot access the cell.");
            }

            int seg = CompositeStorage.GetIntervalIndexByCellTypeID(cellType);

            return CompositeStorage.s_GenericCellOperations[seg].UseGenericCell(cellId, cellPtr, entryIndex, cellType, options);
        }

        public ICellAccessor UseGenericCell(LocalMemoryStorage storage, long cellId, CellAccessOptions options, string cellType)
        {
            int seg = CompositeStorage.GetIntervalIndexByCellTypeName(cellType);
            return CompositeStorage.s_GenericCellOperations[seg].UseGenericCell(storage, cellId, options, cellType);
        }

        public unsafe ICellAccessor UseGenericCell(long cellId, byte* cellPtr, int cellEntryIndex, ushort cellType)
        {
            int seg = CompositeStorage.GetIntervalIndexByCellTypeID(cellType);
            return CompositeStorage.s_GenericCellOperations[seg].UseGenericCell(cellId, cellPtr, cellEntryIndex, cellType);
            
        }

        public unsafe ICellAccessor UseGenericCell(long cellId, byte* cellPtr, int entryIndex, ushort cellType, CellAccessOptions options)
        {
            int seg = CompositeStorage.GetIntervalIndexByCellTypeID(cellType);
            return CompositeStorage.s_GenericCellOperations[seg].UseGenericCell(cellId, cellPtr, entryIndex, cellType, options);

        }
    }
}
