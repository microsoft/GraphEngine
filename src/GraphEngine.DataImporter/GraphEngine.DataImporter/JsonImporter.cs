using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Trinity;
using Trinity.Core.Lib;
using Trinity.Diagnostics;
using Trinity.Storage;

namespace GraphEngine.DataImporter
{
    class JsonImporter : IImporter<string>
    {
        public ICell ImportEntity(string type, string line, long? parent_id)
        {
            ICell           cell       = Global.LocalStorage.NewGenericCell(type);
            JObject         jobj       = JObject.Parse(line);
            ICellDescriptor cell_desc  = Importer.s_cellTypes[type];
            long?           cellid     = null;

            List<Action> tree_imports  = new List<Action>();

            foreach (var fd in cell_desc.GetFieldDescriptors())
            {
                string field_name     = fd.Name;
                JToken field_content  = null;
                bool   field_exist    = jobj.TryGetValue(field_name, out field_content);
                string treeimport     = fd.GetAttributeValue(Consts.c_KW_TreeImport);
                bool   optional       = fd.Optional;
                bool   is_parent      = fd.GetAttributeValue(Consts.c_KW_TreeParent) != null;
                bool   is_rev_import  = fd.GetAttributeValue(Consts.c_KW_ReverseEdgeImport) != null;

                if (!field_exist && !is_parent)
                {
                    if (optional || is_rev_import)
                    {
                        continue;
                    }
                    else
                    {
                        throw new ImporterException("Non-optional field {0}.{1} not found on entity: {2}", type, field_name, line);
                    }
                }

                if (fd.GetAttributeValue(Consts.c_KW_HashImport) != null)
                {
                    ProcessCellIdOrCellIdListField(type, cell, fd, field_name, optional, field_content, Hash);
                }
                else if (fd.GetAttributeValue(Consts.c_KW_CellIdKey) != null)
                {
                    cell.SetField(field_name, field_content.ToString());
                    cellid = Hash(field_content, fd);
                }
                else if (treeimport != null)
                {
                    // process this later so we're confident about cell id
                    tree_imports.Add(() =>
                        {
                            ProcessCellIdOrCellIdListField(type, cell, fd, field_name, optional, field_content, (c, _) =>
                            {
                                var sub_cell = ImportEntity(treeimport, c.ToString(), cellid);
                                return sub_cell.CellId;
                            });
                        });
                }
                else if (is_parent)
                {
                    cell.SetField(field_name, parent_id.Value);
                }
                else
                {
                    cell.SetField(field_name, field_content.ToString());
                }
            }

            tree_imports.ForEach(_ => _());

            if (cellid == null) cellid = CellIdFactory.NewCellId();
            cell.CellId = cellid.Value;
            if (parent_id != null)
            {
                var parent_fd = cell_desc.GetFieldDescriptors().Where(fd => fd.GetAttributeValue(Consts.c_KW_TreeParent) != null).First();
            }
            // TODO check for duplicate records.
            // TODO in a distributed setting we cannot save to local storage like this.
            Global.LocalStorage.SaveGenericCell(cell);

            return cell;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long Hash(JToken content, IFieldDescriptor field_desc)
        {
            //TODO conflict resolve
            string str;
            string hashimport = field_desc.GetAttributeValue(Consts.c_KW_HashImport);
            string cellidkey = field_desc.GetAttributeValue(Consts.c_KW_CellIdKey);

            if ((cellidkey != null && field_desc.Type == Consts.c_Type_Guid) ||
                (hashimport == Consts.c_KW_Guid))
            {
                str = ((Guid)content).ToString();
            }
            else if ((cellidkey != null && field_desc.Type == Consts.c_Type_DateTime) ||
                (hashimport == Consts.c_KW_DateTime))
            {
                str = ((DateTime)content).ToString("s");//XXX issue #226
            }
            else
            {
                str = content.ToString();
            }
            //TODO what if we're hashing a whole struct to cell id? how do we normalize?

            return Trinity.Core.Lib.HashHelper.HashString2Int64(str);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ProcessCellIdOrCellIdListField<T>(string type, ICell cell, IFieldDescriptor fd, string field_name, bool optional, JToken field_content, Func<JToken, IFieldDescriptor, T> process_content_action)
        {
            if (fd.Type == Consts.c_TYPE_CellId || fd.Type == Consts.c_TYPE_OptionalCellId)
            {
                try
                {
                    cell.SetField(field_name, process_content_action(field_content, fd));
                }
                catch (Exception e)
                {
                    if (!optional) throw;
                    else Log.WriteLine(LogLevel.Warning, e.Message);
                }
            }
            else if (fd.Type == Consts.c_TYPE_CellIdList)
            {
                //cell id list
                if (field_content is JArray)
                {
                    foreach (var sub_content in field_content)
                    {
                        try
                        {
                            cell.AppendToField(field_name, process_content_action(sub_content, fd));
                        }
                        catch (Exception e)
                        {
                            Log.WriteLine(LogLevel.Warning, e.Message);
                        }
                    }
                }
                else
                {
                    try
                    {
                        cell.AppendToField(field_name, process_content_action(field_content, fd));
                    }
                    catch (Exception e)
                    {
                        Log.WriteLine(LogLevel.Warning, e.Message);
                    }
                }
            }
            else
            {
                //not supported
                throw new ImporterException("{0}.{1} has type {2}, expecting CellId or List<CellId>", type, field_name, fd.Type.ToString());
            }
        }


        public IEnumerable<string> PreprocessInput(IEnumerable<string> input)
        {
            //Json reader does not preprocess.
            return input;
        }


        public bool IgnoreType()
        {
            return false;
        }
    }
}
