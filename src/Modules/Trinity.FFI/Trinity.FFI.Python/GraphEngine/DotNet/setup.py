from .dep import Dependency
from .lib import Library
from .env import Env, build_module
from ..utils import file_hash
from bs4 import BeautifulSoup
from Redy.Tools.PathLib import Path
from Redy.Collections import Traversal
from Redy.Magic.Classic import singleton
from Redy.Tools.TypeInterface import Module
from subprocess import call
import ctypes, sys
import typing
from linq import Flow as seq
from toolz import curry, compose


@curry(setattr, Path, '__hash__')
def __hash__(self: Path):
    return hash(self._path)


try:
    from Redy.Tools.Color import Green, Red
except ModuleNotFoundError:
    class _Colored:
        Red = '\033[31m'
        Green = '\033[32m'
        Yellow = '\033[33m'
        Blue = '\033[34m'
        Purple = '\033[35m'
        LightBlue = '\033[36m'
        Clear = '\033[39m'
        Purple2 = '\033[95m'


    def _wrap_color(colored: str):
        def func(*strings: str, sep=''):
            strings = map(lambda each: f'{colored}{each}', strings)
            return f'{sep.join(strings)}{_Colored.Clear}'

        return func


    Red = _wrap_color(_Colored.Red)
    Green = _wrap_color(_Colored.Green)


@singleton
class Collect:
    def __init__(self):
        self.fn = Traversal.flatten_to(Path)

    def __call__(self, collection):
        return self.fn(collection)

    def __ror__(self, other):
        return self(other)


@singleton
class FilterDLL:
    def __init__(self):
        def is_dll(x: Path):
            return x.relative().lower().endswith('.dll')

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
    target_cs_proj_file = cs_proj_build_dir.into('Dependencies.csproj')
    src_cs_proj_file = module_dir.into("Dependencies.csproj")

    if not cs_proj_build_dir.exists() or not target_cs_proj_file.exists() or file_hash(src_cs_proj_file) != file_hash(
            target_cs_proj_file):
        cs_proj_build_dir.mkdir(warning=False)
        src_cs_proj_file.move_to(cs_proj_build_dir)

    cmd_patterns = ["dotnet", "restore", f'"{target_cs_proj_file}"', '--packages', f'"{Env.nuget_root}"']

    # restore
    if call(cmd_patterns):
        raise EnvironmentError("DotNet restoring failed `{}`".format(' '.join(cmd_patterns).__repr__()))

    # search dlls
    with target_cs_proj_file.open('rb') as file:
        deps = [Dependency(package_name=ref.attrs['include'], version=ref.attrs["version"]).all() for ref in
                BeautifulSoup(file, "lxml").select('packagereference')] | Collect | FilterDLL

    libs = [Library('GraphEngine.{}'.format(module), version='2.0.9328', where='runtimes/win-x64/native').all() for
            module in ['Core', 'FFI', 'Jit']] | Collect | FilterDLL

    sys.path.append(str(module_dir.parent()))

    # TODO: optimize loading
    paths_of_libs: seq[typing.Set[Path]] = seq(libs).concat(deps).to_set()
    path_strs_of_libs: typing.Set[str] = paths_of_libs.map(str).to_set()._

    while path_strs_of_libs:
        any_loaded = None

        for each in path_strs_of_libs:
            try:
                ctypes.cdll.LoadLibrary(each)
                any_loaded = each
                print(f'Loading {Green(each)}...')
                break
            except (OSError, ImportError):
                continue

        if any_loaded:
            path_strs_of_libs.remove(any_loaded)
            continue

        print('Cannot load dlls: {}'.format(Red(repr(path_strs_of_libs))))
        break

    # noinspection PyProtectedMember
    dirs = paths_of_libs.map(Path.parent).map(str).to_set().to_list()._
    __ffi = __import__('ffi')
    p1 = str(graph_engine_config_path.into("trinity.xml")).encode('ascii')
    p2 = str(graph_engine_config_path.into('storage')).encode('ascii', 'surrogateescape')

    print(p1, p2)
    __ffi.InitCLR(len(dirs), dirs, p1, p2)
    __ffi.InitInterfaces()

    sys.path.remove(str(module_dir.parent()))
    Env.ffi = __ffi
