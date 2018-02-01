using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using Trinity.Storage;
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
            Namespace = moduleName ?? "CellAssembly";
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
        
        private static int _currentCellTypeOffset = 1;
        public static int CurrentCellTypeOffset { get => _currentCellTypeOffset; }


        private static List<Assembly> _asmSlides = new List<Assembly> { };
        public static List<Assembly> AsmSlides { get => _asmSlides; }


        public static class Cmd
        {
            public static string TSLCodeGenWhere;
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
                var schema = asm.GetType($"{Namespace}.StorageSchema");
                var cellOps = asm.GetType($"{Namespace}.GenericCellOperations");
                var cellDescs = schema
                                .GetMethod("get_CellDescriptors")
                                .Invoke(null, null) 
                                as IEnumerable<ICellDescriptor>;

                // shall we use transactions here?
                Leader.StorageSchema.Add(schema as IStorageSchema);
                Leader.GenericCellOperations.Add(cellOps as IGenericCellOperations);

 
                foreach (var cellDesc in cellDescs)
                {
                    CellTypeIDs[cellDesc.TypeName] = _currentCellTypeOffset;

#if DEBUG
                    Console.WriteLine($"{cellDesc.TypeName}{{");

                    foreach(var fieldDesc in cellDesc.GetFieldDescriptors())
                    {    
                        Console.WriteLine($"    {fieldDesc.Name}: {fieldDesc.TypeName}");
                    }
                    Console.WriteLine("}");
#endif
                    ++_currentCellTypeOffset;
                }
                IDIntervals.Add(_currentCellTypeOffset);


                _asmSlides.Add(asm);
                /// TODO: get cell_type count by `asm`.
                CurrentVersion.Asm = asm;
                AsmVersions.Add(CurrentVersion);
                CurrentVersion = null;
            }
            catch (Exception e)
            {
                throw new AsmLoadError(e.Message);
            }

        }

        public class IntervalSearch 
        {
            int CellId;
            public IntervalSearch(int cellID)
            {
                CellId = cellID;
            }
            public int Call() => Apply(0, IDIntervals.Count);
            public int Apply(int x, int y)
            {   // return the index of interval that the cellID lies in `IDIntervals`.
                throw new NotImplementedException();
               
            }
        }
    }
}
