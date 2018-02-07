import os

__all__ = ["GraphMachine"]


class CellAccessorOptions:
    ThrowExceptionOnCellNotFound = 1
    ReturnNullOnCellNotFound = 2
    CreateNewOnCellNotFound = 4
    StrongLogAhead = 8
    WeakLogAhead = 16


class GraphMachine:
    inst = None

    def __init__(self, storage_root, **configurations):
        from GraphEngine.configure import Settings
        import GraphEngine.ffi

        Settings.storage_root = os.path.abspath(storage_root)
        for k, v in configurations.items():
            if k in Settings.spec:
                setattr(Settings, k, v)
        Settings.configure()
        self.trinity = __import__('Trinity')
        self.agent = self.trinity.FFI.Agent
        self.cache_manager = self.trinity.FFI.CacheStorageManager(False)
        self.version_num = 0
        GraphMachine.inst = self

    def start(self):
        self.trinity.Global.LocalStorage.LoadStorage()
        self.agent.Initialize()

    def load_cell(self, cell_id):
        """
        cell_id: C# long -> python int
        """
        return self.cache_manager.LoadCell(cell_id)

    def new_cell_by_type(self, cell_type):
        """
        cell_type: str
        """
        return self.cache_manager.NewCellByType(cell_type)

    def new_cell_by_id_type(self, cell_id, cell_type):
        """
        cell_id: C# long -> python int
        cell_type: str
        """
        return self.cache_manager.NewCellByIdType(cell_id, cell_type)

    def new_cell_by_type_content(self, cell_type, cell_content):
        """
        cell_id: C# long -> python int
        cell_content: json-like str
        """
        return self.cache_manager.NewCellByTypeContent(cell_type, cell_content)

    def use_cell_by_id(self, cell_id):
        """
        cell_id: C# long -> python int
        """
        return self.cache_manager.UseCellById(cell_id)

    def use_cell_by_id_ops(self, cell_id, cell_accessor_options):
        return self.cache_manager.UseCellById(cell_id, cell_accessor_options)

    def use_cell_by_id_ops_type(self, cell_id, cell_accessor_options, cell_type):
        return self.cache_manager.UseCellById(cell_id, cell_accessor_options, cell_type)

    def save_cell_by_index(self, cell_idx):
        self.cache_manager.SaveCellByIndex(cell_idx)

    def save_cell_by_id_index(self, cell_id, cell_idx):
        self.cache_manager.SaveCellByIdIndex(cell_id, cell_idx)

    def save_cell_by_ops_idx(self, write_ahead_log_options, cell_idx):
        self.cache_manager.SaveCellByOpsIndex(write_ahead_log_options, cell_idx)

    def save_cell_by_ops_id_idx(self, write_ahead_log_options, cell_id, cell_idx):
        self.cache_manager.SaveCellByOpsIdIndex(write_ahead_log_options, cell_id, cell_idx)

    def cell_get_id_by_idx(self, cell_idx):
        return self.cache_manager.CellGetIdByIndex(cell_idx)

    def cell_get_field(self, cell_idx, field_name):
        return self.cache_manager.CellGetField(cell_idx, field_name)

    def cell_get_fields(self, cell_idx, fields):
        return self.cache_manager.CellGetFields(cell_idx, fields)

    def cell_set_field(self, cell_idx, field_name, value):
        self.cache_manager.CellSetField(cell_idx, field_name, value)

    def cell_set_fields(self, cell_idx, fields, values):
        self.cache_manager.CellSetFields(cell_idx, fields, values)

    def cell_append_field(self, cell_idx, field, content):
        self.cache_manager.CellAppendField(cell_idx, field, content)

    def cell_remove_field(self, cell_idx, field_name):
        self.cache_manager.CellRemoveField(cell_idx, field_name)

    def delete(self, cell_idx):
        return self.cache_manager.Del(cell_idx)

    def dispose(self):
        self.cache_manager.Dispose()
        self.trinity.Global.LocalStorage.SaveStorage()
        self.agent.Uninitialize()


def get_machine():
    return GraphMachine.inst
