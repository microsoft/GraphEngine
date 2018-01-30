from functools import reduce
import platform, os
from cytoolz.curried import curry

Undef = None
is_windows = platform.platform().lower().startswith('windows')

# TODO: linux build scripts
BUILD_SCRIPT_PATH = os.path.abspath('../build.ps1' if is_windows else Undef)
BUILD_SCRIPT_CMD = 'Powershell.exe -File' if is_windows else Undef
CURRENT_DIR = os.path.abspath('./')
CORECLR_PATH = os.path.abspath('../../../../bin/coreclr')


def postfix(a, b):
    return b(a)


def and_then(*f):
    def apply(x):
        return reduce(postfix, f, x)

    return apply


def flatten(recur_collections, recursive_collection_types=(list, tuple)):
    def _flat(recur):
        for e in recur:
            if e.__class__ in recursive_collection_types:
                yield from _flat(e)
            else:
                yield e

    return _flat(recur_collections)


def pardir_of(dir):
    return os.path.join(dir, '..')


@curry
def copy_to(_to, _from):
    """shutils sucks, it failed frequently in *NIX OS.
    """
    _to = os.path.join(_to, os.path.split(_from)[-1])
    print(_from, '->', _to)
    with open(_from, 'rb') as _from_file, open(_to, 'wb') as _to_file:
        and_then(
            _from_file.read,
            _to_file.write
        )(None)


def recur_listdir(dir):
    filenames = os.listdir(dir)
    for filename in map(lambda _: os.path.join(dir, _), filenames):
        if os.path.isdir(filename):
            yield from recur_listdir(filename)
        else:
            yield filename
