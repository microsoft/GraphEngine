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
        public Guid ServerId { get; internal set; }

        public string Nickname { get; internal set; }

        public NameDescriptor()
        {
            // !!! Note, ServerId should be consistent if allocated more than once!

            ServerId = GetMachineGuid();
            Nickname = GenerateNickName(ServerId);
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
    }

    public interface INameService
    {
        TrinityErrorCode PublishServerInfo(NameDescriptor name, ServerInfo serverInfo);
        event EventHandler<Tuple<NameDescriptor, ServerInfo>> NewServerInfoPublished;
    }
}
