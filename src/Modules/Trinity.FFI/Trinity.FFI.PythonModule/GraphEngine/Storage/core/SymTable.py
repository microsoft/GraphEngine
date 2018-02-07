# -*- coding: utf-8 -*-
"""
Created on Sun Jan 28 20:39:38 2018

@author: yatli/thautwarm
"""
import os

import GraphEngine as ge
from GraphEngine.TSL.TypeMap import TSLTypeConstructor
from GraphEngine.TSL.tsl_type_parser import token, TSLTypeParse, MetaInfo
from time import ctime
from collections import namedtuple
import json
import warnings
from typing import Dict, Any
from GraphEngine.TSL.TypeMap import FieldType
from GraphEngine.configure import PY3

__all__ = ['SymTable', 'CellType']
gm = ge.get_machine()


class CellType:
    __slots__ = ['name', 'fields', 'default']
    name: str
    fields: Dict[str, FieldType]
    default: Dict[str, Any]

    def __init__(self, name: str, fields: Dict[str, FieldType]):
        self.name = name
        self.fields = fields
        self.default = {k: v.default for k, v in fields.items() if not v.is_optional}

    def __str__(self):
        return '{}{{\n{}\n}}'.format(self.name,
                                     '\n'.join(
                                         "\t{}: {},".format(field_name, field_type.sig)
                                         for field_name, field_type in self.fields.items()))

    def __repr__(self):
        return self.__str__()


def parse_type(type_sig_string) -> type:
    type_ast = TSLTypeParse(token(type_sig_string), MetaInfo())
    return TSLTypeConstructor(type_ast)


# CellType.fields : Dict[str, type]

class _SymTable:
    """
    Record the types of cells
    """
    __slots__ = ['_context', 'version']

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
        return "VERSION[{}]:\n{}".format(self.version, json.dumps(
            {name: {field: filed_typ.sig for field, filed_typ in cell_typ.fields.items()}
             for name, cell_typ in self._context.items()}, indent=SymTable.format_indent))

    def __repr__(self):
        return self.__str__()


class SymTable:
    inst: _SymTable = None
    format_indent = 4

    def __new__(cls, version_thunk=ctime):
        if cls.inst is not None:
            return cls.inst
        cls.inst = _SymTable(version_thunk)
        return cls.inst

    @staticmethod
    def get(item):
        return SymTable.inst.__getitem__(item)

    @staticmethod
    def set_format_indent(i=4):
        SymTable.format_indent = i


def load_symbols(tsl_src_dir, tsl_build_dir=None, module_name=None, version_name=None, do_sync=True):
    """"
    :param tsl_src_dir: str
    :param tsl_build_dir: str
    :param module_name: str
    :param version_name: str
    :param do_sync: boolean, syncronize symbols with Python
    :return: None
    """
    if gm is None:
        raise EnvironmentError("GraphMachine wasn't initialized, "
                               "use `ge.GraphMachine(storage_root: str) to start service.`")

    if module_name is None:
        warnings.warn("It's highly recommended to name the module for logging and so on.")
        module_name = "Default{}".format(gm.version_num)

    tsl_src_dir = os.path.abspath(tsl_src_dir)
    tsl_build_dir = tsl_src_dir if tsl_build_dir is None else os.path.abspath(tsl_build_dir)

    gm.agent.LoadTSL(tsl_src_dir, tsl_build_dir, module_name, version_name)
    if do_sync:
        sync()

    """
    TODO:
    Create new cell types from TSL source directory.
    """


def sync():
    symtable = SymTable()  # singleton symtable

    schemas = gm.agent.get_StorageSchema()

    recorders = gm.agent.get_VersionRecorders()
    if not recorders:
        return

    new_version_num = len(list(gm.agent.get_VersionRecorders()))

    for i in range(gm.version_num, new_version_num):

        for cell_type in schemas[i].get_CellDescriptors():
            symtable[cell_type.TypeName] = \
                CellType(cell_type.TypeName, {field.Name: parse_type(field.TypeName)
                                              for field in cell_type.GetFieldDescriptors()})

    gm.version_num = new_version_num
