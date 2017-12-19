using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoTSL;
using Newtonsoft.Json;
using Trinity;
using Trinity.TSL.Lib;

namespace DemoTrinityStateless
{
    class DemoTrinityServer : DemoTrinityServerBase
    {
        public override void GetOrUpdateHandler(GetOrUpdateRequestReader request, JsonResponseWriter response)
        {
            var serverId = Global.CloudStorage.GetServerIdByCellId(request.cellId);
            if (serverId == Global.MyServerId)
            {
                var update = !string.IsNullOrEmpty(request.content);
                using (var cell = Global.LocalStorage.UseMyCell(request.cellId, update ? CellAccessOptions.CreateNewOnCellNotFound : CellAccessOptions.ReturnNullOnCellNotFound))
                {
                    if (cell != null && update)
                        cell.content = request.content;

                    response.json = JsonConvert.SerializeObject(new
                    {
                        ServerId = Global.MyServerId,
                        PartitionId = Global.CloudStorage.GetPartitionIdByCellId(request.cellId),
                        Cell = cell == null ? null : cell.ToString()
                    });
                }
            }
            else
            {
                using (var req = new GetOrUpdateRequestWriter(request.cellId, request.content))
                {
                    using (var resp = Global.CloudStorage.GetOrUpdateToDemoTrinityServer(serverId, req))
                    {
                        response.json = resp.json;
                    }
                }
            }
        }

        public override void SaveStorageHandler()
        {
            Global.LocalStorage.SaveStorage();
        }
    }
}
