using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
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

        public unsafe void Dispose()
        {
            if (m_messages == null) return;
            while (m_messages.TryDequeue(out var msg))
            {
                Memory.free((void*)msg.Buffer);
            }
            m_messages = null;
        }

        public T GetCommunicationModule<T>() where T : CommunicationModule
            => m_mc.GetCommunicationModule<T>();

        private unsafe Task QueueMessage(void* p, int size)
        {
            TaskCompletionSource<bool> req_token = new TaskCompletionSource<bool>();
            m_req_tokens[(long)p] = req_token;
            TrinityMessage tm = new TrinityMessage((byte*)p, size);
            m_messages.Enqueue(tm);
            return req_token.Task.ContinueWith(_free(p));
        }

        private unsafe (Task, Task<TrinityResponse>) QueueMessageWithRsp(void* p, int size)
        {
            TaskCompletionSource<bool> req_token = new TaskCompletionSource<bool>();
            TaskCompletionSource<TrinityResponse> rsp_token = new TaskCompletionSource<TrinityResponse>();
            m_req_tokens[(long)p] = req_token;
            m_rsp_tokens[(long)p] = rsp_token;
            TrinityMessage tm = new TrinityMessage((byte*)p, size);
            m_messages.Enqueue(tm);
            return (req_token.Task.ContinueWith(_free(p)), rsp_token.Task);
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

        public unsafe void SendMessage(byte* message, int size)
        {
            void* p = Memory.malloc((ulong)size);
            Memory.memcpy(p, message, (ulong)size);
            QueueMessage(p, size).Wait();
        }

        public unsafe void SendMessage(byte* message, int size, out TrinityResponse response)
        {
            void* p = Memory.malloc((ulong)size);
            Memory.memcpy(p, message, (ulong)size);
            var (treq, trsp) = QueueMessageWithRsp(p, size);
            treq.Wait();
            response = trsp.Result;
        }

        public unsafe void SendMessage(byte** message, int* sizes, int count)
        {
            int size = _sum(sizes, count);
            void* p = Memory.malloc((ulong)size);
            _copy((byte*)p, message, sizes, count);
            QueueMessage(p, size).Wait();
        }

        public unsafe void SendMessage(byte** message, int* sizes, int count, out TrinityResponse response)
        {
            int size = _sum(sizes, count);
            void* p = Memory.malloc((ulong)size);
            _copy((byte*)p, message, sizes, count);
            var (treq, trsp) = QueueMessageWithRsp(p, size);
            treq.Wait();
            response = trsp.Result;
        }

        private unsafe Action<Task<bool>> _free(void* p) => _ => { Memory.free(p); };

        private unsafe void _copy(byte* p, byte** message, int* sizes, int count)
        {
            int* pend = sizes + count;
            int size;
            while (sizes != pend)
            {
                size = *sizes;
                Memory.memcpy(p, *message, (ulong)size);
                ++sizes;
                ++message;
                p += size;
            }
        }

        private unsafe int _sum(int* sizes, int count)
        {
            int size = 0;
            int* pend = sizes + count;
            while (sizes != pend)
            {
                size += *sizes;
                ++sizes;
            }
            return size;
        }

        #region Unsupported storage interfaces
        public bool Contains(long cellId) => false;

        public unsafe TrinityErrorCode AddCell(long cellId, byte* buff, int size, ushort cellType)
            => TrinityErrorCode.E_FAILURE;

        public unsafe TrinityErrorCode UpdateCell(long cellId, byte* buff, int size)
            => TrinityErrorCode.E_FAILURE;

        public TrinityErrorCode GetCellType(long cellId, out ushort cellType)
        {
            cellType = 0;
            return TrinityErrorCode.E_FAILURE;
        }

        public TrinityErrorCode LoadCell(long cellId, out byte[] cellBuff, out ushort cellType)
        {
            cellBuff = null;
            cellType = 0;
            return TrinityErrorCode.E_FAILURE;
        }

        public TrinityErrorCode RemoveCell(long cellId)
            => TrinityErrorCode.E_FAILURE;

        public unsafe TrinityErrorCode SaveCell(long cellId, byte* buff, int size, ushort cellType)
            => TrinityErrorCode.E_FAILURE;
        #endregion
    }
}