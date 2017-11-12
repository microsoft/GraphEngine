using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Trinity.Diagnostics;
using Trinity.Network;
using Trinity.Network.Messaging;

namespace Trinity.Storage
{
    /// <summary>
    /// Provides methods for interacting with the distributed memory store.
    /// </summary>
    public unsafe abstract partial class MemoryCloud : IDisposable
    {
        #region Abstract interfaces
        public abstract bool Open(ClusterConfig config, bool nonblocking);
        public abstract int MyPartitionId { get; }
        public abstract int MyProxyId { get; }
        public abstract IEnumerable<int> MyChunkIds { get; }
        public abstract int PartitionCount { get; }
        public abstract int ProxyCount { get; }
        public abstract IList<RemoteStorage> ProxyList { get; }
        public abstract bool IsLocalCell(long cellId);
        public abstract bool LoadStorage();
        public abstract bool SaveStorage();
        public abstract bool ResetStorage();
        #endregion
        #region Base implementation
        // XXX an implementation shall initialize this!
        protected internal Storage[] StorageTable;
        private Action<MemoryCloud, ICell> m_SaveGenericCell_ICell;
        private Func<MemoryCloud, long, ICell> m_LoadGenericCell_long;
        private Func<string, ICell> m_NewGenericCell_string;
        private Func<long, string, ICell> m_NewGenericCell_long_string;
        private Func<string, string, ICell> m_NewGenericCell_string_string;


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
            m_SaveGenericCell_ICell        = cloud_operations.SaveGenericCell;
            m_LoadGenericCell_long         = cloud_operations.LoadGenericCell;
            m_NewGenericCell_string        = cloud_operations.NewGenericCell;
            m_NewGenericCell_long_string   = cloud_operations.NewGenericCell;
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
        public T GetCommunicationModule<T>() where T : CommunicationModule
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
        protected GetServerIdByCellIdDelegate StaticGetPartitionByCellId;


        /// <summary>
        /// Sets a user-defined data partitioning method.
        /// </summary>
        /// <param name="getPartitionIdByCellIdMethod">A method that transforms a 64-bit cell Id to a Trinity server Id.</param>
        public void SetPartitionMethod(GetServerIdByCellIdDelegate getPartitionIdByCellIdMethod)
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


        protected Storage GetStorageByCellId(long cellId)
        {
            return StorageTable[GetPartitionIdByCellId(cellId)];
        }

        #region IDisposable
        private volatile bool disposed = false;

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
            if (!this.disposed)
            {
                foreach (var storage in StorageTable)
                {
                    if (storage != null && storage != Global.local_storage)
                        storage.Dispose();
                }

                this.disposed = true;
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

        // TODO check sendmessage semantics for HA scenario
        internal void SendMessageToServer(int serverId, TrinityMessage msg)
        {
            StorageTable[serverId].SendMessage(msg);
        }

        internal void SendMessageToServer(int serverId, TrinityMessage msg, out TrinityResponse response)
        {
            StorageTable[serverId].SendMessage(msg, out response);
        }

        internal void SendMessageToServer(int serverId, byte[] message, int offset, int size)
        {
            fixed (byte* p = message)
            {
                StorageTable[serverId].SendMessage(p + offset, size);
            }
        }
        internal void SendMessageToServer(int serverId, byte[] message, int offset, int size, out TrinityResponse response)
        {
            fixed (byte* p = message)
            {
                StorageTable[serverId].SendMessage(p + offset, size, out response);
            }
        }

        #region Public
        /// <summary>
        /// Send a binary message to the specified Trinity server.
        /// </summary>
        /// <param name="serverId">A 32-bit server id.</param>
        /// <param name="buffer">A binary message buffer.</param>
        /// <param name="size">The size of the message.</param>
        public virtual void SendMessageToServer(int serverId, byte* buffer, int size)
        {
            var storage = StorageTable[serverId];
            storage.SendMessage(buffer, size);
        }

        /// <summary>
        /// Send a binary message to the specified Trinity server.
        /// </summary>
        /// <param name="serverId">A 32-bit server id.</param>
        /// <param name="buffer">A binary message buffer.</param>
        /// <param name="size">The size of the message.</param>
        /// <param name="response">The TrinityResponse object returned by the Trinity server.</param>
        public virtual void SendMessageToServer(int serverId, byte* buffer, int size, out TrinityResponse response)
        {
            StorageTable[serverId].SendMessage(buffer, size, out response);
        }

        /// <summary>
        /// Send a binary message to the specified Trinity proxy.
        /// </summary>
        /// <param name="proxyId">A 32-bit proxy id.</param>
        /// <param name="buffer">A binary message buffer.</param>
        /// <param name="size">The size of the message.</param>
        public abstract void SendMessageToProxy(int proxyId, byte* buffer, int size);

        /// <summary>
        /// Send a binary message to the specified Trinity proxy.
        /// </summary>
        /// <param name="proxyId">A 32-bit proxy id.</param>
        /// <param name="buffer">A binary message buffer.</param>
        /// <param name="size">The size of the message.</param>
        /// <param name="response">The TrinityResponse object returned by the Trinity proxy.</param>
        public abstract void SendMessageToProxy(int proxyId, byte* buffer, int size, out TrinityResponse response);
        #endregion

        public virtual long GetTotalMemoryUsage()
        {
            throw new NotSupportedException();
        }




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
