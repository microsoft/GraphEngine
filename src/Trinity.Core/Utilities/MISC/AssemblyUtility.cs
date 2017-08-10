using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Trinity.Diagnostics;

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
                            var type = method.DeclaringType;
                            var asm = method.Module.Assembly;

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
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    if (pred(asm)) return true;
                }
                catch { }
            }

            return false;
        }

        /// <summary>
        /// Get non-abstract class types
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        internal static List<Type> GetAllClassTypes(Assembly assembly = null)
        {
            return GetAllClassTypes_impl(assembly, t => true);
        }

        internal static List<Type> GetAllClassTypes(Func<Type, bool> typePred, Assembly assembly = null)
        {
            return GetAllClassTypes_impl(assembly, typePred);
        }

        internal static List<Type> GetAllClassTypes<TBase>(Func<Type, bool> typePred, Assembly assembly = null)
            where TBase : class
        {
            return GetAllClassTypes_impl(assembly, t => typePred(t) && typeof(TBase).IsAssignableFrom(t));
        }

        private static List<Type> GetAllClassTypes_impl(Assembly assembly, Func<Type, bool> typePred)
        {
            List<Type> satisfied_types = new List<Type>();
            List<Type> all_types;

            if (assembly == null) all_types = ForAllAssemblies(asm => GetAllClassTypes(asm)).SelectMany(_ => _).ToList();
            else all_types = GetAllClassTypes(assembly);

            foreach (var type in all_types)
            {
                try
                {
                    if (type.IsAbstract) continue;

                    if (typePred(type))
                    {
                        satisfied_types.Add(type);
                    }
                }
                catch (Exception ex)
                {
                    Log.WriteLine(LogLevel.Verbose, "{0}", ex.ToString());
                }
            }

            return satisfied_types;
        }


        /// <summary>
        /// projections to null are ignored.
        /// </summary>
        internal static List<TBase> GetAllTypeInstances<TBase>(Func<Type, TBase> type_projector)
            where TBase : class
        {
            return GetAllClassInstances_impl(type_projector, GetAllClassTypes());
        }

        /// <summary>
        /// projections to null are ignored
        /// </summary>
        internal static List<TBase> GetAllTypeInstances<TBase>(Assembly assembly, Func<Type, TBase> type_projector)
            where TBase : class
        {
            return GetAllClassInstances_impl(type_projector, GetAllClassTypes(assembly));
        }

        internal static List<TBase> GetAllClassInstances<TBase, TAttribute>(Func<Type, bool> type_pred, Func<TAttribute, TBase> attr_projector)
            where TAttribute : Attribute
            where TBase : class
        {
            return GetAllClassInstances_impl(type =>
            {
                try
                {
                    if (!type_pred(type)) return null;
                    foreach (var attr in type.GetCustomAttributes<TAttribute>(inherit: true))
                    {
                        try
                        {
                            var instance = attr_projector(attr);

                            if (instance != null)
                            {
                                return instance;
                            }
                        }
                        catch { }
                    }
                }
                catch (Exception ex)
                {
                    Log.WriteLine(LogLevel.Verbose, "{0}", ex.ToString());
                }
                return null;
            }, GetAllClassTypes());
        }

        private static List<TBase> GetAllClassInstances_impl<TBase>(Func<Type, TBase> type_projector, List<Type> types)
            where TBase : class
        {
            List<TBase> satisfied_instances = new List<TBase>();
            foreach (var type in types)
            {
                try
                {
                    var instance = type_projector(type);

                    if (instance != null)
                    {
                        satisfied_instances.Add(instance);
                        break;
                    }
                }
                catch { }
            }

            return satisfied_instances;
        }

        internal static List<T> ForAllAssemblies<T>(Func<Assembly, T> func)
        {
            List<T> ret = new List<T>();
            ForAllAssemblies(asm => { ret.Add(func(asm)); });
            return ret;
        }

        internal static void ForAllAssemblies(Action<Assembly> func)
        {
            var all_loaded_assemblies = AppDomain.CurrentDomain.GetAssemblies();
            try
            {
                foreach (var assembly in all_loaded_assemblies)
                {
                    try
                    {
                        func(assembly);
                    }
                    catch { }
                }
            }
            catch { }
        }
    }
}
