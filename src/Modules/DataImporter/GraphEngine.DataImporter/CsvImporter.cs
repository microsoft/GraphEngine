using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Trinity.Storage;
namespace GraphEngine.DataImporter
{
    class CsvImporter : IImporter<string>
    {
        private List<string> m_fieldNames;
        private JsonImporter m_jsonImporter = new JsonImporter();
        private CsvParser parser;

        public CsvImporter(char delimiter, bool trim)
        {
            parser = new CsvParser(delimiter, trim);   
        }

        public ICell ImportEntity(string type, string content, long? parent_id = null)
        {
            var fields = parser.CsvSplit(content);
            if (fields.Count != m_fieldNames.Count)
            {
                throw new ImporterException("Invalid record. The number of a field ({0}) must equal to {1}: {2}",
                    fields.Count, m_fieldNames.Count, content);
            }

            var jsonObj = new JObject();
            var fieldDescDict = GetFieldDescriptors(type);
            IFieldDescriptor fieldDesc = null;
            for (int colIndex = 0; colIndex < m_fieldNames.Count; colIndex++)
            {
                var fieldName = m_fieldNames[colIndex];
                string fieldValue = fields[colIndex];

                if (fieldValue == null || !fieldDescDict.TryGetValue(fieldName, out fieldDesc))
                {
                    // Ignore this field if it's value is null or could not find the descriptor.
                    continue;
                }

                string csvArraySplitBy = fieldDesc.GetAttributeValue(Consts.c_KW_CsvArray);
                if (fieldValue == null || csvArraySplitBy == null)
                {
                    jsonObj[fieldName] = fieldValue;
                }
                else
                {
                    var elements = fieldValue.Split(csvArraySplitBy.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    var jsonArray = new JArray();
                    foreach (var elem in elements)
                    {
                        jsonArray.Add(elem);
                    }
                    jsonObj[fieldName] = jsonArray;
                }
            }
            return m_jsonImporter.ImportEntity(type, jsonObj.ToString(), parent_id);
        }

        public IEnumerable<string> PreprocessInput(IEnumerable<string> input)
        {
                string headerRow = input.First();
                m_fieldNames = new List<string>(parser.CsvSplit(headerRow));
                return input.Skip(1);
        }

        private static Dictionary<string, IFieldDescriptor> GetFieldDescriptors(string type)
        {
            ICellDescriptor cellDesc = Importer.s_cellTypes[type];
            return cellDesc.GetFieldDescriptors().ToDictionary(_ => _.Name);
        }
        public bool IgnoreType()
        {
            return false;
        }
    }
}
