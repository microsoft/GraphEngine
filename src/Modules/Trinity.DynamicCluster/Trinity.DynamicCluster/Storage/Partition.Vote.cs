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
    public partial class Partition
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
            return CollectConsensusResults(tasks, threshold).Result;
        }

        public async Task<TResponse> Vote<TResponse>(Func<IMessagePassingEndpoint, Task<TResponse>> sendFunc, int threshold)
            where TResponse : IAccessor, IDisposable
        {
            var replicas = this.ToList();
            var tasks = replicas.Select(replica => sendFunc(replica));
            return await CollectConsensusResults(tasks, threshold);
        }
        private void DisposeOthers<TResponse>(IEnumerable<Task<TResponse>> tasks, TResponse rsp) 
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

        private unsafe int H<TResponse>(TResponse _)
           where TResponse: IAccessor => HashHelper.HashBytes(_.GetUnderlyingBufferPointer(), _.GetBufferLength());

        private (int hash, int cnt) _Majority<TResponse>(IEnumerable<TResponse> responses)
            where TResponse : IAccessor =>
            responses
           .GroupBy(H)
           .Select(_ => (_.Key, _.Count()))
           .OrderByDescending(_ => _.Item2)
           .FirstOrDefault();

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

        private async Task<TResponse> CollectConsensusResults<TResponse>(IEnumerable<Task<TResponse>> tasks, int threshold)
            where TResponse : IAccessor, IDisposable
        {
            var task_array = Utils.SortByCompletion(tasks);
            List<TResponse> completed = new List<TResponse>();
            foreach (var t in task_array)
            {
                try { completed.Add(await t); }
                catch { }
                if (completed.Count < threshold) continue;
                var (hash, cnt) = _Majority(completed);
                if (cnt >= threshold)
                {
                    var rsp = completed.First(_ => H(_) == hash);
                    DisposeOthers(tasks, rsp);
                    return rsp;
                }
            }
            throw new VoteException(task_array, threshold);
        }
    }
}
