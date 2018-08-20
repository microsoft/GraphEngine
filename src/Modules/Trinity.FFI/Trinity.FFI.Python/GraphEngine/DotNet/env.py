from Redy.Tools.PathLib import Path
from Redy.Magic.Classic import singleton, cast
from Redy.Tools.TypeInterface import Module
from subprocess import call
from time import time
import getpass, sys


@singleton
class Env:
    nuget_root = Path('~/.nuget/packages')
    graph_engine_config_path = Path("~", ".graphengine")
    ffi: Module

    #  TODO is_netstandard
    @staticmethod
    def target_framework(name: str) -> bool:
        return name.startswith('netstandard')

    @property
    @cast(str)
    def include_path(self):
        return self.nuget_root.into('graphengine.ffi/2.0.9328/content/include')

    @property
    @cast(str)
    def lib_path(self):
        platform = 'win-x64' #TODO
        #  note, linux links to .so, no .lib needed
        #  in the setup invocation, we specify lib name only
        return self.nuget_root.into(f'graphengine.ffi/2.0.9328/runtime/{platform}/native')

    @property
    def current_offset(self):
        return self.Trinity.Storage.Composite.CompositeStorage.CurrentCellTypeOffset


Env: Env


def build_module(tsl_code, namespace: str):
    new_path = Env.graph_engine_config_path.into("versions/{}.{}".format(namespace, str(time()).replace('.', '')))

    new_path.mkdir()

    directory = str(new_path)

    # tsl gen
    with open(str(new_path.into('def.tsl')), 'w') as tsl_file:
        tsl_file.write(tsl_code)

    # swig gen
    swig_code = Env.ffi.Jit_SwigGenInProcess(directory.encode(), namespace.encode())

    swig_interface_filename = str(new_path.into('{namespace}.i'.format(namespace=namespace)))
    with open(swig_interface_filename, 'wb') as swig_file:
        swig_file.write(swig_code)

    # swig build
    call([
        "swig",
        "-modern",
        "-c++",
        "-builtin",
        "-python",
        "-outdir",
        directory,
        "-o",
        "{}/{}_wrap.cxx".format(directory, namespace),
        str(swig_interface_filename)])

    # py setup
    py_setup_file = new_path.into('setup.py')

    with open(str(py_setup_file), 'w') as py_file:
        py_file.write(
            "from setuptools import setup, find_packages\n"
            "from setuptools.extension import Extension\n"
            f"ext = Extension('_{namespace}',\n"
            f"                  sources=[r'{directory}/{namespace}_wrap.cxx'],\n"
            f"                  include_dirs=[r'{Env.include_path}'],\n"
            "                   libraries=['trinity_ffi', 'Trinity'],\n"
            f"                  library_dirs=[r'{Env.lib_path}'])\n"
            "\n"
            f"setup(name='{namespace}',\n"
            "ext_modules=[ext],\n"
            "version = '1.0',\n"
            f"author = \"{getpass.getuser()}\",\n"
            f"py_modules = [\"{namespace}\"])\n")

    # pyd gen

    call(['python', str(py_setup_file), 'build', '--build-lib', directory])

    sys.path.append(directory)
    module = __import__(namespace)
    sys.path.remove(directory)
    return module
