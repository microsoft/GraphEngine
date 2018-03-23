import sys, os, json, redis

redis_storage = redis.StrictRedis()

__mypath = os.path.abspath(os.path.dirname(__file__))
os.chdir(os.path.join(__mypath, '..'))
os.rmdir('../build/lib.win-amd64-3.6')
os.system('python ./setup.py build')
os.chdir(__mypath)
sys.path.append('../build/lib.win-amd64-3.6')

import GraphEngine as ge

ffi = ge.__ffi

import Trinity
import Trinity.Storage
import Trinity.Storage.Composite

Trinity.Storage.Composite.CompositeStorage.AddStorageExtension("./tsl3", "BenchMark")
cell_id = 0

new_cell_pynet = Trinity.Global.LocalStorage.NewGenericCell
new_cell_swig = ffi.NewCell_1
new_cell_co = ffi.CO_New_1
new_cell_redis = None  # redis does not have to new an object.

new_cell_id_pynet = Trinity.Global.LocalStorage.NewGenericCell


def new_cell_id_co(cell_id, cell_type):
    return ffi.CO_New_2(cell_type, cell_id)


new_cell_id_swig = ffi.NewCell_2
new_cell_id_redis = None  # redis data does not have an internal id.

new_cell_content_pynet = Trinity.Global.LocalStorage.NewGenericCell
new_cell_content_swig = ffi.NewCell_3


def new_cell_content_co(cell_type, cell_content):
    global cell_id
    return ffi.CO_New_3(cell_type, cell_id, cell_content)


new_cell_content_redis = None  # redis doesn't have this one.

save_cell_pynet = Trinity.Global.LocalStorage.SaveGenericCell
save_cell_swig = ffi.SaveCell_1
save_cell_redis = redis_storage.set

load_cell_pynet = Trinity.Global.LocalStorage.LoadGenericCell
load_cell_swig = ffi.LoadCell
load_cell_redis = redis_storage.get


def load_cell_co(cid):
    c = ffi.CO_Load(cid)
    ffi.CO_Del(c)


def save_cell_co(cid, c):
    ffi.CO_Save_2(cid, c)
    ffi.CO_Del(c)


# load_cell_co = ffi.Co_Load
# save_cell_co = ffi.CO_Save_2

ops = {
    'pynet': (new_cell_pynet, new_cell_id_pynet, new_cell_content_pynet, save_cell_pynet, load_cell_pynet),
    'swig': (new_cell_swig, new_cell_id_swig, new_cell_content_swig, save_cell_swig, load_cell_swig),
    'redis': (new_cell_redis, new_cell_id_redis, new_cell_content_redis, save_cell_redis, load_cell_redis),
    'co': (new_cell_co, new_cell_id_co, new_cell_content_co, save_cell_co, load_cell_co)
}


def test_fn(fn):
    print(sys.argv[0])
    res = fn()
    with open('result.json', 'r') as f:
        stats = json.load(f)

    for k, v in res.items():
        if k in stats:
            stats[k].append(v)
        else:
            stats[k] = [v]

    with open('result.json', 'w') as f:
        json.dump(stats, f, indent=4)
