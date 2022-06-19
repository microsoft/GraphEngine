using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Attributes.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trinity;
using Trinity.Diagnostics;
using Trinity.Storage;

namespace memStoreTest2
{
    public class ParallelBenchmark
    {
        [ParamsSource(nameof(ThreadCountValue))]
        public int ThreadCount { get; set; }

        [Params(1*MB, 512*MB)]
        public long TotalSize { get; set; }

        [Params(17, 134)]
        public int CellSize { get; set; }

        public IEnumerable<int> ThreadCountValue
        {
            get
            {
                yield return 1;
                yield return 2;
                for (int thread_cnt = 4; thread_cnt <= Environment.ProcessorCount * 4; thread_cnt += 2)
                {
                    yield return thread_cnt;
                }
            }
        }

        public const long MB = (1024 * 1024);

        private int[][] m_CellIds;
        private int m_CntPerThread;

        [IterationSetup(Target = nameof(ParallelSave))]
        public void SetupCellIds()
        {
            TrinityConfig.LoggingLevel = LogLevel.Off;
            Global.LocalStorage.ResetStorage();

            m_CntPerThread = (int)(TotalSize / ThreadCount / CellSize);
            Random r = new Random((int)19900921);
            m_CellIds = Enumerable.Range(0, ThreadCount)
                .Select(_ => Enumerable.Range(0, m_CntPerThread)
                    .Select(__ => r.Next()).ToArray())
                .ToArray();
        }

        [IterationSetup(Target = nameof(ParallelUse))]
        public void SetupCells()
        {
            SetupCellIds();
            ParallelSave();
        }

        [Benchmark]
        public void ParallelSave()
        {
            List<Thread> threads = new List<Thread>();
            for (int i = 0; i < ThreadCount; ++i)
            {
                int thread_id = i;
                Thread t = new Thread(() =>
                {
                    byte[] bytes = new byte[CellSize];

                    for (int n = 0; n < m_CntPerThread; ++n)
                    {
                        Global.LocalStorage.SaveCell(
                            m_CellIds[thread_id][n]
                            , bytes);
                    }
                });
                t.Start();
                threads.Add(t);
            }
            threads.ForEach(x => x.Join());
        }

        [Benchmark]
        public unsafe void ParallelUse()
        {
            List<Thread> threads = new List<Thread>();
            for (int i = 0; i < ThreadCount; ++i)
            {
                int thread_id = i;
                Thread t = new Thread(() =>
                {
                    for (int n = 0; n < m_CntPerThread; ++n)
                    {
                        long cellid = m_CellIds[thread_id][n];
                        int size; ushort type; byte* cellPtr; int entryIndex;
                        Global.LocalStorage.GetLockedCellInfo(cellid, out size, out type, out cellPtr, out entryIndex);
                        Global.LocalStorage.ReleaseCellLock(cellid, entryIndex);
                    }
                });
                t.Start();
                threads.Add(t);
            }
            threads.ForEach(x => x.Join());
        }
    }
}
