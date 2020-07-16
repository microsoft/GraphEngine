using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Trinity.Storage;
using Trinity.TSL;

namespace t_Namespace.MODULES
{
    public abstract class t_base_class_name : __meta
    {
        protected ushort t_protocol_typeIdOffset = 0;
        internal static int s_t_protocol_name_token_counter;
        internal static Dictionary<int, TaskCompletionSource<t_protocol_responseReader>> s_t_protocol_name_token_sources;

        protected virtual void RegisterMessageHandler()
        {

        }

        protected unsafe Task SendMessageAsync(IMessagePassingEndpoint messagePassingEndpoint, byte* buffer, int v)
        {
            throw new NotImplementedException();
        }

        protected virtual Task RootHttpHandlerAsync(HttpListenerContext context)
        {
            throw new NotImplementedException();
        }

        protected abstract Task DispatchHttpRequestAsync(HttpListenerContext ctx, string handler_name, string url);

    }
}
