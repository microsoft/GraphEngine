using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.DynamicCluster.Persistency
{
    public enum BandwidthMode
    {
        // TODO: Review this collection and complete the list.    
        UnlimitedBandwidth,
        PartialLimited, // Only some main components of the system have enough bandwidth to connect with blob storages healthily.
        Limited // Lack of bandwidth when busy.
    }
}