/***********************************

  Auto-generated from FFIMethods.tt

 ***********************************/

using Trinity.Diagnostics;

namespace Trinity.FFI
{
    public static class FFIMethods
    {
        private static TRINITY_INTERFACES s_interfaces = new TRINITY_INTERFACES
        {
			sync_registry = TrinityWrapper.sync_registry,
			async_registry = TrinityWrapper.async_registry,
			sync_send = TrinityWrapper.sync_send,
			async_send = TrinityWrapper.async_send,
			local_loadcell = TrinityWrapper.local_loadcell,
			cloud_loadcell = TrinityWrapper.cloud_loadcell,
			cell_fieldenum_movenext = TrinityWrapper.cell_fieldenum_movenext,
			cell_fieldinfo_name = TrinityWrapper.cell_fieldinfo_name,
			cell_fieldenum_current = TrinityWrapper.cell_fieldenum_current,
			cell_fieldenum_dispose = TrinityWrapper.cell_fieldenum_dispose,
			cell_fieldenum_get = TrinityWrapper.cell_fieldenum_get,
			local_savecell_1 = TrinityWrapper.local_savecell_1,
			local_savecell_2 = TrinityWrapper.local_savecell_2,
			local_savecell_3 = TrinityWrapper.local_savecell_3,
			local_savecell_4 = TrinityWrapper.local_savecell_4,
			cloud_savecell = TrinityWrapper.cloud_savecell,
			local_removecell = TrinityWrapper.local_removecell,
			newcell_1 = TrinityWrapper.newcell_1,
			newcell_2 = TrinityWrapper.newcell_2,
			newcell_3 = TrinityWrapper.newcell_3,
			local_usecell_1 = TrinityWrapper.local_usecell_1,
			local_usecell_2 = TrinityWrapper.local_usecell_2,
			local_usecell_3 = TrinityWrapper.local_usecell_3,
			cell_dispose = TrinityWrapper.cell_dispose,
			cell_tostring = TrinityWrapper.cell_tostring,
			cell_getid = TrinityWrapper.cell_getid,
			cell_setid = TrinityWrapper.cell_setid,
			cell_get = TrinityWrapper.cell_get,
			cell_has = TrinityWrapper.cell_has,
			cell_set = TrinityWrapper.cell_set,
			cell_append = TrinityWrapper.cell_append,
			cell_delete = TrinityWrapper.cell_delete,
        };

        public static void Initialize()
        {
            Log.WriteLine(LogLevel.Info, $"{nameof(FFIMethods)}: Initializing FFI methods");
            Native.TRINITY_FFI_SET_INTERFACES(ref s_interfaces);
        }
    }
}