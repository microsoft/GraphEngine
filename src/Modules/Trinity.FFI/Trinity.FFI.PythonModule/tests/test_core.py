# -*- coding: utf-8 -*-
"""
Created on Tue Jan 30 04:17:03 2018

@author: twshe
"""

import unittest

from GraphEngine.Storage.core.CellSymTable import SymTablePicker, sync
from GraphEngine.Storage.core.TSLCell import Cell


class TestMethods(unittest.TestCase):
    def test_new_cell(self):
        sync()
        s = SymTablePicker()
        cell = Cell(s.C1)
        cell.get('foo')
        print(cell)

unittest.main()