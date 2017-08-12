using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Trinity.TSL.Lib;

namespace Trinity.FFI
{
    internal unsafe delegate string TRINITY_FFI_SYNC_HANDLER(int methodId, [MarshalAs(UnmanagedType.LPStr)] string content);
    internal unsafe delegate void TRINITY_FFI_ASYNC_HANDLER(int methodId, [MarshalAs(UnmanagedType.LPStr)]string content);

    internal unsafe delegate void TRINITY_FFI_SYNC_REGISTRY(int methodId, [MarshalAs(UnmanagedType.FunctionPtr)] TRINITY_FFI_SYNC_HANDLER handler);
    internal unsafe delegate void TRINITY_FFI_ASYNC_REGISTRY(int methodId, [MarshalAs(UnmanagedType.FunctionPtr)] TRINITY_FFI_ASYNC_HANDLER handler);

    internal unsafe delegate string TRINITY_FFI_SYNC_SEND(int partitionId, int methodId, [MarshalAs(UnmanagedType.LPStr)]string content);
    internal unsafe delegate void TRINITY_FFI_ASYNC_SEND(int partitionId, int methodId, [MarshalAs(UnmanagedType.LPStr)]string content);

    internal unsafe delegate TrinityErrorCode TRINITY_FFI_LOCAL_SAVESTORAGE();
    internal unsafe delegate TrinityErrorCode TRINITY_FFI_LOCAL_LOADSTORAGE();
    internal unsafe delegate TrinityErrorCode TRINITY_FFI_LOCAL_RESETSTORAGE();

    internal unsafe delegate TrinityErrorCode TRINITY_FFI_CLOUD_SAVESTORAGE();
    internal unsafe delegate TrinityErrorCode TRINITY_FFI_CLOUD_LOADSTORAGE();
    internal unsafe delegate TrinityErrorCode TRINITY_FFI_CLOUD_RESETSTORAGE();

    internal unsafe delegate TrinityErrorCode TRINITY_FFI_LOCAL_LOADCELL(long cellId, out IntPtr cell);
    internal unsafe delegate TrinityErrorCode TRINITY_FFI_CLOUD_LOADCELL(long cellId, out IntPtr cell);

    internal unsafe delegate TrinityErrorCode TRINITY_FFI_LOCAL_SAVECELL_1(long cellId, IntPtr cell);
    internal unsafe delegate TrinityErrorCode TRINITY_FFI_LOCAL_SAVECELL_2(long cellId, [MarshalAs(UnmanagedType.I4)] CellAccessOptions options, IntPtr cell);
    internal unsafe delegate TrinityErrorCode TRINITY_FFI_CLOUD_SAVECELL(long cellId, IntPtr cell);

    internal unsafe delegate TrinityErrorCode TRINITY_FFI_LOCAL_REMOVECELL(long cellId);
    internal unsafe delegate TrinityErrorCode TRINITY_FFI_CLOUD_REMOVECELL(long cellId);

    internal unsafe delegate TrinityErrorCode TRINITY_FFI_NEWCELL_1([MarshalAs(UnmanagedType.LPStr)]string cellType, out IntPtr cell);
    internal unsafe delegate TrinityErrorCode TRINITY_FFI_NEWCELL_2(long cellId, [MarshalAs(UnmanagedType.LPStr)]string cellType, out IntPtr cell);
    internal unsafe delegate TrinityErrorCode TRINITY_FFI_NEWCELL_3([MarshalAs(UnmanagedType.LPStr)]string cellType, [MarshalAs(UnmanagedType.LPStr)]string cellContent, out IntPtr cell);

    internal unsafe delegate string TRINITY_FFI_CELL_TOSTRING(IntPtr cell);
    internal unsafe delegate long TRINITY_FFI_CELL_GETID(IntPtr cell);
    internal unsafe delegate void TRINITY_FFI_CELL_SETID(IntPtr cell, long cellId);
    internal unsafe delegate string TRINITY_FFI_CELL_GET(IntPtr cell, [MarshalAs(UnmanagedType.LPStr)]string field);
    internal unsafe delegate int TRINITY_FFI_CELL_HAS(IntPtr cell, [MarshalAs(UnmanagedType.LPStr)]string field);//non-zero if cell has field.
    internal unsafe delegate void TRINITY_FFI_CELL_SET(IntPtr cell, [MarshalAs(UnmanagedType.LPStr)]string field, [MarshalAs(UnmanagedType.LPStr)]string content);
    internal unsafe delegate void TRINITY_FFI_CELL_APPEND(IntPtr cell, [MarshalAs(UnmanagedType.LPStr)]string field, [MarshalAs(UnmanagedType.LPStr)]string content);
    internal unsafe delegate void TRINITY_FFI_CELL_DELETE(IntPtr cell, [MarshalAs(UnmanagedType.LPStr)]string field);
    internal unsafe delegate void TRINITY_FFI_CELL_DISPOSE(IntPtr cell);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
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

        //ICellAccessor UseGenericCell(LocalMemoryStorage storage, long cellId);
        //ICellAccessor UseGenericCell(LocalMemoryStorage storage, long cellId, CellAccessOptions options);
        //ICellAccessor UseGenericCell(LocalMemoryStorage storage, long cellId, CellAccessOptions options, string cellType);
    };

    internal static unsafe class Native
    {
        [DllImport("trinity_ffi", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        internal static extern void TRINITY_FFI_SET_INTERFACES([MarshalAs(UnmanagedType.LPStruct)] TRINITY_INTERFACES interfaces);
    }
}
