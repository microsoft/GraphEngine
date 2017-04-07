using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trinity;
using Trinity.Diagnostics;
using Trinity.Storage;

namespace GraphEngine.DataImporter
{
    public class Importer
    {
        private class ReverseEdgeFieldDescriptor
        {
            internal string SourceCellType;
            internal string TargetCellType;
            internal string SourceFieldName;
            internal string TargetFieldName;
        }


        #region Fields
        internal static Dictionary<string, ICellDescriptor> s_cellTypes = null;
        internal static int s_current_batch_cnt = 0;
        internal static CmdOptions g_opts;
        #endregion
        internal static void InitlalizeSchema(string tslAssembly)
        {
            Global.LoadTSLStorageExtension(tslAssembly, ignoreNonIncrementalStorageSchemaChanges: true);
            s_cellTypes = new Dictionary<string, ICellDescriptor>();
            foreach (var cd in Global.StorageSchema.CellDescriptors)
            {
                s_cellTypes[cd.TypeName] = cd;
            }
        }

        internal static void Import(string tslAssembly, IEnumerable<string> files, CmdOptions options)
        {
            Log.WriteLine("Importing data files.");
            if (options.Output != null)
            {
                TrinityConfig.StorageRoot = options.Output;
            }

            Global.LocalStorage.ResetStorage();
            g_opts = options;
            InitlalizeSchema(tslAssembly);

            foreach (var file in files)
            {
                ImportFile(file, options);
            }

            ImportReverseEdges();

            Global.LocalStorage.SaveStorage();
        }

        private static void ImportReverseEdges()
        {
            Log.WriteLine("Importing reverse edges.");
            List<ReverseEdgeFieldDescriptor> rev_descs = new List<ReverseEdgeFieldDescriptor>();
            foreach (var cd in Global.StorageSchema.CellDescriptors)
            {
                try
                {
                    var reverse_edge_fields_descs = cd.GetFieldDescriptors()
                        .Where(fd => fd.GetAttributeValue(Consts.c_KW_ReverseEdgeImport) != null)
                        .Select(fd =>
                        {
                            string rev_edge_src_str = fd.GetAttributeValue(Consts.c_KW_ReverseEdgeImport);
                            string[] arr = rev_edge_src_str.Split('.');
                            if (arr.Length != 2 || string.IsNullOrEmpty(arr[0]) || string.IsNullOrEmpty(arr[1])) throw new ImporterException("Invalid {0} parameter: {1}", Consts.c_KW_ReverseEdgeImport, rev_edge_src_str);
                            return new ReverseEdgeFieldDescriptor
                            {
                                SourceCellType = arr[0],
                                SourceFieldName = arr[1],
                                TargetCellType = cd.TypeName,
                                TargetFieldName = fd.Name
                            };
                        })
                        .GroupBy(rfd => rfd.SourceCellType);

                    foreach (var rfd_group in reverse_edge_fields_descs)
                    {
                        ImportReverseEdges(rfd_group);
                    }
                }
                catch (Exception e)
                {
                    Log.WriteLine(LogLevel.Error, "An error occured during import: \n{0}\n", e.Message);
                }
            }
        }

        private static void ImportReverseEdges(IGrouping<string, ReverseEdgeFieldDescriptor> rfd_group)
        {
            ushort src_type_code = Global.StorageSchema.GetCellType(rfd_group.Key);
            List<long> cell_ids = Global.LocalStorage
                .Where(cell => cell.CellType == src_type_code)
                .Select(cell => cell.CellId).ToList();
            object save_cell_lock = new object();

            Parallel.ForEach(cell_ids,
#if DEBUG
                new ParallelOptions { MaxDegreeOfParallelism = 1 },
#endif

 cellid =>
 {
     var cell = Global.LocalStorage.LoadGenericCell(cellid);
     foreach (var rfd in rfd_group)
     {
         foreach (var target_id in cell.EnumerateField<long>(rfd.SourceFieldName))
         {
             begin:
             try
             {
                 ICell remote_cell = Global.LocalStorage.UseGenericCell(target_id, Trinity.TSL.Lib.CellAccessOptions.ReturnNullOnCellNotFound);
                 bool save_otherwise_dispose = false;
                 if (remote_cell == null)
                 {
                     save_otherwise_dispose = true;
                     remote_cell = Global.LocalStorage.NewGenericCell(rfd.TargetCellType);
                     remote_cell.CellID = target_id;
                 }

                 var fd = remote_cell.GetFieldDescriptors().First(_ => _.Name == rfd.TargetFieldName);
                 if (fd.IsOfType<long>() || fd.IsOfType<string>())
                 {
                     remote_cell.AppendToField(rfd.TargetFieldName, cell.CellID);
                 }
                 else if (fd.IsOfType<int>())
                 {
                     remote_cell.AppendToField(rfd.TargetFieldName, (int)cell.CellID);
                 }
                 else
                 {
                     throw new Exception("Unsupported reverse edge import type: " + fd.TypeName);
                 }

                 if (save_otherwise_dispose)
                 {
                     lock (save_cell_lock)
                     {
                         if (Global.LocalStorage.Contains(target_id))
                         {
                             goto begin;
                         }

                         Global.LocalStorage.SaveGenericCell(remote_cell);
                     }

                 }
                 else
                 {
                     (remote_cell as ICellAccessor).Dispose();
                 }
             }
             catch (Exception e)
             {
                 Log.WriteLine(LogLevel.Warning, e.Message);
             }
         }
     }
 });
        }

        private static void ImportFile(string file, CmdOptions options)
        {
            Log.WriteLine("Importing {0}", file);
            string typename = Path.GetFileNameWithoutExtension(file);
            IImporter importer = GetImporter(file, options);

            if (importer == null)
            {
                Log.WriteLine("Unrecognized file type: {0}", typename);
                return;
            }

            if (!importer.IgnoreType() && Global.StorageSchema.CellDescriptors.FirstOrDefault(cd => cd.TypeName == typename) == null)
            {
                Log.WriteLine("File {0} does not match any types defined in the storage extension.", file);
                return;
            }

            var importer_type = importer.GetType();
            var importer_interface_type = importer_type.FindInterfaces((t, _) =>
            {
                return t.Name == "IImporter`1" && t.IsGenericType;
            }, null).First();
            var importer_entry_type = importer_interface_type.GetGenericArguments()[0];
            var importer_method = typeof(Importer).GetMethod("_Import", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).MakeGenericMethod(importer_entry_type);
            importer_method.Invoke(null, new object[] { importer, file, typename });
        }

        private static void _Import<T>(IImporter<T> importer, string file, string typename)
        {
            IEnumerable<T> preprocessed_lines = importer.PreprocessInput(FileReader.ReadFile(file));

            Parallel.ForEach(preprocessed_lines,
#if DEBUG
                new ParallelOptions { MaxDegreeOfParallelism = 1 },
#else
                new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount * 2 },
#endif
             line =>
             {
                 try
                 {
                     importer.ImportEntity(typename, line, null);
                 }
                 catch (Exception e)
                 {
                     Log.WriteLine(LogLevel.Error, "An error occured during import: \n{0}\n", e.Message);
                 }
             });
        }

        private static IImporter GetImporter(string path, CmdOptions options)
        {
            const char comma = ',';
            const char tab = '\t';
            string filename, filetype;
            bool delimiterSpecify = (options.Delimiter == '\0') ? false : true;

            if (options.FileFormat != null)
            {
                filetype = options.FileFormat.StartsWith(".") ? options.FileFormat : "." + options.FileFormat;
            }
            else
            {
                filename = Path.GetFileName(path);
                filetype = Path.GetExtension(filename).ToLower();

                if (filetype == ".gz" || filetype == ".zip")
                {
                    filetype = Path.GetExtension(Path.GetFileNameWithoutExtension(filename)).ToLower();
                }
            }

            switch (filetype)
            {
                case ".json":
                    return new JsonImporter();
                case ".csv":
                    return new CsvImporter(delimiterSpecify ? options.Delimiter : comma);
                case ".tsv":
                    return new CsvImporter(delimiterSpecify ? options.Delimiter : tab);
                case ".ntriples":
                    return g_opts.Sorted ? (IImporter)new UnsortedRDFImporter() : new SortedRDFImporter();
                default:
                    {
                        return null;
                    }
            }
        }
    }
}
