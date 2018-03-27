using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.ServiceFabric.Storage.External
{
    public interface IPartitionedImageStorage
    {
        Task<string> LoadImagePartitionAsync(int partition);
        Task<string> SaveImagePartitionAsync(int partition);

        ImagePartitionSignature LoadPartitionSignature(int partition);
        Task SavePartitionSignatureAsync(ImagePartitionSignature signature);
    }
}
