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
            string line = "    123     ,     \"aaa\"   ,    1.233    ";

            var expectedTokens = new List<string> { "123", "aaa", "1.233" };
            List<string> result = null;
            result = csvParser.CsvSplit(line);
            Assert.True(result.SequenceEqual(expectedTokens));
        }

        [Fact]
        public void Csv_CommaInQuote_Process()
        {
            string line = "123,\"I am fine,\\thank you!\",aaa";
            var expectedTokens = new List<string> { "123", "I am fine,\\thank you!", "aaa" };
            List<string> result = null;
            result = csvParser.CsvSplit(line);
            Assert.True(result.SequenceEqual(expectedTokens));
        }

        [Fact]
        public void Csv_EscapeOnly_Process()
        {
            string line = "a\\a\\a,\\,c\\c\\c";
            var expectedTokens = new List<string> { "a\\a\\a", "\\", "c\\c\\c" };
            List<string> result = null;
            result = csvParser.CsvSplit(line);
            Assert.True(result.SequenceEqual(expectedTokens));
        }

        [Fact]
        public void Csv_Escape_Process()
        {
            string line = "123,\"\"\"thank you,\"\" he said.\",aaa";
            var expectedTokens = new List<string> { "123", "\"thank you,\" he said.", "aaa" };
            List<string> result = null;
            result = csvParser.CsvSplit(line);
            Assert.True(result.SequenceEqual(expectedTokens));
        }

        [Fact]
        public void Csv_FieldNull_Process()
        {
            string line = ",,,";
            var expectedTokens = new List<string> { null, null, null, null };
            List<string> result = null;
            result = csvParser.CsvSplit(line);
            Assert.True(result.SequenceEqual(expectedTokens));
        }

        [Fact]
        public void Csv_EscapeError_ThrowException()
        {
            string line = "\"\"\"\"hello\"\"\"\"";
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
            string line = "aaa,\"b\"b\"b\",ccc";
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
            string line = "aaa\t\"\"\"hi\"\"\tJack\"";
            var expectedTokens = new List<string> { "aaa", "\"hi\"\tJack" };
            List<string> result = null;
            result = tsvParser.CsvSplit(line);
            Assert.True(result.SequenceEqual(expectedTokens));
        }

        [Fact]
        public void Tsv_NoEndQuote_ThrowException()
        {
            string line = "aaa\t\"hi\"\"\t\"\"";
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
