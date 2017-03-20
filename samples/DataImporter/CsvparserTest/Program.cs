using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using GraphEngine.DataImporter;

namespace CsvparserTest
{
    class Program
    {
        static void Main(string[] args)
        {
            CsvParser parser = new CsvParser();
            char delimiter = ',';
            using (StreamReader sr = new StreamReader("..\\..\\CsvParserTestInputString.txt"))
            {
                string line;
                int count = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.StartsWith("//"))
                        continue;
                    count++;
                    if (count > 1)
                    {
                        Console.WriteLine();
                    }
                    Console.WriteLine("Test {0}: ", count);
                    Console.WriteLine("Original Line: {0}", line);
                    try
                    {
                        List<string> list = parser.CsvSplit(line, delimiter);
                        Console.WriteLine("Processed Line: {0}", String.Join("  ", list.ToArray()));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error: {0}", e.Message);
                    }
                }
            }

        }
    }
}
