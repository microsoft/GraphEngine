using System;
using System.Collections.Generic;
using System.Text;

namespace Trinity.FFI
{
    internal class ProgramRunner
    {
        private ILanguageRuntimeProvider m_runtimeProvider;
        private ILanguageRuntimeProvider runtime_provider;

        public ProgramRunner(ILanguageRuntimeProvider runtime_provider)
        {
            this.runtime_provider = runtime_provider;
        }
    }
}
