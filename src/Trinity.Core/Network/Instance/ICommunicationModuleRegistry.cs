using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.Network
{
    /// <summary>
    /// Provides methods for registering and querying communication modules.
    /// </summary>
    public interface ICommunicationModuleRegistry
    {
        /// <summary>
        /// Gets an instance of a registered communication module.
        /// </summary>
        /// <typeparam name="T">The type of the communication module.</typeparam>
        /// <returns>A communication module object if the module type is registered. Otherwise returns null.</returns>
        T GetCommunicationModule<T>() where T : CommunicationModule;
    }
}
