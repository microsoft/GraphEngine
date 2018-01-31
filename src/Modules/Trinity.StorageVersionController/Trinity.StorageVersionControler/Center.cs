using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Trinity.StorageVersionController
{
    
    public static class Center
    {
        public const string Namespace = "TSLAssembly";

        public static List<AsmVersion> AsmVersions = new List<AsmVersion> { };

        public static AsmVersion CurrentVersion;

        
        
        private static int _currentCellTypeOffset = 0;
        
        public static int CurrentCellTypeOffset { get => _currentCellTypeOffset; }



        private static List<Assembly> _asmSlides;

        public static List<Assembly> AsmSlides { get => _asmSlides; }


        public static class Cmd
        {
            public static Func<string, bool> TSLCodeGenCmd;
            public static Func<string, bool> DotNetBuildCmd;
        }

        public static bool CodeGenFn() =>
            Cmd.TSLCodeGenCmd(
                string.Join(' ', CurrentVersion.TslSrcDir) + 
                $" --offset {CurrentVersion.CellTypeOffset} --n {Namespace}");

        public static bool BuildFn() =>
            Cmd.DotNetBuildCmd(CurrentVersion.TslBuildDir);

        public static Assembly  LoadFn() =>
            Assembly.LoadFile(CurrentVersion.AsmLoadDir);
      
        public static void LoadFrom(
                    string tslsrcDir, 
                    string tslBuildDir, 
                    string asmLoadDir,
                    string versionName = null)
        {
            CurrentVersion = new AsmVersion(
                _currentCellTypeOffset,
                versionName ?? DateTime.Now.ToString(),
                tslsrcDir, tslBuildDir, asmLoadDir);

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
                _asmSlides.Add(asm);

                /// TODO: get cell_type count by `asm`.
                _currentCellTypeOffset += 99999;
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
