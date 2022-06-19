using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.TSL;

/*MAP_VAR("t_Namespace", "Trinity::Codegen::GetNamespace()")*/
namespace t_Namespace
{
    [TARGET("NTSL")]
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
