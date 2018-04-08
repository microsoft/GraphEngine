module JitCompiler 

open GraphEngine.Jit.Native.asmjit
open GraphEngine.Jit.Native.asmjit.x86
open TypeSystem
open Verbs
open System.Collections.Generic

type NativeFunction = { CallSite: nativeint; Descriptor: FunctionDescriptor; Code: CodeHolder  }

let s_rt = new JitRuntime()
let s_types = new Dictionary<TypeDescriptor, NativeFunction [] >()

type X86Compiler with
    member cc.CompileBGet(t: TypeDescriptor) =
        use p = cc.newUIntPtr()
        let r, pp = match TryGetTypeWidth t with
                    | Some w -> cc.newGpReg(uint32 <| FindTypeId t), x86operand.ptr(p, 0, uint32 w)
                    | _ -> failwith "not implemented"
        
        cc.setArg(0u, p) |> ignore
        cc.mov(r, pp)
        cc.ret(r)
        r.Dispose()
        pp.Dispose()
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
    let fs     = FunctionSignature f
    use cbuf   = new CodeHolder()
    cbuf.init(s_rt.codeInfo) |> ignore
    use cc     = new X86Compiler(cbuf)
    use func   = fs.AsmJitFuncSignature
    use ccfunc = cc.addFunc func

    match f.Verb with
    | BGet -> cc.CompileBGet f.DeclaringType
    | BSet -> cc.CompileBSet f.DeclaringType
    | _ -> failwith "NotImplemented"

    cc.endFunc() |> ignore
    cc.finalize() |> ignore

    { NativeFunction.CallSite = GraphEngine.Jit.Native.Helper.Add(s_rt, cbuf)
      Descriptor = f
      Code = cbuf }
