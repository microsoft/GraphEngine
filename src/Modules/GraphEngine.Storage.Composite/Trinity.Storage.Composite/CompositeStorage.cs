using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
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
        private static int                           s_currentCellTypeOffset;
        private static object                        s_lock = new object();

        #region State
        public static int CurrentCellTypeOffset => s_currentCellTypeOffset;
        #endregion

        static CompositeStorage() => Init();

        //  !caller shall hold s_lock
        private static void Init()
        {
            s_currentCellTypeOffset = 0;
            s_IDIntervals           = new List<int> { s_currentCellTypeOffset };
            s_CellTypeIDs           = new Dictionary<string, int>();
            s_StorageSchemas        = new List<IStorageSchema>();
            s_GenericCellOperations = new List<IGenericCellOperations>();
            s_Extensions            = new List<StorageExtensionRecord>();
        }

        public static void LoadMetadata()
        {
            lock (s_lock)
                Utils.Session(
                    path: PathHelper.Directory,
                    start: () => Log.WriteLine($"{nameof(CompositeStorage)}: Loading composite storage extension metadata."),
                    err: (e) => Log.WriteLine(LogLevel.Error, $"{nameof(CompositeStorage)}: {{0}}", e.Message),
                    end: () => Log.WriteLine($"{nameof(CompositeStorage)}: Successfully loaded composite storage extension metadata."),
                    behavior: () =>
                    {
                        Init();
                        Serialization.Deserialize<List<StorageExtensionRecord>>(PathHelper.ExtensionRecords)
                                     .Each(Load);
                    });
        }

        public static void SaveMetadata()
        {
            lock (s_lock)
                Utils.Session(
                    path: PathHelper.Directory,
                    start: () => Log.WriteLine($"{nameof(CompositeStorage)}: Saving composite storage extension metadata."),
                    err: (e) => Log.WriteLine(LogLevel.Error, $"{nameof(CompositeStorage)}: {{0}}", e.Message),
                    end: () => Log.WriteLine($"{nameof(CompositeStorage)}: Successfully saved composite storage extension metadata."),
                    behavior: () =>
                    {
                        Serialization.Serialize(s_Extensions, PathHelper.ExtensionRecords);
                    }
                );
        }

        public static void ResetMetadata()
        {
            lock (s_lock)
                Utils.Session(
                    path: PathHelper.Directory,
                    start: () => Log.WriteLine($"{nameof(CompositeStorage)}: Resetting composite storage extension metadata."),
                    err: (e) => Log.WriteLine(LogLevel.Error, $"{nameof(CompositeStorage)}: {{0}}", e.Message),
                    end: () => Log.WriteLine($"{nameof(CompositeStorage)}: Successfully reset composite storage extension metadata."),
                    behavior: () =>
                    {
                        Init();
                        Serialization.Serialize(s_Extensions, PathHelper.ExtensionRecords);
                    }
                );
        }


        #region TSL-CodeGen-Build-Load
        private static void CreateCSProj(string projDir, string assemblyName)
        {
            var path = Path.Combine(projDir, $"{assemblyName}.csproj");
            File.WriteAllText(path, CSProj.Template);
        }

        private static void CodeGen(string srcDir, string rootNamespace, int cellTypeOffset, string projDir)
        {
            string newNamespace = rootNamespace;

            while (s_Extensions.Any(_ => _.RootNamespace == newNamespace))
            {
                Regex update_pattern = new Regex(@".*_rev_(\d+)");
                Match m = update_pattern.Match(newNamespace);
                if (m.Success)
                {
                    var prefix = newNamespace.Substring(0, m.Groups[0].Index);
                    var ver    = int.Parse(newNamespace.Substring(m.Groups[0].Index)) + 1;
                    newNamespace = prefix + ver;
                }
                else
                {
                    newNamespace += "_rev_1";
                }
            }

            if (newNamespace != rootNamespace)
            {
                Log.WriteLine(LogLevel.Warning, $"{nameof(CompositeStorage)}: renaming duplicated extension root namespace: {rootNamespace}->{newNamespace}");
                rootNamespace = newNamespace;
            }

            if (!Commands.TSLCodeGenCmd(
                    string.Join(" ", Directory.GetFiles(srcDir, "*.tsl"))
                    + $" -offset {cellTypeOffset} -n {rootNamespace} -o {projDir}"))
                throw new TSLCodeGenException();
        }

        private static void Build(string projDir, string outDir, string assemblyName)
        {
            if (!Commands.DotNetBuildCmd($"build {projDir} -o {outDir}"))
                throw new TSLBuildException();

            ShadowCopy(outDir, Path.Combine(PathHelper.Directory, assemblyName));
        }

        private static void Load(StorageExtensionRecord ext)
        {
            //  capture states
            var ctoffset = ext.CellTypeOffset;
            var ctmap    = new Dictionary<string, int>(s_CellTypeIDs);
            var gcops    = new List<IGenericCellOperations>(s_GenericCellOperations);
            var schemas  = new List<IStorageSchema>(s_StorageSchemas);
            var iditv    = new List<int>(s_IDIntervals);

            var pshadow  = ShadowCopy(Path.Combine(PathHelper.Directory, ext.AssemblyName));
            var asm      = Assembly.LoadFrom(Path.Combine(pshadow, ext.AssemblyName+".dll"));

            new[] { "Current Storage Info:",
                   $"#Loaded extensions: {s_Extensions.Count}",
                   $"#IDIntervals: : {iditv.Count}",
                   $"#CellTypeIDs:{ctmap.Count}",
                   $"#StorageSchema:{schemas.Count}",
                   $"#GenericCellOperations:{gcops.Count}" }
                   .Each(_ => Log.WriteLine(LogLevel.Debug, $"{nameof(CompositeStorage)}: {{0}}", _));

            var schema    = AssemblyUtility.GetAllClassInstances<IStorageSchema>(assembly: asm).FirstOrDefault();
            var cellOps   = AssemblyUtility.GetAllClassInstances<IGenericCellOperations>(assembly: asm).FirstOrDefault();

            if (schema == null || cellOps == null) throw new AsmLoadException("Not a TSL extension.");

            var cellDescs = schema.CellDescriptors.ToList();

            schemas.Add(schema);
            gcops.Add(cellOps);

            int maxoffset = ctoffset;

            foreach (var cellDesc in cellDescs)
            {
                if (ctmap.TryGetValue(cellDesc.TypeName, out var existing_tid))
                {
                    Log.WriteLine(LogLevel.Info, $"{nameof(CompositeStorage)}: overriding type {cellDesc.TypeName}: TypeId {existing_tid}->{cellDesc.CellType}");
                }

                ctmap[cellDesc.TypeName] = cellDesc.CellType;
                if (cellDesc.CellType < ctoffset) throw new AsmLoadException("New cell type id conflicts existing type space");
                maxoffset = Math.Max(maxoffset, cellDesc.CellType);
            }

            ctoffset += cellDescs.Count + 1; // extra typeid for undefined/reserved cell type
            if (ctoffset != maxoffset + 1) throw new AsmLoadException("The whole type id space is not compact");

            iditv.Add(ctoffset);
            if (!iditv.OrderBy(_ => _).SequenceEqual(iditv)) throw new AsmLoadException("intervals do not grow monotonically");

            //  commit states
            lock (s_lock)
            {
                s_currentCellTypeOffset = ctoffset;
                s_CellTypeIDs           = ctmap;
                s_GenericCellOperations = gcops;
                s_StorageSchemas        = schemas;
                s_IDIntervals           = iditv;
                s_Extensions.Add(ext);
            }
        }
        #endregion

        public static void UpdateStorageExtensionSchema(SchemaUpdate changes)
        {
            throw new NotImplementedException();
        }

        public static StorageExtensionRecord AddStorageExtension(string tslSrcDir, string rootNamespace)
        {
            //  assemblyName serves as a global-unique identifier for a storage extension.
            var assemblyName = Guid.NewGuid().ToString("N");
            var projDir      = GetTempDirectory();
            var buildDir     = GetTempDirectory();
            var outDir       = GetTempDirectory();
            var ext          = new StorageExtensionRecord(s_currentCellTypeOffset, rootNamespace, assemblyName);

            CreateCSProj(projDir, assemblyName);

            CodeGen(tslSrcDir, rootNamespace, ext.CellTypeOffset, projDir);

            Build(projDir, outDir, assemblyName);

            Load(ext);

            SaveMetadata();

            return ext;
        }

        private static string GetTempDirectory()
        {
            var tmpDir = Path.GetTempFileName();
            File.Delete(tmpDir);
            Directory.CreateDirectory(tmpDir);
            return tmpDir;
        }

        private static string ShadowCopy(string pfrom, string pto = null)
        {
            if (pto == null) pto = GetTempDirectory();
            else FileUtility.CompletePath(pto, create_nonexistent: true);

            Directory.EnumerateFiles(pfrom).Each(f => File.Copy(f, Path.Combine(pto, Path.GetFileName(f))));
            Directory.EnumerateDirectories(pfrom).Each(d => ShadowCopy(d, Path.Combine(pto, Path.GetFileName(d))));
            return pto;
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
