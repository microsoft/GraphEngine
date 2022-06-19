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
using System.Reflection;

namespace LockOverheadTest
{
    public unsafe class Program
    {
        static void Main(string[] args)
        {
            Func<RunStrategy, IConfig> cfg = strategy => ManualConfig.Create(DefaultConfig.Instance)
                    .With(Job.Default.With(CsProjCoreToolchain.NetCoreApp20).With(strategy).WithGcServer(true))
                    .With(Job.Default.With(CsProjClassicNetToolchain.Current.Value).With(strategy).WithGcServer(true))
                    .With(DefaultExporters.Html)
                    .With(DefaultExporters.RPlot);

            if (!args.Any())
            {
                args = new[] { "-micro_nofill", "-micro_fill", "-macro_nofill", "-macro_fill" };
            }

            if(args.Contains("-micro_nofill")) BenchmarkRunner.Run<MicroTestsNoFill>(cfg(RunStrategy.Throughput));
            if(args.Contains("-micro_fill")) BenchmarkRunner.Run<MicroTestsFill>(cfg(RunStrategy.Throughput));
            if(args.Contains("-macro_nofill")) BenchmarkRunner.Run<MacroTestsNoFill>(cfg(RunStrategy.Monitoring));
            if(args.Contains("-macro_fill")) BenchmarkRunner.Run<MacroTestsFill>(cfg(RunStrategy.Monitoring));
        }
    }
}
