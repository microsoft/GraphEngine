#pragma warning disable 162,168,649,660,661,1522

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Trinity;
using Trinity.Storage;
using Trinity.TSL;
using Trinity.TSL.Lib;
namespace CellAssembly
{
    /// <summary>
    /// Exposes Load/Save/New operations of <see cref="Trinity.Storage.ICell"/> and Use operation on <see cref="Trinity.Storage.ICellAccessor"/> on <see cref="Trinity.Storage.LocalMemoryStorage"/> and <see cref="Trinity.Storage.MemoryCloud"/>.
    /// </summary>
    internal class GenericCellOperations : IGenericCellOperations
    {
        #region LocalMemoryStorage operations
        public void SaveGenericCell(Trinity.Storage.LocalMemoryStorage storage, CellAccessOptions writeAheadLogOptions, ICell cell)
        {
            switch ((CellType)cell.CellType)
            {
                
                case CellType.C1:
                    storage.SaveC1(writeAheadLogOptions, (C1)cell);
                    break;
                
            }
        }
        public void SaveGenericCell(Trinity.Storage.LocalMemoryStorage storage, CellAccessOptions writeAheadLogOptions, long cellId, ICell cell)
        {
            switch ((CellType)cell.CellType)
            {
                
                case CellType.C1:
                    storage.SaveC1(writeAheadLogOptions, cellId, (C1)cell);
                    break;
                
            }
        }
        public unsafe ICell LoadGenericCell(Trinity.Storage.LocalMemoryStorage storage, long cellId)
        {
            ushort type;
            int    size;
            byte*  cellPtr;
            int    entryIndex;
            var err = storage.GetLockedCellInfo(cellId, out size, out type, out cellPtr, out entryIndex);
            if (err != TrinityErrorCode.E_SUCCESS)
            {
                throw new CellNotFoundException("Cannot access the cell.");
            }
            switch ((CellType)type)
            {
                
                case CellType.C1:
                    var C1_accessor = new C1_Accessor(cellPtr);
                    var C1_cell = (C1)C1_accessor;
                    storage.ReleaseCellLock(cellId, entryIndex);
                    C1_cell.CellID = cellId;
                    return C1_cell;
                    break;
                
                default:
                    throw new CellTypeNotMatchException("Cannot determine cell type.");
            }
        }
        #endregion
        #region New operations
        public ICell NewGenericCell(string cellType)
        {
            CellType type;
            if (!StorageSchema.cellTypeLookupTable.TryGetValue(cellType, out type))
                Throw.invalid_cell_type();
            switch (type)
            {
                
                case global::CellAssembly.CellType.C1:
                    return new C1();
                    break;
                
            }
            /* Should not reach here */
            return null;
        }
        public ICell NewGenericCell(long cellId, string cellType)
        {
            CellType type;
            if (!StorageSchema.cellTypeLookupTable.TryGetValue(cellType, out type))
                Throw.invalid_cell_type();
            switch (type)
            {
                
                case global::CellAssembly.CellType.C1:
                    return new C1(cell_id: cellId);
                    break;
                
            }
            /* Should not reach here */
            return null;
        }
        public ICell NewGenericCell(string cellType, string content)
        {
            CellType type;
            if (!StorageSchema.cellTypeLookupTable.TryGetValue(cellType, out type))
                Throw.invalid_cell_type();
            switch (type)
            {
                
                case global::CellAssembly.CellType.C1:
                    return C1.Parse(content);
                    break;
                
            }
            /* Should not reach here */
            return null;
        }
        #endregion
        #region LocalMemoryStorage Use operations
        public unsafe ICellAccessor UseGenericCell(Trinity.Storage.LocalMemoryStorage storage, long CellId)
        {
            ushort type;
            int    size;
            byte*  cellPtr;
            int    entryIndex;
            var err = storage.GetLockedCellInfo(CellId, out size, out type, out cellPtr, out entryIndex);
            if (err != TrinityErrorCode.E_SUCCESS)
            {
                throw new CellNotFoundException("Cannot access the cell.");
            }
            switch ((CellType)type)
            {
                
                case CellType.C1:
                    return C1_Accessor.New(CellId, cellPtr, entryIndex, CellAccessOptions.ThrowExceptionOnCellNotFound);
                
                default:
                    storage.ReleaseCellLock(CellId, entryIndex);
                    throw new CellTypeNotMatchException("Cannot determine cell type.");
             }
        }
        /// <summary>
        /// Allocate a generic cell accessor on the specified cell.
        /// If <c><see cref="Trinity.TrinityConfig.ReadOnly"/> == false</c>,
        /// on calling this method, it attempts to acquire the lock of the cell,
        /// and blocks until it gets the lock.
        /// </summary>
        /// <param name="storage">A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="CellId">The id of the specified cell.</param>
        /// <param name="options">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <returns>A <see cref="CellAssembly.GenericCellAccessor"/> instance.</returns>
        public unsafe ICellAccessor UseGenericCell(Trinity.Storage.LocalMemoryStorage storage, long CellId, CellAccessOptions options)
        {
            ushort type;
            int    size;
            byte*  cellPtr;
            int    entryIndex;
            var err = storage.GetLockedCellInfo(CellId, out size, out type, out cellPtr, out entryIndex);
            switch (err)
            {
                case TrinityErrorCode.E_SUCCESS:
                    break;
                case TrinityErrorCode.E_CELL_NOT_FOUND:
                    {
                        if ((options & CellAccessOptions.ThrowExceptionOnCellNotFound) != 0)
                        {
                            Throw.cell_not_found(CellId);
                        }
                        else if ((options & CellAccessOptions.CreateNewOnCellNotFound) != 0)
                        {
                            throw new ArgumentException("CellAccessOptions.CreateNewOnCellNotFound is not valid for this method. Cannot determine new cell type.", "options");
                        }
                        else if ((options & CellAccessOptions.ReturnNullOnCellNotFound) != 0)
                        {
                            return null;
                        }
                        else
                        {
                            Throw.cell_not_found(CellId);
                        }
                        break;
                    }
                default:
                    throw new CellNotFoundException("Cannot access the cell.");
            }
            switch ((CellType)type)
            {
                
                case CellType.C1:
                    return C1_Accessor.New(CellId, cellPtr, entryIndex, options);
                
                default:
                    storage.ReleaseCellLock(CellId, entryIndex);
                    throw new CellTypeNotMatchException("Cannot determine cell type.");
             };
        }
        /// <summary>
        /// Allocate a generic cell accessor on the specified cell.
        /// If <c><see cref="Trinity.TrinityConfig.ReadOnly"/> == false</c>,
        /// on calling this method, it attempts to acquire the lock of the cell,
        /// and blocks until it gets the lock.
        /// </summary>
        /// <param name="storage">A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="CellId">The id of the specified cell.</param>
        /// <param name="options">Cell access options.</param>
        /// <param name="cellType">Specifies the type of cell to be created.</param>
        /// <returns>A <see cref="CellAssembly.GenericCellAccessor"/> instance.</returns>
        public unsafe ICellAccessor UseGenericCell(Trinity.Storage.LocalMemoryStorage storage, long CellId, CellAccessOptions options, string cellType)
        {
            switch (cellType)
            {
                
                case "C1": return C1_Accessor.New(CellId, options);
                
                default:
                    Throw.invalid_cell_type();
                    return null;
            }
        }
        #endregion
        #region LocalMemoryStorage Enumerate operations
        public IEnumerable<ICell> EnumerateGenericCells(LocalMemoryStorage storage)
        {
            foreach (var cellInfo in Global.LocalStorage)
            {
                switch ((CellType)cellInfo.CellType)
                {
                    
                    case CellType.C1:
                        {
                            var C1_accessor = C1_Accessor.AllocIterativeAccessor(cellInfo);
                            var C1_cell     = (C1)C1_accessor;
                            C1_accessor.Dispose();
                            yield return C1_cell;
                            break;
                        }
                    
                    default:
                        continue;
                }
            }
            yield break;
        }
        public IEnumerable<ICellAccessor> EnumerateGenericCellAccessors(LocalMemoryStorage storage)
        {
            foreach (var cellInfo in Global.LocalStorage)
            {
                switch ((CellType)cellInfo.CellType)
                {
                    
                    case CellType.C1:
                        {
                            var C1_accessor = C1_Accessor.AllocIterativeAccessor(cellInfo);
                            yield return C1_accessor;
                            C1_accessor.Dispose();
                            break;
                        }
                    
                    default:
                        continue;
                }
            }
            yield break;
        }
        #endregion
        #region IKeyValueStore operations
        /// <summary>
        /// Adds a new cell to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists.
        /// Note that the generic cell will be saved as a strongly typed cell. It can then be loaded into either a strongly-typed cell or a generic cell.
        /// </summary>
        /// <param name="storage">A <see cref="IKeyValueStore"/> instance.</param>
        /// <param name="cell">The cell to be saved.</param>
        public void SaveGenericCell(IKeyValueStore storage, ICell cell)
        {
            switch ((CellType)cell.CellType)
            {
                
                case CellType.C1:
                    storage.SaveC1((C1)cell);
                    break;
                
            }
        }
        public void SaveGenericCell(IKeyValueStore storage, long cellId, ICell cell)
        {
            switch ((CellType)cell.CellType)
            {
                
                case CellType.C1:
                    storage.SaveC1(cellId, (C1)cell);
                    break;
                
            }
        }
        /// <summary>
        /// Loads the content of the cell with the specified cell Id.
        /// </summary>
        /// <param name="storage">A <see cref="IKeyValueStore"/> instance.</param>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <returns></returns>
        public unsafe ICell LoadGenericCell(IKeyValueStore storage, long cellId)
        {
            ushort type;
            byte[] buff;
            var err = storage.LoadCell(cellId, out buff, out type);
            if (err != TrinityErrorCode.E_SUCCESS)
            {
                switch (err)
                {
                    case TrinityErrorCode.E_CELL_NOT_FOUND:
                        throw new CellNotFoundException("Cannot access the cell.");
                    case TrinityErrorCode.E_NETWORK_SEND_FAILURE:
                        throw new System.IO.IOException("Network error while accessing the cell.");
                    default:
                        throw new Exception("Cannot access the cell. Error code: " + err.ToString());
                }
            }
            switch ((CellType)type)
            {
                
                case CellType.C1:
                    fixed (byte* C1_ptr = buff)
                    {
                        C1_Accessor C1_accessor = new C1_Accessor(C1_ptr);
                        C1_accessor.CellID = cellId;
                        return (C1)C1_accessor;
                    }
                    break;
                
                default:
                    throw new CellTypeNotMatchException("Cannot determine cell type.");
            }
        }
        #endregion
    }
}

#pragma warning restore 162,168,649,660,661,1522
