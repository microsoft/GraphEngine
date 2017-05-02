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

// preloading dependencies
using Trinity;
using NUnit.Framework;

namespace NUnitMetaRunner
{
    class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.Error.WriteLine("Usage: runner <runner-path> <test-assembly> <result-dir> [nunitlite-arg ..]");
                return 1;
            }

            var runnerPath = Path.GetFullPath(args[0]);
            var resultDirPath = Path.GetFullPath(args[1]);
            var assemblyPath = Path.GetFullPath(args[2]);
            var remainingOptions = args.Skip(3).ToArray();

            // NOTE(leasunhy): this may fail silently if some of the dependencies are not preloaded!
            //                 In that case, no tests can be discovered in `assembly`.
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
                var process = CreateProcessForTest(runnerPath, assemblyPath, resultDirPath, remainingOptions, testName);
                process.WaitForExit();
                Console.WriteLine(process.StandardOutput.ReadToEnd());
                // TODO(leasunhy): check the exit status of the process and regard the test as a failure
                //                 if the process did not exit normally.
            }
            return 0;
        }

        private static Process CreateProcessForTest(string runnerPath, string assemblyPath, string resultDir,
                                                    string[] runnerOptions, string testName)
        {
            var commandLineOptions = new List<string>();
            commandLineOptions.Add($"\"{runnerPath}\"");
            commandLineOptions.Add($"\"{assemblyPath}\"");
            commandLineOptions.AddRange(runnerOptions);
            commandLineOptions.Add($"--test={testName}");
            var testResultPath = Path.Combine(resultDir, $"{testName}.xml");
            commandLineOptions.Add($"--result=\"{testResultPath}\"");

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
