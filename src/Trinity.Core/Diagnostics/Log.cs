// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Trinity;
using Trinity.Utilities;
using Trinity.Core.Lib;
using Trinity.Daemon;

namespace Trinity.Diagnostics
{
    /// <summary>
    /// Specifies a set of available logging levels.
    /// </summary>
    public enum LogLevel : int
    {
        /// <summary>
        /// No message is logged
        /// </summary>
        Off     = 0,

        /// <summary>
        /// Only unrecoverable system errors are logged
        /// </summary>
        Fatal   = 1,

        /// <summary>
        /// Unrecoverable system errors and application logLevel errors are logged
        /// </summary>
        Error   = 2,

        /// <summary>
        /// Fatal system error, application error and application warning are logged
        /// </summary>
        Warning = 3,

        /// <summary>
        /// All errors, warnings and notable application messages are logged
        /// </summary>
        Info    = 4,

        /// <summary>
        /// All errors, warnings, application messages and debugging messages are logged
        /// </summary>
        Debug   = 5,

        /// <summary>
        /// All messages are logged
        /// </summary>
        Verbose = 6,
    }

    /// <summary>
    /// Represents a log entry.
    /// </summary>
    public struct LOG_ENTRY
    {
        /// <summary>
        /// The log message string.
        /// </summary>
        public string   logMessage;
        /// <summary>
        /// The Unix timestamp of the log. 
        /// Note, this is not compatible with DateTime.FromBinary(long).
        /// </summary>
        public long     logTimestamp;
        /// <summary>
        /// The log level.
        /// </summary>
        public LogLevel logLevel;
    }

    /// <summary>
    /// A utility class for logging. 
    /// </summary>
    public unsafe class Log
    {
        #region Fields
        private static IFormatProvider s_InternalFormatProvider;
        private static bool            s_EchoOnConsole = true;
        private const  int             c_LogEntryCollectorIdleInterval = 3000;
        private const  int             c_LogEntryCollectorBusyInterval = 50;
        #endregion

        static Log()
        {
            TrinityC.Ping();
            TrinityConfig.LoadTrinityConfig(false);

            string unitTestAssemblyName = "Microsoft.VisualStudio.QualityTools.UnitTestFramework";
            bool isInUnitTest           = AppDomain.CurrentDomain.GetAssemblies()
                .Any(a => a.FullName.StartsWith(unitTestAssemblyName, StringComparison.Ordinal));

            if (isInUnitTest)
            {
                WriteLine(LogLevel.Info, "UnitTestFramework detected. Enabling echo callback.");
                var LogFilename = Path.Combine(TrinityConfig.LogDirectory, "trinity-log", "trinity-[" + DateTime.Now.ToStringForFilename() + "].log");
                new Thread(_unitTestLogEchoThread).Start(LogFilename);
            }

            BackgroundThread.AddBackgroundTask(new BackgroundTask(CollectLogEntries, c_LogEntryCollectorIdleInterval));
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct LOG_ENTRY_PINVOKE
        {
            public IntPtr   logMessage;
            public long     logTimestamp;
            public LogLevel logLevel;
        }

        private static int CollectLogEntries()
        {
            ulong entry_count;
            IntPtr p_entry;
            TrinityErrorCode eResult = CLogCollectEntries(out entry_count, out p_entry);
            if (TrinityErrorCode.E_SUCCESS == eResult)
            {
                Debug.Assert(entry_count < Int32.MaxValue, "CollectLogEntries: too many log entries");
                LOG_ENTRY_PINVOKE *p       = (LOG_ENTRY_PINVOKE*)p_entry;
                LOG_ENTRY_PINVOKE *pend    = p + entry_count;
                List<LOG_ENTRY>    entries = new List<LOG_ENTRY>((int)entry_count);

                while (p != pend)
                {
                    entries.Add(new LOG_ENTRY
                    {
                        logLevel = p->logLevel,
                        logMessage = new string((char*)p->logMessage),
                        logTimestamp = p->logTimestamp
                    });
                    Memory.free(p->logMessage.ToPointer());
                    ++p;
                }
                Memory.free(p_entry.ToPointer());

                LogsWritten(entries);
            }
            return (TrinityErrorCode.E_SUCCESS == eResult) ? 
                c_LogEntryCollectorBusyInterval : 
                c_LogEntryCollectorIdleInterval;
        }

        private static void _unitTestLogEchoThread(object param)
        {
            string filename = (string)param;

            try
            {
                var stream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                var reader = new StreamReader(stream);
                while (true)
                {
                    try
                    {
                        var line = (reader.ReadLine()).Trim();
                        if (line != "")
                            Console.WriteLine(line);
                        else
                            Thread.Sleep(10);
                    }
                    catch { Thread.Sleep(500); }
                }
            }
            catch
            { Console.WriteLine("Failed to open log file {0}", filename); }
        }

        /// <summary>
        /// The event fires when new log entries are written.
        /// </summary>
        public static event Action<IList<LOG_ENTRY>> LogsWritten = delegate { };

        /// <summary>
        /// Flushes the log content to the disk immediately. Note that for
        /// messages with LogLevel equal to LogLevel.Info or higher, the log
        /// will be automatically flushed immediately. Lower priority logs
        /// (LogLevel.Debug and LogLevel.Verbose) will be flushed periodically.
        /// </summary>
        public static void Flush()
        {
            CLogFlush();
        }

        /// <summary>
        /// Gets of sets a value indicating whether the logged messages are echoed to the Console.
        /// </summary>
        public static bool EchoOnConsole
        {
            get { return s_EchoOnConsole; }
            set { s_EchoOnConsole = value; TrinityConfig.CLogSetEchoOnConsole(value); }
        }

        /// <summary>
        /// Writes the text representation of the specified array of objects, followed by the current line terminator, to the log using the specified format information.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="arg">An array of objects to write using format.</param>
        public static void WriteLine(string format = "", params object[] arg)
        {
            WriteLine(LogLevel.Info, format, arg);
        }

        /// <summary>
        /// Writes the text representation of the specified array of objects, followed by the current line terminator, to the log using the specified format information at the specified logging level.
        /// </summary>
        /// <param name="logLevel">The logging level at which the message is written.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="arg">An array of objects to write using format.</param>
        public static unsafe void WriteLine(LogLevel logLevel, string format = "", params object[] arg)
        {
            CLogWriteLine((int)logLevel, string.Format(FormatProvider, format, arg));
        }

        private static IFormatProvider FormatProvider
        {
            get
            {
                if (s_InternalFormatProvider == null)
                {
                    // XXX use CurrentCulture or InvariantCulture for logging?
                    s_InternalFormatProvider = Thread.CurrentThread.CurrentCulture;
                }
                return s_InternalFormatProvider;
            }
        }


        [DllImport(TrinityC.AssemblyName, CharSet = CharSet.Unicode)]
        private static extern unsafe void CLogWriteLine(int level, string p_buf);

        [DllImport(TrinityC.AssemblyName)]
        private static extern void CLogFlush();

        [DllImport(TrinityC.AssemblyName, CharSet = CharSet.Unicode)]
        private static extern TrinityErrorCode CLogCollectEntries(
            [Out] out ulong arr_size,
            [Out] out IntPtr entries);
    }
}
