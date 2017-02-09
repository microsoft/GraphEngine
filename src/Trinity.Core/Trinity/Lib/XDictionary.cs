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
    internal class XDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
    {
        private Dictionary<TKey, TValue>[] DictionaryArray;
        private int m;
        public List<TKey> Keys
        {
            get
            {
                List<TKey> keys = new List<TKey>();
                foreach ( Dictionary<TKey, TValue> dict in DictionaryArray )
                    keys.AddRange( dict.Keys );
                return keys;
            }
        }
        public XDictionary(int k = 2)
        {
            this.m = k;
            DictionaryArray = new Dictionary<TKey, TValue>[m];
            for (int i = 0; i < m; i++)
            {
                DictionaryArray[i] = new Dictionary<TKey, TValue>();
            }
        }

        public XDictionary(int k, int single_capacity)
        {
            this.m = k;
            DictionaryArray = new Dictionary<TKey, TValue>[m];
            for (int i = 0; i < m; i++)
            {
                DictionaryArray[i] = new Dictionary<TKey, TValue>(single_capacity);
            }
        }

        public void Add(TKey key, TValue value)
        {
            DictionaryArray[ (key.GetHashCode() & 0x7fffffff) % m].Add(key, value);
        }
        public bool Remove(TKey key)
        {
            return DictionaryArray[(key.GetHashCode() & 0x7fffffff) % m].Remove(key);
        }

        public bool ContainsKey(TKey key)
        {
            return DictionaryArray[(key.GetHashCode() & 0x7fffffff) % m].ContainsKey(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return DictionaryArray[(key.GetHashCode() & 0x7fffffff) % m].TryGetValue(key, out value);
        }

        public TValue this[TKey key]
        {
            get
            {
                return DictionaryArray[(key.GetHashCode() & 0x7fffffff) % m][key];
            }
            set
            {
                DictionaryArray[(key.GetHashCode() & 0x7fffffff) % m][key] = value;
            }
        }

        public void Clear()
        {
            for (int i = 0; i < m; i++)
            {
                DictionaryArray[i].Clear();
            }
        }
        public Int64 Size()
        {
            Int64 size = 0;
            for (int i = 0; i < m; i++)
            {
                size += DictionaryArray[i].Count;
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
                Console.WriteLine("The Dictionary[" + i + "].Count=" + DictionaryArray[i].Count);
            }
        }
        public System.Collections.Generic.IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            for (int i = 0; i < m; i++)
            {
                foreach (KeyValuePair<TKey, TValue> key_value_pair in DictionaryArray[i])
                {
                    yield return key_value_pair;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
