using GraphEngine.Utilities;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trinity;
using Trinity.Diagnostics;

namespace GraphEngine.DataImporter
{
    class TSLGenerator
    {
        internal static ConcurrentDictionary<string, int> g_field_idmap = new ConcurrentDictionary<string, int>();
        internal static ConcurrentDictionary<string, int> g_type_idmap = new ConcurrentDictionary<string, int>();
        internal static int g_field_id = 1;
        internal static int g_type_id = 1;
        internal static CmdOptions g_opts;
        internal static string g_output_tsl_name = "generated.tsl";

        internal static int GetFieldId(string property)
        {
            start:
            int id;
            if (!g_field_idmap.TryGetValue(property, out id))
            {
                id = Interlocked.Increment(ref g_field_id);
                if (!g_field_idmap.TryAdd(property, id))
                {
                    goto start;
                }
            }

            return id;
        }

        internal static int GetTypeId(string typename)
        {
            start:
            int id;
            if (!g_type_idmap.TryGetValue(typename, out id))
            {
                id = Interlocked.Increment(ref g_type_id);
                if (!g_type_idmap.TryAdd(typename, id))
                {
                    goto start;
                }
            }

            return id;
        }

        private static string GetTypeName(int typeid)
        {
            return g_type_idmap.Where(kvp => kvp.Value == typeid).First().Key;
        }

        private static string GetFieldName(int fieldId)
        {
            return g_field_idmap.Where(kvp => kvp.Value == fieldId).First().Key;
        }


        internal static void Generate(List<string> files, CmdOptions opts)
        {
            Log.WriteLine("Scanning data files to generate a TSL file.");
            if(opts.Output != null)
            {
                g_output_tsl_name = opts.Output;
            }

            Global.LocalStorage.ResetStorage();
            g_opts = opts;
            g_field_idmap.Clear();
            foreach (var file in files)
            {
                ScanFile(file);
            }
            Log.WriteLine("All files scanned. Aggregating type information.");
            AggregateTypeInformation();
            Log.WriteLine("TSL file generated successfully.");
        }

        internal static void AggregateTypeInformation()
        {
            CodeWriter cw = new CodeWriter();
            cw.UseSpaces = true;

            //Type-designated cells
            //TODO

            //Multi-typed cells
            var aggregated_types = Global.LocalStorage.MetaCell_Selector()
                .AsParallel()
                .Where(_ => _.TypeId == -1)
                .SelectMany(_ => _.Fields)
                .GroupBy(_ => _.fieldId)
                .Select(AggregateFieldInstances)
                .GroupBy(_ => _.typeId)
                .ToList();

            if (aggregated_types.Count != 0)
            {
                aggregated_types.Sort((ta, tb) =>
                {
                    string ta_name = GetTypeName(ta.Key);
                    string tb_name = g_type_idmap.Where(kvp => kvp.Value == tb.Key).First().Key;

                    return ta_name.CompareTo(tb_name);
                });

                var type_object = aggregated_types.First(_ => GetTypeName(_.Key) == "type_object");

                aggregated_types.Remove(type_object);

                cw.S = GenerateCellHead("type_object");
                cw.SL1 = "u8string mid;";
                cw.SL1 = "List<ushort> types;";

                foreach (var field in type_object)
                {
                    if (field.fieldType == FieldType.ft_graphedge)
                    {
                        cw.WL1("[GraphEdge]");
                    }

                    cw.WL1("optional {0} {1};", GetTSLDataType(field.fieldType, field.isList), GetFieldName(field.fieldId));
                }

                cw.SL = "}";

                foreach (var type in aggregated_types)
                {
                    cw.S = GenerateCellHead(GetTypeName(type.Key));

                    foreach (var field in type)
                    {
                        if (field.fieldType == FieldType.ft_graphedge)
                        {
                            cw.WL1("[GraphEdge]");
                        }

                        cw.WL1("optional {0} {1};", GetTSLDataType(field.fieldType, field.isList), GetFieldName(field.fieldId));
                    }

                    cw.SL = "}";
                }
            }

            File.WriteAllText(g_output_tsl_name, cw.ToString());

        }

        private static string GetTSLDataType(FieldType fieldType, bool isList)
        {
            string element = "";
            switch (fieldType)
            {
                case FieldType.ft_byte:
                    element = "byte";
                    break;
                case FieldType.ft_sbyte:
                    element = "sbyte";
                    break;
                case FieldType.ft_short:
                    element = "short";
                    break;
                case FieldType.ft_ushort:
                    element = "ushort";
                    break;
                case FieldType.ft_int:
                    element = "int";
                    break;
                case FieldType.ft_uint:
                    element = "uint";
                    break;
                case FieldType.ft_long:
                    element = "long";
                    break;
                case FieldType.ft_graphedge:
                    element = "CellId";
                    break;
                case FieldType.ft_ulong:
                    element = "ulong";
                    break;
                case FieldType.ft_float:
                    element = "float";
                    break;
                case FieldType.ft_double:
                    element = "double";
                    break;
                case FieldType.ft_decimal:
                    element = "decimal";
                    break;
                case FieldType.ft_bool:
                    element = "bool";
                    break;
                case FieldType.ft_guid:
                    element = "Guid";
                    break;
                case FieldType.ft_datetime:
                    element = "DateTime";
                    break;
                case FieldType.ft_char:
                    element = "char";
                    break;
                case FieldType.ft_string:
                    element = "u8string";
                    break;
            }

            if (isList) return "List<" + element + ">";
            else return element;
        }

        private static string GenerateCellHead(string cellName)
        {
            StringBuilder buffer = new StringBuilder();
            buffer.AppendLine("cell " + cellName);
            buffer.AppendLine("{");
            return buffer.ToString();
        }

        private static MetaField AggregateFieldInstances(IGrouping<int, MetaField> field_instances)
        {
            int fieldId = field_instances.Key;
            MetaField field = field_instances.Aggregate(field_instances.First(), (f, current) =>
            {
                f.isList = f.isList || current.isList;
                f.fieldType = UpdateFieldType(f.fieldType, current.fieldType);
                return f;
            });
            return field;
        }

        internal static FieldType UpdateFieldType(FieldType thistype, FieldType thattype)
        {
            if (ConflictFieldType(thistype, thattype)) return FieldType.ft_string;
            return (FieldType)Math.Max((int)thistype, (int)thattype);
        }

        private static bool ConflictFieldType(FieldType fieldType, FieldType newFieldType)
        {
            if ((int)fieldType > (int)FieldType.ft_decimal) return true;
            if ((int)newFieldType > (int)FieldType.ft_decimal) return true;

            return false;
        }

        private static void ScanFile(string file)
        {
            Log.WriteLine("Scanning file {0}", file);
            string typename = Path.GetFileNameWithoutExtension(file);
            object generator = GetGenerator(file);

            var generator_type = generator.GetType();
            var generator_interface_type = generator_type.FindInterfaces((t, _) =>
            {
                return t.Name == "IGenerator`1" && t.IsGenericType;
            }, null).First();
            var generator_entry_type = generator_interface_type.GetGenericArguments()[0];
            var generator_method = typeof(TSLGenerator).GetMethod("_Scan", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).MakeGenericMethod(generator_entry_type);
            generator_method.Invoke(null, new object[] { generator, file, typename });
        }

        private static void _Scan<T>(IGenerator<T> generator, string file, string typename)
        {
            generator.SetType(typename);
            IEnumerable<T> preprocessed_lines = generator.PreprocessInput(FileReader.ReadFile(file));

            Parallel.ForEach(preprocessed_lines,
#if DEBUG
                new ParallelOptions{MaxDegreeOfParallelism = 1},
#else
                new ParallelOptions{MaxDegreeOfParallelism = Environment.ProcessorCount * 2},
#endif
             line =>
             {
                 try
                 {
                     generator.Scan(line);
                 }
                 catch (Exception e)
                 {
                     //Log.WriteLine(LogLevel.Error, "An error occured during import: \n{0}\n", e.Message);
                 }
             });
        }

        private static object GetGenerator(string file)
        {
            string filename = Path.GetFileName(file);
            string filetype = Path.GetExtension(filename).ToLower();

            if (filetype == ".gz" || filetype == ".zip")
            {
                filetype = Path.GetExtension(Path.GetFileNameWithoutExtension(filename)).ToLower();
            }

            switch (filetype)
            {
                case ".json":
                case ".txt":
                    throw new NotImplementedException();
                case ".csv":
                case ".tsv":
                    throw new NotImplementedException();
                case ".ntriples":
                    return g_opts.Sorted ? (object)new SortedRDFTSLGenerator() : new UnsortedRDFTSLGenerator();
                default:
                    return null;
            }
        }
    }
}
