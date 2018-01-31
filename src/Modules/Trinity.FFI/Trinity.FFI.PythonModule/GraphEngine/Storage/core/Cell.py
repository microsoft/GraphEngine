# -*- coding: utf-8 -*-
"""
Created on Sun Jan 28 20:36:19 2018

@author: yatli/thautwarm
"""

import GraphEngine.ffi.GraphEngine as _ge
from .Serialize import Serializer, TSLJSONEncoder
from .TypeMap import CellType

import json
from functools import update_wrapper


class Cell:
    def __init__(self, typ: CellType, cell_id=None):

        self._attrs = None

        self._cell_id = cell_id

        self._typ = typ

        self._has_eval = False

        self._body: _ge.Cell = None

        self.get = py_cell_getter(self)

        self.set = py_cell_setter(self)

    @property
    def compute(self):

        if self._attrs is None:
            if self._cell_id is None:
                self._body = _ge.NewCell_1(self._typ.name)
            else:
                self._body = _ge.NewCell_2(self._typ.name, self._cell_id)
        else:
            # default value
            self._attrs = {k: v for k, v in self._attrs.items()}

            self._body = _ge.NewCell_3(self._typ.name,
                                       json.dumps(self._attrs, cls=TSLJSONEncoder))
        self._has_eval = True
        self._cell_id = self._body.ID
        self.get = computed_cell_getter(self)
        self.set = computed_cell_setter(self)
        return self

    @property
    def typ(self):
        return self._typ

    @property
    def cell_id(self):
        return self._cell_id

    @property
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
        return str({name: self._body.GetField(name) for name in self._typ.attrs}
                   if self._has_eval else self._attrs)

    def __repr__(self):
        return self.__str__()


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
