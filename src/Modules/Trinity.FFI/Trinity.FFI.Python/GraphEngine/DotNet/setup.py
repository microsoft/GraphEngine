from .dep import Dependency
from .lib import Library
from .env import Env, build_module
from bs4 import BeautifulSoup
from Redy.Tools.PathLib import Path
from Redy.Collections import Traversal
from Redy.Magic.Classic import singleton
from Redy.Tools.TypeInterface import Module
from subprocess import call
import ctypes, clr, sys
from toolz import compose


@singleton
class Collect:
    def __init__(self):
        self.fn = compose(Traversal.map_by(str), Traversal.flatten_to(Path))

    def __call__(self, collection):
        return self.fn(collection)

    def __ror__(self, other):
        return self(other)


@singleton
class FilterDLL:
    def __init__(self):
        def is_dll(x: str):
            return x.lower().endswith('.dll')

        self.fn = Traversal.filter_by(is_dll)

    def __call__(self, collection):
        return self.fn(collection)

    def __ror__(self, other):
        return self(other)


Collect: Collect


def init_trinity_service() -> Module:
    module_dir = Path(__file__).parent()

    cs_proj_file = module_dir.into("Dependencies.csproj")

    cmd_patterns = ["dotnet", "restore", f'"{cs_proj_file}"', '--packages', f'"{Env.nuget_root}"']

    # restore
    if call(cmd_patterns):
        raise EnvironmentError("DotNet restoring failed `{}`".format(' '.join(cmd_patterns).__repr__()))

    # search dlls
    with open(str(cs_proj_file)) as file:
        deps = [Dependency(package_name=ref.attrs['include'],
                           version=ref.attrs["version"]
                           ).all()
                for ref in
                BeautifulSoup(file, "lxml").select('packagereference')
                ] | Collect | FilterDLL

    libs = [Library('GraphEngine.{}'.format(module),
                    version='2.0.9328',
                    where='runtimes/win-x64/native'
                    ).all()
            for module in
            ['Core', 'FFI', 'Jit']
            ] | Collect | FilterDLL

    sys.path.append(str(module_dir.parent()))

    # TODO: native hosting

    for each_lib in libs:
        ctypes.cdll.LoadLibrary(each_lib)

    for each_dep in deps:
        clr.AddReference(each_dep)

    graph_engine_config_path = Env.graph_engine_config_path
    # TODO
    __Trinity = __import__('Trinity')

    # TODO
    __Trinity.TrinityConfig.StorageRoot = str(graph_engine_config_path.into('storage'))

    # __Trinity.TrinityConfig.LoggingLevel = __Trinity.Diagnostics.LogLevel.Info

    # TODO
    __Trinity.TrinityConfig.LoadConfig(str(graph_engine_config_path.into("trinity.xml")))

    # TODO: hosting
    __Trinity.Global.Initialize()

    __ffi = __import__('ffi')

    __ffi.Init()


    __import__('Trinity.Storage')
    __import__('Trinity.Storage.Composite')
    __import__('Trinity.FFI')
    __import__('Trinity.FFI.Metagen')

    Env.Trinity = __Trinity
    return __Trinity
