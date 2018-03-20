using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity;
using Trinity.TSL.Lib;
using System.Diagnostics;
using MemStoreTest1;
using BenchmarkDotNet.Attributes.Jobs;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.CsProj;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Exporters;

namespace LockOverheadTest
{
    public unsafe class Program
    {
        static void Main(string[] args)
        {
            Func<RunStrategy, IConfig> cfg = strategy => ManualConfig.Create(DefaultConfig.Instance)
                    .With(Job.Default.With(CsProjCoreToolchain.NetCoreApp21).With(strategy))
                    .With(Job.Default.With(CsProjClassicNetToolchain.Current.Value).With(strategy))
                    .With(DefaultExporters.Csv)
                    .With(DefaultExporters.Json)
                    .With(DefaultExporters.Html)
                    .With(DefaultExporters.RPlot);

            BenchmarkRunner.Run<MicroTestsNoFill>(cfg(RunStrategy.Throughput));
            BenchmarkRunner.Run<MicroTestsFill>(cfg(RunStrategy.Throughput));
            BenchmarkRunner.Run<MacroTestsNoFill>(cfg(RunStrategy.Monitoring));
            BenchmarkRunner.Run<MacroTestsFill>(cfg(RunStrategy.Monitoring));
        }
    }
}
