// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Trinity;
using System.Threading;
using System.Threading.Tasks;
using Trinity.Network;
using Trinity.Core.Lib;
using Trinity.Win32;
using System.Runtime.InteropServices;
using Trinity.Diagnostics;
using Trinity.Network.Client;
using System.Net;
using Trinity.Network.Messaging;
using System.Globalization;

namespace Trinity.Utilities
{
    internal enum TrinitySocketMessage
    {
        //access mode
        ReadTFSFile,
        WriteTFSFile,
        AppendTFSFile,

        Write,
        Read,
        CloseTFSFile,

        HeartBeat
    }

    internal static class DateTimeExtension
    {
        internal static string ToStringForFilename(this DateTime dt)
        {
            return String.Format(CultureInfo.InvariantCulture, "{0:D2}_{1:D2}_{2:D4}_{3:D2}_{4:D2}_{5:D2}_{6}", dt.Month, dt.Day, dt.Year, dt.Hour12(), dt.Minute, dt.Second, dt.IsAM() ? "AM" : "PM");
        }

        internal static int Hour12(this DateTime dt)
        {
            int ret = dt.Hour % 12;
            if (ret == 0)
                ret = 12;
            return ret;
        }

        internal static bool IsAM(this DateTime dt)
        {
            return (dt.Hour < 12);
        }
    }

    /// <summary>
    /// Provides file related utility functions.
    /// </summary>
    public unsafe class FileUtility
    {
        /// <summary>
        /// Gets a random file name without extension.
        /// </summary>
        public static string RandomFileNameWithoutExtension
        {
            get
            {
                return Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            }
        }

        /// <summary>
        /// Complete the given file directory path to make it end with directory separator char.
        /// </summary>
        /// <param name="path">The directory path.</param>
        /// <param name="create_nonexistent">The directory is created if true; otherwise do thing.</param>
        /// <returns>The completed directory path.</returns>
        public static string CompletePath(string path, bool create_nonexistent = true)
        {
          try
            {
                string _path = path;
                if (path[path.Length - 1] != Path.DirectorySeparatorChar)
                    _path = path + Path.DirectorySeparatorChar;
                if (create_nonexistent && (!Directory.Exists(_path)))
                {
                    Directory.CreateDirectory(_path);
                }
                return _path;
            }
            catch(Exception e)
            {
                return e.Message;
            }

        }

        internal static List<string> GetAllFilesRecursively(string folder)
        {
            List<string> files = new List<string>();
            RecursivelyScanFiles(folder, ref files);
            return files;
        }

        private static void RecursivelyScanFiles(string folder, ref List<string> list)
        {
            String[] Files = Directory.GetFileSystemEntries(folder);
            foreach (string fs_entry in Files)
            {
                if (Directory.Exists(fs_entry))
                    RecursivelyScanFiles(fs_entry, ref list);
                else
                    list.Add(fs_entry);
            }
        }
    }
}
