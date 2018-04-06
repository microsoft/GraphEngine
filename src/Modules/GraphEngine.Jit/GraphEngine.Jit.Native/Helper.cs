using GraphEngine.Jit.Native.Asmjit;
using System;
using System.Linq;

namespace GraphEngine.Jit.Native
{
    public static class Helper
    {
        public static unsafe IntPtr Add(this JitRuntime cc, CodeHolder code)
        {
            void* callsite = null;
            if(0 != cc.Add(&callsite, code))
            {
                throw new AsmJitException();
            }
            return (IntPtr)callsite;
        }

        public static unsafe void Init(this FuncSignature fs, CallConv.Id ccid, TypeId.Id retid, TypeId.Id[] argsid)
        {
            var buf = argsid.Select(x => (byte)x).ToArray();
            fixed (byte* p = buf)
            {
                fs.Init((uint)ccid, (byte)retid, p, (uint)buf.Length);
            }
        }
    }
}
