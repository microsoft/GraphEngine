using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Collections.Generic;
using System.Diagnostics;
using NUnitLite;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;

namespace NUnitLiteNetCoreTest
{
    class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.Error.WriteLine("Usage: runner <runner-path> <test-assembly> [nunitlite-arg ..]");
                return 1;
            }

            var runnerPath = Path.GetFullPath(args[0]);
            var assemblyPath = Path.GetFullPath(args[1]);
            var remainingOptions = args.Skip(2).ToArray();

            var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);

            var rootTestSuite = GetITest(assembly, remainingOptions);
            var allTestNames = GetDecendentTests(rootTestSuite)
                                .Select(_ => _.FullName)
                                .ToList();

            Console.WriteLine("Discovered tests:");
            foreach (var testName in allTestNames)
            {
                Console.WriteLine(testName);
            }

            foreach (var testName in allTestNames)
            {
                var process = CreateProcessForTest(runnerPath, assemblyPath, testName);
                process.WaitForExit();
            }
            return 0;
        }

        private static Process CreateProcessForTest(string runnerPath, string assemblyPath, string testName)
        {
            var commandLineOptions = new List<string>();
            commandLineOptions.Add($"\"{runnerPath}\"");
            commandLineOptions.Add($"\"{assemblyPath}\"");
            commandLineOptions.Add("--workers=1");
            commandLineOptions.Add($"--test={testName}");

            var startInfo = new ProcessStartInfo();
            startInfo.FileName = "dotnet";
            startInfo.Arguments = string.Join(" ", commandLineOptions);
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;

            return Process.Start(startInfo);
        }

        private static ITest GetITest(Assembly assembly, string[] commandLineOptions)
        {
            // we should control what `options` holds
            commandLineOptions = new [] {"--explore"};
            var options = new NUnitLiteOptions(commandLineOptions);
            var builder = new DefaultTestAssemblyBuilder();
            var runSettings = TextRunner.MakeRunSettings(options);
            return builder.Build(assembly, runSettings);
        }

        private static IEnumerable<ITest> GetDecendentTests(ITest root)
        {
            if (root.IsSuite)
                return root.Tests.SelectMany(GetDecendentTests);
            return new []{root};
        }
    }
}
