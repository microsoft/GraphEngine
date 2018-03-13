// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity;
using FanoutSearch.Test.TSL;
using FanoutSearch.Standard;
using System.Threading;
using Xunit;

namespace FanoutSearch.UnitTest
{
    public class LocalStressTest : IDisposable
    {
        public LocalStressTest()
        {
            Initializer.Initialize();

            //Load some data
            Global.LocalStorage.SaveMyCell(0, new List<long> { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, f2: new List<float> { 1, 2, 3, 4, 5, 6, 7, 8 });
            Global.LocalStorage.SaveMyCell(1, new List<long> { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, f2: new List<float> { 1, 2, 3, 4, 5, 6, 7, 8 });
            Global.LocalStorage.SaveMyCell(2, new List<long> { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, f2: new List<float> { 1, 2, 3, 4, 5, 6, 7, 8 });
            Global.LocalStorage.SaveMyCell(3, new List<long> { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, f2: new List<float> { 1, 2, 3, 4, 5, 6, 7, 8 });
            Global.LocalStorage.SaveMyCell(4, new List<long> { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, f2: new List<float> { 1, 2, 3, 4, 5, 6, 7, 8 });
            Global.LocalStorage.SaveMyCell(5, new List<long> { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, f2: new List<float> { 1, 2, 3, 4, 5, 6, 7, 8 });
            Global.LocalStorage.SaveMyCell(6, new List<long> { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, f2: new List<float> { 1, 2, 3, 4, 5, 6, 7, 8 });
            Global.LocalStorage.SaveMyCell(7, new List<long> { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, f2: new List<float> { 1, 2, 3, 4, 5, 6, 7, 8 });
            Global.LocalStorage.SaveMyCell(8, new List<long> { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, f2: new List<float> { 1, 2, 3, 4, 5, 6, 7, 8 });
            Global.LocalStorage.SaveMyCell(9, new List<long> { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, f2: new List<float> { 1, 2, 3, 4, 5, 6, 7, 8 });


            // WARM UP
            _stress_test_impl(10, 10);
            // Set timeout, and disable cache
            FanoutSearchModule.SetCacheEnabled(false);
            FanoutSearchModule.SetQueryTimeout(800);
        }

        public void Dispose()
        {
            Initializer.Uninitialize();
        }

        private static void _stress_test_impl(int client_count, int round)
        {
            List<Thread> threads = new List<Thread>();
            for (int tid = 0; tid < client_count; ++tid)
            {
                Thread thread = new Thread(() =>
                    {
                        for (int r = 0; r<round; ++r)
                        {
                            g.v(0).outV(Action.Continue).outV(_ => Action.Return, select: new List<string> {"CellID" }).ToList();
                        }
                    });
                threads.Add(thread);
                thread.Start();
            }

            threads.ForEach(_ => _.Join());
        }

        [Fact]
        public void StressTest_1()
        {
            _stress_test_impl(50, 10);
        }

        [Fact]
        public void StressTest_2()
        {
            _stress_test_impl(100, 10);
        }

        [Fact]
        public void StressTest_3()
        {
            _stress_test_impl(200, 20);
        }

        [Fact]
        public void StressTest_4()
        {
            _stress_test_impl(500, 20);
        }

        [Fact]
        public void StressTest_5()
        {
            _stress_test_impl(500, 80);
        }
    }
}
