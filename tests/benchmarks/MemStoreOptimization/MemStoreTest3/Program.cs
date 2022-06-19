using System;
using System.Threading;
using System.Threading.Tasks;
using Trinity;
using Trinity.Storage;

namespace MemStoreTest3
{
    class Program
    {
        static void Main(string[] args)
        {
            //TrinityConfig.LoggingLevel = Trinity.Diagnostics.LogLevel.Verbose;
            //TrinityConfig.TrunkCount = 1;
            // 1 billion entries
            int ncell = 1<<30;
            Random r = new Random(12345);
            //Global.LocalStorage.SaveMyCell(0, 0);
            //LocalMemoryStorage.PauseMemoryDefragmentation();
            int i = 0;
            //Task.Run(() =>
            //{
            //    while (true)
            //    {
            //        Thread.Sleep(1000);
            //        Console.WriteLine(i);
            //    }
            //});
            for (i = 0; i< ncell; ++i)
            {
                var id = r.Next();
                Global.LocalStorage.SaveMyCell(id, 0);
            }
            //while (true)
            //{
            //    for (i = 0; i< ncell; ++i)
            //    {
            //        //var id = r.Next();
            //        //if(id == 1953158751)
            //        //{
            //        //    Hit();
            //        //}
            //        Global.LocalStorage.SaveMyCell(i, 0);
            //    }
            //}
        }

        private static void Hit()
        {
            Console.WriteLine("Hit");
        }
    }
}
