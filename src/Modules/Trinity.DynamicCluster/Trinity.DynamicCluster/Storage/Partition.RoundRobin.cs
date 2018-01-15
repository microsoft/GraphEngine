using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Network.Messaging;

namespace Trinity.DynamicCluster.Storage
{
    internal unsafe partial class Partition
    {
        public void RoundRobin(byte* message, int size)
        {
            throw new NotImplementedException();
        }

        public void RoundRobin(byte* message, int size, out TrinityResponse[] response)
        {
            throw new NotImplementedException();
        }
        
    }
}
