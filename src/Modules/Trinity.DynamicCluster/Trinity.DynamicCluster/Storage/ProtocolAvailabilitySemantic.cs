using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.DynamicCluster.Storage
{
    public enum ProtocolAvailabilitySemantic
    {
        FirstAvailable,
        RoundRobin,
        UniformRandom,
        Broadcast,
        Vote,
    }
}
