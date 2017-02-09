// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.TSL.Lib
{
    /// <summary>
    /// Provides escaping/unescaping methods for json strings.
    /// </summary>
    public static class JsonStringProcessor
    {
        static Dictionary<char, ushort> hex_map = new Dictionary<char, ushort>()
        {
            {'0', 0},
            {'1', 1},
            {'2', 2},
            {'3', 3},
            {'4', 4},
            {'5', 5},
            {'6', 6},
            {'7', 7},
            {'8', 8},
            {'9', 9},
            {'A', 10},
            {'B', 11},
            {'C', 12},
            {'D', 13},
            {'E', 14},
            {'F', 15},
            {'a', 10},
            {'b', 11},
            {'c', 12},
            {'d', 13},
            {'e', 14},
            {'f', 15},
        };

        [ThreadStatic]
        static StringBuilder s_stringBuilder = null;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void ensure_string_builder()
        {
            if (s_stringBuilder == null)
                s_stringBuilder = new StringBuilder();
            s_stringBuilder.Clear();
        }

        /// <summary>
        /// Unescapes a json value string.
        /// </summary>
        /// <param name="json">The escaped value.</param>
        /// <returns>The unescaped value.</returns>
        public static string unescape(string json)
        {
            if (json == null || json.Length == 0)
                return "";

            ensure_string_builder();

            bool    need_unescape  = false;
            int     utf_sequence   = 0;
            ushort  utf_value      = 0;
            char    quote_char     = '\0';
            int     begin          = 0;

            switch (json[0])
            {
                case '\'':
                    quote_char = '\'';
                    break;
                case '"':
                    quote_char = '"';
                    break;
            }

            if(quote_char != '\0')
            {
                begin = 1;
            }

            for (int idx = begin, end = json.Length; idx < end; ++idx)
            {
                char ch  = json[idx];

                if (utf_sequence > 0)
                {
                    ushort char_val;
                    if (!hex_map.TryGetValue(ch, out char_val))
                        throw new ArgumentOutOfRangeException("Invalid unicode escape sequence.");
                    utf_value = (ushort)((utf_value << 4) | (ushort)ch);
                    --utf_sequence;
                    if (utf_sequence == 0)
                        s_stringBuilder.Append((char)utf_value);
                }
                else if (need_unescape)
                {
                    switch (ch)
                    {
                        case 'b': s_stringBuilder.Append('\b'); break; // Backspace
                        case 't': s_stringBuilder.Append('\t'); break; // Horizontal tab
                        case 'n': s_stringBuilder.Append('\n'); break; // Newline
                        case 'f': s_stringBuilder.Append('\f'); break; // Form feed
                        case 'r': s_stringBuilder.Append('\r'); break; // Carriage return 

                        case 'u':
                            {
                                utf_sequence = 4;
                                utf_value    = 0;
                                break;
                            }

                        default:
                            s_stringBuilder.Append(ch);
                            break;
                    }
                    need_unescape = false;
                }
                else if (ch == '\\')
                {
                    need_unescape = true;
                    continue;
                }
                else
                {
                    if (ch == quote_char)
                        break;
                    s_stringBuilder.Append(ch);
                }
            }

            return s_stringBuilder.ToString();
        }

        /// <summary>
        /// Escapes a json value string.
        /// </summary>
        /// <param name="raw">The unescaped raw value string.</param>
        /// <returns>The escaped string.</returns>
        public static string escape(string raw)
        {
            if (raw == null || raw.Length == 0)
                return "\"\"";
            ensure_string_builder();
            s_stringBuilder.Append('"');

            char h_ch = '\0';

            foreach (var ch in raw)
            {
                switch (ch)
                {
                    case '\\':
                    case '"':
                        s_stringBuilder.Append('\\');
                        s_stringBuilder.Append(ch);
                        break;
                    case '/':
                        if (h_ch == '<')
                            s_stringBuilder.Append('\\');
                        s_stringBuilder.Append(ch);
                        break;
                    case '\b': s_stringBuilder.Append("\\b"); break;
                    case '\t': s_stringBuilder.Append("\\t"); break;
                    case '\n': s_stringBuilder.Append("\\n"); break;
                    case '\f': s_stringBuilder.Append("\\f"); break;
                    case '\r': s_stringBuilder.Append("\\r"); break;
                    default:
                        if (ch < ' ')
                        {
                            s_stringBuilder.Append("\\u");
                            s_stringBuilder.Append(((int)ch).ToString("x4", CultureInfo.InvariantCulture));
                        }
                        else
                        {
                            s_stringBuilder.Append(ch);
                        }
                        break;
                }
                h_ch = ch;
            }

            s_stringBuilder.Append('"');
            return s_stringBuilder.ToString();
        }

    }
}
