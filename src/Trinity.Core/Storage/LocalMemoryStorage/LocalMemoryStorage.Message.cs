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

namespace Trinity.Storage
{
    public unsafe partial class LocalMemoryStorage : Storage
    {
        protected internal override void SendMessage(TrinityMessage message)
        {
            SendMessage(message.Buffer, message.Size);
        }

        protected internal override void SendMessage(TrinityMessage message, out TrinityResponse response)
        {
            SendMessage(message.Buffer, message.Size, out response);
        }

        protected internal override void SendMessage(byte* message, int size)
        {
            TrinityMessageType msgType = (TrinityMessageType)message[TrinityProtocol.MsgTypeOffset];
            int msgId = message[TrinityProtocol.MsgIdOffset];
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
                default:
                    throw new IOException("Wrong message type.");
            }

            if(msgProcessResult == TrinityErrorCode.E_RPC_EXCEPTION)
            {
                throw new IOException("Local message handler throws an exception.");
            }
        }

        protected internal override void SendMessage(byte* message, int size, out TrinityResponse response)
        {
            TrinityMessageType msgType = (TrinityMessageType)message[TrinityProtocol.MsgTypeOffset];
            byte msgId = message[TrinityProtocol.MsgIdOffset];
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
    }
}
