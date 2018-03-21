using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trinity.Diagnostics;
using Trinity.Network;
using Trinity.Network.Messaging;

namespace Trinity.Storage
{
    /// <summary>
    /// Provides methods for interacting with the distributed memory store.
    /// </summary>
    public unsafe abstract partial class MemoryCloud : IKeyValueStore, IEnumerable<IMessagePassingEndpoint>, ICommunicationModuleRegistry, IDisposable
    {
        #region Abstract interfaces
        public abstract bool Open(ClusterConfig config, bool nonblocking);
        /// <summary>
        /// Represents a unique identifier of this communication instance, within a memory cloud.
        /// </summary>
        public abstract int MyInstanceId { get; }

        /// <summary>
        /// Gets the ID of current server instance in the cluster.
        /// </summary>
        public abstract int MyPartitionId { get; }
        /// <summary>
        /// Gets the ID of current proxy instance in the cluster.
        /// </summary>
        public abstract int MyProxyId { get; }
        public abstract IEnumerable<Chunk> MyChunks { get; }
        /// <summary>
        /// The number of servers in the cluster.
        /// </summary>
        public abstract int PartitionCount { get; }
        public abstract bool IsLocalCell(long cellId);
        public abstract bool LoadStorage();
        public abstract bool SaveStorage();
        public abstract bool ResetStorage();
        /// <summary>
        /// Gets the number of proxies in the cluster.
        /// </summary>
        public abstract int ProxyCount { get; }
        public abstract IList<RemoteStorage> ProxyList { get; }
        /// <summary>
        /// Represents the partitions in the memory cloud.
        /// </summary>
        protected internal abstract IList<IStorage> StorageTable { get; }
        #endregion
        #region KVStore
        /// <summary>
        /// Loads the type and the content of the cell with the specified cell Id.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="cellBuff">The bytes of the cell. An empty byte array is returned if the cell is not found.</param>
        /// <param name="cellType">The type of the cell, represented with a 16-bit unsigned integer.</param>
        /// <returns>A Trinity error code. Possible values are E_SUCCESS and E_NOT_FOUND.</returns>
        public TrinityErrorCode LoadCell(long cellId, out byte[] cellBuff, out ushort cellType)
        {
            return GetStorageByCellId(cellId).LoadCell(cellId, out cellBuff, out cellType);
        }
        
        /// <summary>
        /// Adds a new cell to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <param name="size">The size of the cell.</param>
        /// <param name="cellType">The type of the cell, represented with a 16-bit unsigned integer.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        public unsafe TrinityErrorCode SaveCell(long cellId, byte* buff, int size, ushort cellType)
        {
            return GetStorageByCellId(cellId).SaveCell(cellId, buff, size, cellType);
        }

        /// <summary>
        /// Adds a new cell to the Trinity key-value store.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <param name="size">The size of the cell.</param>
        /// <param name="cellType">The type of the cell, represented with a 16-bit unsigned integer.</param>
        /// <returns>true if adding succeeds; otherwise, false.</returns>
        public unsafe TrinityErrorCode AddCell(long cellId, byte* buff, int size, ushort cellType)
        {
            return GetStorageByCellId(cellId).AddCell(cellId, buff, size, cellType);
        }

        /// <summary>
        /// Removes the cell with the specified cell Id from the key-value store.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <returns>true if removing succeeds; otherwise, false.</returns>
        public TrinityErrorCode RemoveCell(long cellId)
        {
            return GetStorageByCellId(cellId).RemoveCell(cellId);
        }
        
        /// <summary>
        /// Gets the type of the cell with specified cell Id.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="cellType">The type of the cell specified by cellId.</param>
        /// <returns>A Trinity error code. Possible values are E_SUCCESS and E_NOT_FOUND.</returns>
        public unsafe TrinityErrorCode GetCellType(long cellId, out ushort cellType)
        {
            return GetStorageByCellId(cellId).GetCellType(cellId, out cellType);
        }

        /// <summary>
        /// Determines whether there is a cell with the specified cell Id in Trinity key-value store.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <returns>true if a cell whose Id is cellId is found; otherwise, false.</returns>
        public unsafe bool Contains(long cellId)
        {
            return GetStorageByCellId(cellId).Contains(cellId);
        }

        /// <summary>
        /// Updates an existing cell in the Trinity key-value store.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <param name="size">The size of the cell.</param>
        /// <returns>true if updating succeeds; otherwise, false.</returns>
        public unsafe TrinityErrorCode UpdateCell(long cellId, byte* buff, int size)
        {
            return GetStorageByCellId(cellId).UpdateCell(cellId, buff, size);
        }
        #endregion
        #region Base implementation
        private Action<MemoryCloud, ICell> m_SaveGenericCell_ICell;
        private Func<MemoryCloud, long, ICell> m_LoadGenericCell_long;
        private Func<string, ICell> m_NewGenericCell_string;
        private Func<long, string, ICell> m_NewGenericCell_long_string;
        private Func<string, string, ICell> m_NewGenericCell_string_string;

        /// <summary>
        /// Gets the message passing endpoint bound to a partition.
        /// </summary>
        /// <param name="partitionId">The id of the target partition.</param>
        /// <returns></returns>
        public IMessagePassingEndpoint this[int partitionId] => StorageTable[partitionId];
        /// <inheritdoc/>
        public IEnumerator<IMessagePassingEndpoint> GetEnumerator()
        {
            return StorageTable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// An event that is triggered when a server is connected.
        /// </summary>
        public event ServerStatusEventHandler ServerConnected = delegate { };

        /// <summary>
        /// An event that is triggered when a server is disconnected.
        /// </summary>
        public event ServerStatusEventHandler ServerDisconnected = delegate { };

        /// <summary>
        /// Invokes a ServerConnected event.
        /// </summary>
        /// <param name="e">A event that indicates the server status is changed.</param>
        protected virtual void OnConnected(RemoteStorageEventArgs e)
        {
            ServerConnected(this, e);
        }

        /// <summary>
        /// Invoked a ServerDisconnected event.
        /// </summary>
        /// <param name="e">A event that indicates the server status is changed.</param>
        protected virtual void OnDisconnected(RemoteStorageEventArgs e)
        {
            ServerDisconnected(this, e);
        }

        internal void ReportServerConnectedEvent(RemoteStorage rs)
        {
            RemoteStorageEventArgs e = new RemoteStorageEventArgs(rs);
            OnConnected(e);
        }

        internal void ReportServerDisconnectedEvent(RemoteStorage rs)
        {
            RemoteStorageEventArgs e = new RemoteStorageEventArgs(rs);
            OnDisconnected(e);
        }

        /// <summary>
        /// Loads the content of the cell with the specified cell Id.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <returns>An generic cell instance that implements <see cref="Trinity.Storage.ICell"/> interfaces.</returns>
        public ICell LoadGenericCell(long cellId)
        {
            return m_LoadGenericCell_long(this, cellId);
        }

        /// <summary>
        /// Adds a new cell to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists.
        /// Note that the generic cell will be saved as a strongly typed cell. It can then be loaded into either a strongly-typed cell or a generic cell.
        /// </summary>
        /// <param name="cell">The cell to be saved.</param>
        public void SaveGenericCell(ICell cell)
        {
            m_SaveGenericCell_ICell(this, cell);
        }

        /// <summary>
        /// Instantiate a new generic cell with the specified type.
        /// </summary>
        /// <param name="cellType">The string representation of the cell type.</param>
        /// <returns>The allocated generic cell.</returns>
        public ICell NewGenericCell(string cellType)
        {
            return m_NewGenericCell_string(cellType);
        }

        /// <summary>
        /// Instantiate a new generic cell with the specified type and a cell ID.
        /// </summary>
        /// <param name="cellId">Cell Id.</param>
        /// <param name="cellType">The string representation of the cell type.</param>
        /// <returns>The allocated generic cell.</returns>
        public ICell NewGenericCell(long cellId, string cellType)
        {
            return m_NewGenericCell_long_string(cellId, cellType);
        }

        /// <summary>
        /// Instantiate a new generic cell with the specified type and a cell ID.
        /// </summary>
        /// <param name="cellType">The string representation of the cell type.</param>
        /// <param name="content">The json representation of the cell.</param>
        /// <returns>The allocated generic cell.</returns>
        public ICell NewGenericCell(string cellType, string content)
        {
            return m_NewGenericCell_string_string(cellType, content);
        }

        internal void RegisterGenericOperationsProvider(IGenericCellOperations cloud_operations)
        {
            m_SaveGenericCell_ICell = cloud_operations.SaveGenericCell;
            m_LoadGenericCell_long = cloud_operations.LoadGenericCell;
            m_NewGenericCell_string = cloud_operations.NewGenericCell;
            m_NewGenericCell_long_string = cloud_operations.NewGenericCell;
            m_NewGenericCell_string_string = cloud_operations.NewGenericCell;
        }

        protected unsafe void CheckProtocolSignatures_impl(RemoteStorage storage, RunningMode from, RunningMode to)
        {
            if (storage == null)
                return;

            string my_schema_name;
            string my_schema_signature;
            string remote_schema_name;
            string remote_schema_signature;
            ICommunicationSchema my_schema;

            storage.GetCommunicationSchema(out remote_schema_name, out remote_schema_signature);

            if (from != to)// Asymmetrical checking, need to scan for matching local comm schema first.
            {
                var local_candidate_schemas = Global.ScanForTSLCommunicationSchema(to);

                /* If local or remote is default, we skip the verification. */

                if (local_candidate_schemas.Count() == 0)
                {
                    Log.WriteLine(LogLevel.Info, "{0}-{1}: Local instance has default communication capabilities.", from, to);
                    return;
                }

                if (remote_schema_name == DefaultCommunicationSchema.GetName() || remote_schema_signature == "{[][][]}")
                {
                    Log.WriteLine(LogLevel.Info, "{0}-{1}: Remote cluster has default communication capabilities.", from, to);
                    return;
                }

                /* Both local and remote are not default instances. */

                my_schema = local_candidate_schemas.FirstOrDefault(_ => _.Name == remote_schema_name);

                if (my_schema == null)
                {
                    Log.WriteLine(LogLevel.Fatal, "No candidate local communication schema signature matches the remote one.\r\n\tName: {0}\r\n\tSignature: {1}", remote_schema_name, remote_schema_signature);
                    Global.Exit(-1);
                }
            }
            else
            {
                my_schema = Global.CommunicationSchema;
            }

            my_schema_name = my_schema.Name;
            my_schema_signature = CommunicationSchemaSerializer.SerializeProtocols(my_schema);

            if (my_schema_name != remote_schema_name)
            {
                Log.WriteLine(LogLevel.Error, "Local communication schema name not matching the remote one.\r\n\tLocal: {0}\r\n\tRemote: {1}", my_schema_name, remote_schema_name);
            }

            if (my_schema_signature != remote_schema_signature)
            {
                Log.WriteLine(LogLevel.Fatal, "Local communication schema signature not matching the remote one.\r\n\tLocal: {0}\r\n\tRemote: {1}", my_schema_signature, remote_schema_signature);
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Gets an instance of a registered communication module on the started communication instance.
        /// </summary>
        /// <typeparam name="T">The type of the communication module.</typeparam>
        /// <returns>A communication module object if a communication instance is started, and the module type is registered. Otherwise returns null.</returns>
        public virtual T GetCommunicationModule<T>() where T : CommunicationModule
        {
            var comm_instance = Global.CommunicationInstance;

            if (comm_instance == null)
            {
                return default(T);
            }
            else
            {
                return comm_instance.GetCommunicationModule<T>();
            }
        }

        /// <summary>
        /// Gets the Id of the server on which the cell with the specified cell Id is located.
        /// </summary>
        protected GetPartitionIdByCellIdDelegate StaticGetPartitionByCellId;


        /// <summary>
        /// Sets a user-defined data partitioning method.
        /// </summary>
        /// <param name="getPartitionIdByCellIdMethod">A method that transforms a 64-bit cell Id to a Trinity server Id.</param>
        public void SetPartitionMethod(GetPartitionIdByCellIdDelegate getPartitionIdByCellIdMethod)
        {
            StaticGetPartitionByCellId = getPartitionIdByCellIdMethod;
        }

        /// <summary>
        /// Gets the Id of the server on which the cell with the specified cell Id is located.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <returns>A Trinity server Id.</returns>
        public int GetPartitionIdByCellId(long cellId)
        {
            return StaticGetPartitionByCellId(cellId);
        }


        protected IStorage GetStorageByCellId(long cellId)
        {
            return StorageTable[GetPartitionIdByCellId(cellId)];
        }

        #region IDisposable
        private int m_disposed = 0;

        /// <summary>
        /// Disposes current MemoryCloud instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes current MemoryCloud instance.
        /// </summary>
        /// <param name="disposing">This parameter is not used.</param>
        protected virtual void Dispose(bool disposing)
        {
            if(0 == Interlocked.CompareExchange(ref this.m_disposed, 1, 0))
            {
                foreach (var storage in StorageTable)
                {
                    if (storage != null && storage != Global.local_storage)
                        storage.Dispose();
                }
            }
        }

        /// <summary>
        /// The deconstruction method of MemoryCloud class.
        /// </summary>
        ~MemoryCloud()
        {
            Dispose(false);
        }
        #endregion//IDisposable

        /// <summary>
        /// Gets the total memory usage.
        /// </summary>
        public abstract long GetTotalMemoryUsage();

        #endregion
    }

    /// <summary>
    /// Represents an event class that is triggered when the status of a <see cref="RemoteStorage"/> is changed.
    /// </summary>
    public class RemoteStorageEventArgs : EventArgs
    {

        /// <summary>
        /// Constructs an instance of ServerStatusEventArgs.
        /// </summary>
        /// <param name="rs">The RemoteStorage whose status is changed.</param>
        public RemoteStorageEventArgs(RemoteStorage rs)
        {
            this.RemoteStorage = rs;
        }

        /// <summary>
        /// The target remote storage
        /// </summary>
        public RemoteStorage RemoteStorage { get; private set; }
    }

    /// <summary>
    /// A delegates that represents a handler for ServerStatusEventArgs.
    /// </summary>
    /// <param name="sender">The object that triggers the current event.</param>
    /// <param name="e">An instance of ServerStatusEventArgs.</param>
    public delegate void ServerStatusEventHandler(object sender, RemoteStorageEventArgs e);

}
