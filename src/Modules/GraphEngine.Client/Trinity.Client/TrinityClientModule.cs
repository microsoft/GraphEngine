using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Trinity.Client.ServerSide;
using Trinity.Core.Lib;
using Trinity.Diagnostics;
using Trinity.Extension;
using Trinity.Network.Messaging;
using Trinity.Storage;

namespace Trinity.Client
{
    [AutoRegisteredCommunicationModule]
    public class TrinityClientModule : TrinityClientModuleBase
    {
        public override string GetModuleName() => "TrinityClient";

        // ===== Server-side members =====
        private IClientRegistry Registry => m_memorycloud as IClientRegistry;
        private ConcurrentDictionary<int, ClientIStorage> m_client_storages = new ConcurrentDictionary<int, ClientIStorage>();
        public IEnumerable<IStorage> Clients => m_client_storages.Values;
        // ===== Client-side members =====
        private Lazy<int> m_client_cookie = new Lazy<int>(() => new Random().Next(int.MinValue, int.MaxValue));
        internal int MyCookie => m_client_cookie.Value;
        private Task m_ttl_proc = null;
        private CancellationTokenSource m_cancel_src;
        private static readonly TimeSpan m_client_ttl = TimeSpan.FromSeconds(30);

        public TrinityClientModule()
        {
            m_cancel_src = new CancellationTokenSource();
            m_ttl_proc = TTLProc(m_cancel_src.Token);
        }

        private async Task TTLProc(CancellationToken cancel)
        {
            while (!cancel.IsCancellationRequested)
            {
                Log.WriteLine(LogLevel.Verbose, $"{nameof(TTLProc)}: scanning for timed-out clients.");
                try
                {
                    DateTime now = DateTime.Now;
                    var clients = m_client_storages.ToArray();
                    foreach (var c in clients.Where(_ => (now - _.Value.Pulse) >= m_client_ttl))
                    {
                        RemoveClient(c.Key, c.Value);
                        Log.WriteLine(LogLevel.Info, $"{nameof(TTLProc)}: client [{c.Value.InstanceId}] timed-out.");
                    }
                }
                catch (Exception ex)
                {
                    Log.WriteLine(LogLevel.Error, "{0}: {1}", nameof(TTLProc), ex.ToString());
                }
                await Task.Delay(10000);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal unsafe new void SendMessage(IMessagePassingEndpoint ep, byte** bufs, int* sizes, int cnt)
            => base.SendMessage(ep, bufs, sizes, cnt);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal unsafe new void SendMessage(IMessagePassingEndpoint ep, byte** bufs, int* sizes, int cnt, out TrinityResponse rsp)
            => base.SendMessage(ep, bufs, sizes, cnt, out rsp);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal unsafe new void SendMessage(IMessagePassingEndpoint ep, byte* buf, int size, out TrinityResponse rsp)
            => base.SendMessage(ep, buf, size, out rsp);

        protected override void RegisterMessageHandler()
        {
            base.RegisterMessageHandler();
            MessageRegistry.RegisterMessageHandler((ushort)TSL.CommunicationModule.TrinityClientModule.SynReqMessageType.PostResponse, PostResponse_impl);
            MessageRegistry.RegisterMessageHandler((ushort)TSL.CommunicationModule.TrinityClientModule.SynReqRspMessageType.PollEvents, PollEvents_impl);
            MessageRegistry.RegisterMessageHandler((ushort)TSL.CommunicationModule.TrinityClientModule.SynReqMessageType.RedirectMessage, RedirectMessage_impl);
            MessageRegistry.RegisterMessageHandler((ushort)TSL.CommunicationModule.TrinityClientModule.SynReqRspMessageType.RedirectMessageWithResponse, RedirectMessageWithResponse_impl);
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
             * Response:[4B E_RESULT Header] - |8B p| TrinityMessage |
             * Response.p != 0 if the resposne is a "request with response"
             * Response.TrinityMessage length header < 0 if there are no events
             * E_RESULT values:
             * 
             *      0 = E_SUCCESS
             *      1 = E_NO_EVENTS
             *      2 = E_INVALID_CLIENT
             * 
             * !NOTE Here, response itself is a TrinityMessage and Response.TrinityMessage
             * is the inner payload.
             ******************************/
            PointerHelper sp = PointerHelper.New(args.Buffer + args.Offset);
            int instanceId = *sp.ip++;
            int cookie = *sp.ip++;
            byte* outer_buf;
            int outer_len;
            long p = 0;
            TrinityMessage tm = null;
            TaskCompletionSource<bool> tsrc = null;
            bool invalid_client = false;

            try
            {
                var stg = CheckInstanceCookie(cookie, instanceId);
                stg.Pulse = DateTime.Now;
                (p, tm, tsrc) = stg.PollEvents_impl();
            }
            catch (ClientInstanceNotFoundException)
            {
                invalid_client = true;
            }

            if (tm == null)
            {
                outer_len = TrinityProtocol.MsgHeader + sizeof(long) + sizeof(int);
                outer_buf = (byte*)Memory.malloc((ulong)outer_len);
                *(int*)outer_buf = outer_len - TrinityProtocol.SocketMsgHeader;
                *(long*)(outer_buf + TrinityProtocol.MsgHeader) = 0;
                *(int*)(outer_buf + TrinityProtocol.MsgHeader + sizeof(long)) = -1;
                *(int*)(outer_buf + TrinityProtocol.SocketMsgHeader) = invalid_client ? 2 : 1;
            }
            else
            {
                outer_len = TrinityProtocol.MsgHeader + sizeof(long) + tm.Size;
                outer_buf = (byte*)Memory.malloc((ulong)outer_len);
                *(int*)outer_buf = outer_len - TrinityProtocol.SocketMsgHeader;
                *(long*)(outer_buf + TrinityProtocol.MsgHeader) = p;
                Memory.memcpy(outer_buf + TrinityProtocol.MsgHeader + sizeof(long), tm.Buffer, (ulong)tm.Size);
                tsrc.SetResult(true);
                *(int*)(outer_buf + TrinityProtocol.SocketMsgHeader) = 0;
            }
            args.Response = new TrinityMessage(outer_buf, outer_len);
        }

        private unsafe void RedirectMessage_impl(SynReqArgs args)
        {
            /******************************
             * Protocol: RedirectMessage
             * Request: |4B InstanceId| TrinityMessage |
             * Response: VOID
             * 
             * Redirects the message to another instance.
             ******************************/

            PointerHelper sp = PointerHelper.New(args.Buffer + args.Offset);
            int instanceId = *sp.ip++;

            TrinityMessage tm = new TrinityMessage(sp.bp, *sp.ip + sizeof(int));
            m_memorycloud[instanceId].SendMessage(tm);
        }

        private unsafe void RedirectMessageWithResponse_impl(SynReqRspArgs args)
        {
            /******************************
             * Protocol: RedirectMessage
             * Request: |4B InstanceId| TrinityMessage |
             * Response: | TrinityResponse |
             * 
             * Redirects the message to another instance.
             ******************************/

            PointerHelper sp = PointerHelper.New(args.Buffer + args.Offset);
            int instanceId = *sp.ip++;

            TrinityMessage tm = new TrinityMessage(sp.bp, *sp.ip + sizeof(int));
            m_memorycloud[instanceId].SendMessage(tm, out var rsp);

            int rsp_size = TrinityProtocol.MsgHeader + rsp.Size;
            byte* rsp_buf = (byte*)Memory.malloc((ulong)rsp_size);
            *(int*)rsp_buf = rsp_size - TrinityProtocol.SocketMsgHeader;
            Memory.Copy(rsp.Buffer, rsp.Offset, rsp_buf, TrinityProtocol.MsgHeader, rsp.Size);
            rsp.Dispose();

            args.Response = new TrinityMessage(rsp_buf, rsp_size);
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

        public override void RedirectMessageHandler(PostResponseRequestReader request)
        {
            throw new NotImplementedException();
        }

        public override void RedirectMessageWithResponseHandler(PostResponseRequestReader request, PostResponseRequestWriter response)
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
                stg.Pulse = DateTime.Now;
                int new_id = Registry.RegisterClient(stg);
                stg.InstanceId = new_id;
                return stg;
            }, (_, stg) => stg);
            response.InstanceId = cstg.InstanceId;
        }

        public override void UnregisterClientHandler(UnregisterClientRequestReader request)
        {
            var stg = CheckInstanceCookie(request.Cookie, request.InstanceId);
            RemoveClient(request.Cookie, stg);
        }

        private void RemoveClient(int cookie, ClientIStorage stg)
        {
            if (m_client_storages.TryRemove(cookie, out _))
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
