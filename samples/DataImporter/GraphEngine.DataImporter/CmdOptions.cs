using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GraphEngine.DataImporter
{
    class CmdOptions
    {
        [Option('t', "tsl", HelpText = "Specifies the TSL assembly for data importing.", MutuallyExclusiveSet = "Action")]
        public string TSLAssembly { get; set; }

        [Option('d', "dir", HelpText = "Import all .json and .txt files from directory", Required=false)]
        public string InputDirectory { get; set; }

        [Option('o', "output", HelpText = "Specifies data import output directory for importing tasks, and specifies the output TSL file name for TSL generation tasks", Required=false)]
        public string Output { get; set; }

        [Option('g', "generate_tsl", HelpText = "Generates TSL", MutuallyExclusiveSet = "Action")]
        public bool GenerateTSL { get; set; }

        [Option('s', "sorted", HelpText = "Specifies that the data is already sorted/grouped by entities", DefaultValue = false)]
        public bool Sorted { get; set; }

        [Option('p', "delimiter", HelpText = "Specifies the delimiter of CSV or TSV file", Required = false)]
        public char Delimiter { get; set; }

        [Option('f', "fileFormat", HelpText = "Specifies the file format", Required = false)]
        public string FileFormat { get; set; }

        [ValueList(typeof(List<string>))]
        public IList<string> ExplicitFiles { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var help = new HelpText
            {
                AdditionalNewLineAfterOption = true,
                AddDashesToOption = true
            };

            help.AddPreOptionsLine("Import from files to Graph Engine storage.");
            help.AddPreOptionsLine(string.Format("Usage: {0} [-t tsl_assembly|-g] [-d directory] [-o output_dir] [-p delimiter] [-f file_format] [explicit files]", Path.GetFileName(Assembly.GetExecutingAssembly().Location)));

            help.AddOptions(this);
            help.AddPostOptionsLine("Only files with .json, .csv, .tsv and .ntriples suffix are recognized.");
            help.AddPostOptionsLine("The file name of a data file will be used as the type for deserialization (except for RDF files).");
            help.AddPostOptionsLine("The type must be defined as a Graph Engine cell in the TSL.\n");

            return help;
        }
    }
}
