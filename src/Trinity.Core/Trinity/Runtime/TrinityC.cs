// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Trinity.Utilities;

namespace Trinity
{
    internal static partial class TrinityC
    {
#if CORECLR
        internal const string AssemblyName = "Trinity";
#else
        internal const string AssemblyName = "Trinity.C.dll";
#endif
        private static object s_initlock = new object();
        private static bool   s_initialized = false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal unsafe static void Init()
        {
            lock (s_initlock)
            {
                if (s_initialized) return;

                string assembly_path = AssemblyUtility.TrinityCorePath;
#if !CORECLR
                string native_assembly_name = "Trinity.C.dll";
#else
                string native_assembly_name = null;
                switch (Environment.OSVersion.Platform)
                {
                    case PlatformID.Win32NT:
                        native_assembly_name = "Trinity.dll";
                        break;
                    case PlatformID.Unix:
                        native_assembly_name = "libTrinity.so";
                        break;
                    default: throw new NotImplementedException();
                }
#endif
                string trinity_c_path = Path.Combine(assembly_path, native_assembly_name);
                bool found = false;

                if (File.Exists(trinity_c_path))
                {
                    using (Stream resourceStream = GetTrinityCAssembly(native_assembly_name))
                    {
                        string embedded_sha = SecureHashHelper.GetSHA512(resourceStream).Trim().ToLowerInvariant();
                        string ondisk_sha = SecureHashHelper.GetFileSHA512(trinity_c_path).Trim().ToLowerInvariant();
                        if (embedded_sha.Equals(ondisk_sha))
                        {
                            found = true;
                        }
                    }
                }

                if (!found)
                {
                    ReleaseNativeAssembly(native_assembly_name, trinity_c_path);
                }

                /* native assembly is released. initialize Trinity.C now */
                fixed(char* pAssemblyPath = AssemblyUtility.MyAssemblyPath)
                {
                    __INIT_TRINITY_C__(pAssemblyPath);
                }

                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    Win32.NativeAPI.timeBeginPeriod(1);
#if !CORECLR
                    Register();
#endif
                }

                s_initialized = true;
            }
        }

        private static void ReleaseNativeAssembly(string native_assembly_name, string trinity_c_path)
        {
            try
            {
                using (Stream resourceStream = GetTrinityCAssembly(native_assembly_name))
                using (FileStream fs = new FileStream(trinity_c_path, FileMode.Create))
                {
                    resourceStream.CopyTo(fs);
                    fs.Flush();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Trinity.C cannot be released, please make sure the working directory is writable.");
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    Win32.NativeAPI.timeEndPeriod(1);
                }
                Environment.Exit(0);
            }
        }

        private static Stream GetTrinityCAssembly(string resource_name)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            return assembly.GetManifestResourceStream("Trinity." + resource_name);
        }

        [DllImport(TrinityC.AssemblyName)]
        private static extern unsafe void __INIT_TRINITY_C__(char* pAssemblyPath);
    }
}
