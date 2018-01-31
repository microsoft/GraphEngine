# -*- coding: utf-8 -*-
"""
Created on Wed Jan 31 23:50:30 2018

@author: twshe
"""

import GraphEngine
import DynamicLoading
import os, clr
code_gen_path = 'Trinity.TSL.CodeGen.exe'
now = os.path.abspath('.')
DynamicLoading.Center.Init(os.path.join(GraphEngine.__path__[0], 'ffi'), code_gen_path)
DynamicLoading.Center.LoadFrom(*[r'E:\pl\ts', r'E:\pl\ts', os.path.join(GraphEngine.__path__[0], 'ffi')], 'abc')
#clr.CellAssembly.StorageSchema.get_CellDescriptors()