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
            string[] sarray = { "123", "aaa", "bbb" };
            List<string> validation = new List<string>(sarray);
            List<string> result = null;
            Exception exception = null;
            try
            {
                result = csvParser.CsvSplit(line);
            }
            catch (Exception e)
            {
                exception = e;
            }
            Assert.True(result.SequenceEqual(validation));
        }

        [Fact]
        public void Csv_CommaInQuote_Process()
        {
            string line = "123,\"I am fine, thank you!\",aaa";
            string[] sarray = { "123", "I am fine, thank you!", "aaa" };
            List<string> validation = new List<string>(sarray);
            List<string> result = null;
            Exception exception = null;
            try
            {
                result = csvParser.CsvSplit(line);
            }
            catch (Exception e)
            {
                exception = e;
            }
            Assert.True(result.SequenceEqual(validation));
        }

        [Fact]
        public void Csv_Escape_Process()
        {
            string line = "123,\"\"\"thank you,\"\" he said.\",aaa";
            string[] sarray = { "123", "\"thank you,\" he said.", "aaa" };
            List<string> validation = new List<string>(sarray);
            List<string> result = null;
            Exception exception = null;
            try
            {
                result = csvParser.CsvSplit(line);
            }
            catch (Exception e)
            {
                exception = e;
            }
            Assert.True(result.SequenceEqual(validation));
        }

        [Fact]
        public void Csv_FieldNull_Process()
        {
            string line = ",,,";
            string[] sarray = { null, null, null, null };
            List<string> validation = new List<string>(sarray);
            List<string> result = null;
            Exception exception = null;
            try
            {
                result = csvParser.CsvSplit(line);
            }
            catch (Exception e)
            {
                exception = e;
            }
            Assert.True(result.SequenceEqual(validation));
        }

        [Fact]
        public void Csv_EscapeError_ThrowException()
        {
            string line = "\"\"\"\"hello\"\"\"";
            List<string> result = null;
            Exception exception = null;
            try
            {
                result = csvParser.CsvSplit(line);
            }
            catch (Exception e)
            {
                exception = e;
            }
            Assert.NotNull(exception);
        }

        [Fact]
        public void Csv_QuoteNotAtStartPosition_ThrowException()
        {
            string line = "aaa,b\"bb,ccc";
            List<string> result = null;
            Exception exception = null;
            try
            {
                result = csvParser.CsvSplit(line);
            }
            catch (Exception e)
            {
                exception = e;
            }
            Assert.NotNull(exception);
        }

        [Fact]
        public void Csv_NoEndQuote_ThrowException()
        {
            string line = "123,\",123,123";
            List<string> result = null;
            Exception exception = null;
            try
            {
                result = csvParser.CsvSplit(line);
            }
            catch (Exception e)
            {
                exception = e;
            }
            Assert.NotNull(exception);
        }

        [Fact]
        public void Csv_NoEscape_ThrowException()
        {
            string line = "\",123,123,\"aaa\",123";
            List<string> result = null;
            Exception exception = null;
            try
            {
                result = csvParser.CsvSplit(line);
            }
            catch (Exception e)
            {
                exception = e;
            }
            Assert.NotNull(exception);
        }

        [Fact]
        public void Csv_QuoteNotAtEndPosition_ThrowException()
        {
            string line = "123,\"aa\"aa,123";
            List<string> result = null;
            Exception exception = null;
            try
            {
                result = csvParser.CsvSplit(line);
            }
            catch (Exception e)
            {
                exception = e;
            }
            Assert.NotNull(exception);
        }

        [Fact]
        public void Tsv_Escape_Quote_Process()
        {
            string line = "aaa	\"\"\"hi\"\"	Jack\"";
            string[] sarray = { "aaa", "\"hi\"\tJack" };
            List<string> validation = new List<string>(sarray);
            List<string> result = null;
            Exception exception = null;
            try
            {
                result = tsvParser.CsvSplit(line);
            }
            catch (Exception e)
            {
                exception = e;
            }
            Assert.True(result.SequenceEqual(validation));
        }

        [Fact]
        public void Tsv_NoEndQuote_ThrowException()
        {
            string line = "aaa	\"hi\"\"	\"\"";
            List<string> result = null;
            Exception exception = null;
            try
            {
                result = tsvParser.CsvSplit(line);
            }
            catch (Exception e)
            {
                exception = e;
            }
            Assert.NotNull(exception);
        }
    }
}
