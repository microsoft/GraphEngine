// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Trinity.Core.Lib
{
    unsafe class TrinityList
    {
        long[] Key = new long[65536];
        int[] NextEntryArray = new int[65536];
        int[] Buckets = new int[BucketCount];
        const int TAIL = 0xFFFFFF;
        const int HEAD = 0xFFFFFF;

        int NonEmptyEntryCount = 0;
        const int BucketCount = 3;

        public TrinityList()
        {
            for (int i = 0; i < BucketCount; i++)
            {
                Buckets[i] = HEAD;
            }
        }

        private static int UnmarkedValue(int value)
        {
            return value & 0xFFFFFF;
        }

        private static int MarkedValue(int value)
        {
            return value | 0x1000000;
        }

        private static bool IsMarked(int value)
        {
            return (value & 0xFF000000) == 0x1000000;
        }

        private int Search(long key, int bucket_index, out int left_node)
        {
            left_node = HEAD;
            int left_node_next = -1, right_node = -1;
        search_again:
            do
            {
                int t = HEAD;
                int t_next = Buckets[bucket_index];

                do
                {
                    if (!IsMarked(t_next))
                    {
                        left_node = t;
                        left_node_next = t_next;
                    }
                    t = UnmarkedValue(t_next);
                    if (t == TAIL)
                        break;
                    t_next = NextEntryArray[t];
                } while (IsMarked(t_next) || (Key[t] < key));

                right_node = t;

                if (left_node_next == right_node)
                    if ((right_node != TAIL) && IsMarked(NextEntryArray[right_node]))
                        goto search_again;
                    else
                        return right_node;
                if (Interlocked.CompareExchange(ref NextEntryArray[left_node], right_node, left_node_next) == left_node_next)
                    if ((right_node != TAIL) && IsMarked(NextEntryArray[right_node]))
                        goto search_again;
                    else
                        return right_node;
            } while (true);
        }

        public bool Insert(long key)
        {
            int bucket_index = key.GetHashCode() % BucketCount;
            int index = Interlocked.Increment(ref NonEmptyEntryCount) - 1;
            Key[index] = key;
            int right_node, left_node;
            do
            {
                right_node = Search(key, bucket_index, out left_node);
                if ((right_node != TAIL) && (Key[right_node] == key))
                    return false;
                NextEntryArray[index] = right_node;
                if (left_node == HEAD)
                {
                    if (Interlocked.CompareExchange(ref Buckets[bucket_index], index, right_node) == right_node)
                        return true;
                }
                else
                    if (Interlocked.CompareExchange(ref NextEntryArray[left_node], index, right_node) == right_node)
                        return true;
            } while (true);
        }

        public bool Delete(long key)
        {
            int bucket_index = key.GetHashCode() % BucketCount;
            int right_node, right_node_next, left_node;
            do
            {
                right_node = Search(key, bucket_index, out left_node);
                if ((right_node == TAIL) || Key[right_node] != key)
                    return false;
                right_node_next = NextEntryArray[right_node];
                if (!IsMarked(right_node_next))
                    if (Interlocked.CompareExchange(ref NextEntryArray[right_node], MarkedValue(right_node_next), right_node_next) == right_node_next)
                        break;
            } while (true);
            if (left_node == HEAD)
            {
                if (Interlocked.CompareExchange(ref Buckets[bucket_index], right_node_next, right_node) != right_node)
                    right_node = Search(Key[right_node], bucket_index, out left_node);
            }
            else
                if (Interlocked.CompareExchange(ref NextEntryArray[left_node], right_node_next, right_node) != right_node)
                    right_node = Search(Key[right_node], bucket_index, out left_node);
            return true;
        }

        public bool Find(long key)
        {
            int bucket_index = key.GetHashCode() % BucketCount;
            int right_node, left_node;
            right_node = Search(key, bucket_index, out left_node);
            if ((right_node == TAIL) || (Key[right_node] != key))
                return false;
            else
                return true;
        }
    }
}
