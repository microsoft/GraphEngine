#pragma warning disable 0162 // disable the "unreachable code" warning
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

/*MAP_VAR("t_Namespace", "Trinity::Codegen::GetNamespace()")*/
namespace t_Namespace
{
    [TARGET("NTSL")]
    [MAP_LIST("t_cell", "node->cellList")]
    [MAP_VAR("t_cell", "")]
    [MAP_VAR("t_cell_name", "name", MemberOf = "t_cell")]
    /// <summary>
    /// Exposes Load/Save/New operations of <see cref="Trinity.Storage.ICell"/> and Use operation on <see cref="Trinity.Storage.ICellAccessor"/> on <see cref="Trinity.Storage.LocalMemoryStorage"/> and <see cref="Trinity.Storage.MemoryCloud"/>.
    /// </summary>
    internal class GenericCellOperations : IGenericCellOperations
    {
        #region LocalMemoryStorage Save operations
        public void SaveGenericCell(Trinity.Storage.LocalMemoryStorage storage, ICell cell)
        {
            switch ((CellType)cell.CellType)
            {
                /*FOREACH*/
                case CellType.t_cell_name:
                    storage.Savet_cell_name((t_cell_name)cell);
                    break;
                /*END*/
            }
        }

        public void SaveGenericCell(Trinity.Storage.LocalMemoryStorage storage, CellAccessOptions writeAheadLogOptions, ICell cell)
        {
            switch ((CellType)cell.CellType)
            {
                /*FOREACH*/
                case CellType.t_cell_name:
                    storage.Savet_cell_name(writeAheadLogOptions, (t_cell_name)cell);
                    break;
                /*END*/
            }
        }

        public void SaveGenericCell(Trinity.Storage.LocalMemoryStorage storage, long cellId, ICell cell)
        {
            switch ((CellType)cell.CellType)
            {
                /*FOREACH*/
                case CellType.t_cell_name:
                    storage.Savet_cell_name(cellId, (t_cell_name)cell);
                    break;
                /*END*/
            }
        }

        public void SaveGenericCell(Trinity.Storage.LocalMemoryStorage storage, CellAccessOptions writeAheadLogOptions, long cellId, ICell cell)
        {
            switch ((CellType)cell.CellType)
            {
                /*FOREACH*/
                case CellType.t_cell_name:
                    storage.Savet_cell_name(writeAheadLogOptions, cellId, (t_cell_name)cell);
                    break;
                /*END*/
            }
        }
        #endregion

        #region LocalMemoryStorage Load operations
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
                /*FOREACH*/
                case CellType.t_cell_name:
                    var t_cell_name_accessor = new t_cell_name_Accessor(cellPtr);
                    var t_cell_name_cell = (t_cell_name)t_cell_name_accessor;
                    storage.ReleaseCellLock(cellId, entryIndex);
                    t_cell_name_cell.CellID = cellId;
                    return t_cell_name_cell;
                    break;
                /*END*/
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
                /*FOREACH*/
                case global::t_Namespace.CellType.t_cell_name:
                    return new t_cell_name();
                    break;
                /*END*/
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
                /*FOREACH*/
                case global::t_Namespace.CellType.t_cell_name:
                    return new t_cell_name(cell_id: cellId);
                    break;
                /*END*/
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
                /*FOREACH*/
                case global::t_Namespace.CellType.t_cell_name:
                    return t_cell_name.Parse(content);
                    break;
                /*END*/
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
                /*FOREACH*/
                case CellType.t_cell_name:
                    return t_cell_name_Accessor.New(CellId, cellPtr, entryIndex, CellAccessOptions.ThrowExceptionOnCellNotFound);
                /*END*/
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
        /// <returns>A <see cref="t_Namespace.GenericCellAccessor"/> instance.</returns>
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
                            throw new ArgumentException("CellAccessOptions.CreateNewOnCellNotFound is not valid for UseGenericCell. Cannot determine new cell type.", "options");
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
                /*FOREACH*/
                case CellType.t_cell_name:
                    return t_cell_name_Accessor.New(CellId, cellPtr, entryIndex, options);
                /*END*/
                default:
                    storage.ReleaseCellLock(CellId, entryIndex);
                    throw new CellTypeNotMatchException("Cannot determine cell type.");
             };
        }
        #endregion

        #region LocalMemoryStorage Enumerate operations
        public IEnumerable<ICell> EnumerateGenericCells(LocalMemoryStorage storage)
        {
            foreach (var cellInfo in Global.LocalStorage)
            {
                switch ((CellType)cellInfo.CellType)
                {
                    /*FOREACH*/
                    case CellType.t_cell_name:
                        {
                            var t_cell_name_accessor = t_cell_name_Accessor.AllocIterativeAccessor(cellInfo);
                            var t_cell_name_cell     = (t_cell_name)t_cell_name_accessor;
                            t_cell_name_accessor.Dispose();
                            yield return t_cell_name_cell;
                            break;
                        }
                    /*END*/
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
                    /*FOREACH*/
                    case CellType.t_cell_name:
                        {
                            var t_cell_name_accessor = t_cell_name_Accessor.AllocIterativeAccessor(cellInfo);
                            yield return t_cell_name_accessor;
                            t_cell_name_accessor.Dispose();
                            break;
                        }
                    /*END*/
                    default:
                        continue;
                }
            }

            yield break;
        }
        #endregion

        #region MemoryCloud operations
        /// <summary>
        /// Adds a new cell to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists.
        /// Note that the generic cell will be saved as a strongly typed cell. It can then be loaded into either a strongly-typed cell or a generic cell.
        /// </summary>
        /// <param name="storage">A <see cref="Trinity.Storage.MemoryCloud"/> instance.</param>
        /// <param name="cell">The cell to be saved.</param>

        public void SaveGenericCell(Trinity.Storage.MemoryCloud storage, ICell cell)
        {
            switch ((CellType)cell.CellType)
            {
                /*FOREACH*/
                case CellType.t_cell_name:
                    storage.Savet_cell_name((t_cell_name)cell);
                    break;
                /*END*/
            }
        }

        /// <summary>
        /// Loads the content of the cell with the specified cell Id.
        /// </summary>
        /// <param name="storage">A <see cref="Trinity.Storage.MemoryCloud"/> instance.</param>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <returns></returns>
        public unsafe ICell LoadGenericCell(Trinity.Storage.MemoryCloud storage, long cellId)
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
                /*FOREACH*/
                case CellType.t_cell_name:
                    fixed (byte* t_cell_name_ptr = buff)
                    {
                        t_cell_name_Accessor/*_*/t_cell_name_accessor = new t_cell_name_Accessor(t_cell_name_ptr);
                        t_cell_name_accessor.CellID = cellId;
                        return (t_cell_name)t_cell_name_accessor;
                    }
                    break;
                /*END*/
                default:
                    throw new CellTypeNotMatchException("Cannot determine cell type.");
            }
        }

        #endregion
    }
}
