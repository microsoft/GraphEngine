// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Trinity;
using Trinity.Storage;

namespace Ping
{
    class MyPingServer : MyServerBase
    {
        public override void SynPingHandler(MyMessageReader request)
        {
            Console.WriteLine("Received SynPing, sn={0}", request.sn);
        }

        public override void AsynPingHandler(MyMessageReader request)
        {
            Console.WriteLine("Received AsynPing, sn={0}", request.sn);
        }

        public override void SynEchoPingHandler(MyMessageReader request,
        MyMessageWriter response)
        {
            Console.WriteLine("Received SynEchoPing, sn={0}", request.sn);
            response.sn = request.sn;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var server = new MyPingServer();
            server.Start();

            var synReq = new MyMessageWriter(0, 1);

            Global.CloudStorage.SynPingToMyServer(0, synReq);

            var asynReq = new MyMessageWriter(0,2);
            Global.CloudStorage.AsynPingToMyServer(0, asynReq);

            var synReqRsp = new MyMessageWriter(0,3);
            Console.WriteLine("Response of EchoPing: {0}", Global.CloudStorage.SynEchoPingToMyServer(0, synReqRsp).sn);

            Console.WriteLine("Done.");
            Console.WriteLine("Press any key to exit ...");
            Console.ReadKey();
            Global.Exit(0);
        }
    }
}

