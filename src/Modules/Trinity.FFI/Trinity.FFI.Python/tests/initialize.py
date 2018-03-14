import sys, os, json

__mypath = os.path.abspath(os.path.dirname(__file__))

os.chdir(os.path.join(__mypath, '..'))
os.system('python ./setup.py build')
os.chdir(__mypath)
sys.path.append('../build/lib.win-amd64-3.6')

import GraphEngine as ge

ffi = ge.__ffi

import Trinity
import Trinity.Storage
import Trinity.Storage.Composite

Trinity.Storage.Composite.CompositeStorage.AddStorageExtension("./tsl3", "BenchMark")

new_cell_pynet = Trinity.Global.LocalStorage.NewGenericCell
new_cell_swig = ffi.NewCell_1

new_cell_id_pynet = Trinity.Global.LocalStorage.NewGenericCell
new_cell_id_swig = ffi.NewCell_2

new_cell_content_pynet = Trinity.Global.LocalStorage.NewGenericCell
new_cell_content_swig = ffi.NewCell_3

save_cell_pynet = Trinity.Global.LocalStorage.SaveGenericCell
save_cell_swig = ffi.SaveCell_1

load_cell_pynet = Trinity.Global.LocalStorage.LoadGenericCell
load_cell_swig = ffi.LoadCell

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
