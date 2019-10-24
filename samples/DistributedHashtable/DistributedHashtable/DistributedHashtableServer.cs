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
using Trinity.Core.Lib;
using Trinity.Extension.DistributedHashtable;
using Trinity.TSL.Lib;

namespace DistributedHashtable
{
    class DistributedHashtableServer : DistributedHashtableServerBase
    {
        internal static int GetServerIdByKey(string key)
        {
            return Global.CloudStorage.GetPartitionIdByCellId(HashHelper.HashString2Int64(key));
        }
        public override void GetHandler(GetMessageReader request, GetResponseWriter response)
        {
            long cellId = HashHelper.HashString2Int64(request.Key);
            response.IsFound = false;

            using (var cell = Global.LocalStorage.UseBucketCell(cellId, CellAccessOptions.ReturnNullOnCellNotFound))
            {
                if (cell == null)
                    return;
                int count = cell.KVList.Count;
                for (int i = 0; i < count; i++)
                {
                    if (cell.KVList[i].Key == request.Key)
                    {
                        response.IsFound = true;
                        response.Value = cell.KVList[i].Value;
                        break;
                    }
                }
            }
        }

        public override void SetHandler(SetMessageReader request)
        {
            long cellId = HashHelper.HashString2Int64(request.Key);
            using (var cell = Global.LocalStorage.UseBucketCell(cellId, CellAccessOptions.CreateNewOnCellNotFound))
            {
                int count = cell.KVList.Count;
                int index = -1;

                for (int i = 0; i < count; i++)
                {
                    if (cell.KVList[i].Key == request.Key)
                    {
                        index = i;
                        break;
                    }
                }
                if (index != -1)
                {
                    cell.KVList[index].Value = request.Value;
                }
                else
                    cell.KVList.Add(new KVPair(request.Key, request.Value));
            }
        }
    }
}
