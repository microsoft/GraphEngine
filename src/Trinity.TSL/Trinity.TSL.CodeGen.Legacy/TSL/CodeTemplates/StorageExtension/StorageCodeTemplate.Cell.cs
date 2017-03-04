using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trinity.Utilities;

namespace Trinity.TSL
{
    partial class StorageCodeTemplate
    {
        private static string GenerateStorageExtensionForCell(StructDescriptor desc, SpecificationScript script)
        {
            string coreCode = CellCodeTemplate.GenerateParametersToByteArrayCode(desc, generatePreserveHeaderCode: false, forCell: true);
            CodeWriter cw = new CodeWriter();
            cw += @"
    ///<summary>
    ///Provides interfaces for accessing " + desc.Name + @" cells
    ///on <see cref=""Trinity.Storage.LocalMemorySotrage""/>.
    static public class StorageExtension_" + desc.Name + @"
    {
";
            #region LocalStorage non logging
            cw +=
            TSLCompiler.GenerateAssignmentPrototypeParameterList(@"
        /// <summary>
        /// Adds a new cell of type " + desc.Name + @" to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. The value of the cell is specified in the method parameters.
        /// </summary>
        /// <param name=""storage""/>A <see cref=""Trinity.Storage.LocalMemoryStorage""/> instance.</param>
        /// <param name=""CellID"">A 64-bit cell Id.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool Save" + desc.Name + @"(this Trinity.Storage.LocalMemoryStorage storage,long CellID", desc);
            cw += coreCode +
             @"
        return storage.SaveCell(CellID, tmpcell, (ushort)CellType." + desc.Name + @") == TrinityErrorCode.E_SUCCESS;
    }
";
            cw += @"
        /// <summary>
        /// Adds a new cell of type " + desc.Name + @" to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. The parameter <paramref name=""CellID""/> overrides the cell id in the content object.
        /// </summary>
        /// <param name=""storage""/>A <see cref=""Trinity.Storage.LocalMemoryStorage""/> instance.</param>
        /// <param name=""CellID"">A 64-bit cell Id.</param>
        /// <param name=""CellContent"">The content of the cell.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool Save" + desc.Name + @"(this Trinity.Storage.LocalMemoryStorage storage, long CellID, " + desc.Name + @" CellContent)
        {
            return Save" + desc.Name + @"(storage,CellID";
            foreach (var f in desc.Fields)
            {
                cw += ", CellContent." + f.Name;
            }
            cw += @");
        }";

            cw += @"
        /// <summary>
        /// Adds a new cell of type " + desc.Name + @" to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. Cell Id is specified by the CellID field in the content object.
        /// </summary>
        /// <param name=""storage""/>A <see cref=""Trinity.Storage.LocalMemoryStorage""/> instance.</param>
        /// <param name=""CellContent"">The content of the cell.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool Save" + desc.Name + @"(this Trinity.Storage.LocalMemoryStorage storage, " + desc.Name + @" CellContent)
        {
            return Save" + desc.Name + @"(storage,CellContent.CellID";
            foreach (var f in desc.Fields)
            {
                cw += ", CellContent." + f.Name;
            }

            cw += @");
        }";


            cw += @"
        /// <summary>
        /// Allocate a cell accessor on the specified cell, which inteprets
        /// the cell as a " + desc.Name + @". Any changes done to the accessor
        /// are written to the storage immediately.
        /// If <c><see cref=""Trinity.TrinityConfig.ReadOnly""/> == false</c>,
        /// on calling this method, it attempts to acquire the lock of the cell,
        /// and blocks until it gets the lock. Otherwise this method is wait-free.
        /// </summary>
        /// <param name=""storage"">A <see cref=""Trinity.Storage.LocalMemoryStorage""/> instance.</param>
        /// <param name=""CellId"">The id of the specified cell.</param>
        /// <param name=""options"">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <returns>A <see cref=""" + script.RootNamespace + "." + desc.Name + @"""/> instance.</returns>
        public unsafe static " + desc.Name + @"_Accessor Use" + desc.Name + @"(this Trinity.Storage.LocalMemoryStorage storage,long CellID, CellAccessOptions options)
        {
";
            if (TSLCompiler.CompileWithDebugFeatures)
            {
                cw += @"
            if (storage.GetCellType(CellID) != (ushort)CellType." + desc.Name + @")
                throw new CellTypeNotMatchException(" + "\"Cell Type doesn't match, cell id is \" + " + @"CellID);";
            }
            cw += @"
            return " + desc.Name + @"_Accessor.New(CellID,options);
        }";

            cw += @"
        /// <summary>
        /// Allocate a cell accessor on the specified cell, which inteprets
        /// the cell as a " + desc.Name + @". Any changes done to the accessor
        /// are written to the storage immediately.
        /// If <c><see cref=""Trinity.TrinityConfig.ReadOnly""/> == false</c>,
        /// on calling this method, it attempts to acquire the lock of the cell,
        /// and blocks until it gets the lock.
        /// </summary>
        /// <param name=""storage"">A <see cref=""Trinity.Storage.LocalMemoryStorage""/> instance.</param>
        /// <param name=""CellId"">The id of the specified cell.</param>
        /// <returns>A <see cref=""" + script.RootNamespace + "." + desc.Name + @"""/> instance.</returns>
        public unsafe static " + desc.Name + @"_Accessor Use" + desc.Name + @"(this Trinity.Storage.LocalMemoryStorage storage,long CellID)
        {
";
            if (TSLCompiler.CompileWithDebugFeatures)
            {
                cw += @"
            if (storage.GetCellType(CellID) != (ushort)CellType." + desc.Name + @")
                throw new CellTypeNotMatchException(" + "\"Cell Type doesn't match, cell id is \" + " + @"CellID);";
            }
            cw += @"
            return " + desc.Name + @"_Accessor.New(CellID,CellAccessOptions.ThrowExceptionOnCellNotFound);
        }";

            cw += @"
        /// <summary>
        /// Loads the content of the specified cell. Any changes done to this object are not written to the store, unless
        /// the content object is saved back into the storage.
        /// </summary>
        public unsafe static " + desc.Name + " Load" + desc.Name + @"(this Trinity.Storage.LocalMemoryStorage storage, long CellID)
        {
            using (var cell = new " + desc.Name + @"_Accessor(CellID, CellAccessOptions.ThrowExceptionOnCellNotFound))
            {
                " + desc.Name + @" ret = cell;
                ret.CellID = CellID;
                return ret;
            }
        }
";

            #endregion

            #region LocalStorage logging
            cw +=
            TSLCompiler.GenerateAssignmentPrototypeParameterList(@"
        /// <summary>
        /// Adds a new cell of type " + desc.Name + @" to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. The value of the cell is specified in the method parameters.
        /// </summary>
        /// <param name=""storage""/>A <see cref=""Trinity.Storage.LocalMemoryStorage""/> instance.</param>
        /// <param name=""CellID"">A 64-bit cell Id.</param>
        /// <param name=""options"">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool Save" + desc.Name + @"(this Trinity.Storage.LocalMemoryStorage storage, CellAccessOptions options, long CellID", desc);
            cw += coreCode +
             @"
        return storage.SaveCell(options, CellID, tmpcell, (ushort)CellType." + desc.Name + @") == TrinityErrorCode.E_SUCCESS;
    }
";
            cw += @"
        /// <summary>
        /// Adds a new cell of type " + desc.Name + @" to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. The parameter <paramref name=""CellID""/> overrides the cell id in the content object.
        /// </summary>
        /// <param name=""storage""/>A <see cref=""Trinity.Storage.LocalMemoryStorage""/> instance.</param>
        /// <param name=""CellID"">A 64-bit cell Id.</param>
        /// <param name=""options"">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <param name=""CellContent"">The content of the cell.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool Save" + desc.Name + @"(this Trinity.Storage.LocalMemoryStorage storage, CellAccessOptions options, long CellID, " + desc.Name + @" CellContent)
        {
            return Save" + desc.Name + @"(storage, options, CellID";
            foreach (var f in desc.Fields)
            {
                cw += ", CellContent." + f.Name;
            }
            cw += @");
        }";

            cw += @"
        /// <summary>
        /// Adds a new cell of type " + desc.Name + @" to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. Cell Id is specified by the CellID field in the content object.
        /// </summary>
        /// <param name=""storage""/>A <see cref=""Trinity.Storage.LocalMemoryStorage""/> instance.</param>
        /// <param name=""options"">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <param name=""CellContent"">The content of the cell.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool Save" + desc.Name + @"(this Trinity.Storage.LocalMemoryStorage storage, CellAccessOptions options, " + desc.Name + @" CellContent)
        {
            return Save" + desc.Name + @"(storage, options, CellContent.CellID";
            foreach (var f in desc.Fields)
            {
                cw += ", CellContent." + f.Name;
            }

            cw += @");
        }";
            #endregion

            #region CloudStorage
            cw +=
            TSLCompiler.GenerateAssignmentPrototypeParameterList(@"
        /// <summary>
        /// Adds a new cell of type " + desc.Name + @" to the cloud storage if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. The value of the cell is specified in the method parameters. 
        /// </summary>
        /// <param name=""storage""/>A <see cref=""Trinity.Storage.MemoryCloud""/> instance.</param>
        /// <param name=""CellID"">A 64-bit cell Id.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool Save" + desc.Name + "(this Trinity.Storage.MemoryCloud storage,long CellID", desc);
            cw += coreCode +
                @"
            return storage.SaveCell(CellID, tmpcell,  (ushort)CellType." + desc.Name + @") == TrinityErrorCode.E_SUCCESS;
        }
";
            cw += @"
        /// <summary>
        /// Adds a new cell of type " + desc.Name + @" to the cloud storage if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. The parameter <paramref name=""CellID""/> overrides the cell id in the content object.
        /// </summary>
        /// <param name=""storage""/>A <see cref=""Trinity.Storage.MemoryCloud""/> instance.</param>
        /// <param name=""CellID"">A 64-bit cell Id.</param>
        /// <param name=""CellContent"">The content of the cell.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool Save" + desc.Name + @"(this Trinity.Storage.MemoryCloud storage, long CellID, " + desc.Name + @" CellContent)
        {
            return Save" + desc.Name + @"(storage,CellID";
            foreach (var f in desc.Fields)
            {
                cw += ", CellContent." + f.Name;
            }
            cw += @");
        }
";
            cw += @"
        /// <summary>
        /// Adds a new cell of type " + desc.Name + @" to the cloud storage if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists. Cell Id is specified by the CellID field in the content object.
        /// </summary>
        /// <param name=""storage""/>A <see cref=""Trinity.Storage.MemoryCloud""/> instance.</param>
        /// <param name=""CellContent"">The content of the cell.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe static bool Save" + desc.Name + @"(this Trinity.Storage.MemoryCloud storage, " + desc.Name + @" CellContent)
        {
            return Save" + desc.Name + @"(storage,CellContent.CellID";
            foreach (var f in desc.Fields)
            {
                cw += ", CellContent." + f.Name;
            }
            cw += @");
        }
";

            cw += @"
        /// <summary>
        /// Loads the content of the specified cell. Any changes done to this object are not written to the store, unless
        /// the content object is saved back into the storage.
        /// </summary>
        public unsafe static " + desc.Name + " Load" + desc.Name + @"(this Trinity.Storage.MemoryCloud storage, long CellID)
        {
            byte[] cellBuff;
            var eResult = storage.LoadCell(CellID, out cellBuff);
            if(eResult == TrinityErrorCode.E_CELL_NOT_FOUND)
            { throw new CellNotFoundException(""Cell with ID="" + CellID + "" not found!""); }
            else if(eResult == TrinityErrorCode.E_NETWORK_SEND_FAILURE)
            { throw new IOException(""Network error when loading cell with ID="" + CellID); }
            fixed(byte* ptr = cellBuff)
            {
                using (var cell = new " + desc.Name + @"_Accessor(ptr))
                {
                    " + desc.Name + @" ret = cell;
                    ret.CellID = CellID;
                    return ret;
                }
            }
        }
";
            #endregion
            cw += "}";
            return cw;
        }
    }
}