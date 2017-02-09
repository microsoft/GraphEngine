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
    internal class CellTypeDictionary
    {
        private List<string> list_A;
        private List<byte> list_B;

        public CellTypeDictionary()
        {
            list_A = new List<string>();
            list_B = new List<byte>();
        }

        public byte GetValue(string key)
        {
            for (int i = 0; i < list_A.Count; i++)
            {
                if (list_A[i] == key)
                {
                    return list_B[i];
                }
            }
            return 0; //undefined
        }

        public string GetKey(byte value)
        {
            for (int i = 0; i < list_B.Count; i++)
            {
                if (list_B[i] == value)
                {
                    return list_A[i];
                }
            }
            return "undefined"; //undefined
        }
    }
}
