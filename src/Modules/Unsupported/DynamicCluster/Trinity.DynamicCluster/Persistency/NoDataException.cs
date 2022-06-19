using System;

namespace Trinity.DynamicCluster.Persistency
{
    [Serializable]
    public class NoDataException : Exception
    {
        public NoDataException(): base() { }
        public NoDataException(string msg): base(msg) { }
    }
}
