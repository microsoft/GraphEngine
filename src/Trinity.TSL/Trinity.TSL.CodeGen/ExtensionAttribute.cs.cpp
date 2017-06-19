#include "common.h"
#include <string>
#include "SyntaxNode.h"

using std::string;

namespace Trinity
{
    namespace Codegen
    {
        string* 
ExtensionAttribute(
NTSL* node)
        {
            string* source = new string();
            
source->append(R"::(using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.TSL;
namespace )::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(
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
)::");

            return source;
        }
    }
}
