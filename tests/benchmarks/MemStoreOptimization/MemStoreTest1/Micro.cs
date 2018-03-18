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
    public class MicroTestsNoFill
    {
        [GlobalSetup]
        public void Init()
        {
            TrinityConfig.LoggingLevel = Trinity.Diagnostics.LogLevel.Off;
            Global.LocalStorage.ResetStorage();
            Global.LocalStorage.SaveMyCell(cell);
        }

        [Benchmark]
        public void Use()
        {
            using (var cell = Global.LocalStorage.UseMyCell(0, CellAccessOptions.CreateNewOnCellNotFound))
            {
            }
        }

        [Benchmark]
        public void UseManipulate()
        {
            using (var cell = Global.LocalStorage.UseMyCell(0, CellAccessOptions.CreateNewOnCellNotFound))
            {
                ++cell.A;
            }
        }

        [Benchmark]
        public MyCell LoadMyCell()
        {
            return Global.LocalStorage.LoadMyCell(0);
        }

        public MyCell cell = new MyCell();

        [Benchmark]
        public bool SaveMyCell()
        {
            return Global.LocalStorage.SaveMyCell(cell);
        }

        [Benchmark]
        public TrinityErrorCode LoadCellBinary()
        {
            return Global.LocalStorage.LoadCell(0, out _, out _);
        }

        [Benchmark]
        public bool Contains()
        {
            return Global.LocalStorage.Contains(0);
        }

        [Benchmark]
        public CellType GetCellType()
        {
            return Global.LocalStorage.GetCellType(0);
        }
    }

    [ClrJob, CoreJob]
    [MinColumn, MaxColumn]
    [MarkdownExporter, RPlotExporter]
    public class MicroTestsFill
    {
        [Params(10000000, 20000000, 40000000, 80000000)]
        public int TestSize { get; set; }

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

        [Benchmark]
        public void Use()
        {
            using (var cell = Global.LocalStorage.UseMyCell(0, CellAccessOptions.CreateNewOnCellNotFound))
            {
            }
        }

        [Benchmark]
        public void UseManipulate()
        {
            using (var cell = Global.LocalStorage.UseMyCell(0, CellAccessOptions.CreateNewOnCellNotFound))
            {
                ++cell.A;
            }
        }

        [Benchmark]
        public MyCell LoadMyCell()
        {
            return Global.LocalStorage.LoadMyCell(0);
        }

        public MyCell cell = new MyCell();

        [Benchmark]
        public bool SaveMyCell()
        {
            return Global.LocalStorage.SaveMyCell(cell);
        }

        [Benchmark]
        public TrinityErrorCode LoadCellBinary()
        {
            return Global.LocalStorage.LoadCell(0, out _, out _);
        }

        [Benchmark]
        public bool Contains()
        {
            return Global.LocalStorage.Contains(0);
        }

        [Benchmark]
        public CellType GetCellType()
        {
            return Global.LocalStorage.GetCellType(0);
        }
    }
}
