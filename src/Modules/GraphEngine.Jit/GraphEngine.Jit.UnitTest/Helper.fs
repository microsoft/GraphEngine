module GraphEngine.Jit.Helper

open System.Runtime.InteropServices

type CallHelper =
    static member Call (callsite: nativeint, paccessor: nativeint, arg0: int32) = 
        Marshal.GetDelegateForFunctionPointer<nativeint * int32 -> unit>(callsite)
        <| (paccessor, arg0)
    static member Call<'a> (callsite: nativeint, paccessor: nativeint) = 
        Marshal.GetDelegateForFunctionPointer<nativeint -> 'a>(callsite)
        <| (paccessor)