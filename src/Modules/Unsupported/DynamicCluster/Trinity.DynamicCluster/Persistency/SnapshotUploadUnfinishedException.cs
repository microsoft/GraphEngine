using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.DynamicCluster.Persistency
{
    [Serializable]
    public class SnapshotUploadUnfinishedException : Exception
    {
        public SnapshotUploadUnfinishedException() : base() { }
        public SnapshotUploadUnfinishedException(string message) : base(message) { }
    }
}
