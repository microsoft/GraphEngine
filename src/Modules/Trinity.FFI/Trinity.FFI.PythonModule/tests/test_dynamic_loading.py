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

def test():
	code_gen_path = os.path.join(GraphEngine.__path__[0], 'Command', 'Trinity.TSL.CodeGen.exe')
	CompositeStorageExtension.Cmd.TSLCodeGenExeLocation = code_gen_path
	Trinity.TrinityConfig.StorageRoot = os.path.join(GraphEngine.__path__[0], 'ffi')
	Trinity.Global.LocalStorage.LoadStorage()
	Trinity.Global.Initialize()
	if not os.path.exists(r"E:\GraphEngine\src\Modules\Trinity.FFI\Trinity.FFI.PythonModule\GraphEngine\ffi\composite-helper\Trinity.Extension.abc.dll"):
	    CompositeStorageExtension.Controller.LoadFrom(*[r'E:\tsl', r'E:\tsl', "abc"]) 

	Flow(Trinity.Global.StorageSchema.CellDescriptors).Each(
	    lambda cell_desc: print(f'{cell_desc.TypeName}{list(cell_desc.GetFieldNames())}'))
	Trinity.Global.LocalStorage.SaveStorage()