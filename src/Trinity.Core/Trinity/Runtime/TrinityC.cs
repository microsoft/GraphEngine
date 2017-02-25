// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Trinity.Properties;
using Trinity.Utilities;

namespace Trinity
{
    internal static class TrinityC
    {
#if CORECLR
        internal const string AssemblyName = "Trinity";
#else
        internal const string AssemblyName = "Trinity.C.dll";
#endif
        static TrinityC()
        {
            string assembly_path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
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
#if !CORECLR
            string trinity_c_md5 = Resources.Trinity_C_sha512.Trim().ToLowerInvariant();

            bool found = false;
            if (File.Exists(trinity_c_path))
            {
                string md5 = SecureHashHelper.GetFileSHA512(trinity_c_path).Trim().ToLowerInvariant();
                if (trinity_c_md5.Equals(md5))
                {
                    found = true;
                }
            }

            if (found)
            {
                return;
            }
#endif
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream resourceStream = assembly.GetManifestResourceStream("Trinity." + native_assembly_name);
            try
            {
                using (FileStream fs = new FileStream(trinity_c_path, FileMode.Create))
                {
                    resourceStream.CopyTo(fs);
                    fs.Flush();
                }
                if (resourceStream != null)
                    resourceStream.Dispose();
            }
            catch (Exception)
            {
                Console.WriteLine("Trinity.C cannot be released, please make sure the working directory is writable.");
                if(Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    Win32.NativeAPI.timeEndPeriod(1);
                }
                Environment.Exit(0);
            }
        }

        internal static void Ping()
        {

        }
    }
}
