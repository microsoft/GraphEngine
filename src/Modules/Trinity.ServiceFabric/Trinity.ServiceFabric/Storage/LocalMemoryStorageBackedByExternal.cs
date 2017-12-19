using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.ServiceFabric.Diagnostics;
using Trinity.ServiceFabric.Storage.External;
using Trinity.Storage;

namespace Trinity.ServiceFabric.Storage
{
    class LocalMemoryStorageBackedByExternal : LocalMemoryStorage
    {
        private ITrinityStorageImage externalStorage;

        public LocalMemoryStorageBackedByExternal(ITrinityStorageImage externalStorage)
        {
            this.externalStorage = externalStorage;
        }

        protected override bool LoadLocalMemoryStorage()
        {
            try
            {
                Log.Info("Server {0} is loading storage ...", Global.MyServerId);

                var suc = externalStorage.LoadLocalStorage();

                Log.Info("Server {0} storage loaded: {1}", Global.MyServerId, suc);
                return suc;
            }
            catch (Exception e)
            {
                Log.Fatal("Server {0} loading storage exception {1}", Global.MyServerId, e);
                return false;
            }
        }

        protected override bool SaveLocalMemoryStorage()
        {
            try
            {
                Log.Info("Server {0} is saving storage ...", Global.MyServerId);

                var suc = externalStorage.SaveLocalStorage();

                Log.Info("Server {0} storage saved: {1}", Global.MyServerId, suc);
                return suc;
            }
            catch (Exception e)
            {
                Log.Fatal("Server {0} saving storage exception {1}", Global.MyServerId, e);
                return false;
            }
        }
    }
}
