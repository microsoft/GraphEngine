#include "common.h"
#include <string>
#include "SyntaxNode.h"

using std::string;

namespace Trinity
{
    namespace Codegen
    {
        namespace Modules
        {
            string* 
HTTPModule(
NProtocolGroup* node, ModuleContext* context)
            {
                string* source = new string();
                
source->append(R"::(
    public abstract partial class )::");
source->append(Codegen::GetString(node->name));
source->append(R"::(Base : )::");
source->append(Codegen::GetString(get_comm_class_basename(node)));
source->append(R"::(
    {
        #region Handler lookup table
        private static Dictionary<string, uint> s_HttpHandlerLookupTable = new Dictionary<string, uint>
        {
            )::");
for (size_t iterator_1 = 0; iterator_1 < (node->protocolList)->size();++iterator_1)
{
if ((*(node->protocolList))[iterator_1]->referencedNProtocol->is_http_protocol())
{
source->append(R"::(
            { ")::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(", )::");
source->append(Codegen::GetString(iterator_1));
source->append(R"::(  }
            ,)::");
}
}
source->append(R"::(
        };
        #endregion
        )::");
for (size_t iterator_1 = 0; iterator_1 < (node->protocolList)->size();++iterator_1)
{
if ((*(node->protocolList))[iterator_1]->referencedNProtocol->is_http_protocol())
{
source->append(R"::(
        public abstract void )::");
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(Handler()::");
source->append(Codegen::GetString(get_http_handler_parameters((*(node->protocolList))[iterator_1]->referencedNProtocol)));
source->append(R"::();
        )::");
}
}
source->append(R"::(
        /// <summary>
        /// Processes requests on the root endpoint. By default it
        /// will list available API endpoints in html.
        /// Override this method to get custom behaviors.
        /// </summary>
        /// <param name="ctx">A <see cref="HttpListenerContext"/> object.</param>
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
            var querystring_idx =)::");
source->append(R"::( url.IndexOf('?');
            switch (handler_id)
            {
                )::");
for (size_t iterator_1 = 0; iterator_1 < (node->protocolList)->size();++iterator_1)
{
if ((*(node->protocolList))[iterator_1]->referencedNProtocol->is_http_protocol())
{
source->append(R"::(
                case  )::");
source->append(Codegen::GetString(iterator_1));
source->append(R"::( :
                    {
                        )::");
if ((*(node->protocolList))[iterator_1]->referencedNProtocol->pt_request == PT_STRUCT_REQUEST)
{
source->append(R"::(
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
                                json_string = sr.ReadToEnd();
                        }
                        else
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                            return;
                        }
                        if (!t_struct_name.TryParse(json_string, out request_struct))
                 )::");
source->append(R"::(       {
                            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                            return;
                        }
                        )::");
}
if ((*(node->protocolList))[iterator_1]->referencedNProtocol->pt_response == PT_STRUCT_RESPONSE)
{
source->append(R"::(t_struct_name   response_struct ;
                        )::");
}
source->append(Codegen::GetString((*(node->protocolList))[iterator_1]->name));
source->append(R"::(Handler()::");
source->append(Codegen::GetString(get_http_handler_calling_parameters((*(node->protocolList))[iterator_1]->referencedNProtocol)));
source->append(R"::();
                        )::");
if ((*(node->protocolList))[iterator_1]->referencedNProtocol->pt_response == PT_STRUCT_RESPONSE)
{
source->append(R"::(
                        context.Response.ContentType = "application/json";
                        string jsonp_callback  = context.Request.QueryString["jsonp_callback"] ?? context.Request.QueryString["callback"];
                        string iframe_callback = context.Request.QueryString["iframe_callback"];
                        using (var sw = new System.IO.StreamWriter(context.Response.OutputStream))
                        {
                            if (jsonp_callback != null)
                            {
                                sw.Write("{0}(", jsonp_callback);
                                sw.Write(Serializer.ToString(response_struct));
                                sw.Write(");", jsonp_callback);
                            }
                            else if (iframe_callback != null)
                            {
                                context.Response.ContentType = "text/html";
                                sw.Write("<script language=\"javascript\" type)::");
source->append(R"::(=\"text/javascript\">window.top.window.{0}(", iframe_callback);
                                sw.Write(Serializer.ToString(response_struct));
                                sw.Write(");</script>");
                            }
                            else
                            {
                                sw.Write(Serializer.ToString(response_struct));
                            }
                        }
                        )::");
}
source->append(R"::(
                    }
                    break;
                )::");
}
}
source->append(R"::(
            }
        }
    }
    )::");

                return source;
            }
        }
    }
}
