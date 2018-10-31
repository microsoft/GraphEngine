// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;

namespace Trinity.Utilities
{
    internal class Misc<T>
    {
        public static string ToString(ICollection<T> collection, int indent = 0, string separator = " ")
        {
            var indentSpace = new string(' ', indent);
            return string.Join(separator, collection.Select(t => indentSpace + t.ToString()));
        }
    }

    internal class Misc
    {
        public static void DisplayByteArray(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; )
            {
                for (int j = 0; j < 8; j++)
                {
                    if (i < bytes.Length)
                        Console.Write("{0}\t", bytes[i++]);
                }
                Console.WriteLine();
            }
        }
    }
}
