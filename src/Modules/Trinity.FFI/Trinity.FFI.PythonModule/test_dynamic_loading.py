# -*- coding: utf-8 -*-
"""
Created on Wed Jan 31 23:50:30 2018

@author: twshe
"""

import GraphEngine
import CompositeStorageExtension
import os, clr
import Trinity
from linq import Flow


code_gen_path = os.path.join(GraphEngine.__path__[0], 'Command', 'Trinity.TSL.CodeGen.exe')
CompositeStorageExtension.Cmd.TSLCodeGenExeLocation = code_gen_path
Trinity.TrinityConfig.StorageRoot = os.path.join(GraphEngine.__path__[0], 'ffi')
Trinity.Global.LocalStorage.LoadStorage()
Trinity.Global.Initialize()

if not os.path.exists(r"GraphEngine\ffi\composite-helper\Trinity.Extension.abc.dll"):
    tsl_path = os.path.abspath('./tests/tsl')
    CompositeStorageExtension.Controller.LoadFrom(*[tsl_path, tsl_path, "abc"])

Flow(Trinity.Global.StorageSchema.CellDescriptors).Each(
    lambda cell_desc: print(f'{cell_desc.TypeName}{list(cell_desc.GetFieldNames())}'))

Trinity.Global.LocalStorage.SaveStorage()
