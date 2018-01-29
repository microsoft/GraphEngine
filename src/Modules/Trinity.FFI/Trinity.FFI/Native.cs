using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Trinity.TSL.Lib;

namespace Trinity.FFI
{
    internal unsafe delegate string TRINITY_FFI_SYNC_HANDLER(string content);
    internal unsafe delegate void TRINITY_FFI_ASYNC_HANDLER(string content);

    internal unsafe delegate void TRINITY_FFI_SYNC_REGISTRY(int methodId, TRINITY_FFI_SYNC_HANDLER handler);
    internal unsafe delegate void TRINITY_FFI_ASYNC_REGISTRY(int methodId, TRINITY_FFI_ASYNC_HANDLER handler);

    internal unsafe delegate string TRINITY_FFI_SYNC_SEND(int partitionId, int methodId, string content);
    internal unsafe delegate void TRINITY_FFI_ASYNC_SEND(int partitionId, int methodId, string content);

    internal unsafe delegate TrinityErrorCode TRINITY_FFI_LOCAL_SAVESTORAGE();
    internal unsafe delegate TrinityErrorCode TRINITY_FFI_LOCAL_LOADSTORAGE();
    internal unsafe delegate TrinityErrorCode TRINITY_FFI_LOCAL_RESETSTORAGE();

    internal unsafe delegate TrinityErrorCode TRINITY_FFI_CLOUD_SAVESTORAGE();
    internal unsafe delegate TrinityErrorCode TRINITY_FFI_CLOUD_LOADSTORAGE();
    internal unsafe delegate TrinityErrorCode TRINITY_FFI_CLOUD_RESETSTORAGE();

    internal unsafe delegate TrinityErrorCode TRINITY_FFI_LOCAL_LOADCELL(long cellId, ref IntPtr cell);
    internal unsafe delegate TrinityErrorCode TRINITY_FFI_CLOUD_LOADCELL(long cellId, ref IntPtr cell);

    internal unsafe delegate TrinityErrorCode TRINITY_FFI_LOCAL_SAVECELL_1(long cellId, IntPtr cell);
    internal unsafe delegate TrinityErrorCode TRINITY_FFI_LOCAL_SAVECELL_2(long cellId, [MarshalAs(UnmanagedType.I4)] CellAccessOptions options, IntPtr cell);
    internal unsafe delegate TrinityErrorCode TRINITY_FFI_CLOUD_SAVECELL(long cellId, IntPtr cell);

    internal unsafe delegate TrinityErrorCode TRINITY_FFI_LOCAL_REMOVECELL(long cellId);
    internal unsafe delegate TrinityErrorCode TRINITY_FFI_CLOUD_REMOVECELL(long cellId);

    internal unsafe delegate TrinityErrorCode TRINITY_FFI_NEWCELL_1(string cellType, ref IntPtr cell);
    internal unsafe delegate TrinityErrorCode TRINITY_FFI_NEWCELL_2(long cellId, string cellType, ref IntPtr cell);
    internal unsafe delegate TrinityErrorCode TRINITY_FFI_NEWCELL_3(string cellType, string cellContent, ref IntPtr cell);

    internal unsafe delegate string TRINITY_FFI_CELL_TOSTRING(IntPtr cell);
    internal unsafe delegate long TRINITY_FFI_CELL_GETID(IntPtr cell);
    internal unsafe delegate void TRINITY_FFI_CELL_SETID(IntPtr cell, long cellId);
    internal unsafe delegate string TRINITY_FFI_CELL_GET(IntPtr cell, string field);
    internal unsafe delegate int TRINITY_FFI_CELL_HAS(IntPtr cell, string field);//non-zero if cell has field.
    internal unsafe delegate void TRINITY_FFI_CELL_SET(IntPtr cell, string field, string content);
    internal unsafe delegate void TRINITY_FFI_CELL_APPEND(IntPtr cell, string field, string content);
    internal unsafe delegate void TRINITY_FFI_CELL_DELETE(IntPtr cell, string field);
    internal unsafe delegate void TRINITY_FFI_CELL_DISPOSE(IntPtr cell);

    internal unsafe delegate TrinityErrorCode TRINITY_FFI_CELL_FIELD_ENUMERATOR_GET(IntPtr cell, ref IntPtr enumerator);
    internal unsafe delegate TrinityErrorCode TRINITY_FFI_CELL_FIELD_ENUMERATOR_MOVENEXT(IntPtr enumerator, IntPtr field_info);
    internal unsafe delegate TrinityErrorCode TRINITY_FFI_CELL_FIELD_ENUMERATOR_CURRENT(IntPtr enumerator, ref IntPtr field_info);
    internal unsafe delegate TrinityErrorCode TRINITY_FFI_CELL_FIELD_ENUMERATOR_DISPOSE(IntPtr enumerator);
    internal unsafe delegate TrinityErrorCode TRINITY_FFI_CELL_FIELD_INFO_NAME(IntPtr field_info, out string value);

    struct TRINITY_INTERFACES
    {
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_SYNC_REGISTRY sync_registry;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_ASYNC_REGISTRY async_registry;

        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_SYNC_SEND sync_send;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_ASYNC_SEND async_send;

        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_LOCAL_LOADSTORAGE local_loadstorage;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_LOCAL_SAVESTORAGE local_savestorage;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_LOCAL_RESETSTORAGE local_resetstorage;

        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_CLOUD_LOADSTORAGE cloud_loadstorage;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_CLOUD_SAVESTORAGE cloud_savestorage;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_CLOUD_RESETSTORAGE cloud_resetstorage;


        //IEnumerable<ICellAccessor> EnumerateGenericCellAccessors(LocalMemoryStorage storage);
        //IEnumerable<ICell> EnumerateGenericCells(LocalMemoryStorage storage);
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_LOCAL_LOADCELL local_loadcell;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_CLOUD_LOADCELL cloud_loadcell;

        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_LOCAL_SAVECELL_1 local_savecell_1;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_LOCAL_SAVECELL_2 local_savecell_2;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_CLOUD_SAVECELL cloud_savecell;

        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_LOCAL_REMOVECELL local_removecell;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_LOCAL_REMOVECELL cloud_removecell;

        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_NEWCELL_1 newcell_1;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_NEWCELL_2 newcell_2;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_NEWCELL_3 newcell_3;

        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_CELL_TOSTRING cell_tostring;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_CELL_GETID cell_getid;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_CELL_SETID cell_setid;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_CELL_HAS cell_hasfield;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_CELL_GET cell_getfield;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_CELL_SET cell_setfield;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_CELL_APPEND cell_appendfield;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_CELL_DELETE cell_removefield;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_CELL_DISPOSE cell_dispose;

        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_CELL_FIELD_ENUMERATOR_GET cell_fieldenum_get;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_CELL_FIELD_ENUMERATOR_CURRENT cell_fieldenum_current;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_CELL_FIELD_ENUMERATOR_MOVENEXT cell_fieldenum_movenext;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_CELL_FIELD_ENUMERATOR_DISPOSE cell_fieldenum_dispose;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TRINITY_FFI_CELL_FIELD_INFO_NAME cell_fieldinfo_name;

        //ICellAccessor UseGenericCell(LocalMemoryStorage storage, long cellId);
        //ICellAccessor UseGenericCell(LocalMemoryStorage storage, long cellId, CellAccessOptions options);
        //ICellAccessor UseGenericCell(LocalMemoryStorage storage, long cellId, CellAccessOptions options, string cellType);
    };

    internal static unsafe class Native
    {
        [DllImport("trinity_ffi", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        internal static extern void TRINITY_FFI_SET_INTERFACES(ref TRINITY_INTERFACES interfaces);
    }
}
