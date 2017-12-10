using System;
using Trinity;
using Trinity.TSL.Lib;

namespace GraphEngine.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var c1 = Global.LocalStorage.UseC1(0, CellAccessOptions.CreateNewOnCellNotFound))
            {
                Console.WriteLine(c1.ToString());
            }
        }
    }
}
