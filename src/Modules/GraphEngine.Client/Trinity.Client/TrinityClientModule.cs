using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Expressions;
using Trinity.Core.Lib;
using Trinity.Diagnostics;
using Trinity.Extension;
using Trinity.Network.Messaging;
using Trinity.Storage;
using System.Runtime.CompilerServices;
using System.IO;

namespace Trinity.Client.TrinityClientModule
{
    [AutoRegisteredCommunicationModule]
    public class TrinityClientModule : TrinityClientModuleBase
    {
        public override string GetModuleName() => "TrinityClient";

        private IClientRegistry _clientRegistry;

        // ===== Server-side members =====
        private IClientRegistry Registry
        {
            get => m_memorycloud as IClientRegistry;
            set => m_memorycloud = value as MemoryCloud;
        }

        private ConcurrentDictionary<int, ClientIStorage> m_client_storages = new ConcurrentDictionary<int, ClientIStorage>();
        public IEnumerable<IStorage> Clients => m_client_storages.Values;
        private CancellationTokenSource m_cancel_src;
        private static readonly TimeSpan m_client_ttl = TimeSpan.FromSeconds(30);
        // ===== Client-side members =====
        private Lazy<int> m_client_cookie = new Lazy<int>(() => new Random().Next(int.MinValue, int.MaxValue));
        internal int MyCookie => m_client_cookie.Value;

        private IClientRegistry ClientRegistry
        {
            get => _clientRegistry = m_memorycloud as IClientRegistry;
            set => _clientRegistry = value;
        }

        private Task m_ttl_proc = null;

        public TrinityClientModule()
        {
            m_cancel_src = new CancellationTokenSource();
            m_ttl_proc = TTLProc(m_cancel_src.Token);
        }

        private async Task TTLProc(CancellationToken cancel)
        {
            //TODO client should not start TTLProc
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
                await Task.Delay(10000, cancel);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal new unsafe void SendMessage(IMessagePassingEndpoint ep, byte** bufs, int* sizes, int cnt)
            => base.SendMessage(ep, bufs, sizes, cnt);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal new unsafe void SendMessage(IMessagePassingEndpoint ep, byte** bufs, int* sizes, int cnt, out TrinityResponse rsp)
            => base.SendMessage(ep, bufs, sizes, cnt, out rsp);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal new unsafe void SendMessage(IMessagePassingEndpoint ep, byte* buf, int size, out TrinityResponse rsp)
            => base.SendMessage(ep, buf, size, out rsp);

        protected override void RegisterMessageHandler()
        {
            base.RegisterMessageHandler();
            MessageRegistry.RegisterMessageHandler((ushort)TSL.CommunicationModule.TrinityClientModule.SynReqMessageType.PostResponse, PostResponse_impl);
            MessageRegistry.RegisterMessageHandler((ushort)TSL.CommunicationModule.TrinityClientModule.SynReqRspMessageType.PollEvents, PollEvents_impl);
            MessageRegistry.RegisterMessageHandler((ushort)TSL.CommunicationModule.TrinityClientModule.SynReqMessageType.RedirectMessage, RedirectMessage_impl);
            MessageRegistry.RegisterMessageHandler((ushort)TSL.CommunicationModule.TrinityClientModule.SynReqRspMessageType.RedirectMessageWithResponse, RedirectMessageWithResponse_impl);

            MessageRegistry.RegisterMessageHandler((ushort)TSL.CommunicationModule.TrinityClientModule.SynReqRspMessageType.AddCell, AddCell_impl);
            MessageRegistry.RegisterMessageHandler((ushort)TSL.CommunicationModule.TrinityClientModule.SynReqRspMessageType.LoadCell, LoadCell_impl);
            MessageRegistry.RegisterMessageHandler((ushort)TSL.CommunicationModule.TrinityClientModule.SynReqRspMessageType.SaveCell, SaveCell_impl);
            MessageRegistry.RegisterMessageHandler((ushort)TSL.CommunicationModule.TrinityClientModule.SynReqRspMessageType.UpdateCell, UpdateCell_impl);
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
             * Response.p != 0 if the response is a "request with response"
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
             * Request: |4B PartitionId| TrinityMessage |
             * Response: VOID
             * 
             * Redirects the message to another instance.
             ******************************/

            PointerHelper sp = PointerHelper.New(args.Buffer + args.Offset);
            int partitionId = *sp.ip++;

            TrinityMessage tm = new TrinityMessage(sp.bp, (*sp.ip) + TrinityProtocol.SocketMsgHeader);
            m_memorycloud[partitionId].SendMessage(tm);
        }

        private unsafe void RedirectMessageWithResponse_impl(SynReqRspArgs args)
        {
            /******************************
             * Protocol: RedirectMessage
             * Request: |4B PartitionId| TrinityMessage |
             * Response: | TrinityResponse |
             * 
             * Redirects the message to another instance.
             ******************************/

            PointerHelper sp = PointerHelper.New(args.Buffer + args.Offset);
            int partitionId = *sp.ip++;

            TrinityMessage tm = new TrinityMessage(sp.bp, (*sp.ip) + TrinityProtocol.SocketMsgHeader);
            m_memorycloud[partitionId].SendMessage(tm, out var rsp);

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

        public override void AddCellHandler(__CellIdStructReader request, ErrorCodeResponseWriter response)
        {
            throw new NotImplementedException();
        }

        public override void LoadCellHandler(__CellIdStructReader request, __CellIdStructWriter response)
        {
            throw new NotImplementedException();
        }

        public override void SaveCellHandler(__CellIdStructReader request, ErrorCodeResponseWriter response)
        {
            throw new NotImplementedException();
        }

        public override void UpdateCellHandler(__CellIdStructReader request, ErrorCodeResponseWriter response)
        {
            throw new NotImplementedException();
        }
        #endregion

        public override void RegisterClientHandler(RegisterClientRequestReader request, RegisterClientResponseWriter response)
        {
            if (m_memorycloud != null)
            {
                response.PartitionCount = m_memorycloud.PartitionCount;

                if (ClientRegistry is null)
                    ClientRegistry = m_memorycloud as IClientRegistry;

                var cstg = m_client_storages.AddOrUpdate(request.Cookie, _ =>
                {
                    ClientIStorage stg = new ClientIStorage(m_memorycloud) {Pulse = DateTime.Now};

                    if (ClientRegistry != null)
                    {
                        int new_id = ClientRegistry.RegisterClient(stg);
                        stg.InstanceId = new_id;
                    }

                    return stg;
                }, (_, stg) => stg);
                response.InstanceId = cstg.InstanceId;
            }
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
                ClientRegistry.UnregisterClient(stg.InstanceId);
                // no requests come in from servers now, safe to dispose
                stg.Dispose();
            }
        }

        private ClientIStorage CheckInstanceCookie(int cookie, int instanceId)
        {
            if (m_client_storages.TryGetValue(cookie, out var storage) && instanceId == storage.InstanceId) return storage;
            throw new ClientInstanceNotFoundException();
        }

        public override void LoadStorageHandler()
        {
            if (!Global.CloudStorage.LoadStorage()) throw new IOException();
        }

        public override void SaveStorageHandler()
        {
            if (!Global.CloudStorage.SaveStorage()) throw new IOException();
        }

        public override void ResetStorageHandler()
        {
            if (!Global.CloudStorage.ResetStorage()) throw new IOException();
        }

        public override void ContainsHandler(__CellIdStructReader request, ErrorCodeResponseWriter response)
        {
            response.code = (int)(m_memorycloud.Contains(request.id) ? TrinityErrorCode.E_CELL_FOUND : TrinityErrorCode.E_CELL_NOT_FOUND);
        }

        public override void RemoveCellHandler(__CellIdStructReader request, ErrorCodeResponseWriter response)
        {
            response.code = (int)m_memorycloud.RemoveCell(request.id);
        }

        public override void GetCellTypeHandler(__CellIdStructReader request, ErrorCodeResponseWriter response)
        {
            var err = (int)m_memorycloud.GetCellType(request.id, out var cellType);
            response.code = err < 0 ? err : cellType;
        }

        private unsafe void UpdateCell_impl(SynReqRspArgs args)
        {
            /******************************
             * Protocol: UpdateCell
             * Request: |8B CellId|4B Size| Payload |
             * Response: [ 4B TrinityErrorCode header ]
             ******************************/
            var sp = PointerHelper.New(args.Buffer + args.Offset);
            var id = *sp.lp++;
            var size = *sp.ip++;

            var err = (int)m_memorycloud.UpdateCell(id, sp.bp, size);
            var buf = (byte*)Memory.malloc(TrinityProtocol.MsgHeader);
            sp = PointerHelper.New(buf);
            *sp.ip++ = TrinityProtocol.MsgHeader - TrinityProtocol.SocketMsgHeader;
            *sp.ip = err;
            args.Response = new TrinityMessage(buf, TrinityProtocol.MsgHeader);
        }

        private unsafe void SaveCell_impl(SynReqRspArgs args)
        {
            /******************************
             * Protocol: SaveCell
             * Request: |8B CellId|4B Size|2B CellType| Payload |
             * Response: | 4B TrinityErrorCode |
             ******************************/
            var sp = PointerHelper.New(args.Buffer + args.Offset);
            var id = *sp.lp++;
            var size = *sp.ip++;
            var type = (ushort)*sp.sp++;

            var err = (int)m_memorycloud.SaveCell(id, sp.bp, size, type);
            var buf = (byte*)Memory.malloc(TrinityProtocol.MsgHeader);
            sp = PointerHelper.New(buf);
            *sp.ip++ = TrinityProtocol.MsgHeader - TrinityProtocol.SocketMsgHeader;
            *sp.ip = err;
            args.Response = new TrinityMessage(buf, TrinityProtocol.MsgHeader);
        }

        private unsafe void AddCell_impl(SynReqRspArgs args)
        {
            /******************************
             * Protocol: AddCell
             * Request: |8B CellId|4B Size|2B CellType| Payload |
             * Response: [ 4B TrinityErrorCode header ]
             ******************************/
            var sp = PointerHelper.New(args.Buffer + args.Offset);
            var id = *sp.lp++;
            var size = *sp.ip++;
            var type = (ushort)*sp.sp++;

            var err = (int)m_memorycloud.AddCell(id, sp.bp, size, type);
            var buf = (byte*)Memory.malloc(TrinityProtocol.MsgHeader);
            sp = PointerHelper.New(buf);
            *sp.ip++ = TrinityProtocol.MsgHeader - TrinityProtocol.SocketMsgHeader;
            *sp.ip = err;
            args.Response = new TrinityMessage(buf, TrinityProtocol.MsgHeader);
        }

        private unsafe void LoadCell_impl(SynReqRspArgs args)
        {
            /******************************
             * Protocol: LoadCell
             * Request: |8B CellId|
             * Response: [ 4B TrinityErrorCode header ] -- if success --> | 4B Size|2B CellType| Payload |
             ******************************/
            var id = *(long*)(args.Buffer + args.Offset);
            var err = m_memorycloud.LoadCell(id, out var cellBuff, out var cellType);
            if (err == TrinityErrorCode.E_SUCCESS)
            {
                var len = TrinityProtocol.MsgHeader + sizeof(int) + cellBuff.Length + sizeof(ushort);
                var buf = (byte*)Memory.malloc((ulong)len);
                var sp = PointerHelper.New(buf);
                *sp.ip++ = len - TrinityProtocol.SocketMsgHeader;
                *sp.ip++ = (int)err;
                *sp.ip++ = cellBuff.Length;
                *sp.sp++ = (short)cellType;
                fixed(byte* p = cellBuff)
                {
                    Memory.memcpy(sp.bp, p, (ulong)cellBuff.Length);
                }

                args.Response = new TrinityMessage(buf, len);
            }
            else
            {
                var buf = (byte*)Memory.malloc(TrinityProtocol.MsgHeader);
                var sp = PointerHelper.New(buf);
                *sp.ip++ = TrinityProtocol.MsgHeader - TrinityProtocol.SocketMsgHeader;
                *sp.ip = (int)err;
                args.Response = new TrinityMessage(buf, TrinityProtocol.MsgHeader);
            }
        }
    }
}
