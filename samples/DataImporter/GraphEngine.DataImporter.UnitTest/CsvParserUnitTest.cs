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
            int beginLine = 0;
            StreamReader sr = new StreamReader("..\\..\\CsvParserTestInputString.txt");
            for (int i = 0; i < beginLine; i++)
                sr.ReadLine();
            string line = sr.ReadLine();
            string validation = sr.ReadLine();
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
            sr.Close();
        }

        [Fact]
        public void Csv_CommaInQuote_Process()
        {
            int beginLine = 2;
            StreamReader sr = new StreamReader("..\\..\\CsvParserTestInputString.txt");
            for (int i = 0; i < beginLine; i++)
                sr.ReadLine();
            string line = sr.ReadLine();
            string validation = sr.ReadLine();
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
            sr.Close();
        }

        [Fact]
        public void Csv_Escape_Process()
        {
            int beginLine = 4;
            StreamReader sr = new StreamReader("..\\..\\CsvParserTestInputString.txt");
            for (int i = 0; i < beginLine; i++)
                sr.ReadLine();
            string line = sr.ReadLine();
            string validation = sr.ReadLine();
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
            sr.Close();
        }

        [Fact]
        public void Csv_FieldNull_Process()
        {
            int beginLine = 6;
            StreamReader sr = new StreamReader("..\\..\\CsvParserTestInputString.txt");
            for (int i = 0; i < beginLine; i++)
                sr.ReadLine();
            string line = sr.ReadLine();
            string validation = sr.ReadLine();
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
            sr.Close();
        }

        [Fact]
        public void Csv_EscapeError_ThrowException()
        {
            int beginLine = 8;
            StreamReader sr = new StreamReader("..\\..\\CsvParserTestInputString.txt");
            for (int i = 0; i < beginLine; i++)
                sr.ReadLine();
            string line = sr.ReadLine();
            string validation = sr.ReadLine();
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
            sr.Close();
        }

        [Fact]
        public void Csv_QuoteNotAtStartPosition_ThrowException()
        {
            int beginLine = 10;
            StreamReader sr = new StreamReader("..\\..\\CsvParserTestInputString.txt");
            for (int i = 0; i < beginLine; i++)
                sr.ReadLine();
            string line = sr.ReadLine();
            string validation = sr.ReadLine();
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
            sr.Close();
        }

        [Fact]
        public void Csv_NoEndQuote_ThrowException()
        {
            int beginLine = 12;
            StreamReader sr = new StreamReader("..\\..\\CsvParserTestInputString.txt");
            for (int i = 0; i < beginLine; i++)
                sr.ReadLine();
            string line = sr.ReadLine();
            string validation = sr.ReadLine();
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
            sr.Close();
        }

        [Fact]
        public void Csv_NoEscape_ThrowException()
        {
            int beginLine = 14;
            StreamReader sr = new StreamReader("..\\..\\CsvParserTestInputString.txt");
            for (int i = 0; i < beginLine; i++)
                sr.ReadLine();
            string line = sr.ReadLine();
            string validation = sr.ReadLine();
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
            sr.Close();
        }

        [Fact]
        public void Csv_QuoteNotAtEndPosition_ThrowException()
        {
            int beginLine = 16;
            StreamReader sr = new StreamReader("..\\..\\CsvParserTestInputString.txt");
            for (int i = 0; i < beginLine; i++)
                sr.ReadLine();
            string line = sr.ReadLine();
            string validation = sr.ReadLine();
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
            sr.Close();
        }

        [Fact]
        public void Tsv_Escape_Quote_Process()
        {
            int beginLine = 18;
            StreamReader sr = new StreamReader("..\\..\\CsvParserTestInputString.txt");
            for (int i = 0; i < beginLine; i++)
                sr.ReadLine();
            string line = sr.ReadLine();
            string validation = sr.ReadLine();
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
            sr.Close();
        }

        [Fact]
        public void Tsv_NoEndQuote_ThrowException()
        {
            int beginLine = 20;
            StreamReader sr = new StreamReader("..\\..\\CsvParserTestInputString.txt");
            for (int i = 0; i < beginLine; i++)
                sr.ReadLine();
            string line = sr.ReadLine();
            string validation = sr.ReadLine();
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
            sr.Close();
        }

    }
}
