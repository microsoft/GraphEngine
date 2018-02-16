using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Trinity.Core.Lib;
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

        public static InMemoryDataChunk New(IEnumerable<CellInfo> cells, int estimated_size)
        {
            MemoryStream ms = new MemoryStream(estimated_size);
            byte[] buf = new byte[sizeof(long) + sizeof(ushort) + sizeof(int)];
            byte[] buf_cell = new byte[1024];
            long lowkey = 0, highkey = 0;
            int size = 0;
            fixed (byte* p = buf)
            {
                foreach (var cell in cells)
                {
                    if (size == 0) lowkey = cell.CellId;
                    highkey = cell.CellId;
                    PointerHelper sp = PointerHelper.New(p);
                    *sp.lp++ = cell.CellId;
                    *sp.sp++ = (short)cell.CellType;
                    *sp.ip++ = cell.CellSize;
                    ms.Write(buf, 0, buf.Length);
                    while(buf_cell.Length < cell.CellSize)
                    {
                        buf_cell = new byte[buf_cell.Length * 2];
                    }
                    Memory.Copy(cell.CellPtr, 0, buf_cell, 0, cell.CellSize);
                    ms.Write(buf_cell, 0, cell.CellSize);
                    size += cell.CellSize + buf.Length;
                }
            }
            byte[] payload = ms.GetBuffer();
            Chunk chunk = new Chunk(lowkey, highkey);
            return new InMemoryDataChunk(chunk, payload, lowkey, highkey);
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
