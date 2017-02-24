using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.Utilities
{
    internal static class AssemblyPath
    {
        private static string my_assembly_path = "";
        internal static string MyAssemblyPath
        {
            get
            {
                if (my_assembly_path.Length == 0)
                {
                    my_assembly_path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar;
                }
                return my_assembly_path;
            }
        }
    }
}
