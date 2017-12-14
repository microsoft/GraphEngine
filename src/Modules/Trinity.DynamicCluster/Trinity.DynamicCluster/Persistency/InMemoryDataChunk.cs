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

namespace Trinity.DynamicCluster.Persistency
{
    public unsafe class InMemoryDataChunk : IPersistentDataChunk
    {
        private Chunk  m_data_chunk;// actual low/high keys
        private byte[] m_content;
        private long   m_target_lowKey;
        private long   m_target_highKey;

        public InMemoryDataChunk(Chunk chunk, byte[] content, long lowKey, long highKey)
        {
            this.m_data_chunk     = chunk;
            this.m_content        = content;
            this.m_target_lowKey  = lowKey;
            this.m_target_highKey = highKey;
        }

        public Chunk DataChunkRange => m_data_chunk;
        public byte[] GetBuffer() => m_content;

        public void Dispose() { } 

        public IEnumerator<PersistedCell> GetEnumerator()
        {
            return new PersistedCellEnumerator(m_content, m_target_lowKey, m_target_highKey);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }
}
