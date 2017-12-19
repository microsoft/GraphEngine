using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.ServiceFabric.Storage.External
{
    public interface ITrinityStorageImage
    {
        /// <summary>
        /// Saves the local memory storage into the external storage.
        /// </summary>
        /// <returns>true if the local memory storage was saved successfully; otherwise, false.</returns>
        bool SaveLocalStorage();

        /// <summary>
        /// Loads the local memory storage from the external storage.
        /// </summary>
        /// <returns>true if the local memory storage was loaded successfully; otherwise, false.</returns>
        bool LoadLocalStorage();
    }
}
