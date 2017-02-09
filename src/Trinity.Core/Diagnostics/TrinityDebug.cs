// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#define DEBUG_ON

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using Trinity.Diagnostics;
using System.Diagnostics;
namespace Trinity.Diagnostics
{
    internal class TrinityDebug
    {
        public static void HexDump(byte[] data)
        {
            Trinity.Diagnostics.HexDump.Dump(data);
        }
        public static void Die(string message = "Exception")
        {
            Log.WriteLine(LogLevel.Fatal, message);
            while (true)
            {
                Thread.Sleep(1000);
            }
        }

        public static void WriteLine(string fmt, params object[] args)
        {
#if DEBUG_ON
            Console.WriteLine(fmt, args);
#endif
        }
        public static void Write(string fmt, params object[] args)
        {
#if DEBUG_ON
            Console.Write(fmt, args);
#endif
        }
        public static void Pause()
        {
            //TODO
        }
        public static void Assert(bool condition, string message)
        {
            if (!condition)
            {
                throw new Exception(message);
            }
        }
    }
}
