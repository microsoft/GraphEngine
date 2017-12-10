using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.DynamicCluster.Persistency
{
    public class PersistencyInfo
    {
        public BandwidthMode MBandwidthMode { get; }

        public PersistentStorageMode MStorageMode { get; }

        public AccessorMode MAccessorMode { get; }

        public PersistencyInfo(Dictionary<string, string> config)
        {
            throw new NotImplementedException();
            
        }

    }
}