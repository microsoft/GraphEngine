using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using NUnitLite;
using Trinity.Core;
using NUnit.Framework;

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
            var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(args[0]);
            return new AutoRun(assembly).Execute(args.Skip(1).ToArray());
        }
    }
}
