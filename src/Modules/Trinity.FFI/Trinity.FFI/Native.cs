/***********************************

  Auto-generated from Native.tt

 ***********************************/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Trinity.TSL.Lib;
using Trinity.FFI.Interop;
using static GraphEngine.Jit.JitNativeInterop;

namespace Trinity.FFI
{
    internal unsafe delegate string TRINITY_FFI_SYNC_HANDLER(string content);
    internal unsafe delegate void TRINITY_FFI_ASYNC_HANDLER(string content);

    internal unsafe delegate String TRINITY_FFI_JITSWIGGEN(String p1, String p2);
    internal unsafe delegate TrinityErrorCode TRINITY_FFI_ACCESSOR_USE_1(Int64 p0, ref IntPtr p1);
    internal unsafe delegate TrinityErrorCode TRINITY_FFI_ACCESSOR_USE_2(Int64 p0, [MarshalAs(UnmanagedType.I4)]CellAccessOptions p1, ref IntPtr p2);
    internal unsafe delegate TrinityErrorCode TRINITY_FFI_ACCESSOR_USE_3(Int64 p0, [MarshalAs(UnmanagedType.I4)]CellAccessOptions p1, ref IntPtr p2, String p3);
    internal unsafe delegate TrinityErrorCode TRINITY_FFI_CELL_NEW_1(String p0, ref IntPtr p1);
    internal unsafe delegate TrinityErrorCode TRINITY_FFI_CELL_NEW_2(Int64 p0, String p1, ref IntPtr p2);
    internal unsafe delegate TrinityErrorCode TRINITY_FFI_CELL_NEW_3(String p0, String p1, ref IntPtr p2);
    internal unsafe delegate String TRINITY_FFI_CELL_TOSTRING(IntPtr p0);
    internal unsafe delegate TrinityErrorCode TRINITY_FFI_CELL_TOBINARY(String p0, String p1, out Int64 p2, out Int64 p3);
    internal unsafe delegate Int64 TRINITY_FFI_CELL_GETID(IntPtr p0);
    internal unsafe delegate void TRINITY_FFI_CELL_SETID(IntPtr p0, Int64 p1);
    internal unsafe delegate String TRINITY_FFI_CELL_GET(IntPtr p0, String p1);
    internal unsafe delegate Int32 TRINITY_FFI_CELL_HAS(IntPtr p0, String p1);
    internal unsafe delegate void TRINITY_FFI_CELL_SET(IntPtr p0, String p1, String p2);
    internal unsafe delegate void TRINITY_FFI_CELL_APPEND(IntPtr p0, String p1, String p2);
    internal unsafe delegate void TRINITY_FFI_CELL_DELETE(IntPtr p0, String p1);
    internal unsafe delegate TrinityErrorCode TRINITY_FFI_CLOUD_LOADCELL(Int64 p0, ref IntPtr p1);
    internal unsafe delegate TrinityErrorCode TRINITY_FFI_CLOUD_SAVECELL(Int64 p0, IntPtr p1);
    internal unsafe delegate TrinityErrorCode TRINITY_FFI_ENUM_NEXT(IntPtr p0);
    internal unsafe delegate TrinityErrorCode TRINITY_FFI_GC_FREE(IntPtr p0);
    internal unsafe delegate TrinityErrorCode TRINITY_FFI_GC_DISPOSE(IntPtr p0);
    internal unsafe delegate TrinityErrorCode TRINITY_FFI_LOCAL_LOADCELL(Int64 p0, ref IntPtr p1);
    internal unsafe delegate TrinityErrorCode TRINITY_FFI_LOCAL_SAVECELL_1(Int64 p0, IntPtr p1);
    internal unsafe delegate TrinityErrorCode TRINITY_FFI_LOCAL_SAVECELL_2(Int64 p0, [MarshalAs(UnmanagedType.I4)]CellAccessOptions p1, IntPtr p2);
    internal unsafe delegate TrinityErrorCode TRINITY_FFI_LOCAL_SAVECELL_3(IntPtr p0);
    internal unsafe delegate TrinityErrorCode TRINITY_FFI_LOCAL_SAVECELL_4([MarshalAs(UnmanagedType.I4)]CellAccessOptions p0, IntPtr p1);
    internal unsafe delegate TrinityErrorCode TRINITY_FFI_LOCAL_REMOVECELL(Int64 p0);
    internal unsafe delegate TrinityErrorCode TRINITY_FFI_LOCAL_SAVESTORAGE();
    internal unsafe delegate TrinityErrorCode TRINITY_FFI_LOCAL_LOADSTORAGE();
    internal unsafe delegate TrinityErrorCode TRINITY_FFI_LOCAL_RESETSTORAGE();
    internal unsafe delegate TrinityErrorCode TRINITY_FFI_RT_GETFUNCTION(String p0, String p1, String p2, out IntPtr p3);
    internal unsafe delegate TrinityErrorCode TRINITY_FFI_SCHEMA_GET(out NativeTypeDescriptor[] p0, out Int32 p1);

    internal struct TRINITY_INTERFACES
    {
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_JITSWIGGEN jitSwigGen;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_ACCESSOR_USE_1 accessor_use_1;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_ACCESSOR_USE_2 accessor_use_2;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_ACCESSOR_USE_3 accessor_use_3;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_CELL_NEW_1 cell_new_1;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_CELL_NEW_2 cell_new_2;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_CELL_NEW_3 cell_new_3;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_CELL_TOSTRING cell_tostring;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_CELL_TOBINARY cell_tobinary;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_CELL_GETID cell_getid;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_CELL_SETID cell_setid;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_CELL_GET cell_get;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_CELL_HAS cell_has;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_CELL_SET cell_set;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_CELL_APPEND cell_append;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_CELL_DELETE cell_delete;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_CLOUD_LOADCELL cloud_loadcell;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_CLOUD_SAVECELL cloud_savecell;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_ENUM_NEXT enum_next;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_GC_FREE gc_free;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_GC_DISPOSE gc_dispose;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_LOCAL_LOADCELL local_loadcell;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_LOCAL_SAVECELL_1 local_savecell_1;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_LOCAL_SAVECELL_2 local_savecell_2;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_LOCAL_SAVECELL_3 local_savecell_3;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_LOCAL_SAVECELL_4 local_savecell_4;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_LOCAL_REMOVECELL local_removecell;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_LOCAL_SAVESTORAGE local_savestorage;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_LOCAL_LOADSTORAGE local_loadstorage;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_LOCAL_RESETSTORAGE local_resetstorage;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_RT_GETFUNCTION rt_getfunction;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_SCHEMA_GET schema_get;
    };

    internal static unsafe class Native
    {
        [DllImport("trinity_ffi", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        internal static extern void TRINITY_FFI_SET_INTERFACES(ref TRINITY_INTERFACES interfaces);
    }
}
