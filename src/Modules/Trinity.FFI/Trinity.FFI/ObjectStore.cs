using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Trinity.FFI
{
    internal class ObjectStore<T>
    {
        // 16 + 8 bytes per entry
        internal struct AssocT
        {
            public T value;
            public int next;
        }

        protected AssocT[] m_array = null;

        private long m_pad0 = 0;
        private long m_pad1 = 0;
        private long m_pad2 = 0;
        private long m_pad3 = 0;
        private long m_pad4 = 0;
        private long m_pad5 = 0;
        private long m_pad6 = 0;

        protected int m_head = 0;
        protected int m_len = 0;

        private long m_pad7 = 0;
        private long m_pad8 = 0;
        private long m_pad9 = 0;
        private long m_pad10 = 0;
        private long m_pad11 = 0;
        private long m_pad12 = 0;
        private long m_pad13 = 0;

        protected object m_lock;

        private const int c_default_len = 1 << 20;

        public ObjectStore()
        {
            m_lock = new object();
            _Alloc_Init(c_default_len);
            
        }
        public Action<int> _Alloc_Mutable;

        private void _Alloc_Init(int newlen)
        {
            AssocT[] ret = new AssocT[newlen];
            for (int init_from = 0; init_from < newlen; ++init_from)
            {
                ret[init_from].next = init_from + 1;
                ret[init_from].value = default(T);
            }
            m_array = ret;
            m_len = m_array.Length;
            _Alloc_Mutable = _Alloc_Normal;
        }
        private void _Alloc_Normal(int newlen)
        {
            if (newlen < 0) throw new OutOfMemoryException();

            AssocT[] ret = new AssocT[newlen];
            int init_from = m_array.Length;
            m_head = init_from;
            Array.Copy(m_array, ret, init_from);

            for (; init_from < newlen; ++init_from)
            {
                ret[init_from].next = init_from + 1;
                ret[init_from].value = default(T);
            }
            m_array = ret;
            m_len = m_array.Length;
        }

        private void _Alloc(int newlen)
        {
            _Alloc_Mutable(newlen);
        }

        public void DeAlloc()
        {
            m_array = null;
            m_lock = null;
            _Alloc_Mutable = _Alloc_Init;
        }

        public int Put(T value)
        {
begin:
            int head = m_head;
            if (head >= m_len) lock (m_lock)
                {
                    if (m_head > m_len)
                    {
                        _Alloc(CalcLen(m_len));
                    }
                    goto begin;
                }
            /*****************
             * !!ABA:
             * Thread 1 reads head and next = X;
             * Thread 2 TAKES head, move m_head to X
             * Thread 3 TAKES X, move m_head to Y
             * Thread 2 RETURNS head, set m_head to head, but m_head.next = Y this time.
             * Thread 1 mistakenly thinks the next ptr is not taken, and thus m_head move to X
             * Thread 4 TAKES X (!!), but sees active data held by thread 3, backoff, PROBLEM SOLVED.
             * 
             * To wrap it up, m_head STALLS when it's not kept up to date. However, if the unit
             * captured is not available, we can simply backoff and catch up with the latest data.
             * 
             *****************/

            if (head != Interlocked.CompareExchange(ref m_head, m_array[head].next, head)) goto begin;
            if (!m_array[head].value.Equals(default(T))) goto begin; // <- head stall backoff
            m_array[head].value = value;
            return head;
        }

        private int CalcLen(int m_len) => m_len + (m_len >> 1);

        public T Get(int index) => m_array[index].value;

        public void Del(int index)
        {
            m_array[index].value = default(T);
begin:
            int head = m_head;
            m_array[index].next = head;
            if (head != Interlocked.CompareExchange(ref m_head, index, head)) goto begin;
        }
    }

    internal class DisposableStore<T> : ObjectStore<T>, IDisposable
        where T : IDisposable
    {
        public DisposableStore() : base() { }

        public new void Del(int index)
        {
            m_array[index].value.Dispose();
            m_array[index].value = default(T);
begin:
            int head = m_head;
            m_array[index].next = head;
            if (head != Interlocked.CompareExchange(ref m_head, index, head)) goto begin;
        }

        public void Dispose()
        {
            lock (m_lock)
            {
                foreach (var tuple in m_array)
                {
                    if (tuple.value != null) tuple.value.Dispose();
                }
            }
            DeAlloc();
        }
    }

}
