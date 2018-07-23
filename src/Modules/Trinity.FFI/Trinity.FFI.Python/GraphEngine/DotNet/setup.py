from .dep import Dependency
from .lib import Library
from .env import Env, build_module
from bs4 import BeautifulSoup
from Redy.Tools.PathLib import Path
from Redy.Collections import Traversal
from Redy.Magic.Classic import singleton
from Redy.Tools.TypeInterface import Module
from subprocess import call
import ctypes, sys
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
    graph_engine_config_path = Env.graph_engine_config_path
    cs_proj_build_dir = graph_engine_config_path.into('Dependencies')

    if not cs_proj_build_dir.exists() or not cs_proj_build_dir.into("Dependencies.csproj").exists():

        cs_proj_build_dir.mkdir(warning=False)
        cs_proj_file = module_dir.into("Dependencies.csproj")
        cs_proj_file.move_to(cs_proj_build_dir)

    cs_proj_file = cs_proj_build_dir.into('Dependencies.csproj')
    cmd_patterns = ["dotnet", "restore", f'"{cs_proj_build_dir}"', '--packages', f'"{Env.nuget_root}"']

    # restore
    if call(cmd_patterns):
        raise EnvironmentError("DotNet restoring failed `{}`".format(' '.join(cmd_patterns).__repr__()))

    # search dlls
    with cs_proj_file.open('rb') as file:
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

    libs = set(libs)
    from rbnf.Color import Red, Green
    while libs:
        for each in libs.copy():
            try:
                ctypes.cdll.LoadLibrary(each)
                print(Green(f'load dll succeed: {each}.'))
                libs.remove(each)
                break
            except WindowsError:
                print(f'load dll failed, reorder import path {Red(each)}.')
                pass
    #
    # for each_lib in libs:
    #     print(each_lib)
    #     ctypes.cdll.LoadLibrary(each_lib)

    dirs = list({Path(each).parent().__str__() for each in deps})

    #for each_dep in deps:
        #clr.AddReference(each_dep)


    # TODO
    #__Trinity.TrinityConfig.StorageRoot = str(graph_engine_config_path.into('storage'))

    # TODO
    #__Trinity.TrinityConfig.LoadConfig(str(graph_engine_config_path.into("trinity.xml")))

    __ffi = __import__('ffi')
    __ffi.InitCLR(len(dirs), dirs, str(graph_engine_config_path.into("trinity.xml")),
                  str(graph_engine_config_path.into('storage')))
    Env.ffi = __ffi

