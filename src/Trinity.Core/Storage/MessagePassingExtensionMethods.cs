using System;
using System.Threading.Tasks;
using Trinity.Core.Lib;
using Trinity.Network;
using Trinity.Network.Messaging;

namespace Trinity.Storage
{
    public static unsafe class MessagePassingExtensionMethods
    {
        public static Task SendMessageAsync(this IMessagePassingEndpoint storage, TrinityMessage message)
        {
            return storage.SendMessageAsync(message.Buffer, message.Size);
        }

        public static Task<TrinityResponse> SendRecvMessageAsync(this IMessagePassingEndpoint storage, TrinityMessage message)
        {
            return storage.SendRecvMessageAsync(message.Buffer, message.Size);
        }

        public static Task SendMessageAsync<T>(this IMessagePassingEndpoint storage, byte* message, int size)
            where T : CommunicationModule
        {
            return storage.GetCommunicationModule<T>().SendMessageAsync(storage, message, size);
        }

        public static Task<TrinityResponse> SendRecvMessageAsync<T>(this IMessagePassingEndpoint storage, byte* message, int size)
            where T : CommunicationModule
        {
            return storage.GetCommunicationModule<T>().SendRecvMessageAsync(storage, message, size);
        }

        public static Task SendMessageAsync<T>(this IMessagePassingEndpoint storage, byte** message, int* sizes, int count)
            where T : CommunicationModule
        {
            return storage.GetCommunicationModule<T>().SendMessageAsync(storage, message, sizes, count);
        }

        public static Task<TrinityResponse> SendRecvMessageAsync<T>(this IMessagePassingEndpoint storage, byte** message, int* sizes, int count)
            where T : CommunicationModule
        {
            return storage.GetCommunicationModule<T>().SendRecvMessageAsync(storage, message, sizes, count);
        }

        internal static Task<(string Name, string Signature)> GetCommunicationSchemaAsync(this IMessagePassingEndpoint storage)
        {
            /******************
             * Comm. protocol:
             *  - REQUEST : VOID
             *  - RESPONSE: [char_cnt, char[] name, char_cnt, char[] sig]
             ******************/
            TrinityMessage tm = new TrinityMessage(
                TrinityMessageType.PRESERVED_SYNC_WITH_RSP,
                (ushort)RequestType.GetCommunicationSchema,
                size: 0);
            return storage.SendRecvMessageAsync(tm)
                          .ContinueWith(
                t => 
                {
                    TrinityResponse response = null;

                    try
                    {
                        response = t.Result;
                        PointerHelper sp = PointerHelper.New(response.Buffer + response.Offset);
                        int name_string_len = *sp.ip++;
                        string name = BitHelper.GetString(sp.bp, name_string_len * 2);
                        sp.cp += name_string_len;
                        int sig_string_len = *sp.ip++;
                        string signature = BitHelper.GetString(sp.bp, sig_string_len * 2);

                        return (name, signature);
                    }
                    finally
                    {
                        response?.Dispose();
                        tm?.Dispose();
                    }
                },
                TaskContinuationOptions.ExecuteSynchronously);
        }

        internal static Task<(bool Succeeded, ushort SynReqOffset, ushort SynReqRspOffset, ushort AsynReqOffset, ushort AsynReqRspOffset)> GetCommunicationModuleOffsetAsync(this IMessagePassingEndpoint storage, string moduleName)
        {
            /******************
             * Comm. protocol:
             *  - REQUEST : [char_cnt, char[] moduleName]
             *  - RESPONSE: [int synReqOffset, int synReqRspOffset, int asynReqOffset, int asynReqRspOffset]
             * An response error code other than E_SUCCESS indicates failure of remote module lookup.
             ******************/

            TrinityMessage tm = new TrinityMessage(
                TrinityMessageType.PRESERVED_SYNC_WITH_RSP,
                (ushort)RequestType.GetCommunicationModuleOffsets,
                size: sizeof(int) + sizeof(char) * moduleName.Length);
            PointerHelper sp = PointerHelper.New(tm.Buffer + TrinityMessage.Offset);
            *sp.ip++         = moduleName.Length;

            BitHelper.WriteString(moduleName, sp.bp);

            return storage.SendRecvMessageAsync(tm)
                          .ContinueWith(
                t =>
                {
                    TrinityResponse response = null;

                    try
                    {
                        bool ret = t.Status == TaskStatus.RanToCompletion;
                        if (!ret)
                        {
                            return (ret, (ushort)0, (ushort)0, (ushort)0, (ushort)0);
                        }

                        response = t.Result;
                        sp.bp = response.Buffer + response.Offset;
                        int synReq_msg = *sp.ip++;
                        int synReqRsp_msg = *sp.ip++;
                        int asynReq_msg = *sp.ip++;
                        int asynReqRsp_msg = *sp.ip++;

                        return (ret, (ushort)synReq_msg, (ushort)synReqRsp_msg, (ushort)asynReq_msg, (ushort)asynReqRsp_msg);
                    }
                    finally
                    {
                        response?.Dispose();
                        tm?.Dispose();
                    }
                },
                TaskContinuationOptions.ExecuteSynchronously);
        }
    }
}
