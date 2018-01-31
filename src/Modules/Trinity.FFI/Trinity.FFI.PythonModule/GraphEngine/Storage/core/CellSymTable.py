# -*- coding: utf-8 -*-
"""
Created on Sun Jan 28 20:39:38 2018

@author: yatli/thautwarm
"""

from collections import namedtuple
from time import ctime

__all__ = ['CellType', 'SymTableConstructor', 'sync']
CellType = namedtuple('CellType', ['name', 'attrs'])


# CellType.attrs : Dict[str, type]


class Symtable:
    __slots__ = ['_context', 'version']

    def __init__(self, version_thunk=ctime):
        self.version = version_thunk()
        self._context = []

    def __getattr__(self, name):
        """TODO : 
            we need extensions to do auto-completion 
            for dynamically loaded symtable.
        """
        return self._context[name]


class SymTableConstructor:
    __inst__ = None

    def __new__(cls, version_thunk):
        if cls.__inst__ is not None:
            return cls.__inst__
        cls.__inst__ = Symtable(version_thunk)
        return cls


def sync(path):
    """
    syncronize the symtable of cell types from .NET Core hosting
    """
    pass
