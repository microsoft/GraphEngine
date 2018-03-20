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

namespace LockPerformanceTest
{
    unsafe internal class Program
    {
        static void Main(string[] args)
        {
            //SaveTwiceTest();
            //EnumerationPerformanceTest();
            LocalStorageParallelSaveEfficiencyTest();
            //RecursiveCellLockTest();
            //RecursiveCellLockTest2();
            //UseCellTest();
            //DeadlockTest();
            //DeadlockTest2();
            //Deadlock_Savestorage_Test();
            //Deadlock_Enum_test();
        }

        private static void __parallel_save(int thread_cnt, long total_cell_cnt)
        {
            Global.LocalStorage.ResetStorage();
            TrinityConfig.LoggingLevel = LogLevel.Info;

            List<Thread> threads = new List<Thread>();

            //int thread_cnt = Environment.ProcessorCount;
            int cell_size = 134;
            int cnt_per_thread = (int)(total_cell_cnt / thread_cnt / cell_size);
            Random r = new Random((int)19900921);
            int[][] cell_ids = Enumerable.Range(0, thread_cnt)
                .Select(_ => Enumerable.Range(0, cnt_per_thread)
                    .Select(__ => r.Next()).ToArray())
                .ToArray();

            Log.WriteLine(LogLevel.Warning, "Start testing parallel save [{0} threads, {1} cells]", thread_cnt, total_cell_cnt);
            Stopwatch sw = Stopwatch.StartNew();

            for (int i=0; i < thread_cnt; ++i)
            {
                int thread_id = i;
                threads.Add(new Thread(() =>
                {
                    byte[] bytes = new byte[cell_size];

                    for (int n = 0; n < cnt_per_thread; ++n)
                    {
                        Global.LocalStorage.SaveCell(
                            cell_ids[thread_id][n]
                            , bytes);
                    }
                }));

                threads.Last().Start();
            }

            threads.ForEach(x => x.Join());

            sw.Stop();

            Log.WriteLine(LogLevel.Warning, "Parallel save time cost = {0}ms", sw.ElapsedMilliseconds);
            Log.WriteLine(LogLevel.Warning, "Total cell count = {0}", Global.LocalStorage.CellCount);
            Log.WriteLine(LogLevel.Warning, "Total cell size = {0}", Global.LocalStorage.TotalCellSize);

            sw.Restart();
            for (int i=0; i < thread_cnt; ++i)
            {
                int thread_id = i;
                threads.Add(new Thread(() =>
                {
                    for (int n = 0; n < cnt_per_thread; ++n)
                    {
                        long cellid = cell_ids[thread_id][n];
                        int size; ushort type; byte* cellPtr; int entryIndex;
                        Global.LocalStorage.GetLockedCellInfo(cellid, out size, out type, out cellPtr, out entryIndex);
                        Global.LocalStorage.ReleaseCellLock(cellid, entryIndex);
                    }
                }));

                threads.Last().Start();
            }

            threads.ForEach(x => x.Join());

            sw.Stop();

            Log.WriteLine(LogLevel.Warning, "Parallel lock cell time cost = {0}ms", sw.ElapsedMilliseconds);
        }
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

        public static void LocalStorageParallelSaveEfficiencyTest()
        {
            TrinityConfig.LoggingLevel = LogLevel.Warning;
            long total_cell_cnt = 1L << 29;
            __parallel_save(1, total_cell_cnt);
            __parallel_save(2, total_cell_cnt);
            for (int thread_cnt = 4; thread_cnt < Environment.ProcessorCount * 4; thread_cnt += 2)
            {
                __parallel_save(thread_cnt, total_cell_cnt);
            }
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
