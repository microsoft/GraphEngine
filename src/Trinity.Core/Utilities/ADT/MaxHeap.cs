// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trinity.Utilities
{
    internal class MaxHeap
    {
        public static void BuildHeap<T>(T[] array, Comparison<T> comparison)
        {
            if (array.Length > 1)
            {
                for (int i = array.Length / 2; i >= 0; --i)
                {
                    Adjust(array, i, comparison);
                }
            }
        }

        public static void Adjust<T>(T[] array, int idx, Comparison<T> comparison)
        {
            T val = array[idx];
            int cur = idx;
            int curchild = 2 * idx + 2;

            for (; curchild < array.Length; )
            {
                if (comparison(array[curchild], array[curchild - 1]) < 0)
                    --curchild;


                if (comparison(array[cur], array[curchild]) < 0)
                {
                    Swap(array, cur, curchild);
                    cur = curchild;
                    curchild = curchild * 2 + 2;
                }
                else
                    break;
            }
            if (curchild == array.Length)
            {
                --curchild;

                if (comparison(array[cur], array[curchild]) < 0)
                {
                    Swap(array, cur, curchild);
                    cur = curchild;
                }
            }
        }

        private static void Swap<T>(T[] array, int i1, int i2)
        {
            T tmp = array[i1];
            array[i1] = array[i2];
            array[i2] = tmp;
        }
    }
}
