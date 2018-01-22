using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.DynamicCluster.Storage
{
    public enum ProtocolSemantic
    {
        FirstAvailable = 0,
        RoundRobin     = 1,
        UniformRandom  = 2,
        Broadcast      = 3,
        Vote           = 4,

        ProtocolSemanticEND
    }
}
