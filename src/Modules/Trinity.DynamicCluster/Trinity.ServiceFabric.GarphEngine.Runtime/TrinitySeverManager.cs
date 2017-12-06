using System.IO;
using Trinity.Diagnostics;
using Trinity.Network;
using Trinity.ServiceFabric.GarphEngine.Infrastructure.Interfaces;

namespace Trinity.ServiceFabric.GarphEngine.Infrastructure
{
    public class TrinitySeverManager : ITrinityServerManager
    {
        private readonly int HttpPort;
        private TrinityServer m_trinityServer;
        private int m_port;
        private object m_partitionId;

        public TrinitySeverManager()
        {
            var ags = TrinityConfig.CurrentClusterConfig.Servers;
            ags.Clear();
            ags.Add(new AvailabilityGroup("LOCAL", new ServerInfo("localhost", Port, null, LogLevel.Info)));

            TrinityConfig.HttpPort = HttpPort;

            Log.WriteLine("{0}", $"WorkingDirectory={context.CodePackageActivationContext.WorkDirectory}");
            TrinityConfig.StorageRoot = Path.Combine(context.CodePackageActivationContext.WorkDirectory, $"P{PartitionId}{Path.GetRandomFileName()}");
            Log.WriteLine("{0}", $"StorageRoot={TrinityConfig.StorageRoot}");
        }

        public object PartitionId
        {
            get { return m_partitionId; }
        }

        public int Port
        {
            get { return m_port; }
        }

        private TrinityServer Server
        {
            get { return m_trinityServer; }
            set { m_trinityServer = value; }
        }

        public TrinityErrorCode Start()
        {
            throw new System.NotImplementedException();
        }

        public TrinityErrorCode Stop()
        {
            throw new System.NotImplementedException();
        }
    }
}