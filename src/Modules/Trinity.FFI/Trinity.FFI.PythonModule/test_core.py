# -*- coding: utf-8 -*-
"""
Created on Wed Feb  7 00:37:40 2018

@author: twshe
"""

import GraphEngine as ge
gm = ge.GraphMachine('storage')
gm.start()

from GraphEngine.Storage.core.Cell import Cell
from GraphEngine.Storage.core.SymTable import load_symbols, sync
from GraphEngine.Storage.core.SymTable import SymTable

load_symbols('tests/tsl/')
print(SymTable.inst)


# take the Type `C1` from symtable, and use it to new a cell.
c1 = Cell(SymTable.inst['C1'])
c1.set('foo', 1)
c1.compute()
c1.set('foo', 2)
