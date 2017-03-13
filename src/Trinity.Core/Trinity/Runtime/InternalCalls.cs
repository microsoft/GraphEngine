// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using System.Reflection;
using Trinity.Utilities;
using System.Runtime.CompilerServices;
using System.IO;

using Trinity.Diagnostics;
using Trinity.Mathematics;
using Trinity.Core.Lib;
using Trinity.Storage;
using Trinity.Daemon;
using Trinity.Properties;
using Trinity.TSL.Lib;
using Trinity.Network;
using System.Globalization;

namespace Trinity
{
#if !CORECLR
    partial class InternalCalls
    {
        /*************************************************************
         * 
         * One to one mapped to Trinity.C/header/InternalCalls.h
         * Each entry is first looked up with reflection. When a matched
         * method handle is found, the full name of the entry is then
         * synthesized with the following rules:
         * 
         * 1. basic form:
         *    name_space.class_name::method_name[(optional signature)]
         * 2. when parameter type list is provided in the C# entry,
         *    [optional signature] is synthesized and complies with
         *    mono naming convension. See GetTypeSignature(Type) for
         *    detailed rules.
         * 
         *************************************************************/
        static private List<InternalCallEntry> iCallEntries = new List<InternalCallEntry>
        {
    #region File I/O
            new InternalCallEntry("C_wfopen_s"                              , typeof(CStdio)),
            new InternalCallEntry("C_fread"                                 , typeof(CStdio)),
            new InternalCallEntry("C_fwrite"                                , typeof(CStdio)),
            new InternalCallEntry("C_fflush"                                , typeof(CStdio)),
            new InternalCallEntry("C_fclose"                                , typeof(CStdio)),
            new InternalCallEntry("C_feof"                                  , typeof(CStdio)),
    #endregion

    #region Memory
            new InternalCallEntry("Copy"                                    , typeof(CMemory), new List<Type>(){ typeof(void*), typeof(void*),typeof(int) }),
            new InternalCallEntry("C_malloc"                                , typeof(CMemory)),
            new InternalCallEntry("C_free"                                  , typeof(CMemory)),
            new InternalCallEntry("C_realloc"                               , typeof(CMemory)),
            new InternalCallEntry("C_memcpy"                                , typeof(CMemory)),
            new InternalCallEntry("C_memmove"                               , typeof(CMemory)),
            new InternalCallEntry("C_memset"                                , typeof(CMemory)),
            new InternalCallEntry("C_memcmp"                                , typeof(CMemory)),
            new InternalCallEntry("C_aligned_malloc"                        , typeof(CMemory)),
            new InternalCallEntry("C_aligned_free"                          , typeof(CMemory)),
            new InternalCallEntry("AlignedAlloc"                            , typeof(CMemory)),
            new InternalCallEntry("SetWorkingSetProfile"                    , typeof(CMemory)),
            new InternalCallEntry("SetMaxWorkingSet"                        , typeof(CMemory)),
    #endregion

    #region Mathematics
            new InternalCallEntry("C_multiply_double_vector"                  , typeof(CMathUtility)),
            new InternalCallEntry("C_multiply_sparse_double_vector"           , typeof(CMathUtility)),
    #endregion

    #region GetLastError
            new InternalCallEntry("C_GetLastError"                            , typeof(CTrinityC)),
    #endregion

    #region Storage
            new InternalCallEntry("CInitialize"                             , typeof(CLocalMemoryStorage)),
            new InternalCallEntry("CCellCount"                              , typeof(CLocalMemoryStorage)),
            new InternalCallEntry("CResetStorage"                           , typeof(CLocalMemoryStorage)),
            new InternalCallEntry("CDispose"                                , typeof(CLocalMemoryStorage)),
            new InternalCallEntry("CSaveStorage"                            , typeof(CLocalMemoryStorage)),
            new InternalCallEntry("CLoadStorage"                            , typeof(CLocalMemoryStorage)),
            new InternalCallEntry("CGetTrinityImageSignature"               , typeof(CLocalMemoryStorage)),

            /* Non-logging interfaces */
            new InternalCallEntry("CSaveCell"                               , typeof(CLocalMemoryStorage), new List<Type>{typeof(long), typeof(byte*), typeof(int), typeof(ushort)}),
            new InternalCallEntry("CAddCell"                                , typeof(CLocalMemoryStorage), new List<Type>{typeof(long), typeof(byte*), typeof(int), typeof(ushort)}),
            new InternalCallEntry("CUpdateCell"                             , typeof(CLocalMemoryStorage), new List<Type>{typeof(long), typeof(byte*), typeof(int)}),
            new InternalCallEntry("CRemoveCell"                             , typeof(CLocalMemoryStorage), new List<Type>{typeof(long)}),

            /* Logging interfaces */
            new InternalCallEntry("CLoggedSaveCell"                         , typeof(CLocalMemoryStorage), new List<Type>{typeof(long), typeof(byte*), typeof(int), typeof(ushort), typeof(CellAccessOptions)}),
            new InternalCallEntry("CLoggedAddCell"                          , typeof(CLocalMemoryStorage), new List<Type>{typeof(long), typeof(byte*), typeof(int), typeof(ushort), typeof(CellAccessOptions)}),
            new InternalCallEntry("CLoggedUpdateCell"                       , typeof(CLocalMemoryStorage), new List<Type>{typeof(long), typeof(byte*), typeof(int), typeof(CellAccessOptions)}),
            new InternalCallEntry("CLoggedRemoveCell"                       , typeof(CLocalMemoryStorage), new List<Type>{typeof(long), typeof(CellAccessOptions)}),
            new InternalCallEntry("CWriteAheadLog"                          , typeof(CLocalMemoryStorage)),
            new InternalCallEntry("CSetWriteAheadLogFile"                   , typeof(CLocalMemoryStorage)),
            new InternalCallEntry("CWriteAheadLogComputeChecksum"           , typeof(LOG_RECORD_HEADER)),
            new InternalCallEntry("CWriteAheadLogValidateChecksum"          , typeof(LOG_RECORD_HEADER)),
            ///////////////////////

            new InternalCallEntry("CResizeCell"                             , typeof(CLocalMemoryStorage)),
            new InternalCallEntry("CGetCellType"                            , typeof(CLocalMemoryStorage)),
            new InternalCallEntry("CReleaseCellLock"                        , typeof(CLocalMemoryStorage)),
            new InternalCallEntry("CContains"                               , typeof(CLocalMemoryStorage)),

            new InternalCallEntry("CLocalMemoryStorageEnumeratorAllocate"   , typeof(CLocalMemoryStorage)),
            new InternalCallEntry("CLocalMemoryStorageEnumeratorDeallocate" , typeof(CLocalMemoryStorage)),
            new InternalCallEntry("CLocalMemoryStorageEnumeratorMoveNext"   , typeof(CLocalMemoryStorage)),
            new InternalCallEntry("CLocalMemoryStorageEnumeratorReset"      , typeof(CLocalMemoryStorage)),

            new InternalCallEntry("SetDefragmentationPaused"                , typeof(CLocalMemoryStorage)),
            new InternalCallEntry("StopDefragAndAwaitCeased"                , typeof(CLocalMemoryStorage)),
            new InternalCallEntry("RestartDefragmentation"                  , typeof(CLocalMemoryStorage)),

            new InternalCallEntry("CTrunkCommittedMemorySize"               , typeof(CLocalMemoryStorage)),
            new InternalCallEntry("CMTHashCommittedMemorySize"              , typeof(CLocalMemoryStorage)),
            new InternalCallEntry("CTotalCommittedMemorySize"               , typeof(CLocalMemoryStorage)),
            new InternalCallEntry("CTotalCellSize"                          , typeof(CLocalMemoryStorage)),

            new InternalCallEntry("CGetLockedCellInfo4CellAccessor"         , typeof(CLocalMemoryStorage)),
            new InternalCallEntry("CGetLockedCellInfo4SaveCell"             , typeof(CLocalMemoryStorage)),
            new InternalCallEntry("CGetLockedCellInfo4AddCell"              , typeof(CLocalMemoryStorage)),
            new InternalCallEntry("CGetLockedCellInfo4UpdateCell"           , typeof(CLocalMemoryStorage)),
            new InternalCallEntry("CGetLockedCellInfo4LoadCell"             , typeof(CLocalMemoryStorage)),
            new InternalCallEntry("CGetLockedCellInfo4AddOrUseCell"         , typeof(CLocalMemoryStorage)),
            new InternalCallEntry("CLockedGetCellSize"                      , typeof(CLocalMemoryStorage)),

            new InternalCallEntry("CStartDebugger"                          , typeof(CLocalMemoryStorage)),
    #endregion

    #region Network
            //TODO remove network InternalCall entries
            new InternalCallEntry("StartSocketServer"                       , typeof(CNativeNetwork)),
            new InternalCallEntry("ShutdownSocketServer"                    , typeof(CNativeNetwork)),
            new InternalCallEntry("AwaitRequest"                            , typeof(CNativeNetwork)),
            new InternalCallEntry("SendResponse"                            , typeof(CNativeNetwork)),

            new InternalCallEntry("CreateClientSocket"                      , typeof(CNativeNetwork)),
            new InternalCallEntry("ClientSocketConnect"                     , typeof(CNativeNetwork)),
            new InternalCallEntry("ClientSend"                              , typeof(CNativeNetwork)),
            new InternalCallEntry("ClientReceive"                           , typeof(CNativeNetwork)),
            new InternalCallEntry("WaitForAckPackage"                       , typeof(CNativeNetwork)),
            new InternalCallEntry("WaitForStatusPackage"                    , typeof(CNativeNetwork)),
            new InternalCallEntry("CloseClientSocket"                       , typeof(CNativeNetwork)),
    #endregion
        };
    }
    struct InternalCallEntry
    {
        public InternalCallEntry(string name, Type ctype) : this(name, ctype, null) { }
        public InternalCallEntry(string name, Type ctype, List<Type> parameterTypes)
        {
            MethodName = name;
            ClassType = ctype;
            ParameterTypes = parameterTypes;
        }

        public string MethodName;
        public Type ClassType;
        public List<Type> ParameterTypes;
    }
    partial class InternalCalls
    {
        static unsafe InternalCalls()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                TrinityC.Ping();
                // ---------------- Methods are JITted in Register(), RuntimeHelpers.PrepareMethod
                Win32.NativeAPI.timeBeginPeriod(1);
                __INIT_TRINITY_C__();
                Register();
            }
        }

        /// <summary>
        /// Hotswap a C# method with a C function in internal call table
        /// </summary>
        internal static unsafe void HotSwap(MethodInfo method, string CMethodName)
        {
            //Prepare the method
            RuntimeHelpers.PrepareMethod(method.MethodHandle);
            if (!RegisterInternalCall(method.MethodHandle.Value.ToPointer(), CMethodName))
            {
                throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "{0} cannot be hot hot swapped!", method.Name));
            }
        }
        /// <summary>
        /// Hotswap a C# method with another one
        /// !!Caution: Make sure that ptr1 and source have same prototype
        /// !!Caution: Make sure both methods are marked as "NoInlining"
        /// </summary>
        internal static unsafe void HotSwap(MethodInfo target, MethodInfo source)
        {
            RuntimeHelpers.PrepareMethod(target.MethodHandle);
            RuntimeHelpers.PrepareMethod(source.MethodHandle);
            void* target_ptr = target.MethodHandle.Value.ToPointer();
            void* src_ptr = source.MethodHandle.Value.ToPointer();
            HotSwapCSharpMethod(target_ptr, src_ptr);
        }
        /// <summary>
        /// Hotswap a C# method with another one
        /// !!Caution: Make sure that ptr1 and source have same prototype
        /// !!Caution: Make sure both methods are marked as "NoInlining"
        /// </summary>
        internal static unsafe void HotSwap(Type targetType, string targetMethod, Type sourceType, string sourceMethod)
        {
            var source = sourceType.GetMethod(sourceMethod, (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance));
            var target = targetType.GetMethod(targetMethod, (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance));

            if (source != null && target != null)
            {
                //Console.WriteLine("Swapping {0} from {1}", target.Name, source.Name);
                HotSwap(target, source);
            }
            else
            {
                throw new InvalidOperationException("HotSwap: Failed to find method!");
            }
        }
        private static unsafe void Register()
        {
            foreach (var entry in iCallEntries)
            {
                foreach (MethodInfo m in entry.ClassType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                {
                    //Only static functions are supported
                    if (!m.IsStatic)
                        continue;
                    if (entry.MethodName != m.Name)
                        continue;
                    if (m.GetCustomAttribute<DllImportAttribute>() != null)
                        continue;
                    if (entry.ParameterTypes != null)
                    {
                        var param = m.GetParameters();
                        if (entry.ParameterTypes.Count != param.Length)
                            continue;
                        bool sameTypes = true;
                        for (int i = 0; i < param.Length; ++i)
                        {
                            if (entry.ParameterTypes[i] != param[i].ParameterType)
                            {
                                sameTypes = false;
                                continue;
                            }
                        }
                        if (!sameTypes)
                            continue;
                    }

                    //Method matched. Register it now.

                    string entry_namespace    = entry.ClassType.Namespace;
                    string entry_classname    = entry.ClassType.Name;
                    string entry_methodname   = m.Name;
                    string entry_methodparams = String.Join(",", from p in m.GetParameters()
                                                                 select GetTypeSignature(p));
                    string completeName;

                    if (entry.ParameterTypes != null)
                    {
                        completeName = entry_namespace + "." + entry_classname + "::" + entry_methodname + "(" + entry_methodparams + ")";
                    }
                    else
                    {
                        completeName = entry_namespace + "." + entry_classname + "::" + entry_methodname;
                    }

                    if (!RegisterInternalCall(m.MethodHandle.Value.ToPointer(), completeName))
                    {
                        throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "InternalCall {0} cannot be registered!", completeName));
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// See mono source code mono/metadata/debug-helpers.c: mono_type_get_desc
        /// </summary>
        private static Dictionary<Type, string> s_built_in_types = new Dictionary<Type, string>
        {
            {typeof(void)    ,"void"},
            {typeof(char)    ,"char"},
            {typeof(bool)    ,"bool"},
            {typeof(byte)    ,"byte"},
            {typeof(sbyte)   ,"sbyte"},
            {typeof(ushort)  ,"uint16"},
            {typeof(short)   ,"int16"},
            {typeof(uint)    ,"uint"},
            {typeof(int)     ,"int"},
            {typeof(ulong)   ,"ulong"},
            {typeof(long)    ,"long"},
     //{typeof(uintptr)      ,"uintptr"},
     //{typeof(intptr)       ,"intptr"},
            {typeof(float)   ,"single"},
     	    {typeof(double)  ,"double"},
     	    {typeof(string)  ,"string"},
     	    {typeof(object)  ,"object"},
        };

        private static string GetTypeSignature(ParameterInfo p)
        {
            Type ptype = p.ParameterType;
            Type etype = ptype;
            bool is_byref = false;
            bool is_pointer = false;
            do
            {
                // XXX: no support for array, generics, function pointers etc.
                // but these types are not used in InternalCalls.
                if (etype.IsByRef) { is_byref = true; }
                if (etype.IsPointer) { is_pointer = true; }
                if (etype.GetElementType() != null) { etype = etype.GetElementType(); }
            } while (etype.GetElementType() != null);

            string built_in_type_name;

            if (s_built_in_types.TryGetValue(etype, out built_in_type_name))
            {
                if (is_pointer) { built_in_type_name += "*"; }
                if (is_byref) { built_in_type_name += "&"; }
                return built_in_type_name;
            }

            return ptype.FullName;
        }

        internal static void __init()
        {
            //Nothing, just trigger the static constructor
        }

        [DllImport(TrinityC.AssemblyName)]
        private static extern unsafe void __INIT_TRINITY_C__();

        [DllImport(TrinityC.AssemblyName)]
        static private unsafe extern bool RegisterInternalCall(void* MethodTablePtr, string name);

        [DllImport(TrinityC.AssemblyName)]
        static private unsafe extern void HotSwapCSharpMethod(void* TargetMethodDesc, void* SourceMethodDesc);
    }
#else
    class InternalCalls
    {
        static InternalCalls()
        {
            // TODO we have to figure out whether to auto-release Trinity.C for multiple platforms,
            // or to rely on a nuget package to deliver the correct binary for a specific platform.

            TrinityC.Ping();
            __INIT_TRINITY_C__();

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                Win32.NativeAPI.timeBeginPeriod(1);
            }
        }

        internal static void __init()
        {
            //Nothing, just trigger the static constructor
        }

        [DllImport(TrinityC.AssemblyName)]
        private static extern unsafe void __INIT_TRINITY_C__();
    }

#endif
}
