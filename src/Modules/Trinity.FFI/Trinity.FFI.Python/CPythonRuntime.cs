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
        }

        public void LoadProgram(string path)
        {
            throw new NotImplementedException();
        }

        public void RegisterOperations(IGenericCellOperations storageOperations, IGenericMessagePassingOperations messagePassingOperations)
        {
            throw new NotImplementedException();
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

        int ILanguageRuntime.LoadProgram(string path)
        {
            throw new NotImplementedException();
        }
    }
}
