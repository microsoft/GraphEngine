using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity;
using Trinity.Diagnostics;
using Trinity.Storage;
using Trinity.TSL.Lib;

namespace GraphEngine.DataImporter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CmdOptions opts = new CmdOptions();
            if (CommandLine.Parser.Default.ParseArguments(args, opts))
            {
                List<string> files = new List<string>(opts.ExplicitFiles.Select(_ => _.Trim()));
                if (opts.InputDirectory != null)
                {
                  //  Log.WriteLine("Including files from directory {0}", Path.GetFullPath(opts.InputDirectory));
                    files.AddRange(Directory.GetFiles(opts.InputDirectory).Select(_ => _.Trim()));
                }

                Stopwatch timer = Stopwatch.StartNew();

                if (opts.TSL != null)
                {
                    var tslCompiler = new TSLCompiler();
                    opts.TSLAssembly = tslCompiler.Compile(opts.TSL);
                    if (opts.TSLAssembly != null)
                    {
                        Importer.Import(opts.TSLAssembly, files, opts);
                    }
                    else
                    {
                        Log.WriteLine("TSL File Compile Error."); 
                    }
                }
                else if (opts.TSLAssembly != null)
                {
                    Importer.Import(opts.TSLAssembly, files, opts);
                }
                else if (opts.GenerateTSL)
                {
                    TSLGenerator.Generate(files, opts);
                }

                timer.Stop();
                Log.WriteLine("Time: {0} seconds.", timer.ElapsedMilliseconds / 1000);
                
            }
        }
    }
}
