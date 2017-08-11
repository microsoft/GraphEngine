using System;
using System.Collections.Generic;
using System.Text;
using Trinity.Storage;
using Python.Runtime;

namespace Trinity.FFI.Python
{
    class CPythonRuntime : ILanguageRuntime
    {
        private Py.GILState m_GIL;

        public CPythonRuntime()
        {
            m_GIL = Py.GIL();
        }

        public void Dispose()
        {
            m_GIL.Dispose();
            PythonEngine.Shutdown();
        }

        public void RegisterOperations(IGenericCellOperations storageOperations, IGenericMessagePassingOperations messagePassingOperations)
        {
        }

        public int LoadProgram(string path)
        {
            return 0;
        }

        public string SynHandler(int methodId, string input)
        {
            return "";
        }

        public void AsynHandler(int methodId, string input)
        {
            return;
        }
    }
}
