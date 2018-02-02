# -*- coding: utf-8 -*-
"""
Created on Wed Jan 31 23:50:30 2018

@author: twshe
"""

import GraphEngine
import CompositeStorageExtension
import os, clr
import Trinity.Storage

code_gen_path = os.path.join(GraphEngine.__path__[0], 'Command', 'Trinity.TSL.CodeGen.exe')
CompositeStorageExtension.Cmd.TSLCodeGenExeLocation = code_gen_path
CompositeStorageExtension.Controller.LoadFrom(*[r'D:\tsl', r'D:\tsl', "abc"]) 
# clr.CellAssembly.StorageSchema.get_CellDescriptors()
"""
LocalStorage didn't not load storage automatically.
"""
