//  Low-level interop module for asmjit

namespace GraphEngine.Jit

module AsmJit =
    open System

    [<System.Runtime.InteropServices.DllImport("GraphEngine.Jit.Native.dll")>]
    extern int32 test(nativeint& fn)
    [<System.Runtime.InteropServices.DllImport("GraphEngine.Jit.Native.dll")>]
    extern int32 test2(nativeint fn)

    [<System.Runtime.InteropServices.DllImport("GraphEngine.Jit.Native.dll")>]
    extern nativeint private NewJitRuntime()
    [<System.Runtime.InteropServices.DllImport("GraphEngine.Jit.Native.dll")>]
    extern unit private DeleteJitRuntime(nativeint)

    [<System.Runtime.InteropServices.DllImport("GraphEngine.Jit.Native.dll")>]
    extern nativeint private NewCodeHolder(nativeint)
    [<System.Runtime.InteropServices.DllImport("GraphEngine.Jit.Native.dll")>]
    extern unit private DeleteCodeHolder(nativeint)

    [<System.Runtime.InteropServices.DllImport("GraphEngine.Jit.Native.dll")>]
    extern nativeint private NewX86Assembler(nativeint)
    [<System.Runtime.InteropServices.DllImport("GraphEngine.Jit.Native.dll")>]
    extern unit private DeleteX86Assembler(nativeint)

    [<System.Runtime.InteropServices.DllImport("GraphEngine.Jit.Native.dll")>]
    extern nativeint private NewX86Compiler(nativeint)
    [<System.Runtime.InteropServices.DllImport("GraphEngine.Jit.Native.dll")>]
    extern unit private DeleteX86Compiler(nativeint)

    [<System.Runtime.InteropServices.DllImport("GraphEngine.Jit.Native.dll")>]
    extern nativeint private AddFunction(nativeint, nativeint)
    [<System.Runtime.InteropServices.DllImport("GraphEngine.Jit.Native.dll")>]
    extern unit private RemoveFunction(nativeint, nativeint)

    type Runtime() =
        let m_Handle = NewJitRuntime()
        interface IDisposable with member x.Dispose() = DeleteJitRuntime(m_Handle)
        member internal x.Handle = m_Handle
        member x.MakeCode() = CodeHolder(x)

    and CodeHolder(rt: Runtime) = 
        let m_rt = rt
        let m_Handle = NewCodeHolder(rt.Handle)
        interface IDisposable with member x.Dispose() = DeleteCodeHolder(m_Handle)
        member internal x.Handle = m_Handle
        member x.AttachAssembler() = X86Assembler(x)
        member x.AttachCompiler() = X86Compiler(x)
        member x.Emit() = Function(m_rt, x)

    and X86Assembler(code: CodeHolder) =
        let m_Handle = NewX86Assembler(code.Handle)
        interface IDisposable with member x.Dispose() = DeleteX86Assembler(m_Handle)
        member internal x.Handle = m_Handle

    and X86Compiler(code: CodeHolder) =
        let m_Handle = NewX86Compiler(code.Handle)
        interface IDisposable with member x.Dispose() = DeleteX86Compiler(m_Handle)
        member internal x.Handle = m_Handle

    and Function(rt: Runtime, code: CodeHolder) = 
        let m_rt = rt
        let m_fn = AddFunction(rt.Handle, code.Handle)
        interface IDisposable with member x.Dispose() = RemoveFunction(m_rt.Handle, m_fn)
        member x.FunctionPointer = m_fn
