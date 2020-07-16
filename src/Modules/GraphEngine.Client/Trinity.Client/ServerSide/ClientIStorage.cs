using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Trinity.Core.Lib;
using Trinity.Network;
using Trinity.Network.Messaging;
using Trinity.Storage;

namespace Trinity.Client
{
    /// <summary>
    /// Installed in server-side storage tables.
    /// </summary>
    internal class ClientIStorage : IStorage
    {
        private ConcurrentDictionary<long, TaskCompletionSource<bool>> m_req_tokens;
        private ConcurrentDictionary<long, TaskCompletionSource<TrinityResponse>> m_rsp_tokens;
        private ConcurrentQueue<TrinityMessage> m_messages;
        private MemoryCloud m_mc;

        public int InstanceId { get; internal set; }

        public ClientIStorage(MemoryCloud mc)
        {
            m_messages = new ConcurrentQueue<TrinityMessage>();
            m_req_tokens = new ConcurrentDictionary<long, TaskCompletionSource<bool>>();
            m_rsp_tokens = new ConcurrentDictionary<long, TaskCompletionSource<TrinityResponse>>();
            m_mc = mc;
        }

        internal DateTime Pulse { get; set; }

        public unsafe void Dispose()
        {
            if (m_messages == null) return;
            List<long> captured_reqs = new List<long>();

            while (m_messages.TryDequeue(out var msg))
            {
                captured_reqs.Add((long)msg.Buffer);
            }

            m_messages = null;

            /**************************************************************
             * Two possibilities:
             * - PollEvents_impl gets the entry -> message considered sent
             * - Dispose gets the entry -> message considered intercepted
             **************************************************************/

            foreach(var rk in captured_reqs)
            {
                if(m_req_tokens.TryRemove(rk, out var treq))
                {
                    treq.SetException(new IOException("Client timed out"));
                }
            }

            m_req_tokens.Clear();
            m_req_tokens = null;

            /**************************************************************
             * The requests corresponding to these responses have been already 
             * set-exception'ed and thus those with pending requests will not
             * reach the outer logic waiting for TrinityResponse. 
             * This is because we call treq.Wait() before we wait for the response.
             * However, for already-sent requests, it is possible that a client 
             * will post back a response and it somehow gets routed into this 
             * ClientIStorage (although we do call UnregisterClient before we dispose). 
             * So here we compete with the client and grab the entries with TryRemove. 
             * Two possibilities here:
             * - PostResponse_impl gets the entry -> TrinityResponse passed
             * - Dispose gets the entry -> TrinityResponse intercepted
             **************************************************************/

            var rsp_keys = m_rsp_tokens.Keys.ToList();
            foreach(var rk in rsp_keys)
            {
                if(m_rsp_tokens.TryRemove(rk, out var trsp))
                {
                    trsp.SetException(new IOException("Client timed out"));
                }
            }

            m_rsp_tokens.Clear();
            m_rsp_tokens = null;
        }

        public T GetCommunicationModule<T>() where T : CommunicationModule
            => m_mc.GetCommunicationModule<T>();

        private unsafe Task QueueMessage(void* p, int size)
        {
            TaskCompletionSource<bool> req_token = new TaskCompletionSource<bool>();
            m_req_tokens[(long)p] = req_token;
            TrinityMessage tm = new TrinityMessage((byte*)p, size);
            m_messages.Enqueue(tm);
            req_token.Task.ContinueWith(_free(p));
            return req_token.Task;
        }

        private unsafe (Task, Task<TrinityResponse>) QueueMessageWithRsp(void* p, int size)
        {
            TaskCompletionSource<bool> req_token = new TaskCompletionSource<bool>();
            TaskCompletionSource<TrinityResponse> rsp_token = new TaskCompletionSource<TrinityResponse>();
            m_req_tokens[(long)p] = req_token;
            m_rsp_tokens[(long)p] = rsp_token;
            TrinityMessage tm = new TrinityMessage((byte*)p, size);
            m_messages.Enqueue(tm);
            req_token.Task.ContinueWith(_free(p));
            return (req_token.Task, rsp_token.Task);
        }

        internal unsafe void PostResponse_impl(long p, byte* buf, int payload_len)
        {
            if (!m_rsp_tokens.TryRemove(p, out var tsrc)) throw new KeyNotFoundException();
            int len = *(int*)buf;
            if (len < 0)
            {
                tsrc.SetException(new IOException("The client remote handler throws an exception."));
            }
            else if (payload_len != len + TrinityProtocol.TrinityMsgHeader)
            {
                var ex = new IOException($"Client remote handler response size mismatch. Got {payload_len}, expecting {len + TrinityProtocol.TrinityMsgHeader}");
                tsrc.SetException(ex);
                throw ex;
            }
            else
            {
                byte* alloc_buf = (byte*)Memory.malloc((ulong)len);
                Memory.memcpy(alloc_buf, buf + TrinityProtocol.SocketMsgHeader, (ulong)len);
                TrinityResponse rsp = new TrinityResponse(alloc_buf, len);
                tsrc.SetResult(rsp);
            }
        }

        internal unsafe (long, TrinityMessage, TaskCompletionSource<bool>) PollEvents_impl()
        {
            if (!m_messages.TryDequeue(out var msg))
            {
                return (0, null, null);
            }

            long ptr = (long)msg.Buffer;
            if(!m_req_tokens.TryRemove(ptr, out var tsrc))
            {
                msg.Dispose();
                throw new KeyNotFoundException();
            }

            if (!m_rsp_tokens.ContainsKey(ptr)) ptr = 0;

            return (ptr, msg, tsrc);
        }

        public unsafe Task SendMessageAsync(byte* message, int size)
        {
            void* p = Memory.malloc((ulong)size);
            Memory.memcpy(p, message, (ulong)size);
            return QueueMessage(p, size);
        }

        public unsafe Task<TrinityResponse> SendRecvMessageAsync(byte* message, int size)
        {
            void* p = Memory.malloc((ulong)size);
            Memory.memcpy(p, message, (ulong)size);
            return QueueMessageWithRsp(p, size).Item2;
        }

        public unsafe Task SendMessageAsync(byte** message, int* sizes, int count)
        {
            int size = Utils._sum(sizes, count);
            void* p = Memory.malloc((ulong)size);
            Utils._copy((byte*)p, message, sizes, count);
            return QueueMessage(p, size);
        }

        public unsafe Task<TrinityResponse> SendRecvMessageAsync(byte** message, int* sizes, int count)
        {
            int size = Utils._sum(sizes, count);
            void* p = Memory.malloc((ulong)size);
            Utils._copy((byte*)p, message, sizes, count);
            return QueueMessageWithRsp(p, size).Item2;
        }

        private unsafe Action<Task<bool>> _free(void* p) => _ => { Memory.free(p); };


        #region Unsupported storage interfaces
        public Task<bool> ContainsAsync(long cellId) => Task.FromResult(false);

        public unsafe Task<TrinityErrorCode> AddCellAsync(long cellId, byte* buff, int size, ushort cellType)
            => Task.FromResult(TrinityErrorCode.E_NOTSUPPORTED);

        public unsafe Task<TrinityErrorCode> UpdateCellAsync(long cellId, byte* buff, int size)
            => Task.FromResult(TrinityErrorCode.E_NOTSUPPORTED);

        public Task<(TrinityErrorCode, ushort)> GetCellTypeAsync(long cellId)
            => Task.FromResult((TrinityErrorCode.E_NOTSUPPORTED, (ushort)0));

        public Task<LoadCellResponse> LoadCellAsync(long cellId)
            => Task.FromResult(new LoadCellResponse(TrinityErrorCode.E_NOTSUPPORTED, null, 0));

        public unsafe Task<LoadCellUnsafeResponse> LoadCellUnsafeAsync(long cellId)
            => Task.FromResult(new LoadCellUnsafeResponse(TrinityErrorCode.E_NOTSUPPORTED, null, 0, 0));

        public Task<TrinityErrorCode> RemoveCellAsync(long cellId)
            => Task.FromResult(TrinityErrorCode.E_FAILURE);

        public unsafe Task<TrinityErrorCode> SaveCellAsync(long cellId, byte* buff, int size, ushort cellType)
            => Task.FromResult(TrinityErrorCode.E_FAILURE);
        #endregion
    }
}