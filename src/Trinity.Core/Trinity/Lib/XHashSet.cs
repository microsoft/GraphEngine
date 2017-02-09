// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trinity.Core.Lib
{
    /*
     * One hash set can store 47,995,853 
     * */
    class multi_guid_hashset
    {
        private HashSet<Int64>[] HashSetArray;
        private int m;
        public multi_guid_hashset(int k = 2)
        {
            this.m = k;
            HashSetArray = new HashSet<Int64>[m];
            for (int i = 0; i < m; i++)
            {
                HashSetArray[i] = new HashSet<Int64>();
            }
        }
        public void Add(Int64 guid)
        {
            HashSetArray[(guid.GetHashCode() & 0x7fffffff) % m].Add(guid);
        }
        public void Remove(Int64 guid)
        {
            HashSetArray[(guid.GetHashCode() & 0x7fffffff) % m].Remove(guid);
        }
        public void Clear()
        {
            for (int i = 0; i < m; i++)
            {
                HashSetArray[i].Clear();
            }
        }
        public Int64 Size()
        {
            Int64 size = 0;
            for (int i = 0; i < m; i++)
            {
                size += HashSetArray[i].Count;
            }
            return size;
        }
        public Int64 Count
        {
            get
            {
                return this.Size();
            }
        }
        internal void PrintInternalInfo()
        {
            for (int i = 0; i < m; i++)
            {
                Console.WriteLine("The Hashset[" + i + "].Count=" + HashSetArray[i].Count);
            }
        }
        public System.Collections.IEnumerator GetEnumerator()
        {
            for (int i = 0; i < m; i++)
            {
                foreach (Int64 guid in HashSetArray[i])
                {
                    yield return guid;
                }
            }
        }
    }

    [Serializable]
    internal class XHashSet<T> : IEnumerable<T>, IEnumerable
    {
        private HashSet<T>[] HashSetArray;
        private int m;
        public XHashSet(int k = 2)
        {
            this.m = k;
            HashSetArray = new HashSet<T>[m];
            for (int i = 0; i < m; i++)
            {
                HashSetArray[i] = new HashSet<T>();
            }
        }
        public bool Add(T item)
        {
            return HashSetArray[(item.GetHashCode() & 0x7fffffff) % m].Add(item);
        }

        public bool Contains(T item)
        {
            return HashSetArray[(item.GetHashCode() & 0x7fffffff) % m].Contains(item);
        }

        public void Add(ICollection<T> collection)
        {
            foreach (T item in collection)
            {
                HashSetArray[(item.GetHashCode() & 0x7fffffff) % m].Add(item);
            }
        }
        public void Remove(T item)
        {
            HashSetArray[(item.GetHashCode() & 0x7fffffff) % m].Remove(item);
        }
        public void Remove(ICollection<T> collection)
        {
            foreach (T item in collection)
            {
                HashSetArray[(item.GetHashCode() & 0x7fffffff) % m].Remove(item);
            }
        }
        public void Clear()
        {
            for (int i = 0; i < m; i++)
            {
                HashSetArray[i].Clear();
            }
        }
        public Int64 Size()
        {
            Int64 size = 0;
            for (int i = 0; i < m; i++)
            {
                size += HashSetArray[i].Count;
            }
            return size;
        }
        public Int64 Count
        {
            get
            {
                return this.Size();
            }
        }
        internal void PrintInternalInfo()
        {
            for (int i = 0; i < m; i++)
            {
                Console.WriteLine("The Hashset[" + i + "].Count=" + HashSetArray[i].Count);
            }
        }
        public System.Collections.Generic.IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < m; i++)
            {
                foreach (T guid in HashSetArray[i])
                {
                    yield return guid;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }
}
