using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Trinity.TSL.Lib;

namespace Trinity.FFI
{
    internal unsafe delegate string TRINITY_FFI_SYNC_HANDLER(string content);
    internal unsafe delegate void TRINITY_FFI_ASYNC_HANDLER(string content);

    internal unsafe delegate void TRINITY_FFI_SYNC_REGISTRY(int methodId,  TRINITY_FFI_SYNC_HANDLER handler);
    internal unsafe delegate void TRINITY_FFI_ASYNC_REGISTRY(int methodId,  TRINITY_FFI_ASYNC_HANDLER handler);

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

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
    struct TRINITY_INTERFACES
    {
        public TRINITY_FFI_SYNC_REGISTRY sync_registry;
        public TRINITY_FFI_ASYNC_REGISTRY async_registry;

        public TRINITY_FFI_SYNC_SEND sync_send;
        public TRINITY_FFI_ASYNC_SEND async_send;

        public TRINITY_FFI_LOCAL_LOADSTORAGE local_loadstorage;
        public TRINITY_FFI_LOCAL_SAVESTORAGE local_savestorage;
        public TRINITY_FFI_LOCAL_RESETSTORAGE local_resetstorage;

        public TRINITY_FFI_CLOUD_LOADSTORAGE cloud_loadstorage;
        public TRINITY_FFI_CLOUD_SAVESTORAGE cloud_savestorage;
        public TRINITY_FFI_CLOUD_RESETSTORAGE cloud_resetstorage;


        //IEnumerable<ICellAccessor> EnumerateGenericCellAccessors(LocalMemoryStorage storage);
        //IEnumerable<ICell> EnumerateGenericCells(LocalMemoryStorage storage);
        public TRINITY_FFI_LOCAL_LOADCELL local_loadcell;
        public TRINITY_FFI_CLOUD_LOADCELL cloud_loadcell;

        public TRINITY_FFI_LOCAL_SAVECELL_1 local_savecell_1;
        public TRINITY_FFI_LOCAL_SAVECELL_2 local_savecell_2;
        public TRINITY_FFI_CLOUD_SAVECELL cloud_savecell;

        public TRINITY_FFI_LOCAL_REMOVECELL local_removecell;
        public TRINITY_FFI_LOCAL_REMOVECELL cloud_removecell;

        public TRINITY_FFI_NEWCELL_1 newcell_1;
        public TRINITY_FFI_NEWCELL_2 newcell_2;
        public TRINITY_FFI_NEWCELL_3 newcell_3;

        public TRINITY_FFI_CELL_TOSTRING cell_tostring;
        public TRINITY_FFI_CELL_GETID cell_getid;
        public TRINITY_FFI_CELL_SETID cell_setid;
        public TRINITY_FFI_CELL_HAS cell_hasfield;
        public TRINITY_FFI_CELL_GET cell_getfield;
        public TRINITY_FFI_CELL_SET cell_setfield;
        public TRINITY_FFI_CELL_APPEND cell_appendfield;
        public TRINITY_FFI_CELL_DELETE cell_removefield;
        public TRINITY_FFI_CELL_DISPOSE cell_dispose;
        public TRINITY_FFI_CELL_FIELD_ENUMERATOR_GET cell_fieldenum_get;
        public TRINITY_FFI_CELL_FIELD_ENUMERATOR_CURRENT cell_fieldenum_current;
        public TRINITY_FFI_CELL_FIELD_ENUMERATOR_MOVENEXT cell_fieldenum_movenext;
        public TRINITY_FFI_CELL_FIELD_ENUMERATOR_DISPOSE cell_fieldenum_dispose;
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
