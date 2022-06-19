using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphEngine.DataImporter
{
    using Trinity;
    using Trinity.Storage;
    using static RDFUtils;
    internal class SortedRDFTSLGenerator : IGenerator<List<string>>
    {
        public void Scan(List<string> entity)
        {
            var type_groups = entity.Select(ParseTriple).Where(_ => _.Subject != null).GroupBy(_ => GetTypeName(_.Predicate)).ToList();

            if (type_groups.Count == 0) return;
            var cellId = MID2CellId(type_groups[0].First().Subject);

            var metacell = new MetaCell(cellId, Fields: new List<MetaField>());
            metacell.TypeId = -1;//mark type id to -1, indicating that this a multi-typed cell TODO constant
            foreach (var type_group in type_groups)
            {
                string type   = type_group.Key;
                int    typeId = TSLGenerator.GetTypeId(type);

                foreach (var property_instances in type_group.GroupBy(_ => GetTslName(_.Predicate)))
                {
                    string    property  = property_instances.Key;
                    int       fieldId   = TSLGenerator.GetFieldId(property);
                    bool      isList    = property_instances.Count() > 1;
                    FieldType fieldType = property_instances.Aggregate(FieldType.ft_byte, (v, current) =>
                    {
                        var current_ft = GetFieldType(ref current);
                        return TSLGenerator.UpdateFieldType(v, current_ft);
                    });
                    MetaField field = new MetaField { fieldId = fieldId, fieldType = fieldType, isList = isList, typeId = typeId };
                    metacell.Fields.Add(field);
                }
            }

            Global.LocalStorage.SaveMetaCell(metacell);
        }

        public IEnumerable<List<string>> PreprocessInput(IEnumerable<string> input)
        {
            return GroupSortedLinesBySubject(input);
        }

        public void SetType(string type)
        {
            //Ignore the type
            return;
        }
    }
}
