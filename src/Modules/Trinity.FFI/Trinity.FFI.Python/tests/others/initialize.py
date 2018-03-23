import sys, os, json, redis

redis_storage = redis.StrictRedis()

__mypath = os.path.abspath(os.path.dirname(__file__))
os.chdir(os.path.join(__mypath, '../../'))
if not os.path.exists('../build/lib.win-amd64-3.6'):
    os.system('python ./setup.py build')
os.chdir(__mypath)
sys.path.append('../../build/lib.win-amd64-3.6')

import GraphEngine as ge

ffi = ge.__ffi

import Trinity
import Trinity.Storage
import Trinity.Storage.Composite

