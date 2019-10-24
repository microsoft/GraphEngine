// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections;

using Trinity;
using Trinity.Storage;
using Trinity.Extension.GraphGenerator;

namespace GraphGenerator
{
    struct NodeCreationThreadObject
    {
        public int threadCount;
        public int threadIndex;
        public NodeCreationThreadObject(int threadCount, int threadIndex)
        {
            this.threadCount = threadCount;
            this.threadIndex = threadIndex;
        }
    }
    struct EdgeCreationThreadObject
    {
        public int threadCount;
        public int threadIndex;
        public EdgeCreationThreadObject(int threadCount, int threadIndex)
        {
            this.threadCount = threadCount;
            this.threadIndex = threadIndex;
        }
    }

    struct CellCleaningThreadObject
    {
        public int threadCount;
        public int threadIndex;
        public long nodeNum;
        public CellCleaningThreadObject(int thrCou, int thrInd, long noNu)
        {
            threadCount = thrCou;
            threadIndex = thrInd;
            nodeNum = noNu;
        }
    }

    class GraphGen
    {
        int index;
        List<string> labelSet = new List<string>();
        static Random[] randomArray = new Random[Environment.ProcessorCount];
        HashSet<string> usedAlphabet = new HashSet<string>();

        long nodeCount;
        int avgDegree;
        long edgeCount;

        #region const values
        const double p1 = 0.4;
        const double p2 = 0.15;
        const double p3 = 0.2;
        const double p4 = 0.25;
        #endregion

        public GraphGen(long nodeCount, int avgDegree, int labelCount)
        {
            this.nodeCount = nodeCount;
            this.avgDegree = avgDegree;
            this.edgeCount = nodeCount * avgDegree;
            for (int i = 0; i < labelCount; i++)
            {
                labelSet.Add(i.ToString());
            }
            BuildRandomNumber();
        }

        void BuildRandomNumber()
        {
            byte[] bytes;
            for (int i = 4; i < Environment.ProcessorCount + 4; i++)
            {
                bytes = new byte[i];
                System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
                rng.GetBytes(bytes);
                randomArray[i - 4] = new Random(BitConverter.ToInt32(bytes, 0));
            }
        }
        void CreateEdgeThreadProc(object par)
        {
            EdgeCreationThreadObject p = (EdgeCreationThreadObject)par;
            long start;
            long end;
            if (p.threadCount - 1 == p.threadIndex)
            {
                start = 0;
                end = edgeCount / (p.threadCount) + edgeCount % (p.threadCount);
            }
            else
            {
                start = 0;
                end = edgeCount / p.threadCount;
            }
            long times = (long)Math.Log(nodeCount, 2);
            for (long i = start; i < end; i++)
            {
                long x1 = 0;
                long x2 = 0;
                long y1 = nodeCount;
                long y2 = nodeCount;
                double probability = 0;
                for (int timesIndex = 0; timesIndex < times; timesIndex++)
                {
                    probability = randomArray[p.threadIndex].NextDouble();
                    if (probability <= 0.4)
                    {
                        y1 = y1 - (y1 - x1) / 2;
                        y2 = y2 - (y2 - x2) / 2;
                    }
                    else if (probability < 0.55 && probability > 0.4)
                    {
                        x1 = x1 + (y1 - x1) / 2;
                        y2 = y2 - (y2 - x2) / 2;
                    }
                    else if (probability > 0.55 && probability <= 0.75)
                    {
                        x2 = x2 + (y2 - x2) / 2;
                        y1 = y1 - (y1 - x1) / 2;
                    }
                    else if (probability > 0.75)
                    {
                        x1 = x1 + (y1 - x1) / 2;
                        x2 = x2 + (y2 - x2) / 2;
                    }
                }
                using (var node = Global.LocalStorage.UseGraphNode(y2))
                {
                    if (!node.Edges.Contains(y1))
                    {
                        if (node.EdgeCount < node.Edges.Count)
                        {
                            node.Edges[node.EdgeCount++] = y1;
                        }
                        else
                        {
                            int reserveNum = avgDegree;
                            List<long> reserved_list = new List<long>();
                            reserved_list.AddRange(Enumerable.Range(-avgDegree, reserveNum).Select(x => (long)x));
                            node.Edges.AddRange(reserved_list);
                            node.Edges[node.EdgeCount++] = y1;
                        }
                    }
                }
            }
        }
        void CreateGraphNodeThreadProc(object par)
        {
            NodeCreationThreadObject p = (NodeCreationThreadObject)par;

            long start = -1;
            long end = -1;
            long ele = nodeCount / p.threadCount;

            if (p.threadCount != p.threadIndex + 1)
            {
                start = p.threadIndex * ele + 1;
                end = start + ele;
            }
            else
            {
                start = p.threadIndex * ele + 1;
                end = nodeCount + 1;
            }

            int loopTimes = (int)Math.Log(nodeCount, 2);

            for (long i = start; i < end; i++)
            {
                long begin = 1;
                long finish = nodeCount;
                long capacity = edgeCount;
                double up = p1 + p2;
                double down = p3 + p4;

                for (int j = 0; j < loopTimes; j++)
                {
                    if (i >= begin && i <= finish - ((finish - begin + 1) / 2))
                    {
                        finish = finish - ((finish - begin + 1) / 2);
                        capacity = (long)(capacity * up);
                    }
                    else if (i >= begin + ((finish - begin + 1) / 2) && i <= finish)
                    {
                        begin = begin + ((finish - begin + 1) / 2);
                        capacity = (long)(capacity * down);
                    }
                }

                List<long> reservationList = new List<long>((int)capacity);

                reservationList.AddRange(Enumerable.Range(-(int)(capacity + 1), (int)capacity).Select(x => (long)x));

                index = randomArray[p.threadIndex].Next(0, labelSet.Count);

                GraphNode node = new GraphNode(i, 0, labelSet[index], reservationList);

                Global.LocalStorage.SaveGraphNode(node);
            }
        }

        public void CreateGraph()
        {
            int threadCount = Environment.ProcessorCount;
            Thread[] threadNum = new Thread[threadCount];
            ////////////////////////////////PHASE 1///////////////////////////////
            Console.WriteLine("Generating ID values");
            Stopwatch s = new Stopwatch();
            s.Start();
            TrinityConfig.CurrentRunningMode = RunningMode.Embedded;
            TrinityConfig.DefragInterval = 2500;
            for (int threadIndex = 0; threadIndex < threadCount; threadIndex++)
            {
                NodeCreationThreadObject p = new NodeCreationThreadObject(threadCount, threadIndex);
                threadNum[threadIndex] = new Thread(CreateGraphNodeThreadProc);
                threadNum[threadIndex].Start(p);
            }
            for (int inde = 0; inde < threadCount; inde++)
                threadNum[inde].Join();
            s.Stop();
            Console.WriteLine("add idValue cost time:{0}", s.ElapsedMilliseconds);
            ////////////////////////////////PHASE 2///////////////////////////////
            Console.WriteLine("add edges");
            Stopwatch ss = new Stopwatch();
            ss.Start();
            for (int threadIndex = 0; threadIndex < threadCount; threadIndex++)
            {
                EdgeCreationThreadObject p = new EdgeCreationThreadObject(threadCount, threadIndex);
                threadNum[threadIndex] = new Thread(CreateEdgeThreadProc);
                threadNum[threadIndex].Start(p);
            }
            for (int inde = 0; inde < threadCount; inde++)
                threadNum[inde].Join();
            ss.Stop();
            Console.WriteLine("add outlinks cost time:{0}", ss.ElapsedMilliseconds);

            ///////////////Remove dirty outlinks//////////////
            Stopwatch watch = new Stopwatch();
            watch.Start();
            for (int threadIndex = 0; threadIndex < threadCount; threadIndex++)
            {
                CellCleaningThreadObject p = new CellCleaningThreadObject(threadCount, threadIndex, nodeCount);
                threadNum[threadIndex] = new Thread(CellCleaningThreadProc);
                threadNum[threadIndex].Start(p);
            }
            for (int inde = 0; inde < threadCount; inde++)
                threadNum[inde].Join();
            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds);
            TrinityConfig.DefragInterval = 100;
            ///////////////////////////////////////////////
            Stopwatch sss = new Stopwatch();
            sss.Start();
            Global.LocalStorage.SaveStorage();
            sss.Stop();
            Console.WriteLine("saveStorage cost time:{0}", sss.ElapsedMilliseconds);
        }
        public static void CellCleaningThreadProc(object par)
        {
            CellCleaningThreadObject p = (CellCleaningThreadObject)par;
            long start = -1;
            long end = -1;
            long ele = p.nodeNum / p.threadCount;
            if (p.threadCount != p.threadIndex + 1)
            {
                start = p.threadIndex * ele + 1;
                end = start + ele;
            }
            else
            {
                start = p.threadIndex * ele + 1;
                end = p.nodeNum + 1;
            }
            for (long i = start; i < end; i++)
            {
                using (var node = Global.LocalStorage.UseGraphNode(i))
                {
                    if (node.EdgeCount < node.Edges.Count)
                    {
                        node.Edges.RemoveRange(node.EdgeCount, node.Edges.Count - node.EdgeCount);
                    }
                }
            }
        }
    }
}
