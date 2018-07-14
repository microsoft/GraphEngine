using System;
using Trinity.FFI.Metagen;
using Trinity;
using Trinity.FFI;
using Trinity.Storage.Composite;
using System.Linq;
using Trinity.Storage;
using GraphEngine.Jit;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace CSFunctionFactory
{
    public class Test 
    {


        readonly char ManglingCode = '_';
        static IStorageSchema Schema;

        public Test()
        {
            Global.Initialize();
 
            Schema = CompositeStorage.AddStorageExtension("../../../tsl", "Some");
        }

        public void Run()
        {
            var ty_descs = Schema.CellDescriptors.Select(GraphEngine.Jit.TypeSystem.Make);

            var all_type_collected = Trinity.FFI.MetaGen.analyzer.collect_type(ty_descs);

            foreach (var e in all_type_collected)
            {
                Console.WriteLine(e.TypeName);
            }
            Console.WriteLine("=====================");

            var all_chaining_ty_descs = FFI.MetaGen.analyzer.calc_chaining(all_type_collected);

            foreach (var e in all_chaining_ty_descs)
            {
                Console.WriteLine(e.First().TypeName);
            }
            Console.WriteLine("=====================");

            Console.WriteLine(all_chaining_ty_descs.ToString());
        }

    }

}


