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
            Assert.False(Commands.TSLCodeGenCmd(""));
        }

        [Fact]
        public void FindDotnet()
        {
            Assert.False(Commands.DotNetBuildCmd(""));
        }

        [Fact]
        public void LoadEmptyTslFile()
        {
            Global.LocalStorage.ResetStorage();
            Directory.CreateDirectory("single_tsl");
            File.WriteAllText("single_tsl/content.tsl", "");
            Assert.Throws<AsmLoadException>(() => CompositeStorage.AddStorageExtension("single_tsl", "SingleTsl"));
            Global.LocalStorage.SaveStorage();
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
        }

        [Theory]
        [MemberData(nameof(SingleTslContent))]
        public void LoadSingleTslFileTwice(string content, int cellTypeCount)
        {
            Global.LocalStorage.ResetStorage();
            Directory.CreateDirectory("single_tsl");
            File.WriteAllText("single_tsl/content.tsl", content);
            CompositeStorage.AddStorageExtension("single_tsl", "SingleTsl");
            CompositeStorage.AddStorageExtension("single_tsl", "SingleTsl");
            Assert.Equal(cellTypeCount * 2, Global.StorageSchema.CellDescriptors.Count());
            Global.LocalStorage.SaveStorage();
            //CompositeStorage.AddStorageExtension()
        }

        [Theory]
        [MemberData(nameof(BadSingleTslContent))]
        public void LoadBadTsl(string content)
        {
            Global.LocalStorage.ResetStorage();
            Directory.CreateDirectory("single_tsl");
            File.WriteAllText("single_tsl/content.tsl", content);
            Assert.Throws<TSLCodeGenException>(() => CompositeStorage.AddStorageExtension("single_tsl", "BadTsl"));
            //CompositeStorage.AddStorageExtension()
        }

        [Theory]
        [MemberData(nameof(MultiTslContent))]
        public void LoadMultiTsl(int expect, params string[] content)
        {
            Global.LocalStorage.ResetStorage();
            Directory.CreateDirectory("multi_good");
            Directory.GetFiles("multi_good").Each(File.Delete);
            for (int i = 0; i<content.Length; ++i)
            {
                File.WriteAllText($"multi_good/{i}.tsl", content[i]);
            }
            CompositeStorage.AddStorageExtension($"multi_good", $"Tsl");
            Assert.Equal(expect, Global.StorageSchema.CellDescriptors.Count());
        }

        [Theory]
        [MemberData(nameof(MultiTslContent))]
        public void LoadMultiTsl_Separatedly(int expect, params string[] content)
        {
            Global.LocalStorage.ResetStorage();
            for (int i = 0; i<content.Length; ++i)
            {
                Directory.CreateDirectory($"multi_good{i}");
                Directory.GetFiles($"multi_good{i}").Each(File.Delete);
                File.WriteAllText($"multi_good{i}/content.tsl", content[i]);
                CompositeStorage.AddStorageExtension($"multi_good{i}", $"Tsl{i}");
            }
            Assert.Equal(expect, Global.StorageSchema.CellDescriptors.Count());
        }

        [Theory]
        [MemberData(nameof(MultiTslWithBadContent))]
        public void LoadMultiTslWithBad(int expect, params string[] content)
        {
            Global.LocalStorage.ResetStorage();
            for (int i = 0; i<content.Length; ++i)
            {
                Directory.CreateDirectory($"multi{i}");
                Directory.GetFiles($"multi{i}").Each(File.Delete);
                File.WriteAllText($"multi{i}/content.tsl", content[i]);
                try { CompositeStorage.AddStorageExtension($"multi{i}", $"Tsl{i}"); }
                catch { }
            }
            Assert.Equal(expect, Global.StorageSchema.CellDescriptors.Count());
        }

        [Fact]
        public void tsl3()
        {
            Global.LocalStorage.ResetStorage();
            CompositeStorage.AddStorageExtension($"./tsl3", $"BenchMark");
        }

        public static IEnumerable<object[]> MultiTslContent()
        {
            yield return new object[] { 1, @" cell empty_thing { } " };
            yield return new object[] { 2, @" cell empty_thing { } ", @" cell empty_thing_2 { } " };
            yield return new object[] { 3, @" cell C1 { } ", @" cell C2 { } ", @" cell C3 { } " };
        }

        public static IEnumerable<object[]> MultiTslWithBadContent()
        {
            yield return new object[] { 1, @" cell empty_thing { } " };
            yield return new object[] { 1, @" cell empty_thing { } ", @" BAD " };
            yield return new object[] { 2, @" cell C1 { } ", @" BAD ", @" cell C1 { } " };
            yield return new object[] { 2, @" BAD ", @" cell C1 { } ", @" cell C1 { } " };
            yield return new object[] { 1, @" BAD ", @" cell C1 { } ", @" BAD " };
        }

        public static IEnumerable<object[]> SingleTslContent()
        {
            yield return new object[] { @" cell empty_thing { } ", 1 };
            yield return new object[] { @" cell C1 { int foo; } ", 1 };
            yield return new object[] { @" cell C1 { int foo; } cell C2 { int bar;} ", 2 };
        }

        public static IEnumerable<object[]> BadSingleTslContent()
        {
            yield return new object[] { @" obviously not a TSL. " };
            yield return new object[] { @" cell cell cell C1 { int foo; } " };
            yield return new object[] { @" protocol P1 { } cell C2 { int bar;} " };
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
