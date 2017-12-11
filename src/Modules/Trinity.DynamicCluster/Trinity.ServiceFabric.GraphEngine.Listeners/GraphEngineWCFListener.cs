using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Trinity.Networking.WCFService;
using Trinity.Core.Lib;

namespace Trinity.ServiceFabric.GraphEngine.Listeners
{
    public class GraphEngineWCFAdapter : ITrinityWCFAdapter
    {
        public unsafe void SendMessageWithoutResponse(byte[] request)
        {
            fixed(byte* p = request)
            {
                Global.LocalStorage.SendMessage(p, request.Length);
            }
        }

        public unsafe byte[] SendMessageWithResponse(byte[] request)
        {
            fixed(byte* p = request)
            {
                Global.LocalStorage.SendMessage(p, request.Length, out var rsp);
                byte[] ret = new byte[rsp.Size];
                Memory.Copy(rsp.Buffer, rsp.Offset, ret, 0, rsp.Size);
                rsp.Dispose();
                return ret;
            }
        }
    }

    public class GraphEngineWCFListener : ICommunicationListener
    {
        public void Abort()
        {
            throw new System.NotImplementedException();
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}