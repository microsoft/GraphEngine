// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;

namespace Trinity.Diagnostics
{
    /// <summary>
    /// A utility class for dumping binary data in hexadecimal format.
    /// </summary>
    public unsafe static class HexDump
    {
        /// <summary>
        /// Dumps a byte array to the standard output in hexadecimal format.
        /// </summary>
        /// <param name="bytes">The byte array from which bytes are read.</param>
        public static void Dump(byte[] bytes)
        {
            int i = 0;
            int row = 0;
            int length = bytes.Length;

            #region Hex Dump
            Console.WriteLine();
            while (i < length)
            {
                if (row % 16 == 0 && i % 16 == 0)
                {
                    Console.Write("        "/*Address*/);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    //Console.WriteLine("  00 01 02 03 04 05 06 07 08 09 0a 0b 0c 0d 0e 0f");
                    Console.Write("  ");
                    for (byte b = 0; b < 16; ++b)
                    {
                        PrintByte(b, b);
                        if (b == 7)
                        {
                            Console.Write("  ");
                        }
                        if (b != 15)
                            Console.Write(" ");
                        else
                            Console.WriteLine();
                    }

                    Console.ResetColor();
                    Console.WriteLine();
                }
                if (i % 16 == 0)
                {
                    Console.Write(i.ToString("X8", CultureInfo.InvariantCulture));
                    Console.Write("  ");
                }

                PrintByte(bytes[i], i % 16);

                //Console.Write(fs.ReadByte().ToString("x2", CultureInfo.InvariantCulture));

                if (i % 16 == 15)
                {
                    Console.ResetColor();
                    Console.WriteLine();
                    row++;
                }
                else
                {
                    if (i % 16 == 7)
                    {
                        Console.Write("  ");
                    }
                    Console.Write(" ");
                }


                i++;
                if (row % 16 == 0 && i % 16 == 0 && row != 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("--More--");
                    Console.WriteLine();
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Q)
                        return;
                }
            }

            Console.WriteLine();
            Console.WriteLine();
            #endregion

            Console.ResetColor();
        }

        /// <summary>
        /// Dumps a file to the standard output in hexadecimal format
        /// </summary>
        /// <param name="fileName">The fully qualified name of the file, or the relative file name.</param>
        internal static void Dump(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open))
            {
                long length = fs.Length;
                int i = 0;
                int row = 0;

                #region Hex Dump
                Console.WriteLine();
                while (i < length)
                {
                    if (row % 16 == 0 && i % 16 == 0)
                    {
                        Console.Write("        "/*Address*/);
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        //Console.WriteLine("  00 01 02 03 04 05 06 07 08 09 0a 0b 0c 0d 0e 0f");
                        Console.Write("  ");
                        for (byte b = 0; b < 16; ++b)
                        {
                            PrintByte(b, b);
                            if (b == 7)
                            {
                                Console.Write("  ");
                            }
                            if (b != 15)
                                Console.Write(" ");
                            else
                                Console.WriteLine();
                        }

                        Console.ResetColor();
                        Console.WriteLine();
                    }
                    if (i % 16 == 0)
                    {
                        Console.Write(i.ToString("X8", CultureInfo.InvariantCulture));
                        Console.Write("  ");
                    }

                    PrintByte((byte)fs.ReadByte(), i % 16);

                    //Console.Write(fs.ReadByte().ToString("x2", CultureInfo.InvariantCulture));

                    if (i % 16 == 15)
                    {
                        Console.ResetColor();
                        Console.WriteLine();
                        row++;
                    }
                    else
                    {
                        if (i % 16 == 7)
                        {
                            Console.Write("  ");
                        }
                        Console.Write(" ");
                    }


                    i++;
                    if (row % 16 == 0 && i % 16 == 0 && row != 0)
                    {
                        Console.WriteLine();
                        Console.WriteLine("--More--");
                        Console.WriteLine();
                        ConsoleKeyInfo key = Console.ReadKey(true);
                        if (key.Key == ConsoleKey.Q)
                            return;
                    }
                }

                Console.WriteLine();
                Console.WriteLine();
                #endregion
            }

            Console.ResetColor();
        }

        /// <summary>
        /// Prints the bits of the specified byte value to the console.
        /// </summary>
        /// <param name="value">The byte value.</param>
        public static void PrintBits(byte value)
        {
            Console.Write(value + "\t");
            for (int i = 7; i >= 0; i--)
            {
                int c = (value >> i) & 1;
                Console.Write(c == 0 ? '0' : '1');
                if (i % 4 == 0)
                    Console.Write(' ');
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Prints the bits of the specified 32-bit signed integer value to the console.
        /// </summary>
        /// <param name="value">The 32-bit integer value.</param>
        public static void PrintBits(int value)
        {
            Console.Write(value + "\t");
            for (int i = 31; i >= 0; i--)
            {
                int c = (value >> i) & 1;
                Console.Write(c == 0 ? '0' : '1');
                if (i % 8 == 0)
                    Console.Write(' ');
                if (i % 4 == 0)
                    Console.Write(' ');
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Print the guid byte by byte.
        /// </summary>
        /// <param name="guid">The specified Guid instance.</param>
        public static void PrintGuid(Guid guid)
        {
            Console.WriteLine(guid);
            byte[] bytes = guid.ToByteArray();
            for (int i = 0; i < bytes.Length; i++)
            {
                Console.Write(bytes[i] + " ");
            }
            Console.WriteLine();
        }

        private static void PrintByte(byte b, int color_index)
        {
            Console.ForegroundColor = (ConsoleColor)(8 + color_index % 8);
            Console.Write(b.ToString("x2", CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Outputs the hexadecimal dump to a string.
        /// </summary>
        /// <param name="bytes">The buffer to dump.</param>
        /// <param name="length">The length of the specified buffer.</param>
        /// <param name="max">Max number of bytes to dump.</param>
        /// <returns>A string that represents the hexadecimal dump.</returns>
        public static string ToString(byte* bytes, int length, int max = int.MaxValue)
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            int row = 0;

            #region Hex Dump
            sb.AppendLine();
            sb.AppendLine();
            while (i < length && i < max)
            {
                if (row % 16 == 0 && i % 16 == 0)
                {
                    //sb.Append("        "/*Address*/);
                    sb.Append("Address ");
                    sb.Append("  ");
                    for (byte b = 0; b < 16; ++b)
                    {
                        sb.Append(b.ToString("x2", CultureInfo.InvariantCulture));
                        if (b == 7)
                        {
                            sb.Append("  ");
                        }
                        if (b != 15)
                            sb.Append(" ");
                        else
                            sb.AppendLine();
                    }

                    sb.AppendLine();
                }
                if (i % 16 == 0)
                {
                    sb.Append(i.ToString("X8", CultureInfo.InvariantCulture));
                    sb.Append("  ");
                }

                sb.Append(bytes[i].ToString("x2", CultureInfo.InvariantCulture));

                if (i % 16 == 15)
                {
                    sb.AppendLine();
                    row++;
                }
                else
                {
                    if (i % 16 == 7)
                    {
                        sb.Append("  ");
                    }
                    sb.Append(" ");
                }

                i++;
                if (row % 16 == 0 && i % 16 == 0 && row != 0)
                {
                    sb.AppendLine();
                    sb.AppendLine();
                }
            }

            sb.AppendLine();
            sb.AppendLine();
            #endregion

            return sb.ToString();
        }
    }
}
