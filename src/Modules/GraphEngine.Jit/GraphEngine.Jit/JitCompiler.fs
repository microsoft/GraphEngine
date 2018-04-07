namespace GraphEngine.Jit

module JitCompiler = 
    open GraphEngine.Jit.Native.Asmjit
    open GraphEngine.Jit.TypeSystem
    open GraphEngine.Jit.VerbTraits
    open System.Collections.Generic
    open Microsoft.FSharp.NativeInterop
    open System

    type NativeFunction = { CallSite: nativeint; Descriptor: seq<FunctionDescriptor>; Code: CodeHolder  }

    let s_rt = new JitRuntime()
    let s_types = new Dictionary<TypeDescriptor, NativeFunction [] >()

    let CompileBGet (f: FunctionDescriptor) =
        let cbuf = new CodeHolder()
        cbuf.Init(s_rt.CodeInfo) |> ignore
        use cc = new X86Compiler(cbuf)
        use func = MakeFuncSignature (Output f) None |> cc.AddFunc

        let p = cc.NewUIntPtr()
        cc.SetArg(0u, p) |> ignore

        { NativeFunction.CallSite = GraphEngine.Jit.Native.Helper.Add(s_rt, cbuf)
          Descriptor = seq [ f ] 
          Code = cbuf}

    let CompileFunction (f: FunctionDescriptor): NativeFunction =
        match f.Verb with
        | BGet -> CompileBGet f
        | _ -> failwith "NotImplemented"

    let Compile (verbs: seq<FunctionDescriptor>)  = 
        ()
