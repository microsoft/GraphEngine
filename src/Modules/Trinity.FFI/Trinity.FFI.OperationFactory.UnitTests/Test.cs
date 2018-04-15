using System;
using Xunit;
using Xunit.Abstractions;
using Trinity.FFI.OperationFactory;
using Trinity.Storage.Composite;
using System.Linq;
using Trinity.Storage;

namespace Trinity.FFI.OperationFactory.UnitTests
{
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
                    var (verb, (fnName, Code)) = nested;
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
                    for_each_type
                        .Item2
                        .SelectMany(_ => _)
                        .Take(2)
                        .Each(x => $"{{Type: {x.Item2.DeclaringType.TypeName}; Verb: {x.Item2.Verb.ToString()}}}".By(Output.WriteLine))
              );
        }
    }
}
