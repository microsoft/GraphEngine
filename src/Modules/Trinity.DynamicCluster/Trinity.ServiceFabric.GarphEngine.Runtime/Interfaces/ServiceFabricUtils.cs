using Microsoft.ServiceFabric.Data;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Diagnostics;
using Trinity.DynamicCluster.Consensus;

namespace Trinity.ServiceFabric.GarphEngine.Infrastructure.Interfaces
{
    internal static class ServiceFabricUtils
    {
        internal static async Task<T> CreateReliableStateAsync<T>(IService svc, string name, int p) where T : IReliableState
        {
            name = $"{name}-P{p}";
            await GraphEngineStatefulServiceRuntime.Instance.GetRoleAsync();
            var statemgr = GraphEngineStatefulServiceRuntime.Instance.StateManager;
            T ret = default(T);
            await DoWithTimeoutRetryImpl(async () =>
            {
                if (svc.IsMaster)
                {
                    ret = await statemgr.GetOrAddAsync<T>(name);
                }
                else
                {
                    var result = await statemgr.TryGetAsync<T>(name);
                    if (result.HasValue) { ret = result.Value; }
                    else { throw new TimeoutException(); }
                }
            });
            return ret;
        }

        internal static ITransaction CreateTransaction()
        {
            var statemgr = GraphEngineStatefulServiceRuntime.Instance.StateManager;
            return statemgr.CreateTransaction();
        }

        private static async Task DoWithTimeoutRetryImpl(Func<Task> task)
        {
retry:
            try { await task(); }
            catch (TimeoutException)
            { await Task.Delay(1000); goto retry; }
            catch (FabricNotReadableException)
            { Log.WriteLine("Fabric not readable from Primary/ActiveSecondary, retrying."); await Task.Delay(1000); goto retry; }
            catch (OperationCanceledException ex)
            { Log.WriteLine(LogLevel.Warning, "{0}", $"{nameof(ServiceFabricUtils)}: Operation cancelled, retry aborted: {ex.ToString()}."); throw; }
            catch (Exception ex)
            {
                Log.WriteLine(LogLevel.Warning, "{0}", $"ServiceFabricUtils: {ex.ToString()}.");
                throw;
            }
        }

        /// <summary>
        /// Retry stepper
        /// </summary>
        internal static async Task DoWithTimeoutRetry(params Func<Task>[] tasks)
        {
            foreach (var task in tasks)
            {
                await DoWithTimeoutRetryImpl(task);
            }
        }
    }
}
