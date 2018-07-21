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
            var typeIdMap = schema.CellDescriptors.ToDictionary(it => it.Type.AssemblyQualifiedName, it => it.CellType);
            var tyDescs = schema.CellDescriptors.Select(GraphEngine.Jit.TypeSystem.Make);
            var collectedTydescs = MetaGen.analyzer.collect_type(tyDescs);
            var chains = MetaGen.analyzer.generate_chaining_verb(collectedTydescs);
            var result = MetaGen.code_gen.code_gen(typeIdMap, ModuleName, chains);
            return result;
        }

    }
}
