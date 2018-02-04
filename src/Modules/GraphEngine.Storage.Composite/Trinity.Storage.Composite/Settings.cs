using System;
using System.Diagnostics;
using System.IO;
using Trinity.Utilities;

namespace Trinity.Storage.CompositeExtension
{
    // Allow to configure
    #region Constants 
    public static class ConfigConstant
    {
        private static int avgMaxAsmNum;
        private static int avgCellNum;
        private static int avgFieldNum;

        public static int AvgMaxAsmNum { get => avgMaxAsmNum; set => avgMaxAsmNum = value; }
        public static int AvgCellNum { get => avgCellNum; set => avgCellNum = value; }
        public static int AvgFieldNum { get => avgFieldNum; set => avgFieldNum = value; }
    }
    #endregion

    // Command tool configurations
    #region Cmd
    public static class Cmd
    {
        public static string TSLCodeGenExeLocation = "Trinity.TSL.CodeGen.exe";
        // TODO conditionals for supporting both msbuild (netfx) and dotnet (coreclr)
        public static string DotNetExeLocation = "dotnet.exe";
        public static bool TSLCodeGenCmd(string arguments)
        {
            try
            {
                CmdCall(TSLCodeGenExeLocation, arguments);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }

        public static bool DotNetBuildCmd(string arguments)
        {
            try
            {
                CmdCall(DotNetExeLocation, arguments);
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
            Console.WriteLine("command:  " + cmd + " " + arguments);
            Process proc = new Process();
            proc.StartInfo.FileName = cmd;
            proc.StartInfo.Arguments = arguments;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.Start();
            Console.WriteLine(proc.StandardOutput.ReadToEnd());
            proc.WaitForExit();
        }
    }
    #endregion

    // Settings of storage path
    #region Path settings 
    public class PathHelper
    {
        const string FolderName = "composite-helper";
        public static string Directory => FileUtility.CompletePath(Path.Combine(Trinity.TrinityConfig.StorageRoot, FolderName));
        public static string VersionRecorders => Path.Combine(Directory, "VersionRecorders.bin");
        public static string CellTypeIDs => Path.Combine(Directory, "CellTypeIDs.bin");
        public static string IDIntervals => Path.Combine(Directory, "IDIntervals.bin");
        public static string DLL(string dllName) => Path.Combine(Directory, dllName);
    }
    #endregion

    public static class Setting
    {
        public static void Configure(string IncludeDirectory, string StorageRoot,
                                     string TSLCodeGenExeLocation, string DotNetExeLocation,
                                     int avgMaxAsmNum, int avgCellNum, int avgFieldNum)
        {
            CSProj.IncludeDirectory = IncludeDirectory;
            Trinity.TrinityConfig.StorageRoot = StorageRoot;

            Cmd.TSLCodeGenExeLocation = TSLCodeGenExeLocation;
            Cmd.DotNetExeLocation = DotNetExeLocation;

            ConfigConstant.AvgMaxAsmNum = avgMaxAsmNum;
            ConfigConstant.AvgCellNum = avgCellNum;
            ConfigConstant.AvgFieldNum = avgFieldNum;
        }
    }

}
