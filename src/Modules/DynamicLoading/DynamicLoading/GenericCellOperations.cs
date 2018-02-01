using System;
using System.Collections.Generic;
using System.Text;
using Trinity.Storage;
using Trinity.Extension;
using Trinity.TSL.Lib;

namespace DynamicLoading
{
    class GenericCellOperations : IGenericCellOperations
    {
        public IEnumerable<ICellAccessor> EnumerateGenericCellAccessors(LocalMemoryStorage storage)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ICell> EnumerateGenericCells(LocalMemoryStorage storage)
        {
            throw new NotImplementedException();
        }

        public ICell LoadGenericCell(IKeyValueStore storage, long cellId)
        {
            throw new NotImplementedException();
        }

        public ICell LoadGenericCell(LocalMemoryStorage storage, long cellId)
        {
            throw new NotImplementedException();
        }

        public ICell NewGenericCell(string cellType)
        {
            throw new NotImplementedException();
        }

        public ICell NewGenericCell(long cellId, string cellType)
        {
            throw new NotImplementedException();
        }

        public ICell NewGenericCell(string cellType, string content)
        {
            throw new NotImplementedException();
        }

        public void SaveGenericCell(IKeyValueStore storage, ICell cell)
        {
            throw new NotImplementedException();
        }

        public void SaveGenericCell(IKeyValueStore storage, long cellId, ICell cell)
        {
            throw new NotImplementedException();
        }

        public void SaveGenericCell(LocalMemoryStorage storage, CellAccessOptions writeAheadLogOptions, ICell cell)
        {
            throw new NotImplementedException();
        }

        public void SaveGenericCell(LocalMemoryStorage storage, CellAccessOptions writeAheadLogOptions, long cellId, ICell cell)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }
}
