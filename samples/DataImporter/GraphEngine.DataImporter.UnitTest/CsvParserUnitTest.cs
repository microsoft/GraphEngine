using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using GraphEngine.DataImporter;

namespace GraphEngine.DataImporter.UnitTest
{
    public class CsvParserUnitTest
    {
        private const char comma = ',';
        private const char tab = '\t';
        CsvParser csvParser = new CsvParser(comma);
        CsvParser tsvParser = new CsvParser(tab);

        [Fact]
        public void Csv_Quote_Process()
        {
            string line = "123,\"aaa\",bbb";
            string validation = "123 aaa bbb";
            string result;
            try
            {
                List<string> list = csvParser.CsvSplit(line);
                result = String.Join(" ", list.ToArray());
            }
            catch (Exception e)
            {
                result = e.Message;
            }
            Assert.Equal(result, validation);
        }

        [Fact]
        public void Csv_CommaInQuote_Process()
        {
            string line = "123,\"I am fine, thank you!\",aaa";
            string validation = "123 I am fine, thank you! aaa";
            string result;
            try
            {
                List<string> list = csvParser.CsvSplit(line);
                result = String.Join(" ", list.ToArray());
            }
            catch (Exception e)
            {
                result = e.Message;
            }
            Assert.Equal(result, validation);
        }

        [Fact]
        public void Csv_Escape_Process()
        {
            string line = "123,\"\"\"thank you,\"\" he said.\",aaa";
            string validation = "123 \"thank you,\" he said. aaa";
            string result;
            try
            {
                List<string> list = csvParser.CsvSplit(line);
                result = String.Join(" ", list.ToArray());
            }
            catch (Exception e)
            {
                result = e.Message;
            }
            Assert.Equal(result, validation);
        }

        [Fact]
        public void Csv_FieldNull_Process()
        {
            string line = ",,,";
            string validation = "   ";
            string result;
            try
            {
                List<string> list = csvParser.CsvSplit(line);
                result = String.Join(" ", list.ToArray());
            }
            catch (Exception e)
            {
                result = e.Message;
            }
            Assert.Equal(result, validation);
        }

        [Fact]
        public void Csv_EscapeError_ThrowException()
        {
            string line = "\"\"\"\"hello\"\"\"";
            string validation = "Unexpected double-quote at position 3 of \"\"\"\"hello\"\"\"";
            string result;
            try
            {
                List<string> list = csvParser.CsvSplit(line);
                result = String.Join(" ", list.ToArray());
            }
            catch (Exception e)
            {
                result = e.Message;
            }
            Assert.Equal(result, validation);
        }

        [Fact]
        public void Csv_QuoteNotAtStartPosition_ThrowException()
        {
            string line = "aaa,b\"bb,ccc";
            string validation = "Unexpected double-quote at position 5 of aaa,b\"bb,ccc";
            string result;
            try
            {
                List<string> list = csvParser.CsvSplit(line);
                result = String.Join(" ", list.ToArray());
            }
            catch (Exception e)
            {
                result = e.Message;
            }
            Assert.Equal(result, validation);
        }

        [Fact]
        public void Csv_NoEndQuote_ThrowException()
        {
            string line = "123,\",123,123";
            string validation = "Unexpected double-quote at position 4 of 123,\",123,123";
            string result;
            try
            {
                List<string> list = csvParser.CsvSplit(line);
                result = String.Join(" ", list.ToArray());
            }
            catch (Exception e)
            {
                result = e.Message;
            }
            Assert.Equal(result, validation);
        }

        [Fact]
        public void Csv_NoEscape_ThrowException()
        {
            string line = "\",123,123,\"aaa\",123";
            string validation = "Unexpected double-quote at position 10 of \",123,123,\"aaa\",123";
            string result;
            try
            {
                List<string> list = csvParser.CsvSplit(line);
                result = String.Join(" ", list.ToArray());
            }
            catch (Exception e)
            {
                result = e.Message;
            }
            Assert.Equal(result, validation);
        }

        [Fact]
        public void Csv_QuoteNotAtEndPosition_ThrowException()
        {
            string line = "123,\"aa\"aa,123";
            string validation = "Unexpected double-quote at position 7 of 123,\"aa\"aa,123";
            string result;
            try
            {
                List<string> list = csvParser.CsvSplit(line);
                result = String.Join(" ", list.ToArray());
            }
            catch (Exception e)
            {
                result = e.Message;
            }
            Assert.Equal(result, validation);
        }

        [Fact]
        public void Tsv_Escape_Quote_Process()
        {
            string line = "aaa	\"\"\"hi\"\"	Jack\"";
            string validation = "aaa \"hi\"	Jack";
            string result;
            try
            {
                List<string> list = tsvParser.CsvSplit(line);
                result = String.Join(" ", list.ToArray());
            }
            catch (Exception e)
            {
                result = e.Message;
            }
            Assert.Equal(result, validation);
        }

        [Fact]
        public void Tsv_NoEndQuote_ThrowException()
        {
            string line = "aaa	\"hi\"\"	\"\"";
            string validation = "Unexpected double-quote at position 4 of aaa	\"hi\"\"	\"\"";
            string result;
            try
            {
                List<string> list = tsvParser.CsvSplit(line);
                result = String.Join(" ", list.ToArray());
            }
            catch (Exception e)
            {
                result = e.Message;
            }
            Assert.Equal(result, validation);

        }
    }
}
