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
        private const char DefaultQuote = '"';
        private const char DefaultEscape = '"';

        public CsvParser(char delimiter)
        {
            this.delimiter = delimiter;
        }

        public List<string> CsvSplit(string line)
        {
            if (line == null)
                throw new ArgumentNullException();

            List<string> fields = new List<string>();
            string processedLine = line + delimiter;
            int beginIndex = NextNotSpaceCharIndex(line, 0);
            int curIndex = beginIndex;
            int countQuoteEscape = 0;// The count of quote and escape.

            for (; curIndex < processedLine.Length; curIndex++)
            {
                char c = processedLine[curIndex];
                if (beginIndex == curIndex && c == delimiter)
                {
                    fields.Add(null);
                    beginIndex = NextNotSpaceCharIndex(processedLine, curIndex + 1);
                    curIndex = beginIndex - 1;
                }
                else if (c == delimiter && countQuoteEscape % 2 == 0)
                {
                    if (countQuoteEscape == 0)
                        fields.Add(processedLine.Substring(beginIndex, curIndex - beginIndex).Trim());
                    else
                        fields.Add(SanitizeCsvField(processedLine.Substring(beginIndex, curIndex - beginIndex), true));

                    beginIndex = NextNotSpaceCharIndex(processedLine, curIndex + 1);
                    curIndex = beginIndex - 1;
                    countQuoteEscape = 0;
                }
                else if (c == DefaultEscape && countQuoteEscape > 0 && processedLine[curIndex + 1] == DefaultQuote)
                {
                    countQuoteEscape++;
                }
                else if (c == DefaultQuote)
                {
                    countQuoteEscape++;

                    if (countQuoteEscape == 1 && curIndex != beginIndex)
                    {
                        throw new ImporterException("Unexpected double-quote at position {0} of {1}", curIndex, line);
                    }
                    else if (countQuoteEscape % 2 == 0 && processedLine[NextNotSpaceCharIndex(processedLine, curIndex + 1)] != delimiter)
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
            sanitized = sanitized.Replace($"{ DefaultEscape }{ DefaultQuote }", $"{ DefaultQuote }");
            return sanitized;
        }

        public int NextNotSpaceCharIndex(string line, int index)
        {
            if (index >= line.Length)
                return line.Length;

            while (index < line.Length && line[index] == ' ')
            {
                index++;
            }

            return index;
        }
    }
}