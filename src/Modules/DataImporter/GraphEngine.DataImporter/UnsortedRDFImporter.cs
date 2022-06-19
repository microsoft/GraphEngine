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
    class UnsortedRDFImporter : IImporter<string>
    {
        // type and parent_id are ignored
        public Trinity.Storage.ICell ImportEntity(string _type_unused, string line, long? _parent_id_unused = null)
        {
            Triple triple = ParseTriple(line);

            if (triple.Subject == null)
            {
                return null;
            }

            var cellId = MID2CellId(triple.Subject);
            string type = GetTypeName(triple.Predicate);
            string property = GetTslName(triple.Predicate);
            ushort curType = Global.StorageSchema.GetCellType(type);

            using (var type_cell = Global.LocalStorage.UseGenericCell(cellId, Trinity.TSL.Lib.CellAccessOptions.CreateNewOnCellNotFound, "type_object"))
            {
                if (!type_cell.EnumerateField<ushort>("types").Contains(curType))
                {
                    /* type cell initialization */
                    type_cell.AppendToField("types", curType);
                    type_cell.SetField("mid", triple.Subject);
                }

                if (type == "type_object")
                {
                    if (property == "type_object_type")
                    {
                        //type_cell.AppendToField(property, (int)curType);
                    }
                    else
                    {
                        type_cell.AppendToField(property, triple.Object);
                    }
                    return null;
                }
            }

            /* type is not type_object */
            using (var entity_cell = Global.LocalStorage.UseGenericCell(RootCellIdToTypedCellId(cellId, curType), Trinity.TSL.Lib.CellAccessOptions.CreateNewOnCellNotFound, type))
            {
                entity_cell.AppendToField(property, triple.Object);
            }

            return null;
        }

        public IEnumerable<string> PreprocessInput(IEnumerable<string> input)
        {
            return input;
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
