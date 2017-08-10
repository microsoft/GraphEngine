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
    /// <summary>
    /// Provides utility functions to manipulate assemblies, types and instances.
    /// </summary>
    public static class AssemblyUtility
    {
        private static string my_assembly_path = null;
        private static Assembly trinity_asm = Assembly.GetExecutingAssembly();

        /// <summary>
        /// The directory containing the GraphEngine core assembly.
        /// </summary>
        public static string TrinityCorePath
        {
            get
            {
                return Path.GetDirectoryName(trinity_asm.Location) + Path.DirectorySeparatorChar;

            }
        }

        /// <summary>
        /// Returns the directory path containing the user assembly calling into MyAssemblyPath.
        /// </summary>
        public static string MyAssemblyPath
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

        /// <summary>
        /// Tells if any assembly satisfies the predicate.
        /// </summary>
        /// <remarks>
        /// Exceptions are swallowed.
        /// </remarks>
        public static bool AnyAssembly(Func<Assembly, bool> pred)
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
        /// <remarks>
        /// Exceptions are swallowed.
        /// </remarks>
        public static List<Type> GetAllClassTypes(Assembly assembly = null)
        {
            return GetAllClassTypes_impl(assembly, t => true);
        }

        /// <summary>
        /// Get non-abstract class types.
        /// </summary>
        /// <remarks>
        /// Exceptions are swallowed.
        /// </remarks>
        public static List<Type> GetAllClassTypes(Func<Type, bool> typePredicate, Assembly assembly = null)
        {
            return GetAllClassTypes_impl(assembly, typePredicate);
        }

        /// <summary>
        /// Get non-abstract class types
        /// </summary>
        /// <remarks>
        /// Exceptions are swallowed.
        /// </remarks>
        public static List<Type> GetAllClassTypes<TBase>(Func<Type, bool> typePredicate, Assembly assembly = null)
            where TBase : class
        {
            return GetAllClassTypes_impl(assembly, t => typePredicate(t) && typeof(TBase).IsAssignableFrom(t));
        }

        private static List<Type> GetAllClassTypes_impl(Assembly assembly, Func<Type, bool> typePredicate)
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

                    if (typePredicate(type))
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
        /// Project from types to instances.
        /// Projections to null are ignored.
        /// </summary>
        /// <param name="assembly">The assembly constraint. Passing null to scan all loaded assemblies.</param>
        /// <param name="typeProjector">The projector.</param>
        /// <typeparam name="TBase">The base class constraint.</typeparam>
        /// <remarks>
        /// Exceptions are swallowed.
        /// </remarks>
        public static List<TBase> GetAllClassInstances<TBase>(Func<Type, TBase> typeProjector, Assembly assembly = null)
            where TBase : class
        {
            return GetAllClassInstances_impl(typeProjector, GetAllClassTypes(assembly));
        }

        /// <summary>
        /// Project from types to instances.
        /// Projections to null are ignored.
        /// </summary>
        /// <param name="assembly">The assembly constraint. Passing null to scan all loaded assemblies.</param>
        /// <param name="type_pred">The predicate that filters wanted types.</param>
        /// <param name="attr_projector">The attribute-to-type projector.</param>
        /// <typeparam name="TBase">The base class constraint.</typeparam>
        /// <typeparam name="TAttribute">
        /// The attribute constraint. Can be an inherited attribute.
        /// If there are multiple attributes of this type attached to a class,
        /// the first one projecting to a non-null instance will be selected.
        /// </typeparam>
        /// <remarks>
        /// Exceptions are swallowed.
        /// </remarks>
        public static List<TBase> GetAllClassInstances<TBase, TAttribute>(Func<Type, bool> type_pred, Func<TAttribute, TBase> attr_projector, Assembly assembly = null)
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
            }, GetAllClassTypes(assembly));
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

        /// <summary>
        /// Iterate over all assemblies and perform projection.
        /// </summary>
        /// <typeparam name="T">The projection target type.</typeparam>
        /// <param name="func">The projector.</param>
        public static List<T> ForAllAssemblies<T>(Func<Assembly, T> func)
        {
            List<T> ret = new List<T>();
            ForAllAssemblies(asm => { ret.Add(func(asm)); });
            return ret;
        }

        /// <summary>
        /// Iterate over all assemblies and perform an action for each of them.
        /// </summary>
        /// <param name="action">The action.</param>
        public static void ForAllAssemblies(Action<Assembly> action)
        {
            var all_loaded_assemblies = AppDomain.CurrentDomain.GetAssemblies();
            try
            {
                foreach (var assembly in all_loaded_assemblies)
                {
                    try
                    {
                        action(assembly);
                    }
                    catch { }
                }
            }
            catch { }
        }
    }
}
