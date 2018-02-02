using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Storage;
using Trinity.TSL.Lib;

namespace DynamicLoading
{
    class GenericCellOperations : IGenericCellOperations
    {
        public IEnumerable<ICellAccessor> EnumerateGenericCellAccessors(LocalMemoryStorage storage)
        {
            return Center.Leader.GenericCellOperations.SelectMany(cellOps => cellOps.EnumerateGenericCellAccessors(storage));
        }

        public IEnumerable<ICell> EnumerateGenericCells(LocalMemoryStorage storage)
        {
            return Center.Leader.GenericCellOperations.SelectMany(cellOps => cellOps.EnumerateGenericCells(storage));
        }

        public ICell LoadGenericCell(IKeyValueStore storage, long cellId)
        {
            // TODO: Use the records in disk to lookup which `GenericCellOperations` the cell with this id belongs to.
            throw new NotImplementedException();
        }

        public ICell LoadGenericCell(LocalMemoryStorage storage, long cellId)
        {
            throw new NotImplementedException();
        }

        public ICell NewGenericCell(string cellType)
        {
            int seg = Center.GetIntervalIndex.ByCellTypeName(cellType);
            return Center.Leader.GenericCellOperations[seg].NewGenericCell(cellType);
        }

        public ICell NewGenericCell(long cellId, string cellType)
        {
            int seg = Center.GetIntervalIndex.ByCellTypeName(cellType);
            return Center.Leader.GenericCellOperations[seg].NewGenericCell(cellId, cellType);
        }

        public ICell NewGenericCell(string cellType, string content)
        {
            int seg = Center.GetIntervalIndex.ByCellTypeName(cellType);
            return Center.Leader.GenericCellOperations[seg].NewGenericCell(cellType, content);
        }

        public void SaveGenericCell(IKeyValueStore storage, ICell cell)
        {
            int seg = Center.GetIntervalIndex.ByCellTypeID(cell.CellType);
            Center.Leader.GenericCellOperations[seg].SaveGenericCell(storage, cell);
        }

        public void SaveGenericCell(IKeyValueStore storage, long cellId, ICell cell)
        {
            int seg = Center.GetIntervalIndex.ByCellTypeID(cell.CellType);
            Center.Leader.GenericCellOperations[seg].SaveGenericCell(storage, cellId, cell);
        }

        public void SaveGenericCell(LocalMemoryStorage storage, CellAccessOptions writeAheadLogOptions, ICell cell)
        {
            int seg = Center.GetIntervalIndex.ByCellTypeID(cell.CellType);
            Center.Leader.GenericCellOperations[seg].SaveGenericCell(storage, writeAheadLogOptions, cell);
        }

        public void SaveGenericCell(LocalMemoryStorage storage, CellAccessOptions writeAheadLogOptions, long cellId, ICell cell)
        {
            int seg = Center.GetIntervalIndex.ByCellTypeID(cell.CellType);
            Center.Leader.GenericCellOperations[seg].SaveGenericCell(storage, writeAheadLogOptions, cellId, cell);
        }

        public ICellAccessor UseGenericCell(LocalMemoryStorage storage, long cellId)
        {
            throw new NotImplementedException();
        }

        public ICellAccessor UseGenericCell(LocalMemoryStorage storage, long cellId, CellAccessOptions options)
        {
            throw new NotImplementedException();
        }

        public ICellAccessor UseGenericCell(LocalMemoryStorage storage, long cellId, CellAccessOptions options, string cellType)
        {
            int seg = Center.GetIntervalIndex.ByCellTypeName(cellType);
            return Center.Leader.GenericCellOperations[seg].UseGenericCell(storage, cellId, options, cellType);
        }
    }
}
