import sys, os, json, redis, shutil

redis_storage = redis.StrictRedis()

__mypath = os.path.abspath(os.path.dirname(__file__))
os.chdir(os.path.join(__mypath, '../../'))
shutil.rmtree('build', ignore_errors=True)
shutil.rmtree(os.path.expanduser('~/.nuget/packages/graphengine.ffi'), ignore_errors=True)
os.system('python ./setup.py build')
os.chdir(__mypath)
sys.path.append('../../build/lib.win-amd64-3.6')

import GraphEngine as ge

ffi = ge.__ffi

import Trinity
import Trinity.Storage
import Trinity.Storage.Composite

