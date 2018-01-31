using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
namespace DynamicLoading
{
    
    public static class Center
    {
        public static void Init(string DLLInclude, 
                                string TSLCodeGenWhere, 
                                string DotNetWhere="dotnet.exe", 
                                string moduleName=null) 
        {
            Namespace = moduleName ?? "CellAssembly";
            Cmd.TSLCodeGenWhere = TSLCodeGenWhere;
            Cmd.DotNetWhere = DotNetWhere;
            CSProj.Include = DLLInclude;
        }

        public static string Namespace;

        public static List<AsmVersion> AsmVersions = new List<AsmVersion> { };
        public static AsmVersion CurrentVersion;
        
        
        private static int _currentCellTypeOffset = 1;
        public static int CurrentCellTypeOffset { get => _currentCellTypeOffset; }


        private static List<Assembly> _asmSlides = new List<Assembly> { };
        public static List<Assembly> AsmSlides { get => _asmSlides; }


        public static class Cmd
        {
            public static string TSLCodeGenWhere;
            public static string DotNetWhere = "dotnet";
            public static bool TSLCodeGenCmd(string arguments) {
                try {
                    CmdCall(TSLCodeGenWhere, arguments);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
                return true;
            }

            public static bool DotNetBuildCmd(string arguments){
                try {
                    CmdCall(DotNetWhere, arguments);
                }
                catch(Exception e)
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

        public static void CreateCSProj()
        {
            var path = System.IO.Path.Combine(
                        CurrentVersion.TslBuildDir,
                        $"{Namespace}.csproj");
            using (var s = System.IO.File.CreateText(path))
            {
                CSProj.TemplateLine.ForEach(s.WriteLine);
            }
        }
        public static bool CodeGenFn()
        {
            System.IO.Directory.GetFiles(CurrentVersion.TslSrcDir, "*.tsl").ToList().ForEach(Console.WriteLine);
            return Cmd.TSLCodeGenCmd(
                string.Join(" ", System.IO.Directory.GetFiles(CurrentVersion.TslSrcDir, "*.tsl")) +
                $" -offset {CurrentVersion.CellTypeOffset} -n {Namespace} -o {CurrentVersion.TslBuildDir}");
        }

        public static bool BuildFn() =>
            Cmd.DotNetBuildCmd($"build {CurrentVersion.TslBuildDir} -o {CurrentVersion.AsmLoadDir}");

        public static Assembly LoadFn() {
            Console.WriteLine("Loading " + System.IO.Path.Combine(CurrentVersion.AsmLoadDir, $"{Namespace}.dll"));
            return Assembly.LoadFrom(System.IO.Path.Combine(CurrentVersion.AsmLoadDir, $"{Namespace}.dll"));
        }
      
        public static void LoadFrom(
                    string tslsrcDir, 
                    string tslBuildDir = null, 
                    string asmLoadDir = null,
                    string versionName = null)
        {

            CurrentVersion = new AsmVersion(
                _currentCellTypeOffset,
                versionName ?? DateTime.Now.ToString(),
                tslsrcDir, tslBuildDir?? tslsrcDir, asmLoadDir??tslBuildDir??tslsrcDir);

            Console.WriteLine("tslsrcDir: " + tslsrcDir +
                              "TslBuildDir : " + CurrentVersion.TslBuildDir + 
                              ", AsmLoadDir : " + CurrentVersion.AsmLoadDir);

            try
            {
                CreateCSProj();
            }
            catch (Exception e)
            {
                throw e;
            }
            if (!CodeGenFn())
            {
                throw new TSLCodeGenError();
            }

            if (!BuildFn())
            {
                throw new TSLBuildError();
            }

            try
            {
                var asm = LoadFn();
                asm.GetTypes().ToList().ForEach(Console.WriteLine);
                _asmSlides.Add(asm);

                /// TODO: get cell_type count by `asm`.
                _currentCellTypeOffset += 999;
                AsmVersions.Add(CurrentVersion);
                CurrentVersion = null;
            }
            catch(Exception e)
            {
                throw new AsmLoadError(e.Message);
            }      
            
        }
    }
}
