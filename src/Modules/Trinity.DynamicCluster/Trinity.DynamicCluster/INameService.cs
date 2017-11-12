using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Trinity.Network;

namespace Trinity.DynamicCluster
{
    public class NameDescriptor
    {
        public Guid ServerId { get; private set; }

        public string Nickname { get; private set; }

        public NameDescriptor()
        {
            // !!! Note, ServerId should be consistent if allocated more than once!

            ServerId = GetMachineGuid();
            Nickname = GenerateNickName(ServerId);
        }

        private NameDescriptor(string name, Guid serverId)
        {
            ServerId = serverId;
            Nickname = name;
        }

        private string GenerateNickName(Guid serverId)
        {
            string[] names;
            using (var sr = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Trinity.DynamicCluster.names.txt")))
            {
                names = sr.ReadToEnd().Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            }

            int seed = serverId.GetHashCode();
            Random r = new Random(seed);
            var n1 = names[r.Next(names.Length)];
            var n2 = names[r.Next(names.Length)];

            return n1 + " " + n2;
        }

        private Guid GetMachineGuid()
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();
            int hash = 0;
            foreach (var net in interfaces)
            {
                var mac = net.GetPhysicalAddress();
                hash = (hash << 5) ^ mac.GetHashCode();
            }
            Random r = new Random(hash);
            byte[] b = new byte[16];
            r.NextBytes(b);
            return new Guid(b);
        }

        public static implicit operator NameDescriptor(StorageInformation si)
        {
            return new NameDescriptor(si.name, si.id);
        }
    }

    public interface INameService
    {
        TrinityErrorCode Start();
        TrinityErrorCode PublishServerInfo(NameDescriptor name, ServerInfo serverInfo);
        event EventHandler<Tuple<NameDescriptor, ServerInfo>> NewServerInfoPublished;
        TrinityErrorCode Stop();
    }
}
