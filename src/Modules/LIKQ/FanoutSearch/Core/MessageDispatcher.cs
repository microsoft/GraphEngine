// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using FanoutSearch.Protocols.TSL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity;
using System.Collections.Concurrent;
using Trinity.Core.Lib;
using Trinity.Network.Messaging;
using System.Threading;
using System.Runtime.CompilerServices;

namespace FanoutSearch
{
    unsafe class MessageDispatcher : IDisposable
    {
        static int s_ServerCount;
        static FanoutSearchModule s_Module;

        const int c_default_capacity = 64;
        const int c_realloc_threshold = 1 << 20;
        const int c_realloc_step_size = 1 << 20;

        static MessageDispatcher()
        {
            s_ServerCount = Global.CloudStorage.ServerCount;
            s_Module = Global.CloudStorage.GetCommunicationModule<FanoutSearchModule>();
        }
        public MessageDispatcher(int hop, int transaction)
        {
            this.hop           = hop;
            this.transaction   = transaction;
            this.serverCount   = s_ServerCount;
            buffer             = new byte*[serverCount];
            buffer_len         = new int[serverCount];
            buffer_size        = new int[serverCount];
            buffer_locks       = new object[serverCount];
            int default_size = c_default_capacity * sizeof(long) + TrinityProtocol.MsgHeader + sizeof(int)*2;

            for (int i = 0; i < serverCount; ++i)
            {
                buffer[i] = (byte*)Memory.malloc((uint)default_size);
                buffer_len[i] = sizeof(int) * 2 + TrinityProtocol.MsgHeader;
                buffer_size[i] = default_size;
                buffer_locks[i] = new object();
            }
        }

        public unsafe void addAugmentedPath(long* pathptr, int current_hop, long next_cell)
        {
            int slaveID = Global.CloudStorage.GetServerIdByCellId(next_cell);
            int path_size = sizeof(long) * (current_hop + 2);

            // not lock free, but easy to read through.
            lock (buffer_locks[slaveID])
            {
                buffer_len[slaveID] += path_size;
                int buffer_offset = buffer_len[slaveID];
                while (buffer_offset > buffer_size[slaveID])
                {
                    int new_size = buffer_size[slaveID];

                    if (new_size < c_realloc_threshold) new_size *= 2;
                    else new_size += c_realloc_step_size;

                    byte* new_buf = (byte*)Memory.malloc((uint)new_size);
                    Memory.memcpy(new_buf, buffer[slaveID], (uint)buffer_size[slaveID]);
                    Memory.free(buffer[slaveID]);

                    buffer[slaveID] = new_buf;
                    buffer_size[slaveID] = new_size;
                }
                long* add_ptr = (long*)(buffer[slaveID] + buffer_offset - path_size);
                Memory.memcpy(add_ptr, pathptr, (uint)(current_hop + 1) * sizeof(long));
                add_ptr[current_hop + 1] = next_cell;
            }
        }

        private byte*[] buffer;
        private int[] buffer_len;
        private int[] buffer_size;
        private object[] buffer_locks;
        private int hop;
        private int transaction;
        private int serverCount;

        public void Dispatch()
        {
            if (buffer == null)
                return;

            Parallel.For(0, serverCount, serverId =>
            {
                var bufferPtr = this.buffer[serverId];
                FillHeader(hop, transaction, buffer_len[serverId], bufferPtr);

                s_Module.FanoutSearch_impl_Send(serverId, bufferPtr, buffer_len[serverId]);
                Memory.free(bufferPtr);
            });

            buffer = null;
        }

        public static unsafe void DispatchOriginMessage(int serverId, int transaction, IEnumerable<FanoutPathDescriptor> origins)
        {
            int origin_cnt  = origins.Count();
            int msg_size    = (TrinityProtocol.MsgHeader + 3 * sizeof(int) + origin_cnt * sizeof(long));
            byte* bufferPtr = (byte*)Memory.malloc((uint)msg_size);

            byte* body = FillHeader(0, transaction, msg_size, bufferPtr);

            long* originPtr = (long*)(body + 2 * sizeof(int));
            foreach (var origin_desc in origins)
            {
                *originPtr++ = origin_desc.hop_0;
            }

            s_Module.FanoutSearch_impl_Send(serverId, bufferPtr, msg_size);
            Memory.free(bufferPtr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe byte* FillHeader(int hop, int transaction, int msg_size, byte* bufferPtr)
        {
            byte* body      = bufferPtr + TrinityProtocol.MsgHeader;

            *(bufferPtr + TrinityProtocol.MsgTypeOffset) = (byte)TrinityMessageType.ASYNC;
            *(ushort*)(bufferPtr + TrinityProtocol.MsgIdOffset) = (ushort)global::FanoutSearch.Protocols.TSL.TSL.CommunicationModule.FanoutSearch.AsynReqMessageType.FanoutSearch_impl;
            *(int*)bufferPtr = msg_size - sizeof(int);
            *(int*)(body) = hop;
            *(int*)(body + sizeof(int)) = transaction;
            return body;
        }

        public void Dispose()
        {
            if (buffer != null)
                Dispatch();
        }

    }
}