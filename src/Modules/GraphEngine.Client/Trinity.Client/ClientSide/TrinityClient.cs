using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trinity.Client.ServerSide;
using Trinity.Client.TrinityClientModule;
using Trinity.Configuration;
using Trinity.Core.Lib;
using Trinity.Daemon;
using Trinity.Diagnostics;
using Trinity.Network;
using Trinity.Network.Messaging;
using Trinity.Storage;
using Trinity.Utilities;

namespace Trinity.Client
{
    public class TrinityClient : CommunicationInstance, IMessagePassingEndpoint
    {
        private IClientConnectionFactory m_clientfactory = null;
        private IMessagePassingEndpoint m_client;
        private CancellationTokenSource m_tokensrc;
        private TrinityClientModule.TrinityClientModule m_mod;
        private Task m_polltask;
        private readonly string m_endpoint;
        private int m_id;
        private int m_cookie;

        public TrinityClient(string endpoint)
            : this(endpoint, null)
        { }

        public TrinityClient(string endpoint, IClientConnectionFactory clientConnectionFactory)
        {
            m_endpoint = endpoint;
            m_clientfactory = clientConnectionFactory;
            RegisterCommunicationModule<TrinityClientModule.TrinityClientModule>();
            ExtensionConfig.Instance.Priority.Add(new ExtensionPriority { Name = typeof(ClientMemoryCloud).AssemblyQualifiedName, Priority = int.MaxValue });
            ExtensionConfig.Instance.Priority.Add(new ExtensionPriority { Name = typeof(HostMemoryCloud).AssemblyQualifiedName, Priority = int.MinValue });
            ExtensionConfig.Instance.Priority = ExtensionConfig.Instance.Priority; // trigger update of priority table
        }

        protected override sealed RunningMode RunningMode => RunningMode.Client;

        public unsafe void SendMessage(byte* message, int size)
            => m_client.SendMessage(message, size);

        public unsafe void SendMessage(byte* message, int size, out TrinityResponse response)
            => m_client.SendMessage(message, size, out response);

        public unsafe void SendMessage(byte** message, int* sizes, int count)
            => m_client.SendMessage(message, sizes, count);

        public unsafe void SendMessage(byte** message, int* sizes, int count, out TrinityResponse response)
            => m_client.SendMessage(message, sizes, count, out response);

        protected override sealed void DispatchHttpRequest(HttpListenerContext ctx, string handlerName, string url)
            => throw new NotSupportedException();

        protected override sealed void RootHttpHandler(HttpListenerContext ctx)
            => throw new NotSupportedException();

        protected override void StartCommunicationListeners()
        {
            if (m_clientfactory == null) { ScanClientConnectionFactory(); }
            m_client = m_clientfactory.ConnectAsync(m_endpoint, this).Result;
            ClientMemoryCloud.BeginInitialize(m_client, this);
            this.Started += StartPolling;
        }

        private void StartPolling()
        {
            m_mod = GetCommunicationModule<TrinityClientModule.TrinityClientModule>();
            RegisterClient();
            m_polltask = PollProc(m_tokensrc.Token);
        }

        private void RestartPolling()
        {
            m_tokensrc.Cancel();
            StartPolling();
        }

        private void RegisterClient()
        {
            ClientMemoryCloud.EndInitialize();
            m_tokensrc = new CancellationTokenSource();
            m_id = Global.CloudStorage.MyInstanceId;
            m_cookie = m_mod.MyCookie;
        }

        private async Task PollProc(CancellationToken token)
        {
            TrinityMessage poll_req = _AllocPollMsg(m_id, m_cookie);
            while (!token.IsCancellationRequested)
            {
                try
                {
                    _PollImpl(poll_req);
                    await Task.Delay(100);
                }
                catch (Exception ex)
                {
                    Log.WriteLine(LogLevel.Error, $"{nameof(TrinityClient)}: error occured during polling: {{0}}", ex.ToString());
                    await Task.Delay(100);
                }
            }
            poll_req.Dispose();
        }

        private unsafe TrinityMessage _AllocPollMsg(int myInstanceId, int myCookie)
        {
            int msglen                                      = sizeof(int) + sizeof(int) + TrinityProtocol.MsgHeader;
            byte* buf                                       = (byte*)Memory.malloc((ulong)msglen);
            PointerHelper sp                                = PointerHelper.New(buf);
            *sp.ip                                          = msglen - TrinityProtocol.SocketMsgHeader;
            *(TrinityMessageType*)(sp.bp + TrinityProtocol.MsgTypeOffset) = TrinityMessageType.SYNC_WITH_RSP;
            *(ushort*)(sp.bp + TrinityProtocol.MsgIdOffset) = (ushort)TSL.CommunicationModule.TrinityClientModule.SynReqRspMessageType.PollEvents;
            sp.bp                                          += TrinityProtocol.MsgHeader;
            *sp.ip++                                        = myInstanceId;
            *sp.ip++                                        = myCookie;
            return new TrinityMessage(buf, msglen);
        }

        private unsafe void _PollImpl(TrinityMessage poll_req)
        {
            m_mod.SendMessage(m_client, poll_req.Buffer, poll_req.Size, out var poll_rsp);
            var sp = PointerHelper.New(poll_rsp.Buffer + poll_rsp.Offset);
            //HexDump.Dump(poll_rsp.ToByteArray());
            //Console.WriteLine($"poll_rsp.Size = {poll_rsp.Size}");
            //Console.WriteLine($"poll_rsp.Offset = {poll_rsp.Offset}");
            var payload_len = poll_rsp.Size - TrinityProtocol.TrinityMsgHeader;
            if (payload_len < sizeof(long) + sizeof(int)) { throw new IOException("Poll response corrupted."); }
            var errno = *(sp.ip - 1);

            try
            {
                if (errno == 2)
                {
                    Log.WriteLine(LogLevel.Warning, $"{nameof(TrinityClient)}: server drops our connection. Registering again.");
                    RestartPolling();
                    return;
                }
                if (errno != 0) { return; }

                var pctx = *sp.lp++;
                var msg_len = *sp.ip++;
                if (msg_len < 0) return; // no events
                MessageBuff msg_buff = new MessageBuff{ Buffer = sp.bp, BytesReceived = (uint)msg_len };
                MessageDispatcher(&msg_buff);
                // !Note, void-response messages are not acknowledged. 
                // Server would not be aware of client side error in this case.
                // This is by-design and an optimization to reduce void-response
                // message delivery latency. In streaming use cases this will be
                // very useful.
                try { if (pctx != 0) _PostResponseImpl(pctx, &msg_buff); }
                finally { Memory.free(msg_buff.Buffer); }
            }
            finally
            {
                poll_rsp.Dispose();
            }
        }

        private unsafe void _PostResponseImpl(long pctx, MessageBuff* messageBuff)
        {
            int header_len = TrinityProtocol.MsgHeader + sizeof(int) + sizeof(int) + sizeof(long);
            int socket_header = header_len + (int)messageBuff->BytesToSend - TrinityProtocol.SocketMsgHeader;

            byte* buf = stackalloc byte[header_len];
            byte** bufs = stackalloc byte*[2];
            int* sizes = stackalloc int[2];

            sizes[0] = header_len;
            sizes[1] = (int)messageBuff->BytesToSend;
            bufs[0] = buf;
            bufs[1] = messageBuff->Buffer;

            PointerHelper sp                                = PointerHelper.New(buf);
            *sp.ip                                          = socket_header;
            *(TrinityMessageType*)(sp.bp + TrinityProtocol.MsgTypeOffset) = TrinityMessageType.SYNC;
            *(ushort*)(sp.bp + TrinityProtocol.MsgIdOffset) = (ushort)TSL.CommunicationModule.TrinityClientModule.SynReqMessageType.PostResponse;
            sp.bp                                          += TrinityProtocol.MsgHeader;
            *sp.ip++                                        = m_id;
            *sp.ip++                                        = m_cookie;
            *sp.lp++                                        = pctx;
            m_mod.SendMessage(m_client, bufs, sizes, 2);
        }

        private void ScanClientConnectionFactory()
        {
            Log.WriteLine(LogLevel.Info, $"{nameof(TrinityClient)}: scanning for client connection factory.");
            var rank = ExtensionConfig.Instance.ResolveTypePriorities();
            Func<Type, int> rank_func = t =>
            {
                if(rank.TryGetValue(t, out var r)) return r;
                else return 0;
            };
            m_clientfactory = AssemblyUtility.GetBestClassInstance<IClientConnectionFactory, DefaultClientConnectionFactory>(null, rank_func);
        }

        protected override void StopCommunicationListeners()
        {
            m_tokensrc.Cancel();
            m_polltask.Wait();
            m_polltask = null;
            m_clientfactory.DisconnectAsync(m_client).Wait();
        }
    }
}
