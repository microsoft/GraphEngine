using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.DynamicCluster.Persistency
{
    public enum AccessorMode
    {
        Indexable,
        Recordable,  
        /* Although random access is denied, 
           records for accessing can be allowed to established by the blob storage server.
        */
        StreamEntry  // Only an entry of traversable stream is available.
    }
}
