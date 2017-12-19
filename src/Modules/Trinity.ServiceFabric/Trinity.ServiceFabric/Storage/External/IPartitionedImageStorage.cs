using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.ServiceFabric.Storage.External
{
    public interface IPartitionedImageStorage
    {
        string LoadImagePartition(int partition);
        string SaveImagePartition(int partition);

        ImagePartitionSignature LoadPartitionSignature(int partition);
        void SavePartitionSignature(ImagePartitionSignature signature);
    }
}
