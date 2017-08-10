using System;

namespace Trinity.FFI.Python
{
    public class PythonRuntimeProvider : ILanguageRuntimeProvider
    {
        public string Name => "Python";

        public ThreadingModel ThreadingModel => ThreadingModel.SingleThreaded;

        public RuntimeModel RuntimeModel => RuntimeModel.SingleRuntime;

        public string[] SupportedSuffix => new[] { "py" };

        public ILanguageRuntime NewRuntime()
        {
            return new CPythonRuntime();
        }
    }
}
