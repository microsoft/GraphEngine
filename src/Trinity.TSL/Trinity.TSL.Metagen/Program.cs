using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Trinity.TSL.Metagen
{
    partial class Metagen
    {
        static bool s_force_process = false;
        static HashSet<string> s_forbidden_directories = new HashSet<string>
        {
            "bin",
            "obj",
            "x64",
            "tsl",
            ".",
            "..",
        };
        static string s_root;

        static void Main(string[] args)
        {
            if (args.Length > 1 &&args[1] == "-force")
            {
                s_force_process = true;
            }

            Console.WriteLine($"Trinity.TSL.MetaGen: path = {args[0]}");

            s_root = Path.GetFullPath(args[0]);
            ProcessDirectory(s_root);
        }

        private static void ProcessDirectory(string directoryPath)
        {
            directoryPath = Path.GetFullPath(directoryPath);

            Environment.CurrentDirectory = directoryPath;

            foreach (var file in Directory.GetFiles(directoryPath))
                if (!Path.GetFileName(file).StartsWith("_", StringComparison.Ordinal) && Path.GetExtension(file) == ".cs")
                    ProcessFile(file);

            foreach (var dir in Directory.GetDirectories(directoryPath))
            {
                if (!s_forbidden_directories.Contains(Path.GetFileName(dir).ToLowerInvariant()))
                    ProcessDirectory(dir);
            }
        }

        private static void ProcessFile(string filename)
        {
            try
            {
                filename              = Path.GetFullPath(filename);
                String literal        = File.ReadAllText(filename);
                string name           = Path.GetFileNameWithoutExtension(filename);
                string dir_path       = filename
                    .Substring(0, filename.Length - Path.GetFileName(filename).Length)
                    .Substring(s_root.Length)
                    .Trim(Path.DirectorySeparatorChar);

                dir_path = Path.Combine(s_root, "", "Trinity.TSL.CodeGen", dir_path);

                string targetFilename = Path.Combine(dir_path, Path.GetFileName(filename)+".cpp");
                bool needProcess      = (File.GetLastWriteTime(targetFilename) < File.GetLastWriteTime(filename));

                if (needProcess || s_force_process)
                {
                    Console.Write("Processing     {0}", filename);
                    var tokens            = GetTokens(ref literal);
                    try
                    {
                        string codegen        = GenerateCodeGen(name, tokens);
                        codegen = codegen.Replace("\r\n", "\n");

                        if (!Directory.Exists(dir_path))
                            Directory.CreateDirectory(dir_path);

                        File.WriteAllText(targetFilename, codegen);
                        PrintOK();
                    }
                    catch (Exception ex)
                    {
                        if (ex.Data.Contains("file_offset"))
                        {
                            Console.WriteLine("Context:");

                            int file_offset = (int)ex.Data["file_offset"];
                            int begin       = file_offset - 100;
                            int end         = file_offset + 100;
                            if (begin < 0)
                                begin = 0;
                            if (end > literal.Length)
                                end = literal.Length;

                            Console.WriteLine(literal.Substring(begin, end - begin));

                            Console.WriteLine();
                            if (ex.InnerException != null)
                            {
                                Console.WriteLine(ex.InnerException.ToString());
                            }
                        }

                        throw;
                    }
                }
                else
                {
                    Console.WriteLine("Skipping       {0}", filename);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error on file {0}", filename);
                if (ex.InnerException != null)
                {
                    Console.WriteLine(ex.InnerException.ToString());
                }

                throw;
            }
        }

        private static void PrintOK()
        {
            try
            {
                Console.SetCursorPosition(Console.BufferWidth - 4, Console.CursorTop);
                Console.Write("[OK]");
            }
            catch { Console.WriteLine(); }
        }

        /** 
         * There are two types of tempaltes.
         *  1. Module:
         *      Modules serve as code snippets, and will not be generated stand-alone.
         *  2. Template:
         *      Tempaltes are stand-alone, and each template corresponds to a C++ class
         *      that generates a corresponding C# file according to the template.
         */
        private static string GenerateCodeGen(string template_name, List<MetaToken> source_template)
        {
            var codegen          = new StringBuilder();
            var vm               = new VM(codegen, source_template, template_name);

            vm.Execute();


            return codegen.ToString();
        }
    }
}
