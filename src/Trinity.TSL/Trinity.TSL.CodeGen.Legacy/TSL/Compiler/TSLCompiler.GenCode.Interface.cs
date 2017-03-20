using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                ContractTemplate.GenerateCode() +
                MessageAccessorTemplate.GenerateCode();

            cw +=
            @"
}
";
            cw += Verbatim.GetFooter();
            commit_source_file(src_protocol);

            return source_files;
        }
    }
}
