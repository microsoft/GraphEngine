using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.Extension
{

    /// <summary>
    /// Provides assembly-level extension declaration.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    public class GraphEngineExtensionAttribute : Attribute
    {
        /// <summary>
        /// ctor
        /// </summary>
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public GraphEngineExtensionAttribute()
        {
            __AssemblyForceLoad.Trigger(5);
        }
    }

    internal static class __AssemblyForceLoad
    {
        [MethodImpl(MethodImplOptions.NoOptimization)]
        internal static void Trigger(int depth)
        {
            if (depth > 0)
                Trigger(depth - 1);
        }
    }
}
