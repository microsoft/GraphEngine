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

symtable = SymTable()
load_symbols('tests/tsl/')
print(symtable)


# take the Type `C1` from symtable, and use it to new a cell.
c1 = Cell(symtable['C1'])
c1.set('foo', 1)
c1.compute()
c1.set('foo', 2)

c2 = Cell(symtable['C2'])
#c2.compute()



