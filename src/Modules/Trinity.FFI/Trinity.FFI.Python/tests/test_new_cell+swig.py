from initialize import *
import timeit
from pprint import pprint
import sys

file_name, cell_type = sys.argv[:2]


@test_fn
def test_new_cell_without_id_by_pynet():
    time = timeit.timeit(f"new_cell_swig('{cell_type}')",
                         number=int(3e6),
                         globals=globals())
    res = {file_name: (cell_type, time)}
    return pprint(res) or res
