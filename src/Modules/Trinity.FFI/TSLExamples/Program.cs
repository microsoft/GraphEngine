using System;
using System.Runtime.InteropServices;
using Trinity.Extension;
namespace TSLExamples
{
    unsafe class Program
    {
        public void Cell_C1_get_lst(void* src, out void* data)
        {
            var lst = ((C1)GCHandle.FromIntPtr((IntPtr)src).Target).bar;
            data = GCHandle.ToIntPtr(GCHandle.Alloc(lst)).ToPointer();
        }
        static void Main(string[] args)
        {
            var c = new C1();
            
            Console.WriteLine("Hello World!");
        }
    }
}
