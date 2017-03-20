using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphEngine.DataImporter
{
    public class CsvParser
    {
        public List<string> CsvSplit(string line, char delimiter)
        {
            if (line == null)
                throw new ArgumentNullException();
            List<string> fields = new List<string>();
            int beginIndex = 0;
            int curIndex = 0;
            int cDoubleQuotes = 0;
            for (; curIndex < line.Length; curIndex++)
            {
                char c = line[curIndex];
                if (beginIndex == curIndex && c == delimiter)
                {
                    fields.Add(null);
                    beginIndex = curIndex + 1;
                    cDoubleQuotes = 0;
                }
                else if (c == delimiter && cDoubleQuotes % 2 == 0)
                {
                    fields.Add(SanitizeCsvField(line.Substring(beginIndex, curIndex - beginIndex)));
                    beginIndex = curIndex + 1;
                    cDoubleQuotes = 0;
                }
                else if (c == '"')
                {
                    cDoubleQuotes++;
                    if (cDoubleQuotes == 1 && curIndex != beginIndex)
                    {
                        throw new ImporterException("Unexpected double-quote at position {0} of {1}", curIndex, line);
                    }
                }
            }
            if (beginIndex < curIndex)
            {
                fields.Add(SanitizeCsvField(line.Substring(beginIndex, curIndex - beginIndex)));
            }
            if (beginIndex == curIndex && beginIndex == line.Length)
            {
                fields.Add(null);
            }
            return fields;
        }

        public string SanitizeCsvField(string field, bool trim = false, bool treatEmptyAsNull = true)
        {
            string quotation = "\"", escape = "\"\"";
            if (field == null)
                return null;

            string sanitized = trim ? field.Trim() : field;
            if (sanitized == "" && treatEmptyAsNull)
                return null;
            if (sanitized.StartsWith(quotation))
            {
                if (sanitized.EndsWith(quotation) && sanitized.Length > 1)
                {
                    sanitized = sanitized.Substring(1, sanitized.Length - 2);
                    if (sanitized.Count(c => c == '\"') % 2 != 0)
                    {
                        throw new ImporterException("Field is not properly quoted: {0}", field);
                    }
                    if (sanitized.Count(c => c == '\"') != sanitized.Replace(escape, quotation).Count(c => c == '\"') * 2)
                    {
                        throw new ImporterException("Field is not properly quoted: {0}", field);
                    }
                    sanitized = sanitized.Replace(escape, quotation);
                }
                else
                {
                    throw new ImporterException("Field is not properly quoted: {0}", field);
                }
            }
            return sanitized;
        }
    }
}
