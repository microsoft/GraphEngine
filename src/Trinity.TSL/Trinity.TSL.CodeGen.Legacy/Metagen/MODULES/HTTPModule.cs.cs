using System.Text;
using System.Collections.Generic;

namespace Trinity.TSL.CodeTemplates
{
    internal partial class Modules
    {
        internal static string 
HTTPModule(
NProtocolGroup node, ModuleContext context)
        {
            StringBuilder source = new StringBuilder();
            
NProtocol* protocol_1;
source.Append(@"
    public abstract partial class ");
source.Append(Codegen.GetString(node->name));
source.Append(@"Base : ");
source.Append(Codegen.GetString(context->m_arguments[0]));
source.Append(@"
    {
        #region Handler lookup table
        private static Dictionary<string, uint> s_HttpHandlerLookupTable = new Dictionary<string, uint>
        {
            ");
for (int iterator_1 = 0; iterator_1 < (node.protocolList).Count;++iterator_1)
{
if (tsl->find_protocol((node.protocolList)[iterator_1].name)->is_http_protocol())
{
source.Append(@"
            { """);
source.Append(Codegen.GetString((node.protocolList)[iterator_1].name));
source.Append(@""",");
source.Append(Codegen.GetString(iterator_1));
source.Append(@"  }
            ,");
}
}
source.Append(@"
        };
        #endregion
        ");
for (int iterator_1 = 0; iterator_1 < (node.protocolList).Count;++iterator_1)
{
protocol_1 = tsl->find_protocol((node.protocolList)[iterator_1].name);
if (protocol_1->is_http_protocol())
{
source.Append(@"
        public abstract void ");
source.Append(Codegen.GetString((node.protocolList)[iterator_1].name));
source.Append(@"Handler(");
source.Append(Codegen.GetString(get_http_handler_parameters(protocol_1)));
source.Append(@");
        ");
}
}
source.Append(@"
        /// <summary>
        /// Processes requests on the root endpoint. By default it
        /// will list available API endpoints in html.
        /// Override this method to get custom behaviors.
        /// </summary>
        /// <param name=""ctx"">A <see cref=""HttpListenerContext""/> object.</param>
        protected override void RootHttpHandler(HttpListenerContext ctx)
        {
            CommonHttpHandlers.ListAvailableEndpoints(ctx, s_HttpHandlerLookupTable.Keys, this.GetType());
        }
        protected override void DispatchHttpRequest(HttpListenerContext context, string endpoint_name, string url)
        {
            var method          = context.Request.HttpMethod;
            uint handler_id     = 0;
            if (!s_HttpHandlerLookupTable.TryGetValue(endpoint_name, out handler_id))
            {
                CommonHttpHandlers.PageNotFound(context);
                RootHttpHandler(context);
                return;
            }
            var querystring_idx =");
source.Append(@" url.IndexOf('?');
            switch (handler_id)
            {
                ");
for (int iterator_1 = 0; iterator_1 < (node.protocolList).Count;++iterator_1)
{
protocol_1 = tsl->find_protocol((node.protocolList)[iterator_1].name);
if (protocol_1->is_http_protocol())
{
source.Append(@"
                case ");
source.Append(Codegen.GetString(iterator_1));
source.Append(@" :
                    {
                        ");
if (protocol_1->pt_request == PT_STRUCT_REQUEST)
{
source.Append(@"
                        string          json_string;
                        ");
source.Append(Codegen.GetString(tsl->find_struct_or_cell(protocol_1->request_message_struct)->name));
source.Append(@"   request_struct;
                        if (method == ""GET"")
                        {
                            if (querystring_idx == -1)
                                json_string = url;
                            else
                                json_string = url.Substring(0, querystring_idx);
                        }
                        else if (method == ""POST"")
                        {
                            using (var sr = new System.IO.StreamReader(context.Request.InputStream))
                                json_string = sr.ReadToEnd();
                        }
                        else
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                            return;
                        }
                        if (!");
source.Append(Codegen.GetString(tsl->find_struct_or_cell(protocol_1->request_message_struct)->name));
source.Append(@".TryParse(json_string, out request_struct))
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                            return;
                        }
                        ");
}
if (protocol_1->pt_response == PT_STRUCT_RESPONSE)
{
source.Append(Codegen.GetString(tsl->find_struct_or_cell(protocol_1->response_message_struct)->name));
source.Append(@"   response_struct ;
                        ");
}
source.Append(Codegen.GetString((node.protocolList)[iterator_1].name));
source.Append(@"Handler(");
source.Append(Codegen.GetString(get_http_handler_calling_parameters(protocol_1)));
source.Append(@");
                        ");
if (protocol_1->pt_response == PT_STRUCT_RESPONSE)
{
source.Append(@"
                        context.Response.ContentType = ""application/json"";
                        string jsonp_callback  = context.Request.QueryString[""jsonp_callback""] ?? context.Request.QueryString[""callback""];
                        string iframe_callback = context.Request.QueryString[""iframe_callback""];
                        using (var sw = new System.IO.StreamWriter(context.Response.OutputStream))
                        {
                            if (jsonp_callback != null)
                            {
                                sw.Write(""{0}("", jsonp_callback);
                                sw.Write(Serializer.ToString(response_struct));
                                sw.Write("");"", jsonp_callback);
                            }
                            else if (iframe_callback != null)
                            {
                                context.Response.ContentType = ""text/html"";
                                sw.Write(""<script language=\""javascript\"" type");
source.Append(@"=\""text/javascript\"">window.top.window.{0}("", iframe_callback);
                                sw.Write(Serializer.ToString(response_struct));
                                sw.Write("");</script>"");
                            }
                            else
                            {
                                sw.Write(Serializer.ToString(response_struct));
                            }
                        }
                        ");
}
source.Append(@"
                    }
                    break;
                ");
}
}
source.Append(@"
            }
        }
    }
    ");

            return source.ToString();
        }
    }
}
