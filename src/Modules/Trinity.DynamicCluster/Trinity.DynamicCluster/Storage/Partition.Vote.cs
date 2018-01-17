using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Core.Lib;
using Trinity.Network.Messaging;
using Trinity.Storage;

namespace Trinity.DynamicCluster.Storage
{
    internal unsafe partial class Partition
    {
        public void Vote(Action<IMessagePassingEndpoint> sendFunc, int threshold)
        {
            var replicas = this.ToList();
            var tasks = replicas.Select(replica => Task.Run(() => sendFunc(replica)));
            CollectConsensusResults(tasks, threshold);
        }

        public TResponse Vote<TResponse>(Func<IMessagePassingEndpoint, TResponse> sendFunc, int threshold)
            where TResponse : IAccessor, IDisposable
        {
            var replicas = this.ToList();
            var tasks = replicas.Select(replica => Task.Run(() => sendFunc(replica)));
            return CollectConsensusResults(tasks, threshold);
        }

        public TResponse Vote<TResponse>(Func<IMessagePassingEndpoint, Task<TResponse>> sendFunc, int threshold)
            where TResponse : IAccessor, IDisposable
        {
            var replicas = this.ToList();
            var tasks = replicas.Select(replica => sendFunc(replica));
            return CollectConsensusResults(tasks, threshold);
        }
        private void FreeTasksExcept<TResponse>(IEnumerable<Task<TResponse>> tasks, TResponse rsp) 
            where TResponse : IDisposable
        {
            foreach (var task in tasks)
            {
                task.ContinueWith(t =>
                {
                    if (t.IsCompleted && !t.Result.Equals(rsp))
                    {
                        t.Result.Dispose();
                    }
                });
            }
        }

        private int H(IAccessor _) => HashHelper.HashBytes(_.GetUnderlyingBufferPointer(), _.GetBufferLength());

        private (int hash, int cnt) _Majority<TResponse>(IEnumerable<TResponse> responses)
            where TResponse : IAccessor
        {
            Dictionary<int, int> dict = new Dictionary<int, int>();
            foreach (var t in responses)
            {
                int h = H(t);
                if (!dict.ContainsKey(h)) dict.Add(h, 0);
                ++dict[h];
            }
            var pair = dict.OrderByDescending(_ => _.Value).FirstOrDefault();
            return (pair.Key, pair.Value);
        }

        private void CollectConsensusResults(IEnumerable<Task> tasks, int threshold)
        {
            var task_array = Utils.SortByCompletion(tasks);
            int wait_idx = threshold - 1;
            foreach (var t in task_array.Skip(threshold - 1))
            {
                t.Wait();
                if (task_array.Take(wait_idx + 1).Count(_ => _.IsCompleted) >= threshold) return;
                ++wait_idx;
            }
            throw new VoteException(task_array, threshold);
        }

        private TResponse CollectConsensusResults<TResponse>(IEnumerable<Task<TResponse>> tasks, int threshold)
            where TResponse : IAccessor, IDisposable
        {
            var task_array = Utils.SortByCompletion(tasks);
            int wait_idx = threshold - 1;
            foreach (var t in task_array.Skip(threshold - 1))
            {
                t.Wait();
                var completed = task_array.Take(wait_idx + 1).Where(_ => _.IsCompleted).Select(_ => _.Result);
                var (hash, cnt) = _Majority(completed);
                if (cnt >= threshold)
                {
                    var rsp = completed.First(_ => H(_) == hash);
                    FreeTasksExcept(tasks, rsp);
                    return rsp;
                }
                ++wait_idx;
            }
            throw new VoteException(task_array, threshold);
        }
    }
}
