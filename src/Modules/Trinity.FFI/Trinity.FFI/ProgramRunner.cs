using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Trinity.Storage;

namespace Trinity.FFI
{
    internal class ProgramRunner : IDisposable
    {
        #region Fields
        private FFIModule m_module;
        private ILanguageRuntimeProvider m_runtimeProvider;
        private ConcurrentStack<ILanguageRuntime> m_runtimes;

        #endregion

        public ProgramRunner(ILanguageRuntimeProvider runtime_provider, FFIModule module)
        {
            m_runtimeProvider = runtime_provider;
            m_runtimes = new ConcurrentStack<ILanguageRuntime>();
            m_module = module;

            //  Two situations where we just allocate a single runtime:
            //  1. The provider specified that only one runtime can be created.
            //  2. The provider claims that a runtime has multi-threading capabilities.
            if (runtime_provider.RuntimeModel == RuntimeModel.SingleRuntime ||
               runtime_provider.ThreadingModel == ThreadingModel.MultiThreaded)
            {
                _AllocSingleRuntime();
            }
            else
            {
                _AllocMultiRuntime();

            }

            if (m_runtimeProvider.ThreadingModel == ThreadingModel.SingleThreaded)
            {

            }
            else
            {

            }
        }

        private void _AllocSingleRuntime()
        {
            m_runtimes.Push(m_runtimeProvider.NewRuntime());
        }

        private void _AllocMultiRuntime()
        {
        }

        public string RuntimeName => m_runtimeProvider.Name;

        public void Dispose()
        {
        }

        public void RegisterOperations(IGenericCellOperations storageOperations, IGenericMessagePassingOperations messagePassingOperations)
        {
        }

        public string Run(int methodId, string input)
        {
            throw new NotImplementedException();
        }

        public int RunAsync(int methodId, string input)
        {
            throw new NotImplementedException();
        }

        public int Wait(int handle, int timeout, out string output)
        {
            throw new NotImplementedException();
        }

        internal void LoadProgram(string file)
        {
            throw new NotImplementedException();
        }
    }
}
