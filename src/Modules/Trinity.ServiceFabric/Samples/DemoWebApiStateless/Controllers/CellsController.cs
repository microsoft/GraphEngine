using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Trinity;
using DemoTSL;
using Newtonsoft.Json;

namespace DemoWebApiStateless.Controllers
{
    [Route("api/[controller]")]
    public class CellsController : Controller
    {
        private static Random rand = new Random();

        // GET api/cells/5
        [HttpGet("{id}"), Route("cells")]
        public object GetOrUpdate(long id, [FromQuery]string content)
        {
            return GetOrUpdateCell(id, content);
        }

        // GET api/cells/save
        [HttpGet, Route("save")]
        public string Save()
        {
            for (var i = 0; i < Global.ServerCount; i++)
            {
                Global.CloudStorage.SaveStorageToDemoTrinityServer(i);
            }

            return "saving";
        }

        public static object GetOrUpdateCell(long id, string content)
        {
            var serverId = rand.Next() % Global.ServerCount;

            using (var request = new GetOrUpdateRequestWriter(id, content))
            {
                using (var response = Global.CloudStorage.GetOrUpdateToDemoTrinityServer(serverId, request))
                {
                    return JsonConvert.DeserializeObject(response.json);
                }
            }
        }
    }
}
