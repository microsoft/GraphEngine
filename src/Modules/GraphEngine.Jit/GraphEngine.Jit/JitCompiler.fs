namespace GraphEngine.Jit

module JitCompiler = 
    open GraphEngine.Jit.Native.asmjit
    open GraphEngine.Jit.Native.asmjit.x86
    open GraphEngine.Jit.TypeSystem
    open GraphEngine.Jit.VerbTraits
    open System.Collections.Generic
    open Microsoft.FSharp.NativeInterop
    open System
    open System.Runtime.InteropServices.ComTypes

    type NativeFunction = { CallSite: nativeint; Descriptor: seq<FunctionDescriptor>; Code: CodeHolder  }

    let s_rt = new JitRuntime()
    let s_types = new Dictionary<TypeDescriptor, NativeFunction [] >()

    type X86Compiler with
        member cc.CompileBGet(t: TypeDescriptor) =
            let p = cc.newUIntPtr()
            let r, pp = match TryGetTypeWidth t with
                        | Some w -> cc.newGpReg(uint32 <| FindTypeId t), x86operand.ptr(p, 0, uint32 w)
                        | _ -> failwith "not implemented"
            
            cc.setArg(0u, p)
            cc.mov(r, pp)
            cc.ret(r)
            ()
        member cc.CompileBSet(t: TypeDescriptor) =
            let p = cc.newUIntPtr()
            let v, pp = match TryGetTypeWidth t with
                        | Some w -> cc.newGpReg(uint32 <| FindTypeId t), x86operand.ptr(p, 0, uint32 w)
                        | _ -> failwith "not implemented"
            
            cc.setArg(0u, p)
            cc.setArg(1u, v)
            cc.mov(pp, v)
            cc.ret()
            ()

    let CompileFunction (f: FunctionDescriptor): NativeFunction =
        let cbuf = new CodeHolder()
        cbuf.init(s_rt.codeInfo) |> ignore
        use cc = new X86Compiler(cbuf)
        use func = MakeFuncSignature (Output f) None |> cc.addFunc

        match f.Verb with
        | BGet -> cc.CompileBGet f.DeclaringType
        | BSet -> cc.CompileBSet f.DeclaringType
        | _ -> failwith "NotImplemented"

        cc.endFunc() |> ignore
        cc.finalize() |> ignore

        { NativeFunction.CallSite = GraphEngine.Jit.Native.Helper.Add(s_rt, cbuf)
          Descriptor = seq [ f ] 
          Code = cbuf }


    let Compile (verbs: seq<FunctionDescriptor>)  = 
        ()
