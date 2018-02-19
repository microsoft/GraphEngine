import os

__all__ = ["GraphMachine"]


class CellAccessorOptions:
    ThrowExceptionOnCellNotFound = 1
    ReturnNullOnCellNotFound = 2
    CreateNewOnCellNotFound = 4
    StrongLogAhead = 8
    WeakLogAhead = 16


class GraphMachine:
    trinity = None
    env = None
    config = None
    storage = None
    version_num = None
    global_cell_manager = None
    initialized = False

    def __new__(cls, storage_root, **configurations):
        from GraphEngine.configure import Settings
        from GraphEngine.ffi import Trinity

        Settings.storage_root = os.path.abspath(storage_root)
        for k, v in configurations.items():
            if k in Settings.spec:
                setattr(Settings, k, v)
        Settings.configure()
        GraphMachine.trinity = Trinity
        agent = GraphMachine.trinity.FFI.Agent
        GraphMachine.env = agent.Environment
        GraphMachine.config = agent.Config
        GraphMachine.storage = agent.Storage
        GraphMachine.version_num = 0
        return GraphMachine

    @staticmethod
    def start():
        print('Initializing Graph Machine...')
        GraphMachine.trinity.Global.LocalStorage.LoadStorage()
        GraphMachine.config.Initialize()
        GraphMachine.initialized = True
        print('Graph Machine Initialized!')

    @staticmethod
    def end(save_storage=False):
        print('Exiting Graph Machine...')
        if save_storage:
            print('Saving storage...')
            GraphMachine.trinity.Global.LocalStorage.SaveStorage()
            GraphMachine.config.Uninitialize()
            GraphMachine.initialized = False
        print('GraphEngine Exited.')

    @staticmethod
    def default_module():
        if GraphMachine.global_cell_manager is None:
            from GraphEngine.Storage.cache_manager import CellManager
            GraphMachine.global_cell_manager = CellManager()
        return GraphMachine.global_cell_manager
