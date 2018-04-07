using GraphEngine.Jit.Native.asmjit;
using System;
using System.Linq;

namespace GraphEngine.Jit.Native
{
    public static class Helper
    {
        public static unsafe IntPtr Add(this JitRuntime cc, CodeHolder code)
        {
            void* callsite = null;
            if(0 != cc._add(&callsite, code))
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
                fs.init((uint)ccid, (byte)retid, p, (uint)buf.Length);
            }
        }
    }
}
