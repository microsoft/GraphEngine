using GraphEngine.Jit.Native.asmjit;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using Trinity.Core.Lib;

namespace GraphEngine.Jit.Native
{
    public static class Helper
    {
        public static unsafe IntPtr malloc(int size)
        {
            return (IntPtr)Memory.malloc((ulong)size);
        }

        public static unsafe IntPtr Add(JitRuntime cc, CodeHolder code)
        {
            void* callsite = null;
            if(0 != cc._add(&callsite, code))
            {
                throw new AsmJitException();
            }
            return (IntPtr)callsite;
        }

        public static unsafe void Init(FuncSignature fs, CallConv.Id ccid, TypeId.Id retid, TypeId.Id[] argsid)
        {
            var buf = argsid.Select(x => (byte)x).ToArray();
            fixed (byte* p = buf)
            {
                fs.init((uint)ccid, (byte)retid, p, (uint)buf.Length);
            }
        }

        public static unsafe TRet Call<TRet>(IntPtr callsite)
        {
            return Marshal.GetDelegateForFunctionPointer<Func<TRet>>(callsite)();
        }

        public static unsafe TRet Call<TRet>(IntPtr callsite, IntPtr p)
        {
            return Marshal.GetDelegateForFunctionPointer<Func<IntPtr, TRet>>(callsite)(p);
        }

        public static unsafe void Call(IntPtr callsite)
        {
            Marshal.GetDelegateForFunctionPointer<Action>(callsite)();
        }

        public static unsafe void Call<TArg>(IntPtr callsite, IntPtr p, TArg arg)
        {
            Marshal.GetDelegateForFunctionPointer<Action<IntPtr, TArg>>(callsite)(p, arg);
        }
    }
}
