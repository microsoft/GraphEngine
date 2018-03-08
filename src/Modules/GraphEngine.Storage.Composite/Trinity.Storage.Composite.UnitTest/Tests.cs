using System;
using System.Collections.Generic;
using System.IO;
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
        public void LoadSingleTslFile(string content)
        {
            try { Directory.Delete("single_tsl", recursive: true); } catch { }
            Directory.CreateDirectory("single_tsl");
            File.WriteAllText("single_tsl/content.tsl", content);
            CompositeStorage.AddStorageExtension("single_tsl", "single_tsl", "single_tsl");
            Global.LocalStorage.SaveStorage();
            //CompositeStorage.AddStorageExtension()
        }

        public static IEnumerable<object[]> SingleTslContent()
        {
            yield return new[] { @" cell empty_thing { } " };
            yield return new[] { @" cell C1 { int foo; } " };
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
