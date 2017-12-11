using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Trinity.DynamicCluster.Persistency;
using Trinity.Storage;

namespace Trinity.Azure.Storage
{
    unsafe class BlobDataChunk : IPersistentDataChunk
    {
        private Chunk data_chunk;
        private byte[] content;
        private long target_lowKey;
        private long target_highKey;

        public BlobDataChunk(Chunk chunk, byte[] content, long lowKey, long highKey)
        {
            this.data_chunk     = chunk;
            this.content        = content;
            this.target_lowKey  = lowKey;
            this.target_highKey = highKey;
        }

        public Chunk DataChunkRange => data_chunk;

        public void Dispose() { } 

        public IEnumerator<PersistedCell> GetEnumerator()
        {
            return new PersistedCellEnumerator(content, target_lowKey, target_highKey);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
