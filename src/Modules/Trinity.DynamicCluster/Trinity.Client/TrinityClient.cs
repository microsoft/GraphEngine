using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Trinity.Network;
using Trinity.Network.Messaging;
using Trinity.Storage;

namespace Trinity.Client
{
    public class TrinityClient : CommunicationInstance, IMessagePassingEndpoint
    {
        protected override sealed RunningMode RunningMode => RunningMode.Client;

        public T GetModule<T>() where T : CommunicationModule => GetCommunicationModule<T>();

        public unsafe void SendMessage(byte* message, int size)
        {
            throw new NotImplementedException();
        }

        public unsafe void SendMessage(byte* message, int size, out TrinityResponse response)
        {
            throw new NotImplementedException();
        }

        public unsafe void SendMessage(byte** message, int* sizes, int count)
        {
            throw new NotImplementedException();
        }

        public unsafe void SendMessage(byte** message, int* sizes, int count, out TrinityResponse response)
        {
            throw new NotImplementedException();
        }

        protected override sealed void DispatchHttpRequest(HttpListenerContext ctx, string handlerName, string url)
            => throw new NotSupportedException();

        protected override sealed void RootHttpHandler(HttpListenerContext ctx)
            => throw new NotSupportedException();

        protected override void StartCommunicationListeners()
        {
        }

        protected override void StopCommunicationListeners()
        {
        }
    }
}
