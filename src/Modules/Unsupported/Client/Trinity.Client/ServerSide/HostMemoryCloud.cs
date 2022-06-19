using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trinity.Extension;
using Trinity.Storage;

namespace Trinity.Client.ServerSide
{
    internal class ClientHostStorageTable : IList<IStorage>
    {
        private ConcurrentDictionary<int, IStorage> m_clientTable;
        private Random m_rng;

        private IList<IStorage> m_baseTable = null;

        public ClientHostStorageTable()
        {
            m_clientTable = new ConcurrentDictionary<int, IStorage>();
            m_rng = new Random();
        }

        internal void SetBase(IList<IStorage> bt) => m_baseTable = bt;

        public IStorage this[int index]
        {
            get
            {
                if (m_clientTable.TryGetValue(index, out var client)) return client;
                else return m_baseTable[index];
            }
            set
            {
                m_clientTable[index] = value;
            }
        }

        public int InsertClient(IStorage cstg)
        {
            while (true)
            {
                int id = m_rng.Next(int.MinValue, -1);
                if (m_clientTable.TryAdd(id, cstg)) return id;
            }
        }

        public void RemoveClient(int idx)
        {
            m_clientTable.TryRemove(idx, out _);
        }

        #region Hiding clients
        public int Count => m_baseTable.Count;

        public bool IsReadOnly => false;

        public void Add(IStorage item) { m_baseTable.Add(item); }

        public void Clear()
        {
            m_clientTable.Clear();
            m_baseTable.Clear();
        }

        public bool Contains(IStorage item) => m_clientTable.Values.Contains(item) || m_baseTable.Contains(item);

        public void CopyTo(IStorage[] array, int arrayIndex) => m_baseTable.CopyTo(array, arrayIndex);

        public IEnumerator<IStorage> GetEnumerator() => m_baseTable.GetEnumerator();

        public int IndexOf(IStorage item) => m_baseTable.IndexOf(item);

        public void Insert(int index, IStorage item) => m_baseTable.Insert(index, item);

        public bool Remove(IStorage item) => m_baseTable.Remove(item);

        public void RemoveAt(int index) => m_baseTable.RemoveAt(index);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion
    }

    // TODO make FixedMemoryCloud a component instead of base -- and thus we can support other types of MCs
    [ExtensionPriority(-80)]
    public class HostMemoryCloud : FixedMemoryCloud, IClientRegistry
    {
        private volatile ClientHostStorageTable m_hostTable = null;
        protected override IList<IStorage> StorageTable { get { var bt = base.StorageTable; m_hostTable.SetBase(bt); return m_hostTable; } }

        public override bool Open(ClusterConfig config, bool nonblocking)
        {
            m_hostTable = new ClientHostStorageTable();
            var ret = base.Open(config, nonblocking);
            return ret;
        }

        public int RegisterClient(IStorage client)
        {
            while (m_hostTable == null) ;
            return m_hostTable.InsertClient(client);
        }

        public void UnregisterClient(int instanceId)
        {
            while (m_hostTable == null) ;
            m_hostTable.RemoveClient(instanceId);
        }
    }
}
