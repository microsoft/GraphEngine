
from setuptools import setup, find_packages
from setuptools.extension import Extension

ext = Extension('_operations',
                      sources=['operations_wrap.cxx'],
                      include_dirs=[r'C:\Users\twshe\.nuget\packages\graphengine.ffi.metagen\2.0.9328\content\include'],
                      libraries=['trinity_ffi'],
                      library_dirs=[r'C:\Users\twshe\.nuget\packages\graphengine.ffi.metagen\2.0.9328\content\win-x64'])

setup(name='operations',
	ext_modules=[ext],
	version = '0.1',
    author      = "twshe",
	py_modules = ["operations"])
