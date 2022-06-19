using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.DynamicCluster.Persistency
{
    [Serializable]
    public class SnapshotNotFoundException : Exception
    {
        public SnapshotNotFoundException() : base() { }
        public SnapshotNotFoundException(string message) : base(message) { }
    }
}
