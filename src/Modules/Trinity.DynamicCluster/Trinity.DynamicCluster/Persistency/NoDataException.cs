using System;

namespace Trinity.DynamicCluster.Persistency
{
    public class NoDataException : Exception
    {
        public NoDataException(): base() { }
        public NoDataException(string msg): base(msg) { }
    }
}
