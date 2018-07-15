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
            void Output(string s){
                Console.WriteLine(s);
            }


            var ty_descs = Schema.CellDescriptors.Select(GraphEngine.Jit.TypeSystem.Make);

            var all_type_collected = Trinity.FFI.MetaGen.analyzer.collect_type(ty_descs);

            foreach (var e in all_type_collected)
            {
                Output(e.TypeName);
            }
            Output("=====================");


            var all_verbs = Trinity.FFI.MetaGen.analyzer.generate_chaining_verb(all_type_collected);
            //var num = 0;
            //foreach (var e in all_verbs)
            //{

            //    var len = e.Item2.Length;
            //    Output.WriteLine($"{e.Item1.TypeName}, method num: {len}");
            //    num += len;
            //    e.Item2.Select(_ => _.ToString()).By(_ => String.Join("\n", _)).By(Output.WriteLine);
            //    Output.WriteLine("=====================");
            //}
            //Output.WriteLine($"Total method num: {num}");
            var (a, b) = Trinity.FFI.MetaGen.code_gen.code_gen(all_verbs);
            foreach (var code in a.Zip(b, (l, r) => $"{l}\n{r}\n"))
            {
                Output(code);
            }
        }

    }

    public class M
    {

        public static void Main()
        {

            var t = new Test();
            t.Run();
        }
    }

}


