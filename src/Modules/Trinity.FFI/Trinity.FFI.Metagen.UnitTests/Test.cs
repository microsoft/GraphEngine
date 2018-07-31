using System;
using Xunit;
using Xunit.Abstractions;
using Trinity.FFI.Metagen;
using Trinity.FFI;
using Trinity.Storage.Composite;
using System.Linq;
using Trinity.Storage;
using GraphEngine.Jit;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Microsoft.FSharp.Collections;

namespace Trinity.FFI.Metagen.UnitTests
{
    public delegate int SBGet(IntPtr cell);
    public delegate int LBGet(IntPtr list, int idx);
    public delegate int LCount(IntPtr list);
    public unsafe class Test : IDisposable
    {

        readonly ITestOutputHelper Output;
        readonly char ManglingCode = '_';
        static IStorageSchema Schema;

        public Test(ITestOutputHelper output)
        {
            Global.Initialize();
            Output = output;
            Schema = CompositeStorage.AddStorageExtension("../../../tsl", "Some");
        }

        public void Disp(ICellAccessor c)
        {
            c.ToByteArray().By(_ => String.Join(" ", _)).By(Output.WriteLine);
            Output.WriteLine("");
        }

        [Fact]
        public void TestTSLBytes()
        {

            var c = Trinity.Global.LocalStorage.UseGenericCell(10, TSL.Lib.CellAccessOptions.CreateNewOnCellNotFound, "C1");
            c.SetField<int>("i", 1);
            c.SetField<float>("y", 1.0f);
            
            Disp(c);
         


            var c2 = Trinity.Global.LocalStorage.UseGenericCell(12, TSL.Lib.CellAccessOptions.CreateNewOnCellNotFound, "C2");
            c2.AppendToField<int>("ls", 2);
            Disp(c2);

            var c3 = Trinity.Global.LocalStorage.UseGenericCell(14, TSL.Lib.CellAccessOptions.CreateNewOnCellNotFound, "C3");
            Disp(c3);

         


        }

        [Fact]
        public void TestNewSwigGen()
        {

            var ty_descs = Schema.CellDescriptors.Select(TypeSystem.Make);
            var all_type_collected = FFI.MetaGen.analyzer.collect_type(ty_descs);

            foreach (var e in all_type_collected)
            {
                Output.WriteLine(e.TypeName);
            }
            Output.WriteLine("=====================");


            var all_verbs = FFI.MetaGen.analyzer.generate_chaining_verb(all_type_collected);
            //var num = 0;
            //foreach (var e in all_verbs)
            //{

            //    //var len = e.Item2.Length;
            //    //Output.WriteLine($"{e.Item1.TypeName}, method num: {len}");
            //    //num += len;
            //    e.Item2.Select(_ => _.ToString()).By(_ => String.Join("\n", _)).By(Output.WriteLine);
            //    Output.WriteLine("=====================");
            //}
            //Output.WriteLine($"Total method num: {num}");
            var swig_code = FFI.MetaGen.code_gen.code_gen("", all_verbs);
            Output.WriteLine(swig_code);


        }

        public void Dispose()
        {
            Global.Uninitialize();
            
        }

        
        [Fact]
        public void TestSwigGen()
        {
            var swig = MetaGen.GenerateSwig(ManglingCode).Invoke(Schema);
            swig
                .Each(_ =>
                      _.Item2
                       .Each(swigCodeGetter =>
                             swigCodeGetter
                             .Item2
                             .Invoke("0x00")
                             .By(Output.WriteLine)));
        }

        [Fact]
        public void TestCSharpGen()
        {
            
            var csharp = MetaGen.GenerateCSharp(ManglingCode).Invoke(Schema);
            csharp
                .Take(2)
                .Each(
                _ => _.Item2.Take(2).Each(nested =>
                {
                    var (fnName, Code) = nested;
                    Output.WriteLine($"Get function address by calling this one: {fnName}");
                    Output.WriteLine(Code);
                }));
        }

        [Fact]
        public void TestJitGen()
        {
            var jit = MetaGen.GenerateJit(ManglingCode).Invoke(Schema);
            jit
              .Take(2)
              .Each(
                for_each_type =>
                    for_each_type.Item2
                        .Take(2)
                        .Each(x => $"{{Type: {x.DeclaringType.TypeName}; Verb: {x.Verb.ToString()}}}".By(Output.WriteLine))
              );
        }

        

        [Fact]
        public void TestJitRun()
        {
            int _foo, foo = 1;
            
            var c1 = Global.LocalStorage.NewGenericCell(1, "C1");
            

            c1.SetField("foo", foo);
            Global.LocalStorage.SaveGenericCell(c1.CellId, c1);
            using (var s = Global.LocalStorage.UseGenericCell(c1.CellId))
            {
                Output.WriteLine(s.GetField<string>("foo"));
            }

            
           
            {
                var acc = Helper.LockCell(c1.CellId);
                var jit = MetaGen.GenerateJit(ManglingCode).Invoke(Schema);

                jit.Each(x => 
                    Output.WriteLine(
                        $"{x.Item1.TypeName}: {x.Item2.Select(fnDesc => fnDesc.Verb.ToString()).By(all => String.Join('\n', all))}"));

                var fnDescs =
                    jit
                        .Where(_ => _.Item1.TypeName.Equals("C1"))
                        .First()
                        .By(type_and_fields => type_and_fields.Item2);


                var get_foo_from_c1 =
                    fnDescs
                    .Where(v => v.Verb is Verbs.Verb.ComposedVerb)
                    .First()
                    .By(JitCompiler.CompileFunction)
                    .By(native => 
                        Marshal.GetDelegateForFunctionPointer(
                            native.CallSite, typeof(SBGet)));

                _foo = (int) get_foo_from_c1.DynamicInvoke(acc);
                
                Output.WriteLine(_foo.ToString());
            }
            Assert.Equal(_foo, foo);

        }

        [Fact]
        public void TestSwigFileGen()
        {
            var codeGenerators = MetaGen.CodeGenSwigJit(ManglingCode, Schema);
            codeGenerators.Invoke("moduleName").By(Output.WriteLine);
        }

        [Fact]
        public void TestList()
        {
            var lst = new List<int> { 1, 2, 3 };

            var jit = MetaGen.GenerateJit(ManglingCode).Invoke(Schema);

            var fnDescs = jit.First(_ => _.Item1.TypeName.StartsWith("List"))
                             .By(type_and_fields => type_and_fields.Item2);

            var get_from_lst_int =
                   fnDescs
                   .First(v => v.Verb.Equals(Verbs.Verb.NewComposedVerb(Verbs.Verb.LGet, Verbs.Verb.BGet)))
                   .By(_ => {
                       Output.WriteLine(_.Verb.ToString());
                       return JitCompiler.CompileFunction(_); })
                   .By(native =>
                       Marshal.GetDelegateForFunctionPointer(
                           native.CallSite, typeof(LBGet)));

            var count_lst_int =
                   fnDescs
                   .First(v => v.Verb.Equals(Verbs.Verb.LCount))
                   .By(_ => {
                       Output.WriteLine(_.Verb.ToString());
                       return JitCompiler.CompileFunction(_);
                   })
                   .By(native =>
                       Marshal.GetDelegateForFunctionPointer(
                           native.CallSite, typeof(LCount)));


            Helper.ListToNativeAndThen(
                                lst,
                                subject =>
                                {
                                    var list_1 = (int)get_from_lst_int.DynamicInvoke(subject, 1);
                                    var list_len = (int)count_lst_int.DynamicInvoke(subject);
                                   
                                    Output.WriteLine("list[1]=" + list_1.ToString());
                                    Output.WriteLine("list length =" + list_len.ToString());

                                    Assert.Equal(list_1, lst[1]);
                                    Assert.Equal(list_len, lst.Count());
                                    return 0;
                                });

        }
    }
}
