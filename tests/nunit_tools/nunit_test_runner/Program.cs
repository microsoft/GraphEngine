using System;
using System.Linq;
using System.Reflection;
using System.IO;
using NUnitLite;
using Trinity.Core;
using NUnit.Framework;

#if !NETFRAMEWORK
using System.Runtime.Loader;
#endif

namespace nunitlite_runner
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.Error.WriteLine("Usage: runner <test-assembly> [nunit-arg ...]");
                return 1;
            }
            var path = Path.GetFullPath(args[0]);
            var assembly = LoadAssembly(path);
            return new AutoRun(assembly).Execute(args.Skip(1).ToArray());
        }

        private static Assembly LoadAssembly(string path)
        {
#if NETFRAMEWORK
            return Assembly.LoadFrom(path);
#else
            return AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
#endif
        }
    }
}
