using System;
using System.Runtime.InteropServices;
using Trinity.Extension;
namespace TSLExamples
{
    
    unsafe static class Program
    {
        public delegate void DEL_Cell_C1_get_lst(void* subject, out void* @object);
        public static void FUNC_Cell_C1_get_lst(void* subject, out void* @object)
        {
            var h = (C1) GCHandle.FromIntPtr((IntPtr)subject).Target;

            var field = ((C1)GCHandle.FromIntPtr((IntPtr)subject).Target).bar;
            @object = GCHandle.ToIntPtr(GCHandle.Alloc(field)).ToPointer();
        }
        public static DEL_Cell_C1_get_lst INST_Cell_C1_get_lst = FUNC_Cell_C1_get_lst;

        public static int ADDR_Cell_C1_get_lst => Marshal.GetFunctionPointerForDelegate(INST_Cell_C1_get_lst).ToInt32();



        public delegate void DEL_Cell_C1_set_lst(void* subject, void* @object);
        public static void FUNC_Cell_C1_set_lst(void* subject, void* @object)
        {
            var src = ((C1)GCHandle.FromIntPtr((IntPtr)subject).Target);
            src.bar = ((string)GCHandle.FromIntPtr((IntPtr)@object).Target);
        }
        public static int ADDR_Cell_C1_set_lst() =>
            Marshal.GetFunctionPointerForDelegate(GCHandle.Alloc((DEL_Cell_C1_get_lst)FUNC_Cell_C1_get_lst)).ToInt32();

    }
}
