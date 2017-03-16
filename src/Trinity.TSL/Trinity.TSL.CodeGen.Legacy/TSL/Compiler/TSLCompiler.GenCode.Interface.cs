using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.TSL.CodeTemplates;
using Trinity.Utilities;

namespace Trinity.TSL
{
    public partial class TSLCompiler
    {
        internal static List<string> GenCode_impl(string outputDir, string defaultNamespace, bool genCodeOnly = false, bool cleanCode = false)
        {
            SpecificationScript script = new SpecificationScript();

            SpecificationScript.CurrentScript = script;

            /*******************************************Generate Code*********************************************/

            List<string> source_files = new List<string>();
            HashSet<string> types     = new HashSet<string>();
            CodeWriter cw             = new CodeWriter();

            #region Helper function
            Action<string> commit_source_file = (filename) =>
            {
                if (!Path.IsPathRooted(filename))
                {
                    filename = Path.Combine(outputDir, filename);
                }

                /* Case-insensitive filesystem/filename duplication fix, must be synchronized with Trinity.TSL.CodeGen.Neo.cpp */
                while (source_files.Any(file => file.ToLowerInvariant() == filename.ToLowerInvariant()))
                {
                    filename = Path.Combine(
                        Path.GetDirectoryName(filename), 
                        Path.GetFileNameWithoutExtension(filename) + "_.cs");
                }

                Console.WriteLine("Generating {0}...", Path.GetFileName(filename));
                File.WriteAllText(filename, cw.ToString());
                source_files.Add(filename);
            };
            #endregion

            string default_namespace = "namespace " + script.RootNamespace;

            //reset code writer
            cw = Verbatim.GetHeader(script);
            cw += default_namespace;
            cw +=
@"
{
";
            cw +=
                AbstractServerTemplate.GenerateCode(script) +
                AbstractProxyTemplate.GenerateCode(script) +
                AbstractModuleTemplate.GenerateCode(script) +
                ContractTemplate.GenerateCode() +
                MessageAccessorTemplate.GenerateCode();

            cw +=
            @"
}
";
            cw += Verbatim.GetFooter();
            commit_source_file(src_protocol);

            //reset ret
            cw = Verbatim.GetHeader(script);
            cw += default_namespace;
            cw +=
@"
{
";
            // Generate code for extension method in LocalStorage and MemoryCloud
            cw += StorageCodeTemplate.GenerateStorageIntegrationCode(script);
            cw +=
@"
}
";
            cw += Verbatim.GetFooter();
            commit_source_file(src_storage);


            //reset ret
            cw = Verbatim.GetHeader(script);
            cw += default_namespace;
            cw +=
@"
{
";
            List<string> external_namespaces = new List<string>();
            if (included_src_files.Count > 0)
            {
                cw = "using Trinity.External;\r\n" + cw;
            }

            StringBuilder str_builder = new StringBuilder();
            foreach (var src in included_src_files)
            {
                string[] lines = File.ReadAllLines(src);
                {
                    foreach (var line in lines)
                    {
                        if (line.StartsWith("namespace", StringComparison.Ordinal))
                        {
                            str_builder.AppendLine("namespace Trinity.External");
                            string[] parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            external_namespaces.Add(parts[1].Trim());
                        }
                        else
                        {
                            string new_line = line;
                            foreach (var ns in external_namespaces)
                            {
                                new_line = line.Replace(ns, "Trinity.External");
                            }
                            str_builder.AppendLine(new_line);
                        }
                    }
                }
            }
            cw += str_builder.ToString();
            cw +=
@"
}
";
            cw += Verbatim.GetFooter();
            commit_source_file(src_external);

            return source_files;
        }
    }
}
