#include "common.h"
#include <string>
#include "SyntaxNode.h"

using std::string;

namespace Trinity
{
    namespace Codegen
    {
        string* 
GenericCell(
NTSL* node)
        {
            string* source = new string();
            
source->append(R"::(
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using Trinity;
using Trinity.Storage;
using Trinity.TSL;
using Trinity.TSL.Lib;
namespace )::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(
{
    /// <summary>
    /// Exposes Load/Save/New operations of <see cref="Trinity.Storage.ICell"/> and Use operation on <see cref="Trinity.Storage.ICellAccessor"/> on <see cref="Trinity.Storage.LocalMemoryStorage"/> and <see cref="Trinity.Storage.MemoryCloud"/>.
    /// </summary>
    internal class GenericCellOperations : IGenericCellOperations
    {
        #region LocalMemoryStorage operations
        /// <inheritdoc/>
        public void SaveGenericCell(Trinity.Storage.LocalMemoryStorage storage, CellAccessOptions writeAheadLogOptions, ICell cell)
        {
            switch ((CellType)cell.CellType)
            {
                )::");
for (size_t iterator_1 = 0; iterator_1 < (node->cellList)->size();++iterator_1)
{
source->append(R"::(
                case CellType.)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(:
                storage.Save)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::((writeAheadLogOptions, ()::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::()cell);
                break;
                )::");
}
source->append(R"::(
            }
        }
        /// <inheritdoc/>
        public void SaveGenericCell(Trinity.Storage.LocalMemoryStorage storage, CellAccessOptions writeAheadLogOptions, long cellId, ICell cell)
        {
            switch ((CellType)cell.CellType)
            {
                )::");
for (size_t iterator_1 = 0; iterator_1 < (node->cellList)->size();++iterator_1)
{
source->append(R"::(
                case CellType.)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(:
                storage.Save)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::((writeAheadLogOptions, cellId, ()::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::()cell);
                break;
                )::");
}
source->append(R"::(
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
                throw;
            }
        }
       )::");
source->append(R"::( #endregion
        #region New operations
        /// <inheritdoc/>
        public ICell NewGenericCell(string cellType)
        {
            CellType type;
            if (!StorageSchema.cellTypeLookupTable.TryGetValue(cellType, out type))
                Throw.invalid_cell_type();
            switch (type)
            {
                )::");
for (size_t iterator_1 = 0; iterator_1 < (node->cellList)->size();++iterator_1)
{
source->append(R"::(
                case global::)::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(.CellType.)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(:
                return new )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(();
                break;
                )::");
}
source->append(R"::(
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
                )::");
for (size_t iterator_1 = 0; iterator_1 < (node->cellList)->size();++iterator_1)
{
source->append(R"::(
                case global::)::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(.CellType.)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(:
                return new )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::((cell_id: cellId);
                )::");
}
source->append(R"::(
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
                )::");
for (size_t iterator_1 = 0; iterator_1 < (node->cellList)->size();++iterator_1)
{
source->append(R"::(
                case global::)::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(.CellType.)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(:
                return )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(.Parse(content);
                )::");
}
source->append(R"::(
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
                )::");
for (size_t iterator_1 = 0; iterator_1 < (node->cellList)->size();++iterator_1)
{
source->append(R"::(
                case CellType.)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(:
                return )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor._get()._Setup(cellId, cellPtr, entryIndex, CellAccessOptions.ThrowExceptionOnCellNotFound, null);
                )::");
}
source->append(R"::(
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
                        els)::");
source->append(R"::(e if ((options & CellAccessOptions.CreateNewOnCellNotFound) != 0)
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
                storage.ReleaseCellLock()::");
source->append(R"::(cellId, entryIndex);
                ExceptionDispatchInfo.Capture(ex).Throw();
                throw;
            }
        }
        /// <inheritdoc/>
        public unsafe ICellAccessor UseGenericCell(Trinity.Storage.LocalMemoryStorage storage, long cellId, CellAccessOptions options, string cellType)
        {
            switch (cellType)
            {
                )::");
for (size_t iterator_1 = 0; iterator_1 < (node->cellList)->size();++iterator_1)
{
source->append(R"::(
                case ")::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(": return )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor._get()._Lock(cellId, options, null);
                )::");
}
source->append(R"::(
                default:
                Throw.invalid_cell_type();
                return null;
            }
        }
        #endregion
        #region LocalMemoryStorage Enumerate operations
        /// <inheritdoc/>
        public IEnumerable<ICell> EnumerateGenericCells(LocalMemoryStorage storage)
        {
            foreach (var cellInfo in Global.LocalStorage)
            {
                switch ((CellType)cellInfo.CellType)
                {
                    )::");
for (size_t iterator_1 = 0; iterator_1 < (node->cellList)->size();++iterator_1)
{
source->append(R"::(
                    case CellType.)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(:
                        {
                            var )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_accessor = )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor.AllocIterativeAccessor(cellInfo, null);
                            var )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_cell     = ()::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::())::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_accessor;
                            )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_accessor.Dispose();
                            yield return )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_cell;
                            break;
                        }
                    )::");
}
source->append(R"::(
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
                    )::");
for (size_t iterator_1 = 0; iterator_1 < (node->cellList)->size();++iterator_1)
{
source->append(R"::(
                    case CellType.)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(:
                        {
                            var )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_accessor = )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor.AllocIterativeAccessor(cellInfo, null);
                            yield return )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_accessor;
                            )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_accessor.Dispose();
                            break;
                        }
                    )::");
}
source->append(R"::(
                    default:
                    continue;
                }
            }
            yield break;
        }
        #endregion
        #region IKeyValueStore operations
        /// <inheritdoc/>
        public void SaveGenericCell(IKeyValueStore storage, ICell cell)
        {
            switch ((CellType)cell.CellType)
            {
                )::");
for (size_t iterator_1 = 0; iterator_1 < (node->cellList)->size();++iterator_1)
{
source->append(R"::(
                case CellType.)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(:
                storage.Save)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::((()::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::()cell);
                break;
                )::");
}
source->append(R"::(
            }
        }
        /// <inheritdoc/>
        public void SaveGenericCell(IKeyValueStore storage, long cellId, ICell cell)
        {
            switch ((CellType)cell.CellType)
            {
                )::");
for (size_t iterator_1 = 0; iterator_1 < (node->cellList)->size();++iterator_1)
{
source->append(R"::(
                case CellType.)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(:
                storage.Save)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::((cellId, ()::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::()cell);
                break;
                )::");
}
source->append(R"::(
            }
        }
        /// <inheritdoc/>
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
            fixed (byte* p = buff)
            {
                var accessor = UseGenericCell(cellId, p, -1, type);
                return accessor.Deserialize();
  )::");
source->append(R"::(          }
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
                )::");
for (size_t iterator_1 = 0; iterator_1 < (node->cellList)->size();++iterator_1)
{
source->append(R"::(
                case CellType.)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(:
                return )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor._get()._Setup(cellId, cellBuffer, entryIndex, options, null);
                )::");
}
source->append(R"::(
                default:
                throw new CellTypeNotMatchException("Cannot determine cell type.");
            }
        }
        #endregion
    }
}
)::");

            return source;
        }
    }
}
