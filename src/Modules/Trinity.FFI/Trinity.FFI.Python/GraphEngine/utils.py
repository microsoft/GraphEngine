import sys
from typing import Mapping, TypeVar
from Redy.Magic.Classic import record

from collections import namedtuple

K = TypeVar('K')
V = TypeVar('V')

_PY36 = sys.version_info[:2] >= (3, 6)


def raise_exc(e):
    raise e


def binding(cls):
    def call(_):
        return cls

    return call


class ImmutableDict(dict, Mapping[K, V]):

    def __hash__(self):
        return hash(tuple(sorted(self.items())))
