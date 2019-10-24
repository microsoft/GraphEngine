// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Trinity;
using Trinity.Storage;
using Trinity.Network.Messaging;

namespace SSSP
{
    class SSSPServerImpl : SSSPServerBase
    {
        public override void DistanceUpdatingHandler(DistanceUpdatingMessageReader request)
        {
            List<DistanceUpdatingMessage> DistanceUpdatingMessageList = new List<DistanceUpdatingMessage>();
            request.recipients.ForEach((cellId) =>
            {
                using (var cell = Global.LocalStorage.UseSSSPCell(cellId))
                {
                    if (cell.distance > request.distance + 1)
                    {
                        cell.distance = request.distance + 1;
                        cell.parent = request.senderId;
                        Console.Write(cell.distance + " ");
                        MessageSorter sorter = new MessageSorter(cell.neighbors);

                        for (int i = 0; i < Global.ServerCount; i++)
                        {
                            DistanceUpdatingMessageWriter msg = new DistanceUpdatingMessageWriter(cell.CellId,
                                cell.distance, sorter.GetCellRecipientList(i));
                            Global.CloudStorage.DistanceUpdatingToSSSPServer(i, msg);
                        }

                    }
                }
            });
        }

        public override void StartSSSPHandler(StartSSSPMessageReader request)
        {
            if (Global.CloudStorage.IsLocalCell(request.root))
            {
                using (var rootCell = Global.LocalStorage.UseSSSPCell(request.root))
                {
                    rootCell.distance = 0;
                    rootCell.parent = -1;
                    MessageSorter sorter = new MessageSorter(rootCell.neighbors);

                    for (int i = 0; i < Global.ServerCount; i++)
                    {
                        DistanceUpdatingMessageWriter msg = new DistanceUpdatingMessageWriter(rootCell.CellId, 0, sorter.GetCellRecipientList(i));
                        Global.CloudStorage.DistanceUpdatingToSSSPServer(i, msg);
                    }
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            TrinityConfig.AddServer(new Trinity.Network.ServerInfo("127.0.0.1", 5304, Global.MyAssemblyPath, Trinity.Diagnostics.LogLevel.Error));

            if (args.Length >= 1 && args[0].StartsWith("-s"))
            {
                SSSPServerImpl server = new SSSPServerImpl();
                server.Start();
            }

            //SSSP.exe -c startcell
            if (args.Length >= 2 && args[0].StartsWith("-c"))
            {
                TrinityConfig.CurrentRunningMode = RunningMode.Client;
                for (int i = 0; i < Global.ServerCount; i++)
                {
                    using (var msg = new StartSSSPMessageWriter(long.Parse(args[1].Trim())))
                    {
                        Global.CloudStorage.StartSSSPToSSSPServer(i, msg);
                    }
                }
            }

            //SSSP.exe -q cellID
            if (args.Length >= 2 && args[0].StartsWith("-q"))
            {
                TrinityConfig.CurrentRunningMode = RunningMode.Client;
                var cell = Global.CloudStorage.LoadSSSPCell(int.Parse(args[1]));
                Console.WriteLine("Current vertex is {0}, the distance to the source vertex is {1}.",
                    cell.CellId, cell.distance);
                while (cell.distance > 0)
                {
                    cell = Global.CloudStorage.LoadSSSPCell(cell.parent);
                    Console.WriteLine("Current vertex is {0}, the distance to the source vertex is {1}.",
                        cell.CellId, cell.distance);
                }

            }

            //SSSP.exe -g node count
            if (args.Length >= 2 && args[0].StartsWith("-g"))
            {
                TrinityConfig.CurrentRunningMode = RunningMode.Client;

                Random rand = new Random();
                int nodeCount = int.Parse(args[1].Trim());
                for (int i = 0; i < nodeCount; i++)
                {
                    HashSet<long> neighbors = new HashSet<long>();
                    for (int j = 0; j < 10; j++)
                    {
                        long neighhor = rand.Next(0, nodeCount);
                        if (neighhor != i)
                        {
                            neighbors.Add(neighhor);
                        }
                    }
                    Global.CloudStorage.SaveSSSPCell(i, distance: int.MaxValue, parent: -1, neighbors: neighbors.ToList());
                }
            }
        }
    }
}

