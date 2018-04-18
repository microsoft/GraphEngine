using System;
using System.Collections.Generic;
using System.Text;
using Trinity.FFI.OperationFactory;
using Trinity.Storage;
using GraphEngine.Jit;

namespace Trinity.FFI
{
    public static class JitTools
    {
        internal static char ManglingCode => '_';
        public static string SwigGen(IStorageSchema schema, string ModuleName)
        {
            var codeGenerators = MetaGen.CodeGenSwigJit(ManglingCode, schema);
            return codeGenerators.Invoke(ModuleName);
        }

    }
}
