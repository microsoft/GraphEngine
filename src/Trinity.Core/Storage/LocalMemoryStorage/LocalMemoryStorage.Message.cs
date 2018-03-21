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
using System.Runtime.CompilerServices;
using Trinity.Network;

namespace Trinity.Storage
{
    public unsafe partial class LocalMemoryStorage : IStorage
    {
        /// <inheritdoc/>
        public void SendMessage(byte* message, int size)
        {
            TrinityMessageType msgType = (TrinityMessageType)message[TrinityProtocol.MsgTypeOffset];
            ushort msgId = *(ushort*)(message + TrinityProtocol.MsgIdOffset);
            TrinityErrorCode msgProcessResult;

            switch (msgType)
            {
                case TrinityMessageType.PRESERVED_SYNC:
                    SynReqArgs preserved_sync_args = new SynReqArgs(message,
                        TrinityProtocol.MsgHeader, size - TrinityProtocol.MsgHeader,
                        MessageHandlers.DefaultParser.preserved_sync_handlers[msgId]);
                    msgProcessResult = preserved_sync_args.MessageProcess();
                    break;

                case TrinityMessageType.SYNC:
                    SynReqArgs sync_args = new SynReqArgs(message,
                        TrinityProtocol.MsgHeader,
                        size - TrinityProtocol.MsgHeader,
                        MessageHandlers.DefaultParser.sync_handlers[msgId]);
                    msgProcessResult = sync_args.MessageProcess();
                    break;

                case TrinityMessageType.PRESERVED_ASYNC:
                    {
                        AsynReqArgs aut_request = new AsynReqArgs(message,
                            TrinityProtocol.MsgHeader,
                            size - TrinityProtocol.MsgHeader,
                            MessageHandlers.DefaultParser.preserved_async_handlers[msgId]);
                        msgProcessResult = aut_request.AsyncProcessMessage();
                    }
                    break;
                case TrinityMessageType.ASYNC:
                    {
                        AsynReqArgs aut_request = new AsynReqArgs(message,
                            TrinityProtocol.MsgHeader,
                            size - TrinityProtocol.MsgHeader,
                            MessageHandlers.DefaultParser.async_handlers[msgId]);
                        msgProcessResult = aut_request.AsyncProcessMessage();
                    }
                    break;
                case TrinityMessageType.ASYNC_WITH_RSP:
                    {
                        AsynReqRspArgs async_rsp_args = new AsynReqRspArgs(message,
                            TrinityProtocol.MsgHeader,
                            size - TrinityProtocol.MsgHeader,
                            MessageHandlers.DefaultParser.async_rsp_handlers[msgId]);
                        msgProcessResult = async_rsp_args.AsyncProcessMessage();
                    }
                    break;
                default:
                    throw new IOException("Wrong message type.");
            }

            if (msgProcessResult == TrinityErrorCode.E_RPC_EXCEPTION)
            {
                throw new IOException("Local message handler throws an exception.");
            }
        }

        /// <inheritdoc/>
        public void SendMessage(byte* message, int size, out TrinityResponse response)
        {
            TrinityMessageType msgType = (TrinityMessageType)message[TrinityProtocol.MsgTypeOffset];
            ushort msgId = *(ushort*)(message+TrinityProtocol.MsgIdOffset);
            SynReqRspArgs sync_rsp_args;
            TrinityErrorCode msgProcessResult;

            if (msgType == TrinityMessageType.PRESERVED_SYNC_WITH_RSP)
            {
                sync_rsp_args = new SynReqRspArgs(message,
                    TrinityProtocol.MsgHeader,
                    size - TrinityProtocol.MsgHeader,
                    MessageHandlers.DefaultParser.preserved_sync_rsp_handlers[msgId]);
            }
            else// msgType == TrinityMessageType.SYNC_WITH_RSP
            {
                sync_rsp_args = new SynReqRspArgs(message,
                    TrinityProtocol.MsgHeader,
                    size - TrinityProtocol.MsgHeader,
                    MessageHandlers.DefaultParser.sync_rsp_handlers[msgId]);
            }
            msgProcessResult = sync_rsp_args.MessageProcess();
            if (msgProcessResult == TrinityErrorCode.E_SUCCESS)
            {
                response = new TrinityResponse(sync_rsp_args.Response);
            }
            else//  msgProcessResult == TrinityErrorCode.E_RPC_EXCEPTION
            {
                throw new IOException("Local message handler throws an exception.");
            }
        }

        /// <inheritdoc/>
        public void SendMessage(byte** message, int* sizes, int count)
        {
            byte* buf;
            int len;
            _serialize(message, sizes, count, out buf, out len);

            TrinityMessageType msgType = (TrinityMessageType)buf[TrinityProtocol.MsgTypeOffset];
            ushort msgId = *(ushort*)(buf + TrinityProtocol.MsgIdOffset);

            // For async messages, we omit the buffer copy, use the serialized buffer directly.
            switch (msgType)
            {
                case TrinityMessageType.ASYNC:
                    {
                        AsynReqArgs aut_request = new AsynReqArgs(
                            MessageHandlers.DefaultParser.async_handlers[msgId],
                            buf,
                            TrinityProtocol.MsgHeader,
                            len - TrinityProtocol.MsgHeader);
                        if (aut_request.AsyncProcessMessage() == TrinityErrorCode.E_RPC_EXCEPTION)
                        {
                            throw new IOException("Local message handler throws an exception.");
                        }
                    }
                    break;
                case TrinityMessageType.ASYNC_WITH_RSP:
                    {
                        AsynReqRspArgs async_rsp_args = new AsynReqRspArgs(
                            MessageHandlers.DefaultParser.async_rsp_handlers[msgId],
                            buf,
                            TrinityProtocol.MsgHeader,
                            len - TrinityProtocol.MsgHeader);
                        if (async_rsp_args.AsyncProcessMessage() == TrinityErrorCode.E_RPC_EXCEPTION)
                        {
                            throw new IOException("Local message handler throws an exception.");
                        }
                    }
                    break;
                default:
                    {
                        SendMessage(buf, len);
                        CMemory.C_free(buf);
                    }
                    break;
            }
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
