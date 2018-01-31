using System;
using System.Collections.Concurrent;
using Trinity.Extension;

namespace Trinity.Client.TrinityClientModule
{
    [AutoRegisteredCommunicationModule]
    class TrinityClientModule : TrinityClientModuleBase
    {
        public override string GetModuleName() => "TrinityClient";

        private IClientRegistry Registry => m_memorycloud as IClientRegistry;
        private ConcurrentDictionary<int, int> m_cookie_id = new ConcurrentDictionary<int, int>();

        public override void PollAsyncEventsHandler(PollEventsRequestReader request, PollEventsResponseWriter response)
        {
            CheckInstanceCookie(request.Cookie, request.InstanceId);
            //response.Result = new 
        }

        public override void PollSyncEventsHandler(PollEventsRequestReader request, PollEventsResponseWriter response)
        {
            CheckInstanceCookie(request.Cookie, request.InstanceId);
        }

        public override void RegisterClientHandler(RegisterClientRequestReader request, RegisterClientResponseWriter response)
        {
            if (m_cookie_id.TryGetValue(request.Cookie, out var existing_id))
            {
                response.InstanceId = existing_id;
            }
            else
            {
                ClientIStorage cstg = new ClientIStorage(m_memorycloud);
                int new_id = Registry.RegisterClient(cstg);
                response.InstanceId = new_id;
            }
        }

        public override void UnregisterClientHandler(UnregisterClientRequestReader request)
        {
            CheckInstanceCookie(request.Cookie, request.InstanceId);
            Registry.UnregisterClient(request.InstanceId);
        }

        private void CheckInstanceCookie(int cookie, int instanceId)
        {
            if (m_cookie_id.TryGetValue(cookie, out var existing_id))
            {
                if (instanceId != existing_id)
                    throw new ClientCookieMismatchException();
            }
            else
            {
                throw new ClientInstanceNotFoundException();
            }
        }
    }
}
