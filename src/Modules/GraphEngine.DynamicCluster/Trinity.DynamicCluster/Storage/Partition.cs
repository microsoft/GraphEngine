using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Trinity;
using Trinity.Network.Messaging;
using Trinity.Diagnostics;
using System.Runtime.CompilerServices;
using Trinity.Network;
using Trinity.Utilities;
using Trinity.Storage;
using Trinity.DynamicCluster;
using Trinity.Configuration;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace Trinity.DynamicCluster.Storage
{
    /// <summary>
    /// A Partition represent multiple storages scattered across the cluster,
    /// which add up to a complete availability group (aka partition). Hence, it
    /// is both a storage, and a list of storages.
    /// </summary>
    public partial class Partition : IStorage, IEnumerable<IStorage>
    {
        internal unsafe delegate Task SendMessageFunc(byte* message, int size);
        internal unsafe delegate Task SendMessageMultiFunc(byte** message, int* sizes, int count);
        internal unsafe delegate Task<TrinityResponse> SendMessageWithRspFunc(byte* message, int size);
        internal unsafe delegate Task<TrinityResponse> SendMessageWithRspMultiFunc(byte** message, int* sizes, int count);

        private ImmutableDictionary<IStorage, IEnumerable<Chunk>> m_storages = null;
        private object                                            m_syncroot = new object();
        private Func<IMessagePassingEndpoint>                     m_firstavailable_getter = null;
        private Func<IMessagePassingEndpoint>                     m_roundrobin_getter = null;
        private Func<IMessagePassingEndpoint>                     m_random_getter = null;
        private SendMessageFunc[]                                 m_smfuncs = null;
        private SendMessageMultiFunc[]                            m_smmfuncs = null;
        private SendMessageWithRspFunc[]                          m_smrfuncs = null;
        private SendMessageWithRspMultiFunc[]                     m_smrmfuncs = null;

        public unsafe Partition()
        {
            m_storages  = ImmutableDictionary<IStorage, IEnumerable<Chunk>>.Empty;
            _UpdateIterators();

            m_smfuncs   = new SendMessageFunc[(int)ProtocolSemantic.ProtocolSemanticEND];
            m_smmfuncs  = new SendMessageMultiFunc[(int)ProtocolSemantic.ProtocolSemanticEND];
            m_smrfuncs  = new SendMessageWithRspFunc[(int)ProtocolSemantic.ProtocolSemanticEND];
            m_smrmfuncs = new SendMessageWithRspMultiFunc[(int)ProtocolSemantic.ProtocolSemanticEND];

            m_smfuncs[(int)ProtocolSemantic.FirstAvailable] = (msg, size) => FirstAvailable(ep => ep.SendMessageAsync(msg, size));
            m_smfuncs[(int)ProtocolSemantic.RoundRobin] = (msg, size) => RoundRobin(ep => ep.SendMessageAsync(msg, size));
            m_smfuncs[(int)ProtocolSemantic.UniformRandom] = (msg, size) => UniformRandom(ep => ep.SendMessageAsync(msg, size));
            m_smfuncs[(int)ProtocolSemantic.Broadcast] = (msg, size) => Broadcast(ep => ep.SendMessageAsync(msg, size));
            m_smfuncs[(int)ProtocolSemantic.Vote] = (msg, size) => Vote(ep => ep.SendMessageAsync(msg, size), m_storages.Count);

            m_smrfuncs[(int)ProtocolSemantic.FirstAvailable] = (msg, size) => FirstAvailable(ep => ep.SendRecvMessageAsync(msg, size));
            m_smrfuncs[(int)ProtocolSemantic.RoundRobin] = (msg, size) => RoundRobin(ep => ep.SendRecvMessageAsync(msg, size));
            m_smrfuncs[(int)ProtocolSemantic.UniformRandom] = (msg, size) => UniformRandom(ep => ep.SendRecvMessageAsync(msg, size));
            //BROADCAST WITH RSP NOT SUPPORTED -- USE THE API METHOD INSTEAD
            //m_smrfuncs[(int)ProtocolSemantic.Broadcast] = (msg, size) => Broadcast(ep => { ep.SendMessage(msg, size, out var rsp); return rsp; });
            m_smrfuncs[(int)ProtocolSemantic.Vote] = (msg, size) => Vote(ep => ep.SendRecvMessageAsync(msg, size), m_storages.Count);

            m_smmfuncs[(int)ProtocolSemantic.FirstAvailable] = (msg, size, count) => FirstAvailable(ep => ep.SendMessageAsync(msg, size, count));
            m_smmfuncs[(int)ProtocolSemantic.RoundRobin] = (msg, size, count) => RoundRobin(ep => ep.SendMessageAsync(msg, size, count));
            m_smmfuncs[(int)ProtocolSemantic.UniformRandom] = (msg, size, count) => UniformRandom(ep => ep.SendMessageAsync(msg, size, count));
            m_smmfuncs[(int)ProtocolSemantic.Broadcast] = (msg, size, count) => Broadcast(ep => ep.SendMessageAsync(msg, size, count));
            m_smmfuncs[(int)ProtocolSemantic.Vote] = (msg, size, count) => Vote(ep => ep.SendMessageAsync(msg, size, count), m_storages.Count);

            m_smrmfuncs[(int)ProtocolSemantic.FirstAvailable] = (msg, size, count) => FirstAvailable(ep => ep.SendRecvMessageAsync(msg, size, count));
            m_smrmfuncs[(int)ProtocolSemantic.RoundRobin] = (msg, size, count) => RoundRobin(ep => ep.SendRecvMessageAsync(msg, size, count));
            m_smrmfuncs[(int)ProtocolSemantic.UniformRandom] = (msg, size, count) => UniformRandom(ep => ep.SendRecvMessageAsync(msg, size, count));
            //BROADCAST WITH RSP NOT SUPPORTED -- USE THE API METHOD INSTEAD
            //m_smrfuncs[(int)ProtocolSemantic.Broadcast] = (msg, size, count) => Broadcast(ep => { ep.SendMessage(msg, size, out var rsp); return rsp; });
            m_smrmfuncs[(int)ProtocolSemantic.Vote] = (msg, size, count) => Vote(ep => ep.SendRecvMessageAsync(msg, size, count), m_storages.Count);
        }

        internal TrinityErrorCode Mount(IStorage storage, IEnumerable<Chunk> cc)
        {
            lock (m_syncroot)
            {
                m_storages = m_storages.SetItem(storage, cc);
                _UpdateIterators();
                return TrinityErrorCode.E_SUCCESS;
            }
        }

        private void _UpdateIterators()
        {
            m_firstavailable_getter = Utils.Schedule(this, Utils.SchedulePolicy.First).ParallelGetter();
            m_random_getter         = Utils.Schedule(this, Utils.SchedulePolicy.UniformRandom).ParallelGetter();
            m_roundrobin_getter     = Utils.Schedule(this, Utils.SchedulePolicy.RoundRobin).ParallelGetter();
        }

        internal TrinityErrorCode Unmount(IStorage s)
        {
            lock (m_syncroot)
            {
                try
                {
                    if (s == null) return TrinityErrorCode.E_FAILURE;
                    m_storages = m_storages.Remove(s);
                    _UpdateIterators();
                    return TrinityErrorCode.E_SUCCESS;
                }
                catch (Exception ex)
                {
                    Log.WriteLine(LogLevel.Error, "ChunkedStorage: Errors occurred during Unmount.");
                    Log.WriteLine(LogLevel.Error, ex.ToString());
                    return TrinityErrorCode.E_FAILURE;
                }
            }
        }

        internal bool IsLocal(long cellId)
        {
            return PickStorages(cellId).Any(s => s == Global.LocalStorage);
        }

        /// <summary>
        /// Returns the storages that cover the cellId
        /// </summary>
        /// <param name="cellId"></param>
        /// <returns></returns>
        private IEnumerable<IStorage> PickStorages(long cellId)
        {
            return m_storages.Where(s => s.Value.Any(c => c.Covers(cellId))).Select(_ => _.Key);
        }

        #region IEnumerable
        public IEnumerator<IStorage> GetEnumerator()
        {
            return m_storages.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    m_storages.ForEach(kvp => kvp.Key.Dispose());
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
