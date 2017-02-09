// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trinity.Configuration;
using Trinity.Diagnostics;
using Trinity.TSL.Lib;

namespace Trinity.Network.Http
{
    /// <summary>
    /// Represents an HTTP request handler.
    /// </summary>
    /// <param name="context">An context object that provides access to the request and response objects.</param>
    public delegate void HttpHandler(HttpListenerContext context);

    /// <summary>
    /// Represents an HTTP server.
    /// </summary>
    public class TrinityHttpServer : IDisposable
    {
        private HttpListener m_HttpServer                   = null;
        private HttpHandler  m_HttpHandler                  = null;
        private List<AvailabilityGroup> m_RelayInstanceList = null;
        private List<int>    m_RelayRoundRobinTable         = null;
        private bool         m_AllowCrossDomainRequest      = true;

        /// <summary>
        /// Initializes a new instance of Trinity HTTP server with the specified <see cref="T:Trinity.Network.Http.HttpListener"/> and service endpoints.
        /// </summary>
        /// <param name="primaryHandler">An HTTP request handler.</param>
        /// <param name="serviceEndpoints">A list of Uniform Resource Identifier (URI) prefixes handled by this HTTP server.</param>
        public TrinityHttpServer(HttpHandler primaryHandler, List<string> serviceEndpoints)
        {
            m_HttpHandler = primaryHandler;
            m_HttpServer  = new HttpListener();
            foreach (var prefix in serviceEndpoints)
            {
                m_HttpServer.Prefixes.Add(prefix);
            }
            m_HttpServer.Start();
        }

        internal void SetAuthenticationSchemes(AuthenticationSchemes schemes)
        {
            m_HttpServer.AuthenticationSchemes = schemes;
        }

        internal void AddServiceEndpoint(string serviceEndpoint)
        {
            m_HttpServer.Prefixes.Add(serviceEndpoint);
        }

        internal void RemoveServiceEndpoint(string serviceEndpoint)
        {
            m_HttpServer.Prefixes.Remove(serviceEndpoint);
        }

        /// <summary>
        /// Sets the instance list. This is required for <see cref="Trinity.Network.Http.TrinityHttpServer.RelayRequest"/> to function properly.
        /// This method will be called by TSL-generated code upon server start up.
        /// </summary>
        /// <param name="list">The server list.</param>
        public void SetInstanceList(List<AvailabilityGroup> list)
        {
            this.m_RelayInstanceList = new List<AvailabilityGroup>(list);
            this.m_RelayRoundRobinTable = new List<int>(Enumerable.Repeat(0, list.Count));
        }

        /// <summary>
        /// Starts listening for incoming HTTP requests.
        /// </summary>
        public void Listen()
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                try
                {
                    while (m_HttpServer.IsListening)
                    {
                        ThreadPool.QueueUserWorkItem((context) =>
                        {
                            ProcessHttpRequest(context as HttpListenerContext);
                        }, m_HttpServer.GetContext());
                    }
                }
                catch (Exception) { }
            });
        }

        private void ProcessHttpRequest(HttpListenerContext ctx)
        {
            // Error handling: Microsoft OneAPI Guidelines section 7.10.2
            try
            {
                if (m_AllowCrossDomainRequest)
                {
                    ctx.Response.AddHeader("Access-Control-Allow-Origin", "*");
                }

                if (m_HttpServer.AuthenticationSchemes != AuthenticationSchemes.None)
                {
                    ctx.Response.AddHeader("Access-Control-Allow-Credentials", "true");
                }

                m_HttpHandler(ctx);
            }
            catch (BadRequestException ex)
            {
                WriteResponseForBadRequest(ex, ctx);
                try { Log.WriteLine(LogLevel.Warning, "Bad http request: " + ex.Code + ": " + ex.ToString()); }
                catch { }
            }
            catch (Exception ex)
            {
                WriteResponseForInternalServerError(ctx);
                try { Log.WriteLine(LogLevel.Error, ex.ToString()); }
                catch { }
            }
            finally
            {
                try
                {
                    ctx.Response.OutputStream.Close();
                }
                catch (Exception) { }
            }
        }

        private void WriteResponseForInternalServerError(HttpListenerContext ctx)
        {
            ctx.Response.StatusCode  = (int)HttpStatusCode.InternalServerError;
            using (var writer = new StreamWriter(ctx.Response.OutputStream))
            {
                writer.Write(@"{
  ""error"": {
    ""code"": ""Unspecified"",
    ""message"": ""Internal server error""
  }
}");
            }
        }

        private void WriteResponseForBadRequest(BadRequestException ex, HttpListenerContext ctx)
        {
            ctx.Response.StatusCode  = (int)HttpStatusCode.BadRequest;
            using (var writer = new StreamWriter(ctx.Response.OutputStream))
            {
                writer.Write(@"{
  ""error"": {
    ""code"": " + JsonStringProcessor.escape(ex.Code) + @",
    ""message"": " + JsonStringProcessor.escape(ex.Message) + @"
  }
}");
            }
        }

        private static void _RelayRequest(HttpListenerContext context, AvailabilityGroup ag, int idx)
        {
            ServerInfo        s          = ag.Instances[idx];
            int               port       = context.Request.LocalEndPoint.Port;
            string            requestUrl = context.Request.RawUrl;

            if (requestUrl == String.Empty || !requestUrl.StartsWith("/", StringComparison.Ordinal))
                requestUrl = requestUrl + "/";

            requestUrl = requestUrl.Substring(requestUrl.IndexOf('/', 1));

            string         relayUrl     = String.Format(CultureInfo.InvariantCulture, "http://{0}:{1}{2}", s.HostName, port, requestUrl);
            HttpWebRequest relayRequest = WebRequest.CreateHttp(relayUrl);
            relayRequest.Proxy          = null;

            if (context.Request.HttpMethod == "POST")
            {
                Stream relayStream = relayRequest.GetRequestStream();
                context.Request.InputStream.CopyTo(relayStream);
            }

            WebResponse relayResponse = relayRequest.GetResponse();
            context.Response.ContentType = relayResponse.ContentType;
            relayResponse.GetResponseStream().CopyTo(context.Response.OutputStream);
            relayResponse.Dispose();
        }

        /// <summary>
        /// Relays an HTTP request to the specified instance. Should only be called if <see cref="Trinity.Network.Http.TrinityHttpServer.SetInstanceList"/> is properly called.
        /// </summary>
        /// <param name="instance_id">The server id.</param>
        /// <param name="context">The context of the request to relay.</param>
        public void RelayRequest(int instance_id, HttpListenerContext context)
        {
            if (instance_id < 0 || instance_id >= m_RelayInstanceList.Count)
            {
                Log.WriteLine(LogLevel.Error, "RelayRequest: relay target server index out of bound.");
                Log.WriteLine(LogLevel.Error, "RelayRequest: Index: {0}.", instance_id);
                Log.WriteLine(LogLevel.Error, "RelayRequest: Remote endpoint: {0}.", context.Request.RemoteEndPoint.ToString());
            }

            AvailabilityGroup ag  = m_RelayInstanceList[instance_id];
            int               idx = m_RelayRoundRobinTable[instance_id] = (m_RelayRoundRobinTable[instance_id] + 1) % ag.Instances.Count;
            _RelayRequest(context, ag, idx);
        }

        public void Dispose()
        {
            if (m_HttpServer != null && m_HttpServer.IsListening)
            {
                try
                {
                    m_HttpServer.Stop();
                    (m_HttpServer as IDisposable).Dispose();
                    m_HttpServer = null;
                }
                catch { }
            }
        }
    }
}
