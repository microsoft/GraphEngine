using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.ServiceFabric.Diagnostics;
using Trinity.ServiceFabric.Stateless;
using Trinity.ServiceFabric.Storage.External;
using Trinity.Storage;

namespace Trinity.ServiceFabric.Storage
{
    class LocalMemoryStorageBackedByExternal : LocalMemoryStorage
    {
        private TrinityStatelessService service;
        private ITrinityStorageImage externalStorage;

        public LocalMemoryStorageBackedByExternal(TrinityStatelessService trinityStatelessService, ITrinityStorageImage externalStorage)
        {
            this.service = trinityStatelessService;
            this.externalStorage = externalStorage;
        }

        protected override bool LoadLocalMemoryStorage()
        {
            var serverId = service.ClusterConfig.MyServerId;
            try
            {
                Log.Info("Server {0} is loading storage ...", serverId);

                var suc = externalStorage.LoadLocalStorage();

                Log.Info("Server {0} storage loaded: {1}", serverId, suc);
                return suc;
            }
            catch (Exception e)
            {
                Log.Fatal("Server {0} loading storage exception {1}", serverId, e);
                return false;
            }
        }

        protected override bool SaveLocalMemoryStorage()
        {
            var serverId = service.ClusterConfig.MyServerId;
            try
            {
                Log.Info("Server {0} is saving storage ...", serverId);

                var suc = externalStorage.SaveLocalStorage();

                Log.Info("Server {0} storage saved: {1}", serverId, suc);
                return suc;
            }
            catch (Exception e)
            {
                Log.Fatal("Server {0} saving storage exception {1}", serverId, e);
                return false;
            }
        }
    }
}
