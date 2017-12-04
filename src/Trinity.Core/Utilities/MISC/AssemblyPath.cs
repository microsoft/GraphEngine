using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.Utilities
{
    internal static class AssemblyUtility
    {
        private static string my_assembly_path = null;
        private static Assembly trinity_asm = Assembly.GetExecutingAssembly();
        internal static string TrinityCorePath
        {
            get
            {
                return Path.GetDirectoryName(trinity_asm.Location) + Path.DirectorySeparatorChar;

            }
        }
        internal static string MyAssemblyPath
        {
            get
            {
                if (my_assembly_path == null)
                {
                    //  primary heuristics: find the assembly that calls into Trinity
                    int firstTrinityFrame = 0;
                    for (int skipFrames = 1; ; skipFrames++)
                    {
                        StackFrame stackFrame;
                        try
                        {
                            stackFrame = new StackFrame(skipFrames);
                            if (stackFrame.GetMethod() == null) break;
                        }
                        catch { break; }

                        try
                        {
                            var asm = stackFrame.GetMethod().Module.Assembly;
                            if (asm == trinity_asm) firstTrinityFrame = skipFrames;
                        }
                        catch { continue; }
                    }

                    for (int skipFrames = firstTrinityFrame + 1; ; skipFrames++)
                    {
                        StackFrame stackFrame;
                        try
                        {
                            stackFrame = new StackFrame(skipFrames);
                            if (stackFrame.GetMethod() == null) break;
                        }
                        catch { break; }

                        try
                        {
                            var method = stackFrame.GetMethod();
                            var type   = method.DeclaringType;
                            var asm    = method.Module.Assembly;

                            if (asm == trinity_asm) continue;
                            if (asm.IsDynamic) continue;
                            if (type == typeof(System.RuntimeMethodHandle)) continue;
                            if (asm == typeof(object).Assembly) continue;
                            if (asm == typeof(System.Linq.Enumerable).Assembly) continue;

                            var path = asm.Location;
                            my_assembly_path = System.IO.Path.GetDirectoryName(path) + Path.DirectorySeparatorChar;
                            break;
                        }
                        catch { continue; }
                    }
                }

                if (my_assembly_path == null)
                {
                    //  last resort heuristic: return the path of the Trinity assembly.
                    my_assembly_path = System.IO.Path.GetDirectoryName(trinity_asm.Location) + Path.DirectorySeparatorChar;
                }

                return my_assembly_path;
            }
        }

        internal static bool AnyAssembly(Func<Assembly, bool> pred)
        {
            foreach(var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    if (pred(asm)) return true;
                }
                catch { }
            }

            return false;
        }

        internal static List<Type> GetAllTypes(Assembly asm)
        {

            List<Type> types = new List<Type>();
            try
            {
                foreach (var type in asm.GetTypes())
                {
                    types.Add(type);
                }
            }
            catch { }
            return types;
        }

        internal static List<Type> GetAllTypes()
        {
            List<Type> types = new List<Type>();
            try
            {
                foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        types.AddRange(GetAllTypes(asm));
                    }
                    catch { }
                }
            }
            catch { }

            return types;
        }
    }
}
