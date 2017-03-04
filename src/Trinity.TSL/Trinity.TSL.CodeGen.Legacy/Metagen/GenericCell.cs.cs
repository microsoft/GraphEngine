using System.Text;
using System.Collections.Generic;

namespace Trinity.TSL.CodeTemplates
{
    internal partial class SourceFiles
    {
        internal static string 
GenericCell(
NTSL node)
        {
            StringBuilder source = new StringBuilder();
            
source.Append(@"
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
namespace ");
source.Append(Codegen.GetString(Trinity::Codegen::GetNamespace()));
source.Append(@"
{
    /// <summary>
    /// Exposes Load/Save/New operations of <see cref=""Trinity.Storage.ICell""/> and Use operation on <see cref=""Trinity.Storage.ICellAccessor""/> on <see cref=""Trinity.Storage.LocalMemoryStorage""/> and <see cref=""Trinity.Storage.MemoryCloud""/>.
    /// </summary>
    internal class GenericCellOperations : IGenericCellOperations
    {
        #region LocalMemoryStorage Save operations
        public void SaveGenericCell(Trinity.Storage.LocalMemoryStorage storage, ICell cell)
        {
            switch ((CellType)cell.CellType)
            {
                ");
for (int iterator_1 = 0; iterator_1 < (node->cellList).Count;++iterator_1)
{
source.Append(@"
                case CellType.");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@":
                    storage.Save");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"((");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@")cell);
                    break;
                ");
}
source.Append(@"
            }
        }
        public void SaveGenericCell(Trinity.Storage.LocalMemoryStorage storage, CellAccessOptions writeAheadLogOptions, ICell cell)
        {
            switch ((CellType)cell.CellType)
            {
                ");
for (int iterator_1 = 0; iterator_1 < (node->cellList).Count;++iterator_1)
{
source.Append(@"
                case CellType.");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@":
                    storage.Save");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"(writeAheadLogOptions, (");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@")cell);
                    break;
                ");
}
source.Append(@"
            }
        }
        public void SaveGenericCell(Trinity.Storage.LocalMemoryStorage storage, long cellId, ICell cell)
        {
            switch ((CellType)cell.CellType)
            {
                ");
for (int iterator_1 = 0; iterator_1 < (node->cellList).Count;++iterator_1)
{
source.Append(@"
                case CellType.");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@":
                    storage.Save");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"(cellId, (");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@")cell);
                    break;
                ");
}
source.Append(@"
            }
        }
        public void SaveGenericCell(Trinity.Storage.LocalMemoryStorage storage, CellAccessOptions writeAheadLogOptions, long cellId, ICell cell)
        {
            switch ((CellType)cell.CellType)
            {
                ");
for (int iterator_1 = 0; iterator_1 < (node->cellList).Count;++iterator_1)
{
source.Append(@"
                case CellType.");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@":
                    storage.Save");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"(writeAheadLogOptions, cellId, (");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@")cell);
                    break;
                ");
}
source.Append(@"
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
                throw new CellNotFoundException(""Cannot access the cell."");
            }
            switch ((CellType)type)
            {
                ");
for (int iterator_1 = 0; iterator_1 < (node->cellList).Count;++iterator_1)
{
source.Append(@"
                case CellType.");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@":
                    var ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_accessor = new ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Accessor(cellPtr);
                    var ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_cell = (");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@")");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_accessor;
                    storage.ReleaseCellLock(cellId, entryIndex);
                    ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_cell.CellID = cellId;
                    return ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_cell;
                    break;
                ");
}
source.Append(@"
                default:
                    throw new CellTypeNotMatchException(""Cannot determine cell type."");
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
                ");
for (int iterator_1 = 0; iterator_1 < (node->cellList).Count;++iterator_1)
{
source.Append(@"
                case global::");
source.Append(Codegen.GetString(Trinity::Codegen::GetNamespace()));
source.Append(@".CellType.");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@":
                    return new ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"();
                    break;
                ");
}
source.Append(@"
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
                ");
for (int iterator_1 = 0; iterator_1 < (node->cellList).Count;++iterator_1)
{
source.Append(@"
                case global::");
source.Append(Codegen.GetString(Trinity::Codegen::GetNamespace()));
source.Append(@".CellType.");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@":
                    return new ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"(cell_id: cellId);
                    break;
                ");
}
source.Append(@"
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
                ");
for (int iterator_1 = 0; iterator_1 < (node->cellList).Count;++iterator_1)
{
source.Append(@"
                case global::");
source.Append(Codegen.GetString(Trinity::Codegen::GetNamespace()));
source.Append(@".CellType.");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@":
                    return ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@".Parse(content);
                    break;
                ");
}
source.Append(@"
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
                throw new CellNotFoundException(""Cannot access the cell."");
            }
            switch ((CellType)type)
            {
                ");
for (int iterator_1 = 0; iterator_1 < (node->cellList).Count;++iterator_1)
{
source.Append(@"
                case CellType.");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@":
                    return ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Accessor.New(CellId, cellPtr, entryIndex, CellAccessOptions.ThrowExceptionOnCellNotFound);
                ");
}
source.Append(@"
                default:
                    storage.ReleaseCellLock(CellId, entryIndex);
                    throw new CellTypeNotMatchException(""Cannot determine cell type."");
             }
        }
        /// <summary>
        /// Allocate a generic cell accessor on the specified cell.
        /// If <c><see cref=""Trinity.TrinityConfig.ReadOnly""/> == false</c>,
        /// on calling this method, it attempts to acquire the lock of the cell,
        /// and blocks until it gets the lock.
        /// </summary>
        /// <param name=""storage"">A <see cref=""Trinity.Storage.LocalMemoryStorage""/> instance.</param>
        /// <param name=""CellId"">The id of the specified cell.</param>
        /// <param name=""options"">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <returns>A <see cref=""");
source.Append(Codegen.GetString(Trinity::Codegen::GetNamespace()));
source.Append(@".GenericCellAccessor""/> instance.</returns>
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
                            throw new ArgumentException(""CellAccessOptions.CreateNewO");
source.Append(@"nCellNotFound is not valid for UseGenericCell. Cannot determine new cell type."", ""options"");
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
                    throw new CellNotFoundException(""Cannot access the cell."");
            }
            switch ((CellType)type)
            {
                ");
for (int iterator_1 = 0; iterator_1 < (node->cellList).Count;++iterator_1)
{
source.Append(@"
                case CellType.");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@":
                    return ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Accessor.New(CellId, cellPtr, entryIndex, options);
                ");
}
source.Append(@"
                default:
                    storage.ReleaseCellLock(CellId, entryIndex);
                    throw new CellTypeNotMatchException(""Cannot determine cell type."");
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
                    ");
for (int iterator_1 = 0; iterator_1 < (node->cellList).Count;++iterator_1)
{
source.Append(@"
                    case CellType.");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@":
                        {
                            var ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_accessor = ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Accessor.AllocIterativeAccessor(cellInfo);
                            var ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_cell     = (");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@")");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_accessor;
                            ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_accessor.Dispose();
                            yield return ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_cell;
                            break;
                        }
                    ");
}
source.Append(@"
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
                    ");
for (int iterator_1 = 0; iterator_1 < (node->cellList).Count;++iterator_1)
{
source.Append(@"
                    case CellType.");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@":
                        {
                            var ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_accessor = ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Accessor.AllocIterativeAccessor(cellInfo);
                            yield return ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_accessor;
                            ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_accessor.Dispose();
                            break;
                        }
                    ");
}
source.Append(@"
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
        /// <param name=""storage"">A <see cref=""Trinity.Storage.MemoryCloud""/> instance.</param>
        /// <param name=""cell"">The cell to be saved.</param>
        public void SaveGenericCell(Trinity.Storage.MemoryCloud storage, ICell cell)
        {
            switch ((CellType)cell.CellType)
            {
                ");
for (int iterator_1 = 0; iterator_1 < (node->cellList).Count;++iterator_1)
{
source.Append(@"
                case CellType.");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@":
                    storage.Save");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"((");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@")cell);
                    break;
                ");
}
source.Append(@"
            }
        }
        /// <summary>
        /// Loads the content of the cell with the specified cell Id.
        /// </summary>
        /// <param name=""storage"">A <see cref=""Trinity.Storage.MemoryCloud""/> instance.</param>
        /// <param name=""cellId"">A 64-bit cell Id.</param>
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
                        throw new CellNotFoundException(""Cannot access the cell."");
                    case TrinityErrorCode.E_NETWORK_SEND_FAILURE:
                        throw new System.IO.IOException(""Network error while accessing the cell."");
                    default:
 ");
source.Append(@"                       throw new Exception(""Cannot access the cell. Error code: "" + err.ToString());
                }
            }
            switch ((CellType)type)
            {
                ");
for (int iterator_1 = 0; iterator_1 < (node->cellList).Count;++iterator_1)
{
source.Append(@"
                case CellType.");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@":
                    fixed (byte* ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_ptr = buff)
                    {
                        ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Accessor/*_*/");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_accessor = new ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Accessor(");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_ptr);
                        ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_accessor.CellID = cellId;
                        return (");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@")");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_accessor;
                    }
                    break;
                ");
}
source.Append(@"
                default:
                    throw new CellTypeNotMatchException(""Cannot determine cell type."");
            }
        }
        #endregion
    }
}
");

            return source.ToString();
        }
    }
}
