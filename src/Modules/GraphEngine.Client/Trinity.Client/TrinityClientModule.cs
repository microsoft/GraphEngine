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

        // ===== Server-side members =====
        private IClientRegistry Registry => m_memorycloud as IClientRegistry;
        private ConcurrentDictionary<int, ClientIStorage> m_client_storages = new ConcurrentDictionary<int, ClientIStorage>();
        public IEnumerable<IStorage> Clients => m_client_storages.Values;
        private CancellationTokenSource m_cancel_src;
        private static readonly TimeSpan m_client_ttl = TimeSpan.FromSeconds(30);
        // ===== Client-side members =====
        private Lazy<int> m_client_cookie = new Lazy<int>(() => new Random().Next(int.MinValue, int.MaxValue));
        internal int MyCookie => m_client_cookie.Value;
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
        protected internal unsafe new Task SendMessageAsync(IMessagePassingEndpoint ep, byte** bufs, int* sizes, int cnt)
            => base.SendMessageAsync(ep, bufs, sizes, cnt);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal unsafe new Task<TrinityResponse> SendRecvMessageAsync(IMessagePassingEndpoint ep, byte** bufs, int* sizes, int cnt)
            => base.SendRecvMessageAsync(ep, bufs, sizes, cnt);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected internal unsafe new Task<TrinityResponse> SendRecvMessageAsync(IMessagePassingEndpoint ep, byte* buf, int size)
            => base.SendRecvMessageAsync(ep, buf, size);

        protected override void RegisterMessageHandler()
        {
            base.RegisterMessageHandler();
            MessageRegistry.RegisterMessageHandler((ushort)TSL.CommunicationModule.TrinityClientModule.SynReqMessageType.PostResponse, PostResponse_impl_Async);
            MessageRegistry.RegisterMessageHandler((ushort)TSL.CommunicationModule.TrinityClientModule.SynReqRspMessageType.PollEvents, PollEvents_impl_Async);
            MessageRegistry.RegisterMessageHandler((ushort)TSL.CommunicationModule.TrinityClientModule.SynReqMessageType.RedirectMessage, RedirectMessage_impl_Async);
            MessageRegistry.RegisterMessageHandler((ushort)TSL.CommunicationModule.TrinityClientModule.SynReqRspMessageType.RedirectMessageWithResponse, RedirectMessageWithResponse_impl_Async);

            MessageRegistry.RegisterMessageHandler((ushort)TSL.CommunicationModule.TrinityClientModule.SynReqRspMessageType.AddCell, AddCell_impl_Async);
            MessageRegistry.RegisterMessageHandler((ushort)TSL.CommunicationModule.TrinityClientModule.SynReqRspMessageType.LoadCell, LoadCell_impl_Async);
            MessageRegistry.RegisterMessageHandler((ushort)TSL.CommunicationModule.TrinityClientModule.SynReqRspMessageType.SaveCell, SaveCell_impl_Async);
            MessageRegistry.RegisterMessageHandler((ushort)TSL.CommunicationModule.TrinityClientModule.SynReqRspMessageType.UpdateCell, UpdateCell_impl_Async);
        }


        private unsafe Task PostResponse_impl_Async(SynReqArgs args)
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
            return Task.CompletedTask;
        }

        private unsafe Task PollEvents_impl_Async(SynReqRspArgs args)
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
            return Task.CompletedTask;
        }

        private unsafe Task RedirectMessage_impl_Async(SynReqArgs args)
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
            return m_memorycloud[partitionId].SendMessageAsync(tm);
        }

        private unsafe Task RedirectMessageWithResponse_impl_Async(SynReqRspArgs args)
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
            return m_memorycloud[partitionId].SendRecvMessageAsync(tm).ContinueWith(
                t => 
                {
                    var rsp = t.Result;

                    int rsp_size = TrinityProtocol.MsgHeader + rsp.Size;
                    byte* rsp_buf = (byte*)Memory.malloc((ulong)rsp_size);
                    *(int*)rsp_buf = rsp_size - TrinityProtocol.SocketMsgHeader;
                    Memory.Copy(rsp.Buffer, rsp.Offset, rsp_buf, TrinityProtocol.MsgHeader, rsp.Size);
                    rsp.Dispose();

                    args.Response = new TrinityMessage(rsp_buf, rsp_size);
                });
        }

        #region not used
        public override Task PollEventsHandlerAsync(PollEventsRequestReader request, PollEventsResponseWriter response)
        {
            throw new NotImplementedException();
        }

        public override Task PostResponseHandlerAsync(PostResponseRequestReader request)
        {
            throw new NotImplementedException();
        }

        public override Task RedirectMessageHandlerAsync(PostResponseRequestReader request)
        {
            throw new NotImplementedException();
        }

        public override Task RedirectMessageWithResponseHandlerAsync(PostResponseRequestReader request, PostResponseRequestWriter response)
        {
            throw new NotImplementedException();
        }

        public override Task AddCellHandlerAsync(__CellIdStructReader request, ErrorCodeResponseWriter response)
        {
            throw new NotImplementedException();
        }

        public override Task LoadCellHandlerAsync(__CellIdStructReader request, __CellIdStructWriter response)
        {
            throw new NotImplementedException();
        }

        public override Task SaveCellHandlerAsync(__CellIdStructReader request, ErrorCodeResponseWriter response)
        {
            throw new NotImplementedException();
        }

        public override Task UpdateCellHandlerAsync(__CellIdStructReader request, ErrorCodeResponseWriter response)
        {
            throw new NotImplementedException();
        }
        #endregion

        public override Task RegisterClientHandlerAsync(RegisterClientRequestReader request, RegisterClientResponseWriter response)
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
            return Task.CompletedTask;
        }

        public override Task UnregisterClientHandlerAsync(UnregisterClientRequestReader request)
        {
            var stg = CheckInstanceCookie(request.Cookie, request.InstanceId);
            RemoveClient(request.Cookie, stg);
            return Task.CompletedTask;
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

        public override async Task LoadStorageHandlerAsync()
        {
            if (!await Global.CloudStorage.LoadStorageAsync()) throw new IOException();
        }

        public override async Task SaveStorageHandlerAsync()
        {
            if (!await Global.CloudStorage.SaveStorageAsync()) throw new IOException();
        }

        public override async Task ResetStorageHandlerAsync()
        {
            if (!await Global.CloudStorage.ResetStorageAsync()) throw new IOException();
        }

        public override async Task ContainsHandlerAsync(__CellIdStructReader request, ErrorCodeResponseWriter response)
        {
            response.code = (int)(await m_memorycloud.ContainsAsync(request.id) ? TrinityErrorCode.E_CELL_FOUND : TrinityErrorCode.E_CELL_NOT_FOUND);
        }

        public override async Task RemoveCellHandlerAsync(__CellIdStructReader request, ErrorCodeResponseWriter response)
        {
            response.code = (int)await m_memorycloud.RemoveCellAsync(request.id);
        }

        public override async Task GetCellTypeHandlerAsync(__CellIdStructReader request, ErrorCodeResponseWriter response)
        {
            var result = await m_memorycloud.GetCellTypeAsync(request.id);
            var err = (int)result.ErrorCode;
            response.code = err < 0 ? err : result.CellType;
        }

        private unsafe Task UpdateCell_impl_Async(SynReqRspArgs args)
        {
            /******************************
             * Protocol: UpdateCell
             * Request: |8B CellId|4B Size| Payload |
             * Response: [ 4B TrinityErrorCode header ]
             ******************************/
            var sp = PointerHelper.New(args.Buffer + args.Offset);
            var id = *sp.lp++;
            var size = *sp.ip++;

            return m_memorycloud.UpdateCellAsync(id, sp.bp, size).ContinueWith(
                t =>
                {
                    var err = (int)t.Result;
                    var buf = (byte*)Memory.malloc(TrinityProtocol.MsgHeader);
                    sp = PointerHelper.New(buf);
                    *sp.ip++ = TrinityProtocol.MsgHeader - TrinityProtocol.SocketMsgHeader;
                    *sp.ip = err;
                    args.Response = new TrinityMessage(buf, TrinityProtocol.MsgHeader);
                });
        }

        private unsafe Task SaveCell_impl_Async(SynReqRspArgs args)
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

            return m_memorycloud.SaveCellAsync(id, sp.bp, size, type).ContinueWith(
                t =>
                {
                    var err = (int)t.Result;
                    var buf = (byte*)Memory.malloc(TrinityProtocol.MsgHeader);
                    sp = PointerHelper.New(buf);
                    *sp.ip++ = TrinityProtocol.MsgHeader - TrinityProtocol.SocketMsgHeader;
                    *sp.ip = err;
                    args.Response = new TrinityMessage(buf, TrinityProtocol.MsgHeader);
                });
        }

        private unsafe Task AddCell_impl_Async(SynReqRspArgs args)
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

            return m_memorycloud.AddCellAsync(id, sp.bp, size, type).ContinueWith(
                t =>
                {
                    var err = (int)t.Result;
                    var buf = (byte*)Memory.malloc(TrinityProtocol.MsgHeader);
                    sp = PointerHelper.New(buf);
                    *sp.ip++ = TrinityProtocol.MsgHeader - TrinityProtocol.SocketMsgHeader;
                    *sp.ip = err;
                    args.Response = new TrinityMessage(buf, TrinityProtocol.MsgHeader);
                });
        }

        private unsafe Task LoadCell_impl_Async(SynReqRspArgs args)
        {
            /******************************
             * Protocol: LoadCell
             * Request: |8B CellId|
             * Response: [ 4B TrinityErrorCode header ] -- if success --> | 4B Size|2B CellType| Payload |
             ******************************/
            var id = *(long*)(args.Buffer + args.Offset);
            return m_memorycloud.LoadCellAsync(id).ContinueWith(
                t =>
                {
                    var err = t.Result.ErrorCode;
                    if (err == TrinityErrorCode.E_SUCCESS)
                    {
                        var cellBuff = t.Result.CellBuff;
                        var cellType = t.Result.CellType;
                        var len = TrinityProtocol.MsgHeader + sizeof(int) + cellBuff.Length + sizeof(ushort);
                        var buf = (byte*)Memory.malloc((ulong)len);
                        var sp = PointerHelper.New(buf);
                        *sp.ip++ = len - TrinityProtocol.SocketMsgHeader;
                        *sp.ip++ = (int)err;
                        *sp.ip++ = cellBuff.Length;
                        *sp.sp++ = (short)cellType;
                        fixed (byte* p = cellBuff)
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
                });
        }
    }
}
