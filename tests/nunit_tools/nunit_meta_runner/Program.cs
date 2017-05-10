using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.Loader;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Xml;
using System.Xml.Linq;
using CommandLine;
using CommandLine.Text;
using NUnitLite;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

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
            var allTests = GetDecendentTests(rootTestSuite).ToList();

            Console.WriteLine("Discovered tests:");
            foreach (var test in allTests)
            {
                Console.WriteLine(test.FullName);
            }

            var resultDirPath = Path.Combine(resultDirRoot, rootTestSuite.Name);
            if (!Directory.Exists(resultDirPath))
                Directory.CreateDirectory(resultDirPath);

            foreach (var test in allTests)
            {
                var testName = test.FullName;
                var testResultPath = Path.Combine(resultDirPath, $"{testName}.xml");
                try
                {
                    var process = CreateProcessForTest(runnerPath, assemblyPath, testResultPath,
                                                       randomSeed, runnerOptions, testName);
                    if (!process.WaitForExit(timeout < 0 ? Int32.MaxValue : timeout))
                    {
                        Console.WriteLine($"Test {testName} has timed out.");
                        process.Kill();
                        process.WaitForExit();
                    }
                    Console.WriteLine(process.StandardOutput.ReadToEnd());
                    // TODO(leasunhy): check the exit status of the process and regard the test as a failure
                    //                 if the process did not exit normally.
                    if (process.ExitCode != 0)
                    {
                        using (var stream = new FileStream(testResultPath, FileMode.Create))
                        using (var writer = new StreamWriter(stream))
                        {
                            writer.WriteLine(ConstructXmlDocumentForAbnormalExit(test));
                        }
                    }
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
            if (xmlDocs.Count != allTests.Count)
            {
                Console.Error.WriteLine($"Warning: {allTests.Count} tests should be run in {rootTestSuite.Name}, " +
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

        private static Process CreateProcessForTest(string runnerPath, string assemblyPath, string testResultPath,
                                                    int randomSeed, string runnerOptions, string testName)
        {
            var commandLineOptions = new List<string>();
            commandLineOptions.Add($"\"{runnerPath}\"");
            commandLineOptions.Add($"\"{assemblyPath}\"");
            commandLineOptions.Add(runnerOptions);
            commandLineOptions.Add($"--test={testName}");
            commandLineOptions.Add($"--result=\"{testResultPath}\"");
            commandLineOptions.Add($"--seed=\"{randomSeed}\"");

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

        private static string ConstructXmlDocumentForAbnormalExit(ITest test)
        {
            var caseResult = new TestCaseResult((TestMethod)test);
            var result = new ResultState(TestStatus.Failed, FailureSite.Test);
            caseResult.SetResult(result);
            TestResult currentResult = caseResult;
            ITest currentTest = test;
            while (currentTest.Parent != null)
            {
                var suiteResult = new TestSuiteResult((TestSuite)currentTest.Parent);
                suiteResult.AddResult(currentResult);
                currentResult = suiteResult;
                currentTest = currentTest.Parent;
            }
            var node = currentResult.ToXml(true);
            var res = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<test-run fullname=""" + currentTest.FullName + @""">
" + node.OuterXml + @"</test-run>";
            return res;
        }
    }
}
