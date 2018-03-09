// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Trinity.Utilities;
using Trinity.Diagnostics;
using Trinity;
using Trinity.Daemon;
using System.Diagnostics;
using Trinity.Core.Lib;
using System.Runtime.CompilerServices;
using Trinity.Configuration;

namespace Trinity.Storage
{
    public unsafe partial class LocalMemoryStorage
    {
        /// <summary>
        /// Loads Trinity key-value store from disk to main memory.
        /// </summary>
        /// <returns>true if loading succeeds; otherwise, false.</returns>
        public bool LoadStorage()
        {
            lock (m_lock)
            {
                if (TrinityErrorCode.E_SUCCESS != CSynchronizeStorageRoot()) { return false; }

                RaiseStorageEvent(StorageBeforeLoad, nameof(StorageBeforeLoad));

                bool ret = CLocalMemoryStorage.CLoadStorage();

                RaiseStorageEvent(StorageLoaded, nameof(StorageLoaded));

                return ret;
            }
        }

        /// <summary>
        /// Dumps the in-memory key-value store to disk files.
        /// </summary>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public bool SaveStorage()
        {
            lock (m_lock)
            {
                if (TrinityErrorCode.E_SUCCESS != CSynchronizeStorageRoot()) { return false; }

                RaiseStorageEvent(StorageBeforeSave, nameof(StorageBeforeSave));

                bool ret = CLocalMemoryStorage.CSaveStorage();

                if (ret)
                {
                    RaiseStorageEvent(StorageSaved, nameof(StorageSaved));
                }

                return ret;
            }
        }

        /// <summary>
        /// Resets local memory storage to the initial state. The content in the memory storage will be cleared. And the memory storage will be shrunk to the initial size.
        /// </summary>
        /// <returns>true if resetting succeeds; otherwise, false.</returns>
        public bool ResetStorage()
        {
            lock (m_lock)
            {
                if (TrinityErrorCode.E_SUCCESS != CSynchronizeStorageRoot()) { return false; }

                RaiseStorageEvent(StorageBeforeReset, nameof(StorageBeforeReset));

                bool ret = CLocalMemoryStorage.CResetStorage();

                RaiseStorageEvent(StorageReset, nameof(StorageReset));

                return ret;
            }
        }

        /// <summary>
        /// Retrieves the path of the given storage slot.
        /// A storage slot is a subdirectory of the storage root,
        /// containing a specific version of disk image.
        /// </summary>
        /// <param name="isPrimary">Whether to use primary slot or secondary slot.</param>
        /// <returns></returns>
        public unsafe string GetStorageSlot(bool isPrimary)
        {
            char* buf  = CLocalMemoryStorage.CGetStorageSlot(isPrimary ? 1 : 0);
            string ret = new string(buf);
            Memory.free(buf);
            return ret;
        }

        /// <summary>
        /// Synchronizes storage root path with Trinity.C.
        /// Creates the directory if it does not exist.
        /// </summary>
        private static TrinityErrorCode CSynchronizeStorageRoot()
        {
            string storage_root = StorageConfig.Instance.StorageRoot;
            if (!Directory.Exists(storage_root))
            {
                try
                {
                    Directory.CreateDirectory(storage_root);
                }
                catch
                {
                    Log.WriteLine(LogLevel.Error, "Error occurs when creating StorageRoot: " + storage_root);
                    return TrinityErrorCode.E_FAILURE;
                }
            }

            try
            {
                byte[] buff = BitHelper.GetBytes(storage_root);
                fixed (byte* p = buff)
                {
                    CTrinityConfig.SetStorageRoot(p, buff.Length);
                }
            }
            catch
            {
                return TrinityErrorCode.E_FAILURE;
            }

            return TrinityErrorCode.E_SUCCESS;
        }


        internal TrinityErrorCode GetTrinityImageSignature(TRINITY_IMAGE_SIGNATURE* pSignature)
        {
            return CLocalMemoryStorage.CGetTrinityImageSignature(pSignature);
        }

        #region Schema integrity check
        private void LoadCellTypeSignatures()
        {
            try
            {
                string path = Path.Combine(GetStorageSlot(true), c_celltype_signature_file_name);
                if (!File.Exists(path))
                    return;
                Log.WriteLine(LogLevel.Info, "Loading cell type signatures.");
                var schema_sig_from_storage_root = File.ReadAllLines(path);
                var schema_sig_from_tsl = Global.storage_schema.CellTypeSignatures.ToArray();

                if (schema_sig_from_storage_root.Length > schema_sig_from_tsl.Length)
                {
                    Log.WriteLine(LogLevel.Warning, "The disk image contains more cell types than defined in the loaded TSL storage extension!");
                }

                if (schema_sig_from_storage_root.Length < schema_sig_from_tsl.Length)
                {
                    Log.WriteLine(LogLevel.Warning, "The disk image contains less cell types than defined in the loaded TSL storage extension!");
                }

                int min_len = Math.Min(schema_sig_from_storage_root.Length, schema_sig_from_tsl.Length);

                for (int i = 0; i<min_len; ++i)
                {
                    if (schema_sig_from_storage_root[i] != schema_sig_from_tsl[i])
                    {
                        Log.WriteLine(LogLevel.Error, "Inconsistent cell type signature for type #{0}.", i);
                        Log.WriteLine(LogLevel.Error, "Expecting: {0}.", schema_sig_from_tsl[i]);
                        Log.WriteLine(LogLevel.Error, "Got: {0}.", schema_sig_from_storage_root[i]);
                    }
                }
            }
            catch
            {
                Log.WriteLine(LogLevel.Error, "Errors occurred while examining storage schema signature.");
            }
        }

        private void SaveCellTypeSignatures()
        {
            Log.WriteLine(LogLevel.Info, "Saving cell type signatures.");
            try
            {
                File.WriteAllLines(Path.Combine(GetStorageSlot(true), c_celltype_signature_file_name), Global.storage_schema.CellTypeSignatures);
            }
            catch
            {
                Log.WriteLine(LogLevel.Error, "Errors occurred while saving storage schema signature.");
            }
        }
        #endregion
    }
}
