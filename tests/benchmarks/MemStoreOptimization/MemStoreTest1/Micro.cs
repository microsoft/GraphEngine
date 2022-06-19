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
    public class MicroTests
    {
        [Benchmark]
        public void UseMyCell()
        {
            using (var cell = Global.LocalStorage.UseMyCell(0, CellAccessOptions.CreateNewOnCellNotFound)) { }
        }

        [Benchmark]
        public void UseManipulateMyCell()
        {
            using (var cell = Global.LocalStorage.UseMyCell(0, CellAccessOptions.CreateNewOnCellNotFound)) { ++cell.A; }
        }

        [Benchmark]
        public void UseC2()
        {
            using (var cell = Global.LocalStorage.UseC2(1, CellAccessOptions.CreateNewOnCellNotFound)) { }
        }

        [Benchmark]
        public void UseManipulateC2()
        {
            using (var cell = Global.LocalStorage.UseC2(1, CellAccessOptions.CreateNewOnCellNotFound)) { cell.F12 = "hey"; }
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

    public class MicroTestsNoFill : MicroTests
    {
        [GlobalSetup]
        public void Init()
        {
            TrinityConfig.LoggingLevel = Trinity.Diagnostics.LogLevel.Off;
            Global.LocalStorage.ResetStorage();
            Global.LocalStorage.SaveMyCell(0);
        }
    }

    public class MicroTestsFill : MicroTests
    {
        [Params(1000000, 2000000, 4000000, 8000000)]
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

            Global.LocalStorage.SaveC2(1);
        }
    }
}
