using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.DynamicCluster.Consensus
{
    public interface IReliableDictionary: IDisposable
    {
        TrinityErrorCode Start();
    }
}
