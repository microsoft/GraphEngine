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
        private bool trim;

        public CsvParser(char delimiter, bool trim)
        {
            this.delimiter = delimiter;
            this.trim = trim;
        }

        public List<string> CsvSplit(string line)
        {
            if (line == null)
                throw new ArgumentNullException();

            List<string> fields = new List<string>();
            string processedLine = line + delimiter;
            int beginIndex = NextCharIndex(line, 0, trim);
            int curIndex = beginIndex;
            int countQuoteEscape = 0;// The count of quote and escape.

            for (; curIndex < processedLine.Length; curIndex++)
            {
                char c = processedLine[curIndex];
                if (beginIndex == curIndex && c == delimiter)
                {
                    fields.Add(null);
                    beginIndex = NextCharIndex(processedLine, curIndex + 1, trim);
                    curIndex = beginIndex - 1;
                }
                else if (c == delimiter && countQuoteEscape % 2 == 0)
                {
                    fields.Add(SanitizeCsvField(processedLine.Substring(beginIndex, curIndex - beginIndex), countQuoteEscape != 0, trim));
                    beginIndex = NextCharIndex(processedLine, curIndex + 1, trim);
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
                    else if (countQuoteEscape % 2 == 0 && processedLine[NextCharIndex(processedLine, curIndex + 1, trim)] != delimiter)
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

        public string SanitizeCsvField(string field, bool haveQuote, bool trim = false, bool treatEmptyAsNull = true)
        {
            string sanitized = trim ? field.Trim() : field;

            if (sanitized == "" && treatEmptyAsNull)
                return null;

            if (haveQuote)
            {
                sanitized = sanitized.Substring(1, sanitized.Length - 2);
                sanitized = sanitized.Replace($"{ DefaultEscape }{ DefaultQuote }", $"{ DefaultQuote }");
            }
            return sanitized;
        }

        public int NextCharIndex(string line, int index, bool trim)
        {
            if (trim)
            {
                if (index >= line.Length)
                    return line.Length;

                while (index < line.Length && line[index] == ' ')
                {
                    index++;
                }
            }

            return index;
        }
    }
}