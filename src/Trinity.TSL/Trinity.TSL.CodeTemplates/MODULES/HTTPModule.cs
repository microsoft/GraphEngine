#pragma warning disable 0162 // disable the "unreachable code" warning
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using t_Namespace.MODULES;
using Trinity;
using Trinity.Network;
using Trinity.Network.Http;
using Trinity.TSL;
using Trinity.TSL.Lib;

namespace t_Namespace
{
    [MODULE_BEGIN]
    [TARGET("NProtocolGroup")]
    [MAP_VAR("t_server", "node")]
    [MAP_VAR("t_base_class_name", "get_comm_class_basename(node)")]
    [MAP_VAR("t_server_name", "node->name")]
    [MAP_LIST("t_protocol", "node->protocolList")]
    [MAP_VAR("t_protocol_name", "name", MemberOf = "t_protocol")]
    [MAP_VAR("t_protocol", "referencedNProtocol")]
    public abstract partial class t_server_nameBase : t_base_class_name
    {
        #region Handler lookup table
        private static Dictionary<string, uint> s_HttpHandlerLookupTable = new Dictionary<string, uint>
        {
            /*FOREACH*/
            /*IF("$t_protocol->is_http_protocol()")*/
            { "t_protocol_name",/*MUTE*/ 0 /*MUTE_END*/ /*GET_ITERATOR_VALUE()*/  }
            /*LITERAL_OUTPUT(",")*/
            /*END*/
            /*END*/
        };
        #endregion

        [FOREACH]
        [USE_LIST("t_protocol")]
        [IF("$t_protocol->is_http_protocol()")]
        public abstract Task t_protocol_nameHandlerAsync(/*META_OUTPUT("get_http_handler_parameters($t_protocol)")*/);
        [END]//IF
        [END]//FOREACH

            
        /// <summary>
        /// Processes requests on the root endpoint. By default it
        /// will list available API endpoints in html.
        /// Override this method to get custom behaviors.
        /// </summary>
        /// <param name="ctx">A <see cref="HttpListenerContext"/> object.</param>
        protected override Task RootHttpHandlerAsync(HttpListenerContext ctx)
        {
            return CommonHttpHandlers.ListAvailableEndpointsAsync(ctx, s_HttpHandlerLookupTable.Keys, this.GetType());
        }

        protected override async Task DispatchHttpRequestAsync(HttpListenerContext context, string endpoint_name, string url)
        {
            var method          = context.Request.HttpMethod;
            uint handler_id     = 0;

            if (!s_HttpHandlerLookupTable.TryGetValue(endpoint_name, out handler_id))
            {
                CommonHttpHandlers.PageNotFound(context);
                await RootHttpHandlerAsync(context);
                return;
            }

            var querystring_idx = url.IndexOf('?');

            switch (handler_id)
            {
                /*FOREACH*/
                /*USE_LIST("t_protocol")*/
                /*IF("$t_protocol->is_http_protocol()")*/
                case /*MUTE*/0/*MUTE_END*/ /*GET_ITERATOR_VALUE*/ :
                    {
                        IF("$t_protocol->pt_request == PT_STRUCT_REQUEST");
                        MAP_VAR("t_struct_name", "META_OUTPUT(tsl->find_struct_or_cell($t_protocol->request_message_struct)->name)");
                        string          json_string;
                        t_struct_name   request_struct;
                        if (method == "GET")
                        {
                            if (querystring_idx == -1)
                                json_string = url;
                            else
                                json_string = url.Substring(0, querystring_idx);
                        }
                        else if (method == "POST")
                        {
                            using (var sr = new System.IO.StreamReader(context.Request.InputStream))
                                json_string = await sr.ReadToEndAsync();
                        }
                        else
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                            return;
                        }
                        if (!t_struct_name.TryParse(json_string, out request_struct))
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                            return;
                        }
                        END();

                        IF("$t_protocol->pt_response == PT_STRUCT_RESPONSE");
                        MAP_VAR("t_struct_name", "META_OUTPUT(tsl->find_struct_or_cell($t_protocol->response_message_struct)->name)");
                        t_struct_name   response_struct /*MUTE*/ = null /*MUTE_END*/;
                        END();

                        await t_protocol_nameHandlerAsync(/*META_OUTPUT("get_http_handler_calling_parameters($t_protocol)")*/);

                        IF("$t_protocol->pt_response == PT_STRUCT_RESPONSE");

                        context.Response.ContentType = "application/json";

                        string jsonp_callback  = context.Request.QueryString["jsonp_callback"] ?? context.Request.QueryString["callback"];
                        string iframe_callback = context.Request.QueryString["iframe_callback"];
                        using (var sw = new System.IO.StreamWriter(context.Response.OutputStream))
                        {
                            if (jsonp_callback != null)
                            {
                                await sw.WriteAsync(string.Format("{0}({1});", jsonp_callback, Serializer.ToString(response_struct)));
                            }
                            else if (iframe_callback != null)
                            {
                                context.Response.ContentType = "text/html";
                                await sw.WriteAsync(string.Format("<script language=\"javascript\" type=\"text/javascript\">window.top.window.{0}({1});</script>", iframe_callback, Serializer.ToString(response_struct)));
                            }
                            else
                            {
                                await sw.WriteAsync(Serializer.ToString(response_struct));
                            }
                        }
                        END();
                    }
                    break;
                /*END*/
                /*END*/
            }
        }

    }
    /*MODULE_END*/

}
