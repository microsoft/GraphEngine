using System;
using Xunit;
using Xunit.Abstractions;
using Trinity.FFI.Metagen;
using Trinity.Storage.Composite;
using System.Linq;
using Trinity.Storage;
using GraphEngine.Jit;
using System.Runtime.InteropServices;



namespace Trinity.FFI.Metagen.UnitTests
{
    public delegate int TestFnType(IntPtr cell);
    public class Test : IDisposable
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

        public void Dispose()
        {
            Global.Uninitialize();
        }

        
        [Fact]
        public void TestSwigGen()
        {
            var swig = MetaGen.GenerateSwig(ManglingCode).Invoke(Schema);
            swig
                .Take(2)
                .Each(_ =>
                      _.Item2
                       .Take(2)
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
                var fnDescs =
                    jit
                        .SelectMany(
                            type_and_fields =>
                                type_and_fields.Item2);

                var get_foo_from_c1 =
                    fnDescs
                    .First()
                    .By(
                        _ => 
                        JitCompiler.CompileFunction(
                            new Verbs.FunctionDescriptor(
                                _.DeclaringType,
                                Verbs.Verb.NewComposedVerb(_.Verb, Verbs.Verb.BGet)
                                )))
                    .By(native => 
                        Marshal.GetDelegateForFunctionPointer(
                            native.CallSite, typeof(TestFnType)));

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
    }
}
