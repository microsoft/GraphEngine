# -*- coding: utf-8 -*-
"""
Created on Sun Jan 28 20:39:38 2018

@author: yatli/thautwarm
"""

from time import ctime
from linq import Flow
import json
DynamicLoading = __import__('DynamicLoading')


__all__ = ['SymTablePicker']


# CellType.attrs : Dict[str, type]

class SymTable:
    __slots__ = ['_context', 'version']
    format_indent = 4

    def __init__(self, version_thunk):
        self.version = version_thunk()
        self._context = {}

    def __getattr__(self, name):
        """
        TODO :
        we need extensions to do auto-completion
        for dynamically loaded symtable.
        """
        return self._context[name]

    def __str__(self):
        return "VERSION[{}]:\n{}".format(self.version, json.dumps(self._context, indent=SymTable.format_indent))

    def __repr__(self):
        return self.__str__()


class SymTablePicker:
    __inst__ = None

    def __new__(cls, version_thunk=ctime):
        if cls.__inst__ is not None:
            return cls.__inst__
        cls.__inst__ = SymTable(version_thunk)
        return cls.__inst__


def load_symbols(tsl_dir):
    """
    TODO:
    Create new cell types from TSL source directory.
    """
    DynamicLoading.Center.LoadFrom
