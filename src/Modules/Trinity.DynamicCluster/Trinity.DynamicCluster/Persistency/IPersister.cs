using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.DynamicCluster.Tasks;

namespace Trinity.DynamicCluster.Persistency
{
    using RawData = ValueTuple<long, ushort, byte[]>;
    public interface IPersister
    {

        bool IsCacheFilledUp();
        void RegisterData(Guid guid, IEnumerable<RawData> rawDatas);
        Task Dump();
        Task DumpToBlobStorageServer(Guid guid, List<byte[]> srcs, out bool succeed);
        Task Start();
        

       
        

    }
}