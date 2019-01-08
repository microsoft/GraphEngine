using System;
using System.Linq;
using Trinity;
using Trinity.Diagnostics;

namespace save_load_tsl
{
    class Program
    {
        static void Main(string[] args)
        {
            TrinityConfig.LoggingLevel = LogLevel.Debug;
            TrinityConfig.StorageRoot  = Environment.CurrentDirectory;
            foreach (var item in Enumerable.Range(0, 100))
            {
                AddPerson(item, "person-" + item);
            }
            Global.LocalStorage.SaveStorage();

            Global.LocalStorage.LoadStorage();
            Console.WriteLine("Done!");
        }

        static void AddPerson(int age, string name)
        {
            Person p = new Person(age, name);
            Global.LocalStorage.SavePerson(p);
        }
    }
}
