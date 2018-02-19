import os
import functools
from collections import deque
import linq
from linq.core.collections import Generator as gen

__all__ = ["GraphMachine"]


class CellAccessorOptions:
    ThrowExceptionOnCellNotFound = 1
    ReturnNullOnCellNotFound = 2
    CreateNewOnCellNotFound = 4
    StrongLogAhead = 8
    WeakLogAhead = 16


class IDAllocator:
    def __init__(self):
        self.residual_ids = set()
        self.id_exceptions = set()
        self.next_fn = [None, next, None]
        self.stream_inf = 0

    def next_trivial(self):
        ret = self.stream_inf
        self.stream_inf += 1
        return ret

    def add_exceptions(self, cell_id):
        if cell_id in self.residual_ids:
            self.residual_ids.remove(cell_id)
        elif cell_id < self.stream_inf:
            return  # no need to add the exception, it has been consumed.
        self.id_exceptions.add(cell_id)
        if self.next_fn[2] is None:
            self.next_fn[2] = self.end

        return cell_id

    def end(self, cell_id):
        if cell_id in self.id_exceptions:
            self.id_exceptions.remove(cell_id)
            ret = self.next()
            if not self.id_exceptions:
                self.next_fn[2] = None
            return ret
        return cell_id

    def dealloc(self, cell_id):
        self.residual_ids.add(cell_id)
        if self.next_fn[0] is None:
            self.next_fn[0] = self.before

    def before(self):
        try:
            left = self.residual_ids.pop()
            return left
        except KeyError:
            self.next_fn[0] = None

    def next(self):
        before, main, end = self.next_fn
        if before:
            ret = before()
        else:
            ret = main()

        if end:
            ret = end(ret)
        return ret


class GraphMachine:
    trinity = None
    env = None
    config = None
    storage = None
    version_num = None
    global_cell_manager = None
    initialized = False
    id_allocator: IDAllocator = None

    def __new__(cls, storage_root, **configurations):
        from GraphEngine.configure import Settings
        from GraphEngine.ffi import Trinity

        Settings.storage_root = os.path.abspath(storage_root)
        for k, v in configurations.items():
            if k in Settings.spec:
                setattr(Settings, k, v)
        Settings.configure()
        id_allocator_path = os.path.join(Settings.storage_root, 'py-id-allocator.bin')
        if os.path.exists(id_allocator_path):
            try:
                with open(id_allocator_path, 'rb') as binary_src:
                    GraphMachine.id_allocator = __import__('dill').load(binary_src)
            except (ModuleNotFoundError if __import__('sys').version_info.minor >= 6 else ImportError):
                raise IOError('Error when loading id allocator binaries!')
                # GraphMachine.id_allocator = IDAllocator()
        else:
            GraphMachine.id_allocator = IDAllocator()

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
