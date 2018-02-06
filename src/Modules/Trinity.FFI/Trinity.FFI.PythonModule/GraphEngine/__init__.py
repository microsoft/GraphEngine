__all__ = ["GraphMachine"]


class CellAccessorOptions:
    ThrowExceptionOnCellNotFound = 1
    ReturnNullOnCellNotFound = 2
    CreateNewOnCellNotFound = 4
    StrongLogAhead = 8
    WeakLogAhead = 16


class _GraphMachine:
    def __init__(self, storage_root, **configurations):
        from GraphEngine.configure import Settings
        import GraphEngine.ffi

        Settings.storage_root = storage_root
        for k, v in configurations.items():
            if k in Settings.spec:
                setattr(Settings, k, v)
        Settings.configure()
        self.trinity = __import__('Trinity')
        self.agent = self.trinity.FFI.Agent
        self.cache_manager = self.trinity.FFI.CacheStorageManager(False)
        self.version_num = 0

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

    def delete(self, cell_idx):
        return self.cache_manager.Del(cell_idx)

    def dispose(self):
        self.cache_manager.Dispose()


class GraphMachine:
    inst: _GraphMachine = None

    @classmethod
    def __new__(cls, storage_root, **configurations):
        GraphMachine.inst = _GraphMachine(storage_root, **configurations)
        return GraphMachine.inst


def get_machine():
    return GraphMachine.inst
