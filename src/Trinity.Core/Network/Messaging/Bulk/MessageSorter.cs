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

namespace Trinity.Network.Messaging
{
    /// <summary>
    /// Provides methods for partitioning messages.
    /// </summary>
    public class MessageSorter
    {
        private List<long>[] dest_cell_ids;
        private MemoryCloud memory_cloud;

        /// <summary>
        /// Constructs a MessageSorter instance
        /// </summary>
        public MessageSorter(MemoryCloud mc = null)
        {
            memory_cloud = mc;
            if (memory_cloud == null)
                memory_cloud = Global.CloudStorage;

            dest_cell_ids = new List<long>[memory_cloud.ServerCount];
            for (int i = 0; i < memory_cloud.ServerCount; i++)
            {
                dest_cell_ids[i] = new List<long>();
            }
        }

        /// <summary>
        /// Constructs a MessageSorter using a given cell Id list.
        /// </summary>
        /// <param name="cellIdList">A list of cell Ids.</param>
        public MessageSorter(List<long> cellIdList):this()
        {
            for (int i = 0; i < cellIdList.Count; i++)
            {
                dest_cell_ids[memory_cloud.StaticGetServerIdByCellId(cellIdList[i])].Add(cellIdList[i]);
            }
        }

        /// <summary>
        /// Adds a cellId to the sorter
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        public void Add(long cellId)
        {
            dest_cell_ids[memory_cloud.StaticGetServerIdByCellId(cellId)].Add(cellId);
        }

        /// <summary>
        /// Get the cell id list for the specified server.
        /// </summary>
        /// <param name="serverId">The server Id.</param>
        /// <returns>A list of cell Ids.</returns>
        public List<long> GetCellRecipientList(int serverId)
        {
            return dest_cell_ids[serverId];
        }
    }
}
