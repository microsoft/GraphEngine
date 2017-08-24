using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Trinity.Extension;
using Trinity.Network;

namespace Trinity.DynamicCluster
{
    [AutoRegisteredCommunicationModule]
    class DynamicClusterCommModule : CommunicationModule
    {
        public override string GetModuleName()
        {
            return "DynamicClusterCommModule";
        }

        protected override void DispatchHttpRequest(HttpListenerContext ctx, string endpointName, string url)
        {
            throw new NotImplementedException();
        }

        protected override void RegisterMessageHandler()
        {
            throw new NotImplementedException();
        }

        protected override void RootHttpHandler(HttpListenerContext ctx)
        {
            throw new NotImplementedException();
        }
    }
}
