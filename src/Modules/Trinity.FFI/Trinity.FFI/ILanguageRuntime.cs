using System;
using System.Collections.Generic;
using System.Text;
using Trinity.Storage;

namespace Trinity.FFI
{
    enum ThreadingModel
    {
        //  For runtimes like V8, Chakra, CPython etc.
        SingleThreaded,
        //  For runtimes like CLR, JVM or C-runtime.
        MultiThreaded,
        //  For functional programming runtimes, and message-passing 
        //  runtimes (Haskell, Erlang etc.)
        AutoThreaded,
    }

    enum RuntimeModel
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

    interface ILanguageRuntime
    {
        TrinityErrorCode RegisterGenericCellOps(IGenericCellOperations operations);
        TrinityErrorCode RegisterGenericMessagePassingInterfaces(IGenericCellOperations operations);
    }

    interface ILanguageRuntimeProvider
    {
        //  Specifies the name of the runtime.
        string Name { get; }
        //  Specifies the threading model for a runtime.
        ThreadingModel ThreadingModel { get; }
        //  Specifies the runtime model.
        RuntimeModel RuntimeModel { get; }
        string[] SupportedSuffix { get; }
    }
}
