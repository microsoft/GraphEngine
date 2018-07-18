import sys
from typing import Mapping, TypeVar
import operator
from functools import reduce as _reduce
from collections import OrderedDict
import typing

def hash_from_stream(stream):
    return _reduce(operator.xor, stream)


K = TypeVar('K')
V = TypeVar('V')

_PY36 = sys.version_info[:2] >= (3, 6)


def raise_exc(e):
    raise e


def binding(cls):
    def call(_):
        return cls

    return call


_dict = dict if _PY36 else OrderedDict


class FrozenDict(Mapping[K, V]):

    def __init__(self, args: typing.Iterable[typing.Tuple[K, V]]):
        self._ = _dict(sorted(args))
        self.len = len(self._)
        self._hash = None

    def __getitem__(self, item):
        return self._[item]

    def __len__(self):
        return self.len

    def __contains__(self, item: K):
        return item in self._

    def __hash__(self):
        if self._hash is None:
            self._hash = hash_from_stream(map(hash, self._.items()))
        return self._hash

    def __eq__(self, other: 'FrozenDict'):
        if other.__class__ is not FrozenDict:
            return False
        if hash(self) != hash(other):
            return False

        return all(map(operator.eq, self.items(), other.items()))

    def __iter__(self):
        return iter(self._)

    def items(self):
        return self._.items()

    # noinspection PyMethodOverriding
    def get(self, k: K, default: V):
        return self._.get(k, V)

    def copy(self):
        return self._.copy()

    def __str__(self):
        return str(self._)


ImmutableDict = FrozenDict
