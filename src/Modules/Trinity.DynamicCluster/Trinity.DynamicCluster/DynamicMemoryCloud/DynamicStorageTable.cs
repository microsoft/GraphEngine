using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Trinity.DynamicCluster.Storage
{
    using Storage = Trinity.Storage.Storage;
    using static Utils;
    using System.Collections.Immutable;

    /// <summary>
    /// An array-like storage table populated with partitions and instances.
    /// From <![CDATA[IList<Storage>]]> interface, only the partitions are exposed.
    /// The instances are to be retrieved separatedly.
    /// </summary>
    internal class DynamicStorageTable : IList<Storage>
    {
        private ImmutableDictionary<int, Storage> m_instances = ImmutableDictionary<int, Storage>.Empty;
        private Storage[] m_partitions;
        private object m_lock = new object();
        private int m_pcnt;

        public DynamicStorageTable(int partitionCount)
        {
            this.m_partitions   = Infinity<Partition>()
                                   .Take(partitionCount)
                                   .ToArray();
            this.m_pcnt = partitionCount;
        }

        /// <summary>
        /// Adds an instance
        /// </summary>
        public void AddInstance(int id, Storage storage)
        {
            lock (m_lock)
            {
                m_instances = m_instances.Add(id, storage);
            }
        }

        /// <summary>
        /// Removes the instance from the table
        /// </summary>
        public void RemoveInstance(int id)
        {
            lock (m_lock)
            {
                m_instances = m_instances.Remove(id);
            }
        }

        /// <summary>
        /// The indexer behaves like a "leaky" C-style array.
        /// Although Count reports only the number of partitions,
        /// and the enumerators reflects this value, the indexer
        /// can be used to access the "hidden" instances.
        /// </summary>
        public Storage this[int index]
        {
            get
            {
                if (index >= 0 && index < m_pcnt)
                    return m_partitions[index];
                else
                    return m_instances[index];
            }
            set => m_partitions[index] = value;
        }

        public int Count => m_pcnt;

        public bool IsReadOnly => true;

        public bool Contains(Storage item) => m_partitions.Contains(item);

        public void CopyTo(Storage[] array, int arrayIndex)
        {
            Array.Copy(m_partitions, 0, array, arrayIndex, m_pcnt);
        }

        public int IndexOf(Storage item) => Array.FindIndex(m_partitions, _ => _ == item);

        #region NotSupported -- readonly
        public void Insert(int index, Storage item)
        {
            throw new NotSupportedException();
        }

        public void Add(Storage item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Remove(Storage item)
        {
            throw new NotSupportedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        #endregion
        public IEnumerator<Storage> GetEnumerator() => m_partitions.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}