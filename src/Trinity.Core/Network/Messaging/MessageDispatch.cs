// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;

using Trinity.Network.Messaging;
using Trinity.Utilities;
using Trinity;
using Trinity.Network.Sockets;
using Trinity.Diagnostics;
using System.Diagnostics;
using Trinity.Core.Lib;
using System.Runtime.CompilerServices;


namespace Trinity.Network.Sockets
{
    internal struct TypeSyncRequestResponseHandlerTuple
    {
        public ushort Id;
        public SynReqRspHandler Handler;
    }

    internal struct TypeSyncRequestHandlerTuple
    {
        public ushort Id;
        public SynReqHandler Handler;
    }


    internal struct TypeAsyncRequestHandlerTuple
    {
        public ushort Id;
        public AsyncReqHandler Handler;
    }


    unsafe class MessageHandlers
    {
        /// <summary>
        /// We need to guarantee that all handlers are registered before accepting requests
        /// </summary>
        static MessageHandlers()
        {
            _message_parser = new MessageHandlers();
        }
        /// <summary>
        /// Used to trigger static constructor.
        /// </summary>
        internal static void Initialize()
        {
        }

        private static MessageHandlers _message_parser;
        public static MessageHandlers DefaultParser
        {
            get
            {
                return _message_parser;
            }
        }

        /// <summary>
        /// Handler table for synchronous messages with responseBuff
        /// </summary>
        internal SynReqRspHandler[] sync_rsp_handlers = new SynReqRspHandler[ushort.MaxValue + 1];
        internal SynReqRspHandler[] preserved_sync_rsp_handlers = new SynReqRspHandler[ushort.MaxValue + 1];

        /// <summary>
        /// Handler table for synchronous messages without responseBuff
        /// </summary>
        internal SynReqHandler[] sync_handlers = new SynReqHandler[ushort.MaxValue + 1];
        internal SynReqHandler[] preserved_sync_handlers = new SynReqHandler[ushort.MaxValue + 1];

        /// <summary>
        /// Handler table for asynchronous message without responseBuff
        /// </summary>
        internal AsyncReqHandler[] async_handlers = new AsyncReqHandler[ushort.MaxValue + 1];
        internal AsyncReqHandler[] preserved_async_handlers = new AsyncReqHandler[ushort.MaxValue + 1];

        /// <summary>
        /// Handler table for asynchronous message with responseBuff
        /// </summary>
        internal AsyncReqRspHandler[] async_rsp_handlers = new AsyncReqRspHandler[ushort.MaxValue + 1];

        public MessageHandlers()
        {
            for (int i = 0; i <= ushort.MaxValue; ++i)
            {
                sync_rsp_handlers[i] = null;
                sync_handlers[i] = null;
                async_handlers[i] = null;
                async_rsp_handlers[i] = null;

                preserved_sync_rsp_handlers[i] = null;
                preserved_sync_handlers[i] = null;
                preserved_async_handlers[i] = null;
            }

            //install default synchronous message without response handlers
            for (int i = 0; i < DefaultSyncReqHandlerSet.MessageHandlerList.Count; ++i)
            {
                preserved_sync_handlers[DefaultSyncReqHandlerSet.MessageHandlerList[i].Id] = DefaultSyncReqHandlerSet.MessageHandlerList[i].Handler;
                Log.WriteLine(LogLevel.Debug, "Preserved sync message " + (RequestType)DefaultSyncReqHandlerSet.MessageHandlerList[i].Id + " is registered.");
            }

            //install the default synchronous request with response handlers
            for (int i = 0; i < DefaultSyncReqRspHandlerSet.MessageHandlerList.Count; ++i)
            {
                preserved_sync_rsp_handlers[DefaultSyncReqRspHandlerSet.MessageHandlerList[i].Id] = DefaultSyncReqRspHandlerSet.MessageHandlerList[i].Handler;
                Log.WriteLine(LogLevel.Debug, "Preserved sync (rsp) message " + (RequestType)DefaultSyncReqRspHandlerSet.MessageHandlerList[i].Id + " is registered.");
            }

            //install default asynchronous request handlers
            for (int i = 0; i < DefaultAsynReqHandlerSet.MessageHandlerList.Count; ++i)
            {
                preserved_async_handlers[DefaultAsynReqHandlerSet.MessageHandlerList[i].Id] = DefaultAsynReqHandlerSet.MessageHandlerList[i].Handler;
                Log.WriteLine(LogLevel.Debug, "Preserved async message " + (RequestType)DefaultAsynReqHandlerSet.MessageHandlerList[i].Id + " is registered.");
            }
        }

        public bool RegisterMessageHandler(ushort msgId, SynReqRspHandler message_handler)
        {
            try
            {
                sync_rsp_handlers[msgId] = message_handler;
                Log.WriteLine(LogLevel.Debug, "Sync (rsp) message " + msgId + " is registered.");
                return true;
            }
            catch (Exception ex)
            {
                Trinity.Diagnostics.Log.WriteLine(ex.Message);
                Trinity.Diagnostics.Log.WriteLine(ex.StackTrace);
                return false;
            }
        }

        public bool RegisterPreservedMessageHandler(ushort msgId, SynReqRspHandler message_handler)
        {
            try
            {
                preserved_sync_rsp_handlers[msgId] = message_handler;
                Log.WriteLine(LogLevel.Debug, "Preserved sync (rsp) message " + msgId + " is registered.");
                return true;
            }
            catch (Exception ex)
            {
                Trinity.Diagnostics.Log.WriteLine(ex.Message);
                Trinity.Diagnostics.Log.WriteLine(ex.StackTrace);
                return false;
            }
        }

        public bool RegisterMessageHandler(ushort msgId, SynReqHandler message_handler)
        {
            try
            {
                sync_handlers[msgId] = message_handler;
                Log.WriteLine(LogLevel.Debug, "Sync message " + msgId + " is registered.");
                return true;
            }
            catch (Exception ex)
            {
                Trinity.Diagnostics.Log.WriteLine(ex.Message);
                Trinity.Diagnostics.Log.WriteLine(ex.StackTrace);
                return false;
            }
        }

        public bool RegisterPreservedMessageHandler(ushort msgId, SynReqHandler message_handler)
        {
            try
            {
                preserved_sync_handlers[msgId] = message_handler;
                Log.WriteLine(LogLevel.Debug, "Preserved sync message " + msgId + " is registered.");
                return true;
            }
            catch (Exception ex)
            {
                Trinity.Diagnostics.Log.WriteLine(ex.Message);
                Trinity.Diagnostics.Log.WriteLine(ex.StackTrace);
                return false;
            }
        }

        public bool RegisterMessageHandler(ushort msgId, AsyncReqHandler request_handler)
        {
            try
            {
                async_handlers[msgId] = request_handler;
                Log.WriteLine(LogLevel.Debug, "Async message " + msgId + " is registered.");
                return true;
            }
            catch (Exception ex)
            {
                Trinity.Diagnostics.Log.WriteLine(ex.Message);
                Trinity.Diagnostics.Log.WriteLine(ex.StackTrace);
                return false;
            }
        }

        public bool RegisterPreservedMessageHandler(ushort msgId, AsyncReqHandler request_handler)
        {
            try
            {
                preserved_async_handlers[msgId] = request_handler;
                Log.WriteLine(LogLevel.Debug, "Preserved async message " + msgId + " is registered.");
                return true;
            }
            catch (Exception ex)
            {
                Trinity.Diagnostics.Log.WriteLine(ex.Message);
                Trinity.Diagnostics.Log.WriteLine(ex.StackTrace);
                return false;
            }
        }

        public bool RegisterMessageHandler(ushort msgId, AsyncReqRspHandler request_handler)
        {
            try
            {
                async_rsp_handlers[msgId] = request_handler;
                Log.WriteLine(LogLevel.Debug, "Async with response message " + msgId + " is registered.");
                return true;
            }
            catch (Exception ex)
            {
                Trinity.Diagnostics.Log.WriteLine(ex.Message);
                Trinity.Diagnostics.Log.WriteLine(ex.StackTrace);
                return false;
            }
        }

        #region _SetSendRecvBuff helpers
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void _SetSendRecvBuff(TrinityErrorCode msgProcessResult, MessageBuff* sendRecvBuff, TrinityMessage response)
        {
            if (TrinityErrorCode.E_SUCCESS == msgProcessResult)
            {
                // Response buffer will be freed in Trinity.C after it is sent
                sendRecvBuff->Buffer                     = response.Buffer;
                sendRecvBuff->Length                     = (uint)response.Size;
            }
            else// TrinityErrorCode.E_RPC_EXCEPTION == msgProcessResult
            {
                //  The client is expecting a reply payload, it will be notified because
                //  the payload length is E_RPC_EXCEPTION;
                sendRecvBuff->Buffer                     = (byte*)Memory.malloc(sizeof(TrinityErrorCode));
                sendRecvBuff->Length                     = sizeof(TrinityErrorCode);
                *(TrinityErrorCode*)sendRecvBuff->Buffer = TrinityErrorCode.E_RPC_EXCEPTION;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void _SetSendRecvBuff(TrinityErrorCode msgProcessResult, MessageBuff* sendRecvBuff)
        {
            if (TrinityErrorCode.E_SUCCESS == msgProcessResult)
            {
                //  Response buffer will be freed in Trinity.C after it is sent
                sendRecvBuff->Buffer                     = (byte*)Memory.malloc(sizeof(TrinityErrorCode));
                sendRecvBuff->Length                     = sizeof(TrinityErrorCode);
                *(TrinityErrorCode*)sendRecvBuff->Buffer = TrinityErrorCode.E_SUCCESS;
            }
            else//  TrinityErrorCode.E_RPC_EXCEPTION == msgProcessResult
            {
                //  The client is expecting an ACK package, it will be notified because
                //  the status code would be E_RPC_EXCEPTION or E_MSG_OVERFLOW;
                sendRecvBuff->Buffer                     = (byte*)Memory.malloc(sizeof(TrinityErrorCode));
                sendRecvBuff->Length                     = sizeof(TrinityErrorCode);
                *(TrinityErrorCode*)sendRecvBuff->Buffer = msgProcessResult;
            }
        }
        #endregion

        internal void DispatchMessage(MessageBuff* sendRecvBuff)
        {
            byte* ByteArray = sendRecvBuff->Buffer;
            int Length = (int)sendRecvBuff->Length;

            TrinityMessageType msgType = *(TrinityMessageType*)(sendRecvBuff->Buffer+TrinityProtocol.TrinityMsgTypeOffset);
            ushort msgId = *(ushort*)(ByteArray + TrinityProtocol.TrinityMsgIdOffset);
            TrinityErrorCode msgProcessResult;

            try
            {
                switch (msgType)
                {
                    case TrinityMessageType.SYNC_WITH_RSP:
                        SynReqRspArgs sync_rsp_args = new SynReqRspArgs(ByteArray,
                            TrinityProtocol.TrinityMsgHeader,
                            Length - TrinityProtocol.TrinityMsgHeader,
                            sync_rsp_handlers[msgId]);
                        msgProcessResult = sync_rsp_args.MessageProcess();
                        _SetSendRecvBuff(msgProcessResult, sendRecvBuff, sync_rsp_args.Response);
                        return;

                    case TrinityMessageType.PRESERVED_SYNC_WITH_RSP:
                        SynReqRspArgs preserved_sync_rsp_args = new SynReqRspArgs(ByteArray,
                            TrinityProtocol.TrinityMsgHeader,
                            Length - TrinityProtocol.TrinityMsgHeader,
                            preserved_sync_rsp_handlers[msgId]);
                        msgProcessResult = preserved_sync_rsp_args.MessageProcess();
                        _SetSendRecvBuff(msgProcessResult, sendRecvBuff, preserved_sync_rsp_args.Response);
                        return;

                    case TrinityMessageType.SYNC:
                        SynReqArgs sync_args = new SynReqArgs(ByteArray,
                            TrinityProtocol.TrinityMsgHeader,
                            Length - TrinityProtocol.TrinityMsgHeader,
                            sync_handlers[msgId]);
                        msgProcessResult = sync_args.MessageProcess();
                        _SetSendRecvBuff(msgProcessResult, sendRecvBuff);
                        return;
                    case TrinityMessageType.PRESERVED_SYNC:
                        SynReqArgs preserved_sync_args = new SynReqArgs(ByteArray,
                            TrinityProtocol.TrinityMsgHeader,
                            Length - TrinityProtocol.TrinityMsgHeader,
                            preserved_sync_handlers[msgId]);
                        msgProcessResult = preserved_sync_args.MessageProcess();
                        _SetSendRecvBuff(msgProcessResult, sendRecvBuff);
                        return;

                    case TrinityMessageType.ASYNC:
                        AsynReqArgs async_args = new AsynReqArgs(ByteArray,
                            TrinityProtocol.TrinityMsgHeader,
                            Length - TrinityProtocol.TrinityMsgHeader,
                            async_handlers[msgId]);
                        msgProcessResult = async_args.AsyncProcessMessage();
                        _SetSendRecvBuff(msgProcessResult, sendRecvBuff);
                        return;
                    case TrinityMessageType.PRESERVED_ASYNC:
                        AsynReqArgs preserved_async_args = new AsynReqArgs(ByteArray,
                            TrinityProtocol.TrinityMsgHeader,
                            Length - TrinityProtocol.TrinityMsgHeader,
                            preserved_async_handlers[msgId]);
                        msgProcessResult = preserved_async_args.AsyncProcessMessage();
                        _SetSendRecvBuff(msgProcessResult, sendRecvBuff);
                        return;

                    case TrinityMessageType.ASYNC_WITH_RSP:
                        AsynReqRspArgs async_rsp_args = new AsynReqRspArgs(ByteArray,
                            TrinityProtocol.TrinityMsgHeader,
                            Length - TrinityProtocol.TrinityMsgHeader,
                            async_rsp_handlers[msgId]);
                        msgProcessResult = async_rsp_args.AsyncProcessMessage();
                        _SetSendRecvBuff(msgProcessResult, sendRecvBuff);
                        return;

                    default:
                        throw new Exception("Not recognized message type.");
                }
            }
            catch (MessageTooLongException ex)
            {
                Log.WriteLine("Message Type: " + msgType);
                Log.WriteLine("Message SN: " + msgId);

                Log.WriteLine(ex.Message);
                Log.WriteLine(ex.StackTrace);
                _SetSendRecvBuff(TrinityErrorCode.E_MSG_OVERFLOW, sendRecvBuff);
                return;
            }
            catch (Exception ex)
            {
                Log.WriteLine("Message Type: " + msgType);
                Log.WriteLine("Message SN: " + msgId);

                Log.WriteLine(ex.Message);
                Log.WriteLine(ex.StackTrace);
                _SetSendRecvBuff(TrinityErrorCode.E_RPC_EXCEPTION, sendRecvBuff);
                return;
            }
        }
    }
}
