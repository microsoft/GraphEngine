"""
setup.py
"""
from setuptools import setup, find_packages
from setuptools.extension import Extension
from Redy.Tools.Version import Version

version_filename = 'next_version'

with open(version_filename) as f:
    version = Version(f.read().strip())

try:
    with open('./README.rst', encoding='utf-8') as f:
        readme = f.read()
except FileNotFoundError:
    readme = ''

ge_module = Extension('_ffi',
                      sources=['Trinity.FFI.SWIG_wrap.cxx',
                               '../../GraphEngine.Jit/GraphEngine.Jit.Native/TypeSystem.cpp'],
                      include_dirs=['../Trinity.FFI.Native',
                                    '../../../Trinity.C/include',
                                    '../../GraphEngine.Jit/GraphEngine.Jit.Native'],
                      libraries=['trinity_ffi'],
                      library_dirs=['../../../../bin'],
                      # Uncomment the following lines to produce PDB
                      extra_compile_args=['/Zi'],
                      extra_link_args=['/DEBUG'])

setup(name='GraphEngine',
      version=str(version),
      keywords='distributed, storage, cloud memory, graph computing',
      description='Trinity GraphEngine Engine interoperatibility',
      long_description=readme,
      license='MIT',
      url='https://github.com/Microsoft/GraphEngine',
      author='Microsoft GraphEngine Engine Team',
      author_email='graph@microsoft.com',
      include_package_data=True,
      install_requires=['toolz', 'Redy', 'pythonnet'],
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

version.increment(version_number_idx=2, increment=1)
if version[2] is 42:
    version.increment(version_number_idx=1, increment=1)
if version[1] is 42:
    version.increment(version_number_idx=0, increment=1)

with open(version_filename, 'w') as f:
    f.write(str(version))
