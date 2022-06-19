using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Trinity.Storage;

namespace Trinity.DynamicCluster.Storage
{
    [Serializable]
    public class BroadcastException : Exception
    {
        protected List<(IStorage, Exception)> m_exceptions;

        public IEnumerable<(IStorage, Exception)> Exceptions => m_exceptions;

        internal BroadcastException(IEnumerable<(IStorage, Exception)> enumerable)
        {
            m_exceptions = enumerable.Where(tup => tup.Item2 != null).ToList();
        }

    }

    [Serializable]
    public class BroadcastException<TResponse> : BroadcastException, IDisposable
        where TResponse : IDisposable
    {
        protected List<(IStorage, TResponse)> m_results;

        public IEnumerable<(IStorage, TResponse)> Results => m_results;

        public BroadcastException(IEnumerable<(IStorage, Exception)> exs, IEnumerable<(IStorage, TResponse)> results) : base(exs)
        {
            m_results = results.Where(tup => tup.Item2 != null).ToList();
        }

        public void Dispose()
        {
            if (m_results != null)
            {
                m_results.ForEach(_ => _.Item2.Dispose());
                m_results = null;
            }
        }
    }
}
