// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace Trinity.Utilities
{
    internal class TrinityEnvironment
    {
        internal static string ResolveTrinityHome()
        {
            string assembly_path = AssemblyUtility.MyAssemblyPath;
            string trinity_home = Environment.GetEnvironmentVariable("TrinityHome");
            string trinity_path = Environment.GetEnvironmentVariable("TrinityPath");
            string trinity_flag_file = assembly_path + "trinity";
            if (trinity_home == null)
            {
                if (trinity_path != null)
                {
                    trinity_home = Path.GetFullPath(FileUtility.CompletePath(trinity_path, false) + "..\\");
                }
                else
                {
                    if (File.Exists(trinity_flag_file))
                    {
                        trinity_home = FileUtility.CompletePath(File.ReadAllText(trinity_flag_file).Trim(),false);
                    }
                }
            }

            if (trinity_home == null)
            {
                return null;
            }
            else
            {
                return FileUtility.CompletePath(trinity_home, false);
            }
        }

        internal static string ResolveTrinityPath()
        {
            string assembly_path = AssemblyUtility.TrinityCorePath;
            string trinity_home = Environment.GetEnvironmentVariable("TrinityHome");
            string trinity_path = Environment.GetEnvironmentVariable("TrinityPath");
            string trinity_flag_file = Path.Combine(assembly_path, "trinity");
            if (trinity_path == null)
            {
                if (trinity_home != null)
                {
                    trinity_path = FileUtility.CompletePath(trinity_home, false) + "bin\\";
                }
                else
                {
                    if (File.Exists(trinity_flag_file))
                    {
                        trinity_path = FileUtility.CompletePath(File.ReadAllText(trinity_flag_file).Trim(), false) + "bin\\";
                    }
                }
            }

            if (trinity_path == null)
            {
                return null;
            }
            else
            {
                return FileUtility.CompletePath(trinity_path, false);
            }
        }
    }
}
