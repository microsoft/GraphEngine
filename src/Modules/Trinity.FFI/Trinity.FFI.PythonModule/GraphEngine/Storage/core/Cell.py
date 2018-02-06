# -*- coding: utf-8 -*-
"""
Created on Sun Jan 28 20:36:19 2018

@author: yatli/thautwarm
"""

import json
from functools import update_wrapper
import GraphEngine as ge
from .SymTable import SymTable, CellType
from .Serialize import Serializer, TSLJSONEncoder

gm: ge.GraphMachine = ge.GraphMachine.inst
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

        self.dated = False  # is the data needed to be updated

        self.cache_index = None

        self.get = py_cell_getter(self)

        self.set = py_cell_setter(self)

    def compute(self):
        if not self.has_eval:
            if self._fields is None:
                if self._cell_id is None:
                    self.cache_index = gm.new_cell_by_type(self.typ.name)
                else:
                    self.cache_index = gm.new_cell_by_id_type(self.cell_id, self.typ.name)
            else:
                # default value

                self.cache_index = gm.new_cell_by_type_content(
                    self._typ.name,
                    json.dumps({field: tsl_value.value
                                for field, tsl_value in self.fields.items()},
                               cls=TSLJSONEncoder))

            self._cell_id = gm.cell_get_id_by_idx(self.cache_index)

            self._has_eval = True
            self._update_fields()

            # TODO: use cache here？
            self.get = computed_cell_getter(self)
            self.set = computed_cell_setter(self)

            return self

    @ReadOnly
    def cell_id(self):
        if self._cell_id is None and self.has_eval:
            self._cell_id = gm.cell_get_fields(self.cache_index, list(self.fields.keys()))
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

    @ReadOnly
    def fields(self):
        if self._fields is None:
            self._fields = {field_name: field_type()
                            for field_name, field_type in SymTable.get(self.typ.name).fields.items()}
        elif self._has_eval and self.dated:
            self._update_fields()
        return self._fields

    def _update_fields(self):
        """
        this method is for the computed cell.
        :return: None
        """
        self._fields = dict(zip(self.fields.keys(),
                                gm.cell_get_fields(self.cache_index, list(self.fields.keys()))))


def py_cell_getter(cell: Cell):
    def callback(name):
        return cell.fields[name]

    return callback


def py_cell_setter(cell: Cell):
    def callback(name, value):
        """
        Return False if attr does not exist
        Return True if succeed
        Raise TypeError if type not match.
        """

        typ = cell.typ.fields.get(name)
        if typ is None:
            return False

        cell.fields[name].set(value)
        return True

    return callback


def computed_cell_getter(cell: Cell):
    """handling with interfaces of ctypes methods
    """

    def callback(field_name):
        gm.cell_get_field(cell.cache_index, field_name)

    return callback


def computed_cell_setter(cell: Cell):
    def callback(name, value):
        """
        Return 
                 True if succeed in setting field.
                 False if the field does not exist.
        
        Raise   
                 TypeError if type of the value you set is not suitable.
        """

        typ = cell.typ.fields.get(name)
        if typ is None:
            return False

        if typ.type_checker(value):
            gm.cell_set_field(cell.cache_index, name, json.dumps(value, cls=TSLJSONEncoder))
            if not cell.dated:
                cell.dated = True
            return True

        raise TypeError(
            "Invalid type of `value` to set attribute {}.".format(name))

    return callback
