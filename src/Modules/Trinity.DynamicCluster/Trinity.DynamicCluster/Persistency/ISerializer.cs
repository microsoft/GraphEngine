using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.DynamicCluster.Persistency
{
    using RawData = ValueTuple<long, ushort, byte[]>;

    public interface ISerializer
    {

        byte[] Apply(RawData rawData);  // serializing and counting.
        int CountAndRefresh(); // Count of the serialized and refresh.
    }
}