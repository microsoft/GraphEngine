using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Trinity.Diagnostics;
using Trinity.Utilities;

namespace Trinity.Storage.Composite
{
    // Command tool configurations
    public static class Commands
    {
        private static string c_codegen_cmd
        {
            get
            {
                var nuget_proc = _System("dotnet", "nuget locals global-packages -l");
                var output = nuget_proc.StandardOutput.ReadToEnd();
                var tag = "global-packages: ";
                var package_root = output.Substring(output.IndexOf(tag) + tag.Length).Trim();
                Console.WriteLine(package_root);
                var codegen = Path.Combine(package_root, "GraphEngine.Core/1.0.9083/tools/Trinity.TSL.CodeGen");
                if (Environment.OSVersion.Platform == PlatformID.Win32NT) codegen += ".exe";
                return codegen;
            }
        }
        private static string c_dotnet_cmd => "dotnet";

        public static bool TSLCodeGenCmd(string arguments)
        {
            try
            {
                CmdCall(c_codegen_cmd, arguments);
            }
            catch (Exception e)
            {
                Log.WriteLine(LogLevel.Error, "{0}", e.Message);
                return false;
            }
            return true;
        }

        public static bool DotNetBuildCmd(string arguments)
        {
            try
            {
                CmdCall(c_dotnet_cmd, arguments);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }

        private static void CmdCall(string cmd, string arguments)
        {
            Log.WriteLine("Commands: " + cmd + " " + arguments);
            Process proc = _System(cmd, arguments);
            proc.OutputDataReceived += OnChildStdout;
            proc.ErrorDataReceived += OnChildStderr;
            proc.BeginOutputReadLine();
            proc.WaitForExit();
        }

        private static Process _System(string cmd, string arguments)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = cmd;
            proc.StartInfo.Arguments = arguments;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;
            proc.Start();
            return proc;
        }

        private static void OnChildStdout(object sender, DataReceivedEventArgs e)
            => OnChildOutputImpl(sender as Process, e.Data, LogLevel.Info);

        private static void OnChildStderr(object sender, DataReceivedEventArgs e)
            => OnChildOutputImpl(sender as Process, e.Data, LogLevel.Error);

        private static void OnChildOutputImpl(Process process, string data, LogLevel logLevel)
        {
            data = data?.TrimEnd();
            if (data?.Length <= 0) return;
            Log.WriteLine(logLevel, $"{nameof(Commands)}: {{0}}", data);
        }
    }

    // Settings of storage path
    internal class PathHelper
    {
        private const string FolderName = "composite-helper";
        public static string Directory => FileUtility.CompletePath(Path.Combine(TrinityConfig.StorageRoot, FolderName), create_nonexistent: true);
        public static string VersionRecorders => Path.Combine(Directory, "VersionRecorders.bin");
        public static string CellTypeIDs => Path.Combine(Directory, "CellTypeIDs.bin");
        public static string IDIntervals => Path.Combine(Directory, "IDIntervals.bin");
        public static string DLL(string dllName) => Path.Combine(Directory, dllName);
    }
}
