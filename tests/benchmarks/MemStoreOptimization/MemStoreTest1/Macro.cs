using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes.Jobs;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Configs;
using Trinity;
using Trinity.TSL.Lib;

namespace MemStoreTest1
{
    [MarkdownExporter, RPlotExporter, HtmlExporter, CsvExporter]
    public class MacroTests
    {
        [Params(10000000, 20000000, 40000000, 80000000)]
        public int TestSize { get; set; }


        [Benchmark]
        public void LoadMyCellMacro()
        {
            for (int i = 0; i<TestSize; ++i)
            {
                Global.LocalStorage.LoadMyCell(i);
            }
        }


        [Benchmark]
        public void CreateMyCellMacro()
        {
            for (int i = 0; i<TestSize; ++i)
            {
                using (var cell = Global.LocalStorage.UseMyCell(i, CellAccessOptions.CreateNewOnCellNotFound))
                {
                }
            }
        }


        [Benchmark]
        public void ContainsMacro()
        {
            for (int i = 0; i<TestSize; ++i)
            {
                Global.LocalStorage.Contains(i);
            }
        }


        [Benchmark]
        public void GetCellTypeMacro()
        {
            for (int i = 0; i<TestSize; ++i)
            {
                Global.LocalStorage.GetCellType(i);
            }
        }

        [Benchmark]
        public void IterateAccessor()
        {
            foreach (var cellAccessor in Global.LocalStorage.MyCell_Accessor_Selector()) { }
        }

        [Benchmark]
        public void Iterate()
        {
            foreach (var cellInfo in Global.LocalStorage) { }
        }
    }

    public class MacroTestsNoFill : MacroTests
    {
        [IterationSetup]
        public void Init()
        {
            TrinityConfig.LoggingLevel = Trinity.Diagnostics.LogLevel.Off;
            Global.LocalStorage.ResetStorage();
        }

    }

    public class MacroTestsFill : MacroTests
    {
        [GlobalSetup]
        public void Init()
        {
            TrinityConfig.LoggingLevel = Trinity.Diagnostics.LogLevel.Off;
            Global.LocalStorage.ResetStorage();
            Parallel.For(0, TestSize, i =>
            {
                Global.LocalStorage.SaveMyCell(i, new MyCell());
            });

        }
    }
}
