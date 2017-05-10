using CommandLine;
using CommandLine.Text;

namespace NUnitMetaRunner
{
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
