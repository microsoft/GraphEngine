using System;
using Trinity.Extension;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CSFunctionFactory
{
    unsafe class P
    {


        // List<int>
        public delegate int DELE(void* subject, int idx);

        public static int FUNC(void* subject, int idx)

        {
            var src = (List<int>)GCHandle.FromIntPtr((IntPtr)subject).Target;
            return src[idx];
        }

        public static DELE INST = FUNC;

        public static Int64 ADDR => Marshal.GetFunctionPointerForDelegate(INST).ToInt64();

    }

    public class H
    {
        public static Int64 Addr() => P.ADDR;

        static void Main(string[] args)
        {
            var x = new C
            {
                s = 1,
                x = 2
            };

            Console.WriteLine($"Hello World! {P.ADDR}");

        }
    }
}


