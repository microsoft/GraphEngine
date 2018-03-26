//  Low-level interop module for asmjit

namespace GraphEngine.Jit.Assembly

module AsmJit =
    open Trinity
    [<System.Runtime.InteropServices.DllImport("GraphEngine.Jit.Native.dll")>]
    extern TrinityErrorCode test(nativeint& fn)
    [<System.Runtime.InteropServices.DllImport("GraphEngine.Jit.Native.dll")>]
    extern void test2(nativeint fn)

