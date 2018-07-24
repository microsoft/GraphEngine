/***********************************

  Auto-generated from FFIMethods.tt

 ***********************************/

using Trinity.Diagnostics;
using Trinity.FFI.Interop;

namespace Trinity.FFI
{
    public static class FFIMethods
    {
        private static TRINITY_INTERFACES s_interfaces = new TRINITY_INTERFACES
        {
            jitSwigGen = JitTools.SwigGen,
            accessor_use_1 = Accessor.use_1,
            accessor_use_2 = Accessor.use_2,
            accessor_use_3 = Accessor.use_3,
            cell_new_1 = Cell.new_1,
            cell_new_2 = Cell.new_2,
            cell_new_3 = Cell.new_3,
            cell_tostring = Cell.tostring,
            cell_tobinary = Cell.tobinary,
            cell_getid = Cell.getid,
            cell_setid = Cell.setid,
            cell_get = Cell.get,
            cell_has = Cell.has,
            cell_set = Cell.set,
            cell_append = Cell.append,
            cell_delete = Cell.delete,
            cloud_loadcell = CloudStorage.loadcell,
            cloud_savecell = CloudStorage.savecell,
            enum_next = Enumeration.next,
            gc_free = GC.free,
            gc_dispose = GC.dispose,
            local_loadcell = LocalStorage.loadcell,
            local_savecell_1 = LocalStorage.savecell_1,
            local_savecell_2 = LocalStorage.savecell_2,
            local_savecell_3 = LocalStorage.savecell_3,
            local_savecell_4 = LocalStorage.savecell_4,
            local_removecell = LocalStorage.removecell,
            local_savestorage = LocalStorage.savestorage,
            local_loadstorage = LocalStorage.loadstorage,
            local_resetstorage = LocalStorage.resetstorage,
            rt_getfunction = Runtime.getfunction,
            schema_get = Schema.get,
        };

        public static void Initialize()
        {
            Log.WriteLine(LogLevel.Info, $"{nameof(FFIMethods)}: Initializing FFI methods");
            Native.TRINITY_FFI_SET_INTERFACES(ref s_interfaces);
        }
    }
}