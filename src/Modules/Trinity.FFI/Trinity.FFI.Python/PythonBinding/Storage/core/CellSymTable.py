# -*- coding: utf-8 -*-
"""
Created on Sun Jan 28 20:39:38 2018

@author: yatli
"""

from collections import namedtuple
from time import ctime

__all__ = ['CellType', 'SymTable', 'sync']
CellType = namedtuple('CellType', ['name', 'attrs'])
# CellType.attrs : Dict[str, type]


class symtable:
    
    __slots__ = ['_context', 'version']
    
    def __init__(self, version_thunk=ctime):
        self.version = version_thunk()
        self._context = []
        
    def __getattr__(self, name):
        """TODO : 
            we need extensions to do auto-completion 
            for dynamic loaded symtable.
        """
        return self._context[name]


class SymTable:
    __inst__ = None
    def __new__(cls, version_thunk):
        if cls.__inst__ is not None:
            return cls.__inst__
        cls.__inst__ = symtable(version_thunk)
        return cls
    


def sync(path):
    """
    syncronize the symtable of cell types from .NET Core hosting
    """
    pass





