using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphEngine.DataImporter
{
    using Trinity;
    using static RDFUtils;
    internal class UnsortedRDFTSLGenerator : IGenerator<string>
    {
        public void Scan(string content)
        {
            Triple triple = ParseTriple(content);

            if (triple.Subject == null)
            {
                return;
            }

            var cellId = MID2CellId(triple.Subject);
            string type = GetTypeName(triple.Predicate);
            string property = GetTslName(triple.Predicate);

            int fieldId = TSLGenerator.GetFieldId(property);

            using (var metacell = Global.LocalStorage.UseMetaCell(cellId, Trinity.TSL.Lib.CellAccessOptions.CreateNewOnCellNotFound))
            {
                metacell.TypeId = -1;//mark type id to -1, indicating that this a multi-typed cell TODO constant
                MetaField_Accessor field = metacell.Fields.FirstOrDefault(_ => _.fieldId == fieldId);

                if (field == null)
                {
                    //add new field
                    metacell.Fields.Add(new MetaField
                    {
                        fieldId = fieldId,
                        fieldType = GetFieldType(ref triple),
                        isList = false,
                        typeId = TSLGenerator.GetTypeId(type)
                    });
                }
                else
                {
                    field.isList = true;
                    var newFieldType = GetFieldType(ref triple);
                    field.fieldType = TSLGenerator.UpdateFieldType(field.fieldType, newFieldType);
                }
            }
        }


        public IEnumerable<string> PreprocessInput(IEnumerable<string> input)
        {
            //No preprocessing
            return input;
        }

        public void SetType(string type)
        {
            //Ignore the type
            return;
        }
    }
}