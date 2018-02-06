# -*- coding: utf-8 -*-
"""
Created on Sun Jan 28 20:39:38 2018

@author: yatli/thautwarm
"""

import GraphEngine as ge
from GraphEngine.TSL.TypeMap import TSLTypeConstructor
from GraphEngine.TSL.tsl_type_parser import token, TSLTypeParse, MetaInfo
from time import ctime
from linq import Flow
import datetime
from collections import namedtuple
import json
import warnings
from typing import Dict

CellType = namedtuple('CellType', ['name', 'fields'])

__all__ = ['SymTable']


def parse_type(type_name):
    type_ast = TSLTypeParse(token(type_name), MetaInfo())
    return TSLTypeConstructor(type_ast)


# CellType.attrs : Dict[str, type]

class _SymTable:
    """
    Record the types of cells
    """
    __slots__ = ['_context', 'version']
    format_indent = 4

    def __init__(self, version_thunk):
        self.version = version_thunk()
        self._context: Dict[str, CellType] = {}

    def __getattr__(self, name):
        """
        TODO :
        we need extensions to do auto-completion
        for dynamically loaded symtable.
        """
        return self._context[name]

    def __setitem__(self, key, value):
        self._context[key] = value

    def __getitem__(self, item):
        return self._context[item]

    def __str__(self):
        return "VERSION[{}]:\n{}".format(self.version, json.dumps(self._context, indent=_SymTable.format_indent))

    def __repr__(self):
        return self.__str__()


class SymTable:
    __inst__: _SymTable = None

    def __new__(cls, version_thunk=ctime):
        if cls.__inst__ is not None:
            return cls.__inst__
        cls.__inst__ = _SymTable(version_thunk)
        return cls.__inst__

    @staticmethod
    def get(item):
        return SymTable.__inst__.__getitem__(item)


def load_symbols(tsl_src_dir, tsl_build_dir=None, module_name=None, version_name=None, sync=True):
    """"
    :param tsl_src_dir: str
    :param tsl_build_dir: str
    :param module_name: str
    :param version_name: str
    :param sync: boolean, syncronize symbols with Python
    :return: None
    """
    gm = ge.get_machine()
    if gm is None:
        raise EnvironmentError("GraphMachine wasn't initialized, "
                               "use `ge.GraphMachine(storage_root: str) to start service.`")
    if module_name is None:
        warnings.warn("It's highly recommended to name the module for logging and so on.")
        module_name = "Default{}".format(gm.agent.get_VersionRecorders().Count())

    if tsl_build_dir is None:
        tsl_build_dir = tsl_src_dir

    if sync:
        symtable = SymTable()  # singleton symtable

        gm.agent.LoadTSL(tsl_src_dir, tsl_build_dir, module_name, version_name)

        schemas = gm.agent.get_StorageSchema()
        for i in range(gm.version_num, len(list(gm.agent.get_VersionRecorders()))):
            for cell_type in schemas[i].get_CellDescriptors():
                symtable[cell_type.TypeName] = \
                    CellType(cell_type.TypeName,
                             {field.Name: parse_type(field.TypeName)
                              for field in cell_type.GetFieldDescriptors()})

    """
    TODO:
    Create new cell types from TSL source directory.
    """
