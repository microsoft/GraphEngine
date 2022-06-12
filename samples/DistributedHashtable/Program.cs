// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trinity;
using Trinity.Core.Lib;
using Trinity.Extension.DistributedHashtable;

namespace DistributedHashtable
{
    class Program
    {
        static void Main(string[] args)
        {
            TrinityConfig.AddServer(new Trinity.Network.ServerInfo("127.0.0.1", 5304, Global.MyAssemblyPath, Trinity.Diagnostics.LogLevel.Error)); 
            if (args.Length < 1)
            {
                Console.WriteLine("Please provide a command line parameter (-s|-c).");
                Console.WriteLine("  -s  Start as a distributed hashtable server.");
                Console.WriteLine("  -c  Start as a distributed hashtable client.");
                return;
            }

            if (args[0].Trim().ToLower().StartsWith("-s"))
            {
                DistributedHashtableServer server = new DistributedHashtableServer();
                server.Start();
                while(true) {
                  Thread.Sleep(1000);
                }
            }

            if (args[0].Trim().ToLower().StartsWith("-c"))
            {
                HandlingUserInput();
            }
        }

        static void HandlingUserInput()
        {
            TrinityConfig.CurrentRunningMode = RunningMode.Client;
            while (true)
            {
                Console.WriteLine("Please input a command (set|get):");
                string input = Console.ReadLine().Trim();
                string[] fields = input.Split(new char[] { '(', ',', ')' }, StringSplitOptions.RemoveEmptyEntries);

                if (fields.Length > 0)
                {
                    string command = fields[0].Trim().ToLower();

                    #region get
                    if (command.Equals("get"))
                    {
                        if (fields.Length < 2)
                        {
                            Console.WriteLine("example: get(key1) or get(\"key 2\")");
                            continue;
                        }
                        string key = fields[1].Trim().Trim(new char[] { '\"' });
                        using (var request = new GetMessageWriter(key))
                        {
                            using (var response = Global.CloudStorage.GetToDistributedHashtableServer(DistributedHashtableServer.GetServerIdByKey(key), request))
                            {
                                if (response.IsFound)
                                    Console.WriteLine("The value of \"{0}\" is \"{1}\"", key, response.Value);
                                else
                                    Console.WriteLine("The key is not found.");
                            }
                        }
                        continue;
                    }
                    #endregion

                    #region set
                    if (command.Equals("set"))
                    {
                        if (fields.Length < 3)
                        {
                            Console.WriteLine("example: set(key1, value1) or set(\"key 2\", \"value 2\")");
                            continue;
                        }
                        string key = fields[1].Trim().Trim(new char[] { '\"' });
                        string value = fields[2].Trim().Trim(new char[] { '\"' });
                        using (var request = new SetMessageWriter(key, value))
                        {
                            Console.WriteLine("Set the value of \"{0}\" is \"{1}\"", key, value);
                            Global.CloudStorage.SetToDistributedHashtableServer(DistributedHashtableServer.GetServerIdByKey(key), request);
                        }
                        continue;
                    }
                    #endregion
                }
                Console.WriteLine("Please input a valid command (set|get):");
            }
        }
    }
}
