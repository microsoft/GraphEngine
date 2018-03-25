using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Trinity;
using Trinity.Core.Lib;
using Trinity.Storage;

namespace GraphEngine.DataImporter
{
    using static RDFUtils;
    class SortedRDFImporter : IImporter<List<string>>
    {
        // type and parent_id are ignored
        public Trinity.Storage.ICell ImportEntity(string _type_unused, List<string> entity, long? _parent_id_unused = null)
        {
            var triples = entity.Select(ParseTriple).Where(_ => _.Subject != null).GroupBy(_ => GetTypeName(_.Predicate)).ToList();

            if (triples.Count == 0) return null;
            var cellId = MID2CellId(triples[0].First().Subject);

            var type_cell = Global.LocalStorage.NewGenericCell(cellId, "type_object");
            type_cell.SetField("types", new List<ushort>());
            foreach (var triple_group in triples)
            {
                /* each triple_group is a type */
                string type = triple_group.Key;
                ushort curType = Global.StorageSchema.GetCellType(type);
                if (!type_cell.EnumerateField<ushort>("types").Contains(curType))
                {
                    /* type cell initialization */
                    type_cell.AppendToField("types", curType);
                    type_cell.SetField("mid", triple_group.First().Subject);
                }

                if (type == "type_object")
                {
                    foreach (var triple in triple_group)
                    {
                        string property = GetTslName(triple.Predicate);
                        if (property == "type_object_type")
                        {
                            //type_cell.AppendToField(property, (int)curType);
                        }
                        else
                        {
                            type_cell.AppendToField(property, triple.Object);
                        }
                    }
                }
                else
                {
                    /* type is not type_object */
                    var entity_cell = Global.LocalStorage.NewGenericCell(RootCellIdToTypedCellId(cellId, curType), type);
                    foreach (var triple in triple_group)
                    {
                        string property = GetTslName(triple.Predicate);
                        entity_cell.AppendToField(property, triple.Object);
                    }
                    Global.LocalStorage.SaveGenericCell(entity_cell);
                }
            }

            Global.LocalStorage.SaveGenericCell(type_cell);
            return null;
        }

        public IEnumerable<List<string>> PreprocessInput(IEnumerable<string> input)
        {
            return GroupSortedLinesBySubject(input);
        }

        public bool IgnoreType()
        {
            return true;
        }
    }
}

/*
-tD:\home\src\Trinity\Projects\Modules\GraphEngine.DataImporter\Importer.TSL6\bin\Release\Importer.TSL6.dll "\\msradb000\e$\yatao\freebase\freebase-rdf-latest.ntriples"
-tD:\home\src\Trinity\Projects\Modules\GraphEngine.DataImporter\Importer.TSL6\bin\Release\Importer.TSL6.dll "D:\Freebase\freebase-rdf-latest.ntriples.gz"
-g D:\Freebase\freebase-rdf-latest.ntriples.gz
 */
