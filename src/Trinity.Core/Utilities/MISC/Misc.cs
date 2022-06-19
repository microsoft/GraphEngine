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
    internal class Misc<T>
    {
        public static string ToString(ICollection<T> collection, int indent = 0, string separator = " ")
        {
            string indent_space = "";
            for (int i = 0; i < indent; i++)
            {
                indent_space += " ";
            }
            string ret = "";
            foreach (T t in collection)
            {
                ret += indent_space + t.ToString() + separator;
            }
            if (ret.LastIndexOf(separator, StringComparison.Ordinal) != -1) //make sure the input is not an empty collection
            {
                ret = ret.Substring(0, ret.LastIndexOf(separator, StringComparison.Ordinal)); //remove the last separator
            }
            return ret;
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
