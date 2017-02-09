// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Trinity.Network;
using Trinity;
using System.Collections.Generic;

namespace FanoutSearch.UnitTest
{
    [TestClass]
    public class LambdaDSLTest
    {
        [ClassInitialize]
        public static void Init(TestContext ctx)
        {
            Initializer.Initialize();
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            Initializer.Uninitialize();
        }

        [TestMethod]
        [ExpectedException(typeof(FanoutSearchQueryException))]
        public void LambdaDSLTest1()
        {
            //throw due to incorrect prolog. should be MAG
            var fs_desc = LambdaDSL.Evaluate("KnowledgeGraph.StartFrom(0).haha();");
        }

        [TestMethod]
        [ExpectedException(typeof(FanoutSearchQueryException))]
        public void LambdaDSLTest1_2()
        {
            // throw due to unrecognized method 'haha'
            var fs_desc = LambdaDSL.Evaluate("MAG.StartFrom(0).haha();");
        }

        [TestMethod]
        [ExpectedException(typeof(FanoutSearchQueryException))]
        public void LambdaDSLTest1_3()
        {
            // throws due to lack of "...".StartFrom(...)
            var fs_desc = LambdaDSL.Evaluate("StartFrom(0);");
        }

        [TestMethod]
        [ExpectedException(typeof(FanoutSearchQueryException))]
        public void LambdaDSLTest1_4()
        {
            // throw due to lack of terminating VisitNode
            var fs_desc = LambdaDSL.Evaluate("MAG.StartFrom(0).FollowEdge(\"*\");");
        }

        [TestMethod]
        public void LambdaDSLTest1_5_1()
        {
            var fs_desc = LambdaDSL.Evaluate("MAG.StartFrom(0).FollowEdge(\"*\").VisitNode(Action.Return);");
        }

        [TestMethod]
        public void LambdaDSLTest1_5_1_2()
        {
            // 1_5_1 with selection list
            var fs_desc = LambdaDSL.Evaluate("MAG.StartFrom(0).FollowEdge(\"*\").VisitNode(Action.Return, new[] {\"test\", \"test2\"});");
            CollectionAssert.AreEquivalent(new[] { "test", "test2" }, fs_desc.m_selectFields[1]);
        }

        [TestMethod]
        public void LambdaDSLTest1_5_1_3()
        {
            var fs_desc = LambdaDSL.Evaluate("MAG.StartFrom(0, new[] {\"test\", \"test2\"}).FollowEdge(\"*\").VisitNode(Action.Return);");
            CollectionAssert.AreEquivalent(new[] { "test", "test2" }, fs_desc.m_selectFields[0]);
        }

        [TestMethod]
        public void LambdaDSLTest1_5_2()
        {
            // the lambda version of 1_5
            var fs_desc = LambdaDSL.Evaluate("MAG.StartFrom(0).FollowEdge(\"*\").VisitNode(_ => Action.Return);");
        }

        [TestMethod]
        [ExpectedException(typeof(FanoutSearchQueryException))]
        public void LambdaDSLTest1_5_3()
        {
            // the lambda version of 1_5
            // throw due to incorrect lambda return type.
            var fs_desc = LambdaDSL.Evaluate("MAG.StartFrom(0).FollowEdge(\"*\").VisitNode(_ => false);");
        }

        [TestMethod]
        public void LambdaDSLTest1_5_4()
        {
            // the lambda version of 1_5
            var fs_desc = LambdaDSL.Evaluate("MAG.StartFrom(0).FollowEdge(\"*\").VisitNode(_ => Action.Continue);");
        }

        [TestMethod]
        public void LambdaDSLTest1_5_5()
        {
            // the lambda version of 1_5
            var fs_desc = LambdaDSL.Evaluate("MAG.StartFrom(0).FollowEdge(\"*\").VisitNode(_ => FanoutSearch.Action.Continue);");
        }

        [TestMethod]
        public void LambdaDSLTest1_6()
        {
            var fs_desc = LambdaDSL.Evaluate("MAG.StartFrom(new List<long>{1,2,3}).FollowEdge(\"L1\", \"L2\").VisitNode(Action.Return);");
        }

        [TestMethod]
        public void LambdaDSLTest1_6_2()
        {
            var fs_desc = LambdaDSL.Evaluate("MAG.StartFrom(new long[]{1,2,3}).FollowEdge(\"L1\", \"L2\").VisitNode(Action.Return);");
        }

        [TestMethod]
        public void LambdaDSLTest1_6_2_1()
        {
            // a variation: spaces
            var fs_desc = LambdaDSL.Evaluate("MAG.StartFrom(new long []{1,2,3}).FollowEdge(\"L1\", \"L2\").VisitNode(Action.Return);");
            CollectionAssert.AreEquivalent(new long[] { 1, 2, 3 }, fs_desc.m_origin);
        }

        [TestMethod]
        public void LambdaDSLTest1_6_2_2()
        {
            // a variation: spaces
            var fs_desc = LambdaDSL.Evaluate("MAG.StartFrom(new long [   ]{1,2,3}).FollowEdge(\"L1\", \"L2\").VisitNode(Action.Return);");
            CollectionAssert.AreEquivalent(new long[] { 1, 2, 3 }, fs_desc.m_origin);
        }

        [TestMethod]
        public void LambdaDSLTest1_6_3()
        {
            var fs_desc = LambdaDSL.Evaluate("MAG.StartFrom(new []{1,2,3}).FollowEdge(\"L1\", \"L2\").VisitNode(Action.Return);");
            CollectionAssert.AreEquivalent(new long[] { 1, 2, 3 }, fs_desc.m_origin);
        }

        [TestMethod]
        [ExpectedException(typeof(FanoutSearchQueryException))]
        public void LambdaDSLTest1_6_4()
        {
            //throw due to incorrect type
            try
            {
                var fs_desc = LambdaDSL.Evaluate("MAG.StartFrom(new []{\"hello\"}).FollowEdge(\"L1\", \"L2\").VisitNode(Action.Return);");
            }
            catch (FanoutSearchQueryException ex)
            {
                if (ex.Message.Contains("Invalid collection element type")) throw;
                else throw new Exception(ex.Message);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(FanoutSearchQueryException))]
        public void LambdaDSLTest1_6_5()
        {
            var fs_desc = LambdaDSL.Evaluate("MAG.StartFrom(new []{new { a = 1}}).FollowEdge(\"L1\", \"L2\").VisitNode(Action.Return);");
        }

        [TestMethod]
        public void LambdaDSLTest1_7()
        {
            var fs_desc = LambdaDSL.Evaluate("MAG.StartFrom(new List<long>{1,2,3}).FollowEdge(\"*\").VisitNode(Action.Return);");
        }

        [TestMethod]
        [ExpectedException(typeof(FanoutSearchQueryException))]
        public void LambdaDSLTest1_8()
        {
            //throw due to the anonymous object creation
            var fs_desc = LambdaDSL.Evaluate("MAG.StartFrom(new { a = 123 }).FollowEdge(\"*\").VisitNode(Action.Return);");
        }

        [TestMethod]
        [ExpectedException(typeof(FanoutSearchQueryException))]
        public void LambdaDSLTest2()
        {
            var fs_desc = LambdaDSL.Evaluate("Console.WriteLine(\"Hey\"); KnowledgeGraph.StartFrom(0).haha();");
        }

        [TestMethod]
        [ExpectedException(typeof(FanoutSearchQueryException))]
        public void LambdaDSLTest3()
        {
            var fs_desc = LambdaDSL.Evaluate(@"
(new Func<FanoutSearchDescriptor>(() => 
return KnowledgeGraph.StartFrom(0).haha();))()");
        }

        [TestMethod]
        [ExpectedException(typeof(FanoutSearchQueryException))]
        public void LambdaDSLTest4()
        {
            var fs_desc = LambdaDSL.Evaluate(@"
"" as FanoutSearchDescriptor ?? KnowledgeGraph.StartFrom(0);
");
        }

        [TestMethod]
        [ExpectedException(typeof(FanoutSearchQueryException))]
        public void LambdaDSLTest5()
        {
            var fs_desc = LambdaDSL.Evaluate(@"
void as FanoutSearchDescriptor ?? KnowledgeGraph.StartFrom(0);
");
        }

        [TestMethod]
        public void LambdaDSLTest6()
        {
            var fs_desc = LambdaDSL.Evaluate("MAG.StartFrom(1234)");
            Assert.AreEqual(0, fs_desc.m_edgeTypes.Count);
            Assert.AreEqual(0, fs_desc.m_traverseActions.Count);
            Assert.AreEqual(1234, fs_desc.m_origin[0]);
        }

        [TestMethod]
        public void LambdaDSLTest7()
        {
            //Test json string acceptance
            var fs_desc = LambdaDSL.Evaluate("MAG.StartFrom(\"{json body}\")");
            Assert.AreEqual(null, fs_desc.m_origin);
            Assert.AreEqual("{json body}", fs_desc.m_origin_query);
        }

        [TestMethod]
        [ExpectedException(typeof(FanoutSearchQueryException))]
        public void LambdaDSLTest8()
        {
            var fs_desc = LambdaDSL.Evaluate("MAG.StartFrom(0).VisitNode(_ => _.Do(()=>Console.WriteLine()) & Action.Return");
        }

        [TestMethod]
        public void LambdaDSLTest9()
        {
            try
            {
                var fs_desc = LambdaDSL.Evaluate("MAG.StartFrom(0).VisitNode(_ => _.Do(() => Global.LocalStorage.ResetStorage()) & Action.Return)");
            }
            catch (FanoutSearchQueryException ex)
            {
                Assert.IsTrue(ex.Message.Contains("Referencing a type not allowed") && ex.Message.Contains("LocalMemoryStorage"));
                return;
            }
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(FanoutSearchQueryException))]
        public void LambdaDSLTest10()
        {
            // throw due to lack of terminating VisitNode
            var fs_desc = LambdaDSL.Evaluate("MAG.StartFrom(0).FollowEdge(\"*\");");
        }

        [TestMethod]
        public void LambdaDSLTest11()
        {
            var fs_desc = LambdaDSL.Evaluate(@"MAG.StartFrom(""{}"");");
        }

        [TestMethod]
        [ExpectedException(typeof(FanoutSearchQueryException))]
        public void LambdaDSLTest12()
        {
            // throw due to incorrect parameter type in StartFrom
            var fs_desc = LambdaDSL.Evaluate(@"MAG.StartFrom(""{}"", ""{}"");");
        }

        [TestMethod]
        [ExpectedException(typeof(FanoutSearchQueryException))]
        public void LambdaDSLTest13()
        {
            // throw due to lack of terminating VisitNode
            var fs_desc = LambdaDSL.Evaluate(@"MAG.StartFrom(""{}"");");
            fs_desc.ToList();
        }


        [TestMethod]
        public void LambdaDSLTest_verb_has()
        {
            var fs_desc = LambdaDSL.Evaluate(@"
MAG.StartFrom(""{}"").VisitNode(v => v.continue_if(v.has(""a"", ""b"")) & v.return_if(true));");
        }
    }
}
