using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Trinity.TSL;

namespace t_Namespace.MODULES
{
    public abstract class t_base_class_name : __meta
    {
        protected virtual void RootHttpHandler(HttpListenerContext context)
        {
            throw new NotImplementedException();
        }

        protected abstract void DispatchHttpRequest(HttpListenerContext ctx, string handler_name, string url);

    }
}
