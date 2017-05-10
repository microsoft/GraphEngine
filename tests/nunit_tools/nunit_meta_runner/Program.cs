using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;
using CommandLine;
using CommandLine.Text;

using NUnitLite;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;

using NUnitLiteNetCoreTest.ResultAggregator;

// preloading dependencies
using Trinity;
using NUnit.Framework;

namespace NUnitMetaRunner
{
    class Program
    {
        public static int Main(string[] args)
        {
            var options = new CommandLineOptions();
            if (!CommandLine.Parser.Default.ParseArguments(args, options))
            {
                return 1;
            }

            var runnerPath = Path.GetFullPath(options.RunnerPath);
            var resultDirRoot = Path.GetFullPath(options.ResultDirPath);
            var assemblyPath = Path.GetFullPath(options.AssemblyPath);
            var runnerOptions = options.RunnerOptions;
            var timeout = options.Timeout;
            var randomSeed = options.RandomSeed ?? new Random().Next();

            // NOTE(leasunhy): this may fail silently if some of the dependencies are not preloaded!
            //                 In that case, no tests can be discovered in `assembly`.
            var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);

            var rootTestSuite = GetITest(assembly, runnerOptions);
            var allTestNames = GetDecendentTests(rootTestSuite)
                                .Select(_ => _.FullName)
                                .ToList();

            Console.WriteLine("Discovered tests:");
            foreach (var testName in allTestNames)
            {
                Console.WriteLine(testName);
            }

            var resultDirPath = Path.Combine(resultDirRoot, rootTestSuite.Name);
            if (!Directory.Exists(resultDirPath))
                Directory.CreateDirectory(resultDirPath);

            foreach (var testName in allTestNames)
            {
                try
                {
                    var process = CreateProcessForTest(runnerPath, assemblyPath, resultDirPath,
                                                       randomSeed, runnerOptions, testName);
                    process.WaitForExit(timeout < 0 ? Int32.MaxValue : timeout);
                    if (!process.HasExited)
                    {
                        Console.WriteLine($"Test {testName} has timed out.");
                        process.Kill();
                    }
                    Console.WriteLine(process.StandardOutput.ReadToEnd());
                    // TODO(leasunhy): check the exit status of the process and regard the test as a failure
                    //                 if the process did not exit normally.
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.Message);
                }
            }

            var xmlDocs = Directory.EnumerateFiles(resultDirPath)
                .Where(f => Path.GetExtension(f) == ".xml")
                .Select(XDocument.Load)
                .ToList();
            if (xmlDocs.Count != allTestNames.Count)
            {
                Console.Error.WriteLine($"Warning: {allTestNames.Count} tests should be run in {rootTestSuite.Name}, " +
                                        $"but only {xmlDocs.Count} result files are found.");
            }

            var aggregated = ResultAggregator.Aggregate(xmlDocs);
            var resultFile = Path.Combine(resultDirRoot, rootTestSuite.Name + ".xml");
            using (var output = new StreamWriter(new FileStream(resultFile, FileMode.Create)))
            {
                aggregated.Save(output);
            }
            return 0;
        }

        private static Process CreateProcessForTest(string runnerPath, string assemblyPath, string resultDir,
                                                    int randomSeed, string runnerOptions, string testName)
        {
            var commandLineOptions = new List<string>();
            commandLineOptions.Add($"\"{runnerPath}\"");
            commandLineOptions.Add($"\"{assemblyPath}\"");
            commandLineOptions.Add(runnerOptions);
            commandLineOptions.Add($"--test={testName}");
            var testResultPath = Path.Combine(resultDir, $"{testName}.xml");
            commandLineOptions.Add($"--result=\"{testResultPath}\"");
            commandLineOptions.Add($"--seed={randomSeed}");

            var startInfo = new ProcessStartInfo();
            startInfo.FileName = "dotnet";
            startInfo.Arguments = string.Join(" ", commandLineOptions);
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;

            return Process.Start(startInfo);
        }

        private static ITest GetITest(Assembly assembly, string commandLineOptions)
        {
            // we should control what `options` holds
            var options = new NUnitLiteOptions(commandLineOptions.Split(' '));
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

    class CommandLineOptions
    {
        [Option('r', "runner", Required = true,
                HelpText = "The path to the runner for the test assembly.")]
        public string RunnerPath { get; set; }

        [Option('t', "timeout", Required = false, DefaultValue = -1,
                HelpText = "Set timeout for each test case in milliseconds.")]
        public int Timeout { get; set; }

        [Option('d', "resultDirectory", Required = true,
                HelpText = "Set the directory to put the results.")]
        public string ResultDirPath { get; set; }

        [Option('a', "assembly", Required = true,
                HelpText = "The path to the test assembly.")]
        public string AssemblyPath { get; set; }

        [Option('o', "options", Required = false, DefaultValue = "",
                HelpText = "The command line arguments to be passed to the runner.")]
        public string RunnerOptions { get; set; }

        [Option('s', "seed", Required = false,
                HelpText = "The random seed to use for the tests.")]
        public int? RandomSeed { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
                    (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
