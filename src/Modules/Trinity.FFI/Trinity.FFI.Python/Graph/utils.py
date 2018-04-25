import sys
from typing import Mapping, TypeVar
from collections import namedtuple

K = TypeVar('K')
V = TypeVar('V')

_PY36 = sys.version_info[:2] >= (3, 6)


def raise_exc(e):
    raise e


class Record:
    """
    doctest
    >>> @Record
    >>> class S:
    >>>        a: int
    >>> s = S(1)
    >>> print(s)

    >>> try:
    >>>  s = S("1")
    >>> except TypeError:
    >>>     pass
    """

    def __new__(cls, cls_def: type):
        typ: type = namedtuple(cls_def.__name__, list(cls_def.__annotations__.keys()))
        return type(cls_def.__name__,
                    (*cls_def.__bases__, typ),
                    dict(cls_def.__dict__))


def binding(cls):
    def call(_):
        return cls

    return call


class ImmutableDict(dict, Mapping[K, V]):

    def __hash__(self):
        return hash(tuple(sorted(self.items())))
