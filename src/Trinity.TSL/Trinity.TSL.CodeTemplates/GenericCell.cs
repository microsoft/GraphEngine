#pragma warning disable 0162 // disable the "unreachable code" warning
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
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
        #region LocalMemoryStorage operations
        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

            try
            {
                var accessor = UseGenericCell(cellId, cellPtr, entryIndex, type);
                var cell = accessor.Deserialize();
                accessor.Dispose();
                return cell;
            }
            catch (Exception ex)
            {
                storage.ReleaseCellLock(cellId, entryIndex);
                ExceptionDispatchInfo.Capture(ex).Throw();
                // should not reach here
                throw;
            }
        }
        #endregion

        #region New operations
        /// <inheritdoc/>
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
                /*END*/
            }
            /* Should not reach here */
            return null;
        }

        /// <inheritdoc/>
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
                /*END*/
            }
            /* Should not reach here */
            return null;
        }
        #endregion

        #region LocalMemoryStorage Use operations
        /// <inheritdoc/>
        public unsafe ICellAccessor UseGenericCell(Trinity.Storage.LocalMemoryStorage storage, long cellId)
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
                return t_cell_name_Accessor._get()._Setup(cellId, cellPtr, entryIndex, CellAccessOptions.ThrowExceptionOnCellNotFound);
                /*END*/
                default:
                storage.ReleaseCellLock(cellId, entryIndex);
                throw new CellTypeNotMatchException("Cannot determine cell type.");
            }
        }

        /// <inheritdoc/>
        public unsafe ICellAccessor UseGenericCell(Trinity.Storage.LocalMemoryStorage storage, long cellId, CellAccessOptions options)
        {
            ushort type;
            int    size;
            byte*  cellPtr;
            int    entryIndex;

            var err = storage.GetLockedCellInfo(cellId, out size, out type, out cellPtr, out entryIndex);
            switch (err)
            {
                case TrinityErrorCode.E_SUCCESS:
                break;
                case TrinityErrorCode.E_CELL_NOT_FOUND:
                    {
                        if ((options & CellAccessOptions.ThrowExceptionOnCellNotFound) != 0)
                        {
                            Throw.cell_not_found(cellId);
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
                            Throw.cell_not_found(cellId);
                        }
                        break;
                    }
                default:
                throw new CellNotFoundException("Cannot access the cell.");
            }

            try
            {
                return UseGenericCell(cellId, cellPtr, entryIndex, type, options);
            }
            catch (Exception ex)
            {
                storage.ReleaseCellLock(cellId, entryIndex);
                ExceptionDispatchInfo.Capture(ex).Throw();
                // should never reach here
                throw;
            }
        }

        /// <inheritdoc/>
        public unsafe ICellAccessor UseGenericCell(Trinity.Storage.LocalMemoryStorage storage, long cellId, CellAccessOptions options, string cellType)
        {
            switch (cellType)
            {
                /*FOREACH*/
                case "t_cell_name": return t_cell_name_Accessor._get()._Lock(cellId, options);
                /*END*/
                default:
                Throw.invalid_cell_type();
                return null;// should not reach here
            }
        }
        #endregion

        #region LocalMemoryStorage Enumerate operations

        /// <inheritdoc/>
        public IEnumerable<ICell> EnumerateGenericCells(LocalMemoryStorage storage)
        {
            //TODO no tx
            foreach (var cellInfo in Global.LocalStorage)
            {
                switch ((CellType)cellInfo.CellType)
                {
                    /*FOREACH*/
                    case CellType.t_cell_name:
                        {
                            var t_cell_name_accessor = t_cell_name_Accessor.AllocIterativeAccessor(cellInfo, null);
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

        /// <inheritdoc/>
        public IEnumerable<ICellAccessor> EnumerateGenericCellAccessors(LocalMemoryStorage storage)
        {
            foreach (var cellInfo in Global.LocalStorage)
            {
                switch ((CellType)cellInfo.CellType)
                {
                    /*FOREACH*/
                    case CellType.t_cell_name:
                        {
                            var t_cell_name_accessor = t_cell_name_Accessor.AllocIterativeAccessor(cellInfo, null);
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

        #region IKeyValueStore operations

        /// <inheritdoc/>
        public Task SaveGenericCellAsync(IKeyValueStore storage, ICell cell)
        {
            switch ((CellType)cell.CellType)
            {
                /*FOREACH*/
                case CellType.t_cell_name:
                return storage.Savet_cell_nameAsync((t_cell_name)cell);
                break;
                /*END*/
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task SaveGenericCellAsync(IKeyValueStore storage, long cellId, ICell cell)
        {
            switch ((CellType)cell.CellType)
            {
                /*FOREACH*/
                case CellType.t_cell_name:
                storage.Savet_cell_nameAsync(cellId, (t_cell_name)cell);
                break;
                /*END*/
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public unsafe Task<ICell> LoadGenericCellAsync(IKeyValueStore storage, long cellId)
        {
            return storage.LoadCellAsync(cellId)
                          .ContinueWith(
                                t =>
                                {
                                    var result = t.Result;
                                    var err = result.ErrorCode;
                                    var buff = result.CellBuff;
                                    var type = result.CellType;
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

                                    fixed (byte* p = buff)
                                    {
                                        var accessor = UseGenericCell(cellId, p, -1, type);
                                        return accessor.Deserialize();
                                    }
                                },
                                TaskContinuationOptions.ExecuteSynchronously);
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe ICellAccessor UseGenericCell(long cellId, byte* cellPtr, int entryIndex, ushort cellType)
         => UseGenericCell(cellId, cellPtr, entryIndex, cellType, CellAccessOptions.ThrowExceptionOnCellNotFound);


        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe ICellAccessor UseGenericCell(long cellId, byte* cellBuffer, int entryIndex, ushort cellType, CellAccessOptions options)
        {
            switch ((CellType)cellType)
            {
                /*FOREACH*/
                case CellType.t_cell_name:
                return t_cell_name_Accessor._get()._Setup(cellId, cellBuffer, entryIndex, options);
                /*END*/
                default:
                throw new CellTypeNotMatchException("Cannot determine cell type.");
            }
        }

        #endregion
    }
}
