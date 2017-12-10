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
        public static int tcase = 0;
		public static void StorageSavedHandler()
        {
            storage6.tcase++;
            Console.WriteLine("Save Finished.");
            return;
        }
        public static void StorageBeforeSaveHandler()
        {
            storage6.tcase++;
            Console.WriteLine("Save Started.");
            return;
        }
        public static void StorageLoadedHandler()
        {
            storage6.tcase++;
            Console.WriteLine("Load Finished.");
            return;
        }
        public static void StorageBeforeLoadHandler()
        {
            storage6.tcase++;
            Console.WriteLine("Load Started.");
            return;
        }
        public static void StorageResetHandler()
        {
            storage6.tcase++;
            Console.WriteLine("Reset Finished.");
            return;
        }
        public static void StorageBeforeResetHandler()
        {
            storage6.tcase++;
            Console.WriteLine("Reset Started.");
            return;
        }
		
		[Test]
        public unsafe void T1()
        {
            Global.LocalStorage.StorageBeforeLoad += new Action(StorageBeforeLoadHandler);
            Global.LocalStorage.StorageLoaded += new Action(StorageLoadedHandler);
            Global.LocalStorage.LoadStorage();
            Assert.AreEqual(2, storage6.tcase);

            Global.LocalStorage.StorageBeforeReset += new Action(StorageBeforeResetHandler);
            Global.LocalStorage.StorageReset += new Action(StorageResetHandler);
            Global.LocalStorage.ResetStorage();
            Assert.AreEqual(4, storage6.tcase);

            Global.LocalStorage.StorageBeforeSave += new Action(StorageBeforeSaveHandler);
            Global.LocalStorage.StorageSaved += new Action(StorageSavedHandler);
            Global.LocalStorage.SaveStorage();
            Assert.AreEqual(6, storage6.tcase);
        }
    }
}
