using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Storage;

namespace Trinity.DynamicCluster.Persistency
{
    public static class ChunkSerialization
    {
        public static string ToString(this Chunk chunk) => JsonConvert.SerializeObject(chunk);
        public static Chunk Parse(string str) => JsonConvert.DeserializeObject<Chunk>(str);
    }
}
