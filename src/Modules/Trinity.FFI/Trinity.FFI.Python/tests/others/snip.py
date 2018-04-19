# -*- coding: utf-8 -*-
"""
Created on Wed Apr 18 16:35:40 2018

@author: twshe
"""

import initialize
import clr
import os
import GraphEngine as ge
import Trinity
import Trinity.Storage
import Trinity.Storage.Composite
import Trinity.FFI.Metagen
import Trinity.FFI as tf
import getpass
from Redy.Tools.PathLib import Path
from subprocess import call


schema = Trinity.Storage.Composite.CompositeStorage.AddStorageExtension("./tsl4", "TestTslModule")
namespace = 'operations'

swig_code = tf.JitTools.SwigGen(schema, namespace)

nuget_package_path = Path(os.path.expanduser('~/.nuget/packages'))

include = nuget_package_path.into(r'graphengine.ffi.metagen\2.0.9328\content\include')
lib = nuget_package_path.into(r'graphengine.ffi.metagen\2.0.9328\content\win-x64')

filename = f'{namespace}.i'

with open(filename, 'w') as f:
	f.write(swig_code)

setup_code = f"""
from setuptools import setup, find_packages
from setuptools.extension import Extension

ext = Extension('_{namespace}',
                      sources=['{namespace}_wrap.cxx'],
                      include_dirs=[r'{include}'],
                      libraries=['trinity_ffi'],
                      library_dirs=[r'{lib}'])

setup(name='{namespace}',
	ext_modules=[ext],
	version = '0.1',
    author      = "{getpass.getuser()}",
	py_modules = ["{namespace}"])
"""
with open('./setup.py', 'w') as f:
	f.write(setup_code)

os.system(f'swig -modern -c++ -builtin -outcurrentdir -python {filename}')
call(['python', '-c', setup_code, 'build'])