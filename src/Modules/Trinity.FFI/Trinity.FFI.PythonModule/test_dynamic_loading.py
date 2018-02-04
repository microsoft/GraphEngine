# -*- coding: utf-8 -*-
"""
Created on Wed Jan 31 23:50:30 2018

@author: twshe
"""

import GraphEngine
import Trinity
import os, clr

from linq import Flow
from Trinity.Storage.Composite import Cmd
from Trinity.Storage.Composite import Controller

code_gen_path = os.path.join(GraphEngine.__path__[0], 'Command', 'Trinity.TSL.CodeGen.exe')
Cmd.TSLCodeGenExeLocation = code_gen_path
Trinity.TrinityConfig.StorageRoot = os.path.join(GraphEngine.__path__[0], 'ffi')
Trinity.Global.LocalStorage.LoadStorage()
Trinity.Global.Initialize()

if not os.path.exists(r"GraphEngine\ffi\composite-helper\Trinity.Extension.abc.dll"):
    tsl_path = os.path.abspath('./tests/tsl')
    Controller.LoadFrom(*[tsl_path, tsl_path, "abc"])

Flow(Trinity.Global.StorageSchema.CellDescriptors).Each(
    lambda cell_desc: print(f'{cell_desc.TypeName}{list(cell_desc.GetFieldNames())}'))

Trinity.Global.LocalStorage.SaveStorage()
