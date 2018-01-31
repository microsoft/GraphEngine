#pragma warning disable 162,168,649,660,661,1522
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.TSL;
namespace CellAssembly
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    public class ImportGraphEngineExtensionAttribute : Attribute
    {
        public ImportGraphEngineExtensionAttribute()
        {
            __AssemblyForceLoad.Trigger(5);
        }
    }
    internal static class __AssemblyForceLoad
    {
        internal static void Trigger(int depth)
        {
            if (depth > 0)
                Trigger(depth - 1);
        }
    }
}

#pragma warning restore 162,168,649,660,661,1522
