# -*- coding: utf-8 -*-
"""
Created on Sun Jan 28 20:39:38 2018

@author: yatli/thautwarm
"""

from collections import namedtuple
from time import ctime
import json

__all__ = ['CellType', 'SymTableConstructor', 'sync']
CellType = namedtuple('CellType', ['name', 'attrs'])


# CellType.attrs : Dict[str, type]


class SymTable:
    __slots__ = ['_context', 'version']

    def __init__(self, version_thunk):
        self.version = version_thunk()
        self._context = {'C1': CellType('C1',
                                        {'foo': int,
                                         'baz': int,
                                         'bar': str})}

    def __getattr__(self, name):
        """TODO :
            we need extensions to do auto-completion
            for dynamically loaded symtable.
        """
        return self._context[name]


class SymTableConstructor:
    __inst__ = None

    def __new__(cls, version_thunk=ctime):
        if cls.__inst__ is not None:
            return cls.__inst__
        cls.__inst__ = SymTable(version_thunk)
        return cls.__inst__


def sync():
    """
    syncronize the symtable of cell types from .NET Core hosting
    """
    if SymTableConstructor.__inst__ is None:
        SymTableConstructor(ctime)
    else:
        outdate = SymTableConstructor.__inst__
        SymTableConstructor.__inst__ = SymTable(ctime)
        SymTableConstructor.__inst__._context.update(outdate._context)

    from TslAssembly import SymTable as sym
    SymTableConstructor.__inst__._context.update({k: json.loads(v) for k, v in sym.Content})
