// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.Utilities
{
    /// <summary>
    /// Provides a fast lookup table for a small set of strings.
    /// This utility is mainly used by TSL.
    /// </summary>
    public class StringLookupTable
    {
        string[] m_strings;
        int[]    m_hashes;
        int      m_len;

        /// <summary>
        /// Construct a string to int lookup table with the given strings.
        /// </summary>
        /// <param name="strings">The strings to be placed in the lookup table.</param>
        public StringLookupTable(params string[] strings)
        {
            m_len     = strings.Length;
            m_strings = new string[m_len];
            m_hashes  = new int[m_len];

            Array.Copy(strings, m_strings, m_len);
            for (int i=0; i<m_len; ++i)
            {
                m_hashes[i] = m_strings[i].GetHashCode();
            }
        }

        /// <summary>
        /// Perform a lookup.
        /// </summary>
        /// <param name="input">The input string to look up.</param>
        /// <returns>If the input string exists in the lookup table, return its index. Otherwise return -1.</returns>
        public int Lookup(string input)
        {
            int hash = input.GetHashCode();
            for (int i=0; i<m_len; ++i)
            {
                if (hash == m_hashes[i] && input == m_strings[i])
                    return i;
            }

            return -1;
        }
    }
}
