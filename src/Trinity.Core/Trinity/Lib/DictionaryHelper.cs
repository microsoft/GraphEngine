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
    internal class DictionaryHelper
    {
        public static int GetHashCode(long key)
        {
            return (((int)key) ^ ((int)(key >> 0x20))) & 0x7FFFFFFF;
        }
        public static readonly long InvalidKey = -1;
    }
}
