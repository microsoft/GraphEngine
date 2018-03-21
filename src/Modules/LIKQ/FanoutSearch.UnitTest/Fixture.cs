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
using Trinity.Network;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true, MaxParallelThreads = 1)]

namespace FanoutSearch.UnitTest
{
    [CollectionDefinition("All")]
    public class AllLIKQTestCollection : ICollectionFixture<TrinityServerFixture> { }

    public class TrinityServerFixture : IDisposable
    {
        private TrinityServer server;

        public TrinityServerFixture()
        {
            Global.Initialize();
            LambdaDSL.SetDialect("MAG", "StartFrom", "VisitNode", "FollowEdge", "Action");
            FanoutSearchModule.SetQueryTimeout(-1);
            FanoutSearchModule.RegisterIndexService(FakeIndexService);
            FanoutSearchModule.SetCacheEnabled(false);
            server = new TrinityServer();
            server.RegisterCommunicationModule<FanoutSearchModule>();
            server.Start();
        }

        public void Dispose()
        {
            server.Stop();
            Global.Uninitialize();
        }

        private static IEnumerable<long> FakeIndexService(object obj, string type)
        {
            if (obj == null) throw new Exception();
            return new List<long> { 1, 2, 3 } as IEnumerable<long>;
        }

    }

    class Expect
    {
        internal static void FanoutSearchQueryException(System.Action action, string message_substring)
        {
            Assert.Contains(message_substring, Assert.Throws<FanoutSearchQueryException>(action).InnerException.Message);
        }

        internal static void FanoutSearchQueryException(System.Action<string> action, string param, string message_substring)
        {
            var exception = Assert.Throws<FanoutSearchQueryException>(() => action(param));

            var message = exception.InnerException !=null ? exception.InnerException.Message : exception.Message;

            Assert.Contains(message_substring, message);
        }
    }
}
