using System;
using System.Collections.Generic;
using System.Text;
using Trinity.FFI.Metagen;
using Trinity.Storage;
using GraphEngine.Jit;
using System.Linq;

namespace Trinity.FFI
{
    public static class JitTools
    {
        internal static char ManglingCode => '_';
        public static string SwigGen(IStorageSchema schema, string ModuleName)
        {
            var ty_descs = schema.CellDescriptors.Select(GraphEngine.Jit.TypeSystem.Make);
            var collected_tydescs = MetaGen.analyzer.collect_type(ty_descs);
            var chains = MetaGen.analyzer.generate_chaining_verb(collected_tydescs);
            var result = MetaGen.code_gen.code_gen(ModuleName, chains);
            return result;
        }

    }
}
