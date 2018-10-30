// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Net;

using Trinity;
using Trinity.Network.Messaging;
using Trinity.Network.Sockets;
using Trinity.Core.Lib;
using Trinity.Network;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Trinity.Storage
{
    public unsafe partial class LocalMemoryStorage : IStorage
    {
        [DllImport(TrinityC.AssemblyName)]
        private static extern void LocalSendMessage(MessageBuff* lpmsg);

        /// <inheritdoc/>
        public void SendMessage(byte* message, int size)
        {
            MessageBuff buff = new MessageBuff();
            buff.Buffer = message;
            buff.Length = (uint)size;

            LocalSendMessage(&buff);

            if(buff.Length != sizeof(TrinityErrorCode))
            {
                throw new IOException("LocalSendMessage responds with unexpected payload");
            }

            TrinityErrorCode msgProcessResult = *(TrinityErrorCode*)buff.Buffer;

            if (msgProcessResult != TrinityErrorCode.E_SUCCESS)
            {
                throw new IOException("Local message handler throws an exception.");
            }
        }

        /// <inheritdoc/>
        public void SendMessage(byte* message, int size, out TrinityResponse response)
        {
            MessageBuff buff = new MessageBuff();
            buff.Buffer = message;
            buff.Length = (uint)size;

            LocalSendMessage(&buff);

            int response_len = *(int*)buff.Buffer;
            if(buff.Length != response_len + sizeof(int))
            {
                throw new IOException("Local message handler throws an exception.");
            }
            else
            {
                TrinityMessage rsp_message = new TrinityMessage(buff.Buffer, (int)buff.Length);
                response = new TrinityResponse(rsp_message);
            }
        }

        /// <inheritdoc/>
        public void SendMessage(byte** message, int* sizes, int count)
        {
            byte* buf;
            int len;
            _serialize(message, sizes, count, out buf, out len);
            SendMessage(buf, len);
            CMemory.C_free(buf);
        }

        /// <inheritdoc/>
        public void SendMessage(byte** message, int* sizes, int count, out TrinityResponse response)
        {
            byte* buf;
            int len;
            _serialize(message, sizes, count, out buf, out len);
            SendMessage(buf, len, out response);
            CMemory.C_free(buf);
        }

        /// <inheritdoc/>
        public T GetCommunicationModule<T>() where T: CommunicationModule
        {
            return Global.CommunicationInstance.GetCommunicationModule<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void _serialize(byte** message, int* sizes, int count, out byte* buf, out int len)
        {
            len = 0;
            for (int i=0; i<count; ++i)
            {
                len += sizes[i];
            }

            buf = (byte*)CMemory.C_malloc((ulong)len);
            byte* p = buf;
            for (int i=0; i<count; ++i)
            {
                CMemory.C_memcpy((void*)p, (void*)*message, (ulong)*sizes);
                p += *sizes;
                ++message;
                ++sizes;
            }
        }
    }
}
