using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphEngine.DataImporter
{
    public class CsvParser
    {
        private char delimiter;
        string quotation = "\"", escape = "\"";

        public CsvParser(char delimiter)
        {
            this.delimiter = delimiter;
        }

        public List<string> CsvSplit(string line)
        {
            if (line == null)
                throw new ArgumentNullException();

            List<string> fields = new List<string>();
            int beginIndex = 0;
            int curIndex = 0;
            int cDoubleQuotes = 0;
            string processedLine = line + delimiter;

            for (; curIndex < processedLine.Length; curIndex++)
            {
                char c = processedLine[curIndex];
                if (beginIndex == curIndex && c == delimiter)
                {
                    fields.Add(null);
                    beginIndex = curIndex + 1;
                }
                else if (c == delimiter && cDoubleQuotes % 2 == 0)
                {
                    if (cDoubleQuotes == 0)
                        fields.Add(processedLine.Substring(beginIndex, curIndex - beginIndex));
                    else
                        fields.Add(SanitizeCsvField(processedLine.Substring(beginIndex, curIndex - beginIndex)));

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
                    else if (cDoubleQuotes % 2 == 0 && processedLine[curIndex + 1] != '"' && processedLine[curIndex + 1] != delimiter)
                    {
                        throw new ImporterException("Unexpected double-quote at position {0} of {1}", curIndex, line);
                    }
                }
            }

            if (beginIndex != curIndex)
            {
                throw new ImporterException("Unexpected double-quote at position {0} of {1}", beginIndex, line);
            }

            return fields;
        }

        public string SanitizeCsvField(string field, bool trim = false, bool treatEmptyAsNull = true)
        {
            string sanitized = trim ? field.Trim() : field;
            if (sanitized == "" && treatEmptyAsNull)
                return null;

            sanitized = sanitized.Substring(1, sanitized.Length - 2);
            sanitized = sanitized.Replace(escape + quotation, quotation);
            return sanitized;
        }
    }
}
