using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.DynamicCluster.Persistency
{
    public enum PersistentStorageMode
    {
        // TODO: Review this collection and complete the list.    
        BatteriedMemory,
        Memory,
        SolidStateDisk,
        HardDisk
    }
}