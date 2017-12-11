using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.DynamicCluster.Persistency;
using Trinity.Storage;

namespace Trinity.Azure.Storage
{
    class BlobDataChunk : IPersistentDataChunk
    {
        public Chunk DataChunkRange => throw new NotImplementedException();

        public IEnumerable<PersistedCell> GetCells => throw new NotImplementedException();

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
