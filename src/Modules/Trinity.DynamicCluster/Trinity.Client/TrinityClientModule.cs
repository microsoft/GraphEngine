using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Trinity.Core.Lib;
using Trinity.Extension;
using Trinity.Network.Messaging;

namespace Trinity.Client.TrinityClientModule
{
    [AutoRegisteredCommunicationModule]
    class TrinityClientModule : TrinityClientModuleBase
    {
        public override string GetModuleName() => "TrinityClient";

        // ===== Server-side members =====
        private IClientRegistry Registry => m_memorycloud as IClientRegistry;
        private ConcurrentDictionary<int, ClientIStorage> m_client_storages = new ConcurrentDictionary<int, ClientIStorage>();
        // ===== Client-side members =====
        private Lazy<int> m_client_cookie = new Lazy<int>(() => new Random().Next(int.MinValue, int.MaxValue));
        internal int MyCookie => m_client_cookie.Value;

        protected override void RegisterMessageHandler()
        {
            base.RegisterMessageHandler();
            MessageRegistry.RegisterMessageHandler((ushort)TSL.CommunicationModule.TrinityClientModule.SynReqMessageType.PostResponse, PostResponse_impl);
            MessageRegistry.RegisterMessageHandler((ushort)TSL.CommunicationModule.TrinityClientModule.SynReqRspMessageType.PollEvents, PollEvents_impl);
        }

        private unsafe void PostResponse_impl(SynReqArgs args)
        {
            /******************************
             * Protocol: PostResponse
             * Request: |4B InstanceId|4B Cookie|8B p| TrinityResponse |
             * Response: VOID
             ******************************/
            PointerHelper sp = PointerHelper.New(args.Buffer + args.Offset);
            int instanceId = *sp.ip++;
            int cookie = *sp.ip++;
            long p = *sp.lp++;
            var stg = CheckInstanceCookie(cookie, instanceId);
            stg.PostResponse_impl(p, sp.bp, args.Size - sizeof(int) * 2 - sizeof(long));
        }

        private unsafe void PollEvents_impl(SynReqRspArgs args)
        {
            /******************************
             * Protocol: PollEvents
             * Request: |4B InstanceId|4B Cookie|
             * Response:|8B p| TrinityMessage |
             * Response.p != 0 if the resposne is a "request with response"
             * Response.TrinityMessage length header < 0 if there are no events
             * !NOTE Here, response itself is a TrinityMessage and Response.TrinityMessage
             ******************************/
            PointerHelper sp = PointerHelper.New(args.Buffer + args.Offset);
            int instanceId = *sp.ip++;
            int cookie = *sp.ip++;
            var stg = CheckInstanceCookie(cookie, instanceId);
            (long p, TrinityMessage tm, TaskCompletionSource<bool> tsrc) = stg.PollEvents_impl();
            byte* outer_buf;
            int outer_len;
            if(tm == null)
            {
                outer_len = TrinityProtocol.MsgHeader + sizeof(long) + sizeof(int);
                outer_buf = (byte*)Memory.malloc((ulong)outer_len);
                *(int*)outer_buf = outer_len - TrinityProtocol.MsgHeader;
                *(long*)(outer_buf + TrinityProtocol.MsgHeader) = 0;
                *(int*)(outer_buf + TrinityProtocol.MsgHeader + sizeof(long)) = -1;
            }
            else
            {
                outer_len = TrinityProtocol.MsgHeader + sizeof(long) + tm.Size;
                outer_buf = (byte*)Memory.malloc((ulong)outer_len);
                *(int*)outer_buf = outer_len - TrinityProtocol.MsgHeader;
                *(long*)(outer_buf + TrinityProtocol.MsgHeader) = p;
                Memory.memcpy(outer_buf + TrinityProtocol.MsgHeader, tm.Buffer, (ulong)tm.Size);
                tsrc.SetResult(true);
            }
            args.Response = new TrinityMessage(outer_buf, outer_len);
        }

        #region not used
        public override void PollEventsHandler(PollEventsRequestReader request, PollEventsResponseWriter response)
        {
            throw new NotImplementedException();
        }

        public override void PostResponseHandler(PostResponseRequestReader request)
        {
            throw new NotImplementedException();
        }
        #endregion

        public override void RegisterClientHandler(RegisterClientRequestReader request, RegisterClientResponseWriter response)
        {
            response.PartitionCount = m_memorycloud.PartitionCount;
            var cstg = m_client_storages.AddOrUpdate(request.Cookie, _ =>
            {
                ClientIStorage stg = new ClientIStorage(m_memorycloud);
                int new_id = Registry.RegisterClient(stg);
                stg.InstanceId = new_id;
                return stg;
            }, (_, stg) => stg);
            response.InstanceId = cstg.InstanceId;
        }

        public override void UnregisterClientHandler(UnregisterClientRequestReader request)
        {
            var stg = CheckInstanceCookie(request.Cookie, request.InstanceId);
            if (m_client_storages.TryRemove(request.Cookie, out _))
            {
                // no responses come in from clients now
                Registry.UnregisterClient(stg.InstanceId);
                // no requests come in from servers now, safe to dispose
                stg.Dispose();
            }
        }

        private ClientIStorage CheckInstanceCookie(int cookie, int instanceId)
        {
            if (m_client_storages.TryGetValue(cookie, out var storage) && instanceId == storage.InstanceId) return storage;
            throw new ClientInstanceNotFoundException();
        }

    }
}
