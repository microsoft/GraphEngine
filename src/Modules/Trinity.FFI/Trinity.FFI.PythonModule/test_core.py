# -*- coding: utf-8 -*-
"""
Created on Wed Feb  7 00:37:40 2018

@author: twshe
"""

import GraphEngine as ge
import redis

# from redislite import Redis
# redis_connection = Redis('cache/redis.db')
redis_server = redis.StrictRedis()
gm = ge.GraphMachine('storage')
gm.start()

from GraphEngine.Storage.core.Cell import Cell
from GraphEngine.Storage.core.SymTable import load_symbols
from GraphEngine.Storage.core.SymTable import SymTable

symtable = SymTable()
load_symbols('tests/tsl/')
print(symtable)

# take the Type `C1` from symtable, and use it to new a cell.
# c1 = Cell(symtable['C1'])
# c1.set('foo', 1)
# c1.compute()
# c1.set('foo', 2)
# print(c1)
#
# c2 = Cell(symtable['C2'])
# c2.set('bar', '123')
# c2.compute()
# print(c2)
#
# c3 = Cell(symtable['C2'])
# c3['lst'] = [1, 2, 3]
# c3.compute()
# print(c3)


spec = {'lst': [1, 2, 3, 4, 5],
        'bar': '123'}


def test_redis():
    redis_server.append('C2', spec)


def test_redis_embedded():
    # Nil for windows...
    pass


def test_ge():
    c = Cell(symtable['C2'])

    for field, value in spec.items():
        c.set(field, value)
    c.compute()
    c.save()


from GraphEngine.Storage.cache_manager import CellAccessorManager

for _ in range(10):
    c = Cell(symtable['C2'])
    for i, (field, value) in enumerate(spec.items()):
        c.set(field, value)

    print(c)
    c.compute()
    c.save()
    print(c)

    cell_id = c.cell_id
    print(cell_id, c.cache_index)
    print()
    ge.GraphMachine.global_cell_manager.save_cell(0)

    # print(ge.GraphMachine.global_cell_manager.load_cell(cell_id))

    with CellAccessorManager() as m:
        c = m.load_cell(cell_id)
        print(c)
        m.delete(c)
        c = m.use_cell(cell_id)
        print(c)
