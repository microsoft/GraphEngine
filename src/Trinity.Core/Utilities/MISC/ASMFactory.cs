// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using Trinity;
using Trinity.Diagnostics;

namespace Trinity.Utilities
{
    internal class ASMFactory
    {
        static ASMFactory()
        {
            Log.WriteLine(LogLevel.Verbose, "ASMFactory", "ASM factory initializing.");
            AppDomain.CurrentDomain.AssemblyLoad += OnAssemblyLoad;
            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
            //Initialize the Assembly Cache, fill in existing assemblies
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                AssemblyCache[asm.FullName] = asm;
            }
            //Initialize the Core Assembly Dependency List
            Assembly[] coreAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var asm in coreAssemblies)
            {
                try//When attached to debugger, there'storage some exceptions
                {
                    _coreAssemblyDependencyList.AddRange(GetAssemblyDependencyList(asm));
                }
                catch (Exception)
                {
                    //Console.WriteLine(e.Message);
                }
            }
            _coreAssemblyDependencyList = _coreAssemblyDependencyList.Distinct().ToList();
        }
        /// <summary>
        /// Recursively retrive all dependencies ( including itself ) of an assembly.
        /// </summary>
        /// <param name="asm">The given assembly</param>
        /// <param name="searchPath">The path to search.</param>
        /// <returns>A list of assembly full names.</returns>
        internal static List<string> GetAssemblyDependencyList(Assembly asm, string searchPath = "")
        {
            AppDomain newDomain = AppDomain.CreateDomain("Sandbox");
            newDomain.Load(Assembly.GetExecutingAssembly().GetName());
            _asm_factory_ factory = newDomain.CreateInstanceAndUnwrap(
                            Assembly.GetExecutingAssembly().GetName().FullName,
                            "Trinity.Utilities._asm_factory_") as _asm_factory_;
            var ret = factory.__get_assembly_dependency_list__(asm, searchPath);
            AppDomain.Unload(newDomain);
            return (List<string>)ret;
        }
        public static List<string> GetAssemblyDependencyList(byte[] asmBytes, string searchPath = "")
        {
            AppDomain newDomain = AppDomain.CreateDomain("Sandbox");
            newDomain.Load(Assembly.GetExecutingAssembly().GetName());
            _asm_factory_ factory = newDomain.CreateInstanceAndUnwrap(
                            Assembly.GetExecutingAssembly().GetName().FullName,
                            "Trinity.Utilities._asm_factory_") as _asm_factory_;
            var ret = factory.__get_assembly_dependency_list__(asmBytes, searchPath);
            AppDomain.Unload(newDomain);
            return (List<string>)ret;
        }
        public static List<string> GetAssemblyDependencyList(string filename, string searchPath = "")
        {
            AppDomain newDomain = AppDomain.CreateDomain("Sandbox");
            newDomain.Load(Assembly.GetExecutingAssembly().GetName());
            _asm_factory_ factory = newDomain.CreateInstanceAndUnwrap(
                            Assembly.GetExecutingAssembly().GetName().FullName,
                            "Trinity.Utilities._asm_factory_") as _asm_factory_;
            var ret = factory.__get_assembly_dependency_list_with_filename__(filename, searchPath);
            AppDomain.Unload(newDomain);
            return (List<string>)ret;
        }
        static void OnAssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            //register the loaded assembly into the assembly cache
            AssemblyCache[args.LoadedAssembly.FullName] = args.LoadedAssembly;
            Log.WriteLine(LogLevel.Verbose, "ASMFactory", "New assembly loaded: {0}", args.LoadedAssembly.FullName);
        }
        static Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            Assembly asm = null;
            AssemblyCache.TryGetValue(args.Name, out asm);
            Log.WriteLine(LogLevel.Verbose, "ASMFactory", "Resolving assembly: {0}", args.Name);
            return asm;
        }
        static Dictionary<string, Assembly> AssemblyCache = new Dictionary<string, Assembly>();
        static List<string> _coreAssemblyDependencyList = new List<string>();
        /// <summary>
        /// The dependency list generated on application startup, without any plug-ins.
        /// </summary>
        public static List<string> CoreAssemblyDependencyList
        {
            get { return _coreAssemblyDependencyList.ToList(); }
        }

        public static object CreateInstance(Assembly asm, Type t)
        {
            return asm.CreateInstance(t.FullName, true, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, null, null, null);
        }
        public static T CreateInstance<T>(Assembly asm, List<string> RequiredMethodList)
        {
            foreach (Type t in asm.GetTypes())
            {
                //Abstract class, Interface, Generic types cannot be instantiated
                //in our situation.
                if (t.IsAbstract || t.IsInterface || t.IsGenericType)
                    continue;
                //Sometimes an instance cannot be created from certain classes.
                //Wrap it with try-catch for stability.
                try
                {
                    bool hasAllRequiredMethods = true;
                    foreach (string requiredMethod in RequiredMethodList)
                    {
                        bool hasMethod = false;
                        foreach (MethodInfo m in t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance))
                        {
                            if (m.Name == requiredMethod)
                            {
                                hasMethod = true;
                                break;
                            }
                        }
                        if (!hasMethod)
                        {
                            hasAllRequiredMethods = false;
                            break;
                        }
                    }
                    if (!hasAllRequiredMethods)
                        continue;
                    object o = CreateInstance(asm, t);
                    if (o != null && o is T)
                        return (T)o;
                }
                catch (Exception)
                {
                }
            }
            return default(T);
        }
        public static T CreateInstance<T>(Assembly asm)
        {
            Type typeOfT = typeof(T);
            List<string> requiredMethodList = new List<string>();
            foreach (MethodInfo m in typeOfT.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.CreateInstance))
            {
                requiredMethodList.Add(m.Name);
            }
            return CreateInstance<T>(asm, requiredMethodList);
        }

        public static Assembly LoadFrom(string filename, Action<Assembly> lockedAction = null)
        {
            if (!File.Exists(filename))
            {
                return null;
            }
            byte[] asmBytes = File.ReadAllBytes(filename);
            return Load(asmBytes, lockedAction);
        }
        /// <summary>
        /// This method load an assembly into the nameless context.
        /// Note that unlike Assembly.Load, this method will prevent
        /// auto-resolving on disk. When a dependency is not already
        /// loaded into AssemblyCache, it will not load the new assembly.
        /// </summary>
        /// <param name="asmBytes">Target assembly binary</param>
        /// <param name="lockedAction">Action to execute while auto-resolving is disabled</param>
        /// <returns>An Assembly if loaded successful, otherwise null</returns>
        internal static Assembly Load(byte[] asmBytes, Action<Assembly> lockedAction = null)
        {
            return Load(asmBytes, lockedAction, false);
        }
        /// <summary>
        /// This method load an assembly into the nameless context.
        /// Note that unlike Assembly.Load, this method will prevent
        /// auto-resolving on disk. When a dependency is not already
        /// loaded into AssemblyCache, it will not load the new assembly.
        /// </summary>
        /// <param name="asmBytes">Target assembly binary</param>
        /// <param name="lockedAction">Action to execute while auto-resolving is disabled</param>
        /// <param name="ResolveToLoadedAssembly">If the assembly is already loaded, don't load again</param>
        /// <returns>An Assembly if loaded successful, otherwise null</returns>
        public static Assembly Load(byte[] asmBytes, Action<Assembly> lockedAction, bool ResolveToLoadedAssembly)
        {
            if (lockedAction != null)
            {
                AppDomain newDomain = AppDomain.CreateDomain("Sandbox");
                newDomain.Load(Assembly.GetExecutingAssembly().GetName());
                _asm_factory_ factory = newDomain.CreateInstanceAndUnwrap(
                    Assembly.GetExecutingAssembly().GetName().FullName,
                    "Trinity.Utilities._asm_factory_") as _asm_factory_;

                List<string> depList = factory.__get_assembly_dependency_list__(asmBytes);
                //Unload the domain, release the assemblies
                AppDomain.Unload(newDomain);
                List<FileStream> locked_files = LockDependencyFiles(depList);

                Assembly ret = null;
                if (!ResolveToLoadedAssembly)
                    ret = Assembly.Load(asmBytes);
                else
                {
                    newDomain = AppDomain.CreateDomain("Sandbox");
                    newDomain.Load(Assembly.GetExecutingAssembly().GetName());
                    factory = newDomain.CreateInstanceAndUnwrap(
                        Assembly.GetExecutingAssembly().GetName().FullName,
                        "Trinity.Utilities._asm_factory_") as _asm_factory_;
                    string fullName = factory.__get_assembly_fullname__(asmBytes);
                    AppDomain.Unload(newDomain);

                    if (AssemblyCache.ContainsKey(fullName))
                    {
                        ret = AssemblyCache[fullName];
                    }
                    else
                        ret = Assembly.Load(asmBytes);

                }

                lockedAction(ret);

                UnlockDependencyFiles(locked_files);

                return ret;
            }
            else
                return Assembly.Load(asmBytes);
        }
        /// <summary>
        /// Scan through all dll/exe files in current working directory,
        /// load them up and check whether they match the given full asm name.
        /// If one matches the given full name, return an reflection-only assembly of that file.
        /// These interfaces might be slow since it creates a sandbox for the assembly loading.
        /// </summary>
        /// <param name="fullname">The full name of the desired assembly</param>
        /// <param name="searchPath">The path to search</param>
        /// <param name="fullPath">The full path of the located assembly.</param>
        /// <returns>Null if file not found. An assembly if found.</returns>
        internal static Assembly LoadReflectionOnly(string fullname, string searchPath, out string fullPath)
        {
            AppDomain newDomain = AppDomain.CreateDomain("Sandbox");
            newDomain.Load(Assembly.GetExecutingAssembly().GetName());
            fullPath = "";
            _asm_factory_ factory = newDomain.CreateInstanceAndUnwrap(
                Assembly.GetExecutingAssembly().GetName().FullName,
                "Trinity.Utilities._asm_factory_") as _asm_factory_;
            var ret = factory.__load_reflection_only__(fullname, searchPath, out fullPath);
            AppDomain.Unload(newDomain);
            return (Assembly)ret;
        }
        public static Assembly LoadReflectionOnly(string fullname, out string fullPath)
        {
            return LoadReflectionOnly(fullname, Directory.GetCurrentDirectory(), out fullPath);
        }
        public static Assembly LoadReflectionOnly(string fullname)
        {
            string notUsed;
            return LoadReflectionOnly(fullname, out notUsed);
        }

        static internal List<FileStream> LockDependencyFiles(List<string> depList)
        {
            List<FileStream> lockList = new List<FileStream>();
            try
            {
                foreach (string fullName in depList)
                {
                    string fullPath = LookupAssemblyLocation(fullName, Global.MyAssemblyPath);
                    try
                    {
                        File.Move(fullPath, fullPath + "__LOCKED__");
                        FileStream lockStream = new FileStream(
                            fullPath + "__LOCKED__",
                            FileMode.Open,
                            FileAccess.ReadWrite,
                            FileShare.None);
                        lockList.Add(lockStream);
                    }
                    catch (Exception)
                    {
                        //Log.WriteLine(LogLevel.L5, "[ERROR]  ASMFactory : LockDependencyFiles : {0}", e.Message);
                    }
                }
            }
            catch (Exception e)
            {
                Log.WriteLine(LogLevel.Debug, "ASMFactory", "{0}", e.Message);
            }
            return lockList;
        }
        static internal void UnlockDependencyFiles(List<FileStream> locked_files)
        {
            foreach (FileStream fs in locked_files)
            {
                string path = fs.Name;
                fs.Close();
                File.Move(path, path.Substring(0, path.Length - "__LOCKED__".Length));
            }
        }
        internal static string LookupAssemblyLocation(string fullName, string path)
        {
            AppDomain newDomain = AppDomain.CreateDomain("Sandbox");
            newDomain.Load(Assembly.GetExecutingAssembly().GetName());
            string fullPath = "";
            _asm_factory_ factory = newDomain.CreateInstanceAndUnwrap(
                Assembly.GetExecutingAssembly().GetName().FullName,
                "Trinity.Utilities._asm_factory_") as _asm_factory_;
            fullPath = factory.__lookup_assembly_location__(fullName, path);
            AppDomain.Unload(newDomain);
            return fullPath;
        }
    }
    class _asm_factory_ : MarshalByRefObject
    {
        // this interface do not contain the origin assembly in the dependency list
        public List<string> __get_assembly_dependency_list_with_filename__(string filename, string searchPath = "")
        {
            //Console.WriteLine("Executing in domain {0}", AppDomain.CurrentDomain.FriendlyName);
            try
            {
                Assembly asm = Assembly.ReflectionOnlyLoadFrom(filename);
                List<string> list = __get_assembly_dependency_list__(asm, searchPath);
                list.Remove(asm.FullName);
                return list;
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }
        public List<string> __get_assembly_dependency_list__(byte[] asmBytes, string searchPath = "")
        {
            Assembly asm = Assembly.ReflectionOnlyLoad(asmBytes);
            return __get_assembly_dependency_list__(asm, searchPath);
        }
        public List<string> __get_assembly_dependency_list__(Assembly asm, string searchPath = "")
        {
            //Console.WriteLine("Executing in domain {0}", AppDomain.CurrentDomain.FriendlyName);
            string asmLocation;
            if (asm.Location != "")
                asmLocation = Path.GetDirectoryName(asm.Location.ToLowerInvariant());
            else
                asmLocation = "";
            if (
                    (
                    asmLocation != ""//the assembly is not on disk
                    &&
                    asmLocation != Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).ToLowerInvariant()
                    )
                &&
                    (searchPath == "" || (asmLocation != searchPath.ToLowerInvariant()))
                )
            {
                return new List<string>();
            }
            if (asm.FullName.Contains("vshost"))
                return new List<string>();
            List<string> ret = new List<string>();
            ret.Add(asm.FullName);
            foreach (var refAsm in asm.GetReferencedAssemblies())
            {
                //filter assembly Trinity.Shell.exe and Trinity.Core.dll to reduce network traffic
                if (refAsm.Name == "Trinity.Shell" || refAsm.Name == "Trinity.Core" || refAsm.Name.StartsWith("Microsoft.SqlServer", StringComparison.Ordinal))
                    continue;
                //filter all "mscorlib" related items
                if (refAsm.FullName.Contains("mscorlib"))
                    continue;
                string fullPath;
                Assembly refAsmReflectOnly = null;
                try
                {
                    refAsmReflectOnly = __load_reflection_only__(refAsm.FullName, asmLocation, out fullPath);
                }
                catch (Exception)
                {
                    //Cannot load the referenced assembly into current domain.
                    //So don't add this assembly to dependency list
                    continue;
                }
                ret.AddRange(__get_assembly_dependency_list__(refAsmReflectOnly));
            }
            ret = ret.Distinct().ToList();
            return ret;
        }
        public string __lookup_assembly_location__(string fullname, string searchPath)
        {
            string fullPath = "";
            if (searchPath == "")// search the assembly path
            {
                searchPath = Global.MyAssemblyPath;
            }
            foreach (string s in Directory.GetFileSystemEntries(searchPath))
            {
                string fname = s.ToLowerInvariant();
                if (s.EndsWith(".exe", StringComparison.Ordinal) || s.EndsWith(".dll", StringComparison.Ordinal))
                {
                    try
                    {
                        Assembly asm = Assembly.ReflectionOnlyLoadFrom(s);
                        if (asm.FullName == fullname)
                        {
                            fullPath = s;
                            return fullPath;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            return "";
        }
        public Assembly __load_reflection_only__(string fullname, string searchPath, out string fullPath)
        {
            fullPath = "";
            if (searchPath == "")// search the assembly path
            {
                searchPath = Global.MyAssemblyPath;
            }
            foreach (string s in Directory.GetFileSystemEntries(searchPath))
            {
                string fname = s.ToLowerInvariant();
                if (s.EndsWith(".exe", StringComparison.Ordinal) || s.EndsWith(".dll", StringComparison.Ordinal))
                {
                    Assembly asm = Assembly.ReflectionOnlyLoadFrom(s);
                    if (asm.FullName == fullname)
                    {
                        fullPath = s;
                        return asm;
                    }
                }
            }
            return null;
        }

        public string __get_assembly_fullname__(byte[] asmBytes)
        {
            Assembly asm = Assembly.ReflectionOnlyLoad(asmBytes);
            return asm.FullName;
        }
    }
}
