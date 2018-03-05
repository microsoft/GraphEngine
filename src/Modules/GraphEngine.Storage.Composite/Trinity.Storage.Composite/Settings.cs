using System;
using System.Diagnostics;
using System.IO;
using Trinity.Diagnostics;
using Trinity.Utilities;

namespace Trinity.Storage.Composite
{
    // Command tool configurations
    #region Cmd
    internal static class Cmd
    {
        public const string c_codegen_cmd = "Trinity.TSL.CodeGen";
        public const string c_dotnet_cmd = "dotnet";

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

        public static void CmdCall(string cmd, string arguments)
        {
            Log.WriteLine("command:  " + cmd + " " + arguments);
            Process proc = new Process();
            proc.StartInfo.FileName = cmd;
            proc.StartInfo.Arguments = arguments;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;
            proc.Start();
            proc.OutputDataReceived += OnChildStdout;
            proc.ErrorDataReceived += OnChildStderr;
            proc.BeginOutputReadLine();
            proc.WaitForExit();
        }

        private static void OnChildStdout(object sender, DataReceivedEventArgs e)
            => OnChildOutputImpl(sender as Process, e.Data, LogLevel.Info);

        private static void OnChildStderr(object sender, DataReceivedEventArgs e)
            => OnChildOutputImpl(sender as Process, e.Data, LogLevel.Error);

        private static void OnChildOutputImpl(Process process, string data, LogLevel logLevel)
        {
            string name = process?.ProcessName ?? "";
            Log.WriteLine(logLevel, $"{name}: {{0}}", data);
        }
    }
    #endregion

    // Settings of storage path
    #region Path settings 
    internal class PathHelper
    {
        private const string FolderName = "composite-helper";
        public static string Directory => FileUtility.CompletePath(Path.Combine(TrinityConfig.StorageRoot, FolderName), create_nonexistent: true);
        public static string VersionRecorders => Path.Combine(Directory, "VersionRecorders.bin");
        public static string CellTypeIDs => Path.Combine(Directory, "CellTypeIDs.bin");
        public static string IDIntervals => Path.Combine(Directory, "IDIntervals.bin");
        public static string DLL(string dllName) => Path.Combine(Directory, dllName);
    }
    #endregion
}
