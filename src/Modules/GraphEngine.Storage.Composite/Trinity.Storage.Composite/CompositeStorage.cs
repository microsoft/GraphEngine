using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Trinity.Diagnostics;
using Trinity.Utilities;

namespace Trinity.Storage.Composite
{
    public static class CompositeStorage
    {
        internal static List<IStorageSchema>         s_StorageSchemas;
        internal static List<IGenericCellOperations> s_GenericCellOperations;

        private static Dictionary<string, int>       s_CellTypeIDs;
        private static List<int>                     s_IDIntervals;
        private static List<StorageExtensionRecord>  s_Extensions;
        private static int                           s_currentCellTypeOffset = 0;

        #region State
        public static int CurrentCellTypeOffset => s_currentCellTypeOffset;
        #endregion

        static CompositeStorage()
        {
            s_IDIntervals           = new List<int> { s_currentCellTypeOffset };
            s_StorageSchemas        = new List<IStorageSchema>();
            s_CellTypeIDs           = new Dictionary<string, int>();
            s_Extensions            = new List<StorageExtensionRecord>();
            s_GenericCellOperations = new List<IGenericCellOperations>();
        }

        public static void LoadMetadata()
        {
            Utils.Session(
                path: PathHelper.Directory,
                start: () => Log.WriteLine($"{nameof(CompositeStorage)}: Loading composite storage extension metadata."),
                err: (e) => Log.WriteLine(LogLevel.Error, $"{nameof(CompositeStorage)}: {{0}}", e.Message),
                end: () => Log.WriteLine($"{nameof(CompositeStorage)}: Successfully loaded composite storage extension metadata."),
                behavior: () =>
                {
                    s_Extensions              = Serialization.Deserialize<List<StorageExtensionRecord>>(PathHelper.VersionRecords);
                    s_CellTypeIDs           = Serialization.Deserialize<Dictionary<string, int>>(PathHelper.CellTypeIDs);
                    s_IDIntervals           = Serialization.Deserialize<List<int>>(PathHelper.IDIntervals);
                    var assemblies          = s_Extensions.Select(v => $"{v.ModuleName}.dll")
                                              .Select(PathHelper.DLL)
                                              .Select(Assembly.LoadFrom)
                                              .ToList();
                    s_StorageSchemas        = assemblies.Select(_ => AssemblyUtility.GetAllClassInstances<IStorageSchema>(assembly: _).First()).ToList();
                    s_GenericCellOperations = assemblies.Select(_ => AssemblyUtility.GetAllClassInstances<IGenericCellOperations>(assembly: _).First()).ToList();
                });
        }

        public static void SaveMetadata()
        {
            Utils.Session(
                path: PathHelper.Directory,
                start: () => Log.WriteLine($"{nameof(CompositeStorage)}: Saving composite storage extension metadata."),
                err: (e) => Log.WriteLine(LogLevel.Error, $"{nameof(CompositeStorage)}: {{0}}", e.Message),
                end: () => Log.WriteLine($"{nameof(CompositeStorage)}: Successfully saved composite storage extension metadata."),
                behavior: () =>
                {
                    Serialization.Serialize(s_Extensions, PathHelper.VersionRecords);
                    Serialization.Serialize(s_IDIntervals, PathHelper.IDIntervals);
                    Serialization.Serialize(s_CellTypeIDs, PathHelper.CellTypeIDs);
                }
            );
        }

        public static void ResetMetadata()
        {
            Log.WriteLine($"{nameof(CompositeStorage)}: Resetting composite storage extension metadata.");
            // TODO
        }


        #region TSL-CodeGen-Build-Load
        private static void CreateCSProj(string projDir, string assemblyName)
        {
            var path = Path.Combine(projDir, $"{assemblyName}.csproj");
            File.WriteAllText(path, CSProj.Template);
        }

        private static void CodeGen(string srcDir, string moduleName, int cellTypeOffset, string projDir)
        {
            if (!Commands.TSLCodeGenCmd(
                    string.Join(" ", Directory.GetFiles(srcDir, "*.tsl"))
                    + $" -offset {cellTypeOffset} -n {moduleName} -o {projDir}"))
                throw new TSLCodeGenException();
        }

        private static void Build(string projDir, string outDir)
        {
            if (!Commands.DotNetBuildCmd($"build {projDir} -o {outDir}"))
                throw new TSLBuildException();
        }

        private static Assembly Load(StorageExtensionRecord version)
        {
#if DEBUG
            Console.WriteLine("Loading " + Path.Combine(version.AsmLoadDir, $"{version.Namespace}.dll"));
#endif 
            return Assembly.LoadFrom(Path.Combine(PathHelper.Directory, $"{version.AssemblyName}.dll"));
        }
        #endregion

        public static void UpdateStorageExtensionSchema(SchemaUpdate changes)
        {
            throw new NotImplementedException();
        }

        internal static StorageExtensionRecord AddStorageExtension(string tslSrcDir, string rootNamespace)
        {

#if DEBUG

            Trinity.Global.StorageSchema
                   .CellDescriptors
                   .Select(cellDesc =>
                            $"{cellDesc.TypeName}: " +
                            $"[{cellDesc.GetFieldNames().By(_ => string.Join(",", _))}]")
                   .ToList()
                   .ForEach(_ => Log.WriteLine(_));
#endif
            var assemblyName = Guid.NewGuid().ToString("N");
            var temp         = Path.GetTempPath();
            var projDir      = GetTempDirectory();
            var buildDir     = GetTempDirectory();
            var outDir       = GetTempDirectory();

            var ext = new StorageExtensionRecord(s_currentCellTypeOffset, rootNamespace, assemblyName);

            CreateCSProj(projDir, assemblyName);

            CodeGen(tslSrcDir, rootNamespace, ext.CellTypeOffset, projDir);

            Build(projDir, outDir);

            Load(ext);

            try
            {
                string.Join("\n",
                              "Current Storage Info:",
                              $"#VersionRecorders: {s_Extensions.Count}",
                              $"#IDIntervals: : {s_IDIntervals.Count}",
                              $"#CellTypeIDs:{s_CellTypeIDs.Count}",
                              $"#StorageSchema:{s_StorageSchemas.Count}",
                              $"#GenericCellOperations:{s_GenericCellOperations.Count}")
                       .By(_ => Log.WriteLine(LogLevel.Debug, $"{nameof(CompositeStorage)}: {{0}}", _));

                var asm       = Load(ext);
                var schema    = AssemblyUtility.GetAllClassInstances<IStorageSchema>(assembly: asm).First();
                var cellOps   = AssemblyUtility.GetAllClassInstances<IGenericCellOperations>(assembly: asm).First();
                var cellDescs = schema.CellDescriptors.ToList();

                s_StorageSchemas.Add(schema);
                s_GenericCellOperations.Add(cellOps);

                int maxoffset = s_currentCellTypeOffset;

                foreach (var cellDesc in cellDescs)
                {
                    s_CellTypeIDs[cellDesc.TypeName] = cellDesc.CellType;
                    if (s_currentCellTypeOffset > cellDesc.CellType) throw new InvalidOperationException("New cell type id conflicts existing type space");
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

                s_currentCellTypeOffset += cellDescs.Count + 1;

                if (s_currentCellTypeOffset != maxoffset + 1) throw new InvalidOperationException("The whole type id space is not compact");

                s_IDIntervals.Add(s_currentCellTypeOffset);
                s_Extensions.Add(ext);
                if (!s_IDIntervals.OrderBy(_ => _).SequenceEqual(s_IDIntervals)) throw new InvalidOperationException("intervals do not grow monotonically");

                return ext;
            }
            catch (Exception e)
            {
                //TODO rollback
                throw new AsmLoadException(e.Message);
            }
        }

        private static string GetTempDirectory()
        {
            var tmpDir = Path.GetTempFileName();
            File.Delete(tmpDir);
            Directory.CreateDirectory(tmpDir);
            return tmpDir;
        }

        public static int GetIntervalIndexByCellTypeID(int cellTypeID)
        {
            int seg = s_IDIntervals.FindLastIndex(seg_head => seg_head < cellTypeID);
            if (seg == -1 || seg == s_IDIntervals.Count)
                throw new CellTypeNotMatchException("Cell type id out of the valid range.");
            return seg;
        }

        public static int GetIntervalIndexByCellTypeName(string cellTypeName)
        {
            if (!s_CellTypeIDs.TryGetValue(cellTypeName, out var typeId))
                throw new CellTypeNotMatchException("Unrecognized cell type string.");
            return GetIntervalIndexByCellTypeID(typeId);
        }
    }
}
