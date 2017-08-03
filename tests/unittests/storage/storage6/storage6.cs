using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using NUnit.Framework;
using Trinity;
using Trinity.Storage;

namespace storage6
{
    public class storage6
    {
		public static void StorageSavedHandler()
        {
            Console.WriteLine("Save Finished.");
            return;
        }
        public static void StorageBeforeSaveHandler()
        {
            Console.WriteLine("Save Started.");
            return;
        }
        public static void StorageLoadedHandler()
        {
            Console.WriteLine("Load Finished.");
            return;
        }
        public static void StorageBeforeLoadHandler()
        {
            Console.WriteLine("Load Started.");
            return;
        }
        public static void StorageResetHandler()
        {
            Console.WriteLine("Reset Finished.");
            return;
        }
        public static void StorageBeforeResetHandler()
        {
            Console.WriteLine("Reset Started.");
            return;
        }
		
		[Test]
        public unsafe void T1()
        {
            Global.LocalStorage.StorageBeforeLoad += new Action(StorageBeforeLoadHandler);
            Global.LocalStorage.StorageLoaded += new Action(StorageLoadedHandler);
            Global.LocalStorage.LoadStorage();

            Global.LocalStorage.StorageBeforeReset += new Action(StorageBeforeResetHandler);
            Global.LocalStorage.StorageReset += new Action(StorageResetHandler);
            Global.LocalStorage.ResetStorage();

            Global.LocalStorage.StorageBeforeSave += new Action(StorageBeforeSaveHandler);
            Global.LocalStorage.StorageSaved += new Action(StorageSavedHandler);
            Global.LocalStorage.SaveStorage();
        }
    }
}
