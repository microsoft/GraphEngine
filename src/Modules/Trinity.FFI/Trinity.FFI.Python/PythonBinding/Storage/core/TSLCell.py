# -*- coding: utf-8 -*-
"""
Created on Sun Jan 28 20:36:19 2018

@author: yatli
"""

import _graph_engine as ge

from .CellSymTable import CellType
from .functools import update_wrapper
from .Serialize import Serializer, TSLJSONEncoder
import json

class Cell:
    
    def __init__(self, typ: CellType, cell_id=None):
        
        self._attrs = None
        
        self._cell_id = cell_id
        
        self._typ = typ
        
        self._has_eval = False
        
        self._body: ge.Cell = None
        
        self.item_getter = py_cell_getter(self)

        self.item_setter = py_cell_setter(self) 
    
    @property
    def compute(self):
        
        if self._attrs is None:
            if self._cell_id is None:
                self._body = ge.NewCell(self._typ.name)
            else:
                self._body = ge.NewCell2(self._typ.name, self._cell_id)
        else:
            # default value
            self._attrs = {k: v for k, v in self._attrs.items()}
            
            self._body = ge.NewCell3(self._typ.name, 
                                     json.dumps(self._attrs, cls=TSLJSONEncoder))
        self._has_eval = True
        self._cell_id = self._body.ID
        self.item_getter = computed_cell_getter(self)
        self.item_setter = computed_cell_setter(self)
    
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
        pass
    
    def __getitem__(self, key):
        return self.item_getter(key)




def py_cell_getter(cell: Cell):
    
    def callback(name):
        return cell._attrs[name]
    
    def callback_first_time(name):
        cell._attrs = {k: typ() for k, typ in cell._typ.attrs.items()}
        cell.item_getter = callback
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
    
    
    
        
        
        
            
        
    