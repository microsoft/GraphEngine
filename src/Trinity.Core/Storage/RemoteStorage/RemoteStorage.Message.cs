// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

using Trinity;
using Trinity.Network;
using Trinity.Network.Client;
using Trinity.Network.Messaging;
using System.Diagnostics;
using Trinity.Core.Lib;

namespace Trinity.Storage
{
    internal unsafe partial class RemoteStorage : Storage, IDisposable
    {
        /// <summary>
        /// Checks for possible errors that occur during message passing, 
        /// and throw exceptions accordingly.
        /// </summary>
        private static void _error_check(TrinityErrorCode err)
        {
            switch (err)
            {
                case TrinityErrorCode.E_NOMEM:
                    throw new OutOfMemoryException("Cannot allocate memory for response message.");
                case TrinityErrorCode.E_RPC_EXCEPTION:
                    throw new IOException("Remote message handler throws an exception.");
                case TrinityErrorCode.E_NETWORK_SEND_FAILURE:
                case TrinityErrorCode.E_NETWORK_RECV_FAILURE:
                    throw new IOException("Network errors occur.");
            }
        }

        /// <summary>
        /// Get a SynClient and apply the function. Intended for SendMessage calls.
        /// Will retry if the function returns neither E_SUCCESS nor E_RPC_EXCEPTION.
        /// </summary>
        private void _use_synclient(Func<SynClient, TrinityErrorCode> func)
        {
            TrinityErrorCode err = TrinityErrorCode.E_SUCCESS;
            for (int i = 0; i < m_send_retry; i++)
            {
                SynClient sc = GetClient();
                err = func(sc);
                PutBackClient(sc);
                if (err == TrinityErrorCode.E_SUCCESS || err == TrinityErrorCode.E_RPC_EXCEPTION)
                    break;
            }
            _error_check(err);
        }

        internal override void SendMessage(TrinityMessage msg)
        {
            SendMessage(msg.Buffer, msg.Size);
        }


        internal override void SendMessage(TrinityMessage msg, out TrinityResponse response)
        {
            SendMessage(msg.Buffer, msg.Size, out response);
        }

        internal override void SendMessage(byte* message, int size)
        {
            _use_synclient(sc => sc.SendMessage(message, size));
        }

        internal override void SendMessage(byte* message, int size, out TrinityResponse response)
        {
            TrinityResponse _rsp = null;
            _use_synclient(sc => sc.SendMessage(message, size, out _rsp));
            response = _rsp;
        }

        internal override void SendMessage(byte** message, int* sizes, int count)
        {
            _use_synclient(sc => sc.SendMessage(message, sizes, count));
        }

        internal override void SendMessage(byte** message, int* sizes, int count, out TrinityResponse response)
        {
            TrinityResponse _rsp = null;
            _use_synclient(sc => sc.SendMessage(message, sizes, count, out _rsp));
            response = _rsp;
        }

        internal void GetCommunicationSchema(out string name, out string signature)
        {
            /******************
             * Comm. protocol:
             *  - REQUEST : VOID
             *  - RESPONSE: [char_cnt, char[] name, char_cnt, char[] sig]
             ******************/
            using (TrinityMessage tm = new TrinityMessage(
                TrinityMessageType.PRESERVED_SYNC_WITH_RSP,
                (ushort)RequestType.GetCommunicationSchema,
                size: 0))
            {
                TrinityResponse response;
                this.SendMessage(tm, out response);
                PointerHelper sp     = PointerHelper.New(response.Buffer + response.Offset);
                int name_string_len = *sp.ip++;
                name                = BitHelper.GetString(sp.bp, name_string_len * 2);
                sp.cp              += name_string_len;
                int sig_string_len  = *sp.ip++;
                signature           = BitHelper.GetString(sp.bp, sig_string_len * 2);

                response.Dispose();
            }
        }

        internal bool GetCommunicationModuleOffset(string moduleName, out ushort synReqOffset, out ushort synReqRspOffset, out ushort asynReqOffset, out ushort asynReqRspOffset)
        {
            /******************
             * Comm. protocol:
             *  - REQUEST : [char_cnt, char[] moduleName]
             *  - RESPONSE: [int synReqOffset, int synReqRspOffset, int asynReqOffset, int asynReqRspOffset]
             * An response error code other than E_SUCCESS indicates failure of remote module lookup.
             ******************/

            using (TrinityMessage tm = new TrinityMessage(
                TrinityMessageType.PRESERVED_SYNC_WITH_RSP,
                (ushort)RequestType.GetCommunicationModuleOffsets,
                size: sizeof(int) + sizeof(char) * moduleName.Length))
            {
                PointerHelper sp = PointerHelper.New(tm.Buffer + TrinityMessage.Offset);
                *sp.ip++         = moduleName.Length;

                BitHelper.WriteString(moduleName, sp.bp);
                TrinityResponse response;
                this.SendMessage(tm, out response);

                sp.bp             = response.Buffer + response.Offset;
                int synReq_msg    = *sp.ip++;
                int synReqRsp_msg = *sp.ip++;
                int asynReq_msg   = *sp.ip++;
                int asynReqRsp_msg= *sp.ip++;

                synReqOffset      = (ushort)synReq_msg;
                synReqRspOffset   = (ushort)synReqRsp_msg;
                asynReqOffset     = (ushort)asynReq_msg;
                asynReqRspOffset  = (ushort)asynReqRsp_msg;

                return (response.ErrorCode == TrinityErrorCode.E_SUCCESS);
            }
        }
    }
}
