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
using GraphEngine.Jit.TSL;

using static GraphEngine.Jit.Basic;

namespace GraphEngine.Jit.Playground
{
    delegate void Setter(NativeCellAccessor p0, int p1);
    delegate int Getter(NativeCellAccessor p0);
    unsafe class Program
    {
        static void Main(string[] args)
        {
            //Console.ReadLine();

            IntegerBGetBSet();
            StringBGetBSet();
            IntegerSGetSet();
            StringSGetSet();
            IntegerSLGetSet();
            StringSLGetSet();
            IntegerSLCount();

            //byte[] buf = new byte[16];
            //int val = int.Parse(Console.ReadLine());
            //fixed(byte* p = buf)
            //{
            //    *(long*)p += val;
            //}
        }
    }
}
