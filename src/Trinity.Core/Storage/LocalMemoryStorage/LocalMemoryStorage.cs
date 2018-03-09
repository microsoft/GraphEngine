// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Trinity.Diagnostics;
using System.Runtime.CompilerServices;


namespace Trinity.Storage
{

    /// <summary>
    /// Provides methods for interacting with the in-memory store.
    /// </summary>
    public unsafe partial class LocalMemoryStorage : IStorage, IDisposable, IEnumerable<CellInfo>, IEnumerable
    {


        internal static volatile bool initialized = false;

        private object m_lock = new object();


        static LocalMemoryStorage()
        {
            TrinityC.Init();
            try
            {
                TrinityConfig.LoadTrinityConfig();
            }
            catch
            {
                Log.WriteLine(LogLevel.Error, "Failure to load config file, falling back to default LocalMemoryStorage behavior");
            }
            CSynchronizeStorageRoot();
        }

        /// <summary>
        /// Initializes the LocalStorage instance.
        /// </summary>
        public LocalMemoryStorage()
        {
            if (TrinityErrorCode.E_SUCCESS != CLocalMemoryStorage.CInitialize())
            {
                //TODO more specific exception type
                throw new Exception();
            }

            // Register built-in storage event handlers
            StorageLoaded += InitializeWriteAheadLogFile;
            StorageLoaded += LoadCellTypeSignatures;
            StorageSaved  += CreateWriteAheadLogFile;
            StorageSaved  += SaveCellTypeSignatures;
            StorageReset  += ResetWriteAheadLog;

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
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (initialized)
            {
                _CloseWriteAheadLogFile();
                CLocalMemoryStorage.CDispose();
                initialized = false;
            }
            if (disposing)
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

        #region Thread Context facilities
        ////TODO we may need to cache the previous ThreadContext object to prevent it from being collected by GC..
        //private static AsyncLocal<ThreadContext> s_thread_ctx = new AsyncLocal<ThreadContext>(e =>
        //{
        //    var current_ctx = e.CurrentValue;
        //    if (current_ctx != null) { CLocalMemoryStorage.CThreadContextSet(e.CurrentValue.m_lock_ctx); }
        //    else { CLocalMemoryStorage.CThreadContextSet(null); }
        //});

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //internal static void _EnsureThreadContext()
        //{
        //    if (s_thread_ctx.Value == null)
        //        s_thread_ctx.Value = new ThreadContext();
        //}

        //[ThreadStatic]
        //private static ThreadContext s_thread_ctx = null;
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //internal static void _EnsureThreadContext()
        //{
        //    if (s_thread_ctx == null)
        //        s_thread_ctx = new ThreadContext();
        //}
        #endregion
    }
}
