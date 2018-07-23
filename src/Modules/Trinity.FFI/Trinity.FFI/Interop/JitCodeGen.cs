using System;
using System.Collections.Generic;
using System.Text;
using Trinity.FFI.Metagen;
using Trinity.Storage.Composite;
using GraphEngine.Jit;
using System.Linq;

namespace Trinity.FFI
{
    [FFIPrefix("jit")]
    public static class JitTools
    {
        internal static char ManglingCode => '_';

        [FFIExport]
        public static string SwigGen(string directory, string moduleName)
        {
            var schema = CompositeStorage.AddStorageExtension(directory, moduleName);
            var typeIdMap = schema.CellDescriptors.ToDictionary(it => it.Type.AssemblyQualifiedName, it => it.CellType);
            var tyDescs = schema.CellDescriptors.Select(GraphEngine.Jit.TypeSystem.Make);
            var collectedTydescs = MetaGen.analyzer.collect_type(tyDescs);
            var chains = MetaGen.analyzer.generate_chaining_verb(collectedTydescs);
            var result = MetaGen.code_gen.code_gen(typeIdMap, moduleName, chains);
            return result;
        }

    }
}
