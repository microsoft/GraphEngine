using System;
using Trinity.ServiceFabric.Diagnostics;
using Trinity.ServiceFabric.Storage.External;
using Trinity.Storage;

namespace Trinity.ServiceFabric.Storage
{
    public class LocalMemoryStorageBackedByExternal : LocalMemoryStorage
    {
        private IClusterConfig clusterConfig;
        private ITrinityStorageImage externalStorage;

        public LocalMemoryStorageBackedByExternal(IClusterConfig clusterConfig, ITrinityStorageImage externalStorage)
        {
            this.clusterConfig = clusterConfig;
            this.externalStorage = externalStorage;
        }

        protected override bool LoadLocalMemoryStorage()
        {
            var serverId = clusterConfig.MyServerId;
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
            var serverId = clusterConfig.MyServerId;
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
