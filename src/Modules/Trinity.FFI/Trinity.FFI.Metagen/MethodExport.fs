namespace Trinity.FFI.Metagen

open Trinity.FFI.Metagen

module MethodExport = 
    open Trinity.FFI.Metagen.MetaGen
    open GraphEngine.Jit.TypeSystem
    open GraphEngine.Jit.JitCompiler
    open GraphEngine.Jit.Verbs
    open GraphEngine.Jit
    open System

    let MethodGen (methodInfoSeq: seq<FunctionDescriptor * CodeGenerator>) : seq<NativeFunction * SwigCode> = seq{
        for fnDesc, codeGenerator in methodInfoSeq do
            let nativeFn: NativeFunction =  CompileFunction fnDesc
            let swigCode: SwigCode      =  nativeFn.CallSite.ToString() |> codeGenerator
            yield (nativeFn, swigCode) 
    }
    

