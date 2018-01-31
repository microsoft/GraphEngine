using System;
using System.Collections.Generic;
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
        private unsafe Queue<(long ptr, int size)> m_messages;
        private MemoryCloud m_mc;

        public ClientIStorage(MemoryCloud mc)
        {
            m_messages = new Queue<(long, int)>();
            m_mc = mc;
        }

        public unsafe void Dispose()
        {
            if (m_messages == null) return;
            lock (m_messages)
            {
                while(m_messages.Count != 0)
                {
                    void* ptr = (void*)m_messages.Dequeue().ptr;
                    Memory.free(ptr);
                }
            }
            m_messages = null;
        }

        public T GetCommunicationModule<T>() where T : CommunicationModule
            => m_mc.GetCommunicationModule<T>();

        private unsafe void QueueMessage(void* p, int size)
        {
            lock (m_messages)
            {
                m_messages.Enqueue(((long)p, size));
            }
        }

        private unsafe TrinityResponse GetResponse(void* p)
        {
            throw new NotImplementedException();
        }


        public unsafe void SendMessage(byte* message, int size)
        {
            void* p = Memory.malloc((ulong)size);
            Memory.memcpy(p, message, (ulong)size);
            QueueMessage(p, size);
        }

        public unsafe void SendMessage(byte* message, int size, out TrinityResponse response)
        {
            void* p = Memory.malloc((ulong)size);
            Memory.memcpy(p, message, (ulong)size);
            QueueMessage(p, size);
            response = GetResponse(p);
        }

        public unsafe void SendMessage(byte** message, int* sizes, int count)
        {
            int size = _sum(sizes, count);
            void* p = Memory.malloc((ulong)size);
            _copy((byte*)p, message, sizes, count);
            QueueMessage(p, size);
        }

        public unsafe void SendMessage(byte** message, int* sizes, int count, out TrinityResponse response)
        {
            int size = _sum(sizes, count);
            void* p = Memory.malloc((ulong)size);
            _copy((byte*)p, message, sizes, count);
            QueueMessage(p, size);
            response = GetResponse(p);
        }

        private unsafe void _copy(byte* p, byte** message, int* sizes, int count)
        {
            int* pend = sizes + count;
            int size;
            while(sizes != pend)
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
            while(sizes != pend)
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