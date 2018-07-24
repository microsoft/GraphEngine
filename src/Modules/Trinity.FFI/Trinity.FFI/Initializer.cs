using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Trinity.Diagnostics;

namespace Trinity.FFI
{
    public static class Initializer
    {
        public static TrinityErrorCode Initialize(string config_path, string storage_root)
        {
            try
            {
                TrinityConfig.LoadConfig(config_path);
                TrinityConfig.StorageRoot = storage_root;
                Global.Initialize();
                AppDomain.CurrentDomain.AssemblyResolve += OnMissingAssemblyResolve;
                AppDomain.CurrentDomain.UnhandledException += OnUnresolvedException;

                return TrinityErrorCode.E_SUCCESS;
            }
            catch
            {
                return TrinityErrorCode.E_FAILURE;
            }
        }

        private static IEnumerable<string> DumpException(Exception ex)
        {
            yield return ex.Message;
            switch (ex)
            {
                case FileNotFoundException fnfex:
                    yield return $"FileName: {fnfex.FileName}";
                    break;
            }

            if (ex.InnerException != null)
            {
                foreach (var str in DumpException(ex.InnerException).Select(_ => $">>  {_}"))
                {
                    yield return str;
                }
            }

            yield return $"StackTrace:";
            foreach(var str in ex.StackTrace.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                yield return str;
            }
        }

        private static void OnUnresolvedException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            Log.WriteLine(LogLevel.Fatal, $"Trinity.FFI: Unhandled exception:");
            foreach(var str in DumpException(ex))
            {
                Log.WriteLine(LogLevel.Fatal, $"Trinity.FFI: {str}");
            }
        }

        private static System.Reflection.Assembly OnMissingAssemblyResolve(object sender, ResolveEventArgs args)
        {
            Log.WriteLine(LogLevel.Warning, $"Trinity.FFI: Resolving missing assembly {args.Name}.");
            return null;
        }
    }
}
