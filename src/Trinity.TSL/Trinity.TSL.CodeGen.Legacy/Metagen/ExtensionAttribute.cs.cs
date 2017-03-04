using System.Text;
using System.Collections.Generic;

namespace Trinity.TSL.CodeTemplates
{
    internal partial class SourceFiles
    {
        internal static string 
ExtensionAttribute(
NTSL node)
        {
            StringBuilder source = new StringBuilder();
            
source.Append(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.TSL;
namespace ");
source.Append(Codegen.GetString(Trinity::Codegen::GetNamespace()));
source.Append(@"
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
");

            return source.ToString();
        }
    }
}
