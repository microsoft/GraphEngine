using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using Trinity.Storage;
using Trinity.Utilities;

namespace DynamicLoading
{
    
    
    
    public static class ConfigConstant
    {
        public static int AvgMaxAsmNum = 100;
        public static int AvgCellNum = 10;
        public static int AvgFieldNum = 3;
    }
    public static class Center
    {
        
        public static Dictionary<string, int> CellTypeIDs 
            = new Dictionary<string, int>(ConfigConstant.AvgMaxAsmNum * ConfigConstant.AvgCellNum);
        // CellTypeName => CellTypeID

        public static class Leader
        {
            public static List<IStorageSchema> StorageSchema 
                = new List<IStorageSchema>(ConfigConstant.AvgMaxAsmNum);

            public static List<IGenericCellOperations> GenericCellOperations 
                = new List<IGenericCellOperations>(ConfigConstant.AvgMaxAsmNum);
        }
       
        public static List<int> IDIntervals = new List<int>(ConfigConstant.AvgMaxAsmNum * ConfigConstant.AvgCellNum);
        /// CellID => the index of Leader.StorageSchema and Leader.GenericCellOperations

        public static void Init(string DLLInclude,
                                string TSLCodeGenWhere,
                                string DotNetWhere = "dotnet.exe",
                                string moduleName = null)
        {
            Namespace = moduleName ?? "TslAssembly";
            Cmd.TSLCodeGenWhere = TSLCodeGenWhere;
            Cmd.DotNetWhere = DotNetWhere;
            CSProj.Include = DLLInclude;
            assemblies = CSProj.Reference.Names
                                .Select(_ => Path.Combine(CSProj.Include, $"{_}.dll"))
                                .Select(Assembly.LoadFrom)
                                .ToArray();
            IDIntervals.Add(_currentCellTypeOffset);
        }

        public static string Namespace;
        public static List<AsmVersion> AsmVersions = new List<AsmVersion> { };
        public static AsmVersion CurrentVersion;

        private static Assembly[] assemblies;
        public static Assembly[] ReferenceAssemblies { get => assemblies;}
        
        private static int _currentCellTypeOffset = 0;
        public static int CurrentCellTypeOffset { get => _currentCellTypeOffset; }


        private static List<Assembly> _asmSlides = new List<Assembly> { };
        public static List<Assembly> AsmSlides { get => _asmSlides; }


        public static class Cmd
        {
            public static string TSLCodeGenWhere;
            // TODO conditionals for supporting both msbuild (netfx) and dotnet (coreclr)
            public static string DotNetWhere = "dotnet";
            public static bool TSLCodeGenCmd(string arguments)
            {
                try
                {
                    CmdCall(TSLCodeGenWhere, arguments);
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
                    CmdCall(DotNetWhere, arguments);
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

        public static void CreateCSProj()
        {
            var path = System.IO.Path.Combine(
                        CurrentVersion.TslBuildDir,
                        $"{Namespace}.csproj");

            File.WriteAllText(path, CSProj.Template);
            
        }
        public static bool CodeGenFn()
        {
            Directory.GetFiles(CurrentVersion.TslSrcDir, "*.tsl").ToList().ForEach(Console.WriteLine);
            return Cmd.TSLCodeGenCmd(
                string.Join(" ", Directory.GetFiles(CurrentVersion.TslSrcDir, "*.tsl")) +
                $" -offset {CurrentVersion.CellTypeOffset} -n {Namespace} -o {CurrentVersion.TslBuildDir}");
        }

        public static bool BuildFn() =>
            Cmd.DotNetBuildCmd($"build {CurrentVersion.TslBuildDir} -o {CurrentVersion.AsmLoadDir}");

        public static Assembly LoadFn()
        {
            Console.WriteLine("Loading " + Path.Combine(CurrentVersion.AsmLoadDir, $"{Namespace}.dll"));
            return Assembly.LoadFrom(Path.Combine(CurrentVersion.AsmLoadDir, $"{Namespace}.dll"));
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
                tslsrcDir, tslBuildDir ?? tslsrcDir, asmLoadDir ?? tslBuildDir ?? tslsrcDir);

            Console.WriteLine("\n tslsrcDir: " + tslsrcDir +
                              "\n TslBuildDir : " + CurrentVersion.TslBuildDir +
                              "\n AsmLoadDir : " + CurrentVersion.AsmLoadDir);

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
                var schema = AssemblyUtility.GetAllClassInstances<IStorageSchema>(assembly: asm).First();
                var cellOps = AssemblyUtility.GetAllClassInstances<IGenericCellOperations>(assembly: asm).First();
                var cellDescs = schema.CellDescriptors.ToList();

                // shall we use transactions here? <--- yes, todo, snapshot everything until we are sure there're no exceptions
                Leader.StorageSchema.Add(schema);
                Leader.GenericCellOperations.Add(cellOps);
                int maxoffset = _currentCellTypeOffset;
 
                foreach (var cellDesc in cellDescs)
                {
                    CellTypeIDs[cellDesc.TypeName] = cellDesc.CellType;
                    // Assertion 1: New cell type does not crash into existing type space
                    Debug.Assert(_currentCellTypeOffset <= cellDesc.CellType);
                    maxoffset = Math.Max(maxoffset, cellDesc.CellType);

#if DEBUG
                    Console.WriteLine($"{cellDesc.TypeName}{{");

                    foreach(var fieldDesc in cellDesc.GetFieldDescriptors())
                    {    
                        Console.WriteLine($"    {fieldDesc.Name}: {fieldDesc.TypeName}");
                    }
                    Console.WriteLine("}");
#endif
                }
                IDIntervals.Add(_currentCellTypeOffset);
                // Assertion 2: intervals grow monotonically
                Debug.Assert(IDIntervals.OrderBy(_ => _).SequenceEqual(IDIntervals));
                _currentCellTypeOffset += cellDescs.Count;
                // Assertion 3: The whole type id space is still compact
                Debug.Assert(_currentCellTypeOffset == maxoffset + 1);


                _asmSlides.Add(asm);
                CurrentVersion.Asm = asm;
                AsmVersions.Add(CurrentVersion);
                CurrentVersion = null;
            }
            catch (Exception e)
            {
                throw new AsmLoadError(e.Message);
            }

        }
        public static class GetIntervalIndex
        {
            public static int ByCellTypeID(int cellTypeID) {
                int seg = Center.IDIntervals.FindLastIndex(seg_head => seg_head < cellTypeID);
                if (seg == -1 || seg == Center.IDIntervals.Count)
                    throw new CellTypeNotMatchException("Cell type id out of the valid range.");
                return seg;
            }

            public static int ByCellTypeName(string cellTypeName)
            {
                if (!Center.CellTypeIDs.Keys.Contains(cellTypeName))
                    throw new CellTypeNotMatchException("Unrecognized cell type string.");
                int seg = ByCellTypeID(Center.CellTypeIDs[cellTypeName]);
                return seg;
            }
        }
    }
}
