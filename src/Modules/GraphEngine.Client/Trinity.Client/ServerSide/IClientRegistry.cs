using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Storage;

namespace Trinity.Client
{
    /// <summary>
    /// Should be implemented by a hosting memory cloud.
    /// </summary>
    public interface IClientRegistry
    {
        int RegisterClient(IStorage client);
        void UnregisterClient(int instanceId);
    }
}
