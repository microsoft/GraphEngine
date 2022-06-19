using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity;
using Trinity.Storage;
using System.Diagnostics;
using System.Threading;
using Trinity.Diagnostics;
using memStoreTest2;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Toolchains.CsProj;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Running;

namespace LockPerformanceTest
{
    unsafe internal class Program
    {
        static void Main(string[] args)
        {
            Func<RunStrategy, IConfig> cfg = strategy => ManualConfig.Create(DefaultConfig.Instance)
                    .With(Job.Default.With(CsProjCoreToolchain.NetCoreApp20).With(strategy).WithGcServer(true))
                    .With(Job.Default.With(CsProjClassicNetToolchain.Current.Value).With(strategy).WithGcServer(true))
                    .With(DefaultExporters.Html)
                    .With(DefaultExporters.RPlot);

            BenchmarkRunner.Run<ParallelBenchmark>(cfg(RunStrategy.Monitoring));
        }

        // TODO move simple tests as UTs

        private static void RecursiveCellLockTest2()
        {
            TrinityConfig.LoggingLevel = LogLevel.Verbose;
            Global.LocalStorage.SaveMyCell(0);

            Task.Factory.StartNew(() =>
            {
                List<MyCell_Accessor> accessors = new List<MyCell_Accessor>();

                for (int i=0; i<1024; ++i)
                {
                    Global.LocalStorage.UseMyCell(0);
                    Console.WriteLine(i);
                }

                //accessors.ForEach(c => c.Dispose());
            }).Wait();
        }

        private static void Deadlock_Enum_test()
        {
            TrinityConfig.LoggingLevel = LogLevel.Verbose;
            using (var c0 = Global.LocalStorage.UseMyCell(0, Trinity.TSL.Lib.CellAccessOptions.CreateNewOnCellNotFound))
            {
                foreach (var cell in Global.LocalStorage.MyCell_Accessor_Selector())
                {

                }
            }
        }

        private static void DeadlockTest2()
        {
            //TrinityConfig.LoggingLevel = LogLevel.Verbose;
            int deadlock_ring_size = 23;

            for (int i=0; i<deadlock_ring_size; ++i)
                Global.LocalStorage.SaveMyCell(i);

            List<Task> tasks = new List<Task>();

            for (int i=0; i<deadlock_ring_size; ++i)
            {
                var tid = i;
                var nid = (i+1) % deadlock_ring_size;
                var t = Task.Factory.StartNew(() =>
                {
                    using (var c0 = Global.LocalStorage.UseMyCell(tid))
                    {
                        Thread.Sleep(1000);
                        using (var c1 = Global.LocalStorage.UseMyCell(nid))
                        {
                            Console.WriteLine(tid);
                        }
                    }
                });

                tasks.Add(t);
            }

            tasks.ForEach(t => t.Wait());
        }

        private static void Deadlock_Savestorage_Test()
        {
            TrinityConfig.LoggingLevel = LogLevel.Verbose;
            using (var c0 = Global.LocalStorage.UseMyCell(0, Trinity.TSL.Lib.CellAccessOptions.CreateNewOnCellNotFound))
            {
                Console.WriteLine(
                    Global.LocalStorage.SaveStorage());
            }
        }

        private static void DeadlockTest()
        {
            Global.LocalStorage.SaveMyCell(0);
            Global.LocalStorage.SaveMyCell(1);
            var t1 = Task.Factory.StartNew(() =>
            {
                using (var c0 = Global.LocalStorage.UseMyCell(0))
                {
                    Thread.Sleep(1000);
                    using (var c1 = Global.LocalStorage.UseMyCell(1))
                    {
                        Console.WriteLine("!");
                    }
                }
            });
            var t2 = Task.Factory.StartNew(() =>
            {
                using (var c0 = Global.LocalStorage.UseMyCell(1))
                {
                    Thread.Sleep(1000);
                    using (var c1 = Global.LocalStorage.UseMyCell(0))
                    {
                        Console.WriteLine("!");
                    }
                }
            });
            t1.Wait();
            t2.Wait();

        }

        private static void UseCellTest()
        {
            Global.LocalStorage.SaveMyCell(0);
            Global.LocalStorage.SaveMyCell(1);

            var t = Task.Factory.StartNew(() =>
            {
                using (var c1 = Global.LocalStorage.UseMyCell(0))
                {
                }
                using (var c2 = Global.LocalStorage.UseMyCell(1))
                {
                }
            });

            t.Wait();
        }


        private static void RecursiveCellLockTest()
        {
            Global.LocalStorage.SaveMyCell(0);

            Task.Factory.StartNew(() =>
            {
                using (var c1 = Global.LocalStorage.UseMyCell(0))
                using (var c2 = Global.LocalStorage.UseMyCell(0))
                using (var c3 = Global.LocalStorage.UseMyCell(0))
                using (var c4 = Global.LocalStorage.UseMyCell(0))
                {
                    Console.WriteLine(c1.A);
                    Console.WriteLine(c2.A);
                    Console.WriteLine(c3.A);
                    Console.WriteLine(c4.A);
                }
            }).Wait();



        }

        private static void SaveTwiceTest()
        {
            byte[] content = new byte[12];
            Global.LocalStorage.SaveCell(0, content);
            Global.LocalStorage.SaveCell(0, content);
        }

        private static void EnumerationPerformanceTest()
        {
            Stopwatch sw = new Stopwatch();

            sw.Restart();
            for (int i=0; i<1000000; ++i)
            {
                Global.LocalStorage.SaveMyCell(i, i);
            }
            sw.Stop();

            Console.WriteLine("Save cell {0} ms", sw.ElapsedMilliseconds);

            int sum = 0;

            sw.Restart();
            foreach (var cell_accessor in Global.LocalStorage.MyCell_Accessor_Selector())
            {
                sum += cell_accessor.A;
            }
            sw.Stop();

            Console.WriteLine("Enumeration {0} ms", sw.ElapsedMilliseconds);

            Console.WriteLine(sum);
        }
    }
}
