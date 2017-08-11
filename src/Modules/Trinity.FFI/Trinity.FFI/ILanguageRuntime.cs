using System;
using System.Collections.Generic;
using System.Text;
using Trinity.Storage;

namespace Trinity.FFI
{
    public enum ThreadingModel
    {
        //  For runtimes like V8, Chakra, CPython etc.
        SingleThreaded,
        //  For runtimes like CLR, JVM or C-runtime.
        //  Also for functional programming runtimes, 
        //  and message-passing runtimes (Haskell, Erlang etc.)
        MultiThreaded,
    }

    public enum RuntimeModel
    {
        //  For runtimes holding process-level resources, like the 
        //  Global Interpreter Lock (GIL) for CPython.
        //  CoreCLR also would likely fall into this category, as 
        //  there is no support for AppDomains.
        SingleRuntime,
        //  Some runtimes are designed to support multi-instances
        //  within a single process, for example, the JavaScript
        //  runtimes. This means that despite the single-threaded
        //  threading model, we can still achieve multi-threading
        //  parallelism by message passing and shared local memory
        //  storage provided by Graph Engine.
        MultipleRuntime,
    }

    public delegate string SynchronousFFIHandler(string input);
    public delegate void AsynchronousFFIHandler(string input);

    public interface ILanguageRuntime : IDisposable
    {
        void RegisterOperations(IGenericCellOperations storageOperations, IGenericMessagePassingOperations messagePassingOperations);
        /// <summary>
        /// Loads a program from the given path, and then execute the entry point.
        /// </summary>
        int LoadProgram(string path);
        string SynHandler(int methodId, string input);
        void AsynHandler(int methodId, string input);
    }

    public interface ILanguageRuntimeProvider
    {
        //  Specifies the name of the runtime.
        string Name { get; }
        //  Specifies the threading model for a runtime.
        ThreadingModel ThreadingModel { get; }
        //  Specifies the runtime model.
        RuntimeModel RuntimeModel { get; }
        string[] SupportedSuffix { get; }
        ILanguageRuntime NewRuntime();
    }
}
