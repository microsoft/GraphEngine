using System;
using System.Collections.Generic;
using System.Text;

namespace Trinity.Storage.Transaction
{
    public class LocalTransactionContext : IDisposable
    {
        private unsafe void* m_pctx;

        public unsafe LocalTransactionContext()
        {
            m_pctx = CLocalMemoryStorage.CThreadContextAllocate();
        }

        public unsafe void Dispose()
        {
            if (m_pctx == null) throw new InvalidOperationException("Transaction context already disposed");
            CLocalMemoryStorage.CThreadContextDeallocate(m_pctx);
            m_pctx = null;
        }
    }
}
