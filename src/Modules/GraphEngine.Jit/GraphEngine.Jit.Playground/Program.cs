using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Trinity;
using static GraphEngine.Jit.Verbs;
using static GraphEngine.Jit.JitCompiler;
using static GraphEngine.Jit.TypeSystem;
using static GraphEngine.Jit.JitNativeInterop;

namespace GraphEngine.Jit.Playground
{
    delegate void Setter(NativeCellAccessor p0, int p1);
    delegate int Getter(NativeCellAccessor p0);
    unsafe class Program
    {
        static void Main(string[] args)
        {
            var cd = Global.StorageSchema.CellDescriptors.First();
            var td = Make(cd);

            var gfd = new FunctionDescriptor(td.Members.First().Type, Verb.BGet);
            var sfd = new FunctionDescriptor(td.Members.First().Type, Verb.BSet);

            var ng = GraphEngine.Jit.JitCompiler.CompileFunction(gfd);
            var ns = GraphEngine.Jit.JitCompiler.CompileFunction(sfd);

            Console.WriteLine(ng.CallSite);
            Console.WriteLine(ns.CallSite);
            var nfg = Marshal.GetDelegateForFunctionPointer<Getter>(ng.CallSite);
            var nfs = Marshal.GetDelegateForFunctionPointer<Setter>(ns.CallSite);

            NativeCellAccessor accessor = new NativeCellAccessor( );
            byte[] buf = new byte[16];
            fixed(byte* p = buf)
            {
                accessor.CellPtr = (IntPtr)p;
                nfs(accessor, 123);
                Console.WriteLine(nfg(accessor));
                Console.WriteLine(*(int*)p);
                *(int*)p = 123;
                Console.WriteLine(nfg(accessor));

            }
        }
    }
}
