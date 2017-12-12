using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Trinity.DynamicCluster.Persistency
{
    internal unsafe class PersistedCellEnumerator : IEnumerator<PersistedCell>
    {
        private byte[] m_content;
        private int m_length;
        private long m_target_lowKey;
        private long m_target_highKey;
        private SmartPointer m_sp;
        private byte* m_bp;
        private byte* m_ep;
        private GCHandle m_handle;

        public PersistedCellEnumerator(byte[] content, long target_lowKey, long target_highKey)
        {
            m_content        = content;
            m_length         = content.Length;
            m_target_lowKey  = target_lowKey;
            m_target_highKey = target_highKey;
            m_handle         = GCHandle.Alloc(content, GCHandleType.Pinned);
            m_bp             = (byte*)m_handle.AddrOfPinnedObject().ToPointer();
            m_sp             = SmartPointer.New(m_bp);
            m_ep             = m_bp + m_length;
        }

        public PersistedCell Current => new PersistedCell
        {
            Buffer   = m_content,
            CellId   = *m_sp.lp,
            CellType = *(ushort*)(m_sp.bp + sizeof(long)),
            Length   = *(int*)(m_sp.bp + sizeof(long)+sizeof(ushort)),
            Offset   = (int)(m_sp.bp + sizeof(long) + sizeof(ushort) + sizeof(int) - m_bp)
        };

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            m_handle.Free();
        }

        public bool MoveNext()
        {
retry:
            if (m_sp.bp >= m_ep) return false;
            m_sp.lp++;
            m_sp.sp++;
            m_sp.bp += *m_sp.ip + sizeof(int);
            if (m_sp.bp < m_ep)
            {
                var id = *m_sp.lp;
                if (id < m_target_lowKey || id > m_target_highKey) goto retry;
            }

            return true;
        }

        public void Reset()
        {
            m_sp.bp = m_bp;
        }
    }
}