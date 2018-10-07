// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Text;
using System.Collections.Generic;
using Trinity.Network;
using Trinity;
using FanoutSearch.LIKQ;
using System.Linq;
using FanoutSearch.Test.TSL;
using Trinity.Storage;
using Xunit;

namespace FanoutSearch.UnitTest
{
    class CustomClass
    {
        public struct NestedStruct
        {
            public bool val;
        }

        public NestedStruct val;
    }

    /// <summary>
    /// Summary description for LIKQTest
    /// </summary>
    [Collection("All")]
    public class LIKQTest
    {
        IEnumerable<PathDescriptor> DoQuery(long origin, string property, int property_count_lowerbound)
        {
            Func<int> func = () => property_count_lowerbound;
            return KnowledgeGraph
                    .StartFrom(origin)
                    .FollowEdge("people_person_profession")
                    .FollowEdge("people_profession_people_with_this_profession")
            //.VisitNode(_ => _.return_if(_.count(property) > func()), select: new List<string> { "type_object_name" });
            .VisitNode(_ => _.return_if(_.count(property) > property_count_lowerbound), select: new List<string> { "type_object_name" });
            //.VisitNode(_ => _.return_if(_.count(property) > Environment.ProcessorCount), select: new List<string> { "type_object_name" });
        }

        [Fact]
        public void TestNegativeOrigin()
        {
            bool closure_var = true;
            var paths = KnowledgeGraph.StartFrom(123).FollowEdge("123").VisitNode(_ => _.return_if(closure_var)).ToList();
        }

        [Fact]
        public void Test1()
        {
            bool closure_var = true;
            var paths = KnowledgeGraph.StartFrom(123).FollowEdge("123").VisitNode(_ => _.return_if(closure_var)).ToList();
        }

        [Fact]
        public void Test2()
        {
            long origin = 427535605040901;
            foreach (var path in DoQuery(origin, "people_person_languages", 3))
            {
                Console.WriteLine(path);
            }
        }

        [Fact]
        public void Test3()
        {
            CustomClass closure_custom_var = new CustomClass();
            closure_custom_var.val = new CustomClass.NestedStruct() { val = true };
            var paths = KnowledgeGraph.StartFrom(123).FollowEdge("123").VisitNode(_ => _.return_if(closure_custom_var.val.val)).ToList();
        }

        [Fact]
        public void Test4()
        {
            var paths = KnowledgeGraph.StartFrom(123).VisitNode(_ => _.return_if(_.CellId == 123)).ToList();
        }

        [Fact]
        public void Test5()
        {
            Func<int> func = () => 2;
            var paths = KnowledgeGraph
                    .StartFrom(123)
                    .FollowEdge("people_person_profession")
                    .FollowEdge("people_profession_people_with_this_profession")
            .VisitNode(_ => _.return_if(_.count("prop") > func()), select: new List<string> { "type_object_name" }).ToList();
        }

        [Fact]
        public void Test6()
        {
            Func<int, bool> func = cnt => cnt > 3;
            try
            {
                var paths = KnowledgeGraph.StartFrom(123).VisitNode(_ => _.return_if(func(_.count("prop")))).ToList();
            }
            catch
            {
                Console.WriteLine("Test6 throws expected exception.");
                /* Expected to throw exception due to entangled lambda expressions and non-evaluable parts. */
            }
        }
        [Fact]
        public void Test7()
        {
            Func<int> func = () => 2;
            var paths = KnowledgeGraph
                    .StartFrom(123)
                    .VisitNode(_ => _.return_if(_.count("prop") > func() + Environment.ProcessorCount)).ToList();
        }

        [Fact]
        public void Test8()
        {

            var paths = KnowledgeGraph
                    .StartFrom(123)
                    .VisitNode(_ => _.Let(_.count("prop"), (i) => FanoutSearch.Action.Return)).ToList();
        }

        [Fact]
        public void Test9()
        {
            var paths = KnowledgeGraph
                    .StartFrom(123)
                    .VisitNode(_ => _.return_if(_.count("prop") > "123".Length)).ToList();
        }

        [Fact]
        public void Test10()
        {
            var paths = KnowledgeGraph
                    .StartFrom(123)
                    .VisitNode(_ => _.return_if(_.GetField<string>("prop").Length > "123".Length)).ToList();
        }

        [Fact]
        public void Test11()
        {
            Func<string> string_func = () => "hey";
            var paths = KnowledgeGraph
                    .StartFrom(123)
                    .VisitNode(_ => _.return_if(_.GetField<string>("prop").Length > string_func().Length)).ToList();
        }

        [Fact]
        public void Test12()
        {
            Func<string> string_func = () => "hey";
            var paths = KnowledgeGraph
                    .StartFrom(123)
                    .VisitNode(_ => _.return_if(_.GetField<string>(string_func()).Length > 3)).ToList();
        }

        [Fact]
        public void Test13()
        {
            var paths = KnowledgeGraph
                    .StartFrom(123)
                    .VisitNode(_ => _.return_if(_.GetField<List<string>>("prop").Any(str => str.Length > 3))).ToList();
        }

        static DateTime _get_datetime()
        {
            return new DateTime(1969, 1, 1);
        }

        [Fact]
        public void Test14()
        {
            var dt = _get_datetime();

            var paths = KnowledgeGraph
                    .StartFrom(123)
                    .VisitNode(_ => _.return_if(_.GetField<DateTime>("prop") < dt)).ToList();
        }

        [Fact]
        public void Test15()
        {
            Global.LocalStorage.SaveMyCell(1, edges: new List<long> { 2 }, f1: 10);
            Global.LocalStorage.SaveMyCell(2, edges: new List<long> { 1 }, f1: 10);
            // conversion from float to decimal throws InvalidCastException.
            var paths = KnowledgeGraph
                    .StartFrom(1)
                    .VisitNode(_ => _.return_if(_.GetField<List<decimal>>("f1").Any( x => x> 0.1m))).ToList();
            Assert.True(paths.Count == 0);
        }

        [Fact]
        public void Test16()
        {
            FanoutSearch.Action a = FanoutSearch.Action.Continue,b = FanoutSearch.Action.Return;

            var paths = KnowledgeGraph.StartFrom(1).VisitNode(_ => a & b);
            paths.ToList();
        }

        [Fact]
        public void Test17()
        {
            // long range traverse capability test
            var path = KnowledgeGraph.StartFrom(0);
            for (int hop = 1; hop <= 20; ++hop)
            {
                path = path.VisitNode(FanoutSearch.Action.Continue);
            }

            path = path.VisitNode(FanoutSearch.Action.Return);

            path.ToList();
        }

        [Fact]
        public void Test18()
        {
            try
            {
                KnowledgeGraph.StartFrom(0).VisitNode(_ => _.Do(() => Console.WriteLine("Hey")) & Action.Return).ToList();
            }
            catch (FanoutSearchQueryException ex)
            {
                Assert.True(ex.Message.Contains("Referencing a type not allowed") && ex.Message.Contains("Console"));
                return;
            }

            throw new Exception();
        }

        [Fact]
        public void Test19()
        {
            try
            {
                KnowledgeGraph.StartFrom(0).VisitNode(_ => _.Do(() => Global.LocalStorage.ResetStorage()) & Action.Return).ToList();
            }
            catch (FanoutSearchQueryException ex)
            {
                Assert.True(ex.Message.Contains("Referencing a type not allowed") && ex.Message.Contains("LocalMemoryStorage"));
                return;
            }

            throw new Exception();
        }

        [Fact]
        public void Test20()
        {
            try
            {
                KnowledgeGraph.StartFrom(0).VisitNode(_ => _.Do(() => _.SetField("foo", "bar")) & Action.Return).ToList();
            }
            catch (FanoutSearchQueryException ex)
            {
                Assert.True(ex.Message.Contains("Syntax error") && ex.Message.Contains("SetField"));
                return;
            }

            throw new Exception();
        }
    }
}