namespace GraphEngine.Jit

module Runtime = 
    open GraphEngine.Jit.Native.Asmjit
    open GraphEngine.Jit.TypeSystem
    open System.Collections.Generic

    type NativeFunction = { CallSite: nativeint; Descriptor: FunctionDescriptor  }

    let s_rt = new JitRuntime()
    let s_types = new Dictionary<TypeDescriptor, List<NativeFunction>>()

    //let RegisterDataType: TypeDescriptor -> List<NativeFunction> = ()
