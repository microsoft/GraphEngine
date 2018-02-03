using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Trinity.DynamicCluster.Storage
{
    [Serializable]
    internal class VoteException : Exception
    {
        private Task[] task_array;
        private int threshold;

        public VoteException(Task[] task_array, int threshold)
        {
            this.task_array=task_array;
            this.threshold=threshold;
        }
    }
}