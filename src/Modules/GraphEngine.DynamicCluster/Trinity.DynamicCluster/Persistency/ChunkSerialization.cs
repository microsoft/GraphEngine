using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Trinity.Storage;

namespace Trinity.DynamicCluster.Persistency
{
    public static class ChunkSerialization
    {
        [Serializable]
        public struct PseudoChunk
        {
            [JsonProperty("Id")] public Guid Id;
            [JsonProperty("HighKey")] public long HighKey;
            [JsonProperty("LowKey")] public long LowKey;
        }
        public static Chunk PseudoBox(PseudoChunk pchunk) => new Chunk(pchunk.LowKey, pchunk.HighKey, pchunk.Id);

        public static string ToString(this Chunk chunk) => JsonConvert.SerializeObject(chunk);

        public static Chunk Parse(string str) => PseudoBox((JsonConvert.DeserializeObject<PseudoChunk>(str)));
    }
}