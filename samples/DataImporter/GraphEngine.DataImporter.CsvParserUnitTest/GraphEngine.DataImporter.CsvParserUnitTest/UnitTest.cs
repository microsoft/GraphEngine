using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace GraphEngine.DataImporter.CsvParserUnitTest
{
    public class UnitTest
    {
        [Fact]
        static void Main()
        {
            char delimiter = ',';
            CsvParser parser = new CsvParser(delimiter);
            using (StreamReader si = new StreamReader("..\\..\\CsvParserTestInputString.txt"))
            {
                using (StreamReader sv = new StreamReader("..\\..\\CsvParserTestValidateString.txt"))
                {

                    string line;
                    string result;
                    while ((line = si.ReadLine()) != null)
                    {
                        string validation = sv.ReadLine();
                        try
                        {
                            List<string> list = parser.CsvSplit(line);
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
        }
    }
}
