using Microsoft.ServiceFabric.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Trinity.ServiceFabric.Infrastructure
{
    public class RestoreEventArgs : EventArgs
    {
        private TaskCompletionSource<bool> m_src = new TaskCompletionSource<bool>();
        internal RestoreContext m_rctx;

        public Task Wait()
        {
            return m_src.Task;
        }

        internal void Complete(Exception exception = null)
        {
            if (exception == null)
            {
                m_src.SetResult(true);
            }
            else
            {
                m_src.SetException(exception);
            }
        }

        public RestoreEventArgs(RestoreContext restoreContext, CancellationToken cancellationToken)
        {
            m_rctx = restoreContext;
            cancellationToken.Register(() => m_src.SetCanceled());
        }
    }
}
