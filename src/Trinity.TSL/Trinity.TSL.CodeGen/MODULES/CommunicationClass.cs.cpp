#include "common.h"
#include <string>
#include "SyntaxNode.h"

using std::string;

namespace Trinity
{
    namespace Codegen
    {
        namespace Modules
        {
            string* 
CommunicationClass(
NProtocolGroup* node, ModuleContext* context)
            {
                string* source = new string();
                
source->append(R"::(
    public abstract partial class )::");
source->append(Codegen::GetString(node->name));
source->append(R"::(Base : )::");
source->append(Codegen::GetString(get_comm_class_basename(node)));
source->append(R"::(
    {
        protected override void RegisterMessageHandler()
        {
            )::");
for (size_t iterator_1 = 0; iterator_1 < (node->protocolList)->size();++iterator_1)
{
if((*(node->protocolList))[iterator_1]->referencedNProtocol->is_http_protocol()){continue;}
if ((*(node->protocolList))[iterator_1]->referencedNProtocol->is_asyn_req_rsp_protocol())
{
source->append(R"::(
            {
                )::");
if (node->type() == PGT_MODULE)
{
source->append(R"::(
                MessageRegistry.RegisterMessageHandler((ushort)(this.)::");
source->append(Codegen::GetString(get_comm_protocol_type_string((*(node->protocolList))[iterator_1]->referencedNProtocol)));
source->append(R"::(IdOffset + (ushort)global::)::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(.TSL.)::");
source->append(Codegen::GetString(get_comm_class_basename(node)));
source->append(R"::(.)::");
source->append(Codegen::GetString(node->name));
source->append(R"::(.)::");
source->append(Codegen::GetString(get_comm_protocol_type_string((*(node->protocolList))[iterator_1]->referencedNProtocol)));
source->append(R"::(MessageType.)::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(), _)::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(Handler);
                MessageRegistry.RegisterMessageHandler((ushort)(this.)::");
source->append(Codegen::GetString(get_comm_protocol_type_string((*(node->protocolList))[iterator_1]->referencedNProtocol)));
source->append(R"::(IdOffset + (ushort)global::)::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(.TSL.)::");
source->append(Codegen::GetString(get_comm_class_basename(node)));
source->append(R"::(.)::");
source->append(Codegen::GetString(node->name));
source->append(R"::(.)::");
source->append(Codegen::GetString(get_comm_protocol_type_string((*(node->protocolList))[iterator_1]->referencedNProtocol)));
source->append(R"::(MessageType.)::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(__Response), _)::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(_ResponseHandler);
                )::");
}
else
{
source->append(R"::(
                MessageRegistry.RegisterMessageHandler((ushort)(ushort)global::)::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(.TSL.)::");
source->append(Codegen::GetString(get_comm_class_basename(node)));
source->append(R"::(.)::");
source->append(Codegen::GetString(node->name));
source->append(R"::(.)::");
source->append(Codegen::GetString(get_comm_protocol_type_string((*(node->protocolList))[iterator_1]->referencedNProtocol)));
source->append(R"::(MessageType.)::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(, _)::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(Handler);
                MessageRegistry.RegisterMessageHandler((ushort)(ushort)global::)::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(.TSL.)::");
source->append(Codegen::GetString(get_comm_class_basename(node)));
source->append(R"::(.)::");
source->append(Codegen::GetString(node->name));
source->append(R"::(.)::");
source->append(Codegen::GetString(get_comm_protocol_type_string((*(node->protocolList))[iterator_1]->referencedNProtocol)));
source->append(R"::(MessageType.)::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(__Response, _)::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(_ResponseHandler);
                )::");
}
source->append(R"::(
            }
            )::");
}
else
{
source->append(R"::(
            {
                )::");
if (node->type() == PGT_MODULE)
{
source->append(R"::(
                MessageRegistry.RegisterMessageHandler((ushort)(this.)::");
source->append(Codegen::GetString(get_comm_protocol_type_string((*(node->protocolList))[iterator_1]->referencedNProtocol)));
source->append(R"::(IdOffset + (ushort)global::)::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(.TSL.)::");
source->append(Codegen::GetString(get_comm_class_basename(node)));
source->append(R"::(.)::");
source->append(Codegen::GetString(node->name));
source->append(R"::(.)::");
source->append(Codegen::GetString(get_comm_protocol_type_string((*(node->protocolList))[iterator_1]->referencedNProtocol)));
source->append(R"::(MessageType.)::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(), _)::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(Handler);
                )::");
}
else
{
source->append(R"::(
                MessageRegistry.RegisterMessageHandler((ushort)(ushort)global::)::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(.TSL.)::");
source->append(Codegen::GetString(get_comm_class_basename(node)));
source->append(R"::(.)::");
source->append(Codegen::GetString(node->name));
source->append(R"::(.)::");
source->append(Codegen::GetString(get_comm_protocol_type_string((*(node->protocolList))[iterator_1]->referencedNProtocol)));
source->append(R"::(MessageType.)::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(, _)::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(Handler);
                )::");
}
source->append(R"::(
            }
            )::");
}
}
source->append(R"::(
        }
        )::");
for (size_t iterator_1 = 0; iterator_1 < (node->protocolList)->size();++iterator_1)
{
if((*(node->protocolList))[iterator_1]->referencedNProtocol->is_http_protocol()){continue;}
if (!(*(node->protocolList))[iterator_1]->referencedNProtocol->has_response())
{
source->append(R"::(
        private unsafe void _)::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(Handler()::");
source->append(Codegen::GetString(get_comm_protocol_type_string((*(node->protocolList))[iterator_1]->referencedNProtocol)));
source->append(R"::(Args args)
        {
            )::");
if ((*(node->protocolList))[iterator_1]->referencedNProtocol->has_request())
{
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(Handler(new )::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->referencedNProtocol->request_message_struct));
source->append(R"::(Reader(args.Buffer, args.Offset));
            )::");
}
else
{
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(Handler();
            )::");
}
source->append(R"::(
        }
        )::");
if ((*(node->protocolList))[iterator_1]->referencedNProtocol->has_request())
{
source->append(R"::(
        public abstract void )::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(Handler()::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->referencedNProtocol->request_message_struct));
source->append(R"::(Reader request);
        )::");
}
else
{
source->append(R"::(
        public abstract void )::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(Handler();
        )::");
}
}
else if ((*(node->protocolList))[iterator_1]->referencedNProtocol->has_request() && (*(node->protocolList))[iterator_1]->referencedNProtocol->is_syn_req_rsp_protocol())
{
source->append(R"::(
        private unsafe void _)::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(Handler()::");
source->append(Codegen::GetString(get_comm_protocol_type_string((*(node->protocolList))[iterator_1]->referencedNProtocol)));
source->append(R"::(Args args)
        {
            var rsp = new )::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->referencedNProtocol->response_message_struct));
source->append(R"::(Writer();
            )::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(Handler(new )::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->referencedNProtocol->request_message_struct));
source->append(R"::(Reader(args.Buffer, args.Offset), rsp);
            *(int*)(rsp.m_ptr - TrinityProtocol.MsgHeader) = rsp.Length + TrinityProtocol.TrinityMsgHeader;
            args.Response = new TrinityMessage(rsp.buffer, rsp.Length + TrinityProtocol.MsgHeader);
        }
        public abstract void )::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(Handler()::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->referencedNProtocol->request_message_struct));
source->append(R"::(Reader request, )::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->referencedNProtocol->response_message_struct));
source->append(R"::(Writer response);
        )::");
}
else if (!(*(node->protocolList))[iterator_1]->referencedNProtocol->has_request() && (*(node->protocolList))[iterator_1]->referencedNProtocol->is_syn_req_rsp_protocol())
{
source->append(R"::(
        private unsafe void _)::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(Handler()::");
source->append(Codegen::GetString(get_comm_protocol_type_string((*(node->protocolList))[iterator_1]->referencedNProtocol)));
source->append(R"::(Args args)
        {
            var rsp = new )::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->referencedNProtocol->response_message_struct));
source->append(R"::(Writer();
            )::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(Handler(rsp);
            *(int*)(rsp.m_ptr - TrinityProtocol.MsgHeader) = rsp.Length + TrinityProtocol.TrinityMsgHeader;
            args.Response = new TrinityMessage(rsp.buffer, rsp.Length + TrinityProtocol.MsgHeader);
        }
        public abstract void )::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(Handler()::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->referencedNProtocol->response_message_struct));
source->append(R"::(Writer response);
        )::");
}
else if ((*(node->protocolList))[iterator_1]->referencedNProtocol->has_request() && (*(node->protocolList))[iterator_1]->referencedNProtocol->is_asyn_req_rsp_protocol())
{
source->append(R"::(
        private unsafe void _)::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(Handler()::");
source->append(Codegen::GetString(get_comm_protocol_type_string((*(node->protocolList))[iterator_1]->referencedNProtocol)));
source->append(R"::(Args args)
        {
            using (var rsp = new )::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->referencedNProtocol->response_message_struct));
source->append(R"::(Writer(asyncRspHeaderLength: TrinityProtocol.AsyncWithRspAdditionalHeaderLength))
            {
                Exception exception = null;
                var req = new )::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->referencedNProtocol->request_message_struct));
source->append(R"::(Reader(args.Buffer, args.Offset + TrinityProtocol.AsyncWithRspAdditionalHeaderLength);
                try { )::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(Handler(req, rsp); }
                catch (Exception ex) { exception = ex; }
                int token = *(int*)(args.Buffer + args.Offset);
                int from = *(int*)(args.Buffer + args.Offset + sizeof(int));
                _)::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(_CheckError(exception, token, from);
                *(int*)(rsp.buffer) = TrinityProtocol.TrinityMsgHeader + TrinityProtocol.AsyncWithRspAdditionalHeaderLength + rsp.Length;
                *(rsp.buffer + TrinityProtocol.MsgTypeOffset) = (byte)TrinityMessageType.ASYNC_WITH_RSP;
                *(ushort*)(rsp.buffer + TrinityProtocol.MsgIdOffset) = (ushort)global::)::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(.TSL.)::");
source->append(Codegen::GetString(get_comm_class_basename(node)));
source->append(R"::(.)::");
source->append(Codegen::GetString(node->name));
source->append(R"::(.)::");
source->append(Codegen::GetString(get_comm_protocol_type_string((*(node->protocolList))[iterator_1]->referencedNProtocol)));
source->append(R"::(MessageType.)::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(__Response;
                *(int*)(rsp.m_ptr - TrinityProtocol.AsyncWithRspAdditionalHeaderLength) = token;
                *(int*)(rsp.m_ptr - TrinityProtocol.AsyncWithRspAdditionalHeaderLength + sizeof(int)) = 0;
                )::");
if (node->type() == PGT_MODULE)
{
source->append(R"::(
                this.SendMessage(m_memorycloud[from], rsp.buffer, rsp.Length + TrinityProtocol.MsgHeader + TrinityProtocol.AsyncWithRspAdditionalHeaderLength);
                )::");
}
else
{
source->append(R"::(
                Global.CloudStorage[from].SendMessage(rsp.buffer, rsp.Length + TrinityProtocol.MsgHeader + TrinityProtocol.AsyncWithRspAdditionalHeaderLength);
                )::");
}
source->append(R"::(
            }
        }
        public abstract void )::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(Handler()::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->referencedNProtocol->request_message_struct));
source->append(R"::(Reader request, )::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->referencedNProtocol->response_message_struct));
source->append(R"::(Writer response);
        )::");
}
else
{
source->append(R"::(
        private unsafe void _)::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(Handler()::");
source->append(Codegen::GetString(get_comm_protocol_type_string((*(node->protocolList))[iterator_1]->referencedNProtocol)));
source->append(R"::(Args args)
        {
            using (var rsp = new )::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->referencedNProtocol->response_message_struct));
source->append(R"::(Writer(asyncRspHeaderLength: TrinityProtocol.AsyncWithRspAdditionalHeaderLength))
            {
                Exception exception = null;
                try { )::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(Handler(rsp); }
                catch (Exception ex) { exception = ex; }
                int token = *(int*)(args.Buffer + args.Offset);
                int from = *(int*)(args.Buffer + args.Offset + sizeof(int));
                _)::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(_CheckError(exception, token, from);
                *(int*)(rsp.buffer) = TrinityProtocol.TrinityMsgHeader + TrinityProtocol.AsyncWithRspAdditionalHeaderLength + rsp.Length;
                *(rsp.buffer + TrinityProtocol.MsgTypeOffset) = (byte)TrinityMessageType.ASYNC_WITH_RSP;
                *(ushort*)(rsp.buffer + TrinityProtocol.MsgIdOffset) = (ushort)global::)::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(.TSL.)::");
source->append(Codegen::GetString(get_comm_class_basename(node)));
source->append(R"::(.)::");
source->append(Codegen::GetString(node->name));
source->append(R"::(.)::");
source->append(Codegen::GetString(get_comm_protocol_type_string((*(node->protocolList))[iterator_1]->referencedNProtocol)));
source->append(R"::(MessageType.)::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(__Response;
                *(int*)(rsp.m_ptr - TrinityProtocol.AsyncWithRspAdditionalHeaderLength) = token;
                *(int*)(rsp.m_ptr - TrinityProtocol.AsyncWithRspAdditionalHeaderLength + sizeof(int)) = 0;
                )::");
if (node->type() == PGT_MODULE)
{
source->append(R"::(
                this.SendMessage(m_memorycloud[from], rsp.buffer, rsp.Length + TrinityProtocol.MsgHeader + TrinityProtocol.AsyncWithRspAdditionalHeaderLength);
                )::");
}
else
{
source->append(R"::(
                Global.CloudStorage[from].SendMessage(rsp.buffer, rsp.Length + TrinityProtocol.MsgHeader + TrinityProtocol.AsyncWithRspAdditionalHeaderLength);
                )::");
}
source->append(R"::(
            }
        }
        public abstract void )::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(Handler()::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->referencedNProtocol->response_message_struct));
source->append(R"::(Writer response);
        )::");
}
if ((*(node->protocolList))[iterator_1]->referencedNProtocol->is_asyn_req_rsp_protocol())
{
source->append(R"::(
        #region AsyncWithRsp
        internal static int s_)::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(_token_counter = 0;
        internal static ConcurrentDictionary<int, TaskCompletionSource<)::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->referencedNProtocol->response_message_struct));
source->append(R"::(Reader>> s_)::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(_token_sources = new ConcurrentDictionary<int, TaskCompletionSource<)::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->referencedNProtocol->response_message_struct));
source->append(R"::(Reader>>();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void _)::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(_CheckError(Exception exception, int token, int from)
        {
            if (exception == null) return;
            byte[] rsp = new byte[TrinityProtocol.MsgHeader + TrinityProtocol.AsyncWithRspAdditionalHeaderLength];
            fixed (byte* p = rsp)
            {
                *(int*)(p) = TrinityProtocol.TrinityMsgHeader + TrinityProtocol.AsyncWithRspAdditionalHeaderLength;
                *(p + TrinityProtocol.MsgTypeOffset) = (byte)TrinityMessageType.ASYNC_WITH_RSP;
                *(ushort*)(p + TrinityProtocol.MsgIdOffset) = (ushort)global::)::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(.TSL.)::");
source->append(Codegen::GetString(get_comm_class_basename(node)));
source->append(R"::(.)::");
source->append(Codegen::GetString(node->name));
source->append(R"::(.)::");
source->append(Codegen::GetString(get_comm_protocol_type_string((*(node->protocolList))[iterator_1]->referencedNProtocol)));
source->append(R"::(MessageType.)::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(__Response;
                *(int*)(p + TrinityProtocol.MsgHeader) = token;
                *(int*)(p + TrinityProtocol.MsgHeader + sizeof(int)) = -1;
                )::");
if (node->type() == PGT_MODULE)
{
source->append(R"::(
                this.SendMessage(m_memorycloud[from], p, rsp.Length);
                )::");
}
else
{
source->append(R"::(
                Global.CloudStorage[from].SendMessage(p, rsp.Length);
                )::");
}
source->append(R"::(
            }
            ExceptionDispatchInfo.Capture(exception).Throw();
        }
        internal unsafe void _)::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(_ResponseHandler(AsynReqRspArgs args)
        {
            byte* buffer = args.Buffer + args.Offset;
            int size = args.Size - TrinityProtocol.AsyncWithRspAdditionalHeaderLength;
            if (size < 0)
            {
                throw new ArgumentException("Async task completion handler encountered negative message size.");
            }
            int token = *(int*)buffer;
            int error = *(int*)(buffer + sizeof(int));
            if (!s_)::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(_token_sources.TryRemove(token, out var src))
            {
                throw new ArgumentException("Async task completion token not found while processing a AsyncWithResponse message.");
            }
            if (error != 0)
            {
                src.SetException(new Exception("AsyncWithResponse remote handler failed."));
                return;
            }
            byte* buffer_clone = (byte*)Memory.malloc((ulong)(args.Size));
            Memory.Copy(buffer, buffer_clone, args.Size);
            var reader = new )::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->referencedNProtocol->response_message_struct));
source->append(R"::(Reader(buffer_clone, TrinityProtocol.AsyncWithRspAdditionalHeaderLength);
            try { src.SetResult(reader); }
            catch { Memory.free(buffer_clone); throw; }
        }
        #endregion
        )::");
}
}
source->append(R"::(
    }
    )::");

                return source;
            }
        }
    }
}
