from setuptools import setup
import sys

sys.argv.append('install')


class VERSION:
    major = '0'
    minor = '1'


with open('./README.rst', encoding='utf-8') as f:
    readme = f.read()

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
      packages=['GraphEngine'],
      encoding='gbk',
      entry_points={
          'console_scripts': [
              'tsl-codegen=GraphEngine.Command:code_gen',
              'tsl-metagen=GraphEngine.Command:meta_gen']
      },
      platforms='any',
      classifiers=[
          'Programming Language :: Python :: 2.7',
          'Programming Language :: Python :: 3.4',
          'Programming Language :: Python :: 3.5',
          'Programming Language :: Python :: 3.6',
          'Programming Language :: Python :: 3.7',
          'Programming Language :: Python :: Implementation :: CPython'],
      zip_safe=False)
