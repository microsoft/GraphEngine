# -*- coding: utf-8 -*-
"""
Created on Sun Jan 28 20:36:19 2018

@author: yatli/thautwarm
"""

import json
from functools import update_wrapper
import GraphEngine as ge
from ._SymTable import SymTable, CellType
from .Serialize import Serializer, TSLJSONEncoder

gm = ge.GraphMachine.inst
if gm is None:
    raise EnvironmentError("GraphMachine wasn't initialized, "
                           "use `ge.GraphMachine(storage_root: str) to start service.`")

ReadOnly = property


class Cell:
    def __init__(self, typ, cell_id=None):
        self._fields = None

        self._cell_id = cell_id

        self._typ: CellType = typ

        self._has_eval = False

        self.cache_index = None

        self.get = py_cell_getter(self)

        self.set = py_cell_setter(self)

    @property
    def compute(self):

        if not self.has_eval:
            if self._fields is None:
                if self._cell_id is None:
                    self.cache_index = gm.new_cell_by_type(self.typ.name)
                else:
                    self.cache_index = gm.new_cell_by_id_type(self.cell_id, self.typ.name)
            else:
                # default value

                self.cache_index = gm.new_cell_by_type_content(self._typ.name,
                                                               json.dumps(self.fields, cls=TSLJSONEncoder))
            self._has_eval = True

            # TODO `get_cell_id_by_cell_idx`, `get_fields_by_cell_idx`
            self._cell_id = get_cell_id_by_cell_idx(self.cache_index)

            self._fields = dict(get_fields_by_cell_idx(self.cache_index))

            self.get = computed_cell_getter(self)
            self.set = computed_cell_setter(self)
            return self
        else:
            # TODO: use caches to eval
            pass

    @ReadOnly
    def cell_id(self):
        if self._cell_id is None and self.has_eval:
            self._cell_id = get_cell_id_by_cell_idx(self.cache_index)
        return self._cell_id

    @ReadOnly
    def typ(self):
        return self._typ

    @ReadOnly
    def has_eval(self):
        return self._has_eval

    def __setitem__(self, key, value):
        """Not recommend to use __setitem__ method because you cannot get whether the setting action succeeds or not.
           Use .set instead
        """
        self.set(key, value)

    def __getitem__(self, key):
        return self.get(key)

    def __str__(self):
        return self.fields.__str__()

    def __repr__(self):
        return self.__str__()

    @property
    def fields(self):
        if self._fields is None:
            self._fields = {field_name: field_type()
                            for field_name, field_type in SymTable.get(self.typ.name).fields}
        return self._fields


def py_cell_getter(cell: Cell):
    def callback(name):
        return cell._attrs[name]

    def callback_first_time(name):
        cell._attrs = {k: typ for k, typ in cell._typ.attrs.items()}
        cell.get = callback
        return cell._attrs[name]

    return callback_first_time


def py_cell_setter(cell: Cell):
    def callback(name, value, strict=False):
        """
        Return False if attr does not exist
        Return True if succeed
        Raise TypeError if type not match.
        """

        typ = cell._typ.attrs.get(name)
        if typ is None:
            return False

        if strict:

            if isinstance(value, typ):
                cell._attrs.__setitem__(name, value)
                return True

            raise TypeError(
                "Invalid type of `value` to set attribute {}. Require {} here"
                    .format(name, typ.__str__()))
        else:
            cell._attrs.__setitem__(name, value)
            return True

    return callback


def computed_cell_getter(cell: Cell):
    """handling with interfaces of ctypes methods
    """
    return cell._body.GetField


def computed_cell_setter(cell: Cell):
    def callback(name, value):
        """
        Return 
                 True if succeed in setting field.
                 False if the field does not exist.
        
        Raise   
                 TypeError if type of the value you set is not suitable.
        """

        typ = cell._typ.attrs.get(name)
        if typ is None:
            return False

        if isinstance(value, cell._typ[name]):
            cell._body.SetField(name, Serializer[typ](value))
            return True

        raise TypeError(
            "Invalid type of `value` to set attribute {}.".format(name))

    update_wrapper(callback, cell._body.SetField)
    return callback
