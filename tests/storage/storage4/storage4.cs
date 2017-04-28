using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Security;
using System.Runtime.ExceptionServices;
using Trinity;
using System.Globalization;
using NUnit.Framework;
using Trinity.Storage;

namespace storage4
{
    unsafe class Test
    {
        #region Parameters
        /// <summary>
        /// This seed is used to generate a random seed for each thread
        /// </summary>
        int RandomSeed;
        /// <summary>
        /// This Random is used by the main thread
        /// </summary>
        Random random;
        /// <summary>
        /// Value = true
        /// This field aims to discern cell-lock-related and cell-lock-unrelated bugs,
        /// if it's false, the threads will only operate on their own  cells, otherwise,
        /// they will all operate on all cells and there could be data-race
        /// </summary>
        bool ThreadCellRangeOverlap;
        /// <summary>
        /// Each thread has its own random seed, so if a bug occurs in NON_THREAD_WORK_OVERLAP mode, it can be reproduced
        /// </summary>
        Random[] Randoms;

        /// <summary>
        /// At first we will save some initial cells to the trunks, this field specifies the length of the initial cell
        /// </summary>
        int CellSize;

        int MaxLargeObjectSize;

        /// <summary>
        /// Pauses background defragmentation thread
        /// </summary>
        bool PauseDefragmentation;

        int IterationCount;
        long OpsPerIteration;

        /// <summary>
        /// We will create HALF this number of cells randomly in the beginning,
        /// the other HALF may be added in the testing process.There is a hard limit of
        /// the number of cells each memory trunk can hold, that is 128 M. This is
        /// deliberately designed in this way. When you fed in more than 128 M cells,
        /// it will behave in an unpredictable way.
        /// </summary>
        long MaxCellCount;
        /// <summary>
        /// If SingleTrunk is set to true, all the actions will be performed on the first
        /// MemoryTrunk (and thus producing a very heavy load). Otherwise, Cell IDs will
        /// be scattered to all the trunks.
        /// </summary>
        bool SingleTrunk;
        /// <summary>
        /// the number of operations, which is also the size of ENUM_JOBS
        /// </summary>
        const long OperationCategoryCount = 4;
        enum CellOperationType { USE, ADD, DELETE, UPDATE, Load };
        long[] TimeoutOpCount = new long[OperationCategoryCount];
        Object TimeoutOpCountLock = new Object();  //threads will their counts to the array when they finish
        #endregion

        public Test(int random_seed = 32771, bool thread_cell_range_overlap = true, int cell_size = 128, int max_lo_size = 1 << 16, bool pause_defrag = false, int iter_cnt = 20000, long ops_cnt = 20000, int max_cell_count = 1 << 20, bool single_trunk = false)
        {
            RandomSeed = random_seed;
            random = new Random(random_seed);
            ThreadCellRangeOverlap = thread_cell_range_overlap;
            CellSize = cell_size;
            MaxLargeObjectSize = max_lo_size;
            PauseDefragmentation = pause_defrag;
            IterationCount = iter_cnt;
            OpsPerIteration = ops_cnt;
            MaxCellCount = max_cell_count;
            SingleTrunk = single_trunk;
        }

        /// <summary>
        /// The count of profiles
        /// </summary>
        int WorkerCount;
        long[] DeltaCellCount; //the delta of cell number on each thread(newly-saved - deleted)
        bool[] WorkerResizeException; // toggled by a worker thread when an exception is thrown on cell resize
        List<long>[] ThreadCellIds;
        long InitialCellCount = 0;

        long StartCellId = 0x7FFFFFFF00000000;


        /// <summary>
        /// We want to trace the number of all the cells, so i add a lock when i'm calling saveCell
        /// </summary>
        Object[] cellLocks;
        [HandleProcessCorruptedStateExceptions, SecurityCriticalAttribute]
        public unsafe bool Run()
        {
            if (PauseDefragmentation)
            {
                LocalMemoryStorage.PauseMemoryDefragmentation();
            }
            else
            {
                LocalMemoryStorage.RestartMemoryDefragmentation();
            }

            //Add In Profiles, the PID field must start from 0 and add 1 one by one
            List<WorkerProfile> WorkerProfile = new List<WorkerProfile>();

            WorkerProfile.Add(new WorkerProfile(0, OpsPerIteration, IterationCount, new List<int> { 1, 1, 1, 3, 1 }));           //use, add, delete, update, load
            WorkerProfile.Add(new WorkerProfile(1, OpsPerIteration, IterationCount, new List<int> { 1, 1, 3, 1, 1 }));           //use, add, delete, update, load
            WorkerProfile.Add(new WorkerProfile(2, OpsPerIteration, IterationCount, new List<int> { 1, 3, 1, 1, 1 }));           //use, add, delete, update, load
            WorkerProfile.Add(new WorkerProfile(3, OpsPerIteration, IterationCount, new List<int> { 1, 2, 1, 3, 1 }));           //use, add, delete, update, load
            WorkerProfile.Add(new WorkerProfile(4, OpsPerIteration, IterationCount, new List<int> { 1, 1, 2, 2, 1 }));           //use, add, delete, update, load

            for (int i = 5; i < Environment.ProcessorCount; i++)
            {
                WorkerProfile.Add(new WorkerProfile(i, OpsPerIteration, IterationCount, new List<int> { 1, 1, 1, 1, 1 }));         //use, add, delete, update, load
            }

            WorkerCount = WorkerProfile.Count;

            Initialize();

            List<Thread> RunningThreads = new List<Thread>();
            for (int i = 0; i < WorkerProfile.Count; i++)
            {
                Thread t = new Thread(new ParameterizedThreadStart(WorkerThread));
                RunningThreads.Add(t);
                t.Start(WorkerProfile[i]);
            }

            for (int i = 0; i < RunningThreads.Count; i++)
                RunningThreads[i].Join();

            bool testResult = VerifyData();
            return testResult;
        }
        /// <summary>
        /// A Profile Class used to trace performance of each operation in a thread
        /// </summary>
        class OpPerformanceRecord
        {
            int threadId;
            long sumTime;
            int count;
            long maxTime;
            long minTime;
            int warnings;
            int warnLimit;
            string OpName;
            public OpPerformanceRecord(int thread, string s, int limit)
            {
                threadId = thread;
                OpName = s;
                warnLimit = limit;
                sumTime = 0;
                count = 0;
                maxTime = 0;
                warnings = 0;
                minTime = 1000000;
            }
            public void Record(Stopwatch s)
            {
                long time = s.ElapsedTicks;
                sumTime += time;
                count++;
                if (time > maxTime)
                    maxTime = time;
                if (time < minTime)
                    minTime = time;
                if (time > warnLimit)
                {
                    //Log("THREAD" + threadId + " WARNING : A " + OpName + " elasped " + time + " milliseconds....Timeout!");
                    warnings++;
                }
            }
            public void Output()
            {
                Console.WriteLine("Thead: " + threadId + " Op: " + OpName);
                Console.WriteLine("\tcount: " + count);
                double average_ticks = ((double)sumTime) / count;
                //resize, add, delete, update
                int kk = 1;
                if (OpName == "ADD") kk = 2;
                if (OpName == "DELETE") kk = 3;
                if (OpName == "UPDATE") kk = 4;
                Console.WriteLine(threadId + " " + kk + " " + average_ticks + " " + average_ticks * 1000 / Stopwatch.Frequency);
                Console.WriteLine("\taverage ticks: " + average_ticks + "\t=" + average_ticks * 1000 / Stopwatch.Frequency + "ms");
                Console.WriteLine("\tmax ticks: " + maxTime + "\t=" + maxTime * 1000 / Stopwatch.Frequency + "ms");
                Console.WriteLine("\tmin ticks: " + minTime + "\t=" + minTime * 1000 / Stopwatch.Frequency + "ms");
                Console.WriteLine("\twarnings: " + warnings);
                Console.WriteLine();
            }
        }

        [HandleProcessCorruptedStateExceptions, SecurityCriticalAttribute]
        void WorkerThread(object obj)
        {
            Thread.Sleep(1000);

            WorkerProfile workerProfile = (WorkerProfile)obj;
            List<int> OpPercentages = workerProfile.OpRatio;
            long IterationCount = workerProfile.TotalIteration;
            int CurrentThreadId = workerProfile.WorkerId;
            long OpCountPerIteration = workerProfile.OpCountPerIteration;
            long UseAttempts = 0, DeleteAttempts = 0, AddAttempts = 0, UpdateAttempts = 0, LoadAttempts = 0;

            int ms2Tick = (int)(5 * Stopwatch.Frequency / 1000);

            OpPerformanceRecord useRec = new OpPerformanceRecord(workerProfile.WorkerId, "USE", ms2Tick);
            OpPerformanceRecord deleteRec = new OpPerformanceRecord(workerProfile.WorkerId, "DELETE", ms2Tick);
            OpPerformanceRecord updateRec = new OpPerformanceRecord(workerProfile.WorkerId, "UPDATE", ms2Tick);
            OpPerformanceRecord addRec = new OpPerformanceRecord(workerProfile.WorkerId, "ADD", ms2Tick);
            OpPerformanceRecord loadRec = new OpPerformanceRecord(workerProfile.WorkerId, "LOAD", ms2Tick);

            //byte[] cellBuffer = new byte[CellSize * 3];


            long[] _TimeoutOpCount = new long[OperationCategoryCount];
            long[] _TimeOpSum = new long[OperationCategoryCount];
            for (int i = 0; i < OperationCategoryCount; i++)
            {
                _TimeoutOpCount[i] = 0;
                _TimeOpSum[i] = 0;
            }

            #region initialization
            int opCount = 0;
            for (int i = 0; i < OpPercentages.Count; i++)
                opCount += OpPercentages[i];

            CellOperationType[] OpArray = new CellOperationType[opCount];
            opCount = 0;
            for (int i = 0; i < OpPercentages.Count; i++)
            {
                for (int j = 0; j < OpPercentages[i]; j++)
                {
                    OpArray[opCount] = (CellOperationType)i;
                    opCount++;
                }
            }
            #endregion

            long garbage = 0;
            for (int currentIteration = 0; currentIteration < IterationCount; currentIteration++)
            {
                Stopwatch _sw_ = new Stopwatch();
                int cellCountOfCurrentThread = ThreadCellIds[CurrentThreadId].Count;
                Random tRandom = Randoms[CurrentThreadId];
                for (long opI = 0; opI < OpCountPerIteration; opI++)
                {
                    CellOperationType CurrentOpType = OpArray[tRandom.Next(opCount)];
                    int startI = tRandom.Next(cellCountOfCurrentThread);

                    int finalI = startI + tRandom.Next(10); //! try a random number of attempts
                    for (int i = startI; i < finalI; i++)
                    {
                        long cellId = ThreadCellIds[CurrentThreadId][(int)(i % cellCountOfCurrentThread)];

                        if (CurrentOpType == CellOperationType.USE)
                        {
                            UseAttempts++;
                            #region USE
                            _sw_.Restart();
                            int size, entryIndex;
                            byte* cellPtr;
                            ushort cellType;
                            TrinityErrorCode eResult = Global.LocalStorage.GetLockedCellInfo(cellId, out size, out cellType, out cellPtr, out entryIndex);
                            if (eResult == TrinityErrorCode.E_SUCCESS)
                            {
                                var junk = new byte[100];

                                if (size > 0)
                                {
                                    int offset = Randoms[CurrentThreadId].Next(0, size);

                                    int delta = Randoms[CurrentThreadId].Next(0, size);

                                    if (Randoms[CurrentThreadId].NextDouble() < 0.8)
                                    {
                                        int remaining_bytes = (size - offset);
                                        if (remaining_bytes > 0)
                                            delta = -Randoms[CurrentThreadId].Next(0, remaining_bytes);
                                    }
                                    else if (size + delta > CellSize * 2)
                                    {
                                        delta = CellSize * 2 - size;
                                        if (delta < 0 && offset - delta > size)
                                        {
                                            delta = size - offset;
                                        }
                                    }

                                    if (delta != 0)
                                    {
                                        try{
                                        cellPtr = Global.LocalStorage.ResizeCell(cellId, entryIndex, offset, delta);
                                        }catch{
                                        Console.WriteLine($"{size}:{delta}");
                                        WorkerResizeException[CurrentThreadId] = true;
                                        }
                                    }

                                    size = size + delta;
                                    for (int c = 0; c < size; c++)
                                        cellPtr[c] = (byte)(cellId + c);
                                }

                                Global.LocalStorage.ReleaseCellLock(cellId, entryIndex);
                            }
                            _sw_.Stop();

                            useRec.Record(_sw_);
                            #endregion
                            continue;
                        }

                        if (CurrentOpType == CellOperationType.DELETE)
                        {
                            DeleteAttempts++;
                            #region DELETE
                            _sw_.Restart();
                            if (Global.LocalStorage.RemoveCell(cellId) == TrinityErrorCode.E_SUCCESS)
                            {
                                Interlocked.Decrement(ref DeltaCellCount[CurrentThreadId]);
                            }
                            _sw_.Stop();
                            /*
                            if (_sw_.ElapsedMilliseconds > 10)
                            {
                                _TimeoutOpCount[(int)CellOperationType.DELETE]++;
                                Log("Warning : A Remove elapsed : " + _sw_.ElapsedMilliseconds + " millis.");
                            }*/
                            deleteRec.Record(_sw_);
                            #endregion
                            continue;
                        }

                        if (CurrentOpType == CellOperationType.UPDATE)
                        {
                            UpdateAttempts++;
                            #region UPDATE
                            _sw_.Restart();

                            double prob_value = Randoms[CurrentThreadId].NextDouble();

                            int cell_size = 0;
                            if (prob_value < 0.0003)
                                cell_size = Randoms[CurrentThreadId].Next(2, MaxLargeObjectSize);
                            else
                                cell_size = Randoms[CurrentThreadId].Next(0, CellSize * 2);

                            Global.LocalStorage.UpdateCell(cellId, GetCellContent(cellId, cell_size));
                            _sw_.Stop();
                            updateRec.Record(_sw_);
                            #endregion
                            continue;
                        }

                        if (CurrentOpType == CellOperationType.ADD)
                        {
                            AddAttempts++;
                            #region ADD
                            _sw_.Restart();
                            if (Global.LocalStorage.AddCell(cellId, GetCellContent(cellId, CellSize), 0, CellSize, ushort.MaxValue) == TrinityErrorCode.E_SUCCESS)
                            {
                                Interlocked.Increment(ref DeltaCellCount[CurrentThreadId]);
                            }
                            _sw_.Stop();
                            addRec.Record(_sw_);
                            #endregion
                            continue;
                        }

                        if (CurrentOpType == CellOperationType.Load)
                        {
                            LoadAttempts++;
                            #region Load
                            _sw_.Restart();

                            byte[] cellBuff;
                            Global.LocalStorage.LoadCell(cellId, out cellBuff);
                            garbage += cellBuff.Length;
                            _sw_.Stop();

                            loadRec.Record(_sw_);
                            #endregion
                            continue;
                        }
                    }
                }
            }

            Log("====WorkId : {0} all operations complete==== \n" +
                "WorkId : {0} add attempts : {1}\n" +
                "WorkId : {0} delete attempts : {2}\n" +
                "WorkId : {0} update attempts : {3}\n" +
                "WorkId : {0} cell count delta : {4}\n" +
                "WorkId : {0} garbage : {5}\n"
                ,
                CurrentThreadId, AddAttempts, DeleteAttempts, UpdateAttempts, DeltaCellCount[CurrentThreadId], garbage
                );


            lock (TimeoutOpCountLock)
            {
                for (int i = 0; i < OperationCategoryCount; i++)
                {
                    TimeoutOpCount[i] += _TimeoutOpCount[i];
                }
                deleteRec.Output();
                updateRec.Output();
                addRec.Output();
            }
        }

        void Initialize()
        {
            DeltaCellCount = new long[WorkerCount];
            WorkerResizeException = new bool[WorkerCount];
            for (int i = 0; i < WorkerCount; i++)
            {
                DeltaCellCount[i] = 0;
                WorkerResizeException[i] = false;
            }
            cellLocks = new Object[MaxCellCount];
            for (int i = 0; i < MaxCellCount; i++)
                cellLocks[i] = new Object();
            for (int i = 0; i < 4; i++)
                TimeoutOpCount[i] = 0;

            Log("========== Parameters ==========");
            Log("SEED : " + RandomSeed);
            Log("THREAD_WORK_OVERLAP : " + ThreadCellRangeOverlap);
            Log("THREAD_COUNT : " + WorkerCount);
            Log("CELL_SIZE : " + CellSize);
            Log("MAX_CELL_COUNT : " + MaxCellCount);
            Log("SINGLE_TRUNK : " + SingleTrunk);
            Log("ITERATION_COUNT : " + IterationCount);
            Log("OPS_PER_ITERATION : " + OpsPerIteration);

            Console.WriteLine();

            Randoms = new Random[WorkerCount];
            ThreadCellIds = new List<long>[WorkerCount];
            for (int i = 0; i < WorkerCount; i++)
            {
                Randoms[i] = new Random(random.Next());
                ThreadCellIds[i] = new List<long>();
            }

            Log("Start saving initial cells ...");
            long EndCellId = StartCellId + MaxCellCount;
            for (long id = StartCellId; id < EndCellId; id++)
            {
                if (SingleTrunk && (byte)id != 0)
                    continue;

                if (random.NextDouble() > 0.5) //Firstly, only save about half of the cells
                {
                    ++InitialCellCount;
                    Global.LocalStorage.SaveCell(id, GetCellContent(id, CellSize), ushort.MaxValue);
                }
                if (ThreadCellRangeOverlap) //assign the id to a thread
                {
                    for (int i = 0; i < WorkerCount; i++) //assign it to all thread
                        ThreadCellIds[i].Add(id);
                }
                else
                {
                    ThreadCellIds[random.Next(WorkerCount)].Add(id); //assign it to a random thread
                }
            }
            for (int i = 0; i < WorkerCount; i++)
            {
                if (ThreadCellIds[i].Count == 0)
                {
                    Log("Warning: worker #{0} does not have any cell to manipulate.", i);
                    throw new Exception();
                }
            }
            Log("Initial cell count: " + InitialCellCount);
        }

        #region Utilities
        Object loglock = new Object();
        void Log(string format, params object[] arg)
        {
            var message = string.Format(CultureInfo.InvariantCulture, format, arg);
            lock (loglock)
            {
                Console.Error.WriteLine(message);
            }
        }
        static byte[] GetCellContent(long id, int length)
        {
            byte[] content = new byte[length];
            for (int c = 0; c < length; c++)
                content[c] = (byte)(id + c);
            return content;
        }
        bool VerifyData()
        {
            Log("Saving storage, cell count: {0}", Global.LocalStorage.CellCount);
            Global.LocalStorage.SaveStorage();
            Log("Storage saved.");
            Log("Loading storage ...");
            Global.LocalStorage.LoadStorage();
            Log("Storage loaded");
            Console.WriteLine("Cell count: {0}", Global.LocalStorage.CellCount);
            Log("====Data Verification====");
            bool right = true;
            long finalActualCellCount = 0;
            HashSet<long> DistinctCells = new HashSet<long>();

            foreach (var cell in Global.LocalStorage)
            {
                finalActualCellCount++;
                if (!DistinctCells.Add(cell.CellId))
                {
                    Console.WriteLine("Duplicated ID in the system: {0}", cell.CellId);
                }
                for (int i = 0; i < cell.CellSize; i++)
                {
                    byte b = (byte)(cell.CellId + i);
                    byte c = *(cell.CellPtr + i);
                    if (c != b)
                    {
                        Log("final cell content test error : celId : " + cell.CellId);
                        right = false;
                        break;
                    }
                }
            }

            Console.Error.WriteLine("Cell Count: {0}", Global.LocalStorage.CellCount);

            for (int i = 0; i < OperationCategoryCount; i++)
            {
                Log((CellOperationType)i + " timeout : " + TimeoutOpCount[i]);
            }
            Log("initialCellNum : " + InitialCellCount);
            long finalExpectedCellCount = InitialCellCount;
            for (int i = 0; i < WorkerCount; i++)
            {
                Console.WriteLine("delta on PID " + i + " : " + DeltaCellCount[i]);
                finalExpectedCellCount += DeltaCellCount[i];
            }
            Console.WriteLine("Final Expected Cell Count : " + finalExpectedCellCount);
            Console.WriteLine("Final Actual Cell Count : " + finalActualCellCount);
            Console.WriteLine("Distinct Cell Count: " + DistinctCells.Count);

            if (finalExpectedCellCount != finalActualCellCount || DistinctCells.Count != finalActualCellCount)
            {
                Log("Error : finalCellNum inconsistent!!");
                right = false;
            }

            if(WorkerResizeException.Any(_ => _))
            {
                Log("Error : cell resize exception");
            }

            return right;
        }

        class WorkerProfile
        {
            public readonly int WorkerId;
            /// <summary>
            /// At each iteration, the threads will randomly choose some cells to operate on,
            /// we recommend the user to set the parameter to be close to CELL_EACH_TRUNK * size of TRUNKS_FOR_TEST
            /// </summary>
            public readonly long OpCountPerIteration;
            public readonly int TotalIteration;
            /// <summary>
            /// ratio of workload of save, delete, update
            /// </summary>
            public readonly List<int> OpRatio;
            public WorkerProfile(int workerId, long op_per_iteration, int iterationCount, List<int> op_ratio)
            {
                WorkerId = workerId;
                OpCountPerIteration = op_per_iteration;
                TotalIteration = iterationCount;
                OpRatio = op_ratio;
            }
        }
        #endregion
    }

    public class storage4
    {
        [TestCase(32771, true, 128, 1<<16, false, 20000, 20000, 2 << 20, false)]
        [TestCase(25168, false, 511, 2<<16, false, 30000, 200, 1 << 20, false)]
        [TestCase(496827356, true, 315, 1<<15, false, 20, 200000, 1 << 20, false)]
        [TestCase(9486, false, 7, 3<<16, false, 50000, 1000, 1 << 20, false)]
        [TestCase(67691, true, 916, 1<<13, false, 1000, 50000, 1 << 20, false)]
        [TestCase(459, false, 617, 1<<20, false, 10000, 5000, 1 << 20, false)]

        [TestCase(32771, true, 128, 1<<16, true, 20000, 20000, 2 << 20, false)]
        [TestCase(25168, false, 511, 2<<16, true, 30000, 200, 2 << 20, false)]
        [TestCase(496827356, true, 315, 1<<15, true, 20, 2000000, 2 << 20, false)]
        [TestCase(9486, false, 7, 3<<16, true, 50000, 10000, 2 << 20, false)]
        [TestCase(67691, true, 916, 1<<13, true, 10000, 50000, 2 << 20, false)]
        [TestCase(459, false, 617, 1<<20, true, 10000, 100000, 2 << 20, false)]

        [TestCase(32771, true, 128, 1<<16, false, 20000, 20000, 1 << 13, true)]
        [TestCase(25168, false, 511, 2<<16, false, 30000, 200, 1 << 12, true)]
        [TestCase(496827356, true, 315, 1<<15, false, 20, 2000000, 1 << 12, true)]
        [TestCase(9486, false, 7, 1<<16, false, 50000, 10000, 1 << 12, true)]
        [TestCase(67691, true, 916, 1<<13, false, 10000, 50000, 1 << 13, true)]
        [TestCase(459, false, 617, 1<<20, false, 10000, 100000, 1 << 13, true)]
        public void TestCMemTrunk(int random_seed, bool thread_cell_range_overlap, int cell_size, int max_lo_size, bool pause_defrag, int iter_cnt, long ops_cnt, int max_cell_count, bool single_trunk)
        {
            Global.LocalStorage.ResetStorage();
            Test t = new Test(random_seed, thread_cell_range_overlap, cell_size, max_lo_size, pause_defrag, iter_cnt, ops_cnt, max_cell_count, single_trunk);
            Assert.That(t.Run());
        }
    }
}
