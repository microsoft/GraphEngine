using System;
using System.Collections.Concurrent;
using Trinity.Extension;

namespace Trinity.Client.TrinityClientModule
{
    [AutoRegisteredCommunicationModule]
    class TrinityClientModule : TrinityClientModuleBase
    {
        public override string GetModuleName() => "TrinityClient";

        // ===== Server-side members =====
        private IClientRegistry Registry => m_memorycloud as IClientRegistry;
        private ConcurrentDictionary<int, int> m_cookie_id = new ConcurrentDictionary<int, int>();
        // ===== Client-side members =====
        private Lazy<int> m_client_cookie = new Lazy<int>(() => new Random().Next(int.MinValue, int.MaxValue));
        internal int MyCookie => m_client_cookie.Value;

        public override void PollAsyncEventsHandler(PollEventsRequestReader request, PollEventsResponseWriter response)
        {
            CheckInstanceCookie(request.Cookie, request.InstanceId);
            //response.Result = new 
        }

        public override void PollSyncEventsHandler(PollEventsRequestReader request, PollEventsResponseWriter response)
        {
            CheckInstanceCookie(request.Cookie, request.InstanceId);
            //TODO
        }

        public override void PostSyncResponseHandler(PostSyncResponseRequestReader request)
        {
            CheckInstanceCookie(request.Cookie, request.InstanceId);
            //TODO
        }

        public override void RegisterClientHandler(RegisterClientRequestReader request, RegisterClientResponseWriter response)
        {
            response.PartitionCount = m_memorycloud.PartitionCount;
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
            if (m_cookie_id.TryGetValue(cookie, out var existing_id) && instanceId == existing_id) return;
            throw new ClientInstanceNotFoundException();
        }

    }
}
