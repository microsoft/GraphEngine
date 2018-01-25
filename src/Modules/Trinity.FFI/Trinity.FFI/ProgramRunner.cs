using System;
using System.Collections.Concurrent;
using Trinity.Storage;
using Trinity.TSL.Lib;

namespace Trinity.FFI
{
    internal class ProgramRunner : IDisposable
    {
        #region Fields
        private TrinityFFIModule m_module;
        private ILanguageRuntimeProvider m_runtimeProvider;
        private BlockingCollection<ILanguageRuntime> m_runtimes;
        private bool m_singleThreaded;
        private int m_runtime_type_id = 0;
        private TRINITY_INTERFACES m_interfaces;

        private static int s_runtime_cnt = 0;
        private const int c_runtime_cnt_max = 255;
        private const uint c_max_protocol_id = (1 << 24) - 1;
        #endregion

        /// <summary>
        /// Each runner has a protocol address space of 24bits, and
        /// there can be at most 255 types of runners in the system.
        /// </summary>
        public ProgramRunner(ILanguageRuntimeProvider runtime_provider, TrinityFFIModule module)
        {
            if (s_runtime_cnt == c_runtime_cnt_max) throw new InvalidOperationException("Maximum number of language runtime providers reached.");

            m_runtimeProvider = runtime_provider;
            m_runtimes        = new BlockingCollection<ILanguageRuntime>(new ConcurrentQueue<ILanguageRuntime>());
            m_module          = module;
            m_runtime_type_id = s_runtime_cnt++;

            m_interfaces = new TRINITY_INTERFACES
            {
                async_registry = TrinityWrapper.trinity_ffi_async_registry,
                async_send = TrinityWrapper.trinity_ffi_async_send,
                cell_appendfield = TrinityWrapper.trinity_ffi_cell_append,
                cell_getfield = TrinityWrapper.trinity_ffi_cell_get,
                cell_getid = TrinityWrapper.trinity_ffi_cell_getid,
                cell_dispose = TrinityWrapper.trinity_ffi_cell_dispose,
                cell_fieldenum_get = TrinityWrapper.trinity_ffi_cellenum_get,
                cell_fieldenum_movenext = TrinityWrapper.trinity_ffi_cellenum_movenext,
                cell_fieldenum_current = TrinityWrapper.trinity_ffi_cellenum_current,
                cell_fieldenum_dispose = TrinityWrapper.trinity_ffi_cellenum_dispose,
                cell_fieldinfo_name = TrinityWrapper.trinity_ffi_fieldinfo_name,
                cell_hasfield = TrinityWrapper.trinity_ffi_cell_has,
                cell_removefield = TrinityWrapper.trinity_ffi_cell_delete,
                cell_setfield = TrinityWrapper.trinity_ffi_cell_set,
                cell_setid = TrinityWrapper.trinity_ffi_cell_setid,
                cell_tostring = TrinityWrapper.trinity_ffi_cell_tostring,
                cloud_loadcell = TrinityWrapper.trinity_ffi_cloud_loadcell,
                cloud_removecell = TrinityWrapper.trinity_ffi_cloud_removecell,
                cloud_savecell = TrinityWrapper.trinity_ffi_cloud_savecell,
                local_loadcell = TrinityWrapper.trinity_ffi_local_loadcell,
                local_removecell = TrinityWrapper.trinity_ffi_local_removecell,
                local_savecell_1 = TrinityWrapper.trinity_ffi_local_savecell_1,
                local_savecell_2 = TrinityWrapper.trinity_ffi_local_savecell_2,
                newcell_1 = TrinityWrapper.trinity_ffi_newcell_1,
                newcell_2 = TrinityWrapper.trinity_ffi_newcell_2,
                newcell_3 = TrinityWrapper.trinity_ffi_newcell_3,
                sync_registry = TrinityWrapper.trinity_ffi_sync_registry,
                sync_send = TrinityWrapper.trinity_ffi_sync_send,
            };

            Native.TRINITY_FFI_SET_INTERFACES(ref m_interfaces);

            //  Two situations where we just allocate a single runtime:
            //  1. The provider specified that only one runtime can be created.
            //  2. The provider claims that a runtime has multi-threading capabilities.
            if (runtime_provider.RuntimeModel   == RuntimeModel.SingleRuntime ||
                runtime_provider.ThreadingModel == ThreadingModel.MultiThreaded)
            {
                _AllocSingleRuntime();
            }
            else
            {
                _AllocMultiRuntime();

            }

            m_singleThreaded = (m_runtimeProvider.ThreadingModel == ThreadingModel.SingleThreaded);
        }

        private void _AllocSingleRuntime()
        {
            m_runtimes.Add(m_runtimeProvider.NewRuntime());
        }

        private void _AllocMultiRuntime()
        {
            for (int i = 0; i < Environment.ProcessorCount; ++i)
            {
                m_runtimes.Add(m_runtimeProvider.NewRuntime());
            }
        }

        public string RuntimeName => m_runtimeProvider.Name;

        public void Dispose()
        {
            foreach (var runtime in m_runtimes)
            {
                runtime.Dispose();
            }
        }

        public string SynHandler(int methodId, string input)
        {
            ILanguageRuntime runtime = null;
            try
            {
                runtime = _GetRuntime();
                return runtime.SynHandler(methodId, input);
            }
            finally
            {
                if (runtime != null) _PutRuntime(runtime);
            }
        }

        public void AsynHandler(int methodId, string input)
        {
            ILanguageRuntime runtime = null;
            try
            {
                runtime = _GetRuntime();
                runtime.AsynHandler(methodId, input);
            }
            finally
            {
                if (runtime != null) _PutRuntime(runtime);
            }
        }

        internal int LoadProgram(string file)
        {
            ILanguageRuntime runtime = null;
            try
            {
                runtime = _GetRuntime();
                return runtime.LoadProgram(file);
            }
            finally
            {
                if (runtime != null) _PutRuntime(runtime);
            }
        }

        //  This operation will be blocked until we
        //  have a free runtime.
        private ILanguageRuntime _GetRuntime()
        {
            var runtime = m_runtimes.Take();
            // If the runtime supports multi-threading, immediately put it back to the pool.
            if (!m_singleThreaded) m_runtimes.Add(runtime);

            return runtime;
        }

        private void _PutRuntime(ILanguageRuntime runtime)
        {
            if (m_singleThreaded) m_runtimes.Add(runtime);
        }
    }
}
