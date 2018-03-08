using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace Trinity.Storage.Composite.UnitTest
{
    public class Tests : IDisposable
    {
        [Fact]
        public void FindTslCodegen()
        {
            Assert.True(Commands.TSLCodeGenCmd(""));
        }

        [Fact]
        public void FindDotnet()
        {
            Assert.True(Commands.DotNetBuildCmd(""));
        }

        [Theory]
        [MemberData(nameof(SingleTslContent))]
        public void LoadSingleTslFile(string content, int cellTypeCount)
        {
            Global.LocalStorage.ResetStorage();
            Directory.CreateDirectory("single_tsl");
            File.WriteAllText("single_tsl/content.tsl", content);
            CompositeStorage.AddStorageExtension("single_tsl", "SingleTsl");
            Assert.Equal(cellTypeCount, Global.StorageSchema.CellDescriptors.Count());
            Global.LocalStorage.SaveStorage();
            //CompositeStorage.AddStorageExtension()
        }

        public static IEnumerable<object[]> SingleTslContent()
        {
            yield return new object[] { @" cell empty_thing { } " , 1};
            yield return new object[] { @" cell C1 { int foo; } " , 1};
            yield return new object[] { @" cell C1 { int foo; } cell C2 { int bar;} " , 2};
        }

        public Tests()
        {
            Global.Initialize();
        }

        public void Dispose()
        {
            Global.Uninitialize();
        }
    }
}
