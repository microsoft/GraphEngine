# -*- coding: utf-8 -*-
"""
Created on Wed Jan 31 23:50:30 2018

@author: twshe
"""

import GraphEngine
import DynamicLoading
import os, sys
sys.path.insert(0, r'E:\GraphEngine\src\Modules\Trinity.FFI\Trinity.FFI.PythonModule\GraphEngine\Command')
code_gen_path = 'Trinity.TSL.Metagen.exe'
DynamicLoading.Center.Init(code_gen_path)
DynamicLoading.Center.LoadFrom(*[r'E:\pl\tsls']*3, 'abc')
