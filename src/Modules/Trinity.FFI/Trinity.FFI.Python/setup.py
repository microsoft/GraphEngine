"""
setup.py
"""
from setuptools import setup, find_packages
from setuptools.extension import Extension


class VERSION:
    major = '0'
    minor = '1'


with open('./README.md', encoding='utf-8') as f:
    readme = f.read()

ge_module = Extension('_ffi',
                      sources=['Trinity.FFI.SWIG_wrap.cxx',
                               '../../GraphEngine.Jit/GraphEngine.Jit.Native/TypeSystem.cpp'],
                      include_dirs=['../Trinity.FFI.Native',
                                    '../../../Trinity.C/include',
                                    '../../GraphEngine.Jit/GraphEngine.Jit.Native'],
                      libraries=['trinity_ffi'],
                      library_dirs=['../../../../bin'],
                      # Uncomment the following lines to produce PDB
                      # extra_compile_args = ['/Zi'],
                      # extra_link_args = ['/DEBUG'],
                      )

setup(name='GraphEngine',
      version='{}.{}'.format(VERSION.major, VERSION.minor),
      keywords='distributed, storage, cloud memory, graph computing',
      description='Trinity Graph Engine interoperatibility',
      long_description=readme,
      license='MIT',
      url='https://github.com/Microsoft/GraphEngine',
      author='Microsoft Graph Engine Team',
      author_email='graph@microsoft.com',
      include_package_data=True,
      install_requires=['linq', 'Redy'],
      packages=['GraphEngine'],
      ext_package='GraphEngine',
      ext_modules=[ge_module],
      # encoding='gbk',
      # entry_points={
      #    'console_scripts': [
      #        'tsl-codegen=GraphEngine.Command:code_gen',
      #        'tsl-metagen=GraphEngine.Command:meta_gen']
      # },
      platforms='any',
      classifiers=[
          'Programming Language :: Python :: 2.7',
          'Programming Language :: Python :: 3.4',
          'Programming Language :: Python :: 3.5',
          'Programming Language :: Python :: 3.6',
          'Programming Language :: Python :: 3.7',
          'Programming Language :: Python :: Implementation :: CPython'],
      zip_safe=False)
