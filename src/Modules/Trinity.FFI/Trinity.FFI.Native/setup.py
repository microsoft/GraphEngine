"""
setup.py
"""
 
from distutils.core import setup, Extension
 
 
ge_module = Extension('_GraphEngine',
                           sources=['Trinity.FFI.SWIG_wrap.cxx'],
                           include_dirs = ['../Trinity.FFI.Native',
                                           '../../../Trinity.C/include'],
                           libraries = ['trinity_ffi'],
                           library_dirs = ['../lib'],
                           )
 
setup (name = 'GraphEngine',
       version = '0.1',
       author      = "Microsoft Graph Engine Team",
       description = """GraphEngine bindings for Python""",
       ext_modules = [ge_module],
       py_modules = ["graph_engine"],
)