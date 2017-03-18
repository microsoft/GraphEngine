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
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
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
        #region LocalMemoryStorage Save operations
        public void SaveGenericCell(Trinity.Storage.LocalMemoryStorage storage, ICell cell)
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
        public void SaveGenericCell(Trinity.Storage.LocalMemoryStorage storage, long cellId, ICell cell)
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
                )::");
for (size_t iterator_1 = 0; iterator_1 < (node->cellList)->size();++iterator_1)
{
source->append(R"::(
                case CellType.)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(:
                    var )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_accessor = new )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor(cellPtr);
                    var )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_cell = ()::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::())::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_accessor;
                    storage.ReleaseCellLock(cellId, entryIndex);
                    )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_cell.CellID = cellId;
                    return )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_cell;
                    break;
                )::");
}
source->append(R"::(
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
                    break;
                )::");
}
source->append(R"::(
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
                    break;
                )::");
}
source->append(R"::(
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
                )::");
for (size_t iterator_1 = 0; iterator_1 < (node->cellList)->size();++iterator_1)
{
source->append(R"::(
                case CellType.)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(:
                    return )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor.New(CellId, cellPtr, entryIndex, CellAccessOptions.ThrowExceptionOnCellNotFound);
                )::");
}
source->append(R"::(
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
        /// <returns>A <see cref=")::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(.GenericCellAccessor"/> instance.</returns>
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
                            throw new ArgumentException("CellAccessOptions.CreateNewO)::");
source->append(R"::(nCellNotFound is not valid for UseGenericCell. Cannot determine new cell type.", "options");
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
                )::");
for (size_t iterator_1 = 0; iterator_1 < (node->cellList)->size();++iterator_1)
{
source->append(R"::(
                case CellType.)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(:
                    return )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor.New(CellId, cellPtr, entryIndex, options);
                )::");
}
source->append(R"::(
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
source->append(R"::(_Accessor.AllocIterativeAccessor(cellInfo);
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
source->append(R"::(_Accessor.AllocIterativeAccessor(cellInfo);
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
 )::");
source->append(R"::(                       throw new Exception("Cannot access the cell. Error code: " + err.ToString());
                }
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
                    fixed (byte* )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_ptr = buff)
                    {
                        )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_accessor = new )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor()::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_ptr);
                        )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_accessor.CellID = cellId;
                        return ()::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::())::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_accessor;
                    }
                    break;
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
