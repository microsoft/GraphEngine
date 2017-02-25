// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

using Trinity.Utilities;
using Trinity.Diagnostics;
using Trinity;
using Trinity.Daemon;
using Trinity.Core.Lib;
using Trinity.TSL.Lib;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Globalization;


namespace Trinity.Storage
{

    /// <summary>
    /// Provides methods for interacting with the in-memory store.
    /// </summary>
    public unsafe partial class LocalMemoryStorage : Storage, IDisposable, IEnumerable<CellInfo>, IEnumerable
    {
        #region Write-Ahead-Log facility
        private string WriteAheadLogFilePath
        {
            get
            {
                TRINITY_IMAGE_SIGNATURE current_sig = new TRINITY_IMAGE_SIGNATURE();

                if (TrinityErrorCode.E_SUCCESS != GetTrinityImageSignature(&current_sig))
                {
                    Log.WriteLine("Error trying to get image signature.");
                    return null;
                }

                string log_file_dir  = Path.Combine(TrinityConfig.StorageRoot, c_logfile_path);
                string log_ver_str   = current_sig.IMAGE_VERSION == ulong.MaxValue? "initial" : current_sig.IMAGE_VERSION.ToString(CultureInfo.InvariantCulture);
                string log_file_path = Path.Combine(log_file_dir, String.Format(CultureInfo.InvariantCulture, "{0}_{1}.dat", c_logfile_name, log_ver_str));

                try
                {
                    FileUtility.CompletePath(log_file_dir, create_nonexistent: true);
                }
                catch (Exception ex)
                {
                    Log.WriteLine(LogLevel.Error, "Error trying to complete log file directory: {0}", ex);
                }

                return log_file_path;
            }
        }
        private void*                  m_logfile                       = null;
        private string                 m_logfile_path                  = null;
        private const string           c_logfile_name                  = "primary_storage_log";
        private const string           c_celltype_signature_file_name  = "cell_type.sig";
        private const string           c_logfile_path                  = "write_ahead_log";
        #endregion

        #region KVStore extension facility
        public event Action StorageLoaded  = delegate { };
        public event Action StorageSaved    = delegate{ };
        public event Action StorageReset   = delegate{ };
        #endregion

        internal static volatile bool initialized = false;
        private object m_lock = new object();

        static LocalMemoryStorage()
        {
            InternalCalls.__init();
            TrinityConfig.LoadTrinityConfig();
            //BackgroundThread.StartMemoryStorageBgThreads();
        }

        /// <summary>
        /// Initializes the LocalStorage instance.
        /// </summary>
        public LocalMemoryStorage()
        {
            if (TrinityErrorCode.E_SUCCESS != CLocalMemoryStorage.CInitialize())
            {
                Environment.Exit(1);
            }



            InitializeWriteAheadLogFile();
            Thread.MemoryBarrier();
            initialized = true;
        }

        /// <summary>
        /// Pauses the memory defragmentation daemon thread.
        /// </summary>
        public static void PauseMemoryDefragmentation()
        {
            CLocalMemoryStorage.SetDefragmentationPaused(true);
        }

        /// <summary>
        /// Restarts the memory defragmentation daemon thread.
        /// </summary>
        public static void RestartMemoryDefragmentation()
        {
            CLocalMemoryStorage.SetDefragmentationPaused(false);
        }

        /// <summary>
        /// Gets the total cell count in local memory storage.
        /// </summary>
        public ulong CellCount
        {
            get
            {
                return _CellCountWrapper();
            }
        }

        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        private ulong _CellCountWrapper()
        {
            return CLocalMemoryStorage.CCellCount();
        }

        /// <summary>
        /// Destructs current LocalMemoryStorage instance.
        /// </summary>
        ~LocalMemoryStorage()
        {
            Dispose(false);
        }

        /// <summary>
        /// Release the unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (initialized)
            {
                CloseWriteAheadLogFile();
                CLocalMemoryStorage.CDispose();
                initialized = false;
            }
            if(disposing)
            {
                GC.SuppressFinalize(this);
            }
        }

        internal void DebugDump()
        {
            using (var sw = new StreamWriter("debug.txt"))
            {
                foreach (var cellinfo in Global.LocalStorage)
                {
                    sw.WriteLine("{0}    {1}    {2}    {3}    0x{4:X16}", cellinfo.CellId, cellinfo.CellSize, cellinfo.CellEntryIndex, cellinfo.CellType, (long)cellinfo.CellPtr);
                }
            }
        }

    }
}
