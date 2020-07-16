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
using System.Threading.Tasks;
using Trinity.TSL.Lib;

namespace Trinity.Network.Http
{
    /// <summary>
    /// Represents a set of common http handlers.
    /// </summary>
    public static class CommonHttpHandlers
    {
        internal struct HttpEndpointDescriptor
        {
            public string address;
            public string requestType;
            public List<string> requestParameters;
            public string responseType;
            public List<string> responseParameters;

        }

        internal struct DefaultRootHandlerResponse
        {
            public List<HttpEndpointDescriptor> endpoints;
        }

        #region Data serializer, generated code
        /// <summary>
        /// Provides facilities for serializing data to Json strings.
        /// </summary>
        internal class Serializer
        {
            [ThreadStatic]
            static StringBuilder s_stringBuilder;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static void s_ensure_string_builder()
            {
                if (s_stringBuilder == null)
                    s_stringBuilder = new StringBuilder();
                else
                    s_stringBuilder.Clear();
            }

            /// <summary>
            /// Serializes a string object to a Json string.
            /// </summary>
            /// <param name="value">The target object to be serialized.</param>
            /// <returns>The serialized Json string.</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string ToString(string value)
            {
                s_ensure_string_builder();
                ToString(value, s_stringBuilder);
                return s_stringBuilder.ToString();
            }

            /// <summary>
            /// Serializes a List of string objects to a Json string.
            /// </summary>
            /// <param name="value">The target object to be serialized.</param>
            /// <returns>The serialized Json string.</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string ToString(List<string> value)
            {
                s_ensure_string_builder();
                ToString(value, s_stringBuilder);
                return s_stringBuilder.ToString();
            }

            /// <summary>
            /// Serializes a List of HttpEndpointDescriptor objects to a Json string.
            /// </summary>
            /// <param name="value">The target object to be serialized.</param>
            /// <returns>The serialized Json string.</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string ToString(List<HttpEndpointDescriptor> value)
            {
                s_ensure_string_builder();
                ToString(value, s_stringBuilder);
                return s_stringBuilder.ToString();
            }

            /// <summary>
            /// Serializes a DefaultRootHandlerResponse object to a Json string.
            /// </summary>
            /// <param name="value">The target object to be serialized.</param>
            /// <returns>The serialized Json string.</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string ToString(DefaultRootHandlerResponse value)
            {
                s_ensure_string_builder();
                ToString(value, s_stringBuilder);
                return s_stringBuilder.ToString();
            }

            /// <summary>
            /// Serializes a HttpEndpointDescriptor object to a Json string.
            /// </summary>
            /// <param name="value">The target object to be serialized.</param>
            /// <returns>The serialized Json string.</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static string ToString(HttpEndpointDescriptor value)
            {
                s_ensure_string_builder();
                ToString(value, s_stringBuilder);
                return s_stringBuilder.ToString();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal static void ToString(string value, StringBuilder str_builder)
            {

                str_builder.Append(JsonStringProcessor.escape(value));

            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal static void ToString(List<string> value, StringBuilder str_builder)
            {

                {
                    str_builder.Append('[');
                    bool first = true;
                    foreach (var element in value)
                    {
                        if (first)
                        {
                            first = false;
                        }
                        else
                        {
                            str_builder.Append(',');
                        }

                        {
                            ToString(element, str_builder);
                        }

                    }
                    str_builder.Append(']');
                }

            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal static void ToString(List<HttpEndpointDescriptor> value, StringBuilder str_builder)
            {

                {
                    str_builder.Append('[');
                    bool first = true;
                    foreach (var element in value)
                    {
                        if (first)
                        {
                            first = false;
                        }
                        else
                        {
                            str_builder.Append(',');
                        }

                        {
                            ToString(element, str_builder);
                        }

                    }
                    str_builder.Append(']');
                }

            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal static void ToString(DefaultRootHandlerResponse value, StringBuilder str_builder)
            {

                {

                    str_builder.Append('{');

                    {

                        if (value.endpoints != null)
                        {

                            str_builder.Append("\"endpoints\":");

                            ToString(value.endpoints, str_builder);

                        }

                    }

                    str_builder.Append('}');
                }

            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal static void ToString(HttpEndpointDescriptor value, StringBuilder str_builder)
            {

                {

                    str_builder.Append('{');

                    {

                        if (value.address != null)
                        {

                            str_builder.Append("\"address\":");

                            ToString(value.address, str_builder);

                        }

                    }

                    {

                        if (value.requestType != null)
                        {

                            str_builder.Append(',');

                            str_builder.Append("\"requestType\":");

                            ToString(value.requestType, str_builder);

                        }

                    }

                    {

                        if (value.requestParameters != null)
                        {

                            str_builder.Append(',');

                            str_builder.Append("\"requestParameters\":");

                            ToString(value.requestParameters, str_builder);

                        }

                    }

                    {

                        if (value.responseType != null)
                        {

                            str_builder.Append(',');

                            str_builder.Append("\"responseType\":");

                            ToString(value.responseType, str_builder);

                        }

                    }

                    {

                        if (value.responseParameters != null)
                        {

                            str_builder.Append(',');

                            str_builder.Append("\"responseParameters\":");

                            ToString(value.responseParameters, str_builder);

                        }

                    }

                    str_builder.Append('}');
                }

            }

            #region mute

            #endregion
        }

        #endregion

        /// <summary>
        /// Lists the available HTTP endpoints provided by this HTTP server.
        /// </summary>
        /// <param name="ctx">The HTTP request context.</param>
        /// <param name="availableEndpointNames">The list of available endpoints.</param>
        /// <param name="serverType">The type of the TrinityServer or TrinityProxy.</param>
        public static async Task ListAvailableEndpointsAsync(HttpListenerContext ctx, IEnumerable<string> availableEndpointNames, Type serverType)
        {
            string url_root = ctx.Request.Url.GetLeftPart(UriPartial.Authority);
            if (typeof(CommunicationModule).IsAssignableFrom(serverType))
            {
                //  A module is located at http://some_url/module_name/
                int separator_idx = ctx.Request.RawUrl.IndexOf('/', 1);
                if (separator_idx >= 0)
                {
                    string module_name = ctx.Request.RawUrl.Substring(1, separator_idx - 1);
                    url_root          += "/" + module_name;
                }
            }

            var responseObj = GenerateDefaultHttpHandlerResponseObject(url_root, availableEndpointNames, serverType);
            var rsp_json    = Serializer.ToString(responseObj);

            using (var sw = new StreamWriter(ctx.Response.OutputStream))
            {
                #region HTML output
                if (ctx.Request.AcceptTypes != null && !ctx.Request.AcceptTypes.Contains("application/json"))
                {
                    ctx.Response.ContentType = "text/html";
                    using (var html_stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Trinity.Network.Http.index.html"))
                    using (var stream_reader = new StreamReader(html_stream))
                    {
                        string html_tmpl = await stream_reader.ReadToEndAsync();
                        await sw.WriteLineAsync(html_tmpl.Replace(@"//%AVAILABLE_ENDPOINTS%", "var available_endpoints = " + rsp_json + ";"));
                    }
                }
                #endregion
                else
                {
                    ctx.Response.ContentType = "application/json";
                    await sw.WriteLineAsync(rsp_json);
                }
            }
        }

        #region Private implementations
        private static void GetProtocolDescription(Type param_type, out string type_str, out List<string> param_fields)
        {
            param_fields = new List<string>();
            if (param_type != null && param_type.IsByRef)
            {
                param_type = param_type.GetElementType();
            }

            /* Case 1: void request/response */
            if (param_type == null)
            {
                type_str = "void";
            }
            /* Case 2: stream request/response, denoted as '*' */
            else if (param_type == typeof(HttpListenerResponse))
            {
                type_str = "stream";
                param_fields.Add("*");
            }
            /* Case 3: TSL struct request/response, scan the names now */
            else
            {
                type_str = "json";
                foreach (var field in param_type.GetFields())
                {
                    param_fields.Add(field.Name);
                }
            }
        }

        private static HttpEndpointDescriptor GetHttpEndpointDescriptor(Type serverType, string url_root, string endpoint)
        {
            HttpEndpointDescriptor desc = new HttpEndpointDescriptor { address = String.Format(CultureInfo.InvariantCulture, "{0}/{1}/", url_root, endpoint) };

            string handler_name = endpoint + "Handler";
            var    handler      = serverType.GetMethod(handler_name);

            if (handler == null)
                return desc;

            //  The signature of a HTTP handler should be:
            //  void handler_name(req_type request, out rsp_type response)
            //    -or-
            //  void handler_name(out rsp_type response)
            var    parameters   = handler.GetParameters();
            var    req_param    = parameters.FirstOrDefault(_ => !_.IsOut);
            var    rsp_param    = parameters.FirstOrDefault(_ => _.IsOut);

            var    req_type     = req_param != null ? req_param.ParameterType : null;
            var    rsp_type     = rsp_param != null ? rsp_param.ParameterType : null;

            GetProtocolDescription(req_type, out desc.requestType, out desc.requestParameters);
            GetProtocolDescription(rsp_type, out desc.responseType, out desc.responseParameters);

            return desc;
        }

        private static DefaultRootHandlerResponse GenerateDefaultHttpHandlerResponseObject(string url_root, IEnumerable<string> availableEndpointNames, Type serverType)
        {
            DefaultRootHandlerResponse response = new DefaultRootHandlerResponse { endpoints = new List<HttpEndpointDescriptor>() };

            foreach (var endpoint in availableEndpointNames)
            {
                response.endpoints.Add(GetHttpEndpointDescriptor(serverType, url_root, endpoint));
            }

            return response;
        }
        #endregion

        /// <summary>
        /// Sends a "PageNotFound" response to an HTTP request.
        /// </summary>
        /// <param name="context">The corresponding context object of the HTTP request.</param>
        public static void PageNotFound(HttpListenerContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
        }
    }
}
