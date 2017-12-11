using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Storage;

namespace Trinity.DynamicCluster.Persistency
{
    public interface IPersistentDataChunk : IDisposable
    {
        /// <summary>
        /// Indicates the range of the data chunk. Also, an implementation
        /// can use the Guid tag to uniquely identify this data chunk.
        /// </summary>
        Chunk DataChunkRange { get; }
        IEnumerable<PersistedCell> GetCells { get; }
    }
}
