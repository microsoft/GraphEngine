namespace Trinity.FFI.Metagen




module JitGen = 
    open GraphEngine.Jit.Verbs
    open GraphEngine.Jit.TypeSystem
    open GraphEngine.Jit.JitNativeInterop
    open Trinity.Storage
    open System
    open Trinity.FFI.Metagen.Operator
    open GraphEngine.Jit
    open Microsoft.FSharp.NativeInterop


    type ManglingCode = char
    type Name = string

    let render  (manglingCode        : ManglingCode)
                (name'maker          : ManglingCode -> TypeDescriptor -> Name) 
                (subject             : TypeDescriptor) 
                (verb                : Verb) : FunctionDescriptor = 
        
        {DeclaringType=subject; Verb=verb}
   