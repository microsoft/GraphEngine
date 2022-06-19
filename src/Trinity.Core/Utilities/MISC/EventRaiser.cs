using System;
using System.Collections.Generic;
using System.Text;
using Trinity.Diagnostics;

namespace Trinity.Utilities
{
    internal static class EventRaiser
    {
        internal static TrinityErrorCode RaiseStorageEvent(Delegate handler, string handlerName)
        {
            var ret = TrinityErrorCode.E_SUCCESS;
            var listeners = handler.GetInvocationList();
            foreach (var listener in listeners)
            {
                try { listener.DynamicInvoke(); }
                catch (Exception ex)
                {
                    Log.WriteLine(LogLevel.Error, $"An error occurred in {handlerName}: {{0}}", ex.ToString());
                    ret = TrinityErrorCode.E_FAILURE;
                }
            }
            return ret;
        }
    }
}
