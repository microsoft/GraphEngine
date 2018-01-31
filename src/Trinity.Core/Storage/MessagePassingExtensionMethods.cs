using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Core.Lib;
using Trinity.Network;
using Trinity.Network.Messaging;

namespace Trinity.Storage
{
    public static unsafe class MessagePassingExtensionMethods
    {
        public static void SendMessage(this IMessagePassingEndpoint storage, TrinityMessage message)
        {
            storage.SendMessage(message.Buffer, message.Size);
        }

        public static void SendMessage(this IMessagePassingEndpoint storage, TrinityMessage message, out TrinityResponse response)
        {
            storage.SendMessage(message.Buffer, message.Size, out response);
        }

        public static void SendMessage<T>(this IMessagePassingEndpoint storage, byte* message, int size)
            where T : CommunicationModule
        {
            storage.GetCommunicationModule<T>().SendMessage(storage, message, size);
        }

        public static void SendMessage<T>(this IMessagePassingEndpoint storage, byte* message, int size, out TrinityResponse response)
            where T : CommunicationModule
        {
            storage.GetCommunicationModule<T>().SendMessage(storage, message, size, out response);
        }

        public static void SendMessage<T>(this IMessagePassingEndpoint storage, byte** message, int* sizes, int count)
            where T : CommunicationModule
        {
            storage.GetCommunicationModule<T>().SendMessage(storage, message, sizes, count);
        }

        public static void SendMessage<T>(this IMessagePassingEndpoint storage, byte** message, int* sizes, int count, out TrinityResponse response)
            where T : CommunicationModule
        {
            storage.GetCommunicationModule<T>().SendMessage(storage, message, sizes, count, out response);
        }

        internal static void GetCommunicationSchema(this IMessagePassingEndpoint storage, out string name, out string signature)
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
                storage.SendMessage(tm, out response);
                PointerHelper sp     = PointerHelper.New(response.Buffer + response.Offset);
                int name_string_len = *sp.ip++;
                name                = BitHelper.GetString(sp.bp, name_string_len * 2);
                sp.cp              += name_string_len;
                int sig_string_len  = *sp.ip++;
                signature           = BitHelper.GetString(sp.bp, sig_string_len * 2);

                response.Dispose();
            }
        }

        internal static bool GetCommunicationModuleOffset(this IMessagePassingEndpoint storage, string moduleName, out ushort synReqOffset, out ushort synReqRspOffset, out ushort asynReqOffset, out ushort asynReqRspOffset)
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
                storage.SendMessage(tm, out response);
                bool ret = (response.ErrorCode == TrinityErrorCode.E_SUCCESS);
                if (ret)
                {
                    sp.bp             = response.Buffer + response.Offset;
                    int synReq_msg    = *sp.ip++;
                    int synReqRsp_msg = *sp.ip++;
                    int asynReq_msg   = *sp.ip++;
                    int asynReqRsp_msg= *sp.ip++;

                    synReqOffset      = (ushort)synReq_msg;
                    synReqRspOffset   = (ushort)synReqRsp_msg;
                    asynReqOffset     = (ushort)asynReq_msg;
                    asynReqRspOffset  = (ushort)asynReqRsp_msg;
                }
                else
                {
                    synReqOffset      = 0;
                    synReqRspOffset   = 0;
                    asynReqOffset     = 0;
                    asynReqRspOffset  = 0;
                }


                response.Dispose();
                return ret;
            }
        }
    }
}
