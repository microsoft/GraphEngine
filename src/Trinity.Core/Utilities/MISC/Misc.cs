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
            string indent_space = new string(' ', indent);
            return string.Join(separator, collection.Select(c => indent_space + c));
        }
    }

    internal class Misc
    {
        public static void DisplayByteArray(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                sb.Append(bytes[i]);
                sb.Append('\t');

                // 8 bytes per line
                if ((i + 1) % 8 == 0 || i == bytes.Length - 1)
                {
                    sb.AppendLine();
                }
            }
            Console.Write(sb.ToString());
        }
    }
}
