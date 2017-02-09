// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trinity.Core.Lib
{
    internal class XArray<T>
    {
        T[][] ArrayTable;
        int K;
        long Capacity;

        public XArray(int k, long single_capacity = 0x4000000) //default = 64M
        {
            K = k;
            Capacity = single_capacity;
            ArrayTable = new T[K][];

            for (int i = 0; i < K; i++)
            {
                ArrayTable[i] = new T[single_capacity];
            }
        }

        public T this[long key]
        {            
            get
            {
                long index = key / Capacity;
                long offset = key % Capacity;
                return ArrayTable[index][offset];
            }
            set
            {
                long index = key / Capacity;
                long offset = key % Capacity;
                ArrayTable[index][offset] = value;
            }
        }
    }
}
