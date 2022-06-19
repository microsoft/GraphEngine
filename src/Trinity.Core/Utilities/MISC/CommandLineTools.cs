// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Trinity.Utilities
{
    internal class OptionDescriptor<T>
    {
        public string LongOption;
        public string ShortOption;
        public T value;
        public bool set = false;
    }

    internal class CommandLineTools
    {
        internal static char[] WhiteSpaceArray = new char[] { ' ', '\t' };
        public static string CmdBase(string command)
        {
            return command.Trim().Split(WhiteSpaceArray).First();
        }
        public static string[] CmdSplit(string command)
        {
            int translatedQuoteIndex = command.IndexOf("\\\"", StringComparison.Ordinal);
            if (translatedQuoteIndex >= 0)
            {
                List<string> list = CmdSplit(command.Substring(0, translatedQuoteIndex)).ToList();
                int nextTQI = command.IndexOf("\\\"", translatedQuoteIndex + 2, StringComparison.Ordinal);
                if (nextTQI < 0)
                    nextTQI = command.Length;
                list.Add("\"" + command.Substring(translatedQuoteIndex + 2, nextTQI - translatedQuoteIndex - 2) + "\"");
                if (nextTQI + 2 < command.Length)
                {
                    list.AddRange(CmdSplit(command.Substring(nextTQI + 2)));
                }
                return list.ToArray();
            }
            if (command.Contains('"'))
            {
                int index = command.IndexOf('"');
                if (index == command.Length - 1)
                {
                    return CmdSplit(command.Substring(0, command.Length - 1));
                }
                int nextIndex = command.IndexOf('"', index + 1);
                List<string> list = CmdSplit(command.Substring(0, index)).ToList();
                if (nextIndex < 0)
                {
                    list.Add(command.Substring(index + 1));
                }
                else
                {
                    list.Add(command.Substring(index + 1, nextIndex - index - 1));
                    list.AddRange(CmdSplit(command.Substring(nextIndex + 1)));
                }
                return list.ToArray();
            }
            else
            {
                return command.Trim().Split(WhiteSpaceArray, StringSplitOptions.RemoveEmptyEntries);
            }
        }
        #region GetOpt
        public static OptionDescriptor<T> DefineOption<T>(string longOpt, string shortOpt)
        {
            return new OptionDescriptor<T>
            {
                LongOption = longOpt,
                ShortOption = shortOpt
            };
        }

        private static int Index<T>(string[] command, OptionDescriptor<T> descriptor)
        {
            for (int i = 0, length = command.Length; i < length; ++i)
            {
                if (command[i].StartsWith("--", StringComparison.Ordinal))
                {
                    if (command[i].Substring(2) == descriptor.LongOption)
                        return i;
                }
                else if (command[i].StartsWith("-", StringComparison.Ordinal))
                {
                    if (command[i].Substring(1) == descriptor.ShortOption)
                        return i;
                }
            }
            return -1;
        }
        public static void GetOpt(ref string[] command, OptionDescriptor<bool> descriptor)
        {
            int idx = Index(command, descriptor);
            descriptor.value = false;
            if (idx != -1)
            {
                descriptor.value = true;
                descriptor.set = true;
                List<string> list = command.ToList();
                list.RemoveAt(idx);
                command = list.ToArray();
            }
        }
        public static void GetOpt(ref string[] command, OptionDescriptor<char> descriptor)
        {
            int idx = Index(command, descriptor);
            if (idx != -1 && idx < command.Length - 1)
            {
                descriptor.set = char.TryParse(command[idx + 1], out descriptor.value);
                if (descriptor.set)
                {
                    List<string> list = command.ToList();
                    list.RemoveAt(idx);
                    list.RemoveAt(idx);
                    command = list.ToArray();
                }
            }
        }
        public static void GetOpt(ref string[] command, OptionDescriptor<byte> descriptor)
        {
            int idx = Index(command, descriptor);
            if (idx != -1 && idx < command.Length - 1)
            {
                descriptor.set = byte.TryParse(command[idx + 1], out descriptor.value);
                if (descriptor.set)
                {
                    List<string> list = command.ToList();
                    list.RemoveAt(idx);
                    list.RemoveAt(idx);
                    command = list.ToArray();
                }
            }
        }
        public static void GetOpt(ref string[] command, OptionDescriptor<short> descriptor)
        {
            int idx = Index(command, descriptor);
            if (idx != -1 && idx < command.Length - 1)
            {
                descriptor.set = short.TryParse(command[idx + 1], out descriptor.value);
                if (descriptor.set)
                {
                    List<string> list = command.ToList();
                    list.RemoveAt(idx);
                    list.RemoveAt(idx);
                    command = list.ToArray();
                }
            }
        }
        public static void GetOpt(ref string[] command, OptionDescriptor<int> descriptor)
        {
            int idx = Index(command, descriptor);
            if (idx != -1 && idx < command.Length - 1)
            {
                descriptor.set = int.TryParse(command[idx + 1], out descriptor.value);
                if (descriptor.set)
                {
                    List<string> list = command.ToList();
                    list.RemoveAt(idx);
                    list.RemoveAt(idx);
                    command = list.ToArray();
                }
            }
        }
        public static void GetOpt(ref string[] command, OptionDescriptor<long> descriptor)
        {
            int idx = Index(command, descriptor);
            if (idx != -1 && idx < command.Length - 1)
            {
                descriptor.set = long.TryParse(command[idx + 1], out descriptor.value);
                if (descriptor.set)
                {
                    List<string> list = command.ToList();
                    list.RemoveAt(idx);
                    list.RemoveAt(idx);
                    command = list.ToArray();
                }
            }
        }
        public static void GetOpt(ref string[] command, OptionDescriptor<float> descriptor)
        {
            int idx = Index(command, descriptor);
            if (idx != -1 && idx < command.Length - 1)
            {
                descriptor.set = float.TryParse(command[idx + 1], out descriptor.value);
                if (descriptor.set)
                {
                    List<string> list = command.ToList();
                    list.RemoveAt(idx);
                    list.RemoveAt(idx);
                    command = list.ToArray();
                }
            }
        }
        public static void GetOpt(ref string[] command, OptionDescriptor<double> descriptor)
        {
            int idx = Index(command, descriptor);
            if (idx != -1 && idx < command.Length - 1)
            {
                descriptor.set = double.TryParse(command[idx + 1], out descriptor.value);
                if (descriptor.set)
                {
                    List<string> list = command.ToList();
                    list.RemoveAt(idx);
                    list.RemoveAt(idx);
                    command = list.ToArray();
                }
            }
        }
        public static void GetOpt(ref string[] command, OptionDescriptor<string> descriptor)
        {
            int idx = Index(command, descriptor);
            if (idx != -1 && idx < command.Length - 1)
            {
                descriptor.set = true;
                descriptor.value = command[idx + 1];
                if (descriptor.set)
                {
                    List<string> list = command.ToList();
                    list.RemoveAt(idx);
                    list.RemoveAt(idx);
                    command = list.ToArray();
                }
            }
        }
        public static void GetOpt(ref string[] command, OptionDescriptor<sbyte> descriptor)
        {
            int idx = Index(command, descriptor);
            if (idx != -1 && idx < command.Length - 1)
            {
                descriptor.set = sbyte.TryParse(command[idx + 1], out descriptor.value);
                if (descriptor.set)
                {
                    List<string> list = command.ToList();
                    list.RemoveAt(idx);
                    list.RemoveAt(idx);
                    command = list.ToArray();
                }
            }
        }
        public static void GetOpt(ref string[] command, OptionDescriptor<ushort> descriptor)
        {
            int idx = Index(command, descriptor);
            if (idx != -1 && idx < command.Length - 1)
            {
                descriptor.set = ushort.TryParse(command[idx + 1], out descriptor.value);
                if (descriptor.set)
                {
                    List<string> list = command.ToList();
                    list.RemoveAt(idx);
                    list.RemoveAt(idx);
                    command = list.ToArray();
                }
            }
        }
        public static void GetOpt(ref string[] command, OptionDescriptor<uint> descriptor)
        {
            int idx = Index(command, descriptor);
            if (idx != -1 && idx < command.Length - 1)
            {
                descriptor.set = uint.TryParse(command[idx + 1], out descriptor.value);
                if (descriptor.set)
                {
                    List<string> list = command.ToList();
                    list.RemoveAt(idx);
                    list.RemoveAt(idx);
                    command = list.ToArray();
                }
            }
        }
        public static void GetOpt(ref string[] command, OptionDescriptor<ulong> descriptor)
        {
            int idx = Index(command, descriptor);
            if (idx != -1 && idx < command.Length - 1)
            {
                descriptor.set = ulong.TryParse(command[idx + 1], out descriptor.value);
                if (descriptor.set)
                {
                    List<string> list = command.ToList();
                    list.RemoveAt(idx);
                    list.RemoveAt(idx);
                    command = list.ToArray();
                }
            }
        }
        public static void GetOpt<T>(ref string[] command, OptionDescriptor<T> descriptor)
        {
            //fix: GetOpt will wipe out all quotes with CmdSplit. make them \" first.
            //for ( int i = 0; i < command.Length; ++i )
            //{
            //    command[i] = command[i].Replace( "\"", "\\\"" );
            //}
            //command = CommandLineTools.CmdSplit(string.Join(" ", command));
            if (typeof(T) == typeof(bool))
            {
                GetOpt(ref command, descriptor as OptionDescriptor<bool>); return;
            }
            if (typeof(T) == typeof(byte))
            {
                GetOpt(ref command, descriptor as OptionDescriptor<byte>); return;
            }
            if (typeof(T) == typeof(char))
            {
                GetOpt(ref command, descriptor as OptionDescriptor<char>); return;
            }
            if (typeof(T) == typeof(short))
            {
                GetOpt(ref command, descriptor as OptionDescriptor<short>); return;
            }
            if (typeof(T) == typeof(int))
            {
                GetOpt(ref command, descriptor as OptionDescriptor<int>); return;
            }
            if (typeof(T) == typeof(long))
            {
                GetOpt(ref command, descriptor as OptionDescriptor<long>); return;
            }
            if (typeof(T) == typeof(float))
            {
                GetOpt(ref command, descriptor as OptionDescriptor<float>); return;
            }
            if (typeof(T) == typeof(double))
            {
                GetOpt(ref command, descriptor as OptionDescriptor<double>); return;
            }
            if (typeof(T) == typeof(string))
            {
                GetOpt(ref command, descriptor as OptionDescriptor<string>); return;
            }
            if (typeof(T) == typeof(sbyte))
            {
                GetOpt(ref command, descriptor as OptionDescriptor<sbyte>); return;
            }
            if (typeof(T) == typeof(ushort))
            {
                GetOpt(ref command, descriptor as OptionDescriptor<ushort>); return;
            }
            if (typeof(T) == typeof(uint))
            {
                GetOpt(ref command, descriptor as OptionDescriptor<uint>); return;
            }
            if (typeof(T) == typeof(ulong))
            {
                GetOpt(ref command, descriptor as OptionDescriptor<ulong>); return;
            }
            throw new NotImplementedException("Type " + default(T).GetType() + " is not supported!");
        }
        public static void GetOpt<T1, T2>(ref string[] command, OptionDescriptor<T1> d1, OptionDescriptor<T2> d2)
        {
            GetOpt(ref command, d1);
            GetOpt(ref command, d2);
        }
        public static void GetOpt<T1, T2, T3>(ref string[] command, OptionDescriptor<T1> d1, OptionDescriptor<T2> d2, OptionDescriptor<T3> d3)
        {
            GetOpt(ref command, d1);
            GetOpt(ref command, d2);
            GetOpt(ref command, d3);
        }
        public static void GetOpt<T1, T2, T3, T4>(ref string[] command, OptionDescriptor<T1> d1, OptionDescriptor<T2> d2, OptionDescriptor<T3> d3, OptionDescriptor<T4> d4)
        {
            GetOpt(ref command, d1);
            GetOpt(ref command, d2);
            GetOpt(ref command, d3);
            GetOpt(ref command, d4);
        }
        public static void GetOpt<T1, T2, T3, T4, T5>(ref string[] command, OptionDescriptor<T1> d1, OptionDescriptor<T2> d2, OptionDescriptor<T3> d3, OptionDescriptor<T4> d4, OptionDescriptor<T5> d5)
        {
            GetOpt(ref command, d1);
            GetOpt(ref command, d2);
            GetOpt(ref command, d3);
            GetOpt(ref command, d4);
            GetOpt(ref command, d5);
        }
        public static void GetOpt<T1, T2, T3, T4, T5, T6>(ref string[] command, OptionDescriptor<T1> d1, OptionDescriptor<T2> d2, OptionDescriptor<T3> d3, OptionDescriptor<T4> d4, OptionDescriptor<T5> d5, OptionDescriptor<T6> d6)
        {
            GetOpt(ref command, d1);
            GetOpt(ref command, d2);
            GetOpt(ref command, d3);
            GetOpt(ref command, d4);
            GetOpt(ref command, d5);
            GetOpt(ref command, d6);
        }
        public static void GetOpt<T1, T2, T3, T4, T5, T6, T7>(ref string[] command, OptionDescriptor<T1> d1, OptionDescriptor<T2> d2, OptionDescriptor<T3> d3, OptionDescriptor<T4> d4, OptionDescriptor<T5> d5, OptionDescriptor<T6> d6, OptionDescriptor<T7> d7)
        {
            GetOpt(ref command, d1);
            GetOpt(ref command, d2);
            GetOpt(ref command, d3);
            GetOpt(ref command, d4);
            GetOpt(ref command, d5);
            GetOpt(ref command, d6);
            GetOpt(ref command, d7);
        }
        public static void GetOpt<T1, T2, T3, T4, T5, T6, T7, T8>(ref string[] command, OptionDescriptor<T1> d1, OptionDescriptor<T2> d2, OptionDescriptor<T3> d3, OptionDescriptor<T4> d4, OptionDescriptor<T5> d5, OptionDescriptor<T6> d6, OptionDescriptor<T7> d7, OptionDescriptor<T8> d8)
        {
            GetOpt(ref command, d1);
            GetOpt(ref command, d2);
            GetOpt(ref command, d3);
            GetOpt(ref command, d4);
            GetOpt(ref command, d5);
            GetOpt(ref command, d6);
            GetOpt(ref command, d7);
            GetOpt(ref command, d8);
        }
        public static void GetOpt<T1, T2, T3, T4, T5, T6, T7, T8, T9>(ref string[] command, OptionDescriptor<T1> d1, OptionDescriptor<T2> d2, OptionDescriptor<T3> d3, OptionDescriptor<T4> d4, OptionDescriptor<T5> d5, OptionDescriptor<T6> d6, OptionDescriptor<T7> d7, OptionDescriptor<T8> d8, OptionDescriptor<T9> d9)
        {
            GetOpt(ref command, d1);
            GetOpt(ref command, d2);
            GetOpt(ref command, d3);
            GetOpt(ref command, d4);
            GetOpt(ref command, d5);
            GetOpt(ref command, d6);
            GetOpt(ref command, d7);
            GetOpt(ref command, d8);
            GetOpt(ref command, d9);
        }
        public static void GetOpt<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(ref string[] command, OptionDescriptor<T1> d1, OptionDescriptor<T2> d2, OptionDescriptor<T3> d3, OptionDescriptor<T4> d4, OptionDescriptor<T5> d5, OptionDescriptor<T6> d6, OptionDescriptor<T7> d7, OptionDescriptor<T8> d8, OptionDescriptor<T9> d9, OptionDescriptor<T10> d10)
        {
            GetOpt(ref command, d1);
            GetOpt(ref command, d2);
            GetOpt(ref command, d3);
            GetOpt(ref command, d4);
            GetOpt(ref command, d5);
            GetOpt(ref command, d6);
            GetOpt(ref command, d7);
            GetOpt(ref command, d8);
            GetOpt(ref command, d9);
            GetOpt(ref command, d10);
        }
        #endregion
        /// <summary>
        /// Parse an long integer in the given string.
        /// Valid formats are:
        ///     1. Plain number.
        ///     2. nnnM, nnnB, nnnG (here B means Billion, not 'Byte' or 'Bit'!
        ///     3. Float point number+Unit.
        ///         * For example, 0.5B means 0.5*1024*1024*1024
        ///     4. Well, you do can use sufficies like "KB", "MB", "GB"..
        /// </summary>
        /// <param name="s">The given string.</param>
        /// <returns>The number parsed. If no valid number is found, null will be returned.</returns>
        public static long? ParseNumber(string s)
        {
            s = s.ToLowerInvariant();
            List<KeyValuePair<char, long>> UnitTable = new List<KeyValuePair<char, long>>
            {
                new KeyValuePair<char,long>('k',1L<<10),
                new KeyValuePair<char,long>('m',1L<<20),
                new KeyValuePair<char,long>('g',1L<<30),
                new KeyValuePair<char,long>('b',1L<<30),
                new KeyValuePair<char,long>('t',1L<<40),
                new KeyValuePair<char,long>('p',1L<<50)
            };
            long Unit = 1;
            foreach (var p in UnitTable)
            {
                // What do you think is 100BB??
                if (s.EndsWith(p.Key + "b", StringComparison.Ordinal) && p.Key != 'b')
                {
                    Unit = p.Value;
                    s = s.Substring(0, s.Length - 2);
                    break;
                }
                if (s.EndsWith(p.Key.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal))
                {
                    Unit = p.Value;
                    s = s.Substring(0, s.Length - 1);
                    break;
                }
            }
            if (s.Contains('.'))
            {
                double dValue;
                if (double.TryParse(s, out dValue))
                {
                    return (long)(Unit * dValue);
                }
                else return null;
            }
            long lValue;
            if (long.TryParse(s, out lValue))
            {
                return lValue * Unit;
            }
            return null;
        }
    }
}
