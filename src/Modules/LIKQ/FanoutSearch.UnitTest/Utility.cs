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

namespace FanoutSearch.UnitTest
{
    class Initializer
    {
        internal static void Initialize()
        {
            Global.Initialize();
            LambdaDSL.SetDialect("MAG", "StartFrom", "VisitNode", "FollowEdge", "Action");
            FanoutSearchModule.SetQueryTimeout(-1);
            FanoutSearchModule.RegisterIndexService(FakeIndexService);
            FanoutSearchModule.RegisterExpressionSerializerFactory(() => new ExpressionSerializer());
            TrinityServer server = new TrinityServer();
            server.RegisterCommunicationModule<FanoutSearchModule>();
            server.Start();
        }

        internal static void Uninitialize()
        {
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
            try
            {
                action();
            }
            catch (FanoutSearchQueryException ex) when (ex.Message.Contains(message_substring)) { return; }

            throw new Exception(String.Format("Expected BadRequestException:{0} was not met", message_substring));
        }

        internal static void FanoutSearchQueryException(System.Action<string> action, string param, string message_substring)
        {
            try
            {
                action(param);
            }
            catch (FanoutSearchQueryException ex) when (ex.Message.Contains(message_substring)) { return; }

            throw new Exception(String.Format("Expected BadRequestException:{0} was not met", message_substring));
        }
    }
}
