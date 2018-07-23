using System;
using Trinity;

namespace Trinity.Hosting
{
    public class Initializer
    {
        public static int Init()
        {
            //Global.LocalStorage.LoadStorage();
            Console.WriteLine("Hello from selfhost CLR!");
            Console.ReadKey();
            return 0;
        }
    }
}
