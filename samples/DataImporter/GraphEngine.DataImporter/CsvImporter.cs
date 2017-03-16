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
        private char delimiter;

        private JsonImporter m_jsonImporter = new JsonImporter();

        public CsvImporter(char fileDelimiter)
        {
            delimiter = fileDelimiter;
        }
        public ICell ImportEntity(string type, string content, long? parent_id = null)
        {
            int fieldsCount = m_fieldNames.Count;
            var fields = CsvSplit(content, delimiter);
            if (fields.Count != fieldsCount)
            {
                throw new ImporterException("Invalid record. The number of a field ({0}) must equal to {1}: {2}",
                    fields.Count, fieldsCount, content);
            }
            var jsonObj = new JObject();
            var fieldDescDict = GetFieldDescriptors(type);
            IFieldDescriptor fieldDesc = null;
            for (int colIndex = 0; colIndex < fieldsCount; colIndex++)
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

            m_fieldNames = new List<string>(headerRow.Split(new char[] { delimiter } , StringSplitOptions.RemoveEmptyEntries));
            
            return input.Skip(1);
        }

        private static Dictionary<string, IFieldDescriptor> GetFieldDescriptors(string type)
        {
            ICellDescriptor cellDesc = Importer.s_cellTypes[type];
            return cellDesc.GetFieldDescriptors().ToDictionary(_ => _.Name);
        }

        private static List<string> CsvSplit(string line, char delimiter)
        {
            if (line == null)
                throw new ArgumentNullException();
            List<string> fields = new List<string>();
            int beginIndex = 0;
            int curIndex = 0;
            int cDoubleQuotes = 0;
            bool beginWithDoubleQuote = true;
            for (; curIndex < line.Length; curIndex++)
            {
                char c = line[curIndex];
                if (beginIndex == curIndex && c == delimiter)
                {
                    fields.Add(null);
                    beginIndex = curIndex + 1;
                    cDoubleQuotes = 0;
                    beginWithDoubleQuote = true;
                }
                else if (c == delimiter && cDoubleQuotes % 2 == 0 && beginWithDoubleQuote && line[curIndex - 1] == '"' && curIndex > 0)
                {
                    fields.Add(SanitizeCsvField(CheckDoubleQuote(line.Substring(beginIndex, curIndex - beginIndex))));
                    beginIndex = curIndex + 1;
                    cDoubleQuotes = 0;
                    beginWithDoubleQuote = true;
                }
                else if (c == delimiter && (!beginWithDoubleQuote || line[curIndex - 1] != '"' || beginIndex + 1 == curIndex || curIndex == 0))
                {
                    fields.Add(CheckDoubleQuote(line.Substring(beginIndex, curIndex - beginIndex)));

                    beginIndex = curIndex + 1;
                    cDoubleQuotes = 0;
                    beginWithDoubleQuote = true;
                }
                else if (c == '"')
                {
                    cDoubleQuotes++;
                    if (cDoubleQuotes == 1 && curIndex != beginIndex)
                    {
                        beginWithDoubleQuote = false;
                        //throw new ImporterException("Unexpected double-quote at position {0} of {1}", curIndex, line);
                    }
                }
            }

            if (beginIndex < curIndex)
            {
                if (!beginWithDoubleQuote || line[curIndex - 1] != '"' || beginIndex + 1 == curIndex)
                {
                    fields.Add(CheckDoubleQuote(line.Substring(beginIndex, curIndex - beginIndex)));
                }
                else
                {
                       fields.Add(SanitizeCsvField(CheckDoubleQuote(line.Substring(beginIndex, curIndex - beginIndex))));
                }
            }
            if (beginIndex == curIndex && beginIndex == line.Length)
            {
                fields.Add(null);
            }
            return fields;
        }

        private static string SanitizeCsvField(string field, bool trim = false, bool treatEmptyAsNull = true)
        {

            if (field == null)
                return null;

            string sanitized = trim ? field.Trim() : field;
            if (sanitized == "" && treatEmptyAsNull)
                return null;
            if (sanitized.StartsWith("\""))
            {
                if (!sanitized.EndsWith("\""))
                    throw new ImporterException("Field is not properly quoted: {0}", field);

                sanitized = sanitized.Substring(1, sanitized.Length - 2);
                if(sanitized.Count(c => c=='\"') % 4 !=0)
                {
                    throw new ImporterException("Field is not properly quoted: {0}", field);
                }
            }
            sanitized = sanitized.Replace("\"\"", "\"");

            return sanitized;
        }
        private static string CheckDoubleQuote(string source)
        {
            if (source.Count(c => c == '"') % 2 == 1)
            {
                throw new ImporterException("Field is not properly quoted: {0}", source);
            }
            return source;
        }

        public bool IgnoreType()
        {
            return false;
        }
    }
}
