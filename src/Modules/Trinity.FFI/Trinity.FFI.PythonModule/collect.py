"""
Collect assembly sources from GraphEngine project here and there.
"""

import os
import linq
import argparse

from conf import and_then
from conf import (CURRENT_DIR, CORECLR_PATH,
                  BUILD_SCRIPT_CMD, BUILD_SCRIPT_PATH, is_windows)
from conf import copy_to, recur_listdir, pardir_of

cmdparser = argparse.ArgumentParser(description='collect binaries.')
cmdparser.add_argument("--nocache",
                       help='use cache or not.',
                       default=False, nargs='?',
                       const=True)


def dir_at(*args, create_dir=True, then_call=None):
    """
    join the path and try to create the directory.
    """
    path = os.path.join(*args)
    if create_dir and not os.path.exists(path):
        try:
            os.mkdir(path)
        except FileExistsError:
            pass
    if then_call is not None:
        return then_call(path)
    return path


if is_windows:
    # build dlls
    if cmdparser.parse_args().nocache:
        os.system('{} "{}"'.format(BUILD_SCRIPT_CMD, BUILD_SCRIPT_PATH))

    to_pymodule_dir = lambda path, with_suffix, to_module: (
        linq.Flow(path)
            .Then(recur_listdir)
            .Filter(lambda _: any(_.endswith(suffix) for suffix in with_suffix))
            .Each(dir_at(CURRENT_DIR, 'GraphEngine', to_module,
                         then_call=copy_to)))
    # copy dlls

    to_pymodule_dir(CORECLR_PATH,
                    with_suffix=['.py', '.dll', '.pyd'],
                    to_module='ffi')

    # build Trinity.StorageVersionController.csproj
    dir_at(pardir_of(pardir_of(BUILD_SCRIPT_PATH)), 'CompositeStorageExtension',
           then_call=lambda _: os.system('cd {} && dotnet restore && dotnet build'.format(_)))

    to_pymodule_dir('../../CompositeStorageExtension',
                    with_suffix=['.dll'],
                    to_module='ffi')

    # copy exe
    to_pymodule_dir(pardir_of(CORECLR_PATH),
                    with_suffix=['.exe'],
                    to_module='Command')

    # add newtonsoft.json.dll
    # TODO: a robust way to find out newtonsoft.json.dll.
    to_pymodule_dir(r'C:\Program Files\dotnet\sdk\NuGetFallbackFolder\newtonsoft.json\9.0.1\lib\netstandard1.0',
                    with_suffix=['.dll'],
                    to_module='ffi')

    # deprecated
    # build TslAssembly.csproj
    # dir_at(pardir_of(BUILD_SCRIPT_PATH), 'TslAssembly',
    #        then_call=lambda _: os.system('cd {} && dotnet restore && dotnet build'.format(_)))
    #
    # to_pymodule_dir('../TslAssembly',
    #                 with_suffix=['.dll'],
    #                 to_module='ffi')


else:
    raise NotImplemented
