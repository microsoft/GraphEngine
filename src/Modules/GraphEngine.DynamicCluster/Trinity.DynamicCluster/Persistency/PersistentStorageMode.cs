using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.DynamicCluster.Persistency
{
    [Flags]
    public enum PersistentStorageMode : int
    {
        /* Locality */
        /// <summary>
        /// Data persisted at the local machine.
        /// </summary>
        LO_Local,
        /// <summary>
        /// Data persisted across the memory cloud.
        /// </summary>
        LO_Replication,
        /// <summary>
        /// Data persisted outside memory cloud.
        /// </summary>
        LO_External,
        /* Medium */
        /// <summary>
        /// Storage medium is volatile memory.
        /// </summary>
        ST_VolatileMemory,
        /// <summary>
        /// Storage medium is non-volatile memory.
        /// </summary>
        ST_NonVolatileMemory,
        /// <summary>
        /// Storage medium is mechanical drive.
        /// </summary>
        ST_MechanicalDrive,
        /// <summary>
        /// Storage medium is solid state drive.
        /// </summary>
        ST_SolidStateDrive,
        /* Accessibility */
        /// <summary>
        /// Storage is accessed as a file system.
        /// </summary>
        AC_FileSystem, // Azure Blob Storage, HDFS, etc.
        /// <summary>
        /// Storage is accessed as a row store.
        /// </summary>
        AC_RowStore,
        /// <summary>
        /// Storage is accessed as a key-value store.
        /// </summary>
        AC_KeyValueStore,
        /// <summary>
        /// Storage is accessed as a random-access address space.
        /// </summary>
        AC_AddressSpace, // Memory, buffer, etc.
    }
}
