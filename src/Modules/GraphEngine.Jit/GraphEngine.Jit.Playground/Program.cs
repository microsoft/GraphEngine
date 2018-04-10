using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GraphEngine.Jit.Playground
{
    struct C1
    {
        public string bar;
    }

    unsafe class Program
    {
        void* get_bar_c1(void* c1)
        {
            var h = GCHandle.FromIntPtr((IntPtr)c1);
            var obj = (C1)h.Target;
            return GCHandle.ToIntPtr(GCHandle.Alloc(obj.bar)).ToPointer();
        }

        static void Main(string[] args)
        {
            //IntPtr fn = IntPtr.Zero;
            //Assembly.AsmJit.test(ref fn);
            //Console.WriteLine(Assembly.AsmJit.test2(fn));
        }
    }
}
