# -*- coding: utf-8 -*-
"""
Created on Wed Jan 31 23:50:30 2018

@author: twshe
"""

import GraphEngine
import DynamicLoading
import os, clr

code_gen_path = os.path.join(GraphEngine.__path__[0], 'Command', 'Trinity.TSL.CodeGen.exe')
now = os.path.abspath('.')
DynamicLoading.Center.Init(os.path.join(GraphEngine.__path__[0], 'ffi'), code_gen_path, 'dotnet.exe', 'TSL2')
DynamicLoading.Center.LoadFrom(*[r'D:\tsl', r'D:\tsl', os.path.join(GraphEngine.__path__[0], 'ffi')], 'abc')

# clr.CellAssembly.StorageSchema.get_CellDescriptors()
