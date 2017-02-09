// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity;
using Trinity.Core.Lib;
using Trinity.Network;

namespace FanoutSearch.Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Usage();
                return;
            }

            if (args.Length >= 2) { TrinityConfig.StorageRoot = args[1]; }
            Global.LoadTSLStorageExtension(args[0]);
            Global.LocalStorage.LoadStorage();

            TrinityConfig.HttpPort = 80;
            FanoutSearchModule.EnableExternalQuery(true);
            FanoutSearchModule.SetQueryTimeout(3000);
            FanoutSearchModule.RegisterIndexService(Indexer);
            FanoutSearch.LambdaDSL.SetDialect("g", "v", "outV", "outE", "Action");

            TrinityServer server = new TrinityServer();
            server.RegisterCommunicationModule<FanoutSearchModule>();
            server.Start();
        }

        /// <summary>
        /// Simple hash-based indexer.
        /// </summary>
        private static IEnumerable<long> Indexer(object _queryObj, string type)
        {
            JObject queryObj = (JObject)_queryObj;
            string key = queryObj["key"].ToString();
            yield return HashHelper.HashString2Int64(key);
        }

        private static void Usage()
        {
            Console.WriteLine("usage: FanoutSearch.Server.exe tsl_assembly_file [storage_root]");
        }
    }
}