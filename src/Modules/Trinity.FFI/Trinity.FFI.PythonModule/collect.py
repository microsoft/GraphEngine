"""
Collect assembly sources from GraphEngine project here and there.
"""
import os
import linq
import platform
from utils import and_then
from cytoolz.curried import curry

Undef = None
is_windows = platform.platform().lower().startswith('windows')

# TODO: linux build scripts
BUILD_SCRIPT_PATH = os.path.abspath('../build.ps1' if is_windows else Undef)
BUILD_SCRIPT_CMD = 'Powershell.exe -File' if is_windows else Undef
CURRENT_DIR = os.path.abspath('./')

CORECLR_PATH = os.path.abspath('../../../../bin/coreclr')


@curry
def copy_to(_to, _from):
    """shutils sucks, it failed frequently in *NIX OS.
    """
    _to = os.path.join(_to, os.path.split(_from)[-1])
    print(_to)
    with open(_from, 'rb') as _from_file, open(_to, 'wb') as _to_file:
        and_then(
            _from_file.read,
            _to_file.write
        )(None)


# build dlls
#os.system('{} "{}"'.format(BUILD_SCRIPT_CMD, BUILD_SCRIPT_PATH))

def recur_listdir(dir):
    filenames = os.listdir(dir)
    for filename in map(lambda _: os.path.join(dir, _), filenames):
        if os.path.isdir(filename):
            yield from recur_listdir(filename)
        else:
            yield filename


# copy dlls
for dll_path in (CORECLR_PATH, '../TslAssembly'): 
    (linq.Flow(dll_path)
     .Then(
        and_then(recur_listdir))
     .Filter(lambda _: _.endswith('.dll') or _.endswith('.pyd') or _.endswith('.py'))
     .Each(copy_to(os.path.join(CURRENT_DIR, 'GraphEngine', 'ffi'))))


# copy exe
(linq.Flow(os.path.join(CORECLR_PATH, '../'))
     .Then(
        and_then(recur_listdir))
     .Filter(lambda _: _.endswith('.exe'))
     .Each(copy_to(os.path.join(CURRENT_DIR, 'GraphEngine', 'Command'))))
