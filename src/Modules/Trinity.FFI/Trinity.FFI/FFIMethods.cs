using System.Runtime.InteropServices;
using Trinity.Diagnostics;

namespace Trinity.FFI
{
    public static class FFIMethods
    {
        private static TRINITY_INTERFACES s_interfaces = new TRINITY_INTERFACES
        {
            async_registry = TrinityWrapper.trinity_ffi_async_registry,
            async_send = TrinityWrapper.trinity_ffi_async_send,
            cell_appendfield = TrinityWrapper.trinity_ffi_cell_append,
            cell_getfield = TrinityWrapper.trinity_ffi_cell_get,
            cell_getid = TrinityWrapper.trinity_ffi_cell_getid,
            cell_dispose = TrinityWrapper.trinity_ffi_cell_dispose,
            cell_fieldenum_get = TrinityWrapper.trinity_ffi_cellenum_get,
            cell_fieldenum_movenext = TrinityWrapper.trinity_ffi_cellenum_movenext,
            cell_fieldenum_current = TrinityWrapper.trinity_ffi_cellenum_current,
            cell_fieldenum_dispose = TrinityWrapper.trinity_ffi_cellenum_dispose,
            cell_fieldinfo_name = TrinityWrapper.trinity_ffi_fieldinfo_name,
            cell_hasfield = TrinityWrapper.trinity_ffi_cell_has,
            cell_removefield = TrinityWrapper.trinity_ffi_cell_delete,
            cell_setfield = TrinityWrapper.trinity_ffi_cell_set,
            cell_setid = TrinityWrapper.trinity_ffi_cell_setid,
            cell_tostring = TrinityWrapper.trinity_ffi_cell_tostring,
            cloud_loadcell = TrinityWrapper.trinity_ffi_cloud_loadcell,
            cloud_removecell = TrinityWrapper.trinity_ffi_cloud_removecell,
            cloud_savecell = TrinityWrapper.trinity_ffi_cloud_savecell,
            local_loadcell = TrinityWrapper.trinity_ffi_local_loadcell,
            local_removecell = TrinityWrapper.trinity_ffi_local_removecell,
            local_savecell_1 = TrinityWrapper.trinity_ffi_local_savecell_1,
            local_savecell_2 = TrinityWrapper.trinity_ffi_local_savecell_2,
            newcell_1 = TrinityWrapper.trinity_ffi_newcell_1,
            newcell_2 = TrinityWrapper.trinity_ffi_newcell_2,
            newcell_3 = TrinityWrapper.trinity_ffi_newcell_3,
            sync_registry = TrinityWrapper.trinity_ffi_sync_registry,
            sync_send = TrinityWrapper.trinity_ffi_sync_send,
        };

        public static void Initialize()
        {
            Log.WriteLine(LogLevel.Info, $"{nameof(FFIMethods)}: Initializing FFI methods");
            Native.TRINITY_FFI_SET_INTERFACES(ref s_interfaces);
        }
    }
}