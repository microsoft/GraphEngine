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
    public class MacroTests
    {
        [Params(10000000)]
        public int TestSize { get; set; }


        [Benchmark]
        public void CreateMyCell()
        {
            for (int i = 0; i<TestSize; ++i)
            {
                using (var cell = Global.LocalStorage.UseMyCell(i, CellAccessOptions.CreateNewOnCellNotFound))
                {
                }
            }
        }


        [Benchmark]
        public void Contains()
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
        [GlobalSetup]
        public void Init()
        {
            TrinityConfig.LoggingLevel = Trinity.Diagnostics.LogLevel.Off;
            Global.LocalStorage.ResetStorage();
            Global.LocalStorage.SaveMyCell(0);
        }
    }

    public class MacroTestsFill : MacroTests
    {
        [Params(10000000, 20000000, 40000000, 80000000)]
        public int FillSize { get; set; }

        [GlobalSetup]
        public void Init()
        {
            TrinityConfig.LoggingLevel = Trinity.Diagnostics.LogLevel.Off;
            Global.LocalStorage.ResetStorage();
            Parallel.For(0, FillSize, i =>
            {
                Global.LocalStorage.SaveMyCell(i, new MyCell());
            });

        }

        [Benchmark]
        public void LoadMyCell()
        {
            for (int i = 0; i<TestSize; ++i)
            {
                Global.LocalStorage.LoadMyCell(i);
            }
        }
    }
}
