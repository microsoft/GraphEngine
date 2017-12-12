using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Storage;

namespace Trinity.DynamicCluster.Persistency
{
    public interface IPersistentDataChunk : IEnumerable<PersistedCell>, IDisposable
    {
        /// <summary>
        /// Indicates the range of the data chunk. Also, an implementation
        /// can use the Guid tag to uniquely identify this data chunk.
        /// </summary>
        Chunk DataChunkRange { get; }
        // TODO expose buffer with getter/setter, and thus make an abstraction and decouple serializer/deserializer
        // from data chunk. Need a GUID identifier to resolve serializers

        byte[] GetBuffer();
    }
}
