from distutils.core import setup, Extension

module1 = Extension('trinity',
                    define_macros = [('MAJOR_VERSION', '1'),
                                     ('MINOR_VERSION', '0')],
                    include_dirs = ['../Trinity.FFI.Native',
                                    '../../../Trinity.C/include'],
                    libraries = ['trinity_ffi'],
                    library_dirs = ['../lib'],
                    sources = ['trinity.cpp'])

setup (name = 'trinity',
       version = '1.0',
       description = 'Trinity Graph Engine interoperatibility',
       author = 'Microsoft Graph Engine Team',
       author_email = 'graph@microsoft.com',
       url = 'https://docs.python.org/extending/building',
       long_description = '''
Trinity Graph Engine interoperatibility
''',
       ext_modules = [module1])
