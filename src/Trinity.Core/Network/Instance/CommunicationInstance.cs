// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Trinity.Diagnostics;
using Trinity.Extension;
using Trinity.Network.Http;
using Trinity.Network.Messaging;
using Trinity.Network.Sockets;
using Trinity.Storage;
using Trinity.Utilities;

namespace Trinity.Network
{
    /// <summary>
    /// The method signature for a message dispatching procedure.
    /// </summary>
    /// <param name="sendRecvBuff"></param>
    public unsafe delegate void* MessageDispatchProc(MessageBuff* sendRecvBuff);

    /// <summary>
    /// Represents a Trinity instance that can be started and perform two-way communication with other instances.
    /// </summary>
    public abstract class CommunicationInstance : CommunicationProtocolGroup
    {
        #region Fields
        private TrinityHttpServer m_HttpServer = null;
        private Dictionary<string, CommunicationModule> m_CommunicationModules = new Dictionary<string, CommunicationModule>();
        private ushort m_SynReqIdOffset;
        private ushort m_SynReqRspIdOffset;
        private ushort m_AsynReqIdOffset;
        private ushort m_AsynReqRspIdOffset;
        private MemoryCloud memory_cloud;
        private bool m_started = false;
        private object m_lock = new object();
        private ManualResetEventSlim m_module_init_signal = new ManualResetEventSlim(initialState: false);
        private MessageDispatchProc m_dispatcher = null;
        #endregion

        /// <summary>
        /// Get a reference to the MemoryCloud that the current communication instance is connected to.
        /// </summary>
        public MemoryCloud CloudStorage
        {
            get
            {
                return memory_cloud;
            }
        }

        /// <summary>
        /// Register user-defined message handlers.
        /// </summary>
        protected override void RegisterMessageHandler()
        {
            /* Stock instance does not have additional handlers. */
        }

        internal sealed override CommunicationInstance GetCommunicationInstance()
        {
            return this;
        }

        private void _InitializeModules()
        {
            HashSet<Type> cur_types = new HashSet<Type>();
            HashSet<Type> add_types = new HashSet<Type>(GetRegisteredCommunicationModuleTypes());
            Type[] ctor_ptypes = new Type[] { };
            object[] ctor_params = new object[] { };

            ICommunicationSchema schema = this.GetCommunicationSchema();

            this.SynReqIdOffset = (ushort)schema.SynReqProtocolDescriptors.Count();
            this.SynReqRspIdOffset = (ushort)schema.SynReqRspProtocolDescriptors.Count();
            this.AsynReqIdOffset = (ushort)schema.AsynReqProtocolDescriptors.Count();
            this.AsynReqRspIdOffset = (ushort)schema.AsynReqRspProtocolDescriptors.Count();

            /* TODO check circular dependency */

            while (add_types.Count != 0)
            {
                var added_modules = add_types
                    .Select(type => type.GetConstructor(ctor_ptypes).Invoke(ctor_params) as CommunicationModule)
                    .ToList();

                foreach (var t in add_types) { cur_types.Add(t); }

                add_types.Clear();

                foreach (var m in added_modules)
                {
                    /* Register and initialize the module */

                    m_CommunicationModules[m.GetModuleName()] = m;
                    m.Initialize(this);

                    foreach (var t in m.GetRegisteredCommunicationModuleTypes())
                    {
                        if (!cur_types.Contains(t))
                            add_types.Add(t);
                    }
                }
            }
        }

        #region Http
        /// <summary>
        /// The handler that processes requests on the root endpoint. 
        /// The default handler responds with a list of available endpoints. 
        /// This method can be overridden for custom behaviors.
        /// </summary>
        /// <param name="ctx">The context object.</param>
        protected override Task RootHttpHandlerAsync(HttpListenerContext ctx)
        {
            return CommonHttpHandlers.ListAvailableEndpointsAsync(ctx, new List<string> { }, this.GetType());
        }

        /// <summary>
        /// Dispatches an http request to a handler. The default dispatcher routes all requests to the root handler.
        /// This method can be overridden for custom dispatching logic.
        /// </summary>
        /// <remarks>
        /// This method will be overridden by TSL-generated stub classes.
        /// </remarks>
        /// <param name="ctx">The context object.</param>
        /// <param name="handlerName">The name of the handler.</param>
        /// <param name="url"></param>
        protected override Task DispatchHttpRequestAsync(HttpListenerContext ctx, string handlerName, string url)
        {
            return RootHttpHandlerAsync(ctx);
        }

        private void StartHttpServer()
        {
            try
            {
                int http_port = TrinityConfig.HttpPort;
                if (http_port <= UInt16.MinValue || http_port > UInt16.MaxValue) { return; }

                List<string> endpoints = new List<string> { string.Format(CultureInfo.InvariantCulture, "http://+:{0}/", http_port) };
                m_HttpServer = new TrinityHttpServer(_HttpHandlerAsync, endpoints);

                if (RunningMode == Trinity.RunningMode.Server)
                    m_HttpServer.SetInstanceList(TrinityConfig.Servers);
                else if (RunningMode == Trinity.RunningMode.Proxy)
                    m_HttpServer.SetInstanceList(TrinityConfig.Proxies);

                Task fireAndForget = m_HttpServer.ListenAsync();
                Log.WriteLine(LogLevel.Info, "HTTP server listening on port {0}", http_port);
            }
            catch
            {
                Log.WriteLine(LogLevel.Error, "Failed to start HTTP server. HTTP endpoints are disabled.");
            }
        }

        private void StopHttpServer()
        {
            if (m_HttpServer == null) return;

            try
            {
                m_HttpServer.Dispose();
                m_HttpServer = null;
                Log.WriteLine(LogLevel.Info, "HTTP server stopped");
            }
            catch
            {
                Log.WriteLine(LogLevel.Error, "Failed to stop HTTP server.");
            }
        }

        /// <summary>
        /// The primary http handler that routes the request to the proper handler.
        /// </summary>
        private Task _HttpHandlerAsync(HttpListenerContext context)
        {
            //  The raw url will be starting with "/", trimming all authority part.
            var url = Uri.UnescapeDataString(context.Request.RawUrl);
            var separator_idx = url.IndexOf('/', 1);

            //  If url is "" or "/", or "/{Service_Name}/ cannot be determined (lacking the second separator),
            //  it means that the request is pointing to the root endpoint.
            if (url.Length < 2 || url[1] == '?' || separator_idx == -1)
            {
                return RootHttpHandlerAsync(context);
            }

            var endpoint_name = url.Substring(1, separator_idx - 1);

            //  This request might be of the form "/{instance_id}/..."
            uint instance_id;
            if (UInt32.TryParse(endpoint_name, out instance_id))
            {
                //  In this case, we relay this message to the desired instance. 
                return m_HttpServer.RelayRequestAsync((int)instance_id, context);
            }

            //  This request might be of the form "/{module_name}/..."
            CommunicationModule module = null;
            if (m_CommunicationModules.TryGetValue(endpoint_name, out module))
            {
                /* Swallow the module_name part and reset the variables as if we're working on the root. */
                url = url.Substring(separator_idx);
                separator_idx = url.IndexOf('/', 1);

                if (url.Length < 2 || url[1] == '?' || separator_idx == -1)
                {
                    return module.GetRootHttpHandler()(context);
                }
                else
                {
                    endpoint_name = url.Substring(1, separator_idx - 1);
                    return module.GetHttpRequestDispatcher()(context, endpoint_name, url.Substring(separator_idx + 1));
                }
            }

            //  Otherwise, this request should be dispatched by
            return DispatchHttpRequestAsync(context, endpoint_name, url.Substring(separator_idx + 1));
        }
        #endregion

        /// <summary>
        /// The running mode of the current instance.
        /// </summary>
        protected internal abstract RunningMode RunningMode { get; }

        /// <summary>
        /// Starts a Trinity instance.
        /// </summary>
        public unsafe void Start()
        {
            lock (m_lock)
            {
                if (m_started) return;
                try
                {
                    Log.WriteLine(LogLevel.Debug, "Starting communication instance.");
                    Global.CommunicationInstance = this;
                    TrinityConfig.CurrentRunningMode = this.RunningMode;

                    //  Initialize message handlers
                    MessageHandlers.Initialize();
                    RegisterMessageHandler();
                    MessageDispatcher = (_) => { _MessageInitializationTrapAsync(_); return null; };

                    //  Bring up networking subsystems
                    StartCommunicationListeners();

                    //  Initialize cloud storage
                    memory_cloud = Global.CloudStorage;

                    //  Initialize the modules
                    _ScanForAutoRegisteredModules();
                    _InitializeModules();

                    //  Modules initialized, release pending messages from the trap
                    m_module_init_signal.Set();
                    MessageDispatcher = (_) => { MessageHandlers.DefaultParser.DispatchMessageAsync(_); return null; };

                    Log.WriteLine("Working Directory: {0}", Global.MyAssemblyPath);
                    Log.WriteLines(TrinityConfig.OutputCurrentConfig());

                    m_started = true;
                    Log.WriteLine("{0} {1} is successfully started.", RunningMode, memory_cloud.MyInstanceId);
                    _RaiseStartedEventsAsync().Wait();
                }
                catch (Exception ex)
                {
                    Log.WriteLine(LogLevel.Error, "CommunicationInstance: " + ex.ToString());
                }
            }
        }

        private unsafe Task _MessageInitializationTrapAsync(MessageBuff* sendRecvBuff)
        {
            m_module_init_signal.Wait();
            return MessageHandlers.DefaultParser.DispatchMessageAsync(sendRecvBuff);
        }

        /// <summary>
        /// Stops a Trinity instance.
        /// </summary>
        public void Stop()
        {
            lock (m_lock)
            {
                if (!m_started) return;
                try
                {
                    int id = memory_cloud.MyInstanceId;
                    Log.WriteLine(LogLevel.Debug, "Stopping communication instance.");

                    //  TODO notify the modules
                    StopCommunicationListeners();

                    m_module_init_signal.Reset();

                    //  Unregister cloud storage
                    memory_cloud = null;

                    //  Unregister communication instance
                    Global.CommunicationInstance = null;

                    m_started = false;
                    Log.WriteLine("{0} {1} is successfully stopped.", RunningMode, id);
                }
                catch (Exception ex)
                {
                    Log.WriteLine(LogLevel.Error, "CommunicationInstance: " + ex.ToString());
                }
            }
        }

        /// <summary>
        /// A delegate that points to the message dispatch and processing procedure.
        /// </summary>
        public MessageDispatchProc MessageDispatcher
        {
            get => m_dispatcher;
            set
            {
                m_dispatcher = value;
                GC.SuppressFinalize(m_dispatcher);
                var pfn_dispatch = Marshal.GetFunctionPointerForDelegate(m_dispatcher);
                for (ushort i = 0; i<(ushort)TrinityMessageType.MESSAGE_TYPE_MAX; ++i)
                {
                    Global.RegisterMessageHandler(i, pfn_dispatch);
                }
            }
        }

        /// <summary>
        /// Start listening for incoming connections. When this method is called,
        /// it is guaranteed that <see cref="MessageDispatcher"/> is available for consumption.
        /// </summary>
        protected virtual void StartCommunicationListeners()
        {
            var _config = TrinityConfig.CurrentClusterConfig;
            var _si = _config.GetMyServerInfo() ?? _config.GetMyProxyInfo();
            var _my_ip = Global.MyIPAddress;

            if (_si != null) _my_ip = NetworkUtility.Hostname2IPv4Address(_si.HostName);

            //  Initialize message passing networking
            NativeNetwork.StartTrinityServer((UInt16)_config.ListeningPort);
            //  XXX might not be accurate: NativeNetwork.StartTrinityServer listens on all servers.
            Log.WriteLine("My IPEndPoint: " + _my_ip + ":" + _config.ListeningPort);

            //  Initialize Http server
            StartHttpServer();
        }

        /// <summary>
        /// Stop listening for incoming connections.
        /// </summary>
        protected virtual void StopCommunicationListeners()
        {
            StopHttpServer();

            //  Shutdown message passing networking
            NativeNetwork.StopTrinityServer();
        }

        private void _ScanForAutoRegisteredModules()
        {
            Log.WriteLine("Scanning for auto-registered communication modules.");
            foreach (var m in AssemblyUtility.GetAllClassTypes<CommunicationModule, AutoRegisteredCommunicationModuleAttribute>())
            {
                m_RegisteredModuleTypes.Add(m);
            }
        }

        private async Task _RaiseStartedEventsAsync()
        {
            await this._RaiseStartedEventAsync();
            foreach (var module in m_CommunicationModules.Values)
                await module._RaiseStartedEventAsync();
            Global._RaiseCommunicationInstanceStarted();
        }

        internal T _GetCommunicationModule_impl<T>() where T : CommunicationModule
        {
            return m_CommunicationModules.FirstOrDefault(_ => _.Value is T).Value as T;
        }

        internal CommunicationModule _GetCommunicationModuleByName(string moduleName)
        {
            CommunicationModule module = null;
            m_CommunicationModules.TryGetValue(moduleName, out module);
            return module;
        }

        #region Message id offsets
        internal ushort SynReqIdOffset
        {
            get { return m_SynReqIdOffset; }
            set { m_SynReqIdOffset = value; }
        }

        internal ushort SynReqRspIdOffset
        {
            get { return m_SynReqRspIdOffset; }
            set { m_SynReqRspIdOffset = value; }
        }

        internal ushort AsynReqIdOffset
        {
            get { return m_AsynReqIdOffset; }
            set { m_AsynReqIdOffset = value; }
        }

        internal ushort AsynReqRspIdOffset
        {
            get { return m_AsynReqRspIdOffset; }
            set { m_AsynReqRspIdOffset = value; }
        }
        #endregion

        /// <summary>
        /// Sets the authentication schemes of the Http endpoints.
        /// </summary>
        /// <param name="auth_schemes">A value of <see cref="System.Net.AuthenticationSchemes"/>, specifying acceptable authentication schemes.</param>
        internal void SetHttpAuthenticationSchemes(AuthenticationSchemes auth_schemes)
        {
            this.m_HttpServer.SetAuthenticationSchemes(auth_schemes);
        }

        /// <summary>
        /// Occurs when an exception is not caught by a message handler.
        /// </summary>
        public event MessagingUnhandledExceptionEventHandler UnhandledException;

        /// <summary>
        /// It is guaranteed that this method does not throw exceptions.
        /// When no event handlers subscribes to UnhandledException of the current running communication instance (if there is one),
        /// then the default exception logging routine will be called.
        /// </summary>
        internal static unsafe void _RaiseUnhandledExceptionEvents(object reqArgs, MessagingUnhandledExceptionEventArgs e)
        {
            try
            {
                CommunicationInstance comm_instance = Global.CommunicationInstance;
                MessagingUnhandledExceptionEventHandler exception_event_handler = null;
                if (comm_instance != null) { exception_event_handler = comm_instance.UnhandledException; }
                if (exception_event_handler != null)
                {
                    exception_event_handler(reqArgs, e);
                }
                else
                {
                    _LogMessageReqArgsAndException(reqArgs, e);
                }
            }
            catch (Exception exception)
            {
                //The unhandled exception event handler throws exception.
                //We first log the original exception down, and then explain
                //how the exception handler failed.
                _LogMessageReqArgsAndException(reqArgs, e);
                Log.WriteLine(LogLevel.Error, "Exceptions are caught in the UnhandledException event handler.");
                Log.WriteLine(LogLevel.Error, exception.Message);
                Log.WriteLine(LogLevel.Error, exception.StackTrace);
            }
        }

        private static unsafe void _LogMessageReqArgsAndException(object reqArgs, MessagingUnhandledExceptionEventArgs e)
        {
            Debug.Assert(reqArgs != null);
            Debug.Assert(e != null && e.ExceptionObject != null && e.Buffer != null);

            string message_type = "unknown message";
            if (reqArgs is AsynReqArgs) { message_type = "asynchronous message"; }
            if (reqArgs is SynReqArgs) { message_type = "synchronous message"; }
            if (reqArgs is SynReqRspArgs) { message_type = "synchronous message (with RSP request)"; }

            Log.WriteLine(LogLevel.Error, "Exceptions are caught in the handler of {0}, message sn: {1}", message_type, e.Buffer[TrinityProtocol.MsgIdOffset]);
            Log.WriteLine(LogLevel.Error, e.ExceptionObject.Message);
            Log.WriteLine(LogLevel.Error, e.ExceptionObject.StackTrace);
            Log.WriteLine(LogLevel.Error, "Message buffer length: {0}", e.Size);
            Log.WriteLine(LogLevel.Error, "Hexadecimal dump of the message buffer (first 128 bytes):");
            Log.WriteLine(LogLevel.Error);
            Log.WriteLine(LogLevel.Error, HexDump.ToString(e.Buffer, e.Size + TrinityProtocol.TrinityMsgHeader, 128));
            Log.WriteLine(LogLevel.Error);
            Log.Flush();
        }
    }
}
