using Microsoft.Build.Framework;
using Microsoft.Build.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.TSL
{
    public class TSLCompilerTask : Microsoft.Build.Tasks.Exec
    {
        public TSLCompilerTask()
        {
            /* Bypass the built-in command processor */
            this.UseCommandProcessor = false;
        }

        [Required]
        public string TrinityPath { get; set; }

        [Required]
        public string ProjectRoot { get; set; }

        [Required]
        public string OutputPath { get; set; }

        public string RootNamespace { get; set; }

        [Required]
        public string ScriptList { get; set; }

        protected override Encoding StandardErrorEncoding
        {
            get
            {
                return System.Text.Encoding.UTF8;
            }
        }

        protected override Encoding StandardOutputEncoding
        {
            get
            {
                return System.Text.Encoding.UTF8;
            }
        }

        protected override string ToolName
        {
            get
            {
                //TODO platform
                return "Trinity.TSL.CodeGen.exe";
            }
        }

        protected override void AddCommandLineCommands(Microsoft.Build.Tasks.CommandLineBuilderExtension commandLine)
        {
            commandLine.AppendSwitchIfNotNull("--ProjectRoot ", ProjectRoot);
            commandLine.AppendSwitchIfNotNull("--OutputPath ", OutputPath);
            commandLine.AppendSwitchIfNotNull("--RootNamespace ", RootNamespace);
            commandLine.AppendSwitchIfNotNull("--ScriptList ", ScriptList);

            if (Command != null)
                commandLine.AppendTextUnquoted(" " + Command);
        }

        protected override int ExecuteTool(string pathToTool, string responseFileCommands, string commandLineCommands)
        {
            /* In ExecuteTool Exec will attempt to delete the 'script file'.
             * That would cause an exception for sure. However,
             * we could not derive from its base, nor can we invoke
             * base.base.ExecuteTool. So we make a dummy file for it
             * to delete. To do this, we invoke base's AddCommandLineCommands
             * so that it calls CreateTemporaryBatchFile. Since the base's base
             * already captured our correct parameters, it is safe to hack it.
             */
            this.Command = "TSLCompileTask"; // This will trick VS to display "The command 'TSLCompileTask' failed with code..." on error.
            Microsoft.Build.Tasks.CommandLineBuilderExtension commandLine = new CommandLineBuilderExtension();
            base.AddCommandLineCommands(commandLine);

            return base.ExecuteTool(pathToTool, responseFileCommands, commandLineCommands);
        }

        protected override string GenerateFullPathToTool()
        {
            return Path.Combine(TrinityPath, ToolName);
        }
    }
}
