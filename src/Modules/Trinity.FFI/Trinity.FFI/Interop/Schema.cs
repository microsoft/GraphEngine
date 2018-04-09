using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Trinity.Core.Lib;
using Trinity.Storage;
using static GraphEngine.Jit.JitNativeInterop;

namespace Trinity.FFI.Interop
{
    [FFIPrefix("schema_")]
    internal static unsafe class Schema
    {
        [FFIExport]
        public static TrinityErrorCode get(out NativeTypeDescriptor[] schema, out int size)
        {
            try
            {
                schema = Global.StorageSchema.CellDescriptors.Select(Make).ToArray();
                size = schema.Length;
                return TrinityErrorCode.E_SUCCESS;
            }
            catch (Exception e)
            {
                PrintException(e);
                schema = null;
                size = 0;
                return TrinityErrorCode.E_FAILURE;
            }
        }

        private static void PrintException(Exception e)
        {
            Console.WriteLine(e.ToString());
            Console.WriteLine();
            if (e.InnerException != null)
            {
                Console.WriteLine("Inner exception: ");
                PrintException(e.InnerException);
            }
        }
    }
}
